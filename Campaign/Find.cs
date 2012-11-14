using System;
using FalconNet.FalcLib;
using FalconNet.Common;
using Objective=FalconNet.Campaign.ObjectiveClass;
using Flight=FalconNet.Campaign.FlightClass;
using Team=System.Int32;
using FalconNet.VU;

namespace FalconNet.Campaign
{


	// =============================
	// Path finding flags
	// =============================
	public enum FIND
	{
		FIND_FINDFRIENDLY		=0x01,					// Find friendly stuff (default is enemy)
		FIND_FINDCOUNT			=0x02,
		FIND_NOAIR				=0x04,					// Ignore air units
		FIND_NODETECT			=0x08,					// Ignore detection only units
		FIND_NOMOVERS			=0x10,					// Ignore moving units
		FIND_CAUTIOUS			=0x20,					// Assume slightly larger enemy ranges
		FIND_AD_ONLY			=0x40,					// Find only dedicated AD assets
		FIND_FINDUNSPOTTED		=0x80,					// Returns somewhat lesser score for unspotted stuff
		FIND_THISOBJONLY		=0x100,					// Find children within this secondary objective only
		FIND_SECONDARYONLY		=0x200,					// Find secondary objectives only
		FIND_STANDARDONLY		=0x400					// Find only standard (non secondary) objectives
	}
	
	public static class FindStatic
	{
		public const int ALT_LEVELS =5;

		public static int[] MaxAltAtLevel = new int[ALT_LEVELS];
		public static int[] MinAltAtLevel = new int[ALT_LEVELS];

		// ==========================================
		// Globals
		// ==========================================
		
		// extern byte ThreatSearch[MAX_CAMP_ENTITIES];			// Search data
		
		// =============================
		// Global function headers
		// =============================

		public static int DistSqu (GridIndex ox, GridIndex oy, GridIndex dx, GridIndex dy)
		{throw new NotImplementedException();}

		public static float Distance (GridIndex ox, GridIndex oy, GridIndex dx, GridIndex dy)
		{throw new NotImplementedException();}

		public static float Distance (float ox, float oy, float dx, float dy)
		{throw new NotImplementedException();}

		public static float DistSqu (float ox, float oy, float dx, float dy)
		{throw new NotImplementedException();}

		public static float DistanceToFront (GridIndex x, GridIndex y)
		{throw new NotImplementedException();}

		public static float DirectionToFront (GridIndex x, GridIndex y)
		{throw new NotImplementedException();}

		public static float DirectionTowardFriendly (GridIndex x, GridIndex y, int team)
		{throw new NotImplementedException();}

		public static int GetBearingDeg (float x, float y, float tx, float ty)
		{throw new NotImplementedException();}

		public static int GetRangeFt (float x, float y, float tx, float ty)
		{throw new NotImplementedException();}

		public static object PackXY (GridIndex x, GridIndex y)
		{throw new NotImplementedException();}

		public static void UnpackXY (object n, ref GridIndex x, ref GridIndex y)
		{throw new NotImplementedException();}

		public static CampaignTime TimeToArrive (float distance, float speed)
		{throw new NotImplementedException();}

		public static void Trim (ref GridIndex x, ref GridIndex y)
		{throw new NotImplementedException();}

		public static float AngleTo (GridIndex ox, GridIndex oy, GridIndex tx, GridIndex ty)
		{throw new NotImplementedException();}

		public static CampaignHeading DirectionTo (GridIndex ox, GridIndex oy, GridIndex tx, GridIndex ty)
		{throw new NotImplementedException();}

		public static CampaignHeading DirectionTo (GridIndex ox, GridIndex oy, GridIndex tx, GridIndex ty,
									  GridIndex cx, GridIndex cy)
		{throw new NotImplementedException();}

		public static int OctantTo (GridIndex ox, GridIndex oy, GridIndex tx, GridIndex ty)
		{throw new NotImplementedException();}

		public static int OctantTo (float ox, float oy, float tx, float ty)
		{throw new NotImplementedException();}

		public static CampaignTime TimeBetween (GridIndex x, GridIndex y, GridIndex tx, GridIndex ty, int speed)
		{throw new NotImplementedException();}

		public static CampaignTime TimeBetweenO (Objective o1, Objective o2, int speed)
		{throw new NotImplementedException();}

		public static Objective FindObjective (VU_ID id)
		{throw new NotImplementedException();}

		public static Unit FindUnit (VU_ID id)
		{throw new NotImplementedException();}

		public static CampEntity FindEntity (VU_ID id)
		{throw new NotImplementedException();}

		public static CampEntity GetEntityByCampID (int id)
		{throw new NotImplementedException();}

		public static Objective FindNearestSupplySource(Objective o)
		{throw new NotImplementedException();}

		public static Unit FindNearestEnemyUnit (GridIndex X, GridIndex Y, GridIndex max)
		{throw new NotImplementedException();}

		public static Unit FindNearestRealUnit (GridIndex X, GridIndex Y, ref float last, GridIndex max)
		{throw new NotImplementedException();}

		public static Unit FindNearestUnit (VuFilteredList l, GridIndex X, GridIndex Y, ref float last)
		{throw new NotImplementedException();}

		public static Unit FindNearestUnit (GridIndex X, GridIndex Y, ref float  last)
		{throw new NotImplementedException();}

		public static Unit FindUnitByXY (VuFilteredList l, GridIndex X, GridIndex Y, int domain)
		{throw new NotImplementedException();}

		public static Unit GetUnitByXY (GridIndex X, GridIndex Y, int domain)
		{throw new NotImplementedException();}

		public static Unit GetUnitByXY (GridIndex X, GridIndex Y)
		{throw new NotImplementedException();}

		public static Objective FindNearestObjective (GridIndex X, GridIndex Y, ref float last, GridIndex maxdist)
		{throw new NotImplementedException();}

		public static Objective FindNearestObjective (VuFilteredList l, GridIndex X, GridIndex Y, ref float last)
		{throw new NotImplementedException();}

		public static Objective FindNearestObjective (GridIndex X, GridIndex Y, ref float last)
		{throw new NotImplementedException();}

		public static Objective FindNearestAirbase (GridIndex X, GridIndex Y)
		{throw new NotImplementedException();}

		public static Objective FindNearbyAirbase (GridIndex X, GridIndex Y)
		{throw new NotImplementedException();}

		public static Objective FindNearestFriendlyAirbase (Team who, GridIndex X, GridIndex Y)
		{throw new NotImplementedException();}

		public static Objective FindNearestFriendlyRunway (Team who, GridIndex X, GridIndex Y)
		{throw new NotImplementedException();}

		public static Objective FindNearestFriendlyObjective(VuFilteredList l, Team who, ref GridIndex x, ref GridIndex y, int flags)
		{throw new NotImplementedException();}

		public static Objective FindNearestFriendlyObjective(Team who, ref GridIndex x, ref GridIndex y, int flags)
		{throw new NotImplementedException();}

		public static Objective FindNearestFriendlyPowerStation(VuFilteredList l, Team who, GridIndex x, GridIndex y)
		{throw new NotImplementedException();}

		public static Objective GetObjectiveByXY (GridIndex X, GridIndex Y)
		{throw new NotImplementedException();}

		public static int AnalyseThreats (GridIndex X, GridIndex Y, MoveType mt, int alt, int roe_check, Team who, int flags)
		{throw new NotImplementedException();}

		public static int CollectThreats (GridIndex X, GridIndex Y, int Z, Team who, int flags, F4PFList foundlist)
		{throw new NotImplementedException();}

		public static int CollectThreatsFast (GridIndex X, GridIndex Y, int altlevel, Team who, int flags, F4PFList foundlist)
		{throw new NotImplementedException();}

		public static int ScoreThreat (GridIndex X, GridIndex Y, int Z, Team who, int flags)
		{throw new NotImplementedException();}

		public static int ScoreThreatFast(GridIndex X, GridIndex Y, int altlevel, Team who)
		{throw new NotImplementedException();}

		public static float GridToSim (GridIndex x)
		{throw new NotImplementedException();}

		public static GridIndex SimToGrid (float x)
		{throw new NotImplementedException();}

		public static void ConvertGridToSim(GridIndex x, GridIndex y, ref vector pos)
		{throw new NotImplementedException();}

		public static void ConvertSimToGrid(vector pos, ref GridIndex x, ref GridIndex y)
		{throw new NotImplementedException();}
		
#if TODO
		public static void ConvertSimToGrid (Tpoint pos, ref GridIndex x, ref GridIndex y)
		{throw new NotImplementedException();}
#endif
		
		public static MoveType AltToMoveType (int alt)
		{throw new NotImplementedException();}

		public static int GetAltitudeLevel (int alt)
		{throw new NotImplementedException();}

		public static int GetAltitudeFromLevel (int level, int seed)
		{throw new NotImplementedException();}

		public static CampaignTime TimeTo (GridIndex x, GridIndex y, GridIndex tx, GridIndex ty, int speed)
		{throw new NotImplementedException();}

		public static F4PFList GetDistanceList (Team who, int  i, int j)
		{throw new NotImplementedException();}

		public static void FillDistanceList (List list, Team who, int  i, int j)
		{throw new NotImplementedException();}

		public static FalconSessionEntity FindPlayer (Flight flight, byte planeNum)
		{throw new NotImplementedException();}

		public static FalconSessionEntity FindPlayer (VU_ID flightID, byte planeNum)
		{throw new NotImplementedException();}

		public static FalconSessionEntity FindPlayer (Flight flight, byte planeNum, byte pilotSlot)
		{throw new NotImplementedException();}

		public static FalconSessionEntity FindPlayer (VU_ID flightID, byte planeNum, byte pilotSlot)
		{throw new NotImplementedException();}

	}
}

