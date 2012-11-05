using System;
using FalconNet.UI;

namespace FalconNet.Main
{
	class MainClass
	{
		private static void HandleWinMain()
		{
			F4Config.ReadFalcon4Config();	// OW: read config file
#if TODO	
			TheWeather = new WeatherClass();
		
			// This SHOULD NOT BE REQUIRED -- IT IS _VERY_ EASY TO BREAK CODE THAT DEPENDS ON THIS
			// I'd like to make it go away soon....
			SetCurrentDirectory(FalconDataDirectory);
		
			FileVerify();
#endif
		}
		
		public static void Main (string[] args)
		{
			Console.WriteLine ("FalconNet started...!");
			HandleWinMain();
			Console.WriteLine ("Press any key to finish");
			Console.ReadLine();
		}
	}
}
