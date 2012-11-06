using System;
using FalconNet.Common;

namespace FalconNet.Graphics
{
	public struct CellState {
		public byte	BaseAltitude;	// hundreds of feet MSL
		public byte	Type;			// cloud tile number (or thickness for OVC)
	};


public class WeatherMap 
	{
#if TODO
		// The global weather database used by everyone
public static WeatherMap  TheWeather = new WeatherMap();

// Some handy constants which bound the weather environment
public static float				SKY_ROOF_HEIGHT	= 30000.0f;
public static float				SKY_ROOF_RANGE= 200000.0f;
public static float				SKY_MAX_HEIGHT= 70000.0f;
		
public const int LAST_CLEAR_TYPE	=	2;
public const int FIRST_OVC_TYPE	=	4;
public const int  MAX_CLOUD_TYPE	=	80;		// Maximum value of cloud type
public const float 	CLOUD_CELL_SIZE		= 8.0f;	// How big is a cloud tile (in KM)

// Conversion factor from BYTE base elevation to world space elevation (Z down)
public const float WxAltScale		= -50.0f;
public const float WxThickScale		= -250.0f;
  
	public WeatherMap(   )
		{
	w			= h = 0;
	cellSize	= 0.0F;
	mapType		= 0;
}
	// public ~WeatherMap( void );

  
	public void Setup(   )
			{	
	// Make sure we're clean before doing this
	Cleanup();

	// Record the size of a weather cell
	cellSize = CLOUD_CELL_SIZE * Constants.FEET_PER_KM;

	// Store the weather map's width and height
	w = (uint)( (float)Math.Floor(((TheMap.EastEdge()  - TheMap.WestEdge())  / cellSize) + 0.5f) );
	h = (uint)( (float)Math.Floor(((TheMap.NorthEdge() - TheMap.SouthEdge()) / cellSize) + 0.5f) );

	// Start with no cloud offset (might want to save/restore this)
	xOffset = 0.0f;
	yOffset = 0.0f;

	// Start with no recorded shift history
	rowShiftHistory = 0;
	colShiftHistory = 0;

	// Allocate memory for the weather map.
	map = new CellState[ w*h ];


	// Intialize the map to "clear"
	for (DWORD k = 0; k < w*h; k++) {
		map[k].Type = 0;
		map[k].BaseAltitude = 0xFF;
	}

	mapType = 0;		// Reset the map type, since we cleared the map.
}
	public void Cleanup(   )
			{
	// Release our weather map memory
	delete[] map;
	map = NULL;
}

	public int Load(string filename, int type)
		{
	HANDLE		mapFile;
	int			result;
	unsigned	nw,nh;
	DWORD		bytesRead;
	float		ncs;

	if (mapType && type == mapType) {
		return 1;							// Map already loaded
	}

	ncs = CLOUD_CELL_SIZE * FEET_PER_KM;
	nw = FloatToInt32( (float)floor(((TheMap.EastEdge()  - TheMap.WestEdge())  / cellSize) + 0.5f) );
	nh = FloatToInt32( (float)floor(((TheMap.NorthEdge() - TheMap.SouthEdge()) / cellSize) + 0.5f) );
	if (nw != w || nh != h || ncs != cellSize) {
		Setup();			// Redo the setup stuff if stuff has changed
	}

	// Start with no cloud offset (might want to save/restore this)
	xOffset = 0.0f;
	yOffset = 0.0f;

	// Open the specified file for reading
	mapFile = CreateFile_Open( filename, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL );
	if (mapFile == INVALID_HANDLE_VALUE) {
		//char	string[80];
		//char	message[120];
		//PutErrorString( string );
		//sprintf( message, "%s:  Couldn't open weather map.", string );
		//MessageBox( NULL, message, "Proceeding without intial weather", MB_OK );
		//result = FALSE;
		// We can tolerate no weather
	} else {
		// Read the weather map file.
#if TODO			
		result = ReadFile( mapFile, map, w*h*sizeof(*map), &bytesRead, NULL );
		if ((!result) || (bytesRead != w*h*sizeof(*map))) {
			char	string[80];
			char	message[120];
			PutErrorString( string );
			sprintf( message, "%s:  Couldn'd read weather map.", string );
			MessageBox( NULL, message, "Proceeding without intial weather", MB_OK );
			result = FALSE;
				}
#endif
					throw new NotImplementedException();
		

		// Close the weather map file
		CloseHandle( mapFile );
	}

	mapType = type;

	return 1;
}

	public DWORD			TypeAt( uint r, uint c ){ if ((r>=h) || (c >=w)) return 0; 
											  else return map[r*w+c].Type; }
	public float			BaseAt( uint r, uint c ){ if ((r>=h) || (c >=w)) return 1.0f-SKY_ROOF_HEIGHT;
											  return map[r*w+c].BaseAltitude * WxAltScale + WxAltShift; }
	public float			ThicknessAt( uint r, uint c ){ if ((r>=h) || (c >=w)) return 1.0f;
											  return (map[r*w+c].Type-FIRST_OVC_TYPE) * WxThickScale; }
	public float			TopsAt( uint r, uint c ){ return BaseAt(r,c) + ThicknessAt(r,c); }

	public float	CellSize(   )				{ return cellSize; }
	public int		WorldToTile( float distance )	{ return FloatToInt32((float)floor(distance/cellSize)); }
	public float	TileToWorld( int rowORcol )		{ return (rowORcol * cellSize); }
	public virtual float TemperatureAt( Tpoint pos)	{ return 20.0f;}
	public float RainAt(uint r, uint c)
			{
    float thickness = ThicknessAt(r, c);
    float rainFactor = 0;
    if (fabs(thickness) > g_fMinCloudWeather) { // if the clouds are thick, then its raining.
	rainFactor = ((float)fabs(thickness) - g_fMinCloudWeather)/g_fCloudThicknessFactor;
	if (rainFactor > 1) rainFactor = 1;
    }
    return rainFactor;
}

		
	public float VisRangeAt(uint r, uint c)
			{
    float vr  = RainAt(r, c);
    vr = 1 - vr;
    return max(vr, TheTimeOfDay.GetMinVisibility());
}
		
	public bool  LightningAt(uint r, uint c){
    float thickness = ThicknessAt(r, c);
    float rainFactor = 0;
    if (fabs(thickness) > g_fMinCloudWeather) { // if the clouds are very thick, then its thunderstorms.
	rainFactor = ((float)fabs(thickness) - g_fMinCloudWeather)/g_fCloudThicknessFactor;
	if (rainFactor > 1) return true;
    }
    return false;
}


  
	protected static float WxAltShift;	// Added to cloud heights to keep them above the terrain
	protected CellState[]	map; 
	protected uint		w;
	protected uint		h;

	protected float		cellSize;

	protected int			mapType;		// User defined

	public int			rowShiftHistory;
	public int			colShiftHistory;

	public float		xOffset;
	public float		yOffset;
#endif
}

}

