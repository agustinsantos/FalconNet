using System;

namespace FalconNet.Campaign
{
	public struct CellData
	{
		public byte val;
		public CellData(byte v)
		{
			val = v;
		}
	}
	
	public static class CampCell
	{
		public static void SetReliefType (CellData TheCell, ReliefType NewReliefType)
		{throw new NotImplementedException();}

        public static void SetGroundCover(CellData TheCell, CoverType NewGroundCover)
		{throw new NotImplementedException();}

        public static void SetRoadCell(CellData TheCell, sbyte Road)
		{throw new NotImplementedException();}

        public static void SetRailCell(CellData TheCell, sbyte Rail)
		{throw new NotImplementedException();}

        public static char GetAltitudeCode(CellData TheCell)
		{throw new NotImplementedException();}

        public static ReliefType GetReliefType(CellData TheCell)
		{throw new NotImplementedException();}

        public static CoverType GetGroundCover(CellData TheCell)
		{throw new NotImplementedException();}

        public static sbyte GetRoadCell(CellData TheCell)
		{throw new NotImplementedException();}

        public static sbyte GetRailCell(CellData TheCell)
		{throw new NotImplementedException();}
	}
}

