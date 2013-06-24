using FalconNet.Common.Encoding;
using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VU_BOOL = System.Boolean;
using VU_BYTE = System.Byte;

namespace FalconNet.VU2
{
    // session event subtypes
    public enum vuEventTypes
    {
        VU_SESSION_UNKNOWN_SUBTYPE = 0,
        VU_SESSION_CLOSE,
        VU_SESSION_JOIN_GAME,
        VU_SESSION_CHANGE_GAME,
        VU_SESSION_JOIN_GROUP,
        VU_SESSION_LEAVE_GROUP,
        VU_SESSION_CHANGE_CALLSIGN,
        VU_SESSION_DISTRIBUTE_ENTITIES,
        VU_SESSION_TIME_SYNC,
        VU_SESSION_LATENCY_NOTICE,
    }

    public class VuMessageFilter
    {
        //VuMessageFilter() { }
        //virtual ~VuMessageFilter() { }
        public abstract VU_BOOL Test(VuMessage evnt);
        public abstract VuMessageFilter Copy();
    }

    /// <summary>
    /// The VuNullMessageFilter lets everything through
    /// </summary>
    public class VuNullMessageFilter : VuMessageFilter
    {
        //VuNullMessageFilter() : VuMessageFilter() { }
        //virtual ~VuNullMessageFilter() { }
        public override VU_BOOL Test(VuMessage evnt)
        {
            return true;
        }

        public override VuMessageFilter Copy()
        {
            return new VuNullMessageFilter();
        }
    }

    /// <summary>
    /// provided default filters
    /// </summary>
    public class VuMessageTypeFilter : VuMessageFilter
    {

        public VuMessageTypeFilter(ulong bitfield)
        {
            msgTypeBitfield_ = bitfield;
        }

        // virtual ~VuMessageTypeFilter();
        public override VU_BOOL Test(VuMessage evnt)
        {
            return (1 << evnt.Type() & msgTypeBitfield_) ? true : false;
        }
        public override VuMessageFilter Copy()
        {
            return new VuMessageTypeFilter(msgTypeBitfield_);
        }

        protected ulong msgTypeBitfield_;
    }

    // the VuStandardMsgFilter allows only these events:
    //   - VuEvent(s) from a remote session
    //   - All Delete and Release events
    //   - All Create, FullUpdate, and Manage events
    // it filters out these messages:
    //   - All update events on unknown entities
    //   - All update events on local entities
    //   - All non-event messages (though this can be overridden)
    public class VuStandardMsgFilter : VuMessageFilter
    {

        public VuStandardMsgFilter(ulong bitfield = VU_VU_EVENT_BITS)
        {
            msgTypeBitfield_ = bitfield;
        }

        //virtual ~VuStandardMsgFilter();
        public override VU_BOOL Test(VuMessage message)
        {
            ulong eventBit = 1 << message.Type();
            if ((eventBit & msgTypeBitfield_) == 0)
            {
                return false;
            }
            if (eventBit & (VU_DELETE_EVENT_BITS | VU_CREATE_EVENT_BITS))
            {
                return true;
            }
            if (message.Sender() == vuLocalSession)
            {
                return false;
            }
            // test to see if entity was found in database
            if (message.Entity() != 0)
            {
                return true;
            }
            return false;
        }
        public override VuMessageFilter Copy()
        {
            return new VuStandardMsgFilter(msgTypeBitfield_);
        }

        protected ulong msgTypeBitfield_;
    }

    public delegate VU_BOOL EvalFunc(VuMessage msg, Object arg);

    public class VuMessageQueue
    {
        public VuMessageQueue(int queueSize, VuMessageFilter filter = null) { throw new NotImplementedException(); }
        //TODO public  ~VuMessageQueue();

        public VuMessage PeekVuMessage() { throw new NotImplementedException(); }
        public virtual VuMessage DispatchVuMessage(VU_BOOL autod = false) { throw new NotImplementedException(); }
        public int DispatchAllMessages(VU_BOOL autod = false) { throw new NotImplementedException(); }
        public static int PostVuMessage(VuMessage msg) { throw new NotImplementedException(); }
        public static void FlushAllQueues() { throw new NotImplementedException(); }
        public static int InvalidateMessages(EvalFunc evalFunc, Object arg) { throw new NotImplementedException(); }
        public int InvalidateQueueMessages(EvalFunc evalFunc, Object arg) { throw new NotImplementedException(); }
    }

    public class VuMainMessageQueue : VuMessageQueue
    {
        public VuMainMessageQueue(int queueSize, VuMessageFilter filter = null)
            : base(queueSize, filter)
        { throw new NotImplementedException(); }
        //~VuMainMessageQueue();

        public override VuMessage DispatchVuMessage(VU_BOOL autod = false) { throw new NotImplementedException(); }
    }

    public class VuPendingSendQueue : VuMessageQueue
    {
        public VuPendingSendQueue(int queueSize);
        //TODO ~VuPendingSendQueue();

        public override VuMessage DispatchVuMessage(VU_BOOL autod = false) { throw new NotImplementedException(); }
        public void RemoveTarget(VuTargetEntity target) { throw new NotImplementedException(); }

        public int BytesPending() { return bytesPending_; }

        //protected:
        //TODO  virtual int AddMessage(VuMessage *event); // called only by PostVuMessage()

        // DATA
        protected int bytesPending_;
    }

    public class VuMessage
    {

#if USE_SH_POOLS
public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(gVuMsgMemPool,size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        //TODO public virtual ~VuMessage();

        public VU_MSG_TYPE Type() { return type_; }
        public VU_ID Sender() { return sender_; }
        public VU_ID Destination() { return tgtid_; }
        public VU_BOOL IsLocal()
        {
            return sender_.creator_ == vuLocalSession.creator_ ?
                                true : false;
        }
        public VU_ID EntityId() { return entityId_; }
        public VU_BYTE Flags() { return flags_; }
        public VuEntity Entity() { return ent_; }
        public VU_TIME PostTime() { return postTime_; }
        public VuTargetEntity Target() { return target_; }

        public void SetPostTime(VU_TIME posttime) { postTime_ = posttime; }
        public virtual int Size();


#if TODO
        public virtual int Read(ByteWrapper  buf);
  public virtual int Write(ByteWrapper  buf);

        public VU_ERRCODE Dispatch(VU_BOOL autod);
  public int Send();


 public  void RequestLoopback() { flags_ |= VU_LOOPBACK_MSG_FLAG; }
  public void RequestReliableTransmit() { flags_ |= VU_RELIABLE_MSG_FLAG; }
  public void RequestOutOfBandTransmit() { flags_ |= VU_OUT_OF_BAND_MSG_FLAG; }
  public void RequestLowPriorityTransmit() { flags_ &= 0xf0; }

// app needs to Ref & UnRef messages they keep around
// 	most often this need not be done
  public int Ref();
  public int UnRef();

  // the following determines just prior to sending message whether or not
  // it goes out on the wire (default is TRUE, of course)
  public virtual VU_BOOL DoSend();


  protected VuMessage(VU_MSG_TYPE type, VU_ID entityId, VuTargetEntity *target,
                VU_BOOL loopback);
  protected VuMessage(VU_MSG_TYPE type, VU_ID sender, VU_ID target);
  protected virtual VU_ERRCODE Activate(VuEntity *ent);
  protected abstract VU_ERRCODE Process(VU_BOOL autod)
#endif
        protected virtual int Encode(ByteWrapper buf)
        {
            int size = entityId_.creator_.Encode(buf);
            size += entityId_.num_.Encode(buf);
            return size;
        }

        protected virtual void Decode(ByteWrapper buf)
        {
            entityId_.creator_ = buf.DecodeULong();
            entityId_.num_ = buf.DecodeULong();
        }
        protected VuEntity SetEntity(VuEntity ent);

        private VU_BYTE refcnt_;		// vu references


        protected VU_MSG_TYPE type_;
        protected VU_BYTE flags_;		// misc flags
        protected VU_ID sender_;
        protected VU_ID tgtid_;
        protected VU_ID entityId_;
        // scratch variables (not networked)
        protected VuTargetEntity target_;
        protected VU_TIME postTime_;

        private VuEntity ent_;
    }






    public class VuErrorMessage : VuMessage {
 
  public VuErrorMessage(int errorType, VU_ID senderid, VU_ID entityid, VuTargetEntity *target);
  public VuErrorMessage(VU_ID senderid, VU_ID targetid);
  //TODO public virtual ~VuErrorMessage();

  public virtual int Size();
  public override void Decode(ByteWrapper buf);
  public override int Encode(ByteWrapper buf)
  {
       base.Encode(buf);
      srcmsgid_.
  memcpy(*buf, &srcmsgid_, sizeof(srcmsgid_)); *buf += sizeof(srcmsgid_);
  memcpy(*buf, &etype_,    sizeof(etype_));    *buf += sizeof(etype_);

  }
  public short ErrorType() { return etype_; }
 
  protected virtual VU_ERRCODE Process(VU_BOOL autod);

private  int LocalSize();
 
  protected VU_ID srcmsgid_;
  protected short etype_;
} 
#if TODO
//--------------------------------------------------
public class VuRequestMessage :   VuMessage {
 
  //TODO public virtual ~VuRequestMessage();

  protected VuRequestMessage(VU_MSG_TYPE type, VU_ID entityid, VuTargetEntity *target);
  protected VuRequestMessage(VU_MSG_TYPE type, VU_ID senderid, VU_ID entityid);
  protected abstract VU_ERRCODE Process(VU_BOOL autod);

 
} 

//--------------------------------------------------
enum VU_SPECIAL_GET_TYPE {
  VU_GET_GAME_ENTS,
  VU_GET_GLOBAL_ENTS
} 

public class VuGetRequest :   VuRequestMessage {
 
  public VuGetRequest(VU_SPECIAL_GET_TYPE gettype, VuSessionEntity *sess = 0);
  public VuGetRequest(VU_ID entityId, VuTargetEntity *target);
  public VuGetRequest(VU_ID senderid, VU_ID target);
  //TODO public virtual ~VuGetRequest();
 
  protected virtual VU_ERRCODE Process(VU_BOOL autod);
} 

//--------------------------------------------------
public class VuPushRequest :   VuRequestMessage {
 
  public VuPushRequest(VU_ID entityId, VuTargetEntity *target);
  public VuPushRequest(VU_ID senderid, VU_ID target);
  //TODO public virtual ~VuPushRequest();

 
  protected virtual VU_ERRCODE Process(VU_BOOL autod);
} 

//--------------------------------------------------
public class VuPullRequest :   VuRequestMessage {
 
  public VuPullRequest(VU_ID entityId, VuTargetEntity *target);
  public VuPullRequest(VU_ID senderid, VU_ID target);
  //TODO public virtual ~VuPullRequest();

  protected virtual VU_ERRCODE Process(VU_BOOL autod);
}

//--------------------------------------------------
public class VuEvent :   VuMessage {
 
  //TODO public virtual ~VuEvent();

  public virtual int Size();
  public virtual int Decode(VU_BYTE **buf, int length);
  public virtual int Encode(VU_BYTE **buf);
 
  protected VuEvent(VU_MSG_TYPE type, VU_ID entityId, VuTargetEntity *target,
                VU_BOOL loopback=FALSE);
  protected VuEvent(VU_MSG_TYPE type, VU_ID senderid, VU_ID target);
  protected virtual VU_ERRCODE Activate(VuEntity *ent);
  protected abstract VU_ERRCODE Process(VU_BOOL autod);

private  int LocalSize();

// DATA
 
  // these fields are filled in on Activate()
  public VU_TIME updateTime_; 
};

//--------------------------------------------------
public class VuCreateEvent :   VuEvent {
 
  public VuCreateEvent(VuEntity *entity, VuTargetEntity *target, VU_BOOL loopback=FALSE);
  public VuCreateEvent(VU_ID senderid, VU_ID target);
  //TODO public virtual ~VuCreateEvent();

  public virtual int Size();
  public virtual int Decode(VU_BYTE **buf, int length);
  public virtual int Encode(VU_BYTE **buf);
  public virtual VU_BOOL DoSend();     // returns TRUE if ent is in database

  public VuEntity *EventData() { return expandedData_; }
 
  protected VuCreateEvent(VU_MSG_TYPE type, VuEntity *entity, VuTargetEntity *target,
                        VU_BOOL loopback=FALSE);
  protected VuCreateEvent(VU_MSG_TYPE type, VU_ID senderid, VU_ID target);

  protected virtual VU_ERRCODE Activate(VuEntity *ent);
  protected virtual VU_ERRCODE Process(VU_BOOL autod);

private  int LocalSize();

// data
 
  public VuEntity *expandedData_;
#if  VU_USE_CLASS_INFO
  VU_BYTE classInfo_[CLASS_NUM_BYTES];	// entity class type
#endif
  public ushort vutype_;			// entity type
  public ushort size_;
  public VU_BYTE *data_;
} 

//--------------------------------------------------
public class VuManageEvent :   VuCreateEvent {
  
  public VuManageEvent(VuEntity *entity, VuTargetEntity *target, VU_BOOL loopback=FALSE);
  public VuManageEvent(VU_ID senderid, VU_ID target);
  //TODO public virtual ~VuManageEvent();
 
} 

//--------------------------------------------------
public class VuDeleteEvent :   VuEvent {
 
  public VuDeleteEvent(VuEntity *entity);
  public VuDeleteEvent(VU_ID senderid, VU_ID target);
  //TODO public virtual ~VuDeleteEvent();

  protected virtual VU_ERRCODE Process(VU_BOOL autod);
  protected virtual VU_ERRCODE Activate(VuEntity *ent);
 
} 

//--------------------------------------------------
public class VuUnmanageEvent :   VuEvent {
 
  public VuUnmanageEvent(VuEntity *entity, VuTargetEntity *target,
                        VU_TIME mark, VU_BOOL loopback = FALSE);
  public VuUnmanageEvent(VU_ID senderid, VU_ID target);
  //TODO public virtual ~VuUnmanageEvent();

  public virtual int Size();
  public virtual int Decode(VU_BYTE **buf, int length);
  public virtual int Encode(VU_BYTE **buf);

  protected virtual VU_ERRCODE Process(VU_BOOL autod);

private  int LocalSize();

// data
 
  // time of removal
  public VU_TIME mark_;
} 

//--------------------------------------------------
public class VuReleaseEvent :   VuEvent {
 
  public VuReleaseEvent(VuEntity *entity);
  //TODO public virtual ~VuReleaseEvent();

  public virtual int Size();
  // all these are stubbed out here, as this is not a net message
  public virtual int Decode(VU_BYTE **buf, int length);
  public virtual int Encode(VU_BYTE **buf);
  public virtual VU_BOOL DoSend();     // returns FALSE
 
  protected virtual VU_ERRCODE Activate(VuEntity *ent);
  protected virtual VU_ERRCODE Process(VU_BOOL autod);
 
} 

//--------------------------------------------------
public class VuTransferEvent :   VuEvent {
 
  public VuTransferEvent(VuEntity *entity, VuTargetEntity *target, VU_BOOL loopback=FALSE);
  public VuTransferEvent(VU_ID senderid, VU_ID target);
  public virtual ~VuTransferEvent();

  public virtual int Size();
  public virtual int Decode(VU_BYTE **buf, int length);
  public virtual int Encode(VU_BYTE **buf);
 
  protected virtual VU_ERRCODE Activate(VuEntity *ent);
  protected virtual VU_ERRCODE Process(VU_BOOL autod);

private  int LocalSize();

// data
  public VU_ID newOwnerId_;
} 

//--------------------------------------------------
public class VuPositionUpdateEvent :   VuEvent {
 
  public VuPositionUpdateEvent(VuEntity *entity, VuTargetEntity *target,
                        VU_BOOL loopback=FALSE);
  public VuPositionUpdateEvent(VU_ID senderid, VU_ID target);
  //TODO public virtual ~VuPositionUpdateEvent();

  public virtual int Size();
  public virtual int Decode(VU_BYTE **buf, int length);
  public virtual int Encode(VU_BYTE **buf);

  public virtual VU_BOOL DoSend();
 
  protected virtual VU_ERRCODE Process(VU_BOOL autod);

  private int LocalSize();

// data
 
#if  VU_USE_QUATERNION
  VU_QUAT quat_;	// quaternion indicating current facing
  VU_VECT dquat_;	// unit vector expressing quaternion delta
  SM_SCALAR theta_;	// scalar indicating rate of above delta
#else // !VU_USE_QUATERNION
  public SM_SCALAR yaw_, pitch_, roll_;
  public SM_SCALAR dyaw_, dpitch_, droll_;
#endif
  public BIG_SCALAR x_, y_, z_;
  public SM_SCALAR dx_, dy_, dz_;
} 

//--------------------------------------------------
public class VuBroadcastGlobalEvent :   VuEvent {
 
  public VuBroadcastGlobalEvent(VuEntity *entity, VuTargetEntity *target, VU_BOOL loopback=FALSE);
  public VuBroadcastGlobalEvent(VU_ID senderid, VU_ID target);
  //TODO public virtual ~VuBroadcastGlobalEvent();

  public void MarkAsKeepalive() { flags_ |= VU_KEEPALIVE_MSG_FLAG; }
 
  protected virtual int Size();
  protected virtual int Decode(VU_BYTE **buf, int length);
  protected virtual int Encode(VU_BYTE **buf);

  protected virtual VU_ERRCODE Activate(VuEntity *ent);
  protected virtual VU_ERRCODE Process(VU_BOOL autod);

// data
 
#if  VU_USE_CLASS_INFO
  VU_BYTE classInfo_[CLASS_NUM_BYTES];	// entity class type
#endif
  protected ushort vutype_;			// entity type

} 

//--------------------------------------------------
public class VuFullUpdateEvent :   VuCreateEvent {
 
  public VuFullUpdateEvent(VuEntity *entity, VuTargetEntity *target, VU_BOOL loopback=FALSE);
  public VuFullUpdateEvent(VU_ID senderid, VU_ID target);
  //TODO public virtual ~VuFullUpdateEvent();

  public VU_BOOL WasCreated() { return (VU_BOOL)(Entity() == expandedData_ ? TRUE : FALSE); }
  public void MarkAsKeepalive() { flags_ |= VU_KEEPALIVE_MSG_FLAG; }

 
  protected virtual VU_ERRCODE Activate(VuEntity *ent);
 
};

//--------------------------------------------------
public class VuEntityCollisionEvent :   VuEvent {
 
 public  VuEntityCollisionEvent(VuEntity *entity, VU_ID otherId,
                         VU_DAMAGE hitLocation, int hitEffect,
                         VuTargetEntity *target, VU_BOOL loopback=FALSE);
  public VuEntityCollisionEvent(VU_ID senderid, VU_ID target);

  //TODO public virtual ~VuEntityCollisionEvent();

  public virtual int Size();
  public virtual int Decode(VU_BYTE **buf, int length);
  public virtual int Encode(VU_BYTE **buf);

 
  protected virtual VU_ERRCODE Process(VU_BOOL autod);

  private int LocalSize();

// data
  public VU_ID otherId_;
  public VU_DAMAGE hitLocation_;	// affects damage
  public int hitEffect_;		// affects hitpoints/health
} 

//--------------------------------------------------
public class VuGroundCollisionEvent :   VuEvent {
 
  public VuGroundCollisionEvent(VuEntity *entity, VuTargetEntity *target, VU_BOOL loopback=FALSE);
  public VuGroundCollisionEvent(VU_ID senderid, VU_ID target);
  //TODO public virtual ~VuGroundCollisionEvent();

 
  protected virtual VU_ERRCODE Process(VU_BOOL autod);
 
} 

//--------------------------------------------------
public class VuSessionEvent :   VuEvent {
 
  public VuSessionEvent(VuEntity *entity, ushort subtype, VuTargetEntity *target,
                 VU_BOOL loopback=FALSE);
  public VuSessionEvent(VU_ID senderid, VU_ID target);
  //TODO public virtual ~VuSessionEvent();

  public virtual int Size();
  public virtual int Decode(VU_BYTE **buf, int length);
  public virtual int Encode(VU_BYTE **buf);

 
  protected virtual VU_ERRCODE Process(VU_BOOL autod);

  private int LocalSize();

// data
 
  public ushort subtype_;
  public VU_ID group_;
  public char *callsign_;
  public VU_BYTE syncState_;
  public VU_TIME gameTime_;
} 

//--------------------------------------------------
public class VuTimerEvent :   VuEvent {
 
  public VuTimerEvent(VuEntity *entity, VU_TIME mark, ushort type, VuMessage *event=0);
  //TODO public virtual ~VuTimerEvent();

  public virtual int Size();
  // all these are stubbed out here, as this is not a net message
  public virtual int Decode(VU_BYTE **buf, int length);
  public virtual int Encode(VU_BYTE **buf);
  public virtual VU_BOOL DoSend();     // returns FALSE

 
  protected virtual VU_ERRCODE Process(VU_BOOL autod);

// data
 
  // time of firing
  public VU_TIME mark_;
  public ushort timertype_;
  // event to launch on firing
  public VuMessage *event_;


  private VuTimerEvent *next_;
} 

//--------------------------------------------------
public class VuShutdownEvent :   VuEvent {
 
  public VuShutdownEvent(VU_BOOL all = FALSE);
  //TODO public virtual ~VuShutdownEvent();

  public virtual int Size();
  // all these are stubbed out here, as this is not a net message
  public virtual int Decode(VU_BYTE **buf, int length);
  public virtual int Encode(VU_BYTE **buf);
 public  virtual VU_BOOL DoSend();     // returns FALSE

 
  protected virtual VU_ERRCODE Process(VU_BOOL autod);

// data

  public VU_BOOL shutdownAll_;
  public VU_BOOL done_;
} 

#if VU_SIMPLE_LATENCY
//--------------------------------------------------
class VuTimingMessage : public VuMessage {
public:
  VuTimingMessage(VU_ID entityId, VuTargetEntity *target, VU_BOOL loopback=FALSE);
  VuTimingMessage(VU_ID senderid, VU_ID target);
  virtual ~VuTimingMessage();

  virtual int Size();
  virtual int Decode(VU_BYTE **buf, int length);
  virtual int Encode(VU_BYTE **buf);

protected:
  virtual VU_ERRCODE Process(VU_BOOL autod);

// data
public:
	VU_TIME	sessionRealSendTime_;
	VU_TIME sessionGameSendTime_;
	VU_TIME remoteGameTime_;
};
#endif

//--------------------------------------------------
public class VuUnknownMessage : VuMessage {
 
  public VuUnknownMessage();
  //TODO public virtual ~VuUnknownMessage();

  public virtual int Size();
  // all these are stubbed out here, as this is not a net message
  public virtual int Decode(VU_BYTE **buf, int length);
  public virtual int Encode(VU_BYTE **buf);
  public virtual VU_BOOL DoSend();     // returns FALSE


  protected virtual VU_ERRCODE Process(VU_BOOL autod);
 
}
#endif
}