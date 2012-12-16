using System;


namespace FalconNet.Graphics
{
    public static class Ttypes
    {
        public static int POST_OFFSET_BITS = 4;
        public static int POST_OFFSET_MASK() { return (~(((~1) >> POST_OFFSET_BITS) << POST_OFFSET_BITS)); }
        public static int POSTS_ACROSS_BLOCK() { return (1 << POST_OFFSET_BITS); }
        public static int POSTS_PER_BLOCK() { return (1 << (POST_OFFSET_BITS << 1)); }
#if TODO
        public static float WORLD_TO_FLOAT_GLOBAL_POST(float distance) { return ((distance) / FeetPerPost); }
        public static int WORLD_TO_GLOBAL_POST(float distance) { return ((int)((float)Math.Floor(WORLD_TO_FLOAT_GLOBAL_POST(distance)))); }
        public static float WORLD_TO_FLOAT_LEVEL_POST(float distance, int LOD) { return ((distance) / TheMap.Level(LOD).FTperPOST()); }
        public static float WORLD_TO_LEVEL_POST(float distance, int LOD) { return (WORLD_TO_GLOBAL_POST(distance) >> (LOD)); }
        public static float WORLD_TO_LEVEL_BLOCK(float distance, int LOD)	{ return	(WORLD_TO_GLOBAL_POST( distance ) >> ((LOD)+POST_OFFSET_BITS));}

        public static float GLOBAL_POST_TO_WORLD(float distance) { return ((distance) * FeetPerPost); }
 
        public static float LEVEL_POST_TO_WORLD(int distance, int LOD) { return (GLOBAL_POST_TO_WORLD((distance) << (LOD))); }
        public static float LEVEL_BLOCK_TO_WORLD(int distance, int LOD) { return (GLOBAL_POST_TO_WORLD((distance) << ((LOD) + POST_OFFSET_BITS))); }
#endif 
        public static int LEVEL_POST_TO_LEVEL_BLOCK(int distance) { return ((distance) >> POST_OFFSET_BITS); }
        public static int LEVEL_POST_TO_BLOCK_POST(int distance) { return ((distance) & POST_OFFSET_MASK()); }

        public static int GLOBAL_POST_TO_LEVEL_POST(int distance, int LOD) { return ((distance) >> (LOD)); }
        public static int LEVEL_POST_TO_GLOBAL_POST(int distance, int LOD) { return ((distance) << (LOD)); }

    }
}
