using System;
using CellDataType=System.Byte;
using System.Diagnostics;

namespace FalconNet.Campaign
{
    // ====================
    // Campaign Terrain ADT
    // ====================

    public struct GridIndex
    {
        public Int16 val;
    }


    public struct GridLocation
    {
        public GridIndex x;
        public GridIndex y;
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

        public static short Map_Max_X = 0;							// World Size, in grid coordinates
        public static short Map_Max_Y = 0;
		
		
		// =============================================
		// Campaign Terrain ADT - Private Implementation
		// =============================================
                       
		private static CellDataType[] 	TheaterCells = null;
		private static byte	EastLongitude;
		private static byte	SouthLatitude;
		private static float   Latitude;
		private static float    Longitude;
		private static float    CellSizeInKilometers;
		
        public static void InitTheaterTerrain()
		{
			if (TheaterCells != null)
				FreeTheaterTerrain();
			TheaterCells = new CellDataType[Map_Max_X*Map_Max_Y];
			//TODO memset(TheaterCells,0,sizeof(CellDataType)*Map_Max_X*Map_Max_Y);
		}


        public static void FreeTheaterTerrain()
		{
			TheaterCells = null;
		}

        public static int LoadTheaterTerrain(string FileName)
		{throw new NotImplementedException();}

        public static int LoadTheaterTerrainLight(string name)
		{throw new NotImplementedException();}

        public static int SaveTheaterTerrain(string FileName)
		{throw new NotImplementedException();}

        public static CellData GetCell(GridIndex x, GridIndex y)
		{
		    //TODO Debug.Assert(x >= 0 && x < Map_Max_X && y >= 0 && y < Map_Max_Y);
			return new CellData(TheaterCells[x.val*Map_Max_Y + y.val]);
		}

        public static ReliefType GetRelief(GridIndex x, GridIndex y)
		{
		    //TODO Debug.Assert(x >= 0 && x < Map_Max_X && y >= 0 && y < Map_Max_Y);
			return (ReliefType)((TheaterCells[x.val*Map_Max_Y + y.val] & ReliefMask) >> ReliefShift);
		}

        public static CoverType GetCover(GridIndex x, GridIndex y)
		{
			if ((x.val < 0) || (x.val >= Map_Max_X) || (y.val < 0) || (y.val >= Map_Max_Y))
				return (CoverType) CoverType.Water;
			else
				return (CoverType)((TheaterCells[x.val*Map_Max_Y + y.val] & GroundCoverMask) >> GroundCoverShift);
		}

        public static char GetRoad(GridIndex x, GridIndex y)
		{
			//TODO Debug.Assert(x >= 0 && x < Map_Max_X && y >= 0 && y < Map_Max_Y);
			return (char)((TheaterCells[x.val*Map_Max_Y + y.val] & RoadMask) >> RoadShift);
		}

        public static char GetRail(GridIndex x, GridIndex y)
		{
			//TODO Debug.Assert(x >= 0 && x < Map_Max_X && y >= 0 && y < Map_Max_Y);
			return (char)((TheaterCells[x.val*Map_Max_Y + y.val] & RailMask) >> RailShift);
		}
    }

}
