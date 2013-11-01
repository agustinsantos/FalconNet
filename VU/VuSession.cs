using FalconNet.Common.Encoding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VU_BOOL = System.Boolean;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;
using VU_KEY = System.UInt64;
using VU_ID_NUMBER = System.UInt32;

namespace FalconNet.VU
{
    public enum VuSessionSync
    {
        VU_NO_SYNC,
        VU_SLAVE_SYNC,
        VU_MASTER_SYNC,
    }

    public abstract class VuTargetEntity : VuEntity
    {

        //public virtual ~VuTargetEntity();

        // Special VU type getters
        public override VU_BOOL IsTarget()
        {
            return true;
        }

        public abstract VU_BOOL HasTarget(VU_ID id);

        public abstract VU_BOOL InTarget(VU_ID id);

#if VU_USE_COMMS
  virtual VuTargetEntity *ForwardingTarget(VuMessage *msg = 0);

  int FlushOutboundMessageBuffer();
  int SendMessage(VuMessage *msg);
  int GetMessages();
  int SendQueuedMessage ();

  void SetDirty (void);
  void ClearDirty (void);
  int IsDirty (void);
  
  // normal (best effort) comms handle
  VuCommsConnectStatus GetCommsStatus() { return bestEffortComms_.status_; }
  ComAPIHandle GetCommsHandle() { return bestEffortComms_.handle_; }
  void SetCommsStatus(VuCommsConnectStatus cs) { bestEffortComms_.status_ = cs;}
  void SetCommsHandle(ComAPIHandle ch, int bufSize=0, int packSize=0);

  // reliable comms handle
  VuCommsConnectStatus GetReliableCommsStatus() {return reliableComms_.status_;}
  ComAPIHandle GetReliableCommsHandle() { return reliableComms_.handle_; }
  void SetReliableCommsStatus(VuCommsConnectStatus cs) { reliableComms_.status_ = cs; }
  void SetReliableCommsHandle(ComAPIHandle ch, int bufSize=0, int packSize=0);

  int BytesPending();
  int MaxPacketSize();
  int MaxMessageSize();
  int MaxReliablePacketSize();
  int MaxReliableMessageSize();

protected:
  int Flush(VuCommsContext *ctxt);
  int FlushLow(VuCommsContext *ctxt);
  int SendOutOfBand(VuCommsContext *ctxt, VuMessage *msg);
  int SendNormalPriority(VuCommsContext *ctxt, VuMessage *msg);
  int SendLowPriority(VuCommsContext *ctxt, VuMessage *msg);
#endif


        protected VuTargetEntity(ushort type, VU_ID_NUMBER eid)
            : base(type, eid)
        {
#if VU_USE_COMMS
  Init(&bestEffortComms_);
  Init(&reliableComms_);
  reliableComms_.reliable_ = true;
#endif //VU_USE_COMMS
            dirty = 0;
        }

        internal VuTargetEntity()
            : base((ushort)VU_UNKNOWN_ENTITY_TYPE, 0)
        { }

        //data
#if VU_USE_COMMS
  VuCommsContext bestEffortComms_;
  VuCommsContext reliableComms_;
#endif
        internal int dirty;
    }

    public static class VuTargetEntityEncodingLE
    {

        public static void Encode(Stream stream, VuTargetEntity val)
        {
            UInt16EncodingLE.Encode(stream, val.share_.entityType_);
            UInt16EncodingLE.Encode(stream, (UInt16)val.share_.flags_);
            UInt16EncodingLE.Encode(stream, (UInt16)val.share_.id_.creator_);
            UInt64EncodingLE.Encode(stream, val.share_.id_.num_);
        }

        public static void Decode(Stream stream, ref VuTargetEntity rst)
        {
#if VU_USE_COMMS
  Init(&bestEffortComms_);
  Init(&reliableComms_);
  reliableComms_.reliable_ = true;
#endif //VU_USE_COMMS
            rst.share_.entityType_ = UInt16EncodingLE.Decode(stream);
            rst.share_.flags_ = (VuFlagBits)UInt16EncodingLE.Decode(stream);
            rst.share_.id_  = new VU_ID();
            VU_IDEncodingLE.Decode(stream, rst.share_.id_);
            rst.SetEntityType(rst.share_.entityType_);
            rst.dirty = 0;
        }

        public static int Size
        {
            get { return UInt16EncodingLE.Size * 3 + UInt64EncodingLE.Size; }
        }

    }

    public class VuGroupNode
    {
        public VU_ID gid_;
        public VuGroupNode next_;
    }

    public enum VU_GAME_ACTION
    {
        VU_NO_GAME_ACTION,
        VU_JOIN_GAME_ACTION,
        VU_CHANGE_GAME_ACTION,
        VU_LEAVE_GAME_ACTION
    }


    public class VuSessionEntity : VuTargetEntity
    {
        public static readonly VU_ID VU_SESSION_NULL_CONNECTION = VU_ID.vuNullId;
        public static readonly VU_ID VU_SESSION_NULL_GROUP = VU_ID.vuNullId;

        // constructors & destructor
        protected VuSessionEntity(ushort typeindex, ulong domainMask, string callsign)
            : base(typeindex, VU_ID.VU_SESSION_ENTITY_ID)
        {

            sessionId_ = VU_SESSION_NULL_CONNECTION.creator_; // assigned on session open
            domainMask_ = domainMask;
            callsign_ = null;
            loadMetric_ = 1;
            gameId_ = VU_SESSION_NULL_GROUP; // assigned on session open
            groupCount_ = 0;
            groupHead_ = null;
            bandwidth_ = 33600;
#if VU_SIMPLE_LATENCY
            timeDelta_(0),
            latency_(0),
#endif //VU_SIMPLE_LATENCY
#if VU_TRACK_LATENCY
            timeSyncState_(VU_NO_SYNC),
            latency_(0),
            masterTime_(0),
            masterTimePostTime_(0),
            responseTime_(0),
            masterTimeOwner_(0),
            lagTotal_(0),
            lagPackets_(0),
            lagUpdate_(LAG_COUNT_START),
#endif // VU_TRACK_LATENCY
            lastMsgRecvd_ = 0;
            game_ = null;
            action_ = VU_GAME_ACTION.VU_NO_GAME_ACTION;
#if VU_MAX_SESSION_CAMERAS
            cameraCount_ = 0;
            for (int j = 0; j < VU_MAX_SESSION_CAMERAS; j++) {
                cameras_[j] = vuNullId;
            }
#endif

            share_.id_.creator_ = sessionId_;
            share_.id_.num_ = VU_ID.VU_SESSION_ENTITY_ID;
            share_.ownerId_ = share_.id_;   // need to reset this
            SetCallsign(callsign);
            SetKeepaliveTime(VUSTATIC.vuxRealTime);
        }

        //TODO public virtual ~VuSessionEntity();
        public VuSessionEntity() { }

        public VuSessionEntity(ulong domainMask, string callsign)
            : base(VU_SESSION_ENTITY_TYPE, VU_ID.VU_SESSION_ENTITY_ID)
        {
            sessionId_ = VU_SESSION_NULL_CONNECTION.creator_; // assigned on session open
            domainMask_ = domainMask;
            callsign_ = null;
            loadMetric_ = 1;
            gameId_ = VU_SESSION_NULL_GROUP; // assigned on session open
            groupCount_ = 0;
            groupHead_ = null;
            bandwidth_ = 33600;//me123
#if VU_SIMPLE_LATENCY
            timeDelta_(0),
            latency_(0),
#endif //VU_SIMPLE_LATENCY
#if VU_TRACK_LATENCY
            timeSyncState_(VU_NO_SYNC),
            latency_(0),
            masterTime_(0),
            masterTimePostTime_(0),
            responseTime_(0),
            masterTimeOwner_(0),
            lagTotal_(0),
            lagPackets_(0),
            lagUpdate_(LAG_COUNT_START),
#endif // VU_TRACK_LATENCY
            lastMsgRecvd_ = 0;
            game_ = null;
            action_ = VU_GAME_ACTION.VU_NO_GAME_ACTION;

#if VU_MAX_SESSION_CAMERAS
            cameraCount_ = 0;
            for (int j = 0; j < VU_MAX_SESSION_CAMERAS; j++) {
                cameras_[j] = vuNullId;
            }
#endif

            share_.id_.creator_ = sessionId_;
            share_.id_.num_ = VU_ID.VU_SESSION_ENTITY_ID;
            share_.ownerId_ = share_.id_;   // need to reset this
            SetCallsign(callsign);
            SetKeepaliveTime(VUSTATIC.vuxRealTime);
        }

        // accessors
        public ulong DomainMask() { return domainMask_; }
        public VU_SESSION_ID SessionId() { return sessionId_; }
        public string Callsign() { return callsign_; }
        public VU_BYTE LoadMetric() { return loadMetric_; }
        public VU_ID GameId() { return gameId_; }
        
        public VuGameEntity Game()
        {
            if (game_ == null)
            {
                VuEntity ent = VUSTATIC.vuDatabase.Find(gameId_);
                if (ent != null && ent.IsGame())
                {
                    game_ = (VuGameEntity)ent;
                }
                else
                {
                    game_ = null;
                }
            }
            return game_;
        }

        public int LastMessageReceived() { return lastMsgRecvd_; }
        public VU_TIME KeepaliveTime()
        {
            return LastCollisionCheckTime();
        }

        public int BandWidth() { return bandwidth_; }//me123

        // setters
        public void SetCallsign(string callsign)
        {
            string oldsign = callsign_;
#if VU_USE_COMMS
            SetDirty();
#endif
            if (callsign != null)
            {
                int len = callsign.Length;
                callsign_ = callsign;
            }
            else
            {
                callsign_ = VUSTATIC.VU_DEFAULT_PLAYER_NAME;
            }
            if (oldsign != null)
            {
                //delete [] oldsign;
                if (this == VUSTATIC.vuLocalSessionEntity)
                {
                    VuSessionEvent evnt =
                  new VuSessionEvent(this, vuEventTypes.VU_SESSION_CHANGE_CALLSIGN, VUSTATIC.vuGlobalGroup);
                    VuMessageQueue.PostVuMessage(evnt);
                }
            }
        }
        public void SetLoadMetric(VU_BYTE newMetric) { loadMetric_ = newMetric; }
        public VU_ERRCODE JoinGroup(VuGroupEntity newgroup)  //  < 0 retval ==> failure
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;

            if (newgroup == null)
            {
                return VU_ERRCODE.VU_ERROR;
            }

            retval = AddGroup(newgroup.Id());

            if (retval != VU_ERRCODE.VU_ERROR)
            {
                if (IsLocal())
                {
                    VuSessionEvent evnt = null;
                    if (!newgroup.IsLocal() && newgroup.SessionCount() == 0 &&
                        newgroup != VUSTATIC.vuGlobalGroup)
                    {
                        // we need to transfer it here...
                        VuTargetEntity target = (VuTargetEntity)VUSTATIC.vuDatabase.Find(newgroup.OwnerId());
                        if (target != null && target.IsTarget())
                        {
                            VuMessage pull = new VuPullRequest(newgroup.Id(), target);
                            // this would likely result in replicated p2p (evil!)
                            // pull.RequestReliableTransport();
                            VuMessageQueue.PostVuMessage(pull);
                        }
                    }
                    evnt = new VuSessionEvent(this, vuEventTypes.VU_SESSION_JOIN_GROUP, newgroup);
                    VuMessageQueue.PostVuMessage(evnt);
                }
                retval = newgroup.AddSession(this);
            }
            return retval;
        }

        public VU_ERRCODE LeaveGroup(VuGroupEntity group)
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_ERROR;

            if (group != null)
            {
                retval = RemoveGroup(group.Id());
                if (retval == VU_ERRCODE.VU_SUCCESS && group.RemoveSession(this) >= 0 && IsLocal())
                {
                    VuSessionEvent evnt = new VuSessionEvent(this, vuEventTypes.VU_SESSION_LEAVE_GROUP, VUSTATIC.vuGlobalGroup);
                    VuMessageQueue.PostVuMessage(evnt);
                    retval = VU_ERRCODE.VU_SUCCESS;
                }
            }
            return retval;
        }

        public VU_ERRCODE LeaveAllGroups()
        {
            CriticalSection.VuEnterCriticalSection();
            VuGroupNode gnode = groupHead_;
            VuGroupNode ngnode = (gnode != null ? gnode.next_ : null);

            while (gnode != null)
            {
                VuGroupEntity gent = (VuGroupEntity)VUSTATIC.vuDatabase.Find(gnode.gid_);
                if (gent != null && gent.IsGroup())
                {
                    LeaveGroup(gent);
                }
                else
                {
                    RemoveGroup(gnode.gid_);
                }
                gnode = ngnode;
                if (ngnode != null)
                {
                    ngnode = ngnode.next_;
                }
            }

            CriticalSection.VuExitCriticalSection();
            return VU_ERRCODE.VU_SUCCESS;
        }

        public VU_ERRCODE JoinGame(VuGameEntity newgame)  //  < 0 retval ==> failure
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;
            VuGameEntity game = Game();
            if (newgame != null && game == newgame)
            {
                return VU_ERRCODE.VU_NO_OP;
            }
            if (newgame != null && game == null)
            {
                action_ = VU_GAME_ACTION.VU_JOIN_GAME_ACTION;
            }
            else if (newgame != null && game != null)
            {
                action_ = VU_GAME_ACTION.VU_CHANGE_GAME_ACTION;
            }
            else
            {
                action_ = VU_GAME_ACTION.VU_LEAVE_GAME_ACTION;
            }
#if VU_TRACK_LATENCY
  SetTimeSync(VU_NO_SYNC);
#endif
            VuSessionEvent evnt = null;

            if (game != null)
            {
                if (game.RemoveSession(this) >= 0)
                {
                    LeaveAllGroups();
                    if (IsLocal())
                    {
                        VuMessageQueue.FlushAllQueues();
                        if (game.IsLocal())
                        {
                            // we need to transfer it away...
                            VuSessionsIterator iter = new VuSessionsIterator(Game());
                            VuSessionEntity sess = iter.GetFirst();
                            while (sess == this)
                            {
                                sess = iter.GetNext();
                            }
                            if (sess != null)
                            {
                                VuMessage push = new VuPushRequest(game.Id(), sess);
                                VuMessageQueue.PostVuMessage(push);
                            }
                        }
                        if (newgame != null)
                        {
                            evnt = new VuSessionEvent(this, vuEventTypes.VU_SESSION_CHANGE_GAME, VUSTATIC.vuGlobalGroup);
                            evnt.group_ = newgame.Id();
                        }
                        else
                        {
                            evnt = new VuSessionEvent(this, vuEventTypes.VU_SESSION_CLOSE, VUSTATIC.vuGlobalGroup);
                            gameId_ = VU_SESSION_NULL_GROUP;
                            game_ = null;
                        }

                        if (this == VUSTATIC.vuLocalSessionEntity)
                        {
                            VuMessage msg = new VuShutdownEvent(false);
                            VuMessageQueue.PostVuMessage(msg);
                            VuMainThread.vuMainThread.Update();

                            VuMessageQueue.FlushAllQueues();
                        }
                    }
                }
                else
                {
                    return VU_ERRCODE.VU_ERROR;
                }
            }

            if (newgame != null)
            {
                game_ = newgame;
                gameId_ = newgame.Id();

                if (IsLocal())
                {
                    if (!newgame.IsLocal() && newgame.SessionCount() == 0)
                    {
                        // we need to transfer it here...
                        VuTargetEntity target =
                          (VuTargetEntity)VUSTATIC.vuDatabase.Find(newgame.OwnerId());
                        if (target != null && target.IsTarget())
                        {
                            VuMessage pull = new VuPullRequest(newgame.Id(), target);
                            VuMessageQueue.PostVuMessage(pull);
                        }
                    }
                    if (game == null)
                    {
                        evnt = new VuSessionEvent(this, vuEventTypes.VU_SESSION_JOIN_GAME, VUSTATIC.vuGlobalGroup);
                        evnt.RequestReliableTransmit();
                    }
                }
                retval = newgame.AddSession(this);
            }
            if (evnt != null)
            {
                VuMessageQueue.PostVuMessage(evnt);
            }
            if (newgame == null && IsLocal() && VUSTATIC.vuPlayerPoolGroup != null)
            {
                retval = JoinGame(VUSTATIC.vuPlayerPoolGroup);
            }
            return retval;
        }

        public VU_GAME_ACTION GameAction() { return action_; }
        public void SetLastMessageReceived(int id) { lastMsgRecvd_ = id; }
        public void SetKeepaliveTime(VU_TIME ts)
        {
            SetCollisionCheckTime(ts);
        }
        public void SetBandWidth(int bandwidth) { bandwidth_ = bandwidth; }//me123

#if VU_SIMPLE_LATENCY
  public int TimeDelta()		{ return timeDelta_; }
  public void SetTimeDelta(int delta) { timeDelta_ = delta; }
  public int Latency()		{ return latency_; }
  public void SetLatency(int latency) { latency_ = latency; }
#endif //VU_SIMPLE_LATENCY
#if VU_TRACK_LATENCY
  public VU_BYTE TimeSyncState()	{ return timeSyncState_; }
  public VU_TIME Latency()		{ return latency_; }
  public void SetTimeSync(VU_BYTE newstate);
  public void SetLatency(VU_TIME latency);
#endif //VU_TRACK_LATENCY

        // Special VU type getters
        public override VU_BOOL IsSession()
        {
            return true;
        }

        public override VU_BOOL HasTarget(VU_ID id)  // true -. id contains (or is) ent
        {
            if (id == Id())
            {
                return true;
            }
            return false;
        }

        public override VU_BOOL InTarget(VU_ID id)   // true -. ent contained by (or is) id
        {
            if (id == Id() || id == GameId() || id == VUSTATIC.vuGlobalGroup.Id())
            {
                return true;
            }
            CriticalSection.VuEnterCriticalSection();
            VuGroupNode gnode = groupHead_;
            while (gnode != null)
            {
                if (gnode.gid_ == id)
                {
                    CriticalSection.VuExitCriticalSection();
                    return true;
                }
                gnode = gnode.next_;
            }
            CriticalSection.VuExitCriticalSection();
            return false;
        }

        public VU_ERRCODE AddGroup(VU_ID gid)
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_ERROR;
            if (VUSTATIC.vuGlobalGroup != null && gid == VUSTATIC.vuGlobalGroup.Id())
            {
                return VU_ERRCODE.VU_NO_OP;
            }
            CriticalSection.VuEnterCriticalSection();
            VuGroupNode gnode = groupHead_;
            // make certain group isn't already here

            while (gnode != null && gnode.gid_ != gid)
            {
                gnode = gnode.next_;
            }

            if (gnode == null)
            {
                gnode = new VuGroupNode();
                gnode.gid_ = gid;
                gnode.next_ = groupHead_;
                groupHead_ = gnode;
                groupCount_++;

                retval = VU_ERRCODE.VU_SUCCESS;
            }
            CriticalSection.VuExitCriticalSection();
            return retval;
        }

        public VU_ERRCODE RemoveGroup(VU_ID gid)
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_ERROR;
            CriticalSection.VuEnterCriticalSection();

            VuGroupNode gnode = groupHead_;
            VuGroupNode lastnode = null;

            while (gnode != null && gnode.gid_ != gid)
            {
                lastnode = gnode;
                gnode = gnode.next_;
            }
            if (gnode != null)
            { // found group
                if (lastnode != null)
                {
                    lastnode.next_ = gnode.next_;
                }
                else
                { // node was head
                    groupHead_ = gnode.next_;
                }
                groupCount_--;
                //delete gnode;
                retval = VU_ERRCODE.VU_SUCCESS;
            }

            CriticalSection.VuExitCriticalSection();
            return retval;
        }

        public VU_ERRCODE PurgeGroups()
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;
            CriticalSection.VuEnterCriticalSection();
            VuGroupNode gnode;
            while (groupHead_ != null)
            {
                gnode = groupHead_;
                groupHead_ = groupHead_.next_;
                //TODO delete gnode;
                retval = VU_ERRCODE.VU_SUCCESS;
            }
            groupCount_ = 0;
            CriticalSection.VuExitCriticalSection();
            return retval;
        }


#if VU_USE_COMMS
  public virtual VuTargetEntity *ForwardingTarget(VuMessage *msg = 0);
#endif

        // Rough dead reckoning interface
#if VU_MAX_SESSION_CAMERAS
  public int CameraCount() { return cameraCount_; }
  public VuEntity *Camera(int index);
  public int AttachCamera(VU_ID camera);
  public int RemoveCamera(VU_ID camera);
  public void ClearCameras(void);
#endif

        // evnt Handlers
#if VU_LOW_WARNING_VERSION
  public virtual VU_ERRCODE Handle(VuErrorMessage *error);
  public virtual VU_ERRCODE Handle(VuPushRequest *msg);
  public virtual VU_ERRCODE Handle(VuPullRequest *msg);
  public virtual VU_ERRCODE Handle(VuEvent *evnt);
  public virtual VU_ERRCODE Handle(VuFullUpdateEvent *evnt);
  public virtual VU_ERRCODE Handle(VuPositionUpdateEvent *evnt);
  public virtual VU_ERRCODE Handle(VuEntityCollisionEvent *evnt);
  public virtual VU_ERRCODE Handle(VuTransferEvent *evnt);
  public virtual VU_ERRCODE Handle(VuSessionEvent *evnt);
#else
        public override VU_ERRCODE Handle(VuEvent evnt)
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;

            switch (evnt.Type())
            {
                case VU_MSG_DEF.VU_DELETE_EVENT:
                case VU_MSG_DEF.VU_RELEASE_EVENT:
                    if (Game() != null && this != VUSTATIC.vuLocalSessionEntity)
                    {
                        JoinGame(VUSTATIC.vuPlayerPoolGroup);
                    }
                    retval = VU_ERRCODE.VU_SUCCESS;
                    break;
                default:
                    // do nothing
                    break;
            }
            return retval;
        }

        public override VU_ERRCODE Handle(VuFullUpdateEvent evnt)
        {
            if (evnt.EventData() != null)
            {
                SetKeepaliveTime(VUSTATIC.vuxRealTime);
                if (this != VUSTATIC.vuLocalSessionEntity)
                {
                    VuSessionEntity sData = (VuSessionEntity)evnt.EventData();
#if VU_TRACK_LATENCY
      if (vuLocalSessionEntity.TimeSyncState() == VU_MASTER_SYNC) {
        // gather statistics
        if (vuLocalSessionEntity.masterTimeOwner_ ==
            sData.masterTimeOwner_ &&
            latency_ == sData.Latency()) {

          int lag = ( (evnt.PostTime() - sData.masterTime_) -
                      (sData.responseTime_ - sData.masterTimePostTime_));
          lag = lag/2;
          
          if (lag < 0) lag = 0;
          
          lagTotal_ += lag;
          
          lagPackets_++;
          if (lagPackets_ >= lagUpdate_) {
            VU_TIME newlatency = lagTotal_ / lagPackets_;
            if (newlatency != latency_) {
              VuSessionEvent *evnt =
                new VuSessionEvent(this, VU_SESSION_LATENCY_NOTICE, Game());
              evnt.gameTime_ = newlatency;
              VuMessageQueue.PostVuMessage(evnt);
            }

            latency_    = newlatency;
            lagTotal_   = 0;
            lagPackets_ = 0;
            lagUpdate_  = lagUpdate_ * 2;
          }
        }
      } 
      else if (sData.TimeSyncState() == VU_MASTER_SYNC &&
                 vuLocalSessionEntity.GameId() == GameId()) {
        vuLocalSessionEntity.masterTime_         = sData.masterTime_;
        vuLocalSessionEntity.masterTimePostTime_ = evnt.PostTime();
        vuLocalSessionEntity.masterTimeOwner_    = Id().creator_;
      }
#endif //VU_TRACK_LATENCY
                    if (callsign_ != sData.callsign_)
                    {
                        SetCallsign(sData.callsign_);
                    }
#if VU_MAX_SESSION_CAMERAS
      cameraCount_ = sData.cameraCount_;
      if (cameraCount_ > 0 && cameraCount_ <= VU_MAX_SESSION_CAMERAS) {
        memcpy(&cameras_, &sData.cameras_, sizeof(VU_ID) * cameraCount_);
      }
#endif
                }
            }
            return base.Handle(evnt);
        }

        public override VU_ERRCODE Handle(VuSessionEvent evnt)
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_SUCCESS;
            SetKeepaliveTime(VUSTATIC.vuxRealTime);

            switch (evnt.subtype_)
            {
                case vuEventTypes.VU_SESSION_CLOSE:
                    {
                        CloseSession();
                        break;
                    }

                case vuEventTypes.VU_SESSION_JOIN_GAME:
                    {
                        VuGameEntity game = (VuGameEntity)VUSTATIC.vuDatabase.Find(evnt.group_);

                        if (game != null && game.IsGame())
                        {
                            game.Distribute(this);
                            JoinGame(game);
                        }
                        else
                        {
                            retval = 0;
                        }

                        break;
                    }

                case vuEventTypes.VU_SESSION_CHANGE_GAME:
                    {
                        retval = 0;

                        if (Game() != null)
                        //&& !F4IsBadReadPtr(Game(), sizeof(VuGameEntity)) // JB 010318 CTD
                        //)
                        {
                            Game().Distribute(this);
                            VuGameEntity game = (VuGameEntity)VUSTATIC.vuDatabase.Find(evnt.group_);
                            if (game != null)
                            {
                                if (game.IsGame())
                                {
                                    retval = VU_ERRCODE.VU_SUCCESS;
                                    game.Distribute(this);
                                    JoinGame(game);
                                }
                            }
                            else
                            {
                                //					MonoPrint ("Session refers to a game that does not exist\n");
                                VuTimerEvent timer = new VuTimerEvent(null, VUSTATIC.vuxRealTime + 1000, TIMERTYPES.VU_DELAY_TIMER, evnt);
                                VuMessageQueue.PostVuMessage(timer);
                            }
                        }
                        break;
                    }

                case vuEventTypes.VU_SESSION_JOIN_GROUP:
                    {
                        VuGroupEntity group = (VuGroupEntity)VUSTATIC.vuDatabase.Find(evnt.group_);

                        if (group != null && group.IsGroup())
                        {
                            JoinGroup(group);
                        }
                        else
                        {
                            retval = 0;
                        }
                        break;
                    }

                case vuEventTypes.VU_SESSION_LEAVE_GROUP:
                    {
                        VuGroupEntity group = (VuGroupEntity)VUSTATIC.vuDatabase.Find(evnt.group_);

                        if (group != null && group.IsGroup())
                        {
                            LeaveGroup(group);
                        }
                        else
                        {
                            retval = 0;
                        }
                        break;
                    }

                case vuEventTypes.VU_SESSION_DISTRIBUTE_ENTITIES:
                    {
                        VuGameEntity game = (VuGameEntity)VUSTATIC.vuDatabase.Find(evnt.group_);

                        if (game != null)
                        {
                            game.Distribute(this);
                        }
                        break;
                    }

                case vuEventTypes.VU_SESSION_CHANGE_CALLSIGN:
                    {
                        SetCallsign(evnt.callsign_);

                        break;
                    }

#if VU_TRACK_LATENCY
			
		case vuEventTypes.VU_SESSION_TIME_SYNC:
		{
			SetTimeSync(evnt.syncState_);

			if (evnt.syncState_ == VU_MASTER_SYNC &&
				vuLocalSessionEntity.Game() == Game() &&
				vuLocalSessionEntity.TimeSyncState() != VU_MASTER_SYNC)
			{
				vuLocalSessionEntity.masterTimeOwner_ = Id().creator_;
			}
			break;
		}
			
		case vuEventTypes.VU_SESSION_LATENCY_NOTICE:
		{
			SetLatency(evnt.gameTime_);
			break;
		}

#endif

                default:
                    {
                        // do nothing
                        retval = 0;
                        break;
                    }
            }

            return retval;
        }
#endif //VU_LOW_WARNING_VERSION

        protected VU_SESSION_ID OpenSession()	// returns session id
        {
#if VU_USE_COMMS
              if (!IsLocal() || !vuGlobalGroup || !vuGlobalGroup.Connected()) {
                return 0;
              }
              ComAPIHandle ch = vuGlobalGroup.GetCommsHandle();
              if (!ch) {
                ch = vuGlobalGroup.GetReliableCommsHandle();
              }
              if (!ch) {
                return 0;
              }
  
              int idLen = ComAPIHostIDLen(ch);
              if (idLen > sizeof(VU_SESSION_ID)) {
            //#pragma warning(disable : 4130)
            //    assert("Session id length is too large!\n" == 0);
            //#pragma warning(default : 4130)
              }
              char buf[sizeof(VU_SESSION_ID)];
              ComAPIHostIDGet(ch, buf);
              char *ptr = (char *)&sessionId_;
              for (int i = 0; i < idLen; i++) {
                ptr[sizeof(VU_SESSION_ID) - 1 - i] = buf[i];
              }
#else
            sessionId_ = 1;
#endif

            if (sessionId_ != VUSTATIC.vuLocalSession.creator_)
            {
                VuReferenceEntity(this);
                // temporarily make private to prevnt sending of bogus delete message
                share_.flags_.private_ = true;
                VUSTATIC.vuDatabase.Remove(this);
                share_.flags_.private_ = false;

                share_.ownerId_.creator_ = sessionId_;

                //  - reset ownerId for all local ent's
                VuDatabaseIterator iter = new VuDatabaseIterator();
                VuEntity ent = null;
                for (ent = iter.GetFirst(); ent != null; ent = iter.GetNext())
                {
                    if (ent.OwnerId() == VUSTATIC.vuLocalSession && ent != VUSTATIC.vuPlayerPoolGroup
                                                       && ent != VUSTATIC.vuGlobalGroup)
                    {
                        ent.SetOwnerId(OwnerId());
                    }
                }
                VuMessageQueue.FlushAllQueues();
                share_.id_.creator_ = sessionId_;
                VUSTATIC.vuLocalSession = OwnerId();
                SetVuState(VU_MEM_STATE.VU_MEM_ACTIVE);
                VUSTATIC.vuDatabase.Insert(this);
                VuDeReferenceEntity(this);
            }
            else
            {
                VuMessage dummyCreateMessage = new VuCreateEvent(this, VUSTATIC.vuGlobalGroup);
                dummyCreateMessage.RequestReliableTransmit();
                VuMessageQueue.PostVuMessage(dummyCreateMessage);
            }
            //  danm_note: not needed, as vuGlobalGroup membership is implicit
            //  JoinGroup(vuGlobalGroup);
            JoinGame(VUSTATIC.vuPlayerPoolGroup);
            return sessionId_;
        }

        protected void CloseSession()
        {
            VuGroupEntity game = Game();

            if (game == null)
            {
                // session is not open 
                if (!IsLocal())
                {
                    VUSTATIC.vuDatabase.Remove(this);
                }

                return;
            }

            action_ = VU_GAME_ACTION.VU_LEAVE_GAME_ACTION;

            game.RemoveSession(this);

            game.Distribute(this);

            if (IsLocal())
            {
#if VU_TRACK_LATENCY
		SetTimeSync(VU_NO_SYNC);
#endif
                VuSessionEvent evnt = new VuSessionEvent(this, vuEventTypes.VU_SESSION_CLOSE, VUSTATIC.vuGlobalGroup);
                VuMessageQueue.PostVuMessage(evnt);
                VuMessageQueue.FlushAllQueues();

#if VU_USE_COMMS
		VuMainThread.FlushOutboundMessages();
#endif

                gameId_ = VU_SESSION_NULL_GROUP;
                game_ = null;
            }
            else
            {
                int count = game_.SessionCount();

                VuSessionEntity sess = null;

                if (count >= 1)
                {
                    VuSessionsIterator iter = new VuSessionsIterator(Game());

                    sess = iter.GetFirst();

                    while (sess == this)
                    {
                        sess = iter.GetNext();
                    }
                }

                VuDatabaseIterator iter2 = new VuDatabaseIterator();
                VuEntity ent = iter2.GetFirst();

                while (ent != null)
                {
                    if (ent != this && ent.OwnerId() == Id() && ent.IsGlobal())
                    {
                        if (sess == null || !ent.IsTransferrable() || ((ent.IsGame() && ((VuGameEntity)ent).SessionCount() == 0)))
                        {
                            VUSTATIC.vuDatabase.Remove(ent);
                        }
                        else
                        {
                            ent.SetOwnerId(sess.Id());
                        }
                    }

                    ent = iter2.GetNext();
                }

                VUSTATIC.vuDatabase.Remove(this);
            }

            LeaveAllGroups();
        }


        internal override VU_ERRCODE InsertionCallback()
        {
            if (this != VUSTATIC.vuLocalSessionEntity)
            {
#if VU_USE_COMMS
                VUSTATIC.vuLocalSessionEntity.SetDirty();

                VuxSessionConnect(this);
#endif
                if (Game() != null)
                {
                    action_ = VU_GAME_ACTION.VU_JOIN_GAME_ACTION;
                    Game().AddSession(this);
                }

                CriticalSection.VuEnterCriticalSection();
                VuGroupNode gnode = groupHead_;

                while (gnode != null)
                {
                    VuGroupEntity group = (VuGroupEntity)VUSTATIC.vuDatabase.Find(gnode.gid_);
                    if (group != null && group.IsGroup())
                    {
                        group.AddSession(this);
                    }
                    gnode = gnode.next_;
                }
                CriticalSection.VuExitCriticalSection();
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }
        internal override VU_ERRCODE RemovalCallback()
        {
#if VU_USE_COMMS
            if (this != VUSTATIC.vuLocalSessionEntity)
            {
                CriticalSection.VuEnterCriticalSection();
                VuGroupNode gnode = groupHead_;

                while (gnode != null)
                {
                    VuGroupEntity group = (VuGroupEntity)VUSTATIC.vuDatabase.Find(gnode.gid_);
                    if (group != null && group.IsGroup())
                    {
                        VuxGroupRemoveSession(group, this);
                    }
                    gnode = gnode.next_;
                }
                CriticalSection.VuExitCriticalSection();

                if (Game() != null)
                {
                    VuxGroupRemoveSession(Game(), this);
                }

                VuxSessionDisconnect(this);
                return VU_ERRCODE.VU_SUCCESS;
            }
#endif
            return VU_ERRCODE.VU_NO_OP;
        }

        // DATA

        // shared data
        internal VU_SESSION_ID sessionId_;
        internal ulong domainMask_;
        internal string callsign_;
        internal VU_BYTE loadMetric_;
        internal VU_ID gameId_;
        internal VU_BYTE groupCount_;
        internal VuGroupNode groupHead_;
        internal int bandwidth_;//me123
#if VU_SIMPLE_LATENCY
  protected int timeDelta_;
  protected int latency_;
#endif //VU_SIMPLE_LATENCY;
#if VU_TRACK_LATENCY
  protected VU_BYTE timeSyncState_;
  protected VU_TIME latency_;
  protected VU_TIME masterTime_;		// time from master
  protected VU_TIME masterTimePostTime_;	// local time of net msg post
  protected VU_TIME responseTime_;		// local time local msg post
  protected VU_SESSION_ID masterTimeOwner_;	// sender of master msg 
  // local data
  // time synchro statistics
  protected VU_TIME lagTotal_;
  protected int lagPackets_;
  protected int lagUpdate_;	// when packets > update, change latency value
#endif //VU_TRACK_LATENCY
        // msg tracking
        protected int lastMsgRecvd_;
#if VU_MAX_SESSION_CAMERAS
  protected VU_BYTE cameraCount_;
  protected VU_ID cameras_[VU_MAX_SESSION_CAMERAS];
#endif
        // local data
        public VuGameEntity game_;
        protected VU_GAME_ACTION action_;
    }


    public static class VuSessionEntityEncodingLE
    {
        public static void Encode(Stream stream, VuSessionEntity val)
        {
            VuTargetEntityEncodingLE.Encode(stream, val);
            UInt64EncodingLE.Encode(stream, val.lastCollisionCheckTime_);
            VU_SESSION_IDEncodingLE.Encode(stream, val.sessionId_);
            UInt64EncodingLE.Encode(stream, val.domainMask_);
            stream.WriteByte(val.loadMetric_);
            VU_IDEncodingLE.Encode(stream, val.gameId_);
            stream.WriteByte(val.groupCount_);

            VuGroupNode gnode = val.groupHead_;

            while (gnode != null)
            {
                VU_IDEncodingLE.Encode(stream, gnode.gid_);
                gnode = gnode.next_;
            }
            Int32EncodingLE.Encode(stream, val.bandwidth_);

#if VU_SIMPLE_LATENCY
  memcpy(*stream, &vuxRealTime, sizeof(VU_TIME)); *stream += sizeof(VU_TIME);
  memcpy(*stream, &vuxGameTime, sizeof(VU_TIME)); *stream += sizeof(VU_TIME);
#endif

#if VU_TRACK_LATENCY
  if (TimeSyncState() == VU_MASTER_SYNC) {
    masterTime_ = vuxRealTime;
  }
  responseTime_ = vuxRealTime;
  memcpy(*stream, &timeSyncState_,      sizeof(timeSyncState_));      *stream += sizeof(timeSyncState_);
  memcpy(*stream, &latency_,            sizeof(latency_));            *stream += sizeof(latency_);
  memcpy(*stream, &masterTime_,         sizeof(masterTime_));         *stream += sizeof(masterTime_);
  memcpy(*stream, &masterTimePostTime_, sizeof(masterTimePostTime_)); *stream += sizeof(masterTimePostTime_);
  memcpy(*stream, &responseTime_,       sizeof(responseTime_));       *stream += sizeof(responseTime_);
  memcpy(*stream, &masterTimeOwner_,    sizeof(masterTimeOwner_));    *stream += sizeof(masterTimeOwner_);
#endif
            StringEncodingLE.Encode(stream, val.callsign_);

#if VU_MAX_SESSION_CAMERAS 
  memcpy(*stream, &cameraCount_, sizeof(cameraCount_)); *stream += sizeof(cameraCount_);
  if (cameraCount_ > 0 && cameraCount_ <= VU_MAX_SESSION_CAMERAS) {
    memcpy(*stream, &cameras_, sizeof(VU_ID) * cameraCount_); *stream += sizeof(VU_ID) * cameraCount_;
  }
#else
            Int32EncodingLE.Encode(stream, 0);
#endif
        }

        public static void Decode(Stream stream, ref VuSessionEntity rst)
        {
            throw new NotImplementedException();
        }

        public static int Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }

    }


    //-----------------------------------------
    public class VuGroupEntity : VuTargetEntity
    {
        public const string VU_GAME_GROUP_NAME = "Vu2 Game";
        public const int VU_DEFAULT_GROUP_SIZE = 6;
        // constructors & destructor
        public VuGroupEntity() { }

        public VuGroupEntity(string groupname)
            : base(VU_GROUP_ENTITY_TYPE, VU_ID.VU_SESSION_ENTITY_ID)
        {
            sessionMax_ = VU_DEFAULT_GROUP_SIZE;
            groupName_ = groupname;
            VuSessionFilter filter = new VuSessionFilter(Id());
            sessionCollection_ = new VuOrderedList(filter);
#if TODO
		            sessionCollection_.Init();  
#endif
            throw new NotImplementedException();
        }
        //TODO public virtual ~VuGroupEntity();

        public string GroupName() { return groupName_; }
        public virtual ushort MaxSessions() { return sessionMax_; }
        public virtual int SessionCount() { return sessionCollection_.Count(); }

        // setters
        public void SetGroupName(string groupname)
        {
            groupName_ = groupname;
            if (IsLocal())
            {
                VuSessionEvent evnt =
              new VuSessionEvent(this, vuEventTypes.VU_SESSION_CHANGE_CALLSIGN, VUSTATIC.vuGlobalGroup);
                VuMessageQueue.PostVuMessage(evnt);
            }
        }

        public virtual void SetMaxSessions(ushort max) { sessionMax_ = max; }

        public override VU_BOOL HasTarget(VU_ID id)
        {
            if (id == Id())
            {
                return true;
            }
            VuSessionEntity session = (VuSessionEntity)VUSTATIC.vuDatabase.Find(id);
            if (session != null && session.IsSession())
            {
                return SessionInGroup(session);
            }
            return false;
        }

        public override VU_BOOL InTarget(VU_ID id)
        {
            // supports one level of group nesting: groups are In global group
            if (id == Id() || id == VUSTATIC.vuGlobalGroup.Id())
            {
                return true;
            }
            return false;
        }

        public VU_BOOL SessionInGroup(VuSessionEntity session)
        {
            VuSessionsIterator iter = new VuSessionsIterator(this);
            VuEntity ent = iter.GetFirst();
            while (ent != null)
            {
                if (ent == session)
                {
                    return true;
                }
                ent = iter.GetNext();
            }
            return false;
        }

        public virtual VU_ERRCODE AddSession(VuSessionEntity session)
        {
            short count = (short)(sessionCollection_.Count());
            if (count >= sessionMax_)
            {
                return VU_ERRCODE.VU_ERROR;
            }

#if VU_USE_COMMS
  VuxGroupAddSession(this, session);
#endif

#if TODO
		            if (sessionCollection_.Find(session.Id()) == null)
            {
                return sessionCollection_.Insert(session);
            }  
#endif
            throw new NotImplementedException();
            return VU_ERRCODE.VU_NO_OP;
        }

        public VU_ERRCODE AddSession(VU_ID sessionId)
        {
            VuSessionEntity session = (VuSessionEntity)VUSTATIC.vuDatabase.Find(sessionId);
            if (session != null && session.IsSession())
            {
                return AddSession(session);
            }
            return VU_ERRCODE.VU_ERROR;
        }


        public virtual VU_ERRCODE RemoveSession(VuSessionEntity session)
        {
#if VU_USE_COMMS
  if (session != vuLocalSessionEntity) {
    VuxGroupRemoveSession(this, session);
  }
#endif

            return sessionCollection_.Remove(session);
        }

        public VU_ERRCODE RemoveSession(VU_ID sessionId)
        {
            VuSessionEntity session = (VuSessionEntity)VUSTATIC.vuDatabase.Find(sessionId);
            if (session != null && session.IsSession())
            {
                return RemoveSession(session);
            }
            return VU_ERRCODE.VU_ERROR;
        }


        public override VU_BOOL IsGroup()
        {
            return true;
        }

        // evnt Handlers
#if VU_LOW_WARNING_VERSION
  public virtual VU_ERRCODE Handle(VuErrorMessage *error);
  public virtual VU_ERRCODE Handle(VuPushRequest *msg);
  public virtual VU_ERRCODE Handle(VuPullRequest *msg);
  public virtual VU_ERRCODE Handle(VuEvent *evnt);
  public virtual VU_ERRCODE Handle(VuFullUpdateEvent *evnt);
  public virtual VU_ERRCODE Handle(VuPositionUpdateEvent *evnt);
  public virtual VU_ERRCODE Handle(VuEntityCollisionEvent *evnt);
  public virtual VU_ERRCODE Handle(VuTransferEvent *evnt);
  public virtual VU_ERRCODE Handle(VuSessionEvent *evnt);
#else
        public override VU_ERRCODE Handle(VuSessionEvent evnt)
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;
            switch (evnt.subtype_)
            {
                case vuEventTypes.VU_SESSION_CHANGE_CALLSIGN:
                    SetGroupName(evnt.callsign_);
                    retval = VU_ERRCODE.VU_SUCCESS;
                    break;
                case vuEventTypes.VU_SESSION_DISTRIBUTE_ENTITIES:
                    retval = Distribute(null);
                    break;
                default:
                    break;
            }
            return retval;
        }

        public override VU_ERRCODE Handle(VuFullUpdateEvent evnt)
        {
            // update groups?
            return base.Handle(evnt);
        }

#endif //VU_LOW_WARNING_VERSION


        protected VuGroupEntity(ushort type, string groupname, VuFilter filter = null)
            : base(type, VUSTATIC.VuxGetId())
        {
            sessionMax_ = VU_DEFAULT_GROUP_SIZE;
            groupName_ = groupname;
            VuSessionFilter sfilter = new VuSessionFilter(Id());

            if (filter == null) filter = sfilter;

            sessionCollection_ = new VuOrderedList(filter);
            sessionCollection_.Register();  
        }

        public virtual VU_ERRCODE Distribute(VuSessionEntity ent)
        {
            // do nothing
            return VU_ERRCODE.VU_NO_OP;
        }

        internal override VU_ERRCODE InsertionCallback()
        {
            if (this != VUSTATIC.vuGlobalGroup)
            {
#if VU_USE_COMMS
                VuxGroupConnect(this);
#endif
                VuSessionsIterator iter = new VuSessionsIterator(this);
                VuSessionEntity sess = iter.GetFirst();
                while (sess != null)
                {
                    AddSession(sess);
                    sess = iter.GetNext();
                }
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }
        internal override VU_ERRCODE RemovalCallback()
        {
            VuSessionsIterator iter = new VuSessionsIterator(this);
            VuSessionEntity sess = iter.GetFirst();

            while (sess != null)
            {
                RemoveSession(sess);
                sess = iter.GetNext();
            }
#if VU_USE_COMMS
            VuxGroupDisconnect(this);
#endif
            return VU_ERRCODE.VU_SUCCESS;
        }

        // DATA
        protected string groupName_;
        protected ushort sessionMax_;
        internal protected VuOrderedList sessionCollection_;

        // scratch data
        protected int selfIndex_;
    }

    public static class VuGroupEntityEncodingLE
    {

        public static void Encode(Stream stream, VuGroupEntity val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ref VuGroupEntity rst)
        {
            throw new NotImplementedException();
        }

        public static int Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    //-----------------------------------------
    public class VuGameEntity : VuGroupEntity
    {
        public VuGameEntity() { }

        // constructors & destructor
        public VuGameEntity(ulong domainMask, string gamename)
            : base(VuEntity.VU_GAME_ENTITY_TYPE, VU_GAME_GROUP_NAME)
        {
            domainMask_ = domainMask;
            gameName_ = gamename;
        }

        //TODO public virtual ~VuGameEntity();

        // accessors
        public ulong DomainMask() { return domainMask_; }
        public string GameName() { return gameName_; }
        public override ushort MaxSessions() { return sessionMax_; }
        public override int SessionCount() { return sessionCollection_.Count(); }

        // setters
        public void SetGameName(string gamename)
        {
            gameName_ = gamename;
            if (IsLocal())
            {
                VuSessionEvent evnt = new VuSessionEvent(this, vuEventTypes.VU_SESSION_CHANGE_CALLSIGN, VUSTATIC.vuGlobalGroup);
                VuMessageQueue.PostVuMessage(evnt);
            }
        }
        public override void SetMaxSessions(ushort max) { sessionMax_ = max; }

        public override VU_ERRCODE AddSession(VuSessionEntity session)
        {
            short count = (short)(sessionCollection_.Count());
            if (count >= sessionMax_)
            {
                return VU_ERRCODE.VU_ERROR;
            }

#if VU_USE_COMMS
  if (session.IsLocal()) {
    // connect to all sessions in the group
    VuSessionsIterator siter(this);
    for (VuSessionEntity s = siter.GetFirst(); s; s = siter.GetNext()) {
      if (s != vuLocalSessionEntity) {
        VuxSessionConnect(s);
      }
    }
  } else if (vuLocalSessionEntity.Game() == this) {
    // connect to particular session
    VuxSessionConnect(session);
  }
  VuxGroupAddSession(this, session);
#endif

#if TODO
		            if (sessionCollection_.Find(session.Id()) == null)
            {
                return sessionCollection_.Insert(session);
            }  
#endif
            throw new NotImplementedException();
            return VU_ERRCODE.VU_NO_OP;
        }
        public override VU_ERRCODE RemoveSession(VuSessionEntity session)
        {
#if VU_USE_COMMS
  if (session.IsLocal()) {
    // disconnect all sessions
    VuSessionsIterator siter(this);
    for (VuSessionEntity *s = siter.GetFirst(); s; s = siter.GetNext()) {
      VuxSessionDisconnect(s);
    }
//    VuxGroupDisconnect(this);
  } else {
    // just disconnect from particular session
    VuxSessionDisconnect(session);
  }
  VuxGroupRemoveSession(this, session);
#endif

            return sessionCollection_.Remove(session);
        }

        public override VU_BOOL IsGame()
        {
            return true;
        }

        // evnt Handlers
#if VU_LOW_WARNING_VERSION
  public virtual VU_ERRCODE Handle(VuErrorMessage *error);
  public virtual VU_ERRCODE Handle(VuPushRequest *msg);
  public virtual VU_ERRCODE Handle(VuPullRequest *msg);
  public virtual VU_ERRCODE Handle(VuEvent *evnt);
  public virtual VU_ERRCODE Handle(VuFullUpdateEvent *evnt);
  public virtual VU_ERRCODE Handle(VuPositionUpdateEvent *evnt);
  public virtual VU_ERRCODE Handle(VuEntityCollisionEvent *evnt);
  public virtual VU_ERRCODE Handle(VuTransferEvent *evnt);
  public virtual VU_ERRCODE Handle(VuSessionEvent *evnt);
#else
        public override VU_ERRCODE Handle(VuSessionEvent evnt)
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;
            switch (evnt.subtype_)
            {
                case vuEventTypes.VU_SESSION_CHANGE_CALLSIGN:
                    SetGameName(evnt.callsign_);
                    retval = VU_ERRCODE.VU_SUCCESS;
                    break;
                case vuEventTypes.VU_SESSION_DISTRIBUTE_ENTITIES:
                    retval = Distribute(null);
                    break;
                default:
                    break;
            }
            return retval;
        }

        public override VU_ERRCODE Handle(VuFullUpdateEvent evnt)
        {
            return base.Handle(evnt);
        }

#endif //VU_LOW_WARNING_VERSION


        protected VuGameEntity(ushort type, ulong domainMask, string gamename, string groupname)
            : base(type, groupname)
        {
            domainMask_ = domainMask;
            gameName_ = gamename;
        }

        public override VU_ERRCODE Distribute(VuSessionEntity sess)
        {
            if (sess == null || (Id() == sess.GameId()))
            {
                int i;
                ulong[] count = new ulong[32];
                int totalcount = 0;
                int[] myseedlower = new int[32];
                int[] myseedupper = new int[32];
                for (i = 0; i < 32; i++)
                {
                    count[i] = 0;
                    myseedlower[i] = 0;
                    myseedupper[i] = VUSTATIC.vuLocalSessionEntity.LoadMetric() - 1;
                }

                VuSessionsIterator siter = new VuSessionsIterator(this);
                VuSessionEntity cs = siter.GetFirst();
                while (cs != null)
                {
                    if (sess == null || sess.Id() != cs.Id())
                    {
                        for (i = 0; i < 32; i++)
                        {
                            if (((ulong)(1 << i) & cs.DomainMask()) != 0)
                            {
                                count[i] += cs.LoadMetric();
                                totalcount++;
                            }
                        }
                    }
                    cs = siter.GetNext();
                }
                if (totalcount == 0)
                {
                    // nothing to do!  just return...
                    return VU_ERRCODE.VU_NO_OP;
                }
                cs = siter.GetFirst();
                while (cs != null && cs != VUSTATIC.vuLocalSessionEntity)
                {
                    if (sess == null || sess.Id() != cs.Id())
                    {
                        for (i = 0; i < 32; i++)
                        {
                            if (((ulong)(1 << i) & cs.DomainMask()) != 0)
                            {
                                myseedlower[i] += cs.LoadMetric();
                                myseedupper[i] += cs.LoadMetric();
                            }
                        }
                    }
                    cs = siter.GetNext();
                }
                if (cs == null)
                {
                    if (sess == null)
                    {
                        // note: something is terribly amiss... so bail...
                        return VU_ERRCODE.VU_ERROR;
                    }
                    else
                    {
                        // ... most likely, we did not find any viable sessions
                        VuDatabaseIterator dbiter = new VuDatabaseIterator();
                        VuEntity ent = dbiter.GetFirst();
                        while (ent != null)
                        {
                            if ((ent.OwnerId().creator_ == sess.SessionId()) && (sess != ent) &&
                                (sess.VuState() != VU_MEM_STATE.VU_MEM_ACTIVE ||
                                 (ent.IsTransferrable() && !ent.IsGlobal())))
                            {
                                VUSTATIC.vuDatabase.Remove(ent);
                            }
                            ent = dbiter.GetNext();
                        }
                    }
                }
                else if (Id() == VUSTATIC.vuLocalSessionEntity.GameId())
                {
                    VuDatabaseIterator dbiter = new VuDatabaseIterator();
                    VuEntity ent = dbiter.GetFirst();
                    VuEntity test_ent = ent;
                    VuEntity ent2;
                    int index;
                    while (ent != null)
                    {
                        if (!ent.IsTransferrable() &&
                            sess != null && sess.VuState() != VU_MEM_STATE.VU_MEM_ACTIVE &&
                            ent.OwnerId().creator_ == sess.SessionId())
                        {
                            VUSTATIC.vuDatabase.Remove(ent);
                        }
                        else if ((sess == null || (ent.OwnerId().creator_ == sess.SessionId())) &&
                                 ((ulong)(1 << ent.Domain()) & VUSTATIC.vuLocalSessionEntity.DomainMask()) != 0)
                        {
                            if (ent.Association() != VU_ID.vuNullId &&
                                (ent2 = VUSTATIC.vuDatabase.Find(ent.Association())) != null)
                            {
                                test_ent = ent2;
                            }
                            index = (int)(test_ent.Id().num_ % count[test_ent.Domain()]);
                            if (index >= myseedlower[test_ent.Domain()] &&
                                index <= myseedupper[test_ent.Domain()] &&
                                ent.IsTransferrable() && !ent.IsGlobal())
                            {
                                ent.SetOwnerId(VUSTATIC.vuLocalSession);
                            }
                        }
                        ent = dbiter.GetNext();
                        test_ent = ent;
                    }
                }
                else
                {
                    VuListIterator grpiter = new VuListIterator(VUSTATIC.vuGameList);
                    VuEntity ent = grpiter.GetFirst();
                    VuEntity test_ent = ent;
                    VuEntity ent2;
                    int index;
                    while (ent != null)
                    {
                        if (!ent.IsTransferrable() &&
                            sess != null && sess.VuState() != VU_MEM_STATE.VU_MEM_ACTIVE &&
                            ent.OwnerId().creator_ == sess.SessionId())
                        {
                            VUSTATIC.vuDatabase.Remove(ent);
                        }
                        else if ((sess == null || (ent.OwnerId().creator_ == sess.SessionId())) &&
                                 ((ulong)(1 << ent.Domain()) & VUSTATIC.vuLocalSessionEntity.DomainMask()) != 0)
                        {
                            if (ent.Association() != VU_ID.vuNullId &&
                                (ent2 = VUSTATIC.vuDatabase.Find(ent.Association())) != null)
                            {
                                test_ent = ent2;
                            }
                            index = (int)(test_ent.Id().num_ % count[test_ent.Domain()]);
                            if (index >= myseedlower[test_ent.Domain()] &&
                                index <= myseedupper[test_ent.Domain()] &&
                                ent.IsTransferrable() && !ent.IsGlobal() &&
                                (sess == null || sess.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE))
                            {
                                ent.SetOwnerId(VUSTATIC.vuLocalSession);
                            }
                        }
                        ent = grpiter.GetNext();
                        test_ent = ent;
                    }
                }
            }
            return VU_ERRCODE.VU_SUCCESS;
        }

        internal override VU_ERRCODE RemovalCallback()
        {
            // take care of sessions we still think are in this game...
            VuSessionsIterator iter = new VuSessionsIterator(this);
            VuSessionEntity sess = iter.GetFirst();
            while (sess != null && sess.Game() == this)
            {
                sess.JoinGame(VUSTATIC.vuPlayerPoolGroup);
                sess = iter.GetNext();
            }
#if VU_USE_COMMS
            VuxGroupDisconnect(this);
#endif
            return VU_ERRCODE.VU_SUCCESS;
        }

        // DATA
        // shared data
        protected ulong domainMask_;
        protected string gameName_;
    }

    public static class VuGameEntityEncodingLE
    {
        public static void Encode(Stream stream, VuGameEntity val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ref VuGameEntity rst)
        {
            throw new NotImplementedException();
        }

        public static int Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    //-----------------------------------------
    public class VuPlayerPoolGame : VuGameEntity
    {
        public const string VU_PLAYER_POOL_GROUP_NAME = "Player Pool";
        // constructors & destructor
        public VuPlayerPoolGame(ulong domainMask)
            : base(VU_PLAYER_POOL_GROUP_ENTITY_TYPE, domainMask,
      VU_PLAYER_POOL_GROUP_NAME, VUSTATIC.vuxWorldName)
        {
            // make certain owner is NULL session
            share_.ownerId_ = VU_ID.vuNullId;
            share_.id_.creator_ = 0;
            share_.id_.num_ = VU_ID.VU_PLAYER_POOL_ENTITY_ID;
            sessionMax_ = 255;

            // hack, hack, hack up a lung
#if TODO
		            sessionCollection_.DeInit();
            //delete sessionCollection_;
            VuSessionFilter filter = new VuSessionFilter(Id());
            sessionCollection_ = new VuOrderedList(filter);
            sessionCollection_.Init();  
#endif
            throw new NotImplementedException();
        }

        //TODO public virtual ~VuPlayerPoolGame();

        // do nothing...
        public override VU_ERRCODE Distribute(VuSessionEntity sess)
        {
            // just remove all ents managed by session
            if (sess != null && (Id() == sess.GameId()))
            {
                VuDatabaseIterator dbiter = new VuDatabaseIterator();
                VuEntity ent = dbiter.GetFirst();
                while (ent != null)
                {
                    if ((ent.OwnerId().creator_ == sess.SessionId()) &&
                        (!ent.IsPersistent()) &&
                        (sess.VuState() != VU_MEM_STATE.VU_MEM_ACTIVE ||
                         (ent.IsTransferrable() && !ent.IsGlobal())))
                    {
                        VUSTATIC.vuDatabase.Remove(ent);
                    }
                    ent = dbiter.GetNext();
                }
            }
            return VU_ERRCODE.VU_SUCCESS;
        }

    }

    public class GlobalGroupFilter : VuFilter
    {
        public GlobalGroupFilter()
            : base()
        {
            // empty
        }

        //TODO public virtual ~GlobalGroupFilter();

        public override VU_BOOL Test(VuEntity ent)
        {
            return ent.IsSession();
        }

        public override VU_BOOL RemoveTest(VuEntity ent)
        {
            return ent.IsSession();
        }

        public override int Compare(VuEntity ent1, VuEntity ent2)
        {
            if ((VU_KEY)ent2.Id() > (VU_KEY)ent1.Id())
            {
                return -1;
            }
            else if ((VU_KEY)ent2.Id() < (VU_KEY)ent1.Id())
            {
                return 1;
            }
            return 0;
        }

        public override VuFilter Copy()
        {
            return new GlobalGroupFilter(this);
        }

        protected GlobalGroupFilter(GlobalGroupFilter other)
            : base(other)
        {
            // empty
        }

    }
    //-----------------------------------------
    public class VuGlobalGroup : VuGroupEntity
    {
        public static GlobalGroupFilter globalGrpFilter = new GlobalGroupFilter();

        // constructors & destructor
        public VuGlobalGroup()
            : base(VuEntity.VU_GLOBAL_GROUP_ENTITY_TYPE, VUSTATIC.vuxWorldName, globalGrpFilter)
        {
            // make certain owner is NULL session
            share_.ownerId_ = VU_ID.vuNullId;
            share_.id_.creator_ = 0;
            share_.id_.num_ = VU_ID.VU_GLOBAL_GROUP_ENTITY_ID;
            sessionMax_ = 1024;
            connected_ = false;
        }

        //TODO public virtual ~VuGlobalGroup();

        public override VU_BOOL HasTarget(VU_ID id)          // always returns true
        {
            // global group includes everybody
            return true;
        }

        public VU_BOOL Connected() { return connected_; }
        public void SetConnected(VU_BOOL conn) { connected_ = conn; }

        // DATA
        protected VU_BOOL connected_;
    }

    //-----------------------------------------------------------------------------
    public class VuSessionFilter : VuFilter
    {

        public VuSessionFilter(VU_ID groupId)
            : base()
        {
            groupId_ = groupId;
        }
        //TODO public virtual ~VuSessionFilter();

        public override VU_BOOL Test(VuEntity ent)
        {
            return ((ent.IsSession() && ((VuSessionEntity)ent).GameId() == groupId_) ? true : false);
        }

        public override VU_BOOL RemoveTest(VuEntity ent)
        {
            return ent.IsSession();
        }

        public override int Compare(VuEntity ent1, VuEntity ent2)
        {
            if ((VU_KEY)ent2.Id() > (VU_KEY)ent1.Id())
            {
                return -1;
            }
            else if ((VU_KEY)ent2.Id() < (VU_KEY)ent1.Id())
            {
                return 1;
            }
            return 0;
        }

        public override VU_BOOL Notice(VuMessage evnt)
        {
            // danm_TBD: do we need VU_FULL_UPDATE event as well?
            if (((1 << (int)evnt.Type()) & (int)VU_MSG_DEF.VU_TRANSFER_EVENT) != 0)
            {
                return true;
            }
            return false;
        }

        public override VuFilter Copy()
        {
            return new VuSessionFilter(this);
        }

        protected VuSessionFilter(VuSessionFilter other)
            : base(other)
        {
            groupId_ = other.groupId_;
        }


        // DATA
        protected VU_ID groupId_;
    }

    //-----------------------------------------------------------------------------
    public class VuSessionsIterator : VuIterator
    {
        public VuSessionsIterator(VuGroupEntity group = null)
            : base(group != null ? group.sessionCollection_ :
          (VUSTATIC.vuLocalSessionEntity.Game() != null ?
           VUSTATIC.vuLocalSessionEntity.Game().sessionCollection_ : null))
        {
            throw new NotImplementedException();
        }

        //TODO public virtual ~VuSessionsIterator();

        public VuSessionEntity GetFirst()
        {
            throw new NotImplementedException();
        }

        public VuSessionEntity GetNext()
        {
            throw new NotImplementedException();
        }

        public override VuEntity CurrEnt()
        {
            throw new NotImplementedException();
        }

        //public override VU_BOOL IsReferenced(VuEntity ent)
        //{
        //    throw new NotImplementedException();
        //}

        public override VU_ERRCODE Cleanup()
        {
            throw new NotImplementedException();
        }
    }
}
