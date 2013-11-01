using FalconNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.F4Common
{
    public class GameEngine : ISystem
    {
        static GameEngine() { }
        private static GameEngine _instance = new GameEngine();
        public static GameEngine Instance { get { return _instance; } }
        private GameEngine()
        {
            InstanceManagers();
        }

        public GameConfManager GameConfig {get; private set;}
        public PluginManager PluginManager { get; private set; }
        public ResourcesManager ResourcesManager { get; private set; }

        private void InstanceManagers()
        {
            GameConfig = GameConfManager.Instance;
            PluginManager = PluginManager.Instance;
            ResourcesManager = ResourcesManager.Instance;
        }

        private void InitializeManagers()
        {
            GameConfig.Initialize();
            PluginManager.Initialize();
            ResourcesManager.Initialize();
        }

        public string Name
        {
            get { return "Game Engine"; }
        }

        public bool Initialize()
        {
            InitializeManagers();
            return true;
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}
