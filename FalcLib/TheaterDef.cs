using FalconNet.CampaignBase;
using FalconNet.F4Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace FalconNet.FalcLib
{
    public class TheaterDef
    {
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

        //TODO TheaterDef *m_next; // link

        public static TheaterList g_theaters;
    }

    public class TheaterList
    {
        public static void LoadTheaterList()
        {
            string tlist = F4File.FalconDataDirectory + THEATERLIST;
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

        public int Count() { return theaterlist.Count; }

        private void SetPathName(string dest, string src, string reldir)
        {
            if (string.IsNullOrWhiteSpace(src))
                dest = reldir;
            else if (File.Exists(src)) // probably full pathname
                dest = src;
            else
                dest = reldir + Path.DirectorySeparatorChar + src;
        }

        private static void LoadTheaterDef(string name)
        {
#if TODO
            SimlibFileClass theaterfile;

            theaterfile = SimlibFileClass.Open(name, SIMLIB_READ);

            if (theaterfile == null) return;

            TheaterDef theater = new TheaterDef();
            // memset(theater, 0, sizeof * theater);

            if (ParseSimlibFile(theater, theaterdesc, theaterfile) == false)
            {
                // delete theater;
            }
            else
            {
                AddTheater(theater);
            }

            theaterfile.Close();
#endif
            throw new NotImplementedException();
        }

        private Dictionary<string, TheaterDef> theaterlist = new Dictionary<string, TheaterDef>();
        private TheaterDef actualTheater;

        private const string THEATERLIST = "theater.lst";
    }
}

