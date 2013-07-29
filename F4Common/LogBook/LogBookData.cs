using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using DWORD = System.UInt64;
using FalconNet.Common.Encoding;
using System.Globalization;

namespace FalconNet.F4Common
{
    public class LogBookData
    {
        private void EncryptPwd()
        {
            char[] ptr = new char[(int)LB_PILOT.PASSWORD_LEN];
            Pilot.Password.ToCharArray().CopyTo(ptr, ptr.Length);

            for (int i = 0; i < ptr.Length; i++)
            {
                ptr[i] ^= PwdMask[i % PwdMask.Length];
                ptr[i] ^= PwdMask2[i % PwdMask2.Length];
            }

            Pilot.Password = new string(ptr);
        }

        private void EncryptASCIIPwd()
        {
            byte[] ptr = Encoding.ASCII.GetBytes(Pilot.Password);
            byte[] PwdMaskAcii = Encoding.ASCII.GetBytes(PwdMask);
            byte[] PwdMask2Acii = Encoding.ASCII.GetBytes(PwdMask2);

            for (int i = 0; i < (int)LB_PILOT.PASSWORD_LEN; i++)
            {
                ptr[i] ^= PwdMaskAcii[i % PwdMask.Length];
                ptr[i] ^= PwdMask2Acii[i % PwdMask2.Length];
            }

            Pilot.Password = Encoding.ASCII.GetString(ptr);
        }

        private void CalcRank()
        {
            LB_RANK NewRank = LB_RANK.SEC_LT;

            if ((Pilot.Campaign.TotalScore > 3200) && Pilot.Campaign.GamesWon != 0)
            {
                NewRank = LB_RANK.COLONEL;
            }
            else if ((Pilot.Campaign.TotalScore > 1600) && (Pilot.Campaign.GamesWon != 0 || Pilot.Campaign.GamesTied != 0))
            {
                NewRank = LB_RANK.LT_COL;
            }
            else if ((Pilot.Campaign.TotalScore > 800) && (Pilot.Campaign.GamesWon != 0 || Pilot.Campaign.GamesTied != 0 || Pilot.Campaign.GamesLost != 0))
            {
                NewRank = LB_RANK.MAJOR;
            }
            else if (Pilot.Campaign.TotalScore > 300)
            {
                NewRank = LB_RANK.CAPTAIN;
            }
            else if (Pilot.Campaign.TotalScore > 150)
            {
                NewRank = LB_RANK.LEIUTENANT;
            }
            else
            {
                NewRank = LB_RANK.SEC_LT;
            }

            if (NewRank > Pilot.Rank)
            {
                CampaignMission.MissionResult |= RESULT_FLAGS.PROMOTION;
                Pilot.Rank = NewRank;
            }
        }

        private void AwardMedals(CAMP_MISS_STRUCT MissStats)
        {
            if (MissStats.Score >= 3)
            {
                int MedalPts = 0;

                if (MissStats.Flags.HasFlag(CAMP_MISS_FLAGS.DESTROYED_PRIMARY))
                    MedalPts += 2;

                if (MissStats.Flags.HasFlag(CAMP_MISS_FLAGS.LANDED_AIRCRAFT))
                    MedalPts++;

                if (MissStats.WingmenLost != 0)
                    MedalPts++;

                MedalPts += MissStats.NavalUnitsKilled + MissStats.Kills +
                                Math.Min(10, MissStats.FeaturesDestroyed / 2) + Math.Min(10, MissStats.GroundUnitsKilled / 2);

                MedalPts = (int)(PlayerOptionsClass.PlayerOptions.Realism * MedalPts * CampaignDifficulty() * MissStats.Score * MissionComplexity(MissStats));

                if ((MedalPts > 9600) && (PlayerOptionsClass.PlayerOptions.Realism > 0.9f) && MissStats.Score >= 4)
                {
                    CampaignMission.MissionResult |= RESULT_FLAGS.AWARD_MEDAL | RESULT_FLAGS.MDL_AFCROSS;
                    Pilot.Medals[(int)LB_MEDAL.AIR_FORCE_CROSS]++;
                    Pilot.Campaign.TotalScore += 20;
                }
                else if ((MedalPts > 7800) && (PlayerOptionsClass.PlayerOptions.Realism > 0.7f))
                {
                    CampaignMission.MissionResult |= RESULT_FLAGS.AWARD_MEDAL | RESULT_FLAGS.MDL_SILVERSTAR;
                    Pilot.Medals[(int)LB_MEDAL.SILVER_STAR]++;
                    Pilot.Campaign.TotalScore += 15;
                }
                else if (MedalPts > 6000 && (PlayerOptionsClass.PlayerOptions.Realism > 0.5f))
                {
                    CampaignMission.MissionResult |= RESULT_FLAGS.AWARD_MEDAL | RESULT_FLAGS.MDL_DIST_FLY;
                    Pilot.Medals[(int)LB_MEDAL.DIST_FLY_CROSS]++;
                    Pilot.Campaign.TotalScore += 10;
                }
                else if (MedalPts > 4800)
                {
                    CampaignMission.MissionResult |= RESULT_FLAGS.AWARD_MEDAL | RESULT_FLAGS.MDL_AIR_MDL;
                    Pilot.Medals[(int)LB_MEDAL.AIR_MEDAL]++;
                    Pilot.Campaign.TotalScore += 5;
                }
            }


            if (MissStats.Killed != 0 || MissStats.KilledByHuman != 0 || MissStats.KilledBySelf != 0)
            {
                Pilot.Campaign.ConsecMissions = 0;
            }
            else
            {
                if (!PlayerOptionsClass.PlayerOptions.InvulnerableOn())
                    Pilot.Campaign.ConsecMissions++;
            }

            if (Pilot.Campaign.ConsecMissions >= 100)
            {
                CampaignMission.MissionResult |= RESULT_FLAGS.AWARD_MEDAL | RESULT_FLAGS.MDL_LONGEVITY;
                Pilot.Campaign.ConsecMissions = 0;
                Pilot.Medals[(int)LB_MEDAL.LONGEVITY]++;
            }
        }

        private float MissionComplexity(CAMP_MISS_STRUCT MissStats)
        {
            float Duration;
            float WeapExpended, ShotsAt, AircrftInPkg;

            //determine mission complexity
            if (MissStats.FlightHours > 1.5f)
                Duration = 1.5f;
            else
                Duration = MissStats.FlightHours;

            if (MissStats.WeaponsExpended > 6)
                WeapExpended = 6;
            else
                WeapExpended = (float)(MissStats.WeaponsExpended);

            if (MissStats.ShotsAtPlayer > 10)
                ShotsAt = 10;
            else
                ShotsAt = (float)(MissStats.ShotsAtPlayer);

            if (MissStats.AircraftInPackage > 8)
                AircrftInPkg = 8;
            else
                AircrftInPkg = (float)(MissStats.AircraftInPackage);


            return (Duration / 3.0F + WeapExpended / 6.0F + ShotsAt / 10.0F + AircrftInPkg / 16.0F + 3.0F);
        }

        private float CampaignDifficulty()
        {
#if TODO
			return ((13.0F - CampaignClass.TheCampaign.GroundRatio - CampaignClass.TheCampaign.AirRatio -
								CampaignClass.TheCampaign.AirDefenseRatio - CampaignClass.TheCampaign.NavalRatio / 4.0F) / 39.0F +
								(CampaignClass.TheCampaign.EnemyAirExp + CampaignClass.TheCampaign.EnemyADExp) / 12.0F) * 5.0F + 15.0F;
#endif
            throw new NotImplementedException();
        }

        public LB_PILOT Pilot;

        public LogBookData()
        {
            Initialize();
        }
        // public ~LogBookData();
        public void Initialize()
        {
            Pilot = new LB_PILOT();
            Pilot.Name = "Joe Pilot";
            //if (gStringMgr != null)
            //    Pilot.Name = gStringMgr.GetString (TXT_JOE_PILOT);


            Pilot.Callsign = "Viper";
            Pilot.OptionsFile = "Default";
            Pilot.Password = "";
            EncryptPwd();
            Pilot.Rank = LB_RANK.SEC_LT;
            Pilot.AceFactor = 1.0f;
            Pilot.FlightHours = 0.0F;

            Pilot.Picture = null;
            Pilot.PictureResource = LB_PILOT.NOFACE;
            Pilot.Patch = "";
            Pilot.PatchResource = LB_PILOT.NOPATCH;
            Pilot.Personal = "";
            Pilot.Squadron = "";
            Pilot.voice = 0;

            DateTime systime = DateTime.Now;
            if (F4Config.gLangIDNum != F4LANGUAGE.F4LANG_ENGLISH)
            {
                Pilot.Commissioned = systime.ToString("u");
            }
            else
            {
                Pilot.Commissioned = systime.ToString("d");
            }
            Pilot.CheckSum = 0;
#if TODO
            if (gCommsMgr != 0)
            {
                path = FalconDataDirectory + "config" + Path.DirectorySeparatorChar + Pilot.Callsign + ".plc";
                gCommsMgr.SetStatsFile(path);
            }
#endif
        }

        public void Cleanup()
        {
        }

        public bool Load()
        {
            Initialize();
            return false;
        }

        public bool LoadData(string PilotCallsign)
        {
            Debug.Assert(!string.IsNullOrEmpty(PilotCallsign));

            string path = F4File.F4FindFile(PilotCallsign, "lbk");

            if (!File.Exists(path))
            {
                log.WarnFormat("Failed to open {0} logbook file.", path);

                Initialize();
                return false;
            }

            FileStream fp = File.OpenRead(path);
            try
            {
                byte[] pilotBuf = new byte[LB_PILOTEncodingLE.Size];
                fp.Read(pilotBuf, 0, pilotBuf.Length);
                Encrypter.DecryptBuffer(0x58, pilotBuf, pilotBuf.Length);
                LB_PILOTEncodingLE.Decode(new MemoryStream(pilotBuf), ref Pilot);
                fp.Close();

                if (Pilot.CheckSum != 0)
                {
                    log.Warn("Failed checksum, somebody changed the data ...");
                    Initialize();
                    return (false);
                }
            }
            catch (Exception e)
            {
                log.WarnFormat("Failed to read {0}'s logbook.", PilotCallsign);
                log.Warn(e);
                fp.Close();

                Initialize();
                return false;
            }
#if TODO
			if (gCommsMgr) {
				sprintf (path, "%s\\config\\%s.plc", FalconDataDirectory, PilotName);
				gCommsMgr. SetStatsFile (path);
			}	
#endif
            if (this == LogBook)
            {
#if TODO
				FalconLocalSession. SetPlayerName (NameWRank ());
				FalconLocalSession. SetPlayerCallsign (Callsign ());
				FalconLocalSession. SetAceFactor (AceFactor ());
				FalconLocalSession. SetInitAceFactor (AceFactor ());
				FalconLocalSession. SetVoiceID (static_cast<uchar> (Voice ()));
                LogState |= LB_ENUM.LB_LOADED_ONCE;
#endif
                PlayerOptionsClass.PlayerOptions.LoadOptions();
                RulesClass.LoadAllRules(Callsign());

            }
            return true;
        }

        public bool LoadData(LB_PILOT NewPilot)
        {
            if (NewPilot != null)
            {
                Pilot = NewPilot;
                if (this == LogBook)
                {
#if TODO
					FalconLocalSession. SetPlayerName (NameWRank ());
					FalconLocalSession. SetPlayerCallsign (Callsign ());
					FalconLocalSession. SetAceFactor (AceFactor ());
					FalconLocalSession. SetInitAceFactor (AceFactor ());
					FalconLocalSession. SetVoiceID (static_cast<uchar> (Voice ()));
					PlayerOptions.LoadOptions ();
					LoadAllRules (Callsign ());
					LogState |= LB_ENUM.LB_LOADED_ONCE;
#endif
                    throw new NotImplementedException();
                }
                return true;
            }
            return false;

        }

        public int SaveData()
        {
#if TODO
			FileStream fp;
			string path;
			
			path = FalconDataDirectory + "config" + Path.DirectorySeparatorChar + Pilot.Callsign + ".lbk";
			fp = File.OpenWrite (path);
			if(fp != null)
			{
				MonoPrint(_T("Couldn't save logbook"));
				return false;
			}
			byte[] buff = EncryptBuffer (0x58, Pilot);
			
			fp.Write (buff);
			fp.Close ();
		
			// TODO Pilot is not modified..DecryptBuffer(0x58,(uchar*)&Pilot,sizeof(LB_PILOT));
		
			if (gCommsMgr) {
				path = FalconDataDirectory + "config" + Path.DirectorySeparatorChar + Pilot.Callsign + ".plc";
				gCommsMgr. SetStatsFile (path);
			}
		
#if _USE_REGISTRY_
			DWORD size;
			HKEY theKey;
			long retval;
		
			retval = RegOpenKeyEx(HKEY_LOCAL_MACHINE, FALCON_REGISTRY_KEY,
				0, KEY_ALL_ACCESS, &theKey);
			size = _NAME_LEN_;
			if(retval == ERROR_SUCCESS)
				retval = RegSetValueEx  (theKey, "PilotName", 0, REG_BINARY, (LPBYTE)Name(), size);	
			size = _CALLSIGN_LEN_;
			if(retval == ERROR_SUCCESS)
				retval = RegSetValueEx  (theKey, "PilotCallsign", 0, REG_BINARY, (LPBYTE)Callsign(), size);	
			RegCloseKey(theKey);
#endif
		
			if (this == LogBook) { // TODO should it be a equals or a == ??
				FalconLocalSession. SetPlayerName (NameWRank ());
				FalconLocalSession. SetPlayerCallsign (Callsign ());
				FalconLocalSession. SetAceFactor (AceFactor ());
				FalconLocalSession. SetInitAceFactor (LogBook.AceFactor ());
				FalconLocalSession. SetVoiceID (static_cast<uchar> (Voice ()));
			}
		
			return true;
#endif
            throw new NotImplementedException();
        }

        public void Clear()
        {
            string path;
            path = F4File.FalconDataDirectory + "config" + Path.DirectorySeparatorChar + Pilot.Callsign + ".rul";
            if (File.Exists(path))
                File.Delete(path);

            path = F4File.FalconDataDirectory + "config" + Path.DirectorySeparatorChar + Pilot.Callsign + ".pop";
            if (File.Exists(path))
                File.Delete(path);

            path = F4File.FalconDataDirectory + "config" + Path.DirectorySeparatorChar + Pilot.Callsign + ".lbk";
            if (File.Exists(path))
                File.Delete(path);

            path = F4File.FalconDataDirectory + "config" + Path.DirectorySeparatorChar + Pilot.Callsign + ".plc";
            if (File.Exists(path))
                File.Delete(path);

            Initialize();
            /*
            Pilot.Rank = SEC_LT;
            Pilot.AceFactor = 1.0f;
            Pilot.FlightHours = 0.0F;
            memset(&Pilot.Campaign,0,sizeof(CAMP_STATS));
            memset(&Pilot.Dogfight,0,sizeof(DF_STATS));
            memset(Pilot.Medals,0,sizeof(uchar)*NUM_MEDALS);
            Pilot.Picture[0] = 0;
            Pilot.Patch[0] = 0;
            Pilot.Personal[0] = 0;
            Pilot.voice = 0;
            _TCHAR buf[COMM_LEN];
            _tstrdate(buf);
            _tcscpy(Pilot.Commissioned,buf);
            */
        }

        public void Encrypt()
        {
#if NOTHING // This has been replaced by a routine with a checksum to make sure the user isn't modifying the data file
				int i;
				char *ptr;
			
				ptr=(char *)&Pilot;
			
				for(i=0;i<sizeof(LB_PILOT);i++)
				{
					*ptr ^= XorMask[i % strlen(XorMask)];
					*ptr ^= YorMask[i % strlen(YorMask)];
					ptr++;
				}
#endif
        }

        private const string PwdMask = "Who needs a password!";
        private const string PwdMask2 = "Repent, Falcon is coming!";

        public void UpdateDogfight(short matchWon, float Hours, short VsHuman, short Kills, short Killed, short HumanKills, short KilledByHuman)
        {
            CampaignMission.MissionResult = 0;

            UpdateFlightHours(Hours);

            if (VsHuman != 0)
            {
                if (matchWon >= 1)
                    Pilot.Dogfight.MatchesWonVHum++;
                else if (matchWon <= -1)
                    Pilot.Dogfight.MatchesLostVHum++;
            }

            if (matchWon >= 1)
                Pilot.Dogfight.MatchesWon++;
            else if (matchWon <= -1)
                Pilot.Dogfight.MatchesLost++;

            if (Kills > 0)
                Pilot.Dogfight.Kills += Kills;
            if (Killed > 0)
                Pilot.Dogfight.Killed += Killed;
            if (HumanKills > 0)
                Pilot.Dogfight.HumanKills += HumanKills;
            if (KilledByHuman > 0)
                Pilot.Dogfight.KilledByHuman += KilledByHuman;

            SaveData();
        }

        public void UpdateCampaign(CAMP_MISS_STRUCT MissStats)
        {
            CampaignMission.MissionResult = 0;

            UpdateFlightHours(MissStats.FlightHours);

            if (MissStats.Flags.HasFlag(CAMP_MISS_FLAGS.CRASH_UNDAMAGED) && !F4Config.g_bDisableCrashEjectCourtMartials)
            { // JB 010118
                Pilot.Campaign.TotalScore -= 25;
                CampaignMission.MissionResult |= RESULT_FLAGS.CM_CRASH | RESULT_FLAGS.COURT_MARTIAL;
            }

            if (MissStats.Flags.HasFlag(CAMP_MISS_FLAGS.EJECT_UNDAMAGED) && !F4Config.g_bDisableCrashEjectCourtMartials)
            { // JB 010118
                Pilot.Campaign.TotalScore -= 50;
                CampaignMission.MissionResult |= RESULT_FLAGS.CM_EJECT | RESULT_FLAGS.COURT_MARTIAL;
            }

            short FrKills = (short)(MissStats.FriendlyFireKills);

            while (FrKills != 0)
            {
                if (Pilot.Campaign.FriendliesKilled == 0)
                {
                    Pilot.Campaign.TotalScore -= 100;
                    CampaignMission.MissionResult |= RESULT_FLAGS.CM_FR_FIRE1 | RESULT_FLAGS.COURT_MARTIAL;
                }
                else if (Pilot.Campaign.FriendliesKilled == 1)
                {
                    Pilot.Campaign.TotalScore -= 200;
                    CampaignMission.MissionResult |= RESULT_FLAGS.CM_FR_FIRE2 | RESULT_FLAGS.COURT_MARTIAL;
                }
                else
                {
                    Pilot.Campaign.TotalScore -= 200;
                    if (MissStats.Flags.HasFlag(CAMP_MISS_FLAGS.FR_HUMAN_KILLED))
                    {
                        Pilot.Campaign.TotalScore = 0;
                        Pilot.Rank = LB_RANK.SEC_LT;
                        CampaignMission.MissionResult |= RESULT_FLAGS.CM_FR_FIRE3 | RESULT_FLAGS.COURT_MARTIAL;
                    }
                    else
                        CampaignMission.MissionResult |= RESULT_FLAGS.CM_FR_FIRE2 | RESULT_FLAGS.COURT_MARTIAL;
                }
                Pilot.Campaign.FriendliesKilled++;
                FrKills--;
            }

            //calculate new score using complexity, no mission pts if you get court martialed!
            if (!(CampaignMission.MissionResult.HasFlag(RESULT_FLAGS.COURT_MARTIAL)))
                Pilot.Campaign.TotalScore += (int)(MissStats.Score * MissionComplexity(MissStats) * CampaignDifficulty() *
                                                PlayerOptionsClass.PlayerOptions.Realism / 30.0F + MissStats.FlightHours);

            if (Pilot.Campaign.TotalScore < 0)
                Pilot.Campaign.TotalScore = 0;

            if (!(MissStats.Flags.HasFlag(CAMP_MISS_FLAGS.DONT_SCORE_MISSION)))
            {
                Pilot.Campaign.Missions++;

                Debug.Assert(MissStats.Score >= 0);
                Pilot.Campaign.TotalMissionScore += MissStats.Score;

                Pilot.Campaign.TotalScore -= MissStats.WingmenLost * 5;

                Debug.Assert(MissStats.Kills >= 0);
                if (MissStats.Kills > 0)
                    Pilot.Campaign.Kills += (short)MissStats.Kills;
                if (Pilot.Campaign.Kills < 0)
                    Pilot.Campaign.Kills = 0;

                Debug.Assert(MissStats.Killed >= 0);
                if (MissStats.Killed > 0)
                    Pilot.Campaign.Killed += (short)MissStats.Killed;
                if (Pilot.Campaign.Killed < 0)
                    Pilot.Campaign.Killed = 0;

                Debug.Assert(MissStats.HumanKills >= 0);
                if (MissStats.HumanKills > 0)
                    Pilot.Campaign.HumanKills += (short)MissStats.HumanKills;
                if (Pilot.Campaign.HumanKills < 0)
                    Pilot.Campaign.HumanKills = 0;

                Debug.Assert(MissStats.KilledByHuman >= 0);
                if (MissStats.KilledByHuman > 0)
                    Pilot.Campaign.KilledByHuman += (short)MissStats.KilledByHuman;
                if (Pilot.Campaign.KilledByHuman < 0)
                    Pilot.Campaign.KilledByHuman = 0;

                Debug.Assert(MissStats.KilledBySelf >= 0);
                if (MissStats.KilledBySelf > 0)
                    Pilot.Campaign.KilledBySelf += (short)MissStats.KilledBySelf;
                if (Pilot.Campaign.KilledBySelf < 0)
                    Pilot.Campaign.KilledBySelf = 0;

                Debug.Assert(MissStats.GroundUnitsKilled >= 0);
                if (MissStats.GroundUnitsKilled > 0)
                    Pilot.Campaign.AirToGround += (short)MissStats.GroundUnitsKilled;
                if (Pilot.Campaign.AirToGround < 0)
                    Pilot.Campaign.AirToGround = 0;

                Debug.Assert(MissStats.FeaturesDestroyed >= 0);
                if (MissStats.FeaturesDestroyed > 0)
                    Pilot.Campaign.Static += (short)MissStats.FeaturesDestroyed;
                if (Pilot.Campaign.Static < 0)
                    Pilot.Campaign.Static = 0;

                Debug.Assert(MissStats.NavalUnitsKilled >= 0);
                if (MissStats.NavalUnitsKilled > 0)
                    Pilot.Campaign.Naval += (short)MissStats.NavalUnitsKilled;
                if (Pilot.Campaign.Naval < 0)
                    Pilot.Campaign.Naval = 0;

                if (!(CampaignMission.MissionResult.HasFlag(RESULT_FLAGS.COURT_MARTIAL)))
                    AwardMedals(MissStats);
            }

            CalcRank();

            SaveData();
        }

        public void FinishCampaign(short WonLostTied)
        {
            Pilot.Campaign.TotalScore += 10;

            if (WonLostTied > 0)
            {
                Pilot.Campaign.GamesWon++;
                Pilot.Campaign.TotalScore += 10;
                Pilot.Medals[(int)LB_MEDAL.KOREA_CAMPAIGN]++;
                CampaignMission.MissionResult |= RESULT_FLAGS.AWARD_MEDAL | RESULT_FLAGS.MDL_KOR_CAMP;
            }
            else if (WonLostTied < 0)
            {
                Pilot.Campaign.GamesLost++;
            }
            else
                Pilot.Campaign.GamesTied++;

            SaveData();
        }

        public CAMP_STATS GetCampaign()
        {
            return Pilot.Campaign;
        }

        public DF_STATS GetDogfight()
        {
            return Pilot.Dogfight;
        }

        public LB_PILOT GetPilot()
        {
            return Pilot;
        }

        // This is used for remote pilots...so I can get them in the class used for drawing the UI
        public void SetPilot(LB_PILOT data)
        {
            if (data != null)
                Pilot = data; // TODO the original code was a clone copy
        }

        public byte GetMedal(LB_MEDAL MedalNo)
        {
            if (MedalNo < LB_MEDAL.NUM_MEDALS)
                return Pilot.Medals[(int)MedalNo];
            else
                return 0;
        }

        public void SetMedal(LB_MEDAL MedalNo, byte Medal)
        {
            if (MedalNo < LB_MEDAL.NUM_MEDALS)
                Pilot.Medals[(int)MedalNo] = Medal;
        }

        public void SetFlightHours(float Hours)
        {
            Pilot.FlightHours = Hours;
        }

        public void UpdateFlightHours(float Hours)
        {
            Pilot.FlightHours += Hours;
        }

        public string GetPicture()
        {
            return Pilot.Picture;
        }

        public long GetPictureResource()
        {
            return Pilot.PictureResource;
        }

        public void SetPicture(string filename)
        {
            Pilot.Picture = filename;
            Pilot.PictureResource = 0;
        }

        public void SetPicture(long imageID)
        {
            Pilot.PictureResource = imageID;
            Pilot.Picture = "";
        }

        public string GetPatch()
        {
            return Pilot.Patch;
        }

        public long GetPatchResource()
        {
            return Pilot.PatchResource;
        }

        public void SetPatch(string filename)
        {
            Pilot.Patch = filename;
            Pilot.PatchResource = 0;
        }

        public void SetPatch(long imageID)
        {
            Pilot.PatchResource = imageID;
            Pilot.Patch = "";
        }

        public string Name()
        {
            return Pilot.Name;
        }

        public void SetName(string Name)
        {
            Pilot.Name = Name;
        }

        public string Callsign()
        {
            return Pilot.Callsign;
        }

        public void SetCallsign(string Callsign)
        {
            Pilot.Callsign = Callsign;
        }

        public string Squadron()
        {
            return Pilot.Squadron;
        }

        public void SetSquadron(string Squadron)
        {
            Pilot.Squadron = Squadron;
        }

        public bool CheckPassword(string Pwd)
        {
            //if(Pilot.Password[0] == 0)
            //return true;

            EncryptPwd();
            if (Pwd.Equals(Pilot.Password))
            {
                EncryptPwd();
                return false;
            }
            else
            {
                EncryptPwd();
                return true;
            }
        }

        public bool SetPassword(string Password)
        {
            Pilot.Password = Password;
            EncryptPwd();
            return true;
        }

        public bool GetPassword(ref string Pwd)
        {
            if (string.IsNullOrEmpty(Pilot.Password))
                return false;

            EncryptPwd();
            Pwd = Pilot.Password;
            EncryptPwd();
            return true;
        }

        public string Personal()
        {
            return Pilot.Personal;
        }

        public void SetPersonal(string Personal)
        {
            Pilot.Personal = Personal;
        }

        public string OptionsFile()
        {
            return Pilot.OptionsFile;
        }

        public void SetOptionsFile(string OptionsFile)
        {
            Pilot.OptionsFile = OptionsFile;
        }

        public float AceFactor()
        {
            return Pilot.AceFactor;
        }

        public void SetAceFactor(float Factor)
        {
            Pilot.AceFactor = Factor;
            SaveData();
        }

        public LB_RANK Rank()
        {
            return Pilot.Rank;
        }

        public string Commissioned()
        {
            return Pilot.Commissioned;
        }

        public void SetCommissioned(string Date)
        {
            Pilot.Commissioned = Date;
        }

        public float FlightHours()
        {
            return Pilot.FlightHours;
        }

        public short TotalKills()
        {
            return (short)(Pilot.Campaign.Kills + Pilot.Dogfight.Kills);
        }

        public short TotalKilled()
        {
            return (short)(Pilot.Campaign.Killed + Pilot.Dogfight.Killed);
        }

        public void SetVoice(short newvoice)
        {
            Pilot.voice = newvoice;
        }

        public byte Voice()
        {
            return (byte)Pilot.voice;
        }



        public string NameWRank()
        {
            if (UI_Logbook.gStringMgr != null)
            {
#if TODO
                string rank = UI_Logbook.gStringMgr.GetString(UI_Logbook.gRanksTxt[Rank()]);
				nameWrank = rank + " " + Pilot.Name;
				return nameWrank;
#endif
                throw new NotImplementedException();
                }
			return Name ();
        }

        public static LogBookData LogBook = new LogBookData();
        public static LogBookData UI_logbk;
        private string nameWrank;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    }



    [Flags]
    public enum LB_ENUM
    {
        LB_INVALID_CALLSIGN = 0x01,
        LB_EDITABLE = 0x02,
        LB_OPPONENT = 0x04,
        LB_CHECKED = 0x08,
        LB_REFRESH_PILOT = 0x10,
        LB_LOADED_ONCE = 0x20,
    }

}
