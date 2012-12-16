using System;


namespace FalconNet.Graphics
{

    public struct MPRVtx_t
    {
        public float x, y;
    }
    public enum MPR_STA
    {
        MPR_STA_NONE,
        MPR_STA_ENABLES,               /* use MPR_SE_...   */
        MPR_STA_DISABLES,              /* use MPR_SE_...   */

        MPR_STA_SRC_BLEND_FUNCTION,    /* use MPR_BF_...   */
        MPR_STA_DST_BLEND_FUNCTION,    /* use MPR_BF_...   */

        MPR_STA_TEX_FILTER,            /* use MPR_TX_...   */
        MPR_STA_TEX_FUNCTION,          /* use MPR_TF_...   */

#if MPR_MASKING_ENABLED
  MPR_STA_AREA_MASK,             /* Area pattern bitmask */
  MPR_STA_LINE_MASK,             /* Line pattern bitmask */
  MPR_STA_PIXEL_MASK,            /* RGBA or bitmask  */
#endif
        MPR_STA_FG_COLOR,              /* long, RGBA or index  */
        MPR_STA_BG_COLOR,              /* long, RGBA or index  */

#if MPR_DEPTH_BUFFER_ENABLED
  MPR_STA_Z_FUNCTION,            /* use MPR_ZF_...   */
  MPR_STA_FG_DEPTH,              /* FIXED 16.16 for raster*/
  MPR_STA_BG_DEPTH,              /* FIXED 16.16 for zclear*/
#endif

        MPR_STA_TEX_ID,                /* Handle for current texture.*/

        MPR_STA_FOG_COLOR,             /* long, RGBA       */
        MPR_STA_HARDWARE_ON,           /* Read only - set if hardware supports mode */

        //#if !defined(MPR_GAMMA_FAKE)
        MPR_STA_GAMMA_RED,             /* Gamma correction term for red (set before blue)  */
        MPR_STA_GAMMA_GREEN,           /* Gamma correction term for green (set before blue) */
        MPR_STA_GAMMA_BLUE,            /* Gamma correction term for blue (set last) */
        //#else
        MPR_STA_RAMP_COLOR,           /* Packed color for the ramp table        */
        MPR_STA_RAMP_PERCENT,         /* fractional (0.0=normal, 1.0=saturated) */
        //#endif

        MPR_STA_SCISSOR_LEFT,
        MPR_STA_SCISSOR_TOP,
        MPR_STA_SCISSOR_RIGHT,         /* right,bottom, not inclusive*/
        MPR_STA_SCISSOR_BOTTOM,        /* Validity Check done here.    */

    }

    public enum MPRPacketID
    {
        /*********************************************************************/
        // OW - make them match the D3D primitive constants 
#if NOTHING
  MPR_PKT_END = 0,              /* MUST BE SAME AS HOOK_END            */
  MPR_PKT_SETSTATE,             /* MUST BE SAME AS HOOK_SETSTATE       */
  MPR_PKT_RESTORESTATEID,       /* MUST BE SAME AS HOOK_RESTORESTATEID */
  MPR_PKT_STARTFRAME,           /* MUST BE SAME AS HOOK_STARTFRAME     */
  MPR_PKT_FINISHFRAME,          /* MUST BE SAME AS HOOK_FINISHFRAME    */
  MPR_PKT_CLEAR_BUFFERS,        /* MUST BE SAME AS HOOK_CLEAR...       */
  MPR_PKT_SWAP_BUFFERS,         /* MUST BE SAME AS HOOK_SWAP...        */

  MPR_PKT_POINTS,               /* MUST BE SAME AS HOOK_POINTS         */
  MPR_PKT_LINES,                /* MUST BE SAME AS HOOK_LINES          */
  MPR_PKT_POLYLINE,             /* MUST BE SAME AS HOOK_POLYLINE       */
  MPR_PKT_TRIANGLES,            /* MUST BE SAME AS HOOK_TRIANGLES      */
  MPR_PKT_TRISTRIP,             /* MUST BE SAME AS HOOK_TRISTRIP       */
  MPR_PKT_TRIFAN,               /* MUST BE SAME AS HOOK_TRIFAN         */
#else
        MPR_PKT_POINTS = 1,               /* MUST BE SAME AS HOOK_POINTS         */
        MPR_PKT_LINES,                /* MUST BE SAME AS HOOK_LINES          */
        MPR_PKT_POLYLINE,             /* MUST BE SAME AS HOOK_POLYLINE       */
        MPR_PKT_TRIANGLES,            /* MUST BE SAME AS HOOK_TRIANGLES      */
        MPR_PKT_TRISTRIP,             /* MUST BE SAME AS HOOK_TRISTRIP       */
        MPR_PKT_TRIFAN,               /* MUST BE SAME AS HOOK_TRIFAN         */
#endif
        /*********************************************************************/

        MPR_PKT_ID_COUNT,             /* MUST BE LAST */
    } ;

    public static class Mpr_light
    {
        // Possible values for: MPR_STA_ENABLES 
        //          MPR_STA_DISABLES 
        // Logical OR of ...
        public const long MPR_SE_SHADING = 0x00000001L; /* interpolate r,g,b, a       */
        public const long MPR_SE_TEXTURING = 0x00000002L; /* interpolate u,v,w          */
        public const long MPR_SE_MODULATION = 0x00000004L; /* modulate color & texture   */
        public const long MPR_SE_Z_BUFFERING = 0x00000008L; /* interpolate z and compare  */
        public const long MPR_SE_FOG = 0x00000010L; /* interpolate fog            */
        public const long MPR_SE_PIXEL_MASKING = 0x00000020L; /* selective pixel update     */

        public const long MPR_SE_Z_MASKING = 0x00000040L; /* selective z update         */
        public const long MPR_SE_PATTERNING = 0x00000080L; /* selective pixel & z update */
        public const long MPR_SE_SCISSORING = 0x00000100L; /* selective pixel & z update */
        public const long MPR_SE_CLIPPING = 0x00000200L; /* selective pixel & z update */
        public const long MPR_SE_BLENDING = 0x00000400L; /* per-pixel operation        */

        public const long MPR_SE_FILTERING = 0x00002000L; /* texture filter control     */
        public const long MPR_SE_TRANSPARENCY = 0x00004000L; /* texel use control          */

        public const long MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE = 0x00200000L;
        public const long MPR_SE_NON_SUBPIXEL_PRECISION_MODE = 0x00400000L;
        public const long MPR_SE_HARDWARE_OFF = 0x00800000L;
        public const long MPR_SE_PALETTIZED_TEXTURES = 0x01000000L;
        public const long MPR_SE_DECAL_TEXTURES = 0x04000000L;
        public const long MPR_SE_ANTIALIASING = 0x08000000L;
        public const long MPR_SE_DITHERING = 0x10000000L;


        public const long MPR_DRAW_BUFFER = 0x0080;  /* draw buffer      */
        public const long MPR_DEPTH_BUFFER = 0x0400;  /* z buffer.        */
        public const long MPR_TEXTURE_BUFFER = 0x0800;  /* texture.         */


        // Possible values for : VtxInfo in the MPRPrimitive() prototype below
        // Logical OR of ...
        public const int MPR_VI_COLOR = 0x0002;
        public const int MPR_VI_TEXTURE = 0x0004;

        // Possible values for: BmpInfo in the MPRBitmap() prototype below
        // Logical OR of ...
        public const int MPR_BI_INDEX = 0x0001;
        public const int MPR_BI_CHROMAKEY = 0x0002;
        public const int MPR_BI_ALPHA = 0x0004;

        // Possible values for: SwapInfo in the MPRSwapBuffers() prototype below
        // Logical OR of ...
        public const int MPR_SI_SCISSOR = 0x0004;


        // Possible values for: ClearInfo in the MPRClearBuffers() prototype below
        // Logical OR of ...
        public const int MPR_CI_DRAW_BUFFER = 0x0001;
        public const int MPR_CI_ZBUFFER = 0x0004;


        // Possible values for: TexInfo in the MPRNewTexture() prototype below.
        // Logical OR of ...
        public const int MPR_TI_DEFAULT = 0x0000;
        public const int MPR_TI_MIPMAP = 0x0001;
        public const int MPR_TI_CHROMAKEY = 0x0020;
        public const int MPR_TI_ALPHA = 0x0040;
        public const int MPR_TI_PALETTE = 0x0080;

        // The following flags are used internally by MPR but stored in the same
        // memory as the above TI_ "user" flags, so we'll define them here to ensure
        // they retain mutually exclusive values.
        public const int MPR_TI_RESERVED_DRTY = 0x0100;


        // Possible values for: MPRMsgHdr.MsgHdrVersion field
        // Pick ONE of ...
        public const int MPR_MSG_VERSION = 1;

        // Maximum No of vertices to be passed using MPRPrimitive()
        public const int MPR_MAX_VERTICES = 256;


        // These are used by MPRPrimitive()
        // Pick ONE of ...

        public const int MPR_PRM_POINTS = (int)MPRPacketID.MPR_PKT_POINTS;
        public const int MPR_PRM_LINES = (int)MPRPacketID.MPR_PKT_LINES;
        public const int MPR_PRM_POLYLINE = (int)MPRPacketID.MPR_PKT_POLYLINE;
        public const int MPR_PRM_TRIANGLES = (int)MPRPacketID.MPR_PKT_TRIANGLES;
        public const int MPR_PRM_TRISTRIP = (int)MPRPacketID.MPR_PKT_TRISTRIP;
        public const int MPR_PRM_TRIFAN = (int)MPRPacketID.MPR_PKT_TRIFAN;

    }
}
