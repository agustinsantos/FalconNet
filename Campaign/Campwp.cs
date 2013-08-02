using System;
using System.IO;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using VU_BYTE = System.Byte;
using GridIndex = System.Int16;
using FalconNet.CampaignBase;
using FalconNet.Common.Maths;
namespace FalconNet.Campaign
{
    public static class CampwpStatic
    {
        public static void ConvertSimToGrid(vector pos, ref GridIndex x, ref GridIndex y)
        { throw new NotImplementedException(); }
        public static void ConvertGridToSim(GridIndex x, GridIndex y, out vector pos)
        { throw new NotImplementedException(); }
        public static float GridToSim(GridIndex x)
        { throw new NotImplementedException(); }
        public static GridIndex SimToGrid(float x)
        { throw new NotImplementedException(); }



       // public const int WP_LAST = 50;

        

        // time recalculation flags
        public const int WPTS_KEEP_DEPARTURE_TIMES = 0x01;	// Don't shift departure times when updating waypoint times
        public const int WPTS_SET_ALTERNATE_TIMES = 0x02;	// Set wp times for alternate waypoints

        public const int GRIDZ_SCALE_FACTOR = 10;		// How many feet per pt of Z.

        public const int MINIMUM_ASL_ALTITUDE = 5000;	// Below this # of feet, the WP is considered AGL
        // =================================================== 
        // Global functions
        // ===================================================

        public static void DeleteWPList(WayPointClass w)
        { throw new NotImplementedException(); }

        // Sets a set of waypoint times to start at waypoint w at time start. Returns duration of mission
        public static CampaignTime SetWPTimes(WayPointClass w, CampaignTime start, int speed, int flags)
        { throw new NotImplementedException(); }

        // Shifts a set of waypoints by time delta. Returns duration of mission
        public static CampaignTime SetWPTimes(WayPointClass w, long delta, int flags)
        { throw new NotImplementedException(); }

        // Sets a set of waypoint times to start at waypoint w as soon as we can get there from x,y.
        public static CampaignTime SetWPTimes(WayPointClass w, GridIndex x, GridIndex y, int speed, int flags)
        { throw new NotImplementedException(); }

        public static WayPointClass CloneWPList(WayPointClass w)
        { throw new NotImplementedException(); }
        public static WayPointClass CloneWPToList(WayPointClass w, WayPointClass stop)
        { throw new NotImplementedException(); }

        public static WayPointClass CloneWPList(WayPointClass[] wps, int waypoints)
        { throw new NotImplementedException(); }

        // KCK: This function requires that the graphic's altitude map is loaded
        public static float AdjustAltitudeForMSL_AGL(float x, float y, float z)
        { throw new NotImplementedException(); }

        public static float SetWPSpeed(WayPointClass wp)
        { throw new NotImplementedException(); }
    }



}
