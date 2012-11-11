using System;
using FalconNet.Common;
using FalconNet.FalcLib;

namespace FalconNet.Campaign
{
	// ===================================================
	// Mission defines
	// ===================================================
	
	// Mission defines for aircraft
	public enum MissionTypeEnum
	{        // NOTE: This must fit int a byte!
		AMIS_NONE           = 0,
		AMIS_BARCAP         = 1,			// BARCAP missions to protect a target area
		AMIS_BARCAP2        = 2,			// BARCAP missions to defend a border
		AMIS_HAVCAP         = 3,
		AMIS_TARCAP         = 4,
		AMIS_RESCAP         = 5,
		AMIS_AMBUSHCAP      = 6,
		AMIS_SWEEP          = 7,
		AMIS_ALERT          = 8,
		AMIS_INTERCEPT      = 9,
		AMIS_ESCORT         = 10,
		AMIS_SEADSTRIKE     = 11,
		AMIS_SEADESCORT     = 12,
		AMIS_OCASTRIKE      = 13,			// OCA strike (direct w/escort & sead)
		AMIS_INTSTRIKE      = 14,			// Interdiction (direct, alone)
		AMIS_STRIKE         = 15,			// 
		AMIS_DEEPSTRIKE     = 16,			// Deep strike (safe path, w/escort & sead)
		AMIS_STSTRIKE       = 17,			// Stealth strike (safe path, alone)
		AMIS_STRATBOMB      = 18,
		AMIS_FAC            = 19,
		AMIS_ONCALLCAS      = 20,			// On call CAS
		AMIS_PRPLANCAS      = 21,			// Pre planned CAS
		AMIS_CAS            = 22,			// Immediate CAS
		AMIS_SAD            = 23,			// Search and destroy interdiction
		AMIS_INT            = 24,			// Interdiction (vs supply/fuel lines only)
		AMIS_BAI            = 25,			// Battlefield area interdiction
		AMIS_AWACS          = 26,
		AMIS_JSTAR          = 27,
		AMIS_TANKER         = 28,
		AMIS_RECON          = 29,
		AMIS_BDA            = 30,
		AMIS_ECM            = 31,
		AMIS_AIRCAV         = 32,
		AMIS_AIRLIFT        = 33,
		AMIS_SAR            = 34,
		AMIS_ASW            = 35,
		AMIS_ASHIP          = 36,
		AMIS_PATROL         = 37,
		AMIS_RECONPATROL    = 38,			// Recon for enemy ground vehicles
		AMIS_ABORT          = 39,
		AMIS_TRAINING       = 40,
		AMIS_OTHER          = 41,
	};
	// Mission rolls
	public enum MissionRollEnum
	{
		ARO_CA              = 1,
		ARO_TACTRANS        = 2,     // AMIS_SAR | AMIS_AIRCAV
		ARO_S               = 3,     // AMIS_OCASTRIKE | AMIS_INTSTRIKE | 																// Strike target
		ARO_GA              = 4,     // AMIS_SAD | AMIS_BAI | AMIS_ONCALLCAS | AMIS_PRPLANCAS	 									// Ground attack
		ARO_SB              = 5,     // AMIS_STRATBOMB           																		// Strategic bomb
		ARO_ECM             = 6,     // AMIS_ELJAM           																				// Jam radar
		ARO_SEAD            = 7,     // AMIS_SEADSTRIKE | AMIS_SEADESCORT																// SEAD
		ARO_ASW             = 8,	   // AMIS_ASW
		ARO_ASHIP           = 9,     // AMIS_ASHIP
		ARO_REC             = 10,    // AMIS_BDA | AMIS_RECON | AMIS_PATROL																// Recon
		ARO_TRANS           = 11,    // AMIS_AIRBORNE | AMIS_AIRLIFT          														// Drop off cargo
		ARO_ELINT           = 12,    // AMIS_AWACS
		ARO_AWACS           = 12,
		ARO_JSTAR           = 13,    // AMIS_JSTAR
		ARO_TANK            = 14,    // AMIS_TANKER
		ARO_FAC             = 15,
		ARO_OTHER           = 16,
	};

// Target Type
	public enum MissionTargetTypeEnum
	{
		AMIS_TAR_NONE       = 0,
		AMIS_TAR_OBJECTIVE  = 1,
		AMIS_TAR_UNIT       = 2,
		AMIS_TAR_LOCATION   = 3,
	};

// Additional Escort condition modes (normally ATM_MODE_OCA, DCA or INT)
	public enum MissionEscortConditionEnum
	{
		AMIS_ESC_ALWAYS     = 99,
		AMIS_ESC_NONE       = 98,
	};

	// Tanker conditional modes
	public enum TankerConditionalModeEnum
	{
		TANKER_NONE         = 0,
		TANKER_HALF         = 1,
		TANKER_FULL         = 2,
		TANKER_ALWAYS       = 3,
	};

	// flags for mission data
	[Flags]
	public enum MissionDataFlagEnum
	{
		AMIS_ADDAWACS       = 0x01,				// Request AWACS
		AMIS_ADDJSTAR       = 0x02,				// Request JSTAR
		AMIS_ADDECM         = 0x04,				// Request ECM
		AMIS_ADDBDA         = 0x08,				// Request BDA
		AMIS_ADDESCORT      = 0x10,				// Request CA Escort (if enemy responds)
		AMIS_ADDSEAD        = 0x20,				// Request SEAD Escort (if enemy AD present)
		AMIS_ADDBARCAP      = 0x40,				// Trigger enemy BARCAP
		AMIS_ADDSWEEP       = 0x80,				// Trigger enemy Sweep
		AMIS_ADDOCASTRIKE   = 0x100,				// Trigger friendly OCA Strike vs enemy assets found
		AMIS_ADDTANKER      = 0x200,				// Add a tanker, if possible
		AMIS_NEEDTANKER     = 0x400,				// This mission can't go w/o a tanker
		AMIS_ADDFAC         = 0x800,				// Request FAC aircraft
		AMIS_ADDINTERCEPT   = 0x1000,			// Add an enemy intercept mission to intercept this mission

		AMIS_NOTHREAT       = 0x2000,			// Abort if above the minimum threat threshold
		AMIS_AVOIDTHREAT    = 0x4000,			// Dodge ADs vertically
		AMIS_HIGHTHREAT     = 0x8000,			// Abort only if extreme threat
		AMIS_IMMEDIATE      = 0x10000,			// Set if I'm looking for aircraft in flight
		AMIS_MATCHSPEED     = 0x20000,			// Try to build package with equivalent speed a/c
		AMIS_TARGET_ONLY    = 0x40000,			// go directly to target and back
		AMIS_NO_BREAKPT     = 0x80000,			// Don't add a breakpoint to this mission
		AMIS_DONT_COORD     = 0x100000,			// Don't match ingress/egress times with the main flight
		AMIS_AIR_LAUNCH_OK  = 0x200000,			// This mission can launch in mid-air (ie: from the edge of the map)
		AMIS_DONT_USE_AC    = 0x400000,			// Don't subtract aircraft from the squadron
		AMIS_NPC_ONLY       = 0x800000,			// Don't assign to potential player squadrons
		AMIS_FUDGE_RANGE    = 0x1000000,		// This mission is always in range - we'll just load more fuel
		AMIS_EXPECT_DIVERT  = 0x2000000,		// We want to sit around until we're told to divert
		AMIS_ASSIGNED_TAR   = 0x4000000,		// Used for diverts with a specific target in mind
		AMIS_NO_DIST_BONUS  = 0x8000000,		// Don't adjust mission priority by distance to front (add a constant)
		AMIS_NO_TARGETABORT = 0x10000000,			// Don't abort after reaching our target - just fly home
		AMIS_FLYALWAYS      = 0x20000000,			// Don't restrict percentage of missions flown
		AMIS_HELP_REQUEST   = 0x40000000,		// 2001-10-27 S.G. It's from an help request
	};

	// Flags for mission requests
	[Flags]
	public enum MissionRequestFlagEnum
	{
		REQF_USERESERVES    = 0x01,				// Use reserve aircraft for this request, if nessesary (mostly for support)
		REQF_CHECKED        = 0x02,				// This request has already been checked at least once
		REQF_NEEDRESPONSE   = 0x04,				// This needs to be dealt with immediately, and returns a response
		REQF_MET            = 0x08,				// This has been met already (in planned list)
		REQF_ONETRY         = 0x100,				// We get one try at building this, otherwise cancel
		REQF_USE_REQ_SQUAD  = 0x200,				// Use the squadron which requested the mission
		REQF_PART_OF_ACTION = 0x400,				// This target is part of a larger action by this team
		REQF_ALLOW_ERRORS   = 0x800,				// Allow missions to be planned with errors (for tactical engagement)
		REQF_TE_MISSION	   = 0x1000,		    // This is a TE mission, correct mission length calculation mission.cpp
	};

	// Returned by TargetThreats
	// Flags for what we really need
	[Flags]
	public enum TargetThreatReturnFlagEnum
	{
		NEED_SEAD           = 0x01,
		NEED_ECM            = 0x02,

		// Specifics as to the threat types
		THREAT_LALT_SAM     = 0x10,
		THREAT_HALT_SAM     = 0x20,
		THREAT_AAA          = 0x40,

		// Specifics as to the threat location
		THREAT_ENROUTE      = 0x100,
		THREAT_TARGET       = 0x200,
	};

	// Mission profiles
	public enum MissionProfileEnum
	{
		MPROF_LOW           = 0x01,				// Low engress profile
		MPROF_HIGH          = 0x02,				// High engress profile
	};

	// Target area profiles
	public enum TargetAreaProfileEnum
	{
		TPROF_NONE          = 0,					// No target WP
		TPROF_ATTACK        = 1,					// IP, Target, Turn point  (Assembly, Break point)
		TPROF_HPATTACK      = 2,					// IP, Target (Assembly, Break point)
		TPROF_LOITER        = 3,					// 2 Turn points (Assembly)
		TPROF_TARGET        = 4,					// Target only (Assembly, Break point)
		TPROF_AVOID         = 5,					// Break point, Turn point (Assembly)
		TPROF_SWEEP         = 6,					// 3 circular Turn points
		TPROF_FLYBY         = 7,					// Pass over target, at lowest possible threat
		TPROF_SEARCH        = 8,					// Like loiter, but at full speed
		TPROF_LAND          = 9,					// Land at target for station time, then takeoff again
	};

	// Target desctiption types (when to use target actions)
	public enum TargetDescriptionType
	{
		TDESC_NONE          = 0,
		TDESC_TTL           = 1,					// Takeoff To Landing (mission actions during whole route)
		TDESC_ATA           = 2,					// Assembly To Assembly
		TDESC_TAO           = 3,					// Target Area Only
	};

	// Special time on target types
	public enum SpecialTOTTypeEnum
	{
		TOT_TAKEOFF         = 10,
		TOT_ENROUTE         = 11,
		TOT_INGRESS         = 12,
	};

	// Mission request contexts (Why this mission is being requested)
	public enum MissionContext
	{
		noContext,						// We don't really know why
		enemyUnitAdvanceBridge,			// An enemy unit is advancing over a bridge
		enemyUnitMoveBridge,			// An enemy unit is moving over a bridge
		enemyUnitAdvance,				// An enemy unit is advancing
		enemyUnitMove,					// An enemy unit is moving
		enemyUnitAttacking,				// An enemy unit is attacking our forces
		enemyUnitDefending,				// An enemy unit is defending against our forces
		enemyForcesPresent,				// Enemy forces are suspected to be here
		attackEnemyUnit,				// FAC CALL: Attack specific enemy unit (8)
		emptyUnit1,						// TBD (9)
		emptyUnit2,						// TBD (10)
		friendlyUnitAirborneMovement,	// a friendly unit needs airborne transportation
		emptyUnit5,						// TBD (12)
		emptyUnit6,						// TBD (13)
		enemyStrikesExpected,			// Enemy air strikes expected in this area
		enemyAircraftPresent,			// Enemy aircraft are expected (generic) (15)
		enemyGroundForcesPresent,		// Enemy ground forces are expected (JSTAR trigger)
		enemyRadarPresent,				// Enemy radar operating (ECM trigger)
		enemySupportAircraftPresent,	// Enemy support aircraft operating
		enemyCASAircraftPresent,		// Enemy ground attack aircraft are present
		interceptEnemyAircraft,			// AWACS CALL: Intercept specific enemy aircraft (20)
		emptyEnemyAir1,					// TBD (21)
		hostileAircraftPresent,			// Hostile aircraft operating in an area
		emptyHostileAir1,				// TBD (23)
		friendlyRescueExpected,			// Friendly SAR craft will be operating in the area
		friendlyCASExpected,			// Friendly CAS aircraft will be entering the area (25)
		friendlyAssetsExpected,			// Generic 'Friendly assets' will be operating here
		friendlyAssetsRefueling,		// Friendly aircraft will be refueling in the area (TANKER trigger)
		emptyFriendly1,					// TBD (28)
		emptyFriendly2,					// TBD (29)
		enemySupplyInterdictionBridge,	// Enemy supplies are being transported through here (30)
		enemySupplyInterdictionPort,	// Enemy supplies are being transported through here
		enemySupplyInterdictionDepot,	// Enemy supplies are being stored here
		enemySupplyInterdictionZone,	// Enemy supplies are being moved through here
		emptySupply1,					// TBD (34)
		enemyProductionSource,			// This is producing enemy war materials
		enemyFuelSource,				// This is producing or storing fuel.
		enemyEnergySource,				// This is a source of enemy electrical power
		enemyCommand,					// This is being used as an enemy CCC module
		enemyAirDefense,				// Enemy air defenses are blocking friendly missions
		enemyAirPowerAirbase,			// This is being used to promote enemy air power (40)
		enemyAirPowerRadar,				// This is being used to promote enemy air power
		emptyObj1,						// TBD (42)
		emptyObj2,						// TBD (43)
		friendlyAWACSNeeded,			// We need an awacs (AWACS trigger)
		friendlySuppliesIncomingAir,	// We've got friendly supplies coming in by air
		friendlySuppliesIncomingNaval,	// Same by naval
		friendlySuppliesIncomingGround,	// Same by ground
		friendlySuppliesIncomingRail,	// Same by rail
		targetReconNeeded,				// Need recon of this objective
		emptyx,							// TBD (50)
		emptyy,							// TBD (51)
		AirActionPrepAD,				// OCA vs Air Defenses part of any air action
		AirActionPrepAB,				// OCA vs Air Bases/Radar part of non-OCA action
		AirActionPrepAir,				// Sweep/Escort part of a any action
		AirActionDCA,					// Part of a DCA action
		AirActionOCA,					// Part of an OCA action
		AirActionInterdiction,			// Part of an interdiction action
		AirActionAttrition,				// Part of an attrition action
		AirActionCAS,					// Part of a CAS action (59)
		emptyAction1,					// TBD (60)
		enemyNavalForceActive,			// Ships are operating here
		enemyNavalForceStatic,			// Ships in port
		enemyNavalForceUnloading,		// Transport ships unloading in port
		otherContext
	};

// ===================================================
// Mission Request Class		
// ===================================================

// This class is filled in order to request a mission
	public class MissionRequestClass
	{
#if USE_SH_POOLS
   public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { ShiAssert( size == sizeof(MissionRequestClass) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(MissionRequestClass), 50, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif
		public VU_ID				requesterID;
		public VU_ID				targetID;
		public VU_ID				secondaryID;				// Is this being used?
		public VU_ID				pakID;
		public Team				who;
		public Team				vs;
		public CampaignTime		tot;
		public GridIndex			tx, ty;
		public ulong				flags;
		public short				caps;
		public short				target_num;
		public short				speed;
		public short				match_strength;				// How much Air to Air strength we should try to match
		public short				priority;
		public byte				tot_type;
		public byte				action_type;				// Type of action we're associated with
		public byte				mission;
		public byte				aircraft;
		public byte				context;					// Context code (why this was requested)
		public byte				roe_check;
		public byte				delayed;					// number of times it's been pushed back
		public byte				start_block;				// time block we're taking off during
		public byte				final_block;				// time block we're landing during
		public byte[]				slots = new byte[4];					// squadron slots we're using.
		public char				min_to;						// minimum block we found planes for
		public char				max_to;						// maximum block we found planes for
		
		public MissionRequestClass ()
		{
			throw new NotImplementedException ();
		}
		//TODO public ~MissionRequestClass();
		public int RequestMission ()
		{
			throw new NotImplementedException ();
		}

		public int RequestEnemyMission ()
		{
			throw new NotImplementedException ();
		}
	};
	
// TODO typedef MissionRequestClass *MissionRequest;

// ======================================================
// Mission Data Type - Stores data used to build missions
// ======================================================

	public struct MissionDataType
	{
		public byte					type;
		public byte					target;						// Target type
		public byte					skill;						// Primary skill to look for
		public byte					mission_profile;			// Allowable mission profiles
		public byte					target_profile;				// Type of target profile
		public byte					target_desc;				// When to use our target action (target area/whole mission/etc)
		public byte					routewp;					// Waypoint type along route
		public byte					targetwp;					// What to do at target location
		public short					minalt;						// Minimum alt at target (in hundreds of feet)
		public short					maxalt;						// Maximum alt at target (in hundreds of feet)
		public short					missionalt;					// Suggested mission altitude (in hundreds of feet)
		public short					separation;					// Seperation from main flight, in seconds
		public short					loitertime;					// Loiter time in minutes
		public byte					str;						// Aircraft strength typically assigned
		public byte					min_time;					// Minimum # of minutes in advance to consider a planned mission
		public byte					max_time;					// Maximum # of minutes in advance to consider a planned mission
		public byte					escorttype;					// What sort of mission to build for an escort
		public byte					mindistance;				// Minimum distance between two similar missions
		public byte					mintime;					// Minimum time between missions of similar types
		public byte					caps;						// Special capibilities required (stealth, navy, etc)
		public ulong					flags;						// flags
	};
	
	public static class MissionStatic
	{
		public static MissionDataType[] MissionData = new MissionDataType [AMIS_OTHER];

// ===================================================
// Global functions
// ===================================================

		public static  int BuildPathToTarget (Flight u, MissionRequestClass *mis, VU_ID airbaseID)
		{
			throw new NotImplementedException ();
		}

		public static  void BuildDivertPath (Flight u, MissionRequestClass *mis)
		{
			throw new NotImplementedException ();
		}

		public static  void AddInformationWPs (Flight u, MissionRequestClass *mis)
		{
			throw new NotImplementedException ();
		}

		public static  void ClearDivertWayPoints (Flight flight)
		{
			throw new NotImplementedException ();
		}

		public static  int AddTankerWayPoint (Flight u, int refuel)
		{
			throw new NotImplementedException ();
		} // M.N.

		public static  long SetWPTimes (Flight u, MissionRequestClass *mis)
		{
			throw new NotImplementedException ();
		}

		public static  long SetWPTimesTanker (Flight u, MissionRequestClass *mis, bool type, CampaignTime time)
		{
			throw new NotImplementedException ();
		}

		public static  int SetWPAltitudes (Flight u)
		{
			throw new NotImplementedException ();
		}

		public static  int CheckPathThreats (Unit u)
		{
			throw new NotImplementedException ();
		}

		public static  int TargetThreats (Team team, int priority, F4PFList list, MoveType mt, CampaignTime time, long target_flags, short* targeted)
		{
			throw new NotImplementedException ();
		}

		public static  bool LoadMissionData ()
		{
			throw new NotImplementedException ();
		}
	}
}

