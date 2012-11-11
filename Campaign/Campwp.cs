﻿using System;
using System.IO;
using FalconNet.Common;
using FalconNet.FalcLib;


namespace FalconNet.Campaign
{
    public static class CampwpStatic
    {
        public static void ConvertSimToGrid(vector* pos, GridIndex* x, GridIndex* y);
        public static void ConvertGridToSim(GridIndex x, GridIndex y, vector* pos);
        public static float GridToSim(GridIndex x);
        public static GridIndex SimToGrid(float x);


        // Waypoint actions
        public const int WP_NOTHING = 0;
        public const int WP_TAKEOFF = 1;
        public const int WP_ASSEMBLE = 2;
        public const int WP_POSTASSEMBLE = 3;
        public const int WP_REFUEL = 4;
        public const int WP_REARM = 5;
        public const int WP_PICKUP = 6;			// Pick up a unit
        public const int WP_LAND = 7;
        public const int WP_TIMING = 8;				// Just cruise around wasting time
        public const int WP_CASCP = 9;				// CAS contact point

        public const int WP_ESCORT = 10;				// Engage engaging fighters
        public const int WP_CA = 11;			// Engage all enemy aircraft
        public const int WP_CAP = 12;				// Patrol area for enemy aircraft
        public const int WP_INTERCEPT = 13;				// Engage specific enemy aircraft
        public const int WP_GNDSTRIKE = 14;				// Engage enemy units at target
        public const int WP_NAVSTRIKE = 15;				// Engage enemy shits at target
        public const int WP_SAD = 16;				// Engage any enemy at target
        public const int WP_STRIKE = 17;				// Destroy enemy installation at target
        public const int WP_BOMB = 18;				// Strategic bomb enemy installation at target				
        public const int WP_SEAD = 19;				// Suppress enemy air defense at target
        public const int WP_ELINT = 20;				// Electronic intellicence (AWACS, JSTAR, ECM)
        public const int WP_RECON = 21;				// Photograph target location
        public const int WP_RESCUE = 22;				// Rescue a pilot at location
        public const int WP_ASW = 23;
        public const int WP_TANKER = 24;				// Respond to tanker requests
        public const int WP_AIRDROP = 25;
        public const int WP_JAM = 26;

        // M.N. fix for Airlift missions
        //public const int  WP_LAND2			27				// this is our 2nd landing waypoint for Airlift missions
        // supply will be given at WP_LAND MNLOOK -> Change in string.txt 377

        public const int WP_B5 = 27;
        public const int WP_B6 = 28;
        public const int WP_B7 = 29;
        public const int WP_FAC = 30;

        public const int WP_MOVEOPPOSED = 40;				// These are movement wps
        public const int WP_MOVEUNOPPOSED = 41;
        public const int WP_AIRBORNE = 42;
        public const int WP_AMPHIBIOUS = 43;
        public const int WP_DEFEND = 44;				// These are action wps
        public const int WP_REPAIR = 45;
        public const int WP_RESERVE = 46;
        public const int WP_AIRDEFENSE = 47;
        public const int WP_FIRESUPPORT = 48;
        public const int WP_SECURE = 49;

        public const int WP_LAST = 50;

        // WP flags
        public const int WPF_TARGET = 0x0001;			// This is a target wp
        public const int WPF_ASSEMBLE = 0x0002;			// Wait for other elements here
        public const int WPF_BREAKPOINT = 0x0004;			// Break point
        public const int WPF_IP = 0x0008;			// IP waypoint
        public const int WPF_TURNPOINT = 0x0010;			// Turn point
        public const int WPF_CP = 0x0020;			// Contact point
        public const int WPF_REPEAT = 0x0040;			// Return to previous WP until time is exceeded
        public const int WPF_TAKEOFF = 0x0080;
        public const int WPF_LAND = 0x0100;			// Suck aircraft back into squadron
        public const int WPF_DIVERT = 0x0200;			// This is a divert WP (deleted upon completion of divert)
        public const int WPF_ALTERNATE = 0x0400;			// Alternate landing site
        // Climb profile flags			
        public const int WPF_HOLDCURRENT = 0x0800;			// Stay at current altitude until last minute
        // Other stuff
        public const int WPF_REPEAT_CONTINUOUS = 0x1000;			// Do this until the end of time
        public const int WPF_IN_PACKAGE = 0x2000;			// This is a package-coordinated wp
        // Even better "Other Stuff"
        public const int WPF_TIME_LOCKED = 0x4000;			// This waypoint will have an arrive time as given, and will not be changed
        public const int WPF_SPEED_LOCKED = 0x8000;			// This waypoint will have a speed as given, and will not be changed.
        public const int WPF_REFUEL_INFORMATION = 0x10000;			// This waypoint is only an informational waypoint, no mission waypoint
        public const int WPF_REQHELP = 0x20000;			// This divert waypoint is one from a request help call

        public const int WPF_CRITICAL_MASK = 0x07FF;			// If it's one of these, we can't skip this waypoint

        // time recalculation flags
        public const int WPTS_KEEP_DEPARTURE_TIMES = 0x01;	// Don't shift departure times when updating waypoint times
        public const int WPTS_SET_ALTERNATE_TIMES = 0x02;	// Set wp times for alternate waypoints

        public const int GRIDZ_SCALE_FACTOR = 10;		// How many feet per pt of Z.

        public const int MINIMUM_ASL_ALTITUDE = 5000;	// Below this # of feet, the WP is considered AGL
        // =================================================== 
        // Global functions
        // ===================================================

        public static void DeleteWPList(WayPointClass w);

        // Sets a set of waypoint times to start at waypoint w at time start. Returns duration of mission
        public static CampaignTime SetWPTimes(WayPointClass w, CampaignTime start, int speed, int flags);

        // Shifts a set of waypoints by time delta. Returns duration of mission
        public static CampaignTime SetWPTimes(WayPointClass w, long delta, int flags);

        // Sets a set of waypoint times to start at waypoint w as soon as we can get there from x,y.
        public static CampaignTime SetWPTimes(WayPointClass w, GridIndex x, GridIndex y, int speed, int flags);

        public static WayPointClass CloneWPList(WayPointClass w);
        public static WayPointClass CloneWPToList(WayPointClass w, WayPointClass stop);

        public static WayPointClass CloneWPList(WayPointClass[] wps, int waypoints);

        // KCK: This function requires that the graphic's altitude map is loaded
        public static float AdjustAltitudeForMSL_AGL(float x, float y, float z);

        public static float SetWPSpeed(WayPointClass wp);
    }

    // ============================================
    // WayPoint Class
    // ============================================

    public class WayPointClass
    {
#if USE_SH_POOLS
   
      public // Overload new/delete to use a SmartHeap fixed size pool
      public void *operator new(size_t size) { ShiAssert( size == sizeof(WayPointClass) ); return MemAllocFS(pool);	};
      public void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      public static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(WayPointClass), 200, 0 ); };
      public static void ReleaseStorage()	{ MemPoolFree( pool ); };
      public static MEM_POOL	pool;
#endif

        private GridIndex GridX;					// Waypoint's X,Y and Z coordinates (in km from southwest corner)
        private GridIndex GridY;
        private short GridZ;					// Z is in 10s of feet
        private CampaignTime Arrive;
        private CampaignTime Depart;					// This is only used for loiter waypoints 
        private VU_ID TargetID;
        private byte Action;
        private byte RouteAction;
        private byte Formation;
        private byte TargetBuilding;
        private ulong Flags;					// Various wp flags
        private short Tactic;					// Tactic to use here

        protected float Speed;
        protected WayPointClass PrevWP;					// Make this one public for kicks..
        protected WayPointClass NextWP;					// Make this one public for kicks..

        public WayPointClass();
        public WayPointClass(GridIndex x, GridIndex y, int alt, int speed, CampaignTime arr, CampaignTime station, byte action, int flags);
        public WayPointClass(ref VU_BYTE[] stream);
        public WayPointClass(FileStream fp);
        public int SaveSize();
        public int Save(ref VU_BYTE[] stream);
        public int Save(FileStream fp);

        // These functions are intended for general use
        public void SetWPTarget(VU_ID e) { TargetID = e; }
        public void SetWPTargetBuilding(byte t) { TargetBuilding = t; }
        public void SetWPAction(int a) { Action = (byte)a; }
        public void SetWPRouteAction(int a) { RouteAction = (byte)a; }
        public void SetWPFormation(int f) { Formation = (byte)f; }
        public void SetWPFlags(ulong f) { Flags = (ulong)f; }
        public void SetWPFlag(ulong f) { Flags |= (ulong)f; }
        public void UnSetWPFlag(ulong f) { Flags &= ~((ulong)(f)); }
        public void SetWPTactic(int f) { Tactic = (short)f; }
        public VU_ID GetWPTargetID() { return TargetID; }
        public CampBaseClass GetWPTarget() { return (CampBaseClass)vuDatabase.Find(TargetID); }
        public byte GetWPTargetBuilding() { return TargetBuilding; }
        public int GetWPAction() { return (int)Action; }
        public int GetWPRouteAction() { return (int)RouteAction; }
        public int GetWPFormation() { return (int)Formation; }
        public ulong GetWPFlags() { return (ulong)Flags; }
        public int GetWPTactic() { return (int)Tactic; }
        public WayPointClass GetNextWP() { return NextWP; }
        public WayPointClass GetPrevWP() { return PrevWP; }

        public void SetNextWP(WayPointClass next);
        public void SetPrevWP(WayPointClass prev);
        public void UnlinkNextWP();

        public void SplitWP();
        public void InsertWP(WayPointClass w);
        public void DeleteWP();

        public void CloneWP(WayPointClass w);
        public void SetWPTimes(CampaignTime t);
        public float DistanceTo(WayPointClass w);

        // These functions are intended for use by the campaign (They use Campaign Coordinates and times)
        public void SetWPAltitude(int alt) { GridZ = (short)(alt / GRIDZ_SCALE_FACTOR); }
        public void SetWPAltitudeLevel(int alt) { GridZ = (short)alt; }
        public void SetWPStationTime(CampaignTime t) { Depart = Arrive + t; }
        public void SetWPDepartTime(CampaignTime t) { Depart = t; }
        public void SetWPArrive(CampaignTime t) { Arrive = t; }
        public void SetWPSpeed(float s) { Speed = s; }
        public float GetWPSpeed() { return Speed; }
        public void SetWPLocation(GridIndex x, GridIndex y) { GridX = x; GridY = y; }
        public int GetWPAltitude() { return (int)(GridZ * GRIDZ_SCALE_FACTOR); }
        public int GetWPAltitudeLevel() { return GridZ; }
        public CampaignTime GetWPStationTime() { return Depart - Arrive; }
        public CampaignTime GetWPArrivalTime() { return Arrive; }
        public CampaignTime GetWPDepartureTime() { return Depart; }
        public void AddWPTimeDelta(CampaignTime dt) { Arrive += dt; Depart += dt; }
        public void GetWPLocation(ref GridIndex x, ref GridIndex y) { x = GridX; y = GridY; }

        // These functions are intended for use by the Sim (They use sim coordinates and times)
        public void SetLocation(float x, float y, float z) { GridX = CampwpStatic.SimToGrid(y); GridY = CampwpStatic.SimToGrid(x); GridZ = (short)((-1.0F * z) / GRIDZ_SCALE_FACTOR); }
        public void GetLocation(ref float x, ref float y, ref float z) { x = CampwpStatic.GridToSim(GridY); y = CampwpStatic.GridToSim(GridX); z = -1.0F * GridZ * GRIDZ_SCALE_FACTOR; }
    }


}