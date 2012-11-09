using System;
using System.IO;
using FalconNet.Common;

namespace FalconNet.FalcLib
{
	// TODO enum{RUL_PW_LEN = 20}
	public class RulesStruct
	{
		internal string				Password;
		internal int				MaxPlayers;
		internal float				ObjMagnification;
		internal PO_SIM_FLAGS		SimFlags;					// Sim flags
		internal FlightModelType	SimFlightModel;			// Flight model type
		internal WeaponEffectType	SimWeaponEffect;
		internal AvionicsType		SimAvionicsType;
		internal AutopilotModeType  SimAutopilotType;
		internal RefuelModeType		SimAirRefuelingMode;
		internal PadlockModeType	SimPadlockMode;
		internal ulong				BumpTimer;
		internal ulong				AiPullTime;
		internal ulong				AiPatience;
		internal ulong				AtcPatience;
		internal PO_GEN_FLAGS		GeneralFlags;		
	}

	public  enum RulesModes
	{
		rINSTANT_ACTION,
		rDOGFIGHT,
		rTACTICAL_ENGAGEMENT,
		rCAMPAIGN,
		rNUM_MODES,
	}

	public class RulesClass: RulesStruct
	{
	

		public 	RulesClass ()
		{
			Initialize();
		}

		public 	void Initialize ()
		{
#if TODO
			Password= "";
			MaxPlayers			= 16;
			ObjMagnification	= 5;	
			SimFlags			= SIM_RULES_FLAGS;		// Sim flags
			SimFlightModel		= FMSimplified;			// Flight model type
			SimWeaponEffect		= WEExaggerated;
			SimAvionicsType		= ATEasy;
			SimAutopilotType	= APIntelligent;
			SimAirRefuelingMode	= ARSimplistic;				
			SimPadlockMode		= PDEnhanced;
			GeneralFlags		= GEN_RULES_FLAGS;	
		
			string dataFileName;
			dataFileName= FalconCampaignSaveDirectory +"atc.ini";
			BumpTimer		= max(0, GetPrivateProfileInt("ATC", "PlayerBumpTime", 10, dataFileName)); 	
			BumpTimer		*= 60000;
			AiPullTime		= max(0, GetPrivateProfileInt("ATC", "AIPullTime", 20, dataFileName)); 	
			AiPullTime		*= 60000;
			AiPatience		= max(0, GetPrivateProfileInt("ATC", "AIPatience", 120, dataFileName)); 	
			AiPatience		*= 1000;
			AtcPatience		= max(0, GetPrivateProfileInt("ATC", "ATCPatience", 180, dataFileName)); 	
			AtcPatience		*= 1000;
#endif
		}


		public 	int LoadRules (string  filename = "default")
		{
#if TODO
			if(rules)
				memcpy(this,rules,sizeof(RulesStruct));
			/*
			_tcscpy(Password,rules->Password);
			MaxPlayers			= rules->MaxPlayers;
			ObjMagnification	= rules->ObjMagnification;	
			SimFlags			= rules->SimFlags;					// Sim flags
			SimFlightModel		= rules->SimFlightModel;			// Flight model type
			SimWeaponEffect		= rules->SimWeaponEffect;
			SimAvionicsType		= rules->SimAvionicsType;
			SimAutopilotType	= rules->SimAutopilotType;
			SimAirRefuelingMode	= rules->SimAirRefuelingMode;				
			SimPadlockMode		= rules->SimPadlockMode;
			GeneralFlags		= rules->GeneralFlags;*/
#endif
			throw new NotImplementedException();
		}


		public 	int SaveRules (string  filename = "default")
		{
#if TODO
			FILE		*fp;
			_TCHAR		path[_MAX_PATH];
			size_t		success = 0;
			
			_stprintf(path,_T("%s\\config\\%s.rul"),FalconDataDirectory,filename);
				
			if((fp = _tfopen(path,"wb")) == NULL)
			{
				MonoPrint(_T("Couldn't save rules"));
				return FALSE;
			}
			success = fwrite(gRules, sizeof(RulesStruct), rNUM_MODES, fp);
			fclose(fp);
			if(success != rNUM_MODES)
			{
				MonoPrint(_T("Couldn't save rules"));
				return FALSE;
			}
			
			return TRUE;
#endif
			throw new NotImplementedException();
		}

		private const string PwdMask="Blood makes the grass grow, kill, kill, kill!";
		private const string PwdMask2="ojodp^&SANDsfsl,[poe5487wqer1]@&$N";

		public 	void LoadRules (RulesStruct rules)
		{
#if TODO
			size_t		success = 0;
			_TCHAR		path[_MAX_PATH];
			long		size;
			FILE *fp;
		
			_stprintf(path,_T("%s\\config\\%s.rul"),FalconDataDirectory,filename);
			
			fp = _tfopen(path,_T("rb"));
			if(!fp)
			{
				MonoPrint(_T("Couldn't open %s rules file\n"),filename);
				_stprintf(path,_T("%s\\Config\\default.rul"),FalconDataDirectory);
				fp = _tfopen(path,"rb");
				if(!fp)
				{
					MonoPrint(_T("Couldn't open default rules\n"),filename);
					Initialize();
					return FALSE;
				}
			}
			
			fseek(fp,0,SEEK_END);
			size = ftell(fp);
			fseek(fp,0,SEEK_SET);
		
			if(size != sizeof(RulesStruct) * rNUM_MODES)
			{
				MonoPrint(_T("%s's rules are in old file format\n"),filename);
				return FALSE;
			}
		
		
			RulesClass tempRules[rNUM_MODES];
		
			success = fread(&tempRules, sizeof(RulesStruct), rNUM_MODES, fp);
			fclose(fp);
			if(success != rNUM_MODES)
			{
				MonoPrint(_T("Failed to read %s's rules file\n"),filename);
				//Initialize();
				return FALSE;
			}
			char dataFileName[_MAX_PATH];
			sprintf (dataFileName, "%s\\atc.ini", FalconCampaignSaveDirectory);
			tempRules[RuleMode].BumpTimer		= max(0, GetPrivateProfileInt("ATC", "PlayerBumpTime", 10, dataFileName)); 	
			tempRules[RuleMode].BumpTimer		*= 60000;
			tempRules[RuleMode].AiPullTime		= max(0, GetPrivateProfileInt("ATC", "AIPullTime", 20, dataFileName)); 	
			tempRules[RuleMode].AiPullTime		*= 60000;
			tempRules[RuleMode].AiPatience		= max(0, GetPrivateProfileInt("ATC", "AIPatience", 120, dataFileName)); 	
			tempRules[RuleMode].AiPatience		*= 1000;
			tempRules[RuleMode].AtcPatience		= max(0, GetPrivateProfileInt("ATC", "ATCPatience", 180, dataFileName)); 	
			tempRules[RuleMode].AtcPatience		*= 1000;
		
			memcpy(this,&(tempRules[RuleMode]),sizeof(RulesStruct));
			return TRUE;
#endif
			throw new NotImplementedException();
		}


		public 	RulesStruct GetRules ()
		{
			return (this);
		}
		
		private void EncryptPwd ()
		{
		#if TODO
			int i;
			_TCHAR *ptr;
		
			ptr=Password;
		
			for(i=0;i<RUL_PW_LEN;i++)
			{
				*ptr ^= PwdMask[i % strlen(PwdMask)];
				*ptr ^= PwdMask2[i % strlen(PwdMask2)];
				ptr++;
			}
		#endif
					throw new NotImplementedException();
		}
		

		public 	int CheckPassword (string  Pwd)
		{
		#if TODO
			//if(Pilot.Password[0] == 0)
				//return TRUE;
		
			//EncryptPwd();
			if( _tcscmp( Pwd, Password) )
			{
				//EncryptPwd();
				return FALSE;
			}
			else
			{
				//EncryptPwd();
				return TRUE;
			}
		#endif
					throw new NotImplementedException();
		}

		public 	int SetPassword (string  Pwd)
		{
#if TODO
			if(_tcslen(newPassword) <= RUL_PW_LEN) 
			{
				_tcscpy(Password,newPassword);
				//EncryptPwd();
				return TRUE;
			}
		
			return FALSE;
#endif
			throw new NotImplementedException();
		}

		public 	int GetPassword (string  Pwd)
		{
#if TODO
			//EncryptPwd();
			_tcscpy( Pwd, Password );
			//EncryptPwd();
			return TRUE;
		#endif
					throw new NotImplementedException();
		}

		public 	float ObjectMagnification ()
		{
			return ObjMagnification;
		}

		public FlightModelType GetFlightModelType ()
		{
			return SimFlightModel;
		}

		public WeaponEffectType GetWeaponEffectiveness ()
		{
			return SimWeaponEffect;
		}

		public 	AvionicsType GetAvionicsType ()
		{
			return SimAvionicsType;
		}

		public 	AutopilotModeType GetAutopilotMode ()
		{
			return SimAutopilotType;
		}

		public RefuelModeType GetRefuelingMode ()
		{
			return SimAirRefuelingMode;
		}

		public 	PadlockModeType GetPadlockMode ()
		{
			return SimPadlockMode;
		}

		public 	bool AutoTargetingOn ()
		{
			return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_AUTO_TARGET)) && true;
		}

		public bool BlackoutOn ()
		{
			return !(SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_NO_BLACKOUT));
		}

		public bool NoBlackout ()
		{
			return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_NO_BLACKOUT)) && true;
		}

		public bool UnlimitedFuel ()
		{
			return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_UNLIMITED_FUEL)) && true;
		}

		public bool UnlimitedAmmo ()
		{
			return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_UNLIMITED_AMMO)) && true;
		}

		public bool UnlimitedChaff ()
		{
			return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_UNLIMITED_CHAFF)) && true;
		}

		public bool CollisionsOn ()
		{
			return !(SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_NO_COLLISIONS));
		}

		public bool NoCollisions ()
		{
			return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_NO_COLLISIONS)) && true;
		}

		public bool NameTagsOn ()
		{
			return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_NAMETAGS)) && true;
		}

		public bool WeatherOn ()
		{
			return !(GeneralFlags.IsFlagSet(PO_GEN_FLAGS.GEN_NO_WEATHER));
		}

		public bool PadlockViewOn ()
		{
			return (GeneralFlags.IsFlagSet(PO_GEN_FLAGS.GEN_PADLOCK_VIEW)) && true;
		}

		public bool HawkeyeViewOn ()
		{
			return (GeneralFlags.IsFlagSet(PO_GEN_FLAGS.GEN_HAWKEYE_VIEW)) && true;
		}

		public bool ExternalViewOn ()
		{
			return (GeneralFlags.IsFlagSet(PO_GEN_FLAGS.GEN_EXTERNAL_VIEW)) && true;
		}

		public bool InvulnerableOn ()
		{
			return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_INVULNERABLE)) && true;
		}

		public void SetSimFlag (PO_SIM_FLAGS flag)
		{
			SimFlags |= flag;
		}

		public 	void ClearSimFlag (PO_SIM_FLAGS flag)
		{
			SimFlags &= ~flag;
		}

		public 	void SetGenFlag (PO_GEN_FLAGS flag)
		{
			GeneralFlags |= flag;
		}

		public 	void ClearGenFlag (PO_GEN_FLAGS flag)
		{
			GeneralFlags &= ~flag;
		}

		public 	void SetObjMagnification (float mag)
		{
			ObjMagnification = mag;
		}

		public 	void SetMaxPlayers (int num)
		{
			if (num > 0)
				MaxPlayers = num;
		}

		public 	void SetSimFlightModel (FlightModelType FM)
		{
			SimFlightModel = FM;
		}

		public 	void SetSimWeaponEffect (WeaponEffectType WE)
		{
			SimWeaponEffect = WE;
		}

		public 	void SetSimAvionicsType (AvionicsType AT)
		{
			SimAvionicsType = AT;
		}

		public 	void SetSimAutopilotType (AutopilotModeType AM)
		{
			SimAutopilotType = AM;
		}

		public 	void SetRefuelingMode (RefuelModeType RM)
		{
			SimAirRefuelingMode = RM;
		}

		public 	void SetPadlockMode (PadlockModeType PM)
		{
			SimPadlockMode = PM;
		}

		public static RulesClass[] gRules = new RulesClass[(int)RulesModes.rNUM_MODES];
		public static RulesModes RuleMode = RulesModes.rINSTANT_ACTION;

		public static int LoadAllRules (string filename)
		{
#if TODO
			size_t		success = 0;
			_TCHAR		path[_MAX_PATH];
			long		size;
			FILE *fp;
		
			_stprintf(path,_T("%s\\config\\%s.rul"),FalconDataDirectory,filename);
			
			fp = _tfopen(path,_T("rb"));
			if(!fp)
			{
				MonoPrint(_T("Couldn't open %s rules file\n"),filename);
				_stprintf(path,_T("%s\\Config\\default.rul"),FalconDataDirectory);
				fp = _tfopen(path,"rb");
				if(!fp)
				{
					MonoPrint(_T("Couldn't open default rules\n"),filename);
					return FALSE;
				}
			}
			
			fseek(fp,0,SEEK_END);
			size = ftell(fp);
			fseek(fp,0,SEEK_SET);
		
			if(size != sizeof(RulesStruct) * rNUM_MODES)
			{
				MonoPrint(_T("%s's rules are in old file format\n"),filename);
				return FALSE;
			}
		
		
			RulesClass tempRules[rNUM_MODES];
		
			success = fread(&tempRules, sizeof(RulesStruct), rNUM_MODES, fp);
			fclose(fp);
			if(success != rNUM_MODES)
			{
				MonoPrint(_T("Failed to read %s's rules file\n"),filename);
				//Initialize();
				return FALSE;
			}
		
			for(int i = 0; i < rNUM_MODES;i++)
			{
				char dataFileName[_MAX_PATH];
				sprintf (dataFileName, "%s\\atc.ini", FalconCampaignSaveDirectory);
				tempRules[i].BumpTimer		= max(0, GetPrivateProfileInt("ATC", "PlayerBumpTime", 10, dataFileName)); 	
				tempRules[i].BumpTimer		*= 60000;
				tempRules[i].AiPullTime		= max(0, GetPrivateProfileInt("ATC", "AIPullTime", 20, dataFileName)); 	
				tempRules[i].AiPullTime		*= 60000;
				tempRules[i].AiPatience		= max(0, GetPrivateProfileInt("ATC", "AIPatience", 120, dataFileName)); 	
				tempRules[i].AiPatience		*= 1000;
				tempRules[i].AtcPatience	= max(0, GetPrivateProfileInt("ATC", "ATCPatience", 180, dataFileName)); 	
				tempRules[i].AtcPatience	*= 1000;
			}
			memcpy(&gRules,&tempRules,sizeof(RulesStruct)*rNUM_MODES);
			return TRUE;
#endif
			throw new NotImplementedException();
		}

	}

}

