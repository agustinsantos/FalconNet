using System;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using VU_BYTE=System.Byte;
using Objective=FalconNet.Campaign.ObjectiveClass;
using Team=System.Int32;
namespace FalconNet.Campaign
{
	// Transmittable
	[Flags]
    public enum Transmittable
	{
		CBC_EMITTING		=0x01,
		CBC_JAMMED		=	0x04
	}
	// Local
	public enum CBC_ENUM
	{
		CBC_CHECKED		=	0x001,			// Used by mission planning to prevent repeated targetting
		CBC_AWAKE			=0x002,			// Deaggregated on local machine
		CBC_IN_PACKAGE	=	0x004,			// This item is in our local package (only applicable to flights)
		CBC_HAS_DELTA		=0x008,
		CBC_IN_SIM_LIST	=	0x010,			// In the sim's nearby campaign entity lists
		CBC_INTEREST		=0x020,			// Some session still is interested in this entity
		CBC_RESERVED_ONLY	=0x040,			// This entity is here only in order to reserve namespace
		CBC_AGGREGATE		=0x080,
		CBC_HAS_TACAN		=0x100
	}

	public static class CampbaseStatic
	{
// ===================================
// Camp base defines
// ===================================

// ===================================
// Base class flags
// ===================================




// ===================================
// Name space shit
// ===================================
		/* TODO
public static int  FIRST_OBJECTIVE_VU_ID_NUMBER	(VU_FIRST_ENTITY_ID)
public static int  LAST_OBJECTIVE_VU_ID_NUMBER		(VU_FIRST_ENTITY_ID+MAX_NUMBER_OF_OBJECTIVES)
public static int  FIRST_NON_VOLITILE_VU_ID_NUMBER	(LAST_OBJECTIVE_VU_ID_NUMBER+1)
public static int  LAST_NON_VOLITILE_VU_ID_NUMBER	(FIRST_NON_VOLITILE_VU_ID_NUMBER+(MAX_NUMBER_OF_UNITS))
public static int  FIRST_LOW_VOLITILE_VU_ID_NUMBER	(LAST_NON_VOLITILE_VU_ID_NUMBER+1)
public static int  LAST_LOW_VOLITILE_VU_ID_NUMBER	(FIRST_LOW_VOLITILE_VU_ID_NUMBER+(MAX_NUMBER_OF_VOLITILE_UNITS))
public static int  FIRST_VOLITILE_VU_ID_NUMBER		(LAST_LOW_VOLITILE_VU_ID_NUMBER+1)
public static int  LAST_VOLITILE_VU_ID_NUMBER		~((VU_ID_NUMBER)0);


public static  VU_ID_NUMBER vuAssignmentId;
public static  VU_ID_NUMBER vuLowWrapNumber;
public static  VU_ID_NUMBER vuHighWrapNumber;
public static  VU_ID_NUMBER lastObjectiveId;
public static  VU_ID_NUMBER lastNonVolitileId;
public static  VU_ID_NUMBER lastLowVolitileId;
public static  VU_ID_NUMBER lastFlightId;
public static  VU_ID_NUMBER lastPackageId;
public static  VU_ID_NUMBER lastVolitileId;
    TODO */
// ===================================
// Camp base globals
// ===================================

		public static  byte[] CampSearch = new byte[Camplib.MAX_CAMP_ENTITIES];	// Search data - Could reduce to bitwise

		// ===========================
// Global functions
// ===========================

		public static CampEntity GetFirstEntity (F4LIt list)
		{throw new NotImplementedException();}

		public static CampEntity GetNextEntity (F4LIt list)
		{throw new NotImplementedException();}

		public static int Parent (CampEntity e)
		{throw new NotImplementedException();}

		public static int Real (int type)
		{throw new NotImplementedException();}

		public static short GetEntityClass (VuEntity h)
		{throw new NotImplementedException();}

		public static short GetEntityDomain (VuEntity h)
		{throw new NotImplementedException();}

		public static Unit GetEntityUnit (VuEntity h)
		{throw new NotImplementedException();}

		public static Objective GetEntityObjective (VuEntity h)
		{throw new NotImplementedException();}

		public static short FindUniqueID ()
		{throw new NotImplementedException();}

		public static int GetVisualDetectionRange (int mt)
		{throw new NotImplementedException();}
	}

	public class CampBaseClass :  FalconEntity
	{
 	
		private CampaignTime		spotTime;		// Last time this entity was spotted
		private short				spotted;		// Bitwise array of spotting data, by team
		private  Transmittable		base_flags;		// Various user flags
		private short				camp_id;		// Unique campaign id
		private Control          	owner;			// Controlling Country
		// Don't transmit below this line
		private  CBC_ENUM		local_flags;	// Non transmitted flags
		private TailInsertList		 components;	// List of deaggregated sim entities
		private VU_ID				deag_owner;		// Owner of deaggregated components
		private VU_ID				new_deag_owner;	// Who is most interrested in this guy
		private int					dirty_camp_base;

	
		// Access Functions
		public CampaignTime GetSpotTime ()
		{
			return spotTime;
		}

		public short GetSpotted ()
		{
			return spotted;
		}

		public Transmittable GetBaseFlags ()
		{
			return base_flags;
		}

		public short GetCampId ()
		{
			return camp_id;
		}

		public CBC_ENUM GetLocalFlags ()
		{
			return local_flags;
		}

		public TailInsertList GetComponents ()
		{
			return components;
		}

		public VU_ID GetDeagOwner ()
		{
			return deag_owner;
		}

		public void SetBaseFlags (short s)
		{throw new NotImplementedException();}

		public virtual void SetOwner (Control c)
		{throw new NotImplementedException();}

		public void SetCampId (short s)
		{throw new NotImplementedException();}

		public void SetLocalFlags ()
		{throw new NotImplementedException();}

		public void SetComponents (TailInsertList t)
		{throw new NotImplementedException();}

		public void SetDeagOwner (VU_ID v)
		{throw new NotImplementedException();}

		// Dirty Functions
		public void MakeCampBaseDirty (Dirty_Campaign_Base bits, Dirtyness score)
		{throw new NotImplementedException();}

		public virtual void WriteDirty (byte[] stream)
		{throw new NotImplementedException();}

		public virtual void ReadDirty (byte[] stream)
		{throw new NotImplementedException();}

		// Constructors and serial functions
		public CampBaseClass (int typeindex) : base(typeindex)
		{
			SetTypeFlag(EntityEnum.FalconCampaignEntity);
			spotted = 0;
			base_flags = Transmittable.CBC_EMITTING;
			local_flags = CBC_ENUM.CBC_AGGREGATE;	
			owner = Control.nullCONTROL;	
			camp_id = CampbaseStatic.FindUniqueID();
			pos_.z_ = 0.0F;
			components = null;
			deag_owner = VU_ID.FalconNullId;
			SetAggregate(1);
			SetAssociation(FalconSessionEntity.FalconLocalGame().Id());
			dirty_camp_base = 0;
			spotTime = new CampaignTime(0); // JB 010719
		}

		public CampBaseClass (VU_BYTE[] stream) : base(VuEntity.VU_LAST_ENTITY_TYPE)
		{throw new NotImplementedException();}
		public CampBaseClass(byte[] bytes, ref int offset, int version)
            : base(bytes, ref offset, version)
        {
#if TODO
            id = new VU_ID();
            id.num_ = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            id.creator_ = BitConverter.ToUInt32(bytes, offset);
            offset += 4;

            entityType = BitConverter.ToUInt16(bytes, offset);
            offset += 2;

            x = BitConverter.ToInt16(bytes, offset);
            offset += 2;

            y = BitConverter.ToInt16(bytes, offset);
            offset += 2;

            if (version < 70)
            {
                z = 0;
            }
            else
            {
                z = BitConverter.ToSingle(bytes, offset);
                offset += 4;
            }
            spotTime = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            spotted = BitConverter.ToInt16(bytes, offset);
            offset += 2;
            baseFlags = BitConverter.ToInt16(bytes, offset);
            offset += 2;
            owner = bytes[offset];
            offset++;
            campId = BitConverter.ToInt16(bytes, offset);
            offset += 2;
#endif
        }
		
		// public virtual ~CampBaseClass ();
		public override int SaveSize ()
		{throw new NotImplementedException();}

		public virtual int Save (ref VU_BYTE[] stream)
		{throw new NotImplementedException();}

		// event handlers
		public virtual int Handle (VuEvent evnt)
		{throw new NotImplementedException();}

		public virtual VU_ERRCODE Handle (VuFullUpdateEvent evnt)
		{throw new NotImplementedException();}

		public virtual VU_ERRCODE Handle (VuPositionUpdateEvent evnt)
		{throw new NotImplementedException();}

		public virtual VU_ERRCODE Handle (VuEntityCollisionEvent evnt)
		{throw new NotImplementedException();}

		public virtual VU_ERRCODE Handle (VuTransferEvent evnt)
		{throw new NotImplementedException();}

		public virtual VU_ERRCODE Handle (VuSessionEvent evnt)
		{throw new NotImplementedException();}

		// Required pure virtuals
		public virtual void SendDeaggregateData (VuTargetEntity t)
		{
		}

		public virtual int RecordCurrentState (FalconSessionEntity s, int i)
		{
			return 0;
		}

		public virtual int Deaggregate (FalconSessionEntity s)
		{
			return 0;
		}

		public virtual int Reaggregate (FalconSessionEntity s)
		{
			return 0;
		}

		public virtual int TransferOwnership (FalconSessionEntity s)
		{
			return 0;
		}

		public override int Wake ()
		{
			return 0;
		}

		public override int Sleep ()
		{
			return 0;
		}

		public virtual void InsertInSimLists (float f1, float f2)
		{
		}

		public virtual void RemoveFromSimLists ()
		{
		}

		public virtual void DeaggregateFromData (int i, byte[] b)
		{
			return;
		}

		public virtual void ReaggregateFromData (int i, byte[] b)
		{
			return;
		}

		public virtual void TransferOwnershipFromData (int i, byte[] b)
		{
			return;
		}

		public virtual int ApplyDamage (FalconCampWeaponsFire f, byte b)
		{
			return 0;
		}

		public virtual int ApplyDamage (DamType d, ref int i, int i2, short s)
		{
			return 0;
		}

		public virtual int DecodeDamageData (byte[] b, Unit u, FalconDeathMessage m)
		{
			return 0;
		}

		public virtual int CollectWeapons (byte[] buf, MoveType m, short [] s, byte [] b, int i)
		{
			return 0;
		}

		public virtual string  GetName (string s, int i, int i2)
		{
			return "None";
		}

		public virtual string  GetFullName (string s, int i, int i2)
		{
			return "None";
		}

		public virtual string  GetDivisionName (string s, int i, int i2)
		{
			return "None";
		}

		public virtual int GetHitChance (int i1, int i2)
		{
			return 0;
		}

		public virtual int GetAproxHitChance (int i1, int i2)
		{
			return 0;
		}

		public virtual int GetCombatStrength (int i1, int i2)
		{
			return 0;
		}

		public virtual int GetAproxCombatStrength (int i1, int i2)
		{
			return 0;
		}

		public virtual int GetWeaponRange (int p, FalconEntity  target = null)
		{
			return 0;
		} // 2008-03-08 ADDED SECOND DEFAULT PARM
		public virtual int GetAproxWeaponRange (int r)
		{
			return 0;
		}

		public virtual int GetDetectionRange (int r)
		{
			return 0;
		}			// Takes into account emitter status
		public virtual int GetElectronicDetectionRange (int r)
		{
			return 0;
		}			// Full range, regardless of emitter
		public virtual int CanDetect (FalconEntity  e)
		{
			return 0;
		}			// Nonzero if this entity can see ent
		public override bool OnGround ()
		{
			return false;
		}

		public override float Vt ()
		{
			return 0.0F;
		}

		public override float Kias ()
		{
			return 0.0F;
		}

		public override short GetCampID ()
		{
			return camp_id;
		}

		public override byte GetTeam ()
		{
			return  TeamStatic.GetTeam (owner);
		}

		public override Control GetCountry ()
		{
			return owner;
		}		// New FalcEnt friendly form
		public virtual FEC_RADAR StepRadar (int t, int d, float range)
		{
			return FEC_RADAR.FEC_RADAR_OFF;
		}

		public Control GetOwner ()
		{
			return owner;
		}			// Old form

		// These are only really relevant for sam/airdefense/radar entities
		public virtual int GetNumberOfArcs ()
		{
			return 1;
		}

		public virtual float GetArcRatio (int i)
		{
			return 0.0F;
		}

		public virtual float GetArcRange (int i)
		{
			return 0.0F;
		}

		public virtual void GetArcAngle (int i, ref float a1, ref float a2)
		{	
			a1 = 0.0F; 
			a2 = (float)(2 * Math.PI);
		}

		// Core functions
		public void SendMessage (VU_ID id, short msg, short d1, short d2, short d3, short d4)
		{throw new NotImplementedException();}

		public void BroadcastMessage (VU_ID id, short msg, short d1, short d2, short d3, short d4)
		{throw new NotImplementedException();}

		public VU_ERRCODE Remove ()
		{throw new NotImplementedException();}

		public int ReSpot ()
		{throw new NotImplementedException();}

		public FalconSessionEntity GetDeaggregateOwner ()
		{throw new NotImplementedException();}

		// Component accessers (Sim Flight emulators)
		public int GetComponentIndex (VuEntity me)
		{throw new NotImplementedException();}

		public SimBaseClass GetComponentEntity (int idx)
		{throw new NotImplementedException();}

		public SimBaseClass GetComponentLead ()
		{throw new NotImplementedException();}

		public SimBaseClass GetComponentNumber (int component)
		{throw new NotImplementedException();}

		public int NumberOfComponents ()
		{throw new NotImplementedException();}

		public byte Domain ()
		{
			return GetDomain ();
		}

		// Queries
		public override bool IsEmitting ()
		{
			return base_flags.IsFlagSet (Transmittable.CBC_EMITTING);
		}

		public bool IsJammed ()
		{
			return base_flags.IsFlagSet (Transmittable.CBC_JAMMED);
		}
		// Local flag access
		public bool IsChecked ()
		{
			return local_flags.IsFlagSet (CBC_ENUM.CBC_CHECKED);
		}

		public bool IsAwake ()
		{
			return local_flags.IsFlagSet (CBC_ENUM.CBC_AWAKE);
		}

		public bool InPackage ()
		{
			return local_flags.IsFlagSet (CBC_ENUM.CBC_IN_PACKAGE);
		}

		public bool InSimLists ()
		{
			return local_flags.IsFlagSet (CBC_ENUM.CBC_IN_SIM_LIST);
		}

		public bool IsInterested ()
		{
			return local_flags.IsFlagSet (CBC_ENUM.CBC_INTEREST);
		}

		public bool	IsReserved ()
		{
			return local_flags.IsFlagSet (CBC_ENUM.CBC_RESERVED_ONLY);
		}

		public bool IsAggregate ()
		{
			return local_flags.IsFlagSet (CBC_ENUM.CBC_AGGREGATE);
		}

		public bool IsTacan ()
		{
			return local_flags.IsFlagSet (CBC_ENUM.CBC_HAS_TACAN);
		}

		public bool HasDelta ()
		{
			return local_flags.IsFlagSet (CBC_ENUM.CBC_HAS_DELTA);
		}

		// Getters
		public override byte GetDomain ()
		{
			return (EntityType ()).classInfo_ [(int)VU_CLASS.VU_DOMAIN];
		}

		public byte GetClass ()
		{
			return (EntityType ()).classInfo_ [(int)VU_CLASS.VU_CLASS];
		}

		public byte GetType ()
		{
			return (EntityType ()).classInfo_ [(int)VU_CLASS.VU_TYPE];
		}

		public byte GetSType ()
		{
			return (EntityType ()).classInfo_ [(int)VU_CLASS.VU_STYPE];
		}

		public byte GetSPType ()
		{
			return (EntityType ()).classInfo_ [(int)VU_CLASS.VU_SPTYPE];
		}

		public CampaignTime GetSpottedTime ()
		{
			return spotTime;
		}

		public int GetSpotted (Team t)
		{throw new NotImplementedException();}

		public int GetIdentified (Team t)
		{
			return (spotted >> (t + 8)) & 0x01;
		} // 2002-02-11 ADDED BY S.G. Getter to know if the target is identified or not.

		// Setters
		public void SetLocation (GridIndex x, GridIndex y)
		{throw new NotImplementedException();}

		public void SetAltitude (int alt)
		{throw new NotImplementedException();}

		public void SetSpottedTime (CampaignTime t)
		{
			spotTime = t;
		}

		public void SetSpotted (Team t, CampaignTime time, int identified = 0)
		{throw new NotImplementedException();} // 2002-02-11 ADDED S.G. Added identified which defaults to 0 (not identified or don't change)
		public void SetEmitting (int e)
		{throw new NotImplementedException();}

		public void SetAggregate (int a)
		{throw new NotImplementedException();}

		public virtual void SetJammed (int j)
		{throw new NotImplementedException();}

		public void SetTacan (int t)
		{throw new NotImplementedException();}

		public void SetChecked ()
		{
			local_flags |= CBC_ENUM.CBC_CHECKED;
		}

		public void UnsetChecked ()
		{
			local_flags &= ~CBC_ENUM.CBC_CHECKED;
		}

		public void SetInterest ()
		{
			local_flags |= CBC_ENUM.CBC_INTEREST;
		}

		public void UnsetInterest ()
		{
			local_flags &= ~CBC_ENUM.CBC_INTEREST;
		}

		public void SetAwake (int d)
		{throw new NotImplementedException();}

		public void SetInPackage (int p)
		{throw new NotImplementedException();}

		public void SetDelta (int d)
		{throw new NotImplementedException();}

		public void SetInSimLists (int l)
		{throw new NotImplementedException();}

		public void SetReserved (int r)
		{throw new NotImplementedException();}
	}
}
