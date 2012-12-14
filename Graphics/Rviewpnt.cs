using System;

namespace FalconNet.Graphics
{
/***************************************************************************\
    RViewPnt.h
    Scott Randolph
    August 20, 1996

    Manages information about a world space location and keeps the weather,
	terrain, and object lists in synch.
\***************************************************************************/
	public struct ObjectListRecord
	{
		public ObjectDisplayList	displayList;
		public float				Ztop;
	};

	public class RViewPoint :  TViewPoint
	{
  
		public RViewPoint ()
		{
			objectLists = NULL;
			nObjectLists = 0;
			weather = NULL;
		}
		//public  ~RViewPoint()	{ ShiAssert( !IsReady() ); };

		public void	Setup (float gndRange, int maxDetail, int minDetail, float wxRange);

		public void	Cleanup ();

		public void	SetGroundRange (float gndRange, int maxDetail, int minDetail);

		public void	SetWeatherRange (float wxRange);

		public BOOL	IsReady ()
		{
			return (objectLists != NULL) && (TViewPoint::IsReady ());
		}

		// Add/remove drawable objects from this viewpoint's display list
		public void	InsertObject (DrawableObject object_);

		public void	RemoveObject (DrawableObject object_);

		// Query terrain and weather properties for the area arround this viewpoint
		public float	GetTerrainFloor ()
		{
			return terrainFloor;
		}

		public float	GetTerrainCeiling ()
		{
			return terrainCeiling;
		}

		public float	GetCloudBase ()
		{
			return cloudBase;
		}

		public float	GetCloudTops ()
		{
			return cloudTops;
		}

		public float	GetWeatherRange ()
		{
			return weather ? weather->GetMaxRange () : 0.0f;
		}

		public float	GetRainFactor ()
		{
			return weather ? weather->GetRainFactor () : 0.0f;
		}

		public float	GetVisibility ()
		{
			return weather ? weather->GetVisibility () : 1.0f;
		}

		public bool	GetLightning ()
		{
			return weather ? weather->HasLightning () : false;
		}

		public float	GetLocalCloudTops ()
		{
			return weather ? weather->GetLocalCloudTops () : 0.0f;
		}

		// Cloud properties at this viewpoint's exact position in space
		public float	CloudOpacity ()
		{
			return cloudOpacity;
		}

		public Tcolor	CloudColor ()
		{
			return cloudColor;
		}

		// Ask if a line of sight exists between two points with respect to both clouds and terrain
		public float	CompositLineOfSight (Tpoint *p1, Tpoint *p2);

		public int		CloudLineOfSight (Tpoint *p1, Tpoint *p2);

		public void	Update (Tpoint *pos);

		public void	UpdateMoon ();

		public void	ResetObjectTraversal ();

		public int		GetContainingList (float zValue);

		public ObjectDisplayList	ObjectsInTerrain ()
		{
			return &objectLists [0].displayList;
		}

		public ObjectDisplayList	ObjectsBelowClouds ()
		{
			return &objectLists [1].displayList;
		}

		public ObjectDisplayList	Clouds ()
		{
			return &cloudList;
		}

		public ObjectDisplayList	ObjectsInClouds ()
		{
			return &objectLists [2].displayList;
		}

		public ObjectDisplayList	ObjectsAboveClouds ()
		{
			return &objectLists [3].displayList;
		}

		public ObjectDisplayList	ObjectsAboveRoof ()
		{
			return &objectLists [4].displayList;
		}

		public Texture		SunTexture, GreenSunTexture;
		public Texture		MoonTexture, GreenMoonTexture, OriginalMoonTexture;
		protected int			lastDay;				// Used to decide when the moon needs updating

		protected  LocalWeather	*weather;
		protected int					nObjectLists;
		protected ObjectListRecord	*objectLists;
		protected ObjectDisplayList	cloudList;
		protected float				cloudOpacity;	// 0.0 for no effect, 1.0 when inCloud is TRUE
		protected Tcolor				cloudColor;
		protected float	terrainFloor;
		protected float	terrainCeiling;
		protected float	cloudBase;
		protected float	cloudTops;
		protected float	roofHeight;

		protected void		SetupTextures ();

		protected void		ReleaseTextures ();

		protected static void	TimeUpdateCallback (object self);
	}
}

