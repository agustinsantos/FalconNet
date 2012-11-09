using System;

namespace FalconNet.Graphics
{
	public class ImageBuffer
	{
#if TODO

		// Constructor/Destructor for this buffer
		public ImageBuffer (){	m_bReady = false;
	m_bFrontRectValid = false;
	m_bBitsLocked = false;

	m_pDDSFront = null;
	m_pDDSBack = null;
	ZeroMemory(&m_rcFront, sizeof(m_rcFront));
	m_pBltTarget = null;
}
		//public virtual ~ImageBuffer();

		// Functions used to set up and manage this buffer
		public bool Setup (DisplayDevice *dev, int width, int height, MPRSurfaceType front, MPRSurfaceType back, HWND targetWin = NULL, bool clip = FALSE, bool fullScreen = FALSE, bool bWillCallSwapBuffer = FALSE);

		public void Cleanup ();

		public bool IsReady ()
		{
	return m_bReady;
}


		public void SetChromaKey (UInt32 RGBcolor);

		public void UpdateFrontWindowRect (RECT *rect);

		public void AttachSurfaces (DisplayDevice *pDev, IDirectDrawSurface7 *pDDSFront, IDirectDrawSurface7 *pDDSBack);

		// Used to get the properties of the under lying surfaces
		public DisplayDevice GetDisplayDevice ()
		{
			return device;
		}

		public IDirectDrawSurface7 targetSurface ()
		{
			return m_pDDSBack;
		}

		public IDirectDrawSurface7 frontSurface ()
		{
			return m_pDDSFront;
		}

		public int targetXres ()
		{
			return width;
		}

		public int	targetYres ()
		{
			return height;
		}

		public int	targetStride ()
		{
			return m_ddsdBack.lPitch;
		}

		public void GetColorMasks (UInt32 *r, UInt32 *g, UInt32* b);

		public UInt32 RedMask ()
		{
			return m_ddsdFront.ddpfPixelFormat.dwRBitMask;
		}

		public UInt32 GreenMask ()
		{
			return m_ddsdFront.ddpfPixelFormat.dwGBitMask;
		}

		public UInt32 BlueMask ()
		{
			return m_ddsdFront.ddpfPixelFormat.dwBBitMask;
		}

		public int RedShift ()
		{
			return redShift;
		}

		public int	GreenShift ()
		{
			return greenShift;
		}

		public int	BlueShift ()
		{
			return blueShift;
		}

		public int	PixelSize ()
		{
			return m_ddsdFront.ddpfPixelFormat.dwRGBBitCount >> 3;
		}

		public void RestoreAll ();	// OW

		// Used to allow direct pixel access to the back buffer
		public void *Lock (bool bDontLockBits = false, bool bWriteOnly = true);

		public void  Unlock ();

		// Used to convert from a 32 bit color to a 16 bit pixel (assumes 16 bit display mode)
		public UInt16  Pixel32toPixel16 (UInt32 ABGR);

		public UInt32  Pixel16toPixel32 (UInt16 pixel);

		public UInt32  Pixel32toPixel32 (UInt32 pixel);

		// Used to write a pixel to the back buffer surface (requires a Lock)
		public void* Pixel (void* ptr, int row, int col)
		{
			Debug.Assert (ptr);
			return (BYTE*)ptr + 
											  row * m_ddsdBack.lPitch + 
											  col * PixelSize ();
		}

		// Used to control image compositing
		public void Compose (ImageBuffer *srcBuffer, RECT *srcRect, RECT *dstRect);

		public void ComposeTransparent (ImageBuffer *srcBuffer, RECT *srcRect, RECT *dstRect);

		// Used to control image compositing with a clockwise rotation (in radians)
		public void ComposeRot (ImageBuffer *srcBuffer, RECT *srcRect, RECT *dstRect, float angle);

		public void ComposeRoundRot (ImageBuffer *srcBuffer, RECT *srcRect, RECT *dstRect, float angle, int *startStopArray);

		// Swap rolls of front and back buffers (page flip, blt, or nothing depending on types)
		public void SwapBuffers (bool bDontFlip);

		// Helpful function to drop a screen capture to disk (BACK buffer to 24 bit RAW file)
		public void BackBufferToRAW (char *filename);
	
		protected void ComputeColorShifts ();
	 
		protected bool m_bReady;
		protected DisplayDevice *device;
		protected IDirectDrawSurface7 *m_pDDSFront;
		protected DDSURFACEDESC2	m_ddsdFront;
		protected HWND m_hWnd;
		protected RECT m_rcFront;
		protected bool m_bFrontRectValid;
		protected IDirectDrawSurface7 *m_pDDSBack;
		protected 	DDSURFACEDESC2	m_ddsdBack;
		protected int width;
		protected int height;
		protected int redShift;
		protected int greenShift;
		protected int blueShift;
		protected DWORD m_dwColorKey;
		protected IDirectDrawSurface7 *m_pBltTarget;
		protected CRITICAL_SECTION m_cs;
		protected bool m_bBitsLocked;
#endif
	}
}

