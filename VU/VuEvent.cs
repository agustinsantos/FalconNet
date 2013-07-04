using FalconNet.Common.Encoding;
using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VU_BOOL = System.Boolean;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;
using SM_SCALAR = System.Single;
using BIG_SCALAR = System.Single;
using VU_DAMAGE = System.UInt64;

namespace FalconNet.VU
{
    [Flags]
    public enum VU_MSG_TYPE
    {
        VU_UNKNOWN_MESSAGE = 0,

        // error message
        VU_ERROR_MESSAGE = 1,		// 0x00000002

        // request messages
        VU_GET_REQUEST_MESSAGE = 2,		// 0x00000004
        VU_PUSH_REQUEST_MESSAGE = 3,		// 0x00000008
        VU_PULL_REQUEST_MESSAGE = 4,		// 0x00000010

        // internal events
        VU_TIMER_EVENT = 5,		// 0x00000020
        VU_RELEASE_EVENT = 6,		// 0x00000040

        // event messages
        VU_DELETE_EVENT = 7,		// 0x00000080
        VU_UNMANAGE_EVENT = 8,		// 0x00000100
        VU_MANAGE_EVENT = 9,		// 0x00000200
        VU_CREATE_EVENT = 10,		// 0x00000400
        VU_SESSION_EVENT = 11,		// 0x00000800
        VU_TRANSFER_EVENT = 12,		// 0x00001000
        VU_BROADCAST_GLOBAL_EVENT = 13,		// 0x00002000
        VU_POSITION_UPDATE_EVENT = 14,		// 0x00004000
        VU_FULL_UPDATE_EVENT = 15,		// 0x00008000
        VU_RESERVED_UPDATE_EVENT = 16,		// 0x00010000 ***
        VU_ENTITY_COLLISION_EVENT = 17,		// 0x00020000 ***
        VU_GROUND_COLLISION_EVENT = 18,		// 0x00040000 ***

        // shutdown event
        VU_SHUTDOWN_EVENT = 19,		// 0x00080000

        // latency/timing message
        VU_TIMING_MESSAGE = 20,		// 0x00100000
    }

    public enum VUBITS : uint
    {
        VU_VU_MESSAGE_BITS = 0x001ffffe,
        VU_REQUEST_MSG_BITS = 0x0000001c,
        VU_VU_EVENT_BITS = 0x000fffe0,
        VU_DELETE_EVENT_BITS = 0x000800c0,
        VU_CREATE_EVENT_BITS = 0x00000600,
        VU_TIMER_EVENT_BITS = 0x00000020,
        VU_INTERNAL_EVENT_BITS = 0x00080060,
        VU_EXTERNAL_EVENT_BITS = 0x0017ff82,
        VU_USER_MESSAGE_BITS = 0xffe00000
    }

    public enum VU_MSG_FLAG
    {
        // message flags
        VU_NORMAL_PRIORITY_MSG_FLAG = 0x01,	// send normal priority
        VU_OUT_OF_BAND_MSG_FLAG = 0x02,	// send unbuffered
        VU_KEEPALIVE_MSG_FLAG = 0x04,	// this is a keepalive msg
        VU_RELIABLE_MSG_FLAG = 0x08,	// attempt to send reliably
        VU_LOOPBACK_MSG_FLAG = 0x10,	// post msg to self as well
        VU_REMOTE_MSG_FLAG = 0x20,	// msg came from outside
        VU_SEND_FAILED_MSG_FLAG = 0x40,	// msg has been sent
        VU_PROCESSED_MSG_FLAG = 0x80	// msg has been processed
    }

    public enum VUERROR
    {
        VU_UNKNOWN_ERROR = 0,
        VU_NO_SUCH_ENTITY_ERROR = 1,
        VU_CANT_MANAGE_ENTITY_ERROR = 2,	// for push request denial
        VU_DONT_MANAGE_ENTITY_ERROR = 3,	// for pull request denial
        VU_CANT_TRANSFER_ENTITY_ERROR = 4,	// for non-transferrable ents
        VU_TRANSFER_ASSOCIATION_ERROR = 5,	// for association errors
        VU_NOT_AVAILABLE_ERROR = 6	// session too busy or exiting
    }

    // timer types
    public enum TIMERTYPES
    {
        VU_UNKNOWN_TIMER = 0,
        VU_DELETE_TIMER = 1,
        VU_DELAY_TIMER = 2
    }

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

    public abstract class VuMessageFilter
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
            return ((1 << (int)evnt.Type()) != 0 && msgTypeBitfield_ != 0) ? true : false;
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

        public VuStandardMsgFilter(VUBITS bitfield = VUBITS.VU_VU_EVENT_BITS)
        {
            msgTypeBitfield_ = bitfield;
        }

        //virtual ~VuStandardMsgFilter();
        public override VU_BOOL Test(VuMessage message)
        {
            VUBITS eventBit = (VUBITS)(1 << (int)message.Type());
            if ((eventBit & msgTypeBitfield_) == 0)
            {
                return false;
            }
            if ((eventBit & (VUBITS.VU_DELETE_EVENT_BITS | VUBITS.VU_CREATE_EVENT_BITS)) != 0)
            {
                return true;
            }
            if (message.Sender() == VUSTATIC.vuLocalSession)
            {
                return false;
            }
            // test to see if entity was found in database
            if (message.Entity() != null)
            {
                return true;
            }
            return false;
        }
        public override VuMessageFilter Copy()
        {
            return new VuStandardMsgFilter(msgTypeBitfield_);
        }

        protected VUBITS msgTypeBitfield_;
    }

    public delegate VU_BOOL EvalFunc(VuMessage msg, Object arg);

    public class VuMessageQueue
    {
        public VuMessageQueue(int queueSize, VuMessageFilter filter = null)
        { throw new NotImplementedException(); }

        //TODO public  ~VuMessageQueue();

        public VuMessage PeekVuMessage() { throw new NotImplementedException(); }
        public virtual VuMessage DispatchVuMessage(VU_BOOL autod = false)
        { throw new NotImplementedException(); }

        public int DispatchAllMessages(VU_BOOL autod = false)
        { throw new NotImplementedException(); }

        public static int PostVuMessage(VuMessage msg) { throw new NotImplementedException(); }
        public static void FlushAllQueues() { throw new NotImplementedException(); }
        public static int InvalidateMessages(EvalFunc evalFunc, Object arg) { throw new NotImplementedException(); }
        public int InvalidateQueueMessages(EvalFunc evalFunc, Object arg) { throw new NotImplementedException(); }


        // called when queue is about to wrap -- default does nothing & returns false
        protected virtual VU_BOOL ReallocQueue() { throw new NotImplementedException(); }

        protected virtual int AddMessage(VuMessage evnt) // called only by PostVuMessage()
        { throw new NotImplementedException(); }
        protected static void RepostMessage(VuMessage evnt, int delay)
        { throw new NotImplementedException(); }

        // DATA

        protected VuMessage[] head_;	// also queue mem store
        protected VuMessage[] read_;
        protected VuMessage[] write_;
        protected VuMessage[] tail_;

        protected VuMessageFilter filter_;
#if _DEBUG
  int _count; // JPO - see what occupancy is like
#endif


        private static VuMessageQueue queuecollhead_;
        private VuMessageQueue nextqueue_;

    }

    public class VuMainMessageQueue : VuMessageQueue
    {
        public VuMainMessageQueue(int queueSize, VuMessageFilter filter = null)
            : base(queueSize, filter)
        {
            timerlisthead_ = null;
        }
        //~VuMainMessageQueue();

        public override VuMessage DispatchVuMessage(VU_BOOL autod = false) { throw new NotImplementedException(); }
        protected virtual int AddMessage(VuMessage evnt) // called only by PostVuMessage()
        { throw new NotImplementedException(); }

        // DATA
        protected VuTimerEvent timerlisthead_;
    }

    public class VuPendingSendQueue : VuMessageQueue
    {
        private static VuResendMsgFilter resendMsgFilter = new VuResendMsgFilter();

        public VuPendingSendQueue(int queueSize) : base(queueSize, resendMsgFilter) { throw new NotImplementedException(); }
        //TODO ~VuPendingSendQueue();

        public override VuMessage DispatchVuMessage(VU_BOOL autod = false) { throw new NotImplementedException(); }
        public void RemoveTarget(VuTargetEntity target) { throw new NotImplementedException(); }

        public int BytesPending() { return bytesPending_; }

        protected override int AddMessage(VuMessage evnt) // called only by PostVuMessage()
        { throw new NotImplementedException(); }

        // DATA
        protected int bytesPending_;
    }

    public abstract class VuMessage
    {

#if USE_SH_POOLS
public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(gVuMsgMemPool,size,false);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        //TODO public virtual ~VuMessage();

        public VU_MSG_TYPE Type() { return type_; }
        public VU_ID Sender() { return sender_; }
        public VU_ID Destination() { return tgtid_; }
        public VU_BOOL IsLocal()
        {
            return sender_.creator_ == VUSTATIC.vuLocalSession.creator_ ?
                                true : false;
        }
        public VU_ID EntityId() { return entityId_; }
        public VU_MSG_FLAG Flags() { return flags_; }
        public VuEntity Entity() { return ent_; }
        public VU_TIME PostTime() { return postTime_; }
        public VuTargetEntity Target() { return target_; }

        public void SetPostTime(VU_TIME posttime) { postTime_ = posttime; }
        public virtual int Size() { throw new NotImplementedException(); }


        public virtual int Read(ByteWrapper buf) { throw new NotImplementedException(); }
        public virtual int Write(ByteWrapper buf) { throw new NotImplementedException(); }

        public VU_ERRCODE Dispatch(VU_BOOL autod) { throw new NotImplementedException(); }
        public VU_ERRCODE Send() { throw new NotImplementedException(); }


        public void RequestLoopback() { flags_ |= VU_MSG_FLAG.VU_LOOPBACK_MSG_FLAG; }
        public void RequestReliableTransmit() { flags_ |= VU_MSG_FLAG.VU_RELIABLE_MSG_FLAG; }
        public void RequestOutOfBandTransmit() { flags_ |= VU_MSG_FLAG.VU_OUT_OF_BAND_MSG_FLAG; }
        public void RequestLowPriorityTransmit() { flags_ &= (VU_MSG_FLAG)0xf0; }

        // app needs to Ref & UnRef messages they keep around
        // 	most often this need not be done
        public int Ref() { throw new NotImplementedException(); }
        public int UnRef() { throw new NotImplementedException(); }

        // the following determines just prior to sending message whether or not
        // it goes out on the wire (default is true, of course)
        public virtual VU_BOOL DoSend() { throw new NotImplementedException(); }


        protected VuMessage(VU_MSG_TYPE type, VU_ID entityId, VuTargetEntity target,
                      VU_BOOL loopback) { throw new NotImplementedException(); }
        protected VuMessage(VU_MSG_TYPE type, VU_ID sender, VU_ID target) { throw new NotImplementedException(); }
        protected virtual VU_ERRCODE Activate(VuEntity ent) { throw new NotImplementedException(); }
        protected abstract VU_ERRCODE Process(VU_BOOL autod);

        public virtual int Encode(ByteWrapper buf)
        {
            int size = entityId_.creator_.Encode(buf);
            size += entityId_.num_.Encode(buf);
            return size;
        }

        public virtual int Decode(ByteWrapper buf)
        {
            entityId_.creator_ = buf.DecodeULong();
            entityId_.num_ = buf.DecodeULong();
            return sizeof(ulong) + sizeof(ulong);
        }

        protected VuEntity SetEntity(VuEntity ent) { throw new NotImplementedException(); }

        private VU_BYTE refcnt_;		// vu references


        protected VU_MSG_TYPE type_;
        protected VU_MSG_FLAG flags_;		// misc flags
        protected VU_ID sender_;
        protected VU_ID tgtid_;
        protected VU_ID entityId_;
        // scratch variables (not networked)
        protected VuTargetEntity target_;
        protected VU_TIME postTime_;

        private VuEntity ent_;
    }

    public class VuErrorMessage : VuMessage
    {

        public VuErrorMessage(VUERROR errorType, VU_ID srcmsgid, VU_ID entityid, VuTargetEntity target)
            : base(VU_MSG_TYPE.VU_ERROR_MESSAGE, entityid, target, false)
        {
            srcmsgid_ = srcmsgid;
            etype_ = errorType;
        }
        public VuErrorMessage(VU_ID senderid, VU_ID targetid)
            : base(VU_MSG_TYPE.VU_ERROR_MESSAGE, senderid, targetid)
        {
            etype_ = VUERROR.VU_UNKNOWN_ERROR;
            srcmsgid_.num_ = 0;
            srcmsgid_.creator_ = 0;
        }
        //TODO public virtual ~VuErrorMessage();

        public override int Size() { throw new NotImplementedException(); }
        public override int Decode(ByteWrapper buf) { throw new NotImplementedException(); }
        public override int Encode(ByteWrapper buf)
        {
#if TODO 
            base.Encode(buf);
            srcmsgid_.
        memcpy(*buf, &srcmsgid_, sizeof(srcmsgid_)); *buf += sizeof(srcmsgid_);
            memcpy(*buf, &etype_, sizeof(etype_)); *buf += sizeof(etype_);

#endif
            throw new NotImplementedException();

        }
        public VUERROR ErrorType() { return etype_; }

        protected override VU_ERRCODE Process(VU_BOOL autod) { throw new NotImplementedException(); }

        private int LocalSize() { throw new NotImplementedException(); }

        protected VU_ID srcmsgid_;
        protected VUERROR etype_;
    }

    //--------------------------------------------------
    public abstract class VuRequestMessage : VuMessage
    {

        //TODO public virtual ~VuRequestMessage();

        protected VuRequestMessage(VU_MSG_TYPE type, VU_ID entityid, VuTargetEntity target)
            : base(type, entityid, target, false)
        {
            // empty
        }
        protected VuRequestMessage(VU_MSG_TYPE type, VU_ID senderid, VU_ID dest)
            : base(type, senderid, dest)
        {
            // empty
        }
        //protected abstract VU_ERRCODE Process(VU_BOOL autod);


    }

    //--------------------------------------------------
    public enum VU_SPECIAL_GET_TYPE
    {
        VU_GET_GAME_ENTS,
        VU_GET_GLOBAL_ENTS
    }

    public class VuGetRequest : VuRequestMessage
    {

        public VuGetRequest(VU_SPECIAL_GET_TYPE sgt, VuSessionEntity sess = null)
            : base(VU_MSG_TYPE.VU_GET_REQUEST_MESSAGE, VU_ID.vuNullId,
                     (sess != null ? sess
                      : ((sgt == VU_SPECIAL_GET_TYPE.VU_GET_GLOBAL_ENTS) ? (VuTargetEntity)VUSTATIC.vuGlobalGroup
                         : (VuTargetEntity)VUSTATIC.vuLocalSessionEntity.Game())))
        {
        }
        public VuGetRequest(VU_ID entityId, VuTargetEntity target)
            : base(VU_MSG_TYPE.VU_GET_REQUEST_MESSAGE, entityId, target)
        {
            // empty
        }
        public VuGetRequest(VU_ID senderid, VU_ID target)
            : base(VU_MSG_TYPE.VU_GET_REQUEST_MESSAGE, senderid, target)
        {
            // empty
        }
        //TODO public virtual ~VuGetRequest();

        protected override VU_ERRCODE Process(VU_BOOL autod) { throw new NotImplementedException(); }
    }

    //--------------------------------------------------
    public class VuPushRequest : VuRequestMessage
    {

        public VuPushRequest(VU_ID entityId, VuTargetEntity target)
            : base(VU_MSG_TYPE.VU_PUSH_REQUEST_MESSAGE, entityId, target)
        {
            // empty
        }
        public VuPushRequest(VU_ID senderid, VU_ID target)
            : base(VU_MSG_TYPE.VU_PUSH_REQUEST_MESSAGE, senderid, target)
        {
            // empty
        }
        //TODO public virtual ~VuPushRequest();


        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;

            if (!IsLocal() && Destination() == VUSTATIC.vuLocalSession)
            {
                if (Entity() != null)
                {
                    retval = Entity().Handle(this);
                }
                else
                {
                    VuTargetEntity sender = (VuTargetEntity)VUSTATIC.vuDatabase.Find(Sender());
                    if (sender != null && sender.IsTarget())
                    {
                        VuMessage resp = new VuErrorMessage(VUERROR.VU_NO_SUCH_ENTITY_ERROR, Sender(),
                              EntityId(), sender);
                        resp.RequestReliableTransmit();
                        VuMessageQueue.PostVuMessage(resp);
                        retval = VU_ERRCODE.VU_SUCCESS;
                    }
                }
            }
            return retval;
        }

    }

    //--------------------------------------------------
    public class VuPullRequest : VuRequestMessage
    {

        public VuPullRequest(VU_ID entityId, VuTargetEntity target)
            : base(VU_MSG_TYPE.VU_PULL_REQUEST_MESSAGE, entityId, target)
        {
            // empty
        }
        public VuPullRequest(VU_ID senderid, VU_ID target)
            : base(VU_MSG_TYPE.VU_PUSH_REQUEST_MESSAGE, senderid, target)
        {
            // empty
        }
        //TODO public virtual ~VuPullRequest();

        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;

            if (!IsLocal() && Destination() == VUSTATIC.vuLocalSession)
            {
                if (Entity() != null)
                {
                    retval = Entity().Handle(this);
                }
                else
                {
                    VuTargetEntity sender = (VuTargetEntity)VUSTATIC.vuDatabase.Find(Sender());
                    if (sender != null && sender.IsTarget())
                    {
                        VuMessage resp = new VuErrorMessage(VUERROR.VU_NO_SUCH_ENTITY_ERROR, Sender(),
                                                  EntityId(), sender);
                        resp.RequestReliableTransmit();
                        VuMessageQueue.PostVuMessage(resp);
                        retval = VU_ERRCODE.VU_SUCCESS;
                    }
                }
            }

            return retval;
        }

    }

    //--------------------------------------------------
    public abstract class VuEvent : VuMessage
    {

        //TODO public virtual ~VuEvent();

        public override int Size() { throw new NotImplementedException(); }
        public override int Decode(ByteWrapper buf) { throw new NotImplementedException(); }
        public override int Encode(ByteWrapper buf) { throw new NotImplementedException(); }

        protected VuEvent(VU_MSG_TYPE type, VU_ID entityId, VuTargetEntity target, VU_BOOL loopback = false)
            : base(type, entityId, target, loopback)
        {

            updateTime_ = VUSTATIC.vuxGameTime;
        }

        protected VuEvent(VU_MSG_TYPE type, VU_ID senderid, VU_ID target)
            : base(type, senderid, target)
        {

            updateTime_ = VUSTATIC.vuxGameTime;
        }
        protected override VU_ERRCODE Activate(VuEntity ent) { throw new NotImplementedException(); }
        //protected abstract VU_ERRCODE Process(VU_BOOL autod);

        private int LocalSize() { throw new NotImplementedException(); }

        // DATA

        // these fields are filled in on Activate()
        public VU_TIME updateTime_;
    }

    //--------------------------------------------------
    public class VuCreateEvent : VuEvent
    {

        public VuCreateEvent(VuEntity entity, VuTargetEntity target, VU_BOOL loopback = false)
            : base(VU_MSG_TYPE.VU_CREATE_EVENT, entity.Id(), target, loopback)
        {
#if VU_USE_CLASS_INFO
  memcpy(classInfo_, entity.EntityType().classInfo_, CLASS_NUM_BYTES);
#endif
            vutype_ = entity.Type();
            size_ = 0;
            data_ = null;
            expandedData_ = null;
        }
        public VuCreateEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_TYPE.VU_CREATE_EVENT, senderid, target)
        {
#if  VU_USE_CLASS_INFO
  memset(classInfo_, '\0', CLASS_NUM_BYTES);
#endif
            size_ = 0;
            data_ = null;
            vutype_ = 0;
            expandedData_ = null;
        }
        //TODO public virtual ~VuCreateEvent();

        public override int Size() { throw new NotImplementedException(); }
        public override int Decode(ByteWrapper buf) { throw new NotImplementedException(); }
        public override int Encode(ByteWrapper buf) { throw new NotImplementedException(); }
        public override VU_BOOL DoSend()     // returns true if ent is in database
        {
            if (Entity() != null && Entity().VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                return true;
            }
            return false;
        }


        public VuEntity EventData() { return expandedData_; }

        protected VuCreateEvent(VU_MSG_TYPE type, VuEntity ent, VuTargetEntity target,
                              VU_BOOL loopback = false)
            : base(type, ent.Id(), target, loopback)
        {
            SetEntity(ent);
#if VU_USE_CLASS_INFO
  memcpy(classInfo_, ent.EntityType().classInfo_, CLASS_NUM_BYTES);
#endif
            vutype_ = ent.Type();
            size_ = 0;
            data_ = null;
            expandedData_ = null;

#if _DEBUG
//  MonoPrint("VuCreateEvent id target loopback %d,%d,%d\n", entity.Id(), target,loopback);//me123
#endif
        }

        protected VuCreateEvent(VU_MSG_TYPE type, VU_ID senderid, VU_ID target)
            : base(type, senderid, target)
        {
#if VU_USE_CLASS_INFO
  memset(classInfo_, '\0', CLASS_NUM_BYTES);
#endif
            size_ = 0;
            data_ = null;
            vutype_ = 0;
            expandedData_ = null;

#if _DEBUG
//MonoPrint("VuCreateEvent1 senderid target %d,%d\n", senderid, target);//me123
#endif
        }

        protected override VU_ERRCODE Activate(VuEntity ent) { throw new NotImplementedException(); }
        protected override VU_ERRCODE Process(VU_BOOL autod) { throw new NotImplementedException(); }

        private int LocalSize() { throw new NotImplementedException(); }

        // data

        public VuEntity expandedData_;
#if  VU_USE_CLASS_INFO
  VU_BYTE classInfo_[CLASS_NUM_BYTES];	// entity class type
#endif
        public ushort vutype_;			// entity type
        public ushort size_;
        public VU_BYTE[] data_;
    }

    //--------------------------------------------------
    public class VuManageEvent : VuCreateEvent
    {

        public VuManageEvent(VuEntity entity, VuTargetEntity target, VU_BOOL loopback = false)
            : base(VU_MSG_TYPE.VU_MANAGE_EVENT, entity, target, loopback)
        {
            // empty
        }

        public VuManageEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_TYPE.VU_MANAGE_EVENT, senderid, target)
        {
            // empty
        }

        //TODO public virtual ~VuManageEvent();

    }

    //--------------------------------------------------
    public class VuDeleteEvent : VuEvent
    {

        public VuDeleteEvent(VuEntity entity)
            : base(VU_MSG_TYPE.VU_DELETE_EVENT, entity.Id(),
      entity.IsGlobal() ? (VuTargetEntity)VUSTATIC.vuGlobalGroup
                         : (VuTargetEntity)VUSTATIC.vuLocalSessionEntity.Game(), true)
        {
            SetEntity(entity);
        }

        public VuDeleteEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_TYPE.VU_DELETE_EVENT, senderid, target)
        {
            // empty
        }

        //TODO public virtual ~VuDeleteEvent();

        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            if (Entity() != null)
            {
                Entity().Handle(this);
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        protected override VU_ERRCODE Activate(VuEntity ent)
        {
            if (ent != null)
            {
                SetEntity(ent);
            }
            if (Entity() != null)
            {
                if (Entity().VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
                {
                    VUSTATIC.vuDatabase.DeleteRemove(Entity());
                    return VU_ERRCODE.VU_SUCCESS;
                }
                else if (VUSTATIC.vuAntiDB.Find(entityId_) != null)
                {
                    VUSTATIC.vuAntiDB.Remove(entityId_);
                    return VU_ERRCODE.VU_SUCCESS;
                }
                else if (!Entity().IsLocal())
                {
                    // prevent duplicate delete event from remote source
                    SetEntity(null);
                }
            }
            return VU_ERRCODE.VU_NO_OP;
        }

    }

    //--------------------------------------------------
    public class VuUnmanageEvent : VuEvent
    {

        public VuUnmanageEvent(VuEntity entity, VuTargetEntity target,
                              VU_TIME mark, VU_BOOL loopback = false)
            : base(VU_MSG_TYPE.VU_UNMANAGE_EVENT, entity.Id(), target, loopback)
        {
            mark_ = mark;

            SetEntity(entity);
            // set kill fuse
            VuReleaseEvent release = new VuReleaseEvent(entity);
            VuTimerEvent timer = new VuTimerEvent(null, mark, TIMERTYPES.VU_DELETE_TIMER, release);
            VuMessageQueue.PostVuMessage(timer);
        }

        public VuUnmanageEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_TYPE.VU_UNMANAGE_EVENT, senderid, target)
        {
            mark_ = 0;
            // empty
        }

        //TODO public virtual ~VuUnmanageEvent();

        public override int Size()
        {
            return sizeof(VU_TIME);
        }
        public override int Decode(ByteWrapper buf) { throw new NotImplementedException(); }
        public override int Encode(ByteWrapper buf) { throw new NotImplementedException(); }

        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            if (Entity() != null)
            {
                Entity().Handle(this);
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        private int LocalSize() { throw new NotImplementedException(); }

        // data

        // time of removal
        public VU_TIME mark_;
    }

    //--------------------------------------------------
    public class VuReleaseEvent : VuEvent
    {

        public VuReleaseEvent(VuEntity entity)
            : base(VU_MSG_TYPE.VU_RELEASE_EVENT, entity.Id(), VUSTATIC.vuLocalSessionEntity, true)
        {
            SetEntity(entity);
        }
        //TODO public virtual ~VuReleaseEvent();

        public override int Size()
        {
            return 0;
        }

        // all these are stubbed out here, as this is not a net message
        public override int Decode(ByteWrapper buf)
        {
            // not a net event, so just return
            return 0;
        }


        public override int Encode(ByteWrapper buf)
        {
            // not a net event, so just return
            return 0;
        }


        public override VU_BOOL DoSend()     // returns false
        {
            return false;
        }

        protected override VU_ERRCODE Activate(VuEntity ent)
        {
            if (ent != null)
            {
                SetEntity(ent);
            }
            if (Entity() != null && Entity().VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                if (Entity().Id().creator_ == VUSTATIC.vuLocalSession.creator_)
                {
                    // anti db will check for presense in db prior to insert
                    VUSTATIC.vuAntiDB.Insert(Entity());
                }
                Entity().share_.ownerId_ = VU_ID.vuNullId;
                VUSTATIC.vuDatabase.DeleteRemove(Entity());
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }
        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            if (Entity() != null)
            {
                Entity().Handle(this);
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }


    }

    //--------------------------------------------------
    public class VuTransferEvent : VuEvent
    {

        public VuTransferEvent(VuEntity entity, VuTargetEntity target, VU_BOOL loopback = false)
            : base(VU_MSG_TYPE.VU_TRANSFER_EVENT, entity.Id(), target, loopback)
        {
            newOwnerId_ = entity.OwnerId();
            SetEntity(entity);
        }
        public VuTransferEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_TYPE.VU_TRANSFER_EVENT, senderid, target)
        {
            newOwnerId_ = VU_ID.vuNullId;
            // empty
        }
        //TODO public virtual ~VuTransferEvent();

        public override int Size() { throw new NotImplementedException(); }

        public override int Decode(ByteWrapper buf) { throw new NotImplementedException(); }
        public override int Encode(ByteWrapper buf) { throw new NotImplementedException(); }

        protected override VU_ERRCODE Activate(VuEntity ent)
        {
            return base.Activate(ent);
        }
        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            if (Entity() != null)
            {
                Entity().Handle(this);
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        private int LocalSize() { throw new NotImplementedException(); }

        // data
        public VU_ID newOwnerId_;
    }

    //--------------------------------------------------
    public class VuPositionUpdateEvent : VuEvent
    {

        public VuPositionUpdateEvent(VuEntity entity, VuTargetEntity target,
                              VU_BOOL loopback = false)
            : base(VU_MSG_TYPE.VU_POSITION_UPDATE_EVENT, entity.Id(), target, loopback)
        {
            if (entity != null)
            {
                SetEntity(entity);
                updateTime_ = entity.LastUpdateTime();

                x_ = entity.XPos();
                y_ = entity.YPos();
                z_ = entity.ZPos();

                dx_ = entity.XDelta();
                dy_ = entity.YDelta();
                dz_ = entity.ZDelta();

#if VU_USE_QUATERNION
    VU_QUAT *quat = entity.Quat();
    ML_QuatCopy(quat_, *quat);
    VU_VECT *dquat = entity.DeltaQuat();
    ML_VectCopy(dquat_, *dquat);
    theta_ = entity.Theta();
#else
                yaw_ = entity.Yaw();
                pitch_ = entity.Pitch();
                roll_ = entity.Roll();

                //dyaw_   = entity.YawDelta();
                //dpitch_ = entity.PitchDelta();
                //droll_  = entity.RollDelta();
                //VU_PRINT("yaw=%3.3f, pitch=%3.3f, roll=%3.3f, dyaw=%3.3f, dpitch=%3.3f, droll=%3.3f\n", yaw_, pitch_, roll_, dyaw_, dpitch_, droll_);
#endif
            }
        }
        public VuPositionUpdateEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_TYPE.VU_POSITION_UPDATE_EVENT, senderid, target)
        {
            // empty
        }
        //TODO public virtual ~VuPositionUpdateEvent();

        public override int Size() { throw new NotImplementedException(); }
        public override int Decode(ByteWrapper buf) { throw new NotImplementedException(); }
        public override int Encode(ByteWrapper buf) { throw new NotImplementedException(); }

        public override VU_BOOL DoSend()
        {
            // test is done in vudriver.cpp, prior to generation of event
            return true;
        }


        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            if (Entity() != null)
            {
                Entity().Handle(this);
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        private int LocalSize() { throw new NotImplementedException(); }

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
    public class VuBroadcastGlobalEvent : VuEvent
    {

        public VuBroadcastGlobalEvent(VuEntity entity, VuTargetEntity target, VU_BOOL loopback = false)
            : base(VU_MSG_TYPE.VU_BROADCAST_GLOBAL_EVENT, entity.Id(), target, loopback)
        {

#if VU_USE_CLASS_INFO
	memcpy(classInfo_, entity->EntityType()->classInfo_, CLASS_NUM_BYTES);
#endif


            vutype_ = entity.Type();

            SetEntity(entity);
            updateTime_ = entity.LastUpdateTime();
        }

        public VuBroadcastGlobalEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_TYPE.VU_BROADCAST_GLOBAL_EVENT, senderid, target)
        {
#if VU_USE_CLASS_INFO
	memset(classInfo_, '\0', CLASS_NUM_BYTES);
#endif
            vutype_ = 0;
        }
        //TODO public virtual ~VuBroadcastGlobalEvent();

        public void MarkAsKeepalive() { flags_ |= VU_MSG_FLAG.VU_KEEPALIVE_MSG_FLAG; }

        public override int Size() { throw new NotImplementedException(); }
        public override int Decode(ByteWrapper buf) { throw new NotImplementedException(); }
        public override int Encode(ByteWrapper buf) { throw new NotImplementedException(); }

        protected override VU_ERRCODE Activate(VuEntity ent) { throw new NotImplementedException(); }
        protected override VU_ERRCODE Process(VU_BOOL autod) { throw new NotImplementedException(); }

        // data

#if  VU_USE_CLASS_INFO
  VU_BYTE classInfo_[CLASS_NUM_BYTES];	// entity class type
#endif
        protected ushort vutype_;			// entity type

    }

    //--------------------------------------------------
    public class VuFullUpdateEvent : VuCreateEvent
    {

        public VuFullUpdateEvent(VuEntity entity, VuTargetEntity target, VU_BOOL loopback = false)
            : base(VU_MSG_TYPE.VU_FULL_UPDATE_EVENT, entity, target, loopback)
        {
            SetEntity(entity);
            updateTime_ = entity.LastUpdateTime();
        }

        public VuFullUpdateEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_TYPE.VU_FULL_UPDATE_EVENT, senderid, target)
        {
            // empty
        }
        //TODO public virtual ~VuFullUpdateEvent();

        public VU_BOOL WasCreated() { return (VU_BOOL)(Entity() == expandedData_ ? true : false); }
        public void MarkAsKeepalive() { flags_ |= VU_MSG_FLAG.VU_KEEPALIVE_MSG_FLAG; }


        protected override VU_ERRCODE Activate(VuEntity ent) { throw new NotImplementedException(); }

    };

    //--------------------------------------------------
    public class VuEntityCollisionEvent : VuEvent
    {

        public VuEntityCollisionEvent(VuEntity entity, VU_ID otherId,
                                VU_DAMAGE hitLocation, int hitEffect,
                                VuTargetEntity target, VU_BOOL loopback = false)
            : base(VU_MSG_TYPE.VU_ENTITY_COLLISION_EVENT, entity.Id(), target, loopback)
        {

            otherId_ = otherId;
            hitLocation_ = hitLocation;
            hitEffect_ = hitEffect;
            SetEntity(entity);
        }
        public VuEntityCollisionEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_TYPE.VU_ENTITY_COLLISION_EVENT, senderid, target)
        {

            otherId_ = new VU_ID(0, 0);
        }

        //TODO public virtual ~VuEntityCollisionEvent();

        public override int Size() { throw new NotImplementedException(); }
        public override int Decode(ByteWrapper buf) { throw new NotImplementedException(); }
        public override int Encode(ByteWrapper buf) { throw new NotImplementedException(); }


        protected override VU_ERRCODE Process(VU_BOOL autod) { throw new NotImplementedException(); }

        private int LocalSize() { throw new NotImplementedException(); }

        // data
        public VU_ID otherId_;
        public VU_DAMAGE hitLocation_;	// affects damage
        public int hitEffect_;		// affects hitpoints/health
    }

    //--------------------------------------------------
    public class VuGroundCollisionEvent : VuEvent
    {

        public VuGroundCollisionEvent(VuEntity entity, VuTargetEntity target, VU_BOOL loopback = false)
            : base(VU_MSG_TYPE.VU_GROUND_COLLISION_EVENT, entity.Id(), target, loopback)
        {
            // empty
        }
        public VuGroundCollisionEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_TYPE.VU_GROUND_COLLISION_EVENT, senderid, target)
        {
            // empty
        }
        //TODO public virtual ~VuGroundCollisionEvent();


        protected override VU_ERRCODE Process(VU_BOOL autod) { throw new NotImplementedException(); }

    }

    //--------------------------------------------------
    public class VuSessionEvent : VuEvent
    {

        public VuSessionEvent(VuEntity ent, vuEventTypes subtype, VuTargetEntity target,
                       VU_BOOL loopback = false)
            : base(VU_MSG_TYPE.VU_SESSION_EVENT, ent.Id(), target, loopback)
        {
            subtype_ = subtype;
            group_ = new VU_ID(0, 0);
            callsign_ = null;
            syncState_ = VuSessionSync.VU_NO_SYNC;
            gameTime_ = VUSTATIC.vuxGameTime;
            string name = "bad session";
            if (ent.IsSession())
            {
                name = ((VuSessionEntity)ent).Callsign();
                group_ = ((VuSessionEntity)ent).GameId();

#if VU_TRACK_LATENCY
    syncState_ = ((VuSessionEntity*)ent)->TimeSyncState();
#endif

            }
            else if (ent.IsGroup())
            {
                name = ((VuGroupEntity)ent).GroupName();
            }
            callsign_ = name;

            SetEntity(ent);
            RequestReliableTransmit();
            RequestOutOfBandTransmit();
        }

        public VuSessionEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_TYPE.VU_SESSION_EVENT, senderid, target)
        {
            subtype_ = vuEventTypes.VU_SESSION_UNKNOWN_SUBTYPE;
            group_ = new VU_ID(0, 0);
            callsign_ = null;
            syncState_ = VuSessionSync.VU_NO_SYNC;
            gameTime_ = VUSTATIC.vuxGameTime;
            RequestReliableTransmit();
        }
        //TODO public virtual ~VuSessionEvent();

        public override int Size() { throw new NotImplementedException(); }
        public override int Decode(ByteWrapper buf) { throw new NotImplementedException(); }
        public override int Encode(ByteWrapper buf) { throw new NotImplementedException(); }


        protected override VU_ERRCODE Process(VU_BOOL autod) { throw new NotImplementedException(); }

        private int LocalSize() { throw new NotImplementedException(); }

        // data

        public vuEventTypes subtype_;
        public VU_ID group_;
        public string callsign_;
        public VuSessionSync syncState_;
        public VU_TIME gameTime_;
    }

    //--------------------------------------------------
    public class VuTimerEvent : VuEvent
    {

        public VuTimerEvent(VuEntity entity, VU_TIME mark, TIMERTYPES timertype, VuMessage evnt = null)
            : base(VU_MSG_TYPE.VU_TIMER_EVENT, (entity != null ? entity.Id() : VU_ID.vuNullId), VUSTATIC.vuLocalSessionEntity, true)
        {
            mark_ = mark;
            timertype_ = timertype;
            event_ = evnt;
            next_ = null;
            SetEntity(entity);
            if (event_ != null)
            {
                event_.Ref();
            }
        }
        //TODO public virtual ~VuTimerEvent();

        public override int Size()
        {
            return 0;
        }

        // all these are stubbed out here, as this is not a net message
        public override int Decode(ByteWrapper buf)
        {
            // not a net event, so just return
            return 0;
        }

        public override int Encode(ByteWrapper buf)
        {
            // not a net event, so just return
            return 0;
        }

        public override VU_BOOL DoSend()
        {
            return false;
        }

        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;

            if (event_ != null)
            {
#if DEBUG_COMMS
    VU_PRINT("VU: Posting timer delayed event id %d -- time = %d\n", msgid_.id_, vuxRealTime);
#endif

                if (event_.Target() != null && event_.Target() != VUSTATIC.vuLocalSessionEntity)//me123 from Target() to event_->Target()
                {
                    retval = event_.Send();
                }
                else
                {
                    retval = event_.Dispatch(false);
                }

                //VuMessageQueue::PostVuMessage(event_);
                event_.UnRef();
                retval = VU_ERRCODE.VU_SUCCESS;
            }

            if (EntityId() != VU_ID.vuNullId)
            {
                if (Entity() != null)
                {
                    Entity().Handle(this);
                    retval++;
                }
            }
            return retval;
        }

        // data

        // time of firing
        public VU_TIME mark_;
        public TIMERTYPES timertype_;
        // event to launch on firing
        public VuMessage event_;


        private VuTimerEvent next_;
    }

    //--------------------------------------------------
    public class VuShutdownEvent : VuEvent
    {

        public VuShutdownEvent(VU_BOOL all = false)
            : base(VU_MSG_TYPE.VU_SHUTDOWN_EVENT, VU_ID.vuNullId, VUSTATIC.vuLocalSessionEntity, true)
        {
            shutdownAll_ = all;
            done_ = false;
            // empty
        }
        //TODO public virtual ~VuShutdownEvent();

        public override int Size() { throw new NotImplementedException(); }
        // all these are stubbed out here, as this is not a net message
        public override int Decode(ByteWrapper buf) { throw new NotImplementedException(); }
        public override int Encode(ByteWrapper buf) { throw new NotImplementedException(); }
        public override VU_BOOL DoSend()     // returns false
        { throw new NotImplementedException(); }

        protected override VU_ERRCODE Process(VU_BOOL autod) { throw new NotImplementedException(); }

        // data

        public VU_BOOL shutdownAll_;
        public VU_BOOL done_;
    }

#if VU_SIMPLE_LATENCY
//--------------------------------------------------
class VuTimingMessage : public VuMessage {
public:
  VuTimingMessage(VU_ID entityId, VuTargetEntity *target, VU_BOOL loopback=false);
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
    public class VuUnknownMessage : VuMessage
    {

        public VuUnknownMessage()
            : base(VU_MSG_TYPE.VU_UNKNOWN_MESSAGE, VU_ID.vuNullId, VUSTATIC.vuLocalSessionEntity, true)
        {
            // empty
        }
        //TODO public virtual ~VuUnknownMessage();

        public override int Size()
        {
            return 0;
        }

        // all these are stubbed out here, as this is not a net message
        public override int Decode(ByteWrapper buf)
        {
            // not a net event, so just return
            return 0;
        }

        public override int Encode(ByteWrapper buf)
        {
            // Not a net event, so just return
            return 0;
        }

        public override VU_BOOL DoSend()     // returns false
        {
            return false;
        }

        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            return VU_ERRCODE.VU_NO_OP;
        }
    }

    //--------------------------------------------------
    // the VuResendMsgFilter is used to hold messages for which send failed:
    //   - IsLocal() is true
    //   - VU_SEND_FAILED_MSG_FLAG is set
    //   - either VU_RELIABLE_MSG_FLAG or VU_KEEPALIVE_MSG_FLAG are set
    internal class VuResendMsgFilter : VuMessageFilter
    {

        public VuResendMsgFilter()
        {
            // emtpy
        }

        //TODO public virtual ~VuResendMsgFilter();
        public override VU_BOOL Test(VuMessage message)
        {
#if NOTHING
  if ((message.Flags() & VU_SEND_FAILED_MSG_FLAG) &&
      ((message.Flags() & VU_RELIABLE_MSG_FLAG) ||
       (message.Flags() & VU_KEEPALIVE_MSG_FLAG)) ) {
    return TRUE;
  }
  return FALSE;
#else
            if ((message.Flags() & VU_MSG_FLAG.VU_SEND_FAILED_MSG_FLAG) != 0 &&
                (message.Flags() & (VU_MSG_FLAG.VU_RELIABLE_MSG_FLAG | VU_MSG_FLAG.VU_KEEPALIVE_MSG_FLAG)) != 0)
                return true;
            else
                return false;
#endif
        }

        public override VuMessageFilter Copy()
        {
            return new VuResendMsgFilter();
        }

    }
}