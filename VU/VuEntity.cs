using System;

using BIG_SCALAR = System.Single;
//typedef float BIG_SCALAR;
using SM_SCALAR = System.Single;
using VU_DAMAGE = System.UInt64;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;
using VU_BOOL = System.Boolean;

using System.IO;
using FalconNet.Common.Encoding;//typedef float SM_SCALAR;

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

    public struct VuFlagBits
    {
        public bool private_;//: 1;	// 1 --> not public
        public bool transfer_;//: 1;	// 1 --> can be transferred
        public bool tangible_;//: 1;	// 1 --> can be seen/touched with
        public bool collidable_;//: 1;	// 1 --> put in auto collision table
        public bool global_;//: 1;	// 1 --> visible to all groups
        public bool persistent_;//: 1;	// 1 --> keep ent local across group joins
        public uint pad_;//: 10;	// unused

        public static explicit operator VuFlagBits(ushort val)
        {
            VuFlagBits flag = new VuFlagBits();
            throw new NotImplementedException();
            //TODO return flag;
        }
        public static explicit operator ushort(VuFlagBits flag)
        {
            ushort val = 0;
            throw new NotImplementedException();
            //TODO return val;
        }
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
        public VuFlagBits flags_;
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
        public OrientationData(ByteWrapper buffer)
        {
            yaw_ = buffer.DecodeFloatLE();
            pitch_ = buffer.DecodeFloatLE();
            roll_ = buffer.DecodeFloatLE();
            dyaw_ = buffer.DecodeFloatLE();
            dpitch_ = buffer.DecodeFloatLE();
            droll_ = buffer.DecodeFloatLE();
        }
#endif
    }

    public struct PositionData
    {
        public BIG_SCALAR x_, y_, z_;
        public SM_SCALAR dx_, dy_, dz_;
        public PositionData(ByteWrapper buffer)
        {
            x_ = buffer.DecodeFloatLE();
            y_ = buffer.DecodeFloatLE();
            z_ = buffer.DecodeFloatLE();
            dx_ = buffer.DecodeFloatLE();
            dy_ = buffer.DecodeFloatLE();
            dz_ = buffer.DecodeFloatLE();
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

        public VuEntity(ByteWrapper buffer)
        {
            refcount_ = 0;
            //driver_ = 0;
            share_.entityType_ = buffer.DecodeUShortLE();
            share_.flags_ = (VuFlagBits)buffer.DecodeUShortLE();
            share_.id_.creator_ = new VU_SESSION_ID(buffer.DecodeUShortLE());
            share_.id_.num_ = buffer.DecodeULongLE();
            share_.ownerId_.creator_ = new VU_SESSION_ID(buffer.DecodeULongLE());
            share_.ownerId_.num_ = buffer.DecodeULongLE();
            share_.assoc_.creator_ = new VU_SESSION_ID(buffer.DecodeULongLE());
            share_.assoc_.num_ = buffer.DecodeULongLE();
            pos_ = new PositionData(buffer);
            orient_ = new OrientationData(buffer);

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


        public VuEntity(Stream file)
        {
        }

        public virtual int Save(ByteWrapper buffer)
        {
            throw new NotImplementedException();
        }

        public virtual int Save(Stream file)
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

        public bool IsPrivate() { return (bool)share_.flags_.private_; }
        public bool IsTransferrable() { return (bool)share_.flags_.transfer_; }
        public bool IsTangible() { return (bool)share_.flags_.tangible_; }
        public bool IsCollidable() { return (bool)share_.flags_.collidable_; }
        public bool IsGlobal() { return (bool)share_.flags_.global_; }
        public bool IsPersistent() { return (bool)share_.flags_.persistent_; }
        public VuFlagBits Flags() { return share_.flags_; }
        public ushort FlagValue() { return (ushort)share_.flags_; }

        public ushort Type() { return share_.entityType_; }
        public bool IsLocal() { return (bool)((VUSTATIC.vuLocalSession == OwnerId()) ? true : false); }

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

#if VU_GRID_TREE_Y_MAJOR
            VUSTATIC.vuCollectionManager.HandleMove(this, y, x);
#else
            VUSTATIC.vuCollectionManager.HandleMove(this, x, y);
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

        // Special VU type getters
        public virtual VU_BOOL IsTarget()
        {
            return false;
        }
        public virtual VU_BOOL IsSession()
        {
            return false;
        }
        public virtual VU_BOOL IsGroup()
        {
            return false;
        }
        public virtual VU_BOOL IsGame()
        {
            return false;
        }
        // not really a type, but a utility nonetheless
        public virtual VU_BOOL IsCamera()
        {
            return false;
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
        int RefCount() { return refcount_; }
        // destructor
        //TODO protected virtual ~VuEntity();
        protected virtual void ChangeId(VuEntity other) { throw new NotImplementedException(); }
        internal void SetVuState(VU_MEM_STATE newState) { vuState_ = newState; }
        internal virtual VU_ERRCODE InsertionCallback() { throw new NotImplementedException(); }
        internal virtual VU_ERRCODE RemovalCallback() { throw new NotImplementedException(); }

        public static int VuReferenceEntity(VuEntity ent)
        {
            if (ent!=null)
                return ++ent.refcount_;
            else
                return -1;
        }

        public static int VuDeReferenceEntity(VuEntity ent)
        {
            //Debug.Assert(ent == null || FALSE == F4IsBadWritePtr(ent, sizeof(VuEntity)));
            //if (ent) { // JB 010305 CTD
            if (ent!= null)// && !F4IsBadWritePtr(ent, sizeof(VuEntity)))
            { // JB 010305 CTD
                if (--ent.refcount_ <= 0)
                {
                    //assert(vuDatabase == NULL || vuDatabase->Find(ent->Id()) == NULL);
                    //assert(vuCollectionManager == NULL || vuCollectionManager->FindEnt(ent) == 0);
                    //delete ent;
                    return 0;
                }
                return ent.refcount_;
            }
            return -1;
        }

        internal ShareData share_;
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