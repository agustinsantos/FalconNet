using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FalconNet.Sim;
using System.Diagnostics;
using System.Globalization;

namespace FalconNet.UI
{
	public class ConfigOption<T>
	{
		public string Name;
		public T Value;

		public bool CheckID (string str)
		{
			return str.Substring (3).ToLower ().Equals (Name);
		}
	};

	public static class F4Config
	{
		#if TODO
		extern "C" g_nBWMaxDeltaTime;	// needed to link into C files (capi.c)
		extern "C" g_nBWCheckDeltaTime;
		
		// #define OPENEYES //; disabled until futher
		//bool g_bEnableCATIIIExtension = false;	//MI replaced with g_bRealisticAvionics
		#endif
		public static bool g_bForceDXMultiThreadedCoopLevel = true; // JB 010401 Safer according to e
		public static int g_nThrottleMode = 0;
		public static bool g_bEnableABRelocation = false;
		public static bool g_bEnableWeatherExtensions = true;
		public static bool g_bEnableWindsAloft = false;
		public static bool g_bEnableColorMfd = true;
		public static bool g_bEPAFRadarCues = false;
		public static bool g_bRadarJamChevrons = false;
		public static bool g_bAWACSSupport = false;
		public static bool g_bAWACSRequired = false;
		public static bool g_bAWACSBackground = false;
		public static bool g_bMLU = false;
		public static bool g_bServer = false;
		public static bool g_bServerHostAll = false;
		public static bool g_bLogEvents = false;
		public static bool g_bVoiceCom = true;
		public static bool g_bwoeir = false;
		public static int g_nPadlockBoxSize = 2;
		public static int g_nDeagTimer = 0;
		public static int g_nReagTimer = 0;
		public static bool g_bShowFlaps = false;
		public static bool g_bLowBwVoice = false;
		public static float clientbwforupdatesmodifyer = 0.7f;
		public static float hostbwforupdatesmodifyer = 0.7f;
		public static int g_nMaxUIRefresh = 16; // 2002-02-23 S.G. To limit the UI refresh rate to prevent from running out of resources because it can't keep up with the icons (ie planes) to display on the map.
		public static int g_nUnidentifiedInUI = 1; // 2002-02-24 S.G. To limit the UI refresh rate to prevent from running out of resources because it can't keep up with the icons (ie planes) to display on the map.
		public static float g_fIdentFactor = 0.75f; // 2002-03-07 S.G. So identification is not at full detect range but a factor of it
		public static bool g_bLimit2DRadarFight = true; // 2002-03-07 S.G. So 2D fights are limited in min altitude and range like their 3D counterpart
		public static bool g_bAdvancedGroundChooseWeapon = true; // 2002-03-08 S.G. So 3D ground vehicle choose the best weapon based target min/max altitude and min/max range while the original code was just range (max range, not min range)
		public static bool g_bUseNewCanEnage = true; // 2002-03-11 S.G. SensorFusion and CanEngage will use the 'GetIdentified' code instead of always knowing the combat type of the enemy
		public static int g_nLowestSkillForGCI = 3; // 2002-03-12 S.G. Externalized the lowest skill that can use GCI
		public static int g_nAIVisualRetentionTime = 24 * 1000; // 2002-03-12 S.G. Time before AI looses sight of its target
		public static int g_nAIVisualRetentionSkill = 2 * 1000; // 2002-03-12 S.G. Time before AI looses sight of its target (skill related)
		public static float g_fBiasFactorForFlaks = 100000.0f; // 2002-03-12 S.G. Defaults bias for flaks. See guns.cpp
		public static bool g_bUseSkillForFlaks = true; // 2002-03-12 S.G. If flaks uses the skill of the ground troop or not
		public static float g_fTracerAccuracyFactor = 0.1f; // 2002-03-12 S.G. For tracers, multiply the dispersion (tracerError) by this value
		public static bool g_bToggleAAAGunFlag = false; // 2002-03-12 S.G. RP5 have set the AAA flag for NONE AAA guns and have reset it for AAA guns! This flag toggle the AAA gun check in the code
		public static bool g_bUseComplexBVRForPlayerAI = false; // 2002-03-13 S.G. If false, Player's wingman will perform RP5 BVR code instead of the SP2 BVR code
		public static float g_fFuelBaseProp = 100.0f;	// 2002-03-14 S.G. For better fuel consomption tweaking
		public static float g_fFuelMultProp = 0.008f;	// 2002-03-14 S.G. For better fuel consomption tweaking
		public static float g_fFuelTimeStep = 0.001f;	// 2002-03-14 S.G. For better fuel consomption tweaking
		public static bool g_bFuelUseVtDot = true;	// 2002-03-14 S.G. For better fuel consomption tweaking
		public static float g_fFuelVtClip = 5.0f;	// 2002-03-14 S.G. For better fuel consomption tweaking
		public static float g_fFuelVtDotMult = 5.0f;	// 2002-03-14 S.G. For better fuel consomption tweaking
		public static bool g_bFuelLimitBecauseVtDot = true;	// 2002-03-14 S.G. For better fuel consomption tweaking
		public static float g_fSearchSimTargetFromRangeSqr = (20.0F * Phyconst.NM_TO_FT) * (20.0F * Phyconst.NM_TO_FT);	// 2002-03-15 S.G. Will lookup Sim target instead of using the campain target from this range
		public static bool g_bUseAggresiveIncompleteA2G = true; // 2002-03-22 S.G. If false, AI on incomplete A2G missions will be defensive
		public static float g_fHotNoseAngle = 50.0f;	 // 2002-03-22 S.G. Default angle (in degrees) before considering the target pointing at us
		public static float g_fMaxMARNoIdA = 10.0f;	 // 2002-03-22 ADDED BY S.G. Max Start MAR for this type of aicraft when target is NOT ID'ed, fast
		public static float g_fMinMARNoId5kA = 5.0f;	 // 2002-03-22 ADDED BY S.G. MinMAR for this type of aicraft when target is NOT ID'ed, fast and below 5K
		public static float g_fMinMARNoId18kA = 12.0f; // 2002-03-22 ADDED BY S.G. MinMAR for this type of aicraft when target is NOT ID'ed, fast  and below 18K
		public static float g_fMinMARNoId28kA = 17.0f; // 2002-03-22 ADDED BY S.G. MinMAR for this type of aicraft when target is NOT ID'ed, fast  and below 28K
		public static float g_fMaxMARNoIdB = 5.0f;	 // 2002-03-22 ADDED BY S.G. Max Start MAR for this type of aicraft when target is NOT ID'ed, medium
		public static float g_fMinMARNoId5kB = 3.0f;	 // 2002-03-22 ADDED BY S.G. MinMAR for this type of aicraft when target is NOT ID'ed, medium and below 5K
		public static float g_fMinMARNoId18kB = 5.0f;	 // 2002-03-22 ADDED BY S.G. MinMAR for this type of aicraft when target is NOT ID'ed, medium and below 18K
		public static float g_fMinMARNoId28kB = 8.0f;  // 2002-03-22 ADDED BY S.G. MinMAR for this type of aicraft when target is NOT ID'ed, medium and below 28K
		public static float g_fMinMARNoIdC = 5.0f;	 // 2002-03-22 ADDED BY S.G. MinMAR for this type of aicraft when target is NOT ID'ed
		// DEMON - This gets messy. Back to false.
		public static bool g_bOldStackDump = false;	 // 2002-04-01 ADDED BY S.G. Also output the stack dump in the old format when generating a crashlog.
		public static float g_fSSoffsetManeuverPoints1a = 5.0f; // 2002-04-07 ADDED BY S.G. Externalize the offset used in the AiInitSSOffset code
		public static float g_fSSoffsetManeuverPoints1b = 5.0f; // 2002-04-07 ADDED BY S.G. Externalize the offset used in the AiInitSSOffset code
		public static float g_fSSoffsetManeuverPoints2a = 4.0f; // 2002-04-07 ADDED BY S.G. Externalize the offset used in the AiInitSSOffset code
		public static float g_fSSoffsetManeuverPoints2b = 5.0f; // 2002-04-07 ADDED BY S.G. Externalize the offset used in the AiInitSSOffset code
		public static float g_fPinceManeuverPoints1a = 5.0f; // 2002-04-07 ADDED BY S.G. Externalize the offset used in the AiInitPince code
		public static float g_fPinceManeuverPoints1b = 5.0f; // 2002-04-07 ADDED BY S.G. Externalize the offset used in the AiInitPince code
		public static float g_fPinceManeuverPoints2a = 4.0f; // 2002-04-07 ADDED BY S.G. Externalize the offset used in the AiInitPince code
		public static float g_fPinceManeuverPoints2b = 5.0f; // 2002-04-07 ADDED BY S.G. Externalize the offset used in the AiInitPince code
		public static bool g_bUseDefinedGunDomain = false; // 2002-04-17 ADDED BY S.G. Instead of 'fudging' the weapon domain, if it's set to true, use the weapon domain set in the data file

		// 2000-11-24 ADDED BY S.G. FOR THE 'new padlock' code
		public const int PLockModeNormal = 0;
		public const int  PLockModeNearLabelColor = 1;
		public const int  PLockModeNoSnap = 2;
		public const int  PLockModeBreakLock = 4;
		public const int  PLockNoTrees = 8;
		public static int g_nPadlockMode = PLockModeNoSnap | PLockModeBreakLock | PLockNoTrees;

		// 2001-08-31 ADDED BY S.G. FOR AIRBASE RELOCATION CHOICE
		//TODO #define AirBaseRelocTeamOnly 1
		//TODO #define AirBaseRelocNoFar 2
		public static int g_nAirbaseReloc = 2;

		// 2002-01-30 ADDED BY S.G. Pitch limiter for AI activation variable
		public static bool g_bPitchLimiterForAI = true;

		// 2002-01-29 ADDED BY S.G. Default timeout for target bubbles
		public static int g_nTargetSpotTimeout = 2 * 60 * 1000;

		// 2001-09-07 ADDED BY S.G. FOR RP5 COMPATIBILITY DATA TEST
		public static bool g_bRP5Comp = true;
		public static int NumHats; //TODO Estaba como external
		public static bool g_bEnableNonPersistentTextures = false;
		public static bool g_bEnableStaticTerrainTextures = false;
		//bool g_bEnableAircraftLimits = false;		//MI replaced with g_bRealisticAvionics
		//bool g_bArmingDelay = false;				//MI replaced with g_bRealisticAvionics
		//bool g_bHardCoreReal = false;				//MI replaced with g_bRealisticAvionics
		public static bool g_bCheckBltStatusBeforeFlip = true;
		public static bool g_bForceSoftwareGUI = false;
		public static bool g_bUseMipMaps = false;
		public static bool g_bShowMipUsage = false;
		public static bool g_bUse3dSound = false; // JPO
		public static bool g_bOldSoundAlg = true; // JPO
		public static bool g_bMFDHighContrast = false; // JPO
		public static bool g_bPowerGrid = true; // JPO
		public static bool g_bUseMappedFiles = false; // JPO
		//bool g_bUserRadioVoice = false; // JPO	// M.N. transferred into UI/PlayerOp structure
		public static bool g_bNewRackData = true; // JPO
		public static bool g_bNewAcmiHud = true; // JPO
		public static int g_nLowDetailFactor = 0; // JPO - adjustment to the LOD show at low level

		public static float g_fMipLodBias;
		public static float g_fCloudMinHeight = -1.0f; // JPO
		public static float g_fRadarScale = 1.0f; // JPO
		public static float g_fCursorSpeed = 1.0f; // JPO
		public static float g_fMinCloudWeather = 1500; // JPO
		public static float g_fCloudThicknessFactor = 4000; //JPO

		public static float g_fdwPortclient = 2937;
		public static float g_fdwPorthost = 2936;
		public static bool g_bEnableUplink = false;
		public static string g_strMasterServerName;
		public static int g_nMasterServerPort = 0;
		public static string g_strServerName;
		public static string g_strServerLocation;
		public static string g_strServerAdmin;
		public static string g_strServerAdminEmail;
		public static string g_strVoiceHostIP;
		public static string g_strWorldName;


		// JB
#if _DEBUG
		public static int g_nNearLabelLimit = 150; // nm		M.N.
#else
		public static int g_nNearLabelLimit = 20; // nm
#endif
		public static int g_nACMIOptionsPopupHiResX = 666;
		public static int g_nACMIOptionsPopupHiResY = 581;
		public static int g_nACMIOptionsPopupLowResX = 500;
		public static int g_nACMIOptionsPopupLowResY = 500;
		public static bool g_bNeedSimThreadToRemoveObject = false;
		public static int g_nMaxSimTimeAcceleration = 64; // JB 020315
		public static bool g_bMPStartRestricted = false; // JB 0203111 Restrict takeoff/ramp options.
		public static int g_nMPStartTime = 5; // JB 0203111 MP takeoff time if g_bMPStartRestricted enabled.
		public static int g_nFFEffectAutoCenter = -1; // JB 020306 Don't stop the centering FF effect (-1 disabled)
		public static bool g_bMissionACIcons = true; // JB 020211 Load the correct mission icons for each type of aircraft
		public static float g_fRecoveryAOA = 35.0F; // JB 020125 Specify the max AOA at which you can recover from a deep stall.
		public static int g_nRNESpeed = 1; // JB 020123 More realistic No Escape DLZ.  Specify higher g_nRNESpeed to lower calculated RNE ranges.
		public static bool g_bSlowButSafe = false; // JB 020115 Turn on extra ISBad CTD checks
		public static float g_fCarrierStartTolerance = 6.0f; // JB 020117 How high can an aircraft be off the water to be "on" the carrier.
		public static bool g_bNewDamageEffects = true;
		public static bool g_bDisableFunkyChicken = true;
		public static bool g_bSmartScaling = false; // JB 010112
		public static bool g_bFloatingBullseye = false;// JB/Codec 010115
		public static bool g_bDisableCrashEjectCourtMartials = true; // JB 010118
		public static bool g_bSmartCombatAP = true; // JB 010224
		public static bool g_bVoodoo12Compatible = false; // JB 010330 Disables the cockpit kneemap to prevent CTDs on the Voodoo 1 and 2.
		public static float g_fDragDilutionFactor = 1.0f; // JB 010707
		public static bool g_bRealisticAttrition = false; // JB 010710
		public static bool g_bIFFRWR = false; // JB 010727
		public static int g_nRelocationWait = 3; // JB 010728
		public static int g_nLoadoutTimeLimit = 120; // JB 010729 Time limit in seconds before takeoff when you can change your loadout.
		public static float g_fLatitude = 38.0f; // JB 010804 now set up by the theater 
		public static int g_nYear = 2004; // JB 010804;
		public static int g_nDay = 135; // JB 010804
		public static bool g_bSimpleFMUpdates = false; // JB 010805 // These update cause bad AI behaviour, see afsimple.cpp
		public static bool g_b3dDynamicPilotHead = false; // JB 010804
		// JB 010802
		public static bool g_b3dCockpit = true;
		public static bool g_b3dMFDLeft = true;
		public static bool g_b3dMFDRight = true;
		public static bool g_bRWR = true;
#if NOTHING
bool g_b3dHUD = true;
		bool g_b3dRWR = true;
		bool g_b3dICP = true;
		bool g_b3dDials = true;
		float g_f3dlMFDulx = 18.205f;
		float g_f3dlMFDuly = -7.089f;
		float g_f3dlMFDulz = 7.793f;
		
		float g_f3dlMFDurx = 18.205f;
		float g_f3dlMFDury = -2.770f;
		float g_f3dlMFDurz = 7.793f;
		
		float g_f3dlMFDllx = 17.501f;
		float g_f3dlMFDlly = -7.089f;
		float g_f3dlMFDllz = 11.816f;
		
		float g_f3drMFDulx = 18.181f;
		float g_f3drMFDuly = 2.834f;
		float g_f3drMFDulz = 7.883f;
		
		float g_f3drMFDurx = 18.181f;
		float g_f3drMFDury = 6.994f;
		float g_f3drMFDurz = 7.883f;
		
		float g_f3drMFDllx = 17.475f;
		float g_f3drMFDlly = 2.834f;
		float g_f3drMFDllz = 11.978f;
#endif
		// JB
		public static int g_nNoPlayerPlay = 2; // JB 010926
		public static bool g_bModuleList = false; // JB 010101

		public static int g_npercentage_available_aircraft = 75; //me123
		public static int g_nminimum_available_aircraft = 4; //me123
		public static int g_nMaxVertexSpace = -1; // JPO - graphics option
		public static int g_nMinTacanChannel = 70; // JPO - tacan variable for other theaters.
		public static int g_nFlightVisualBonus = 1; // JPO - flight visual detection bonus

		//MI 
		public static bool g_bCATIIIDefault = false;
		public static bool g_bRealisticAvionics = true; // M.N. now changed by UI, Avionics "Realistic" = true, "Enhanced" = false
		public static bool g_bIFlyMirage = false;	//MI support for a possible new mirage
		public static bool g_bNoMFDsIn1View = false;
		// DEMON 110106
		public static bool g_bIFF = true;
		// END - FALSE
		public static bool g_bINS = true;
		public static bool g_bNoRPMOnHud = true;
		public static bool g_bNoPadlockBoxes = false;	//MI 18/01/02 removes the box around padlocked objects
		public static bool g_bFallingHeadingTape = false;	//28/02/02 let's the heading tape fall off of the hud, for those who want it.
		public static bool g_bTFRFixes = true;
		public static bool g_bCalibrateTFR_PitchCtrl = false;
		public static bool g_bLantDebug = false;
		public static bool g_bNewPitchLadder = true;
		public static float g_fGroundImpactMod = 0.0F;		// Grndfcc groundZ modification from S.G. / RP5
		public static bool  g_bAGRadarFixes = true;
		public static float g_fGMTMinSpeed = 3.0F;		// min Vt to be displayed on GMT radar
		public static float g_fGMTMaxSpeed = 100.0F;   // max Vt to be displayed on GMT radar
		public static float g_fReconCameraHalfFOV = 8.4F;
		public static float g_fReconCameraOffset = -8.0F;
		public static float g_fBombTimeStep = 0.05F;	//original 0.25F;
		public static bool g_bBombNumLoopOnly = true;
		public static float g_fHighDragGravFactor = 0.65F;
		public static bool g_bTO_LDG_LightFix = true;
		//MI
		public static bool g_bNewFm = true;//me123 new flight model
		public static bool g_bAIRefuelInComplexAF = false;	// 2002-02-20 ADDED BY S.G. Test to see if the AI can refuel in complex AF

		//M.N.
		public static float g_fFormationBurnerDistance = 10.0F; // M.N. 2001-10-29 - allow burner distance to lead when not in formation
		//float g_fHitChanceAir = 3.5F; // Only added to test out the best value. 6 seems to high (CampLIB/unit.cpp)
		//float g_fHitChanceGround = 2.0F; // moved into Falcon4.aii in campaign\save folder
		public static bool g_bHiResUI = true;				// false = 800x600, true = 1024x768
		public static bool g_bAWACSFuel = false;				// for debug, shows fuel of flight in UI when AWACSSupport = true
		//bool g_bShowManeuverLabels = true;		// for debug, shows currently performed BVR/WVR maneuver in SIM
		public static bool g_bFullScreenNVG = true;			// a NVG makes tunnel vision, but a pilot can turn around his head...
		public static bool g_bLogUiErrors = false;			// debug UI
		public static bool g_bLoadoutSquadStoreResupply = true;	// code checked & working
		public static bool g_bDisplayTrees = true;				// if true, loads falcon4tree.fed/ocd instead of falcon4.fed/ocd. If tree version not available, loads falcon4.fed/ocd
		public static bool g_bRequestHelp = true;				// enable RequestHelp in DLOGIC.cpp
		public static bool g_bLightsKC135 = true;	// once we have the KC-135 with director lights...
		public static float g_fPadlockBreakDistance = 6.0F; // nm
		public static bool g_bOldSamActivity = false; // for switching 3D sams also by 2D code
		// better keyboard support
		public static float g_fAFRudderRight = 0.5F;
		public static float g_fAFRudderLeft = 0.5F;
		public static float g_fAFThrottleDown = 0.01F;
		public static float g_fAFThrottleUp = 0.01F;
		public static float g_fAFAileronLeft = 0.8F;
		public static float g_fAFAileronRight = 0.8F;
		public static float g_fAFElevatorDown = 0.5F;
		public static float g_fAFElevatorUp = 0.5F;
		public static float g_frollStickOffset = 0.9F;
		public static float g_fpitchStickOffset = 0.9F;
		public static float g_frudderOffset = 0.9F;
		public static bool g_bAddACSizeVisual = true;			// adds drawpointer radius value to eyeball GetSignature() 
		public static float g_fVisualNormalizeFactor = 40.0F;	// 40.0F = F-16 drawpointer radius
		//bool g_bShowFuelLabel = false;			// for debugging fuel consumption in 3D replaced by label debug stuff
		public static bool g_bHelosReloc = true;				// A.S. relocate helo squadrons faster
		public static bool g_bNewPadlock = true;
		public static int g_nlookAroundWaterTiles = 2;		// we've 2 tile bridges, so use "2" here
		public static float g_fPullupTime = 0.2f;				// pull up for 0.2 seconds before reevaluating
		public static float g_fAIRefuelRange = 10.0F;			// range to the tanker at which AI asks for fuel
		public static bool g_bNewRefuelHelp = true;			// 2002-02-28 more refuel help for the player
		public static bool g_bOtherGroundCheck = false;		// try the old algorithm together with the new pullup timer
		public static bool g_bAIGloc = false;					// turns on/off AI GLoc prediction
		public static float g_fAIDropStoreLauncherRange = 10.0F; // if launcher is outside 10 nm, don't drop stores
		public static int g_nAirbaseCheck = 30;				// each x seconds check distance to closest airbase at bingo states, RTB at fumes
		public static bool g_bUseTankerTrack = true;			// tanker flies track box 60 * 25 nm
		public static float g_fTankerRStick = 0.2f;			// RStick in wingmnvers.cpp in SimpleTrackTanker (to finetune turning rate)
		public static float g_fTankerPStick = 0.01f;			// PStick in wingmnvers.cpp in SimpleTrackTanker (to finetune turning rate)
		public static float g_fTankerTrackFactor = 0.5f;		// adds a distance in nm in front of the tanker track points if we need to start turn earlier
		public static float g_fTankerHeadsupDistance = 2.5f;	// this is the distance to trackpoint when "Heads up, tanker is entering turn" is called out
		public static float g_fTankerBackupDistance = 3.0f;	// this is the "backup turn distance" to keep the tanker from circling a trackpoint
		public static float g_fHeadingStabilizeFactor = 0.004f; // this is the heading difference to the trackpoint at which rStick is set to zero to stabalize the tanker in leveled flight
		public static float g_fAIRefuelSpeed = 1.0f;			// If we want to speed up AI refueling later
		public static float g_fClimbRatio = 0.3f;				// Used in Camptask\Mission.cpp for fixing too steep climbs
		public static float g_fNukeStrengthFactor = 0.1f;		// modifier for proximity damage (Bombmain.cpp)
		public static float g_fNukeDamageMod = 100000.0f;		// range damage modifier in Bombmain.cpp for nukes
		public static float g_fNukeDamageRadius = 30.0f;		// radius of proximity damage for objectives in nm
		public static int g_nNoWPRefuelNeeded = 2000;			// amount of needed fuel which doesn't trigger tanker WP creation
		public static bool g_bAddIngressWP = true;			// add ingress waypoint if needed
		public static bool g_bTankerWaypoints = true;			// add tanker waypoints if needed
		public static bool g_bPutAIToBoom = true;				// hack: put AI sticking to the boom when close to it
		public static float g_fWaypointBurnerDelta = 700.0f;	// burnerdelta for WaypointMode and WingyMode
		public static int g_nSkipWaypointTime = 30000;		// time in milliseconds added to waypoint departure time at which flight switches to next waypoint 
		public static bool g_bLookCloserFix = true;			// fixes look closer view through the cockpit
		public static float g_fEXPfactor = 0.5f;				// 50% cursorspeed in EXP
		public static float g_fDBS1factor = 0.35f;			// 35% cursorspeed in DBS1
		public static float g_fDBS2factor = 0.20f;			// 20% cursorspeed in DBS2
		public static float g_fePropFactor = 40.0f;			// Mnvers.cpp - to control restricted speed (curMaxStoreSpeed) for AI
		public static float g_fSunPadlockTimeout = 1.5f;		// After how many seconds look on a padlocked object into the sun break lock
		public static int g_nGroundAttackTime = 6;			// how many minutes after SetupAGMode the AI will continue to do a ground attack
		public static bool g_bAGTargetWPFix = false;			// stop skipping of target WP because of departure time for AGMissions if several conditions are met
		public static bool g_bAlwaysAnisotropic = false;		// if true, the "Anisotropic" button in gfx setup is always on (workaround for GF3)
		public static float g_fTgtDZFactor = 0.0F;			// factor to reduce targetDZ when track has been lost - for fixing ballistic missiles
		public static bool g_bNoAAAEventRecords = false;		// don't record AAA shots at the player to event list
		public static int g_nATCTaxiOrderFix = 0;				// 1 = fixes player (09:36 takeoff) behind AI planes (09:37 takeoff)
		public static bool g_bEmergencyJettisonFix = true;	// just check not to drop AA weapons and ECM for all

		public static bool g_bFalcEntMaxDeltaTime = true;		// true = use maximum value restriction, false = set 0 and return
		public static int g_nBWMaxDeltaTime = 1;				// true = use maximum value restriction, false = set 0 and return
		public static int g_nBWCheckDeltaTime = 5000;			// maximum value of delta_time in capi.c
		public static int g_nFalcEntCheckDeltaTime = 5000;	// maximum value of delta_time in falcent.cpp
		public static int g_nVUMaxDeltaTime = 5000;			// maximum value of delta_time in vuevent.cpp
		public static bool g_bCampSavedMenuHack = true;
#if _DEBUG
		public static bool g_bActivateDebugStuff = true;
#else
		public static bool g_bActivateDebugStuff = false;		// to activate .label and .fuel chat line switch
#endif
		public static float g_fMoverVrValue = 450.0f;		// bogus Vr value in Radar.cpp - seems a bit too high - must test how change effects the AI
		public static bool g_bEmptyFilenameFix = true;	// fixes savings of "no name" files
		public static float g_fRAPDistance = 3.0F;		// used in MissileEngage() function to decide at which distance we start to roll and pull
		public static bool g_bLabelRadialFix = true;		// Fix for label display at screen edges
		public static bool g_bLabelShowDistance = false;	// If wanted, also show the distance to the target
		public static bool g_bCheckForMode = true;		// saw lead at takeoff asking wingman to do bvrengagement
		public static bool g_bRQDFix = true;				// Fix RQD C/S readout in ICP CRUS page
		public static int g_nSessionTimeout = 30;			// 30 seconds to timeout a disconnected session (might be a bit too high...)
		public static int g_nSessionUpdateRate = 15;		// 15 seconds session update
		public static int g_nMaxInterceptDistance = 60;	// only divert flights within 60 nm distance to the target
		public static bool g_bNewSensorPrecision = true;
		public static bool g_bSAM2D3DHandover = false;		// 2D-3D target handover to SAMs doesn't really work this way - turn off!
		public static int g_nChooseBullseyeFix = 0;			// theater fix for finding best bullseye position
		/* 0x01 = use bullseye central position from campaign trigger files
		   0x02 = change bullseye at each new day (should be tested before activated - what happens in flight, Multiplayer ?)
		*/
		public static int g_nSoundSwitchFix = 0x03;
		/* 0x01 = activate "DoSoundSetup" in TheaterDef.cpp
		   0x02 = add a fix in VM::ResetVoices - I think that garbage in this variable after sound switching
		          can cause chatter messages not to be processed in PlayRadioMessage...*/

		public static int g_nDFRegenerateFix = 0x03;			// fix for DF regenerations
		/*		0x01 = Fix in RegenerateMessage.cpp
				0x02 = Fix in CampUpd\Dogfight.cpp (not sure if this is really needed - but we let it in */
		public static bool g_bAllowOverload = true;   // Allow takeoff even when overloaded - the player may decide...
		public static bool g_bACPlayerCTDFix = true;			// When a player CTD's, put aircraft back to host's AI control
		public static bool g_bSetWaypointNumFix = false;		// Older fix from S.G. in Navfcc.cpp - must still be tested as AI uses this function, too
		public static float g_fLethalRadiusModifier = 1.5f;	// used in 0x20 condition
		public static int g_nMissileFix = 0x7f;				// several missile fixes:
		/*
		
				0x01	"Bomb missile" flag support -> do a ground/feature impact missile end instead 
					    of lethalradius detonation
				0x02	also check if range*range > lethalRadiusSqrd at "closestApprch" flag
				0x04	Use ArmingDelay MissileEndMessage instead of MinTime if warhead is 
					    not armed (fixes missiles being able to apply proximity damage while 
						warhead is unarmed)
				0x08	Do Proximity damage to the missile's target if we didn't hit it directly
					    (for example if missile lost seeker track and hits the dirt)
				0x10	Don't do the change to "NotDone" in Missmain.cpp when we have "closestApprch" flag already set
				        hope this finally fixes floating missiles on the ground...
				0x20	bring missile to an end if missile sensor lost lock on target and we are outside 2 times
						of lethalrange to last known position
				0x40	Change targetDZ when target lost by g_fTgtDZFactor
				0x80	Use correct missile/bomb drop sound (+ flag 0x01)
				0x100   Fix for JDAM - have always cloud LOS if weapon flag 0x400 is set
		*/

		public static bool g_bDarkHudFix = true;				// fix for the host player getting a dark HUD in TAKEOFF/TAXI mode

		public static float g_fBombMissileAltitude = 13000.0f;// altitude at which "bomb-like" missiles are being released
		public static int g_nFogRenderState = 0x01;			// 0x01 turn on the D3D call m_pD3DD->SetRenderState in context.cpp
		// 0x02 turns on StateStack::SetFog call to context->SetState
		//		which seems, according to comment, to be only some test code...
		public static bool g_bTankerFMFix = true;				// fix for tankers simple af flightmodel 
		//M.N.
		public static bool g_bAppendToBriefingFile = false;
		public static int g_nPrintToFile = 0x00; // MNLOOK set to 00 for release
		/*  0x00		= print out
			0x01		= only print to file
		    0x02		= print out and to file*/

		public static int g_nGfxFix = 0x00;					// turn all fixes off by default
		/* 
			0x01		activates 2D clipping fix
			0x02		activates ScatterPlot clipping fix
			0x04		activates 3D OTWSky clipping fix (sun, moon)
			0x08		does new clip code only for 2D drawables (removed, causes AG radar not to work anymore)
													*/
		public static bool g_bExitCampSelectFix = true;
		public static bool g_bAGNoBVRWVR = false;				// stops AG missions from doing any BVR/WVR checks
		// Refuel debugging:
		public static ulong gFuelState = 0;   // to set SimDriver.playerEntity's fuel by ".fuel" chat command

		// DEBUG LABELING:

		public static int g_nShowDebugLabels = 0;		// give each label type a bit
		public static int g_nMaxDebugLabel = 0x40000000; // 2002-04-01 MODIFIED BY S.G. Bumped up to highest without being negative
		
		/*
			DEBUG LABELS:
		
			0x01		Aircraft current mode
			0x02		Missile mode (boost, sustaion, terminal) speed and altitude, Active state
			0x04		SAM radar modes
			0x08		BVR modes
			0x10		WVR modes
			0x20		Energy state modes
			0x40		add current radar mode (RWS, TWS, STT) to 0x08 and 0x10
			0x80		Aircraft Pstick, Rstick, Throttle, Pedal
			0x100		Reset all aircraft labels
			0x200		Radar Digi Emitting/Not Emitting
			0x400		SAM sensor track
			0x800		Refueling: Tanker speed/altitude, refueling aircraft speed, tankerdistance, fuel level
			0x1000		Aircraft Identified/NotIdentified
			0x2000		Fuel level of aircraft
			0x4000		IL78 totalrange value - for fixing IL78Factor in aircraft data files
			0x8000		add flight model (SIMPLE/COMPLEX) to labels 0x01, 0x08, 0x10, 0x20, 0x80
			0x10000		Mnvers.cpp - debugging of afterburner stuff
			0x20000		Refuel.cpp - Relative position to boom
			0x40000		Actions.cpp - show label if aborting in AirbaseCheck
			0x80000		landme.cpp - current taxi point and type
			0x100000	Show aircrafts speed, heading, altitude
			0x200000	Show trackpoint location of aircraft (trackX/trackY/trackZ)
			0x400000	Show player's wing current maneuver (wingactions.cpp)
			0x800000	Show aircrafts "self" pointer address
			0x1000000	Free to use
			0x2000000	Free to use
			0x4000000	Free to use
			0x8000000	Free to use
			0x10000000	Free to use
			0x20000000	Free to use
		    0x40000000	Max debug (Resets all aircraft labels)
		*/

// a.s. begin
		public static bool g_bEnableMfdColors = true;	// enables transparent and colored Mfds
		public static float g_fMfdTransparency = 50;	// set transparence of Mfds as a percentage value, e.g. 100 means no transparency (255), 80 means 20% transparency
		public static float g_fMfdRed = 0;		// set brightness of red as a percentage value for Mfds, e.g. 100 means brightness of 255
		public static float g_fMfdGreen = 30;		// set brightness of green as a percentage value for Mfds, e.g. 100 means brightness of 255
		public static float g_fMfdBlue = 0;		// set brightness of blue as a percentage value for Mfds, e.g. 100 means brightness of 255
		public static bool g_bEnableMfdSize = true; // enables resizing of Mfds
		public static float g_fMfd_p_Size = 90;	// set size of Mfds as a percentage value of normal size (154)
// a.s. end
		public static bool g_bMavFixes = true;  // a.s. New code for slewing MAVs.
		public static bool g_bMavFix2 = false;  // MN When designating inside the 40 nm distance in BORE, and slewing outside, the HUD designation box got stuck
		public static bool g_bLgbFixes = true;  // a.s. New code for slewing LGBs.
		public static bool g_brebuildbobbleFix = true;
		public static bool g_bMPFix = true;
		public static bool g_bMPFix2 = true;
		public static bool g_bMPFix3 = true;
		public static bool g_bMPFix4 = true;
		public static float MinBwForOtherData = 1000.0f;
		public static float g_fclientbwforupdatesmodifyerMAX = 0.8f;
		public static float g_fclientbwforupdatesmodifyerMIN = 0.7f;
		public static float g_fReliablemsgwaitMAX = 60000;
		private static readonly IDictionary<string, Action<bool> > BoolOpts = new Dictionary<string, Action<bool> > (){
		//	{ "EnableCATIIIExtension", v => g_bEnableCATIIIExtension = v },	MI
			{ "ForceDXMultiThreadedCoopLevel", v => g_bForceDXMultiThreadedCoopLevel = v },
			{ "EnableABRelocation", v => g_bEnableABRelocation = v },
			{ "EnableWeatherExtensions", v => g_bEnableWeatherExtensions = v },
			{ "EnableWindsAloft", v => g_bEnableWindsAloft = v },	
			{ "EnableNonPersistentTextures", v => g_bEnableNonPersistentTextures = v },
			{ "EnableStaticTerrainTextures", v => g_bEnableStaticTerrainTextures = v },
		//	{ "EnableAircraftLimits", v => g_bEnableAircraftLimits = v },	MI
		//	{ "EnableArmingDelay", v => g_bArmingDelay = v },				MI
		//	{ "EnableHardCoreReal", v => g_bHardCoreReal = v },				MI
			{ "CheckBltStatusBeforeFlip", v => g_bCheckBltStatusBeforeFlip = v },
			{ "EnableUplink", v => g_bEnableUplink = v },
			{ "EnableColorMfd", v => g_bEnableColorMfd = v  },
			{ "NewDamageEffects", v => g_bNewDamageEffects = v  },
			{ "DisableFunkyChicken", v => g_bDisableFunkyChicken = v  },
			{ "ForceSoftwareGUI", v => g_bForceSoftwareGUI = v  },
			{ "SmartScaling", v => g_bSmartScaling = v },
			{ "FloatingBullseye", v => g_bFloatingBullseye = v },
			{ "DisableCrashEjectCourtMartials", v => g_bDisableCrashEjectCourtMartials = v },
			{ "UseMipMaps", v => g_bUseMipMaps = v },
			{ "ShowMipUsage", v => g_bShowMipUsage = v },
			{ "SmartCombatAP", v => g_bSmartCombatAP = v },
			{ "NoRPMOnHUD", v => g_bNoRPMOnHud = v },
			{ "CATIIIDefault", v => g_bCATIIIDefault = v }, 	
		//	{ "RealisticAvionics", v => g_bRealisticAvionics = v },	
			{ "EPAFRadarCues", v => g_bEPAFRadarCues = v },
			{ "RadarJamChevrons", v => g_bRadarJamChevrons = v },
			{ "AWACSSupport", v => g_bAWACSSupport = v },
			{ "Voodoo12Compatible", v => g_bVoodoo12Compatible = v },
			{ "AWACSRequired", v => g_bAWACSRequired = v },
			{ "Use3dSound", v => g_bUse3dSound = v },
			{ "OldSoundAlg", v => g_bOldSoundAlg = v },
			{ "MFDHighContrast", v => g_bMFDHighContrast = v },
			{ "IFlyMirage", v => g_bIFlyMirage = v },
			{ "PowerGrid", v => g_bPowerGrid = v },
			{ "UseMappedFiles", v => g_bUseMappedFiles = v },
		//	{ "UserRadioVoice", v => g_bUserRadioVoice = v },
			{ "NewFm", v => g_bNewFm = v },
			{ "RealisticAttrition", v => g_bRealisticAttrition = v },
			{ "IFFRWR", v => g_bIFFRWR = v },
			{ "3dCockpit", v => g_b3dCockpit = v },
			{ "RWR", v => g_bRWR = v },
		#if NOTHING // JPO
			{ "3dHUD", v => g_b3dHUD = v },
			{ "3dRWR", v => g_b3dRWR = v },
			{ "3dICP", v => g_b3dICP = v },
			{ "3dDials", v => g_b3dDials = v },
		#endif
			{ "3dMFDLeft", v => g_b3dMFDLeft = v },
			{ "3dMFDRight", v => g_b3dMFDRight = v },
			{ "3dDynamicPilotHead", v => g_b3dDynamicPilotHead = v },
			{ "SimpleFMUpdates", v => g_bSimpleFMUpdates = v },
			{ "NoMFDsIn1View", v => g_bNoMFDsIn1View = v },
			{ "Server", v => g_bServer = v },
			{ "ServerHostAll", v => g_bServerHostAll = v },	
			{ "LogEvents", v => g_bLogEvents = v },
			{ "VoiceCom", v => g_bVoiceCom = v },
		#if NOTHING
			{ "woeir", v => g_bwoeir = v },	
			{ "MLU", v => g_bMLU = v },
			{ "IFF", v => g_bIFF = v },	//MI disabled until further notice
		#endif
			{ "INS", v => g_bINS = v },
			{ "RP5DataCompatiblity", v => g_bRP5Comp = v },
			{ "ModuleList", v => g_bModuleList = v },
			{ "HiResUI", v => g_bHiResUI = v },	
			{ "AWACSFuel", v => g_bAWACSFuel = v },
		//	{ "ShowManeuverLabels", v => g_bShowManeuverLabels},
			{ "FullScreenNVG", v => g_bFullScreenNVG = v },
			{ "LogUiErrors", v => g_bLogUiErrors = v },
			{ "LoadoutSquadStoreResupply", v => g_bLoadoutSquadStoreResupply = v },
			{ "DisplayTrees", v => g_bDisplayTrees = v },
			{ "RequestHelp", v => g_bRequestHelp = v },
			{ "LightsKC135", v => g_bLightsKC135 = v },
			{ "AddACSizeVisual", v => g_bAddACSizeVisual = v },
		//	{ "ShowFuelLabel", v => g_bShowFuelLabel = v },
			{ "NewRackData", v => g_bNewRackData = v },
			{ "HelosReloc", v => g_bHelosReloc = v },
			{ "ShowFlaps", v => g_bShowFlaps = v },
			{ "SlowButSafe", v => g_bSlowButSafe = v },
			{ "NewPadlock", v => g_bNewPadlock = v },
			{ "NoPadlockBoxes", v => g_bNoPadlockBoxes = v },
			{ "PitchLimiterForAI", v => g_bPitchLimiterForAI = v },
			{ "MissionACIcons", v => g_bMissionACIcons = v },
			{ "EnableMfdColors", v => g_bEnableMfdColors = v },	// a.s.
			{ "EnableMfdSize", v => g_bEnableMfdSize = v },		// a.s.
			{ "AIRefuelInComplexAF", v => g_bAIRefuelInComplexAF = v }, // 2002-02-20 S.G.
			{ "NewAcmiHud", v => g_bNewAcmiHud = v },
			{ "AWACSBackground", v => g_bAWACSBackground = v },
			{ "MavFixes", v => g_bMavFixes = v },  // a.s.
			{ "LgbFixes", v => g_bLgbFixes = v },  // a.s.
			{ "FallingHeadingTape", v => g_bFallingHeadingTape = v },
			{ "NewRefuelHelp", v => g_bNewRefuelHelp = v }, // MN
			{ "OtherGroundCheck", v => g_bOtherGroundCheck = v }, // MN
			{ "Limit2DRadarFight", v => g_bLimit2DRadarFight = v }, // 2002-03-07 S.G.
			{ "AdvancedGroundChooseWeapon", v => g_bAdvancedGroundChooseWeapon = v }, // 2002-03-09 S.G.
			{ "TFRFixes", v => g_bTFRFixes = v }, //MI TFR Fixes
			{ "AIGloc", v => g_bAIGloc = v }, // MN
			{ "CalibrateTFR_PitchCtrl", v => g_bCalibrateTFR_PitchCtrl = v },
			{ "LantDebug", v => g_bLantDebug = v },
			{ "UseNewCanEnage", v => g_bUseNewCanEnage = v }, // 2002-03-11 S.G.
			{ "MPStartRestricted", v => g_bMPStartRestricted = v },
			{ "UseSkillForFlaks", v => g_bUseSkillForFlaks = v }, // 2002-03-12 S.G.
			{ "ToggleAAAGunFlag", v => g_bToggleAAAGunFlag = v }, // 2002-03-12 S.G.
			{ "UseTankerTrack", v => g_bUseTankerTrack = v },// 2002-03-13 MN
			{ "UseComplexBVRForPlayerAI", v => g_bUseComplexBVRForPlayerAI = v }, // 2002-03-13 S.G.
			{ "FuelUseVtDot", v => g_bFuelUseVtDot = v },	// 2002-03-14 S.G.
			{ "FuelLimitBecauseVtDot", v => g_bFuelLimitBecauseVtDot = v },	// 2002-03-14 S.G.
			{ "UseAggresiveIncompleteA2G", v => g_bUseAggresiveIncompleteA2G = v }, // 2002-03-22 S.G.
			{ "NewPitchLadder", v => g_bNewPitchLadder = v },
			{ "AddIngressWP", v => g_bAddIngressWP = v }, // 2002-03-25 MN
			{ "TankerWaypoints", v => g_bTankerWaypoints = v }, // 2002-03-25 MN
			{ "PutAIToBoom", v => g_bPutAIToBoom = v }, // 2002-03-28 MN
			{ "OldStackDump", v => g_bOldStackDump = v }, // 2002-04-01 S.G.
			{ "TankerFMFix", v => g_bTankerFMFix = v }, // 2002-04-02 MN
			{ "AGRadarFixes", v => g_bAGRadarFixes = v },	//MI 2002-03-28
			{ "LookCloserFix", v => g_bLookCloserFix = v }, // 2002-04-05 MN by dpc
			{ "LowBwVoice", v => g_bLowBwVoice = v },
			{ "AGTargetWPFix", v => g_bAGTargetWPFix = v }, // 2002-04-06 MN
			{ "BombNumLoopOnly", v => g_bBombNumLoopOnly = v },	//MI 2002-07-04 fix for missing bombs in CCIP
			{ "AlwaysAnisotropic", v => g_bAlwaysAnisotropic = v }, // 2002-04-07 MN
			{ "NoAAAEventRecords", v => g_bNoAAAEventRecords = v }, // 2002-04-07 MN
			{ "NeedSimThreadToRemoveObject", v => g_bNeedSimThreadToRemoveObject = v },
			{ "FalcEntMaxDeltaTime", v => g_bFalcEntMaxDeltaTime = v }, // 2002-04-12 MN
			{ "ActivateDebugStuff", v => g_bActivateDebugStuff = v }, // 2002-04-12 MN
			{ "NewSensorPrecision", v => g_bNewSensorPrecision = v },
			{ "AGNoBVRWVR", v => g_bAGNoBVRWVR = v },
			{ "TO_LDG_LightFix", v => g_bTO_LDG_LightFix = v },	//MI 2002-04-13
			{ "AppendToBriefingFile", v => g_bAppendToBriefingFile = v },
			{ "DarkHudFix", v => g_bDarkHudFix = v },
			{ "RQDFix", v => g_bRQDFix = v }, // MN 2002-04-13
			{ "ACPlayerCTDFix", v => g_bACPlayerCTDFix = v },
			{ "CheckForMode", v => g_bCheckForMode = v }, // MN 2002-04-14
			{ "AllowOverload", v => g_bAllowOverload = v },
			{ "UseDefinedGunDomain", v => g_bUseDefinedGunDomain = v }, // 2002-04-17 S.G.
			{ "LabelRadialFix", v => g_bLabelRadialFix = v },
			{ "LabelShowDistance", v => g_bLabelShowDistance = v },
			{ "SetWaypointNumFix", v => g_bSetWaypointNumFix = v }, // 2002-04-18 MN a fix from S.G. which he didn't put in because it was not really tested yet..
			{ "EmptyFilenameFix", v => g_bEmptyFilenameFix = v },
			{ "RebuildbobbleFix", v => g_brebuildbobbleFix = v },
			{ "MPFix", v => g_bMPFix = v },
			{ "MPFix2", v => g_bMPFix2 = v },
			{ "MPFix3", v => g_bMPFix3 = v },
			{ "MPFix4", v => g_bMPFix4 = v },
			{ "ExitCampSelectFix", v => g_bExitCampSelectFix = v },
			{ "CampSavedMenuHack", v => g_bCampSavedMenuHack = v },
			{ "EmergencyJettisonFix", v => g_bEmergencyJettisonFix = v },
			{ "OldSamActivity", v => g_bOldSamActivity = v }, // no LOS check and stuff - just the old code
			{ "SAM2D3DHandover", v => g_bSAM2D3DHandover = v },
			{ "MavFix2", v => g_bMavFix2 = v }
		};
		private static readonly IDictionary<string, Action<int> > IntOpts = new Dictionary<string, Action<int> > (){
			{ "ThrottleMode", v => g_nThrottleMode = v },
			{ "PadlockBoxSize", v => g_nPadlockBoxSize = v },
			{ "PadlockMode", v => g_nPadlockMode = v },
			{ "NumDefaultHatSwitches", v => NumHats = v },
			{ "NearLabelLimit", v => g_nNearLabelLimit = v },
			{ "percentage_available_aircraft", v => g_npercentage_available_aircraft = v },
			{ "minimum_available_aircraft", v => g_nminimum_available_aircraft = v },
			{ "MasterServerPort", v => g_nMasterServerPort = v },
			{ "MaxVertexSpace", v => g_nMaxVertexSpace = v },
		//	{ "MinTacanChannel", v => g_nMinTacanChannel = v }, -> Theater definition file
			{ "FlightVisualBonus", v => g_nFlightVisualBonus = v },
			{ "RelocationWait", v => g_nRelocationWait = v },
			{ "LoadoutTimeLimit", v => g_nLoadoutTimeLimit = v },
			{ "Year", v => g_nYear = v },
			{ "Day", v => g_nDay = v },
			{ "AirbaseReloc", v => g_nAirbaseReloc = v },
			{ "NoPlayerPlay", v => g_nNoPlayerPlay = v },
			{ "DeagTimer", v => g_nDeagTimer = v },
			{ "ReagTimer", v => g_nReagTimer = v },
			{ "RNESpeed", v => g_nRNESpeed = v },
			{ "TargetSpotTimeout", v => g_nTargetSpotTimeout = v },
			{ "MaxUIRefresh", v => g_nMaxUIRefresh = v }, // 2002-02-23 S.G.
			{ "UnidentifiedInUI", v => g_nUnidentifiedInUI = v }, // 2002-02-24 S.G.
			{ "lookAroundWaterTiles", v => g_nlookAroundWaterTiles = v },
			{ "FFEffectAutoCenter", v => g_nFFEffectAutoCenter = v },
			{ "MPStartTime", v => g_nMPStartTime = v },
			{ "LowestSkillForGCI", v => g_nLowestSkillForGCI = v }, // 2002-03-12 S.G.
			{ "AIVisualRetentionTime", v => g_nAIVisualRetentionTime = v }, // 2002-03-12 S.G.
			{ "AIVisualRetentionSkill", v => g_nAIVisualRetentionSkill = v }, // 2002-03-12 S.G.
			{ "MaxSimTimeAcceleration", v => g_nMaxSimTimeAcceleration = v },
		//	{ "ShowDebugLabels", v => g_nShowDebugLabels = v },// only by .label chatline input
			{ "LowDetailFactor", v => g_nLowDetailFactor = v },
			{ "NoWPRefuelNeeded", v => g_nNoWPRefuelNeeded = v }, // 2002-03-25 MN
			{ "AirbaseCheck", v => g_nAirbaseCheck = v }, // 2002-03-11 MN
			{ "FogRenderState", v => g_nFogRenderState = v },
			{ "MissileFix", v => g_nMissileFix = v }, // 2002-03-28 MN
			{ "SkipWaypointTime", v => g_nSkipWaypointTime = v }, // 2002-04-05 MN
			{ "GroundAttackTime", v => g_nGroundAttackTime = v }, // 2002-04-05 MN
			{ "GfxFix", v => g_nGfxFix = v }, // 2002-04-06 MN
			{ "ATCTaxiOrderFix", v => g_nATCTaxiOrderFix = v },// 2002-04-08 MN
			{ "DFRegenerateFix", v => g_nDFRegenerateFix = v },// 2002-04-09 MN
			{ "BWMaxDeltaTime", v => g_nBWMaxDeltaTime = v }, // 2002-04-12 MN
			{ "BWCheckDeltaTime", v => g_nBWCheckDeltaTime = v }, // 2002-04-12 MN
			{ "FalcEntCheckDeltaTime", v => g_nFalcEntCheckDeltaTime = v }, // 2002-04-12 MN
			{ "VUMaxDeltaTime", v => g_nVUMaxDeltaTime = v }, // 2002-04-12 MN
			{ "ACMIOptionsPopupHiResX", v => g_nACMIOptionsPopupHiResX = v },
			{ "ACMIOptionsPopupHiResY", v => g_nACMIOptionsPopupHiResY = v },
			{ "ACMIOptionsPopupLowResX", v => g_nACMIOptionsPopupLowResX = v },
			{ "ACMIOptionsPopupLowResY", v => g_nACMIOptionsPopupLowResY = v },
			{ "ChooseBullseyeFix", v => g_nChooseBullseyeFix = v }, // 2002-04-12 MN
			{ "SoundSwitchFix", v => g_nSoundSwitchFix = v },
			{ "PrintToFile", v => g_nPrintToFile = v },
			{ "SessionTimeout", v => g_nSessionTimeout = v },
			{ "SessionUpdateRate", v => g_nSessionUpdateRate = v },
			{ "MaxInterceptDistance", v => g_nMaxInterceptDistance = v } // 2002-04-14 MN
		};
		private static readonly IDictionary<string, Action<string> > StringOpts = new Dictionary<string, Action<string> > (){		
			{ "MasterServerName", v => g_strMasterServerName = v },
			{ "ServerName", v => g_strServerName = v },
			{ "ServerLocation", v => g_strServerLocation = v },
			{ "ServerAdmin", v => g_strServerAdmin = v },
			{ "ServerAdminEmail", v => g_strServerAdminEmail = v },
			{ "VoiceHostIP", v => g_strVoiceHostIP = v },
			{ "WorldName", v => g_strWorldName = v }
		};
		private static readonly IDictionary<string, Action<float> > FloatOpts = new Dictionary<string, Action<float> > (){		
			{ "MipLodBias", v => g_fMipLodBias = v },
			{ "CloudMinHeight", v => g_fCloudMinHeight = v }, // JPO
			{ "RadarScale", v => g_fRadarScale = v }, // JPO
			{ "CursorSpeed", v => g_fCursorSpeed = v }, // JPO
			{ "MinCloudWeather", v => g_fMinCloudWeather = v }, //JPO
			{ "CloudThicknessFactor", v => g_fCloudThicknessFactor = v }, //JPO
			{ "DragDilutionFactor", v => g_fDragDilutionFactor = v },
		//	{ "Latitude", v => g_fLatitude = v },	// is now set by the theater.map readout
			{ "dwPorthost", v => g_fdwPorthost = v },
			{ "dwPortclient", v => g_fdwPortclient = v },
		#if NOTHING
			{"3dlMFDulx", v => g_f3dlMFDulx = v },
			{"3dlMFDuly", v => g_f3dlMFDuly = v },
			{"3dlMFDulz", v => g_f3dlMFDulz = v },
			{"3dlMFDurx", v => g_f3dlMFDurx = v },
			{"3dlMFDury", v => g_f3dlMFDury = v },
			{"3dlMFDurz", v => g_f3dlMFDurz = v },
			{"3dlMFDllx", v => g_f3dlMFDllx = v },
			{"3dlMFDlly", v => g_f3dlMFDlly = v },
			{"3dlMFDllz", v => g_f3dlMFDllz = v },
			{"3drMFDulx", v => g_f3drMFDulx = v },
			{"3drMFDuly", v => g_f3drMFDuly = v },
			{"3drMFDulz", v => g_f3drMFDulz = v },
			{"3drMFDurx", v => g_f3drMFDurx = v },
			{"3drMFDury", v => g_f3drMFDury = v },
			{"3drMFDurz", v => g_f3drMFDurz = v },
			{"3drMFDllx", v => g_f3drMFDllx = v },
			{"3drMFDlly", v => g_f3drMFDlly = v },
			{"3drMFDllz", v => g_f3drMFDllz = v },
		#endif
			{"FormationBurnerDistance" , v => g_fFormationBurnerDistance = v },
			{"PadlockBreakDistance", v => g_fPadlockBreakDistance = v },
			{"AFRudderRight", v => g_fAFRudderRight = v },
			{"AFRudderLeft", v => g_fAFRudderLeft = v },
			{"AFThrottleDown", v => g_fAFThrottleDown = v },
			{"AFThrottleUp", v => g_fAFThrottleUp = v },
			{"AFAileronLeft", v => g_fAFAileronLeft = v },
			{"AFAileronRight", v => g_fAFAileronRight = v },
			{"AFElevatorDown", v => g_fAFElevatorDown = v },
			{"AFElevatorUp", v => g_fAFElevatorUp = v },
			{"rollStickOffset", v => g_frollStickOffset = v },
			{"pitchStickOffset", v => g_fpitchStickOffset = v },
			{"rudderOffset", v => g_frudderOffset = v },
			{"VisualNormalizeFactor", v => g_fVisualNormalizeFactor = v },
			{"RecoveryAOA", v => g_fRecoveryAOA = v },
			{"clientbwforupdatesmodifyer", v => clientbwforupdatesmodifyer = v },
			{"hostbwforupdatesmodifyer", v => hostbwforupdatesmodifyer = v },
			{"MfdTransparency", v => g_fMfdTransparency = v }, // a.s. begin
			{"MfdRed", v => g_fMfdRed = v },
			{"MfdBlue", v => g_fMfdBlue = v },
			{"MfdGreen", v => g_fMfdGreen = v },
			{"Mfd_p_Size", v => g_fMfd_p_Size = v },	// a.s. end
			{"PullupTime", v => g_fPullupTime = v },	// 2002-02-24 M.N.
			{"AIRefuelRange", v => g_fAIRefuelRange = v }, // 2002-02-28 MN
			{"IdentFactor", v => g_fIdentFactor = v }, // 2002-03-07 S.G.
			{"AIDropStoreLauncherRange", v => g_fAIDropStoreLauncherRange = v }, // 2002-03-08 MN
			{"BiasFactorForFlaks", v => g_fBiasFactorForFlaks = v }, // 2002-03-12 S.G.
			{ "TracerAccuracyFactor", v => g_fTracerAccuracyFactor = v }, // 2002-03-12 S.G.
			{ "TankerRStick", v => g_fTankerRStick = v }, // 2003-03-13 MN
			{ "TankerPStick", v => g_fTankerPStick = v }, // 2003-03-13 MN
			{ "TankerTrackFactor", v => g_fTankerTrackFactor = v }, // 2003-03-13 MN
			{ "TankerHeadsupDistance", v => g_fTankerHeadsupDistance = v }, // 2003-04-07 MN
			{ "TankerBackupDistance", v => g_fTankerBackupDistance = v }, // 2003-04-07 MN
			{ "HeadingStabilizeFactor", v => g_fHeadingStabilizeFactor = v }, // 2003-04-07 MN
			{ "FuelBaseProp", v => g_fFuelBaseProp = v }, // 2002-03-14 S.G.
			{ "FuelMultProp", v => g_fFuelMultProp = v }, // 2002-03-14 S.G.
			{ "FuelTimeStep", v => g_fFuelTimeStep = v }, // 2002-03-14 S.G.
			{ "FuelVtClip", v => g_fFuelVtClip = v }, // 2002-03-14 S.G.
			{ "FuelVtDotMult", v => g_fFuelVtDotMult = v }, // 2002-03-14 S.G.
			{ "AIRefuelSpeed", v => g_fAIRefuelSpeed = v }, // 2002-03-15 MN
			{ "SearchSimTargetFromRangeSqr", v => g_fSearchSimTargetFromRangeSqr = v }, // 2002-03-15 S.G.
			{ "NukeStrengthFactor", v => g_fNukeStrengthFactor = v }, // 2002-03-22 MN
			{ "NukeDamageMod", v => g_fNukeDamageMod = v }, // 2002-03-25 MN
			{ "NukeDamageRadius", v => g_fNukeDamageRadius = v }, // 2002-03-25 MN
			{ "ClimbRatio", v => g_fClimbRatio = v }, // 2002-03-25 MN
			{ "HotNoseAngle", v => g_fHotNoseAngle = v }, // 2002-03-22 S.G.
			{ "MaxMARNoIdA", v => g_fMaxMARNoIdA = v }, // 2002-03-22 S.G.
			{ "MinMARNoId5kA", v => g_fMinMARNoId5kA = v }, // 2002-03-22 S.G.
			{ "MinMARNoId18kA", v => g_fMinMARNoId18kA = v }, // 2002-03-22 S.G.
			{ "MinMARNoId28kA", v => g_fMinMARNoId28kA = v }, // 2002-03-22 S.G.
			{ "MaxMARNoIdB", v => g_fMaxMARNoIdB = v }, // 2002-03-22 S.G.
			{ "MinMARNoId5kB", v => g_fMinMARNoId5kB = v }, // 2002-03-22 S.G.
			{ "MinMARNoId18kB", v => g_fMinMARNoId18kB = v }, // 2002-03-22 S.G.
			{ "MinMARNoId28kB", v => g_fMinMARNoId28kB = v }, // 2002-03-22 S.G.
			{ "MinMARNoIdC", v => g_fMinMARNoIdC = v }, // 2002-03-22 S.G.
			{ "WaypointBurnerDelta", v => g_fWaypointBurnerDelta = v }, // 2002-03-28 MN
			{ "GroundImpactMod", v => g_fGroundImpactMod = v },	//MI 2002-03-28
			{ "BombMissileAltitude", v => g_fBombMissileAltitude = v }, // 2002-03-28 MN
			{ "GMTMaxSpeed", v => g_fGMTMaxSpeed = v }, // 2002-04-03 MN
			{ "GMTMinSpeed", v => g_fGMTMinSpeed = v }, // 2002-04-03 MN
			{ "ReconCameraHalfFOV", v => g_fReconCameraHalfFOV = v },	//MI 2002-04-03 Recon camera stuff
			{ "ReconCameraOffset", v => g_fReconCameraOffset = v },	//MI 2002-04-03 Recon camera stuff
			{ "EXPfactor", v => g_fEXPfactor = v }, // 2002-04-05 MN cursor speed reduction in EXP
			{ "DBS1factor", v => g_fDBS1factor = v }, // 2002-04-05 MN cursor speed reduction in DBS1
			{ "DBS2factor", v => g_fDBS2factor = v }, // 2002-04-05 MN cursor speed reduction in DBS2
			{ "ePropFactor", v => g_fePropFactor = v }, // 2002-04-05 MN
			{ "SunPadlockTimeout", v => g_fSunPadlockTimeout = v }, // 2002-04-06 MN
			{ "CarrierStartTolerance", v => g_fCarrierStartTolerance = v },
			{ "BombTimeStep", v => g_fBombTimeStep = v },	//MI 2002-04-07 fix for missing bombs
			{ "HighDragGravFactor", v => g_fHighDragGravFactor = v }, //MI 2002-04-07 externalised var to allow tweaking afterwards
			{ "TgtDZFactor", v => g_fTgtDZFactor = v }, // MN 2002-04-07 fix for ballistic missiles
			{ "SSoffsetManeuverPoints1a", v => g_fSSoffsetManeuverPoints1a = v }, // 2002-04-07 S.G.
			{ "SSoffsetManeuverPoints1b", v => g_fSSoffsetManeuverPoints1b = v }, // 2002-04-07 S.G.
			{ "SSoffsetManeuverPoints2a", v => g_fSSoffsetManeuverPoints2a = v }, // 2002-04-07 S.G.
			{ "SSoffsetManeuverPoints2b", v => g_fSSoffsetManeuverPoints2b = v }, // 2002-04-07 S.G.
			{ "PinceManeuverPoints1a", v => g_fPinceManeuverPoints1a = v }, // 2002-04-07 S.G.
			{ "PinceManeuverPoints1b", v => g_fPinceManeuverPoints1b = v }, // 2002-04-07 S.G.
			{ "PinceManeuverPoints2a", v => g_fPinceManeuverPoints2a = v }, // 2002-04-07 S.G.
			{ "PinceManeuverPoints2b", v => g_fPinceManeuverPoints2b = v }, // 2002-04-07 S.G.
			{ "LethalRadiusModifier", v => g_fLethalRadiusModifier = v }, // 2002-04-14 MN
			{ "RAPDistance", v => g_fRAPDistance = v }, // 2002-04-18 MN RollAndPull triggering in MissileEngage
			{ "MoverVrValue", v => g_fMoverVrValue = v },
			{ "MinBwForOtherData", v => MinBwForOtherData = v },
			{ "clientbwforupdatesmodifyerMIN", v => g_fclientbwforupdatesmodifyerMIN = v },
			{ "clientbwforupdatesmodifyerMAX", v => g_fclientbwforupdatesmodifyerMAX = v },
			{ "ReliablemsgwaitMAX", v => g_fReliablemsgwaitMAX = v }
		};

		private static void ParseFalcon4Config (StreamReader file)
		{
			g_strWorldName = "SP3WORLD";
			string strLine;
			while ((strLine = file.ReadLine()) != null) {
				if (string.IsNullOrWhiteSpace (strLine))
					continue;
				if (strLine.StartsWith ("//"))
					continue;
				string[] parts = strLine.Split (new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length != 3 && parts [0] != "set") {
					Debug.Print ("Fail setting: " + strLine);
					continue;
				}
				string strID = parts [1];
				string strVal = parts [2];
				
				try {
					if (strID.StartsWith ("g_s")) { // string value	
						StringOpts [strID.Substring (3)] (strVal.Replace ("\"", ""));
					} else if (strID.StartsWith ("g_f")) { // float value
						FloatOpts [strID.Substring (3)] (float.Parse (strVal,CultureInfo.InvariantCulture));
					} else if (strID.StartsWith ("g_b")) { // bool value
						
						bool val = true;
						if (strVal == "1")
							val = true;
						else if (strVal == "0")
							val = false;
						else {
							val = bool.Parse (strVal.ToLowerInvariant ());
						}
						BoolOpts [strID.Substring (3)] (val);
					} else if (strID.StartsWith ("g_n")) { // int value
						IntOpts [strID.Substring (3)] (int.Parse (strVal));
					} else
						Debug.Print ("Fail setting: " + strLine);
				} catch (Exception ex) {
					Debug.Print ("Fail setting: " + strLine + ", error = " + ex);
				}
			}
			file.Close ();
		}

		public static void ReadFalcon4Config ()
		{
			// Investigate program directory
			string strDir = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
			string filename = strDir + Path.DirectorySeparatorChar + "F4os.cfg";
			if (File.Exists (filename)) {
				// DEMON 110106 Changed values for os.
				using (StreamReader file = new StreamReader(filename)) {
					ParseFalcon4Config (file);
				}
			}
			filename = strDir + Path.DirectorySeparatorChar + "F4osServer.cfg";
			if (File.Exists (filename)) {
				// JB 010104 Second config file overrides the first and can be CRC checked by anti-cheat programs.
				// DEMON 110106 Changed values for os.
				using (StreamReader file = new StreamReader(filename)) {
					ParseFalcon4Config (file);
				}
			}
			// JB 010104
			if (!g_bwoeir) {
				g_bMLU = false;
				g_bIFF = false;
			}
		}
		
		public static void testRead (StreamReader file)
		{
			string line;
			while ((line = file.ReadLine()) != null) {
				Console.WriteLine (line);
			}
			
			file.Close ();
		}
	}
}

