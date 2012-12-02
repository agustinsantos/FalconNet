using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using FalconNet.Common;

namespace FalconNet.Ui95
{
	public class NewParser
	{
		//TODO Defined somewhere
		public static string FalconUIArtDirectory = @"../../../data";
		public static string FalconUIArtThrDirectory = @"../../../data";
		public static string FalconUISoundDirectory = @"../../../data";
		
		private Dictionary<string, long> tableID = new Dictionary<string, long> ();
		private Dictionary<string, string> tableFont = new Dictionary<string, string> ();
		
		public void ParserFile (String filename)
		{
			if (filename.EndsWith (".lst"))
				ParserLstFile (filename);
			else if (filename.EndsWith (".scf"))
				ParserScfFile (filename);
			else if (filename.EndsWith (".irc"))
				ParserIrcFile (filename);
			else if (filename.EndsWith (".id"))
				ParserIdFile (filename);
			else 
				throw new ArgumentException ("Unkown file extension: " + filename);	
		}
		
		protected void ParserLstFile (string filename)
		{
			try {
				FileStream ifp = OpenArtFile (filename);

				using (StreamReader sr = new StreamReader (ifp)) {
					string strLine = sr.ReadLine ();
					while (strLine != null) {
						if (!string.IsNullOrWhiteSpace (strLine) && !strLine.StartsWith ("#")) {
							strLine = strLine.Trim ();
							ParserFile (strLine);
						}
						strLine = sr.ReadLine ();
					}
					sr.Close ();
				}
			} catch (IOException e) {
				Debug.WriteLine ("An IO exception has been thrown!");
				Debug.WriteLine (e.ToString ());
				return;
			}
		}

		protected void ParserScfFile (string filename)
		{
			try {
				FileStream ifp = OpenArtFile (filename);

				using (StreamReader sr = new StreamReader (ifp)) {
					string strLine = sr.ReadLine ();
					while (strLine != null) {
						if (!string.IsNullOrWhiteSpace (strLine) && !strLine.StartsWith ("#")) {
							strLine = strLine.Trim ();
							Debug.WriteLine ("Some SCF found:" + strLine);
						}
						strLine = sr.ReadLine ();
					}
					sr.Close ();
				}
			} catch (IOException e) {
				Debug.WriteLine ("An IO exception has been thrown!");
				Debug.WriteLine (e.ToString ());
				return;
			}
		}

		protected void ParserIrcFile (string filename)
		{
			try {
				FileStream ifp = OpenArtFile (filename);

				using (StreamReader sr = new StreamReader (ifp)) {
					string strLine = sr.ReadLine ();
					while (strLine != null) {
						if (!string.IsNullOrWhiteSpace (strLine) && !strLine.StartsWith ("#")) {
							strLine = strLine.Trim ();
							List<string> tokens = strLine.SplitWords ();
							
							if (tokens[0].ToUpperInvariant() == "[LOADFONT]" && tokens.Count == 3)
								AddTableFont(tokens[1], tokens[2]);
							else
								Debug.WriteLine ("Some IRC found:" + strLine);
						}
						strLine = sr.ReadLine ();
					}
					sr.Close ();
				}
			} catch (IOException e) {
				Debug.WriteLine ("An IO exception has been thrown!");
				Debug.WriteLine (e.ToString ());
				return;
			}
		}

		protected void ParserIdFile (string filename)
		{
			try {
				FileStream ifp = OpenArtFile (filename);

				using (StreamReader sr = new StreamReader (ifp)) {
					string strLine = sr.ReadLine ();
					while (strLine != null) {
						if (!string.IsNullOrWhiteSpace (strLine) && !strLine.StartsWith ("#")) {
							strLine = strLine.Trim ();
							List<string> tokens = strLine.SplitWords ();
							if (tokens.Count != 2)
								throw new ApplicationException ("Error parsing Id file: " + filename + "; line =" + strLine);
							AddTableID (tokens [0], long.Parse (tokens [1]));
						}
						strLine = sr.ReadLine ();
					}
					sr.Close ();
				}
			} catch (IOException e) {
				Debug.WriteLine ("An IO exception has been thrown!");
				Debug.WriteLine (e.ToString ());
				return;
			}
		}

		protected static FileStream OpenArtFile (string filename)
		{
			return OpenArtFile (filename.Replace ('\\', Path.DirectorySeparatorChar), FalconUIArtThrDirectory, FalconUIArtDirectory);
		}
		
		protected static FileStream OpenArtFile (string filename, string thrdir, string maindir)
		{
			string filebuf = filename;
			try {
				if (!filename.StartsWith (@"art/"))
					filename = "art" + Path.DirectorySeparatorChar + filename;
				
				if (char.IsLetter (filename [0]) && filename [1] == ':' && filename [2] == Path.DirectorySeparatorChar)
					return File.Open (filename, FileMode.Open, FileAccess.Read);

				filebuf = thrdir; // Falcon thr root dir
				filebuf += Path.DirectorySeparatorChar + filename;
				if (File.Exists (filebuf))
					return File.Open (filebuf, FileMode.Open, FileAccess.Read);
			
				filebuf = maindir; // Falcon main root dir
				filebuf += Path.DirectorySeparatorChar + filename;
				if (File.Exists (filebuf))	    
					return File.Open (filebuf, FileMode.Open, FileAccess.Read);
				else
					throw new ArgumentException ("File not found " + filename);
			} catch (IOException e) {
				Debug.WriteLine ("Exception opening file: " + filebuf);
				throw e;
			}
		}
		
		public void AddTableID (String str, long id)
		{
			if (tableID.ContainsKey (str)) {
				Debug.WriteLine ("Table ID already contains key =" + str);
				return;
			}
			tableID.Add (str, id);
		}
		
		public void AddTableFont (String name, string file)
		{
			if (tableFont.ContainsKey (name)) {
				Debug.WriteLine ("Table font already contains key =" + name);
				return;
			}
			tableFont.Add (name, file);
		}
		
		public void AddTableID (String str)
		{
			if (tableID.ContainsKey (str)) {
				Debug.WriteLine ("Table ID already contains key =" + str);
				return;
			}
			tableID.Add (str, str.GetHashCode ());
		}
	}
}

