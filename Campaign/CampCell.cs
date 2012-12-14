using System;
using CellData=System.UInt16;

namespace FalconNet.Campaign
{
	public static class CampCell
	{
		public static void SetReliefType (ref CellData TheCell, ReliefType NewReliefType)
		{
			CellData Temp = (CellData)NewReliefType;
		
			TheCell = (CellData)((TheCell & ~CampTerrStatic.ReliefMask) |
			                     ((Temp << CampTerrStatic.ReliefShift) & CampTerrStatic.ReliefMask));
		}

		public static void SetGroundCover (ref CellData TheCell, CoverType NewGroundCover)
		{
			CellData Temp = (CellData)NewGroundCover;

			TheCell = (CellData)((TheCell & ~CampTerrStatic.GroundCoverMask) | 
			                         ((Temp << CampTerrStatic.GroundCoverShift) & CampTerrStatic.GroundCoverMask));
		}

		public static void SetRoadCell (ref CellData TheCell, byte Road)
		{
			CellData Temp = (CellData)Road;
		
			TheCell = (CellData)((TheCell & ~CampTerrStatic.RoadMask) | 
			                     ((Temp << CampTerrStatic.RoadShift) & CampTerrStatic.RoadMask));
		}

		public static void SetRailCell (CellData TheCell, byte Rail)
		{
			CellData Temp = (CellData)Rail;
	
			TheCell = (CellData)((TheCell & ~CampTerrStatic.RailMask) | ((Temp << CampTerrStatic.RailShift) & CampTerrStatic.RailMask));
		}

		public static char GetAltitudeCode (CellData TheCell)
		{
			throw new NotImplementedException ();
		}

		public static ReliefType GetReliefType (CellData TheCell)
	   {
	   		return (ReliefType)((TheCell & CampTerrStatic.ReliefMask) >> CampTerrStatic.ReliefShift);
	   }

		public static CoverType GetGroundCover (CellData TheCell)
		{
			return (CoverType)((TheCell & CampTerrStatic.GroundCoverMask) >> CampTerrStatic.GroundCoverShift);
		}

		public static byte GetRoadCell (CellData TheCell)
		{
			return (byte)((TheCell & CampTerrStatic.RoadMask) >> CampTerrStatic.RoadShift);
		}

		public static byte GetRailCell (CellData TheCell)
		{
			return (byte)((TheCell & CampTerrStatic.RailMask) >> CampTerrStatic.RailShift);
		}
	}
}

