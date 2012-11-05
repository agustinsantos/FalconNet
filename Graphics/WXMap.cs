using System;

namespace FalconNet.Graphics
{
	public class WeatherMap
	{
		public WeatherMap ();
		// public ~WeatherMap( void );

  
		public void Setup ();

		public void Cleanup ();

		public int Load (string filename, int type);

		public DWORD			TypeAt (UINT r, UINT c)
		{
			if ((r >= h) || (c >= w))
				return 0;
			else
				return map [r * w + c].Type;
		}

		public float			BaseAt (UINT r, UINT c)
		{
			if ((r >= h) || (c >= w))
				return 1.0f - SKY_ROOF_HEIGHT;
			return map [r * w + c].BaseAltitude * WxAltScale + WxAltShift;
		}

		public float			ThicknessAt (UINT r, UINT c)
		{
			if ((r >= h) || (c >= w))
				return 1.0f;
			return (map [r * w + c].Type - FIRST_OVC_TYPE) * WxThickScale;
		}

		public float			TopsAt (UINT r, UINT c)
		{
			return BaseAt (r, c) + ThicknessAt (r, c);
		}

		public float	CellSize ()
		{
			return cellSize;
		}

		public int		WorldToTile (float distance)
		{
			return FloatToInt32 ((float)floor (distance / cellSize));
		}

		public float	TileToWorld (int rowORcol)
		{
			return (rowORcol * cellSize);
		}

		public virtual float TemperatureAt (Tpoint pos)
		{
			return 20.0f;
		}

		public float RainAt (UINT r, UINT c);

		public float VisRangeAt (UINT r, UINT c);

		public bool  LightningAt (UINT r, UINT c);
  
		protected static float WxAltShift;	// Added to cloud heights to keep them above the terrain
		protected CellState	*map;
		protected UINT		w;
		protected UINT		h;
		protected float		cellSize;
		protected int			mapType;		// User defined

  
		public int			rowShiftHistory;
		public int			colShiftHistory;
		public float		xOffset;
		public float		yOffset;
	}
}

