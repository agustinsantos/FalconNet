using System;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using Objective= FalconNet.Campaign.ObjectiveClass;

namespace FalconNet.Campaign
{

	// =======================
	// Campaign Objectives ADT
	// =======================
	public struct CampObjectiveTransmitDataType
	{
		CampaignTime last_repair;	// Last time this objective got something repaired
		short aiscore;		// Used for scoring junque
		ulong obj_flags;		// Transmitable flags
		byte supply;			// Amount of supply going through here
		byte fuel;			// Amount of fuel going through here 
		byte losses;			// Amount of supply/fuel losses (in percentage)
		byte status;			// % operational
		byte priority;		// Target's general priority
		byte[] fstatus;		// Array of feature statuses (was [((FEATURES_PER_OBJ*2)+7)/8])
	};

	public struct CampObjectiveStaticDataType
	{
		short nameid;			// Index into name table
		short local_data;		// Local AI data dump
		VU_ID parent;			// ID of parent SO or PO
		Control first_owner;	// Origional objective owner
		byte links;			// Number of links
		RadarRangeClass radar_data;		// Data on what a radar stationed here can see
		ObjClassDataType class_data;		// Pointer to class data
	};

	public class CampObjectiveLinkDataType
	{
		byte[] costs = new byte[MOVEMENT_TYPES];	// Cost to go here, depending on movement type
		VU_ID id;
	};

	public class ObjectiveClass : CampBaseClass
	{
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap fixed size pool
		void *operator new(size_t size) { ShiAssert( size == sizeof(ObjectiveClass) ); return MemAllocFS(pool);	};
		void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
		static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(ObjectiveClass), 2500, 0 ); };
		static void ReleaseStorage()	{ MemPoolFree( pool ); };
		static MEM_POOL	pool;
#endif

		private CampObjectiveTransmitDataType obj_data;
		private int dirty_objective;
		public CampObjectiveStaticDataType static_data;
		public CampObjectiveLinkDataType link_data;		// The actual link data (was [OBJ_MAX_NEIGHBORS])
		public ATCBrain brain;

		// access functions
		public ulong GetObjFlags ()
		{ throw new NotImplementedException();}

		public void ClearObjFlags (ulong flags)
		{
			obj_data.obj_flags &= ~(flags);
		}

		public void SetObjFlags (ulong flags)
		{
			obj_data.obj_flags |= (flags);
		}

		// constructors
		public ObjectiveClass (int type)
		{
			throw new NotImplementedException ();
		}

		public ObjectiveClass (VU_BYTE[]  stream)
		{
			throw new NotImplementedException ();
		}
		//public virtual ~ObjectiveClass();
		public virtual int SaveSize ()
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
		public virtual VU_ERRCODE Handle (VuFullUpdateEvent  evnt)
		{
			throw new NotImplementedException ();
		}

		// Required pure virtuals handled by objective.h
		public virtual void SendDeaggregateData (VuTargetEntity p)
		{
			throw new NotImplementedException ();
		}

		public virtual int RecordCurrentState (FalconSessionEntity  f, int i)
		{
			throw new NotImplementedException ();
		}

		public virtual int Deaggregate (FalconSessionEntity  session)
		{
			throw new NotImplementedException ();
		}

		public virtual int Reaggregate (FalconSessionEntity  session)
		{
			throw new NotImplementedException ();
		}

		public virtual int TransferOwnership (FalconSessionEntity  session)
		{
			throw new NotImplementedException ();
		}

		public virtual int Wake ()
		{
			throw new NotImplementedException ();
		}

		public virtual int Sleep ()
		{
			throw new NotImplementedException ();
		}

		public virtual void InsertInSimLists (float cameraX, float cameraY)
		{
			throw new NotImplementedException ();
		}

		public virtual void RemoveFromSimLists ()
		{
			throw new NotImplementedException ();
		}

		public virtual void DeaggregateFromData (int size, byte[] data)
		{
			throw new NotImplementedException ();
		}

		public virtual void ReaggregateFromData (int size, byte[] data)
		{
			throw new NotImplementedException ();
		}

		public virtual void TransferOwnershipFromData (int size, byte[] data)
		{
			throw new NotImplementedException ();
		}

		public virtual MoveType GetMovementType ()
		{
			return NoMove;
		}

		public virtual int ApplyDamage (FalconCampWeaponsFire cwfm, byte b)
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

		public virtual byte[] GetDamageModifiers ()
		{
			throw new NotImplementedException ();
		}

		public virtual string GetName (string buffer, int size, int obj)
		{
			throw new NotImplementedException ();
		}

		public virtual string GetFullName (string buffer, int size, int obj)
		{
			throw new NotImplementedException ();
		}

		public virtual int GetHitChance (int mt, int range)
		{
			throw new NotImplementedException ();
		}

		public virtual int GetAproxHitChance (int mt, int range)
		{
			throw new NotImplementedException ();
		}

		public virtual int GetCombatStrength (int mt, int range)
		{
			throw new NotImplementedException ();
		}

		public virtual int GetAproxCombatStrength (int mt, int range)
		{
			throw new NotImplementedException ();
		}

		public virtual int GetWeaponRange (int mt, FalconEntity  target = null)
		{
			throw new NotImplementedException ();
		} // 2008-03-08 ADDED SECOND DEFAULT PARM
		public virtual int GetAproxWeaponRange (int mt)
		{
			throw new NotImplementedException ();
		}

		public virtual int GetDetectionRange (int mt)
		{
			throw new NotImplementedException ();
		}						// Takes into account emitter status
		public virtual int GetElectronicDetectionRange (int mt)
		{
			throw new NotImplementedException ();
		}			// Max Electronic detection range, even if turned off
		public virtual int CanDetect (FalconEntity  ent)
		{
			throw new NotImplementedException ();
		}					// Nonzero if this entity can see ent
		public virtual int OnGround ()
		{
			return true;
		}

		public virtual int GetRadarMode ()
		{
			throw new NotImplementedException ();
		}

		public virtual int GetRadarType ()
		{
			throw new NotImplementedException ();
		}

		// These are only really relevant for sam/airdefense/radar entities
		public virtual int GetNumberOfArcs ()
		{
			throw new NotImplementedException ();
		}

		public virtual float GetArcRatio (int anum)
		{
			throw new NotImplementedException ();
		}

		public virtual float GetArcRange (int anum)
		{
			throw new NotImplementedException ();
		}

		public virtual void GetArcAngle (int anum, ref float a1, ref float a2)
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
		public virtual bool IsObjective ()
		{
			return true;
		}

		public bool IsFrontline ()
		{
			return (int)(O_FRONTLINE & obj_data.obj_flags);
		}

		public bool IsSecondline ()
		{
			return (int)(O_SECONDLINE & obj_data.obj_flags);
		}

		public bool IsThirdline ()
		{
			return (int)(O_THIRDLINE & obj_data.obj_flags);
		}

		public bool IsNearfront ()
		{
			return (int)((O_THIRDLINE | O_SECONDLINE | O_FRONTLINE) & obj_data.obj_flags);
		}

		public bool IsBeach ()
		{
			return (int)(O_BEACH & obj_data.obj_flags);
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
			return (int)(O_IS_GCI & obj_data.obj_flags);
		}	// 2002-02-13 ADDED BY S.G.
		public int HasNCTR ()
		{
			return (int)(O_HAS_NCTR & obj_data.obj_flags);
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

		public int ManualSet ()
		{
			return obj_data.obj_flags & O_MANUAL_SET;
		}

		public void SetJammed (int j)
		{
			throw new NotImplementedException ();
		}

		public int Jammed ()
		{
			return obj_data.obj_flags & O_JAMMED;
		}

		public void SetSamSite (int j)
		{
			throw new NotImplementedException ();
		}

		public int SamSite ()
		{
			return obj_data.obj_flags & O_SAM_SITE;
		}

		public void SetArtillerySite (int j)
		{
			throw new NotImplementedException ();
		}

		public int ArtillerySite ()
		{
			return obj_data.obj_flags & O_ARTILLERY_SITE;
		}

		public void SetAmbushCAPSite (int j)
		{
			throw new NotImplementedException ();
		}

		public int AmbushCAPSite ()
		{
			return obj_data.obj_flags & O_AMBUSHCAP_SITE;
		}

		public void SetBorderSite (int j)
		{
			throw new NotImplementedException ();
		}

		public int BorderSite ()
		{
			return obj_data.obj_flags & O_BORDER_SITE;
		}

		public void SetMountainSite (int j)
		{
			throw new NotImplementedException ();
		}

		public int MountainSite ()
		{
			return obj_data.obj_flags & O_MOUNTAIN_SITE;
		}

		public void SetCommandoSite (int j)
		{
			throw new NotImplementedException ();
		}

		public int CommandoSite ()
		{
			return obj_data.obj_flags & O_COMMANDO_SITE;
		}

		public void SetFlatSite (int j)
		{
			throw new NotImplementedException ();
		}

		public int FlatSite ()
		{
			return obj_data.obj_flags & O_FLAT_SITE;
		}

		public void SetRadarSite (int j)
		{
			throw new NotImplementedException ();
		}

		public int RadarSite ()
		{
			return obj_data.obj_flags & O_RADAR_SITE;
		}

		public void SetAbandoned (int t)
		{
			throw new NotImplementedException ();
		}

		public int Abandoned ()
		{
			return obj_data.obj_flags & O_ABANDONED;
		}

		public void SetNeedRepair (int t)
		{
			throw new NotImplementedException ();
		}

		public int NeedRepair ()
		{
			return obj_data.obj_flags & O_NEED_REPAIR;
		}

		// Dirty Functions
		public void MakeObjectiveDirty (Dirty_Objective bits, Dirtyness score)
		{
			throw new NotImplementedException ();
		}

		public void WriteDirty (ref byte[] stream)
		{
			throw new NotImplementedException ();
		}

		public void ReadDirty (ref byte[] stream)
		{
			throw new NotImplementedException ();
		}

		// Objective data stuff
		public virtual void SetOwner (Control c)
		{
			CampBaseClass.SetOwner (c);
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
			obj_data.priority = p;
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
			if (obj_data.status > s)
				obj_data.last_repair = Camp_GetCurrentTime ();
			obj_data.status = s;
			MakeObjectiveDirty (DIRTY_STATUS, DDP [180].priority);
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
			return link_data [n].costs [t];
		}

		public Control GetObjectiveOldown ()
		{
			return static_data.first_owner;
		}

		public ObjectiveClass GetObjectiveParent ()
		{
			return FindObjective (static_data.parent);
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
			return static_data.class_data->Features;
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

	public static class ObjectivStatic
	{
		// =======================
		// Transmitable flags
		// =======================

		public const int O_FRONTLINE = 0x1;
		public const int O_SECONDLINE = 0x2;
		public const int O_THIRDLINE = 0x4;
		public const int O_B3 = 0x8;
		public const int O_JAMMED = 0x10;
		public const int O_BEACH = 0x20;
		public const int O_B1 = 0x40;
		public const int O_B2 = 0x80;
		public const int O_MANUAL_SET = 0x100;
		public const int O_MOUNTAIN_SITE = 0x200;
		public const int O_SAM_SITE = 0x400;
		public const int O_ARTILLERY_SITE = 0x800;
		public const int O_AMBUSHCAP_SITE = 0x1000;
		public const int O_BORDER_SITE = 0x2000;
		public const int O_COMMANDO_SITE = 0x4000;
		public const int O_FLAT_SITE = 0x8000;
		public const int O_RADAR_SITE = 0x10000;
		public const int O_NEED_REPAIR = 0x20000;
		public const int O_EMPTY1 = 0x40000;
		public const int O_EMPTY2 = 0x80000;
		public const int O_ABANDONED = 0x100000;
		// 2002-02-13 ADDED BY MN for Sylvain's new Identify
		public const int O_HAS_NCTR = 0x200000;
		public const int O_IS_GCI = 0x400000;

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
