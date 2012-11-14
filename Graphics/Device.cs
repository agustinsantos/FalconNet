using System;
using FalconNet.Common;

namespace FalconNet.Graphics
{
	/***************************************************************************\
	    This class provides management of a single drawing device in a system.
	\***************************************************************************/
	public class DisplayDevice
	{
	   
		public DisplayDevice ()
		{
			appWin			= null;
			driverNumber	= -1;
		
			m_DXCtx = null;
		}
		
		// TODO public ~DisplayDevice();
	
		public void Setup (int driverNum, int devNum, int width, int height, int nDepth, bool fullScreen, bool dblBuffer, HWND win, bool bWillCallSwapBuffer)
		{
#if TODO
		    RECT		rect;
			DWORD		style;
			WNDCLASS	wc;
			int			resNum;
			UInt		w, h, d;
		
			Debug.Assert( !IsReady() );
		
		
			// Remember our driver number so we know if we're software (driver 0) or hardware.
			driverNumber = driverNum;
		
		
			#if _DEBUG
			// For now we get the driver name again here.
			// TODO:  Eliminate the need for this by improving the MPR API?
			const char *lpDriverName;
			lpDriverName = FalconDisplay.devmgr.GetDriverName( driverNum);
			if(lpDriverName == null) ShiWarning( "Failed to get a name for the driver number" );
			#endif
		
			// OW: always create flipping chain in fullscreen (even if we dont flip - we dont)
			dblBuffer = fullScreen;	// warning: C_Handler.Setup relies on this behaviour
		
			// For now, we go figure out the number for the resolution we want
			// TODO:  Change the DisplayDevice API to require the resNum to be passed in?
			for (resNum=0; true; resNum++) {
				if (FalconDisplay.devmgr.GetMode( driverNum, devNum, resNum, &w, &h, &d)) {
					if((w == (unsigned) width) && (h == (unsigned)height) && (d == (unsigned)depth)) {
						// Found it
						break;
					}
				} else {
					// Ran off the end of the list
					string message = "Requested unavilable resolution " + width + height + depth;
					ShiError( message );
				}
			}
		
			// Create an MPR device handle for this device
			m_DXCtx = FalconDisplay.devmgr.CreateContext(driverNumber, devNum, resNum, fullScreen, win);
		
			if(!m_DXCtx)
			{
				// try default device
				driverNumber = 0;
		
				m_DXCtx = FalconDisplay.devmgr.CreateContext(driverNumber, 0, resNum, fullScreen, win);
				if(!m_DXCtx)
					return;
			}
		
			// See if we need to build our own window
			if (win) {
		
				// Store the applications main window for later use
				appWin = win;
				privateWindow = false;
		
			} else {
		
				// set up and register window class
				wc.style = CS_HREDRAW | CS_VREDRAW | CS_OWNDC | CS_DBLCLKS | CS_NOCLOSE;
				wc.lpfnWndProc = DefWindowProc;
				wc.cbClsExtra = 0;
				wc.cbWndExtra = sizeof(DWORD);
				wc.hInstance = null;
				wc.hIcon = null;
				wc.hCursor = null;
				wc.hbrBackground = (HBRUSH)GetStockObject(BLACK_BRUSH);
				wc.lpszMenuName = null;
				wc.lpszClassName = "RenderTarget";
		
				// Register this class.
				RegisterClass(&wc);
		
				// Choose an appropriate window style
				if (fullScreen) {
					style = WS_POPUP;
				} else {
					style = WS_OVERLAPPEDWINDOW;
				}
		
				// Build a window for this application
				rect.top = rect.left = 0;
				rect.right = width;
				rect.bottom = height;
				AdjustWindowRect(&rect,	style, false);
				appWin = CreateWindow(
			    	"RenderTarget",			/* class */
					"Falcon 4.0 Demo",		/* caption */
					style,					/* style */
					CW_USEDEFAULT,			/* init. x pos */
					CW_USEDEFAULT,			/* init. y pos */
					rect.right-rect.left,	/* init. x size */
					rect.bottom-rect.top,	/* init. y size */
					null,					/* parent window */
					null,					/* menu handle */
					null,					/* program handle */
					null					/* create parms */
				);
				if (!appWin) {
					ShiError( "Failed to construct main window" );
				}
		
				// Make note of the fact that we'll have to release this window when we're done
				privateWindow = true;
			}
		
		
			// Ensure the rendering window is visible
			ShowWindow( appWin, SW_SHOW );
		
		
			// Select the requested mode of operation
			if(fullScreen)
			{
				// If this is other than the default primary display, shrink the target window
				// on the desktop and don't let DirectX muck with it.
		#if NOTHING
				SetWindowPos( appWin, HWND_TOP, 0, 200, 10, 4, SWP_NOCOPYBITS | SWP_SHOWWINDOW );
		#endif
		
				// Create the primary surface(s)
				if(dblBuffer)
					image.Setup( this, width, height, Primary, Flip, appWin, false, true, bWillCallSwapBuffer );
				else
					image.Setup( this, width, height, Primary, IsHardware() ? VideoMem : SystemMem, appWin, false, true, bWillCallSwapBuffer);
			}
		
			else
				image.Setup( this, width, height, Primary, IsHardware() ? VideoMem : SystemMem, appWin, true, false, bWillCallSwapBuffer);
		
			// Make sure we haven't gotten confused about how many contexts we have
		//	Debug.Assert( ContextMPR.StateSetupCounter == 0 );
			if(ContextMPR.StateSetupCounter != 0)	
				ContextMPR.StateSetupCounter = 0;	// Force it for now.  Shouldn't be required.
		
			// Create a rendering context for the primary surface
			m_DXCtx.SetRenderTarget(image.targetSurface());
		
			movieInit(2, m_DXCtx.m_pDD);
#endif 
			throw new NotImplementedException();
		}
		

		public void	Cleanup ()
		{
#if TODO	
			Debug.Assert( IsReady() );
		
			if(m_DXCtx)
			{
			    m_DXCtx.Release();
				//delete m_DXCtx;
				m_DXCtx = null;
			}
		
			// Destroy our primary surface and its default context
			image.Cleanup();
		
			// Make sure we haven't gotten confused about how many contexts we have
		//	Debug.Assert( ContextMPR.StateSetupCounter == 0 );
			if (ContextMPR.StateSetupCounter != 0) {	
				ContextMPR.StateSetupCounter = 0;	// Force it for now.  Shouldn't be required.
			}
		
			// Destroy the application window if we created it
			if ( privateWindow ) {
				DestroyWindow( appWin );
			}
			appWin = null;
		
			movieUnInit();
#endif 
			throw new NotImplementedException();
		}
	
		public bool	IsReady ()
		{
			return (m_DXCtx != null);
		}
	
		public bool	IsHardware ()
		{
#if TODO		
			Debug.Assert (IsReady ());
			return m_DXCtx.m_eDeviceCategory > DXContext.D3DDeviceCategory_Software;
#endif
			throw new NotImplementedException();
		}
	
		public HWND	GetAppWin ()
		{
			return appWin;
		}

		public IDirectDraw7 GetMPRdevice ()
		{
#if TODO			
			return m_DXCtx.m_pDD;
#endif
			throw new NotImplementedException();
		}

		public ImageBuffer GetImageBuffer ()
		{
			return image;
		}

		public DXContext GetDefaultRC ()
		{
			return m_DXCtx;
		}
	  
		protected DXContext m_DXCtx;
		protected HWND		appWin;
		protected bool		privateWindow;
		protected ImageBuffer image;
		protected int		driverNumber;
		protected DisplayDevice	next;
		// The pixel depth is hardwired for now
		protected const int BITS_PER_PIXEL	= 16;
	}
}

