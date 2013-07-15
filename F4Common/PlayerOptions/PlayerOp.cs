using System;
using FalconNet.Common;

namespace FalconNet.F4Common
{
    // =====================
    // PlayerOptions Class
    // =====================
    public class PlayerOptionsClass
    {
        public PO_DISP_FLAGS DispFlags;							// Display Options
        public int DispTextureLevel;					//0-4 (to what level do we display textures)
        public float DispTerrainDist;					//sets max range at which textures are displayed
        public int DispMaxTerrainLevel;				//should 0-2 can be up to 4

        public PO_OBJ_FLAGS ObjFlags;						// Object Display Options
        public float ObjDetailLevel;					// (.5 - 2) (modifies LOD switching distance)
        public float ObjMagnification;				// 1-5
        public int ObjDeaggLevel;					// 0-100 (Percentage of vehicles deaggregated per unit)
        public int BldDeaggLevel;					// 0-5 (determines which buildings get deaggregated)
        public int ACMIFileSize;					// 1-? MB's (determines the largest size a single acmi tape will be)
        public float SfxLevel;						// 1.0-5.0
        public float PlayerBubble;					// 0.5 - 2 multiplier for player bubble size

        public PO_SIM_FLAGS SimFlags;			// Sim flags
        public FlightModelType SimFlightModel;		// FlightModelType
        public WeaponEffectType SimWeaponEffect;	// WeaponEffectType
        public AvionicsType SimAvionicsType;	// Avionics Difficulty
        public AutopilotModeType SimAutopilotType;	// AutopilotModeType
        public RefuelModeType SimAirRefuelingMode;// RefuelModeType
        public PadlockModeType SimPadlockMode;		// PadlockModeType
        public VisualCueType SimVisualCueMode;	// VisualCueType

        public PO_GEN_FLAGS GeneralFlags;						// General stuff

        public int CampGroundRatio;					// Default force ratio values
        public int CampAirRatio;
        public int CampAirDefenseRatio;
        public int CampNavalRatio;
        public int CampEnemyAirExperience;			// 0-4	Green - Ace
        public int CampEnemyGroundExperience;		// 0-4	Green - Ace
        public int CampEnemyStockpile;				// 0-100 % of max
        public int CampFriendlyStockpile;			// 0-100 % of max

        public int[] GroupVol = new int[SoundGroups.NUM_SOUND_GROUPS];		// Values are 0 to -3600 in dBs

        public float Realism;							// stores last realism value saved less the value
        // from UnlimitedAmmo (this is used to modify scores in
        // Instant Action.)
        public string keyfile; //[PL_FNAME_LEN];			// name of keystrokes file to use
        public Guid joystick;						// unique identifier for which joystick to use

        public SimStartFlags SimStart;
        public const int RAMP_MINUTES = 15;  // how long before take off MI increased from 8
        // OW - Start of new stuff
        public bool bFast2DCockpit;
        public bool bHQFiltering;
        public bool bDXMultiThreaded;
        public bool bAllowProcessorExtensions;
        public bool bFilteredObjects;
        public bool bFastGMRadar;

        public enum TexMode
        {
            TEX_MODE_LOW,
            TEX_MODE_MED,
            TEX_MODE_HIGH,
            TEX_MODE_COMPRESS,
        };
        public TexMode m_eTexMode;
        // OW - End of new stuff

        // M.N.
        public byte skycol;	// ID of chosen skyfix (256 should be enough)
        public bool PlayerRadioVoice; // Turn on/off all player radio voices
        public bool UIComms;	// Turn on/off random UI radio chatter

        // Important stuff
        public PlayerOptionsClass()
        {
            Initialize();
        }

        public void Initialize()
        {
            // JB 011124 Start
            /*
            DispFlags = DISP_HAZING|DISP_GOURAUD|DISP_ALPHA_BLENDING;		// Display Options
            DispTextureLevel = 4;			//0-4
            DispTerrainDist = 64.0f;		// sets ranges at which texture sets are switched
            DispMaxTerrainLevel = 0;		//should be 0-2 can be up to 4
		
            ObjFlags = DISP_OBJ_TEXTURES;	// Object Display Options
            SfxLevel = 4.0f;				// 0.0 to 5.0f
            ObjDetailLevel = 1;				// (.5 - 2) (modifies LOD switching distance)
            ObjMagnification = 1;			// 1-9
        // 2001-11-09 M.N. Changed from 60 to 100 % = Slider setting 6 (Realism patch)
            ObjDeaggLevel = 100;			// 0-100 (Percentage of vehicles deaggregated per unit)
            BldDeaggLevel = 5;				// 0-5 (determines which buildings get deaggregated)
            PlayerBubble = 1.0F;			// .5 - 2 (ratio by which to multiply player bubble size)
		
            ACMIFileSize = 5;
		
            SimFlags =  SIM_NO_BLACKOUT | SIM_UNLIMITED_CHAFF | SIM_NAMETAGS | SIM_UNLIMITED_FUEL;		// Sim flags
            SimFlightModel = FMSimplified;			// Flight model type
            SimWeaponEffect = WEExaggerated;
            SimAvionicsType = ATEasy;
            SimAutopilotType = APEnhanced;
            GeneralFlags = GEN_RULES_FLAGS;			// General stuff
        */
            DispFlags = PO_DISP_FLAGS.DISP_HAZING | PO_DISP_FLAGS.DISP_GOURAUD | PO_DISP_FLAGS.DISP_ALPHA_BLENDING | PO_DISP_FLAGS.DISP_BILINEAR;		// Display Options
            DispTextureLevel = 4;			//0-4
            DispTerrainDist = 80.0f;		// sets ranges at which texture sets are switched
            DispMaxTerrainLevel = 0;		//should be 0-2 can be up to 4

            ObjFlags = PO_OBJ_FLAGS.DISP_OBJ_TEXTURES;	// Object Display Options
            SfxLevel = 5.0f;				// 0.0 to 5.0f
            ObjDetailLevel = 2;				// (.5 - 2) (modifies LOD switching distance)
            ObjMagnification = 1;			// 1-9
            // 2001-11-09 M.N. Changed from 60 to 100 % = Slider setting 6 (Realism patch)
            ObjDeaggLevel = 100;			// 0-100 (Percentage of vehicles deaggregated per unit)
            BldDeaggLevel = 5;				// 0-5 (determines which buildings get deaggregated)
            PlayerBubble = 1.0F;			// .5 - 2 (ratio by which to multiply player bubble size)

            ACMIFileSize = 5;

            SimFlags = 0;		// Sim flags
            SimFlightModel = FlightModelType.FMAccurate;			// Flight model type
            SimWeaponEffect = WeaponEffectType.WEAccurate;
            SimAvionicsType = AvionicsType.ATRealisticAV;
            SimAutopilotType = AutopilotModeType.APNormal;
            SimPadlockMode = PadlockModeType.PDRealistic;

            SimVisualCueMode = VisualCueType.VCReflection;
            GeneralFlags = PO_GEN_FLAGS.GEN_RULES_FLAGS; // TODO - PO_GEN_FLAGS.GEN_NO_WEATHER;			// General stuff
            // JB 011124 End

            SimStart = SimStartFlags.START_RUNWAY;


            Realism = 0.25f;				//value from 0 to 1

            for (int i = 0; i < SoundGroups.NUM_SOUND_GROUPS; i++)
            {
                GroupVol[i] = 0;
            }

            CampGroundRatio = 2;			// Default force ratio values
            CampAirRatio = 2;
            CampAirDefenseRatio = 2;
            CampNavalRatio = 2;

            CampEnemyAirExperience = 0;		// 0-4, Green - Ace
            CampEnemyGroundExperience = 0;	// 0-4, Green - Ace
            CampEnemyStockpile = 100;		// 0-100 % of max
            CampFriendlyStockpile = 100;	// 0-100 % of max

            // OW - Start of new stuff
            bFast2DCockpit = true;
            bHQFiltering = false;
            bDXMultiThreaded = false;
            bAllowProcessorExtensions = false;
            m_eTexMode = TexMode.TEX_MODE_LOW;
            bFilteredObjects = false;
            bFastGMRadar = false;
            // OW - End of new stuff

            // M.N. skyfix
            skycol = 1;
            PlayerRadioVoice = true;
            UIComms = true;

            keyfile = "keystrokes";
            joystick = new Guid("");
            ApplyOptions();
        }

        //filename should be callsign of player
        public int LoadOptions()
        {
#if TODO
			return LoadOptions (LogBookData.LogBook.OptionsFile ());
#endif
            throw new NotImplementedException();
        }

        public int LoadOptions(string filename)
        {
#if TODO
			size_t		success = 0;
			_TCHAR		path[_MAX_PATH];
			long		size;
			FILE *fp;
		
			_stprintf(path,_T("%s\\config\\%s.pop"),FalconDataDirectory,filename);
			
			fp = _tfopen(path,_T("rb"));
			if(!fp)
			{
				MonoPrint(_T("Couldn't open %s's player options\n"),filename);
				_stprintf(path,_T("%s\\Config\\default.pop"),FalconDataDirectory);
				fp = _tfopen(path,"rb");
				if(!fp)
				{
					MonoPrint(_T("Couldn't open default player options\n"),filename);
					Initialize();
					return false;
				}
			}
			
			fseek(fp,0,SEEK_END);
			size = ftell(fp);
			fseek(fp,0,SEEK_SET);
		
		// OW - dont break compatibility with 1.03 - 1.08 options
		
			success = fread(this, 1, size, fp);
			fclose(fp);
			if(success != size)
			{
				MonoPrint(_T("Failed to read %s's player options\n"),filename);
				Initialize();
				return false;
			}
		
		// M.N. If older version, set some new stuff to default values. Next save will update the saved file
			if(size != sizeof(class PlayerOptionsClass))
			{
				PlayerOptions.skycol = 1;
				PlayerOptions.PlayerRadioVoice = true;
			}
		
		/*	if(size != sizeof(class PlayerOptionsClass))
			{
				MonoPrint(_T("%s's player options are in old file format\n"),filename);
				return false;
			}
		
		
			success = fread(this, sizeof(class PlayerOptionsClass), 1, fp);
			fclose(fp);
			if(success != 1)
			{
				MonoPrint(_T("Failed to read %s's player options\n"),filename);
				Initialize();
				return false;
			}
		*/
		
			DisplayOptions.LoadOptions();
			FalconLocalSession.SetBubbleRatio(BubbleRatio());
			ApplyOptions();
			// M.N.
			if (SimAvionicsType == ATRealisticAV)
				g_bRealisticAvionics = true;
			else g_bRealisticAvionics = false;
			return true;
#endif
            throw new NotImplementedException();
        }

        public int SaveOptions()
        {
#if TODO
			return SaveOptions (LogBookData.LogBook.Callsign ());
#endif
            throw new NotImplementedException();
        }

        public int SaveOptions(string filename)
        {
#if TODO
			FILE		*fp;
			_TCHAR		path[_MAX_PATH];
			size_t		success = 0;
			
			_stprintf(path,_T("%s\\config\\%s.pop"),FalconDataDirectory,filename);
				
			if((fp = _tfopen(path,"wb")) == null)
			{
				MonoPrint(_T("Couldn't save player options"));
				return false;
			}
		
			if(gCurController >= SIM_JOYSTICK1 && gpDIDevice[gCurController])
			{
				DIDEVICEINSTANCE devinst;
				devinst.dwSize = sizeof(DIDEVICEINSTANCE);
				gpDIDevice[gCurController].GetDeviceInfo(&devinst);
				SetJoystick(devinst.guidInstance);	
			}
		
			success = fwrite(this, sizeof(class PlayerOptionsClass), 1, fp);
			fclose(fp);
			if(success == 1)
			{
				LogBook.SetOptionsFile(filename);
				LogBook.SaveData();
			}
			DisplayOptions.SaveOptions();
			// M.N.
			if (SimAvionicsType == ATRealisticAV)
				g_bRealisticAvionics = true;
			else g_bRealisticAvionics = false;
			return true;
#endif
            throw new NotImplementedException();
        }

        public void ApplyOptions()
        {
#if TODO
			if(VM)
			{
				F4SetStreamVolume(VM.VoiceHandle(0),GroupVol[COM1_SOUND_GROUP]);
				F4SetStreamVolume(VM.VoiceHandle(1),GroupVol[COM2_SOUND_GROUP]);
			}
			if(gSoundMgr)
				gSoundMgr.SetVolume(GroupVol[UI_SOUND_GROUP]);
			if(gMusic)
				gMusic.SetVolume(GroupVol[MUSIC_SOUND_GROUP]);
		
			// JPO - just remove this stuff to stop black triangles problem.
			/*if(bAllowProcessorExtensions)
			{
				// AMD stuff
				if(g_bHas3DNow)
				{
					SetMatrixCPUMode(1);
				}
			}
		
			else */SetMatrixCPUMode(0);
#endif
        }

        // returns true if in FULL compliance w/rules
        public bool InCompliance(RulesStruct rules)
        {
            if (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_RULES_FLAGS & ~rules.SimFlags)) // TODO Review this logic
                return false;

            if (GeneralFlags.IsFlagSet(PO_GEN_FLAGS.GEN_RULES_FLAGS & ~rules.GeneralFlags)) // TODO Review this logic
                return false;

            if (ObjectMagnification() > rules.ObjMagnification)
                return false;

            if (GetFlightModelType() < rules.SimFlightModel)
                return false;

            if (GetWeaponEffectiveness() < rules.SimWeaponEffect)
                return false;

            if (GetAvionicsType() < rules.SimAvionicsType)
                return false;

            if (GetAutopilotMode() < rules.SimAutopilotType)
                return false;

            if (GetRefuelingMode() > rules.SimAirRefuelingMode)
                return false;

            if (GetPadlockMode() < rules.SimPadlockMode)
                return false;

            return true;
        }

        // returns true if in FULL compliance w/rules
        public void ComplyWRules(RulesStruct rules)	// forces all settings not in compliance to minimum settings
        {
            SimFlags &= ~(PO_SIM_FLAGS.SIM_RULES_FLAGS & ~rules.SimFlags);

            GeneralFlags &= ~(PO_GEN_FLAGS.GEN_RULES_FLAGS & ~rules.GeneralFlags);

            if (ObjectMagnification() > rules.ObjMagnification)
                ObjMagnification = rules.ObjMagnification;

            if (GetFlightModelType() < rules.SimFlightModel)
                SimFlightModel = rules.SimFlightModel;

            if (GetWeaponEffectiveness() < rules.SimWeaponEffect)
                SimWeaponEffect = rules.SimWeaponEffect;

            if (GetAvionicsType() < rules.SimAvionicsType)
                SimAvionicsType = rules.SimAvionicsType;

            if (GetAutopilotMode() < rules.SimAutopilotType)
                SimAutopilotType = rules.SimAutopilotType;

            if (GetRefuelingMode() > rules.SimAirRefuelingMode)
                SimAirRefuelingMode = rules.SimAirRefuelingMode;

            if (GetPadlockMode() < rules.SimPadlockMode)
                SimPadlockMode = rules.SimPadlockMode;

        }

        // Nifty Access functions
        public bool GouraudOn()
        {
            return (DispFlags.IsFlagSet(PO_DISP_FLAGS.DISP_GOURAUD)) && true;
        }

        public bool HazingOn()
        {
            return (DispFlags.IsFlagSet(PO_DISP_FLAGS.DISP_HAZING)) && true;
        }

        public bool FilteringOn()
        {
            return (DispFlags.IsFlagSet(PO_DISP_FLAGS.DISP_BILINEAR)) && true;
        }

        public bool PerspectiveOn()
        {
            return (DispFlags.IsFlagSet(PO_DISP_FLAGS.DISP_PERSPECTIVE)) && true;
        }

        public bool AlphaOn()
        {
            return (DispFlags.IsFlagSet(PO_DISP_FLAGS.DISP_ALPHA_BLENDING)) && true;
        }

        public void SetTextureLevel(int level)
        {
            DispTextureLevel = level;
        }

        public int TextureLevel()
        {
            return DispTextureLevel;
        }

        public void SetTerrainDistance(float distance)
        {
            DispTerrainDist = distance;
        }

        public float TerrainDistance()
        {
            return DispTerrainDist;
        }

        public void SetMaxTerrainLevel(int level)
        {
            DispMaxTerrainLevel = level;
        }

        public int MaxTerrainLevel()
        {
            return DispMaxTerrainLevel;
        }

        public bool ObjectDynScalingOn()
        {
            return (ObjFlags.IsFlagSet(PO_OBJ_FLAGS.DISP_OBJ_DYN_SCALING)) && true;
        }

        public bool ObjectTexturesOn()
        {
            return (ObjFlags.IsFlagSet(PO_OBJ_FLAGS.DISP_OBJ_TEXTURES)) && true;
        }

        public bool ObjectShadingOn()
        {
            return (ObjFlags.IsFlagSet(PO_OBJ_FLAGS.DISP_OBJ_SHADING)) && true;
        }

        public float ObjectDetailLevel()
        {
            return ObjDetailLevel;
        }

        public float ObjectMagnification()
        {
            return ObjMagnification;
        }

        public float BubbleRatio()
        {
            return PlayerBubble;
        }

        public int ObjectDeaggLevel()
        {
            return ObjDeaggLevel;
        }

        public int BuildingDeaggLevel()
        {
            return BldDeaggLevel;
        }

        public float SfxDetailLevel()
        {
            return SfxLevel;
        }

        public int AcmiFileSize()
        {
            return ACMIFileSize;
        }

        public FlightModelType GetFlightModelType()
        {
            return SimFlightModel;
        }

        public WeaponEffectType GetWeaponEffectiveness()
        {
            return SimWeaponEffect;
        }

        public AvionicsType GetAvionicsType()
        {
            return SimAvionicsType;
        }

        public AutopilotModeType GetAutopilotMode()
        {
            return SimAutopilotType;
        }

        public RefuelModeType GetRefuelingMode()
        {
            return SimAirRefuelingMode;
        }

        public PadlockModeType GetPadlockMode()
        {
            return SimPadlockMode;
        }

        public VisualCueType GetVisualCueMode()
        {
            return SimVisualCueMode;
        }

        public bool AutoTargetingOn()
        {
            return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_AUTO_TARGET)) && true;
        }

        public bool BlackoutOn()
        {
            return !(SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_NO_BLACKOUT)) && true;
        }

        public bool NoBlackout()
        {
            return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_NO_BLACKOUT)) && true;
        }

        public bool UnlimitedFuel()
        {
            return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_UNLIMITED_FUEL)) && true;
        }

        public bool UnlimitedAmmo()
        {
            return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_UNLIMITED_AMMO)) && true;
        }

        public bool UnlimitedChaff()
        {
            return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_UNLIMITED_CHAFF)) && true;
        }

        public bool CollisionsOn()
        {
            return !(SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_NO_COLLISIONS)) && true;
        }

        public bool NoCollisions()
        {
            return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_NO_COLLISIONS)) && true;
        }

        public bool NameTagsOn()
        {
            return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_NAMETAGS)) && true;
        }

        public bool LiftLineOn()
        {
            return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_LIFTLINE_CUE)) && true;
        }

        public bool BullseyeOn()
        {
            return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_BULLSEYE_CALLS)) && true;
        }

        public bool InvulnerableOn()
        {
            return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_INVULNERABLE)) && true;
        }

        public bool WeatherOn()
        {
            return !(GeneralFlags.IsFlagSet(PO_GEN_FLAGS.GEN_NO_WEATHER));
        }

        public bool MFDTerrainOn()
        {
            return (GeneralFlags.IsFlagSet(PO_GEN_FLAGS.GEN_MFD_TERRAIN)) && true;
        }

        public bool HawkeyeTerrainOn()
        {
            return (GeneralFlags.IsFlagSet(PO_GEN_FLAGS.GEN_HAWKEYE_TERRAIN)) && true;
        }

        public bool PadlockViewOn()
        {
            return (GeneralFlags.IsFlagSet(PO_GEN_FLAGS.GEN_PADLOCK_VIEW)) && true;
        }

        public bool HawkeyeViewOn()
        {
            return (GeneralFlags.IsFlagSet(PO_GEN_FLAGS.GEN_HAWKEYE_VIEW)) && true;
        }

        public bool ExternalViewOn()
        {
            return (GeneralFlags.IsFlagSet(PO_GEN_FLAGS.GEN_EXTERNAL_VIEW)) && true;
        }

        public int CampaignGroundRatio()
        {
            return CampGroundRatio;
        }

        public int CampaignAirRatio()
        {
            return CampAirRatio;
        }

        public int CampaignAirDefenseRatio()
        {
            return CampAirDefenseRatio;
        }

        public int CampaignNavalRatio()
        {
            return CampNavalRatio;
        }

        public int CampaignEnemyAirExperience()
        {
            return CampEnemyAirExperience;
        }

        public int CampaignEnemyGroundExperience()
        {
            return CampEnemyGroundExperience;
        }

        public int CampaignEnemyStockpile()
        {
            return CampEnemyStockpile;
        }

        public int CampaignFriendlyStockpile()
        {
            return CampFriendlyStockpile;
        }

        // Setter functions
        public void SetSimFlag(PO_SIM_FLAGS flag)
        {
            SimFlags |= flag;
        }

        public void ClearSimFlag(PO_SIM_FLAGS flag)
        {
            SimFlags &= ~flag;
        }

        public void SetDispFlag(PO_DISP_FLAGS flag)
        {
            DispFlags |= flag;
        }

        public void ClearDispFlag(PO_DISP_FLAGS flag)
        {
            DispFlags &= ~flag;
        }

        public void SetObjFlag(PO_OBJ_FLAGS flag)
        {
            ObjFlags |= flag;
        }

        public void ClearObjFlag(PO_OBJ_FLAGS flag)
        {
            ObjFlags &= ~flag;
        }

        public void SetGenFlag(PO_GEN_FLAGS flag)
        {
            GeneralFlags |= flag;
        }

        public void ClearGenFlag(PO_GEN_FLAGS flag)
        {
            GeneralFlags &= ~flag;
        }

        public void SetStartFlag(SimStartFlags flag)
        {
            SimStart = flag;
        }

        public SimStartFlags GetStartFlag()
        {
            return (SimStart);
        }

        public void SetKeyFile(string fname)
        {
            keyfile = fname;
        }

        public string GetKeyfile()
        {
            return keyfile;
        }

        public void SetJoystick(Guid newID)
        {
            joystick = newID;
        }

        public Guid GetJoystick()
        {
            return joystick;
        }

        public void SetCampEnemyAirExperience(int exp)
        {
            CampEnemyAirExperience = exp;
        }

        public void SetCampEnemyGroundExperience(int exp)
        {
            CampEnemyGroundExperience = exp;
        }

        public void SetCampGroundRatio(int ratio)
        {
            CampGroundRatio = ratio;
        }

        public void SetCampAirRatio(int ratio)
        {
            CampAirRatio = ratio;
        }

        public void SetCampAirDefenseRatio(int ratio)
        {
            CampAirDefenseRatio = ratio;
        }

        public void SetCampNavalRatio(int ratio)
        {
            CampNavalRatio = ratio;
        }


        // ==================================
        // Our global player options instance
        // ==================================

        public static PlayerOptionsClass PlayerOptions;

#if TODO
		private int CheckNumberPlayers ()
		{
			VuSessionsIterator
				sit(FalconLocalGame);
		
			FalconSessionEntity
				*session;
		
			int
				count;
		
			session = (FalconSessionEntity*) sit.GetFirst ();
			count = 0;
		
			while (session)
			{
				session = (FalconSessionEntity *) sit.GetNext ();
				count ++;
			}
		
			return FalconLocalGame.GetRules ().MaxPlayers - count;
		}
#endif
    }

    // =====================
    // Other functions
    // =====================

    public struct SkyColorDataType
    {
        string name;			// To display in UI
        string todname;	// Filename of tod file
        string image1;	// screenshot 5:00
        string image2;   // screenshot 10:00
        string image3;	// screenshot 15:00
        string image4;   // screenshot 20:00
    }

    public struct WeatherPatternDataType
    {
        string name;			// To display in UI
        string filename; // accompagnied filename
        string picfname;	// picture of weather distribution
    }

}

