using System;

namespace FalconNet.FalcLib
{
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
	
		public enum DisplayMode {Movie, UI, UILarge, Planner, Layout, Sim, NumModes};
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
		// TODO public DeviceManager	devmgr;
		// TODO public DisplayDevice	theDisplayDevice;
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
			wc.hInstance = NULL;
			//   wc.hIcon = NULL;
			 wc.hIcon = LoadIcon (GetModuleHandle(NULL), MAKEINTRESOURCE(105));	// OW BC
			wc.hCursor = NULL;
			wc.hbrBackground = (HBRUSH)GetStockObject(BLACK_BRUSH);
			wc.lpszMenuName = NULL;
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
				   NULL,					/* parent window */
				   NULL,					/* menu handle */
				   NULL,					/* program handle */
				   NULL					/* create parms */
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
			throw new NotImplementedException();
		}
		
		public void LeaveMode()	
		{
			throw new NotImplementedException();
		}

		public void ToggleFullScreen()
		{
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
			   NULL,					/* parent window */
			   NULL,					/* menu handle */
			   NULL,					/* program handle */
			   NULL					/* create parms */
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
#if TODO
		public ImageBuffer GetImageBuffer() {return theDisplayDevice.GetImageBuffer();}
	
		// OW

		protected void _LeaveMode();
		protected void _EnterMode(DisplayMode newMode, int theDevice = 0,int Driver = 0);
		protected void _ToggleFullScreen();
	
		protected CALLBACK SimWndProc (HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam);
		protected CALLBACK FalconMessageHandler (HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam);
#endif
	};
}

