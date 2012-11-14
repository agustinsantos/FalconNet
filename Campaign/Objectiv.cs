using System;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using Objective= FalconNet.Campaign.ObjectiveClass;
using Unit=FalconNet.Campaign.UnitClass;
using VU_BYTE=System.Byte;
//using VU_ERRCODE=System.Int32;

namespace FalconNet.Campaign
{

	// =======================
	// Campaign Objectives ADT
	// =======================
	public struct CampObjectiveTransmitDataType
	{
		public CampaignTime last_repair;	// Last time this objective got something repaired
		public short aiscore;		// Used for scoring junque
		public O_FLAGS obj_flags;		// Transmitable flags
		public byte supply;			// Amount of supply going through here
		public byte fuel;			// Amount of fuel going through here 
		public byte losses;			// Amount of supply/fuel losses (in percentage)
		public byte status;			// % operational
		public byte priority;		// Target's general priority
		public byte[] fstatus;		// Array of feature statuses (was [((FEATURES_PER_OBJ*2)+7)/8])
	};

	public struct CampObjectiveStaticDataType
	{
		public short nameid;			// Index into name table
		public short local_data;		// Local AI data dump
		public VU_ID parent;			// ID of parent SO or PO
		public Control first_owner;	// Origional objective owner
		public byte links;			// Number of links
		public RadarRangeClass radar_data;		// Data on what a radar stationed here can see
		public ObjClassDataType class_data;		// Pointer to class data
	};

	public class CampObjectiveLinkDataType
	{
		public byte[] costs = new byte[(int)MoveType.MOVEMENT_TYPES];	// Cost to go here, depending on movement type
		public VU_ID id;
	};

	public class ObjectiveClass : CampBaseClass
	{
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap fixed size pool
		void *operator new(size_t size) { Debug.Assert( size == sizeof(ObjectiveClass) ); return MemAllocFS(pool);	};
		void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
		static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(ObjectiveClass), 2500, 0 ); };
		static void ReleaseStorage()	{ MemPoolFree( pool ); };
		static MEM_POOL	pool;
#endif

		private CampObjectiveTransmitDataType obj_data;
		private int dirty_objective;
		public CampObjectiveStaticDataType static_data;
		public CampObjectiveLinkDataType[] link_data;		// The actual link data (was [OBJ_MAX_NEIGHBORS])
		public ATCBrain brain;

		// access functions
		public ulong GetObjFlags ()
		{ throw new NotImplementedException();}

		public void ClearObjFlags (O_FLAGS flags)
		{
			obj_data.obj_flags &= ~(flags);
		}

		public void SetObjFlags (O_FLAGS flags)
		{
			obj_data.obj_flags |= (flags);
		}

		// constructors
		public ObjectiveClass (int type) : base(type)
		{
			throw new NotImplementedException ();
		}

		public ObjectiveClass (VU_BYTE[]  stream) : base(stream)
		{
			throw new NotImplementedException ();
		}
		//public virtual ~ObjectiveClass();
		public override int SaveSize ()
		{
			throw new NotImplementedException ();
		}

		public virtual int SaveSize (int toDisk)
		{
			throw new NotImplementedException ();
		}

		public virtual int Save (VU_BYTE[]  stream)
		{
			throw new NotImplementedException ();
		}

		public virtual int Save (VU_BYTE[]  stream, int toDisk)
		{
			throw new NotImplementedException ();
		}

		public void UpdateFromData (VU_BYTE[]  stream)
		{
			throw new NotImplementedException ();
		}

		// event Handlers
		public override VU_ERRCODE Handle (VuFullUpdateEvent  evnt)
		{
			throw new NotImplementedException ();
		}

		// Required pure virtuals handled by objective.h
		public override void SendDeaggregateData (VuTargetEntity p)
		{
			throw new NotImplementedException ();
		}

		public override int RecordCurrentState (FalconSessionEntity  f, int i)
		{
			throw new NotImplementedException ();
		}

		public override int Deaggregate (FalconSessionEntity  session)
		{
			throw new NotImplementedException ();
		}

		public override int Reaggregate (FalconSessionEntity  session)
		{
			throw new NotImplementedException ();
		}

		public override int TransferOwnership (FalconSessionEntity  session)
		{
			throw new NotImplementedException ();
		}

		public override int Wake ()
		{
			throw new NotImplementedException ();
		}

		public override int Sleep ()
		{
			throw new NotImplementedException ();
		}

		public override void InsertInSimLists (float cameraX, float cameraY)
		{
			throw new NotImplementedException ();
		}

		public override void RemoveFromSimLists ()
		{
			throw new NotImplementedException ();
		}

		public override void DeaggregateFromData (int size, byte[] data)
		{
			throw new NotImplementedException ();
		}

		public override void ReaggregateFromData (int size, byte[] data)
		{
			throw new NotImplementedException ();
		}

		public override void TransferOwnershipFromData (int size, byte[] data)
		{
			throw new NotImplementedException ();
		}

		public override MoveType GetMovementType ()
		{
			return MoveType.NoMove;
		}

		public override int ApplyDamage (FalconCampWeaponsFire cwfm, byte b)
		{
			throw new NotImplementedException ();
		}

		public virtual int ApplyDamage (DamType d, int[] str, int where, short flags)
		{
			throw new NotImplementedException ();
		}

		public virtual int DecodeDamageData (byte[] data, Unit shooter, FalconDeathMessage dtm)
		{
			throw new NotImplementedException ();
		}

		public override byte[] GetDamageModifiers ()
		{
			throw new NotImplementedException ();
		}

		public override string GetName (string buffer, int size, int obj)
		{
			throw new NotImplementedException ();
		}

		public override string GetFullName (string buffer, int size, int obj)
		{
			throw new NotImplementedException ();
		}

		public override int GetHitChance (int mt, int range)
		{
			throw new NotImplementedException ();
		}

		public override int GetAproxHitChance (int mt, int range)
		{
			throw new NotImplementedException ();
		}

		public override int GetCombatStrength (int mt, int range)
		{
			throw new NotImplementedException ();
		}

		public override int GetAproxCombatStrength (int mt, int range)
		{
			throw new NotImplementedException ();
		}

		public override int GetWeaponRange (int mt, FalconEntity  target = null)
		{
			throw new NotImplementedException ();
		} // 2008-03-08 ADDED SECOND DEFAULT PARM
		public override int GetAproxWeaponRange (int mt)
		{
			throw new NotImplementedException ();
		}

		public override int GetDetectionRange (int mt)
		{
			throw new NotImplementedException ();
		}						// Takes into account emitter status
		public override int GetElectronicDetectionRange (int mt)
		{
			throw new NotImplementedException ();
		}			// Max Electronic detection range, even if turned off
		public override int CanDetect (FalconEntity  ent)
		{
			throw new NotImplementedException ();
		}					// Nonzero if this entity can see ent
		public override bool OnGround ()
		{
			return true;
		}

		public override FEC_RADAR GetRadarMode ()
		{
			throw new NotImplementedException ();
		}

		public override Radar_types GetRadarType ()
		{
			throw new NotImplementedException ();
		}

		// These are only really relevant for sam/airdefense/radar entities
		public override int GetNumberOfArcs ()
		{
			throw new NotImplementedException ();
		}

		public override float GetArcRatio (int anum)
		{
			throw new NotImplementedException ();
		}

		public override float GetArcRange (int anum)
		{
			throw new NotImplementedException ();
		}

		public override void GetArcAngle (int anum, ref float a1, ref float a2)
		{
			throw new NotImplementedException ();
		}

		public int SiteCanDetect (FalconEntity  ent)
		{
			throw new NotImplementedException ();
		}

		public float GetSiteRange (FalconEntity  ent)
		{
			throw new NotImplementedException ();
		}

		// core functions
		public void SendObjMessage (VU_ID from, short mes, short d1, short d2, short d3)
		{
			throw new NotImplementedException ();
		}

		public void DisposeObjective ()
		{
			throw new NotImplementedException ();
		}

		public void DamageObjective (int loss)
		{
			throw new NotImplementedException ();
		}

		public void AddObjectiveNeighbor (Objective o, byte[] c)
		{
			throw new NotImplementedException ();
		} //TODO[MOVEMENT_TYPES]);
		public void RemoveObjectiveNeighbor (int n)
		{
			throw new NotImplementedException ();
		}

		public void SetNeighborCosts (int num, byte[] c)
		{
			throw new NotImplementedException ();
		} // TODO [MOVEMENT_TYPES]);
		public override bool IsObjective ()
		{
			return true;
		}

		public bool IsFrontline ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_FRONTLINE);
		}

		public bool IsSecondline ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_SECONDLINE);
		}

		public bool IsThirdline ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_THIRDLINE);
		}

		public bool IsNearfront ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_THIRDLINE | O_FLAGS.O_SECONDLINE | O_FLAGS.O_FRONTLINE);
		}

		public bool IsBeach ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_BEACH);
		}

		public int IsPrimary ()
		{
			throw new NotImplementedException ();
		}

		public int IsSecondary ()
		{
			throw new NotImplementedException ();
		}

		public int IsSupplySource ()
		{
			throw new NotImplementedException ();
		}

		public int IsGCI ()
		{
			return (int)(O_FLAGS.O_IS_GCI & obj_data.obj_flags);
		}	// 2002-02-13 ADDED BY S.G.
		public int HasNCTR ()
		{
			return (int)(O_FLAGS.O_HAS_NCTR & obj_data.obj_flags);
		}	// 2002-02-13 ADDED BY S.G.
		public int HasRadarRanges ()
		{
			throw new NotImplementedException ();
		}

		public void UpdateObjectiveLists ()
		{
			throw new NotImplementedException ();
		}

		public void ResetLinks ()
		{
			throw new NotImplementedException ();
		}

		public void Dump ()
		{
			throw new NotImplementedException ();
		}

		public void Repair ()
		{
			throw new NotImplementedException ();
		}

		// Flag setting stuff
		public void SetManual (int s)
		{
			throw new NotImplementedException ();
		}

		public bool ManualSet ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_MANUAL_SET);
		}

		public override  void SetJammed (int j)
		{
			throw new NotImplementedException ();
		}

		public bool Jammed ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_JAMMED);
		}

		public void SetSamSite (int j)
		{
			throw new NotImplementedException ();
		}

		public bool SamSite ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_SAM_SITE);
		}

		public void SetArtillerySite (int j)
		{
			throw new NotImplementedException ();
		}

		public bool ArtillerySite ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_ARTILLERY_SITE);
		}

		public void SetAmbushCAPSite (int j)
		{
			throw new NotImplementedException ();
		}

		public bool AmbushCAPSite ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_AMBUSHCAP_SITE);
		}

		public void SetBorderSite (int j)
		{
			throw new NotImplementedException ();
		}

		public bool BorderSite ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_BORDER_SITE);
		}

		public void SetMountainSite (int j)
		{
			throw new NotImplementedException ();
		}

		public bool MountainSite ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_MOUNTAIN_SITE);
		}

		public void SetCommandoSite (int j)
		{
			throw new NotImplementedException ();
		}

		public bool CommandoSite ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_COMMANDO_SITE);
		}

		public void SetFlatSite (int j)
		{
			throw new NotImplementedException ();
		}

		public bool FlatSite ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_FLAT_SITE);
		}

		public void SetRadarSite (int j)
		{
			throw new NotImplementedException ();
		}

		public bool RadarSite ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_RADAR_SITE);
		}

		public void SetAbandoned (int t)
		{
			throw new NotImplementedException ();
		}

		public bool Abandoned ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_ABANDONED);
		}

		public void SetNeedRepair (int t)
		{
			throw new NotImplementedException ();
		}

		public bool NeedRepair ()
		{
			return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_NEED_REPAIR);
		}

		// Dirty Functions
		public void MakeObjectiveDirty (Dirty_Objective bits, Dirtyness score)
		{
			throw new NotImplementedException ();
		}

		public override void WriteDirty (byte[] stream)
		{
			throw new NotImplementedException ();
		}

		public override void ReadDirty (byte[] stream)
		{
			throw new NotImplementedException ();
		}

		// Objective data stuff
		public override void SetOwner (Control c)
		{
			base.SetOwner (c);
			SetDelta (1);
		}

		public void SetObjectiveOldown (Control c)
		{
			static_data.first_owner = c;
		}

		public void SetObjectiveParent (VU_ID p)
		{
			static_data.parent = p;
		}

		public void SetObjectiveNameID (short n)
		{
			static_data.nameid = n;
		}

		public void SetObjectiveName (string name)
		{
			throw new NotImplementedException ();
		}

		public void SetObjectivePriority (PriorityLevel p)
		{
#if TODO		
			obj_data.priority = p;
#endif 
			throw new NotImplementedException();
		}

		public void SetObjectiveScore (short score)
		{
			obj_data.aiscore = score;
		}

		public void SetObjectiveRepairTime (CampaignTime t)
		{
			obj_data.last_repair = t;
		}
		// JB 000811
		// Set the last repair time to now if some damage has been taken
		//void SetObjectiveStatus (byte s)						{	obj_data.status = s; MakeObjectiveDirty (DIRTY_STATUS, SEND_NOW); }
		public void SetObjectiveStatus (byte s)
		{
#if TODO		
			if (obj_data.status > s)
				obj_data.last_repair = Camp_GetCurrentTime ();
			obj_data.status = s;
			MakeObjectiveDirty (Dirty_Objective.DIRTY_STATUS, DDP [180].priority);
#endif
			throw new NotImplementedException();
		}
		//void SetObjectiveStatus (byte s)						{	if (obj_data.status > s) obj_data.last_repair = Camp_GetCurrentTime(); obj_data.status = s; MakeObjectiveDirty (DIRTY_STATUS, SEND_NOW); }
		// JB 000811
		public void SetObjectiveSupply (byte s)
		{
			obj_data.supply = s;
		}

		public void SetObjectiveFuel (byte f)
		{
			obj_data.fuel = f;
		}

		public void SetObjectiveSupplyLosses (byte l)
		{
			obj_data.losses = l;
		}

		public void SetObjectiveType (ObjectiveType t)
		{
			throw new NotImplementedException ();
		}

		void SetObjectiveSType (byte s)
		{
			throw new NotImplementedException ();
		}

		public void SetObjectiveClass (int dindex)
		{
			throw new NotImplementedException ();
		}

		public void SetFeatureStatus (int f, int n)
		{
			throw new NotImplementedException ();
		}

		public void SetFeatureStatus (int f, int n, int from)
		{
			throw new NotImplementedException ();
		}

		public VU_ID GetNeighborId (int n)
		{
			return link_data [n].id;
		}

		public Objective GetNeighbor (int n)
		{
			throw new NotImplementedException ();
		}

		public float GetNeighborCost (int n, MoveType t)
		{
			return link_data [n].costs [(int)t];
		}

		public Control GetObjectiveOldown ()
		{
			return static_data.first_owner;
		}

		public ObjectiveClass GetObjectiveParent ()
		{
			return FindStatic.FindObjective (static_data.parent);
		}

		public ObjectiveClass GetObjectiveSecondary ()
		{
			throw new NotImplementedException ();
		}

		public ObjectiveClass GetObjectivePrimary ()
		{
			throw new NotImplementedException ();
		}

		public VU_ID GetObjectiveParentID ()
		{
			return static_data.parent;
		}

		public int GetObjectiveNameID ()
		{
			return static_data.nameid;
		}

		public int NumLinks ()
		{
			return static_data.links;
		}

		public short GetObjectivePriority ()
		{
			return obj_data.priority;
		}

		public byte GetObjectiveStatus ()
		{
			return obj_data.status;
		}

		public int GetObjectiveScore ()
		{
			return obj_data.aiscore;
		}

		public CampaignTime GetObjectiveRepairTime ()
		{
			return obj_data.last_repair;
		}

		public short GetObjectiveSupply ()
		{
			return obj_data.supply;
		}

		public short GetObjectiveFuel ()
		{
			return obj_data.fuel;
		}

		public short GetObjectiveSupplyLosses ()
		{
			return obj_data.losses;
		}

		public short GetObjectiveDataRate ()
		{throw new NotImplementedException();}

		public short GetAdjustedDataRate ()
		{
			throw new NotImplementedException ();
		}

		public short GetTotalFeatures ()
		{
			return static_data.class_data.Features;
		}

		public int GetFeatureStatus (int f)
		{throw new NotImplementedException();}

		public int GetFeatureValue (int f)
		{throw new NotImplementedException();}

		public int GetFeatureRepairTime (int f)
		{throw new NotImplementedException();}

		public int GetFeatureID (int f)
		{throw new NotImplementedException();}

		public int GetFeatureOffset (int f, ref float x, ref float y, ref float z)
		{throw new NotImplementedException();}

		public ObjClassDataType GetObjectiveClassData ()
		{throw new NotImplementedException();}

		public string GetObjectiveClassName ()
		{throw new NotImplementedException();}
		//		int RoE (VuEntity* e, int type);

		public byte GetExpectedStatus (int hours)
		{throw new NotImplementedException();}

		public int GetRepairTime (int status)
		{throw new NotImplementedException();}

		public byte GetBestTarget ()
		{throw new NotImplementedException();}

		public void ResetObjectiveStatus ()
		{throw new NotImplementedException();}

		public void RepairFeature (int f)
		{throw new NotImplementedException();}

		public void RecalculateParent ()
		{throw new NotImplementedException();}
	}

	// =======================
	// Transmitable flags
	// =======================
	[Flags]
	public enum O_FLAGS : ulong
	{		
		O_FRONTLINE = 0x1,
		O_SECONDLINE = 0x2,
		O_THIRDLINE = 0x4,
		O_B3 = 0x8,
		O_JAMMED = 0x10,
		O_BEACH = 0x20,
		O_B1 = 0x40,
		O_B2 = 0x80,
		O_MANUAL_SET = 0x100,
		O_MOUNTAIN_SITE = 0x200,
		O_SAM_SITE = 0x400,
		O_ARTILLERY_SITE = 0x800,
		O_AMBUSHCAP_SITE = 0x1000,
		O_BORDER_SITE = 0x2000,
		O_COMMANDO_SITE = 0x4000,
		O_FLAT_SITE = 0x8000,
		O_RADAR_SITE = 0x10000,
		O_NEED_REPAIR = 0x20000,
		O_EMPTY1 = 0x40000,
		O_EMPTY2 = 0x80000,
		O_ABANDONED = 0x100000,
		// 2002-02-13 ADDED BY MN for Sylvain's new Identify
		O_HAS_NCTR = 0x200000,
		O_IS_GCI = 0x400000 
	}
	
	public static class ObjectivStatic
	{


		// =======================
		// Random externals
		// =======================

		public static ObjectiveClass FindObjective (VU_ID id)
		{throw new NotImplementedException();}

		// ================================
		// Objective Externals
		// ================================

		// ================================
		// Inline functions
		// ================================

		// ---------------------------------------
		// External Function Declarations
		// ---------------------------------------

		public static ObjectiveClass NewObjective ()
		{throw new NotImplementedException();}

		public static ObjectiveClass NewObjective (short tid, VU_BYTE[]  stream)
		{throw new NotImplementedException();}

		public static ObjectiveClass NewObjective (short tid, VU_BYTE[]  stream, int fromDisk)
		{ throw new NotImplementedException();}

		public static int LoadBaseObjectives (string scenario)
		{ throw new NotImplementedException();}

		public static int LoadObjectiveDeltas (string savefile)
		{ throw new NotImplementedException();}

		public static void SaveBaseObjectives (string scenario)
		{ throw new NotImplementedException();}

		public static void SaveObjectiveDeltas (string savefile)
		{ throw new NotImplementedException();}

		public static ObjectiveClass GetObjectiveByID (int ID)
		{ throw new NotImplementedException();}

		public static int BestRepairFeature (ObjectiveClass o, int[] hours)
		{ throw new NotImplementedException();}

		public static int BestTargetFeature (ObjectiveClass o, byte[] targeted)
		{ throw new NotImplementedException();}

		public static void RepairObjectives ()
		{ throw new NotImplementedException();}

		public static DamageDataType GetDamageType (ObjectiveClass o, int f)
		{ throw new NotImplementedException();}

		public static F4PFList GetChildObjectives (ObjectiveClass o, int maxdist, int flags)
		{ throw new NotImplementedException();}

		public static ObjectiveClass GetFirstObjective (F4LIt l)
		{ throw new NotImplementedException();}

		public static ObjectiveClass GetNextObjective (F4LIt l)
		{ throw new NotImplementedException();}

		public static ObjectiveClass GetFirstObjective (VuGridIterator  l)
		{ throw new NotImplementedException();}

		public static ObjectiveClass GetNextObjective (VuGridIterator  l)
		{ throw new NotImplementedException();}

		public static void CaptureObjective (ObjectiveClass co, Control who, Unit u = null)
		{ throw new NotImplementedException();}

		public static int EncodeObjectiveDeltas (VU_BYTE[]  stream, FalconSessionEntity  owner)
		{ throw new NotImplementedException();}

		public static int DecodeObjectiveDeltas (VU_BYTE[]  stream, FalconSessionEntity  owner)
		{ throw new NotImplementedException();}
	}

}
