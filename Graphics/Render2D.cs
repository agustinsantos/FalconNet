using System;
using FalconNet.Common;
using DWORD = System.Int16;

namespace FalconNet.Graphics
{

public class TwoDVertex {
	float	x, y;
	float	r, g, b, a;	
	float	u, v, q; 

	DWORD	clipFlag;
} 

#if TODO
	public class Render2D : VirtualDisplay
	{

	public Render2D()			{}
	//public virtual ~Render2D()	{};

	public virtual void Setup( ImageBuffer *imageBuffer );
	public virtual void Cleanup( );

	public virtual void SetImageBuffer( ImageBuffer *imageBuffer );
	public ImageBuffer* GetImageBuffer( ) {return image;}

	public virtual void StartFrame(   );
	public virtual void ClearFrame(   )		{ context.ClearBuffers( MPR_CI_DRAW_BUFFER ); }
	public virtual void FinishFrame(   );

    public virtual void SetViewport( float leftSide, float topSide, float rightSide, float bottomSide );

	public virtual DWORD Color(   )	{return context.CurrentForegroundColor(); }
	public virtual void SetColor( DWORD packedRGBA )	{	  context.RestoreState(STATE_SOLID); context.SelectForegroundColor( packedRGBA ); }
	public virtual void SetBackground( DWORD packedRGBA )	{ context.SetState( MPR_STA_BG_COLOR, packedRGBA ); }

	public void Render2DPoint( float x1, float y1 );
	public void Render2DLine( float x1, float y1, float x2, float y2 );
	public void Render2DTri( float x1, float y1, float x2, float y2, float x3, float y3 );
	public void Render2DBitmap( int srcX, int srcY, int dstX, int dstY, int w, int h, int sourceWidth, DWORD *source );
	public void Render2DBitmap( int srcX, int srcY, int dstX, int dstY, int w, int h, char *filename );
	public void ScreenText( float x, float y, string str, int boxed = 0 );

	public virtual void SetLineStyle (int p) {}

	// Draw a fan with clipping (must set clip flags first)
	public void SetClipFlags( TwoDVertex* vert );
	public void ClipAndDraw2DFan( TwoDVertex** vertPointers, unsigned count, bool gifPicture = false );

  
	// Window and rendering context handles
	public ContextMPR			context;

  
	protected ImageBuffer			*image;

  
	private void IntersectTop(    TwoDVertex *v1, TwoDVertex *v2, TwoDVertex *v );
	private void IntersectBottom( TwoDVertex *v1, TwoDVertex *v2, TwoDVertex *v );
	private void IntersectLeft(   TwoDVertex *v1, TwoDVertex *v2, TwoDVertex *v );
	private void IntersectRight(  TwoDVertex *v1, TwoDVertex *v2, TwoDVertex *v );

	};
#endif
}

