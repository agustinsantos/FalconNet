using System;
using FalconNet.Common;
using System.IO;
using System.Diagnostics;
using FalconNet.VU;

namespace FalconNet.FalcLib
{
	public enum MoveType
	{
		NoMove = 0,
		Foot,
		Wheeled,
		Tracked,
		LowAir,
		Air,
		Naval,
		Rail,
		MOVEMENT_TYPES
	} ;

	[Flags]
    public enum FEC_FLAGS : byte
	{
		FEC_HOLDSHORT = 0x01,		// Don't takeoff until a player attaches
		FEC_PLAYERONLY = 0x02,		// This entity is only valid if under player control
		FEC_HASPLAYERS = 0x04,		// One or more player is attached to this entity
		FEC_REGENERATING = 0x08,		// This entity is undead.
		FEC_PLAYER_ENTERING = 0x10,		// A player is soon to attach to this aircraft/flight
		FEC_INVULNERABLE = 0x20		// This thing can't be destroyed
	}
	
	[Flags]
	public enum EntityEnum : sbyte
	{
		FalconCampaignEntity = 0x1,
		FalconSimEntity = 0x2,
		FalconPersistantEntity = 0x8,
		FalconSimObjective = 0x20
	}
	// Radar Modes
	public enum FEC_RADAR
	{
		FEC_RADAR_OFF = 0x00,	   	// Radar always off
		FEC_RADAR_SEARCH_100 = 0x01,	   	// Search Radar - 100 % of the time (always on)
		FEC_RADAR_SEARCH_1 = 0x02,	   	// Search Sequence #1
		FEC_RADAR_SEARCH_2 = 0x03,	   	// Search Sequence #2
		FEC_RADAR_SEARCH_3 = 0x04,	   	// Search Sequence #3
		FEC_RADAR_AQUIRE = 0x05,	   	// Aquire Mode (looking for a target)
		FEC_RADAR_GUIDE = 0x06,	   	// Missile in flight. Death is imminent
		FEC_RADAR_CHANGEMODE = 0x07	   	// Missile in flight. Death is imminent
	}
// ================================
// FalcEntity class
// ================================

	public abstract class FalconEntity :  VuEntity
	{

		
		private int			dirty_falcent;
		private int			dirty_classes;
		private int			dirty_score;

		private int calc_dirty_bucket ()
		{
			throw new NotImplementedException ();
		}

		private FEC_FLAGS		falconFlags;
		protected EntityEnum	falconType;

		public FalconEntity (int type)
            : base (type)
		{
			falconType = 0;
			falconFlags = 0;
			dirty_falcent = 0;
			dirty_classes = 0;
			dirty_score = 0;
		}
		
		public FalconEntity (byte[] stream, ref int pos)
            : base(stream, ref pos)
		{
			dirty_falcent = 0;
			dirty_classes = 0;
			dirty_score = 0;
			falconType = (EntityEnum)(stream [pos]);
			pos += sizeof(EntityEnum);
#if TODO
			if (CampaignClass.gCampDataVersion >= 32) {
				falconFlags = (FEC_FLAGS)(stream [pos]);
				pos += sizeof(FEC_FLAGS);
			}
#endif 
			throw new NotImplementedException();
		}

		public FalconEntity (FileStream filePtr) 
            :base(filePtr)
		{
			dirty_falcent = 0;
			dirty_classes = 0;
			dirty_score = 0;
			falconType = (EntityEnum)(filePtr.ReadByte ());
#if TODO			
			if (CampaignClass.gCampDataVersion >= 32)
				falconFlags = (FEC_FLAGS)(filePtr.ReadByte ());
#endif 
			throw new NotImplementedException();
		}
		public FalconEntity(byte[] bytes, ref int offset, int version)
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

		public virtual int Save (FileStream filePtr)
		{
			int saveSize = base.Save (filePtr);

			filePtr.WriteByte ((byte)falconType);
			saveSize += sizeof(EntityEnum);
			filePtr.WriteByte ((byte)falconFlags);
			saveSize += sizeof(FEC_FLAGS);
			return (saveSize);
		}

		public override int SaveSize ()
		{
			return base.SaveSize () + sizeof(EntityEnum) + sizeof(FEC_FLAGS);
			//   return VuEntity::SaveSize();
		}


		//TODO public virtual ~FalconEntity();

		public bool IsCampaign ()
		{
			return (falconType.IsFlagSet (EntityEnum.FalconCampaignEntity)) ? true : false;
		}

		public bool IsSim ()
		{
			return (falconType.IsFlagSet (EntityEnum.FalconSimEntity)) ? true : false;
		}

		public bool IsSimObjective ()
		{
			return (falconType.IsFlagSet (EntityEnum.FalconSimObjective)) ? true : false;
		}

		public bool IsPersistant ()
		{
			return (falconType.IsFlagSet (EntityEnum.FalconPersistantEntity)) ? true : false;
		}

		public void SetTypeFlag (EntityEnum flag)
		{
			falconType |= flag;
		}

		public void UnSetTypeFlag (EntityEnum flag)
		{
			falconType &= ~flag;
		}

		public void SetFalcFlag (FEC_FLAGS flag)
		{
			if (!(falconFlags.IsFlagSet (flag))) {
				falconFlags |= flag;
				MakeFlagsDirty ();
			}
		}

		public void UnSetFalcFlag (FEC_FLAGS flag)
		{
			if (falconFlags.IsFlagSet (flag)) {
				falconFlags &= ~flag;
				MakeFlagsDirty ();
			}
		}

		public bool IsSetFalcFlag (FEC_FLAGS flag)
		{
			return falconFlags.IsFlagSet (flag);
		}

		public bool IsPlayer ()
		{
			return IsSetFalcFlag (FEC_FLAGS.FEC_HASPLAYERS);
		}

		public abstract int Wake ();

		public abstract int Sleep ();

		public virtual short GetCampID ()
		{
			Debug.WriteLine ("Illegal use of FalconEntity");
			throw new NotSupportedException();
		}

		public virtual byte GetTeam ()
		{
			Debug.WriteLine ("Illegal use of FalconEntity");
			throw new NotSupportedException();
		}

		public virtual Control GetCountry ()
		{
			Debug.WriteLine ("Illegal use of FalconEntity");
			throw new NotSupportedException();
		}

		public virtual byte GetDomain ()
		{
			//TODO  return Falcon4ClassTable[Type() - VU_LAST_ENTITY_TYPE].vuClassData.classInfo_[VU_DOMAIN];
			throw new NotImplementedException ();
		}

		public virtual FEC_RADAR GetRadarMode ()
		{
			return FEC_RADAR.FEC_RADAR_OFF;
		}

		public virtual void SetRadarMode (FEC_RADAR p)
		{
		}

		public virtual void ReturnToSearch ()
		{
		}

		public virtual void SetSearchMode (int p)
		{
		}

		public virtual int CombatClass ()
		{
			return 999;
		} 
		
		// 2002-02-25 ADDED BY S.G. No combat class for non flight or non aircraft class
		public virtual bool OnGround ()
		{
			return false;
		}

		public virtual bool IsMissile ()
		{
			return false;
		}

		public virtual bool IsBomb ()
		{
			return false;
		}

		public virtual bool IsGun ()
		{
			return false;
		}

		public virtual bool IsMover ()
		{
			return false;
		}

		public virtual bool IsVehicle ()
		{
			return false;
		}

		public virtual bool IsStatic ()
		{
			return false;
		}

		public virtual bool IsHelicopter ()
		{
			return false;
		}

		public virtual bool IsEject ()
		{
			return false;
		}

		public virtual bool IsAirplane ()
		{
			return false;
		}

		public virtual bool IsGroundVehicle ()
		{
			return false;
		}

		public virtual bool IsShip ()
		{
			return false;
		}

		public virtual bool IsWeapon ()
		{
			return false;
		}

		public virtual bool IsExploding ()
		{
			return false;
		}

		public virtual bool IsDead ()
		{
			return false;
		}

		public virtual bool IsEmitting ()
		{
			return false;
		}

		public virtual float Vt ()
		{
			return 0.0f;
		}

		public virtual float Kias ()
		{
			return 0.0f;
		}

		public virtual MoveType GetMovementType ()
		{
			return MoveType.NoMove;
		}

		public virtual bool IsUnit ()
		{
			return false;
		}

		public virtual bool IsObjective ()
		{
			return false;
		}

		public virtual bool IsBattalion ()
		{
			return false;
		}

		public virtual bool IsBrigade ()
		{
			return false;
		}

		public virtual bool IsFlight ()
		{
			return false;
		}

		public virtual bool IsSquadron ()
		{
			return false;
		}

		public virtual bool IsPackage ()
		{
			return false;
		}

		public virtual bool IsTeam ()
		{
			return false;
		}

		public virtual bool IsTaskForce ()
		{
			return false;
		}

		public virtual bool IsSPJamming ()
		{
			return false;
		}

		public virtual bool IsAreaJamming ()
		{
			return false;
		}

		public virtual bool HasSPJamming ()
		{
			return false;
		}

		public virtual bool HasAreaJamming ()
		{
			return false;
		}

		public virtual float GetRCSFactor ()
		{
			return 0.0f;
		}

		public virtual float GetIRFactor ()
		{
			return 0.0f;
		}

		public virtual Radar_types GetRadarType ()
		{
			return Radar_types.RDR_NO_RADAR;
		}
	
		public virtual byte[] GetDamageModifiers ()
		{
		#if TODO
			return CampaignStatic.DefaultDamageMods;
		#endif 
			throw new NotImplementedException();
		}

		public void GetLocation (ref short x, ref short y)
		{
			vector v;
			
			v.x = XPos ();
			v.y = YPos ();
			v.z = 0.0F;
		#if TODO
			ConvertSimToGrid (v, x, y);
		#endif 
			throw new NotImplementedException();
		}

		public int GetAltitude ()
		{
			return (int)(ZPos () * -1.0F);
		}
	
		public void SetOwner (FalconSessionEntity session)
		{
#if TODO
			// Set the owner to session
			share_.ownerId_ = session.OwnerId ();
#endif 
			throw new NotImplementedException();
		}

		public void SetOwner (VU_ID sessionId)
		{
			// Set the owner to session
			share_.ownerId_ = sessionId;
		}

		public void DoFullUpdate ()
		{
			throw new NotImplementedException ();
		}

		// Dirty Functions
		public virtual void ClearDirty ()
		{
			throw new NotImplementedException ();
		}

		public void MakeDirty (Dirty_Class bits, Dirtyness score)
		{
			throw new NotImplementedException ();
		}

		public int EncodeDirty (ref byte[] stream)
		{
			throw new NotImplementedException ();
		}

		public int DecodeDirty (ref byte[] stream)
		{
			throw new NotImplementedException ();
		}

		public static void DoSimDirtyData ()
		{
			throw new NotImplementedException ();
		}

		public static void DoCampaignDirtyData ()
		{
			throw new NotImplementedException ();
		}

		public void MakeFlagsDirty ()
		{
			throw new NotImplementedException ();
		}

		public void MakeFalconEntityDirty (Dirty_Falcon_Entity bits, Dirtyness score)
		{
			throw new NotImplementedException ();
		}

		// 2002-03-22 ADDED BY S.G. Needs them outside of battalion class
		public virtual void SetAQUIREtimer (VU_TIME newTime)
		{
		}

		public virtual void SetSEARCHtimer (VU_TIME newTime)
		{
		}

		public virtual void SetStepSearchMode (int p)
		{
		}

		public virtual VU_TIME GetAQUIREtimer ()
		{
			return null;
		}

		public virtual VU_TIME GetSEARCHtimer ()
		{
			return null;
		}
		// END OF ADDED SECTION 2002-03-22
	}

}

