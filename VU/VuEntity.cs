using System;

using BIG_SCALAR = System.Single;
//typedef float BIG_SCALAR;
using SM_SCALAR = System.Single;
using VU_DAMAGE = System.UInt64;
using VU_BYTE = System.Byte;

using System.IO;//typedef float SM_SCALAR;

namespace FalconNet.VU
{
    // memory state defines
    [Flags]
    public enum VU_MEM_STATE : byte
    {
        VU_MEM_CREATED = 0x01,
        VU_MEM_ACTIVE = 0x02,
        VU_MEM_SUSPENDED = 0x03,       // lives only in AntiDatabase
        VU_MEM_PENDING_DELETE = 0x04,
        VU_MEM_INACTIVE = 0x05,
        VU_MEM_DELETED = 0xdd
    }

    public struct VuEntityType
    {
        public ushort id_;
        public ushort collisionType_;
        public SM_SCALAR collisionRadius_;
        public VU_BYTE[] classInfo_; //[CLASS_NUM_BYTES];
        public VU_TIME updateRate_;
        public VU_TIME updateTolerance_;
        public SM_SCALAR fineUpdateRange_;	// max distance to send position updates
        public SM_SCALAR fineUpdateForceRange_; // distance to force position updates
        public SM_SCALAR fineUpdateMultiplier_; // multiplier for noticing position updates
        public VU_DAMAGE damageSeed_;
        public int hitpoints_;
        public ushort majorRevisionNumber_;
        public ushort minorRevisionNumber_;
        public ushort createPriority_;
        public byte managementDomain_;
        public bool transferable_;
        public bool private_;
        public bool tangible_;
        public bool collidable_;
        public bool global_;
        public bool persistent_;
    }

    //typedef SM_SCALAR VU_QUAT[4];
    //typedef SM_SCALAR VU_VECT[3];

    // shared data
    public struct ShareData
    {
        public ushort entityType_;	// id (table index)
#if TODO
	    union {
	      ushort value_;
	      VuFlagBits breakdown_;
	    } flags_;
#endif
        public ushort flags_;
        public VU_ID id_;
        public VU_ID ownerId_;	// owning session
        public VU_ID assoc_;	// id of ent which must be local to this ent
    }

    public struct OrientationData
    {
#if VU_USE_QUATERNION
		    public VU_QUAT quat_;	// quaternion indicating current facing
		    public VU_VECT dquat_;	// unit vector expressing quaternion delta
		    public SM_SCALAR theta_;	// scalar indicating rate of above delta
#else // !VU_USE_QUATERNION
        public SM_SCALAR yaw_, pitch_, roll_;
        public SM_SCALAR dyaw_, dpitch_, droll_;
        public OrientationData(byte[] stream, ref int pos)
        {
            yaw_ = BitConverter.ToSingle(stream, pos);
            pos += sizeof(Single);
            pitch_ = BitConverter.ToSingle(stream, pos);
            pos += sizeof(Single);
            roll_ = BitConverter.ToSingle(stream, pos);
            pos += sizeof(Single);
            dyaw_ = BitConverter.ToSingle(stream, pos);
            pos += sizeof(Single);
            dpitch_ = BitConverter.ToSingle(stream, pos);
            pos += sizeof(Single);
            droll_ = BitConverter.ToSingle(stream, pos);
            pos += sizeof(Single);
        }
#endif
    }

    public struct PositionData
    {
        public BIG_SCALAR x_, y_, z_;
        public SM_SCALAR dx_, dy_, dz_;
        public PositionData(byte[] stream, ref int pos)
        {
            x_ = BitConverter.ToSingle(stream, pos);
            pos += sizeof(Single);
            y_ = BitConverter.ToSingle(stream, pos);
            pos += sizeof(Single);
            z_ = BitConverter.ToSingle(stream, pos);
            pos += sizeof(Single);
            dx_ = BitConverter.ToSingle(stream, pos);
            pos += sizeof(Single);
            dy_ = BitConverter.ToSingle(stream, pos);
            pos += sizeof(Single);
            dz_ = BitConverter.ToSingle(stream, pos);
            pos += sizeof(Single);
        }
    }

    public class VuEntity
    {
        public const int VU_UNKNOWN_ENTITY_TYPE = 0;
        public const int VU_SESSION_ENTITY_TYPE = 1;
        public const int VU_GROUP_ENTITY_TYPE = 2;
        public const int VU_GLOBAL_GROUP_ENTITY_TYPE = 3;
        public const int VU_GAME_ENTITY_TYPE = 4;
        public const int VU_PLAYER_POOL_GROUP_ENTITY_TYPE = 5;
        public const int VU_LAST_ENTITY_TYPE = 100;

        public const int VU_CREATE_PRIORITY_BASE = 100;

        public VuEntity() { }

        public VuEntity(int t)
        {
        }

        public VuEntity(byte[] stream, ref int pos)
        {
            refcount_ = 0;
            //driver_ = 0;
            share_.entityType_ = BitConverter.ToUInt16(stream, pos);
            pos += sizeof(UInt16);
            share_.flags_ = BitConverter.ToUInt16(stream, pos);
            pos += sizeof(UInt16);
            share_.id_.creator_ = new VU_SESSION_ID(BitConverter.ToUInt64(stream, pos));
            pos += sizeof(UInt64);
            share_.id_.num_ = BitConverter.ToUInt64(stream, pos);
            pos += sizeof(UInt64);
            share_.ownerId_.creator_ = new VU_SESSION_ID(BitConverter.ToUInt64(stream, pos));
            pos += sizeof(UInt64);
            share_.ownerId_.num_ = BitConverter.ToUInt64(stream, pos);
            pos += sizeof(UInt64);
            share_.assoc_.creator_ = new VU_SESSION_ID(BitConverter.ToUInt64(stream, pos));
            pos += sizeof(UInt64);
            share_.assoc_.num_ = BitConverter.ToUInt64(stream, pos);
            pos += sizeof(UInt64);
            pos_ = new PositionData(stream, ref pos);
            orient_ = new OrientationData(stream, ref pos);

            vuState_ = VU_MEM_STATE.VU_MEM_CREATED;
            SetEntityType(share_.entityType_);
            lastUpdateTime_ = vuxGameTime;
            lastCollisionCheckTime_ = vuxGameTime;
#if _DEBUG
          vuentitycount ++;
          if (vuentitycount > vuentitypeak)
              vuentitypeak = vuentitycount;
#endif
        }


        public VuEntity(FileStream filePtr)
        {
        }

        public virtual int Save(byte[] stream, ref int pos)
        {
            throw new NotImplementedException();
        }

        public virtual int Save(FileStream filePtr)
        {
            throw new NotImplementedException();
        }

        public virtual int SaveSize()
        {
            throw new NotImplementedException();
        }

        // getters
        public VU_ID Id() { return share_.id_; }
        public VU_BYTE Domain() { return domain_; }
#if TODO
  bool IsPrivate()	{ return (bool)share_.flags_.breakdown_.private_; }
  bool IsTransferrable(){return (bool)share_.flags_.breakdown_.transfer_;}
  bool IsTangible()	{ return (bool)share_.flags_.breakdown_.tangible_; }
  bool IsCollidable(){ return (bool)share_.flags_.breakdown_.collidable_;}
  bool IsGlobal()	{ return (bool)share_.flags_.breakdown_.global_;}
  bool IsPersistent(){ return (bool)share_.flags_.breakdown_.persistent_;}
  VuFlagBits Flags()	{ return share_.flags_.breakdown_; }
  ushort FlagValue()	{ return share_.flags_.value_; }

  ushort VuState()	{ return vuState_; }
  ushort Type()		{ return share_.entityType_; }
  bool IsLocal()	{ return (bool)((vuLocalSession == OwnerId()) ? true : false);}
#endif
        public VU_ID OwnerId() { return share_.ownerId_; }
        public VU_ID Association() { return share_.assoc_; }

        public BIG_SCALAR XPos() { return pos_.x_; }
        public BIG_SCALAR YPos() { return pos_.y_; }
        public BIG_SCALAR ZPos() { return pos_.z_; }
        public SM_SCALAR XDelta() { return pos_.dx_; }
        public SM_SCALAR YDelta() { return pos_.dy_; }
        public SM_SCALAR ZDelta() { return pos_.dz_; }
#if VU_USE_QUATERNION
  VU_QUAT *Quat() { return &orient_.quat_; }
  VU_VECT *DeltaQuat() { return &orient_.dquat_; }
  SM_SCALAR Theta() { return orient_.theta_; }
#else // !VU_USE_QUATERNION
        public SM_SCALAR Yaw() { return orient_.yaw_; }
        public SM_SCALAR Pitch() { return orient_.pitch_; }
        public SM_SCALAR Roll() { return orient_.roll_; }
        public SM_SCALAR YawDelta() { return orient_.dyaw_; }
        public SM_SCALAR PitchDelta() { return orient_.dpitch_; }
        public SM_SCALAR RollDelta() { return orient_.droll_; }
#endif

        public VU_TIME UpdateRate() { return entityTypePtr_.updateRate_; }
        public VU_TIME LastUpdateTime() { return lastUpdateTime_; }
        public VU_TIME LastTransmissionTime() { return lastTransmissionTime_; }
        public VU_TIME LastCollisionCheckTime() { return lastCollisionCheckTime_; }

        public VuEntityType EntityType() { return entityTypePtr_; }

        public void SetPosition(BIG_SCALAR x, BIG_SCALAR y, BIG_SCALAR z)
        {
            if (z > 0.0F)
            {
                z = 0.0F;
            }
#if TODO
#if VU_GRID_TREE_Y_MAJOR
            vuCollectionManager.HandleMove(this, y, x);
#else
            vuCollectionManager.HandleMove(this, x, y);
#endif
#endif
            //  assert( x > -1e6F && y > -1e6F && z < 20000.0F);
            //  assert( x < 5e6 && y < 5e6 && z > -250000.0F);

            if (!IsLocal())
            {
                BIG_SCALAR lx, ly, lz;

                //MonoPrint ("%08x %3.3f %3.3f %3.3f\n", Id().num_, x - lx, y - ly, z - lz);
                lx = x;
                ly = y;
                lz = z;
            }

            //  MonoPrint ("%08x %3.3f %3.3f %3.3f\n", this, x, y, z);
            pos_.x_ = x;
            pos_.y_ = y;
            pos_.z_ = z;
        }

        public void SetAssociation(VU_ID assoc)
        {
            share_.assoc_ = assoc;
        }

        public bool IsLocal()
        {
            //TODO return (bool)((vuLocalSession == OwnerId()) ? true : false);
            throw new NotImplementedException();
        }

        public void SetEntityType(ushort entityType)
        {
#if TODO
          share_.entityType_ = entityType;

          if (share_.entityType_ < vuTypeTableSize) // JPO - make sure its in the range (CTD) VU_LAST_ENTITY_TYPE == 100 
            entityTypePtr_ = &vuTypeTable[share_.entityType_];
          else if (share_.entityType_ >= VU_LAST_ENTITY_TYPE)
            entityTypePtr_ = VuxType(share_.entityType_);
          else {
	          assert(!"share_.entityType_ out of range");
	          entityTypePtr_ = 0;
          }
  
          if (entityTypePtr_ == 0)
            entityTypePtr_ = &vuTypeTable[VU_UNKNOWN_ENTITY_TYPE];
  
          share_.flags_.breakdown_.private_    = entityTypePtr_.private_;
          share_.flags_.breakdown_.transfer_   = entityTypePtr_.transferable_;
          share_.flags_.breakdown_.tangible_   = entityTypePtr_.tangible_;
          share_.flags_.breakdown_.collidable_ = entityTypePtr_.collidable_;
          share_.flags_.breakdown_.global_     = entityTypePtr_.global_;
          share_.flags_.breakdown_.persistent_ = entityTypePtr_.persistent_;

          if (share_.flags_.breakdown_.persistent_&&share_.flags_.breakdown_.transfer_)
            share_.flags_.breakdown_.transfer_ = 0;
  
          share_.flags_.breakdown_.pad_ = 1;
          domain_ = entityTypePtr_.managementDomain_;
          // cap domain at (bits/long - 1)
          if (domain_ > 31)
            domain_ = 31;
#endif
        }

        public VU_MEM_STATE VuState() { return vuState_; }

        // =========================================
        // Usefull functions. Not sure why it's here
        // =========================================

        public static int SimCompare(VuEntity ent1, VuEntity ent2)
        {
#if TODO
            int retval = 0;

            if (ent1 != null && ent2 != null && ent1.Id() != ent2.Id())
            {
                retval = (ent2.Id() > ent1.Id() ? 1 : -1);
            }

            return (retval);
#endif
            throw new NotImplementedException();
        }
         // event handlers
        public virtual VU_ERRCODE Handle(VuErrorMessage error)
        {
            // default implementation stub
            return VU_ERRCODE.VU_NO_OP;
        }
        public virtual VU_ERRCODE Handle(VuPushRequest msg)
        { throw new NotImplementedException(); }
        public virtual VU_ERRCODE Handle(VuPullRequest msg)
        { throw new NotImplementedException(); }
        public virtual VU_ERRCODE Handle(VuEvent evnt)
        {
            // default implementation stub
            return VU_ERRCODE.VU_NO_OP;
        }
        public virtual VU_ERRCODE Handle(VuFullUpdateEvent evnt)
        { throw new NotImplementedException(); }
        public virtual VU_ERRCODE Handle(VuPositionUpdateEvent evnt)
        { throw new NotImplementedException(); }
        public virtual VU_ERRCODE Handle(VuEntityCollisionEvent evnt)
        {
            // default implementation stub
            return VU_ERRCODE.VU_NO_OP;
        }
        public virtual VU_ERRCODE Handle(VuTransferEvent evnt)
        { throw new NotImplementedException(); }
        public virtual VU_ERRCODE Handle(VuSessionEvent evnt)
        {
            // default does nothing
            return VU_ERRCODE.VU_NO_OP;
        }

        protected ShareData share_;
        protected PositionData pos_;
        protected OrientationData orient_;

        // local data
        protected ushort refcount_;	// entity reference count
        protected VU_MEM_STATE vuState_;
        protected VU_BYTE domain_;
        protected VU_TIME lastUpdateTime_;
        protected VU_TIME lastCollisionCheckTime_;
        protected VU_TIME lastTransmissionTime_;
        protected VuEntityType entityTypePtr_;
        protected VuDriver driver_;


        private static VU_TIME vuxGameTime = new VU_TIME(); // TODO0
    }
}