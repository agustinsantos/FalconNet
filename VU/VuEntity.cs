using System;
using BIG_SCALAR = System.Single;
using SM_SCALAR = System.Single;
using VU_DAMAGE = System.UInt64;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;
using VU_BOOL = System.Boolean;
using VU_ID_NUMBER = System.UInt64;

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

    public class VuEntityType
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
        public bool private_;//: 1;	// 1 -. not public
        public bool transfer_;//: 1;	// 1 -. can be transferred
        public bool tangible_;//: 1;	// 1 -. can be seen/touched with
        public bool collidable_;//: 1;	// 1 -. put in auto collision table
        public bool global_;//: 1;	// 1 -. visible to all groups
        public bool persistent_;//: 1;	// 1 -. keep ent local across group joins
        public uint pad_;//: 10;	// unused


        public static explicit operator VuFlagBits(ushort bits)
        {
            VuFlagBits rst = new VuFlagBits();
            bits *= 2;
            bits *= 2;
            rst.persistent_ = (bits & 0x80) != 0; bits *= 2;
            rst.global_ = (bits & 0x80) != 0; bits *= 2;
            rst.collidable_ = (bits & 0x80) != 0; bits *= 2;
            rst.tangible_ = (bits & 0x80) != 0; bits *= 2;
            rst.transfer_ = (bits & 0x80) != 0; bits *= 2;
            rst.private_ = (bits & 0x80) != 0; bits *= 2;
            return rst;
        }
        public static explicit operator ushort(VuFlagBits flag)
        {
            ushort bits = 0;
            if (flag.persistent_)
                bits |= (byte)(1 << 5);
            if (flag.global_)
                bits |= (byte)(1 << 4);
            if (flag.collidable_)
                bits |= (byte)(1 << 3);
            if (flag.tangible_)
                bits |= (byte)(1 << 2);
            if (flag.transfer_)
                bits |= (byte)(1 << 1);
            if (flag.private_)
                bits |= (byte)(1 << 0);
            return bits;
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
        public OrientationData(Stream buffer)
        {
            yaw_ = SingleEncodingLE.Decode(buffer);
            pitch_ = SingleEncodingLE.Decode(buffer);
            roll_ = SingleEncodingLE.Decode(buffer);
            dyaw_ = SingleEncodingLE.Decode(buffer);
            dpitch_ = SingleEncodingLE.Decode(buffer);
            droll_ = SingleEncodingLE.Decode(buffer);
        }
#endif
    }

    public static class OrientationDataEncodingLE
    {
        public static void Encode(Stream stream, OrientationData val)
        {
            SingleEncodingLE.Encode(stream, val.yaw_);
            SingleEncodingLE.Encode(stream, val.pitch_);
            SingleEncodingLE.Encode(stream, val.roll_);
            SingleEncodingLE.Encode(stream, val.dyaw_);
            SingleEncodingLE.Encode(stream, val.dpitch_);
            SingleEncodingLE.Encode(stream, val.droll_);
        }

        public static void Decode(Stream stream, ref OrientationData rst)
        {
            rst.yaw_ = SingleEncodingLE.Decode(stream);
            rst.pitch_ = SingleEncodingLE.Decode(stream);
            rst.roll_ = SingleEncodingLE.Decode(stream);
            rst.dyaw_ = SingleEncodingLE.Decode(stream);
            rst.dpitch_ = SingleEncodingLE.Decode(stream);
            rst.droll_ = SingleEncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return SingleEncodingLE.Size * 6; }
        }
    }

    public struct PositionData
    {
        public BIG_SCALAR x_, y_, z_;
        public SM_SCALAR dx_, dy_, dz_;
    }

    public static class PositionDataEncodingLE
    {
        public static void Encode(Stream stream, PositionData val)
        {
            SingleEncodingLE.Encode(stream, val.x_);
            SingleEncodingLE.Encode(stream, val.y_);
            SingleEncodingLE.Encode(stream, val.z_);
            SingleEncodingLE.Encode(stream, val.dx_);
            SingleEncodingLE.Encode(stream, val.dy_);
            SingleEncodingLE.Encode(stream, val.dz_);
        }

        public static void Decode(Stream stream, ref PositionData rst)
        {
            rst.x_ = SingleEncodingLE.Decode(stream);
            rst.y_ = SingleEncodingLE.Decode(stream);
            rst.z_ = SingleEncodingLE.Decode(stream);
            rst.dx_ = SingleEncodingLE.Decode(stream);
            rst.dy_ = SingleEncodingLE.Decode(stream);
            rst.dz_ = SingleEncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return SingleEncodingLE.Size * 6; }
        }
    }

    public class VuEntity
    {
        public const int VU_GLOBAL_DOMAIN = 0;

        public const int VU_UNKNOWN_ENTITY_TYPE = 0;
        public const int VU_SESSION_ENTITY_TYPE = 1;
        public const int VU_GROUP_ENTITY_TYPE = 2;
        public const int VU_GLOBAL_GROUP_ENTITY_TYPE = 3;
        public const int VU_GAME_ENTITY_TYPE = 4;
        public const int VU_PLAYER_POOL_GROUP_ENTITY_TYPE = 5;
        public const int VU_LAST_ENTITY_TYPE = 100;

        public const int VU_CREATE_PRIORITY_BASE = 100;

        public static VU_ID_NUMBER vuAssignmentId = VU_ID.VU_FIRST_ENTITY_ID;
        public static VU_ID_NUMBER vuLowWrapNumber = VU_ID.VU_FIRST_ENTITY_ID;
        public static VU_ID_NUMBER vuHighWrapNumber = ~((VU_ID_NUMBER)0);

        public VuEntity() { }

        /// <summary>
        /// creates the entity of a given type with the given id
        /// </summary>
        /// <param name="typeindex"></param>
        /// <param name="eid"></param>
        public VuEntity(ushort typeindex, VU_ID_NUMBER eid)
        {
            refcount_ = 0;
            driver_ = null;
            pos_.x_ = 0.0f;
            pos_.y_ = 0.0f;
            pos_.z_ = 0.0f;
            pos_.dx_ = 0.0f;
            pos_.dy_ = 0.0f;
            pos_.dz_ = 0.0f;

            orient_.yaw_ = 0.0f;
            orient_.pitch_ = 0.0f;
            orient_.roll_ = 0.0f;
            orient_.dyaw_ = 0.0f;
            orient_.dpitch_ = 0.0f;
            orient_.droll_ = 0.0f;

            vuState_ = VU_MEM_STATE.VU_MEM_CREATED;

            SetEntityType(typeindex);
            share_.assoc_ = VU_ID.vuNullId;
            share_.ownerId_ = VUSTATIC.vuLocalSession; // need to fill in from id structure
            share_.id_ = new VU_ID(share_.ownerId_.creator_, eid);

            lastUpdateTime_ = vuxGameTime;
            lastTransmissionTime_ = VUSTATIC.vuTransmitTime - VuRandomTime(UpdateRate());
            lastCollisionCheckTime_ = vuxGameTime;
        }

        // setters
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


        public void SetDelta(SM_SCALAR dx, SM_SCALAR dy, SM_SCALAR dz)
        {
            //	 assert( dx > -10000.0F && dy > -10000.0F && dz > -10000.0F);
            //   assert( dx <  10000.0F && dy <  10000.0F && dz <  10000.0F);
            //assert(!_isnan(dx));
            //assert(!_isnan(dy));
            //assert(!_isnan(dz));

            pos_.dx_ = dx;
            pos_.dy_ = dy;
            pos_.dz_ = dz;
        }
#if VU_USE_QUATERNION
  void SetQuat(VU_QUAT quat)
	{ orient_.quat_[0] = quat[0]; orient_.quat_[1] = quat[1];
	  orient_.quat_[2] = quat[2]; orient_.quat_[3] = quat[3]; }
  void SetQuatDelta(VU_VECT vect, SM_SCALAR theta)
	{ orient_.dquat_[0] = vect[0]; orient_.dquat_[1] = vect[1];
	  orient_.dquat_[2] = vect[2]; orient_.theta_ = theta; }
#else // !VU_USE_QUATERNION
        public void SetYPR(SM_SCALAR yaw, SM_SCALAR pitch, SM_SCALAR roll)
        { orient_.yaw_ = yaw; orient_.pitch_ = pitch; orient_.roll_ = roll; }
        public void SetYPRDelta(SM_SCALAR dyaw, SM_SCALAR dpitch, SM_SCALAR droll)
        { orient_.dyaw_ = dyaw; orient_.dpitch_ = dpitch; orient_.droll_ = droll; }
#endif // VU_USE_QUATERNION
        public void SetUpdateTime(VU_TIME currentTime)
        { lastUpdateTime_ = currentTime; }
        public void SetTransmissionTime(VU_TIME currentTime)
        { lastTransmissionTime_ = currentTime; }
        public void SetCollisionCheckTime(VU_TIME currentTime)
        { lastCollisionCheckTime_ = currentTime; }
        public void SetOwnerId(VU_ID ownerId)
        {
            // New owner sends xfer message: check for locality after set
            if (VUSTATIC.vuLocalSession == ownerId && share_.ownerId_ != ownerId)
            {
                share_.ownerId_ = ownerId;
                VuTargetEntity target = VUSTATIC.vuGlobalGroup;

                if (!IsGlobal())
                    target = VUSTATIC.vuLocalSessionEntity.Game();

                VuTransferEvent event_ = new VuTransferEvent(this, target);
                event_.Ref();
                event_.RequestReliableTransmit();
                VuMessageQueue.PostVuMessage(event_);
                // force immediate local handling of this
                Handle(event_);
                VUSTATIC.vuDatabase.Handle(event_);
                event_.UnRef();
            }
            share_.ownerId_ = ownerId;
        }

        public void SetEntityType(ushort entityType)
        {
            share_.entityType_ = entityType;

            if (share_.entityType_ < vuTypeTable.Length) // JPO - make sure its in the range (CTD) VU_LAST_ENTITY_TYPE == 100 
                entityTypePtr_ = vuTypeTable[share_.entityType_];
            else if (share_.entityType_ >= VU_LAST_ENTITY_TYPE)
                entityTypePtr_ = VUSTATIC.VuxType(share_.entityType_);
            else
            {
                entityTypePtr_ = null;
                throw new Exception("share_.entityType_ out of range");
            }

            if (entityTypePtr_ == null)
                entityTypePtr_ = vuTypeTable[VU_UNKNOWN_ENTITY_TYPE];

            share_.flags_.private_ = entityTypePtr_.private_;
            share_.flags_.transfer_ = entityTypePtr_.transferable_;
            share_.flags_.tangible_ = entityTypePtr_.tangible_;
            share_.flags_.collidable_ = entityTypePtr_.collidable_;
            share_.flags_.global_ = entityTypePtr_.global_;
            share_.flags_.persistent_ = entityTypePtr_.persistent_;

            if (share_.flags_.persistent_ && share_.flags_.transfer_)
                share_.flags_.transfer_ = false;

            share_.flags_.pad_ = 1;
            domain_ = entityTypePtr_.managementDomain_;
            // cap domain at (bits/long - 1)
            if (domain_ > 31)
                domain_ = 31;
        }

        public void SetAssociation(VU_ID assoc)
        {
            share_.assoc_ = assoc;
        }
        public void AlignTimeAdd(VU_TIME dt)
        {
            lastUpdateTime_ += dt;

            if (IsLocal())
                lastCollisionCheckTime_ += dt;

            if (driver_ != null)
                driver_.AlignTimeAdd(dt);

        }

        public void AlignTimeSubtract(VU_TIME dt)
        {
            lastUpdateTime_ -= dt;

            if (IsLocal())
                lastCollisionCheckTime_ -= dt;

            if (driver_ != null)
                driver_.AlignTimeSubtract(dt);
        }

        // getters
        public VU_ID Id() { return share_.id_; }
        public virtual VU_BYTE Domain() { return domain_; }

        public bool IsPrivate() { return (bool)share_.flags_.private_; }
        public bool IsTransferrable() { return (bool)share_.flags_.transfer_; }
        public bool IsTangible() { return (bool)share_.flags_.tangible_; }
        public bool IsCollidable() { return (bool)share_.flags_.collidable_; }
        public bool IsGlobal() { return (bool)share_.flags_.global_; }
        public bool IsPersistent() { return (bool)share_.flags_.persistent_; }
        public VuFlagBits Flags() { return share_.flags_; }
        public ushort FlagValue() { return (ushort)share_.flags_; }
        public VU_ID OwnerId() { return share_.ownerId_; }
        public VU_MEM_STATE VuState() { return vuState_; }
        public ushort Type() { return share_.entityType_; }
        public bool IsLocal() { return (bool)((VUSTATIC.vuLocalSession == OwnerId()) ? true : false); }
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




        // =========================================
        // Usefull functions. Not sure why it's here
        // =========================================

        public static int SimCompare(VuEntity ent1, VuEntity ent2)
        {
            int retval = 0;

            if (ent1 != null && ent2 != null && ent1.Id() != ent2.Id())
            {
                retval = (ent2.Id() > ent1.Id() ? 1 : -1);
            }

            return (retval);
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
        {
            //assert(FALSE == F4IsBadReadPtr(vuDatabase, sizeof *vuDatabase));
            VuTargetEntity sender = (VuTargetEntity)VUSTATIC.vuDatabase.Find(msg.Sender());

            if (sender != null)
            {
                if (sender.IsTarget())
                {
                    if (share_.flags_.transfer_)
                    {
                        VuMessage resp = new VuErrorMessage(VUERROR.VU_CANT_TRANSFER_ENTITY_ERROR,
                                                             msg.Sender(), Id(), sender);
                        resp.RequestReliableTransmit();
                        VuMessageQueue.PostVuMessage(resp);
                    }
                    else if (((ulong)(1 << Domain()) & VUSTATIC.vuLocalSessionEntity.DomainMask()) != 0)
                    {
                        VuEntity assoc = null;
                        if (Association() != VU_ID.vuNullId &&
                            (assoc = VUSTATIC.vuDatabase.Find(Association())) != null &&
                            assoc.OwnerId() != VUSTATIC.vuLocalSession)
                        {
                            // entity has association, and we do not manage it
                            VuMessage resp = new VuErrorMessage(VUERROR.VU_TRANSFER_ASSOCIATION_ERROR, msg.Sender(), Id(), sender);
                            resp.RequestReliableTransmit();
                            VuMessageQueue.PostVuMessage(resp);
                        }
                        else
                        {
                            SetOwnerId(VUSTATIC.vuLocalSession);
                        }
                    }
                    else
                    {
                        // we can't manage the entity, so send the error response
                        VuMessage resp = new VuErrorMessage(VUERROR.VU_CANT_MANAGE_ENTITY_ERROR,
                                                             msg.Sender(), Id(), sender);
                        resp.RequestReliableTransmit();
                        VuMessageQueue.PostVuMessage(resp);
                    }
                    return VU_ERRCODE.VU_SUCCESS;
                }
            }
            return VU_ERRCODE.VU_ERROR;
        }

        public virtual VU_ERRCODE Handle(VuPullRequest msg)
        {
            VuTargetEntity sender = (VuTargetEntity)VUSTATIC.vuDatabase.Find(msg.Sender());
            VuMessage resp = null;

            if (sender != null)
            {
                if (sender.IsTarget())
                {

                    if (share_.flags_.transfer_)
                    {
                        resp = new VuErrorMessage(VUERROR.VU_CANT_TRANSFER_ENTITY_ERROR,
                                                  msg.Sender(), Id(), sender);
                    }
                    else if (OwnerId() == VUSTATIC.vuLocalSession)
                    {
                        VuEntity assoc = null;
                        if (Association() != VU_ID.vuNullId &&
                            (assoc = VUSTATIC.vuDatabase.Find(Association())) != null &&
                            assoc.OwnerId() != msg.Sender())
                        {
                            // entity has association, and target does not manage it
                            resp = new VuErrorMessage(VUERROR.VU_TRANSFER_ASSOCIATION_ERROR,
                                                      msg.Sender(), Id(), sender);
                        }
                        else
                        {
                            SetOwnerId(msg.Sender());
                            // need to make message as SetOwnerId() only generates update if new
                            // owner is local
                            VuTargetEntity target = VUSTATIC.vuGlobalGroup;

                            if (!IsGlobal())
                                target = VUSTATIC.vuLocalSessionEntity.Game();

                            resp = new VuTransferEvent(this, target);
                        }
                    }
                    else
                    {
                        // we don't manage the entity, so send the error response
                        resp = new VuErrorMessage(VUERROR.VU_DONT_MANAGE_ENTITY_ERROR, msg.Sender(), Id(), sender);
                    }
                    resp.RequestReliableTransmit();
                    VuMessageQueue.PostVuMessage(resp);

                    return VU_ERRCODE.VU_SUCCESS;
                }
            }
            return VU_ERRCODE.VU_ERROR;
        }

        public virtual VU_ERRCODE Handle(VuEvent evnt)
        {
            // default implementation stub
            return VU_ERRCODE.VU_NO_OP;
        }

        public virtual VU_ERRCODE Handle(VuFullUpdateEvent evnt)
        {
            // default implementation 
            if (driver_ != null)
            {
                if (driver_.Handle(evnt) > 0)
                    return VU_ERRCODE.VU_SUCCESS;
            }
            else if (evnt.Entity() != null && evnt.Entity() != this)
            {
                share_ = evnt.Entity().share_;
                pos_ = evnt.Entity().pos_;
                orient_ = evnt.Entity().orient_;
                SetUpdateTime(vuxGameTime);
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        public virtual VU_ERRCODE Handle(VuPositionUpdateEvent evnt)
        {
            if (driver_ == null || driver_.Handle(evnt) <= 0)
            {
                SetPosition(evnt.x_, evnt.y_, evnt.z_);
                SetDelta(evnt.dx_, evnt.dy_, evnt.dz_);

#if VU_USE_QUATERNION
    SetQuat(event.quat_);
    SetQuatDelta(event.dquat_, event.theta_);
#else
                SetYPR(evnt.yaw_, evnt.pitch_, evnt.roll_);
                //SetYPRDelta(event.dyaw_, event.dpitch_, event.droll_);
#endif

                SetUpdateTime(vuxGameTime);
            }
            return VU_ERRCODE.VU_SUCCESS;
        }

        public virtual VU_ERRCODE Handle(VuEntityCollisionEvent evnt)
        {
            // default implementation stub
            return VU_ERRCODE.VU_NO_OP;
        }

        public virtual VU_ERRCODE Handle(VuTransferEvent evnt)
        {
            SetOwnerId(evnt.newOwnerId_);
            return VU_ERRCODE.VU_SUCCESS;
        }

        public virtual VU_ERRCODE Handle(VuSessionEvent evnt)
        {
            // default does nothing
            return VU_ERRCODE.VU_NO_OP;
        }
        public int RefCount() { return refcount_; }
        // destructor
        //TODO protected virtual ~VuEntity();
        internal virtual void ChangeId(VuEntity other)
        {
            CriticalSection.VuEnterCriticalSection();
            VuMessageQueue.FlushAllQueues();
            VuReferenceEntity(this);
            VUSTATIC.vuDatabase.Remove(this);
            VuMessageQueue.FlushAllQueues();

            if (vuAssignmentId < vuLowWrapNumber || vuAssignmentId > vuHighWrapNumber)
                vuAssignmentId = vuLowWrapNumber; // cover wrap
            share_.id_.num_ = vuAssignmentId++;

            while (VUSTATIC.vuDatabase.Find(Id()) != null || VUSTATIC.vuAntiDB.Find(Id()) != null || Id() == other.Id())
                share_.id_.num_ = vuAssignmentId++;

            SetVuState(VU_MEM_STATE.VU_MEM_ACTIVE);
            VUSTATIC.vuDatabase.Insert(this);
            VuDeReferenceEntity(this);
            CriticalSection.VuExitCriticalSection();
        }

        internal void SetVuState(VU_MEM_STATE newState) { vuState_ = newState; }
        internal virtual VU_ERRCODE InsertionCallback()
        {
            return VU_ERRCODE.VU_NO_OP;
        }

        internal virtual VU_ERRCODE RemovalCallback()
        {
            return VU_ERRCODE.VU_NO_OP;
        }


        public static int VuReferenceEntity(VuEntity ent)
        {
            if (ent != null)
                return ++ent.refcount_;
            else
                return -1;
        }

        public static int VuDeReferenceEntity(VuEntity ent)
        {
            //Debug.Assert(ent == null || false == F4IsBadWritePtr(ent, sizeof(VuEntity)));
            //if (ent) { // JB 010305 CTD
            if (ent != null)// && !F4IsBadWritePtr(ent, sizeof(VuEntity)))
            { // JB 010305 CTD
                if (--ent.refcount_ <= 0)
                {
                    //assert(vuDatabase == NULL || vuDatabase.Find(ent.Id()) == NULL);
                    //assert(vuCollectionManager == NULL || vuCollectionManager.FindEnt(ent) == 0);
                    //delete ent;
                    return 0;
                }
                return ent.refcount_;
            }
            return -1;
        }

        private static Random rand = new Random();
        private static VU_TIME VuRandomTime(VU_TIME max)
        {
            return (VU_TIME)rand.NextDouble() * max;
        }
        public static VU_ID_NUMBER VuRandomID()
        {
            return (VU_ID_NUMBER)rand.NextDouble() * UInt64.MaxValue;
        }

        protected internal ShareData share_;
        internal protected PositionData pos_;
        internal OrientationData orient_;

        // local data
        internal ushort refcount_;	// entity reference count
        public VU_MEM_STATE vuState_;
        protected VU_BYTE domain_;
        internal VU_TIME lastUpdateTime_;
        internal VU_TIME lastCollisionCheckTime_;
        protected VU_TIME lastTransmissionTime_;
        protected VuEntityType entityTypePtr_;
        protected VuDriver driver_;


        private static VU_TIME vuxGameTime = new VU_TIME(); // TODO

        static VuEntityType[] vuTypeTable = new VuEntityType[]{
            new VuEntityType { id_= VU_UNKNOWN_ENTITY_TYPE,     // class id
                collisionType_ = 0, collisionRadius_ = 0.0f,                    // collision type, radius
                classInfo_ = new VU_BYTE[]{0,0,0,0,0,0,0,0},          // class info
              updateRate_= 0, updateTolerance_= 0,                         // update rate, tolerance
              fineUpdateRange_ = 0.0f, fineUpdateForceRange_ = 0.0f, fineUpdateMultiplier_ = 0.0f,             // fine update range/force/multiplier
              damageSeed_ = 0, hitpoints_ = 0,                         // damage seed, hitpoints
              majorRevisionNumber_= 0, minorRevisionNumber_ = 0,                         // major/minor rev numbers
              createPriority_ = 9999,                         // create priority
              managementDomain_ = VU_GLOBAL_DOMAIN,             // management domain
                transferable_ = false,                      //   transferrable
                private_ = true,                       //   private
                tangible_ = false,                      //   tangible
                collidable_ = false,                      //   collidable
                global_ = false,                      //   global
                persistent_ = false                       //   persistent
            },
            new VuEntityType { id_= VU_SESSION_ENTITY_TYPE,       // class id
                collisionType_ = 0, collisionRadius_ = 0.0f,                    // collision type, radius
              classInfo_ = new VU_BYTE[]{0,1,0,0,0,0,0,0},            // class info
              updateRate_= VUSTATIC.VU_TICS_PER_SECOND*5,         // update rate
              updateTolerance_= VUSTATIC.VU_TICS_PER_SECOND*15,        // update tolerance
              fineUpdateRange_ = 0.0f, fineUpdateForceRange_ = 0.0f, fineUpdateMultiplier_ = 1.0f,             // fine update range/force/multiplier
              damageSeed_ = 0, hitpoints_ = 0,                         // damage seed, hitpoints
              majorRevisionNumber_= 0, minorRevisionNumber_ = 0,                         // major/minor rev numbers
              createPriority_ = 1,                            // create priority
              managementDomain_ = VU_GLOBAL_DOMAIN,             // management domain
                transferable_ = false,                      //   transferrable
                private_ = false,                      //   private
                tangible_ = false,                      //   tangible
                collidable_ = false,                      //   collidable
                global_ = true,                       //   global
                persistent_ = true                        //   persistent
            },
            new VuEntityType { id_= VU_GROUP_ENTITY_TYPE,         // class id
                collisionType_ = 0, collisionRadius_ = 0.0f,                    // collision type, radius
              classInfo_ = new VU_BYTE[]{0,2,0,0,0,0,0,0},            // class info
              updateRate_= VUSTATIC.VU_TICS_PER_SECOND*30,        // update rate
              updateTolerance_= VUSTATIC.VU_TICS_PER_SECOND*60,        // update tolerance
              fineUpdateRange_ = 0.0f, fineUpdateForceRange_ = 0.0f, fineUpdateMultiplier_ = 0.0f,             // fine update range/force/multiplier
              damageSeed_ = 0, hitpoints_ = 0,                         // damage seed, hitpoints
              majorRevisionNumber_= 0, minorRevisionNumber_ = 0,                         // major/minor rev numbers
              createPriority_ = 2,                            // create priority
              managementDomain_ = VU_GLOBAL_DOMAIN,             // management domain
                transferable_ = true,                       //   transferrable
                private_ = false,                      //   private
                tangible_ = false,                      //   tangible
                collidable_ = false,                      //   collidable
                global_ = true,                       //   global
                persistent_ = false                       //   persistent
            },
            new VuEntityType { id_= VU_GLOBAL_GROUP_ENTITY_TYPE,  // class id
                collisionType_ = 0, collisionRadius_ = 0.0f,                    // collision type, radius
              classInfo_ = new VU_BYTE[]{0,3,0,0,0,0,0,0},            // class info
              updateRate_= VUSTATIC.VU_TICS_PER_SECOND*60,        // update rate
              updateTolerance_= VUSTATIC.VU_TICS_PER_SECOND*120,       // update tolerance
              fineUpdateRange_ = 0.0f, fineUpdateForceRange_ = 0.0f, fineUpdateMultiplier_ = 0.0f,             // fine update range/force/multiplier
              damageSeed_ = 0, hitpoints_ = 0,                         // damage seed, hitpoints
              majorRevisionNumber_= 0, minorRevisionNumber_ = 0,                         // major/minor rev numbers
              createPriority_ = 3,                            // create priority
              managementDomain_ = VU_GLOBAL_DOMAIN,             // management domain
                transferable_ = false,                      //   transferrable
                private_ = true,                       //   private
                tangible_ = false,                      //   tangible
                collidable_ = false,                      //   collidable
                global_ = false,                      //   global
                persistent_ = true                        //   persistent
            },
            new VuEntityType { id_= VU_GAME_ENTITY_TYPE,          // class id
                collisionType_ = 0, collisionRadius_ = 0.0f,                    // collision type, radius
              classInfo_ = new VU_BYTE[]{0,4,0,0,0,0,0,0},            // class info
              updateRate_= VUSTATIC.VU_TICS_PER_SECOND*30,        // update rate
              updateTolerance_= VUSTATIC.VU_TICS_PER_SECOND*60,        // update tolerance
              fineUpdateRange_ = 0.0f, fineUpdateForceRange_ = 0.0f, fineUpdateMultiplier_ = 0.0f,             // fine update range/force/multiplier
              damageSeed_ = 0, hitpoints_ = 0,                         // damage seed, hitpoints
              majorRevisionNumber_= 0, minorRevisionNumber_ = 0,                         // major/minor rev numbers
              createPriority_ = 4,                            // create priority
              managementDomain_ = VU_GLOBAL_DOMAIN,             // management domain
                transferable_ = true,                       //   transferrable
                private_ = false,                      //   private
                tangible_ = false,                      //   tangible
                collidable_ = false,                      //   collidable
                global_ = true,                       //   global
                persistent_ = false                       //   persistent
            },
            new VuEntityType { id_= VU_PLAYER_POOL_GROUP_ENTITY_TYPE, // class id
                collisionType_ = 0, collisionRadius_ = 0.0f,                    // collision type, radius
              classInfo_ = new VU_BYTE[]{0,5,0,0,0,0,0,0},            // class info
              updateRate_= VUSTATIC.VU_TICS_PER_SECOND*60,        // update rate
              updateTolerance_= VUSTATIC.VU_TICS_PER_SECOND*120,       // update tolerance
              fineUpdateRange_ = 0.0f, fineUpdateForceRange_ = 0.0f, fineUpdateMultiplier_ = 0.0f,             // fine update range/force/multiplier
              damageSeed_ = 0, hitpoints_ = 0,                         // damage seed, hitpoints
              majorRevisionNumber_= 0, minorRevisionNumber_ = 0,                         // major/minor rev numbers
              createPriority_ = 5,                            // create priority
              managementDomain_ = VU_GLOBAL_DOMAIN,             // management domain
                transferable_ = false,                      //   transferrable
                private_ = true,                       //   private
                tangible_ = false,                      //   tangible
                collidable_ = false,                      //   collidable
                global_ = false,                      //   global
                persistent_ = true                        //   persistent
            }
            };

    }

    public static class VuEntityEncodingLE
    {
        public static void Encode(Stream stream, VuEntity val)
        {
            UInt16EncodingLE.Encode(stream, val.share_.entityType_);
            UInt16EncodingLE.Encode(stream, (UInt16)val.share_.flags_);
            UInt16EncodingLE.Encode(stream, (UInt16)val.share_.id_.creator_);
            UInt64EncodingLE.Encode(stream, val.share_.id_.num_);
            UInt64EncodingLE.Encode(stream, (UInt64)val.share_.ownerId_.creator_);
            UInt64EncodingLE.Encode(stream, val.share_.ownerId_.num_);
            UInt64EncodingLE.Encode(stream, (UInt64)val.share_.assoc_.creator_);
            UInt64EncodingLE.Encode(stream, val.share_.assoc_.num_);
            PositionDataEncodingLE.Encode(stream, val.pos_);
            OrientationDataEncodingLE.Encode(stream, val.orient_);
        }


        public static void Decode(Stream stream, ref VuEntity rst)
        {
            rst.refcount_ = 0;
            //driver_ = 0;
            rst.share_.entityType_ = UInt16EncodingLE.Decode(stream);
            rst.share_.flags_ = (VuFlagBits)UInt16EncodingLE.Decode(stream);
            rst.share_.id_.creator_ = new VU_SESSION_ID(UInt16EncodingLE.Decode(stream));
            rst.share_.id_.num_ = UInt64EncodingLE.Decode(stream);
            rst.share_.ownerId_.creator_ = new VU_SESSION_ID(UInt64EncodingLE.Decode(stream));
            rst.share_.ownerId_.num_ = UInt64EncodingLE.Decode(stream);
            rst.share_.assoc_.creator_ = new VU_SESSION_ID(UInt64EncodingLE.Decode(stream));
            rst.share_.assoc_.num_ = UInt64EncodingLE.Decode(stream);
            PositionDataEncodingLE.Decode(stream, ref rst.pos_);
            OrientationDataEncodingLE.Decode(stream, ref rst.orient_);

            rst.vuState_ = VU_MEM_STATE.VU_MEM_CREATED;
            rst.SetEntityType(rst.share_.entityType_);
            rst.lastUpdateTime_ = VUSTATIC.vuxGameTime;
            rst.lastCollisionCheckTime_ = VUSTATIC.vuxGameTime;
#if _DEBUG
                      vuentitycount ++;
                      if (vuentitycount > vuentitypeak)
                          vuentitypeak = vuentitycount;
#endif
        }

        public static int Size
        {
            get { return UInt16EncodingLE.Size * 3 + UInt64EncodingLE.Size * 4 + PositionDataEncodingLE.Size + OrientationDataEncodingLE.Size; }
        }
    }
}