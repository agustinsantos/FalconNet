using FalconNet.CampaignBase;
using FalconNet.F4Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace FalconNet.FalcLib
{
    public class TheaterDef
    {
        public string Name {
            get { return m_name; }
            set { m_name = value; }
        }
        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }
        public string m_name;
        public string m_description;
        public string m_terrain;
        public string m_campaign;
        public string m_bitmap;
        public string m_artdir;
        public string m_uisounddir;
        public string m_moviedir;
        public string m_objectdir;
        public string m_3ddatadir;
        public string m_misctexdir;
        public int m_mintacan;
        public string m_sounddir;

        public string m_cockpitdir;
        public string m_zipsdir;
        public string m_tacrefdir;
        public string m_splashdir;
        //TODO TheaterDef *m_next; // link

        public static TheaterList g_theaters;
    }

    public class TheaterList
    {
        public void LoadTheaterList()
        {
            string tlist = F4File.F4FindFile(THEATERLIST);

            string[] lines = System.IO.File.ReadAllLines(tlist);

            foreach (string line in lines)
            {
                string thr = line.Trim();
                if (string.IsNullOrWhiteSpace(thr) || thr.StartsWith("#"))
                    continue;
                LoadTheaterDef(thr);
            }
        }

        public TheaterDef FindTheaterByName(string name)
        {
            throw new NotImplementedException();
        }

        public TheaterDef GetCurrentTheater()
        {
            return actualTheater;
        }

        public void SetCurrentTheater(TheaterDef td)
        {
            actualTheater = td;
        }

        public bool SetNewTheater(TheaterDef td)
        {
            throw new NotImplementedException();
        }
        public bool ChooseTheaterByName(string name)
        {
            TheaterDef thr = FindTheaterByName(name);

            if (thr != null)
                return SetNewTheater(thr);

            return false;
        }

        public void AddTheater(TheaterDef ntheater)
        {
            theaterlist.Add(ntheater.m_name, ntheater);
        }

        public void DoSoundSetup()
        {
            throw new NotImplementedException();
        }

        public TheaterDef GetTheater(int n)
        {
            throw new NotImplementedException();
            //return theaterlist.Values[n];
        }

        public int Count() 
        { 
            return theaterlist.Count;
        }

        private void SetPathName(string dest, string src, string reldir)
        {
            if (string.IsNullOrWhiteSpace(src))
                dest = reldir;
            else if (File.Exists(src)) // probably full pathname
                dest = src;
            else
                dest = reldir + Path.DirectorySeparatorChar + src;
        }

        private void LoadTheaterDef(string name)
        {
            string fname = F4File.F4FindFile(name);
            SimlibFileClass theaterfile = SimlibFileClass.Open(fname, FileMode.Open, FileAccess.Read);

            if (theaterfile == null) return;

            TheaterDef theater = new TheaterDef();
            // memset(theater, 0, sizeof * theater);

            if (FileReader.ParseSimlibFile(theater, theaterdesc, theaterfile) == false)
            {
                // delete theater;
            }
            else
            {
                AddTheater(theater);
            }

            theaterfile.Close();
        }

        public IEnumerator<TheaterDef> GetEnumerator()
        {
            return theaterlist.Values.GetEnumerator();
        }

        private Dictionary<string, TheaterDef> theaterlist = new Dictionary<string, TheaterDef>();
        private TheaterDef actualTheater;

        private const string THEATERLIST = @"Terrdata\theaterdefinition\theater.lst";
        private static readonly InputDataDesc[] theaterdesc = new InputDataDesc[]
{
        new InputDataDesc{ name = "name", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_name = (string)v },
        new InputDataDesc{ name = "desc", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_description = (string)v },
        new InputDataDesc{ name = "campaigndir", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_campaign = (string)v },
        new InputDataDesc{ name = "terraindir", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_terrain = (string)v },
        new InputDataDesc{ name = "artdir", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_artdir = (string)v },
        new InputDataDesc{ name = "moviedir", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_moviedir = (string)v },
        new InputDataDesc{ name = "uisounddir", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_uisounddir = (string)v },
        new InputDataDesc{ name = "objectdir", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_objectdir = (string)v },
        new InputDataDesc{ name = "3ddatadir", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_3ddatadir = (string)v },
        new InputDataDesc{ name = "misctexdir", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_misctexdir = (string)v },
        new InputDataDesc{ name = "bitmap", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_bitmap = (string)v },
        new InputDataDesc{ name = "mintacan", defvalue = 70, type = format.ID_INT, action = (o, v) => (o as TheaterDef).m_mintacan = (int)v },
        new InputDataDesc{ name = "sounddir", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_sounddir = (string)v },
        new InputDataDesc{ name = "cockpitdir", defvalue = "art\\ckptart", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_cockpitdir = (string)v },
        new InputDataDesc{ name = "zipsdir", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_zipsdir = (string)v },
        new InputDataDesc{ name = "tacrefdir", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_tacrefdir = (string)v },
        new InputDataDesc{ name = "splashdir", defvalue = "art\\splash", type = format.ID_STRING, action = (o, v) => (o as TheaterDef).m_splashdir = (string)v }
};
    }
}

