using System;
using Objective=FalconNet.Campaign.ObjectiveClass;
using Flight=FalconNet.Campaign.FlightClass;
using Unit=FalconNet.Campaign.UnitClass;
using WayPoint=FalconNet.Campaign.WayPointClass;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using VU_BYTE=System.Byte;
using GridIndex = System.Int16;
using FalconNet.Sim;

namespace FalconNet.Campaign
{


	// ========================================
	// Flight specific mission evaluation flags
	// ========================================
	[Flags]
	public enum FEVAL
	{
		FEVAL_MISSION_STARTED		=0x01,					// Mission is in it's 'critical section'
		FEVAL_GOT_TO_TARGET			=0x02,					// We've arrived at the target
		FEVAL_ON_STATION			=0x04,					// We're in our VOL timewise (not spacewise)
		FEVAL_START_COLD			=0x10,					// not really evaluation flag, start mission from cold, sneaked in so its transfered.... JPO
		
		// 2002-02-12 added by MN 
		FLIGHT_ON_STATION			=0x20					// this is used for player flights. Only get 
	}	
	// AWACS BVR threat warnings when checked in
	public enum PlaneStats
	{
		AIRCRAFT_NOT_ASSIGNED		=0,						// planeStats values
		AIRCRAFT_MISSING			=1,
		AIRCRAFT_DEAD				=2,
		AIRCRAFT_RTB				=3,
		AIRCRAFT_AVAILABLE			=4
	}
	
	// =========================
	// Flight Class
	// =========================
	public class FlightClass :  AirUnitClass
	{
		#if USE_SH_POOLS
		      public // Overload new/delete to use a SmartHeap fixed size pool
		      public void *operator new(size_t size)	{ Debug.Assert( size == sizeof(FlightClass) ); return MemAllocFS(pool);	};
		      public void operator delete(void *mem)	{ if (mem) MemFreeFS(mem); };
		      public static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(FlightClass), 200, 0 ); };
		      public static void ReleaseStorage()		{ MemPoolFree( pool ); };
		      public static MEM_POOL	pool;
		#endif

   
		private int						dirty_flight;				// Which elements are dirty

		private long					fuel_burnt;					// Amount of fuel used since takeoff
		private CampaignTime			last_move;					// Time we moved last
		private CampaignTime			last_combat;				// Last time this entity fired its weapons
		private CampaignTime			time_on_target;				// Time on target
		private CampaignTime			mission_over_time;			// Time off station/target
		private CampaignTime			last_enemy_lock_time;		// Last time an enemy locked us
		private VU_ID					mission_target;				// Our target, or flight we're attached to
		private VU_ID					assigned_target;			// Target we're supposed to IMMEDIATELY attack
		private VU_ID					enemy_locker;				// ID of enemy flight which is locking us
		private WayPointClass			override_wp;				// Divert waypoint or other overriding waypoint
		private LoadoutStruct			loadout;					// A custom loadout from the Payload window
		private byte					loadouts;					// Number of loadouts we have recorded (1 or # of ac)
		private byte					mission;     				// Unit's mission
		private byte					old_mission;				// Previous mission, if we've been diverted
		private byte					last_direction;				// Direction of last move
		private byte					priority;					// Mission priority
		private byte					mission_id;					// Our mission id
		private byte					eval_flags;					// Mission evaluation flags
		private byte					mission_context;			// Our mission context
		private VU_ID					package;					// Our parent package 
		private VU_ID					squadron;					// Our parent squadron
		private VU_ID					requester;					// ID of entity requesting our mission

   
		public byte[]				slots = new byte[PilotStatic.PILOTS_PER_FLIGHT];	// Which vehicle slots this flight is using
		public byte[]					pilots = new byte[PilotStatic.PILOTS_PER_FLIGHT];	// Which squadron pilots we're using
		public byte[]					plane_stats = new byte[PilotStatic.PILOTS_PER_FLIGHT];	// The status of this aircraft
		public byte[]					player_slots = new byte[PilotStatic.PILOTS_PER_FLIGHT];// Which player pilot is in this slot
		public byte					last_player_slot;			// Slot # of last pilot in this slot
		public byte					callsign_id;				// Index into callsign table
		public byte					callsign_num;
		// Locals
		public float					last_collision_x;			// Last point AWACS vectored us to
		public float					last_collision_y;
		public byte					tacan_channel;				// Support for tankers
		public byte					tacan_band;					// Support for tankers

		// Access Functions
		public CampaignTime GetLastMove ()
		{
			return last_move;
		}

		public CampaignTime GetLastCombat ()
		{
			return last_combat;
		}

		public CampaignTime GetTimeOnTarget ()
		{
			return time_on_target;
		}

		public CampaignTime GetMissionOverTime ()
		{
			return mission_over_time;
		}

		public CampaignTime GetLastEnemyLockTime ()
		{
			return last_enemy_lock_time;
		}

		public VU_ID GetAssignedTarget ()
		{
			return assigned_target;
		}

		public VU_ID GetEnemyLocker ()
		{
			return enemy_locker;
		}

		public WayPoint GetOverrideWP ()
		{throw new NotImplementedException();}

		public LoadoutStruct GetLoadout ()
		{
			return loadout;
		}

		public byte GetLoadouts ()
		{
			return loadouts;
		}

		public byte GetOriginalMission ()
		{
			return old_mission;
		}

		public byte GetLastDirection ()
		{
			return last_direction;
		}

		public byte GetEvalFlags ()
		{
			return eval_flags;
		}

		public byte GetMissionContext ()
		{
			return mission_context;
		}

		public VU_ID GetRequesterID ()
		{
			return requester;
		}

//		virtual int CombatClass ()				{ return SimACDefTable[((Falcon4EntityClassType*)EntityType()).vehicleDataIndex].combatClass; } // 2002-02-25 ADDED BY S.G. FlightClass needs to have a combat class like aircrafts.
		public override int CombatClass ()
		{throw new NotImplementedException();} // 2002-03-04 MODIFIED BY S.G. Moved inside Flight.cpp
		public void SetLastDirection (byte b)
		{throw new NotImplementedException();}

		public void SetPackage (VU_ID i)
		{throw new NotImplementedException();}

		public void SetEvalFlag (byte b, int reset = 0)
		{throw new NotImplementedException();} // 2002-02-19 MODIFIED BY S.G. Added the ability to reset the flag to whatever is passed without ORing the bits toghether
		public void ClearEvalFlag (byte b)
		{throw new NotImplementedException();}

		public void SetAssignedTarget (VU_ID targetId)
		{throw new NotImplementedException();}

		public void ClearAssignedTarget ()
		{throw new NotImplementedException();}

		public void SetOverrideWP (WayPoint w, bool ReqHelpHint = false)
		{throw new NotImplementedException();}

		public void MakeStoresDirty ()
		{throw new NotImplementedException();}

		// Dirty Data
		public override void ClearDirty ()
		{throw new NotImplementedException();}

		public void MakeFlightDirty (Dirty_Flight bits, Dirtyness score)
		{throw new NotImplementedException();}

		public override void WriteDirty (byte[] stream, ref int pos)
		{throw new NotImplementedException();}

        public override void ReadDirty(byte[] stream, ref int pos)
		{throw new NotImplementedException();}

		// Other Functions
		public FlightClass (int type, Unit parent, Unit squadron):base(type)
		{throw new NotImplementedException();}
		
		public FlightClass (byte[] stream, ref int offset):base(stream, ref offset)
		{throw new NotImplementedException();}
		//TODO public virtual ~FlightClass();
		public override int SaveSize ()
		{throw new NotImplementedException();}

		public override int Save (VU_BYTE[] stream)
		{throw new NotImplementedException();}

		public virtual VU_ERRCODE RemovalCallback ()
		{throw new NotImplementedException();}

		// event Handlers
		public override VU_ERRCODE Handle (VuFullUpdateEvent evnt)
		{throw new NotImplementedException();}
		
		// virtuals handled by flight.h
		public override int GetDeaggregationPoint (int slot, CampEntity ent)
		{throw new NotImplementedException();}

		public override bool	ShouldDeaggregate ()
		{throw new NotImplementedException();}

		public override int Reaction (CampEntity what, int zone, float range)
		{throw new NotImplementedException();}

		public override int MoveUnit (CampaignTime time)
		{throw new NotImplementedException();}

		public override int DoCombat ()
		{throw new NotImplementedException();}

		public override int ChooseTactic ()
		{throw new NotImplementedException();}

		public override int CheckTactic (int tid)
		{throw new NotImplementedException();}

		public override int DetectOnMove ()
		{throw new NotImplementedException();}

		public override int ChooseTarget ()
		{throw new NotImplementedException();}

		public override int Real ()
		{
			return 1;
		}

		public override int CollectWeapons (byte[] dam, MoveType m, short[] w, byte[] wc, int dist)
		{throw new NotImplementedException();}

		public override CampaignTime UpdateTime ()
		{
			return new CampaignTime((ulong)AIInput.FLIGHT_MOVE_CHECK_INTERVAL * CampaignTime.CampaignSeconds);
		}

		public override CampaignTime CombatTime ()
		{
			return new CampaignTime((ulong)AIInput.FLIGHT_COMBAT_CHECK_INTERVAL * CampaignTime.CampaignSeconds);
		}

		public override bool IsFlight ()
		{
			return true;
		}

		public override int GetDetectionRange (int mt)
		{throw new NotImplementedException();}

		public override FEC_RADAR GetRadarMode ()
		{
			return FEC_RADAR.FEC_RADAR_SEARCH_100;
		}

		public override bool IsSPJamming ()
		{throw new NotImplementedException();}

		public override bool IsAreaJamming ()
		{throw new NotImplementedException();}

		public override bool HasSPJamming ()
		{throw new NotImplementedException();}

		public override bool HasAreaJamming ()
		{throw new NotImplementedException();}

		public override int GetVehicleDeagData (SimInitDataClass simdata, int remote)
		{throw new NotImplementedException();}
			
		public override void SetUnitLastMove (CampaignTime t)
		{
			last_move = t;
		}

		public override void SetCombatTime (CampaignTime t)
		{
			last_combat = t;
		}

		public override void SetBurntFuel (long fuel)
		{
			fuel_burnt = fuel;
		}

		public override void SetUnitMission (byte mis)
		{throw new NotImplementedException();}

		public override void SetUnitPriority (int p)
		{
			priority = (byte)p;
		}

		public override void SetUnitMissionID (int id)
		{
			mission_id = (byte)id;
		}

		public virtual void SetUnitMissionTarget (VU_ID id)
		{
			mission_target = id;
		}

		public override void SetUnitTOT (CampaignTime tot)
		{
			time_on_target = tot;
		}

		public override void SetUnitSquadron (VU_ID ID)
		{
			squadron = ID;
		}
//		virtual void SetUnitTakeoffSlot (int ts)				{ takeoff_slot = ts; }
		public override void SimSetLocation (float x, float y, float z)
		{throw new NotImplementedException();}

		public override void GetRealPosition (ref float x, ref float y, ref float z)
		{throw new NotImplementedException();}

		public override void SimSetOrientation (float yaw, float pitch, float roll)
		{throw new NotImplementedException();}

		public override void SetLoadout (LoadoutStruct loadout, int count)
		{throw new NotImplementedException();}

		public virtual void RemoveLoadout ()
		{throw new NotImplementedException();}

		public virtual LoadoutStruct  GetLoadout (int ac)
		{throw new NotImplementedException();}

		public override int GetNumberOfLoadouts ()
		{
			return loadouts;
		}

		public override CampaignTime GetMoveTime ()
		{throw new NotImplementedException();}

		public override CampaignTime GetCombatTime ()
		{
			return new CampaignTime((CampaignClass.TheCampaign.CurrentTime.time > last_combat.time) ? 
			                        CampaignClass.TheCampaign.CurrentTime.time - last_combat .time: 0);
		}

		public override int GetBurntFuel ()
		{
			return (int)fuel_burnt; // TODO I have inserted the casting to int
		}

		public override MissionTypeEnum GetUnitMission ()
		{
			return (MissionTypeEnum)mission;
		}

		public override int GetUnitCurrentRole ()
		{throw new NotImplementedException();}

		public override int GetUnitPriority ()
		{
			return (int)priority;
		}

		public override int GetUnitWeaponId (int hp, int slot)
		{throw new NotImplementedException();}

		public override int GetUnitWeaponCount (int hp, int slot)
		{throw new NotImplementedException();}

		public override int GetUnitMissionID ()
		{
			return (int)mission_id;
		}

		public override CampEntity GetUnitMissionTarget ()
		{
			return (CampEntity)VuDatabase.vuDatabase.Find (mission_target);
		}

		public override VU_ID GetUnitMissionTargetID ()
		{
			return mission_target;
		}

		public override CampaignTime GetUnitTOT ()
		{
			return time_on_target;
		}

		public override Unit GetUnitSquadron ()
		{
#if TODO
			return FindStatic.FindUnit (squadron);
#endif
			throw new NotImplementedException();
		}

		public override VU_ID GetUnitSquadronID ()
		{
			return squadron;
		}

		public override CampEntity GetUnitAirbase ()
		{throw new NotImplementedException();}

		public override VU_ID GetUnitAirbaseID ()
		{throw new NotImplementedException();}
//		virtual int GetUnitTakeoffSlot ()					{ return (int)takeoff_slot; }
		public virtual int LoadWeapons (object  squadron, ref byte dam, MoveType mt, int num, int type_flags, int guide_flags)
		{throw new NotImplementedException();}

		public override int DumpWeapons ()
		{throw new NotImplementedException();}

		public override CampaignTime ETA ()
		{throw new NotImplementedException();}

		public override F4PFList GetKnownEmitters ()
		{throw new NotImplementedException();}

		public override int BuildMission (MissionRequestClass  mis)
		{throw new NotImplementedException();}

		public override void GetUnitAssemblyPoint (int type, ref GridIndex x, ref GridIndex y)
		{throw new NotImplementedException();}

		public override Unit GetUnitParent ()
		{
			return (Unit)VuDatabase.vuDatabase.Find (package);
		}

		public override VU_ID GetUnitParentID ()
		{
			return package;
		}

		public override void SetUnitParent (Unit p)
		{
			SetPackage (p.Id ());
		}

		public override void IncrementTime (CampaignTime dt)
		{
			last_move += dt;
		}

		public override int GetBestVehicleWeapon (int i, byte[] b, MoveType m, int i2, int[] i3)
		{throw new NotImplementedException();}

		public override void UseFuel (long l)
		{throw new NotImplementedException();}

		// Core functions
		public int DetectVs (AircraftClass ac, ref float d, ref int combat, ref int spotted, ref int estr)
		{throw new NotImplementedException();}

		public int DetectVs (CampEntity e, ref float d, ref int combat, ref int spotted, ref int estr)
		{throw new NotImplementedException();}

		public PackageClass GetUnitPackage ()
		{
			return (PackageClass)VuDatabase.vuDatabase.Find (package);
		}

		public int PickRandomPilot (int seed)
		{throw new NotImplementedException();}

		public int GetAdjustedPlayerSlot (int pslot)
		{throw new NotImplementedException();}

		public int GetPilotSquadronID (int pilotSlot)
		{
			return pilots [pilotSlot];
		}

		public PilotClass GetPilotData (int pilotSlot)
		{throw new NotImplementedException();}

		public int GetPilotID (int pilotSlot)
		{throw new NotImplementedException();}

		public int GetPilotCallNumber (int pilot_slot)
		{throw new NotImplementedException();}

		public byte GetPilotVoiceID (int pilotSlot)
		{throw new NotImplementedException();}

		public int GetPilotCount ()
		{throw new NotImplementedException();}								// Returns # of pilots in flight (including players)
		public int GetACCount ()
		{throw new NotImplementedException();}									// Returns # of aircraft in flight
		public int GetFlightLeadSlot ()
		{throw new NotImplementedException();}							// Returns slot of flightleader
		public int GetFlightLeadCallNumber ()
		{throw new NotImplementedException();}						// Returns the callnumber (1-36) of the flightleader
		public byte GetFlightLeadVoiceID ()
		{throw new NotImplementedException();}						// Returns the voiceId of the flightleader
		public int GetAdjustedAircraftSlot (int aircraftNum)
		{throw new NotImplementedException();}

		public long CalculateFuelAvailable (int aircraftNum)
		{throw new NotImplementedException();}

		public int HasWeapons ()
		{throw new NotImplementedException();}

		public int HasFuel (int limit = 9)
		{throw new NotImplementedException();} // 2002-02-20 MODIFIED BY S.G. Added 'limit' which defaults to 9 so 9/12 is the same as 3/4 used in the original code
		public int CanAbort ()
		{throw new NotImplementedException();}									// returns 1 if don't have enough fuel or weapons
// 2001-04-03 ADDED BY S.G. THE Standoff jammer GETTER WASN'T DEFINED
		public Flight GetECMFlight ()
		{throw new NotImplementedException();}
// END OF ADDED SECTION
		public Flight GetAWACSFlight ()
		{throw new NotImplementedException();}

		public Flight GetJSTARFlight ()
		{throw new NotImplementedException();}

		public Flight GetFACFlight ()
		{throw new NotImplementedException();}

		public Flight GetTankerFlight ()
		{throw new NotImplementedException();}

		public Flight GetFlightController ()
		{throw new NotImplementedException();}

		public int FindCollisionPoint (FalconEntity target, vector collPoint, int noAWACS)
		{throw new NotImplementedException();}

		public void RegisterLock (FalconEntity locker)
		{throw new NotImplementedException();}

		// Component accessers (Sim Flight emulators)
		public void SendComponentMessage (int command, VuEntity sender)
		{throw new NotImplementedException();}

		public int AirbaseOperational (Objective airbase)
		{throw new NotImplementedException();}  // JPO - is the airbase still suitable for takeoff/landings

// 2001-04-03 ADDED BY S.G. NEED SOMETHING TO HOLD THE ecmFlightClassPtr VARIABLE
		public FlightClass ecmFlightPtr;
// 2001-06-25 ADDED BY S.G. NEED SOMETHING TO HOLD WHO FIRED AT IT SO ONLY ONE SIM PLANE WILL LAUNCH HARMS AT AN AGGREGATED TARGET
		public FalconEntity shotAt;
		public AircraftClass whoShot;
		// 2001-10-11 ADDED by M.N. 
		public uint	refuel;									// How much fuel has to be taken from a tanker

		// =================================
// Support functions
// =================================

		public static FlightClass  NewFlight (int type, Unit parent, Unit squad)
		{throw new NotImplementedException();}

		public static int RegroupFlight (Flight flight)
		{throw new NotImplementedException();}

		public static void RegroupAircraft (AircraftClass ac)
		{throw new NotImplementedException();}

		public static void CancelFlight (Flight flight)
		{throw new NotImplementedException();}

		public static void UpdateSquadronStatus (Flight flight, int landed, int playchatter)
		{throw new NotImplementedException();}

		public static WayPoint ResetCurrentWP (Unit u)
		{throw new NotImplementedException();}

		public static void AbortFlight (Flight flight)
		{throw new NotImplementedException();}

		public static Objective FindAlternateStrip (Flight flight)
		{throw new NotImplementedException();}
	}
	
// TODO typedef FlightClass*	Flight;
}

