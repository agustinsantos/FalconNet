using System;
using FalconNet.UI;
using FalconNet.FalcLib;
using System.IO;


namespace FalconNet.Main
{
	class MainClass
	{
		private static void ParseCommandLine(string[] args)
		{
			F4Find.InitDirectories();
		}
		
		private static void HandleWinMain(string[] args)
		{
			F4Config.ReadFalcon4Config();	// OW: read config file
			ParseCommandLine (args);
#if TODO	
			TheWeather = new WeatherClass();
		
			// This SHOULD NOT BE REQUIRED -- IT IS _VERY_ EASY TO BREAK CODE THAT DEPENDS ON THIS
			// I'd like to make it go away soon....
			Directory.SetCurrentDirectory(F4Find.FalconDataDirectory);
 	
			Mddriver.FileVerify();
			
			fileName = F4Find.FalconObjectDataDir + "Falcon4.init";

			gLangIDNum = GetPrivateProfileInt("Lang", "Id", 0, fileName);
			
			UI_LoadSkyWeatherData();

			FalconDisplay.Setup(gLangIDNum);
 #endif
			StartUI();
		}
		
		private static void StartUI()
		{
#if TODO			
			KeepFocus=0;
			TheCampaign.Suspend();
			if (wParam)
				g_theaters.DoSoundSetup();

			FalconLocalSession.SetFlyState(FLYSTATE_IN_UI);
#if DEBUG
			gPlayerPilotLock = 0;
#endif
			doUI=TRUE;
			
			ui_main.UI_Startup();			
			TheCampaign.Resume();
#endif
			throw new NotImplementedException();
		}
		static UI_Main ui_main = new UI_Main();
		
		public static void Main (string[] args)
		{
			Console.WriteLine ("FalconNet started...!");
			ui_main.GlobalSetup ();
			ui_main.UI_Startup();
			HandleWinMain(args);
			Console.WriteLine ("Press any key to finish");
			Console.ReadLine();
		}
	}
}
