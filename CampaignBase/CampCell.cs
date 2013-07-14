using FalconNet.CampaignBase;
using System;
using CellData = System.UInt16;

namespace FalconNet.Campaign
{
    public static class CampCell
    {
        public static void SetReliefType(ref CellData TheCell, ReliefType NewReliefType)
        {
            CellData Temp = (CellData)NewReliefType;

            TheCell = (CellData)((TheCell & ~ReliefMask) |
                                 ((Temp << ReliefShift) & ReliefMask));
        }

        public static void SetGroundCover(ref CellData TheCell, CoverType NewGroundCover)
        {
            CellData Temp = (CellData)NewGroundCover;

            TheCell = (CellData)((TheCell & ~GroundCoverMask) |
                                     ((Temp << GroundCoverShift) & GroundCoverMask));
        }

        public static void SetRoadCell(ref CellData TheCell, byte Road)
        {
            CellData Temp = (CellData)Road;

            TheCell = (CellData)((TheCell & ~RoadMask) |
                                 ((Temp << RoadShift) & RoadMask));
        }

        public static void SetRailCell(CellData TheCell, byte Rail)
        {
            CellData Temp = (CellData)Rail;

            TheCell = (CellData)((TheCell & ~RailMask) | ((Temp << RailShift) & RailMask));
        }

        public static char GetAltitudeCode(CellData TheCell)
        {
            throw new NotImplementedException();
        }

        public static ReliefType GetReliefType(CellData TheCell)
        {
            return (ReliefType)((TheCell & ReliefMask) >> ReliefShift);
        }

        public static CoverType GetGroundCover(CellData TheCell)
        {
            return (CoverType)((TheCell & GroundCoverMask) >> GroundCoverShift);
        }

        public static byte GetRoadCell(CellData TheCell)
        {
            return (byte)((TheCell & RoadMask) >> RoadShift);
        }

        public static byte GetRailCell(CellData TheCell)
        {
            return (byte)((TheCell & RailMask) >> RailShift);
        }

        private const int GroundCoverMask = 0x0F; // 0xF0
        private const int GroundCoverShift = 0;
        private const int ReliefMask = 0x30; // 0xCF
        private const int ReliefShift = 4;
        private const int RoadMask = 0x40; // 0xBF
        private const int RoadShift = 6;
        private const int RailMask = 0x80; // 0x7F
        private const int RailShift = 7;

    }
}

