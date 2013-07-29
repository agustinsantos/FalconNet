using System;
using TextureID = System.Int16;

namespace FalconNet.CampaignBase
{
    //TODO rename TerrainPost
    /// <summary>
    /// Most basic element of the terrain map.  Posts contain all the
    /// information about a point on the ground.  There may in the future be
    /// multiple versions of posts (ie: textured vs. not textured).  For now,
    /// only one.
    /// </summary>
    public struct Tpost
    {

        public const int POST_OFFSET_BITS = 4;
        public const int POST_OFFSET_MASK = (~(((~1) >> POST_OFFSET_BITS) << POST_OFFSET_BITS));
        public const int POSTS_ACROSS_BLOCK = (1 << POST_OFFSET_BITS);
        public const int POSTS_PER_BLOCK = (1 << (POST_OFFSET_BITS << 1));
        public static float GLOBAL_POST_TO_WORLD(float distance) { return ((distance) * TMap.FeetPerPost); }

        public static float WORLD_TO_FLOAT_GLOBAL_POST(float distance) { return ((distance) / TMap.FeetPerPost); }
        public static int WORLD_TO_GLOBAL_POST(float distance) { return ((int)((float)Math.Floor(WORLD_TO_FLOAT_GLOBAL_POST(distance)))); }
#if TODO
        public static float WORLD_TO_FLOAT_LEVEL_POST(float distance, int LOD) { return ((distance) / TheMap.Level(LOD).FTperPOST()); }
#endif
        public static float WORLD_TO_LEVEL_POST(float distance, int LOD) { return (WORLD_TO_GLOBAL_POST(distance) >> (LOD)); }
        public static float WORLD_TO_LEVEL_BLOCK(float distance, int LOD) { return (WORLD_TO_GLOBAL_POST(distance) >> ((LOD) + POST_OFFSET_BITS)); }
        public static float LEVEL_POST_TO_WORLD(int distance, int LOD) { return (GLOBAL_POST_TO_WORLD((distance) << (LOD))); }
        public static float LEVEL_BLOCK_TO_WORLD(int distance, int LOD) { return (GLOBAL_POST_TO_WORLD((distance) << ((LOD) + POST_OFFSET_BITS))); }

        public static int LEVEL_POST_TO_LEVEL_BLOCK(int distance) { return ((distance) >> POST_OFFSET_BITS); }
        public static int LEVEL_POST_TO_BLOCK_POST(int distance) { return ((distance) & POST_OFFSET_MASK); }

        public static int GLOBAL_POST_TO_LEVEL_POST(int distance, int LOD) { return ((distance) >> (LOD)); }
        public static int LEVEL_POST_TO_GLOBAL_POST(int distance, int LOD) { return ((distance) << (LOD)); }



        // X can be obtained from:   x = LEVEL_POST_TO_WORLD( levelRow, LOD );
        // Y can be obtained from:   y = LEVEL_POST_TO_WORLD( levelCol, LOD );
        public float z; // Units of floating point feet -- positive z axis points DOWN

        // Color information
        public int colorIndex; // (could shrink to a byte)

        // Texture information
        public float u; // u is East/West
        public float v; // v is North/South
        public float d;
        public TextureID texID; // 16 bit value (see TerrTex.h)

        // normal information
        public float theta;
        public float phi;
    }
}
