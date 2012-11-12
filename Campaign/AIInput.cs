using System;

namespace FalconNet.Campaign
{
	public class AIInput
	{
		// ATM Inputs
		public static  short IMMEDIATE_MIN_TIME;
		public static  short IMMEDIATE_MAX_TIME;
		public static  short LONGRANGE_MIN_TIME;
		public static  short LONGRANGE_MAX_TIME;
		public static  short ATM_ASSIGN_RATIO;
		public static  short MAX_FLYMISSION_THREAT;
		public static  short MAX_FLYMISSION_HIGHTHREAT;
		public static  short MAX_FLYMISSION_NOTHREAT;
		public static  short MIN_SEADESCORT_THREAT;
		public static  short MIN_AVOID_THREAT;
		public static  short MIN_AP_DISTANCE;
		public static  short BREAKPOINT_DISTANCE;
		public static  short ATM_TARGET_CLEAR_RADIUS;
		public static  short LOITER_DIST;
		public static  short SWEEP_DISTANCE;
		public static  short MINIMUM_BDA_PRIORITY;
		public static  short MINIMUM_AWACS_DISTANCE;
		public static  short MAXIMUM_AWACS_DISTANCE;
		public static  short MINIMUM_JSTAR_DISTANCE;
		public static  short MAXIMUM_JSTAR_DISTANCE;
		public static  short MINIMUM_TANKER_DISTANCE;
		public static  short MAXIMUM_TANKER_DISTANCE;
		public static  short MINIMUM_ECM_DISTANCE;
		public static  short MAXIMUM_ECM_DISTANCE;
		public static  short FIRST_COLONEL;						// Indexes into our pilot name array
		public static  short FIRST_COMMANDER;
		public static  short FIRST_WINGMAN;
		public static  short LAST_WINGMAN;
		public static  short MAX_AA_STR_FOR_ESCORT;				// Max pack Air to Air str in order to add escorts
		public static  short BARCAP_REQUEST_INTERVAL;			// Minimum time between BARCAP mission requests
		public static  short ONCALL_CAS_FLIGHTS_PER_REQUEST;	// How many CAS flights to plan per ONCALL request
		public static  short MIN_TASK_AIR;						// Retask time, in minutes
		public static  short MIN_PLAN_AIR;						// Replan time (KCK NOTE: PLAN_AIR max is 8)
		public static  short VICTORY_CHECK_TIME;				// Check victory condition time in Tactical Engagement
		public static  short FLIGHT_MOVE_CHECK_INTERVAL;		// How often to check a flight for movement (in seconds)
		public static  short FLIGHT_COMBAT_CHECK_INTERVAL;		// How often to check a flight for combat (in seconds)
		public static  short AIR_UPDATE_CHECK_INTERVAL;			// How often to check squadrons/packages for movement/update (in seconds)
		public static  short FLIGHT_COMBAT_RATE;				// Amount of time between weapon shots (in seconds)
		public static  short PACKAGE_CYCLES_TO_WAIT;			// How many planning cycles to wait for tankers/barcap/etc
		public static  short AIR_PATH_MAX;						// Max amount of nodes to search for air paths
		public static  float MIN_IGNORE_RANGE;					// Minimum range (km) at which we'll ignore an air target
		public static  short MAX_SAR_DIST;						// Max distance from front to fly sar choppers into. (km)
		public static  short MAX_BAI_DIST;						// Max distance from front to fly BAI missions into. (km)
		public static  long PILOT_ASSIGN_TIME;					// Time before takeoff to assign pilots
		public static  short AIRCRAFT_TURNAROUND_TIME_MINUTES;	// Time to wait between missions before an aircraft is available again

		// GTM Inputs
		public static  short PRIMARY_OBJ_PRIORITY;				// Priorities for primary and secondary objectives
		public static  short SECONDARY_OBJ_PRIORITY;			// Only cities and towns can be POs and SOs
		public static  short MAX_OFFENSES;						// Maximum number of offensive POs.
		public static  short MIN_OFFENSIVE_UNITS;				// Minimum number of units to constitute an offensive
		public static  short MINIMUM_ADJUSTED_LEVEL;			// Minimum % strength of unit assigned to offense.
		public static  short MINIMUM_VIABLE_LEVEL;				// Minimum % strength in order to be considered a viable unit
		public static  short MAX_ASSIGNMENT_DIST;				// Maximum distance from an object we'll assign a unit to
		public static  short MAXLINKS_FROM_SO_OFFENSIVE;		// Maximum objective links from our secondary objective we'll place units
		public static  short MAXLINKS_FROM_SO_DEFENSIVE;		// Same as above for defensive units
		public static  short BRIGADE_BREAK_BASE;				// Base moral a brigade breaks at (modified by objective priority)
		public static  short ROLE_SCORE_MODIFIER;				// Used to adjust role score for scoring purposes
		public static  short MIN_TASK_GROUND;					// Retask time, in minutes
		public static  short MIN_PLAN_GROUND;					// Replan time, in minutes
		public static  short MIN_REPAIR_OBJECTIVES;				// How often to repair objectives (in minutes);
		public static  short MIN_RESUPPLY;						// How often to resupply troops (in minutes)
		public static  short MORALE_REGAIN_RATE;				// How much morale per hour a typical unit will regain
		public static  short REGAIN_RATE_MULTIPLIER_FOR_TE;		// Morale/Fatigue regain rate multiplier for TE missions
		public static  short FOOT_MOVE_CHECK_INTERVAL;			// How often to check a foot battalion for movement (in seconds)
		public static  short TRACKED_MOVE_CHECK_INTERVAL;		// How often to check a tracked/wheeled battalion for combat (in seconds)
		public static  short GROUND_COMBAT_CHECK_INTERVAL;		// How often to check ground battalions for combat
		public static  short GROUND_UPDATE_CHECK_INTERVAL;		// How often to check brigades for update (in seconds)
		public static  short GROUND_COMBAT_RATE;				// Amount of time between weapon shots (in seconds)
		public static  short GROUND_PATH_MAX;					// Max amount of nodes to search for ground paths
		public static  short OBJ_GROUND_PATH_MAX_SEARCH;		// Max amount of nodes to search for objective paths
		public static  short OBJ_GROUND_PATH_MAX_COST;			// Max amount of nodes to search for objective paths
		public static  short MIN_FULL_OFFENSIVE_INITIATIVE;		// Minimum initiative required to launch a full offensive
		public static  short MIN_COUNTER_ATTACK_INITIATIVE;		// Minimum initiative required to launch a counter-attack
		public static  short MIN_SECURE_INITIATIVE;				// Minimum initiative required to secure defensive objectives
		public static  short MINIMUM_EXP_TO_FIRE_PREGUIDE;		// Minimum experience needed to fire SAMs before going to guide mode
		public static  long OFFENSIVE_REINFORCEMENT_TIME;		// How often we get reinforcements when on the offensive
		public static  long DEFENSIVE_REINFORCEMENT_TIME;		// How often we get reinforcements when on the defensive
		public static  long CONSOLIDATE_REINFORCEMENT_TIME;		// How often we get reinforcements when consolidating
		public static  long ACTION_PREP_TIME;					// How far in advance to plan any offensives

		// NTM Inputs
		public static  short MIN_TASK_NAVAL;					// Retask time, in minutes	
		public static  short MIN_PLAN_NAVAL;					// Replan time, in minutes
		public static  short NAVAL_MOVE_CHECK_INTERVAL;			// How often to check a task force for movement (in seconds)
		public static  short NAVAL_COMBAT_CHECK_INTERVAL;		// How often to fire naval units (in seconds)
		public static  short NAVAL_COMBAT_RATE;					// Amount of time between weapon shots (in seconds)

		// Other Inputs
		public static  short LOW_ALTITUDE_CUTOFF;
		public static  short MAX_GROUND_SEARCH;	          		// How far to look for surface units or objectives
		public static  short MAX_AIR_SEARCH;		           	// How far to look for air threats
		public static  short NEW_CLOUD_CHANCE;
		public static  short DEL_CLOUD_CHANCE;
		public static  short PLAYER_BUBBLE_MOVERS;				// The # of objects we try to keep in player bubble
		public static  short FALCON_PLAYER_TEAM;				// What team the player starts on
		public static  short CAMP_RESYNC_TIME;					// How often to resync campaigns (in seconds)
		public static  ushort BUBBLE_REBUILD_TIME;				// How often to rebuild the player bubble (in seconds)
		public static  short STANDARD_EVENT_LENGTH;				// Length of stored event queues
		public static  short PRIORITY_EVENT_LENGTH;
		public static  short MIN_RECALCULATE_STATISTICS;		// How often to do the unit statistics stuff
		public static  float LOWAIR_RANGE_MODIFIER;				// % of Air range to get LowAir range from
		public static  short MINIMUM_STRENGTH;					// Minimum strength points we'll bother appling with colateral damage
		public static  short MAX_DAMAGE_TRIES;					// Max # of times we'll attempt to find a random target vehicle/feature
		public static  float REAGREGATION_RATIO;				// Ratio over 1.0 which things will reaggregate at
		public static  short MIN_REINFORCE_TIME;				// Value, in minutes, of each reinforcement cycle
		public static  float SIM_BUBBLE_SIZE;					// Radius (in feet) of the Sim's campaign lists.
		public static  short INITIATIVE_LEAK_PER_HOUR;			// How much initiative is adjusted automatically per hour

		// Campaign Inputs
		public static  int StartOffBonusRepl;					// Replacement bonus when a team goes offensive
		public static  int StartOffBonusSup;					// Supply Bonus when a team goes offensive
		public static  int StartOffBonusFuel;					// Fuel Bonus when a team goes offensive
		public static  int ActionRate;							// How often we can start a new action (in hours)
		public static  int ActionTimeOut;						// Maximum time an offensive action can last
		public static  float DataRateModRepl;					// Modification factor for production data rate for replacements
		public static  float DataRateModSup;					// Modification factor for production data rate for fuel and supply
		public static  float RelSquadBonus;						// Relative replacements of squadrons to ground units
		// in fact bools
		public static  int NoActionBonusProd;					// No action bonus for production
		public static  int NoTypeBonusRepl;						// No type bonus for replacements, new supply system, bugfix in "supplies units" function
		public static  int NewInitiativePoints;					// New initiative point setting
		public static  int CampBugFixes;						// Smaller bugfixes
		public static  float HitChanceAir;						// 2D hitchance air target modulation
		public static  float HitChanceGround;					// 2D hitchance ground target modulation
		// These are in fact read from the trigger files now - no need to mess with the campaign files !
		public static  float FLOTDrawDistance;					// Distance between FLOT objectives at which we won't draw a line anymore (2 front campaigns...)
		public static  int FLOTSortDirection;					// to sort FLOT by x or y objective coordinates
		public static  int TheaterXPosition;					// central theater x/y position for calculating new bullseye posit
		public static  int TheaterYPosition;					// central theater x/y position for calculating new bullseye posit

		public static void ReadCampAIInputs (string name);
	}
}

