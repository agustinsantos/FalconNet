using System;
using System.Diagnostics;
using FalconNet.Common;
using DWORD = System.UInt32;
using FalconNet.Common.Graphics;

namespace FalconNet.Graphics
{
    public static class Draw2DStatic
    {
        // types of 2d objects
        public const int DRAW2D_AIR_EXPLOSION1 = 0;
        public const int DRAW2D_SMALL_HIT_EXPLOSION = 1;
        public const int DRAW2D_SMOKECLOUD1 = 2;
        public const int DRAW2D_FLARE = 3;
        public const int DRAW2D_AIR_EXPLOSION2 = 4;
        public const int DRAW2D_SMOKERING = 5;
        public const int DRAW2D_SMALL_CHEM_EXPLOSION = 6;
        public const int DRAW2D_CHEM_EXPLOSION = 7;
        public const int DRAW2D_SMALL_DEBRIS_EXPLOSION = 8;
        public const int DRAW2D_DEBRIS_EXPLOSION = 9;
        public const int DRAW2D_CLOUD5 = 10;
        public const int DRAW2D_AIR_DUSTCLOUD = 11;
        public const int DRAW2D_GUNSMOKE = 12;
        public const int DRAW2D_SMOKECLOUD2 = 13;
        public const int DRAW2D_EXPLSTAR_GLOW = 14;
        public const int DRAW2D_MISSILE_GLOW = 15;
        public const int DRAW2D_EXPLCIRC_GLOW = 16;
        public const int DRAW2D_EXPLCROSS_GLOW = 17;
        public const int DRAW2D_CRATER1 = 18;
        public const int DRAW2D_CRATER2 = 19;
        public const int DRAW2D_CRATER3 = 20;
        public const int DRAW2D_FIRE = 21;
        public const int DRAW2D_GROUND_STRIKE = 22;
        public const int DRAW2D_WATER_STRIKE = 23;
        public const int DRAW2D_HIT_EXPLOSION = 24;
        public const int DRAW2D_SPARKS = 25;
        public const int DRAW2D_ARTILLERY_EXPLOSION = 26;
        public const int DRAW2D_SHOCK_RING = 27;
        public const int DRAW2D_LONG_HANGING_SMOKE = 28;
        public const int DRAW2D_SHAPED_FIRE_DEBRIS = 29;
        public const int DRAW2D_WATER_CLOUD = 30;
        public const int DRAW2D_DARK_DEBRIS = 31;
        public const int DRAW2D_FIRE_DEBRIS = 32;
        public const int DRAW2D_LIGHT_DEBRIS = 33;
        public const int DRAW2D_EXPLCIRC_GLOW_FADE = 34;
        public const int DRAW2D_SHOCK_RING_SMALL = 35;
        public const int DRAW2D_FAST_FADING_SMOKE = 36;
        public const int DRAW2D_LONG_HANGING_SMOKE2 = 37;
        public const int DRAW2D_FLAME = 38;
        public const int DRAW2D_FIRE_EXPAND = 39;
        public const int DRAW2D_GROUND_DUSTCLOUD = 40;
        public const int DRAW2D_FIRE_HOT = 41;
        public const int DRAW2D_FIRE_MED = 42;
        public const int DRAW2D_FIRE_COOL = 43;
        public const int DRAW2D_FIRE1 = 44;
        public const int DRAW2D_FIRE2 = 45;
        public const int DRAW2D_FIRE3 = 46;
        public const int DRAW2D_FIRE4 = 47;
        public const int DRAW2D_FIRE5 = 48;
        public const int DRAW2D_FIRE6 = 49;
        public const int DRAW2D_FIRESMOKE = 50;
        public const int DRAW2D_TRAILSMOKE = 51;
        public const int DRAW2D_TRAILDUST = 52;
        public const int DRAW2D_FIRE7 = 53;
        public const int DRAW2D_BLUE_CLOUD = 54;
        public const int DRAW2D_STEAM_CLOUD = 55;
        public const int DRAW2D_GROUND_FLASH = 56;
        public const int DRAW2D_GROUND_GLOW = 57;
        public const int DRAW2D_MISSILE_GROUND_GLOW = 58;
        public const int DRAW2D_BIG_SMOKE1 = 59;
        public const int DRAW2D_BIG_SMOKE2 = 60;
        public const int DRAW2D_BIG_DUST = 61;
        public const int DRAW2D_INCENDIARY_EXPLOSION = 62;
        public const int DRAW2D_WATERWAKE = 63;
        public const int DRAW2D_MAXTYPES = 64;
    }

    [Flags]
    public enum Type2D
    {
        ANIM_STOP = 0x00000001,		// stop when end reached
        ANIM_HOLD_LAST = 0x00000002,		// hold last frame
        ANIM_LOOP = 0x00000004,		// loop over again
        ANIM_LOOP_PING = 0x00000008,		// fwd-back loop
        FADE_LAST = 0x00000010,		// fade only on last frame
        FADE_START = 0x00000020,		// start fade immediately
        HAS_ORIENTATION = 0x00000040,		// uses rot matrix
        ALPHA_BRIGHTEN = 0x00000080,		// use brighten blending
        USES_BB_MATRIX = 0x00000100,		// use billboard (roll) matrix
        USES_TREE_MATRIX = 0x00000200,		// use tree (roll+pitch) matrix
        GLOW_SPHERE = 0x00000400,		// special type
        GLOW_RAND_POINTS = 0x00000800,		// randomize points of star
        ANIM_HALF_RATE = 0x00001000,		// animate 8 frames/sec
        FIRE_SCATTER_PLOT = 0x00002000,		// kind of a particle effect
        SMOKE_SCATTER_PLOT = 0x00004000,		// kind of a particle effect
        TEXTURED_CONE = 0x00008000,		// "shaped" effect
        EXPLODE_SCATTER_PLOT = 0x00010000,		// kind of a particle effect
        SEQ_SCATTER_ANIM = 0x00020000,		// sequence animation frames in scatter plots
        ANIM_NO_CLAMP = 0x00040000,		// don't clamp maximum texid -- used in scatter
        GOURAUD_TRI = 0x00080000,		// just a solid color tri
        ALPHA_DAYLIGHT = 0x00100000,		// do alpha in daylight
        DO_FIVE_POINTS = 0x00200000,		// do dark alpha in center
        NO_RANDOM_BLEND = 0x00400000,		// don't randomly blend rgb's
        ALPHA_PER_TEXEL = 0x00800000,		// use APL texture set
        NO_FIVE_POINTS = 0x01000000,		// disable 5 point
        RAND_START_FRAME = 0x02000000,		// randomize scatter start
        GROUND_GLOW = 0x04000000		// align with ground
    }
    // table entry to control the animation seq for different types of objects
    public struct TYPES2D
    {
        public Type2D flags;			// controls sequencing

        public float initAlpha;	// starting alpha value
        public float fadeRate;		// rate of alpha fade/ms (i.e. 1 sec total fade = .001)
        public int? texId;		// array of texure Ids in sequence
        public int startTexture;	// staring texture in sheet
        public int numTextures;	// number of textures in sequence
        public float expandRate;	// rate radius explands in ft/sec
        public Tpoint[] glowVerts;	// glowing "sphere" type objs
        public int numGlowVerts;	// number of verts
        public float maxExpand;	// maximum expand size
    }

    public class Drawable2D : DrawableObject
    {

#if USE_SH_POOLS
  public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { Debug.Assert( size == sizeof(Drawable2D) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(Drawable2D), 400, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif

        
        public Drawable2D(int type2d, float scale, Tpoint p)
            : base(scale)
        {
#if TODO
            // sanity check
            Debug.Assert(type2d < DRAW2D_MAXTYPES && type2d >= 0);

            // save type
            type = type2d;

            // get typeData ptr
            typeData = gTypeTable[type2d];

            // init curframe
            curFrame = -1;
            curSFrame = 0;
            curBFrame = 0;
            if ((typeData.flags & Type2D.RAND_START_FRAME) != 0)
                startSFrame = ran() % typeData.numGlowVerts;
            else
                startSFrame = 0;

            // set position -- world coords
            position = *p;

            // no transparency to start
            initAlpha = alpha = typeData.initAlpha;

            // scale is equivalent to radius
            realRadius = initRadius = radius = scale;

            // init misc...
            startFade = false;
            hasOrientation = false;
            numObjVerts = 0;
            startTime = 0;
            explicitStartTime = false;

            scale2d = 1.0f;

            // now update the ref count to the textures and load them
            // if not already done
#if ! PRELOAD_TEX
            // if ( typeData.glTexId )
            // 	glInsertTexture( *typeData.glTexId, 1 );
#endif
#endif
            throw new NotImplementedException();
            }

        public Drawable2D(int type2d, float scale, Tpoint p, Trotation rot)
            : base(scale)
        {
            // save type
            type = type2d;

            // sanity check
            Debug.Assert(type2d < DRAW2D_MAXTYPES && type2d >= 0);

            // get typeData ptr
            typeData = gTypeTable[type2d];


            // init curframe
            curFrame = -1;
            curSFrame = 0;
            curBFrame = 0;
            if ((typeData.flags &  Type2D.RAND_START_FRAME) != 0)
                startSFrame = (uint)(rand() % typeData.numGlowVerts);
            else
                startSFrame = 0;

            // set position -- world coords
            position = p;

            // no transparency to start
            initAlpha = alpha = typeData.initAlpha;

            // scale is equivalent to radius
            realRadius = initRadius = radius = scale;

            // init misc...
            startFade = false;
            numObjVerts = 0;
            startTime = 0;
            explicitStartTime = false;
            scale2d = 1.0f;

            // now update the ref count to the textures and load them
            // if not already done
#if ! PRELOAD_TEX
            // if ( typeData.glTexId )
            // 	glInsertTexture( *typeData.glTexId, 1 );
#endif

            orientation = rot;
            hasOrientation = true;
        }

        public Drawable2D(int type2d, float scale, Tpoint p, int nVerts, Tpoint[] verts, Tpoint[] uvs)
            : base(scale)
        {
            int i;

            // save type
            type = type2d;

            // sanity check
            Debug.Assert(type2d < DRAW2D_MAXTYPES && type2d >= 0);

            // at the moment we only allow 4 verts
            Debug.Assert(nVerts == 4);

            // get typeData ptr
            typeData = gTypeTable[type2d];

            // init curframe
            curFrame = -1;
            curSFrame = 0;
            curBFrame = 0;
            if ((typeData.flags & Type2D.RAND_START_FRAME) != 0)
                startSFrame = (uint)(rand() % typeData.numGlowVerts);
            else
                startSFrame = 0;

            // set position -- world coords
            position = p;

            // no transparency to start
            initAlpha = alpha = typeData.initAlpha;

            // scale is equivalent to radius
            realRadius = initRadius = radius = scale;

            // init misc...
            startFade = false;
            numObjVerts = 0;
            hasOrientation = false;
            numObjVerts = nVerts;
            startTime = 0;
            explicitStartTime = false;
            scale2d = 1.0f;

            // setup the verts
            for (i = 0; i < numObjVerts; i++)
            {
                oVerts[i] = verts[i];
                uvCoords[i] = uvs[i];
            }

            // now update the ref count to the textures and load them
            // if not already done
#if ! PRELOAD_TEX
            // if ( typeData.glTexId )
            // 	glInsertTexture( *typeData.glTexId, 1 );
#endif

        }
        // public virtual ~Drawable2D();

        public override void Draw(RenderOTW renderer, int LOD)
        { throw new NotImplementedException(); }

        public virtual void Update(Tpoint pos, Trotation rot)
        {

            // Update the location of this object
            position = pos;
            orientation = rot;

        }

        public void SetPosition(Tpoint p)
        {
            // Update the location of this object
            position = p;

        }

        public void SetStartTime(DWORD start, DWORD now)
        { throw new NotImplementedException(); }

        public void SetScale2D(float s)
        {
            scale2d = s;
        }

        public void SetRadius(float r)
        {
            initRadius = radius = realRadius = r;
        }

        public void SetAlpha(float a)
        {
            initAlpha = alpha = a;
        }

        public float GetScale2D()
        {
            return scale2d;
        }

        public float GetAlphaTimeToLive()
        {
            float rate;

            // no fading
            if (typeData.fadeRate == 0.0f)
                return 0.0f;

            rate = initAlpha / (1000.0f * typeData.fadeRate);

            if (type == Draw2DStatic.DRAW2D_FIRE1)
                rate += 3.0f;

            return rate;
        }

        public static void SetLOD(float LOD)
        { throw new NotImplementedException(); }

        public static void SetGreenMode(bool mode)
        { throw new NotImplementedException(); }

        public static void DrawGlowSphere(RenderOTW renderer, Tpoint pos, float radius, float alpha)
        { throw new NotImplementedException(); }

        protected int type;				// type
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
        protected TYPES2D typeData;		// data for anim type
        protected bool startFade;			// start fading?
        protected bool explicitStartTime;	// caller set start heading
        protected Texture curTex;		// last valid texture we got
        protected Tpoint[] oVerts = new Tpoint[4];		// object space verts
        protected Tpoint[] uvCoords = new Tpoint[4];		// x = u, y = v
        protected int numObjVerts;		// number of obj space verts ( = 3 or 4 )
        protected float scale2d;			// scale of the object
        protected float realRadius;		// real radius of poly, we may want radius
        // to be larger for better sorting

        // TODO:  Move this to a subclass???
        protected bool hasOrientation;	// uses orientation matrix
        protected Trotation orientation;	// orientation

        protected void DrawGlowSphere(RenderOTW renderer, int LOD)
        { throw new NotImplementedException(); }

        protected void DrawGouraudTri(RenderOTW renderer, int LOD)
        { throw new NotImplementedException(); }

        protected void DrawTexturedCone(RenderOTW renderer, int LOD)
        { throw new NotImplementedException(); }

        protected int GetAnimFrame(int dT, DWORD start)
        { throw new NotImplementedException(); }

        protected void ScatterPlot(RenderOTW renderer)
        { throw new NotImplementedException(); }

        protected void APLScatterPlot(RenderOTW renderer)
        { throw new NotImplementedException(); }

        // Handle heading of day notifications
        protected void TimeUpdateCallback(object unused)
        {
#if TODO
            Tcolor light;

            // Get the light level from the time of day manager
            lightLevel = CTimeOfDay.TheTimeOfDay.GetLightLevel();

            CTimeOfDay.TheTimeOfDay.GetTextureLightingColor(ref gLight);

            // Get the light level from the time of day manager
            CTimeOfDay.TheTimeOfDay.GetTextureLightingColor(ref light);

            // right now we're assuming all APL have the same palette
            gAplTextures[6].palette.LightTexturePalette(ref light);
            gAplTextures[7].palette.LightTexturePalette(ref light);
            gAplTextures[8].palette.LightTexturePalette(ref light);
            gAplTextures[9].palette.LightTexturePalette(ref light);
            gAplTextures[10].palette.LightTexturePalette(ref light);
            gAplTexturesGreen[0].palette.LightTexturePalette(ref light);

            // now light the palette used for all our textures
            // TODO: the range is incorrect at the moment -- until the artists
            // redo the palette
            gGlobTextures[0].palette.LightTexturePaletteRange(&light, 151, 255);

#endif
            throw new NotImplementedException();
        }


        public static void SetupTexturesOnDevice(DXContext rc)
        { throw new NotImplementedException(); }

        public static void ReleaseTexturesOnDevice(DXContext rc)
        { throw new NotImplementedException(); }

        // external proto for lens flare function
        public static void Draw2DLensFlare(RenderOTW renderer)
        { throw new NotImplementedException(); }

        public static void Draw2DSunGlowEffect(RenderOTW renderer, Tpoint cntr, float dist, float alpha)
        { throw new NotImplementedException(); }


        // define for preloading all animation textures
        // comment out to use on-the-fly textures
        // #define PRELOAD_TEX

        private static float sLOD = 1.0f;
        private int sGreenMode = 0;
        private static float sMaxScreenRes = 1.0f;

        private static Tcolor gLight;

        private const int NUM_TEX_SHEETS = 32;
        private const int NUM_TEXTURES_USED = 7;
        private const float TEX_UV_DIM = 0.246f;
        private Texture[] gGlobTextures = new Texture[NUM_TEX_SHEETS];

        // for alpha per texel textures
        private const int NUM_APL_TEXTURES = 11;
        private Texture[] gAplTextures = new Texture[NUM_APL_TEXTURES];
        private Texture[] gAplTexturesGreen = new Texture[NUM_APL_TEXTURES];

        // scatter plotting vertices
        /*
        #define NUM_SMOKE_SCATTER_FRAMES		5
        #define NUM_SMOKE_SCATTER_POINTS		6
        #define NUM_FIRE_SCATTER_FRAMES		8
        #define NUM_FIRE_SCATTER_POINTS		12
        #define NUM_EXPLODE_SCATTER_FRAMES		1
        #define NUM_EXPLODE_SCATTER_POINTS		10
        */
        private const int NUM_SMOKE_SCATTER_FRAMES = 1;
        private const int NUM_SMOKE_SCATTER_POINTS = 1;
        private const int NUM_FIRE_SCATTER_FRAMES = 8;
        private const int NUM_FIRE_SCATTER_POINTS = 12;
        private const int NUM_EXPLODE_SCATTER_FRAMES = 1;
        private const int NUM_EXPLODE_SCATTER_POINTS = 10;

        private readonly float SCATTER_ZMAX = (5000.0f + 15000.0f * sLOD);
        private Tpoint[,] gFireScatterPoints = new Tpoint[NUM_FIRE_SCATTER_FRAMES, NUM_FIRE_SCATTER_POINTS];
        private Tpoint[,] gSmokeScatterPoints = new Tpoint[NUM_SMOKE_SCATTER_FRAMES, NUM_SMOKE_SCATTER_POINTS];
        private Tpoint[,] gExplodeScatterPoints = new Tpoint[NUM_EXPLODE_SCATTER_FRAMES, NUM_EXPLODE_SCATTER_POINTS];

        internal struct UV
        {
            public float u;
            public float v;
        }

        // each sheet is a 256x256 texture containing 16 64x64 textures
        // for animation.  This is a lookup table so we can get the upper left
        // corner uv based on anim frame
        private static UV[] gTexUV = new UV[]
                        {
	                        new UV { u=0.002f, v=0.002f },
	                        new UV { u=0.252f, v=0.002f },
	                        new UV { u=0.502f, v=0.002f },
	                        new UV { u=0.752f, v=0.002f },
	                        new UV { u=0.002f, v=0.252f },
	                        new UV { u=0.252f, v=0.252f },
	                        new UV { u=0.502f, v=0.252f },
	                        new UV { u=0.752f, v=0.252f },
	                        new UV { u=0.002f, v=0.502f },
	                        new UV { u=0.252f, v=0.502f },
	                        new UV { u=0.502f, v=0.502f },
	                        new UV { u=0.752f, v=0.502f },
	                        new UV { u=0.002f, v=0.752f },
	                        new UV { u=0.252f, v=0.752f },
	                        new UV { u=0.502f, v=0.752f },
	                        new UV { u=0.752f, v=0.752f },
                        };


        // current light level
        private static float lightLevel = 1.0f;

        private static Tpoint[] glowCrossVerts = new Tpoint[]
                                        {
	                                        //		X				Y				Z
	                                        new Tpoint{	    x=0.0f,			y=0.0f,			z=-14.0f },
	                                        new Tpoint{		x=0.0f,			y=1.0f,			z=-2.0f },
	                                        new Tpoint{		x=0.0f,			y=2.0f,			z=-1.0f },
	                                        new Tpoint{		x=0.0f,			y=14.0f,		z=	 0.0f },
	                                        new Tpoint{		x=0.0f,			y=2.0f,			 z=1.0f },
	                                        new Tpoint{		x=0.0f,			y=1.0f,			 z=2.0f },
	                                        new Tpoint{		x=0.0f,			y=0.0f,			z= 14.0f },
	                                        new Tpoint{		x=0.0f,		   y=-1.0f,			 z=2.0f },
	                                        new Tpoint{		x=0.0f,		   y=-2.0f,			 z=1.0f },
	                                        new Tpoint{		x=0.0f,		   y=-14.0f,		z=	 0.0f },
	                                        new Tpoint{		x=0.0f,		   y=-2.0f,			z=-1.0f },
	                                        new Tpoint{		x=0.0f,		   y=-1.0f,			z=-2.0f },
                                        };
        private static int numGlowCrossVerts = glowCrossVerts.Length;

        private static Tpoint[] glowCircleVerts;
        private static int numGlowCircleVerts = glowCircleVerts.Length;

        private static Tpoint[] glowSquareVerts = new Tpoint[4];
        private static int numGlowSquareVerts = glowSquareVerts.Length;

        private static Tpoint[] glowStarVerts = new Tpoint[20];
        private static int numGlowStarVerts = glowStarVerts.Length;

        private static Tpoint[] coneDim = new Tpoint[] { new Tpoint () { x=4.5f, y=2.0f, z=2.0f }};

        // lens flare stuff
        public const int NUM_FLARE_CIRCLES = 10;
        private static Tpoint[] lensFlareVerts = new Tpoint[16];
        private static int numLensFlareVerts = lensFlareVerts.Length;
        private static float[] lensAlphas = new float[]
                                {
	                                0.28f,
	                                0.37f,
	                                0.20f,
	                                0.24f,
	                                0.28f,
	                                0.30f,
	                                0.26f,
	                                0.35f,
	                                0.25f,
	                                0.22f,
                                };
        private static float[] lensRadius = new float[]
                                {
	                                2.02f,
	                                3.06f,
	                                1.05f,
	                                0.45f,
	                                0.75f,
	                                1.38f,
	                                0.87f,
	                                6.03f,
	                                7.09f,
	                                10.78f,
                                };

        private static float[] lensDist = new float[]
                            {
	                            -12.00f,
	                            -32.00f,
	                            -70.00f,
	                            -121.00f,
	                            -181.00f,
	                            70.00f,
	                            125.00f,
	                            10.00f,
	                            165.00f,
	                            105.00f,
                            };

        private static Tcolor[] lensRGB = new Tcolor[]
                            {
	                            new Tcolor {	r=1.00f,		g=1.00f, 		b=0.60f 	},
	                            new Tcolor {	r=1.00f,		g=0.80f, 		b=1.00f 	},
	                            new Tcolor {	r=0.90f,		g=1.00f, 		b=0.80f 	},
	                            new Tcolor {	r=1.00f,		g=0.80f, 		b=0.80f 	},
	                            new Tcolor {	r=1.00f,		g=0.90f, 		b=1.00f 	},

	                            new Tcolor {	r=0.80f,		g=0.95f, 		b=0.60f 	},
	                            new Tcolor {	r=1.00f,		g=0.40f, 		b=0.80f 	},
	                            new Tcolor {	r=1.00f,		g=1.00f, 		b=0.50f 	},
	                            new Tcolor {	r=1.00f,		g=0.20f, 		b=1.00f 	},
	                            new Tcolor {	r=0.70f,		g=0.70f, 		b=1.00f 	},
                            };

        private static Tcolor[] lensCenterRGB = new Tcolor[]
                            {
	                            new Tcolor {	r=0.30f,		g=0.30f, 		b=1.00f 	},
	                            new Tcolor {	r=0.60f,		g=0.20f, 		b=0.30f 	},
	                            new Tcolor {	r=0.20f,		g=0.20f, 		b=1.00f 	},
	                            new Tcolor {	r=0.70f,		g=0.30f, 		b=1.00f 	},
	                            new Tcolor {	r=0.20f,		g=0.20f, 		b=0.80f 	},

	                            new Tcolor {	r=1.00f,		g=0.40f, 		b=0.90f 	},
	                            new Tcolor {	r=0.70f,		g=0.70f, 		b=1.00f 	},
	                            new Tcolor {	r=0.30f,		g=1.00f, 		b=0.90f 	},
	                            new Tcolor {	r=0.40f,		g=0.90f, 		b=1.00f 	},
	                            new Tcolor {	r=0.10f,		g=1.00f, 		b=0.60f 	},
                            };

        private static TYPES2D[] gTypeTable = new TYPES2D[] 
{
	// Air explosion
	new TYPES2D {
		flags= Type2D.ANIM_STOP | Type2D.FADE_START | Type2D.DO_FIVE_POINTS,			// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 1,									// texture id sequence
		startTexture= 0,									// startTexture
		numTextures= 16,									// numTextures
		expandRate= 20.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Small Hit Explosion
	new TYPES2D{
		flags= Type2D.ANIM_HALF_RATE | Type2D.ANIM_STOP | Type2D.DO_FIVE_POINTS,							// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 0,									// texture id sequence
		startTexture= 0,									// startTexture
		numTextures= 12,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Smoke
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= .00005f,								// fadeRate
		texId= 10,
		startTexture= 12,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 50.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,			// numGlowVerts
		maxExpand= 300.0f,							// maxExpand
	},
	// Flare
	new TYPES2D {
		flags= Type2D.ANIM_LOOP ,	// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 4,
		startTexture= 14,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Air Explosion 2
	new TYPES2D {
		flags= Type2D.ANIM_HALF_RATE | Type2D.ANIM_STOP | Type2D.EXPLODE_SCATTER_PLOT | Type2D.ANIM_NO_CLAMP,							// flags
		initAlpha= 0.8f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 1,
		startTexture= 0,									// startTexture
		numTextures= 16,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_EXPLODE_SCATTER_FRAMES,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Smoke Ring
	new TYPES2D {
		flags= Type2D.FADE_START | Type2D.ANIM_HOLD_LAST ,	// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0008f,							// fadeRate
		texId= 3,
		startTexture= 15,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 480.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Small Chem Explosion
	new TYPES2D {
		flags= Type2D.ANIM_HALF_RATE | Type2D.ANIM_STOP | Type2D.DO_FIVE_POINTS,							// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 6,									// texture id sequence
		startTexture= 0,									// startTexture
		numTextures= 16,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Chem Explosion
	new TYPES2D {
		flags= Type2D.ANIM_HALF_RATE | Type2D.ANIM_STOP | Type2D.EXPLODE_SCATTER_PLOT | Type2D.ANIM_NO_CLAMP,							// flags
		initAlpha= 0.8f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 6,
		startTexture= 0,									// startTexture
		numTextures= 16,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_EXPLODE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Small Debris Explosion
	new TYPES2D {
		flags= Type2D.ANIM_HALF_RATE | Type2D.ANIM_STOP | Type2D.DO_FIVE_POINTS,							// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 4,									// texture id sequence
		startTexture= 0,									// startTexture
		numTextures= 14,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Debris Explosion
	new TYPES2D {
		flags= Type2D.ANIM_HALF_RATE | Type2D.ANIM_STOP | Type2D.EXPLODE_SCATTER_PLOT | Type2D.ANIM_NO_CLAMP,							// flags
		initAlpha= 0.8f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 4,
		startTexture= 0,									// startTexture
		numTextures= 14,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_EXPLODE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Cloud 5
	new TYPES2D {
		flags= Type2D.ANIM_HOLD_LAST | Type2D.USES_BB_MATRIX,	// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 3,
		startTexture= 13,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Dustcloud
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 0.8f,								// initAlpha
		fadeRate= .00005f,							// fadeRate
		texId= 8,
		startTexture= 13,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 20.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_SMOKE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 300.0f,							// maxExpand
	},
	// Gunsmoke
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START,		// flags
		initAlpha= 0.8f,								// initAlpha
		fadeRate= .0001f,								// fadeRate
		texId= 6,
		startTexture= 8,									// startTexture
		numTextures= 8,									// numTextures
		expandRate= 20.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 300.0f,							// maxExpand
	},
	// Air Smoke 2
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= .0002f,								// fadeRate
		texId= 10,
		startTexture= 12,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 20.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_SMOKE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 400.0f,							// maxExpand
	},
	// Glowing explosion Object Square
	new TYPES2D {
		flags= Type2D.FADE_START | Type2D.ALPHA_DAYLIGHT | Type2D.ALPHA_BRIGHTEN | Type2D.GLOW_SPHERE,		// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= .0006f,								// fadeRate
		texId= null,								// texture id sequence
		startTexture= 0, 									// numTextures
		numTextures= 0, 									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= glowSquareVerts,						// glowVerts
		numGlowVerts= numGlowSquareVerts,					// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// missile Glowing Object
	new TYPES2D {
		flags= Type2D.GLOW_SPHERE | Type2D.GLOW_RAND_POINTS,		// flags
		initAlpha= 0.9f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= null,								// texture id sequence
		startTexture= 0, 									// numTextures
		numTextures= 0, 									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= glowStarVerts,						// glowVerts
		numGlowVerts= numGlowStarVerts,					// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Glowing explosion Object Sphere
	new TYPES2D {
		flags= Type2D.GLOW_SPHERE | Type2D.ALPHA_BRIGHTEN | Type2D.ALPHA_DAYLIGHT,						// flags
		initAlpha= 0.6f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= null,								// texture id sequence
		startTexture= 0, 									// numTextures
		numTextures= 0, 									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= glowCircleVerts,						// glowVerts
		numGlowVerts= numGlowCircleVerts,					// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Glowing explosion Object Cross
	new TYPES2D {
		flags= Type2D.GLOW_SPHERE ,						// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= .0009f,								// fadeRate
		texId= 0,									// texture id sequence
		startTexture= 0, 									// numTextures
		numTextures= 0, 									// numTextures
		expandRate= 50.0f,								// expandRate
		glowVerts= glowCrossVerts,						// glowVerts
		numGlowVerts= numGlowCrossVerts,					// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Crater 1
	new TYPES2D {
		flags= Type2D.ANIM_HOLD_LAST,						// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 3,
		startTexture= 14,									// start Textures
		numTextures= 1,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Crater 2
	new TYPES2D {
		flags= Type2D.ANIM_HOLD_LAST,						// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 3,
		startTexture= 14,									// start Textures
		numTextures= 1,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Crater 3
	new TYPES2D {
		flags= Type2D.ANIM_HOLD_LAST,						// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 3,
		startTexture= 14,									// start Textures
		numTextures= 1,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// FIRE
	new TYPES2D {
		flags= Type2D.RAND_START_FRAME | Type2D.ANIM_LOOP | Type2D.SEQ_SCATTER_ANIM | Type2D.FIRE_SCATTER_PLOT | Type2D.ALPHA_PER_TEXEL,			// flags
		initAlpha= 0.9f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 0,
		startTexture= 0,									// startTexture
		numTextures= 6,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_FIRE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// GROUND STRIKE
	new TYPES2D {
		flags= Type2D.USES_TREE_MATRIX,					// flags
		initAlpha= 0.8f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 5,
		startTexture= 0,									// startTexture
		numTextures= 16,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Water STRIKE
	new TYPES2D {
		flags= Type2D.USES_TREE_MATRIX,					// flags
		initAlpha= 0.8f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 2,
		startTexture= 0,									// startTexture
		numTextures= 16,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Hit Explosion
	new TYPES2D {
		flags= Type2D.ANIM_HALF_RATE | Type2D.ANIM_STOP | Type2D.EXPLODE_SCATTER_PLOT | Type2D.ANIM_NO_CLAMP,							// flags
		initAlpha= 0.8f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 0,
		startTexture= 0,									// startTexture
		numTextures= 12,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_EXPLODE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Sparks
	/*
	{
		flags= ANIM_STOP | USES_BB_MATRIX | ANIM_HALF_RATE,							// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 0,
		startTexture= 12,									// startTexture
		numTextures= 4,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	*/
	// SPARKS explosion Object Star
	new TYPES2D {
		flags= Type2D.FADE_START | Type2D.ALPHA_DAYLIGHT | Type2D.ALPHA_BRIGHTEN | Type2D.GLOW_SPHERE | Type2D.GLOW_RAND_POINTS,		// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= .0015f,								// fadeRate
		texId= null,								// texture id sequence
		startTexture= 0, 									// numTextures
		numTextures= 0, 									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= glowCircleVerts,						// glowVerts
		numGlowVerts= numGlowCircleVerts,					// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Artillery Explosion
	new TYPES2D {
		flags= Type2D.USES_TREE_MATRIX | Type2D.ANIM_STOP,					// flags
		initAlpha= 0.8f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 3,
		startTexture= 0,									// startTexture
		numTextures= 10,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// shock ring
	new TYPES2D {
		flags= Type2D.USES_TREE_MATRIX | Type2D.FADE_START | Type2D.ANIM_HOLD_LAST,	// flags
		initAlpha= 0.5f,								// initAlpha
		fadeRate= 0.0007f,							// fadeRate
		texId= 4,
		startTexture= 15,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 480.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// long hanging smoke
	new TYPES2D {
		flags= Type2D.ANIM_HALF_RATE | Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.DO_FIVE_POINTS,		// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= .00001f,								// fadeRate
		texId= 6,
		startTexture= 0,									// startTexture
		numTextures= 8,									// numTextures
		expandRate= 10.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 500.0f,							// maxExpand
	},
	// Shaped FIRE DEBRIS
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.TEXTURED_CONE | Type2D.FADE_START,					// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0001f,								// fadeRate
		texId= 6,
		startTexture= 8,									// startTexture
		numTextures= 8,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= coneDim,								// glowVerts
		numGlowVerts= 1,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},

	// Water CLOUD
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 0.6f,								// initAlpha
		fadeRate= .0001f,								// fadeRate
		texId= 7,
		startTexture= 14,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 20.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_SMOKE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 100.0f,							// maxExpand
	},
	// DARK DEBRIS
	new TYPES2D {
		flags= Type2D.GOURAUD_TRI | Type2D.GLOW_RAND_POINTS,		 // flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= null,								// texture id sequence
		startTexture= 0,									// startTexture
		numTextures= 0,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// FIRE DEBRIS
	new TYPES2D {
		flags= Type2D.GOURAUD_TRI | Type2D.GLOW_RAND_POINTS,		 // flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= null,								// texture id sequence
		startTexture= 0,									// startTexture
		numTextures= 0,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// LIGHT DEBRIS
	new TYPES2D {
		flags= Type2D.GOURAUD_TRI | Type2D.GLOW_RAND_POINTS,		 // flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= null,								// texture id sequence
		startTexture= 0,									// startTexture
		numTextures= 0,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Glowing explosion Object Sphere
	new TYPES2D {
		flags= Type2D.GLOW_SPHERE | Type2D.ALPHA_BRIGHTEN | Type2D.ALPHA_DAYLIGHT,						// flags
		initAlpha= 0.8f,								// initAlpha
		fadeRate= 0.0009f,								// fadeRate
		texId= null,								// texture id sequence
		startTexture= 0, 									// numTextures
		numTextures= 0, 									// numTextures
		expandRate= 10.0f,								// expandRate
		glowVerts= glowCircleVerts,						// glowVerts
		numGlowVerts= numGlowCircleVerts,					// numGlowVerts
		maxExpand= 100.0f,							// maxExpand
	},
	// shock ring -- small
	new TYPES2D {
		flags= Type2D.USES_TREE_MATRIX | Type2D.FADE_START | Type2D.ANIM_HOLD_LAST,	// flags
		initAlpha= 0.6f,								// initAlpha
		fadeRate= 0.0007f,							// fadeRate
		texId= 4,
		startTexture= 15,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 100.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// fast fading cloud
	new TYPES2D {
		flags= Type2D.ANIM_HALF_RATE | Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.DO_FIVE_POINTS,		// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= .0005f,								// fadeRate
		texId= 6,
		startTexture= 0,									// startTexture
		numTextures= 8,									// numTextures
		expandRate= 60.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// long hanging smoke 2
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 0.5f,								// initAlpha
		fadeRate= .000015f,								// fadeRate
		texId= 6,
		startTexture= 12,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 15.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_EXPLODE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 500.0f,							// maxExpand
	},
	// FLAME
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.DO_FIVE_POINTS,			// flags
		initAlpha= 0.7f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= 4,
		startTexture= 0,									// startTexture
		numTextures= 8,									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// FIRE EXPANDING
	new TYPES2D {
		flags= Type2D.FADE_START | Type2D.RAND_START_FRAME | Type2D.ANIM_LOOP | Type2D.SEQ_SCATTER_ANIM | Type2D.FIRE_SCATTER_PLOT | Type2D.ALPHA_PER_TEXEL,			// flags
		initAlpha= 0.6f,								// initAlpha
		fadeRate= .00045f,								// fadeRate
		texId= 0,
		startTexture= 0,									// startTexture
		numTextures= 6,									// numTextures
		expandRate= 30.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_FIRE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Dustcloud
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 0.4f,								// initAlpha
		fadeRate= .00004f,							// fadeRate
		texId= 8,
		startTexture= 13,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 30.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 1,									// numGlowVerts
		maxExpand= 50.0f,								// maxExpand
	},
	// fire hot
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.SEQ_SCATTER_ANIM | Type2D.FIRE_SCATTER_PLOT | Type2D.ALPHA_PER_TEXEL,			// flags
		initAlpha= 0.7f,								// initAlpha
		fadeRate= .0009f,								// fadeRate
		texId= 0,
		startTexture= 0,									// startTexture
		numTextures= 2,									// numTextures
		expandRate= 10.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_FIRE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// fire med
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.SEQ_SCATTER_ANIM | Type2D.FIRE_SCATTER_PLOT | Type2D.ALPHA_PER_TEXEL,			// flags
		initAlpha= 0.6f,								// initAlpha
		fadeRate= .0009f,								// fadeRate
		texId= 2,
		startTexture= 0,									// startTexture
		numTextures= 2,									// numTextures
		expandRate= 20.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_FIRE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// fire cool
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.SEQ_SCATTER_ANIM | Type2D.FIRE_SCATTER_PLOT | Type2D.ALPHA_PER_TEXEL,			// flags
		initAlpha= 0.4f,								// initAlpha
		fadeRate= .0009f,								// fadeRate
		texId= 4,
		startTexture= 0,									// startTexture
		numTextures= 2,									// numTextures
		expandRate= 30.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_FIRE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// fire 1
	new TYPES2D {
		flags= Type2D.FADE_LAST | Type2D.ANIM_LOOP | Type2D.SEQ_SCATTER_ANIM | Type2D.SMOKE_SCATTER_PLOT | Type2D.ALPHA_PER_TEXEL,			// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0001f,								// fadeRate
		texId= 0,
		startTexture= 0,									// startTexture
		numTextures= 2,									// numTextures
		expandRate= 50.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 1,			// numGlowVerts
		maxExpand= 120.0f,							// maxExpand
	},
	// fire 2
	new TYPES2D {
		flags= Type2D.FADE_START | Type2D.ANIM_LOOP | Type2D.SEQ_SCATTER_ANIM | Type2D.FIRE_SCATTER_PLOT | Type2D.ALPHA_PER_TEXEL,			// flags
		initAlpha= 0.6f,								// initAlpha
		fadeRate= 0.0001f,								// fadeRate
		texId= 0,
		startTexture= 0,									// startTexture
		numTextures= 2,									// numTextures
		expandRate= 20.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_FIRE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// fire 3
	new TYPES2D {
		flags= Type2D.FADE_START | Type2D.ANIM_LOOP | Type2D.SEQ_SCATTER_ANIM | Type2D.FIRE_SCATTER_PLOT | Type2D.ALPHA_PER_TEXEL,			// flags
		initAlpha= 0.4f,								// initAlpha
		fadeRate= 0.0001f,								// fadeRate
		texId= 2,
		startTexture= 0,									// startTexture
		numTextures= 2,									// numTextures
		expandRate= 10.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_FIRE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// fire 4
	new TYPES2D {
		flags= Type2D.FADE_START | Type2D.ANIM_LOOP | Type2D.SEQ_SCATTER_ANIM | Type2D.SMOKE_SCATTER_PLOT | Type2D.ALPHA_PER_TEXEL,			// flags
		initAlpha= 0.9f,								// initAlpha
		fadeRate= 0.00007f,								// fadeRate
		texId= 0,
		startTexture= 0,									// startTexture
		numTextures= 2,									// numTextures
		expandRate= 30.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 1,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// fire 5
	new TYPES2D {
		flags= Type2D.FADE_START | Type2D.ANIM_LOOP | Type2D.SEQ_SCATTER_ANIM | Type2D.FIRE_SCATTER_PLOT | Type2D.ALPHA_PER_TEXEL,			// flags
		initAlpha= 0.9f,								// initAlpha
		fadeRate= 0.00007f,								// fadeRate
		texId= 0,
		startTexture= 0,									// startTexture
		numTextures= 2,									// numTextures
		expandRate= 30.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 1,			// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// fire 6
	new TYPES2D {
		flags= Type2D.FADE_START | Type2D.ANIM_LOOP | Type2D.SEQ_SCATTER_ANIM | Type2D.FIRE_SCATTER_PLOT | Type2D.ALPHA_PER_TEXEL,			// flags
		initAlpha= 0.8f,								// initAlpha
		fadeRate= 0.0001f,								// fadeRate
		texId= 0,
		startTexture= 0,									// startTexture
		numTextures= 2,									// numTextures
		expandRate= 3.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_FIRE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Fire Smoke
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 0.8f,								// initAlpha
		fadeRate= .00005f,								// fadeRate
		texId= 6,
		startTexture= 12,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 30.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_SMOKE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 300.0f,							// maxExpand
	},
	// Trail Smoke
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= .0002f,								// fadeRate
		texId= 10,
		startTexture= 12,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 30.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_SMOKE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 40.0f,							// maxExpand
	},
	// Trail Dust
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 0.8f,								// initAlpha
		fadeRate= .0002f,							// fadeRate
		texId= 8,
		startTexture= 13,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 30.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_SMOKE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 50.0f,							// maxExpand
	},
	// fire 7
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.SEQ_SCATTER_ANIM | Type2D.FIRE_SCATTER_PLOT | Type2D.ALPHA_PER_TEXEL,			// flags
		initAlpha= 0.15f,								// initAlpha
		fadeRate= 0.0001f,								// fadeRate
		texId= 6,
		startTexture= 0,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 40.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_FIRE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Blue Cloud
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 0.6f,								// initAlpha
		fadeRate= .0003f,								// fadeRate
		texId= 9,
		startTexture= 0,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 10.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_SMOKE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 100.0f,							// maxExpand
	},
	// STEAM CLOUD
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 0.8f,								// initAlpha
		fadeRate= .00005f,								// fadeRate
		texId= 7,
		startTexture= 14,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 40.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_SMOKE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 400.0f,							// maxExpand
	},
	// Ground Flash
	new TYPES2D {
		flags= Type2D.GROUND_GLOW | Type2D.GLOW_RAND_POINTS | Type2D.GLOW_SPHERE | Type2D.ALPHA_BRIGHTEN | Type2D.ALPHA_DAYLIGHT,						// flags
		initAlpha= 0.4f,								// initAlpha
		fadeRate= 0.0005f,								// fadeRate
		texId= null,								// texture id sequence
		startTexture= 0, 									// numTextures
		numTextures= 0, 									// numTextures
		expandRate= -170.0f,								// expandRate
		glowVerts= glowCircleVerts,						// glowVerts
		numGlowVerts= numGlowCircleVerts,					// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Ground Glow
	new TYPES2D {
		flags= Type2D.GROUND_GLOW | Type2D.GLOW_RAND_POINTS | Type2D.GLOW_SPHERE | Type2D.ALPHA_BRIGHTEN | Type2D.ALPHA_DAYLIGHT,						// flags
		initAlpha= 0.2f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= null,								// texture id sequence
		startTexture= 0, 									// numTextures
		numTextures= 0, 									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= glowCircleVerts,						// glowVerts
		numGlowVerts= numGlowCircleVerts,					// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Missile Ground Glow
	new TYPES2D {
		flags= Type2D.GROUND_GLOW | Type2D.GLOW_RAND_POINTS | Type2D.GLOW_SPHERE | Type2D.ALPHA_BRIGHTEN | Type2D.ALPHA_DAYLIGHT,						// flags
		initAlpha= 0.2f,								// initAlpha
		fadeRate= 0.0f,								// fadeRate
		texId= null,								// texture id sequence
		startTexture= 0, 									// numTextures
		numTextures= 0, 									// numTextures
		expandRate= 0.0f,								// expandRate
		glowVerts= glowCircleVerts,						// glowVerts
		numGlowVerts= numGlowCircleVerts,					// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
	// Big Smoke 1
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= .00005f,								// fadeRate
		texId= 10,
		startTexture= 12,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 20.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= 0,			// numGlowVerts
		maxExpand= 30000.0f,							// maxExpand
	},
	// Big Smoke 2
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= .00005f,								// fadeRate
		texId= 6,
		startTexture= 12,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 20.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_SMOKE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 30000.0f,							// maxExpand
	},
	// Big Dust 1
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= .00006f,							// fadeRate
		texId= 8,
		startTexture= 13,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 20.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_SMOKE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 30000.0f,							// maxExpand
	},
	// Incendiary Explosion 
	new TYPES2D {
		flags= Type2D.FADE_START | Type2D.ANIM_LOOP | Type2D.ANIM_HALF_RATE | Type2D.EXPLODE_SCATTER_PLOT | Type2D.ANIM_NO_CLAMP,							// flags
		initAlpha= 1.0f,								// initAlpha
		fadeRate= 0.0001f,								// fadeRate
		texId= 1,
		startTexture= 0,									// startTexture
		numTextures= 16,									// numTextures
		expandRate= 80.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_EXPLODE_SCATTER_FRAMES,									// numGlowVerts
		maxExpand= 10000.0f,							// maxExpand
	},
		// Water Wake
	new TYPES2D {
		flags= Type2D.ANIM_LOOP | Type2D.FADE_START | Type2D.ALPHA_PER_TEXEL,		// flags
		initAlpha= 0.6f,								// initAlpha
		fadeRate= 0.00001f,								// fadeRate
		texId= 7,									// texid
		startTexture= 14,									// startTexture
		numTextures= 1,									// numTextures
		expandRate= 10.0f,								// expandRate
		glowVerts= null,								// glowVerts
		numGlowVerts= NUM_SMOKE_SCATTER_FRAMES,			// numGlowVerts
		maxExpand= 100.0f,							// maxExpand
	}

};

        private static readonly int DRAW2D_MAXTYPES = gTypeTable.Length;
        
        private static readonly Random randgen = new Random();
        private static int rand()
        {
            throw new NotImplementedException();
        }
    }

}

