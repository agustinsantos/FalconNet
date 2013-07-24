using System;
using System.IO;
using FalconNet.Common;
using FalconNet.Common.Encoding;

namespace FalconNet.F4Common
{
    // TODO enum{RUL_PW_LEN = 20}
    public class RulesStruct
    {
        internal string Password;
        internal int MaxPlayers;
        internal float ObjMagnification;
        internal PO_SIM_FLAGS SimFlags;					// Sim flags
        internal FlightModelType SimFlightModel;			// Flight model type
        internal WeaponEffectType SimWeaponEffect;
        internal AvionicsType SimAvionicsType;
        internal AutopilotModeType SimAutopilotType;
        internal RefuelModeType SimAirRefuelingMode;
        internal PadlockModeType SimPadlockMode;
        internal ulong BumpTimer;
        internal ulong AiPullTime;
        internal ulong AiPatience;
        internal ulong AtcPatience;
        internal PO_GEN_FLAGS GeneralFlags;
    }

    public enum RulesModes
    {
        rINSTANT_ACTION,
        rDOGFIGHT,
        rTACTICAL_ENGAGEMENT,
        rCAMPAIGN,
        rNUM_MODES,
    }

    public class RulesClass : RulesStruct
    {


        public RulesClass()
        {
            Initialize();
        }

        public void Initialize()
        {

            Password = "";
            MaxPlayers = 16;
            ObjMagnification = 5;
            SimFlags = PO_SIM_FLAGS.SIM_RULES_FLAGS;		// Sim flags
            SimFlightModel = FlightModelType.FMSimplified;			// Flight model type
            SimWeaponEffect = WeaponEffectType.WEExaggerated;
            SimAvionicsType = AvionicsType.ATEasy;
            SimAutopilotType = AutopilotModeType.APIntelligent;
            SimAirRefuelingMode = RefuelModeType.ARSimplistic;
            SimPadlockMode = PadlockModeType.PDEnhanced;
            GeneralFlags = PO_GEN_FLAGS.GEN_RULES_FLAGS;

            string dataFileName = F4File.F4FindFile("atc.ini");
            BumpTimer = (ulong)Math.Max(0L, MultiplatformIni.GetPrivateProfileInt("ATC", "PlayerBumpTime", 10, dataFileName));
            BumpTimer *= 60000;
            AiPullTime = (ulong)Math.Max(0L, MultiplatformIni.GetPrivateProfileInt("ATC", "AIPullTime", 20, dataFileName));
            AiPullTime *= 60000;
            AiPatience = (ulong)Math.Max(0L, MultiplatformIni.GetPrivateProfileInt("ATC", "AIPatience", 120, dataFileName));
            AiPatience *= 1000;
            AtcPatience = (ulong)Math.Max(0L, MultiplatformIni.GetPrivateProfileInt("ATC", "ATCPatience", 180, dataFileName));
            AtcPatience *= 1000;
        }

        public void LoadRules(RulesStruct rules)
        {
            if (rules != null)
            {
                Password = rules.Password;
                MaxPlayers = rules.MaxPlayers;
                ObjMagnification = rules.ObjMagnification;
                SimFlags = rules.SimFlags;					// Sim flags
                SimFlightModel = rules.SimFlightModel;			// Flight model type
                SimWeaponEffect = rules.SimWeaponEffect;
                SimAvionicsType = rules.SimAvionicsType;
                SimAutopilotType = rules.SimAutopilotType;
                SimAirRefuelingMode = rules.SimAirRefuelingMode;
                SimPadlockMode = rules.SimPadlockMode;
                GeneralFlags = rules.GeneralFlags;
            }
        }

        public static bool LoadAllRules(string filename = "default")
        {
            FileStream fp;

            string path = F4File.F4FindFile(filename, "rul");
            if (!File.Exists(path))
                path = F4File.F4FindFile("default", "rul");
            try
            {
                fp = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                //TODOMonoPrint(_T("Couldn't open default rules\n"),filename);
                return false;
            }

            RulesClass[] tempRules = new RulesClass[(int)RulesModes.rNUM_MODES];

            for (int i = 0; i < (int)RulesModes.rNUM_MODES; i++)
            {
                tempRules[i] = RulesClassEncodingLE.Decode(fp);
            }
            fp.Close();


            for (int i = 0; i < (int)RulesModes.rNUM_MODES; i++)
            {
                string dataFileName = F4File.F4FindFile("atc.ini");
                tempRules[i].BumpTimer = (ulong)Math.Max(0L, MultiplatformIni.GetPrivateProfileInt("ATC", "PlayerBumpTime", 10, dataFileName));
                tempRules[i].BumpTimer *= 60000;
                tempRules[i].AiPullTime = (ulong)Math.Max(0L, MultiplatformIni.GetPrivateProfileInt("ATC", "AIPullTime", 20, dataFileName));
                tempRules[i].AiPullTime *= 60000;
                tempRules[i].AiPatience = (ulong)Math.Max(0L, MultiplatformIni.GetPrivateProfileInt("ATC", "AIPatience", 120, dataFileName));
                tempRules[i].AiPatience *= 1000;
                tempRules[i].AtcPatience = (ulong)Math.Max(0L, MultiplatformIni.GetPrivateProfileInt("ATC", "ATCPatience", 180, dataFileName));
                tempRules[i].AtcPatience *= 1000;
            }
            gRules = tempRules;
            return true;
        }


        public int SaveRules(string filename = "default")
        {
#if TODO
			FILE		*fp;
			_TCHAR		path[_MAX_PATH];
			size_t		success = 0;
			
			_stprintf(path,_T("%s\\config\\%s.rul"),FalconDataDirectory,filename);
				
			if((fp = _tfopen(path,"wb")) == null)
			{
				MonoPrint(_T("Couldn't save rules"));
				return false;
			}
			success = fwrite(gRules, sizeof(RulesStruct), rNUM_MODES, fp);
			fclose(fp);
			if(success != rNUM_MODES)
			{
				MonoPrint(_T("Couldn't save rules"));
				return false;
			}
			
			return true;
#endif
            throw new NotImplementedException();
        }

        private const string PwdMask = "Blood makes the grass grow, kill, kill, kill!";
        private const string PwdMask2 = "ojodp^&SANDsfsl,[poe5487wqer1]@&$N";

        public RulesStruct GetRules()
        {
            return (this);
        }

        private void EncryptPwd()
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


        public int CheckPassword(string Pwd)
        {
#if TODO
			//if(Pilot.Password[0] == 0)
				//return true;
		
			//EncryptPwd();
			if( _tcscmp( Pwd, Password) )
			{
				//EncryptPwd();
				return false;
			}
			else
			{
				//EncryptPwd();
				return true;
			}
#endif
            throw new NotImplementedException();
        }

        public int SetPassword(string Pwd)
        {
#if TODO
			if(_tcslen(newPassword) <= RUL_PW_LEN) 
			{
				_tcscpy(Password,newPassword);
				//EncryptPwd();
				return true;
			}
		
			return false;
#endif
            throw new NotImplementedException();
        }

        public int GetPassword(string Pwd)
        {
#if TODO
			//EncryptPwd();
			_tcscpy( Pwd, Password );
			//EncryptPwd();
			return true;
#endif
            throw new NotImplementedException();
        }

        public float ObjectMagnification()
        {
            return ObjMagnification;
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

        public bool AutoTargetingOn()
        {
            return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_AUTO_TARGET)) && true;
        }

        public bool BlackoutOn()
        {
            return !(SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_NO_BLACKOUT));
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
            return !(SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_NO_COLLISIONS));
        }

        public bool NoCollisions()
        {
            return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_NO_COLLISIONS)) && true;
        }

        public bool NameTagsOn()
        {
            return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_NAMETAGS)) && true;
        }

        public bool WeatherOn()
        {
            return !(GeneralFlags.IsFlagSet(PO_GEN_FLAGS.GEN_NO_WEATHER));
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

        public bool InvulnerableOn()
        {
            return (SimFlags.IsFlagSet(PO_SIM_FLAGS.SIM_INVULNERABLE)) && true;
        }

        public void SetSimFlag(PO_SIM_FLAGS flag)
        {
            SimFlags |= flag;
        }

        public void ClearSimFlag(PO_SIM_FLAGS flag)
        {
            SimFlags &= ~flag;
        }

        public void SetGenFlag(PO_GEN_FLAGS flag)
        {
            GeneralFlags |= flag;
        }

        public void ClearGenFlag(PO_GEN_FLAGS flag)
        {
            GeneralFlags &= ~flag;
        }

        public void SetObjMagnification(float mag)
        {
            ObjMagnification = mag;
        }

        public void SetMaxPlayers(int num)
        {
            if (num > 0)
                MaxPlayers = num;
        }

        public void SetSimFlightModel(FlightModelType FM)
        {
            SimFlightModel = FM;
        }

        public void SetSimWeaponEffect(WeaponEffectType WE)
        {
            SimWeaponEffect = WE;
        }

        public void SetSimAvionicsType(AvionicsType AT)
        {
            SimAvionicsType = AT;
        }

        public void SetSimAutopilotType(AutopilotModeType AM)
        {
            SimAutopilotType = AM;
        }

        public void SetRefuelingMode(RefuelModeType RM)
        {
            SimAirRefuelingMode = RM;
        }

        public void SetPadlockMode(PadlockModeType PM)
        {
            SimPadlockMode = PM;
        }

        public static RulesClass[] gRules = new RulesClass[(int)RulesModes.rNUM_MODES];
        public static RulesModes RuleMode = RulesModes.rINSTANT_ACTION;
    }
    public static class RulesClassEncodingLE
    {
        public const int RUL_PW_LEN = 20;

        public static void Encode(ByteWrapper buffer, RulesClass val)
        {
            throw new NotImplementedException();

        }
        public static void Encode(Stream stream, RulesClass val)
        {
            throw new NotImplementedException();

        }

        public static RulesClass Decode(ByteWrapper buffer)
        {
            RulesClass rst = new RulesClass();
            rst.Password = StringFixedASCIIEncoding.Decode(buffer, RUL_PW_LEN);
            rst.MaxPlayers = Int32EncodingLE.Decode(buffer);
            rst.ObjMagnification = SingleEncodingLE.Decode(buffer);
            rst.SimFlags = (PO_SIM_FLAGS)Int32EncodingLE.Decode(buffer);		 // Sim flags
            rst.SimFlightModel = (FlightModelType)Int32EncodingLE.Decode(buffer);// Flight model type
            rst.SimWeaponEffect = (WeaponEffectType)Int32EncodingLE.Decode(buffer);
            rst.SimAvionicsType = (AvionicsType)Int32EncodingLE.Decode(buffer);
            rst.SimAutopilotType = (AutopilotModeType)Int32EncodingLE.Decode(buffer);
            rst.SimAirRefuelingMode = (RefuelModeType)Int32EncodingLE.Decode(buffer);
            rst.SimPadlockMode = (PadlockModeType)Int32EncodingLE.Decode(buffer);
            rst.BumpTimer = UInt32EncodingLE.Decode(buffer);
            rst.AiPullTime = UInt32EncodingLE.Decode(buffer);
            rst.AiPatience = UInt32EncodingLE.Decode(buffer);
            rst.AtcPatience = UInt32EncodingLE.Decode(buffer);
            rst.GeneralFlags = (PO_GEN_FLAGS)Int16EncodingLE.Decode(buffer);
            buffer.GetBytes(2);
            return rst;
        }

        public static RulesClass Decode(Stream stream)
        {
            RulesClass rst = new RulesClass();
            rst.Password = StringFixedASCIIEncoding.Decode(stream, RUL_PW_LEN);
            rst.MaxPlayers = Int32EncodingLE.Decode(stream);
            rst.ObjMagnification = SingleEncodingLE.Decode(stream);
            rst.SimFlags = (PO_SIM_FLAGS)Int32EncodingLE.Decode(stream);					// Sim flags
            rst.SimFlightModel = (FlightModelType)Int32EncodingLE.Decode(stream);// Flight model type
            rst.SimWeaponEffect = (WeaponEffectType)Int32EncodingLE.Decode(stream);
            rst.SimAvionicsType = (AvionicsType)Int32EncodingLE.Decode(stream);
            rst.SimAutopilotType = (AutopilotModeType)Int32EncodingLE.Decode(stream);
            rst.SimAirRefuelingMode = (RefuelModeType)Int32EncodingLE.Decode(stream);
            rst.SimPadlockMode = (PadlockModeType)Int32EncodingLE.Decode(stream);
            rst.BumpTimer = UInt32EncodingLE.Decode(stream);
            rst.AiPullTime = UInt32EncodingLE.Decode(stream);
            rst.AiPatience = UInt32EncodingLE.Decode(stream);
            rst.AtcPatience = UInt32EncodingLE.Decode(stream);
            rst.GeneralFlags = (PO_GEN_FLAGS)Int16EncodingLE.Decode(stream);
            stream.ReadBytes(0, 2);
            return rst;
        }

        public static int Size
        {
            get { return 76; }
        }
    }
}

