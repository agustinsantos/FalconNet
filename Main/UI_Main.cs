using System;
using FalconNet.FalcLib;
using FalconNet.UI;
using FalconNet.Common;
using FalconNet.Graphics;

namespace FalconNet.Main
{
	public class UI_Main
	{
		public void UI_Startup ()
		{
			//	BITMAPINFO	bmi;
			C_Window	win;
			//	char		*cptr;
			ImageBuffer Primary;
			int			i;
			//	long		count;
			//	WORD		*color,*dest;
			DWORD		r_mask, g_mask, b_mask;

			// M.N. Large UI
			if (F4Config.g_bHiResUI)
			FalconDisplayConfiguration.FalconDisplay.EnterMode(DisplayMode.UILarge,
														DisplayOptionsClass.DisplayOptions.DispVideoCard, 
				                                        DisplayOptionsClass.DisplayOptions.DispVideoDriver);
			else 
			FalconDisplayConfiguration.FalconDisplay.EnterMode(DisplayMode.UI,
														DisplayOptionsClass.DisplayOptions.DispVideoCard, 
				                                        DisplayOptionsClass.DisplayOptions.DispVideoDriver);
	
			Primary=FalconDisplayConfiguration.FalconDisplay.GetImageBuffer();
#if TODO			
			Primary.GetColorMasks( ref r_mask, ref g_mask, ref b_mask );
			UI95_SetScreenColorInfo((WORD)(r_mask), (WORD)(g_mask), (WORD)(b_mask));
#endif
			throw new NotImplementedException();
		}
		
		public void UI_Cleanup()
		{
			throw new NotImplementedException();
		}
		
		void GlobalSetup()
		{
			throw new NotImplementedException();
		}
		
		void SetStartupFlags()
		{
			throw new NotImplementedException();
		}
		
		void PlayUIMusic()
		{
			throw new NotImplementedException();
		}

		void UI_LoadSkyWeatherData()
		{
			throw new NotImplementedException();
		}

		protected void HookupControls(long ID)
		{
			throw new NotImplementedException();
		}

		protected void CloseAllRenderers(long openID)
		{
			throw new NotImplementedException();
		}
		
		protected void LeaveCurrentGame()
		{
			throw new NotImplementedException();
		}
		
		protected void LoadMainWindow()
		{
			throw new NotImplementedException();
		}
	}
}

