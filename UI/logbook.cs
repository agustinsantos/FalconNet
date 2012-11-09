using System;
using System.IO;

namespace FalconNet.UI
{
	// TODO
	public struct CAMP_MISS_STRUCT
	{
	}

	//define value identifying medals for array index
	public enum LB_MEDAL
	{
		AIR_FORCE_CROSS,
		SILVER_STAR,
		DIST_FLY_CROSS,
		AIR_MEDAL,
		KOREA_CAMPAIGN,		
		LONGEVITY,
		NUM_MEDALS,
	};

	//Ranks
	public enum LB_RANK
	{
		SEC_LT,			
		LEIUTENANT,		
		CAPTAIN,			
		MAJOR,			
		LT_COL,			
		COLONEL,			
		BRIG_GEN,
		NUM_RANKS,
	};

	public enum LEN_ENUM
	{
		FILENAME_LEN = 32,
		PASSWORD_LEN	= 10,
		PERSONAL_TEXT_LEN = 120,
		COMM_LEN = 12,
		_NAME_LEN_ = 20,
		_CALLSIGN_LEN_ = 12,
	};

	public enum LB_ENUM
	{
		LB_INVALID_CALLSIGN	= 0x01,
		LB_EDITABLE			= 0x02,
		LB_OPPONENT			= 0x04,
		LB_CHECKED			= 0x08,
		LB_REFRESH_PILOT	= 0x10,
		LB_LOADED_ONCE		= 0x20,
	};

	public enum LOG_CONST
	{
		NOPATCH = 70050,
		NOFACE	= 60000,
		LOGBOOK_PICTURE_ID = 8649144,
		LOGBOOK_PICTURE_ID_2 = 8649145,
		LOGBOOK_SQUADRON_ID = 8649146,
		LOGBOOK_SQUADRON_ID_2 = 8649147,

		PATCHES_RESOURCE = 59998,
		PILOTS_RESOURCE = 59999,
	};

	public struct DF_STATS //DogfightStats
	{
		short	MatchesWon;
		short	MatchesLost;
		short	MatchesWonVHum;
		short	MatchesLostVHum;
		short	Kills;
		short	Killed;
		short	HumanKills;
		short	KilledByHuman;
	};

	public struct CAMP_STATS //CampaignStats
	{
		internal short	GamesWon;
		internal short	GamesLost;
		internal short	GamesTied;
		internal short	Missions;
		internal long	TotalScore;
		internal long	TotalMissionScore;
		internal short	ConsecMissions;
		internal short	Kills;
		internal short	Killed;
		internal short	HumanKills;
		internal short	KilledByHuman;
		internal short	KilledBySelf;
		internal short	AirToGround;
		internal short	Static;
		internal short	Naval;
		internal short	FriendliesKilled;
		internal short	MissSinceLastFriendlyKill;
	};

	public class LB_PILOT //Pilot
	{
		internal string		Name;
		internal string		Callsign;
		internal string		Password;
		internal string		Commissioned;
		internal string		OptionsFile;
		internal float		FlightHours;
		internal float		AceFactor;
		internal LB_RANK		Rank;
		internal DF_STATS	Dogfight;
		internal CAMP_STATS	Campaign;
		internal byte[]		Medals = new byte[(int)LB_MEDAL.NUM_MEDALS];
		internal long		PictureResource;
		internal string		Picture;
		internal long		PatchResource;
		internal string		Patch;
		internal string		Personal;
		internal string		Squadron;
		internal short		voice;							// index from 0 - 11 indicating which voice they want
		internal long		CheckSum; // If this value is ever NON zero after Decrypting, the Data has been modified
	};

	public class LogBookData
	{
		private void EncryptPwd ()
		{
			char[] ptr = Pilot.Password.ToCharArray();;
		
			for (int i=0; i<(int)LEN_ENUM.PASSWORD_LEN; i++) {
				ptr [i] ^= PwdMask [i % PwdMask.Length];
				ptr [i] ^= PwdMask2 [i % PwdMask2.Length];
			}
			
			Pilot.Password = ptr.ToString();
		}

		private void CalcRank ()
		{
			LB_RANK NewRank = LB_RANK.SEC_LT;
		
			if ((Pilot.Campaign.TotalScore > 3200) && Pilot.Campaign.GamesWon != 0) {
				NewRank = LB_RANK.COLONEL;
			} else if ((Pilot.Campaign.TotalScore > 1600) && (Pilot.Campaign.GamesWon != 0|| Pilot.Campaign.GamesTied != 0)) {
				NewRank = LB_RANK.LT_COL;
			} else if ((Pilot.Campaign.TotalScore > 800) && (Pilot.Campaign.GamesWon != 0 || Pilot.Campaign.GamesTied != 0 || Pilot.Campaign.GamesLost != 0)) {
				NewRank = LB_RANK.MAJOR;
			} else if (Pilot.Campaign.TotalScore > 300) {
				NewRank = LB_RANK.CAPTAIN;
			} else if (Pilot.Campaign.TotalScore > 150) {
				NewRank = LB_RANK.LEIUTENANT;
			} else {
				NewRank = LB_RANK.SEC_LT;
			}
		
			if (NewRank > Pilot.Rank) {
				MissionResult |= PROMOTION;
				Pilot.Rank = NewRank;
			}
		}

		private void AwardMedals (CAMP_MISS_STRUCT MissStats)
		{
			if (MissStats. Score >= 3) {
				int MedalPts = 0;
		
				if (MissStats. Flags & DESTROYED_PRIMARY)
					MedalPts += 2;
		
				if (MissStats. Flags & LANDED_AIRCRAFT)
					MedalPts++;
		
				if (!MissStats. WingmenLost)
					MedalPts++;
				
				MedalPts += MissStats. NavalUnitsKilled + MissStats. Kills + 
								min (10, MissStats. FeaturesDestroyed / 2) + min (10, MissStats. GroundUnitsKilled / 2);
		
				MedalPts = FloatToInt32 (PlayerOptions.Realism * MedalPts * CampaignDifficulty () * MissStats. Score * MissionComplexity (MissStats));
		
				if ((MedalPts > 9600) && (PlayerOptions.Realism > 0.9f) && MissStats. Score >= 4) {
					MissionResult |= AWARD_MEDAL | MDL_AFCROSS;
					Pilot.Medals [AIR_FORCE_CROSS]++;
					Pilot.Campaign.TotalScore += 20;
				} else if ((MedalPts > 7800) && (PlayerOptions.Realism > 0.7f)) {
					MissionResult |= AWARD_MEDAL | MDL_SILVERSTAR;
					Pilot.Medals [SILVER_STAR]++;
					Pilot.Campaign.TotalScore += 15;
				} else if (MedalPts > 6000 && (PlayerOptions.Realism > 0.5f)) {
					MissionResult |= AWARD_MEDAL | MDL_DIST_FLY;
					Pilot.Medals [DIST_FLY_CROSS]++;
					Pilot.Campaign.TotalScore += 10;
				} else if (MedalPts > 4800) {
					MissionResult |= AWARD_MEDAL | MDL_AIR_MDL;
					Pilot.Medals [AIR_MEDAL]++;
					Pilot.Campaign.TotalScore += 5;
				}
			}
		
		
			if (MissStats.Killed || MissStats. KilledByHuman || MissStats. KilledBySelf) {
				Pilot.Campaign.ConsecMissions = 0;
			} else {
				if (!PlayerOptions.InvulnerableOn ())
					Pilot.Campaign.ConsecMissions++;
			}
		
			if (Pilot.Campaign.ConsecMissions >= 100) {
				MissionResult |= AWARD_MEDAL | MDL_LONGEVITY;
				Pilot.Campaign.ConsecMissions = 0;
				Pilot.Medals [LONGEVITY]++;
			}
		}

		private float MissionComplexity (CAMP_MISS_STRUCT MissStats)
		{
			float Duration;
			float WeapExpended, ShotsAt, AircrftInPkg;
		
			//determine mission complexity
			if (MissStats. FlightHours > 1.5f)
				Duration = 1.5f;
			else
				Duration = MissStats. FlightHours;
		
			if (MissStats. WeaponsExpended > 6)
				WeapExpended = 6;
			else
				WeapExpended = static_cast<float> (MissStats. WeaponsExpended);
		
			if (MissStats. ShotsAtPlayer > 10)
				ShotsAt = 10;
			else
				ShotsAt = static_cast<float> (MissStats. ShotsAtPlayer);
		
			if (MissStats. AircraftInPackage > 8)
				AircrftInPkg = 8;
			else
				AircrftInPkg = static_cast<float> (MissStats. AircraftInPackage);
		
			
			return (Duration / 3.0F + WeapExpended / 6.0F + ShotsAt / 10.0F + AircrftInPkg / 16.0F + 3.0F);
		}

		private float CampaignDifficulty ()
		{
			return ((13.0F - TheCampaign.GroundRatio - TheCampaign.AirRatio -
								TheCampaign.AirDefenseRatio - TheCampaign.NavalRatio / 4.0F) / 39.0F +
								(TheCampaign.EnemyAirExp + TheCampaign.EnemyADExp) / 12.0F) * 5.0F + 15.0F;
		}

		public LB_PILOT	Pilot;
	
		public LogBookData ()
		{
			Initialize ();
		}
		// public ~LogBookData();
		public void Initialize ()
		{
			string path;
			if (gStringMgr)
				Pilot.Name = gStringMgr.GetString (TXT_JOE_PILOT);
			else
				Pilot.Name = "Joe Pilot";
			Pilot.Callsign = "Viper";
			Pilot.OptionsFile = "Default";
			Pilot.Password = "";
			EncryptPwd ();
			Pilot.Rank = LB_RANK.SEC_LT;
			Pilot.AceFactor = 1.0f;
			Pilot.FlightHours = 0.0F;
#if TODO // Already initialized by c#
			Pilot.Campaign,0,sizeof(CAMP_STATS));
			memset(&Pilot.Dogfight,0,sizeof(DF_STATS));
			memset(Pilot.Medals,0,sizeof(uchar)*NUM_MEDALS);
			Pilot.Picture[0] = 0;
			Pilot.PictureResource = NOFACE;
			Pilot.Patch[0] = 0;
			Pilot.PatchResource = NOPATCH;
			Pilot.Personal[0] = 0;
			Pilot.Squadron[0] = 0;
			Pilot.voice = 0;
#endif
			DateTime systime = DateTime.Now ();
			if (gLangIDNum != F4LANG_ENGLISH) {
				Pilot.Commissioned = systime.ToString ("d");
			} else {
				Pilot.Commissioned = systime.ToString ("d");
			}
			Pilot.CheckSum = 0;
			if (gCommsMgr) {
				path = FalconDataDirectory + "config" + Path.DirectorySeparatorChar + Pilot.Callsign + ".plc";
				gCommsMgr.SetStatsFile (path);
			}
		}

		public void Cleanup ()
		{
		}

		public bool Load ()
		{	
			Initialize ();
			return false;
		}

		public int LoadData (string  PilotName)
		{
			DWORD size;
			FileStream fp;
			size_t success = 0;
			string path;
		
			ShiAssert (callsign);
		
			path = FalconDataDirectory + "config" + Path.DirectorySeparatorChar + Pilot.Callsign + ".lbk";
			
			fp = File.OpenRead (path);
#if TODO // Handle error....
			if(!fp.)
			{
				//TODO MonoPrint(_T("Couldn't open %s's logbook.\n"),callsign);
				Initialize();
				return false;
			}

			fseek(fp,0,SEEK_END);
			size = ftell(fp);
			fseek(fp,0,SEEK_SET);
		
			if(size != sizeof(LB_PILOT))
			{
				//TODO MonoPrint(_T("%s's logbook is old file format.\n"),callsign);
				fp.Close();
				Initialize();
				return false;
			}
#endif		
			success = fread (&Pilot, sizeof(LB_PILOT), 1, fp);
			fclose (fp);
			if (success != 1) {
				MonoPrint (_T ("Failed to read %s's logbook.\n"), callsign);
				Initialize ();
				return false;
			}
		
			DecryptBuffer (0x58, (uchar*)&Pilot, sizeof(LB_PILOT));
		
		
			if (Pilot.CheckSum) { // Somebody changed the data... init
				MonoPrint ("Failed checksum");
				Initialize ();
				return(false);
			}
			if (gCommsMgr) {
				sprintf (path, "%s\\config\\%s.plc", FalconDataDirectory, callsign);
				gCommsMgr. SetStatsFile (path);
			}	
		
			if (this == LogBook) { // TODO its must be a equals?
				FalconLocalSession. SetPlayerName (NameWRank ());
				FalconLocalSession. SetPlayerCallsign (Callsign ());
				FalconLocalSession. SetAceFactor (AceFactor ());
				FalconLocalSession. SetInitAceFactor (AceFactor ());
				FalconLocalSession. SetVoiceID (static_cast<uchar> (Voice ()));
				PlayerOptions.LoadOptions ();
				LoadAllRules (Callsign ());
				LogState |= LB_LOADED_ONCE;
			}
		
			return true;
		}

		public int LoadData (LB_PILOT NewPilot)
		{
			if (NewPilot != null) {
				Pilot = NewPilot;
				if (this == LogBook) {
					FalconLocalSession. SetPlayerName (NameWRank ());
					FalconLocalSession. SetPlayerCallsign (Callsign ());
					FalconLocalSession. SetAceFactor (AceFactor ());
					FalconLocalSession. SetInitAceFactor (AceFactor ());
					FalconLocalSession. SetVoiceID (static_cast<uchar> (Voice ()));
					PlayerOptions.LoadOptions ();
					LoadAllRules (Callsign ());
					LogState |= LB_LOADED_ONCE;
				}
				return true;
			}
			return false;
		}

		public int SaveData ()
		{
			FileStream fp;
			string path;
			
			path = FalconDataDirectory + "config" + Path.DirectorySeparatorChar + Pilot.Callsign + ".lbk";
			fp = File.OpenWrite (path);
#if TODO //Handle errors...
			if(fp != null)
			{
				MonoPrint(_T("Couldn't save logbook"));
				return FALSE;
			}
#endif
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
		}

		public void Clear ()
		{
			string path;
			path = FalconDataDirectory + "config" + Path.DirectorySeparatorChar + Pilot.Callsign + ".rul";
			File.Delete (path);
			
			path = FalconDataDirectory + "config" + Path.DirectorySeparatorChar + Pilot.Callsign + ".pop";
			File.Delete (path);
			
			path = FalconDataDirectory + "config" + Path.DirectorySeparatorChar + Pilot.Callsign + ".lbk";
			File.Delete (path);
			
			path = FalconDataDirectory + "config" + Path.DirectorySeparatorChar + Pilot.Callsign + ".plc";
			File.Delete (path);
			Initialize ();
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
				
		public void Encrypt ()
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

		public void UpdateDogfight (short MatchWonLost, float Hours, short VsHuman, short Kills, short Killed, short HumanKills, short KilledByHuman)
		{
			MissionResult = 0;
		
			UpdateFlightHours (Hours);
		
			if (VsHuman != 0) {
				if (MatchWon >= 1)
					Pilot.Dogfight.MatchesWonVHum++;
				else if (MatchWon <= -1)
					Pilot.Dogfight.MatchesLostVHum++;
			}
			
			if (MatchWon >= 1)
				Pilot.Dogfight.MatchesWon ++;
			else if (MatchWon <= -1)
				Pilot.Dogfight.MatchesLost++;
		
			if (Kills > 0)
				Pilot.Dogfight.Kills += Kills;
			if (Killed > 0)
				Pilot.Dogfight.Killed += Killed;
			if (HumanKills > 0)
				Pilot.Dogfight.HumanKills += HumanKills;
			if (KilledByHuman > 0)
				Pilot.Dogfight.KilledByHuman += KilledByHuman;
			
			SaveData ();
		}
		
		public void UpdateCampaign (CAMP_MISS_STRUCT MissStats)
		{
			MissionResult = 0;
		
			UpdateFlightHours (MissStats. FlightHours);
		
			if (MissStats. Flags & CRASH_UNDAMAGED && !g_bDisableCrashEjectCourtMartials) { // JB 010118
				Pilot.Campaign.TotalScore -= 25;
				MissionResult |= CM_CRASH | COURT_MARTIAL;
			}
		
			if (MissStats. Flags & EJECT_UNDAMAGED && !g_bDisableCrashEjectCourtMartials) { // JB 010118
				Pilot.Campaign.TotalScore -= 50;
				MissionResult |= CM_EJECT | COURT_MARTIAL;
			}
		
			short FrKills = static_cast<short> (MissStats. FriendlyFireKills);
			
			while (FrKills) {
				if (Pilot.Campaign.FriendliesKilled == 0) {
					Pilot.Campaign.TotalScore -= 100;
					MissionResult |= CM_FR_FIRE1 | COURT_MARTIAL;
				} else if (Pilot.Campaign.FriendliesKilled == 1) {
					Pilot.Campaign.TotalScore -= 200;
					MissionResult |= CM_FR_FIRE2 | COURT_MARTIAL;
				} else {
					Pilot.Campaign.TotalScore -= 200;
					if (MissStats. Flags & FR_HUMAN_KILLED) {
						Pilot.Campaign.TotalScore = 0;
						Pilot.Rank = SEC_LT;
						MissionResult |= CM_FR_FIRE3 | COURT_MARTIAL;
					} else
						MissionResult |= CM_FR_FIRE2 | COURT_MARTIAL;
				}
				Pilot.Campaign.FriendliesKilled++;
				FrKills--;
			}
			
			//calculate new score using complexity, no mission pts if you get court martialed!
			if (!(MissionResult & COURT_MARTIAL))
				Pilot.Campaign.TotalScore += FloatToInt32 (MissStats. Score * MissionComplexity (MissStats) * CampaignDifficulty () *
												PlayerOptions.Realism / 30.0F + MissStats. FlightHours);
		
			if (Pilot.Campaign.TotalScore < 0)
				Pilot.Campaign.TotalScore = 0;
		
			if (!(MissStats. Flags & DONT_SCORE_MISSION)) {
				Pilot.Campaign.Missions++;
		
				ShiAssert (MissStats. Score >= 0);
				Pilot.Campaign.TotalMissionScore += MissStats. Score;
		
				Pilot.Campaign.TotalScore -= MissStats. WingmenLost * 5;
		
				ShiAssert (MissStats. Kills >= 0);
				if (MissStats. Kills > 0)
					Pilot.Campaign.Kills += MissStats. Kills;
				if (Pilot.Campaign.Kills < 0)
					Pilot.Campaign.Kills = 0;
		
				ShiAssert (MissStats. Killed >= 0);
				if (MissStats. Killed > 0)
					Pilot.Campaign.Killed += MissStats. Killed;
				if (Pilot.Campaign.Killed < 0)
					Pilot.Campaign.Killed = 0;
		
				ShiAssert (MissStats. HumanKills >= 0);
				if (MissStats. HumanKills > 0)
					Pilot.Campaign.HumanKills += MissStats. HumanKills;
				if (Pilot.Campaign.HumanKills < 0)
					Pilot.Campaign.HumanKills = 0;
		
				ShiAssert (MissStats. KilledByHuman >= 0);
				if (MissStats. KilledByHuman > 0)
					Pilot.Campaign.KilledByHuman += MissStats. KilledByHuman;
				if (Pilot.Campaign.KilledByHuman < 0)
					Pilot.Campaign.KilledByHuman = 0;
		
				ShiAssert (MissStats. KilledBySelf >= 0);
				if (MissStats. KilledBySelf > 0)
					Pilot.Campaign.KilledBySelf += MissStats. KilledBySelf;
				if (Pilot.Campaign.KilledBySelf < 0)
					Pilot.Campaign.KilledBySelf = 0;
		
				ShiAssert (MissStats. GroundUnitsKilled >= 0);
				if (MissStats. GroundUnitsKilled > 0)
					Pilot.Campaign.AirToGround += MissStats. GroundUnitsKilled;
				if (Pilot.Campaign.AirToGround < 0)
					Pilot.Campaign.AirToGround = 0;
		
				ShiAssert (MissStats. FeaturesDestroyed >= 0);
				if (MissStats. FeaturesDestroyed > 0)
					Pilot.Campaign.Static += MissStats. FeaturesDestroyed;
				if (Pilot.Campaign.Static < 0)
					Pilot.Campaign.Static = 0;
		
				ShiAssert (MissStats. NavalUnitsKilled >= 0);
				if (MissStats. NavalUnitsKilled > 0)
					Pilot.Campaign.Naval += MissStats. NavalUnitsKilled;
				if (Pilot.Campaign.Naval < 0)
					Pilot.Campaign.Naval = 0;
		
				if (!(MissionResult & COURT_MARTIAL))
					AwardMedals (MissStats);
			}
		
			CalcRank ();
		
			SaveData ();
		}

		public void FinishCampaign (short WonLostTied)
		{
			Pilot.Campaign.TotalScore += 10;
		
			if (WonLostTied > 0) {
				Pilot.Campaign.GamesWon++;
				Pilot.Campaign.TotalScore += 10;
				Pilot.Medals [KOREA_CAMPAIGN]++;
				MissionResult |= AWARD_MEDAL | MDL_KOR_CAMP;
			} else if (WonLostTied < 0) {
				Pilot.Campaign.GamesLost++;
			} else
				Pilot.Campaign.GamesTied++;
		
			SaveData ();
		}
	
		public CAMP_STATS GetCampaign ()
		{
			return Pilot.Campaign;
		}

		public DF_STATS GetDogfight ()
		{
			return Pilot.Dogfight;
		}

		public LB_PILOT GetPilot ()
		{
			return Pilot;
		}
		// This is used for remote pilots...so I can get them in the class used for drawing the UI
		public void	SetPilot (LB_PILOT data)
		{
			if (data)
				memcpy (&Pilot, data, sizeof(Pilot));
		}

		public byte	GetMedal (LB_MEDAL MedalNo)
		{
			if (MedalNo < NUM_MEDALS)
				return Pilot.Medals [MedalNo];
			else
				return 0;
		}

		public void	SetMedal (LB_MEDAL MedalNo, byte Medal)
		{
			if (MedalNo < NUM_MEDALS)
				Pilot.Medals [MedalNo] = Medal;
		}

		public void	SetFlightHours (float Hours)
		{
			Pilot.FlightHours = Hours;
		}

		public void	UpdateFlightHours (float Hours)
		{
			Pilot.FlightHours += Hours;
		}

		public string GetPicture ()
		{
			return Pilot.Picture;
		}

		public long	GetPictureResource ()
		{
			return Pilot.PictureResource;
		}

		public void	SetPicture (string  filename)
		{
			if (_tcslen (filename) <= FILENAME_LEN)
				_tcscpy (Pilot.Picture, filename);
			Pilot.PictureResource = 0;
		}

		public void	SetPicture (long imageID)
		{
			Pilot.PictureResource = imageID;
			_tcscpy (Pilot.Picture, "");
		}

		public string GetPatch ()
		{
			return Pilot.Patch;
		}

		public long	GetPatchResource ()
		{
			return Pilot.PatchResource;
		}

		public void	SetPatch (string filename)
		{
			if (filename.Length <= LEN_ENUM.FILENAME_LEN)
				Pilot.Patch = filename;
			Pilot.PatchResource = 0;
		}

		public void	SetPatch (long imageID)
		{
			Pilot.PatchResource = imageID;
			Pilot.Patch = "";
		}

		public string  Name ()
		{
			return Pilot.Name;
		}

		public void	SetName (string Name)
		{
			if (Name.Length <= LEN_ENUM._NAME_LEN_)
				Pilot.Name = Name;
		}
	
		public string Callsign ()
		{
			return Pilot.Callsign;
		}

		public void	SetCallsign (string Callsign)
		{
			if (Callsign.Length <= LEN_ENUM._CALLSIGN_LEN_)
				Pilot.Callsign = Callsign;
		}
	
		public string Squadron ()
		{
			return Pilot.Squadron;
		}

		public void	SetSquadron (string Squadron)
		{
			if (Squadron.Length <= LEN_ENUM._NAME_LEN_)
				Pilot.Squadron = Squadron;
		}
	
		public int CheckPassword (string Pwd)
		{
			//if(Pilot.Password[0] == 0)
			//return TRUE;
		
			EncryptPwd ();
			if (Pwd.Equals (Pilot.Password)) {
				EncryptPwd ();
				return false;
			} else {
				EncryptPwd ();
				return true;
			}
		}

		public bool SetPassword (string Password)
		{
			if (Password.Length <= LEN_ENUM.PASSWORD_LEN) {
				Pilot.Password = Password;
				EncryptPwd ();
				return true;
			}
		
			return false;
		}

		public bool GetPassword (ref string Pwd)
		{
			if (string.IsNullOrEmpty (Pilot.Password))
				return false;
		
			EncryptPwd ();
			Pwd = Pilot.Password;
			EncryptPwd ();
			return true;
		}
	
		public string Personal ()
		{
			return Pilot.Personal;
		}

		public void	SetPersonal (string  Personal)
		{
			if (Personal.Length <= LEN_ENUM.PERSONAL_TEXT_LEN)
				Pilot.Personal = Personal;
		}
	
		public string OptionsFile ()
		{
			return Pilot.OptionsFile;
		}

		public void	SetOptionsFile (string OptionsFile)
		{
			if (OptionsFile.Length <= LEN_ENUM._CALLSIGN_LEN_)
				Pilot.OptionsFile = OptionsFile;
		}
	
		public float AceFactor ()
		{
			return Pilot.AceFactor;
		}

		public void	SetAceFactor (float Factor)
		{
			Pilot.AceFactor = Factor;
			SaveData ();
		}
	
		public LB_RANK Rank ()
		{
			return Pilot.Rank;
		}

		public string Commissioned ()
		{
			return Pilot.Commissioned;
		}

		public void	SetCommissioned (string Date)
		{
			if (Date.Length <= LEN_ENUM.COMM_LEN)
				Pilot.Commissioned = Date;
		}

		public float FlightHours ()
		{
			return Pilot.FlightHours;
		}

		public short TotalKills ()
		{
			return (short)(Pilot.Campaign.Kills + Pilot.Dogfight.Kills);
		}

		public short TotalKilled ()
		{
			return (short)(Pilot.Campaign.Killed + Pilot.Dogfight.Killed);
		}

		public void SetVoice (short newvoice)
		{
			Pilot.voice = newvoice;
		}

		public byte Voice ()
		{
			return (byte)Pilot.voice;
		}

		public  static LogBookData LogBook;
		public  static LogBookData UI_logbk;
		private string nameWrank;

		public string NameWRank ()
		{
			if (gStringMgr) {
				string rank = gStringMgr. GetString (gRanksTxt [Rank ()]);
				nameWrank = rank + " " + Pilot.Name;
				return nameWrank;
			}
			return Name ();
		}
	};


}
