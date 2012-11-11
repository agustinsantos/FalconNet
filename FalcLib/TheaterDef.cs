using System;
using System.Collections.Generic;

namespace FalconNet.FalcLib
{
	public struct TheaterDef {
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
	    int m_mintacan;
		public string m_sounddir;
	
	    //TODO TheaterDef *m_next; // link
			
		public static TheaterList g_theaters;

	};
	
	public class TheaterList  
	{
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
			throw new NotImplementedException();
		}
	    public void AddTheater(TheaterDef ntheater)
		{
			theaterlist.Add(ntheater);
		}
		public void DoSoundSetup()
		{
			throw new NotImplementedException();
		}
		
	    public TheaterDef GetTheater(int n)
		{
			return theaterlist[n];
		}
		
	    public int Count() { return theaterlist.Count; }
	
		private void SetPathName(string dest, string src, string reldir)
		{
			throw new NotImplementedException();
		}
		private List<TheaterDef> theaterlist = new List<TheaterDef>();
		private TheaterDef actualTheater;
	}
}

