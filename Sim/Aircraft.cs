using System;
using FalconNet.VU;
using FalconNet.Common;
using System.IO;
using FalconNet.FalcLib;
using FalconNet.Graphics;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;

namespace FalconNet.Sim
{
    ////////////////////////////////////////////
    public struct DamageF16PieceStructure
    {
        int mask;
        int index;
        int damage;
        int sfxflag;
        int sfxtype;
        float lifetime;
        float dx, dy, dz;
        float yd, pd, rd;
        float pitch, roll;	// resting angle
    };
    ////////////////////////////////////////////



    public class AircraftClass : SimVehicleClass
    {
        // TODO, inserted just for compilation, not in the original code!
        public AircraftClass(): base(0)
        {
            throw new NotImplementedException();
        }
#if TODO
        public const int FLARE_STATION = 0;
        public const int CHAFF_STATION = 1;
        public const int DEBRIS_STATION = 2;

#if USE_SH_POOLS
		
			public // Overload new/delete to use a SmartHeap fixed size pool
			public public void *operator new(size_t size) { Debug.Assert( size == sizeof(AircraftClass) ); return MemAllocFS(pool);	};
			public void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
			public static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(AircraftClass), 200, 0 ); };
			public static void ReleaseStorage()	{ MemPoolFree( pool ); };
			public static MEM_POOL	pool;
#endif


        public enum AutoPilotType
        {
            ThreeAxisAP,
            WaypointAP,
            CombatAP,
            LantirnAP,
            APOff
        }

        [Flags]
        public enum ACFLAGS
        {
            isF16 = 0x1, // is an F-16 - hopefully historic usage only soon
            hasSwing = 0x2, // has swing wing
            isComplex = 0x4, // has a complex model (lots of dofs and switches)
            InRecovery = 0x8, // recovering from gloc
        }

        [Flags]
        public enum AvionicsPowerFlags : uint
        {
            NoPower = 0,
            SMSPower = 0x1,
            FCCPower = 0x2,
            MFDPower = 0x4,
            UFCPower = 0x8,
            GPSPower = 0x10,
            DLPower = 0x20,
            MAPPower = 0x40,
            LeftHptPower = 0x80,
            RightHptPower = 0x100,
            TISLPower = 0x200,
            FCRPower = 0x400,
            HUDPower = 0x800,
            //MI
            EWSRWRPower = 0x1000,
            EWSJammerPower = 0x2000,
            EWSChaffPower = 0x4000,
            EWSFlarePower = 0x8000,
            //MI
            RaltPower = 0x10000,
            RwrPower = 0x20000,
            APPower = 0x40000,
            PFDPower = 0x80000,
            ChaffFlareCount = 0x100000,
            //MI
            IFFPower = 0x200000,
            // systems that don't have power normally.
            SpotLightPower = 0x20000000,
            InstrumentLightPower = 0x40000000,
            InteriorLightPower = 0x80000000, // start from the top down
            AllPower = 0xffffff // all the systems have power normally.
        }

        public enum MainPowerType
        {
            MainPowerOff,
            MainPowerBatt,
            MainPowerMain
        }

        // start of the power state matrix,
        // will get filled in more when I know what I'm talking about a little.
        public enum PowerStates
        {
            PowerNone = 0,
            PowerFlcs,
            PowerBattery,
            PowerEmergencyBus,
            PowerEssentialBus,
            PowerNonEssentialBus,
            PowerMaxState,
        }

        public enum LightSwitch
        {
            LT_OFF,
            LT_LOW,
            LT_NORMAL
        }

        public LightSwitch interiorLight, instrumentLight, spotLight;

        public void SetInteriorLight(LightSwitch st)
        {
            interiorLight = st;
        }

        public void SetInstrumentLight(LightSwitch st)
        {
            instrumentLight = st;
        }

        public void SetSpotLight(LightSwitch st)
        {
            spotLight = st;
        }

        public LightSwitch GetInteriorLight()
        {
            return interiorLight;
        }

        public LightSwitch GetInstrumentLight()
        {
            return instrumentLight;
        }

        public LightSwitch GetSpotLight()
        {
            return spotLight;
        }

        //MI
        public enum EWSPGMSwitch
        {
            Off,
            Stby,
            Man,
            Semi,
            Auto
        }
        //MI

        private static bool Warned = false;

        //MI new OverG/Speed stuff
        public void CheckForOverG()
        {
            throw new NotImplementedException();
        }

        public void CheckForOverSpeed()
        {
            throw new NotImplementedException();
        }

        public void DoOverGSpeedDamage(int station)
        {
            throw new NotImplementedException();
        }

        public void StoreToDamage(WeaponClass thing)
        {
            throw new NotImplementedException();
        }

        public StationFlags StationsFailed;

        [Flags]
        public enum StationFlags
        {
            Station1_Degr = 0x1,
            Station2_Degr = 0x2,
            Station3_Degr = 0x4,
            Station4_Degr = 0x8,
            Station5_Degr = 0x10,
            Station6_Degr = 0x20,
            Station7_Degr = 0x40,
            Station8_Degr = 0x80,
            Station9_Degr = 0x90,
            Station1_Fail = 0x100,
            Station2_Fail = 0x200,
            Station3_Fail = 0x400,
            Station4_Fail = 0x800,
            Station5_Fail = 0x1000,
            Station6_Fail = 0x2000,
            Station7_Fail = 0x4000,
            Station8_Fail = 0x8000,
            Station9_Fail = 0x9000,

        };
        public void StationFailed(StationFlags fl)
        {
            StationsFailed |= fl;
        }

        public int GetStationFailed(StationFlags fl)
        {
            return (StationsFailed & fl) == (StationFlags)fl ? 1 : 0;
        }

        //MI
        public ExtlLightFlags ExteriorLights;
        public enum ExtlLightFlags
        {
            Extl_Main_Power = 0x1,
            Extl_Anit_Coll = 0x2,
            Extl_Steady_Flash = 0x4,
            Extl_Wing_Tail = 0x8,
        };
        public void ExtlOn(ExtlLightFlags fl)
        {
            ExteriorLights |= fl;
        }

        public void ExtlOff(ExtlLightFlags fl)
        {
            ExteriorLights &= ~fl;
        }

        public int ExtlState(ExtlLightFlags fl)
        {
            return (ExteriorLights & fl) == (ExtlLightFlags)fl ? 1 : 0;
        }

        //MI
        public INSAlignFlags INSFlags;

        [Flags]
        public enum INSAlignFlags
        {
            INS_PowerOff = 0x1,
            INS_AlignNorm = 0x2,
            INS_AlignHead = 0x4,
            INS_Cal = 0x8,
            INS_AlignFlight = 0x10,
            INS_Nav = 0x20,
            INS_Aligned = 0x40,
            INS_ADI_OFF_IN = 0x80,
            INS_ADI_AUX_IN = 0x100,
            INS_HUD_FPM = 0x200,
            INS_HUD_STUFF = 0x400,
            INS_HSI_OFF_IN = 0x800,
            INS_HSD_STUFF = 0x1000,
            BUP_ADI_OFF_IN = 0x2000,
        };
        public void INSOn(INSAlignFlags fl)
        {
            INSFlags |= fl;
        }

        public void INSOff(INSAlignFlags fl)
        {
            INSFlags &= ~fl;
        }

        public int INSState(INSAlignFlags fl)
        {
            return (INSFlags & fl) == (INSAlignFlags)fl ? 1 : 0;
        }

        public void RunINS()
        {
            throw new NotImplementedException();
        }

        public void DoINSAlign()
        {
            throw new NotImplementedException();
        }

        public void SwitchINSToAlign()
        {
            throw new NotImplementedException();
        }

        public void SwitchINSToNav()
        {
            throw new NotImplementedException();
        }

        public void SwitchINSToOff()
        {
            throw new NotImplementedException();
        }

        public void SwitchINSToInFLT()
        {
            throw new NotImplementedException();
        }

        public void CheckINSStatus()
        {
            throw new NotImplementedException();
        }

        public void CalcINSDrift()
        {
            throw new NotImplementedException();
        }

        public void CalcINSOffset()
        {
            throw new NotImplementedException();
        }

        public float GetINSLatDrift()
        {
            return (INSLatDrift + INSLatOffset);
        }

        public float GetINSLongDrift()
        {
            return (INSLongDrift + INSLongOffset);
        }

        public float GetINSAltOffset()
        {
            return INSAltOffset;
        }

        public float GetINSHDGOffset()
        {
            return INSHDGOffset;
        }

        public float INSAlignmentTimer;
        public VU_TIME INSAlignmentStart;
        public bool INSAlign, HasAligned;
        public int INSStatus;
        public float INSLatDrift;
        public float INSLongDrift;
        public float INSTimeDiff;
        public bool INS60kts, CheckUFC;
        public float INSLatOffset, INSLongOffset, INSAltOffset, INSHDGOffset;
        public int INSDriftLatDirection, INSDriftLongDirection;
        public float INSDriftDirectionTimer;
        public float BUPADIEnergy;
        public bool GSValid, LOCValid;

        //MI more realistic AVTR
        public AVTRStateFlags AVTRFlags;
        public float AVTRCountown;
        public bool doAVTRCountdown;

        public void AddAVTRSeconds()
        {
            throw new NotImplementedException();
        }

        [Flags]
        public enum AVTRStateFlags
        {
            AVTR_ON = 0x1,
            AVTR_AUTO = 0x2,
            AVTR_OFF = 0x4,
        };
        public void AVTROn(AVTRStateFlags fl)
        {
            AVTRFlags |= fl;
        }

        public void AVTROff(AVTRStateFlags fl)
        {
            AVTRFlags &= ~fl;
        }

        public int AVTRState(AVTRStateFlags fl)
        {
            return (AVTRFlags & fl) == (AVTRStateFlags)fl ? 1 : 0;
        }


        //MI Mal and Ind Lights test button
        public bool TestLights;
        //MI some other stuff
        public bool TrimAPDisc;
        public bool TEFExtend;
        public bool CanopyDamaged;
        public bool LEFLocked;
        public float LTLEFAOA, RTLEFAOA;
        public LEFStateFlags LEFFlags;
        public float leftLEFAngle;
        public float rightLEFAngle;

        [Flags]
        public enum LEFStateFlags
        {
            LT_LEF_DAMAGED = 0x1,
            LT_LEF_OUT = 0x2,
            RT_LEF_DAMAGED = 0x8,
            RT_LEF_OUT = 0x10,
            LEFSASYNCH = 0x20,
        };
        public void LEFOn(LEFStateFlags fl)
        {
            LEFFlags |= fl;
        }

        public void LEFOff(LEFStateFlags fl)
        {
            LEFFlags &= ~fl;
        }

        public int LEFState(LEFStateFlags fl)
        {
            return (LEFFlags & fl) == (LEFStateFlags)fl ? 1 : 0;
        }

        public float CheckLEF(int side)
        {
            throw new NotImplementedException();
        }

        public int MissileVolume;
        public int ThreatVolume;
        public bool GunFire;
        //targeting pod cooling time
        public float PodCooling;
        public bool CockpitWingLight;
        public bool CockpitStrobeLight;

        public void SetCockpitWingLight(bool state)
        {
            throw new NotImplementedException();
        }

        public void SetCockpitStrobeLight(bool state)
        {
            throw new NotImplementedException();
        }

        public AircraftClass(int flag, byte[] stream)
        {
            throw new NotImplementedException();
        }

        public AircraftClass(int flag, FileStream filePtr)
        {
            throw new NotImplementedException();
        }

        public AircraftClass(int flag, int type)
        {
            throw new NotImplementedException();
        }
        //TODO public virtual ~AircraftClass		();

        public float glocFactor;
        public int fireGun, fireMissile, lastPickle;
        public FackClass mFaults;
        public AirframeClass af;
        public FireControlComputer FCC;
        public SMSClass Sms;
        public GunClass Guns;

        public virtual void Init(SimInitDataClass initData)
        {
            throw new NotImplementedException();
        }

        public virtual int Wake()
        {
            throw new NotImplementedException();
        }

        public virtual int Sleep()
        {
            throw new NotImplementedException();
        }

        public virtual int Exec()
        {
            throw new NotImplementedException();
        }

        public virtual void JoinFlight()
        {
            throw new NotImplementedException();
        }

        public virtual void InitData(int i)
        {
            throw new NotImplementedException();
        }

        public virtual void Cleanup()
        {
            throw new NotImplementedException();
        }

        public virtual int CombatClass()
        {
            throw new NotImplementedException();
        } // 2002-02-25 MODIFIED BY S.G. virtual added in front since FlightClass also have one now...
        public void SetAutopilot(AutoPilotType flag)
        {
            throw new NotImplementedException();
        }

        public AutoPilotType AutopilotType()
        {
            return autopilotType;
        }

        public VU_ID HomeAirbase()
        {
            throw new NotImplementedException();
        }

        public VU_ID TakeoffAirbase()
        {
            throw new NotImplementedException();
        }

        public VU_ID LandingAirbase()
        {
            throw new NotImplementedException();
        }

        public VU_ID DivertAirbase()
        {
            throw new NotImplementedException();
        }

        public void DropProgramed()
        {
            throw new NotImplementedException();
        }

        public bool IsF16()
        {
            return (acFlags & ACFLAGS.isF16) == ACFLAGS.isF16 ? true : false;
        }

        public bool IsComplex()
        {
            return ((acFlags & ACFLAGS.isComplex) == ACFLAGS.isComplex ? true : false);
        }
        // 2000-11-17 ADDED BY S.G. SO AIRCRAFT CAN HAVE A ENGINE TEMPERATURE AS WELL AS 'POWER' (RPM) OUTPUT   
        public void SetPowerOutput(float powerOutput)
        {
            throw new NotImplementedException();
        }//me123 changed back
        // END OF ADDED SECTION	
        public virtual void SetVt(float newvt)
        {
            throw new NotImplementedException();
        }

        public virtual void SetKias(float newkias)
        {
            throw new NotImplementedException();
        }

        public void ResetFuel()
        {
            throw new NotImplementedException();
        }

        public virtual long GetTotalFuel()
        {
            throw new NotImplementedException();
        }

        public virtual void GetTransform(TransformMatrix vmat)
        {
            throw new NotImplementedException();
        }

        public virtual void ApplyDamage(FalconDamageMessage damageMessage)
        {
            throw new NotImplementedException();
        }

        public virtual void SetLead(int flag)
        {
            throw new NotImplementedException();
        }

        public virtual void ReceiveOrders(FalconEvent newOrder)
        {
            throw new NotImplementedException();
        }

        public virtual float GetP()
        {
            throw new NotImplementedException();
        }

        public virtual float GetQ()
        {
            throw new NotImplementedException();
        }

        public virtual float GetR()
        {
            throw new NotImplementedException();
        }

        public virtual float GetAlpha()
        {
            throw new NotImplementedException();
        }

        public virtual float GetBeta()
        {
            throw new NotImplementedException();
        }

        public virtual float GetNx()
        {
            throw new NotImplementedException();
        }

        public virtual float GetNy()
        {
            throw new NotImplementedException();
        }

        public virtual float GetNz()
        {
            throw new NotImplementedException();
        }

        public virtual float GetGamma()
        {
            throw new NotImplementedException();
        }

        public virtual float GetSigma()
        {
            throw new NotImplementedException();
        }

        public virtual float GetMu()
        {
            throw new NotImplementedException();
        }

        public virtual void MakePlayerVehicle()
        {
            throw new NotImplementedException();
        }

        public virtual void MakeNonPlayerVehicle()
        {
            throw new NotImplementedException();
        }

        public virtual void MakeLocal()
        {
            throw new NotImplementedException();
        }

        public virtual void MakeRemote()
        {
            throw new NotImplementedException();
        }

        public virtual void ConfigurePlayerAvionics()
        {
            throw new NotImplementedException();
        }

        public virtual void SetVuPosition()
        {
            throw new NotImplementedException();
        }

        public virtual void Regenerate(float x, float y, float z, float yaw)
        {
            throw new NotImplementedException();
        }

        public virtual FireControlComputer GetFCC()
        {
            return FCC;
        }

        public virtual SMSBaseClass GetSMS()
        {
            return (SMSBaseClass)Sms;
        }

        public virtual int HasSPJamming()
        {
            throw new NotImplementedException();
        }

        public virtual int HasAreaJamming()
        {
            throw new NotImplementedException();
        }

#if _DEBUG
		public virtual void	SetDead (int);
#endif // _DEBUG

        // private:

        public long mCautionCheckTime;
        public bool MPOCmd;
        public char dropChaffCmd;
        public char dropFlareCmd;
        public ACFLAGS acFlags;
        public AutoPilotType autopilotType;
        public AutoPilotType lastapType;
        public VU_TIME dropProgrammedTimer;
        public ushort dropProgrammedStep;
        public bool isDigital;
        public float bingoFuel;
        //MI taking these functions for the ICP stuff, made some changes
        public float GetBingoFuel()
        {
            return bingoFuel;
        }//me123
        public void SetBingoFuel(float newbingo)
        {
            bingoFuel = newbingo;
        }//me123

        private Random rand = new Random();
        //MI make some noise when overstressing
        public void DamageSounds()
        {
            int sound = rand.Next(0, 4); // rand() % 5;
            sound++;
            switch (sound)
            {
                case 1:
                    FSound.F4SoundFXSetDist(SFX_TYPES.SFX_HIT_5, false, 0.0f, 1.0f);
                    break;
                case 2:
                    FSound.F4SoundFXSetDist(SFX_TYPES.SFX_HIT_4, false, 0.0f, 1.0f);
                    break;
                case 3:
                    FSound.F4SoundFXSetDist(SFX_TYPES.SFX_HIT_3, false, 0.0f, 1.0f);
                    break;
                case 4:
                    FSound.F4SoundFXSetDist(SFX_TYPES.SFX_HIT_2, false, 0.0f, 1.0f);
                    break;
                case 5:
                    FSound.F4SoundFXSetDist(SFX_TYPES.SFX_HIT_1, false, 0.0f, 1.0f);
                    break;
                default:
                    break;
            }
        }

        public uint SpeedSoundsWFuel;
        public uint SpeedSoundsNFuel;
        public uint GSoundsWFuel;
        public uint GSoundsNFuel;

        public void WrongCAT()
        {
            throw new NotImplementedException();
        }

        public void CorrectCAT()
        {
            throw new NotImplementedException();
        }
        //MI for RALT stuff
        public enum RaltStatus
        {
            ROFF,
            RSTANDBY,
            RON
        };
        public RaltStatus RALTStatus;
        public float RALTCoolTime;	//Cooling is in progress
        public int RaltReady()
        {
            return (RALTCoolTime < 0.0F && RALTStatus == RaltStatus.RON) ? 1 : 0;
        }

        public void RaltOn()
        {
            RALTStatus = RaltStatus.RON;
        }

        public void RaltStdby()
        {
            RALTStatus = RaltStatus.RSTANDBY;
        }

        public void RaltOff()
        {
            RALTStatus = RaltStatus.ROFF;
        }
        //MI for EWS stuff
        public void DropEWS()
        {
            throw new NotImplementedException();
        }

        public void EWSChaffBurst()
        {
            throw new NotImplementedException();
        }

        public void EWSFlareBurst()
        {
            throw new NotImplementedException();
        }

        public void ReleaseManualProgram()
        {
            throw new NotImplementedException();
        }

        public bool ManualECM;
        public int FlareCount, ChaffCount, ChaffSalvoCount, FlareSalvoCount;
        public VU_TIME ChaffBurstInterval, FlareBurstInterval, ChaffSalvoInterval, FlareSalvoInterval;
        //MI Autopilot
        public enum APFlags
        {
            AltHold = 0x1, //Right switch up
            AttHold = 0x2, //Right Switch down
            StrgSel = 0x4,	//Left switch down
            RollHold = 0x8, //Left switch middle
            HDGSel = 0x10,	//Left switch up
            Override = 0x20
        };	//Paddle switch
        public APFlags APFlag;

        public bool IsOn(APFlags flag)
        {
            return (APFlag & flag) == flag;
        }

        public void SetAPFlag(APFlags flag)
        {
            APFlag |= flag;
        }

        public void ClearAPFlag(APFlags flag)
        {
            APFlag &= ~flag;
        }

        public void SetAPParameters();

        public void SetNewRoll();

        public void SetNewPitch();

        public void SetNewAlt();
        //MI seatArm
        public bool SeatArmed;

        public void StepSeatArm();

        public TransformMatrix vmat;
        public float gLoadSeconds;
        public long lastStatus;
        public BasicWeaponStation[] counterMeasureStation = new BasicWeaponStation[3];
        public enum TRAIL_ENUM
        { // what trail is used for what
            TRAIL_DAMAGE = 0, // we've been hit
            TRAIL_ENGINE1,
            TRAIL_ENGINE2,
            TRAIL_MAX,
            MAXENGINES = 4,

        };
        public DrawableTrail[] smokeTrail = new DrawableTrail[(int)TRAIL_ENUM.TRAIL_MAX];
        public DrawableTrail[] conTrails = new DrawableTrail[(int)TRAIL_ENUM.MAXENGINES];
        public DrawableTrail[] engineTrails = new DrawableTrail[(int)TRAIL_ENUM.MAXENGINES];
        public DrawableTrail rwingvortex, lwingvortex;
        public bool playerSmokeOn;
        public DrawableGroundVehicle pLandLitePool;
        public bool mInhibitLitePool;

        public void CleanupLitePool();

        public void AddEngineTrails(int ttype, DrawableTrail[] tlist)
{
	// 2002-02-16 ADDED BY S.G. It's been seen that drawPointer is NULL here and &((DrawableBSP*)drawPointer).orientation is simply drawPointer+0x2C hence why orientation is never NULL
	if (!drawPointer)
		return;

	Tpoint pos;
	Trotation *orientation = &((DrawableBSP*)drawPointer).orientation;

	Debug.Assert(orientation);
	if (!orientation)
		return;

	int nEngines = min(MAXENGINES, af.auxaeroData.nEngines);
	for(int i = 0; i < nEngines; i++) {
		if(tlist[i] == NULL) {
			tlist[i] = new DrawableTrail(ttype);
			OTWDriver.InsertObject(tlist[i]);
		}
		Tpoint *tp = &af.auxaeroData.engineLocation[i];

        Debug.Assert(tp);
		if (!tp)
			return;

		pos.x = orientation.M11*tp.x + orientation.M12*tp.y + orientation.M13*tp.z;
		pos.y = orientation.M21*tp.x + orientation.M22*tp.y + orientation.M23*tp.z;
		pos.z = orientation.M31*tp.x + orientation.M32*tp.y + orientation.M33*tp.z;

		pos.x += XPos();
		pos.y += YPos();
		pos.z += ZPos();

		OTWDriver.AddTrailHead (tlist[i], pos.x, pos.y, pos.z );
		tlist[i].KeepStaleSegs (flag_keep_smoke_trails);
	}
}

        public void CancelEngineTrails(DrawableTrail[] tlist)
        {
            int nEngines = min(MAXENGINES, af.auxaeroData.nEngines);
            for (int i = 0; i < nEngines; i++)
            {
                if (tlist[i])
                {
                    OTWDriver.AddSfxRequest(
                    new SfxClass(
                    21.2f,							// time to live
                    tlist[i]));		// scale
                    tlist[i] = NULL;
                }
            }
        }

        // JPO Avionics power settings;
        public AvionicsPowerFlags powerFlags;

        public void PowerOn(AvionicsPowerFlags fl)
        {
            powerFlags |= fl;
        }

        public int HasPower(AvionicsPowerFlags fl);

        public void PowerOff(AvionicsPowerFlags fl)
        {
            powerFlags &= ~fl;
        }

        public void PowerToggle(AvionicsPowerFlags fl)
        {
            powerFlags ^= fl;
        }

        public int PowerSwitchOn(AvionicsPowerFlags fl)
        {
            return (powerFlags & fl) ? true : false;
        }

        public void PreFlight(); // JPO - do preflight checks.

        // JPPO Main Power
        public MainPowerType mainPower;

        public MainPowerType MainPower()
        {
            return mainPower;
        }

        public bool MainPowerOn()
        {
            return mainPower == MainPowerMain;
        }

        public void SetMainPower(MainPowerType t)
        {
            mainPower = t;
        }

        public void IncMainPower();

        public void DecMainPower();

        public PowerStates currentPower;
        public enum ElectricLights
        {
            ElecNone = 0x0,
            ElecFlcsPmg = 0x1,
            ElecEpuGen = 0x2,
            ElecEpuPmg = 0x4,
            ElecToFlcs = 0x8,
            ElecFlcsRly = 0x10,
            ElecBatteryFail = 0x20,
        };
        public ElectricLights elecLights;

        public bool ElecIsSet(ElectricLights lt)
        {
            return (elecLights & lt) ? true : false;
        }

        public void ElecSet(ElectricLights lt)
        {
            elecLights |= lt;
        }

        public void ElecClear(ElectricLights lt)
        {
            elecLights &= ~lt;
        }

        public void DoElectrics();

        public const ulong[] systemStates = new ulong[PowerMaxState];

        //MI EWS PGM Switch
        public EWSPGMSwitch EWSPgm;

        public EWSPGMSwitch EWSPGM()
        {
            return EWSPgm;
        }

        public void SetPGM(EWSPGMSwitch t)
        {
            EWSPgm = t;
        }

        public void IncEWSPGM();

        public void DecEWSPGM();

        public void DecEWSProg();

        public void IncEWSProg();
        //Prog select switch
        public uint EWSProgNum;
        //MI caution stuff
        public bool NeedsToPlayCaution;
        public bool NeedsToPlayWarning;
        public VU_TIME WhenToPlayCaution;
        public VU_TIME WhenToPlayWarning;

        public void SetExternalData();
        //MI Inhibit VMS Switch
        public bool playBetty;

        public void ToggleBetty();
        //MI RF switch
        public int RFState;	//0 = NORM; 1 = QUIET; 2 = SILENT
        public void GSounds();

        public void SSounds();

        public int SpeedToleranceTanks;
        public int SpeedToleranceBombs;
        public float GToleranceTanks;
        public float GToleranceBombs;
        public int[] OverSpeedToleranceTanks = new int[3];	//3 levels of OverSpeed for tanks
        public int[] OverSpeedToleranceBombs = new int[3]; //3 levels of OverSpeed for bombs
        public int[] OverGToleranceTanks = new int[3];	//3 levels of OverG for tanks
        public int[] OverGToleranceBombs = new int[3]; //3 levels of OverG for bombs
        public void AdjustTankSpeed(int level);

        public void AdjustBombSpeed(int level);

        public void AdjustTankG(int level);

        public void AdjustBombG(int level);

        public void DoWeapons();

        public float GlocPrediction();

        public void InitCountermeasures();

        public void CleanupCountermeasures();

        public void InitDamageStation();

        public void CleanupDamageStation();

        public void DoCountermeasures();

        public void DropChaff();

        public void DropFlare();

        public void GatherInputs();

        public void RunSensors();

        public bool LandingCheck(float noseAngle, float impactAngle, int groundType);

        public void GroundFeatureCheck(float groundZ);

        public void RunExplosion();

        public void ShowDamage()
        {
#if TODO
            BOOL hasMilSmoke = false;
            float radius;
            Tpoint pos, rearOffset;

            // handle case when moving slow and/or on ground -- no trails
            if (Vt() < 40.0f)
            {
                for (int i = 0; i < TRAIL_MAX; i++)
                {
                    if (smokeTrail[i])
                    {
                        OTWDriver.AddSfxRequest(
                        new SfxClass(
                        15.2f,							// time to live
                        smokeTrail[i]));		// scale
                        smokeTrail[i] = NULL;
                    }
                }
                CancelEngineTrails(conTrails);
                CancelEngineTrails(engineTrails);

                // no do puffy smoke if damaged
                if (pctStrength < 0.50f)
                {
                    rearOffset.x = PRANDFloat() * 20.0f;
                    rearOffset.y = PRANDFloat() * 20.0f;
                    rearOffset.z = -PRANDFloatPos() * 20.0f;

                    pos.x = XPos();
                    pos.y = YPos();
                    pos.z = ZPos();

                    OTWDriver.AddSfxRequest(
                            new SfxClass(SFX_TRAILSMOKE,			// type
                            SFX_MOVES | SFX_NO_GROUND_CHECK,						// flags
                            &pos,							// world pos
                            &rearOffset,							// vector
                            3.5f,							// time to live
                            4.5f));		// scale
                }
                return;
            }

            // mig29's, f4's and f5's all smoke when at MIL power
            // it's not damage, but since we handle the smoke here anyway.....

            if (!OnGround() &&
                af.EngineTrail() >= 0)
            {
                if (OTWDriver.renderer && OTWDriver.renderer.GetAlphaMode())
                    hasMilSmoke = af.EngineTrail();
            }

            // get rear offset behind the plane
            if (drawPointer)
                radius = drawPointer.Radius();
            else
                radius = 25.0f;
            rearOffset.x = -dmx[0][0] * radius;
            rearOffset.y = -dmx[0][1] * radius;
            rearOffset.z = -dmx[0][2] * radius;

            // check for contrail alt
            // also this can be toggled by the player
            if ((ZPos() * -0.001F > ((WeatherClass*)TheWeather).TodaysConLow &&
                 ZPos() * -0.001F < ((WeatherClass*)TheWeather).TodaysConHigh) || playerSmokeOn)
            {
                AddEngineTrails(TRAIL_CONTRAIL, conTrails); // Whitish smoke
            }
            else
            {
                CancelEngineTrails(conTrails);
            }

            // JPO - maybe some wing tip vortexes
            if (af.auxaeroData.wingTipLocation.y != 0 &&
                OTWDriver.renderer && OTWDriver.renderer.GetAlphaMode() &&
                -ZPos() > minwingvortexalt && -ZPos() < maxwingvortexalt &&
                af.alpha > wingvortexalpha && af.nzcgb > wingvortexgs)
            {
                Tpoint pos;
                Trotation* orientation = &((DrawableBSP*)drawPointer).orientation;

                Debug.Assert(orientation);
                if (orientation)
                {
                    if (lwingvortex == NULL)
                    {
                        lwingvortex = new DrawableTrail(TRAIL_WINGTIPVTX);
                        OTWDriver.InsertObject(lwingvortex);
                    }

                    Tpoint* tp = &af.auxaeroData.wingTipLocation;
                    pos.x = orientation.M11 * tp.x + orientation.M12 * tp.y + orientation.M13 * tp.z;
                    pos.y = orientation.M21 * tp.x + orientation.M22 * tp.y + orientation.M23 * tp.z;
                    pos.z = orientation.M31 * tp.x + orientation.M32 * tp.y + orientation.M33 * tp.z;

                    pos.x += XPos();
                    pos.y += YPos();
                    pos.z += ZPos();

                    OTWDriver.AddTrailHead(lwingvortex, pos.x, pos.y, pos.z);

                    if (rwingvortex == NULL)
                    {
                        rwingvortex = new DrawableTrail(TRAIL_WINGTIPVTX);
                        OTWDriver.InsertObject(rwingvortex);
                    }

                    tp = &af.auxaeroData.wingTipLocation;
                    pos.x = orientation.M11 * tp.x + orientation.M12 * -tp.y + orientation.M13 * tp.z;
                    pos.y = orientation.M21 * tp.x + orientation.M22 * -tp.y + orientation.M23 * tp.z;
                    pos.z = orientation.M31 * tp.x + orientation.M32 * -tp.y + orientation.M33 * tp.z;

                    pos.x += XPos();
                    pos.y += YPos();
                    pos.z += ZPos();

                    OTWDriver.AddTrailHead(rwingvortex, pos.x, pos.y, pos.z);
                }
            }
            else
            {
                if (lwingvortex)
                {
                    OTWDriver.AddSfxRequest(
                    new SfxClass(
                    21.2f,			// time to live
                    lwingvortex));		// scale
                    lwingvortex = NULL;
                }

                if (rwingvortex)
                {
                    OTWDriver.AddSfxRequest(
                    new SfxClass(
                    21.2f,			// time to live
                    rwingvortex));		// scale
                    rwingvortex = NULL;
                }
            }

            // do military power smoke if its that type of craft
            // PowerOutput() runs from 0.7 (flight idle) to 1.5 (max ab) with 1.0 being mil power
            // M.N. added Poweroutput > 0.65, stops smoke trails when engine is shut down.
            if (!OnGround() && af.EngineTrail() >= 0 && OTWDriver.renderer && OTWDriver.renderer.GetAlphaMode())
            {
                if (PowerOutput() <= 1.0f && PowerOutput() > 0.65f)
                {
                    AddEngineTrails(af.EngineTrail(), engineTrails); // smoke
                }
                else CancelEngineTrails(engineTrails);
            }
            if (pctStrength > 0.50f)
            {
                if (!hasMilSmoke && smokeTrail[TRAIL_DAMAGE] && smokeTrail[TRAIL_DAMAGE].InDisplayList())
                {
                    OTWDriver.AddSfxRequest(
                    new SfxClass(
                    11.2f,							// time to live
                    smokeTrail[TRAIL_DAMAGE]));		// scale
                    smokeTrail[TRAIL_DAMAGE] = NULL;
                }

                return;
            }

            // if we're dying, just maitain the status quo...
            if (pctStrength < 0.0f)
            {
                radius = 3.0f;

                if (smokeTrail[TRAIL_DAMAGE])
                {
                    pos.x = XPos() + rearOffset.x;
                    pos.y = YPos() + rearOffset.y;
                    pos.z = ZPos() + rearOffset.z;
                    OTWDriver.AddTrailHead(smokeTrail[TRAIL_DAMAGE], pos.x, pos.y, pos.z);
                }

                if (smokeTrail[TRAIL_ENGINE1])
                {
                    pos.x = dmx[1][0] * radius + XPos() + rearOffset.x * 0.75f;
                    pos.y = dmx[1][1] * radius + YPos() + rearOffset.y * 0.75f;
                    pos.z = dmx[1][2] * radius + ZPos() + rearOffset.z * 0.75f;
                    OTWDriver.AddTrailHead(smokeTrail[TRAIL_ENGINE1], pos.x, pos.y, pos.z);
                }

                if (smokeTrail[TRAIL_ENGINE2])
                {
                    pos.x = -dmx[1][0] * radius + XPos() + rearOffset.x * 0.75f;
                    pos.y = -dmx[1][1] * radius + YPos() + rearOffset.y * 0.75f;
                    pos.z = -dmx[1][2] * radius + ZPos() + rearOffset.z * 0.75f;
                    OTWDriver.AddTrailHead(smokeTrail[TRAIL_ENGINE2], pos.x, pos.y, pos.z);
                }
                return;
            }
            // at this point we know we've got enough damage for 1 trail in
            // the center
            if (!smokeTrail[TRAIL_DAMAGE])
            {
                smokeTrail[TRAIL_DAMAGE] = new DrawableTrail(TRAIL_SMOKE);	// smoke
                OTWDriver.InsertObject(smokeTrail[TRAIL_DAMAGE]);
            }

            pos.x = XPos() + rearOffset.x;
            pos.y = YPos() + rearOffset.y;
            pos.z = ZPos() + rearOffset.z;
            OTWDriver.AddTrailHead(smokeTrail[TRAIL_DAMAGE], pos.x, pos.y, pos.z);

            // occasionalyy add a smoke cloud
            /*
            if ( sfxTimer > 0.2f && (rand() & 0x3) == 0x3 )
            {
                sfxTimer = 0.0f;
                pos.x = XPos();
                pos.y = YPos();
                pos.z = ZPos();
                OTWDriver.AddSfxRequest(
                     new SfxClass (SFX_TRAILSMOKE,				// type
                     &pos,							// world pos
                     2.5f,							// time to live
                     2.0f ) );		// scale
            }
            */

            // now test for additional damage and add smoke trails on the left and right
            if (pctStrength < 0.35f)
            {
                radius = 3.0f;

                pos.x = dmx[1][0] * radius + XPos() + rearOffset.x * 0.75f;
                pos.y = dmx[1][1] * radius + YPos() + rearOffset.y * 0.75f;
                pos.z = dmx[1][2] * radius + ZPos() + rearOffset.z * 0.75f;

                if (!smokeTrail[TRAIL_ENGINE1])
                {
                    smokeTrail[TRAIL_ENGINE1] = new DrawableTrail(TRAIL_SMOKE);	// smoke
                    OTWDriver.InsertObject(smokeTrail[TRAIL_ENGINE1]);
                }
                OTWDriver.AddTrailHead(smokeTrail[TRAIL_ENGINE1], pos.x, pos.y, pos.z);

                // occasionalyy add a smoke cloud
                /*
                if ( sfxTimer > 0.2f && (rand() & 0x3) == 0x3 )
                {
                    sfxTimer = 0.0f;
                    OTWDriver.AddSfxRequest(
                         new SfxClass (SFX_TRAILSMOKE,				// type
                         &pos,							// world pos
                         2.5f,							// time to live
                         2.0f ) );		// scale
                }
                */
            }

            if (pctStrength < 0.15f)
            {
                pos.x = -dmx[1][0] * radius + XPos() + rearOffset.x * 0.75f;
                pos.y = -dmx[1][1] * radius + YPos() + rearOffset.y * 0.75f;
                pos.z = -dmx[1][2] * radius + ZPos() + rearOffset.z * 0.75f;

                if (!smokeTrail[TRAIL_ENGINE2])
                {
                    smokeTrail[TRAIL_ENGINE2] = new DrawableTrail(TRAIL_SMOKE);	// smoke
                    OTWDriver.InsertObject(smokeTrail[TRAIL_ENGINE2]);
                }
                OTWDriver.AddTrailHead(smokeTrail[TRAIL_ENGINE2], pos.x, pos.y, pos.z);

                // occasionalyy add a smoke cloud
                /*
                if ( sfxTimer > 0.2f && (rand() & 0x3) == 0x3 )
                {
                    sfxTimer = 0.0f;
                    OTWDriver.AddSfxRequest(
                         new SfxClass (SFX_TRAILSMOKE,				// type
                         &pos,							// world pos
                         2.5f,							// time to live
                         2.0f ) );		// scale
                }
                */
            }

            // occasionally, perturb the controls
            // JB 010104
            //if ( pctStrength < 0.5f && (rand() & 0xf) == 0xf)
            if (!g_bDisableFunkyChicken && pctStrength < 0.5f && (rand() & 0xf) == 0xf)
            // JB 010104
            {
                ioPerturb = 0.5f + (1.0f - pctStrength);
                // good place also to stick in a damage, clunky sound....
            }
#endif 
            throw new NotImplementedException();
        }


        public void CleanupDamage()
        {
            for (int i = 0; i < TRAIL_MAX; i++)
            {
                if (smokeTrail[i] && smokeTrail[i].InDisplayList())
                {
                    OTWDriver.AddSfxRequest(
                        new SfxClass(
                        11.2f,							// time to live
                        smokeTrail[i]));		// scale
                    smokeTrail[i] = NULL;
                }
            }
            return;
        }



        public void MoveSurfaces();

        public void RunGear();

        public void ToggleAutopilot();

        public void OnGroundInit(SimInitDataClass initData);

        public void CheckObjectCollision()
            {
	SimObjectType *obj;
	SimBaseClass *theObject;
	SimObjectLocalData *objData;
	FalconDamageMessage *message;
	bool setOnObject = false; // JB carrier

	// no detection on ground when not moving
	if ( 
		!af.IsSet(AirframeClass.OnObject) && // JB carrier
		OnGround() && af.vt == 0.0f )
	{
		return;
	}

	if (
		!af.IsSet(AirframeClass.OnObject) && // JB carrier
		OnGround() && af.vcas <= 50.0f  && gCommsMgr && gCommsMgr.Online()) // JB 010107
	{
		return; // JB 010107
	}

	// loop thru all targets
    for( obj = targetList; obj; obj = obj.next )
    {
      	objData = obj.localData;
		   theObject = (SimBaseClass*)obj.BaseData();

		// Throw out inappropriate collision partners
		// edg: allow collisions with ground objects
      	// if ( theObject == NULL || theObject.OnGround() || this == theObject.Parent() )
      	if (theObject == NULL ||
					F4IsBadReadPtr(theObject, sizeof(SimBaseClass)) || // JB 010305 CTD
			(theObject.IsWeapon() && this == ((SimWeaponClass*)theObject).Parent()) ||	//	. Why would a missile's parent be in it's target list?
                                                                  // It typically wouldn't, but then, this is not the missiles target list.
			!theObject.IsSim() ||
			(OnGround() && !theObject.OnGround()) ||
			theObject.drawPointer == NULL )
      	{
         	continue;
      	}

		// Stop now if the spheres don't overlap
		// special case the tanker -- we want to be able to get in closer
		if ( 
			af.IsSet(AirframeClass.OnObject) || // JB carrier
			!OnGround() )
		{
			if ( IsSetFlag( I_AM_A_TANKER ) || theObject.IsSetFlag( I_AM_A_TANKER ) )
			{
				/*
				if ( objData.range >  0.1f * theObject.drawPointer.Radius()){// + drawPointer.Radius() ) // PJW
					continue;
				}
				*/
				
				if ( objData.range > 0.2F * theObject.drawPointer.Radius() + drawPointer.Radius() ) 
					continue;

				Tpoint org, vec, pos;

				org.x = XPos();
				org.y = YPos();
				org.z = ZPos();
				vec.x = XDelta() - theObject.XDelta();
				vec.y = YDelta() - theObject.YDelta();
				vec.z = ZDelta() - theObject.ZDelta();
				// we're within the range of the object's radius.
				// for ships, this may be a BIG radius -- longer than
				// high.  Let's also check the bounding box
				// scale the box so we might possibly miss it
				if ( !theObject.drawPointer.GetRayHit( &org, &vec, &pos, 0.1f ) )
					continue;
				

 			} else
			{
				if ( objData.range > theObject.drawPointer.Radius() + drawPointer.Radius())
				{
					continue;
				}
				//else if ( theObject.OnGround() )
				//{
					Tpoint org, vec, pos;

					org.x = XPos();
					org.y = YPos();
					if (af.IsSet(AirframeClass.OnObject)) // JB carrier
						org.z = ZPos() + 5; // Make sure its detecting that we are on top // JB carrier
					else // JB carrier
						org.z = ZPos();
					vec.x = XDelta() - theObject.XDelta();
					vec.y = YDelta() - theObject.YDelta();
					vec.z = ZDelta() - theObject.ZDelta();
					// we're within the range of the object's radius.
					// for ships, this may be a BIG radius -- longer than
					// high.  Let's also check the bounding box
				    if ( !theObject.drawPointer.GetRayHit( &org, &vec, &pos, 1.0f ) )
						continue;
				//}
			}
		}
		else // on ground
		{
			if ( objData.range > theObject.drawPointer.Radius()){// + drawPointer.Radius() ) { // PJW
					continue;
			}
		}

		// Don't collide ejecting pilots with their aircraft
		if (theObject.IsEject())
		{
			if (((EjectedPilotClass*)theObject).GetParentAircraft() == this)
				continue;
		}

		//***********************************************
		// If we get here, we've decided we've collided!
		//***********************************************

		// JB carrier start
		if (IsAirplane() && theObject.GetType() == TYPE_CAPITAL_SHIP)
		{
			Tpoint minB; Tpoint maxB;
			((DrawableBSP*) theObject.drawPointer).GetBoundingBox(&minB, &maxB);
			
			// JB 010731 Hack for unfixed hitboxes.
			if (minB.z < -193 && minB.z > -194)
				minB.z = -72;

			if((ZPos() <= minB.z * .96 && ZPos() > minB.z * 1.02) || (ZPos() > -g_fCarrierStartTolerance && af.vcas < .01))
			{	
				// the eagle has landed
				attachedEntity = theObject;
				af.SetFlag(AirframeClass.OnObject);
				af.SetFlag(AirframeClass.OverRunway);
				setOnObject = true;
				
				// Set our anchor so that when we're moving slowly we can accumulate our position in high precision
				af.groundAnchorX = af.x;
				af.groundAnchorY = af.y;
				af.groundDeltaX = 0.0f;
				af.groundDeltaY = 0.0f;
				af.platform.SetFlag( ON_GROUND );
				
				float gndGmma, relMu;
				
				af.CalculateGroundPlane(&gndGmma, &relMu);
				
				af.stallMode = AirframeClass.None;
				af.slice = 0.0F;
				af.pitch = 0.0F;

				if( af.IsSet(af.GearBroken) || af.gearPos <= 0.1F )
				{
					if ( af.platform.DBrain() && !af.platform.IsSetFalcFlag(FEC_INVULNERABLE))
					{
						af.platform.DBrain().SetATCFlag(DigitalBrain.Landed);
						af.platform.DBrain().SetATCStatus(lCrashed);
						// KCK NOTE. Don't set timer for players
						if(af.platform != SimDriver.playerEntity)
							af.platform.DBrain().SetWaitTimer(SimLibElapsedTime + 1 * CampaignMinutes);
					}
				}

				Tpoint velocity;
				Tpoint noseDir;
				float impactAngle, noseAngle;
				float tmp;

				velocity.x = af.xdot/vt;
				velocity.y = af.ydot/vt;
				velocity.z = af.zdot/vt;
				
				noseDir.x = af.platform.platformAngles.costhe * af.platform.platformAngles.cospsi;
				noseDir.y = af.platform.platformAngles.costhe * af.platform.platformAngles.sinpsi;
				noseDir.z = -af.platform.platformAngles.sinthe;
				tmp = (float)sqrt(noseDir.x*noseDir.x + noseDir.y*noseDir.y + noseDir.z*noseDir.z);
				noseDir.x /= tmp;
				noseDir.y /= tmp;
				noseDir.z /= tmp;
				
				noseAngle = af.gndNormal.x*noseDir.x + af.gndNormal.y*noseDir.y + af.gndNormal.z*noseDir.z;
				impactAngle = af.gndNormal.x*velocity.x + af.gndNormal.y*velocity.y + af.gndNormal.z*velocity.z;
				
				impactAngle = (float)fabs(impactAngle);

				if (ZPos() > -g_fCarrierStartTolerance)
				{
					// We just started inside the carrier
					SetAutopilot(APOff);
					af.onObjectHeight = minB.z;
					noseAngle = 0;
					impactAngle = 0;
				}
				else
					af.onObjectHeight = ZPos();

				if (vt == 0.0)
					continue;

				// do the landing check (no damage)
				if (!af.IsSet(AirframeClass.InAir) || af.platform.LandingCheck( noseAngle, impactAngle, COVERAGE_OBJECT) )
				{
					af.ClearFlag (AirframeClass.InAir);
					continue;
				}
			}
			else if(ZPos() <= minB.z * .96)
				continue;
		}

		if (isDigital || !PlayerOptions.CollisionsOn())
			continue;
		// JB carrier end

// 2002-04-17 MN fix for killer chaff / flare
		if (theObject.GetType() == TYPE_BOMB && 
			(theObject.GetSType() == STYPE_CHAFF || theObject.GetSType() == STYPE_FLARE1) &&
			(theObject.GetSPType() == SPTYPE_CHAFF1 || theObject.GetSPType() == SPTYPE_CHAFF1+1))
			continue;
		
		if (!isDigital)
			g_intellivibeData.CollisionCounter++;

		// send message to self
		// VuTargetEntity *owner_session = (VuTargetEntity*)vuDatabase.Find(OwnerId());
		message = new FalconDamageMessage (Id(), FalconLocalGame);
		message.dataBlock.fEntityID  = theObject.Id();

        message.dataBlock.fCampID    = theObject.GetCampID();
        message.dataBlock.fSide      = theObject.GetCountry();

		if (theObject.IsAirplane())
		   message.dataBlock.fPilotID   = ((SimMoverClass*)theObject).pilotSlot;
		else
		   message.dataBlock.fPilotID   = 255;
		message.dataBlock.fIndex     = theObject.Type();
		message.dataBlock.fWeaponID  = theObject.Type();
		message.dataBlock.fWeaponUID = theObject.Id();

		message.dataBlock.dEntityID  = Id();
		Debug.Assert(GetCampaignObject());
		message.dataBlock.dCampID = GetCampID();
		message.dataBlock.dSide   = GetCountry();
		if (IsAirplane())
		   message.dataBlock.dPilotID   = pilotSlot;
		else
		   message.dataBlock.dPilotID   = 255;
		message.dataBlock.dIndex     = Type();
		message.dataBlock.damageType = FalconDamageType.ObjectCollisionDamage;

		Tpoint Objvec, Myvec, relVec;
		float relVel;

		Myvec.x = XDelta();
		Myvec.y = YDelta();
		Myvec.z = ZDelta();

		Objvec.x = theObject.XDelta();
		Objvec.y = theObject.YDelta();
		Objvec.z = theObject.ZDelta();

		relVec.x = Myvec.x - Objvec.x;
		relVec.y = Myvec.y - Objvec.y;
		relVec.z = Myvec.z - Objvec.z;

		relVel = (float)sqrt(relVec.x*relVec.x + relVec.y*relVec.y + relVec.z*relVec.z);

		// for now use maxStrength as amount of damage.
		// later we'll want to add other factors into the equation --
		// on ground, speed, etc....	
		
		message.dataBlock.damageRandomFact = 1.0f;

		message.dataBlock.damageStrength = min(1000.0F, relVel * theObject.Mass() * 0.0001F + relVel * relVel * theObject.Mass()* 0.000002F);
	
		message.RequestOutOfBandTransmit ();

		if (message.dataBlock.damageStrength > 0.0f) // JB carrier
			FalconSendMessage (message,true);

		// send message to other ship
		// owner_session = (VuTargetEntity*)vuDatabase.Find(theObject.OwnerId());
		message = new FalconDamageMessage (theObject.Id(), FalconLocalGame);
		message.dataBlock.fEntityID  = Id();
        Debug.Assert(GetCampaignObject());
		message.dataBlock.fCampID = GetCampID();
		message.dataBlock.fSide   = GetCountry();
		message.dataBlock.fPilotID   = pilotSlot;
		message.dataBlock.fIndex     = Type();
		message.dataBlock.fWeaponID  = Type();
		message.dataBlock.fWeaponUID = theObject.Id();

		message.dataBlock.dEntityID  = theObject.Id();
		message.dataBlock.dCampID = theObject.GetCampID();
		message.dataBlock.dSide   = theObject.GetCountry();
		if (theObject.IsAirplane())
		   message.dataBlock.dPilotID   = ((SimMoverClass*)theObject).pilotSlot;
		else
		   message.dataBlock.dPilotID   = 255;
		message.dataBlock.dIndex     = theObject.Type();
		// for now use maxStrength as amount of damage.
		// later we'll want to add other factors into the equation --
		// on ground, speed, etc....
		
		message.dataBlock.damageRandomFact = 1.0f;
		message.dataBlock.damageStrength = min(1000.0F, relVel * Mass() * 0.0001F + relVel * relVel * Mass()* 0.000002F);
				
		message.dataBlock.damageType = FalconDamageType.ObjectCollisionDamage;
		message.RequestOutOfBandTransmit ();
	
		if (message.dataBlock.damageStrength > 0.0f) // JB carrier
			FalconSendMessage (message,true);
	} // end target list loop

	// JB carrier start
	if (!setOnObject && af.IsSet(AirframeClass.OnObject))
	{
		attachedEntity = NULL;
		af.ClearFlag(AirframeClass.OverRunway);
		af.ClearFlag(AirframeClass.OnObject);
		af.ClearFlag(AirframeClass.Planted);
		af.platform.UnSetFlag( ON_GROUND );
		af.SetFlag (AirframeClass.InAir);
	}
	// JB carrier end
}


        public void CheckPersistantCollision();

        public void CautionCheck()
        {
            if (!isDigital)
            {
                // Check fuel
                if (af.Fuel() + af.ExternalFuel() < bingoFuel && !mFaults.GetFault(FaultClass.fms_fault))
                {
                    if (g_bRealisticAvionics)
                    {
                        //MI added for ICP stuff.
                        //rewritten 04/21/01
#if NOTHING
			  bingoFuel = bingoFuel * 0.5F;
			  //Update our ICP readout
			  if(OTWDriver.pCockpitManager.mpIcp.IsICPSet(ICPClass.BINGO_MODE))
				  OTWDriver.pCockpitManager.mpIcp.ExecBingo();
			  if (bingoFuel < 100.0F)
				  bingoFuel = -1.0F;
			  cockpitFlightData.SetLightBit(FlightData.FuelLow);
			  mFaults.SetFault(fuel_low_fault); 
			  mFaults.SetMasterCaution();
			  F4SoundFXSetDist( af.auxaeroData.sndBBBingo, true, 0.0f, 1.0f );
#else
                        //Only warn us if we've not already been warned.
                        if (!mFaults.GetFault(fuel_low_fault))
                        {
                            cockpitFlightData.SetLightBit(FlightData.FuelLow);
                            //mFaults.SetFault(fuel_low_fault);
                            mFaults.SetWarning(fuel_low_fault);
                            if (F4SoundFXPlaying(af.auxaeroData.sndBBBingo))
                                F4SoundFXSetDist(af.auxaeroData.sndBBBingo, true, 0.0f, 1.0f);
                        }
#endif
                    }
                    else
                    {
                        //me123 let's set a bingo manualy
                        bingoFuel = 100.0f;
                        if (af.Fuel() <= 100.0F)
                            bingoFuel = -10.0F;
                        cockpitFlightData.SetLightBit(FlightData.FuelLow);
                        mFaults.SetFault(fuel_low_fault);
                        mFaults.SetMasterCaution();
                        if (!F4SoundFXPlaying(af.auxaeroData.sndBBBingo))
                            F4SoundFXSetDist(af.auxaeroData.sndBBBingo, true, 0.0f, 1.0f);
                    }

                }
                //MI reset our fuel low fault if we set a bingo value below our current level
                else if (g_bRealisticAvionics && mFaults.GetFault(fuel_low_fault))
                {
                    if (bingoFuel < af.Fuel() + af.ExternalFuel())
                    {
                        cockpitFlightData.ClearLightBit(FlightData.FuelLow);
                        mFaults.ClearFault(fuel_low_fault);
                    }
                }

                // Caution TO/LDG Config
                //MI
                if (IsF16())
                {
                    if (ZPos() > -10000.0F && Kias() < 190.0F && ZDelta() * 60.0F >= 250.0F && af.gearPos != 1.0F)
                    {
                        if (!mFaults.GetFault(to_ldg_config))
                        {
                            if (!g_bRealisticAvionics)
                                mFaults.SetFault(to_ldg_config);
                            else
                                mFaults.SetWarning(to_ldg_config);
                        }
                    }
                    else
                        mFaults.ClearFault(to_ldg_config);

                    // JPO check for trapped fuel
                    if (!mFaults.GetFault(FaultClass.fms_fault) && af.CheckTrapped())
                    {
                        if (!mFaults.GetFault(fuel_trapped))
                        {
                            if (!g_bRealisticAvionics)
                                mFaults.SetFault(fuel_trapped);
                            else
                                mFaults.SetWarning(fuel_trapped);
                        }
                    }
                    else
                        mFaults.ClearFault(fuel_trapped);

                    //MI Fuel HOME warning		  
                    if (!mFaults.GetFault(FaultClass.fms_fault) && af.CheckHome())
                    {
                        if (!mFaults.GetFault(fuel_home))
                        {
                            if (!g_bRealisticAvionics)
                                mFaults.SetFault(fuel_home);
                            else
                                mFaults.SetWarning(fuel_home);
                            //Make noise
                            if (!F4SoundFXPlaying(af.auxaeroData.sndBBBingo))
                                F4SoundFXSetDist(af.auxaeroData.sndBBBingo, true, 0.0f, 1.0f);
                        }
                    }
                    else
                        mFaults.ClearFault(fuel_home);
                }

                /////////////me123 let's brake something if we fly too fast
                //me123 OWLOOK switch here to enable aircraft limits (overg and max speed)
                //if (g_bEnableAircraftLimits) {	MI
                if (g_bRealisticAvionics)
                {
                    // Marco Edit - OverG DOES NOT affect		!!!
                    // (at least not before the aircraft falls apart)
                    //MI put back in after discussing with Marco
                    CheckForOverG();
                    CheckForOverSpeed();
                }
                // save for later.
                int savewarn = mFaults.WarnReset();
                int savemc = mFaults.MasterCaution();
                //// JPO - check hydraulics too.
                ///////////
                if ((af.rpm * 37.0F) < 15.0F || mFaults.GetFault(FaultClass.eng_fault))
                {
                    if (!mFaults.GetFault(oil_press))
                    {
                        if (!g_bRealisticAvionics)
                            // less than 15 psi
                            mFaults.SetFault(oil_press);
                        else
                            mFaults.SetWarning(oil_press);
                    }
                }
                else
                    mFaults.ClearFault(oil_press);

                if (!af.HydraulicOK())
                {
                    if (!mFaults.GetFault(hyd))
                    {
                        if (!g_bRealisticAvionics)
                            mFaults.SetFault(hyd);
                        else
                            mFaults.SetWarning(hyd);
                    }
                }
                else
                    mFaults.ClearFault(hyd);

                // JPO Sec is active below 20% rpm
                if (af.rpm < 0.20F)
                {
                    if (!mFaults.GetFault(sec_fault))
                    {
                        if (!g_bRealisticAvionics)
                            mFaults.SetFault(sec_fault);
                        else
                            mFaults.SetCaution(sec_fault);
                    }
                }
                else
                    mFaults.ClearFault(sec_fault);

                // this is a hack JPO
                // when starting up we don't want to set the warn/caution lights,
                // but we do want the indicator lights.
                // so we clear the cautions if nothing else had them set.
                if (af.rpm < 1e-2 && OnGround())
                {
                    if (savewarn == 0) mFaults.ClearWarnReset();
                    if (savemc == 0) mFaults.ClearMasterCaution();
                }

#if NOTHING // JPO: I don't think this makes any sense to me... me123????
///////////me123 changed 27000 to -27000
		if(ZPos() < -27000.0F && mFaults.GetFault(FaultClass.eng_fault)) 
		{
			if(!mFaults.GetFault(to_ldg_config))
				mFaults.SetFault(cabin_press_fault);
		}
		else
			mFaults.ClearFault(cabin_press_fault);
#endif
                // JPO - dump dumps cabin pressure, off means its not there anyway.
                // 10000 is a guess - thats where you requirte oxygen
                if (ZPos() < -10000 && (af.GetAirSource() == AirframeClass.AS_DUMP ||
                    af.GetAirSource() == AirframeClass.AS_OFF))
                {
                    if (!mFaults.GetFault(cabin_press_fault))
                    {
                        if (!g_bRealisticAvionics)
                            mFaults.SetFault(cabin_press_fault);
                        else
                            mFaults.SetCaution(cabin_press_fault);
                    }
                }
                else
                    mFaults.ClearFault(cabin_press_fault);

                if (mFaults.GetFault(FaultClass.hud_fault) && mFaults.GetFault(FaultClass.fcc_fault))
                {
                    if (!mFaults.GetFault(canopy))
                    {
                        if (!g_bRealisticAvionics)
                            mFaults.SetFault(canopy);
                        else
                            mFaults.SetWarning(canopy);
                    }
                }
                else
                    mFaults.ClearFault(canopy);
                ///////////
                if (mFaults.GetFault(FaultClass.fcc_fault))
                {
                    if (!mFaults.GetFault(dual_fc))
                    {
                        if (!g_bRealisticAvionics)
                            mFaults.SetFault(dual_fc);
                        else
                            mFaults.SetWarning(dual_fc);
                    }
                }
                else
                    mFaults.ClearFault(dual_fc);
                ///////////
                if (mFaults.GetFault(FaultClass.amux_fault) || mFaults.GetFault(FaultClass.bmux_fault))
                {
                    if (!g_bRealisticAvionics)
                        mFaults.SetFault(avionics_fault);
                    else
                        mFaults.SetCaution(avionics_fault);
                }
                else
                {
                    mFaults.ClearFault(avionics_fault);
                }
                ////////////
                if (mFaults.GetFault(FaultClass.ralt_fault))
                {
                    if (!mFaults.GetFault(radar_alt_fault))
                    {
                        if (!g_bRealisticAvionics)
                            mFaults.SetFault(radar_alt_fault);
                        else
                            mFaults.SetCaution(radar_alt_fault);
                    }
                }
                else
                    mFaults.ClearFault(radar_alt_fault);
                ///////////////
                if (mFaults.GetFault(FaultClass.iff_fault))
                {
                    if (!mFaults.GetFault(iff_fault))
                    {
                        if (!g_bRealisticAvionics)
                            mFaults.SetFault(iff_fault);
                        else
                            mFaults.SetCaution(iff_fault);
                    }
                }
                else
                    mFaults.ClearFault(iff_fault);
                ///////////////
                if (mFaults.GetFault(FaultClass.rwr_fault))
                {
                    if (!mFaults.GetFault(ecm_fault))
                    {
                        if (!g_bRealisticAvionics)
                            mFaults.SetFault(ecm_fault);
                        else
                            mFaults.SetCaution(ecm_fault);
                    }
                }
                else
                    mFaults.ClearFault(ecm_fault);
                ///////////////
                if (mFaults.GetFault(FaultClass.rwr_fault))
                {
                    if (!mFaults.GetFault(nws_fault))
                    {
                        if (!g_bRealisticAvionics)
                            mFaults.SetFault(nws_fault);
                        else
                            mFaults.SetCaution(nws_fault);
                    }
                }
                else
                    mFaults.ClearFault(nws_fault);
                /////////////
                //MI
                // Overheat Fault
                if (mFaults.GetFault(FaultClass.eng_fault) && af.rpm <= 0.75)
                {
                    if (!mFaults.GetFault(overheat_fault))
                    {
                        if (!g_bRealisticAvionics)
                            mFaults.SetFault(overheat_fault);
                        else
                            mFaults.SetCaution(overheat_fault);
                    }
                }
                else
                    mFaults.ClearFault(overheat_fault);


                // if lg up and aoa and speed
                // if airbrakes on
                //MI what kind of bullshit is this anyway?????
                if (!g_bRealisticAvionics)
                {
                    if (mFaults.GetFault(FaultClass.rwr_fault))
                    {
                        mFaults.SetFault(to_ldg_config);
                    }
                    else
                    {
                        mFaults.ClearFault(to_ldg_config);
                    }
                }
                //////////////
                // Set external data if this is Ownship and player
                if (this == SimDriver.playerEntity)
                    SetExternalData();

                // AMUX and BMUX combined failure forces FCC into NAV
                if (mFaults.GetFault(FaultClass.amux_fault) && mFaults.GetFault(FaultClass.bmux_fault))
                {
                    FCC.SetMasterMode(FireControlComputer.Nav);
                }

                // If blanker broken, no ECM
                if (mFaults.GetFault(FaultClass.blkr_fault))
                {
                    SensorClass* theRwr = FindSensor(this, SensorClass.RWR);

                    if (theRwr)
                        theRwr.SetPower(false);
                    UnSetFlag(ECM_ON);
                }

                // Shut down radar when broken
                if (mFaults.GetFault(FaultClass.fcc_fault) == FaultClass.xmtr)
                {
                    RadarClass* theRadar = (RadarClass*)FindSensor(this, SensorClass.Radar);

                    if (theRadar)
                        theRadar.SetEmitting(false);
                }

                if (mFaults.GetFault(FaultClass.fcc_fault) == FaultClass.bus)
                {
                    RadarClass* theRadar = (RadarClass*)FindSensor(this, SensorClass.Radar);

                    if (theRadar)
                        theRadar.SetPower(false);
                }

                // Shut down rwr when broken
                if (mFaults.GetFault(FaultClass.rwr_fault))
                {
                    SensorClass* theRwr = FindSensor(this, SensorClass.RWR);

                    if (theRwr)
                        theRwr.SetPower(false);
                }

                // Shut down HTS when broken
                if (mFaults.GetFault(FaultClass.harm_fault))
                {
                    SensorClass* theHTS = FindSensor(this, SensorClass.HTS);

                    if (theHTS)
                        theHTS.SetPower(false);
                }
            }
            //MI new home of the wrong/correct CAT stuff
            //if(af.platform.IsPlayer() && g_bEnableCATIIIExtension)	MI
            if (IsPlayer() && g_bRealisticAvionics)
            {
                float MaxG = af.curMaxGs;
                float limitGs = 6.5f;
                Limiter* limiter = NULL; // JPO - use dynamic figure , not 6.5


                if (limiter = gLimiterMgr.GetLimiter(CatIIIMaxGs, af.VehicleIndex()))
                    limitGs = limiter.Limit(0);

                if (MaxG <= limitGs)
                {
                    //we need CATIII
                    if (!af.IsSet(AirframeClass.CATLimiterIII))
                        WrongCAT();
                    else
                        CorrectCAT();
                }
                else
                {
                    //we don't need CATIII
                    if (af.IsSet(AirframeClass.CATLimiterIII))
                        WrongCAT();
                    else
                        CorrectCAT();
                }
            }
            //MI Seat Arm switch
            if (IsPlayer() && g_bRealisticAvionics)
            {
                if (!SeatArmed)
                {
                    if (!mFaults.GetFault(seat_notarmed_fault))
                        mFaults.SetCaution(seat_notarmed_fault);
                }
                else
                    mFaults.ClearFault(seat_notarmed_fault);
            }
            if (g_bRealisticAvionics)
            {
                //MI WARN Reset stuff
                //me123 loopign warnign sound is just T_LCFG i think
                if (cockpitFlightData.IsSet(FlightData.T_L_CFG))	//this one gives continous warning
                {
                    //sound
                    if (mFaults.WarnReset())
                    {
                        if (vuxGameTime >= WhenToPlayWarning)
                        {
                            if (!F4SoundFXPlaying(SFX_BB_WARNING))
                                F4SoundFXSetDist(SFX_BB_WARNING, true, 0.0f, 1.0f);
                        }
                    }
                }
                else
                {
                    if (mFaults.DidManWarn())
                    {
                        mFaults.ClearManWarnReset();
                        mFaults.ClearWarnReset();
                    }
                }
                //MI Caution sound
                if (NeedsToPlayCaution)
                {
                    if (vuxGameTime >= WhenToPlayCaution)
                    {
                        if (mFaults.MasterCaution())
                        {
                            if (!F4SoundFXPlaying(af.auxaeroData.sndBBCaution))
                                F4SoundFXSetDist(af.auxaeroData.sndBBCaution, true, 0.0f, 1.0f);
                        }
                        NeedsToPlayCaution = false;
                    }
                }
                if (NeedsToPlayWarning)
                {
                    if (vuxGameTime >= WhenToPlayWarning)
                    {
                        if (mFaults.WarnReset())
                        {
                            if (!F4SoundFXPlaying(af.auxaeroData.sndBBWarning))
                                F4SoundFXSetDist(af.auxaeroData.sndBBWarning, true, 0.0f, 1.0f);
                        }
                        NeedsToPlayWarning = false;
                    }
                }
                //MI RF In SILENT gives TF FAIL
                if (RFState == 2)
                {
                    if (!mFaults.GetFault(tf_fail))
                        mFaults.SetWarning(tf_fail);
                }
                else
                    mFaults.ClearFault(tf_fail);
            }
        }

        public DigitalBrain DBrain()
        {
            return (DigitalBrain)theBrain;
        }

        public TankerBrain TBrain()
        {
            return (TankerBrain)theBrain;
        }
        // so we can discover we have an aircraft at the falcentity level
        public virtual bool IsAirplane()
        {
            return true;
        }

        public virtual float Mass();

        // Has the player triggered the ejection sequence?
        public bool ejectTriggered;
        public float ejectCountdown;
        public bool doEjectCountdown;

        //MI Emergency jettison
        public bool EmerJettTriggered;
        public float JettCountown;
        public bool doJettCountdown;

        //MI Cockpit nightlighting
        public bool NightLight, WideView;

        public void RemovePilot();

        public void RunCautionChecks()
        {
        }

        // Run the ejection sequence
        public void Eject();

        public virtual int HasPilot()
        {
            return (IsSetFlag(PILOT_EJECTED) ? false : true);
        }

        public virtual float GetKias();

        // Public for debug
        public void AddFault(int failures, uint failuresPossible, int numToBreak, int sourceOctant)
        {
            int i;

            //	failures = numToBreak * (float)rand() / (float)RAND_MAX;

            for (i = 0; i < failures; i++)
            {
                mFaults.SetFault(failuresPossible, !isDigital);
            }
            // JPO - break hydraulics occasionally
            if ((failuresPossible & FaultClass.eng_fault) &&
            (mFaults.GetFault(FaultClass.eng_fault) & FaultClass.hydr))
            {
                if (rand() % 100 < 20)
                { // 20% failure chance of A system
                    af.HydrBreak(AirframeClass.HYDR_A_SYSTEM);
                }
                if (rand() % 100 < 20)
                { // 20% failure chance of B system
                    af.HydrBreak(AirframeClass.HYDR_B_SYSTEM);
                }
            }
            // also break the generators now and then
            if (failuresPossible & FaultClass.eng_fault)
            {
                if (rand() % 7 == 1)
                    af.GeneratorBreak(AirframeClass.GenStdby);
                if (rand() % 7 == 1)
                    af.GeneratorBreak(AirframeClass.GenMain);
            }
#if NOTHING
int failedThing;
int i, j = 0;
int failedThings[FaultClass.NumFaultListSubSystems];
BOOL Found;
int canFail;
int numFunctions = 0;
int failedFunc;

	failures = numToBreak * (float)rand() / (float)RAND_MAX;

	for(i = 0; i < FaultClass.NumFaultListSubSystems; i++) {
      if(failuresPossible & (1 << i)) {
			failedThings[j] = i;
			j++;
		}
	}
   
	for(i = 0; i < failures; i++) {
		Found = false;

		do {
			failedThing = j * (float)rand() / (float)RAND_MAX;
         numFunctions = 0;

			if(failedThings[failedThing] != -1) {
            // FLCS is a special case, as it has 1 informational fault which MUST happen first
            if ((FaultClass.type_FSubSystem)failedThings[failedThing] == FaultClass.flcs_fault &&
               !mFaults.GetFault(FaultClass.flcs_fault))
            {
               failedFunc = 1;
               numFunctions = -1;
               while (failedFunc)
               {
                  numFunctions ++;
                  if (FaultClass.sngl & (1 << numFunctions))
                     failedFunc --;
               }
            }
            else
            {
               canFail   = mFaults.Breakable((FaultClass.type_FSubSystem)failedThings[failedThing]);

               // How many functions?
               while (canFail)
               {
                  if (canFail & 0x1)
                     numFunctions ++;
                  canFail = canFail >> 1;
               }

               // pick 1 of canFail things
               failedFunc = numFunctions * (float)rand() / (float)RAND_MAX + 1;

               // Find that function
               canFail   = mFaults.Breakable((FaultClass.type_FSubSystem)failedThings[failedThing]);
               numFunctions = -1;
               while (failedFunc)
               {
                  numFunctions ++;
                  if (canFail & (1 << numFunctions))
                     failedFunc --;
               }
            }

				mFaults.SetFault((FaultClass.type_FSubSystem)failedThings[failedThing],
						(FaultClass.type_FFunction) (1 << numFunctions),	
						(FaultClass.type_FSeverity) FaultClass.fail,
						!isDigital);	// none, fail for now
	if (failedThings[failedThing] == FaultClass.eng_fault &&
		(1 << numFunctions) == hydr) {
		if (rand() % 100 < 20) { // 20% failure chance of A system
			af.HydrBreak (Airframe.HYDR_A_SYSTEM);
		}
		if (rand() % 100 < 20) { // 20% failure chance of B system
			af.HydrBreak (Airframe.HYDR_B_SYSTEM);
		}
	}

            MonoPrint ("Failed %s %s\n", FaultClass.mpFSubSystemNames[failedThings[failedThing]],
               FaultClass.mpFFunctionNames[numFunctions+1]);
				failedThings[failedThing] = -1;
				Found = true;
			}
		}
		while(Found = false);
	}
#endif
        }




        //used for safe deletion of sensor array when making a player vehicle
        private SensorClass** tempSensorArray;
        private int tempNumSensors;

        protected int SetDamageF16PieceType(DamageF16PieceStructure* piece, int type, int flag, int mask, float speed);

        protected int CreateDamageF16Piece(DamageF16PieceStructure* piece, int* mask);

        protected int CreateDamageF16Effects();

        protected void SetupDamageF16Effects(DamageF16PieceStructure* piece);

        public VuEntity* attachedEntity; // JB carrier
        public bool AWACSsaidAbort;		// MN when target got occupied, AWACS says something useful

        private void CalculateAileronAndFlap(float qf, float* al, float* ar, float* fl, float* fr);

        private void CalculateLef(float qfactor);

        private void CalculateStab(float qfactor, float* sl, float* sr);

        private float CalculateRudder(float qfactor);

        private void MoveDof(int dof, float newvalue, float rate, int ssfx = -1, int lsfx = -1, int esfx = -1);

        private void DeployDragChute(int n);

        private int FindBestSpawnPoint(ObjectiveClass* obj, SimInitDataClass* initData);
#endif
    }
}

