using System;
using FalconNet.VU;
using VU_BYTE=System.Byte;
using Objective=FalconNet.Campaign.ObjectiveClass;
using Team=System.Int32;
using GridIndex = System.Int16;
using FalconNet.FalcLib;
using FalconNet.Common;

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
		public static int[] OrderPriority = new int[(int)GORD.GORD_LAST];		// Update this if new orders are added
	
		private byte				orders;    		// Current orders
		private short				division;		// What division it belongs to (abstract)
		private VU_ID				pobj;			// Primary objective we're assigned to
		private VU_ID				sobj;			// Secondary objective we're attached to
		private VU_ID				aobj;			// Actual objective we're assigned to do something with

		private int					dirty_ground_unit;

	
		// constructors and serial functions
		public GroundUnitClass (int type) : base(type)
		{throw new NotImplementedException();}
		public GroundUnitClass (VU_BYTE[] stream) :base(stream)
		{throw new NotImplementedException();}
		
		public GroundUnitClass(byte[] bytes, ref int offset, int version)
            : base(bytes, ref offset, version)
        {
#if TODO
            orders = bytes[offset];
            offset++;
            division = BitConverter.ToInt16(bytes, offset);
            offset += 2;
            aobj = new VU_ID();
            aobj.num_ = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            aobj.creator_ = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
#endif 
		}
		
		//TODO public virtual ~GroundUnitClass();
		public override int SaveSize () 
		{throw new NotImplementedException();}

		public override int Save (VU_BYTE[] stream) 
		{throw new NotImplementedException();}

		// event Handlers
		public override VU_ERRCODE Handle (VuFullUpdateEvent evnt) 
		{throw new NotImplementedException();}

		// Required pure virtuals handled by GroundUnitClass
		public override MoveType GetMovementType () 
		{throw new NotImplementedException();}

		public override MoveType GetObjMovementType (Objective o, int n) 
		{throw new NotImplementedException();}

		public override int DetectOnMove () 
		{throw new NotImplementedException();}

		public override int ChooseTarget () 
		{throw new NotImplementedException();}

		public override CampaignTime UpdateTime ()
		{
			return new CampaignTime((ulong)AIInput.GROUND_UPDATE_CHECK_INTERVAL * CampaignTime.CampaignSeconds);
		}

		public override bool OnGround ()
		{
			return true;
		}

		public override float Vt ()
		{
			return (Moving () ? 40.0F : 0.0F);
		}

		public override float Kias ()
		{
			return (Moving () ? 40.0F : 0.0F);
		}

		// Access functions
		public byte GetOrders ()
		{
			return orders;
		}

		public short GetDivision ()
		{
			return division;
		}

		public void SetOrders (byte p) 
		{throw new NotImplementedException();}

		public void SetDivision (short p) 
		{throw new NotImplementedException();}

		public void SetPObj (VU_ID p) 
		{throw new NotImplementedException();}

		public void SetSObj (VU_ID p) 
		{throw new NotImplementedException();}

		public void SetAObj (VU_ID p) 
		{throw new NotImplementedException();}

		// Core functions
		public int DetectVs (AircraftClass ac, ref float d, ref int combat, ref int spotted, ref int capture, ref int nomove, ref int estr) 
		{throw new NotImplementedException();}

		public int DetectVs (CampEntity e, ref float d, ref int combat, ref int spotted, ref int capture, ref int nomove, ref int estr) 
		{throw new NotImplementedException();}

		public virtual void SetUnitOrders (byte o)
		{
			orders = o;
		}

		public virtual void SetUnitDivision (short d)
		{
			division = d;
		}

		public override void SetUnitPrimaryObj (VU_ID id)
		{
			pobj = id;
		}

		public override void SetUnitSecondaryObj (VU_ID id)
		{
			sobj = id;
		}

		public override void SetUnitObjective (VU_ID id)
		{
			aobj = id;
		}

		public override int GetUnitOrders ()
		{
			return (int)orders;
		}

		public override int GetUnitDivision ()
		{
			return (int)division;
		}

		public override Objective GetUnitPrimaryObj ()
		{
			return (Objective)VuDatabase.vuDatabase.Find (pobj);
		}

		public override Objective GetUnitSecondaryObj ()
		{
			return (Objective)VuDatabase.vuDatabase.Find (sobj);
		}

		public override Objective GetUnitObjective ()
		{
			return (Objective)VuDatabase.vuDatabase.Find (aobj);
		}

		public override VU_ID GetUnitPrimaryObjID ()
		{
			return pobj;
		}

		public override VU_ID GetUnitSecondaryObjID ()
		{
			return sobj;
		}

		public override VU_ID GetUnitObjectiveID ()
		{
			return aobj;
		}

		public override  int CheckForSurrender () 
		{throw new NotImplementedException();}

		public override  int GetUnitNormalRole () 
		{throw new NotImplementedException();}

		public override  int GetUnitCurrentRole () 
		{throw new NotImplementedException();}

		public override int BuildMission () 
		{throw new NotImplementedException();}

		public void MakeGndUnitDirty (Dirty_Ground_Unit bits, Dirtyness score)
		{throw new NotImplementedException();}

        public override void WriteDirty(byte[] stream, ref int pos)
		{throw new NotImplementedException();}

        public override void ReadDirty(byte[] stream, ref int pos)
		{throw new NotImplementedException();}
		
		// ============================
		// Global functions
		// ============================
		
		public static  char[] DirectionToEnemy (char[] buf, GridIndex x, GridIndex y, Team who) 
		{throw new NotImplementedException();}
		
		public static  void ReorderRallied (Unit u) 
		{throw new NotImplementedException();}
		
		public static  Unit BestElement (Unit u, int at, int role) 
		{throw new NotImplementedException();}
		
		public static  int FindNextBest (int d, int[] pos) 
		{throw new NotImplementedException();}
		
		public static  void ReorganizeUnit (Unit u) 
		{throw new NotImplementedException();}
		
		public static  int BuildGroundWP (Unit u) 
		{throw new NotImplementedException();}
		
		public static  void GetCombatPos (Unit e, int[] positions, char[] ed, ref GridIndex tx, ref GridIndex ty) 
		{throw new NotImplementedException();}
		
		public static  int GetActionFromOrders (int orders) 
		{throw new NotImplementedException();}
		
		public static  int CheckUnitStatus (Unit u) 
		{throw new NotImplementedException();}
		
		public static  int CheckReady (Unit u) 
		{throw new NotImplementedException();}
		
		public static  int CalculateOpposingStrength (Unit u, F4PFList list) 
		{throw new NotImplementedException();}
		
		public static  int SOSecured (Objective o, Team who) 
		{throw new NotImplementedException();}
		
		public static  CampaignHeading GetAlternateHeading (Unit u, GridIndex x, GridIndex y, GridIndex nx, GridIndex ny, CampaignHeading h) 
		{throw new NotImplementedException();}
		
		public static  int ScorePosition (GridIndex x, GridIndex y, GridIndex px, GridIndex py, int position, int ours) 
		{throw new NotImplementedException();}
		
		public static  Objective FindBestPosition (Unit u, Unit e, F4PFList nearlist) 
		{throw new NotImplementedException();}
		
		public static  void ClassifyUnitElements (Unit u, ref int recon, ref int combat, ref int reserve, ref int support) 
		{throw new NotImplementedException();}
		
		public static  int GetPositionOrders (Unit e)
		{throw new NotImplementedException();}
		
		public static  void FindBestCover (GridIndex x, GridIndex y, CampaignHeading h, ref GridIndex cx, ref GridIndex cy, int roadok) 
		{throw new NotImplementedException();}
		
		public static  Objective FindRetreatPath (Unit u, int depth, int flags) 
		{throw new NotImplementedException();}
		
		public static  Unit RequestArtillerySupport (Unit req, Unit target) 
		{throw new NotImplementedException();}
		
		public static  int RequestCAS (int Team, Unit target) 
		{throw new NotImplementedException();}
		
		public static  int RequestSupport (Unit req, Unit target) 
		{throw new NotImplementedException();}
		
		public static  void RequestOCCAS (Unit u, GridIndex x, GridIndex y, CampaignTime time) 
		{throw new NotImplementedException();}
		
		public static  void RequestBAI (Unit u, GridIndex x, GridIndex y, CampaignTime time) 
		{throw new NotImplementedException();}
		
		public static  int GetGroundRole (int orders) 
		{throw new NotImplementedException();}
		
		public static  int GetGroundOrders (int role) 
		{throw new NotImplementedException();}
	}
}

