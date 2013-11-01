using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FalconNet.Common
{
    public class PluginManager : ISystem
    {
        static PluginManager() { }
        private static PluginManager _instance = new PluginManager();
        public static PluginManager Instance { get { return _instance; } }
        private PluginManager()
        {
            LoadList = new List<string>();
            Plugins = new List<IPlugin>();
        }

        public List<string> LoadList { get; private set; }
        public List<IPlugin> Plugins { get; private set; }



        public bool RegisterPlugin(string fileName)
        {
            if (LoadList != null)
            {
                LoadList.Add(fileName);
                return true;
            }

            return LoadPlugin(fileName);
        }

        public void LoadPlugins()
        {
            if (LoadList == null)
                return;

            foreach (string fileName in LoadList)
                LoadPlugin(fileName);

            LoadList = null;
        }

        private bool LoadPlugin(string fileName)
        {
            fileName = Path.GetFullPath(fileName);

            if (!File.Exists(fileName))
                return false;

            Assembly ass = Assembly.LoadFile(fileName);
            foreach (IPlugin plugin in from type in ass.GetExportedTypes() where type.GetInterfaces().Any(x => x == typeof(IPlugin)) select (IPlugin)Activator.CreateInstance(type))
            {
                if (plugin.Initialize())
                    Plugins.Add(plugin);
                else
                    return false;

                break;
            }

            return true;
        }

        public string Name
        {
            get { return "Plugin Manager"; }
        }

        public bool Initialize()
        {
            LoadPlugins();
            return true;
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}
