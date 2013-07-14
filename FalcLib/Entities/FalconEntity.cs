using FalconNet.Common;
using FalconNet.VU;
using System;
using System.Diagnostics;
using Control = System.Byte;
using VU_TIME = System.UInt64;
using VU_ID_NUMBER = System.UInt64;
using FalconNet.CampaignBase;
using FalconNet.Common.Maths;

namespace FalconNet.FalcLib
{
    // ================================
    // FalcEntity class
    // ================================

    public abstract class FalconEntity : VuEntity
    {
        public FalconEntity(ushort type, VU_ID_NUMBER eid)
            : base(type, eid)
        {
            falconType = 0;
            falconFlags = 0;
            dirty_falcent = 0;
            dirty_classes = 0;
            dirty_score = 0;
        }

#if TODO	
        public FalconEntity (byte[] stream, ref int pos)
            : base(stream, ref pos)
		{
			dirty_falcent = 0;
			dirty_classes = 0;
			dirty_score = 0;
			falconType = (EntityEnum)(stream [pos]);
			pos += sizeof(EntityEnum);

			if (CampaignClass.gCampDataVersion >= 32) {
				falconFlags = (FEC_FLAGS)(stream [pos]);
				pos += sizeof(FEC_FLAGS);
			}

		}

		public FalconEntity (FileStream filePtr) 
            :base(filePtr)
		{
			dirty_falcent = 0;
			dirty_classes = 0;
			dirty_score = 0;
			falconType = (EntityEnum)(filePtr.ReadByte ());
			
			if (CampaignClass.gCampDataVersion >= 32)
				falconFlags = (FEC_FLAGS)(filePtr.ReadByte ());

		}
	

        public FalconEntity(byte[] bytes, ref int offset, int version)
            : base(bytes, ref offset)
		{
            throw new NotImplementedException();
		}
 	
		public virtual int Save (byte[] stream, ref int pos)
		{
			int saveSize = base.Save (stream, ref pos);

			stream [pos] = (byte)falconType;
			pos += sizeof(EntityEnum);
			saveSize += sizeof(EntityEnum);	
			stream [pos] = (byte)falconFlags;
			pos += sizeof(FEC_FLAGS);
			saveSize += sizeof(FEC_FLAGS);
			return (saveSize);
		}

        public virtual int Save(FileStream filePtr)
        {
            int saveSize = base.Save(filePtr);

            filePtr.WriteByte((byte)falconType);
            saveSize += sizeof(EntityEnum);
            filePtr.WriteByte((byte)falconFlags);
            saveSize += sizeof(FEC_FLAGS);
            return (saveSize);
        }

        public override int SaveSize()
        {
            return base.SaveSize() + sizeof(EntityEnum) + sizeof(FEC_FLAGS);
            //   return VuEntity::SaveSize();
        }
#endif

        //TODO public virtual ~FalconEntity();

        /// <summary>
        /// inits entity data, calling base class initialization in sequence.
        /// </summary>
        public virtual void InitData()
        {
            InitLocalData();
        }

        /* cleans up the whole hierarchy data, calling base class in sequence. */
        /// <summary>
        /// cleans up the whole hierarchy data, calling base class in sequence.
        /// </summary>
        public virtual void CleanupData()
        {
            CleanupLocalData();
        }

        /// <summary>
        /// calls cleanup data. Called after entity is removed from database in thread safe spot.
        /// </summary>
        /// <returns></returns>
        public virtual VU_ERRCODE RemovalCallback()
        {
            //CleanupData();
            return VU_ERRCODE.VU_SUCCESS;
        }

        public virtual bool IsSimBase()
        {
            return false;
        }

        public virtual bool IsCampBase()
        {
            return false;
        }

        public bool IsCampaign()
        {
            return (falconType.IsFlagSet(EntityEnum.FalconCampaignEntity)) ? true : false;
        }

        public bool IsSim()
        {
            return (falconType.IsFlagSet(EntityEnum.FalconSimEntity)) ? true : false;
        }

        public bool IsSimObjective()
        {
            return (falconType.IsFlagSet(EntityEnum.FalconSimObjective)) ? true : false;
        }

        public bool IsPersistant()
        {
            return (falconType.IsFlagSet(EntityEnum.FalconPersistantEntity)) ? true : false;
        }

        public void SetTypeFlag(EntityEnum flag)
        {
            falconType |= flag;
        }

        public void UnSetTypeFlag(EntityEnum flag)
        {
            falconType &= ~flag;
        }

        public void SetFalcFlag(FEC_FLAGS flag)
        {
            if (!(falconFlags.IsFlagSet(flag)))
            {
                falconFlags |= flag;
                MakeFlagsDirty();
            }
        }

        public void UnSetFalcFlag(FEC_FLAGS flag)
        {
            if (falconFlags.IsFlagSet(flag))
            {
                falconFlags &= ~flag;
                MakeFlagsDirty();
            }
        }

        public bool IsSetFalcFlag(FEC_FLAGS flag)
        {
            return falconFlags.IsFlagSet(flag);
        }

        public bool IsPlayer()
        {
            return IsSetFalcFlag(FEC_FLAGS.FEC_HASPLAYERS);
        }

        public abstract int Wake();

        public abstract int Sleep();

        public abstract short GetCampID();

        public abstract byte GetTeam();

        public abstract Control GetCountry();

        public virtual byte GetDomain()
        {
            return EntityDB.Falcon4ClassTable[Type() - VU_LAST_ENTITY_TYPE].vuClassData.classInfo_[Vu_CLASS.VU_DOMAIN];
        }

        public virtual FEC_RADAR GetRadarMode()
        {
            return FEC_RADAR.FEC_RADAR_OFF;
        }

        public virtual void SetRadarMode(FEC_RADAR p)
        {
        }

        public virtual void ReturnToSearch()
        {
        }

        public virtual void SetSearchMode(int p)
        {
        }

        public virtual int CombatClass()
        {
            return 999;
        }

        // 2002-02-25 ADDED BY S.G. No combat class for non flight or non aircraft class
        public virtual bool OnGround()
        {
            return false;
        }

        public virtual bool HasEntity(VuEntity e)
        {
            return this == e;    // sfr: added for new driver
        }

        public virtual bool IsMissile()
        {
            return false;
        }

        public virtual bool IsLauncher()
        {
            return false;    // MLR 3/4/2004 - rocket pods
        }

        public virtual bool IsBomb()
        {
            return false;
        }

        public virtual bool IsGun()
        {
            return false;
        }

        public virtual bool IsMover()
        {
            return false;
        }

        public virtual bool IsVehicle()
        {
            return false;
        }

        public virtual bool IsStatic()
        {
            return false;
        }

        public virtual bool IsHelicopter()
        {
            return false;
        }

        public virtual bool IsEject()
        {
            return false;
        }

        public virtual bool IsAirplane()
        {
            return false;
        }

        public virtual bool IsGroundVehicle()
        {
            return false;
        }

        public virtual bool IsShip()
        {
            return false;
        }

        public virtual bool IsWeapon()
        {
            return false;
        }

        public virtual bool IsExploding()
        {
            return false;
        }

        public virtual bool IsDead()
        {
            return false;
        }

        public virtual bool IsEmitting()
        {
            return false;
        }

        public virtual float GetVt()
        {
            return 0.0f;
        }

        public virtual float GetKias()
        {
            return 0.0f;
        }

        public virtual MoveType GetMovementType()
        {
            return MoveType.NoMove;
        }

        public virtual bool IsUnit()
        {
            return false;
        }

        public virtual bool IsObjective()
        {
            return false;
        }

        public virtual bool IsBattalion()
        {
            return false;
        }

        public virtual bool IsBrigade()
        {
            return false;
        }

        public virtual bool IsFlight()
        {
            return false;
        }

        public virtual bool IsSquadron()
        {
            return false;
        }

        public virtual bool IsPackage()
        {
            return false;
        }

        public virtual bool IsTeam()
        {
            return false;
        }

        public virtual bool IsTaskForce()
        {
            return false;
        }

        public virtual bool IsSPJamming()
        {
            return false;
        }

        public virtual bool IsAreaJamming()
        {
            return false;
        }

        public virtual bool HasSPJamming()
        {
            return false;
        }

        public virtual bool HasAreaJamming()
        {
            return false;
        }

        public virtual float GetRCSFactor()
        {
            return 0.0f;
        }

        public virtual float GetIRFactor()
        {
            return 0.0f;
        }

        public virtual Radar_types GetRadarType()
        {
            return Radar_types.RDR_NO_RADAR;
        }

        public virtual byte[] GetDamageModifiers()
        {
#if TODO
			return CampaignStatic.DefaultDamageMods;
#endif
            throw new NotImplementedException();
        }

        public void GetLocation(out short x, out short y)
        {
            vector v;

            v.x = XPos();
            v.y = YPos();
            v.z = 0.0F;
#if TODO
			ConvertSimToGrid (v, x, y);
#endif
            throw new NotImplementedException();
        }

        public int GetAltitude()
        {
            return (int)(ZPos() * -1.0F);
        }

        public void SetOwner(FalconSessionEntity session)
        {
            // Set the owner to session
            share_.ownerId_ = session.OwnerId();
        }

        public void SetOwner(VU_ID sessionId)
        {
            // Set the owner to session
            share_.ownerId_ = sessionId;
        }

        public void DoFullUpdate()
        {
            VuEvent evnt = new VuFullUpdateEvent(this, (VUSTATIC.vuLocalSessionEntity != null ? ((FalconSessionEntity)VUSTATIC.vuLocalSessionEntity).GetGame() : null));
            evnt.RequestReliableTransmit();
            VuMessageQueue.PostVuMessage(evnt);
        }

        // Dirty Functions
        public virtual void ClearDirty()
        {
            // dodirtydata removes it from lists nows
            dirty_classes = 0;
            dirty_score = 0;
#if NOTHING
            //sfr: old
            int bin;
            VuEnterCriticalSection();
            bin = calc_dirty_bucket();
            assert((bin >= 0) && (bin < MAX_DIRTY_BUCKETS));
            dirty_classes = 0;
            dirty_score = 0;
            DirtyBucket[bin].Remove(this);
            VuExitCriticalSection();
#endif
        }

        public void MakeDirty(Dirty_Class bits, Dirtyness score)
        {
#if TODO
            dirty_classes |= bits;

            // sfr: for player entities, always send reliable and immediatelly
            if (IsPlayer())
            {
                score = Dirtyness.SEND_RELIABLEANDOOB;
            }

            // send only local units which are active (in DB) and if the unit is more dirty than currently is
            if (
                (!IsLocal()) ||
                (VuState() != VU_MEM_STATE.VU_MEM_ACTIVE) ||
                (score <= dirty_score) ||
                !(TheCampaign.Flags & CAMP_LOADED)
            )
            {
                return;
            }

            dirty_score = score;
            int bin = calc_dirty_bucket((int)score);

            if (IsSimBase())
            {
                lock (simDirtyMutexes[bin])
                {
#if USE_VU_COLL_FOR_DIRTY
        simDirtyBuckets[bin].ForcedInsert(this);
#else
                    simDirtyBuckets[bin].push_back(FalconEntityBin(this));
                }
#endif
            }
            else
            {
                lock (campDirtyMutexes[bin])
                {
#if USE_VU_COLL_FOR_DIRTY
        campDirtyBuckets[bin].ForcedInsert(this);
#else
                    campDirtyBuckets[bin].push_back(FalconEntityBin(this));
                }
#endif
            }
#endif
            throw new NotImplementedException();
        }

        public int EncodeDirty(ref byte[] stream)
        {
            throw new NotImplementedException();
        }

        public int DecodeDirty(ref byte[] stream)
        {
            throw new NotImplementedException();
        }

        public static void DoSimDirtyData()
        {
            throw new NotImplementedException();
        }

        public static void DoCampaignDirtyData()
        {
            throw new NotImplementedException();
        }

        public void MakeFlagsDirty()
        {
            MakeFalconEntityDirty(Dirty_Falcon_Entity.DIRTY_FALCON_FLAGS, Dirtyness.SEND_RELIABLEANDOOB);
        }

        public void MakeFalconEntityDirty(Dirty_Falcon_Entity bits, Dirtyness score)
        {
            if ((!IsLocal()) || (VuState() != VU_MEM_STATE.VU_MEM_ACTIVE))
            {
                return;
            }

            dirty_falcent |= bits;

            MakeDirty(Dirty_Class.DIRTY_FALCON_ENTITY, score);
        }

        // 2002-03-22 ADDED BY S.G. Needs them outside of battalion class
        public virtual void SetAQUIREtimer(VU_TIME newTime)
        {
        }

        public virtual void SetSEARCHtimer(VU_TIME newTime)
        {
        }

        public virtual void SetStepSearchMode(int p)
        {
        }

        public virtual VU_TIME GetAQUIREtimer()
        {
            return 0;
        }

        public virtual VU_TIME GetSEARCHtimer()
        {
            return 0;
        }

        public void SetFELocalFlag(int flag)
        {
            feLocalFlags |= flag;
        }
        public void UnSetFELocalFlag(int flag)
        {
            feLocalFlags &= ~flag;
        }
        public int IsSetFELocalFlag(int flag)
        {
            return feLocalFlags & flag;
        }
        /// <summary>
        /// initializes only local data (not base class), so not virtual.
        /// </summary>
        private void InitLocalData()
        {
            falconType = 0;
            falconFlags = 0;
            dirty_falcent = 0;
            dirty_classes = 0;
            dirty_score = 0;
            feLocalFlags = 0;
        }

        /// <summary>
        /// cleans up only the local data 
        /// </summary>
        private void CleanupLocalData()
        {
            // nothing to do here
        }

        private Dirty_Falcon_Entity dirty_falcent;
        private Dirty_Class dirty_classes;
        /// <summary>
        /// flags indicating unit dirtyness. Can be Ored. See enum Dirtyness for valid scores.
        /// </summary>
        private Dirtyness dirty_score;

        /// <summary>
        /// returns the dirty bucket where unit should be inserted based on its dirty_score.
        /// </summary>
        /// <returns>If unit is not dirty, returns -1</returns>
        private static int calc_dirty_bucket(int dirty_score)
        {
            int ds = dirty_score; //just for debugging

            if (dirty_score == 0)
            {
                return -1;
            }

            // sfr : new
            int bin = 0;

            while ((dirty_score & 0x1) == 0)
            {
                ++bin;
                dirty_score >>= 4;
            }

            return bin;

#if NOTHING
    // sfr: old
    else if (dirty_score <= SEND_EVENTUALLY)
    {
        return 1;
    }
    else if (dirty_score <= SEND_SOMETIME)
    {
        return 2;
    }
    else if (dirty_score <= SEND_LATER)
    {
        return 3;
    }
    else if (dirty_score <= SEND_SOON)
    {
        return 4;
    }
    else if (dirty_score <= SEND_NOW)
    {
        return 5;
    }
    else if (dirty_score <= SEND_RELIABLE)
    {
        return 6;
    }
    else if (dirty_score <= SEND_OOB)
    {
        return 7;
    }
    else if (dirty_score > SEND_OOB)
    {
        return 8;
    }
    else
    {
        ShiAssert("This can't happen at all...");
        return 0;
    }

#endif
        }

        private FEC_FLAGS falconFlags;
        protected EntityEnum falconType;
        private int feLocalFlags;
    }

}

