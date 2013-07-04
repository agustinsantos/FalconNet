using FalconNet.Common.Encoding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VU_BOOL = System.Boolean;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;

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

        // virtual function interface
        public override int SaveSize() { throw new NotImplementedException(); }
        public override int Save(ByteWrapper buffer) { throw new NotImplementedException(); }
        public override int Save(Stream file) { throw new NotImplementedException(); }

        // Special VU type getters
        public virtual VU_BOOL IsTarget() { throw new NotImplementedException(); }	// returns TRUE

        public abstract VU_BOOL HasTarget(VU_ID id); // TRUE -. id contains (or is) ent
        public abstract VU_BOOL InTarget(VU_ID id);  // TRUE -. ent contained by (or is) id

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


        protected VuTargetEntity(int type) { throw new NotImplementedException(); }
        protected VuTargetEntity(ByteWrapper buffer) { throw new NotImplementedException(); }
        protected VuTargetEntity(Stream file) { throw new NotImplementedException(); }


        private int LocalSize() { throw new NotImplementedException(); }                      // returns local bytes written

        //data

#if VU_USE_COMMS
  VuCommsContext bestEffortComms_;
  VuCommsContext reliableComms_;
#endif
        protected int dirty;
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
        // constructors & destructor
        public VuSessionEntity(ulong domainMask, string callsign) : base(0) { throw new NotImplementedException(); }
        public VuSessionEntity(ByteWrapper buf) : base(buf) { throw new NotImplementedException(); }
        public VuSessionEntity(Stream file) : base(file) { throw new NotImplementedException(); }
        //TODO public virtual ~VuSessionEntity();

        // virtual function interface
        public override int SaveSize() { throw new NotImplementedException(); }
        public override int Save(ByteWrapper buf) { throw new NotImplementedException(); }
        public override int Save(Stream file) { throw new NotImplementedException(); }

        // accessors
        public ulong DomainMask() { return domainMask_; }
        public VU_SESSION_ID SessionId() { return sessionId_; }
        public string Callsign() { return callsign_; }
        public VU_BYTE LoadMetric() { return loadMetric_; }
        public VU_ID GameId() { return gameId_; }
        public VuGameEntity Game() { throw new NotImplementedException(); }
        public int LastMessageReceived() { return lastMsgRecvd_; }
        public VU_TIME KeepaliveTime() { throw new NotImplementedException(); }
        public int BandWidth() { return bandwidth_; }//me123

        // setters
        public void SetCallsign(string callsign) { throw new NotImplementedException(); }
        public void SetLoadMetric(VU_BYTE newMetric) { loadMetric_ = newMetric; }
        public VU_ERRCODE JoinGroup(VuGroupEntity newgroup)  //  < 0 retval ==> failure
        { throw new NotImplementedException(); }
        public VU_ERRCODE LeaveGroup(VuGroupEntity group) { throw new NotImplementedException(); }
        public VU_ERRCODE LeaveAllGroups() { throw new NotImplementedException(); }
        public VU_ERRCODE JoinGame(VuGameEntity newgame)  //  < 0 retval ==> failure
        { throw new NotImplementedException(); }
        public VU_GAME_ACTION GameAction() { return action_; }
        public void SetLastMessageReceived(int id) { lastMsgRecvd_ = id; }
        public void SetKeepaliveTime(VU_TIME ts) { throw new NotImplementedException(); }
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
        public virtual VU_BOOL IsSession()	// returns TRUE
        { throw new NotImplementedException(); }
        public override VU_BOOL HasTarget(VU_ID id)  // TRUE -. id contains (or is) ent
        { throw new NotImplementedException(); }

        public override VU_BOOL InTarget(VU_ID id)   // TRUE -. ent contained by (or is) id
        { throw new NotImplementedException(); }

        public VU_ERRCODE AddGroup(VU_ID gid) { throw new NotImplementedException(); }
        public VU_ERRCODE RemoveGroup(VU_ID gid) { throw new NotImplementedException(); }
        public VU_ERRCODE PurgeGroups() { throw new NotImplementedException(); }

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
        public override VU_ERRCODE Handle(VuEvent evnt) { throw new NotImplementedException(); }
        public override VU_ERRCODE Handle(VuFullUpdateEvent evnt) { throw new NotImplementedException(); }
        public override VU_ERRCODE Handle(VuSessionEvent evnt) { throw new NotImplementedException(); }
#endif //VU_LOW_WARNING_VERSION


        protected VuSessionEntity(int typeindex, ulong domainMask, string callsign) : base(typeindex) { throw new NotImplementedException(); }
        protected VU_SESSION_ID OpenSession()	// returns session id
        { throw new NotImplementedException(); }
        protected void CloseSession() { throw new NotImplementedException(); }
        internal override VU_ERRCODE InsertionCallback() { throw new NotImplementedException(); }
        internal override VU_ERRCODE RemovalCallback() { throw new NotImplementedException(); }


        private int LocalSize()                    // returns local bytes written
        { throw new NotImplementedException(); }

        // DATA

        // shared data
        protected VU_SESSION_ID sessionId_;
        protected ulong domainMask_;
        protected string callsign_;
        protected VU_BYTE loadMetric_;
        protected VU_ID gameId_;
        protected VU_BYTE groupCount_;
        protected VuGroupNode groupHead_;
        protected int bandwidth_;//me123
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
        protected VuGameEntity game_;
        protected VU_GAME_ACTION action_;
    }

    //-----------------------------------------
    public class VuGroupEntity : VuTargetEntity
    {
        public const string VU_GAME_GROUP_NAME = "Vu2 Game";
        public const int VU_DEFAULT_GROUP_SIZE = 6;
        // constructors & destructor
        public VuGroupEntity(string groupname)
            : base(VU_GROUP_ENTITY_TYPE)
        {
            sessionMax_ = VU_DEFAULT_GROUP_SIZE;
            groupName_ = groupname;
            VuSessionFilter filter = new VuSessionFilter(Id());
            sessionCollection_ = new VuOrderedList(filter);
            sessionCollection_.Init();
        }
        public VuGroupEntity(ByteWrapper buf) : base(buf) { throw new NotImplementedException(); }
        public VuGroupEntity(Stream file) : base(file) { throw new NotImplementedException(); }
        //TODO public virtual ~VuGroupEntity();

        // virtual function interface
        public override int SaveSize() { throw new NotImplementedException(); }
        public override int Save(ByteWrapper buf) { throw new NotImplementedException(); }
        public override int Save(Stream file) { throw new NotImplementedException(); }

        public string GroupName() { return groupName_; }
        public virtual ushort MaxSessions() { return sessionMax_; }
        public virtual int SessionCount() { return sessionCollection_.Count(); }

        // setters
        public void SetGroupName(string groupname) { throw new NotImplementedException(); }
        public void SetMaxSessions(ushort max) { sessionMax_ = max; }

        public override VU_BOOL HasTarget(VU_ID id) { throw new NotImplementedException(); }
        public override VU_BOOL InTarget(VU_ID id) { throw new NotImplementedException(); }
        public VU_BOOL SessionInGroup(VuSessionEntity session) { throw new NotImplementedException(); }
        public virtual VU_ERRCODE AddSession(VuSessionEntity session) { throw new NotImplementedException(); }
        public VU_ERRCODE AddSession(VU_ID sessionId) { throw new NotImplementedException(); }
        public virtual VU_ERRCODE RemoveSession(VuSessionEntity session) { throw new NotImplementedException(); }
        public VU_ERRCODE RemoveSession(VU_ID sessionId) { throw new NotImplementedException(); }

        public virtual VU_BOOL IsGroup()	// returns TRUE
        { throw new NotImplementedException(); }
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
        public override VU_ERRCODE Handle(VuSessionEvent evnt) { throw new NotImplementedException(); }
        public override VU_ERRCODE Handle(VuFullUpdateEvent evnt) { throw new NotImplementedException(); }
#endif //VU_LOW_WARNING_VERSION


        protected VuGroupEntity(int type, string groupname, VuFilter filter = null)
            : base(type)
        {
            sessionMax_ = VU_DEFAULT_GROUP_SIZE;
            groupName_ = groupname;
            VuSessionFilter sfilter = new VuSessionFilter(Id());

            if (filter == null) filter = sfilter;

            sessionCollection_ = new VuOrderedList(filter);
            sessionCollection_.Init();
        }

        protected virtual VU_ERRCODE Distribute(VuSessionEntity ent) { throw new NotImplementedException(); }
        internal override VU_ERRCODE InsertionCallback() { throw new NotImplementedException(); }
        internal override VU_ERRCODE RemovalCallback() { throw new NotImplementedException(); }

        private int LocalSize()                       // returns local bytes written
        { throw new NotImplementedException(); }

        // DATA
        protected string groupName_;
        protected ushort sessionMax_;
        internal VuOrderedList sessionCollection_;

        // scratch data
        protected int selfIndex_;
    }

    //-----------------------------------------
    public class VuGameEntity : VuGroupEntity
    {
        // constructors & destructor
        public VuGameEntity(ulong domainMask, string gamename)
            : base(VuEntity.VU_GAME_ENTITY_TYPE, VU_GAME_GROUP_NAME)
        {
            domainMask_ = domainMask;
            gameName_ = gamename;
        }

        public VuGameEntity(ByteWrapper buf)
            : base(buf) { throw new NotImplementedException(); }

        public VuGameEntity(Stream file)
            : base(file) { throw new NotImplementedException(); }

        //TODO public virtual ~VuGameEntity();

        // virtual function interface
        public override int SaveSize() { throw new NotImplementedException(); }
        public override int Save(ByteWrapper buf) { throw new NotImplementedException(); }
        public override int Save(Stream file) { throw new NotImplementedException(); }

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
        public void SetMaxSessions(ushort max) { sessionMax_ = max; }

        public virtual VU_ERRCODE AddSession(VuSessionEntity session) { throw new NotImplementedException(); }
        public virtual VU_ERRCODE RemoveSession(VuSessionEntity session) { throw new NotImplementedException(); }

        public virtual VU_BOOL IsGame()
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
        public virtual VU_ERRCODE Handle(VuSessionEvent evnt) { throw new NotImplementedException(); }
        public virtual VU_ERRCODE Handle(VuFullUpdateEvent evnt) { throw new NotImplementedException(); }
#endif //VU_LOW_WARNING_VERSION


        protected VuGameEntity(int type, ulong domainMask, string gamename, string groupname)
            : base(type, groupname)
        {
            domainMask_ = domainMask;
            gameName_ = gamename;
        }
        protected virtual VU_ERRCODE Distribute(VuSessionEntity ent) { throw new NotImplementedException(); }
        protected virtual VU_ERRCODE RemovalCallback() { throw new NotImplementedException(); }

        private int LocalSize()                      // returns local bytes written
        { throw new NotImplementedException(); }

        // DATA
        // shared data
        protected ulong domainMask_;
        protected string gameName_;
    }

    //-----------------------------------------
    public class VuPlayerPoolGame : VuGameEntity
    {
        public const int VU_PLAYER_POOL_ENTITY_ID = 2;
        public const string VU_PLAYER_POOL_GROUP_NAME = "Player Pool";
        // constructors & destructor
        public VuPlayerPoolGame(ulong domainMask)
            : base(VU_PLAYER_POOL_GROUP_ENTITY_TYPE, domainMask,
      VU_PLAYER_POOL_GROUP_NAME, VUSTATIC.vuxWorldName)
        {
            // make certain owner is NULL session
            share_.ownerId_ = VU_ID.vuNullId;
            share_.id_.creator_ = 0;
            share_.id_.num_ = VU_PLAYER_POOL_ENTITY_ID;
            sessionMax_ = 255;

            // hack, hack, hack up a lung
            sessionCollection_.DeInit();
            //delete sessionCollection_;
            VuSessionFilter filter = new VuSessionFilter(Id());
            sessionCollection_ = new VuOrderedList(filter);
            sessionCollection_.Init();
        }

        //TODO public virtual ~VuPlayerPoolGame();

        // virtual function interface	-- stubbed out here
        public virtual int SaveSize() { throw new NotImplementedException(); }
        public virtual int Save(ByteWrapper buf) { throw new NotImplementedException(); }
        public virtual int Save(Stream file) { throw new NotImplementedException(); }

        // do nothing...
        public virtual VU_ERRCODE Distribute(VuSessionEntity ent) { throw new NotImplementedException(); }

        private VuPlayerPoolGame(ByteWrapper buf) : base(buf) { throw new NotImplementedException(); }
        private VuPlayerPoolGame(Stream file) : base(file) { throw new NotImplementedException(); }
    }

    public class GlobalGroupFilter : VuFilter
    {
        public GlobalGroupFilter() { throw new NotImplementedException(); }
        //TODO public virtual ~GlobalGroupFilter();

        public override VU_BOOL Test(VuEntity ent) { throw new NotImplementedException(); }
        public override VU_BOOL RemoveTest(VuEntity ent) { throw new NotImplementedException(); }
        public override int Compare(VuEntity ent1, VuEntity ent2) { throw new NotImplementedException(); }
        public override VuFilter Copy() { throw new NotImplementedException(); }

        protected GlobalGroupFilter(GlobalGroupFilter other) { throw new NotImplementedException(); }
    }
    //-----------------------------------------
    public class VuGlobalGroup : VuGroupEntity
    {
        public const int VU_GLOBAL_GROUP_ENTITY_ID = 1;
        public static GlobalGroupFilter globalGrpFilter = new GlobalGroupFilter();

        // constructors & destructor
        public VuGlobalGroup()
            : base(VuEntity.VU_GLOBAL_GROUP_ENTITY_TYPE, VUSTATIC.vuxWorldName, globalGrpFilter)
        {
            // make certain owner is NULL session
            share_.ownerId_ = VU_ID.vuNullId;
            share_.id_.creator_ = 0;
            share_.id_.num_ = VU_GLOBAL_GROUP_ENTITY_ID;
            sessionMax_ = 1024;
            connected_ = false;
        }

        //TODO public virtual ~VuGlobalGroup();

        // virtual function interface	-- stubbed out here
        public virtual int SaveSize() { throw new NotImplementedException(); }
        public virtual int Save(ByteWrapper buf) { throw new NotImplementedException(); }
        public virtual int Save(Stream file) { throw new NotImplementedException(); }

        public virtual VU_BOOL HasTarget(VU_ID id)          // always returns TRUE
        {
            // global group includes everybody
            return true;
        }

        public VU_BOOL Connected() { return connected_; }
        public void SetConnected(VU_BOOL conn) { connected_ = conn; }

        private VuGlobalGroup(ByteWrapper buf) : base(buf) { throw new NotImplementedException(); }
        private VuGlobalGroup(Stream file) : base(file) { throw new NotImplementedException(); }

        // DATA
        protected VU_BOOL connected_;
    }

    //-----------------------------------------------------------------------------
    public class VuSessionFilter : VuFilter
    {

        public VuSessionFilter(VU_ID groupId) { throw new NotImplementedException(); }
        //TODO public virtual ~VuSessionFilter();

        public override VU_BOOL Test(VuEntity ent) { throw new NotImplementedException(); }
        public override VU_BOOL RemoveTest(VuEntity ent) { throw new NotImplementedException(); }
        public override int Compare(VuEntity ent1, VuEntity ent2) { throw new NotImplementedException(); }
        public override VU_BOOL Notice(VuMessage evnt) { throw new NotImplementedException(); }
        public override VuFilter Copy() { throw new NotImplementedException(); }

        protected VuSessionFilter(VuSessionFilter other) { throw new NotImplementedException(); }

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
            curr_ = VuTailNode.vuTailNode;
            VUSTATIC.vuCollectionManager.Register(this);
        }
        //TODO public virtual ~VuSessionsIterator();

        public VuSessionEntity GetFirst()
        {
            if (collection_ != null)
                curr_ = ((VuLinkedList)collection_).head_;
            return (VuSessionEntity)curr_.entity_;
        }

        public VuSessionEntity GetNext()
        { curr_ = curr_.next_; return (VuSessionEntity)curr_.entity_; }
        public override VuEntity CurrEnt() { throw new NotImplementedException(); }

        public override VU_BOOL IsReferenced(VuEntity ent) { throw new NotImplementedException(); }
        public override VU_ERRCODE Cleanup() { throw new NotImplementedException(); }

        internal VuLinkNode curr_;
    }
}
