using System;
using FalconNet.Common;
using DWORD = System.UInt32;
using System.Diagnostics;

namespace FalconNet.Graphics
{
    public class TwoDVertex
    {
        public float x, y;
        public float r, g, b, a;
        public float u, v, q;
        public ClippingFlags clipFlag;
    }

    public class Render2D : VirtualDisplay
    {

        public Render2D()
        {
        }
        //public virtual ~Render2D()	{};

        public virtual void Setup(ImageBuffer imageBuffer)
        {
#if TODO
            bool result;

            image = imageBuffer;

            // Create the MPR rendering context (frame buffer, etc.)

            // OW
            //result = context.Setup( (DWORD)imageBuffer.targetSurface(), (DWORD)imageBuffer.GetDisplayDevice().GetMPRdevice());
            result = context.Setup(imageBuffer, imageBuffer.GetDisplayDevice().GetDefaultRC());

            if (!result)
            {
                throw new Exception("Failed to setup rendering context");
            }

            // Store key properties of our target buffer
            xRes = image.targetXres();
            yRes = image.targetYres();


            // Set the renderer's default foreground and background colors
            SetColor((short)(0xFFFFFFFF));
            SetBackground(0xFF000000);
#endif
            // Call our base classes setup function (must come AFTER xRes and Yres have been set)
            base.Setup();
        }

        public override void Cleanup()
        {
            context.Cleanup();

            base.Cleanup();
        }

        public virtual void SetImageBuffer(ImageBuffer imageBuffer)
        {
#if TODO
            // Remember who our new image buffer is, and tell MPR about the change
            image = imageBuffer;
            context.NewImageBuffer((DWORD)imageBuffer.targetSurface());

            // This shouldn't be required, but _might_ be
            context.InvalidateState();
            context.RestoreState(STATE_SOLID);

            // Store key properties of our target buffer
            xRes = image.targetXres();
            yRes = image.targetYres();

            // Setup the default viewport
            SetViewport(-1.0f, 1.0f, 1.0f, -1.0f);

            // Setup the default offset and rotation
            CenterOriginInViewport();
            ZeroRotationAboutOrigin();
#endif
            throw new NotImplementedException();
        }

        public ImageBuffer GetImageBuffer()
        {
            return image;
        }

        public override void StartFrame()
        {
            Debug.Assert(image!= null);
            context.StartFrame();
         }

        public override void ClearFrame()
        {
            context.ClearBuffers(Mpr_light.MPR_CI_DRAW_BUFFER);
        }

        public override void FinishFrame()
        {
            Debug.Assert(image!= null);
            context.FinishFrame(null);
        }

        public override void SetViewport(float leftSide, float topSide, float rightSide, float bottomSide)
        {
            // First call the base classes version of this function
            base.SetViewport(leftSide, topSide, rightSide, bottomSide);

            // Send the new clipping region to MPR
            // (top/right inclusive, bottom/left exclusive)
            context.SetState(MPR_STA.MPR_STA_ENABLES, Mpr_light.MPR_SE_SCISSORING);
            context.SetState(MPR_STA.MPR_STA_SCISSOR_TOP, (int)((float)Math.Floor(topPixel)));
            context.SetState(MPR_STA.MPR_STA_SCISSOR_LEFT, (int)((float)Math.Floor(leftPixel)));
            context.SetState(MPR_STA.MPR_STA_SCISSOR_RIGHT, (int)((float)Math.Ceiling(rightPixel)));
            context.SetState(MPR_STA.MPR_STA_SCISSOR_BOTTOM, (int)((float)Math.Ceiling(bottomPixel)));
        }

        public override int Color()
        {
            return context.CurrentForegroundColor();
         }

        public override void SetColor(DWORD packedRGBA)
        {
            context.RestoreState(MPRState.STATE_SOLID);
            context.SelectForegroundColor(packedRGBA);
         }

        public override void SetBackground(DWORD packedRGBA)
        {
            context.SetState(MPR_STA.MPR_STA_BG_COLOR, packedRGBA);
        }

        public override void Render2DPoint(float x1, float y1)
        {
#if ! NOTHING
            context.Draw2DPoint(x1, y1);
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

        public override void Render2DLine(float x1, float y1, float x2, float y2)
        {
#if ! NOTHING
            context.Draw2DLine(x1, y1, x2, y2);
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

        public override void Render2DTri(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            MPRVtx_t[] verts = new MPRVtx_t[3];

            //Clip test
            if ((Math.Max(Math.Max(x1, x2), x3) > rightPixel) ||
                  (Math.Min(Math.Min(x1, x2), x3) < leftPixel) ||
                  (Math.Max(Math.Max(y1, y2), y3) > bottomPixel) ||
                  (Math.Min(Math.Min(y1, y2), y3) < topPixel)
                  )
                return;

            // Package up the tri's coordinates
            verts[0].x = x1;
            verts[0].y = y1;
            verts[1].x = x2;
            verts[1].y = y2;
            verts[2].x = x3;
            verts[2].y = y3;

            // Draw the triangle
            //	context.RestoreState( STATE_ALPHA_SOLID );
            context.DrawPrimitive(Mpr_light.MPR_PRM_TRIANGLES, 0, 3, verts, 2*sizeof(float));
        }

        public void Render2DBitmap(int srcX, int srcY, int dstX, int dstY, int w, int h, int totalWidth, byte[] source)
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
            context.Render2DBitmap(srcX, srcY, dstX, dstY, w, h, totalWidth, source);
#endif
        }

        public void Render2DBitmap(int srcX, int srcY, int dstX, int dstY, int w, int h, string filename)
        {
#if TODO
            int result;
            CImageFileMemory texFile;
            int totalWidth;
            DWORD* dataptr;


            // Make sure we recognize this file type
            texFile.imageType = CheckImageType(filename);
            Debug.Assert(texFile.imageType != ImageType.IMAGE_TYPE_UNKNOWN);

            // Open the input file
            result = texFile.glOpenFileMem(filename);
            Debug.Assert(result == 1);

            // Read the image data (note that ReadTextureImage will close texFile for us)
            texFile.glReadFileMem();
            result = ReadTextureImage(&texFile);
            if (result != GOOD_READ)
            {
                throw new Exception("Failed to read bitmap.  CD Error?");
            }

            // Store the image size (check it in debug mode)
            totalWidth = texFile.image.width;
            Debug.Assert(sX + w <= texFile.image.width);
            Debug.Assert(sY + h <= texFile.image.height);

            // Force the data into 32 bit color
            dataptr = (DWORD*)ConvertImage(&texFile.image, COLOR_16M, null);
            Debug.Assert(dataptr);

            // Release the unconverted image data
            // edg: I've seen palette be null
            if (texFile.image.palette)
                glReleaseMemory((char*)texFile.image.palette);
            glReleaseMemory((char*)texFile.image.image);

            // Pass the bitmap data into the bitmap display function
            Render2DBitmap(sX, sY, dX, dY, w, h, totalWidth, dataptr);

            // Release the converted image data
            glReleaseMemory(dataptr);
#endif
            throw new NotImplementedException();
        }

        public void ScreenText(float x, float y, string str, int boxed = 0)
        {
            throw new NotImplementedException();
        }

        public virtual void SetLineStyle(int p)
        {
        }

        // Draw a fan with clipping (must set clip flags first)
        public void SetClipFlags(TwoDVertex vert)
        {
            vert.clipFlag = ClippingFlags.ON_SCREEN;

            if (vert.x < leftPixel) vert.clipFlag |= ClippingFlags.CLIP_LEFT;
            else if (vert.x > rightPixel) vert.clipFlag |= ClippingFlags.CLIP_RIGHT;

            if (vert.y < topPixel) vert.clipFlag |= ClippingFlags.CLIP_TOP;
            else if (vert.y > bottomPixel) vert.clipFlag |= ClippingFlags.CLIP_BOTTOM;
        }

        public void ClipAndDraw2DFan(TwoDVertex[] vertPointers, uint count, bool gifPicture = false)
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
	if ( gifPicture /*|| F4Config.g_nGfxFix & 0x08*/) //	removed again, caused AG radar not to be displayed on Matrox G400
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
        public IContext context;
        protected ImageBuffer image;

        private void IntersectTop(TwoDVertex v1, TwoDVertex v2, ref TwoDVertex v)
        {
            float t;

            // Compute the parametric location of the intersection of the edge and the clip plane
            t = (topPixel - v1.y) / (v2.y - v1.y);
            Debug.Assert((t >= -0.001f) && (t <= 1.001f));

            // Compute the camera space intersection point
            v.y = topPixel;
            v.x = v1.x + t * (v2.x - v1.x);

            // Compute the interpolated color and texture coordinates
            InterpolateColorAndTex(v1, v2, ref v, t);


            // Now determine if the point is out to the sides
            if (v.x < leftPixel)
            {
                v.clipFlag = ClippingFlags.CLIP_LEFT;
            }
            else if (v.x > rightPixel)
            {
                v.clipFlag = ClippingFlags.CLIP_RIGHT;
            }
            else
            {
                v.clipFlag = ClippingFlags.ON_SCREEN;
            }
        }

        private void IntersectBottom(TwoDVertex v1, TwoDVertex v2, ref TwoDVertex v)
        {
            float t;

            // Compute the parametric location of the intersection of the edge and the clip plane
            t = (bottomPixel - v1.y) / (v2.y - v1.y);
            Debug.Assert((t >= -0.001f) && (t <= 1.001f));

            // Compute the camera space intersection point
            v.y = bottomPixel;
            v.x = v1.x + t * (v2.x - v1.x);

            // Compute the interpolated color and texture coordinates
            InterpolateColorAndTex(v1, v2, ref v, t);


            // Now determine if the point is out to the sides
            if (v.x < leftPixel)
            {
                v.clipFlag = ClippingFlags.CLIP_LEFT;
            }
            else if (v.x > rightPixel)
            {
                v.clipFlag = ClippingFlags.CLIP_RIGHT;
            }
            else
            {
                v.clipFlag = ClippingFlags.ON_SCREEN;
            }
        }

        private void IntersectLeft(TwoDVertex v1, TwoDVertex v2, ref TwoDVertex v)
        {
            float t;

            // Compute the parametric location of the intersection of the edge and the clip plane
            t = (leftPixel - v1.x) / (v2.x - v1.x);
            Debug.Assert((t >= -0.001f) && (t <= 1.001f));

            // Compute the camera space intersection point
            v.x = leftPixel;
            v.y = v1.y + t * (v2.y - v1.y);
            v.clipFlag = ClippingFlags.ON_SCREEN;

            // Compute the interpolated color and texture coordinates
            InterpolateColorAndTex(v1, v2, ref v, t);
        }

        private void IntersectRight(TwoDVertex v1, TwoDVertex v2, ref TwoDVertex v)
        {
            float t;

            // Compute the parametric location of the intersection of the edge and the clip plane
            t = (rightPixel - v1.x) / (v2.x - v1.x);
            Debug.Assert((t >= -0.001f) && (t <= 1.001f));

            // Compute the camera space intersection point
            v.x = rightPixel;
            v.y = v1.y + t * (v2.y - v1.y);
            v.clipFlag = ClippingFlags.ON_SCREEN;

            // Compute the interpolated color and texture coordinates
            InterpolateColorAndTex(v1, v2, ref v, t);
        }

        private void InterpolateColorAndTex(TwoDVertex v1, TwoDVertex v2, ref TwoDVertex v, float t)
        {
            // Compute the interpolated color and texture coordinates
            v.r = v1.r + t * (v2.r - v1.r);
            v.g = v1.g + t * (v2.g - v1.g);
            v.b = v1.b + t * (v2.b - v1.b);
            v.a = v1.a + t * (v2.a - v1.a);

            v.u = v1.u + t * (v2.u - v1.u);
            v.v = v1.v + t * (v2.v - v1.v);
        }

    }
}

