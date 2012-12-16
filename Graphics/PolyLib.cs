using System;

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

	public struct	Pcolor
	{
		public float	r, g, b, a;
	}

	public struct	Pnormal
	{
		public float	i, j, k;
	}

	public struct	Ptexcoord
	{
		public float	u, v;
	}

	//typedef float	Pintensity;
	public struct	Pintensity
	{
		public float v;
	}
	
	public struct	PclipInfo
	{
		public UInt32	clipFlag;			// Which edges this point is outside
		public float	csX, csY, csZ;		// Camera space coordinates of this point
	}


	/********************************************\
		Polygon structures
	\********************************************/
	public class Prim
	{
		public PpolyType	type;
		public int			nVerts;
		public int[]		xyz;		// Indexes XformedPosPool
	}

	public  class PrimPointFC: Prim
	{
		public int			rgba;		// Indexes ColorPool
	}

	public class PrimLineFC: Prim
	{
		public int			rgba;		// Indexes ColorPool
	}

	public class PrimLtStr: Prim
	{
		public int			rgba;			// Indexes ColorPool
		public int			rgbaBack;		// Indexes ColorPool (-1 means omnidirectional -- could subclass instead)
		public float		i, j, k;		// Direction vector (negative dot with eyepos means use back color)
	}

	public class Poly:   Prim
	{
		float		A, B, C, D;	// Polygon plane equation for back face culling
	}  

	public class PolyFC:   Poly
	{
		int			rgba;		// Indexes ColorPool
	}  

	public class PolyVC:   Poly
	{
		int[] rgba;		// Indexes ColorPool
	}  

	public class PolyFCN:   PolyFC
	{
		int	I;			// Indexes IntesityPool
	}  

	public class PolyVCN:   PolyVC
	{
		int[]   I;			// Indexes IntesityPool
	}  

	public class PolyTexFC:   PolyFC
	{
		int			texIndex;	// Indexes the local texture id table
		Ptexcoord[] uv;
	} 

	public class PolyTexVC:   PolyVC
	{
		int			texIndex;	// Indexes the local texture id table
		Ptexcoord[] uv;
	}  

	public class PolyTexFCN:   PolyFCN
	{
		int			texIndex;	// Indexes the local texture id table
		Ptexcoord[] uv;
	}  

	public class PolyTexVCN:   PolyVCN
	{
		int			texIndex;	// Indexes the local texture id table
		Ptexcoord[] uv;
	}  
	
	internal struct ClipVert
	{
		int			xyz;
		int			rgba;
		int			I;
		Ptexcoord	uv;
	}
	
	public static class PolyLib
	{
#if TODO
/********************************************\
	Polygon render state tables
\********************************************/
extern const int *RenderStateTable;

extern const int *RenderStateTablePC;
extern const int *RenderStateTableNPC;

extern const int *RenderStateTableNoFogPC;
extern const int *RenderStateTableNoFogNPC;
extern const int *RenderStateTableFogPC;
extern const int *RenderStateTableFogNPC;

// OW
// Original Tables without filtering

extern const int RenderStateTableNoTex[];
extern const int RenderStateTableWithPCTex[];
extern const int RenderStateTableWithNPCTex[];
extern const int RenderStateTableFogWithPCTex[];
extern const int RenderStateTableFogWithNPCTex[];

// OW 
// Tables with Filtering
extern const int RenderStateTableNoTex_Filter[];
extern const int RenderStateTableWithPCTex_Filter[];
extern const int RenderStateTableWithNPCTex_Filter[];
extern const int RenderStateTableFogWithPCTex_Filter[];
extern const int RenderStateTableFogWithNPCTex_Filter[];


/********************************************\
	Jump table for polygon draw functions
\********************************************/
typedef void (*DrawPrimFp)( Prim *prim );
typedef void (*ClipPrimFp)( Prim *prim, UInt32 clipFlag );

extern const DrawPrimFp *DrawPrimJumpTable;

extern const DrawPrimFp *DrawPrimNoClipJumpTable;
extern const ClipPrimFp *ClipPrimJumpTable;

extern const DrawPrimFp *DrawPrimNoFogNoClipJumpTable;
extern const ClipPrimFp *ClipPrimNoFogJumpTable;
extern const DrawPrimFp *DrawPrimFogNoClipJumpTable;
extern const ClipPrimFp *ClipPrimFogJumpTable;

extern const DrawPrimFp DrawPrimWithClipJumpTable[];
extern const DrawPrimFp DrawPrimNoClipWithTexJumpTable[];
extern const DrawPrimFp DrawPrimNoClipNoTexJumpTable[];
extern const DrawPrimFp DrawPrimFogNoClipWithTexJumpTable[];
extern const DrawPrimFp DrawPrimFogNoClipNoTexJumpTable[];
extern const ClipPrimFp ClipPrimWithTexJumpTable[];
extern const ClipPrimFp ClipPrimNoTexJumpTable[];
extern const ClipPrimFp ClipPrimFogWithTexJumpTable[];
extern const ClipPrimFp ClipPrimFogNoTexJumpTable[];


/********************************************\
	Polygon draw functions (no clipping)
\********************************************/
void DrawPrimPoint(	PrimPointFC		*point );
void DrawPrimLine(	PrimLineFC		*line );
void DrawPrimLtStr(	PrimLtStr		*ltstr );
void DrawPoly(		PolyFC			*poly );
void DrawPolyL(		PolyFCN			*poly );
void DrawPolyG(		PolyVC			*poly );
void DrawPolyGL(	PolyVCN			*poly );
void DrawPolyT(		PolyTexFC		*poly );
void DrawPolyTL(	PolyTexFCN		*poly );
void DrawPolyTG(	PolyTexVC		*poly );
void DrawPolyTGL(	PolyTexVCN		*poly );

void DrawPrimFPoint(PrimPointFC		*point );
void DrawPrimFLine(	PrimLineFC		*line );
void DrawPolyF(		PolyFC			*poly );
void DrawPolyFL(	PolyFCN			*poly );
void DrawPolyFG(	PolyVC			*poly );
void DrawPolyFGL(	PolyVCN			*poly );
void DrawPolyFT(	PolyTexFC		*poly );
void DrawPolyFTL(	PolyTexFCN		*poly );
void DrawPolyFTG(	PolyTexVC		*poly );
void DrawPolyFTGL(	PolyTexVCN		*poly );

void DrawPolyAT(	PolyTexFC		*poly );
void DrawPolyATL(	PolyTexFCN		*poly );
void DrawPolyATG(	PolyTexVC		*poly );
void DrawPolyATGL(	PolyTexVCN		*poly );


/********************************************\
	Polygon clip functions
\********************************************/
void DrawClippedPrim( Prim *prim );

void ClipPrimPoint(	PrimPointFC		*point,	UInt32 clipFlag );
void ClipPrimLine(	PrimLineFC		*line,	UInt32 clipFlag );
void ClipPrimLtStr(	PrimLtStr		*ltstr,	UInt32 clipFlag );
void ClipPoly(		PolyFC			*poly,	UInt32 clipFlag );
void ClipPolyL(		PolyFCN			*poly,	UInt32 clipFlag );
void ClipPolyG(		PolyVC			*poly,	UInt32 clipFlag );
void ClipPolyGL(	PolyVCN			*poly,	UInt32 clipFlag );
void ClipPolyT(		PolyTexFC		*poly,	UInt32 clipFlag );
void ClipPolyTL(	PolyTexFCN		*poly,	UInt32 clipFlag );
void ClipPolyTG(	PolyTexVC		*poly,	UInt32 clipFlag );
void ClipPolyTGL(	PolyTexVCN		*poly,	UInt32 clipFlag );

void ClipPrimFPoint(PrimPointFC		*point,	UInt32 clipFlag );
void ClipPrimFLine(	PrimLineFC		*line,	UInt32 clipFlag );
void ClipPolyF(		PolyFC			*poly,	UInt32 clipFlag );
void ClipPolyFL(	PolyFCN			*poly,	UInt32 clipFlag );
void ClipPolyFG(	PolyVC			*poly,	UInt32 clipFlag );
void ClipPolyFGL(	PolyVCN			*poly,	UInt32 clipFlag );
void ClipPolyFT(	PolyTexFC		*poly,	UInt32 clipFlag );
void ClipPolyFTL(	PolyTexFCN		*poly,	UInt32 clipFlag );
void ClipPolyFTG(	PolyTexVC		*poly,	UInt32 clipFlag );
void ClipPolyFTGL(	PolyTexVCN		*poly,	UInt32 clipFlag );

void ClipPolyAT(	PolyTexFC		*poly,	UInt32 clipFlag );
void ClipPolyATL(	PolyTexFCN		*poly,	UInt32 clipFlag );
void ClipPolyATG(	PolyTexVC		*poly,	UInt32 clipFlag );
void ClipPolyATGL(	PolyTexVCN		*poly,	UInt32 clipFlag );


/********************************************\
	Polygon decode functions
\********************************************/
Prim	*RestorePrimPointers( BYTE *baseAddress, int offset );

void	RestorePrimPointFC( PrimPointFC *prim, BYTE *baseAddress );
void	RestorePrimLineFC( PrimLineFC *prim, BYTE *baseAddress );
void	RestorePrimLtStr( PrimLtStr *prim, BYTE *baseAddress );
void	RestorePolyFC( PolyFC *prim, BYTE *baseAddress );
void	RestorePolyVC( PolyVC *prim, BYTE *baseAddress );
void	RestorePolyFCN( PolyFCN *prim, BYTE *baseAddress );
void	RestorePolyVCN( PolyVCN *prim, BYTE *baseAddress );
void	RestorePolyTexFC( PolyTexFC *prim, BYTE *baseAddress );
void	RestorePolyTexVC( PolyTexVC *prim, BYTE *baseAddress );
void	RestorePolyTexFCN( PolyTexFCN *prim, BYTE *baseAddress );
void	RestorePolyTexVCN( PolyTexVCN *prim, BYTE *baseAddress );
#endif
	}
}

