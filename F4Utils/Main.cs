using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using F4Utils.Resources;
using F4Utils.Campaign;
using System.Text;
using System.Diagnostics;
using F4Utils.Terrain;

namespace F4Resources
{
	class MainClass
	{
		public static string FalconDirectory = @"../../../data";

		public static void Main (string[] args)
		{
			TextWriterTraceListener tr1 = new TextWriterTraceListener (System.Console.Out);
			Debug.Listeners.Add (tr1);
			Debug.WriteLine ("FalconNet Converter started...!");
			ReadTerrain ();
			//ConvertResources(FalconDirectory+"/art/resource/music.idx");
			//ReadClassData (FalconDirectory + "/terrdata/objects/FALCON4.ct");
			//ReadCampaign (FalconDirectory + "/campaign/save/Save-Day 1 13 19 39.cam", 71);
			Debug.WriteLine ("FalconNet Converter has finished...!");
		}
		
		public static void ReadTerrain ()
		{
			TerrainBrowser terrain = new TerrainBrowser (true);
			terrain.LoadCurrentTheaterTerrainDatabase ();
			terrain.LoadFarTilesAsync ();
			var mapinfo = terrain.CurrentTheaterDotMapFileInfo;
			Bitmap bitmap = new Bitmap(500, 500);
			for (int x = 0; x < 500; x++)
				for (int y = 0; y < 500; y++) {
					float h = terrain.GetTerrainHeight (x, y);
					bitmap.SetPixel(x, y, mapinfo.Pallete[(int)h]);
				}
			bitmap.Save("Bitmap.bmp");
		}
		
		public static void ReadClassData (string filename)
		{
			Falcon4EntityClassType[] entities = ClassTable.ReadClassTable (filename); 
			int i = 0;
			Debug.WriteLine ("Class file has " + entities.Length + " entities classes");
			if (entities.Length > 0)
				foreach (Falcon4EntityClassType ent in entities)
					if (i < MAX)
						Debug.WriteLine (string.Format ("\t\tEntity {0} type={1}, Vehicle={2}", i++, ent.dataType, ent.vehicleDataIndex));

		}

		private const int MAX = 5;

		public static void ReadCampaign (string filename, int ver)
		{
			F4CampaignFileBundleReader reader = new F4CampaignFileBundleReader (filename);
			EmbeddedFileInfo[] files = reader.GetEmbeddedFileDirectory ();
			Debug.WriteLine ("Campaign file has " + files.Length + " files");
			foreach (EmbeddedFileInfo embfile in files) {
				Debug.WriteLine (string.Format ("Extracting file {0} ", embfile.FileName));
				byte[] fileData = reader.GetEmbeddedFileContents (embfile.FileName);
				using (FileStream fs = new FileStream(embfile.FileName, FileMode.Create)) {
					fs.Write (fileData, 0, fileData.Length);
					fs.Flush ();
					fs.Close ();
				}
				FileInfo file = new FileInfo (embfile.FileName);
				//.CMP file (basic campaign information)
				//.OBJ file (campaign objectives list)
				//.OBD file (campaign objective deltas)
				//.UNI file (campaign units list)
				//.TEA file (campaign teams list)
				//.EVT file (campaign events list)
				//.POL file (campaign primary objectives list)
				//.PLT file (campaign pilots list)
				//.PST file (persistent objects list)
				//.WTH file (weather)
				//.VER file (version information)
				//.TE file (Victory conditions)
				int i;
				switch (file.Extension.ToLowerInvariant ()) {

				case ".cmp":
					CmpFile cmp = new CmpFile (fileData, ver);
					Debug.WriteLine ("CMP data:");
					Debug.WriteLine (string.Format ("\tTheaterName  {0}", cmp.TheaterName));
					Debug.WriteLine (string.Format ("\tScenario  {0}", cmp.Scenario));
					Debug.WriteLine (string.Format ("\tSaveFile  {0}", cmp.SaveFile));
					Debug.WriteLine (string.Format ("\tTE_StartTime  {0}", cmp.TE_StartTime));
					Debug.WriteLine (string.Format ("\tTE_TimeLimit  {0}", cmp.TE_TimeLimit));
					Debug.WriteLine (string.Format ("\tTE_VictoryPoints  {0}", cmp.TE_VictoryPoints));
					Debug.WriteLine (string.Format ("\tTE_number_teams  {0}", cmp.TE_number_teams));
					Debug.WriteLine (string.Format ("\tBullseye at {0}, {1}", cmp.BullseyeX, cmp.BullseyeY));
					break;
					
				case ".obj":
					ObjFile objData = new ObjFile (fileData, ver);
					Debug.WriteLine ("OBJ data:");
					Debug.WriteLine (string.Format ("\tNum Pilots  {0}", objData.objectives.Length));
					i = 0;
					if (objData.objectives.Length > 0)
						foreach (Objective obj in objData.objectives)
							if (i < MAX)
								Debug.WriteLine (string.Format ("\t\tObj {0} Pos {1}, {2}, {3}", i++, obj.x, obj.y, obj.z));
							
					break;
					
				case ".obd":
					ObdFile obdData = new ObdFile (fileData, ver);
					Debug.WriteLine ("OBD data:");
					Debug.WriteLine (string.Format ("\tNum Deltas  {0}", (obdData.deltas != null ? obdData.deltas.Length : 0)));
					break;
					
				case ".uni":
//					UniFile unitData = new UniFile(fileData, ver, );
//					Debug.WriteLine("UNI data:");
//					Debug.WriteLine("\tNum Units  {0}", unitData.units.Length);
//					foreach(Unit unit in unitData.units)
//						Debug.WriteLine("\t\tUnit Pos {0}, {1}, {2}", unit.x, unit.y, unit.z);
					break;	
					
				case ".tea":
					TeaFile teaData = new TeaFile (fileData, ver);
					Debug.WriteLine ("TEA data:");
					Debug.WriteLine ("\tNum Teams  {0}", teaData.numTeams);
					if (teaData.numTeams > 0)
						foreach (Team team in teaData.teams) {
							Debug.WriteLine (string.Format ("\t\tTeam Name {0} with id = {1}", team.name, team.id.num_));
							Debug.WriteLine (string.Format ("\t\tTeam Fuel {0} , reinforcement = {1}", team.fuelAvail, team.reinforcement));
						}
					break;	
					
				case ".plt":
					PltFile plt = new PltFile (fileData, ver);
					Debug.WriteLine ("PLT data:");
					Debug.WriteLine ("\tNum Pilots  {0}", plt.NumPilots);
					if (plt.NumPilots > 0)
						foreach (PilotInfoClass pilot in plt.PilotInfo)
							if (pilot.photo_id != 0 || pilot.voice_id != 0 || pilot.usage != 0)
								Debug.WriteLine (string.Format ("\t\tPilot  {0}, {1}, {2}", pilot.usage, pilot.photo_id, pilot.voice_id));
					break;
					
				case ".pst":
					PstFile pstData = new PstFile (fileData, ver);
					Debug.WriteLine ("PST data:");
					Debug.WriteLine (string.Format ("\tNum Persistant Objects  {0}", pstData.numPersistantObjects));
					i = 0;
					if (pstData.numPersistantObjects > 0)
						foreach (PersistantObject pObj in pstData.persistantObjects)
							if (i < MAX)
								Debug.WriteLine (string.Format ("\t\tObject {0} Pos {1}, {2}", i++, pObj.x, pObj.y));
					break;		

				case ".wth":
					WthFile wthData = new WthFile (fileData, ver);
					Debug.WriteLine ("WTH data:");
					Debug.WriteLine (string.Format ("\tLast Check  {0}", wthData.LastCheck));
					Debug.WriteLine (string.Format ("\tVisibility  {0}, {0}, {0}", wthData.visMin, wthData.visMax, wthData.visibilityPct));
					break;
					
				case ".ver":
					Debug.WriteLine ("VER data:");
					Debug.WriteLine (string.Format ("\tVer  {0}", Encoding.ASCII.GetString (fileData, 0, fileData.Length)));
					break;
				default:
					break;
				}
			}
		}
		
		public static void ConvertResources (string filename)
		{
			Debug.WriteLine ("Converting F4 Resources");
			F4ResourceBundleReader reader = new F4ResourceBundleReader ();
			reader.Load (filename);
			Debug.WriteLine ("Read " + reader.NumResources + " resources");
			for (int i = 0; i < reader.NumResources; i++) {
				Debug.WriteLine ("Resource " + i + " is " + reader.GetResourceType (i));
				string name = reader.GetResourceName (i).ToLowerInvariant ();
				switch (reader.GetResourceType (i)) {
				case F4ResourceType.FlatResource:
					break;
				case F4ResourceType.ImageResource:
					Bitmap bitmap = reader.GetImageResource (i);
					bitmap.Save (name + ".png", ImageFormat.Png);
					break;
				case F4ResourceType.SoundResource:
					byte[] thisSound = reader.GetSoundResource (i);
					using (FileStream fs = new FileStream(name + ".wav", FileMode.Create)) {
						fs.Write (thisSound, 0, thisSound.Length);
						fs.Flush ();
						fs.Close ();
					}
					break;
				case F4ResourceType.Unknown:
					break;
				}
			}
		}
	}
}
