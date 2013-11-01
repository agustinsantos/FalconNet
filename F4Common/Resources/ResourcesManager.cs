using FalconNet.Common;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FalconNet.F4Common
{
    public class ResourcesManager : ISystem
    {
        static ResourcesManager() { }
        private static ResourcesManager _instance = new ResourcesManager();
        public static ResourcesManager Instance { get { return _instance; } }
        private ResourcesManager() { }

        private Dictionary<string, long> tableStrId = new Dictionary<string, long>();
        private Dictionary<long, string> tableIdStr = new Dictionary<long, string>();

        public long IdFromName(string name)
        {
            return tableStrId[name];
        }

        public string NamefromId(long id)
        {
            return tableIdStr[id];
        }

        public string Name
        {
            get { return "Resource Manager"; }
        }

        public bool Initialize()
        {
            LoadIdList();
            return true;
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        private static readonly string[] fileidList = new string[]{ "userids", 
                                                                    "fontids", 
                                                                    "imageids",
                                                                    "soundids",
                                                                    "textids", 
                                                                    "movieids" };
        private void LoadIdList()
        {
            foreach (string file in fileidList)
                LoadLstFile(file);
        }

        private void LoadLstFile(string file)
        {
            string path = F4File.F4FindFile(file, "lst");
            log.Debug("Reading resource file: " + path);
            using (StreamReader sr = new StreamReader(path))
            {
                string strLine = sr.ReadLine();
                while (strLine != null)
                {
                    if (!string.IsNullOrWhiteSpace(strLine) && !strLine.StartsWith("#"))
                    {
                        strLine = strLine.Trim();
                        ParserFile(strLine);
                    }
                    strLine = sr.ReadLine();
                }
                sr.Close();
            }
        }

        private void ParserFile(String filename)
        {
            if (filename.EndsWith(".lst"))
                ParserLstFile(filename);
            //else if (filename.EndsWith(".scf"))
            //    ParserScfFile(filename);
            //else if (filename.EndsWith(".irc"))
            //    ParserIrcFile(filename);
            else if (filename.EndsWith(".id"))
                ParserIdFile(filename);
            else
                throw new ArgumentException("Unkown file extension: " + filename);
        }

        private void ParserLstFile(string file)
        {
            throw new NotImplementedException();
        }

        private void ParserIdFile(string file)
        {
            string path = F4File.F4FindFile(file);
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string strLine;
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(strLine) && !strLine.StartsWith("#"))
                        {
                            strLine = strLine.Trim();
                            List<string> tokens = strLine.SplitWords();
                            if (tokens.Count != 2)
                            {
                                log.Error("Error parsing Id file: " + file + "; line =" + strLine);
                                continue;
                            }
                            try
                            {
                                long id = long.Parse(tokens[1]);
                                if (!tableStrId.ContainsKey(tokens[0]) && !tableIdStr.ContainsKey(id))
                                {
                                    tableStrId.Add(tokens[0], id);
                                    tableIdStr.Add(id, tokens[0]);
                                }
                                else
                                    log.Error("Duplicated key/value found for: " + strLine);
                            }
                            catch (FormatException e)
                            {
                                log.Error("An FormatException has been thrown parsing :" + strLine);
                            }
                        }
                    }
                    sr.Close();
                }
            }
            catch (IOException e)
            {
                log.Error("An IO exception has been thrown! " + e.ToString());
                return;
            }
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
