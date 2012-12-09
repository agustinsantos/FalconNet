// TODO delete this define, for the moment is just for testing purposes
#define USE_GWEN

using System;
using FalconNet.UI;
using FalconNet.FalcLib;
using System.IO;
using OpenTK;
using FalconNet.Ui95;
using System.Diagnostics;


namespace FalconNet.Main
{
	public class MainClass
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
		
		[STAThread]
		public static void Main (string[] args)
		{	

			TextWriterTraceListener tr1 = new TextWriterTraceListener(System.Console.Out);
			Debug.Listeners.Add(tr1);
			Debug.WriteLine ("FalconNet started...!");

			//GuiConfiguration guiConf = ui_main.GlobalSetup ();
#if USE_GWEN	
			NewParser parser = new NewParser();
			parser.ParserFile(@"art\main\main_win.scf");
			GuiConfiguration guiConf = parser.guiConf; 
			using (MainGwenGui example = new MainGwenGui(guiConf))
            {
                example.Title = "FalconNet Gwen GUI test";
                example.VSync = VSyncMode.Off; // to measure performance
                example.Run(0.0, 0.0);
                //example.TargetRenderFrequency = 60;
            }
#else			
			ui_main.UI_Startup();
			HandleWinMain(args);
			Console.WriteLine ("Press any key to finish");
			Console.ReadLine();
#endif	
		}
	}
}
