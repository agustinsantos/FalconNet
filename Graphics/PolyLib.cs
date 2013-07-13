using System;
using Pintensity = System.Single;  //typedef float	Pintensity;


namespace FalconNet.Graphics
{

    public enum PpolyType
    {
        PointF = 0,
        LineF,
        //	LtStr,

        F,
        FL,
        G,
        GL,
        Tex,
        TexL,
        TexG,
        TexGL,
        CTex,
        CTexL,
        CTexG,
        CTexGL,

        AF,
        AFL,
        AG,
        AGL,
        ATex,
        ATexL,
        ATexG,
        ATexGL,
        CATex,
        CATexL,
        CATexG,
        CATexGL,

        BAptTex,

        PpolyTypeNum
    };

    //typedef Trotation	Pmatrix;

    //typedef Tpoint		Tpoint;

    public struct Pcolor
    {
        public float r, g, b, a;
    }

    public struct Pnormal
    {
        public float i, j, k;
    }

    public struct Ptexcoord
    {
        public float u, v;
    }


    public struct PclipInfo
    {
        public ClippingFlags clipFlag;			// Which edges this point is outside
        public float csX, csY, csZ;		// Camera space coordinates of this point
    }


    /********************************************\
        Polygon structures
    \********************************************/
    public class Prim
    {
        public PpolyType type;
        public int nVerts;
        public int[] xyz;		// Indexes XformedPosPool
    }

    public class PrimPointFC : Prim
    {
        public int rgba;		// Indexes ColorPool
    }

    public class PrimLineFC : Prim
    {
        public int rgba;		// Indexes ColorPool
    }

    public class PrimLtStr : Prim
    {
        public int rgba;			// Indexes ColorPool
        public int rgbaBack;		// Indexes ColorPool (-1 means omnidirectional -- could subclass instead)
        public float i, j, k;		// Direction vector (negative dot with eyepos means use back color)
    }

    public class Poly : Prim
    {
        public float A, B, C, D;	// Polygon plane equation for back face culling
    }

    public class PolyFC : Poly
    {
        public int rgba;		// Indexes ColorPool
    }

    public class PolyVC : Poly
    {
        public int[] rgba;		// Indexes ColorPool
    }

    public class PolyFCN : PolyFC
    {
        int I;			// Indexes IntesityPool
    }

    public class PolyVCN : PolyVC
    {
        int[] I;			// Indexes IntesityPool
    }

    public class PolyTexFC : PolyFC
    {
        int texIndex;	// Indexes the local texture id table
        Ptexcoord[] uv;
    }

    public class PolyTexVC : PolyVC
    {
        int texIndex;	// Indexes the local texture id table
        Ptexcoord[] uv;
    }

    public class PolyTexFCN : PolyFCN
    {
        int texIndex;	// Indexes the local texture id table
        Ptexcoord[] uv;
    }

    public class PolyTexVCN : PolyVCN
    {
        int texIndex;	// Indexes the local texture id table
        Ptexcoord[] uv;
    }

    internal struct ClipVert
    {
        int xyz;
        int rgba;
        int I;
        Ptexcoord uv;
    }

    public static class PolyLib
    {

        /********************************************\
            Polygon render state tables
        \********************************************/
        public static readonly int[] RenderStateTable;

        public static readonly int[] RenderStateTablePC;
        public static readonly int[] RenderStateTableNPC;

        public static readonly int[] RenderStateTableNoFogPC;
        public static readonly int[] RenderStateTableNoFogNPC;
        public static readonly int[] RenderStateTableFogPC;
        public static readonly int[] RenderStateTableFogNPC;

        // OW
        // Original Tables without filtering

        public static readonly int[] RenderStateTableNoTex;
        public static readonly int[] RenderStateTableWithPCTex;
        public static readonly int[] RenderStateTableWithNPCTex;
        public static readonly int[] RenderStateTableFogWithPCTex;
        public static readonly int[] RenderStateTableFogWithNPCTex;

        // OW 
        // Tables with Filtering
        public static readonly int[] RenderStateTableNoTex_Filter;
        public static readonly int[] RenderStateTableWithPCTex_Filter;
        public static readonly int[] RenderStateTableWithNPCTex_Filter;
        public static readonly int[] RenderStateTableFogWithPCTex_Filter;
        public static readonly int[] RenderStateTableFogWithNPCTex_Filter;


        /********************************************\
            Jump table for polygon draw functions
        \********************************************/
        public delegate void DrawPrimFp(Prim prim);
        public delegate void ClipPrimFp(Prim prim, UInt32 clipFlag);

        public static readonly DrawPrimFp[] DrawPrimJumpTable;

        public static readonly DrawPrimFp[] DrawPrimNoClipJumpTable;
        public static readonly ClipPrimFp[] ClipPrimJumpTable;

        public static readonly DrawPrimFp[] DrawPrimNoFogNoClipJumpTable;
        public static readonly ClipPrimFp[] ClipPrimNoFogJumpTable;
        public static readonly DrawPrimFp[] DrawPrimFogNoClipJumpTable;
        public static readonly ClipPrimFp[] ClipPrimFogJumpTable;

        public static readonly DrawPrimFp[] DrawPrimWithClipJumpTable;
        public static readonly DrawPrimFp[] DrawPrimNoClipWithTexJumpTable;
        public static readonly DrawPrimFp[] DrawPrimNoClipNoTexJumpTable;
        public static readonly DrawPrimFp[] DrawPrimFogNoClipWithTexJumpTable;
        public static readonly DrawPrimFp[] DrawPrimFogNoClipNoTexJumpTable;
        public static readonly ClipPrimFp[] ClipPrimWithTexJumpTable;
        public static readonly ClipPrimFp[] ClipPrimNoTexJumpTable;
        public static readonly ClipPrimFp[] ClipPrimFogWithTexJumpTable;
        public static readonly ClipPrimFp[] ClipPrimFogNoTexJumpTable;

#if TODO
/********************************************\
	Polygon draw functions (no clipping)
\********************************************/
public static void DrawPrimPoint(	PrimPointFC		point );
public static void DrawPrimLine(	PrimLineFC		line );
public static void DrawPrimLtStr(	PrimLtStr		ltstr );
public static void DrawPoly(		PolyFC			poly );
public static void DrawPolyL(		PolyFCN			poly );
public static void DrawPolyG(		PolyVC			poly );
public static void DrawPolyGL(	PolyVCN			poly );
public static void DrawPolyT(		PolyTexFC		poly );
public static void DrawPolyTL(	PolyTexFCN		poly );
public static void DrawPolyTG(	PolyTexVC		poly );
public static void DrawPolyTGL(	PolyTexVCN		poly );

public static void DrawPrimFPoint(PrimPointFC		point );
public static void DrawPrimFLine(	PrimLineFC		line );
public static void DrawPolyF(		PolyFC			poly );
public static void DrawPolyFL(	PolyFCN			poly );
public static void DrawPolyFG(	PolyVC			poly );
public static void DrawPolyFGL(	PolyVCN			poly );
public static void DrawPolyFT(	PolyTexFC		poly );
public static void DrawPolyFTL(	PolyTexFCN		poly );
public static void DrawPolyFTG(	PolyTexVC		poly );
public static void DrawPolyFTGL(	PolyTexVCN		poly );

public static void DrawPolyAT(	PolyTexFC		poly );
public static void DrawPolyATL(	PolyTexFCN		poly );
public static void DrawPolyATG(	PolyTexVC		poly );
public static void DrawPolyATGL(	PolyTexVCN		poly );


/********************************************\
	Polygon clip functions
\********************************************/
public static void DrawClippedPrim( Prim prim );

public static void ClipPrimPoint(	PrimPointFC		*point,	UInt32 clipFlag );
public static void ClipPrimLine(	PrimLineFC		*line,	UInt32 clipFlag );
public static void ClipPrimLtStr(	PrimLtStr		*ltstr,	UInt32 clipFlag );
public static void ClipPoly(		PolyFC			*poly,	UInt32 clipFlag );
public static void ClipPolyL(		PolyFCN			*poly,	UInt32 clipFlag );
public static void ClipPolyG(		PolyVC			*poly,	UInt32 clipFlag );
public static void ClipPolyGL(	PolyVCN			*poly,	UInt32 clipFlag );
public static void ClipPolyT(		PolyTexFC		*poly,	UInt32 clipFlag );
public static void ClipPolyTL(	PolyTexFCN		*poly,	UInt32 clipFlag );
public static void ClipPolyTG(	PolyTexVC		*poly,	UInt32 clipFlag );
public static void ClipPolyTGL(	PolyTexVCN		*poly,	UInt32 clipFlag );

public static void ClipPrimFPoint(PrimPointFC		*point,	UInt32 clipFlag );
public static void ClipPrimFLine(	PrimLineFC		*line,	UInt32 clipFlag );
public static void ClipPolyF(		PolyFC			*poly,	UInt32 clipFlag );
public static void ClipPolyFL(	PolyFCN			*poly,	UInt32 clipFlag );
public static void ClipPolyFG(	PolyVC			*poly,	UInt32 clipFlag );
public static void ClipPolyFGL(	PolyVCN			*poly,	UInt32 clipFlag );
public static void ClipPolyFT(	PolyTexFC		*poly,	UInt32 clipFlag );
public static void ClipPolyFTL(	PolyTexFCN		*poly,	UInt32 clipFlag );
public static void ClipPolyFTG(	PolyTexVC		*poly,	UInt32 clipFlag );
public static void ClipPolyFTGL(	PolyTexVCN		*poly,	UInt32 clipFlag );

public static void ClipPolyAT(	PolyTexFC		*poly,	UInt32 clipFlag );
public static void ClipPolyATL(	PolyTexFCN		*poly,	UInt32 clipFlag );
public static void ClipPolyATG(	PolyTexVC		*poly,	UInt32 clipFlag );
public static void ClipPolyATGL(	PolyTexVCN		*poly,	UInt32 clipFlag );


/********************************************\
	Polygon decode functions
\********************************************/
public static Prim	*RestorePrimPointers( BYTE *baseAddress, int offset );

public static void	RestorePrimPointFC( PrimPointFC *prim, BYTE *baseAddress );
public static void	RestorePrimLineFC( PrimLineFC *prim, BYTE *baseAddress );
public static void	RestorePrimLtStr( PrimLtStr *prim, BYTE *baseAddress );
public static void	RestorePolyFC( PolyFC *prim, BYTE *baseAddress );
public static void	RestorePolyVC( PolyVC *prim, BYTE *baseAddress );
public static void	RestorePolyFCN( PolyFCN *prim, BYTE *baseAddress );
public static void	RestorePolyVCN( PolyVCN prim, BYTE *baseAddress );
public static void	RestorePolyTexFC( PolyTexFC prim, BYTE *baseAddress );
public static void	RestorePolyTexVC( PolyTexVC prim, BYTE *baseAddress );
public static void	RestorePolyTexFCN( PolyTexFCN prim, BYTE *baseAddress );
public static void	RestorePolyTexVCN( PolyTexVCN prim, BYTE *baseAddress );
#endif
    }
}

