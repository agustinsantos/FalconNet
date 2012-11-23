using System;
using System.IO;
using FalconNet.FalcLib;
using FalconNet.Common;
using Objective = FalconNet.Campaign.ObjectiveClass;
using Path = FalconNet.Campaign.BasePathClass;
using GridIndex = System.Int16;
using Control = System.Byte;
using System.Diagnostics;

namespace FalconNet.Campaign
{
	public static class CampaignStatic
	{
		// ============
		// Map Deltas
		// ============

		public static GridIndex[] dx = new GridIndex[17];     // dx per direction
		public static GridIndex[] dy = new GridIndex[17];     // dy per direction

		// ============
		// Placeholders
		// ============
		//TODO typedef UnitClass* Unit;
		//TODOtypedef ObjectiveClass* Objective;

		// =====================
		// Campaign wide globals
		// =====================

		public static F4CSECTIONHANDLE campCritical;
		public static int[] VisualDetectionRange = new int[(int)DamageDataType.OtherDam];
		public static byte[] DefaultDamageMods = new byte[(int)DamageDataType.OtherDam + 1];

		// ================
		// Defines & macros
		// ================

		private const int MAX_WCH_FILES = 4;
		private static int next_wch_file = 0;
		private static FileStream[] wch_fp = new FileStream[MAX_WCH_FILES];
		private static string[] wch_filename = new string[MAX_WCH_FILES];
		public const int InfiniteCost = 32000;

		// TODO #define CampEnterCriticalSection()	F4EnterCriticalSection(campCritical)
		// TODO #define CampLeaveCriticalSection()	F4LeaveCriticalSection(campCritical)

		// ======================
		// public static al functions
		// ======================

		public static Objective AddObjectiveToCampaign (GridIndex x, GridIndex y)
		{
			Objective o;

			o = ObjectivStatic.NewObjective ();
			o.SetLocation (x, y);
			o.UpdateObjectiveLists ();
			CampListStatic.RebuildObjectiveLists ();
			CampListStatic.RebuildParentsList ();
			return o;
		}

		public static void RemoveObjective (Objective O)
		{
			throw new NotImplementedException ();
		}

		public static bool LoadTheater (string theater)
		{
			// This assumes the Class Table was loaded elsewhere
			CampEnterCriticalSection ();
			if (!CampTerrStatic.LoadTheaterTerrain (theater)) {
				Debug.WriteLine ("Failed to open theater: {0}, using default theater.", theater);
				CampTerrStatic.Map_Max_X = CampTerrStatic.Map_Max_Y = 500;
				CampTerrStatic.InitTheaterTerrain ();
				CampLeaveCriticalSection ();
				return false;
			}
			Name.LoadNames (theater);
			CampLeaveCriticalSection ();
			return true;
		}

		public static int SaveTheater (string filename)
		{
#if TODO
            CampEnterCriticalSection();
            SaveTheaterTerrain(theater);
            SaveNames(theater);
            CampLeaveCriticalSection();
            return 1;
#endif
			throw new NotImplementedException ();
		}

		public static int LinkCampaignObjectives (Path p, Objective O1, Objective O2)
		{
#if TODO
            int i = 0, cost = 0, found = 0;
            byte[] costs = new byte[MOVEMENT_TYPES]; // TODO={254};
            GridIndex x = 0, y = 0;

            for (i = 0; i < MOVEMENT_TYPES; i++)
            {
                if (FindLinkPath(path, O1, O2, (MoveType)i) < 1)
                {
                    if (FindLinkPath(path, O2, O1, (MoveType)i) < 1)
                        costs[i] = 255;
                    else
                    {
                        O2.GetLocation(&x, &y);
                        cost = (FloatToInt32(path.GetCost()) + cost) / 2;
                        if (cost > 254)
                            cost = 254;			// If it's possible to move, but very expensive, mark it as our max movable cost (254)
                        costs[i] = (byte)cost;
                        found = 1;
                    }
                }
                else
                {
                    O1.GetLocation(&x, &y);
                    cost = FloatToInt32(path.GetCost());
                    if (cost > 254)
                        cost = 254;				// If it's possible to move, but very expensive, mark it as our max movable cost (254)
                    costs[i] = (byte)cost;
                    found = 1;
                }
                // KCK Hack: If we can get there by road, fool it into thinking it's a 254 cost link for
                // any otherwise unfound paths.
                if (costs[i] == 255 && (i == Foot || i == Wheeled || i == Tracked) && costs[NoMove] < 255)
                    costs[i] = 254;
            }
            if (found)
            {
                O1.AddObjectiveNeighbor(O2, costs);
                O2.AddObjectiveNeighbor(O1, costs);
                return 1;
            }
            else
                return 0;
#endif
			throw new NotImplementedException ();
		}

		public static int UnLinkCampaignObjectives (Objective O1, Objective O2)
		{
#if TODO
            int i, unlinked = 0;

            for (i = 0; i < O1.NumLinks(); i++)
            {
                if (O1.GetNeighborId(i) == O2.Id())
                {
                    O1.RemoveObjectiveNeighbor(i);
                    unlinked = 1;
                }
            }
            for (i = 0; i < O2.NumLinks(); i++)
            {
                if (O2.GetNeighborId(i) == O1.Id())
                {
                    O2.RemoveObjectiveNeighbor(i);
                    unlinked = 1;
                }
            }
            return unlinked;
#endif
			throw new NotImplementedException ();
		}

		public static int RecalculateLinks (Objective o)
		{
#if TODO
            PathClass path;
            Objective n;
            char nn;

            for (nn = 0; nn < o.NumLinks(); nn++)
            {
                n = o.GetNeighbor(nn);
                if (n)
                    LinkCampaignObjectives(&path, o, n);
            }
            return 1;
#endif
			throw new NotImplementedException ();
		}

		public static UnitClass GetUnitByXY (GridIndex I, GridIndex J)
		{
			throw new NotImplementedException ();
		}

		public static UnitClass AddUnit (GridIndex I, GridIndex J, char Side)
		{
#if TODO
            Unit nu;

            nu = NewUnit(DOMAIN_LAND, TYPE_BRIGADE, STYPE_UNIT_ARMOR, 1, null);
            nu.SetOwner(Side);
            nu.ResetLocations(x, y);
            nu.ResetDestinations(x, y);
            nu.SetUnitOrders(GORD_DEFEND);
            return nu;
#endif
			throw new NotImplementedException ();
		}

		public static UnitClass CreateUnit (Control who, int Domain, UnitType Type, byte SType, byte SPType, Unit Parent)
		{
			throw new NotImplementedException ();
		}

		public static void RemoveUnit (UnitClass u)
		{
			u.Remove ();
		}

		public static int TimeOfDayGeneral (CampaignTime time)
		{
#if TODO
            // 2001-04-10 MODIFIED BY S.G. SO IT USES MILISECOND AND NOT MINUTES! I COULD CHANGE THE .H FILE BUT IT WOULD TAKE TOO LONG TO RECOMPILE :-(
            /*	if (time < TOD_SUNUP)
                    return TOD_NIGHT;
                else if (time < TOD_SUNUP + 120*CampaignMinutes)
                    return TOD_DAWNDUSK;
                else if (time < TOD_SUNDOWN - 120*CampaignMinutes)
                    return TOD_DAY;
                else if (time < TOD_SUNDOWN)
                    return TOD_DAWNDUSK;
                else
                    return TOD_NIGHT;
            */
            // time can be over one day so I need to keep just the time in the current day...
            time = time % CampaignTime.CampaignDay;

            if (time < TOD_SUNUP * CampaignMinutes)
                return TOD_NIGHT;
            else if (time < TOD_SUNUP * CampaignMinutes + 120 * CampaignMinutes)
                return TOD_DAWNDUSK;
            else if (time < TOD_SUNDOWN * CampaignMinutes - 120 * CampaignMinutes)
                return TOD_DAY;
            else if (time < TOD_SUNDOWN * CampaignMinutes)
                return TOD_DAWNDUSK;
            else
                return TOD_NIGHT;
#endif
			throw new NotImplementedException ();
		}

		public static int TimeOfDayGeneral ()
		{
#if TODO
            return TimeOfDayGeneral(TheCampaign.TimeOfDay);
#endif
			throw new NotImplementedException ();
		}

		public static CampaignTime TimeOfDay ()
		{
#if TODO
            return TheCampaign.TimeOfDay;
#endif
			throw new NotImplementedException ();
		}

		public static int CreateCampFile (string filename, string path)
		{
#if TODO
            string fullname;
            FileStream fp;

            // This filename doesn't exist yet (At least res manager doesn't think so)
            // Create it, so that the manager can find it -
            sprintf(fullname, "%s\\%s", path, filename);
            fp = fopen(fullname, "wb");
            fclose(fp);
            // Now add the current save directory path, if we still can't find this file
            //	if (!ResExistFile(filename))
            //		ResAddPath(path, false);
            return 1;
#endif
			throw new NotImplementedException ();
		}

		public static FileStream OpenCampFile (string filename, string ext, string mode)
		{
#if TODO
            string fullname, path;
            int index;

            string buffer;

            FileStream fp;

            if (strcmp(ext, "wch") == 0)
            {
                for (index = 0; index < MAX_WCH_FILES; index++)
                {
                    if ((wch_fp[index]) && (strcmp(wch_filename[index], filename) == 0))
                    {
                        fseek(wch_fp[index], 0, 0);

                        return wch_fp[index];
                    }
                }
            }

            sprintf(buffer, "OpenCampFile %s.%s %s\n", filename, ext, mode);
            // MonoPrint (buffer);
            // OutputDebugString (buffer);

            // 2002-03-25 MN added check for not being WCH file - otherwise can crash sometimes
            // especially after theater switching situations !
            if ((reading_campressed_file) && (mode[0] == 'r') && strcmp(ext, "wch") != 0)
            {
                if (strcmp(filename, camp_file_name) != 0 && IsCampFile(camp_game_type, filename))
                {
                    EndReadCampFile();
                    StartReadCampFile(camp_game_type, filename);
                    Debug.Assert(reading_campressed_file);
                }

                strcpy(fullname, filename);
                strcat(fullname, ".");
                strcat(fullname, ext);

                for (index = 0; index < camp_num_files; index++)
                {
                    if (string_compare_extensions(fullname, camp_names[index]) == 0)
                    {
                        fseek(camp_fp, camp_offset[index], 0);

                        return camp_fp;
                    }
                }

                // MonoPrint ("Cannot OpenCampFile while we are reading a campressed file\n");
            }

            if ((writing_campressed_file) && (mode[0] == 'w'))
            {
                strcpy(fullname, filename);
                strcat(fullname, ".");
                strcat(fullname, ext);

                fseek(camp_fp, 0, 2);

                strcpy(camp_names[camp_num_files], fullname);

                camp_offset[camp_num_files] = ftell(camp_fp);

                camp_size[camp_num_files] = 0;

                return camp_fp;
            }

            if (strcmp(ext, "cmp") == 0)
                sprintf(path, F4Find.FalconCampUserSaveDirectory);
            else if (stricmp(ext, "obj") == 0)
                sprintf(path, F4Find.FalconCampUserSaveDirectory);
            else if (stricmp(ext, "obd") == 0)
                sprintf(path, F4Find.FalconCampUserSaveDirectory);
            else if (stricmp(ext, "uni") == 0)
                sprintf(path, F4Find.FalconCampUserSaveDirectory);
            else if (stricmp(ext, "tea") == 0)
                sprintf(path, F4Find.FalconCampUserSaveDirectory);
            else if (stricmp(ext, "wth") == 0)
                sprintf(path, F4Find.FalconCampUserSaveDirectory);
            else if (stricmp(ext, "plt") == 0)
                sprintf(path, F4Find.FalconCampUserSaveDirectory);
            else if (stricmp(ext, "mil") == 0)
                sprintf(path, F4Find.FalconCampUserSaveDirectory);
            else if (stricmp(ext, "tri") == 0)
                sprintf(path, F4Find.FalconCampUserSaveDirectory);
            else if (stricmp(ext, "evl") == 0)
                sprintf(path, F4Find.FalconCampUserSaveDirectory);
            else if (stricmp(ext, "smd") == 0)
                sprintf(path, F4Find.FalconCampUserSaveDirectory);
            else if (stricmp(ext, "sqd") == 0)
                sprintf(path, F4Find.FalconCampUserSaveDirectory);
            else if (stricmp(ext, "pol") == 0)
                sprintf(path, F4Find.FalconCampUserSaveDirectory);
            else if (stricmp(ext, "ct") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "ini") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "ucd") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "ocd") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "fcd") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "vcd") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "wcd") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "rcd") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "icd") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "rwd") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "vsd") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "swd") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "acd") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "wld") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "phd") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "pd") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "fed") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "ssd") == 0)
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "rkt") == 0)		// 2001-11-05 Added by M.N.
                sprintf(path, FalconObjectDataDir);
            else if (stricmp(ext, "ddp") == 0)		// 2002-04-20 Added by M.N.
                sprintf(path, FalconObjectDataDir);
            else
                sprintf(path, FalconCampaignSaveDirectory);

            //	Outdated by resmgr:
            //	if (!ResExistFile(filename))
            //		ResAddPath(path, false);

            sprintf(fullname, "%s\\%s.%s", path, filename, ext);
            fp = fopen(fullname, mode);

            if ((fp) && (strcmp(ext, "wch") == 0))
            {
                index = next_wch_file++;

                //TODO F4Assert (next_wch_file <= MAX_WCH_FILES)

                wch_fp[index] = fp;
                strcpy(wch_filename[index], filename);
            }

            return fp;
#endif
			throw new NotImplementedException ();
		}

		public static void CloseCampFile (FileStream fs)
		{
#if TODO
            int
                index;

            for (index = 0; index < MAX_WCH_FILES; index++)
            {
                if (fp == wch_fp[index])
                {
                    // do nothing - we want to keep this file open...
                    // MonoPrint ("Keeping WCH File %s.wch\n", wch_filename);
                    return;
                }
            }

            if (fp == camp_fp)
            {
                if (writing_campressed_file)
                {
                    fseek(camp_fp, 0, 2);

                    camp_size[camp_num_files] = ftell(camp_fp) - camp_offset[camp_num_files];

                    camp_num_files++;
                }
                else if (reading_campressed_file)
                {
                }
            }
            else
            {
                if (fp)
                {
                    fclose(fp);
                }
            }
#endif
			throw new NotImplementedException ();
		}

		public static void StartReadCampFile (FalconGameType type, string filename)
		{
#if TODO
            int index,
                str_len,
                offset;

            string path;

            if (reading_campressed_file)
            {
                MonoPrint("Already Reading Campressed File\n");
                Debug.Assert(!reading_campressed_file);
                return;
            }

            reading_campressed_file = TRUE;

            GetCampFilePath(type, filename, path);

            camp_game_type = type;
            strcpy(camp_file_name, filename);

            camp_fp = fopen(path, "rb");

            if (camp_fp)
            {
                // MonoPrint ("Opening Campressed File %s\n", path);

                fread(&offset, 4, 1, camp_fp);

                fseek(camp_fp, offset, 0);

                fread(&camp_num_files, 4, 1, camp_fp);

                for (index = 0; index < camp_num_files; index++)
                {
                    fread(&str_len, 1, 1, camp_fp);

                    str_len &= 0xff;

                    fread(camp_names[index], str_len, 1, camp_fp);

                    camp_names[index][str_len] = 0;

                    fread(&camp_offset[index], 4, 1, camp_fp);

                    fread(&camp_size[index], 4, 1, camp_fp);

                    // MonoPrint ("  %s %d %d\n", camp_names[index], camp_offset[index], camp_size[index]);
                }
            }
            else
            {
                MonoPrint("Cannot Open %s\n", path);

                ShiWarning("Cannot Open Campressed File\n");
            }
#endif
			throw new NotImplementedException ();
		}

		public static byte[] ReadCampFile (string filename, string ext)
		{
#if TODO
			int 		size,		index;

			byte[] data;
			string buffer;

			FileStream fp;

			if (reading_campressed_file) {
				if (strcmp (filename, camp_file_name) != 0 && IsCampFile (camp_game_type, filename)) {
					EndReadCampFile ();
					StartReadCampFile (camp_game_type, filename);
					ShiAssert (reading_campressed_file);
				}
				strcpy (buffer, filename);
				strcat (buffer, ".");
				strcat (buffer, ext);

				for (index = 0; index < camp_num_files; index ++) {
					if (string_compare_extensions (buffer, camp_names [index]) == 0) {
						fseek (camp_fp, camp_offset [index], 0);

						data = new char [camp_size [index] + 1];
				
						fread (data, camp_size [index], 1, camp_fp);

						data [camp_size [index]] = 0;

						return data;
					}
				}

				reading_campressed_file = FALSE;

				fp = OpenCampFile (filename, ext, "rb");
		
				reading_campressed_file = TRUE;
			} else {
				fp = OpenCampFile (filename, ext, "rb");
			}
	
			if (fp) {
				fseek (fp, 0, 2);

				size = ftell (fp);

				fseek (fp, 0, 0);
		
				data = new char [size + 1];
		
				fread (data, size, 1, fp);
		
				data [size] = 0;
		
				fclose (fp);

				return data;
			}

			return null;
#endif
		throw new NotImplementedException();
		}

		public static void EndReadCampFile ()
		{
#if TODO
            if (camp_fp)
            {
                fclose(camp_fp);

                camp_fp = null;

                camp_num_files = 0;
            }

            reading_campressed_file = false;
#endif
			throw new NotImplementedException ();
		}

		public static void StartWriteCampFile (FalconGameType type, string filename)
		{
#if TODO
            string path;

            writing_campressed_file = TRUE;

            if (type == game_TacticalEngagement)
            {
                sprintf(path, "%s\\%s.tac", F4Find.FalconCampUserSaveDirectory, filename);
            }
            else
            {
                sprintf(path, "%s\\%s.cam", F4Find.FalconCampUserSaveDirectory, filename);
            }

            camp_fp = fopen(path, "wb");

            if (camp_fp)
            {
                camp_num_files = 0;

                fwrite(&camp_num_files, sizeof(int), 1, camp_fp);
            }
            else
            {
                MonoPrint("Cannot Open %s\n", path);

                ShiWarning("Cannot Open Campressed File\n");
            }
#endif
			throw new NotImplementedException ();
		}

		public static void WriteCampFile (string filename, string ext, string data, int size)
		{
			throw new NotImplementedException ();
		}

		public static void EndWriteCampFile ()
		{
#if TODO
            int
                str_len,
                index,
                offset;

            if (camp_fp)
            {
                // MonoPrint ("Writing Campressed File\n");

                fseek(camp_fp, 0, 2);

                offset = ftell(camp_fp);

                fseek(camp_fp, 0, 0);

                fwrite(&offset, sizeof(int), 1, camp_fp);

                fseek(camp_fp, offset, 0);

                fwrite(&camp_num_files, 4, 1, camp_fp);

                for (index = 0; index < camp_num_files; index++)
                {
                    str_len = strlen(camp_names[index]);

                    fwrite(&str_len, 1, 1, camp_fp);

                    fwrite(camp_names[index], str_len, 1, camp_fp);

                    fwrite(&camp_offset[index], 4, 1, camp_fp);

                    fwrite(&camp_size[index], 4, 1, camp_fp);

                    // MonoPrint ("  %s %d %d\n", camp_names[index], camp_offset[index], camp_size[index]);
                }

                fclose(camp_fp);

                camp_fp = null;
            }

            writing_campressed_file = false;
#endif
			throw new NotImplementedException ();
		}

		// Bubble rebuilding stuff
		public static void CampaignRequestSleep ()
		{
			throw new NotImplementedException ();
		}

		public static int CampaignAllAsleep ()
		{
			throw new NotImplementedException ();
		}

		private static void ResizeBubble (int moversInBubble)
		{
#if TODO
            float br, new_ratio;

            br = (float)(PLAYER_BUBBLE_MOVERS - moversInBubble);
            new_ratio = ((FalconSessionEntity*)vuLocalSessionEntity).GetBubbleRatio() * 1.0F + (br * 0.01F);

            if (new_ratio > 1.2F)
            {
                new_ratio = 1.2F;
            }

            if (new_ratio < 0.25F)
            {
                new_ratio = 0.25F;
            }

            ((FalconSessionEntity*)vuLocalSessionEntity).SetBubbleRatio(new_ratio);
#endif
			throw new NotImplementedException ();
		}

		private static void CampEnterCriticalSection ()
		{
			throw new NotImplementedException ();
		}

		private static void CampLeaveCriticalSection ()
		{
			throw new NotImplementedException ();
		}

		private static void GetCampFilePath (FalconGameType type, string filename, out string path)
		{
			if (type == FalconGameType.game_TacticalEngagement) {
				path = F4Find.FalconCampUserSaveDirectory + System.IO.Path.DirectorySeparatorChar + filename + ".tac";
				if (!File.Exists (path))
					path = F4Find.FalconCampUserSaveDirectory + System.IO.Path.DirectorySeparatorChar + filename + ".trn";
			} else {
				path = F4Find.FalconCampUserSaveDirectory + System.IO.Path.DirectorySeparatorChar + filename + ".cam";
			}
		}

		private static bool IsCampFile (FalconGameType type, string filename)
		{
			string path;

			GetCampFilePath (type, filename, out path);
			return File.Exists (path);
		}
	}
}

