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
 #endif
		}
		
		
		public static void Main (string[] args)
		{
			Console.WriteLine ("FalconNet started...!");
			HandleWinMain(args);
			Console.WriteLine ("Press any key to finish");
			Console.ReadLine();
		}
	}
}
