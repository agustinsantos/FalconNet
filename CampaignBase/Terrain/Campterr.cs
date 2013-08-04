using FalconNet.CampaignBase;
using FalconNet.Common.Encoding;
using FalconNet.F4Common;
using System;
using System.Diagnostics;
using System.IO;
using CellData = System.UInt16;
using CellDataType = System.Byte;
using GridIndex = System.Int16;

namespace FalconNet.Campaign
{


	// ---------------------------------------
	// Type and public static al Function Declarations
	// ---------------------------------------
	public static class CampTerrStatic
	{
		public static short Map_Max_X = 0;							// World Size, in grid coordinates
		public static short Map_Max_Y = 0;
		
		
		// =============================================
		// Campaign Terrain ADT - Private Implementation
		// =============================================
                       
		private static CellDataType[] 	TheaterCells = null;
        //private static byte	EastLongitude;
        //private static byte	SouthLatitude;
        //private static float Latitude;
        //private static float Longitude;
        //private static float CellSizeInKilometers;
		
		public static void InitTheaterTerrain ()
		{
			if (TheaterCells != null)
				FreeTheaterTerrain ();
		}

		public static void FreeTheaterTerrain ()
		{
			TheaterCells = null;
		}

		public static bool LoadTheaterTerrain (string name)
		{
            MemoryStream data;
	
			FreeTheaterTerrain ();
			data = F4File.ReadCampFile (name, "thr");
			if (data == null)
				return false;
	
			Map_Max_X = Int16EncodingLE.Decode(data);
            Map_Max_Y = Int16EncodingLE.Decode(data);

#if DEBUG
			//TODO Debug.Assert(Map_Max_X == CampaignClass.TheCampaign.TheaterSizeX);
			//TODO Debug.Assert(Map_Max_Y == CampaignClass.TheCampaign.TheaterSizeY);
#endif

			InitTheaterTerrain ();
	
			TheaterCells = new CellDataType[sizeof(CellDataType) * Map_Max_X * Map_Max_Y];
			//copy data to TheaterCells
             data.Read(TheaterCells, 0, Map_Max_X * Map_Max_Y);
			return true;
		}

        public static bool LoadTheaterTerrainLight(string fileName)
        {
            Stream fp;

            FreeTheaterTerrain();
            if ((fp = F4File.OpenCampFile(fileName, "thr", FileAccess.Read)) == null)
                return false;

            Map_Max_X = Int16EncodingLE.Decode(fp);
            Map_Max_Y = Int16EncodingLE.Decode(fp);
            F4File.CloseCampFile(fp);
            return true;
        }

		public static bool SaveTheaterTerrain (string fileName)
        {
            Stream fp;

            if (TheaterCells == null)
                return false;

            if ((fp = F4File.OpenCampFile(fileName, "thr", FileAccess.Write)) == null)
                return false;

            Int16EncodingLE.Encode(fp, Map_Max_X);
            Int16EncodingLE.Encode(fp, Map_Max_Y);
            fp.Write(TheaterCells, 0, Map_Max_X * Map_Max_Y);
            F4File.CloseCampFile(fp);
            return true;
        }

		public static CellData GetCell (GridIndex x, GridIndex y)
		{
			Debug.Assert (x >= 0 && x < Map_Max_X && y >= 0 && y < Map_Max_Y);
			return (CellData)(TheaterCells [x * Map_Max_Y + y]);
		}

		public static ReliefType GetRelief (GridIndex x, GridIndex y)
		{
			Debug.Assert (x >= 0 && x < Map_Max_X && y >= 0 && y < Map_Max_Y);
            return CampCell.GetReliefType(TheaterCells[x * Map_Max_Y + y]);
		}

		public static CoverType GetCover (GridIndex x, GridIndex y)
		{
			if ((x < 0) || (x >= Map_Max_X) || (y < 0) || (y >= Map_Max_Y))
				return CoverType.Water;
			else
                return CampCell.GetGroundCover(TheaterCells[x * Map_Max_Y + y]);
		}

        public static byte GetRoad(GridIndex x, GridIndex y)
		{
			Debug.Assert (x >= 0 && x < Map_Max_X && y >= 0 && y < Map_Max_Y);
            return CampCell.GetRoadCell(TheaterCells[x * Map_Max_Y + y]);
		}

        public static byte GetRail(GridIndex x, GridIndex y)
		{
			Debug.Assert (x >= 0 && x < Map_Max_X && y >= 0 && y < Map_Max_Y);
            return CampCell.GetRailCell(TheaterCells[x * Map_Max_Y + y]);
        }
	}
}
