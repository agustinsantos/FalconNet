using System;
using FalconNet.VU;

namespace FalconNet.Sim
{




    public class SimulationDriver
    {
        public const int MAX_IA_CAMP_UNIT = 0x10000;

        public SimulationDriver();
        //TODO public ~SimulationDriver();

        public void Startup();		// One time setup (at application start)
        public void Cleanup();	// One time shutdown (at application exit)

        public void Exec();		// The master thread loop -- runs from Startup to Cleanup
        public void Cycle();		// One SIM cycle (could be multiple time steps)

        public void Enter();		// Enter the SIM from the UI
        public void Exit();		// Set up sim for exiting


        public bool InSim() { return SimulationLoopControl.InSim(); }
        public int RunningInstantAction() { return FalconLocalGame.GetGameType() == game_InstantAction; }
        public int RunningDogfight() { return FalconLocalGame.GetGameType() == game_Dogfight; }
        public int RunningTactical() { return FalconLocalGame.GetGameType() == game_TacticalEngagement; }
        public int RunningCampaign() { return FalconLocalGame.GetGameType() == game_Campaign; }
        public int RunningCampaignOrTactical()
        {
            return FalconLocalGame.GetGameType() == game_Campaign ||
                   FalconLocalGame.GetGameType() == game_TacticalEngagement;
        }

        public void NoPause();	// Pause time in the sim
        public void TogglePause();	// Pause time in the sim

        public void SetFrameDescription(int mSecPerFrame, int numMinorFrames);
        public void SetPlayerEntity(SimBaseClass newObject);
        public void UpdateIAStats(SimBaseClass oldEntity);
        public SimBaseClass FindNearestThreat(ref float bearing, ref float range, ref float altitude);
        public SimBaseClass FindNearestThreat(ref short x, ref short y, ref float alt);
        public SimBaseClass FindNearestThreat(AircraftClass aircraft, ref short x, ref short y, ref float alt);
        public SimBaseClass FindNearestEnemyPlane(AircraftClass aircraft, ref short x, ref short y, ref float alt);
        public CampBaseClass FindNearestCampThreat(AircraftClass aircraft, ref short x, ref short y, ref float alt);
        public CampBaseClass FindNearestCampEnemy(AircraftClass aircraft, ref short x, ref short y, ref float alt);

        public void UpdateRemoteData();
        public SimBaseClass FindFac(SimBaseClass center);
        public FlightClass FindTanker(SimBaseClass center);
        public SimBaseClass FindATC(VU_ID desiredATC);
        public int MotionOn() { return motionOn; }
        public void SetMotion(int newFlag) { motionOn = newFlag; }
        public int AVTROn() { return avtrOn; }
        public void SetAVTR(int newFlag) { avtrOn = newFlag; }
        public void AddToFeatureList(VuEntity theObject);
        public void AddToObjectList(VuEntity theObject);
        public void AddToCampUnitList(VuEntity theObject);
        public void AddToCampFeatList(VuEntity theObject);
        public void AddToCombUnitList(VuEntity theObject);
        public void AddToCombFeatList(VuEntity theObject);
        public void RemoveFromFeatureList(VuEntity theObject);
        public void RemoveFromObjectList(VuEntity theObject);
        public void RemoveFromCampUnitList(VuEntity theObject);
        public void RemoveFromCampFeatList(VuEntity theObject);
        public void InitACMIRecord();
        public void POVKludgeFunction(short povHatAngle);
        public void InitializeSimMemoryPools();
        public void ReleaseSimMemoryPools();
        public void ShrinkSimMemoryPools();
        public void WakeCampaignFlight(int ctype, CampBaseClass baseEntity, TailInsertList flightList);
        public void WakeObject(SimBaseClass theObject);
        public void SleepCampaignFlight(TailInsertList flightList);
        public void SleepObject(SimBaseClass theObject);
        public void NotifyExit() { doExit = true; }
        public void NotifyGraphicsExit() { doGraphicsExit = true; }


        public AircraftClass playerEntity;

        public FalconPrivateOrderedList objectList;		// List of locally deaggregated sim vehicles
        public FalconPrivateOrderedList featureList;		// List of locally deaggregated sim features
        public FalconPrivateOrderedList campUnitList;		// List of nearby aggregated campaign units
        public FalconPrivateOrderedList campObjList;		// List of nearby aggregated campaign objectives
        public FalconPrivateOrderedList combinedList;		// List of everything nearby
        public FalconPrivateOrderedList combinedFeatureList;	// List of everything nearby
        public FalconPrivateList ObjsWithNoCampaignParentList;
        public FalconPrivateList facList;
        public VuFilteredList atcList;
        public VuFilteredList tankerList;

        public int doFile;
        public int doEvent;
        public uint eventReadPointer;

        public ulong lastRealTime;

        // SCR:  These used to be local to the loop function, but moved
        // here when the loop got broken out into a cycle per function call.
        // Some or all of these may be unnecessary, but I'll keep them for now...

        private ulong last_elapsedTime;
        private int lastFlyState;
        private int curFlyState;
        private bool doExit;
        private bool doGraphicsExit;

        private VuThread vuThread;
        private int curIALevel;
        private string dataName;
        private int motionOn;
        private int avtrOn;
        private ulong nextATCTime;

        private void UpdateEntityLists();
        private void ReaggregateAllFlights();
        private SimBaseClass FindNearest(SimBaseClass center, VuLinkedList sourceList);
        private void UpdateATC();
        public static SimulationDriver SimDriver;
    }
}

