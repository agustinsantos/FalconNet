using System;
using FalconNet.Common;
using FalconNet.Graphics;

namespace FalconNet.FalcLib
{
		
	public enum DisplayMode {
		Movie,
		UI,
		UILarge, 
		Planner, 
		Layout, 
		Sim, 
		NumModes
	}
	
	public class FalconDisplayConfiguration
	{
		public static FalconDisplayConfiguration FalconDisplay = new FalconDisplayConfiguration();		
		public FalconDisplayConfiguration()
		{
		   xOffset = 40;
		   yOffset = 40;
		
		   width[(int)DisplayMode.Movie] = 640;
		   height[(int)DisplayMode.Movie] = 480;
		   depth[(int)DisplayMode.Movie] = 16;
		   doubleBuffer[(int)DisplayMode.Movie] = false;
		
		   width[(int)DisplayMode.UI] = 800;
		   height[(int)DisplayMode.UI] = 600;
		   depth[(int)DisplayMode.UI] = 16;
		   doubleBuffer[(int)DisplayMode.UI] = false;
		
		   width[(int)DisplayMode.UILarge] = 1024;
		   height[(int)DisplayMode.UILarge] = 768;
		   depth[(int)DisplayMode.UILarge] = 16;
		   doubleBuffer[(int)DisplayMode.UILarge] = false;
		
		   width[(int)DisplayMode.Planner] = 800;
		   height[(int)DisplayMode.Planner] = 600;
		   depth[(int)DisplayMode.Planner] = 16;
		   doubleBuffer[(int)DisplayMode.Planner] = false;
		
		   width[(int)DisplayMode.Layout] = 1024;
		   height[(int)DisplayMode.Layout] = 768;
		   depth[(int)DisplayMode.Layout] = 16;
		   doubleBuffer[(int)DisplayMode.Layout] = false;
		
		   //default values
		   width[(int)DisplayMode.Sim] = 1024;
		   height[(int)DisplayMode.Sim] = 768;
		   depth[(int)DisplayMode.Sim] = 32;
		   doubleBuffer[(int)DisplayMode.Sim] = true;
		
		   deviceNumber = 0;
		   displayFullScreen = true;
		}

		// public ~FalconDisplayConfiguration();

		const int modes = (int)DisplayMode.NumModes;
		public DisplayMode currentMode;
		public int	xOffset;
		public int	yOffset;
		public int[]	width = new int[modes];
		public int[]	height = new int[modes];
		public int[]	depth = new int[modes];
		public bool[]	doubleBuffer = new bool[modes];
		// TODO public HWND appWin;
		public int	windowStyle;
		// Device managment
		public DeviceManager	devmgr;
		public DisplayDevice	theDisplayDevice;
		public int	deviceNumber;
		public bool	displayFullScreen;
	
		public void Setup( int languageNum )
		{
#if TODO
			WNDCLASS	wc;
			
			// Setup the graphics databases	- M.N. changed to Falcon3DDataDir for theater switching
			DeviceIndependentGraphicsSetup( FalconTerrainDataDir, Falcon3DDataDir, FalconMiscTexDataDir );
			
			// set up and register window class
			wc.style = CS_HREDRAW | CS_VREDRAW | CS_OWNDC | CS_NOCLOSE;
			wc.lpfnWndProc = FalconMessageHandler;
			wc.cbClsExtra = 0;
			wc.cbWndExtra = sizeof(DWORD);
			wc.hInstance = null;
			//   wc.hIcon = null;
			 wc.hIcon = LoadIcon (GetModuleHandle(null), MAKEINTRESOURCE(105));	// OW BC
			wc.hCursor = null;
			wc.hbrBackground = (HBRUSH)GetStockObject(BLACK_BRUSH);
			wc.lpszMenuName = null;
			wc.lpszClassName = "FalconDisplay";
			
			// Register this class.
			RegisterClass(&wc);
			#if NOTHING
			   // Choose an appropriate window style
			   if (displayFullScreen)
			   {
					xOffset = 0;
					yOffset = 0;
					windowStyle = WS_POPUP;
			   }
			   else
			   {
					windowStyle = WS_OVERLAPPEDWINDOW;
			   }
			
			   // Build a window for this application
			   rect.top = rect.left = 0;
			   rect.right = width[Movie];
			   rect.bottom = height[Movie];
			   AdjustWindowRect(&rect,	windowStyle, false);
			   appWin = CreateWindow(
				   "FalconDisplay",			/* class */
				   "3D Output",				/* caption */
				   windowStyle,					/* style */
				   40,			/* init. x pos */
				   40,			/* init. y pos */
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
			
			   // Display the new rendering window
			   ShowWindow( appWin, SW_SHOW );
			#endif
			   MakeWindow();
			   // Set up the display device manager
			   devmgr.Setup( languageNum );
			//   TheaterReload(FalconTerrainDataDir); // JPO test if this works.
#endif
		}

		public void Cleanup()
		{
#if TODO
			devmgr.Cleanup();
		
			DeviceIndependentGraphicsCleanup();
#endif
		}

		public void SetSimMode(int nwidth, int nheight, int ndepth)
		{
			width[(int)DisplayMode.Sim] = nwidth;
			height[(int)DisplayMode.Sim] = nheight;
			depth[(int)DisplayMode.Sim] = ndepth;
		}

		public void EnterMode (DisplayMode newMode, int theDevice = 0,int Driver = 0)
		{
#if TODO
			RECT rect;
		
			#if _FORCE_MAIN_THREAD
			Debug.Assert(::GetCurrentThreadId() == GetWindowThreadProcessId(appWin, null));	// Make sure this is called by the main thread
			#endif
		
			currentMode = newMode;
		
			rect.top = rect.left = 0;
			rect.right = width[currentMode];
			rect.bottom = height[currentMode];
			AdjustWindowRect(&rect, windowStyle, false);
		
			SetWindowPos(appWin, null, xOffset, yOffset,
				rect.right-rect.left, rect.bottom-rect.top, SWP_NOZORDER);
		
		
			DeviceManager::DDDriverInfo *pDI = FalconDisplay.devmgr.GetDriver(Driver);
		
			if(pDI)
			{
				if((g_bForceSoftwareGUI || pDI.Is3dfx() || !pDI.CanRenderWindowed()) && currentMode != Sim)
				{
		#if !NOTHING
					// V1, V2 workaround - use primary display adapter with RGB Renderer
					int nIndexPrimary = FalconDisplay.devmgr.FindPrimaryDisplayDriver();
					Debug.Assert(nIndexPrimary != -1);
		
					if(nIndexPrimary != -1)
					{
						DeviceManager.DDDriverInfo pDI = FalconDisplay.devmgr.GetDriver(nIndexPrimary);
						int nIndexRGBRenderer = pDI.FindRGBRenderer();
						Debug.Assert(nIndexRGBRenderer != -1);
		
						if(nIndexRGBRenderer != -1)
						{
							Driver = nIndexPrimary;
							theDevice = nIndexRGBRenderer;
						}
					}
		#else
					displayFullScreen = true;	// force fullscreen
		#endif
				}
		
				if(!pDI.SupportsSRT() && PlayerOptions.bFastGMRadar)
					PlayerOptions.bFastGMRadar = false;
			}
		
		
			theDisplayDevice.Setup( Driver, theDevice, width[currentMode],
				height[currentMode], depth[currentMode], displayFullScreen, doubleBuffer[currentMode], appWin, currentMode == Sim);
		
			SetForegroundWindow (appWin);
			Sleep (0);
#endif
			throw new NotImplementedException();
		}
		
		public void LeaveMode()	
		{
#if TODO	
			#if _FORCE_MAIN_THREAD
			Debug.Assert(::GetCurrentThreadId() == GetWindowThreadProcessId(appWin, null));	// Make sure this is called by the main thread
			#endif
		
		   theDisplayDevice.Cleanup();
#endif
			throw new NotImplementedException();
		}

		public void ToggleFullScreen()
		{
#if TODO			
			#if _FORCE_MAIN_THREAD
			Debug.Assert(::GetCurrentThreadId() == GetWindowThreadProcessId(appWin, null));	// Make sure this is called by the main thread
			#endif
		
			LeaveMode();
			DestroyWindow(appWin);
			displayFullScreen = !displayFullScreen;
			MakeWindow();
			EnterMode(currentMode);
#endif
			throw new NotImplementedException();			
		}		

		public void MakeWindow()
		{
#if TODO
			RECT		rect;
			
			// Choose an appropriate window style
			if (displayFullScreen)
			{
				xOffset = 0;
				yOffset = 0;
				windowStyle = WS_POPUP;
			}
			else
			{
				windowStyle = WS_OVERLAPPEDWINDOW;
			}
			
			// Build a window for this application
			rect.top = rect.left = 0;
			rect.right = width[Movie];
			rect.bottom = height[Movie];
			AdjustWindowRect(&rect,	windowStyle, false);
			appWin = CreateWindow(
			   "FalconDisplay",			/* class */
			   "F4:OS 3D Output",				/* caption */
			   windowStyle,					/* style */
			   40,			/* init. x pos */
			   40,			/* init. y pos */
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
			
			UpdateWindow(appWin);
			SetFocus(appWin);
			
			// Display the new rendering window
			ShowWindow( appWin, SW_SHOW );
#endif
		}

		public ImageBuffer GetImageBuffer() {return theDisplayDevice.GetImageBuffer();}
#if TODO	
		// OW

		protected void _LeaveMode();
		protected void _EnterMode(DisplayMode newMode, int theDevice = 0,int Driver = 0);
		protected void _ToggleFullScreen();
	
		protected CALLBACK SimWndProc (HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam);
		protected CALLBACK FalconMessageHandler (HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam);
#endif
	};
}

