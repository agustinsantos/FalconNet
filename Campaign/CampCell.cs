using System;

namespace FalconNet.Campaign
{
	public struct CellData
	{
		private byte val;
	}
	
	public static class CampCell
	{
		public static void SetReliefType (CellData TheCell, ReliefType NewReliefType);

        public static void SetGroundCover(CellData TheCell, CoverType NewGroundCover);

        public static void SetRoadCell(CellData TheCell, sbyte Road);

        public static void SetRailCell(CellData TheCell, sbyte Rail);

        public static char GetAltitudeCode(CellData TheCell);

        public static ReliefType GetReliefType(CellData TheCell);

        public static CoverType GetGroundCover(CellData TheCell);

        public static sbyte GetRoadCell(CellData TheCell);

        public static sbyte GetRailCell(CellData TheCell);
	}
}

