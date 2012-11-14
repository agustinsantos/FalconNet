using System;
using System.Diagnostics;
using FalconNet.Common;

namespace FalconNet.Graphics
{
    public struct FontDataType
    {
        public float top;
        public float left;
        public float width;
        public float height;
        public float pixelWidth;
        public float pixelHeight;
    };

    // Clipping flags.  Some of these are only used in 3D clipping, but I want to keep them together.
    [Flags]
    public enum ClippingFlags
    {
        ON_SCREEN = 0x00,
        CLIP_LEFT = 0x01,
        CLIP_RIGHT = 0x02,
        CLIP_TOP = 0x04,
        CLIP_BOTTOM = 0x08,
        CLIP_NEAR = 0x10,
        CLIP_FAR = 0x20,
        OFF_SCREEN = 0xFF
    }

    public struct DisplayMatrix
    { // JPO - how a display is oriented
        public float translationX, translationY;
        public float rotation00, rotation01;
        public float rotation10, rotation11;
    };

    public abstract class VirtualDisplay
    {
        public const int NUM_FONT_RESOLUTIONS = 4;
        public const int CircleStep = 4;							// In units of degrees
        public const int CircleSegments = 360 / CircleStep + 1;	// How many segments (plus one)?

        public VirtualDisplay()
        {
            ready = false;
        }

        // public virtual ~VirtualDisplay()	{ Debug.Assert( ready == false ); };

        // One time call to create inverse font
        public static void InitializeFonts()
        {
#if TODO
			int c, r;

			for (c=0; c<VirtualDisplay.FontLength; c++) {
				// Shift each row right one to make room for the edging
				for (r=0; r<8; r++) {
					InvFont [c] [r] = (byte)(~(Font [c] [r] >> 1));
				}
			}
#endif
        }

        // Parents Setup() must set xRes and yRes before call this...
        /***************************************************************************\
        Setup the rendering context for this display
        \***************************************************************************/
        public virtual void Setup()
        {
            Debug.Assert(!IsReady());

            // Setup the default viewport
            SetViewport(-1.0f, 1.0f, 1.0f, -1.0f);

            // Setup the default offset and rotation
            CenterOriginInViewport();
            ZeroRotationAboutOrigin();

            // Initialize the unit circle array (in case someone else hasn't done it already)
            double angle;
            int entry;
            for (entry = 0; entry < CircleSegments; entry++)
            {
                angle = (entry * CircleStep) * Math.PI / 180.0;

                CircleX[entry] = (float)Math.Cos(angle);
                CircleY[entry] = -(float)Math.Sin(angle);	// Account for the y axis flip
            }

            type = DisplayType.DISPLAY_GENERAL;

            ready = true;
        }

        /***************************************************************************\
            Shutdown the rendering context for this display
        \***************************************************************************/
        public virtual void Cleanup()
        {
            Debug.Assert(IsReady());
            ready = false;
        }

        public bool IsReady()
        {
            return ready;
        }

        public abstract void StartFrame();

        public abstract void ClearFrame();

        public abstract void FinishFrame();

        /***************************************************************************\
            Put a pixel on the display.
        \***************************************************************************/
        public virtual void Point(float x1, float y1)
        {
            float x, y;

            // Rotation and translate this point based on the current settings
            x = x1 * dmatrix.rotation00 + y1 * dmatrix.rotation01 + dmatrix.translationX;
            y = x1 * dmatrix.rotation10 + y1 * dmatrix.rotation11 + dmatrix.translationY;

            // Clipping
            if ((x >= -1.0f) && (x <= 1.0f) && (y <= 1.0f) && (y >= -1.0f))
            {

                // Convert to pixel coordinates and draw the point on the display
                Render2DPoint(viewportXtoPixel(x), viewportYtoPixel(-y));

            }
        }

        /***************************************************************************\
            Put a one pixel wide line on the display
        \***************************************************************************/
        public virtual void Line(float x1, float y1, float x2, float y2)
        {
            float x;
            ClippingFlags clipFlag = ClippingFlags.ON_SCREEN;

            // Rotation and translate this point based on the current settings
            x = x1 * dmatrix.rotation00 + y1 * dmatrix.rotation01 + dmatrix.translationX;
            y1 = x1 * dmatrix.rotation10 + y1 * dmatrix.rotation11 + dmatrix.translationY;
            x1 = x;

            x = x2 * dmatrix.rotation00 + y2 * dmatrix.rotation01 + dmatrix.translationX;
            y2 = x2 * dmatrix.rotation10 + y2 * dmatrix.rotation11 + dmatrix.translationY;
            x2 = x;


            // Clip point 1
            clipFlag = ClippingFlags.ON_SCREEN;
            if (x1 < -1.0f)
            {
                y1 = y2 + (y1 - y2) * ((x2 + 1.0f) / (x2 - x1));
                x1 = -1.0f;
                clipFlag = ClippingFlags.CLIP_LEFT;
            }
            else if (x1 > 1.0f)
            {
                y1 = y2 + (y1 - y2) * ((x2 - 1.0f) / (x2 - x1));
                x1 = 1.0f;
                clipFlag = ClippingFlags.CLIP_RIGHT;
            }
            if (y1 < -1.0f)
            {
                x1 = x2 + (x1 - x2) * ((y2 + 1.0f) / (y2 - y1));
                y1 = -1.0f;
                clipFlag |= ClippingFlags.CLIP_BOTTOM;
            }
            else if (y1 > 1.0f)
            {
                x1 = x2 + (x1 - x2) * ((y2 - 1.0f) / (y2 - y1));
                y1 = 1.0f;
                clipFlag |= ClippingFlags.CLIP_TOP;
            }

            // Clip point 2
            if (x2 < -1.0f)
            {
                y2 = y1 + (y2 - y1) * ((x1 + 1.0f) / (x1 - x2));
                x2 = -1.0f;
                if (clipFlag.IsFlagSet(ClippingFlags.CLIP_LEFT))
                    return;
            }
            else if (x2 > 1.0f)
            {
                y2 = y1 + (y2 - y1) * ((x1 - 1.0f) / (x1 - x2));
                x2 = 1.0f;
                if (clipFlag.IsFlagSet(ClippingFlags.CLIP_RIGHT))
                    return;
            }
            if (y2 < -1.0f)
            {
                x2 = x1 + (x2 - x1) * ((y1 + 1.0f) / (y1 - y2));
                y2 = -1.0f;
                if (clipFlag.IsFlagSet(ClippingFlags.CLIP_BOTTOM))
                    return;
            }
            else if (y2 > 1.0f)
            {
                x2 = x1 + (x2 - x1) * ((y1 - 1.0f) / (y1 - y2));
                y2 = 1.0f;
                if (clipFlag.IsFlagSet(ClippingFlags.CLIP_TOP))
                    return;
            }

            Render2DLine(viewportXtoPixel(x1),
                  viewportYtoPixel(-y1),
                  viewportXtoPixel(x2),
                  viewportYtoPixel(-y2));
        }


        /***************************************************************************\
            Put a triangle on the display.  It is not filled (for now at least)
        \***************************************************************************/
        public virtual void Tri(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            float x;

            // Rotation and translate this point based on the current settings
            x = x1 * dmatrix.rotation00 + y1 * dmatrix.rotation01 + dmatrix.translationX;
            y1 = x1 * dmatrix.rotation10 + y1 * dmatrix.rotation11 + dmatrix.translationY;
            x1 = x;

            x = x2 * dmatrix.rotation00 + y2 * dmatrix.rotation01 + dmatrix.translationX;
            y2 = x2 * dmatrix.rotation10 + y2 * dmatrix.rotation11 + dmatrix.translationY;
            x2 = x;

            x = x3 * dmatrix.rotation00 + y3 * dmatrix.rotation01 + dmatrix.translationX;
            y3 = x3 * dmatrix.rotation10 + y3 * dmatrix.rotation11 + dmatrix.translationY;
            x3 = x;

            Render2DTri(viewportXtoPixel(x1), viewportYtoPixel(-y1),
                         viewportXtoPixel(x2), viewportYtoPixel(-y2),
                         viewportXtoPixel(x3), viewportYtoPixel(-y3));
        }




        /***************************************************************************\
            Draw a circle in the viewport.  The radius is given indpendently
            in the x and y direction.
        \***************************************************************************/
        public virtual void Oval(float x, float y, float xRadius, float yRadius)
        {
            int entry;

            float x1, y1;
            float x2, y2;

            // Prime the pump
            x1 = x + xRadius * CircleX[0];
            y1 = y + yRadius * CircleY[0];

            for (entry = 1; entry <= CircleSegments - 1; entry++)
            {

                // Compute the end point of this next segment
                x2 = (x + xRadius * CircleX[entry]);
                y2 = (y + yRadius * CircleY[entry]);

                // Draw the segment
                Line(x1, y1, x2, y2);

                // Save the end point of this one to use as the start point of the next one
                x1 = x2;
                y1 = y2;
            }
        }

        /***************************************************************************\
            Draw a portion of a circle in the viewport.  The radius given is in
            the X direction.  The Y direction will be scaled to account for the
            aspect ratio of the display and viewport.  The start and stop angles
            will be adjusted lie between 0 and 2PI.
        \***************************************************************************/
        public virtual void OvalArc(float x, float y, float xRadius, float yRadius, float start, float stop)
        {
            int entry, startEntry, stopEntry;

            // Find the first and last segment end point of interest
            startEntry = (int)((start % (2.0f * Math.PI)) / Math.PI * 180.0) / CircleStep;
            stopEntry = (int)((stop % (2.0f * Math.PI)) / Math.PI * 180.0) / CircleStep;

            // Make sure we aren't overrunning the precomputed array
            Debug.Assert(startEntry >= 0);
            Debug.Assert(stopEntry >= 0);
            Debug.Assert(startEntry < CircleSegments);
            Debug.Assert(stopEntry < CircleSegments);

            if (startEntry <= stopEntry)
            {
                for (entry = startEntry; entry < stopEntry; entry++)
                {
                    Line(x + xRadius * CircleX[entry], y + yRadius * CircleY[entry],
                      x + xRadius * CircleX[entry + 1], y + yRadius * CircleY[entry + 1]);
                }
            }
            else
            {
                for (entry = startEntry; entry < CircleSegments - 1; entry++)
                {
                    Line(x + xRadius * CircleX[entry], y + yRadius * CircleY[entry],
                      x + xRadius * CircleX[entry + 1], y + yRadius * CircleY[entry + 1]);
                }
                for (entry = 0; entry < stopEntry; entry++)
                {
                    Line(x + xRadius * CircleX[entry], y + yRadius * CircleY[entry],
                      x + xRadius * CircleX[entry + 1], y + yRadius * CircleY[entry + 1]);
                }
            }
        }

        public virtual void Circle(float x, float y, float xRadius)
        {
            Oval(x, y, xRadius, xRadius * scaleX / scaleY);
        }

        public virtual void Arc(float x, float y, float xRadius, float start, float stop)
        {
            OvalArc(x, y, xRadius, xRadius * scaleX / scaleY, start, stop);
        }

        /***************************************************************************\
            Put a mono-colored string of text on the display.
            (The location given is used as the lower left corner of the text)
        \***************************************************************************/
        public virtual void TextLeft(float x1, float y1, string str, int boxed = 0)
        {
            float x, y;

            // Rotation and translate this point based on the current settings
            x = x1 * dmatrix.rotation00 + y1 * dmatrix.rotation01 + dmatrix.translationX;
            y = x1 * dmatrix.rotation10 + y1 * dmatrix.rotation11 + dmatrix.translationY;

            // Convert from viewport coordiants to screen space and draw the string
            ScreenText(viewportXtoPixel(x), viewportYtoPixel(-y), str, boxed);
        }

        public virtual void TextLeftVertical(float x1, float y1, string str, int boxed = 0)
        {
            float x, y;

            // Rotation and translate this point based on the current settings
            x = viewportXtoPixel(x1 * dmatrix.rotation00 + y1 * dmatrix.rotation01 + dmatrix.translationX);
            y = viewportYtoPixel(-x1 * dmatrix.rotation10 - y1 * dmatrix.rotation11 - dmatrix.translationY);

            y -= ScreenTextHeight() / 2;

            // Convert from viewport coordiants to screen space and draw the string
            ScreenText(x, y, str, boxed);
        }

        /***************************************************************************\
            Put a mono-colored string of text on the display.
            (The location given is used as the lower right corner of the text)
        \***************************************************************************/
        public virtual void TextRight(float x1, float y1, string str, int boxed = 0)
        {
            float xPixel, yPixel;

            // Rotation and translate this point based on the current settings
            // Convert from viewport coordiants to screen space
            xPixel = viewportXtoPixel(x1 * dmatrix.rotation00 + y1 * dmatrix.rotation01 + dmatrix.translationX);
            yPixel = viewportYtoPixel(-x1 * dmatrix.rotation10 - y1 * dmatrix.rotation11 - dmatrix.translationY);

            // Adjust our starting point in screen space to get proper alignment
            xPixel -= ScreenTextWidth(str);

            // Draw the string on the screen
            ScreenText(xPixel, yPixel, str, boxed);
        }

        public virtual void TextRightVertical(float x1, float y1, string str, int boxed = 0)
        {
            float xPixel, yPixel;

            // Rotation and translate this point based on the current settings
            // Convert from viewport coordiants to screen space
            xPixel = viewportXtoPixel(x1 * dmatrix.rotation00 + y1 * dmatrix.rotation01 + dmatrix.translationX);
            yPixel = viewportYtoPixel(-x1 * dmatrix.rotation10 - y1 * dmatrix.rotation11 - dmatrix.translationY);

            // Adjust our starting point in screen space to get proper alignment
            xPixel -= ScreenTextWidth(str);
            yPixel -= ScreenTextHeight() / 2;

            // Draw the string on the screen
            ScreenText(xPixel, yPixel, str, boxed);
        }


        /***************************************************************************\
            Put a mono-colored string of text on the display.
            (The location given is used as the horizontal center and vertical lower
             edge of the text)
        \***************************************************************************/
        public virtual void TextCenter(float x1, float y1, string str, int boxed = 0)
        {
            float xPixel, yPixel;

            // Rotation and translate this point based on the current settings
            // Convert from viewport coordiants to screen space
            xPixel = viewportXtoPixel(x1 * dmatrix.rotation00 + y1 * dmatrix.rotation01 + dmatrix.translationX);
            yPixel = viewportYtoPixel(-x1 * dmatrix.rotation10 - y1 * dmatrix.rotation11 - dmatrix.translationY);

            // Adjust our starting point in screen space to get proper alignment
            xPixel -= ScreenTextWidth(str) / 2;

            // Draw the string on the screen
            ScreenText(xPixel, yPixel, str, boxed);
        }

        /***************************************************************************\
            Put a mono-colored string of text on the display.
            (The location given is used as the horizontal center and vertical lower
             edge of the text)
        \***************************************************************************/
        public virtual void TextCenterVertical(float x1, float y1, string str, int boxed = 0)
        {
            float xPixel, yPixel;

            // Rotation and translate this point based on the current settings
            // Convert from viewport coordiants to screen space
            xPixel = viewportXtoPixel(x1 * dmatrix.rotation00 + y1 * dmatrix.rotation01 + dmatrix.translationX);
            yPixel = viewportYtoPixel(-x1 * dmatrix.rotation10 - y1 * dmatrix.rotation11 - dmatrix.translationY);

            // Adjust our starting point in screen space to get proper alignment
            xPixel -= ScreenTextWidth(str) / 2;
            yPixel -= ScreenTextHeight() / 2;

            // Draw the string on the screen
            ScreenText(xPixel, yPixel, str, boxed);
        }

        /***************************************************************************\
            Print the string on multiple lines.  h,v is the starting point.
            spacing is the line height and width is the line width.
            THIS ASSUMES that no single word is longer than will fit in the
            specified width.
        \***************************************************************************/
        public virtual int TextWrap(float h, float v, string str, float spacing, float width)
        {
#if TODO
			int pixelsLeft;
			char lineBreak;
			char prevChar;
			int line = 0;

			// While we have more to display
			while (*str) {

				// We start from the end of the string
				lineBreak = str + strlen (str);
				Debug.Assert (*lineBreak == '\0');	
				prevChar = '\0';
				pixelsLeft = FloatToInt32 (width * scaleX) - ScreenTextWidth (str);

				// While we need to shorten the string...
				while (pixelsLeft < 0) {

					// Take the synthetic null back out
			*
					lineBreak = prevChar;

					// Find the next word to slice off
					do {
						lineBreak--;
						Debug.Assert (lineBreak >= str);
					} while (*lineBreak != ' ');
					do {
						lineBreak--;
						Debug.Assert (lineBreak >= str);
					} while (*lineBreak == ' ');
					lineBreak++;	// Step back to the first space after the word

					// Save the existing character, then insert null terminator
					prevChar = *lineBreak;
			*
					lineBreak = '\0';

					// Recompute the length of the trimmed string
					pixelsLeft = FloatToInt32 (width * scaleX) - ScreenTextWidth (str);
				}

				// Now print the trimmed string
				TextLeft (h, v - line * spacing, str);

				// Take the synthetic null back out and advance
		*
				lineBreak = prevChar;
				str = lineBreak;
				line++;

				// Skip any extra white space
				while (*str == ' ') {
					str++;
				}
			}

			return line;
#endif
            throw new NotImplementedException();
        }

        // NOTE:  These might need to be virtualized and overloaded by canvas3d (maybe???)
        public virtual float TextWidth(string str)
        {
            return ScreenTextWidth(str) / scaleX;
        }	// normalized screen space

        public virtual float TextHeight()
        {
            return ScreenTextHeight() / scaleY;
        }		// normalized screen space

        // Screen space text printing (based on upper left starting pixel)
        /***************************************************************************\
            Put a mono-colored string of text on the display in screen space.
            (The location given is used as the upper left corner of the text in units of pixels)
        \***************************************************************************/
        public virtual void ScreenText(float xLeft, float yTop, string str, int boxed = 0)
        {
#if TODO
	int			x, y;
	int			width;
	uint	num;


	x = FloatToInt32( xLeft );
	y = FloatToInt32( yTop );

	// If this string falls off the top or bottom of the viewport, don't draw it
	if ((y+8 > bottomPixel) || (y < topPixel)) {
		return;
	}


	// Draw the string one character at a time
	while (*str) {

		// Draw only if we're on the screen
		if ((x + 7 >= rightPixel) || (x < leftPixel)) {
			width = 5;
		} else {

			// Get the font array offset
			num = FontLUT[*(byte[]  *)str];
			Debug.Assert( num < FontLength );

			width = Font[num][8];
			if (boxed == 2) {
				const byte[]  *map = InvFont[num];
				for (int r=0; r<8; r++)	{
					for (int c=0; c<width; c++) {
						// Draw the pixels which need to be turned "ON"
						if ( map[r] & (1 << (7-c)) ) {
							Render2DPoint( (float)(x+c-1), (float)(y+r-1) );
						}
					}
				}
			} else {
				const byte[]  *map = Font[num];
				for (int r=1; r<7; r++)	{
					for (int c=0; c<width; c++) {
						// Draw the pixels which need to be turned "ON"
						if ( map[r] & (1 << (7-c)) ) {
							Render2DPoint( (float)(x+c), (float)(y+r) );
						}
					}
				}
			}
		}

		// Update the target location for the next character
		str++;

		x += width+1;
	}

	if (boxed == 1)
	{
		float x1 = xLeft - 2.0f;
		float y1 = yTop  - 2.0f;
		float x2 = (float)(x + 1);
		float y2 = yTop +  7.0f;
		
      x1 = max (x1, 0.0F);
		Render2DLine (x1, y1, x2, y1);
		Render2DLine (x2, y1, x2, y2);
		Render2DLine (x2, y2, x1, y2);
		Render2DLine (x1, y2, x1, y1);
	}
#endif
        }

        /***************************************************************************\
            This function is used by 3d canvas and is needed to do the boxed strings
            since we do 1 char at a time in the canvas
        \***************************************************************************/
        public virtual void ScreenChar(float xLeft, float yTop, string str, int boxed = 0)
        {
#if TODO
	int			x, y;
	unsigned	num;
	int			width;
	float 		x1, y1, x2, y2;
	int			boxstart;


	x = FloatToInt32( xLeft );
	y = FloatToInt32( yTop );

	// If this string falls off the top or bottom of the viewport, don't draw it
	if ((y+8 > bottomPixel) || (y < topPixel) || x < leftPixel || x > rightPixel)
	{
		return;
	}

	boxstart = boxed & 0x10000000;
	boxed &= 0x0fffffff;

	// boxed 2 is inverse
	// for boxed 1, 4 and 8 we'll just use vert bars.
	// if *string is null and boxed is a box, that means we draw a vert bar
	if ( *str == 0 &&
		 (boxed == 1 || boxed ==4 || boxed == 8 ) )
	{
		// we extend 2 pixels above and 2 below for box
		y -= 1;

		if ((y+9 > bottomPixel) || (y < topPixel))
		{
			return;
		}

		x1 = xLeft;
		y1 = yTop  - 1.0f;
		x2 = xLeft;
		y2 = yTop +  8.0f;
		
		Render2DLine (x1, y1, x2, y2);

		if ( boxstart )
		{
			Render2DPoint( x1 + 1.0f, y1 );
			Render2DPoint( x1 + 1.0f, y2 );
		}
		else
		{
			Render2DPoint( x1 - 1.0f, y1 );
			Render2DPoint( x1 - 1.0f, y2 );
		}

		return;
	}


	// Draw the string one character at a time
	while (*str) {

		// Draw only if we're on the screen
		if ((x + 7 >= rightPixel) || (x < leftPixel)) {
			width = 5;
		} else {

			// Get the font array offset
			num = FontLUT[*(byte[]  *)str];
			Debug.Assert( num < FontLength );

			width = Font[num][8];
			if (boxed == 2) {
				const byte[]  *map = InvFont[num];
				for (int r=0; r<8; r++)	{
					for (int c=0; c<width; c++) {
						// Draw the pixels which need to be turned "ON"
						if ( map[r] & (1 << (7-c)) ) {
							Render2DPoint( (float)(x+c-1), (float)(y+r-1) );
						}
					}
				}
			} else {
				const byte[]  *map = Font[num];
				for (int r=1; r<7; r++)	{
					for (int c=0; c<width; c++) {
						// Draw the pixels which need to be turned "ON"
						if ( map[r] & (1 << (7-c)) ) {
							Render2DPoint( (float)(x+c), (float)(y+r) );
						}
					}
				}
			}
		}

		// Update the target location for the next character
		str++;

		x += width+1;
	}

	if (boxed == 1 || boxed == 4 || boxed == 8)
	{
		x1 = xLeft;
		y1 = yTop  - 1.0f;
		x2 = (float)(x + 1);
		y2 = yTop +  8.0f;
		
		Render2DLine (x1, y1, x2, y1);
		Render2DLine (x2, y2, x1, y2);
	}
#endif
        }


        /***************************************************************************\
            Get the width of a text string about to be placed onto the display
            // Returns in units of pixels
        \***************************************************************************/
        public static int ScreenTextWidth(string str)
        {
#if TODO
#if !USE_TEXTURE_FONT
	uint	num;
	int			width;

	width = 0;

	while (*str)
	{
		num = FontLUT[*(byte[]  *)str];
		Debug.Assert( num < FontLength );

		width += Font[num][8] + 1;

		str++;
	}

	return width - 1;
#else
int width = 0;

	while (*str)
	{
		width += FloatToInt32(FontData[FontNum][*str].pixelWidth);
		str++;
	}

	return width;
#endif
#endif
            throw new NotImplementedException();
        }

        /***************************************************************************\
            Get the width of a text string about to be placed onto the display
         // Returns in units of pixels
        \***************************************************************************/
        public static int ScreenTextHeight()
        {
#if !USE_TEXTURE_FONT
            // Right now we have only one font.  It draws 6 pixels high with one
            // pixel each above and below for spacing and reverse video effects.
            return 8;
#else
	return FloatToInt32(FontData[FontNum][32].pixelHeight);
#endif
        }

        public static int CurFont()
        {
            return FontNum;
        }

        public static void SetFont(int newfont)
        {
            Debug.Assert(newfont >= 0 && newfont < NUM_FONT_RESOLUTIONS);
            if (newfont >= TotalFont)
                newfont = TotalFont - 1;
            else
                FontNum = newfont;
        }

        public virtual void SetLineStyle(int p)
        {
        }

        public virtual DWORD Color()
        {
            return new DWORD(0x0);
        }

        public virtual void SetColor(DWORD c)
        {
        }				// Override for color displays
        public virtual void SetBackground(DWORD c)
        {
        }			// Override for color displays


        /***************************************************************************\
    Set the dimensions and location of the viewport.
\***************************************************************************/
        public virtual void SetViewport(float leftSide, float topSide, float rightSide, float bottomSide)
        {
            const float E = 0.01f;	// Eplsion value to ensure we stay within our pixel limits

            left = leftSide;
            top = topSide;
            right = rightSide;
            bottom = bottomSide;

            scaleX = (rightSide - leftSide) * xRes * 0.25f - E;
            if (scaleX < 0.0f)
                scaleX = 0.0f;
            shiftX = (leftSide + 1 + (rightSide - leftSide) * 0.5f) * xRes * 0.5f;

            scaleY = (topSide - bottomSide) * yRes * 0.25f - E;
            if (scaleY < 0.0f)
                scaleY = 0.0f;
            shiftY = yRes - ((bottomSide + 1 + (topSide - bottomSide) * 0.5f) * yRes * 0.5f);

            // Now store our pixel space boundries
            // (top/right inclusive, bottom/left exclusive)
            topPixel = viewportYtoPixel(-1.0f);
            bottomPixel = viewportYtoPixel(1.0f);
            leftPixel = viewportXtoPixel(-1.0f);
            rightPixel = viewportXtoPixel(1.0f);

            Debug.Assert(Math.Floor(topPixel) >= 0.0f);
            Debug.Assert(Math.Ceiling(bottomPixel) >= 0);
            Debug.Assert(Math.Floor(leftPixel) >= 0.0f);
            Debug.Assert(Math.Ceiling(rightPixel) >= 0);
            Debug.Assert(Math.Floor(topPixel) <= yRes);
            Debug.Assert(Math.Ceiling(bottomPixel) <= yRes);
            Debug.Assert(Math.Floor(leftPixel) <= xRes);
            Debug.Assert(Math.Ceiling(rightPixel) <= xRes);
        }


        /***************************************************************************\
            Set the dimensions and location of the viewport.  This one assumes
            the inputs are relative to the currently set viewport.
        \***************************************************************************/
        public virtual void SetViewportRelative(float left, float top, float right, float bottom)
        {
            float w = right - left;
            float h = top - bottom;

            float topSide = top - (1.0f - top) / 2.0f * h;
            float bottomSide = bottom + (1.0f + bottom) / 2.0f * h;
            float leftSide = left + (1.0f + left) / 2.0f * w;
            float rightSide = right - (1.0f - right) / 2.0f * w;

            SetViewport(leftSide, topSide, rightSide, bottomSide);
        }


        /***************************************************************************\
        Compound the current offset with the new one requested.
    \***************************************************************************/
        public void AdjustOriginInViewport(float horizontal, float vertical)
        {
            dmatrix.translationX += horizontal;
            dmatrix.translationY += vertical;
        }


        /***************************************************************************\
    Compound the current rotation with the new one requested.
\***************************************************************************/
        public void AdjustRotationAboutOrigin(float angle)
        {
            float temp;
            float cosAng = (float)Math.Cos(angle);
            float sinAng = (float)Math.Sin(angle);

            temp = dmatrix.rotation00 * cosAng - dmatrix.rotation01 * sinAng;
            dmatrix.rotation01 = dmatrix.rotation00 * sinAng + dmatrix.rotation01 * cosAng;
            dmatrix.rotation00 = temp;

            temp = dmatrix.rotation10 * cosAng - dmatrix.rotation11 * sinAng;
            dmatrix.rotation11 = dmatrix.rotation10 * sinAng + dmatrix.rotation11 * cosAng;
            dmatrix.rotation10 = temp;
        }

        public void CenterOriginInViewport()
        {
            dmatrix.translationX = 0.0f;
            dmatrix.translationY = 0.0f;
        }

        public void ZeroRotationAboutOrigin()
        {
            dmatrix.rotation01 = dmatrix.rotation10 = 0.0f;
            dmatrix.rotation00 = dmatrix.rotation11 = 1.0f;
        }


        // save restore context
        public void SaveDisplayMatrix(DisplayMatrix dm)
        {
            dm = dmatrix;
        }

        public void RestoreDisplayMatrix(DisplayMatrix dm)
        {
            dmatrix = dm;
        }

        public int GetXRes()
        {
            return xRes;
        }

        public int GetYRes()
        {
            return yRes;
        }


        /***************************************************************************\
    Return the current normalized screen space dimensions of the viewport
    on the drawing target buffer.
    \***************************************************************************/
        public void GetViewport(ref float leftSide, ref float topSide, ref float rightSide, ref float bottomSide)
        {
            leftSide = left;
            topSide = top;
            rightSide = right;
            bottomSide = bottom;
        }

        public float GetTopPixel()
        {
            return topPixel;
        }

        public float GetBottomPixel()
        {
            return bottomPixel;
        }

        public float GetLeftPixel()
        {
            return leftPixel;
        }

        public float GetRightPixel()
        {
            return rightPixel;
        }

        public float GetXOffset()
        {
            return shiftX;
        }

        public float GetYOffset()
        {
            return shiftY;
        }

        public enum DisplayType
        {
            DISPLAY_GENERAL = 0,
            DISPLAY_CANVAS
        };
        public DisplayType type;

        // Functions to convert from normalized coordinates to pixel coordinates
        // (assumes at this point that x is right and y is down)
        public float viewportXtoPixel(float x)
        {
            return (x * scaleX) + shiftX;
        }

        public float viewportYtoPixel(float y)
        {
            return (y * scaleY) + shiftY;
        }


        // Functions which must be provided by all derived classes
        protected abstract void Render2DPoint(float x1, float y1);

        protected abstract void Render2DLine(float x1, float y1, float x2, float y2);

        // Functions which should be provided by all derived classes
        protected virtual void Render2DTri(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            Render2DLine(x1, y1, x2, y2);
            Render2DLine(x2, y2, x3, y3);
            Render2DLine(x3, y3, x1, y1);
        }

        // Store the currently selected resolution
        protected int xRes;
        protected int yRes;

        // The viewport properties in normalized screen space (-1 to 1)
        protected float left, right;
        protected float top, bottom;

        // The parameters required to get from normalized screen space to pixel space
        // TEMPORARILY PUBLIC TO GET THINGS GOING...

        public float scaleX;
        public float scaleY;
        public float shiftX;
        public float shiftY;


        // Store the pixel space boundries of the current viewport
        // (top/right inclusive, bottom/left exclusive)
        protected float topPixel;
        protected float bottomPixel;
        protected float leftPixel;
        protected float rightPixel;

        // The 2D rotation/translation settings
        protected DisplayMatrix dmatrix; // JPO - now in a sub structure so you can save/restore
        //float	translationX, translationY;
        //float	rotation00,	rotation01;
        //float	rotation10,	rotation11;

        // The font information for drawing text
        protected static readonly byte[][] Font = {
	Space, OpenParen, CloseParen, Asterisk, Plus,				/* Index 0 through 4 */
	Comma, Minus, Period, Slash,								/* Index 5 through 8 */
	Number0,Number1,Number2,Number3,Number4,					/* Index 9 through 13 */
	Number5,Number6,Number7,Number8,Number9,					/* Index 14 through 18 */
	Colon, SemiColon,Less,Equal,More,Quest,Each,				/* Index 19 through 25 */
	LetterA,LetterB,LetterC,LetterD,LetterE,LetterF,LetterG,	/* Index 26 through 32 */
	LetterH,LetterI,LetterJ,LetterK,LetterL,LetterM,LetterN,	/* Index 33 through 39 */
	LetterO,LetterP,LetterQ,LetterR,LetterS,LetterT,LetterU,	/* Index 40 through 46 */
	LetterV,LetterW,LetterX,LetterY,LetterZ, Apostrophe,		/* Index 47 through 52 */
	LetterAumlaut,LetterOumlaut,LetterUumlaut,LetterBeta,		/* Index 53 through 56 */
	Degree,	Mu,	Exclaim, Quote, And,							/* Index 57 through 61 */
	LetterAaccent,		// 62
	LetterAsquiggle,	// 63
	LetterEaccent,		// 64
	LetterEhat,			// 65
	LetterIaccent,		// 66
	LetterNsquiggle,	// 67
	LetterOsquiggle,	// 68
	LetterOaccent,		// 69
	LetterUaccent,		// 70
	LetterCstem,		// 71
	LetterAhat,			// 72
	LetterAbackaccent,	// 73
};

        //TODO protected  static readonly int  FontLength = sizeof(Font)/sizeof(Font[0]);

        protected static readonly byte[] FontLUT = 
{
	  0, /* ASCII   0 */	  0, /* ASCII   1 */	  0, /* ASCII   2 */      0,  /* ASCII   3 */
	  0, /* ASCII   4 */	  0, /* ASCII   5 */	  0, /* ASCII   6 */      0,  /* ASCII   7 */
	  0, /* ASCII   8 */	  0, /* ASCII   9 */	  0, /* ASCII  10 */      0,  /* ASCII  11 */
	  0, /* ASCII  12 */	  0, /* ASCII  13 */	  0, /* ASCII  14 */      0,  /* ASCII  15 */
	  0, /* ASCII  16 */	  0, /* ASCII  17 */	  0, /* ASCII  18 */      0,  /* ASCII  19 */
	  0, /* ASCII  20 */	  0, /* ASCII  21 */	  0, /* ASCII  22 */      0,  /* ASCII  23 */
	  0, /* ASCII  24 */	  0, /* ASCII  25 */	  0, /* ASCII  26 */      0,  /* ASCII  27 */
	  0, /* ASCII  28 */	  0, /* ASCII  29 */	  0, /* ASCII  30 */      0,  /* ASCII  31 */
	  0, /* ASCII  32 */	 59, /* ASCII  33 */	 60, /* ASCII  34 */      0,  /* ASCII  35 */
	  0, /* ASCII  36 */	  0, /* ASCII  37 */	 61, /* ASCII  38 */     52,  /* ASCII  39 */
	  1, /* ASCII  40 */	  2, /* ASCII  41 */	  3, /* ASCII  42 */      4,  /* ASCII  43 */
	  5, /* ASCII  44 */	  6, /* ASCII  45 */	  7, /* ASCII  46 */      8,  /* ASCII  47 */
	  9, /* ASCII  48 */	 10, /* ASCII  49 */	 11, /* ASCII  50 */     12,  /* ASCII  51 */
	 13, /* ASCII  52 */	 14, /* ASCII  53 */	 15, /* ASCII  54 */     16,  /* ASCII  55 */
	 17, /* ASCII  56 */	 18, /* ASCII  57 */	 19, /* ASCII  58 */     20,  /* ASCII  59 */
	 21, /* ASCII  60 */	 22, /* ASCII  61 */	 23, /* ASCII  62 */     24,  /* ASCII  63 */
	 25, /* ASCII  64 */	 26, /* ASCII  65 */	 27, /* ASCII  66 */     28,  /* ASCII  67 */
	 29, /* ASCII  68 */	 30, /* ASCII  69 */	 31, /* ASCII  70 */     32,  /* ASCII  71 */
	 33, /* ASCII  72 */	 34, /* ASCII  73 */	 35, /* ASCII  74 */     36,  /* ASCII  75 */
	 37, /* ASCII  76 */	 38, /* ASCII  77 */	 39, /* ASCII  78 */     40,  /* ASCII  79 */
	 41, /* ASCII  80 */	 42, /* ASCII  81 */	 43, /* ASCII  82 */     44,  /* ASCII  83 */
	 45, /* ASCII  84 */	 46, /* ASCII  85 */	 47, /* ASCII  86 */     48,  /* ASCII  87 */
	 49, /* ASCII  88 */	 50, /* ASCII  89 */	 51, /* ASCII  90 */     52,  /* ASCII  91 */
	  0, /* ASCII  92 */	  0, /* ASCII  93 */	  0, /* ASCII  94 */      0,  /* ASCII  95 */
	  0, /* ASCII  96 */	 26, /* ASCII  97 */	 27, /* ASCII  98 */     28,  /* ASCII  99 */
	 29, /* ASCII 100 */	 30, /* ASCII 101 */	 31, /* ASCII 102 */	 32,  /* ASCII 103 */
	 33, /* ASCII 104 */	 34, /* ASCII 105 */	 35, /* ASCII 106 */	 36,  /* ASCII 107 */
	 37, /* ASCII 108 */	 38, /* ASCII 109 */	 39, /* ASCII 110 */	 40,  /* ASCII 111 */
	 41, /* ASCII 112 */	 42, /* ASCII 113 */	 43, /* ASCII 114 */	 44,  /* ASCII 115 */
	 45, /* ASCII 116 */	 46, /* ASCII 117 */	 47, /* ASCII 118 */	 48,  /* ASCII 119 */
	 49, /* ASCII 120 */	 50, /* ASCII 121 */	 51, /* ASCII 122 */	  0,  /* ASCII 123 */
	  0, /* ASCII 124 */	  0, /* ASCII 125 */	  0, /* ASCII 126 */	  0,  /* ASCII 127 */
	 28, /* ASCII 128 */	 46, /* ASCII 129 */	 30, /* ASCII 130 */	 26,  /* ASCII 131 */
	 26, /* ASCII 132 */	 26, /* ASCII 133 */	 26, /* ASCII 134 */	 28,  /* ASCII 135 */
	 30, /* ASCII 136 */	 30, /* ASCII 137 */	 30, /* ASCII 138 */	 37,  /* ASCII 139 */
	 37, /* ASCII 140 */	 37, /* ASCII 141 */	 26, /* ASCII 142 */	 26,  /* ASCII 143 */
	 30, /* ASCII 144 */	 26, /* ASCII 145 */	 26, /* ASCII 146 */	 40,  /* ASCII 147 */
	 40, /* ASCII 148 */	 40, /* ASCII 149 */	 46, /* ASCII 150 */	 46,  /* ASCII 151 */
	 50, /* ASCII 152 */	 40, /* ASCII 153 */	 46, /* ASCII 154 */	  0,  /* ASCII 155 */
	  0, /* ASCII 156 */	  0, /* ASCII 157 */	  0, /* ASCII 158 */	  0,  /* ASCII 159 */
	 26, /* ASCII 160 */	 37, /* ASCII 161 */	 40, /* ASCII 162 */	 46,  /* ASCII 163 */
	 39, /* ASCII 164 */	 39, /* ASCII 165 */	  0, /* ASCII 166 */	  0,  /* ASCII 167 */
	  0, /* ASCII 168 */	  0, /* ASCII 169 */	  0, /* ASCII 170 */	  0,  /* ASCII 171 */
	  0, /* ASCII 172 */	  0, /* ASCII 173 */	  0, /* ASCII 174 */	  0,  /* ASCII 175 */
	 57, /* ASCII 176 */	  0, /* ASCII 177 */	  0, /* ASCII 178 */	  0,  /* ASCII 179 */
	  0, /* ASCII 180 */	 58, /* ASCII 181 */	  0, /* ASCII 182 */	  0,  /* ASCII 183 */
	  0, /* ASCII 184 */	  0, /* ASCII 185 */	  0, /* ASCII 186 */	  0,  /* ASCII 187 */
	  0, /* ASCII 188 */	  0, /* ASCII 189 */	  0, /* ASCII 190 */	  0,  /* ASCII 191 */
	 73, /* ASCII 192 */	 62, /* ASCII 193 */	 72, /* ASCII 194 */	 63,  /* ASCII 195 */
	 53, /* ASCII 196 */	 26, /* ASCII 197 */	 26, /* ASCII 198 */	 71,  /* ASCII 199 */
	 30, /* ASCII 200 */	 64, /* ASCII 201 */	 65, /* ASCII 202 */	 30,  /* ASCII 203 */
	 34, /* ASCII 204 */	 66, /* ASCII 205 */	 34, /* ASCII 206 */	 34,  /* ASCII 207 */
	 29, /* ASCII 208 */	 67, /* ASCII 209 */	 40, /* ASCII 210 */	 69,  /* ASCII 211 */
	 40, /* ASCII 212 */	 68, /* ASCII 213 */	 54, /* ASCII 214 */	  0,  /* ASCII 215 */
	 40, /* ASCII 216 */	 46, /* ASCII 217 */	 70, /* ASCII 218 */	 46,  /* ASCII 219 */
	 55, /* ASCII 220 */	  0, /* ASCII 221 */	  0, /* ASCII 222 */	 56,  /* ASCII 223 */
	 73, /* ASCII 224 */	 62, /* ASCII 225 */	 72, /* ASCII 226 */	 63,  /* ASCII 227 */
	 53, /* ASCII 228 */	 26, /* ASCII 229 */	 26, /* ASCII 230 */	 71,  /* ASCII 231 */
	 30, /* ASCII 232 */	 64, /* ASCII 233 */	 65, /* ASCII 234 */	 30,  /* ASCII 235 */
	 37, /* ASCII 236 */	 66, /* ASCII 237 */	 37, /* ASCII 238 */	 37,  /* ASCII 239 */
	  0, /* ASCII 240 */	 67, /* ASCII 241 */	 40, /* ASCII 242 */	 69,  /* ASCII 243 */
	 40, /* ASCII 244 */	 68, /* ASCII 245 */	 54, /* ASCII 246 */	 40,  /* ASCII 247 */
	  0, /* ASCII 248 */	 46, /* ASCII 249 */	 70, /* ASCII 250 */	 46,  /* ASCII 251 */
	 55, /* ASCII 252 */	 50, /* ASCII 253 */	  0, /* ASCII 254 */	 50,  /* ASCII 255 */
};


        //TODO protected static readonly byte[][]       InvFont = new byte[][8];




        protected bool ready;


        // An array of precomputed points on a unit circle to be shared by all instances of this class
        // Element zero is for theta = 0, and each successive element adds 4 degrees to
        // theta.  theta = 360 is a repeat of theta = 0.  The whole unit circle is
        // represented without the need for reflecting points between quadrants.
        private float[] CircleX = new float[CircleSegments];
        private float[] CircleY = new float[CircleSegments];

        //TODO private FontDataType[][] FontData = new FontDataType[NUM_FONT_RESOLUTIONS][256] = {0};
        private int[] fontSpacing = new int[NUM_FONT_RESOLUTIONS];
        private static int FontNum = 0;
        private static int TotalFont = 3;

        /***************************************************************************\
            This is the font data used to draw text.  For now, it uses an 8x8 cell.
            NOTE:  The last number in each character it the actual width of the
            character for proportional spacing.
        \***************************************************************************/

        static readonly byte[] Space = {
    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 3
};
        static readonly byte[] OpenParen = {
	0x00,0x00,0x40,0x80,0x80,0x80,0x40,0x00, 2
};
        static readonly byte[] CloseParen = {
	0x00,0x00,0x80,0x40,0x40,0x40,0x80,0x00, 2
};
        static readonly byte[] Asterisk = {
	0x00,0x00,0xe0,0xa0,0xe0,0x00,0x00,0x00, 3
};												// Note: This is the slot for the ascii asterisk, but I'm mapping a degree symbol to it.
        static readonly byte[] Plus = {
	0x00,0x00,0x40,0xe0,0x00,0xe0,0x40,0x00, 3
};												// Note: This is the slot for the ascii plus symbol, but I'm mapping a 'roll' symbol to it.
        static readonly byte[] Comma = {
	0x00,0x00,0x00,0x00,0x00,0x00,0x40,0x80, 2
};
        static readonly byte[] Minus = {
	0x00,0x00,0x00,0x00,0xe0,0x00,0x00,0x00, 3
};
        static readonly byte[] Period = {
	0x00,0x00,0x00,0x00,0x00,0x00,0x80,0x00, 2
};
        static readonly byte[] Slash = {
	0x00,0x00,0x20,0x20,0x40,0x80,0x80,0x00, 3
};
        static readonly byte[] Number0 = {
	0x00,0x00,0xe0,0xa0,0xa0,0xa0,0xe0,0x00, 3
};
        static readonly byte[] Number1 = {
	0x00,0x00,0x40,0xc0,0x40,0x40,0xe0,0x00, 3
};
        static readonly byte[] Number2 = {
	0x00,0x00,0xe0,0x20,0xe0,0x80,0xe0,0x00, 3
};
        static readonly byte[] Number3 = {
	0x00,0x00,0xe0,0x20,0x60,0x20,0xe0,0x00, 3
};
        static readonly byte[] Number4 = {
	0x00,0x00,0x80,0xa0,0xe0,0x20,0x20,0x00, 3
};
        static readonly byte[] Number5 = {
	0x00,0x00,0xe0,0x80,0xc0,0x20,0xc0,0x00, 3
};
        static readonly byte[] Number6 = {
    0x00,0x00,0x80,0x80,0xe0,0xa0,0xe0,0x00, 3
};
        static readonly byte[] Number7 = {
    0x00,0x00,0xe0,0x20,0x20,0x20,0x20,0x00, 3
};
        static readonly byte[] Number8 = {
    0x00,0x00,0xe0,0xa0,0xe0,0xa0,0xe0,0x00, 3
};
        static readonly byte[] Number9 = {
    0x00,0x00,0xe0,0xa0,0xe0,0x20,0x20,0x00, 3
};
        static readonly byte[] Colon = {
    0x00,0x00,0x00,0x80,0x00,0x80,0x00,0x00, 1
};
        static readonly byte[] SemiColon = {
    0x00,0x00,0x00,0x40,0x00,0x40,0x40,0x80, 2
};
        static readonly byte[] Less = {
    0x00,0x00,0x20,0x40,0x80,0x40,0x20,0x00, 3
};
        static readonly byte[] Equal = {
    0x00,0x00,0x00,0xe0,0x00,0xe0,0x00,0x00, 3
};
        static readonly byte[] More = {
    0x00,0x00,0x80,0x40,0x20,0x40,0x80,0x00, 3
};
        static readonly byte[] Quest = {
    0x40,0x00,0xc0,0x20,0x40,0x00,0x40,0x00, 3
};
        static readonly byte[] Each = {
    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 3
};
        static readonly byte[] LetterA = {
    0x00,0x00,0x40,0xa0,0xa0,0xe0,0xa0,0x00, 3
};
        static readonly byte[] LetterB = {
    0x00,0x00,0xc0,0xa0,0xc0,0xa0,0xc0,0x00, 3
};
        static readonly byte[] LetterC = {
    0x00,0x00,0x40,0xa0,0x80,0xa0,0x40,0x00, 3
};
        static readonly byte[] LetterD = {
    0x00,0x00,0xc0,0xa0,0xa0,0xa0,0xc0,0x00, 3
};
        static readonly byte[] LetterE = {
    0x00,0x00,0xe0,0x80,0xc0,0x80,0xe0,0x00, 3
};
        static readonly byte[] LetterF = {
    0x00,0x00,0xe0,0x80,0xc0,0x80,0x80,0x00, 3
};
        static readonly byte[] LetterG = {
    0x00,0x00,0x60,0x80,0xa0,0xa0,0x60,0x00, 3
};
        static readonly byte[] LetterH = {
    0x00,0x00,0xa0,0xa0,0xe0,0xa0,0xa0,0x00, 3
};
        static readonly byte[] LetterI = {
    0x00,0x00,0xe0,0x40,0x40,0x40,0xe0,0x00, 3
};
        static readonly byte[] LetterJ = {
    0x00,0x00,0x20,0x20,0x20,0xa0,0x40,0x00, 3
};
        static readonly byte[] LetterK = {
    0x00,0x00,0xa0,0xa0,0xc0,0xa0,0xa0,0x00, 3
};
        static readonly byte[] LetterL = {
    0x00,0x00,0x80,0x80,0x80,0x80,0xe0,0x00, 3
};
        static readonly byte[] LetterM = {
    0x00,0x00,0x88,0xd8,0xa8,0xa8,0x88,0x00, 5
};
        static readonly byte[] LetterN = {
    0x00,0x00,0x90,0xd0,0xb0,0x90,0x90,0x00, 4
};
        static readonly byte[] LetterO = {
    0x00,0x00,0x60,0x90,0x90,0x90,0x60,0x00, 4
};
        static readonly byte[] LetterP = {
    0x00,0x00,0xe0,0xa0,0xe0,0x80,0x80,0x00, 3
};
        static readonly byte[] LetterQ = {
    0x00,0x00,0x60,0x90,0x90,0xa0,0xd0,0x00, 4
};
        static readonly byte[] LetterR = {
    0x00,0x00,0xc0,0xa0,0xc0,0xa0,0xa0,0x00, 3
};
        static readonly byte[] LetterS = {
    0x00,0x00,0x60,0x80,0x40,0x20,0xc0,0x00, 3
};
        static readonly byte[] LetterT = {
    0x00,0x00,0xe0,0x40,0x40,0x40,0x40,0x00, 3
};
        static readonly byte[] LetterU = {
    0x00,0x00,0xa0,0xa0,0xa0,0xa0,0xe0,0x00, 3
};
        static readonly byte[] LetterV = {
    0x00,0x00,0xa0,0xa0,0xa0,0xe0,0x40,0x00, 3
};
        static readonly byte[] LetterW = {
    0x00,0x00,0x88,0x88,0xa8,0xa8,0x50,0x00, 5
};
        static readonly byte[] LetterX = {
    0x00,0x00,0xa0,0xa0,0x40,0xa0,0xa0,0x00, 3
};
        static readonly byte[] LetterY = {
    0x00,0x00,0xa0,0xa0,0xe0,0x40,0x40,0x00, 3
};
        static readonly byte[] LetterZ = {
    0x00,0x00,0xe0,0x20,0x40,0x80,0xe0,0x00, 3
};
        static readonly byte[] Apostrophe = {
	0x00,0x00,0x40,0x80,0x00,0x00,0x00,0x00, 2
};
        static readonly byte[] LetterAumlaut = {
    0x00,0xa0,0x40,0xa0,0xa0,0xe0,0xa0,0x00, 3
};
        static readonly byte[] LetterOumlaut = {
    0x00,0x90,0x60,0x90,0x90,0x90,0x60,0x00, 4
};
        static readonly byte[] LetterUumlaut = {
    0x00,0xa0,0x00,0xa0,0xa0,0xa0,0xe0,0x00, 3
};
        static readonly byte[] LetterBeta = {
    0x00,0x00,0xf0,0x90,0xb8,0x88,0xf8,0x00, 5
};
        static readonly byte[] Degree = {
	0x00,0x00,0xe0,0xa0,0xe0,0x00,0x00,0x00, 3
};
        static readonly byte[] Mu = {
	0x00,0x00,0x50,0x50,0x50,0x60,0x80,0x00, 4
};
        static readonly byte[] Exclaim = {
	0x00,0x00,0x80,0x80,0x80,0x00,0x80,0x00, 1
};
        static readonly byte[] Quote = {
	0x00,0x00,0xa0,0xa0,0x00,0x00,0x00,0x00, 3
};
        static readonly byte[] And = {
	0x00,0x00,0x40,0xa0,0x40,0xa0,0x50,0x00, 4
};
        static readonly byte[] LetterAaccent = {
    0x10,0x20,0x40,0x60,0x90,0xf0,0x90,0x00, 4
};
        static readonly byte[] LetterAbackaccent = {
    0x40,0x20,0x10,0x60,0x90,0xf0,0x90,0x00, 4
};
        static readonly byte[] LetterAsquiggle = {
    0x50,0xa0,0x00,0x60,0x90,0xf0,0x90,0x00, 4
};
        static readonly byte[] LetterAhat = {
    0x60,0x90,0x00,0x60,0x90,0xf0,0x90,0x00, 4
};
        static readonly byte[] LetterEaccent = {
    0x20,0x40,0xe0,0x80,0xc0,0x80,0xe0,0x00, 3
};
        static readonly byte[] LetterEhat = {
    0x40,0xa0,0xe0,0x80,0xc0,0x80,0xe0,0x00, 3
};
        static readonly byte[] LetterIaccent = {
    0x20,0x40,0xe0,0x40,0x40,0x40,0xe0,0x00, 3
};
        static readonly byte[] LetterNsquiggle = {
    0x50,0xa0,0x00,0x90,0xd0,0xb0,0x90,0x00, 4
};
        static readonly byte[] LetterOsquiggle = {
    0x50,0xa0,0x00,0xf0,0x90,0x90,0xf0,0x00, 4
};
        static readonly byte[] LetterOaccent = {
    0x10,0x20,0x40,0xf0,0x90,0x90,0xf0,0x00, 4
};
        static readonly byte[] LetterUaccent = {
    0x10,0x20,0x40,0x90,0x90,0x90,0xf0,0x00, 4
};
        static readonly byte[] LetterCstem = {
    0x00,0x00,0xe0,0x80,0x80,0xe0,0x40,0xc0, 3
};

    };

}

