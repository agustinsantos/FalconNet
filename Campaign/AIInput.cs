using System;
using FalconNet.Common;
using System.Globalization;
using FalconNet.FalcLib;
using FalconNet.CampaignBase;
using FalconNet.F4Common;

namespace FalconNet.Campaign
{
    public class AIInput
    {
        // ATM Inputs
        public static short IMMEDIATE_MIN_TIME;
        public static short IMMEDIATE_MAX_TIME;
        public static short LONGRANGE_MIN_TIME;
        public static short LONGRANGE_MAX_TIME;
        public static short ATM_ASSIGN_RATIO;
        public static short MAX_FLYMISSION_THREAT;
        public static short MAX_FLYMISSION_HIGHTHREAT;
        public static short MAX_FLYMISSION_NOTHREAT;
        public static short MIN_SEADESCORT_THREAT;
        public static short MIN_AVOID_THREAT;
        public static short MIN_AP_DISTANCE;
        public static short BREAKPOINT_DISTANCE;
        public static short ATM_TARGET_CLEAR_RADIUS;
        public static short LOITER_DIST;
        public static short SWEEP_DISTANCE;
        public static short MINIMUM_BDA_PRIORITY;
        public static short MINIMUM_AWACS_DISTANCE;
        public static short MAXIMUM_AWACS_DISTANCE;
        public static short MINIMUM_JSTAR_DISTANCE;
        public static short MAXIMUM_JSTAR_DISTANCE;
        public static short MINIMUM_TANKER_DISTANCE;
        public static short MAXIMUM_TANKER_DISTANCE;
        public static short MINIMUM_ECM_DISTANCE;
        public static short MAXIMUM_ECM_DISTANCE;
        public static short FIRST_COLONEL;						// Indexes into our pilot name array
        public static short FIRST_COMMANDER;
        public static short FIRST_WINGMAN;
        public static short LAST_WINGMAN;
        public static short MAX_AA_STR_FOR_ESCORT;				// Max pack Air to Air str in order to add escorts
        public static short BARCAP_REQUEST_INTERVAL;			// Minimum time between BARCAP mission requests
        public static short ONCALL_CAS_FLIGHTS_PER_REQUEST;	// How many CAS flights to plan per ONCALL request
        public static short MIN_TASK_AIR;						// Retask time, in minutes
        public static short MIN_PLAN_AIR;						// Replan time (KCK NOTE: PLAN_AIR max is 8)
        public static short VICTORY_CHECK_TIME;				// Check victory condition time in Tactical Engagement
        public static short FLIGHT_MOVE_CHECK_INTERVAL;		// How often to check a flight for movement (in seconds)
        public static short FLIGHT_COMBAT_CHECK_INTERVAL;		// How often to check a flight for combat (in seconds)
        public static short AIR_UPDATE_CHECK_INTERVAL;			// How often to check squadrons/packages for movement/update (in seconds)
        public static short FLIGHT_COMBAT_RATE;				// Amount of time between weapon shots (in seconds)
        public static short PACKAGE_CYCLES_TO_WAIT;			// How many planning cycles to wait for tankers/barcap/etc
        public static short AIR_PATH_MAX;						// Max amount of nodes to search for air paths
        public static float MIN_IGNORE_RANGE;					// Minimum range (km) at which we'll ignore an air target
        public static short MAX_SAR_DIST;						// Max distance from front to fly sar choppers into. (km)
        public static short MAX_BAI_DIST;						// Max distance from front to fly BAI missions into. (km)
        public static long PILOT_ASSIGN_TIME;					// Time before takeoff to assign pilots
        public static short AIRCRAFT_TURNAROUND_TIME_MINUTES;	// Time to wait between missions before an aircraft is available again

        // GTM Inputs
        public static short PRIMARY_OBJ_PRIORITY;				// Priorities for primary and secondary objectives
        public static short SECONDARY_OBJ_PRIORITY;			// Only cities and towns can be POs and SOs
        public static short MAX_OFFENSES;						// Maximum number of offensive POs.
        public static short MIN_OFFENSIVE_UNITS;				// Minimum number of units to constitute an offensive
        public static short MINIMUM_ADJUSTED_LEVEL;			// Minimum % strength of unit assigned to offense.
        public static short MINIMUM_VIABLE_LEVEL;				// Minimum % strength in order to be considered a viable unit
        public static short MAX_ASSIGNMENT_DIST;				// Maximum distance from an object we'll assign a unit to
        public static short MAXLINKS_FROM_SO_OFFENSIVE;		// Maximum objective links from our secondary objective we'll place units
        public static short MAXLINKS_FROM_SO_DEFENSIVE;		// Same as above for defensive units
        public static short BRIGADE_BREAK_BASE;				// Base moral a brigade breaks at (modified by objective priority)
        public static short ROLE_SCORE_MODIFIER;				// Used to adjust role score for scoring purposes
        public static short MIN_TASK_GROUND;					// Retask time, in minutes
        public static short MIN_PLAN_GROUND;					// Replan time, in minutes
        public static short MIN_REPAIR_OBJECTIVES;				// How often to repair objectives (in minutes);
        public static short MIN_RESUPPLY;						// How often to resupply troops (in minutes)
        public static short MORALE_REGAIN_RATE;				// How much morale per hour a typical unit will regain
        public static short REGAIN_RATE_MULTIPLIER_FOR_TE;		// Morale/Fatigue regain rate multiplier for TE missions
        public static short FOOT_MOVE_CHECK_INTERVAL;			// How often to check a foot battalion for movement (in seconds)
        public static short TRACKED_MOVE_CHECK_INTERVAL;		// How often to check a tracked/wheeled battalion for combat (in seconds)
        public static short GROUND_COMBAT_CHECK_INTERVAL;		// How often to check ground battalions for combat
        public static short GROUND_UPDATE_CHECK_INTERVAL;		// How often to check brigades for update (in seconds)
        public static short GROUND_COMBAT_RATE;				// Amount of time between weapon shots (in seconds)
        public static short GROUND_PATH_MAX;					// Max amount of nodes to search for ground paths
        public static short OBJ_GROUND_PATH_MAX_SEARCH;		// Max amount of nodes to search for objective paths
        public static short OBJ_GROUND_PATH_MAX_COST;			// Max amount of nodes to search for objective paths
        public static short MIN_FULL_OFFENSIVE_INITIATIVE;		// Minimum initiative required to launch a full offensive
        public static short MIN_COUNTER_ATTACK_INITIATIVE;		// Minimum initiative required to launch a counter-attack
        public static short MIN_SECURE_INITIATIVE;				// Minimum initiative required to secure defensive objectives
        public static short MINIMUM_EXP_TO_FIRE_PREGUIDE;		// Minimum experience needed to fire SAMs before going to guide mode
        public static ulong OFFENSIVE_REINFORCEMENT_TIME;		// How often we get reinforcements when on the offensive
        public static ulong DEFENSIVE_REINFORCEMENT_TIME;		// How often we get reinforcements when on the defensive
        public static ulong CONSOLIDATE_REINFORCEMENT_TIME;		// How often we get reinforcements when consolidating
        public static ulong ACTION_PREP_TIME;					// How far in advance to plan any offensives

        // NTM Inputs
        public static short MIN_TASK_NAVAL;					// Retask time, in minutes	
        public static short MIN_PLAN_NAVAL;					// Replan time, in minutes
        public static short NAVAL_MOVE_CHECK_INTERVAL;			// How often to check a task force for movement (in seconds)
        public static short NAVAL_COMBAT_CHECK_INTERVAL;		// How often to fire naval units (in seconds)
        public static short NAVAL_COMBAT_RATE;					// Amount of time between weapon shots (in seconds)

        // Other Inputs
        public static short LOW_ALTITUDE_CUTOFF;
        public static short MAX_GROUND_SEARCH;	          		// How far to look for surface units or objectives
        public static short MAX_AIR_SEARCH;		           	// How far to look for air threats
        public static short NEW_CLOUD_CHANCE;
        public static short DEL_CLOUD_CHANCE;
        public static short PLAYER_BUBBLE_MOVERS;				// The # of objects we try to keep in player bubble
        public static short FALCON_PLAYER_TEAM;				// What team the player starts on
        public static short CAMP_RESYNC_TIME;					// How often to resync campaigns (in seconds)
        public static ushort BUBBLE_REBUILD_TIME;				// How often to rebuild the player bubble (in seconds)
        public static short STANDARD_EVENT_LENGTH;				// Length of stored event queues
        public static short PRIORITY_EVENT_LENGTH;
        public static short MIN_RECALCULATE_STATISTICS;		// How often to do the unit statistics stuff
        public static float LOWAIR_RANGE_MODIFIER;				// % of Air range to get LowAir range from
        public static short MINIMUM_STRENGTH;					// Minimum strength points we'll bother appling with colateral damage
        public static short MAX_DAMAGE_TRIES;					// Max # of times we'll attempt to find a random target vehicle/feature
        public static float REAGREGATION_RATIO;				// Ratio over 1.0 which things will reaggregate at
        public static short MIN_REINFORCE_TIME;				// Value, in minutes, of each reinforcement cycle
        public static float SIM_BUBBLE_SIZE;					// Radius (in feet) of the Sim's campaign lists.
        public static short INITIATIVE_LEAK_PER_HOUR;			// How much initiative is adjusted automatically per hour

        // Campaign Inputs
        public static int StartOffBonusRepl;					// Replacement bonus when a team goes offensive
        public static int StartOffBonusSup;					// Supply Bonus when a team goes offensive
        public static int StartOffBonusFuel;					// Fuel Bonus when a team goes offensive
        public static int ActionRate;							// How often we can start a new action (in hours)
        public static int ActionTimeOut;						// Maximum time an offensive action can last
        public static float DataRateModRepl;					// Modification factor for production data rate for replacements
        public static float DataRateModSup;					// Modification factor for production data rate for fuel and supply
        public static float RelSquadBonus;						// Relative replacements of squadrons to ground units
        // in fact bools
        public static int NoActionBonusProd;					// No action bonus for production
        public static int NoTypeBonusRepl;						// No type bonus for replacements, new supply system, bugfix in "supplies units" function
        public static int NewInitiativePoints;					// New initiative point setting
        public static int CampBugFixes;						// Smaller bugfixes
        public static float HitChanceAir;						// 2D hitchance air target modulation
        public static float MIN_DEAD_PCT;						// Strength below which a reaggregating vehicle will be considered dead
        public static float HitChanceGround;					// 2D hitchance ground target modulation
        // These are in fact read from the trigger files now - no need to mess with the campaign files !
        public static float FLOTDrawDistance;					// Distance between FLOT objectives at which we won't draw a line anymore (2 front campaigns...)
        public static int FLOTSortDirection;					// to sort FLOT by x or y objective coordinates
        public static int TheaterXPosition;					// central theater x/y position for calculating new bullseye posit
        public static int TheaterYPosition;					// central theater x/y position for calculating new bullseye posit

        public static void ReadCampAIInputs(string name)
        {
            string tmpName;
            short temp;

            tmpName = name + ".AII";
            string fileName = F4File.FalconCampUserSaveDirectory + F4File.Sep + tmpName; //TODO F4File.F4FindFile(tmpName);

            MultiplatformIni initFile = new MultiplatformIni(fileName);
            /* ATM Inputs */
            IMMEDIATE_MIN_TIME = (short)initFile.ReadInt("ATM", "ImmediatePlanMinTime", 0);
            IMMEDIATE_MAX_TIME = (short)initFile.ReadInt("ATM", "ImmediatePlanMaxTime", 0);
            LONGRANGE_MIN_TIME = (short)initFile.ReadInt("ATM", "LongrangePlanMinTime", 0);
            LONGRANGE_MAX_TIME = (short)initFile.ReadInt("ATM", "LongrangePlanMaxTime", 0);
            ATM_ASSIGN_RATIO = (short)initFile.ReadInt("ATM", "AircraftAssignmentRatio", 0);
            MAX_FLYMISSION_THREAT = (short)initFile.ReadInt("ATM", "MaxFlymissionThreat", 0);
            MAX_FLYMISSION_HIGHTHREAT = (short)initFile.ReadInt("ATM", "MaxFlymissionHighThreat", 0);
            MAX_FLYMISSION_NOTHREAT = (short)initFile.ReadInt("ATM", "MaxFlymissionNoThreat", 0);
            MIN_SEADESCORT_THREAT = (short)initFile.ReadInt("ATM", "MinSeadescortThreat", 0);
            MIN_AVOID_THREAT = (short)initFile.ReadInt("ATM", "MinAvoidThreat", 0);
            MIN_AP_DISTANCE = (short)initFile.ReadInt("ATM", "MinAssemblyPtDist", 0);
            BREAKPOINT_DISTANCE = (short)initFile.ReadInt("ATM", "BreakpointDist", 0);
            ATM_TARGET_CLEAR_RADIUS = (short)initFile.ReadInt("ATM", "TargetClearRadius", 0);
            LOITER_DIST = (short)initFile.ReadInt("ATM", "LoiterTurnDistance", 0);
            SWEEP_DISTANCE = (short)initFile.ReadInt("ATM", "SweepRadius", 0);
            MINIMUM_BDA_PRIORITY = (short)initFile.ReadInt("ATM", "MinimumBDAPriority", 0);
            MINIMUM_AWACS_DISTANCE = (short)initFile.ReadInt("ATM", "MinimumAWACSDistance", 0);
            MAXIMUM_AWACS_DISTANCE = (short)initFile.ReadInt("ATM", "MaximumAWACSDistance", 0);
            MINIMUM_JSTAR_DISTANCE = (short)initFile.ReadInt("ATM", "MinimumJSTARDistance", 0);
            MAXIMUM_JSTAR_DISTANCE = (short)initFile.ReadInt("ATM", "MaximumJSTARDistance", 0);
            MINIMUM_TANKER_DISTANCE = (short)initFile.ReadInt("ATM", "MinimumTankerDistance", 0);
            MAXIMUM_TANKER_DISTANCE = (short)initFile.ReadInt("ATM", "MaximumTankerDistance", 0);
            MINIMUM_ECM_DISTANCE = (short)initFile.ReadInt("ATM", "MinimumECMDistance", 0);
            MAXIMUM_ECM_DISTANCE = (short)initFile.ReadInt("ATM", "MaximumECMDistance", 0);
            FIRST_COLONEL = (short)initFile.ReadInt("ATM", "FirstColonel", 0);
            FIRST_COMMANDER = (short)initFile.ReadInt("ATM", "FirstCommander", 0);
            FIRST_WINGMAN = (short)initFile.ReadInt("ATM", "FirstWingman", 0);
            LAST_WINGMAN = (short)initFile.ReadInt("ATM", "LastWingman", 0);
            MAX_AA_STR_FOR_ESCORT = (short)initFile.ReadInt("ATM", "MaxEscortAAStrength", 0);
            BARCAP_REQUEST_INTERVAL = (short)initFile.ReadInt("ATM", "BARCAPRequestInterval", 0);
            ONCALL_CAS_FLIGHTS_PER_REQUEST = (short)initFile.ReadInt("ATM", "OnCallFlightsPerRequest", 0);
            MIN_TASK_AIR = (short)initFile.ReadInt("ATM", "AirTaskTime", 0);
            MIN_PLAN_AIR = (short)initFile.ReadInt("ATM", "AirPlanTime", 0);
            VICTORY_CHECK_TIME = (short)initFile.ReadInt("ATM", "VictoryConditionTime", 0);
            FLIGHT_MOVE_CHECK_INTERVAL = (short)initFile.ReadInt("ATM", "FlightMoveCheckInterval", 0);
            FLIGHT_COMBAT_CHECK_INTERVAL = (short)initFile.ReadInt("ATM", "FlightCombatCheckInterval", 0);
            AIR_UPDATE_CHECK_INTERVAL = (short)initFile.ReadInt("ATM", "AirUpdateCheckInterval", 0);
            FLIGHT_COMBAT_RATE = (short)initFile.ReadInt("ATM", "FlightCombatRate", 0);
            PACKAGE_CYCLES_TO_WAIT = (short)initFile.ReadInt("ATM", "PackageCyclesToWait", 0);
            AIR_PATH_MAX = (short)initFile.ReadInt("ATM", "AirPathMax", 0);
            MIN_IGNORE_RANGE = (float)initFile.ReadInt("ATM", "MinimumIgnoreRange", 0);
            MAX_SAR_DIST = (short)initFile.ReadInt("ATM", "MaxSARDistance", 0);
            MAX_BAI_DIST = (short)initFile.ReadInt("ATM", "MaxBAIDistance", 0);
            PILOT_ASSIGN_TIME = (long)initFile.ReadInt("ATM", "PilotAssignTime", 0);
            AIRCRAFT_TURNAROUND_TIME_MINUTES = (short)initFile.ReadInt("ATM", "AircraftTurnaroundMinutes", 0);

            /* GTM Inputs */
            PRIMARY_OBJ_PRIORITY = (short)initFile.ReadInt("GTM", "PrimaryObjPriority", 0);
            SECONDARY_OBJ_PRIORITY = (short)initFile.ReadInt("GTM", "SecondaryObjPriority", 0);
            MAX_OFFENSES = (short)initFile.ReadInt("GTM", "MaximumOffenses", 0);
            MIN_OFFENSIVE_UNITS = (short)initFile.ReadInt("GTM", "MinimumOffensiveUnits", 0);
            MINIMUM_ADJUSTED_LEVEL = (short)initFile.ReadInt("GTM", "MinimumAdjustedLevel", 0);
            MINIMUM_VIABLE_LEVEL = (short)initFile.ReadInt("GTM", "MinimumViableLevel", 0);
            MAX_ASSIGNMENT_DIST = (short)initFile.ReadInt("GTM", "MaximumAssignmentDist", 0);
            MAXLINKS_FROM_SO_OFFENSIVE = (short)initFile.ReadInt("GTM", "MaximumOffensiveLinks", 0);
            MAXLINKS_FROM_SO_DEFENSIVE = (short)initFile.ReadInt("GTM", "MaximumDefensiveLinks", 0);
            BRIGADE_BREAK_BASE = (short)initFile.ReadInt("GTM", "BrigadeBreakBase", 0);
            ROLE_SCORE_MODIFIER = (short)initFile.ReadInt("GTM", "RoleScoreModifier", 0);
            MIN_TASK_GROUND = (short)initFile.ReadInt("GTM", "TaskGroundTime", 0);
            MIN_PLAN_GROUND = (short)initFile.ReadInt("GTM", "PlanGroundTime", 0);
            MIN_REPAIR_OBJECTIVES = (short)initFile.ReadInt("GTM", "ObjectiveRepairInterval", 0);
            MIN_RESUPPLY = (short)initFile.ReadInt("GTM", "ResupplyInterval", 0);
            MORALE_REGAIN_RATE = (short)initFile.ReadInt("GTM", "MoraleRegainRate", 0);
            REGAIN_RATE_MULTIPLIER_FOR_TE = (short)initFile.ReadInt("GTM", "TERegainRateMultiplier", 0);
            FOOT_MOVE_CHECK_INTERVAL = (short)initFile.ReadInt("GTM", "FootMoveCheckInterval", 0);
            TRACKED_MOVE_CHECK_INTERVAL = (short)initFile.ReadInt("GTM", "TrackedMoveCheckInterval", 0);
            GROUND_COMBAT_CHECK_INTERVAL = (short)initFile.ReadInt("GTM", "GroundCombatCheckInterval", 0);
            GROUND_UPDATE_CHECK_INTERVAL = (short)initFile.ReadInt("GTM", "GroundUpdateCheckInterval", 0);
            GROUND_COMBAT_RATE = (short)initFile.ReadInt("GTM", "GroundCombatRate", 0);
            GROUND_PATH_MAX = (short)initFile.ReadInt("GTM", "GroundPathMax", 0);
            OBJ_GROUND_PATH_MAX_SEARCH = (short)initFile.ReadInt("GTM", "ObjGroundPathMaxSearch", 0);
            OBJ_GROUND_PATH_MAX_COST = (short)initFile.ReadInt("GTM", "ObjGroundPathMaxCost", 0);
            MIN_FULL_OFFENSIVE_INITIATIVE = (short)initFile.ReadInt("GTM", "MinFullOffensiveInitiative", 0);
            MIN_COUNTER_ATTACK_INITIATIVE = (short)initFile.ReadInt("GTM", "MinCounterAttackInitiative", 0);
            MIN_SECURE_INITIATIVE = (short)initFile.ReadInt("GTM", "MinSecureInitiative", 0);
            MINIMUM_EXP_TO_FIRE_PREGUIDE = (short)initFile.ReadInt("GTM", "MinExpToFirePreGuide", 0);
            OFFENSIVE_REINFORCEMENT_TIME = (ulong)initFile.ReadInt("GTM", "OffensiveReinforcementTime", 0) * CampaignTime.CampaignMinutes;
            DEFENSIVE_REINFORCEMENT_TIME = (ulong)initFile.ReadInt("GTM", "DefensiveReinforcementTime", 0) * CampaignTime.CampaignMinutes;
            CONSOLIDATE_REINFORCEMENT_TIME = (ulong)initFile.ReadInt("GTM", "ConsolidateReinforcementTime", 0) * CampaignTime.CampaignMinutes;
            ACTION_PREP_TIME = (ulong)initFile.ReadInt("GTM", "ActionPrepTime", 0) * CampaignTime.CampaignMinutes;

            /* NTM Inputs */
            MIN_TASK_NAVAL = (short)initFile.ReadInt("NTM", "TaskNavalTime", 0);
            MIN_PLAN_NAVAL = (short)initFile.ReadInt("NTM", "PlanNavalTime", 0);
            NAVAL_COMBAT_CHECK_INTERVAL = (short)initFile.ReadInt("NTM", "NavalCombatCheckInterval", 0);
            NAVAL_MOVE_CHECK_INTERVAL = (short)initFile.ReadInt("NTM", "NavalMoveCheckInterval", 0);
            NAVAL_COMBAT_RATE = (short)initFile.ReadInt("NTM", "NavalCombatRate", 0);

            /* Other */
            LOW_ALTITUDE_CUTOFF = (short)initFile.ReadInt("Other", "LowAltitudeCutoff", 0);
            MAX_GROUND_SEARCH = (short)initFile.ReadInt("Other", "GroundSearchDistance", 0);
            MAX_AIR_SEARCH = (short)initFile.ReadInt("Other", "AirSearchDistance", 0);
            NEW_CLOUD_CHANCE = (short)initFile.ReadInt("Other", "NewCloudPercentage", 0);
            DEL_CLOUD_CHANCE = (short)initFile.ReadInt("Other", "DeleteCloudPercentage", 0);
            PLAYER_BUBBLE_MOVERS = (short)initFile.ReadInt("Other", "PlayerBubbleObjects", 0);
            FALCON_PLAYER_TEAM = (short)initFile.ReadInt("Other", "PlayerTeam", 0);
            CAMP_RESYNC_TIME = (short)initFile.ReadInt("Other", "CampaignResyncTime", 0);
            BUBBLE_REBUILD_TIME = (ushort)initFile.ReadInt("Other", "BubbleRebuildTime", 0);
            STANDARD_EVENT_LENGTH = (short)initFile.ReadInt("Other", "StandardEventqueueLength", 0);				// Length of stored event queues
            PRIORITY_EVENT_LENGTH = (short)initFile.ReadInt("Other", "PriorityEventqueueLength", 0);
            MIN_RECALCULATE_STATISTICS = (short)initFile.ReadInt("Other", "StatisticsRecalculationInterval", 0);
            temp = (short)initFile.ReadInt("Other", "LowAirRangeModifier", 0);
            LOWAIR_RANGE_MODIFIER = (float)(temp / 100.0F);
            MINIMUM_STRENGTH = (short)initFile.ReadInt("Other", "MinimumStrength", 0);
            MAX_DAMAGE_TRIES = (short)initFile.ReadInt("Other", "MaximumDamageTries", 0);
            temp = (short)initFile.ReadInt("Other", "ReaggregationRatio", 0);
            REAGREGATION_RATIO = (float)(temp / 100.0F);
            MIN_REINFORCE_TIME = (short)initFile.ReadInt("Other", "ReinforcementTime", 0);
            SIM_BUBBLE_SIZE = (float)initFile.ReadInt("Other", "SimBubbleSize", 0);
            INITIATIVE_LEAK_PER_HOUR = (short)initFile.ReadInt("Other", "InitiativeLeakPerHour", 0);

            /* Campaign - MPS defaults */
            StartOffBonusRepl = (int)initFile.ReadInt("Campaign", "StartOffBonusRepl", 1000);
            StartOffBonusSup = (int)initFile.ReadInt("Campaign", "StartOffBonusSup", 5000);
            StartOffBonusFuel = (int)initFile.ReadInt("Campaign", "StartOffBonusFuel", 5000);
            ActionRate = (int)initFile.ReadInt("Campaign", "ActionRate", 8);
            ActionTimeOut = (int)initFile.ReadInt("Campaign", "ActionTimeOut", 24);

            tmpName = initFile.ReadString("Campaign", "DataRateModRepl", "1.0");
            DataRateModRepl = float.Parse(tmpName, CultureInfo.InvariantCulture);

            tmpName = initFile.ReadString("Campaign", "DataRateModSup", "1.0");
            DataRateModSup = float.Parse(tmpName, CultureInfo.InvariantCulture);

            tmpName = initFile.ReadString("Campaign", "RelSquadBonus", "4.0");
            RelSquadBonus = float.Parse(tmpName, CultureInfo.InvariantCulture);

            NoActionBonusProd = (int)initFile.ReadInt("Campaign", "NoActionBonusProd", 0);
            NoTypeBonusRepl = (int)initFile.ReadInt("Campaign", "NoTypeBonusRepl", 0);
            NewInitiativePoints = (int)initFile.ReadInt("Campaign", "NewInitiativePoints", 0);
            CampBugFixes = (int)initFile.ReadInt("Campaign", "CampBugFixes", 0);

            tmpName = initFile.ReadString("Campaign", "2DHitChanceAir", "3.5");
            HitChanceAir = float.Parse(tmpName, CultureInfo.InvariantCulture);
            tmpName = initFile.ReadString("Campaign", "2DHitChanceGround", "1.5");
            HitChanceGround = float.Parse(tmpName, CultureInfo.InvariantCulture);
            tmpName = initFile.ReadString("Campaign", "MinDeadPct", "0.6");
            MIN_DEAD_PCT = float.Parse(tmpName, CultureInfo.InvariantCulture);
            // 2002-04-17 MN these are now read from the campaign trigger files to have them definable per campaign.
            //	GetPrivateProfileString("Campaign","FLOTDrawDistance", "2500.0", tmpName, 40); // 50 km
            //	FLOTDrawDistance	= (float)atof(tmpName);
            //	FLOTSortDirection	= (int)initFile.ReadInt("Campaign","SortNorthSouth", 0);
            //	TheaterXPosition	= (int)initFile.ReadInt("Campaign","TheaterXPosit", 512); // default place in center of korea sized theaters
            //	TheaterYPosition	= (int)initFile.ReadInt("Campaign","TheaterYPosit", 512); // default place in center of korea sized theaters
        }
    }
}

