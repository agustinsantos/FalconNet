using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.FalcLib
{
    // ==================================
    // Point Types
    // ==================================

    public enum PointTypes
    {
        NoPt = 0,
        RunwayPt,
        TakeoffPt,
        TaxiPt,
        SAMPt,
        ArtilleryPt,
        AAAPt,
        RadarPt,
        RunwayDimPt,
        SupportPt,
        StaticRadarPt,
        SmallParkPt,
        LargeParkPt,
        SmallDockPt,
        LargeDockPt,
        TakeRunwayPt,
        HelicopterPt,
        FollowMePt,
        TrackPt,
        CritTaxiPt
    }

    public enum PointListTypes
    {
        NoList = 0,
        RunwayListType = 1,
        SAMListType = 4,
        ArtListType = 5,
        AAAListType = 6,
        RnwyDimListType = 8,
        StaticRadarListType = 10,
        ParkListType = 11,
        RnwyListLtType = 12,
        RnwyListRtType = 13,
        HeliListType = 14,
        FollowListType = 15,
        DockListType = 16,
        TrackListType = 17,
    }

    public static class PtData
    {
        public const int PT_FIRST = 0x01; // Flag set if first in pt list
        public const int PT_LAST = 0x02; // Flag set if last in pt list
        public const int PT_OCCUPIED = 0x04; // 02JAN04 - FRB - Flag set if last Parking spot occupied

        public static int GetTaxiPosition(int point, int rwindex)
        {
            int count = 0;
            int pt = EntityDB.PtHeaderDataTable[rwindex].first;

            while (pt == 0 && pt != point)
            {
                if (pt > point) break;  // 24JAN04 - FRB - Cover case of a/c on parking spot (not TaxiPt)

                pt = GetNextTaxiPt(pt);
                count++;
            }

            return pt != 0 ? count : 0;
        }

        public static int GetCritTaxiPt(int headerindex)
        {
            int point = EntityDB.PtHeaderDataTable[headerindex].first;

            while (EntityDB.PtDataTable[point].type != PointTypes.CritTaxiPt)
            {
                if ((EntityDB.PtDataTable[point].flags & PT_LAST) != 0)
                    return point;

                point++;
            }

            return point;
        }

        public static int GetFirstPt(int headerindex)
        {
            return EntityDB.PtHeaderDataTable[headerindex].first;
        }

        public static int GetNextPt(int ptindex)
        {
            if ((ptindex < 0) || (ptindex >= EntityDB.NumPts))
                ptindex = 0;

            if ((EntityDB.PtDataTable[ptindex].flags & PT_LAST) == 0)
            {
                return ptindex + 1;
            }

            return 0;
        }

        public static int GetNextTaxiPt(int ptindex)
        {
            // FRB - CTD's here
            if ((ptindex < 0) || (ptindex >= EntityDB.NumPts))
                ptindex = 0;

            ptindex = GetNextPt(ptindex);

            while (ptindex != 0 && EntityDB.PtDataTable[ptindex].type != PointTypes.TaxiPt &&
                              EntityDB.PtDataTable[ptindex].type != PointTypes.CritTaxiPt)
            {
                ptindex = GetNextPt(ptindex);
            }

            return ptindex;
        }

        public static int GetNextPtLoop(int ptindex)
        {
            if ((ptindex < 0) || (ptindex >= EntityDB.NumPts))
                ptindex = 0;

            if ((EntityDB.PtDataTable[ptindex].flags & PT_LAST) == 0)
            {
                return ptindex + 1;
            }

            return ptindex;
        }

        public static int GetNextPtCrit(int ptindex)
        {
            if ((ptindex < 0) || (ptindex >= EntityDB.NumPts))
                ptindex = 0;

            if (EntityDB.PtDataTable[ptindex].type != PointTypes.CritTaxiPt &&
               (EntityDB.PtDataTable[ptindex].flags & PT_LAST) == 0)
                return ptindex + 1;

            return 0;
        }

        public static int GetPrevPt(int ptindex)
        {
            if ((ptindex < 0) || (ptindex >= EntityDB.NumPts))
                ptindex = 0;

            if ((EntityDB.PtDataTable[ptindex].flags & PT_FIRST) == 0)
                return ptindex - 1;

            return 0;
        }

        public static int GetPrevTaxiPt(int ptindex)
        {
            if ((ptindex < 0) || (ptindex >= EntityDB.NumPts))
                ptindex = 0;

            ptindex = GetPrevPt(ptindex);

            while (ptindex != 0 && EntityDB.PtDataTable[ptindex].type != PointTypes.TaxiPt &&
                              EntityDB.PtDataTable[ptindex].type != PointTypes.CritTaxiPt)
                ptindex = GetPrevPt(ptindex);

            return ptindex;
        }

        public static int GetPrevPtLoop(int ptindex)
        {
            if ((ptindex < 0) || (ptindex >= EntityDB.NumPts))
                ptindex = 0;

            if ((EntityDB.PtDataTable[ptindex].flags & PT_FIRST) == 0)
                return ptindex - 1;

            return ptindex;
        }

        public static int GetPrevPtCrit(int ptindex)
        {
            if ((ptindex < 0) || (ptindex >= EntityDB.NumPts))
                ptindex = 0;

            if (EntityDB.PtDataTable[ptindex].type != PointTypes.CritTaxiPt &&
                (EntityDB.PtDataTable[ptindex].flags & PT_FIRST) == 0)
                return ptindex - 1;

            return 0;
        }

        public static void TranslatePointData(CampBaseClass e, int ptindex, out float x, out float y)
        {
            x = y = -1;

            if ((ptindex < 0) || (ptindex >= EntityDB.NumPts))
                ptindex = 0;

            if (e != null && e.IsObjective())
            {
                // KCK TODO: Rotate these points by objective's heading before translating
                // SCR 11/29/98  I don't think objectives HAVE headings, so this is correct.
                x = e.XPos();
                y = e.YPos();
                x += EntityDB.PtDataTable[ptindex].yOffset;  // KCK NOTE: axis' are reversed
                y += EntityDB.PtDataTable[ptindex].xOffset;
            }
        }

        //TODO delete public static int FindRunways(CampBaseClass airbase, int usage, int* rw1, int* rw2, int findall);

        public static VIS_TYPES CheckHeaderStatus(CampBaseClass e, int index)
        {
 		     VIS_TYPES status = VIS_TYPES.VIS_NORMAL;
            int i = 0;
            VIS_TYPES fs;

            while (status != VIS_TYPES.VIS_DESTROYED && i < Camplib.MAX_FEAT_DEPEND)
            {
                if (EntityDB.PtHeaderDataTable[index].features[i] < 255)
                {
                    if (e != null && e.IsObjective())
                    {
#if TODO
		                fs = ((ObjectiveClass)e).GetFeatureStatus(EntityDB.PtHeaderDataTable[index].features[i]);
                        // ShiAssert(((Objective)e)->GetFeatureValue(PtHeaderDataTable[index].features[i]) > 0);
 
#endif
                        throw new NotImplementedException();
                    }
                    else
                        fs = VIS_TYPES.VIS_NORMAL;

                    if (fs > status)
                        status = fs;
                }

                i++;
            }

            return status;   
 
        }

        public static int GetQueue(int rwindex)
        {
            return EntityDB.PtHeaderDataTable[rwindex].runwayNum;
        }

        public static int GetFirstParkPt(int headerindex)
        {
            int pt = EntityDB.PtHeaderDataTable[headerindex].first;

            while (pt != 0)
            {
                switch (EntityDB.PtDataTable[pt].type)
                {
                    case PointTypes.SmallParkPt:
                    case PointTypes.LargeParkPt:
                        return pt; // found a parking space
                }

                if ((EntityDB.PtDataTable[pt].flags & PT_LAST) != 0)
                    return 0; // examined all

                pt++; // FRB - Should pt be incremented???? I added pt++;  fn() not used :^(
            }

            return 0;
        }
        public static int GetNextParkPt(int pt)
        {
            if ((EntityDB.PtDataTable[pt].flags & PT_LAST) != 0)
                return 0; // stop

            pt++;

            while (pt != 0)
            {
                switch (EntityDB.PtDataTable[pt].type)
                {
                    case PointTypes.SmallParkPt:
                    case PointTypes.LargeParkPt:
                        return pt; // found a parking space
                }

                if ((EntityDB.PtDataTable[pt].flags & PT_LAST) != 0)
                    return 0; // examined all

                pt++;
            }

            return 0;
        }
        public static int GetPrevParkPt(int pt)
        {
            if ((EntityDB.PtDataTable[pt].flags & PT_FIRST) != 0)
                return 0; // stop

            pt--;

            while (pt > 0)
            {
                switch (EntityDB.PtDataTable[pt].type)
                {
                    case PointTypes.SmallParkPt:
                    case PointTypes.LargeParkPt:
                        return pt; // found a parking space
                }

                if ((EntityDB.PtDataTable[pt].flags & PT_FIRST) != 0)
                    return 0; // examined all

                pt--;
            }

            return 0;
        }
        public static int GetNextParkTypePt(int pt, PointTypes type)
        {
            if ((EntityDB.PtDataTable[pt].flags & PT_LAST) != 0)
                return 0; // stop

            pt++;

            while (pt != 0)
            {
                if (EntityDB.PtDataTable[pt].type == type)
                {
                    return pt; // found a parking space
                }

                if ((EntityDB.PtDataTable[pt].flags & PT_LAST) != 0)
                {
                    return 0; // examined all
                }

                pt++;
            }

            return 0;
        }
        public static int GetPrevParkTypePt(int pt, PointTypes type)
        {
            if ((EntityDB.PtDataTable[pt].flags & PT_FIRST) != 0)
                return 0; // stop

            pt--;

            while (pt > 0)
            {
                if (EntityDB.PtDataTable[pt].type == type)
                    return pt; // found a parking space

                if ((EntityDB.PtDataTable[pt].flags & PT_FIRST) != 0)
                    return 0; // examined all

                pt--;
            }

            return 0;
        }
    }
}
