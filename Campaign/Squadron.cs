using System;
using FalconNet.VU;
using VU_BYTE=System.Byte;
using FalconNet.FalcLib;
using Flight=FalconNet.Campaign.FlightClass;
using FalconNet.Common;
namespace FalconNet.Campaign
{


	// Defines for types of stats update
	public enum ASTAT
	{
		ASTAT_AAKILL		=0,
		ASTAT_AGKILL		=1,
		ASTAT_ASKILL		=2,
		ASTAT_ANKILL		=3,
		ASTAT_MISSIONS		=4,
		ASTAT_PKILL			=5								// Player kill
	}
	

	// =========================
	// Squadron Class
	// =========================
	public class SquadronClass : AirUnitClass
	{
		// Define to flag moving aircraft from reserve
		public const int  UMSG_FROM_RESERVE = 255;
		public const int  SQUADRON_PT_FUEL = 100;				// How many lbs each point of fuel is worth
		public const int  SQUADRON_PT_SUPPLY = 20;					// How many weapon shots each point of supply is worth
		public const int  SQUADRON_MISSIONS_PER_HOUR = 4;					// How many missions we expect each plane to fly per hour
		
		public const int  SQUADRON_SPECIALTY_AA = 1;					// Specialty values
		public const int  SQUADRON_SPECIALTY_AG = 2;

#if USE_SH_POOLS
   
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { Debug.Assert( size == sizeof(SquadronClass) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(SquadronClass), 200, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif

	 
		private long				fuel;							// fuel avail, 100s of lbs
		private byte				specialty;    					// Squadron's specialty
		private byte[]				stores = new byte[Camplib.MAXIMUM_WEAPTYPES];		// # of weapons available
		private PilotClass[]		pilot_data = new PilotClass[PilotStatic.PILOTS_PER_SQUADRON];// Pilot info
		private ulong[]				schedule = new ulong[Camplib.VEHICLES_PER_UNIT];	// Aircraft usage schedule.
		private VU_ID				airbase_id;						// ID of this squadron's airbase/carrier
		private VU_ID				hot_spot;						// ID of 'primary' Primary Objective
		private byte[]				rating = new byte[(int)MissionRollEnum.ARO_OTHER];				// Rating by mission roll
		private short				aa_kills;						// Kill counts (air to air)
		private short				ag_kills;						// (air to ground)
		private short				as_kills;						// (air to static)
		private short				an_kills;						// (air to naval)
		private short				missions_flown;
		private short				mission_score;
		private byte				total_losses;					// Total aircraft losses since start of war
		private byte				pilot_losses;					// Total pilot losses since start of war
		private byte				assigned;						// Assigned to current package
		private byte				squadron_patch;					// ID of this squadron's patch art
		private int					dirty_squadron;
		private CampaignTime		last_resupply_time;				// Last time we received supply/reinforcements
		private byte				last_resupply;					// Number of aircraft we received

	

		// Access Functions
		public byte GetAvailableStores (int i)
		{throw new NotImplementedException();}

		public ulong GetSchedule (int i)
		{
			return schedule [i];
		}

		public VU_ID GetHotSpot ()
		{
			return hot_spot;
		}

		public byte GetRating (int i)
		{
			return rating [i];
		}

		public short GetAAKills ()
		{
			return aa_kills;
		}

		public short GetAGKills ()
		{
			return ag_kills;
		}

		public short GetASKills ()
		{
			return as_kills;
		}

		public short GetANKills ()
		{
			return an_kills;
		}

		public short GetMissionsFlown ()
		{
			return missions_flown;
		}

		public short GetMissionScore ()
		{
			return mission_score;
		}

		public byte GetTotalLosses ()
		{
			return total_losses;
		}

		public byte GetPilotLosses ()
		{
			return pilot_losses;
		}

		public byte GetAssigned ()
		{
			return assigned;
		}

		public byte GetPatchID ()
		{
			return squadron_patch;
		}
		
		public void SetSchedule (int i, ulong l)
		{throw new NotImplementedException();}	// OR ulong into schedule
		public void ClearSchedule (int i)
		{throw new NotImplementedException();}		// set it to 0
		public void ShiftSchedule (int i)
		{throw new NotImplementedException();}		// shift it to the right 1 bit
		public void SetHotSpot (VU_ID i)
		{throw new NotImplementedException();}

		public void SetRating (int i, byte p)
		{throw new NotImplementedException();}

		public void SetAAKills (short s)
		{throw new NotImplementedException();}

		public void SetAGKills (short s)
		{throw new NotImplementedException();}

		public void SetASKills (short s)
		{throw new NotImplementedException();}

		public void SetANKills (short s)
		{throw new NotImplementedException();}

		public void SetMissionsFlown (short s)
		{throw new NotImplementedException();}

		public void SetMissionScore (short s)
		{throw new NotImplementedException();}

		public void SetTotalLosses (byte p)
		{throw new NotImplementedException();}

		public void SetPilotLosses (byte p)
		{throw new NotImplementedException();}

		public void SetAssigned (byte p)
		{throw new NotImplementedException();}

		// Other Functions
		public SquadronClass (int type):base(type)
		{throw new NotImplementedException();}
		
		public SquadronClass (VU_BYTE[] stream):base(stream)
		{throw new NotImplementedException();}
		//TODO public virtual ~SquadronClass();
		public override int SaveSize ()
		{throw new NotImplementedException();}

		public override int Save (VU_BYTE[] stream)
		{throw new NotImplementedException();}

		// event Handlers
		public override VU_ERRCODE Handle (VuFullUpdateEvent evnt)
		{throw new NotImplementedException();}

		// Required pure virtuals
		public override int Reaction (CampEntity c, int i, float f)
		{
			return 0;
		}

		public override int MoveUnit (CampaignTime t)
		{throw new NotImplementedException();}

		public override int ChooseTactic ()
		{
			return 0;
		}

		public override int CheckTactic (int i)
		{
			return 0;
		}

		public override int Real ()
		{
			return 0;
		}

		public override bool IsSquadron ()
		{
			return true;
		}

		public override int GetUnitSupplyNeed (int total)
		{throw new NotImplementedException();}

		public override int GetUnitFuelNeed (int total)
		{throw new NotImplementedException();}

		public override void SupplyUnit (int supply, int fuel)
		{throw new NotImplementedException();}

		// Dirty Data Stuff
		public void MakeSquadronDirty (Dirty_Squadron bits, Dirtyness score)
		{throw new NotImplementedException();}

		public override void WriteDirty (byte[] stream)
		{throw new NotImplementedException();}

		public override void ReadDirty (byte[] stream)
		{throw new NotImplementedException();}

		// Core functions
		public override void UseFuel (long f)
		{throw new NotImplementedException();}

		public override void SetSquadronFuel (long f)
		{throw new NotImplementedException();}

		public override void SetUnitSpecialty (int s)
		{
			specialty = (byte)s;
		}

		public override void SetUnitStores (int w, byte v)
		{throw new NotImplementedException();}

		public override void SetUnitAirbase (VU_ID ID)
		{throw new NotImplementedException();}

		public override void SetLastResupply (int s)
		{throw new NotImplementedException();}

		public override void SetLastResupplyTime (CampaignTime t)
		{
			last_resupply_time = t;
		}

		public override long GetSquadronFuel ()
		{
			return fuel;
		}

		public override int GetUnitSpecialty ()
		{
			return (int)specialty;
		}

		public override byte GetUnitStores (int w)
		{
			return stores [w];
		}

		public override CampaignTime GetLastResupplyTime ()
		{
			return last_resupply_time;
		}

		public override int GetLastResupply ()
		{
			return last_resupply;
		}

		public override CampEntity GetUnitAirbase ()
		{
			return FindStatic.FindEntity (airbase_id);
		}

		public override VU_ID GetUnitAirbaseID ()
		{
			return airbase_id;
		}

		public override void DisposeChildren ()
		{throw new NotImplementedException();}

		public int GetPilotID (int pilot)
		{
			return pilot_data [pilot].pilot_id;
		}

		public PilotClass GetPilotData (int pilot)
		{
			return pilot_data [pilot];
		}
		
#if TODO		
		public PilotInfoClass GetPilotInfo (int pilot)
		{
			return PilotInfo [pilot_data [pilot].pilot_id];
		}
#endif
		public int NumActivePilots ()
		{throw new NotImplementedException();}

		public void InitPilots ()
		{throw new NotImplementedException();}

		public void ReinforcePilots (int max_new_pilots)
		{throw new NotImplementedException();}

		public void SetPilotStatus (int pilot, int s)
		{
			pilot_data [pilot].pilot_status = (byte)s;
		}

		public int GetPilotStatus (int pilot)
		{
			return pilot_data [pilot].pilot_status;
		}

		public void ScoreKill (int pilot, int killtype)
		{throw new NotImplementedException();}

		public void ScoreMission (short missions)
		{
			missions_flown = (short)(missions_flown + missions);
		} // this looks silly but gets rid of warning, since changing the type could invalidate save files
		public void ShiftSchedule ()
		{throw new NotImplementedException();}

		public int FindAvailableAircraft (MissionRequest mis)
		{throw new NotImplementedException();}

		public void ScheduleAircraft (Flight fl, MissionRequest mis)
		{throw new NotImplementedException();}

		public int AssignPilots (Flight fl)
		{throw new NotImplementedException();}

		public void UpdateSquadronStores (short[] weapon, byte[] weapons, int lbsfuel, int planes)
		{throw new NotImplementedException();}
// 2001-12-28 M.N.
		public void ResupplySquadronStores (short[] weapon, byte weapons, int lbsfuel, int planes)
		{throw new NotImplementedException();}
// 2001-07-05 ADDED BY S.G. NEED SOMETHING TO HOLD THE NEW VARIABLE FOR squadron RETASKING ONCE REALLOCATED
		public CampaignTime squadronRetaskAt;
		
		// ============================================
		// Supporting functions
		// ============================================
		
		public static SquadronClass NewSquadron (int type)
		{throw new NotImplementedException();}
	}

	/* TODO
		typedef SquadronClass* Squadron;
			
		class FlightClass;
		typedef FlightClass* Flight;
	 */
}

