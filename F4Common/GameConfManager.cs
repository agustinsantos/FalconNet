using FalconNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.F4Common
{
    public class GameConfManager : ISystem
    {
        static GameConfManager() { }
        private static GameConfManager _instance = new GameConfManager();
        public static GameConfManager Instance { get { return _instance; } }
        private GameConfManager() {}

        public static string FalconDirectory = @"C:\Falcon BMS 4.32\";

        public string Name
        {
            get { return "Configuration Manager"; }
        }

        public bool Initialize()
        {
            log4net.Config.BasicConfigurator.Configure();
            F4File.FalconDirectory = FalconDirectory;
            return true;
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}
