using System;
using FalconNet.VU;
using FalconNet.Common;
using System.IO;

namespace FalconNet.Sim
{
	////////////////////////////////////////////
	public struct DamageF16PieceStructure
	{
		int	mask;
		int	index;
		int	damage;
		int sfxflag;
		int	sfxtype;
		float lifetime;
		float dx, dy, dz;
		float yd, pd, rd;
		float pitch, roll;	// resting angle
	};
	////////////////////////////////////////////



	public class AircraftClass : SimVehicleClass
	{
#if TODO
		public const int FLARE_STATION = 0;
		public const int CHAFF_STATION = 1;
		public const int  DEBRIS_STATION = 2;
			
		#if USE_SH_POOLS
		
			public // Overload new/delete to use a SmartHeap fixed size pool
			public public void *operator new(size_t size) { ShiAssert( size == sizeof(AircraftClass) ); return MemAllocFS(pool);	};
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
		};

		[Flags]
		public enum ACFLAGS
		{
			isF16    = 0x1, // is an F-16 - hopefully historic usage only soon
			hasSwing = 0x2, // has swing wing
			isComplex = 0x4, // has a complex model (lots of dofs and switches)
			InRecovery = 0x8, // recovering from gloc
		};

		[Flags]
	     public  enum AvionicsPowerFlags
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
			RwrPower  = 0x20000,
			APPower = 0x40000,
			PFDPower = 0x80000,
			ChaffFlareCount = 0x100000,
			//MI
			IFFPower = 0x200000,
			// systems that don't have power normally.
			SpotLightPower = 0x20000000,
			InstrumentLightPower = 0x40000000, 
			InteriorLightPower   = 0x80000000, // start from the top down
			AllPower = 0xffffff // all the systems have power normally.
		};

		public enum MainPowerType
		{
			MainPowerOff,
			MainPowerBatt,
			MainPowerMain
		};
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
		};

		public enum LightSwitch
		{
			LT_OFF,
			LT_LOW,
			LT_NORMAL
		};
		public LightSwitch interiorLight, instrumentLight, spotLight;

		public void SetInteriorLight (LightSwitch st)
		{
			interiorLight = st;
		}

		public void SetInstrumentLight (LightSwitch st)
		{
			instrumentLight = st;
		}

		public void SetSpotLight (LightSwitch st)
		{
			spotLight = st;
		}

		public LightSwitch GetInteriorLight ()
		{
			return interiorLight;
		}

		public LightSwitch GetInstrumentLight ()
		{
			return instrumentLight;
		}

		public LightSwitch GetSpotLight ()
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
		};
		//MI new OverG/Speed stuff
		public void CheckForOverG ()
		{
			throw new NotImplementedException ();
		}

		public void CheckForOverSpeed ()
		{
			throw new NotImplementedException ();
		}

		public void DoOverGSpeedDamage (int station)
		{
			throw new NotImplementedException ();
		}

		public void StoreToDamage (WeaponClass thing)
		{
			throw new NotImplementedException ();
		}

		public uint StationsFailed;
		  
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
		public void StationFailed (StationFlags fl)
		{
			StationsFailed |= fl;
		}

		public int GetStationFailed (StationFlags fl)
		{
			return (StationsFailed & fl) == (uint)fl ? 1 : 0;
		}
	
		//MI
		public uint ExteriorLights;
		public enum ExtlLightFlags
		{
			Extl_Main_Power		= 0x1,
			Extl_Anit_Coll		= 0x2,
			Extl_Steady_Flash	= 0x4,
			Extl_Wing_Tail		= 0x8,
		};			
		public void ExtlOn (ExtlLightFlags fl)
		{
			ExteriorLights |= fl;
		}

		public void ExtlOff (ExtlLightFlags fl)
		{
			ExteriorLights &= ~fl;
		}

		public int ExtlState (ExtlLightFlags fl)
		{
			return (ExteriorLights & fl) == (uint)fl ? 1 : 0;
		}
	
		//MI
		public uint INSFlags;
		  
		[Flags]
		  public enum INSAlignFlags
		{
			INS_PowerOff		= 0x1,
			INS_AlignNorm		= 0x2,
			INS_AlignHead		= 0x4,
			INS_Cal				= 0x8,
			INS_AlignFlight		= 0x10,
			INS_Nav				= 0x20,
			INS_Aligned			= 0x40,
			INS_ADI_OFF_IN		= 0x80,
			INS_ADI_AUX_IN		= 0x100,
			INS_HUD_FPM			= 0x200,
			INS_HUD_STUFF		= 0x400,
			INS_HSI_OFF_IN		= 0x800,
			INS_HSD_STUFF		= 0x1000,
			BUP_ADI_OFF_IN		= 0x2000,
		};			
		public void INSOn (INSAlignFlags fl)
		{
			INSFlags |= fl;
		}

		public void INSOff (INSAlignFlags fl)
		{
			INSFlags &= ~fl;
		}

		public int INSState (INSAlignFlags fl)
		{
			return (INSFlags & fl) == (uint)fl ? 1 : 0;
		}
	
		public void RunINS ()
		{
			throw new NotImplementedException ();
		}

		public void DoINSAlign ()
		{
			throw new NotImplementedException ();
		}

		public void SwitchINSToAlign ()
		{
			throw new NotImplementedException ();
		}

		public void SwitchINSToNav ()
		{
			throw new NotImplementedException ();
		}

		public void SwitchINSToOff ()
		{
			throw new NotImplementedException ();
		}

		public void SwitchINSToInFLT ()
		{
			throw new NotImplementedException ();
		}

		public void CheckINSStatus ()
		{
			throw new NotImplementedException ();
		}

		public void CalcINSDrift ()
		{
			throw new NotImplementedException ();
		}

		public void CalcINSOffset ()
		{
			throw new NotImplementedException ();
		}

		public float GetINSLatDrift ()
		{
			return (INSLatDrift + INSLatOffset);
		}

		public float GetINSLongDrift ()
		{
			return (INSLongDrift + INSLongOffset);
		}

		public float GetINSAltOffset ()
		{
			return INSAltOffset;
		}

		public float GetINSHDGOffset ()
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
		public uint AVTRFlags;
		public float AVTRCountown;
		public bool doAVTRCountdown;

		public void AddAVTRSeconds ()
		{
			throw new NotImplementedException ();
		}
			
		[Flags]
		  public enum AVTRStateFlags
		{
			AVTR_ON	= 0x1,
			AVTR_AUTO = 0x2,
			AVTR_OFF	= 0x4,
		};
		public void AVTROn (AVTRStateFlags fl)
		{
			AVTRFlags |= fl;
		}

		public void AVTROff (AVTRStateFlags fl)
		{
			AVTRFlags &= ~fl;
		}

		public int AVTRState (AVTRStateFlags fl)
		{
			return (AVTRFlags & fl) == (uint)fl ? 1 : 0;
		}
	
	
		//MI Mal and Ind Lights test button
		public  bool TestLights;
		//MI some other stuff
		public bool TrimAPDisc;
		public bool TEFExtend;
		public bool CanopyDamaged;
		public bool LEFLocked;
		public float LTLEFAOA, RTLEFAOA;
		public uint LEFFlags;
		public float leftLEFAngle;
		public float rightLEFAngle;
			
		[Flags]
		  public enum LEFStateFlags
		{
			LT_LEF_DAMAGED	= 0x1,
			LT_LEF_OUT		= 0x2,
			RT_LEF_DAMAGED	= 0x8,
			RT_LEF_OUT		= 0x10,
			LEFSASYNCH		= 0x20,
		};
		public void LEFOn (LEFStateFlags fl)
		{
			LEFFlags |= fl;
		}

		public void LEFOff (LEFStateFlags fl)
		{
			LEFFlags &= ~fl;
		}

		public int LEFState (LEFStateFlags fl)
		{
			return (LEFFlags & fl) == (uint)fl ? 1 : 0;
		}

		public float CheckLEF (int side)
		{
			throw new NotImplementedException ();
		}
	
		public int MissileVolume;
		public int ThreatVolume;
		public bool GunFire;
		//targeting pod cooling time
		public float PodCooling;
		public bool CockpitWingLight;
		public bool CockpitStrobeLight;

		public void SetCockpitWingLight (bool state)
		{
			throw new NotImplementedException ();
		}

		public void SetCockpitStrobeLight (bool state)
		{
			throw new NotImplementedException ();
		}
	
		public AircraftClass (int flag, VU_BYTE[] stream)
		{
			throw new NotImplementedException ();
		}

		public AircraftClass (int flag, FileStream filePtr)
		{
			throw new NotImplementedException ();
		}
		
		public AircraftClass (int flag, int type)
		{
			throw new NotImplementedException ();
		}
		//TODO public virtual ~AircraftClass		();
	
		public float					glocFactor;
		public int						fireGun, fireMissile, lastPickle;
		public FackClass				mFaults;
		public AirframeClass			af;
		public FireControlComputer	FCC;
		public SMSClass				Sms;
		public GunClass				Guns;
	
		public virtual void	Init (SimInitDataClass* initData)
		{
			throw new NotImplementedException ();
		}

		public virtual int	   Wake ()
		{
			throw new NotImplementedException ();
		}

		public virtual int 	Sleep ()
		{
			throw new NotImplementedException ();
		}

		public virtual int		Exec ()
		{
			throw new NotImplementedException ();
		}

		public virtual void	JoinFlight ()
		{
			throw new NotImplementedException ();
		}

		public virtual void	InitData (int i)
		{
			throw new NotImplementedException ();
		}

		public virtual void	Cleanup ()
		{
			throw new NotImplementedException ();
		}
		
		public virtual int    CombatClass ()
		{
			throw new NotImplementedException ();
		} // 2002-02-25 MODIFIED BY S.G. virtual added in front since FlightClass also have one now...
		public void           SetAutopilot (AutoPilotType flag)
		{
			throw new NotImplementedException ();
		}

		public AutoPilotType	AutopilotType ()
		{
			return autopilotType;
		}

		public VU_ID	         HomeAirbase ()
		{
			throw new NotImplementedException ();
		}

		public VU_ID	         TakeoffAirbase ()
		{
			throw new NotImplementedException ();
		}

		public VU_ID	         LandingAirbase ()
		{
			throw new NotImplementedException ();
		}

		public VU_ID	         DivertAirbase ()
		{
			throw new NotImplementedException ();
		}

		public void           DropProgramed ()
		{
			throw new NotImplementedException ();
		}

		public int            IsF16 ()
		{
			return (acFlags & isF16 ? true : false);
		}

		public int            IsComplex ()
		{
			return ((acFlags & isComplex) ? true : false);
		}
		// 2000-11-17 ADDED BY S.G. SO AIRCRAFT CAN HAVE A ENGINE TEMPERATURE AS WELL AS 'POWER' (RPM) OUTPUT   
		public void SetPowerOutput (float powerOutput)
		{
			throw new NotImplementedException ();
		}//me123 changed back
		// END OF ADDED SECTION	
		public virtual void	SetVt (float newvt)
		{
			throw new NotImplementedException ();
		}

		public virtual void	SetKias (float newkias)
		{
			throw new NotImplementedException ();
		}

		public void			ResetFuel ()
		{
			throw new NotImplementedException ();
		}

		public virtual long	GetTotalFuel ()
		{
			throw new NotImplementedException ();
		}

		public virtual void	GetTransform (TransformMatrix vmat)
		{
			throw new NotImplementedException ();
		}

		public virtual void	ApplyDamage (FalconDamageMessage *damageMessage)
		{
			throw new NotImplementedException ();
		}

		public virtual void	SetLead (int flag)
		{
			throw new NotImplementedException ();
		}

		public virtual void	ReceiveOrders (FalconEvent* newOrder)
		{
			throw new NotImplementedException ();
		}

		public virtual float	GetP ()
		{
			throw new NotImplementedException ();
		}

		public virtual float	GetQ ()
		{
			throw new NotImplementedException ();
		}

		public virtual float	GetR ()
		{
			throw new NotImplementedException ();
		}

		public virtual float	GetAlpha ()
		{
			throw new NotImplementedException ();
		}

		public virtual float	GetBeta ()
		{
			throw new NotImplementedException ();
		}

		public virtual float	GetNx ()
		{
			throw new NotImplementedException ();
		}

		public virtual float	GetNy ()
		{
			throw new NotImplementedException ();
		}

		public virtual float	GetNz ()
		{
			throw new NotImplementedException ();
		}

		public virtual float	GetGamma ()
		{
			throw new NotImplementedException ();
		}

		public virtual float	GetSigma ()
		{
			throw new NotImplementedException ();
		}

		public virtual float	GetMu ()
		{
			throw new NotImplementedException ();
		}

		public virtual void	MakePlayerVehicle ()
		{
			throw new NotImplementedException ();
		}

		public virtual void	MakeNonPlayerVehicle ()
		{
			throw new NotImplementedException ();
		}

		public virtual void	MakeLocal ()
		{
			throw new NotImplementedException ();
		}

		public virtual void	MakeRemote ()
		{
			throw new NotImplementedException ();
		}

		public virtual void	ConfigurePlayerAvionics ()
		{
			throw new NotImplementedException ();
		}

		public virtual void	SetVuPosition ()
		{
			throw new NotImplementedException ();
		}

		public virtual void	Regenerate (float x, float y, float z, float yaw)
		{
			throw new NotImplementedException ();
		}

		public virtual FireControlComputer* GetFCC ()
		{
			return FCC;
		}

		public virtual SMSBaseClass* GetSMS ()
		{
			return (SMSBaseClass*)Sms;
		}

		public virtual int HasSPJamming ()
		{
			throw new NotImplementedException ();
		}

		public virtual int HasAreaJamming ()
		{
			throw new NotImplementedException ();
		}
	
	#if _DEBUG
		public virtual void	SetDead (int);
	#endif // _DEBUG
	
		// private:
	
		public long				mCautionCheckTime;
		public bool				MPOCmd;
		public char				dropChaffCmd;
		public char				dropFlareCmd;
		public int            acFlags;
		public AutoPilotType		autopilotType;
		public AutoPilotType		lastapType;
		public VU_TIME        dropProgrammedTimer;
		public ushort		dropProgrammedStep;
		public int					isDigital;
		public float				bingoFuel;
		//MI taking these functions for the ICP stuff, made some changes
		public float GetBingoFuel ()
		{
			return bingoFuel;
		}//me123
		public void SetBingoFuel (float newbingo)
		{
			bingoFuel = newbingo;
		}//me123
		public void DamageSounds ()
		{
			throw new NotImplementedException ();
		}

		public uint SpeedSoundsWFuel;
		public uint SpeedSoundsNFuel;
		public uint GSoundsWFuel;
		public uint GSoundsNFuel;

		public void WrongCAT ()
		{
			throw new NotImplementedException ();
		}

		public void CorrectCAT ()
		{
			throw new NotImplementedException ();
		}
		//MI for RALT stuff
		public 	enum RaltStatus
		{
			ROFF,
			RSTANDBY,
			RON
		};
		public float RALTCoolTime;	//Cooling is in progress
		public int RaltReady ()
		{
			return (RALTCoolTime < 0.0F && RALTStatus == RaltStatus.RON) ? 1 : 0;
		}

		public void RaltOn ()
		{
			RALTStatus = RaltStatus.RON;
		}

		public void RaltStdby ()
		{
			RALTStatus = RaltStatus.RSTANDBY;
		}

		public void RaltOff ()
		{
			RALTStatus = RaltStatus.ROFF;
		}
		//MI for EWS stuff
		public void DropEWS ()
		{
			throw new NotImplementedException ();
		}

		public void EWSChaffBurst ()
		{
			throw new NotImplementedException ();
		}

		public void EWSFlareBurst ()
		{
			throw new NotImplementedException ();
		}

		public void ReleaseManualProgram ()
		{
			throw new NotImplementedException ();
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
		public uint APFlag;

		public int IsOn (APFlags flag)
		{
			return APFlag & flag ? 1 : 0;
		}

		public void SetAPFlag (APFlags flag)
		{
			APFlag |= flag;
		}

		public void ClearAPFlag (APFlags flag)
		{
			APFlag &= ~flag;
		}

		public void SetAPParameters ();

		public void SetNewRoll ();

		public void SetNewPitch ();

		public void SetNewAlt ();
		//MI seatArm
		public bool SeatArmed;

		public void StepSeatArm ();
	
		public TransformMatrix		vmat;
		public float				gLoadSeconds;
		public long				lastStatus;
		public BasicWeaponStation[]	counterMeasureStation = new BasicWeaponStation[3];
		public enum TRAIL_ENUM
		{ // what trail is used for what
			TRAIL_DAMAGE = 0, // we've been hit
			TRAIL_ENGINE1,
			TRAIL_ENGINE2,
			TRAIL_MAX,
			MAXENGINES = 4,
	
		};
		public DrawableTrail[]		smokeTrail = new DrawableTrail[TRAIL_MAX];
		public DrawableTrail[]	    conTrails = new DrawableTrail[MAXENGINES];
		public DrawableTrail[]	    engineTrails = new DrawableTrail[MAXENGINES];
		public DrawableTrail	rwingvortex, lwingvortex;
		public bool				playerSmokeOn;
		public DrawableGroundVehicle* pLandLitePool;
		public bool				mInhibitLitePool;

		public void				CleanupLitePool ();

		public void	AddEngineTrails (int ttype, DrawableTrail **tlist);

		public void	CancelEngineTrails (DrawableTrail **tlist);
	
		// JPO Avionics power settings;
		public uint powerFlags;

		public void PowerOn (AvionicsPowerFlags fl)
		{
			powerFlags |= fl;
		}

		public int HasPower (AvionicsPowerFlags fl);

		public void PowerOff (AvionicsPowerFlags fl)
		{
			powerFlags &= ~fl;
		}

		public void PowerToggle (AvionicsPowerFlags fl)
		{
			powerFlags ^= fl;
		}

		public int PowerSwitchOn (AvionicsPowerFlags fl)
		{
			return (powerFlags & fl) ? true : false;
		}
	
		public void PreFlight (); // JPO - do preflight checks.
	
		// JPPO Main Power
		public MainPowerType mainPower;

		public MainPowerType MainPower ()
		{
			return mainPower;
		}

		public bool MainPowerOn ()
		{
			return mainPower == MainPowerMain;
		}

		public void SetMainPower (MainPowerType t)
		{
			mainPower = t;
		}

		public void IncMainPower ();

		public void DecMainPower ();

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
		public uint elecLights;

		public bool ElecIsSet (ElectricLights lt)
		{
			return (elecLights & lt) ? true : false;
		}

		public void ElecSet (ElectricLights lt)
		{
			elecLights |= lt;
		}

		public void ElecClear (ElectricLights lt)
		{
			elecLights &= ~lt;
		}

		public void DoElectrics ();

		public const ulong[] systemStates = new ulong[PowerMaxState];
	
		//MI EWS PGM Switch
		public EWSPGMSwitch EWSPgm;

		public EWSPGMSwitch EWSPGM ()
		{
			return EWSPgm;
		}

		public void SetPGM (EWSPGMSwitch t)
		{
			EWSPgm = t;
		}

		public void IncEWSPGM ();

		public void DecEWSPGM ();

		public void DecEWSProg ();

		public void IncEWSProg ();
		//Prog select switch
		public uint EWSProgNum;
		//MI caution stuff
		public bool NeedsToPlayCaution;
		public bool NeedsToPlayWarning;
		public VU_TIME WhenToPlayCaution;
		public VU_TIME WhenToPlayWarning;

		public void SetExternalData ();
		//MI Inhibit VMS Switch
		public bool playBetty;

		public void ToggleBetty ();
		//MI RF switch
		public int RFState;	//0 = NORM; 1 = QUIET; 2 = SILENT
		public void GSounds ();

		public void SSounds ();

		public int SpeedToleranceTanks;
		public int SpeedToleranceBombs;
		public float GToleranceTanks;
		public float GToleranceBombs;
		public int[] OverSpeedToleranceTanks = new int[3];	//3 levels of OverSpeed for tanks
		public int[] OverSpeedToleranceBombs = new int[3]; //3 levels of OverSpeed for bombs
		public int[] OverGToleranceTanks = new int[3];	//3 levels of OverG for tanks
		public int[] OverGToleranceBombs = new int[3]; //3 levels of OverG for bombs
		public void AdjustTankSpeed (int level);

		public void AdjustBombSpeed (int level);

		public void AdjustTankG (int level);

		public void AdjustBombG (int level);
	
		public void	DoWeapons ();

		public float	GlocPrediction ();

		public void	InitCountermeasures ();

		public void	CleanupCountermeasures ();

		public void	InitDamageStation ();

		public void	CleanupDamageStation ();

		public void	DoCountermeasures ();

		public void	DropChaff ();

		public void	DropFlare ();

		public void	GatherInputs ();

		public void	RunSensors ();

		public bool	LandingCheck (float noseAngle, float impactAngle, int groundType);

		public void	GroundFeatureCheck (float groundZ);

		public void	RunExplosion ();

		public void	ShowDamage ();

		public void	CleanupDamage ();

		public void	MoveSurfaces ();

		public void	RunGear ();

		public void	ToggleAutopilot ();

		public void	OnGroundInit (SimInitDataClass* initData);

		public void	CheckObjectCollision ();

		public void	CheckPersistantCollision ();

		public void	CautionCheck ();
		
		public DigitalBrain *DBrain ()
		{
			return (DigitalBrain *)theBrain;
		}

		public TankerBrain *TBrain ()
		{
			return (TankerBrain *)theBrain;
		}
		// so we can discover we have an aircraft at the falcentity level
		public virtual int IsAirplane ()
		{
			return true;
		}

		public virtual float	Mass ();
		
		// Has the player triggered the ejection sequence?
		public bool	ejectTriggered;
		public float	ejectCountdown;
		public bool	doEjectCountdown;
	
		//MI Emergency jettison
		public bool EmerJettTriggered;
		public float JettCountown;
		public bool doJettCountdown; 
	
		//MI Cockpit nightlighting
		public bool NightLight, WideView;
		
		public void RemovePilot ();

		public void RunCautionChecks ();
		
		// Run the ejection sequence
		public void Eject ();

		public virtual int HasPilot ()
		{
			return (IsSetFlag (PILOT_EJECTED) ? false : true);
		}
	
		public virtual float GetKias ();
	
		// Public for debug
		public void AddFault (int failures, uint failuresPossible, int numToBreak, int sourceOctant);
		
	
	
		//used for safe deletion of sensor array when making a player vehicle
		private SensorClass** tempSensorArray;
		private int tempNumSensors;
	
		protected int SetDamageF16PieceType (DamageF16PieceStructure *piece, int type, int flag, int mask, float speed);

		protected int CreateDamageF16Piece (DamageF16PieceStructure *piece, int *mask);

		protected int CreateDamageF16Effects ();

		protected void SetupDamageF16Effects (DamageF16PieceStructure *piece);
	
		public VuEntity *attachedEntity; // JB carrier
		public bool AWACSsaidAbort;		// MN when target got occupied, AWACS says something useful
	
		private void CalculateAileronAndFlap (float qf, float *al, float *ar, float *fl, float *fr);

		private void CalculateLef (float qfactor);

		private void CalculateStab (float qfactor, float *sl, float *sr);

		private float CalculateRudder (float qfactor);

		private void MoveDof (int dof, float newvalue, float rate, int ssfx = -1, int lsfx = -1, int esfx = -1);

		private void DeployDragChute (int n);

		private int FindBestSpawnPoint (ObjectiveClass *obj, SimInitDataClass* initData);
#endif
	}
}

