using System;


namespace FalconNet.Campaign
{
    // ====================
    // Campaign Terrain ADT
    // ====================

    public struct GridIndex
    {
        Int16 val;
    }


    public struct GridLocation
    {
        GridIndex x;
        GridIndex y;
    } ;

    // ---------------------------------------
    // Type and public static al Function Declarations
    // ---------------------------------------
    public static class CampTerrStatic
    {
        public const int GroundCoverMask = 0x0F; // 0xF0
        public const int GroundCoverShift = 0;
        public const int ReliefMask = 0x30; // 0xCF
        public const int ReliefShift = 4;
        public const int RoadMask = 0x40; // 0xBF
        public const int RoadShift = 6;
        public const int RailMask = 0x80; // 0x7F
        public const int RailShift = 7;

        public static short Map_Max_X;							// World Size, in grid coordinates
        public static short Map_Max_Y;

        public static void InitTheaterTerrain();

        public static void FreeTheaterTerrain();

        public static int LoadTheaterTerrain(string FileName);

        public static int LoadTheaterTerrainLight(string name);

        public static int SaveTheaterTerrain(string FileName);

        public static CellData GetCell(GridIndex x, GridIndex y);

        public static ReliefType GetRelief(GridIndex x, GridIndex y);

        public static CoverType GetCover(GridIndex x, GridIndex y);

        public static char GetRoad(GridIndex x, GridIndex y);

        public static char GetRail(GridIndex x, GridIndex y);
    }

}
