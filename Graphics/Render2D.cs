using System;
using FalconNet.Common;
using DWORD = System.Int16;
using System.Diagnostics;

namespace FalconNet.Graphics
{
	public class TwoDVertex
	{
		float	x, y;
		float	r, g, b, a;
		float	u, v, q;
		DWORD	clipFlag;
	}

	public class Render2D : VirtualDisplay
	{

		public Render2D ()
		{
		}
		//public virtual ~Render2D()	{};

		public virtual void Setup (ImageBuffer imageBuffer)
		{
			bool result;

			image = imageBuffer;

			// Create the MPR rendering context (frame buffer, etc.)

			// OW
			//result = context.Setup( (DWORD)imageBuffer.targetSurface(), (DWORD)imageBuffer.GetDisplayDevice().GetMPRdevice());
			result = context.Setup (imageBuffer, imageBuffer.GetDisplayDevice ().GetDefaultRC ());

			if (!result) {
				ShiError ("Failed to setup rendering context");
			}

			// Store key properties of our target buffer
			xRes = image.targetXres ();
			yRes = image.targetYres ();


			// Set the renderer's default foreground and background colors
			SetColor (0xFFFFFFFF);
			SetBackground (0xFF000000);

			// Call our base classes setup function (must come AFTER xRes and Yres have been set)
			base.Setup ();
		}

		public virtual void Cleanup ()
		{
			context.Cleanup ();

			base.Cleanup ();
		}

		public virtual void SetImageBuffer (ImageBuffer imageBuffer)
		{
			// Remember who our new image buffer is, and tell MPR about the change
			image = imageBuffer;
			context.NewImageBuffer ((DWORD)imageBuffer.targetSurface ());

			// This shouldn't be required, but _might_ be
			context.InvalidateState ();
			context.RestoreState (STATE_SOLID);

			// Store key properties of our target buffer
			xRes = image.targetXres ();
			yRes = image.targetYres ();

			// Setup the default viewport
			SetViewport (-1.0f, 1.0f, 1.0f, -1.0f);

			// Setup the default offset and rotation
			CenterOriginInViewport ();
			ZeroRotationAboutOrigin ();
		}

		public ImageBuffer GetImageBuffer ()
		{
			return image;
		}

		public override void StartFrame ()
		{
			Debug.Assert (image);
			context.StartFrame ();
		}

		public override void ClearFrame ()
		{
			context.ClearBuffers (MPR_CI_DRAW_BUFFER);
		}

		public override void FinishFrame ()
		{
			Debug.Assert (image);
			context.FinishFrame (NULL);
		}

		public override void SetViewport (float leftSide, float topSide, float rightSide, float bottomSide)
		{
			// First call the base classes version of this function
			base.SetViewport (l, t, r, b);

			// Send the new clipping region to MPR
			// (top/right inclusive, bottom/left exclusive)
			context.SetState (MPR_STA_ENABLES, MPR_SE_SCISSORING);
			context.SetState (MPR_STA_SCISSOR_TOP, FloatToInt32 ((float)floor (topPixel)));
			context.SetState (MPR_STA_SCISSOR_LEFT, FloatToInt32 ((float)floor (leftPixel)));
			context.SetState (MPR_STA_SCISSOR_RIGHT, FloatToInt32 ((float)ceil (rightPixel)));
			context.SetState (MPR_STA_SCISSOR_BOTTOM, FloatToInt32 ((float)ceil (bottomPixel)));
		}

		public override DWORD Color ()
		{
			return context.CurrentForegroundColor ();
		}

		public override void SetColor (DWORD packedRGBA)
		{
			context.RestoreState (STATE_SOLID);
			context.SelectForegroundColor (packedRGBA);
		}

		public override void SetBackground (DWORD packedRGBA)
		{
			context.SetState (MPR_STA_BG_COLOR, packedRGBA);
		}

		public override void Render2DPoint (float x1, float y1)
		{
#if ! NOTHING
			context.Draw2DPoint (x1, y1);
#else
	MPRVtx_t	vert;

#if NOTHING // This should be here, but I need to do some fixes first...
   //Clip test
   if (
		!((x1 <= rightPixel)  &&
		(x1 >= leftPixel)   &&
		(y1 <= bottomPixel) &&
		(y1 >= topPixel))
      )
	  return;

   Debug.Assert( 
      (x1 <= rightPixel)  &&
      (x1 >= leftPixel)   &&
      (y1 <= bottomPixel) &&
      (y1 >= topPixel)
      )
#endif

	// Package up the point's coordinates
	vert.x = x1;
	vert.y = y1;

	// Draw the point
	context.DrawPrimitive(MPR_PRM_POINTS, 0, 1, &vert, sizeof(vert));
#endif
		}

		public override void Render2DLine (float x1, float y1, float x2, float y2)
		{
#if ! NOTHING
			context.Draw2DLine (x1, y1, x2, y2);
#else
	MPRVtx_t	verts[2];

#if NOTHING // This should be here, but I need to do some fixes first...
   //Clip test
   if ( 
          !((max (x1, x2) <= rightPixel)  &&
          (min (x1, x2) >= leftPixel)   &&
          (max (y1, y2) <= bottomPixel) &&
          (min (y1, y2) >= topPixel))
      )
	  return;
   Debug.Assert( 
      (max (x1, x2) <= rightPixel)  &&
      (min (x1, x2) >= leftPixel)   &&
      (max (y1, y2) <= bottomPixel) &&
      (min (y1, y2) >= topPixel)
      )
#endif

	// Package up the lines's coordinates
	verts[0].x = x1;
	verts[0].y = y1;
	verts[1].x = x2;
	verts[1].y = y2;

	// Draw the line
	context.DrawPrimitive(MPR_PRM_LINES, 0, 2, verts, sizeof(verts[0]));
#endif
		}

		public void Render2DTri (float x1, float y1, float x2, float y2, float x3, float y3)
		{
			MPRVtx_t[] verts = new MPRVtx_t[3];

			//Clip test
			if (  (max (max (x1, x2), x3) > rightPixel) ||
			      (min (min (x1, x2), x3) < leftPixel) ||
			      (max (max (y1, y2), y3) > bottomPixel) ||
			      (min (min (y1, y2), y3) < topPixel)
			      )
				return;

			// Package up the tri's coordinates
			verts [0].x = x1;
			verts [0].y = y1;
			verts [1].x = x2;
			verts [1].y = y2;
			verts [2].x = x3;
			verts [2].y = y3;

			// Draw the triangle
//	context.RestoreState( STATE_ALPHA_SOLID );
			context.DrawPrimitive (MPR_PRM_TRIANGLES, 0, 3, verts, sizeof(MPRVtx_t));
		}

		public void Render2DBitmap (int srcX, int srcY, int dstX, int dstY, int w, int h, int sourceWidth, byte[] source)
		{
// OW
#if NOTHING
	// Flush the message queue since this call will bypass the queue
	context.SendCurrentPacket();

	// Copy the required portion of the image to the draw buffer
	MPRBitmap(  context.rc, 32, 
					(short)w,
					(short)h,
					(unsigned short)(4*totalWidth), 
					(short)sX, (short)sY,
					(short)dX, (short)dY,
					(unsigned char *)source );
#else
			context.Render2DBitmap (sX, sY, dX, dY, w, h, totalWidth, source);
#endif
		}

		public void Render2DBitmap (int srcX, int srcY, int dstX, int dstY, int w, int h, string filename)
		{
			int result;
			CImageFileMemory texFile;
			int totalWidth;
			DWORD				* dataptr;

	
			// Make sure we recognize this file type
			texFile.imageType = CheckImageType (filename);
			Debug.Assert (texFile.imageType != IMAGE_TYPE_UNKNOWN);

			// Open the input file
			result = texFile.glOpenFileMem (filename);
			Debug.Assert (result == 1);

			// Read the image data (note that ReadTextureImage will close texFile for us)
			texFile.glReadFileMem ();
			result = ReadTextureImage (&texFile);
			if (result != GOOD_READ) {
				ShiError ("Failed to read bitmap.  CD Error?");
			}

			// Store the image size (check it in debug mode)
			totalWidth = texFile.image.width;
			Debug.Assert (sX + w <= texFile.image.width);
			Debug.Assert (sY + h <= texFile.image.height);

			// Force the data into 32 bit color
			dataptr = (DWORD*)ConvertImage (&texFile.image, COLOR_16M, NULL);
			Debug.Assert (dataptr);

			// Release the unconverted image data
			// edg: I've seen palette be NULL
			if (texFile.image.palette)
				glReleaseMemory ((char*)texFile.image.palette);
			glReleaseMemory ((char*)texFile.image.image);

			// Pass the bitmap data into the bitmap display function
			Render2DBitmap (sX, sY, dX, dY, w, h, totalWidth, dataptr);

			// Release the converted image data
			glReleaseMemory (dataptr);
		}

		public void ScreenText (float x, float y, string str, int boxed = 0)
		{
			throw new NotImplementedException();
		}

		public virtual void SetLineStyle (int p)
		{
		}

		// Draw a fan with clipping (must set clip flags first)
		public void SetClipFlags (TwoDVertex vert)
		{
			vert.clipFlag = ON_SCREEN;
		
			if (vert.x < leftPixel)		vert.clipFlag |= CLIP_LEFT;
			else if (vert.x > rightPixel)	vert.clipFlag |= CLIP_RIGHT;
		
			if (vert.y < topPixel)			vert.clipFlag |= CLIP_TOP;
			else if (vert.y > bottomPixel)	vert.clipFlag |= CLIP_BOTTOM;
		}

		public void ClipAndDraw2DFan (TwoDVertex[] vertPointers, uint count, bool gifPicture = false)
{
#if TODO
	TwoDVertex[] v, p, lastIn, nextOut;
	TwoDVertex[] inList, outList, temp;
	TwoDVertex[] vertList1= new ThreeDVertex[MAX_VERT_LIST];	// Used to hold poly vert pointer lists
	TwoDVertex[] vertList2 = new ThreeDVertex[MAX_VERT_LIST];	// Used to hold poly vert pointer lists
	DWORD		clipTest = 0;

	Debug.Assert( vertPointers );
	Debug.Assert( count >= 3 );

	// Intialize the vertex buffers
	outList			= vertList1;
	lastIn			= vertPointers + count;
	for (nextOut = outList; vertPointers < lastIn; nextOut++) {
		clipTest |= (*vertPointers).clipFlag;
		*nextOut = (*vertPointers++);
	}
	inList			= vertList2;
	extraVertCount	= 0;


	// Note:  we handle only leading and trailing culled triangles.  If
	// a more complicated non-planar polygon with interior triangles culled is
	// presented, too many triangles will get culled.  To handle that case, we'd
	// have to check all triangles instead of stopping after the second reject loop below.
	// If a new set of un-culled triangles was encountered, we'd have to make a new polygon
	// and resubmit it.
	if ( gifPicture /*|| g_nGfxFix & 0x08*/) //	removed again, caused AG radar not to be displayed on Matrox G400
	{
		temp = inList;
		inList = outList;
		outList = temp;
		lastIn = nextOut-1;
		nextOut = outList;

		// We only support one flavor of clipping right now. The other version would just
		// be this same code repeated with inverted compare signs.
//		Debug.Assert( CullFlag == CULL_ALLOW_CW );

		// Always copy the vertex at the root of the fan
		*nextOut++ = *inList;

		for (p=&inList[0], v=&inList[1]; v < lastIn; v++) {
			// Only clockwise triangles are accepted
			if ( (((*(v+1)).y - (*v).y))*(((*p).x - (*v).x)) < 
				 (((*(v+1)).x - (*v).x))*(((*p).y - (*v).y))   ) {
				// Accept!
				break;
			}
		}
		*nextOut++ = *v;
		for (p, v; v < lastIn; v++) {
			// Only clockwise triangles are accepted
			if ( (((*(v+1)).y - (*v).y))*(((*p).x - (*v).x)) >= 
				 (((*(v+1)).x - (*v).x))*(((*p).y - (*v).y))   ) {
				// Reject!
				break;
			}
			*nextOut++ = *(v+1);
		}
		Debug.Assert( nextOut - outList <= MAX_VERT_LIST );
		if (nextOut - outList <= 2)  return;
	}
	else // do the old code
	{


	// Clip to the bottom plane
	if (clipTest & CLIP_BOTTOM) {
		temp = inList;
		inList = outList;
		outList = temp;
		lastIn = nextOut-1;
		nextOut = outList;

		for (p=lastIn, v=&inList[0]; v <= lastIn; v++) {

			// If the edge between this vert and the previous one crosses the line, trim it
			if (((*p).clipFlag ^ (*v).clipFlag) & CLIP_BOTTOM) {
				Debug.Assert( extraVertCount < MAX_VERT_LIST );
				*nextOut = &extraVerts[extraVertCount];
				extraVertCount++;
				IntersectBottom( *p, *v, *nextOut++ );
			}
			
			// If this vert isn't clipped, use it
			if (!((*v).clipFlag & CLIP_BOTTOM)) {
				*nextOut++ = *v;
			}

			p = v;
		}
		Debug.Assert( nextOut - outList <= MAX_VERT_LIST );
		if (nextOut - outList <= 2)  return;
	}

	
	// Clip to the top plane
	if (clipTest & CLIP_TOP) {
		temp = inList;
		inList = outList;
		outList = temp;
		lastIn = nextOut-1;
		nextOut = outList;

		for (p=lastIn, v=&inList[0]; v <= lastIn; v++) {

			// If the edge between this vert and the previous one crosses the line, trim it
			if (((*p).clipFlag ^ (*v).clipFlag) & CLIP_TOP) {
				Debug.Assert( extraVertCount < MAX_VERT_LIST );
				*nextOut = &extraVerts[extraVertCount];
				extraVertCount++;
				IntersectTop( *p, *v, *nextOut++ );
			}
			
			// If this vert isn't clipped, use it
			if (!((*v).clipFlag & CLIP_TOP)) {
				*nextOut++ = *v;
			}

			p = v;
		}
		Debug.Assert( nextOut - outList <= MAX_VERT_LIST );
		if (nextOut - outList <= 2)  return;
	}


	// Clip to the right plane
	if (clipTest & CLIP_RIGHT) {
		temp = inList;
		inList = outList;
		outList = temp;
		lastIn = nextOut-1;
		nextOut = outList;

		for (p=lastIn, v=&inList[0]; v <= lastIn; v++) {

			// If the edge between this vert and the previous one crosses the line, trim it
			if (((*p).clipFlag ^ (*v).clipFlag) & CLIP_RIGHT) {
				Debug.Assert( extraVertCount < MAX_VERT_LIST );
				*nextOut = &extraVerts[extraVertCount];
				extraVertCount++;
				IntersectRight( *p, *v, *nextOut++ );
			}
			
			// If this vert isn't clipped, use it
			if (!((*v).clipFlag & CLIP_RIGHT)) {
				*nextOut++ = *v;
			}

			p = v;
		}
		Debug.Assert( nextOut - outList <= MAX_VERT_LIST );
		if (nextOut - outList <= 2)  return;
	}

	
	// Clip to the left plane
	if (clipTest & CLIP_LEFT) {
		temp = inList;
		inList = outList;
		outList = temp;
		lastIn = nextOut-1;
		nextOut = outList;

		for (p=lastIn, v=&inList[0]; v <= lastIn; v++) {

			// If the edge between this vert and the previous one crosses the line, trim it
			if (((*p).clipFlag ^ (*v).clipFlag) & CLIP_LEFT) {
				Debug.Assert( extraVertCount < MAX_VERT_LIST );
				*nextOut = &extraVerts[extraVertCount];
				extraVertCount++;
				IntersectLeft( *p, *v, *nextOut++ );
			}
			
			// If this vert isn't clipped, use it
			if (!((*v).clipFlag & CLIP_LEFT)) {
				*nextOut++ = *v;
			}

			p = v;
		}
		Debug.Assert( nextOut - outList <= MAX_VERT_LIST );
		if (nextOut - outList <= 2)  return;
	}


	}

	// Finally draw the resultant polygon
// OW
#if NOTHING
	context.Primitive(MPR_PRM_TRIFAN,
						MPR_VI_COLOR | MPR_VI_TEXTURE,
						(unsigned short)(nextOut - outList), sizeof(MPRVtxTexClr_t) );
	
	for (v=outList; v<nextOut; v++) {
		context.StorePrimitiveVertexData(*v);
	}
#else
	context.DrawPrimitive(MPR_PRM_TRIFAN, MPR_VI_COLOR | MPR_VI_TEXTURE,
		(ushort)(nextOut - outList), (MPRVtxTexClr_t **) outList);
#endif
#endif 
			throw new NotImplementedException();
}

  
		// Window and rendering context handles
		public ContextMPR			context;
		protected ImageBuffer		image;
  
		private void IntersectTop (TwoDVertex v1, TwoDVertex v2, out TwoDVertex v)
			{
	float	t;

	// Compute the parametric location of the intersection of the edge and the clip plane
	t = (topPixel - v1.y) / (v2.y - v1.y);
	Debug.Assert( (t >= -0.001f) && (t <= 1.001f) );
	
	// Compute the camera space intersection point
	v.y = topPixel;
	v.x = v1.x + t * (v2.x - v1.x);

	// Compute the interpolated color and texture coordinates
	InterpolateColorAndTex( v1, v2, v, t );


	// Now determine if the point is out to the sides
	if (v.x < leftPixel) {
		v.clipFlag = CLIP_LEFT;
	} else if (v.x > rightPixel) {
		v.clipFlag = CLIP_RIGHT;
	} else {
		v.clipFlag = ON_SCREEN;
	}
}

		private void IntersectBottom (TwoDVertex v1, TwoDVertex v2, out TwoDVertex v)
		{
	float	t;

	// Compute the parametric location of the intersection of the edge and the clip plane
	t = (bottomPixel - v1.y) / (v2.y - v1.y);
	Debug.Assert( (t >= -0.001f) && (t <= 1.001f) );
	
	// Compute the camera space intersection point
	v.y = bottomPixel;
	v.x = v1.x + t * (v2.x - v1.x);

	// Compute the interpolated color and texture coordinates
	InterpolateColorAndTex( v1, v2, v, t );


	// Now determine if the point is out to the sides
	if (v.x < leftPixel) {
		v.clipFlag = CLIP_LEFT;
	} else if (v.x > rightPixel) {
		v.clipFlag = CLIP_RIGHT;
	} else {
		v.clipFlag = ON_SCREEN;
	}
}

		private void IntersectLeft (TwoDVertex v1, TwoDVertex v2, out TwoDVertex v)
			{
	float	t;

	// Compute the parametric location of the intersection of the edge and the clip plane
	t = (leftPixel - v1.x) / (v2.x - v1.x);
	Debug.Assert( (t >= -0.001f) && (t <= 1.001f) );
	
	// Compute the camera space intersection point
	v.x = leftPixel;
	v.y = v1.y + t * (v2.y - v1.y);
	v.clipFlag = ON_SCREEN;

	// Compute the interpolated color and texture coordinates
	InterpolateColorAndTex( v1, v2, v, t );
}

		private void IntersectRight (TwoDVertex v1, TwoDVertex v2, out TwoDVertex v)
			{
	float	t;

	// Compute the parametric location of the intersection of the edge and the clip plane
	t = (rightPixel - v1.x) / (v2.x - v1.x);
	Debug.Assert( (t >= -0.001f) && (t <= 1.001f) );
	
	// Compute the camera space intersection point
	v.x = rightPixel;
	v.y = v1.y + t * (v2.y - v1.y);
	v.clipFlag = ON_SCREEN;

	// Compute the interpolated color and texture coordinates
	InterpolateColorAndTex( v1, v2, v, t );
}
		private  void InterpolateColorAndTex( TwoDVertex v1, TwoDVertex v2, out TwoDVertex v, float t )
		{
			// Compute the interpolated color and texture coordinates
			v.r = v1.r + t*(v2.r - v1.r);
			v.g = v1.g + t*(v2.g - v1.g);
			v.b = v1.b + t*(v2.b - v1.b);
			v.a = v1.a + t*(v2.a - v1.a);
					 		   		   
			v.u = v1.u + t*(v2.u - v1.u);
			v.v = v1.v + t*(v2.v - v1.v);
		}

	};
}

