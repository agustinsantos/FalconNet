using System;

namespace FalconNet.Graphics
{
	public static class Draw2DStatic
	{
		// types of 2d objects
		public const int  DRAW2D_AIR_EXPLOSION1 = 0;
		public const int  DRAW2D_SMALL_HIT_EXPLOSION = 1;
		public const int  DRAW2D_SMOKECLOUD1 = 2;
		public const int  DRAW2D_FLARE = 3;
		public const int  DRAW2D_AIR_EXPLOSION2 = 4;
		public const int  DRAW2D_SMOKERING = 5;
		public const int  DRAW2D_SMALL_CHEM_EXPLOSION = 6;
		public const int  DRAW2D_CHEM_EXPLOSION = 7;
		public const int  DRAW2D_SMALL_DEBRIS_EXPLOSION = 8;
		public const int  DRAW2D_DEBRIS_EXPLOSION = 9;
		public const int  DRAW2D_CLOUD5 = 10;
		public const int  DRAW2D_AIR_DUSTCLOUD = 11;
		public const int  DRAW2D_GUNSMOKE = 12;
		public const int  DRAW2D_SMOKECLOUD2 = 13;
		public const int  DRAW2D_EXPLSTAR_GLOW = 14;
		public const int  DRAW2D_MISSILE_GLOW = 15;
		public const int  DRAW2D_EXPLCIRC_GLOW = 16;
		public const int  DRAW2D_EXPLCROSS_GLOW = 17;
		public const int  DRAW2D_CRATER1 = 18;
		public const int  DRAW2D_CRATER2 = 19;
		public const int  DRAW2D_CRATER3 = 20;
		public const int  DRAW2D_FIRE = 21;
		public const int  DRAW2D_GROUND_STRIKE = 22;
		public const int  DRAW2D_WATER_STRIKE = 23;
		public const int  DRAW2D_HIT_EXPLOSION = 24;
		public const int  DRAW2D_SPARKS = 25;
		public const int  DRAW2D_ARTILLERY_EXPLOSION = 26;
		public const int  DRAW2D_SHOCK_RING = 27;
		public const int  DRAW2D_LONG_HANGING_SMOKE = 28;
		public const int  DRAW2D_SHAPED_FIRE_DEBRIS = 29;
		public const int  DRAW2D_WATER_CLOUD = 30;
		public const int  DRAW2D_DARK_DEBRIS = 31;
		public const int  DRAW2D_FIRE_DEBRIS = 32;
		public const int  DRAW2D_LIGHT_DEBRIS = 33;
		public const int  DRAW2D_EXPLCIRC_GLOW_FADE = 34;
		public const int  DRAW2D_SHOCK_RING_SMALL = 35;
		public const int  DRAW2D_FAST_FADING_SMOKE = 36;
		public const int  DRAW2D_LONG_HANGING_SMOKE2 = 37;
		public const int  DRAW2D_FLAME = 38;
		public const int  DRAW2D_FIRE_EXPAND = 39;
		public const int  DRAW2D_GROUND_DUSTCLOUD = 40;
		public const int  DRAW2D_FIRE_HOT = 41;
		public const int  DRAW2D_FIRE_MED = 42;
		public const int  DRAW2D_FIRE_COOL = 43;
		public const int  DRAW2D_FIRE1 = 44;
		public const int  DRAW2D_FIRE2 = 45;
		public const int  DRAW2D_FIRE3 = 46;
		public const int  DRAW2D_FIRE4 = 47;
		public const int  DRAW2D_FIRE5 = 48;
		public const int  DRAW2D_FIRE6 = 49;
		public const int  DRAW2D_FIRESMOKE = 50;
		public const int  DRAW2D_TRAILSMOKE = 51;
		public const int  DRAW2D_TRAILDUST = 52;
		public const int  DRAW2D_FIRE7 = 53;
		public const int  DRAW2D_BLUE_CLOUD = 54;
		public const int  DRAW2D_STEAM_CLOUD = 55;
		public const int  DRAW2D_GROUND_FLASH = 56;
		public const int  DRAW2D_GROUND_GLOW = 57;
		public const int  DRAW2D_MISSILE_GROUND_GLOW = 58;
		public const int  DRAW2D_BIG_SMOKE1 = 59;
		public const int  DRAW2D_BIG_SMOKE2 = 60;
		public const int  DRAW2D_BIG_DUST = 61;
		public const int  DRAW2D_INCENDIARY_EXPLOSION = 62;
		public const int  DRAW2D_WATERWAKE = 63;
		public const int DRAW2D_MAXTYPES = 64;
	}

	[Flags]
				public enum Type2D
	{
		ANIM_STOP			=0x00000001,		// stop when end reached
		ANIM_HOLD_LAST		=0x00000002,		// hold last frame
		ANIM_LOOP			=0x00000004,		// loop over again
		ANIM_LOOP_PING		=0x00000008,		// fwd-back loop
		FADE_LAST			=0x00000010,		// fade only on last frame
		FADE_START			=0x00000020,		// start fade immediately
		HAS_ORIENTATION		=0x00000040,		// uses rot matrix
		ALPHA_BRIGHTEN		=0x00000080,		// use brighten blending
		USES_BB_MATRIX		=0x00000100,		// use billboard (roll) matrix
		USES_TREE_MATRIX	=0x00000200,		// use tree (roll+pitch) matrix
		GLOW_SPHERE			=0x00000400,		// special type
		GLOW_RAND_POINTS	=0x00000800,		// randomize points of star
		ANIM_HALF_RATE		=0x00001000,		// animate 8 frames/sec
		FIRE_SCATTER_PLOT	=0x00002000,		// kind of a particle effect
		SMOKE_SCATTER_PLOT	=0x00004000,		// kind of a particle effect
		TEXTURED_CONE		=0x00008000,		// "shaped" effect
		EXPLODE_SCATTER_PLOT	=0x00010000,		// kind of a particle effect
		SEQ_SCATTER_ANIM	=0x00020000,		// sequence animation frames in scatter plots
		ANIM_NO_CLAMP		=0x00040000,		// don't clamp maximum texid -- used in scatter
		GOURAUD_TRI			=0x00080000,		// just a solid color tri
		ALPHA_DAYLIGHT		=0x00100000,		// do alpha in daylight
		DO_FIVE_POINTS		=0x00200000,		// do dark alpha in center
		NO_RANDOM_BLEND		=0x00400000,		// don't randomly blend rgb's
		ALPHA_PER_TEXEL		=0x00800000,		// use APL texture set
		NO_FIVE_POINTS		=0x01000000,		// disable 5 point
		RAND_START_FRAME	=0x02000000,		// randomize scatter start
		GROUND_GLOW			=0x04000000		// align with ground
	}
// table entry to control the animation seq for different types of objects
	public struct TYPES2D
	{
		Type2D flags;			// controls sequencing

		float initAlpha;	// starting alpha value
		float fadeRate;		// rate of alpha fade/ms (i.e. 1 sec total fade = .001)
		int texId;		// array of texure Ids in sequence
		int startTexture;	// staring texture in sheet
		int numTextures;	// number of textures in sequence
		float expandRate;	// rate radius explands in ft/sec
		Tpoint glowVerts;	// glowing "sphere" type objs
		int numGlowVerts;	// number of verts
		float maxExpand;	// maximum expand size
	}

	public class Drawable2D :   DrawableObject
	{

#if USE_SH_POOLS
  public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { ShiAssert( size == sizeof(Drawable2D) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(Drawable2D), 400, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif

  
		public Drawable2D (int type, float scale, Tpoint *p)
;
		public Drawable2D (int type, float scale, Tpoint *p, Trotation *rot)
;
		public Drawable2D (int type, float scale, Tpoint *p, int nVerts, Tpoint *verts, Tpoint *uvs);
		// public virtual ~Drawable2D();

		public virtual void Draw (RenderOTW renderer, int LOD);

		public virtual void Update (Tpoint pos, Trotation rot);

		public void SetPosition (Tpoint p);

		public void SetStartTime (DWORD start, DWORD now);

		public void SetScale2D (float s)
		{
			scale2d = s;
		}

		public void SetRadius (float r)
		{
			initRadius = radius = realRadius = r;
		}

		public void SetAlpha (float a)
		{
			initAlpha = alpha = a;
		}

		public float GetScale2D ()
		{
			return scale2d;
		}

		public float GetAlphaTimeToLive ();

		public static void SetLOD (float LOD);

		public static void SetGreenMode (BOOL mode);

		public static void DrawGlowSphere (RenderOTW renderer, Tpoint pos, float radius, float alpha);
  
		protected int	type;				// type
		protected float alpha;			// global alpha value for the object
		protected float initAlpha;		// global alpha value for the object
		protected float initRadius;		// radius value for the object
		protected int curFrame;			// current frame we're showing
		protected int curSFrame;			// current scatter frame we're showing
		protected int firstFrame;			// current frame we're showing
		protected int curBFrame;			// current base frame we're showing
		protected uint startTime; // in ms
		protected uint alphaStartTime; // in ms
		protected uint expandStartTime; // in ms
		protected uint startSFrame; // in ms
		protected TYPES2D *typeData;		// data for anim type
		protected BOOL startFade;			// start fading?
		protected BOOL explicitStartTime;	// caller set start time
		protected Texture *curTex;		// last valid texture we got
		protected Tpoint[] oVerts = new Tpoint[4];		// object space verts
		protected Tpoint[] uvCoords = new Tpoint[4];		// x = u, y = v
		protected int numObjVerts;		// number of obj space verts ( = 3 or 4 )
		protected float scale2d;			// scale of the object
		protected float realRadius;		// real radius of poly, we may want radius
		// to be larger for better sorting

		// TODO:  Move this to a subclass???
		protected BOOL hasOrientation;	// uses orientation matrix
		protected Trotation orientation;	// orientation

		protected void DrawGlowSphere (RenderOTW renderer, int LOD);

		protected void DrawGouraudTri (RenderOTW renderer, int LOD);

		protected void DrawTexturedCone (RenderOTW renderer, int LOD);

		protected int GetAnimFrame (int dT, DWORD start);

		protected void ScatterPlot (RenderOTW renderer);

		protected void APLScatterPlot (RenderOTW renderer);

		// Handle time of day notifications
		protected static void TimeUpdateCallback (void *unused);
  
		public static void SetupTexturesOnDevice (DXContext *rc);

		public static void ReleaseTexturesOnDevice (DXContext *rc);

		// external proto for lens flare function
		public static void Draw2DLensFlare (RenderOTW renderer);

		public static void Draw2DSunGlowEffect (RenderOTW renderer, Tpoint *cntr, float dist, float alpha);
	} 

}

