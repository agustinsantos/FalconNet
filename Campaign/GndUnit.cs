using System;

namespace FalconNet.Campaign
{
	//
	// This includes Division, Brigade and Battalion classes
	//
	//	==========================================
	// Orders and roles available to ground units
	// ==========================================
	public enum GORD
	{
		GORD_RESERVE		=0,
		GORD_CAPTURE		=1,
		GORD_SECURE		=2,		// Secure all objectives around the assigned objective
		GORD_ASSAULT		=3,		// Amphibious assault
		GORD_AIRBORNE		=4,		// Airborne assault
		GORD_COMMANDO		=5,		// Commando raid - Land behind lines and cause damage
		GORD_DEFEND		=6,
		GORD_SUPPORT		=7,
		GORD_REPAIR		=8,
		GORD_AIRDEFENSE	=9,
		GORD_RECON		=10,
		GORD_RADAR		=11,		// Generally radar units just detecting stuff
		GORD_LAST			=12
	}
	
	public enum GRO
	{
		RO_RESERVE			=0,
		GRO_ATTACK			=1,
		GRO_ASSAULT			=2,
		GRO_AIRBORNE		=3,
		GRO_DEFENSE			=4,
		GRO_AIRDEFENSE		=5,
		GRO_FIRESUPPORT		=6,
		GRO_ENGINEER		=7,
		GRO_RECON			=8,
		GRO_LAST			=9
	}
	
	// =========================
	// Ground Formations
	// =========================
	public enum GFORM
	{
		GFORM_DISPERSED		=	0,			// Scattered / Disorganized
		GFORM_COLUMN		=	1,			// Your standard column
		GFORM_OVERWATCH		=	3,			// Cautious column
		GFORM_WEDGE			=	4,
		GFORM_ECHELON		=	5,
		GFORM_LINE			=	6
	}
	
	public enum FF
	{
		FF_SECONDLINE		=	0x01,
		FF_LOSTOK			=	0x02
	}
		
	// ============================================
	// Ground unit positions
	// ============================================

	public enum GPOS
	{
		GPOS_NONE		=0,
		GPOS_RECON1		=1,
		GPOS_RECON2		=2,
		GPOS_RECON3		=3,
		GPOS_COMBAT1	=4,
		GPOS_COMBAT2	=5,
		GPOS_COMBAT3	=6,
		GPOS_RESERVE1	=7,
		GPOS_RESERVE2	=8,
		GPOS_RESERVE3	=9,
		GPOS_SUPPORT1	=10,
		GPOS_SUPPORT2	=11,
		GPOS_SUPPORT3	=12	
	}
	


	// =========================
	// Ground Unit Class
	// =========================
	public class GroundUnitClass : UnitClass
	{
		public const int WPA_FINAL = 0;
		public const int WPA_ENROUTE = 1;
		public const int MAX_SUPPORT_DIST = 20;
		public const int MAX_NORMAL_DIST = 20;
		public static int[] OrderPriority = new int[GORD_LAST];		// Update this if new orders are added
	
		private uchar				orders;    		// Current orders
		private short				division;		// What division it belongs to (abstract)
		private VU_ID				pobj;			// Primary objective we're assigned to
		private VU_ID				sobj;			// Secondary objective we're attached to
		private VU_ID				aobj;			// Actual objective we're assigned to do something with

		private int					dirty_ground_unit;

	
		// constructors and serial functions
		public GroundUnitClass (int type)
;
		public GroundUnitClass (VU_BYTE **stream);
		//TODO public virtual ~GroundUnitClass();
		public virtual int SaveSize () ;

		public virtual int Save (VU_BYTE **stream);

		// event Handlers
		public virtual VU_ERRCODE Handle (VuFullUpdateEvent *evnt);

		// Required pure virtuals handled by GroundUnitClass
		public virtual MoveType GetMovementType () ;

		public virtual MoveType GetObjMovementType (Objective o, int n);

		public virtual int DetectOnMove () ;

		public virtual int ChooseTarget () ;

		public virtual CampaignTime UpdateTime ()
		{
			return GROUND_UPDATE_CHECK_INTERVAL * CampaignSeconds;
		}

		public virtual int OnGround ()
		{
			return TRUE;
		}

		public virtual float Vt ()
		{
			return (Moving () ? 40.0F : 0.0F);
		}

		public virtual float Kias ()
		{
			return (Moving () ? 40.0F : 0.0F);
		}

		// Access functions
		public uchar GetOrders ()
		{
			return orders;
		}

		public short GetDivision ()
		{
			return division;
		}

		public void SetOrders (uchar p);

		public void SetDivision (short p);

		public void SetPObj (VU_ID p);

		public void SetSObj (VU_ID p);

		public void SetAObj (VU_ID p);

		// Core functions
		public int DetectVs (AircraftClass *ac, float *d, int *combat, int *spotted, int *capture, int *nomove, int *estr);

		public int DetectVs (CampEntity e, float *d, int *combat, int *spotted, int *capture, int *nomove, int *estr);

		public virtual void SetUnitOrders (uchar o)
		{
			orders = o;
		}

		public virtual void SetUnitDivision (short d)
		{
			division = d;
		}

		public void SetUnitPrimaryObj (VU_ID id)
		{
			pobj = id;
		}

		public void SetUnitSecondaryObj (VU_ID id)
		{
			sobj = id;
		}

		public void SetUnitObjective (VU_ID id)
		{
			aobj = id;
		}

		public virtual int GetUnitOrders ()
		{
			return (int)orders;
		}

		public virtual int GetUnitDivision ()
		{
			return (int)division;
		}

		public Objective GetUnitPrimaryObj ()
		{
			return (Objective)vuDatabase->Find (pobj);
		}

		public Objective GetUnitSecondaryObj ()
		{
			return (Objective)vuDatabase->Find (sobj);
		}

		public Objective GetUnitObjective ()
		{
			return (Objective)vuDatabase->Find (aobj);
		}

		public VU_ID GetUnitPrimaryObjID ()
		{
			return pobj;
		}

		public VU_ID GetUnitSecondaryObjID ()
		{
			return sobj;
		}

		public VU_ID GetUnitObjectiveID ()
		{
			return aobj;
		}

		public virtual int CheckForSurrender () ;

		public virtual int GetUnitNormalRole () ;

		public virtual int GetUnitCurrentRole () ;

		public virtual int BuildMission () ;

		public void MakeGndUnitDirty (Dirty_Ground_Unit bits, Dirtyness score);

		public void WriteDirty (byte **stream);

		public void ReadDirty (byte **stream);
		
		// ============================
		// Global functions
		// ============================
		
		public static  char* DirectionToEnemy (char* buf, GridIndex x, GridIndex y, Team who);
		
		public static  void ReorderRallied (Unit u);
		
		public static  Unit BestElement (Unit u, int at, int role);
		
		public static  int FindNextBest (int d, int[] pos);
		
		public static  void ReorganizeUnit (Unit u);
		
		public static  int BuildGroundWP (Unit u);
		
		public static  void GetCombatPos (Unit e, int[] positions, char[] ed, ref GridIndex tx, ref GridIndex ty);
		
		public static  int GetActionFromOrders (int orders);
		
		public static  int CheckUnitStatus (Unit u);
		
		public static  int CheckReady (Unit u);
		
		public static  int CalculateOpposingStrength (Unit u, F4PFList list);
		
		public static  int SOSecured (Objective o, Team who);
		
		public static  int GetActionFromOrders (int orders);
		
		public static  CampaignHeading GetAlternateHeading (Unit u, GridIndex x, GridIndex y, GridIndex nx, GridIndex ny, CampaignHeading h);
		
		public static  int ScorePosition (GridIndex x, GridIndex y, GridIndex px, GridIndex py, int position, int ours);
		
		public static  Objective FindBestPosition (Unit u, Unit e, F4PFList nearlist);
		
		public static  void ClassifyUnitElements (Unit u, int *recon, int *combat, int *reserve, int *support);
		
		public static  int GetPositionOrders (Unit e);
		
		public static  void FindBestCover (GridIndex x, GridIndex y, CampaignHeading h, GridIndex *cx, GridIndex *cy, int roadok);
		
		public static  Objective FindRetreatPath (Unit u, int depth, int flags);
		
		public static  Unit RequestArtillerySupport (Unit req, Unit target);
		
		public static  int RequestCAS (int Team, Unit target);
		
		public static  int RequestSupport (Unit req, Unit target);
		
		public static  void RequestOCCAS (Unit u, GridIndex x, GridIndex y, CampaignTime time);
		
		public static  void RequestBAI (Unit u, GridIndex x, GridIndex y, CampaignTime time);
		
		public static  int GetGroundRole (int orders);
		
		public static  int GetGroundOrders (int role);
	}
}

