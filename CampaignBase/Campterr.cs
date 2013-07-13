using System;
using CellDataType=System.Byte;
using System.Diagnostics;
using GridIndex = System.Int16;
using CellData=System.UInt16;
using FalconNet.CampaignBase;

namespace FalconNet.Campaign
{
	// ====================
	// Campaign Terrain ADT
	// ====================
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
		private static float Latitude;
		private static float Longitude;
		private static float CellSizeInKilometers;
		
		public static void InitTheaterTerrain ()
		{
			if (TheaterCells != null)
				FreeTheaterTerrain ();
			TheaterCells = new CellDataType[Map_Max_X * Map_Max_Y];
			//TODO memset(TheaterCells,0,sizeof(CellDataType)*Map_Max_X*Map_Max_Y);
		}

		public static void FreeTheaterTerrain ()
		{
			TheaterCells = null;
		}

		public static bool LoadTheaterTerrain (string name)
		{
			byte[] data;
			int data_ptr;
	
			FreeTheaterTerrain ();
			data = CampaignFile.ReadCampFile (name, "thr");
			if (data == null)
				return false;

			data_ptr = 0;
	
			Map_Max_X = BitConverter.ToInt16 (data, data_ptr);
			data_ptr += sizeof(short);
			Map_Max_Y = BitConverter.ToInt16 (data, data_ptr);
			data_ptr += sizeof(short);

#if DEBUG
			//TODO Debug.Assert(Map_Max_X == CampaignClass.TheCampaign.TheaterSizeX);
			//TODO Debug.Assert(Map_Max_Y == CampaignClass.TheCampaign.TheaterSizeY);
#endif

			InitTheaterTerrain ();
	
			TheaterCells = new CellDataType[sizeof(CellDataType) * Map_Max_X * Map_Max_Y];
			//TODO copy data to TheaterCells
			//memcpy (TheaterCells, data_ptr, sizeof (CellDataType) * Map_Max_X * Map_Max_Y);
			Array.Copy(data,data_ptr, TheaterCells, 0,Map_Max_X * Map_Max_Y);
			data = null;
			return true;
		}

		public static int LoadTheaterTerrainLight (string name)
		{
			throw new NotImplementedException ();
		}

		public static int SaveTheaterTerrain (string FileName)
		{
			throw new NotImplementedException ();
		}

		public static CellData GetCell (GridIndex x, GridIndex y)
		{
			Debug.Assert (x >= 0 && x < Map_Max_X && y >= 0 && y < Map_Max_Y);
			return (CellData)(TheaterCells [x * Map_Max_Y + y]);
		}

		public static ReliefType GetRelief (GridIndex x, GridIndex y)
		{
			Debug.Assert (x >= 0 && x < Map_Max_X && y >= 0 && y < Map_Max_Y);
			return (ReliefType)((TheaterCells [x * Map_Max_Y + y] & CampTerrStatic.ReliefMask) >> CampTerrStatic.ReliefShift);
		}

		public static CoverType GetCover (GridIndex x, GridIndex y)
		{
			if ((x < 0) || (x >= Map_Max_X) || (y < 0) || (y >= Map_Max_Y))
				return (CoverType)CoverType.Water;
			else
				return (CoverType)((TheaterCells [x * Map_Max_Y + y] & GroundCoverMask) >> GroundCoverShift);
		}

		public static char GetRoad (GridIndex x, GridIndex y)
		{
			Debug.Assert (x >= 0 && x < Map_Max_X && y >= 0 && y < Map_Max_Y);
			return (char)((TheaterCells [x * Map_Max_Y + y] & RoadMask) >> RoadShift);
		}

		public static char GetRail (GridIndex x, GridIndex y)
		{
			Debug.Assert (x >= 0 && x < Map_Max_X && y >= 0 && y < Map_Max_Y);
			return (char)((TheaterCells [x * Map_Max_Y + y] & RailMask) >> RailShift);
		}
	}
}
