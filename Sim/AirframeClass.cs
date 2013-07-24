using FalconNet.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    public class AeroData
    {
        // public ~AeroData (void) {delete mach; delete alpha; delete clift;
        // delete cdrag; delete cy; };
        public int numMach;
        public int numAlpha;
        public float[] mach;
        public float[] alpha;
        public float[] clift;
        public float[] cdrag;
        public float[] cy;
        public float clFactor;
        public float cdFactor;
        public float cyFactor;
#if USE_SH_POOLS
public:
	// Overload new/delete to use a SmartHeap fixed size pool
	void *operator new(size_t size) { return MemAllocPtr(AirframeDataPool, size, 0);	};
	void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
    }

    public class AuxAeroData
    {
#if TODO
        public AuxAeroData() { rollMomentum = 1; pitchMomentum = 1; yawMomentum = 1; pitchElasticity = 1; sinkRate = 15.0f; }


        //engine
        public float fuelFlowFactorNormal; // fuel flow rate for normal engine rates
        public float fuelFlowFactorAb; // fuel flow rate for afterburner lit
        public float minFuelFlow; // least possible fuel flow rate
        public float normSpoolRate; // factor that the engines spool up/down by at normal operation
        public float abSpoolRate; // factor the engines spool up/down by in the AB regime
        public float jfsSpoolUpRate; // rate the engines spool up when JFS started
        public float jfsSpoolUpLimit; // rpm percentage that JFS stops at
        public float lightupSpoolRate; // spool up rate after engine is lit but before operating range
        public float flameoutSpoolRate; // spool down rate when engine dies
        public float jfsRechargeTime; // time jfs takes to recharge
        public float jfsMinRechargeRpm; // min rpm required to recharge jfs
        public float mainGenRpm; // rpm that main generator starts operating at
        public float stbyGenRpm; // rpm that standby generator starts operating at.
        public float epuBurnTime; // time the epu can burn fuel for in total
        public float jfsSpinTime;	//MI how long the JFS runs with the hydraulic enegery
        public int DeepStallEngineStall;
        public int engineDamageStopThreshold; // 2002-04-11 ADDED BY S.G. Externalized the 13% chance of engine stop
        public int engineDamageNoRestartThreshold; // 2002-04-11 ADDED BY S.G. Externalized the 50% chance of engine not able to restart
        public int engineDamageHitThreshold; // 2002-04-11 ADDED BY S.G. Externalized the hitPoints 'theshold' being more than 0

        public const int AUX_LEFTEF_NONE = 0;
        public const int AUX_LEFTEF_MANUAL = 1;
        public const int AUX_LEFTEF_AOA = 2;
        public const int AUX_LEFTEF_MACH = 3;
        public const int AUX_LEFTEF_TEF = 4;

        //airframe
        public int hasLef; // has LEF
        public int hasTef; // has TEF
        public int hasFlapperons; // has flapperons as opposed to separates.
        public int hasSwingWing; // swing wing
        public int isComplex; // complex model
        public float tefMaxAngle; // divide angle of TEF by this to get amount of influence on CL and CD
        public float lefMaxAngle; // divide angle of LEF by this to get amount of influence on CL and CD
        public int tefNStages;
        public int lefNStages; // number of stages of flaps
        public float tefRate; // how fast the TEFs move.
        public float tefTakeOff; // TEF angle for takeoff
        public float lefRate; // how fast the TEFs move.
        public float rudderMaxAngle;
        public float aileronMaxAngle;
        public float elevonMaxAngle;
        public float airbrakeMaxAngle;
        public float CLtefFactor; // how much the TEF affect the CL
        public float CDtefFactor; // how much the TEF affect the CD
        public float CDlefFactor; // how much the LEF affect the CD
        public float CDSPDBFactor; // how much the speed brake affect drag
        public float CDLDGFactor; // how much the landing gear affects drag
        public float area2Span; // used to convert lift area into span
        public float lefGround; // lef Position on the ground
        public float lefMaxMach; // LEF maximum MACH
        public int flapGearRelative; // if flaps only work with gear down
        public float maxFlapVcas; // what Vcas flaps fully retract at
        public float flapVcasRange; // what range of Vcas flaps work over
        public float rollMomentum;
        public float pitchMomentum;
        public float yawMomentum;
        public float pitchElasticity;
        public float sinkRate; // JPO - for landing
        public float gearPitchFactor; // how much gear down affects AOA bias
        public float rollGearGain; // how the gains change with landing gear down
        public float yawGearGain; // how the gains change with landing gear down
        public float pitchGearGain; // how the gains change with landing gear down
        public float landingAOA; // JPO - what AOA required for landing
        public float dragChuteCd; // extra CD for drag chute
        public float dragChuteMaxSpeed; // how fast the drag chute can stand
        public float rollCouple; // how much rudder affects roll
        public int elevatorRolls; // elevator also responds to roll commands
        public float canopyMaxAngle; // max angle of canopy when open
        public float canopyRate; // deg/sec

        // fuel
        public float fuelFwdRes; // forward resevoir size
        public float fuelAftRes; // aft resevoir size (lbs)
        public float fuelFwd1; // forward tank size
        public float fuelAft1; // aft tank size
        public float fuelWingAl; // left wing tank size
        public float fuelWingFr; // right wing tank size
        public float fuelFwdResRate; // transfer rate from fwd res
        public float fuelAftResRate; // transfer rate from aft res
        public float fuelFwd1Rate; // transfer rate from fwd
        public float fuelAft1Rate; // transfer rate from aft
        public float fuelWingAlRate; // transfer rate from left wing
        public float fuelWingFrRate; // transfer rate from right wing
        public float fuelClineRate; // transfer rate from center line
        public float fuelWingExtRate; // transfer rate from ext wings
        public float fuelMinFwd; // min fuel in the forward tanks to trigger warning
        public float fuelMinAft; // min fuel in the forward tanks to trigger warning

        public Tpoint gunLocation; // offset of the gun
        public int engineSmokes; // engine emits smoke in nonAB
        public int nEngines; // count of engines
        public Tpoint[] engineLocation = new Tpoint[4]; // location of engines
        public Tpoint wingTipLocation; // where the right wing tip is (left symmetric)
        public Tpoint refuelLocation; // where the refuel point is.
        public int nChaff; // number of chaff bundles
        public int nFlare; // number of flares
        public int[] hardpointrg = new int[HARDPOINT_MAX];

        // sounds
        public int sndExt; // SFX_F16EXT
        public int sndWind; // SFX_ENGINEA
        public int sndAbInt; // SFX_BURNERI
        public int sndAbExt; // SFX_BURNERE
        public int sndEject; // SFX_EJECT
        public int sndSpdBrakeStart, sndSpdBrakeLoop, sndSpdBrakeEnd; // SFX_BRAKSTRT, SFX_BRAKLOOP, SFX_BRAKEND
        public int sndSpdBrakeWind; // SFX_BRAKWIND
        public int sndOverSpeed1, sndOverSpeed2; // SFX_OVERGSPEED1, SFX_OVERGSPEED2
        public int sndGunStart, sndGunLoop, sndGunEnd; //SFX_VULSTART, SFX_VULLOOP, SFX_VULLOOPE
        public int sndBBPullup; // SFX_BB_PULLUP
        public int sndBBBingo; // SFX_BB_BINGO
        public int sndBBWarning; // SFX_BB_WARNING
        public int sndBBCaution; // SFX_BB_CAUTION
        public int sndBBChaffFlareLow; //SFX_BB_CHFLLOW
        public int sndBBFlare; // SFX_FLARE
        public int sndBBChaffFlare; //SFX_BB_CHAFLARE
        public int sndBBChaffFlareOut;//SFX_BB_CHFLOUT
        public int sndBBAltitude; // SFX_BB_ALTITUDE
        public int sndBBLock; // SFX_BB_LOCK
        public int sndTouchDown; // SFX_TOUCHDOWN
        public int sndWheelBrakes; // SFX_BIND
        public int sndDragChute; // SFX_DRAGCHUTE
        public int sndLowSpeed; // SFX_LOWSPDTONE
        public int sndFlapStart, sndFlapLoop, sndFlapEnd; // SFX_FLAPSTRT, SFX_FLAPLOOP, SFX_FLAPEND
        public int sndHookStart, sndHookLoop, sndHookEnd; // SFX_HOOKSTRT, SFX_HOOKLOOP, SFX_HOOKEND
        public int sndGearCloseStart, sndGearCloseEnd; // SFX_GEARCST, SFX_GEARCEND
        public int sndGearOpenStart, sndGearOpenEnd; // SFX_GEAROST, SFX_GEAROEND
        public int sndGearLoop; // SFX_GEARLOOP
        public float rollLimitForAiInWP; // 2002-01-31 ADDED BY S.G. AI limition on roll when in waypoint (or similar) mode
        public int flap2Nozzle; // for harrier - nozzles follow flaps HACK HACK HACK
        public float startGroundAvoidCheck; // 2002-04-17 MN start ground avoid check only if closer than this distance to the ground
        public int limitPstick; // 0 = use pStick 1.0f; 1 = use SetPstick function (old code - probably better for heavies)
        public float refuelSpeed; // 2002-02-08 MN tanker speed for this aircraft when refueling
        public float refuelAltitude; // 2002-02-08 MN tanker altitude for this aircraft when refueling
        public int maxRippleCount; // 2002-02-23 MN maximum aircraft's ripple count (hardcoded 19 = F-16's max count)
        public int largePark; // JPO - requires a large parking space
        public float decelDistance; // 2002-03-05 MN different deceleration distances at refuel for each aircraft
        public float followRate; // 2002-03-06 MN different follow rates for each aircraft
        public float desiredClosureFactor; // 2002-03-08 MN another important factor for a smooth approach to the tanker
        public float IL78Factor; // 2002-03-09 MN for fixing "GivingGas" range factor
        public float longLeg; // 2002-03-13 MN long leg for tanker track pattern
        public float shortLeg; // 2002-03-13 MN short leg for tanker track pattern
        public float refuelRate; // 2002-03-15 MN different aircraft have different refuel rates
        public float AIBoomDistance; // 2002-03-28 MN hack to put the AI on the boom when in close range to it
        public float BingoReturnDistance; // MN distance in nm to the closest friendly airbase at which AI is forced to go to RTB mode
        public float jokerFactor; // 2002-03-12 MN default 2.0
        public float bingoFactor; // 2002-03-12 MN default 5.0
        public float fumesFactor; // 2002-03-12 MN default 15.0

        //MI TFR stuff
        public int Has_TFR;
        public float PID_K;			//Proportianal gain in TFR PID pitch controler.
        public float PID_KI;			//Intergral gain in TFR PID pitch controler
        public float PID_KD;			//Differential gain in TFR PID pitch controler
        public int TFR_LimitMX;	//Limit PID Integrator internal value so it doesn't get stuck in exteme.
        public float TFR_Corner;	//Corner speed used in TFR calculations
        public float TFR_Gain;		//Gain for calculating pitch error based on alt. difference
        public float EVA_Gain;		//Pitch setpoint gain in EVA (evade) code
        public float TFR_MaxRoll;	//Do not pull the stick in TFR if roll exceeds this value
        public float TFR_SoftG;		//Max TFR G pull in soft mode
        public float TFR_MedG;		//Max TFR G pull in medium mode
        public float TFR_HardG;		//Max TFR G pull in hard mode
        public float TFR_Clearance;	//Minimum clearance above the top of any obstacle [ft]
        public float SlowPercent;		//Flash SLOW when airspeed is lower then this percentage of corner speed
        public float TFR_lookAhead;	//Distance from ground directly under a/c used to measure ground inclination [ft]
        public float EVA1_SoftFactor;	//Turnradius multiplier to get safe distance from ground for FLY_UP in SLOW
        public float EVA2_SoftFactor;	//Turnradius multiplier to get safe distance from ground for OBSTACLE in SLOW
        public float EVA1_MedFactor;	//Turnradius multiplier to get safe distance from ground for FLY_UP in MED
        public float EVA2_MedFactor;	//Turnradius multiplier to get safe distance from ground for OBSTACLE in MED
        public float EVA1_HardFactor;	//Turnradius multiplier to get safe distance from ground for FLY_UP in HARD
        public float EVA2_HardFactor;	//Turnradius multiplier to get safe distance from ground for FLY_UP in MED
        public float TFR_GammaCorrMult;	//Turnradius multiplier to get safe distance from ground for OBSTACLE in HARD
        public float LantirnCameraX;	//Position of the camera
        public float LantirnCameraY;
        public float LantirnCameraZ;
        public float minTGTMAR;		// 2002-03-22 ADDED BY S.G. Min TGTMAR for this type of aicraft
        public float maxMARIdedStart;	// 2002-03-22 ADDED BY S.G. Max MAR for this type of aicraft when target is ID'ed and below 28K
        public float addMARIded5k;		// 2002-03-22 ADDED BY S.G. Add MAR for this type of aicraft when target is ID'ed and below 5K
        public float addMARIded18k;	// 2002-03-22 ADDED BY S.G. Add MAR for this type of aicraft when target is ID'ed and below 18K
        public float addMARIded28k;	// 2002-03-22 ADDED BY S.G. Add MAR for this type of aicraft when target is ID'ed and below 28K
        public float addMARIdedSpike;	// 2002-03-22 ADDED BY S.G. Add MAR for this type of aicraft when target is ID'ed and spiked
#endif
    }

    /*------------------*/
    /* Engine Data Type */
    /*------------------*/
    public class EngineData
    {
        //TODO ~EngineData(void) {delete mach; delete alt; delete thrust[0];
        // delete thrust[1]; delete thrust[2];};
        public float thrustFactor;
        public float fuelFlowFactor;
        public int numMach;
        public int numAlt;
        public float[] mach;
        public float[] alt;
        public bool hasAB; // JB 010706
        public float[] thrust = new float[3];
#if USE_SH_POOLS
public:
	// Overload new/delete to use a SmartHeap fixed size pool
	void *operator new(size_t size) { return MemAllocPtr(AirframeDataPool, size, 0);	};
	void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
    }

    /*---------------------*/
    /* Roll Rate Data Type */
    /*---------------------*/
    public class RollData
    {
        //TODO ~RollData(void) {delete alpha; delete qbar; delete roll;};
        public int numAlpha;
        public int numQbar;
        public float[] alpha;
        public float[] qbar;
        public float[] roll;
#if USE_SH_POOLS
public:
	// Overload new/delete to use a SmartHeap fixed size pool
	void *operator new(size_t size) { return MemAllocPtr(AirframeDataPool, size, 0);	};
	void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
    }

    public class GearData
    {

        public enum GearFlags
        {
            GearStuck = 0x01,
            GearBroken = 0x02,
            DoorStuck = 0x04,
            DoorBroken = 0x08,
            GearProblem = 0x0F,
        };
        public GearData() { throw new NotImplementedException(); }
        //TODO ~GearData(void) {}
        public float strength;	//how many hitpoints it has left
        public float vel;		//at what rate is it currently compressing/extending in ft/s
        public float obstacle;	//rock height/rut depth
        public uint flags;		//gear stuck/broken, door stuck/broken,
#if USE_SH_POOLS
public:
	// Overload new/delete to use a SmartHeap fixed size pool
	void *operator new(size_t size) { return MemAllocPtr(AirframeDataPool, size, 0);	};
	void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
    }

    public class AirframeClass
    {
#if TODO
#if USE_SH_POOLS
public:
	// Overload new/delete to use a SmartHeap fixed size pool
	void *operator new(size_t size) { ShiAssert( size == sizeof(AirframeClass) ); return MemAllocFS(pool);	};
	void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
	static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(AirframeClass), 200, 0 ); };
	static void ReleaseStorage()	{ MemPoolFree( pool ); };
	static MEM_POOL	pool;
#endif

        public float OPTIMUM_ALPHA = 7.0F;//ME123 FROM 6
        public float OPTIMUM_ALT_M1 = -20.1225F;
        public float OPTIMUM_ALT_M2 = -0.6774F;
        public float OPTIMUM_ALT_B = 59784.5713F;



        // Airframe/Engine data
        internal AeroData aeroData;
        internal AuxAeroData auxaeroData; // JB 010714
        internal EngineData engineData;
        internal RollData rollCmd;
        private float area, fuel, fuelFlow, externalFuel, epuFuel;
        private float mass, weight, dragIndex, emptyWeight;
        private float gsAvail, maxGs, maxRoll, maxRollDelta, startRoll;
        private float minVcas, maxVcas, cornerVcas;
        private uint flags;
        private short vehicleIndex;

        // A bit of a trick to get ground handling to be smooth at low speed
        private float groundAnchorX, groundAnchorY;
        private float groundDeltaX, groundDeltaY;

        // Aerodynamics
        private float clift0, clalph0;
        //float cdalpha;
        private float cnalpha, clalpha;
        private float aoabias;
        private float cl, cd, cy, clalph;
        private float aoamax, aoamin;
        private float betmax, betmin;
        // Thrust Dynamics
        private float thrtab;
        private float thrust;
        private float athrev, anozl, ethrst;
        // Autopilot
        private float forcedHeading, forcedSpeed;

        // Accelerometers
        private float nxcgs, nycgs, nzcgs;
        private float nxcgw, nycgw, nzcgw;

        // Filter Arrays
        //SAVE_ARRAY olde1, olde2, olde3, olde4;
        //SAVE_ARRAY oldvt, oldx,  oldy,  oldz;
        // S.G. I NEED TO ACCES THEM FOR OTHSIDE AirframeClass FUNCTION
        //typedef SIM_FLOAT SAVE_ARRAY[6];
        public float[] oldp01 = new float[6], oldp02 = new float[6], oldp03 = new float[6], oldp04 = new float[6], oldp05 = new float[6];
        public float[] oldr01 = new float[6];
        //SAVE_ARRAY oldy02, oldy04;
        public float[] oldy01 = new float[6], oldy03 = new float[6], olda01 = new float[6];
        //SAVE_ARRAY ptrimArray, rtrimArray, ytrimArray;
        public float[] oldRpm = new float[6];

        // Normalization Params
        private float qbar, qsom, qovt;

        // Control Inputs
        //float ptrim, rtrim, ytrim;
        private float pshape, rshape, yshape;
        private float pstab, rstab;
        private float plsdamp, rlsdamp, ylsdamp;

        //control history
        //float rshape1, rshape2;
        //float pshape1, pshape2;
        //float yshape1, yshape2;
        private float rshape1;
        private float pshape1;
        private float yshape1;
        private float avgPdelta, avgRdelta, avgYdelta;

        //stalls
        private float oscillationTimer;
        private float oscillationSlope;

        private float oldnzcgs;

        // Accelerations
        private float xaero, yaero, zaero;
        private float xsaero, ysaero, zsaero;
        private float xwaero, ywaero, zwaero;
        private float xprop, yprop, zprop;
        private float xsprop, ysprop, zsprop;
        private float xwprop, ywprop, zwprop;

        // Current interpolation breakpoints
        private int curMachBreak, curAlphaBreak;
        private int curRollAlphaBreak, curRollQbarBreak;
        private int curEngMachBreak, curEngAltBreak;

        // Simple flight model
        private int simpleMode;	// 0 = no
        // Functions
        private int ReadData(int idx)
        {
            ShiAssert(idx < num_sets);
            if (idx >= num_sets)
                return 0;

            vehicleIndex = (short)idx;
            /*----------------------------*/
            /* Initialize Inertial Coords */
            /*----------------------------*/
            mach = initialMach;
            if (mach > 3.0F)
                mach *= FTPSEC_TO_KNOTS;
            x = initialX;
            y = initialY;
            z = initialZ;
            sigma = initialPsi;

            emptyWeight = weight = aeroDataset[idx].inputData[AeroDataSet.EmptyWeight];
            area = aeroDataset[idx].inputData[AeroDataSet.Area];
            if (initialFuel < 0.0F)
            {
                fuel = aeroDataset[idx].inputData[AeroDataSet.InternalFuel];
            }
            else if (initialFuel > aeroDataset[idx].inputData[AeroDataSet.InternalFuel])
            {
                fuel = aeroDataset[idx].inputData[AeroDataSet.InternalFuel];
                externalFuel = initialFuel - aeroDataset[idx].inputData[AeroDataSet.InternalFuel];
            }
            else
            {
                fuel = initialFuel;
            }

            weight += (fuel + externalFuel);

            aoamax = aeroDataset[idx].inputData[AeroDataSet.AOAMax];
            aoamin = aeroDataset[idx].inputData[AeroDataSet.AOAMin];
            betmax = aeroDataset[idx].inputData[AeroDataSet.BetaMax];
            betmin = aeroDataset[idx].inputData[AeroDataSet.BetaMin];
            curMaxGs = maxGs = aeroDataset[idx].inputData[AeroDataSet.MaxGs];
            maxRoll = aeroDataset[idx].inputData[AeroDataSet.MaxRoll];
            minVcas = aeroDataset[idx].inputData[AeroDataSet.MinVcas];
            curMaxStoreSpeed = maxVcas = aeroDataset[idx].inputData[AeroDataSet.MaxVcas];//me123 addet curMaxVcas
            cornerVcas = aeroDataset[idx].inputData[AeroDataSet.CornerVcas];

            gear = new GearData[FloatToInt32(aeroDataset[idx].inputData[AeroDataSet.NumGear])];

            for (int i = 0; i < aeroDataset[idx].inputData[AeroDataSet.NumGear]; i++)
            {
                gear[i].strength = 100.0F;	//how many hitpoints it has left
                gear[i].vel = 0.0F;		//at what rate is it currently compressing/extending in ft/s
                gear[i].obstacle = 0.0F;		//rock height/rut depth
                gear[i].flags = 0;
            }

            if (aeroDataset[idx].inputData[AeroDataSet.NumGear] > 1)
                SetFlag(HasComplexGear);
            return (0);
        }

        private void ReadData(float[] inputData, SimlibFileClass inputFile)
        {
            /*-----------------*/
            /* Read in Weight */
            /*-----------------*/
            inputData[AeroDataSet.EmptyWeight] = float.Parse(inputFile.GetNext(),CultureInfo.InvariantCulture);

            /*------------------*/
            /* Read in Ref Area */
            /*------------------*/
            inputData[AeroDataSet.Area] = (float)atof(inputFile.GetNext(),CultureInfo.InvariantCulture);

            /*--------------*/
            /* Read in Fuel */
            /*--------------*/
            inputData[AeroDataSet.InternalFuel] = (float)atof(inputFile.GetNext(),CultureInfo.InvariantCulture);

            /*--------------------------------*/
            /* read in angle of attack limits */
            /*--------------------------------*/

            /*----------------*/
            /* Read in AOAmax */
            /*----------------*/
            inputData[AeroDataSet.AOAMax] = (float)atof(inputFile.GetNext());

            /*----------------*/
            /* Read in AOAmin */
            /*----------------*/
            inputData[AeroDataSet.AOAMin] = (float)atof(inputFile.GetNext());

            /*-----------------*/
            /* Read in BETAmax */
            /*-----------------*/
            inputData[AeroDataSet.BetaMax] = (float)atof(inputFile.GetNext());

            /*-----------------*/
            /* Read in BETAmin */
            /*-----------------*/
            inputData[AeroDataSet.BetaMin] = (float)atof(inputFile.GetNext());

            /*-----------------*/
            /* Read in Max G's */
            /*-----------------*/
            inputData[AeroDataSet.MaxGs] = (float)atof(inputFile.GetNext());

            /*------------------*/
            /* Read in Max Roll */
            /*------------------*/
            inputData[AeroDataSet.MaxRoll] = (float)atof(inputFile.GetNext()) * DTR;

            /*-------------------*/
            /* Read in Min Speed */
            /*-------------------*/
            inputData[AeroDataSet.MinVcas] = (float)atof(inputFile.GetNext());

            /*-------------------*/
            /* Read in Max Speed */
            /*-------------------*/
            inputData[AeroDataSet.MaxVcas] = (float)atof(inputFile.GetNext());

            /*----------------------*/
            /* Read in Corner Speed */
            /*----------------------*/
            inputData[AeroDataSet.CornerVcas] = (float)atof(inputFile.GetNext());

            /*-------------------*/
            /* Read in Theta Max */
            /*-------------------*/
            inputData[AeroDataSet.ThetaMax] = (float)atof(inputFile.GetNext()) * DTR;

            /*-------------------*/
            /* Read in NumGear	*/
            /*-------------------*/
            inputData[AeroDataSet.NumGear] = (float)atof(inputFile.GetNext());

            /*-------------------*/
            /* Read in GearPos	*/
            /*-------------------*/
            for (int i = 0; i < inputData[AeroDataSet.NumGear]; i++)
            {
                inputData[AeroDataSet.NosGearX + i * 4] = (float)atof(inputFile.GetNext());
                inputData[AeroDataSet.NosGearY + i * 4] = (float)atof(inputFile.GetNext());
                inputData[AeroDataSet.NosGearZ + i * 4] = (float)atof(inputFile.GetNext());
                inputData[AeroDataSet.NosGearRng + i * 4] = (float)atof(inputFile.GetNext());
            }

            inputData[AeroDataSet.CGLoc] = (float)atof(inputFile.GetNext());
            inputData[AeroDataSet.Length] = (float)atof(inputFile.GetNext());
            inputData[AeroDataSet.Span] = (float)atof(inputFile.GetNext());
            inputData[AeroDataSet.FusRadius] = (float)atof(inputFile.GetNext());
            inputData[AeroDataSet.TailHt] = (float)atof(inputFile.GetNext());
        }

        private void SuperSimpleFCS();
        private void SuperSimpleEngine();
        private void Accelerometers();
        private void Aerodynamics();
        /********************************************************************/
        /*                                                                  */
        /* Routine: void AirframeClass.AeroRead(char *)                   */
        /*                                                                  */
        /* Description:                                                     */
        /*    Find the right data set for this A/C.                         */
        /*                                                                  */
        /* Inputs:                                                          */
        /*    char *acname  - Aircraft type name (File name)                */
        /*                                                                  */
        /* Outputs:                                                         */
        /*    None                                                          */
        /*                                                                  */
        /*  Development History :                                           */
        /*  Date      Programer           Description                       */
        /*------------------------------------------------------------------*/
        /*  23-Jan-95 LR                  Initial Write                     */
        /*                                                                  */
        /********************************************************************/
        private void AeroRead(int idx)
        {
            ShiAssert(idx < num_sets);
            if (idx >= num_sets)
                return;

            aeroData = aeroDataset[idx].aeroData;
        }

        private void AuxAeroRead(int idx)
        {
            ShiAssert(idx < num_sets);
            if (idx >= num_sets)
                return;

            auxaeroData = aeroDataset[idx].auxaeroData;
        }
        // JB 010714
        private void Atmosphere();
        private void Axial(float dt);

        /********************************************************************/
        /*                                                                  */
        /* Routine: void AirframeClass.EngineRead(char *)                 */
        /*                                                                  */
        /* Description:                                                     */
        /*    Find the right data set for this A/C.                         */
        /*                                                                  */
        /* Inputs:                                                          */
        /*    char *acname  - Aircraft type name (File name)                */
        /*                                                                  */
        /* Outputs:                                                         */
        /*    None                                                          */
        /*                                                                  */
        /*  Development History :                                           */
        /*  Date      Programer           Description                       */
        /*------------------------------------------------------------------*/
        private void EngineRead(int idx)
        {
            ShiAssert(idx < num_sets);
            if (idx >= num_sets)
                return;

            engineData = aeroDataset[idx].engineData;
        }


        private void CalcBodyRates(float dt);
        private void EquationsOfMotion(float dt);
        /********************************************************************/
        /*                                                                  */
        /* Routine: void AirframeClass.FcsRead(char *)                    */
        /*                                                                  */
        /* Description:                                                     */
        /*    Find the right data set for this A/C.                         */
        /*                                                                  */
        /* Inputs:                                                          */
        /*    char *acname  - Aircraft type name (File name)                */
        /*                                                                  */
        /* Outputs:                                                         */
        /*    None                                                          */
        /*                                                                  */
        /*  Development History :                                           */
        /*  Date      Programer           Description                       */
        /*------------------------------------------------------------------*/
        private void FcsRead(int idx)
        {
            ShiAssert(idx < num_sets);
            if (idx >= num_sets)
                return;

            rollCmd = aeroDataset[idx].fcsData;
        }

        /********************************************************************/
        /*                                                                  */
        /* Routine: void AirframeClass.FlightControlSystem (void)         */
        /*                                                                  */
        /* Description:                                                     */
        /*    Do each of the axis in turn.                                  */
        /*                                                                  */
        /* Inputs:                                                          */
        /*    None                                                          */
        /*                                                                  */
        /* Outputs:                                                         */
        /*    None                                                          */
        /*                                                                  */
        /*  Development History :                                           */
        /*  Date      Programer           Description                       */
        /*------------------------------------------------------------------*/
        /*  23-Jan-95 LR                  Initial Write                     */
        /*                                                                  */
        /********************************************************************/
        private void FlightControlSystem()
        {
            Limiter* limiter = NULL;

            limiter = gLimiterMgr.GetLimiter(PitchYawControlDamper, vehicleIndex);
            if (limiter)
            {
                ylsdamp = plsdamp = limiter.Limit(qbar);
            }
            else
            {
                ylsdamp = plsdamp = 1.0F;
            }

            limiter = gLimiterMgr.GetLimiter(RollControlDamper, vehicleIndex);
            if (limiter)
            {
                rlsdamp = limiter.Limit(qbar);
            }
            else
            {
                rlsdamp = 1.0F;
            }

            /*----------------------------*/
            /* gain schedules and filters */
            /*----------------------------*/
            Gains();

            SetStallConditions();

            /*--------------*/
            /* control laws */
            /*--------------*/
            Pitch();
            Roll();
            Yaw();
            Axial(SimLibMinorFrameTime);

            // This is probably unnecessary (it'll happen later)
            // AND it is VERY wasteful...  SCR 8/5/98
            //Trigenometry();
        }


        /********************************************************************/
        /*                                                                  */
        /* Routine: void AirframeClass.Gains(void)                        */
        /*                                                                  */
        /* Description:                                                     */
        /*    Calculate the paramenters for each axis, then do the          */
        /*    feedback paramters need for the digi if needed.               */
        /*                                                                  */
        /* Inputs:                                                          */
        /*    None                                                          */
        /*                                                                  */
        /* Outputs:                                                         */
        /*    None                                                          */
        /*                                                                  */
        /*  Development History :                                           */
        /*  Date      Programer           Description                       */
        /*------------------------------------------------------------------*/
        /*  23-Jan-95 LR                  Initial Write                     */
        /*                                                                  */
        /********************************************************************/
        extern bool g_bNewFm; //TODO Esta en Surface
        private void Gains()
        {
            float pcoef1, pcoef2, pradcl, pfreq1, pfreq2;
            float ycoef1, ycoef2, yradcl, yfreq1, yfreq2;
            float cosmuLim, cosphiLim;
            float omegasp, omegasp1;
            float psmax, nzalpha, ttheta2;
            Limiter* limiter = NULL;
            bool landingGains;

            if (vt == 0.0F)
                return;

            cosphiLim = max(0.0F, platform.platformAngles.cosphi);
            cosmuLim = max(0.0F, platform.platformAngles.cosmu);

            landingGains = gearPos != 0 || IsEngineFlag(FuelDoorOpen) || IsSet(Refueling);
            /*---------------------------------*/
            /* AOA bias for aoa command system */
            /*---------------------------------*/
            /*
            if (clalph0 == 0.0F || IsSet(Planted) )
                aoabias = 0.0F;
            else
            {
                aoabias = (GRAVITY * platform.platformAngles.cosgam *
                             cosmuLim / qsom + 0.1F*gearPos - clift0 * (1.0F + tefFactor * 0.05F)) / clalph0 - tefFactor + lefFactor;
                //aoabias = (GRAVITY * platform.platformAngles.costhe *
                 //		cosphiLim / qsom + 0.1F*gearPos - clift0 * (1.0F + tefFactor * 0.05F)) / clalph0 - tefFactor + lefFactor;
	   
                if(!IsSet(InAir))
                {
                    float bleed = max(0.0F , min(aoabias*(vt - minVcas*KNOTS_TO_FTPSEC*0.5F)/(minVcas*KNOTS_TO_FTPSEC*0.25F), 1.0F));
                    aoabias = max(0.0F,min (bleed, aoamax));
                }
                else
                    aoabias = max(0.0F,min (aoabias, aoamax));
            }*/
            if (IsSet(InAir))
            {
                if (clalph0 == 0.0F)
                    aoabias = 0.0F;
                else
                {
#if NOTHING // JPO - think original is correct 
	       if (g_bNewFm)
				aoabias = (GRAVITY * platform.platformAngles.cosgam *
					cosmuLim / qsom + 0.1F*gearPos - clift0 * (1.0F + tefFactor * auxaeroData.CLtefFactor)) / clalph0 - tefFactor - lefFactor;
			 else
#endif
                    aoabias = (GRAVITY * platform.platformAngles.cosgam *
                        cosmuLim / qsom +
                        0.1F * gearPos -
                        clift0 * (1.0F + tefFactor * auxaeroData.CLtefFactor)) /
                        clalph0 - tefFactor + lefFactor;

                    if (g_bNewFm)
                        aoabias = max(0.0F, min(aoabias, aoamax / 3));
                    else
                        aoabias = max(0.0F, min(aoabias, aoamax));
                }
            }

            /*-------------------*/
            /* AOA or NZ command */
            /*-------------------*/
            gsAvail = aoamax * clalph * qsom / GRAVITY;
            if (IsSet(AutoCommand))
            {
                limiter = gLimiterMgr.GetLimiter(CatIIICommandType, vehicleIndex);
                if (IsSet(CATLimiterIII) && limiter)
                {
                    if (alpha < limiter.Limit(vcas) && (!gearPos || IsSet(GearBroken)))
                        ClearFlag(AOACmdMode);
                    else
                        SetFlag(AOACmdMode);
                }
                else
                {
                    limiter = gLimiterMgr.GetLimiter(CommandType, vehicleIndex);
                    if (limiter)
                    {
                        if (alpha < limiter.Limit(alpha) && (!gearPos || IsSet(GearBroken)))
                            ClearFlag(AOACmdMode);
                        else
                            SetFlag(AOACmdMode);
                    }
                    else if (gsAvail > maxGs && (!gearPos || IsSet(GearBroken)))
                        ClearFlag(AOACmdMode);
                    else
                        SetFlag(AOACmdMode);
                }
            }
            else if (IsSet(GCommand))
                ClearFlag(AOACmdMode);
            else if (IsSet(AlphaCommand))
                SetFlag(AOACmdMode);

            /*---------------------------------------------------*/
            /* pitch rate transfer numerator time constant       */
            /*---------------------------------------------------*/
            nzalpha = clalph0 * qsom * RTD / GRAVITY;
            ttheta2 = vt / (GRAVITY * nzalpha);
            ttheta2 = max(ttheta2, 0.1F);

            /*--------------------------------------*/
            /* pitch axis stick limiter and shaping */
            /*--------------------------------------*/
            pstick = max(-1.0F, min(1.0F, pstick));
            pshape = pstick * pstick;
            if (pstick < 0.0F)
                pshape *= -1.0F;

            /*----------------------------------------*/
            /* pitch axis gains and filter parameters */
            /*----------------------------------------*/
            tp01 = 0.200F;
            zp01 = 0.900F;

            if (!IsSet(Simplified) && simpleMode != SIMPLE_MODE_AF)
            {
                //tp01 *= (1.0F + (loadingFraction - 1.3F) *0.1F);
                zp01 *= (1.0F - 0.15F * (max(0.0F, 1.0F - qbar / 25.0F)) - zpdamp - max(0.0F, (loadingFraction - 1.3F) * 0.01F));
                zp01 = max(0.5F, zp01);
            }

            /*-----------------------------*/
            /* limit closed loop frequency */
            /*-----------------------------*/
            if (pshape > 0.0F)
                kp01 = maxGs - platform.platformAngles.costhe * cosphiLim;
            else
                kp01 = 4.0F + platform.platformAngles.costhe * cosphiLim;

            kp02 = 1.000F;
            kp03 = 2.000F;

            omegasp1 = 1.0F / (ttheta2 * 0.65F);
            omegasp1 = max(1.0F, omegasp1);
            omegasp = omegasp1;

            if (stallMode > Recovering || !IsSet(InAir))
            {
                omegasp *= 2.0F;
            }
            else
            {
                limiter = gLimiterMgr.GetLimiter(LowSpeedOmega, vehicleIndex);
                if (limiter)
                    omegasp *= limiter.Limit(qbar);
            }

            wp01 = omegasp;

            /*----------------------------------------------*/
            /* calculate inner loop dynamics for pitch axis */
            /*----------------------------------------------*/
            pcoef1 = tp01 * wp01 * wp01 -
                      2.0F * zp01 * wp01 - kp03;
            pcoef2 = 2.0F * zp01 * wp01 * kp03 -
                      kp03 * tp01 * wp01 * wp01;
            pradcl = max((pcoef1 * pcoef1 - 4.0F * pcoef2), 0.0F);

            pfreq1 = ((float)sqrt(pradcl) - pcoef1) * 0.5F;
            pfreq2 = -pcoef1 - pfreq1;

            /*------------------------------------------*/
            /* time constants for pitch axis inner loop */
            /*------------------------------------------*/
            tp02 = 1 / pfreq1;
            tp03 = 1 / pfreq2;

            tp03 = max(tp03, 0.5F);

            if (IsSet(AOACmdMode) || !(qsom * cnalpha))
                kp05 = tp02 * tp03 * wp01 * wp01;
            else
                kp05 = GRAVITY * tp02 * tp03 * wp01 * wp01 /
                               (qsom * cnalpha);

            if (landingGains)
                kp05 *= auxaeroData.pitchGearGain;

            if (!IsSet(InAir))
            {
                kp05 *= max(0.0f, min(1.0F, (qbar - 20.0F) / 45.0F));
            }

            F4Assert(!_isnan(kp05));

            /*---------------------------------------*/
            /* roll axis gains and filter parameters */
            /*---------------------------------------*/
            if (qbar >= 250.0F)
                tr01 = 0.25F;
            else
                tr01 = -0.001111F * (qbar - 100.0F) + 0.416F;

            /*-----------------------------------------------*/
            /* roll command gain and stick limiter / shaping */
            /*-----------------------------------------------*/
            rstick = min(max(rstick, -1.0F), 1.0F);
            rshape = rstick * rstick;
            if (rstick < 0.0F)
                rshape *= -1.0F;

            psmax = Math.TwodInterp(alpha, qbar, rollCmd.alpha, rollCmd.qbar,
                              rollCmd.roll, rollCmd.numAlpha, rollCmd.numQbar,
                              &curRollAlphaBreak, &curRollQbarBreak);
            kr01 = psmax * DTR;

            if (landingGains)
                kr01 *= auxaeroData.rollGearGain;

            kr02 = platform.platformAngles.cosalp;

            /*--------------------------------------*/
            /* yaw axis gains and filter parameters */
            /*--------------------------------------*/
            //zy01 = 0.50F;
            zy01 = 0.70F;

            //wy01 = (0.8F/tr01);
            wy01 = (0.3F / tr01);

            if (!IsSet(Simplified) && simpleMode != SIMPLE_MODE_AF)
                wy01 *= (1.0F - loadingFraction * 0.1F);

            ky01 = 1.000F;
            ky02 = 1.000F;
            ky03 = 2.000F;

            /*--------------------------------------------*/
            /* calculate inner loop dynamics for yaw axis */
            /*--------------------------------------------*/
            ycoef1 = -2.0F * zy01 * wy01 - ky03;
            ycoef2 = 2.0F * zy01 * wy01 * ky03;
            yradcl = ycoef1 * ycoef1 - 4.0F * ycoef2;
            if (yradcl < 0.0F) yradcl = 0.0F;

            yfreq1 = ((float)sqrt(yradcl) - ycoef1) * 0.5F;
            yfreq2 = -ycoef1 - yfreq1;

            /*----------------------------------------*/
            /* time constants for yaw axis inner loop */
            /*----------------------------------------*/
            ty01 = 1 / yfreq1;
            ty02 = 1 / yfreq2;

            if (cy != 0.0F)
            {
                ky05 = -GRAVITY * wy01 * wy01 / (qsom * cy * yfreq1 * yfreq2);
            }

            /*------------------------------------*/
            /* yaw axis pedal limiter and shaping */
            /*------------------------------------*/
            ypedal = min(max(ypedal, -1.0F), 1.0F);
            yshape = ypedal * ypedal;
            if (ypedal < 0.0F)
                yshape *= -1.0F;

            if (landingGains)
                ky05 *= auxaeroData.yawGearGain;
        }

        private void Initialize();
        private void InitializeEOM();
        private void ReInitialize();
        private void OpenFiles();
        private void Pitch();
        //TODO private void ReadData(float *);
        private void Roll();
        private void Trigenometry();
        private void TrimModel();
        private void Yaw();
        private float CalcMach(float kias, float pressRatio);
        private float CalcPressureRatio(float alt, float* ttheta, float* rsigma);
        private float Predictor(float x1, float x2, float y1, float y2);
        private void SetStallConditions();
        private void ResetIntegrators();
        private float CalculateVt(float dt);
        private void SetGroundPosition(float dt, float netAccel, float gndGmma, float relMu);
        private void CalculateGroundPlane(float* gndGmma, float* relMu);
        private void CalcGroundTurnRate(float dt);


        public void TEFClose();
        public void TEFMax();
        public void TEFInc();
        public void TEFDec();
        public void TEFTakeoff();
        public void TEFLEFStage1();
        public void TEFLEFStage2();
        public void TEFLEFStage3();
        public void LEFClose();
        public void LEFMax();
        public void LEFInc();
        public void LEFDec();
        public void LEFTakeoff();
        public void SetFlaps(bool islanding);
        public void GetGunLocation(out float x, out float y, out float z) { x = auxaeroData.gunLocation.x; y = auxaeroData.gunLocation.y; z = auxaeroData.gunLocation.z; }
        public int EngineTrail();
        public float EngineSmokeFactor();
        public void GetRefuelPosition(out Tpoint pos) { pos = auxaeroData.refuelLocation; }
        public float GetRollLimitForAiInWP() { return auxaeroData.rollLimitForAiInWP; } // 2002-01-01 ADDED BY S.G.
        public float GetMinTGTMAR() { return auxaeroData.minTGTMAR; } // 2002-03-22 ADDED BY S.G.
        public float GetMaxMARIdedStart() { return auxaeroData.maxMARIdedStart; } // 2002-03-22 ADDED BY S.G.
        public float GetAddMARIded5k() { return auxaeroData.addMARIded5k; } // 2002-03-22 ADDED BY S.G.
        public float GetAddMARIded18k() { return auxaeroData.addMARIded18k; } // 2002-03-22 ADDED BY S.G.
        public float GetAddMARIded28k() { return auxaeroData.addMARIded28k; } // 2002-03-22 ADDED BY S.G.
        public float GetAddMARIdedSpike() { return auxaeroData.addMARIdedSpike; } // 2002-03-22 ADDED BY S.G.

        public int GetEngineDamageStopThreshold() { return auxaeroData.engineDamageStopThreshold; } // 2002-04-11 ADDED BY S.G.
        public int GetEngineDamageNoRestartThreshold() { return auxaeroData.engineDamageNoRestartThreshold; } // 2002-04-11 ADDED BY S.G.
        public int GetEngineDamageHitThreshold() { return auxaeroData.engineDamageHitThreshold; } // 2002-04-11 ADDED BY S.G.

        //MI TFR
        public bool HasTFR() { return auxaeroData.Has_TFR > 0; }
        public float GetPID_K() { return auxaeroData.PID_K; }
        public float GetPID_KI() { return auxaeroData.PID_KI; }
        public float GetPID_KD() { return auxaeroData.PID_KD; }
        public bool GetTFR_LimitMX() { return auxaeroData.TFR_LimitMX > 0; }
        public float GetTFR_Corner() { return auxaeroData.TFR_Corner; }
        public float GetTFR_Gain() { return auxaeroData.TFR_Gain; }
        public float GetEVA_Gain() { return auxaeroData.EVA_Gain; }
        public float GetTFR_MaxRoll() { return auxaeroData.TFR_MaxRoll; }
        public float GetTFR_SoftG() { return auxaeroData.TFR_SoftG; }
        public float GetTFR_MedG() { return auxaeroData.TFR_MedG; }
        public float GetTFR_HardG() { return auxaeroData.TFR_HardG; }
        public float GetTFR_Clearance() { return auxaeroData.TFR_Clearance; }
        public float GetSlowPercent() { return auxaeroData.SlowPercent; }
        public float GetTFR_lookAhead() { return auxaeroData.TFR_lookAhead; }
        public float GetEVA1_SoftFactor() { return auxaeroData.EVA1_SoftFactor; }
        public float GetEVA2_SoftFactor() { return auxaeroData.EVA2_SoftFactor; }
        public float GetEVA1_MedFactor() { return auxaeroData.EVA1_MedFactor; }
        public float GetEVA2_MedFactor() { return auxaeroData.EVA2_MedFactor; }
        public float GetEVA1_HardFactor() { return auxaeroData.EVA1_HardFactor; }
        public float GetEVA2_HardFactor() { return auxaeroData.EVA2_HardFactor; }
        public float GetTFR_GammaCorrMult() { return auxaeroData.TFR_GammaCorrMult; }
        public float GetLantirnCameraX() { return auxaeroData.LantirnCameraX; }
        public float GetLantirnCameraY() { return auxaeroData.LantirnCameraY; }
        public float GetLantirnCameraZ() { return auxaeroData.LantirnCameraZ; }

        public void EngineModel(float dt)
        { throw new NotImplementedException(); }
        public float Cd() { return cd; }
        public float Cl() { return cl; }
        public float Cy() { return cy; }
        public float XAero() { return xaero; }
        public float YAero() { return yaero; }
        public float ZAero() { return zaero; }
        public float XProp() { return xprop; }
        public float YProp() { return yprop; }
        public float ZProp() { return zprop; }
        public float XSAero() { return xsaero; }
        public float YSAero() { return ysaero; }
        public float ZSAero() { return zsaero; }
        public float XSProp() { return xsprop; }
        public float YSProp() { return ysprop; }
        public float ZSProp() { return zsprop; }
        public float AOABias() { return aoabias; }

        public float Thrust() { return thrust; }

        /*------------------------*/
        /* Command mode constants */
        /*------------------------*/
        public enum CommandMode
        {
            IsDigital = 0x1,
            InAir = 0x2,
            Trimming = 0x4,
            WheelBrakes = 0x8,
            Refueling = 0x10,
            AOACmdMode = 0x20,
            AutoCommand = 0x40,
            GCommand = 0x80,
            ErrorCommand = 0x100,
            GroundCommand = 0x200,
            AlphaCommand = 0x400,
            GearBroken = 0x800,
            Planted = 0x1000,
            Simplified = 0x2000,
            NoFuelBurn = 0x4000,
            EngineOff = 0x8000,
            ThrottleCheck = 0x10000,
            SuperSimple = 0x20000,
            MPOverride = 0x40000,
            LowSpdHorn = 0x80000,
            HornSilenced = 0x100000,
            CATLimiterIII = 0x200000,
            NoseSteerOn = 0x400000,
            OverRunway = 0x800000,
            HasComplexGear = 0x1000000,
            GearDamaged = 0x2000000,
            OverAirStrip = 0x4000000,
            EngineStopped = 0x8000000,
            JfsStart = 0x10000000,
            // EpuRunning = 0x20000000, // no longer required
            // JB carrier
            OnObject = 0x40000000,
            Hook = 0x80000000,
            // JB carrier
        }

        public enum StallMode
        {
            None,
            Crashing,
            Recovering,
            EnteringDeepStall,
            DeepStall,
            Spinning,
            FlatSpin
        }

        // State data
        public AircraftClass platform;

        public float e1, e2, e3, e4;
        public float x, y, z;
        public float vt, vcas, rho;
        public float p, q, r;
        public float mach, vRot;
        public float alpha, beta;
        public float theta, phi, psi;
        public float gmma, sigma, mu;
        public float nxcgb, nycgb, nzcgb;
        public float xdot, ydot, zdot, vtDot;
        public float rpm;
        //float limiterAssault;
        public float stallMagnitude;
        public float desiredMagnitude;
        public float loadingFraction;
        public float assymetry;
        public float tefFactor;
        public float lefFactor;
        public float curMaxGs;
        public StallMode stallMode;

        public GearData[] gear;
        public int groundType;
        public float grndphi, grndthe, groundZ;
        public float bumpphi, bumpthe, bumpyaw;
        public Tpoint gndNormal;

        // Geometry Stuff
        public float alpdot;
        public float betdot;
        public float slice;
        public float pitch;

        // Initialization Data
        public float initialX;
        public float initialY;
        public float initialZ;
        public float initialMach;
        public float initialPsi;
        public float initialFuel;

        // Flight Control System Gains
        public float tp01, tp02, tp03, tp04;
        public float zp01, zp02;
        public float kp01, kp02, kp03, kp04, kp05, kp06;
        public float wp01, wp02;
        public int jp01, jp02;
        public float tr01;
        public float zr01;
        public float kr01, kr02, kr03, kr04;
        public float wr01;
        public float ty01, ty02, ty03;
        public float zy01, zy02;
        public float ky01, ky02, ky03, ky04, ky05, ky06;
        public float wy01, wy02;
        public int jy01, jy02;
        public float zpdamp;

        // Pilot Commands
        public float pstick, rstick, ypedal, throtl, pwrlev;
        public float ptrmcmd, rtrmcmd, ytrmcmd;
        public float dbrake, gearPos, gearHandle, speedBrake;
        public float hookPos, hookHandle; // JB carrier
        public bool altGearDeployed;
        public float lefPos, tefPos; // JPO - for manual LEF/TEF
        public enum DragChuteState
        {
            DRAGC_STOWED = 0x00,
            DRAGC_DEPLOYED = 0x01,
            DRAGC_TRAILING = 0x02,
            DRAGC_JETTISONNED = 0x04,
            DRAGC_RIPPED = 0x08,
        };
        public DragChuteState dragChute; // drag chute deployed or not.
        public bool HasDragChute() { return auxaeroData.dragChuteCd > 0; }
        public float DragChuteMaxSpeed() { return auxaeroData.dragChuteMaxSpeed; }
        public bool HasManualFlaps() { return auxaeroData.hasTef == AUX_LEFTEF_MANUAL; }
        public float TefDegrees() { return tefFactor * auxaeroData.tefMaxAngle; }
        public float LefDegrees() { return lefFactor * auxaeroData.lefMaxAngle; }
        public bool canopyState; // open or shut canopy
        public void CanopyToggle()
        {
            if (canopyState == true)
                canopyState = false;
            else if (!IsSet(InAir))
                canopyState = true;
        }


        public enum EpuState
        { // JPO state of the EPU switch
            OFF, AUTO, ON
        }
        public EpuState epuState;
        public EpuState GetEpuSwitch() { return epuState; }
        public void SetEpuSwitch(EpuState mode) { epuState = mode; }
        public void StepEpuSwitch();
        // epu status and stuff
        [Flags]
        enum EpuBurnState { EpuNone = 0x0, EpuHydrazine = 0x1, EpuAir = 0x2 }
        public EpuBurnState epuBurnState;
        public void EpuSetHydrazine() { epuBurnState |= EpuBurnState.EpuHydrazine; }
        public void EpuSetAir() { epuBurnState |= EpuBurnState.EpuAir; }
        public void EpuClear() { epuBurnState = EpuBurnState.EpuNone; }
        public bool EpuIsAir() { return (epuBurnState & EpuBurnState.EpuAir) ? true : false; }
        public bool EpuIsHydrazine() { return (epuBurnState & EpuBurnState.EpuHydrazine) ? true : false; }

        public HYDR hydrAB; // JPO - state of the hydraulics system
        [Flags]
        public enum HYDR
        {
            HYDR_A_SYSTEM = 0x01, HYDR_B_SYSTEM = 0x02,
            HYDR_ALL = HYDR_A_SYSTEM | HYDR_B_SYSTEM,
            HYDR_A_BROKE = 0x04, HYDR_B_BROKE = 0x08, // whats permanently broke?
        }

        public HYDR HydraulicA() { return (hydrAB & HYDR_A_SYSTEM); }
        public HYDR HydraulicB() { return (hydrAB & HYDR_B_SYSTEM); }
        public HYDR HydraulicOK() { return hydrAB == HYDR_ALL ? 1 : 0; }
        public void SetHydraulic(int type) { hydrAB = type; }
        public void HydrBreak(int sys);
        public void HydrDown(int sys) { hydrAB &= ~(sys & HYDR_ALL); }
        public void HydrRestore(int sys);

        public float jfsaccumulator;
        public float JFSSpinTime;	//MI
        public void JfsEngineStart();
        public void QuickEngineStart();
        public float curMaxStoreSpeed;//me123
        public bool LLON, PBON; //MI for LandingLight and Parkingbrake
        public bool BrakesToggle;	//MI for new Speedbrake
        public void ToggleLL();
        public void TogglePB();
        public void ToggleHook(); // JB carrier
        public SAVE_ARRAY oldFtit;
        public float ftit; // Forward Turbine Inlet Temp (Degrees C) / 100
        [Flags]
        public enum EngineFlags
        {
            WingFirst = 0x1,
            MasterFuelOff = 0x2,
            FuelDoorOpen = 0x4,
        };
        public EngineFlags engineFlags;
        public int IsEngineFlag(EngineFlags ef) { return (engineFlags & ef) ? 1 : 0; }
        public void SetEngineFlag(EngineFlags ef) { engineFlags |= ef; }
        public void ClearEngineFlag(EngineFlags ef) { engineFlags &= ~ef; }
        public void ToggleEngineFlag(EngineFlags ef) { engineFlags ^= ef; }
        public enum FuelSwitch
        {
            // ordered so that the first one is the defautl cockpit switch.
            FS_NORM, FS_RESV, FS_WINGINT, FS_WINGEXT, FS_CENTEREXT, FS_TEST,
            FS_FIRST = FS_NORM, FS_LAST = FS_TEST, // for wrap around
        }
        FuelSwitch fuelSwitch;
        public void IncFuelSwitch();
        public void DecFuelSwitch();
        public void SetFuelSwitch(FuelSwitch fs) { fuelSwitch = fs; }
        public FuelSwitch GetFuelSwitch() { return fuelSwitch; }
        public enum FuelPump
        {
            // ordered so that the first one is the defautl cockpit switch.
            FP_OFF, FP_NORM, FP_AFT, FP_FWD,
            FP_FIRST = FP_OFF, FP_LAST = FP_FWD,
        }
        public FuelPump fuelPump;
        public void IncFuelPump();
        public void DecFuelPump();
        public void SetFuelPump(FuelPump fp) { fuelPump = fp; }
        public FuelPump GetFuelPump() { return fuelPump; }
        public enum TANK
        {
            TANK_FWDRES, TANK_AFTRES, TANK_F1, TANK_A1, TANK_WINGFR, TANK_WINGAL,
            TANK_MAXINTERNAL = TANK_WINGAL, TANK_REXT, TANK_LEXT, TANK_CLINE, MAX_FUEL
        };
        public float[] m_tanks = new float[MAX_FUEL]; // tank current capacity
        public float[] m_tankcap = new float[MAX_FUEL]; // tank max capacity
        public float[] m_trate = new float[MAX_FUEL]; // tank transfer rate (the from tank).
        public float AvailableFuel();
        public float GetJoker();
        public float GetBingo();
        public float GetFumes();
        public int CheckTrapped();
        public int CheckHome();
        public int HomeFuel;
        public enum AirSource
        {
            // ordered so that the first one is the defautl cockpit switch.
            AS_OFF, AS_NORM, AS_DUMP, AS_RAM,
            AS_FIRST = AS_OFF, AS_LAST = AS_RAM
        }
        public AirSource airSource;
        public void IncAirSource();
        public void DecAirSource();
        public void SetAirSource(AirSource airs) { airSource = airs; }
        public AirSource GetAirSource() { return airSource; }

        [Flags]
        public enum Generator
        { // come in pairs - first is active/inactive 2nd ok/broke
            GenNone = 0x0,
            GenFlcsPmg = 0x1,
            GenFlcsPmgBroke = 0x2,
            GenEpu = 0x4,
            GenEpuBroke = 0x8,
            GenEpuPmg = 0x10,
            GenEpuPmgBroke = 0x20,
            GenStdby = 0x40,
            GenStdbyBroke = 0x80,
            GenStdbyPmg = 0x100,
            GenStdbyPmgBroke = 0x200,
            GenMain = 0x40,
            GenMainBroke = 0x80,
        };
        public Generator generators;
        public bool GeneratorRunning(Generator gen) { return (generators & gen) ? true : false; }
        public bool GeneratorOK(Generator gen) { return (generators & (gen << 1)) ? false : true; }
        public void GeneratorOn(Generator gen) { if (GeneratorOK(gen)) generators |= gen; }
        public void GeneratorOff(Generator gen) { generators &= ~gen; }
        public void GeneratorBreak(Generator gen) { generators |= (gen << 1); GeneratorOff(gen); }

        public float nozzlePos;

        // Functions
        public AirframeClass(AircraftClass self);
        //TODO public ~AirframeClass ();
        public void Init(int idx);
        public void InitData(int idx);
        public void Reinit();
        public void Exec();
        public void RemoteUpdate();
        public void ResetOrientation();
        public void CalcBodyOrientation(float dt);

        public void YawIt(float betcmd, float dt);
        public void PitchIt(float aoacmd, float dt);
        public void RollIt(float pscmd, float dt);
        public float CheckHeight();
        public void CheckGroundImpact(float dt);

        public float GetOptimumCruise();
        public float GetOptimumAltitude();
        public float CalcThrotlPos(float speed);
        public float CalcMuFric(int groundType);
        public float CalcDesAlpha(float desGs);
        public float CalcDesSpeed(float desAlpha);
        public void SetPStick(float newStick) { pstick = newStick; }
        public void SetRStick(float newStick) { rstick = newStick; }
        public void SetYPedal(float newPedal) { ypedal = newPedal; }
        public void SetThrotl(float newThrot) { throtl = newThrot; }
        public float PStick() { return pstick; }
        public float RStick() { return rstick; }
        public float YPedal() { return ypedal; }
        public float Throtl() { return throtl; }
        public float Mass() { return mass; }
        public float MaxRoll() { return maxRoll * RTD; }
        public void SetMaxRoll(float newRoll) { maxRoll = newRoll * DTR; }
        public void SetMaxRollDelta(float newRoll) { maxRollDelta = newRoll * DTR; startRoll = 0.0F; }
        public void ReSetMaxRoll()
        {
            maxRoll = aeroDataset[vehicleIndex].inputData[AeroDataSet.MaxRoll];
        }
        public float MaxGs() { return maxGs; }
        public float MinVcas() { return minVcas; }
        public float MaxVcas() { return maxVcas; }
        public float CalcTASfromCAS(float cas);
        public float CornerVcas() { return cornerVcas; }
        public float GsAvail() { return gsAvail; }
        public float SustainedGs(int maxAB);
        public float PsubS(int maxAB);
        public float FuelBurn(int maxAB);
        public float Fuel() { return fuel; }
        public float ExternalFuel() { return externalFuel; }
        public void AddExternalFuel(float lbs);
        public int AddFuel(float lbs); //this checks whether we're full
        public void ClearFuel();
        public void AllocateFuel(float totalfuel);
        public int BurnFuel(float bfuel);
        public void RecalculateFuel();
        public void FeedTank(int t1, int t2, float dt);
        public float FuelFlow() { return fuelFlow; }
        public void FuelTransfer(float dt);
        public void DropTank(int n);
        public void GetFuel(float* fwdp, float* aftp, float* total);
        public void FindExternalTanks();
        public float EPUFuel() { return epuFuel; }
        public float VtDot() { return vtDot; }
        public float WingArea() { return area; }
        public int VehicleIndex() { return vehicleIndex; }
        public void AddWeapon(float weight, float dragIndex, float offset);
        public void RemoveWeapon(float Weight, float DragIndex, float offset);
        public void SetForcedConditions(float newSpeed, float newHeading) { forcedSpeed = newSpeed; forcedHeading = newHeading; }
        public float Qsom() { return qsom; }
        public float Cnalpha() { return cnalpha; }
        public int NumGear() { return FloatToInt32(aeroDataset[vehicleIndex].inputData[AeroDataSet.NumGear]); }
        public float GetAeroData(int which) { return aeroDataset[vehicleIndex].inputData[which]; }
        public void DragBodypart();

        public void SetDragIndex(float index) { dragIndex = index; }
        public float GetDragIndex() { return dragIndex; }
        public float GetMaxCurrentRollRate();
        public void ResetAlpha();
        public void SetFlag(int newFlag) { flags |= newFlag; }
        public void ClearFlag(int newFlag) { flags &= ~newFlag; }
        public int IsSet(int testFlag) { return flags & testFlag ? 1 : 0; }
        // KCK added function - this is really only needed temporarily
        public void SetPosition(float x, float y, float z);
        public void ResetFuel();

        // simple flight mode stuff....
        public void SimpleModel()
        {
            float tmp;

            // "springy" constants for pitch and roll
            float kPitch = 0.75f;
            float kRoll = 1.0f;

            float ctlpitch;
            float ctlroll;

            float dT;
            float newVt;
            float maxBank;
            float maxTheta;
            float liftO_alp = (0.5F * rho * vt * vt * area * clalph0);
            float gmmaDes;

            // get inputs
            ctlpitch = pstick;
            ctlroll = rstick;

            dT = SimLibMajorFrameTime;


            // NOTE: 
            //	p     = roll rate (around X axis)
            //	q     = pitch rate (around Y axis)
            //	r     = yaw rate (around Z axis)
            //	phi   = roll (around X axis)
            //	theta = pitch (around Y axis)
            //	psi   = yaw (around Z axis)

            // handle body rates -- flying
            if (IsSet(InAir))
            {
                // we're not nose planted
                ClearFlag(Planted);
                ClearFlag(NoseSteerOn);

                // pitch rate
                // Modify pitch rate if going really slow
                if (qsom * cnalpha < 1.5F && !(playerFlightModelHack &&
                       platform == SimDriver.playerEntity &&
                       platform.AutopilotType() == AircraftClass.APOff))
                {
                    gmmaDes = (1.0F - qsom * cnalpha / 1.5F) * -45.0F * DTR;
                }
                else
                {
                    gmmaDes = 0.0F;
                }

                if (ctlpitch)
                {
                    // pitch where we want to be
                    maxTheta = min(MAX_AF_PITCH, aeroDataset[vehicleIndex].inputData[AeroDataSet.ThetaMax]);
                    tmp = ctlpitch * MAX_AF_PITCH;
                    q = (tmp - gmma + gmmaDes) * kPitch;
                }
                else
                {
                    // snap back to level flight when no input
                    q = (-gmma + gmmaDes) * kPitch;
                }

                // roll rate and yawrate are tied together
                // NOTE: Roll is only +/- 90, if we want to fly inverted we'll likely
                // want to have a specific call to set inverted
                if (ctlroll)
                {
                    //it looks like we're sliding around the turns, need to be rolled more
                    maxBank = min(MAX_AF_ROLL, aeroDataset[vehicleIndex].inputData[AeroDataSet.MaxRoll]);

                    //me123 dont bank more then you can keep the nose up with your max gs availeble
                    if (g_bSimpleFMUpdates && gearPos < 0.7F && platform.DBrain().GetCurrentMode() != DigitalBrain.LandingMode && platform.DBrain().GetCurrentMode() != DigitalBrain.RefuelingMode)
                        maxBank = min(maxBank, acos(1 / (max(min(curMaxGs, gsAvail), 1.5))));

                    tmp = ctlroll * maxBank * 1.2F;
                    p = (tmp - mu) * kRoll;

                    // maximum yaw rate will occur at +/-90 deg or when the ctrl is
                    //ME123 this eliminates the skiddign in the turns
                    float turnangle = phi;
                    if (turnangle < -maxBank)
                        turnangle = -maxBank;

                    if (turnangle > maxBank)
                        turnangle = maxBank;
                    //me123 if the tanker is refuling a player then use FMupdate
                    // 2002-04-02 MN only use that when tanker enters the track pattern (reached first track point), not when heading for the first trackpoint
                    if (g_bTankerFMFix && platform.DBrain() && platform.DBrain().IsTanker() &&
                        platform.TBrain() && platform.TBrain().IsSet(TankerBrain.IsRefueling) &&
                        platform.TBrain().ReachedFirstTrackPoint() ||
                        g_bSimpleFMUpdates && vt > 1 && gearPos < 0.7F &&
                        platform.DBrain().GetCurrentMode() != DigitalBrain.LandingMode &&
                        platform.DBrain().GetCurrentMode() != DigitalBrain.RefuelingMode)
                        r = (360.0f * GRAVITY * tan(turnangle) / (2 * PI * vt)) * DTR;
                    else
                        r = (ctlroll * MAX_AF_YAWRATE);
                }
                else
                {
                    // snap roll back to level flight
                    p = (-mu) * kRoll;

                    // slow and stop yawing
                    r = -r;
                    if (r < 2.0F * DTR)
                        r = 0.0F;
                }

                if (!(playerFlightModelHack &&
                     platform == SimDriver.playerEntity &&
                     platform.AutopilotType() == AircraftClass.APOff))
                {

                    q -= max(1.0F - (qsom * cnalpha / 0.8F), 0.0F) * (float)atan(platform.platformAngles.cosmu * platform.platformAngles.cosgam * GRAVITY / max(vt, 4.0F));
                }
            }
            // handle body rates -- on ground with nose planted
            else
            {
                // snap any roll back to level
                p = -mu;

                // yaw rate coupled to lateral stick and speed
                ctlroll = max(min(2.0F * ctlroll, 1.0F), -1.0F);
                r = ctlroll * min(vt / 25.0F, 1.0F);

                // pitch rate
                // only can pull back on ground when we can achieve 1G with 10 degrees AOA
                //note: we want to make sure the plane takes off, so even if we couldn't 
                //get off the ground, we lift off
                if (ctlpitch > 0.0f && (-zaero > GRAVITY || oldp03[2] == 13.0F))
                {
                    // pitch where we want to be
                    tmp = ctlpitch * MAX_AF_PITCH;
                    q = (tmp - gmma) * kPitch;
                }
                else
                {
                    // snap back to level when no input
                    q = (-gmma);
                }
            }

            // integrate angular velocities to roll, pitch and yaw
            mu = mu + dT * p;
            sigma = sigma + dT * r;
            gmma = gmma + dT * q;

            if (mu > MAX_AF_ROLL)
            {
                mu = MAX_AF_ROLL;
                p = 0.0f;
            }
            else if (mu < -MAX_AF_ROLL)
            {
                mu = -MAX_AF_ROLL;
                p = 0.0f;
            }

            if (gmma > MAX_AF_PITCH)
            {
                gmma = MAX_AF_PITCH;
                q = 0.0f;
            }
            else if (gmma < -MAX_AF_PITCH)
            {
                gmma = -MAX_AF_PITCH;
                q = 0.0f;
            }

            if (sigma > DTR * 180.0f)
                sigma -= 360.0f * DTR;
            else if (sigma < -180.0f)
                sigma += 360.0f * DTR;

            // Given gs, we can get alpha from the standard lift equation and Cl-alpha
            // Assume 1 G for wings level, max at 7 gs at +/- 90 degrees roll
            float desiredGs = 1.0F;
            if (!IsSet(InAir))
            {
                //nzcgs = nzcgb = max(0.0F, (15.0F - weight/liftO_alp)*0.25F);
                if (ctlpitch > 0.0F)
                    desiredGs = max(0.0F, (15.0F - weight / liftO_alp) * 0.25F);
                else
                    desiredGs = 0.0F;
            }
            else
            {
                //nzcgb = 1.0F + 6.0F * fabs (mu / (MAX_AF_ROLL)) * liftO_alp/weight * 5.0F;
                if (platform.platformAngles.cosmu)
                    desiredGs = 1.0F / platform.platformAngles.cosmu;
                else
                    desiredGs = maxGs;
                desiredGs = min(desiredGs, maxGs);
                //nzcgs = nzcgb = min (nzcgb, maxGs);
            }

            //this needs to always be calculated, otherwise it is impossible to taxi or takeoff!
            // Find new velocity
            vtDot = xwaero + xwprop - GRAVITY * platform.platformAngles.singam;

            float netAccel = CalculateVt(dT);

            // edg: my mr steen mode to allow hovering!
            // speed is directly based on throtl
            // speed up yawrate
            if (playerFlightModelHack &&
                 platform == SimDriver.playerEntity &&
                 platform.AutopilotType() == AircraftClass.APOff)
            {
                float oldvt;

                oldvt = vt;

                vt = throtl * 3000.0f;
                vtDot = vt - oldvt;

                // double the yawrate
                sigma = sigma - dT * r;
                r *= 2.0f;
                sigma = sigma + dT * r;
                if (sigma > DTR * 180.0f)
                    sigma -= 360.0f * DTR;
                else if (sigma < -180.0f)
                    sigma += 360.0f * DTR;

                // if in air we don't want alpha effects
                nzcgb = 0.0f;

                newVt = vt + vtDot * dT;

                if (newVt > 0.0F)
                    vt = newVt;
                else
                    vt = 0.00001F;
            }



            if (vt && !(playerFlightModelHack &&
                 platform == SimDriver.playerEntity &&
                 platform.AutopilotType() == AircraftClass.APOff))
            {
                Gains();

                if (!IsSet(InAir))
                {
                    if (desiredGs)
                        tmp = CalcDesAlpha(desiredGs);
                    else
                        tmp = 0.0F;
                    tmp = min(max(tmp, 0.0F), 13.0F);
                }
                else
                {
                    tmp = CalcDesAlpha(desiredGs);
                    tmp = min(max(tmp, -5.0F), 13.0F);
                }
                PitchIt(tmp, dT);
            }
            else
            {
                alpha = 0.0F;
                oldp03[1] = alpha;
                oldp03[2] = alpha;
                oldp03[4] = alpha;
                oldp03[5] = alpha;
            }


            beta = 0.0f;

            // Find angles and sin/cos
            Trigenometry();

            // get delta x y and z entirely based on the direction we're pointing
            if (vt > 0.00001F)
            {
                xdot = vt * platform.platformAngles.cosgam *
                        platform.platformAngles.cossig;
                ydot = vt * platform.platformAngles.cosgam *
                        platform.platformAngles.sinsig;
                zdot = -vt * platform.platformAngles.singam;
                ShiAssert(!_isnan(xdot));
                ShiAssert(!_isnan(ydot));
                ShiAssert(!_isnan(zdot));
            }
            else
            {
                xdot = 0.0F;
                ydot = 0.0F;
                zdot = 0.0F;
                vt = 0.0001f;
            }



            if (!IsSet(InAir))
            {
                groundDeltaX += dT * xdot * gSpeedyGonzales;
                groundDeltaY += dT * ydot * gSpeedyGonzales;
                x = groundAnchorX + groundDeltaX;
                y = groundAnchorY + groundDeltaY;
                z = z + dT * zdot * gSpeedyGonzales;
            }
            else
            {
                x = x + dT * xdot * gSpeedyGonzales;
                y = y + dT * ydot * gSpeedyGonzales;
                z = z + dT * zdot * gSpeedyGonzales;
                groundAnchorX = x;
                groundAnchorY = y;
                groundDeltaX = 0.0f;
                groundDeltaY = 0.0f;
            }

            ShiAssert(!_isnan(x));
            ShiAssert(!_isnan(y));
            ShiAssert(!_isnan(z));

            groundZ = OTWDriver.GetGroundLevel(x, y, &gndNormal);
            // Normalize terrain normal
            tmp = (float)sqrt(gndNormal.x * gndNormal.x + gndNormal.y * gndNormal.y + gndNormal.z * gndNormal.z);
            gndNormal.x /= tmp;
            gndNormal.y /= tmp;
            gndNormal.z /= tmp;

            /*----------------------*/
            /* set flight status
            /*----------------------*/
            if (!IsSet(InAir))
            {
                float gndGmma, relMu;

                CalculateGroundPlane(&gndGmma, &relMu);

                if (qsom * cnalpha < 0.5F)
                {
                    SetFlag(Planted);
                    SetGroundPosition(dT, netAccel, gndGmma, relMu);
                }
                else if (qsom * cnalpha > 0.55F)
                {
                    ClearFlag(Planted);

                    if ((-zaero > GRAVITY || oldp03[2] == 13.0F) && gmma - gndGmma > 0.0F && ctlpitch > 0.0f)
                    {
#if DEBUG
                        ShiAssert(platform.DBrain().ATCStatus() != lLanded);
#endif
                        SetFlag(InAir);
                        platform.UnSetFlag(ON_GROUND);
                    }
                    else
                        SetGroundPosition(dT, netAccel, gndGmma, relMu);
                }
                else
                    SetGroundPosition(dT, netAccel, gndGmma, relMu);
            }
            else
            {
                CheckGroundImpact(dT);
            }
        }

        public void SetSimpleMode(int mode)
        {
            if (mode == simpleMode)
                return;

            if (simpleMode != SIMPLE_MODE_OFF && mode == SIMPLE_MODE_OFF)
            {
                Reinit();
                // MonoPrint("Reinit() Called \n");
            }

            simpleMode = mode;
        }
        public int GetSimpleMode() { return simpleMode; }

        //MI
        public float GetOptKias(int mode);// climb, cruice end, cruice rng  0,1,2
        public float GetLandingAoASpd() { return CalcDesSpeed(auxaeroData.landingAOA); }
        public int GetRackGroup(int i) { return auxaeroData.hardpointrg[i]; }
        public int GetBingoSnd() { return auxaeroData.sndBBBingo; }
        public int GetAltitudeSnd() { return auxaeroData.sndBBAltitude; }
        public const int SIMPLE_MODE_OFF = 0;
        public const int SIMPLE_MODE_AF = 1;
        public const int SIMPLE_MODE_HF = 2;

        // easter egg -- helicopter flight model.....
        public void RunHeliModel();
        public HeliMMClass* hf;
        public float onObjectHeight; // JB carrier

        // MN
        public float GetRefuelSpeed() { return auxaeroData.refuelSpeed; }
        public float GetRefuelAltitude() { return auxaeroData.refuelAltitude; }
        public int GetMaxRippleCount() { return auxaeroData.maxRippleCount; }
        public int GetParkType();
        public float GetDecelerateDistance() { return auxaeroData.decelDistance; }
        public float GetRefuelFollowRate() { return auxaeroData.followRate; }
        public float GetDesiredClosureFactor() { return auxaeroData.desiredClosureFactor; }
        public float GetIL78Factor() { return auxaeroData.IL78Factor; }
        public float GetBingoReturnDistance() { return auxaeroData.BingoReturnDistance; }
        public float GetTankerLongLeg() { return auxaeroData.longLeg; }
        public float GetTankerShortLeg() { return auxaeroData.shortLeg; }
        public float GetRefuelRate() { return auxaeroData.refuelRate; }
        public float GetAIBoomDistance() { return auxaeroData.AIBoomDistance; }
        public float GetStartGroundAvoidCheck() { return auxaeroData.startGroundAvoidCheck; }
        public int LimitPstick() { return auxaeroData.limitPstick; }

        private void AirframeDataInitializeStorage()
        {
#if USE_SH_POOLS
   AirframeDataPool = MemPoolInit( 0 );
#endif
        }

        private void AirframeDataReleaseStorage()
        {
#if USE_SH_POOLS
   MemPoolFree( AirframeDataPool );
#endif
        }
        /********************************************************************/
        /*                                                                  */
        /* Routine: void ReadAllAirframeData (char *)                       */
        /*                                                                  */
        /* Description:                                                     */
        /*    Read all Airframe data sets available.                        */
        /*                                                                  */
        /* Inputs:                                                          */
        /*    char *data_in - Data directory                                */
        /*                                                                  */
        /* Outputs:                                                         */
        /*    None                                                          */
        /*                                                                  */
        /*  Development History :                                           */
        /*  Date      Programer           Description                       */
        /*------------------------------------------------------------------*/
        /*  23-Jan-95 LR                  Initial Write                     */
        /*                                                                  */
        /********************************************************************/
        private void ReadAllAirframeData()
        {
            int i;
            SimlibFileClass* aclist;
            SimlibFileClass* inputFile;
            SimlibFileName buffer;
            SimlibFileName fileName;
            SimlibFileName fName;

            // Allocate pools
            AirframeDataInitializeStorage();

            /*-----------------*/
            /* open input file */
            /*-----------------*/
            sprintf(fileName, "%s\\%s\0", AIRFRAME_DIR, AIRFRAME_DATASET);
            aclist = SimlibFileClass.Open(fileName, SIMLIB_READ);
            num_sets = atoi(aclist.GetNext());

            /*
#if USE_SH_POOLS
            aeroDataset = (AeroDataSet *)MemAllocPtr(AirFrameDataPool, sizeof(AeroDataSet)*num_sets,0);
#else
            aeroDataset = new AeroDataSet[num_sets];
#endif
            */

            aeroDataset = new AeroDataSet[num_sets];

            // TODO if(gLimiterMgr)
            // TODO	delete gLimiterMgr;

            gLimiterMgr = new LimiterMgrClass(num_sets);

            /*--------------------------*/
            /* Do each file in the list */
            /*--------------------------*/
            for (i = 0; i < num_sets; i++)
            {
                aclist.ReadLine(buffer, 80);

                /*-----------------*/
                /* open input file */
                /*-----------------*/
                sprintf(fName, "%s\\%s.dat", AIRFRAME_DIR, buffer);
                inputFile = SimlibFileClass.Open(fName, SIMLIB_READ);

                F4Assert(inputFile);
                ReadData(aeroDataset[i].inputData, inputFile);
                aeroDataset[i].aeroData = AirframeAeroRead(inputFile);
                aeroDataset[i].engineData = AirframeEngineRead(inputFile);
                aeroDataset[i].fcsData = AirframeFcsRead(inputFile);
                gLimiterMgr.ReadLimiters(inputFile, i);
                aeroDataset[i].auxaeroData = AirframeAuxAeroRead(inputFile); // JB 010714
                inputFile.Close();
                delete inputFile;
                inputFile = NULL;
            }

            aclist.Close();
            delete aclist;
            aclist = NULL;
        }

        private void FreeAllAirframeData()
        {
            //TODO delete [] aeroDataset;

            // Deallocate pools
            AirframeDataReleaseStorage();
        }

        /********************************************************************/
        /*                                                                  */
        /* Routine: AeroData *AirframeAeroRead (SimlibFileClass* inputFile)  */
        /*                                                                  */
        /* Description:                                                     */
        /*    Read one set of aero data.                                    */
        /*                                                                  */
        /* Inputs:                                                          */
        /*    inputFile - File handle to read                               */
        /*                                                                  */
        /* Outputs:                                                         */
        /*    aeroData  - aerodynamic data set                              */
        /*                                                                  */
        /*  Development History :                                           */
        /*  Date      Programer           Description                       */
        /*------------------------------------------------------------------*/
        /*  23-Jan-95 LR                  Initial Write                     */
        /*                                                                  */
        /********************************************************************/
        private AeroData AirframeAeroRead(SimlibFileClass* inputFile)
        {
            int i, numMach, numAlpha;
            int mach, alpha;
            AeroData aeroData;

            /*-----------------------------------------------*/
            /* Allocate a block of memeory for the aero data */
            /*-----------------------------------------------*/
            aeroData = new AeroData();

            /*-------------------------*/
            /* Read in Num Mach Breaks */
            /*-------------------------*/
            aeroData.numMach = atoi(inputFile.GetNext());
            numMach = aeroData.numMach;

            /*-------------------------------------------*/
            /* Allocate storage for the mach breakpoints */
            /*-------------------------------------------*/
#if USE_SH_POOLS
	aeroData.mach = (float *)MemAllocPtr(gReadInMemPool, sizeof(float)*aeroData.numMach,0);
#else
            aeroData.mach = new float[aeroData.numMach];
#endif

            /*------------------------------*/
            /* Read in the Mach breakpoints */
            /*------------------------------*/
            for (i = 0; i < numMach; i++)
            {
                aeroData.mach[i] = (float)atof(inputFile.GetNext());
            }

            /*--------------------------*/
            /* Read in Num Alpha Breaks */
            /*--------------------------*/
            aeroData.numAlpha = atoi(inputFile.GetNext());
            numAlpha = aeroData.numAlpha;

            /*--------------------------------------------*/
            /* Allocate storage for the Alpha breakpoints */
            /*--------------------------------------------*/
#if USE_SH_POOLS
	aeroData.alpha = (float *)MemAllocPtr(gReadInMemPool, sizeof(float)*aeroData.numAlpha,0);
#else
            aeroData.alpha = new float[aeroData.numAlpha];
#endif

            /*-------------------------------*/
            /* Read in the Alpha breakpoints */
            /*-------------------------------*/
            for (i = 0; i < numAlpha; i++)
            {
                aeroData.alpha[i] = (float)atof(inputFile.GetNext());
            }

            /*---------------------------------*/
            /* Allocate memeory for CLIFT data */
            /*---------------------------------*/
#if USE_SH_POOLS
	aeroData.clift = (float *)MemAllocPtr(gReadInMemPool, sizeof(float)*numMach*numAlpha,0);
#else
            aeroData.clift = new float[numAlpha * numMach];
#endif

            /*-----------------------*/
            /* Read in lift modifier */
            /*-----------------------*/
            aeroData.clFactor = (float)atof(inputFile.GetNext());

            /*--------------------------*/
            /* read in lift coefficient */
            /*--------------------------*/
            for (mach = 0; mach < numMach; mach++)
            {
                for (alpha = 0; alpha < numAlpha; alpha++)
                {
                    aeroData.clift[mach * numAlpha + alpha] = (float)atof(inputFile.GetNext());
                }
            }

            /*---------------------------------*/
            /* Allocate memeory for CDRAG data */
            /*---------------------------------*/
#if USE_SH_POOLS
	aeroData.cdrag = (float *)MemAllocPtr(gReadInMemPool, sizeof(float)*numMach*numAlpha,0);
#else
            aeroData.cdrag = new float[numAlpha * numMach];
#endif

            /*-----------------------*/
            /* Read in drag modifier */
            /*-----------------------*/
            aeroData.cdFactor = (float)atof(inputFile.GetNext());

            /*--------------------------*/
            /* read in DRAG coefficient */
            /*--------------------------*/
            for (mach = 0; mach < numMach; mach++)
            {
                for (alpha = 0; alpha < numAlpha; alpha++)
                {
                    aeroData.cdrag[mach * numAlpha + alpha] = (float)atof(inputFile.GetNext()) * 1.5F;
                }
            }

            /*------------------------------*/
            /* Allocate memeory for CY data */
            /*------------------------------*/
#if USE_SH_POOLS
	aeroData.cy = (float *)MemAllocPtr(gReadInMemPool, sizeof(float)*numMach*numAlpha,0);
#else
            aeroData.cy = new float[numAlpha * numMach];
#endif

            /*----------------------*/
            /* Read in yaw modifier */
            /*----------------------*/
            aeroData.cyFactor = (float)atof(inputFile.GetNext());

            /*------------------------*/
            /* read in cy coefficient */
            /*------------------------*/
            for (mach = 0; mach < numMach; mach++)
            {
                for (alpha = 0; alpha < numAlpha; alpha++)
                {
                    aeroData.cy[mach * numAlpha + alpha] = (float)atof(inputFile.GetNext());
                }
            }

            return (aeroData);
        }

        // JPO structure to read in auxilliary variables
        //TODO #define OFFSET(x) offsetof(AuxAeroData, x)
        private static const InputDataDesc[] AuxAeroDataDesc = new InputDataDesc[]{
    new InputDataDesc{ "normSpoolRate", InputDataDesc.format.ID_FLOAT, OFFSET(normSpoolRate), "0.7"},
    new InputDataDesc{ "abSpoolRate", InputDataDesc.format.ID_FLOAT, OFFSET(abSpoolRate), "0.4"},
    new InputDataDesc{ "jfsSpoolUpRate", InputDataDesc.format.ID_FLOAT, OFFSET(jfsSpoolUpRate), "15"},
    new InputDataDesc{ "jfsSpoolUpLimit", InputDataDesc.format.ID_FLOAT, OFFSET(jfsSpoolUpLimit), "0.25"},
    new InputDataDesc{ "lightupSpoolRate", InputDataDesc.format.ID_FLOAT, OFFSET(lightupSpoolRate), "10"},
    new InputDataDesc{ "flameoutSpoolRate", InputDataDesc.format.ID_FLOAT, OFFSET(flameoutSpoolRate), "5"},
    new InputDataDesc{ "jfsRechargeTime", InputDataDesc.format.ID_FLOAT, OFFSET(jfsRechargeTime), "60"},
    new InputDataDesc{ "jfsMinRechargeRpm", InputDataDesc.format.ID_FLOAT, OFFSET(jfsMinRechargeRpm), "0.12"},
	new InputDataDesc{ "jfsSpinTime", InputDataDesc.format.ID_FLOAT, OFFSET(jfsSpinTime), "240"}, //MI
    new InputDataDesc{ "mainGenRpm", InputDataDesc.format.ID_FLOAT, OFFSET(mainGenRpm), "0.63"},
    new InputDataDesc{ "stbyGenRpm", InputDataDesc.format.ID_FLOAT, OFFSET(stbyGenRpm), "0.6"},
    new InputDataDesc{ "epuBurnTime", InputDataDesc.format.ID_FLOAT, OFFSET(epuBurnTime), "600"},
	new InputDataDesc{ "DeepStallEngineStall", InputDataDesc.format.ID_INT, OFFSET(DeepStallEngineStall), "0"},	//Need bool
    new InputDataDesc{ "engineDamageStopThreshold", InputDataDesc.format.ID_INT, OFFSET(engineDamageStopThreshold), "13"},  // 2002-04-11 ADDED BY S.G. Externalized the 13% chance of engine stop
    new InputDataDesc{ "engineDamageNoRestartThreshold", InputDataDesc.format.ID_INT, OFFSET(engineDamageNoRestartThreshold), "50"}, // 2002-04-11 ADDED BY S.G. Externalized the 50% chance of engine not able to restart
    new InputDataDesc{ "engineDamageHitThreshold", InputDataDesc.format.ID_INT, OFFSET(engineDamageHitThreshold), "0"}, // 2002-04-11 ADDED BY S.G. Externalized the hitChance > 0 chance of engine shutting down

    // airframe related
    new InputDataDesc{ "rollMomentum", InputDataDesc.format.ID_FLOAT, OFFSET(rollMomentum), "1"},
    new InputDataDesc{ "pitchMomentum", InputDataDesc.format.ID_FLOAT, OFFSET(pitchMomentum), "1"},
    new InputDataDesc{ "yawMomentum", InputDataDesc.format.ID_FLOAT, OFFSET(yawMomentum), "1"},
    new InputDataDesc{ "pitchElasticity", InputDataDesc.format.ID_FLOAT, OFFSET(pitchElasticity), "1"},
    new InputDataDesc{ "gearPitchFactor", InputDataDesc.format.ID_FLOAT, OFFSET(gearPitchFactor), "0.1"},
    new InputDataDesc{ "rollGearGain", InputDataDesc.format.ID_FLOAT, OFFSET(rollGearGain), "0.6"},
    new InputDataDesc{ "yawGearGain", InputDataDesc.format.ID_FLOAT, OFFSET(yawGearGain), "0.6"},
    new InputDataDesc{ "pitchGearGain", InputDataDesc.format.ID_FLOAT, OFFSET(pitchGearGain), "0.8"},
    new InputDataDesc{ "sinkRate", InputDataDesc.format.ID_FLOAT, OFFSET(sinkRate), "15"},
    new InputDataDesc{ "tefMaxAngle", InputDataDesc.format.ID_FLOAT, OFFSET(tefMaxAngle), "20"}, // divide angle of TEF by this to get amount of influence on CL and CD
    new InputDataDesc{ "hasFlapperons", InputDataDesc.format.ID_INT, OFFSET(hasFlapperons), "1"}, // has flapperons are opposed to ailerons & flaps
    new InputDataDesc{ "hasTef", InputDataDesc.format.ID_INT, OFFSET(hasTef), "2"}, // has TEF 0 - no, 1 manual, 2 aoa
    new InputDataDesc{ "CLtefFactor", InputDataDesc.format.ID_FLOAT, OFFSET(CLtefFactor), "0.05"}, // how much the TEF affect the CL
    new InputDataDesc{ "CDtefFactor", InputDataDesc.format.ID_FLOAT, OFFSET(CDtefFactor), "0.05"}, // how much the TEF affect the CD
    new InputDataDesc{ "tefNStages", InputDataDesc.format.ID_INT, OFFSET(tefNStages), "2"}, // number of stages of TEF
    new InputDataDesc{ "tefTakeoff", InputDataDesc.format.ID_FLOAT, OFFSET(tefTakeOff), "0"}, // Tef angle for takeoff
    new InputDataDesc{ "tefRate", InputDataDesc.format.ID_FLOAT, OFFSET(tefRate), "5"}, // Tef rate of change
    new InputDataDesc{ "lefRate", InputDataDesc.format.ID_FLOAT, OFFSET(lefRate), "5"}, // Lef rate of change
    new InputDataDesc{ "hasLef", InputDataDesc.format.ID_INT, OFFSET(hasLef), "2"}, // has LEF
    new InputDataDesc{ "lefGround", InputDataDesc.format.ID_FLOAT, OFFSET(lefGround), "-2"}, // lef position on ground
    new InputDataDesc{ "lefMaxAngle", InputDataDesc.format.ID_FLOAT, OFFSET(lefMaxAngle), "20"}, // max angle of LEF by this to get amount of influence on CL and CD
    new InputDataDesc{ "lefMaxMach", InputDataDesc.format.ID_FLOAT, OFFSET(lefMaxMach), "0.4"}, // LEF stops at this
    new InputDataDesc{ "lefNStages", InputDataDesc.format.ID_INT, OFFSET(lefNStages), "1"}, // number of stages of LEF
    new InputDataDesc{ "CDlefFactor", InputDataDesc.format.ID_FLOAT, OFFSET(CDlefFactor), "0.0"}, // how much the LEF affect the CD
    new InputDataDesc{ "CDSPDBFactor", InputDataDesc.format.ID_FLOAT, OFFSET(CDSPDBFactor), "0.08"}, // how much the speed brake affect drag
    new InputDataDesc{ "CDLDGFactor", InputDataDesc.format.ID_FLOAT, OFFSET(CDLDGFactor), "0.06"}, // how much the landing gear affects drag
    new InputDataDesc{ "flapGearRelative", InputDataDesc.format.ID_INT, OFFSET(flapGearRelative), "1"}, //flaps only work with gear (or tef extend)
    new InputDataDesc{ "maxFlapVcas", InputDataDesc.format.ID_FLOAT, OFFSET(maxFlapVcas), "370"}, // falps fully retract at
    new InputDataDesc{ "flapVcasRange", InputDataDesc.format.ID_FLOAT, OFFSET(flapVcasRange), "130"}, // what range of Vcas flaps work over
    new InputDataDesc{ "area2Span", InputDataDesc.format.ID_FLOAT, OFFSET(area2Span), "0.1066"}, // used to convert life area into span
    new InputDataDesc{ "rudderMaxAngle", InputDataDesc.format.ID_FLOAT, OFFSET(rudderMaxAngle), "20"}, // max angle of rudder
    new InputDataDesc{ "aileronMaxAngle", InputDataDesc.format.ID_FLOAT, OFFSET(aileronMaxAngle), "20"}, // max angle of aileron
    new InputDataDesc{ "elevonMaxAngle", InputDataDesc.format.ID_FLOAT, OFFSET(elevonMaxAngle), "20"}, // max angle of elevon
    new InputDataDesc{ "airbrakeMaxAngle", InputDataDesc.format.ID_FLOAT, OFFSET(airbrakeMaxAngle), "60"}, // max angle of airbrake
    new InputDataDesc{ "hasSwingWing", InputDataDesc.format.ID_INT, OFFSET(hasSwingWing), "0"}, // has swing wing capability
    new InputDataDesc{ "isComplex", InputDataDesc.format.ID_INT, OFFSET(isComplex), "0"}, // has complex model
    new InputDataDesc{ "landingAOA", InputDataDesc.format.ID_FLOAT, OFFSET(landingAOA), "12.5"},
    new InputDataDesc{ "dragChuteCd", InputDataDesc.format.ID_FLOAT, OFFSET(dragChuteCd), "0"},
    new InputDataDesc{ "dragChuteMaxSpeed", InputDataDesc.format.ID_FLOAT, OFFSET(dragChuteMaxSpeed), "170"},
    new InputDataDesc{ "rollCouple", InputDataDesc.format.ID_FLOAT, OFFSET(rollCouple), "0"},
    new InputDataDesc{ "elevatorRoll", InputDataDesc.format.ID_INT, OFFSET(elevatorRolls), "1"},
    new InputDataDesc{ "canopyMaxAngle", InputDataDesc.format.ID_FLOAT, OFFSET(canopyMaxAngle), "20"},
    new InputDataDesc{ "canopyRate", InputDataDesc.format.ID_FLOAT, OFFSET(canopyRate), "5"},

    // fuel related
    new InputDataDesc{ "fuelFlowFactorNormal", InputDataDesc.format.ID_FLOAT, OFFSET(fuelFlowFactorNormal), "0.25"},
    new InputDataDesc{ "fuelFlowFactorAb", InputDataDesc.format.ID_FLOAT, OFFSET(fuelFlowFactorAb), "0.65"},
    new InputDataDesc{ "minFuelFlow", InputDataDesc.format.ID_FLOAT, OFFSET(minFuelFlow), "1200"},
    new InputDataDesc{ "fuelFwdRes", InputDataDesc.format.ID_FLOAT, OFFSET(fuelFwdRes), "0"},
    new InputDataDesc{ "fuelAftRes", InputDataDesc.format.ID_FLOAT, OFFSET(fuelAftRes), "0"},
    new InputDataDesc{ "fuelFwd1", InputDataDesc.format.ID_FLOAT, OFFSET(fuelFwd1), "0"},
    new InputDataDesc{ "fuelAft1", InputDataDesc.format.ID_FLOAT, OFFSET(fuelAft1), "0"},
    new InputDataDesc{ "fuelWingAl", InputDataDesc.format.ID_FLOAT, OFFSET(fuelWingAl), "0"},
    new InputDataDesc{ "fuelWingFr", InputDataDesc.format.ID_FLOAT, OFFSET(fuelWingFr), "0"},
    new InputDataDesc{ "fuelFwdResRate", InputDataDesc.format.ID_FLOAT, OFFSET(fuelFwdResRate), "0"},
    new InputDataDesc{ "fuelAftResRate", InputDataDesc.format.ID_FLOAT, OFFSET(fuelAftResRate), "0"},
    new InputDataDesc{ "fuelFwd1Rate", InputDataDesc.format.ID_FLOAT, OFFSET(fuelFwd1Rate), "0"},
    new InputDataDesc{ "fuelAft1Rate", InputDataDesc.format.ID_FLOAT, OFFSET(fuelAft1Rate), "0"},
    new InputDataDesc{ "fuelWingAlRate", InputDataDesc.format.ID_FLOAT, OFFSET(fuelWingAlRate), "0"},
    new InputDataDesc{ "fuelWingFrRate", InputDataDesc.format.ID_FLOAT, OFFSET(fuelWingFrRate), "0"},
    new InputDataDesc{ "fuelClineRate", InputDataDesc.format.ID_FLOAT, OFFSET(fuelClineRate), "0"},
    new InputDataDesc{ "fuelWingExtRate", InputDataDesc.format.ID_FLOAT, OFFSET(fuelWingExtRate), "0"},
    new InputDataDesc{ "fuelMinFwd", InputDataDesc.format.ID_FLOAT, OFFSET(fuelMinFwd), "400"},
    new InputDataDesc{ "fuelMinAft", InputDataDesc.format.ID_FLOAT, OFFSET(fuelMinAft), "250"},
    new InputDataDesc{ "nChaff", InputDataDesc.format.ID_INT, OFFSET(nChaff), "60"},
    new InputDataDesc{ "nFlare", InputDataDesc.format.ID_INT, OFFSET(nFlare), "30"},
    new InputDataDesc{ "gunLocation", InputDataDesc.format.ID_VECTOR, OFFSET(gunLocation), "2 -2 0"},
    new InputDataDesc{ "nEngines", InputDataDesc.format.ID_INT, OFFSET(nEngines), "1"},
    new InputDataDesc{ "engine1Location", InputDataDesc.format.ID_VECTOR, OFFSET(engineLocation[0]), "0 0 0"},
    new InputDataDesc{ "engine2Location", InputDataDesc.format.ID_VECTOR, OFFSET(engineLocation[1]), "0 0 0"},
    new InputDataDesc{ "engine3Location", InputDataDesc.format.ID_VECTOR, OFFSET(engineLocation[2]), "0 0 0"},
    new InputDataDesc{ "engine4Location", InputDataDesc.format.ID_VECTOR, OFFSET(engineLocation[3]), "0 0 0"},
    new InputDataDesc{ "engineSmokes", InputDataDesc.format.ID_INT, OFFSET(engineSmokes), "0"},
    new InputDataDesc{ "wingTipLocation", InputDataDesc.format.ID_VECTOR, OFFSET(wingTipLocation), "0 0 0"},
    new InputDataDesc{ "refuelLocation", InputDataDesc.format.ID_VECTOR, OFFSET(refuelLocation), "0 0 0"},
    new InputDataDesc{ "hardpoint1Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[0]), "-1"},
    new InputDataDesc{ "hardpoint2Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[1]), "-1"},
    new InputDataDesc{ "hardpoint3Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[2]), "-1"},
    new InputDataDesc{ "hardpoint4Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[3]), "-1"},
    new InputDataDesc{ "hardpoint5Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[4]), "-1"},
    new InputDataDesc{ "hardpoint6Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[5]), "-1"},
    new InputDataDesc{ "hardpoint7Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[6]), "-1"},
    new InputDataDesc{ "hardpoint8Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[7]), "-1"},
    new InputDataDesc{ "hardpoint9Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[8]), "-1"},
    new InputDataDesc{ "hardpoint10Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[9]), "-1"},
    new InputDataDesc{ "hardpoint11Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[10]), "-1"},
    new InputDataDesc{ "hardpoint12Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[11]), "-1"},
    new InputDataDesc{ "hardpoint13Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[12]), "-1"},
    new InputDataDesc{ "hardpoint14Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[13]), "-1"},
    new InputDataDesc{ "hardpoint15Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[14]), "-1"},
    new InputDataDesc{ "hardpoint16Grp", InputDataDesc.format.ID_INT, OFFSET(hardpointrg[15]), "-1"},

    // sound stuff
    new InputDataDesc{ "sndExt", InputDataDesc.format.ID_INT, OFFSET(sndExt), "35"},
    new InputDataDesc{ "sndWind", InputDataDesc.format.ID_INT, OFFSET(sndWind), "33"},
    new InputDataDesc{ "sndAbInt", InputDataDesc.format.ID_INT, OFFSET(sndAbInt), "40"},
    new InputDataDesc{ "sndAbExt", InputDataDesc.format.ID_INT, OFFSET(sndAbExt), "39"},
    new InputDataDesc{ "sndEject", InputDataDesc.format.ID_INT, OFFSET(sndEject), "5"},
    new InputDataDesc{ "sndSpdBrakeStart", InputDataDesc.format.ID_INT, OFFSET(sndSpdBrakeStart), "141"},
    new InputDataDesc{ "sndSpdBrakeLoop", InputDataDesc.format.ID_INT, OFFSET(sndSpdBrakeLoop), "140"},
    new InputDataDesc{ "sndSpdBrakeEnd", InputDataDesc.format.ID_INT, OFFSET(sndSpdBrakeEnd), "139"},
    new InputDataDesc{ "sndSpdBrakeWind", InputDataDesc.format.ID_INT, OFFSET(sndSpdBrakeWind), "142"},
    new InputDataDesc{ "sndOverSpeed1", InputDataDesc.format.ID_INT, OFFSET(sndOverSpeed1), "191"},
    new InputDataDesc{ "sndOverSpeed2", InputDataDesc.format.ID_INT, OFFSET(sndOverSpeed2), "192"},
    new InputDataDesc{ "sndGunStart", InputDataDesc.format.ID_INT, OFFSET(sndGunStart), "25"},
    new InputDataDesc{ "sndGunLoop", InputDataDesc.format.ID_INT, OFFSET(sndGunLoop), "26"},
    new InputDataDesc{ "sndGunEnd", InputDataDesc.format.ID_INT, OFFSET(sndGunEnd), "27"},
    new InputDataDesc{ "sndBBPullup", InputDataDesc.format.ID_INT, OFFSET(sndBBPullup), "68"},
    new InputDataDesc{ "sndBBBingo", InputDataDesc.format.ID_INT, OFFSET(sndBBBingo), "66"},
    new InputDataDesc{ "sndBBWarning", InputDataDesc.format.ID_INT, OFFSET(sndBBWarning), "63"},
    new InputDataDesc{ "sndBBCaution", InputDataDesc.format.ID_INT, OFFSET(sndBBCaution), "64"},
    new InputDataDesc{ "sndBBChaffFlareLow", InputDataDesc.format.ID_INT, OFFSET(sndBBChaffFlareLow), "184"},
    new InputDataDesc{ "sndBBFlare", InputDataDesc.format.ID_INT, OFFSET(sndBBFlare), "138"},
    new InputDataDesc{ "sndBBChaffFlare", InputDataDesc.format.ID_INT, OFFSET(sndBBChaffFlare), "183"},
    new InputDataDesc{ "sndBBChaffFlareOut", InputDataDesc.format.ID_INT, OFFSET(sndBBChaffFlareOut), "185"},
    new InputDataDesc{ "sndBBAltitude", InputDataDesc.format.ID_INT, OFFSET(sndBBAltitude), "65"},
    new InputDataDesc{ "sndBBLock", InputDataDesc.format.ID_INT, OFFSET(sndBBLock), "67"},
    new InputDataDesc{ "sndTouchDown", InputDataDesc.format.ID_INT, OFFSET(sndTouchDown), "42"},
    new InputDataDesc{ "sndWheelBrakes", InputDataDesc.format.ID_INT, OFFSET(sndWheelBrakes), "132"},
    new InputDataDesc{ "sndDragChute", InputDataDesc.format.ID_INT, OFFSET(sndDragChute), "217"},
    new InputDataDesc{ "sndLowSpeed", InputDataDesc.format.ID_INT, OFFSET(sndLowSpeed), "167"},
    new InputDataDesc{ "sndFlapStart", InputDataDesc.format.ID_INT, OFFSET(sndFlapStart), "145"},
    new InputDataDesc{ "sndFlapLoop", InputDataDesc.format.ID_INT, OFFSET(sndFlapLoop), "144"},
    new InputDataDesc{ "sndFlapEnd", InputDataDesc.format.ID_INT, OFFSET(sndFlapEnd), "143"},
    new InputDataDesc{ "sndHookStart", InputDataDesc.format.ID_INT, OFFSET(sndHookStart), "195"},
    new InputDataDesc{ "sndHookLoop", InputDataDesc.format.ID_INT, OFFSET(sndHookLoop), "194"},
    new InputDataDesc{ "sndHookEnd", InputDataDesc.format.ID_INT, OFFSET(sndHookEnd), "193"},
    new InputDataDesc{ "sndGearCloseStart", InputDataDesc.format.ID_INT, OFFSET(sndGearCloseStart), "147"},
    new InputDataDesc{ "sndGearCloseEnd", InputDataDesc.format.ID_INT, OFFSET(sndGearCloseEnd), "146"},
    new InputDataDesc{ "sndGearOpenStart", InputDataDesc.format.ID_INT, OFFSET(sndGearOpenStart), "149"},
    new InputDataDesc{ "sndGearOpenEnd", InputDataDesc.format.ID_INT, OFFSET(sndGearOpenEnd), "150"},
    new InputDataDesc{ "sndGearLoop", InputDataDesc.format.ID_INT, OFFSET(sndGearLoop), "148"},
    new InputDataDesc{ "rollLimitForAiInWP", InputDataDesc.format.ID_FLOAT, OFFSET(rollLimitForAiInWP), "180.0"}, // 2002-01-31 ADDED BY S.G. AI limition on roll when in waypoint (or similar) mode
	new InputDataDesc{ "flap2Nozzle", InputDataDesc.format.ID_INT, OFFSET(flap2Nozzle), "0"},
	new InputDataDesc{ "startGroundAvoidCheck", InputDataDesc.format.ID_FLOAT, OFFSET(startGroundAvoidCheck), "3000.0"}, // 2002-04-17 MN only do ground avoid check when we are closer than this distance to the ground
	new InputDataDesc{ "limitPstick", InputDataDesc.format.ID_INT, OFFSET(limitPstick), "0"}, // 2002-04-17 MN method of pullup; 1 = use old code (G force limited), 0 = use full pull (pStick = 1.0f)
	// 2002-02-08 MN refuelling speed, altitude, deceleration distance, follow rate, desiredClosureFactor and IL78Factor
	new InputDataDesc{ "refuelSpeed", InputDataDesc.format.ID_FLOAT, OFFSET(refuelSpeed), "310.0"},
	new InputDataDesc{ "refuelAltitude", InputDataDesc.format.ID_FLOAT, OFFSET(refuelAltitude), "22500.0"},
	new InputDataDesc{ "decelDistance", InputDataDesc.format.ID_FLOAT, OFFSET(decelDistance), "1000.0"},
	new InputDataDesc{ "followRate", InputDataDesc.format.ID_FLOAT, OFFSET(followRate), "20.0"}, // original was 10, 20 brings some better results
	new InputDataDesc{ "desiredClosureFactor", InputDataDesc.format.ID_FLOAT, OFFSET(desiredClosureFactor), "0.25"}, // original was hardcoded to 0.25
	new InputDataDesc{ "IL78Factor", InputDataDesc.format.ID_FLOAT, OFFSET(IL78Factor), "0.0"}, // IL78 range to basket modifier (to fix "GivingGas" flagging)
	new InputDataDesc{ "longLeg", InputDataDesc.format.ID_FLOAT, OFFSET(longLeg), "60.0"}, // 60 nm long leg of tanker track pattern
	new InputDataDesc{ "shortLeg", InputDataDesc.format.ID_FLOAT, OFFSET(shortLeg), "25.0"}, // 25nm short leg of tanker track pattern
	new InputDataDesc{ "refuelRate", InputDataDesc.format.ID_FLOAT, OFFSET(refuelRate), "50.0"}, // 50 lbs per second
	new InputDataDesc{ "AIBoomDistance", InputDataDesc.format.ID_FLOAT, OFFSET(AIBoomDistance), "50.0"}, // distance to boom at which we will put the AI on the boom (hack...)

	// 2002-03-12 MN fuel handling stuff
	new InputDataDesc{ "BingoReturnDistance", InputDataDesc.format.ID_FLOAT, OFFSET(BingoReturnDistance), "0.0"},
	new InputDataDesc{ "jokerFactor", InputDataDesc.format.ID_FLOAT, OFFSET(jokerFactor), "2.0"},
	new InputDataDesc{ "bingoFactor", InputDataDesc.format.ID_FLOAT, OFFSET(bingoFactor), "5.0"},
	new InputDataDesc{ "fumesFactor", InputDataDesc.format.ID_FLOAT, OFFSET(fumesFactor), "15.0"},

	// 2002-02-23 MN max ripple count
	new InputDataDesc{ "maxRippleCount", InputDataDesc.format.ID_INT, OFFSET(maxRippleCount), "19"}, // F-16's max ripple count
	new InputDataDesc{ "largePark", InputDataDesc.format.ID_INT, OFFSET(largePark), "0"}, // requires large parking space

	//MI TFR
	new InputDataDesc{ "Has_TFR", InputDataDesc.format.ID_INT, OFFSET(Has_TFR), "0"},	//does this plane have TFR?
	new InputDataDesc{ "PID_K", InputDataDesc.format.ID_FLOAT, OFFSET(PID_K), "0.5"}, //Proportianal gain in TFR PID pitch controler.
	new InputDataDesc{ "PID_KI", InputDataDesc.format.ID_FLOAT, OFFSET(PID_KI), "0.0"}, //Intergral gain in TFR PID pitch controler
	new InputDataDesc{ "PID_KD", InputDataDesc.format.ID_FLOAT, OFFSET(PID_KD), "0.39"}, //Differential gain in TFR PID pitch controler
	new InputDataDesc{ "TFR_LimitMX", InputDataDesc.format.ID_INT, OFFSET(TFR_LimitMX), "0"}, //Limit PID Integrator internal value so it doesn't get stuck in exteme.
	new InputDataDesc{ "TFR_Corner", InputDataDesc.format.ID_FLOAT, OFFSET(TFR_Corner), "350.0"}, //Corner speed used in TFR calculations
	new InputDataDesc{ "TFR_Gain", InputDataDesc.format.ID_FLOAT, OFFSET(TFR_Gain), "0.010"}, //Gain for calculating pitch error based on alt. difference
	new InputDataDesc{ "EVA_Gain", InputDataDesc.format.ID_FLOAT, OFFSET(EVA_Gain), "5.0"}, //Pitch setpoint gain in EVA (evade) code
	new InputDataDesc{ "TFR_MaxRoll", InputDataDesc.format.ID_FLOAT, OFFSET(TFR_MaxRoll), "20.0"}, //Do not pull the stick in TFR if roll exceeds this value
	new InputDataDesc{ "TFR_SoftG", InputDataDesc.format.ID_FLOAT, OFFSET(TFR_SoftG), "2.0"}, //Max TFR G pull in soft mode
	new InputDataDesc{ "TFR_MedG", InputDataDesc.format.ID_FLOAT, OFFSET(TFR_MedG), "4.0"}, //Max TFR G pull in medium mode
	new InputDataDesc{ "TFR_HardG", InputDataDesc.format.ID_FLOAT, OFFSET(TFR_HardG), "6.0"}, //Max TFR G pull in hard mode
	new InputDataDesc{ "TFR_Clearance", InputDataDesc.format.ID_FLOAT, OFFSET(TFR_Clearance), "20.0"}, //Minimum clearance above the top of any obstacle [ft]
	new InputDataDesc{ "SlowPercent", InputDataDesc.format.ID_FLOAT, OFFSET(SlowPercent), "0.0"}, //Flash SLOW when airspeed is lower then this percentage of corner speed	
	new InputDataDesc{ "TFR_lookAhead", InputDataDesc.format.ID_FLOAT, OFFSET(TFR_lookAhead), "4000.0"}, //Distance from ground directly under a/c used to measure ground inclination [ft]
	new InputDataDesc{ "EVA1_SoftFactor", InputDataDesc.format.ID_FLOAT, OFFSET(EVA1_SoftFactor), "0.6"}, //Turnradius multiplier to get safe distance from ground for FLY_UP in SLOW
	new InputDataDesc{ "EVA2_SoftFactor", InputDataDesc.format.ID_FLOAT, OFFSET(EVA2_SoftFactor), "0.4"}, //Turnradius multiplier to get safe distance from ground for OBSTACLE in SLOW
	new InputDataDesc{ "EVA1_MedFactor", InputDataDesc.format.ID_FLOAT, OFFSET(EVA1_MedFactor), "0.8"}, //Turnradius multiplier to get safe distance from ground for FLY_UP in MED
	new InputDataDesc{ "EVA2_MedFactor", InputDataDesc.format.ID_FLOAT, OFFSET(EVA2_MedFactor), "0.6"}, //Turnradius multiplier to get safe distance from ground for OBSTACLE in MED
	new InputDataDesc{ "EVA1_HardFactor", InputDataDesc.format.ID_FLOAT, OFFSET(EVA1_HardFactor), "1.2"}, //Turnradius multiplier to get safe distance from ground for FLY_UP in HARD
	new InputDataDesc{ "EVA2_HardFactor", InputDataDesc.format.ID_FLOAT, OFFSET(EVA2_HardFactor), "0.9"}, //Turnradius multiplier to get safe distance from ground for FLY_UP in MED
	new InputDataDesc{ "TFR_GammaCorrMult", InputDataDesc.format.ID_FLOAT, OFFSET(TFR_GammaCorrMult), "1.0"}, //Turnradius multiplier to get safe distance from ground for OBSTACLE in HARD
	new InputDataDesc{ "LantirnCameraX", InputDataDesc.format.ID_FLOAT, OFFSET(LantirnCameraX), "10.0"}, //Position of the camera
	new InputDataDesc{ "LantirnCameraY", InputDataDesc.format.ID_FLOAT, OFFSET(LantirnCameraY), "2.0"},
	new InputDataDesc{ "LantirnCameraZ", InputDataDesc.format.ID_FLOAT, OFFSET(LantirnCameraZ), "5.0"},

	// Digi's related MAR calculation
    new InputDataDesc{ "MinTGTMAR", InputDataDesc.format.ID_FLOAT, OFFSET(minTGTMAR), "0.0"},	// 2002-03-22 ADDED BY S.G. Min TGTMAR for this type of aicraft
    new InputDataDesc{ "MaxMARIdedStart", InputDataDesc.format.ID_FLOAT, OFFSET(maxMARIdedStart), "0.0"},		// 2002-03-22 ADDED BY S.G. MinMAR for this type of aicraft when target is ID'ed and below 5K
    new InputDataDesc{ "AddMARIded5k", InputDataDesc.format.ID_FLOAT, OFFSET(addMARIded5k), "0.0"},		// 2002-03-22 ADDED BY S.G. MinMAR for this type of aicraft when target is ID'ed and below 5K
    new InputDataDesc{ "AddMARIded18k", InputDataDesc.format.ID_FLOAT, OFFSET(addMARIded18k), "0.0"},		// 2002-03-22 ADDED BY S.G. MinMAR for this type of aicraft when target is ID'ed and below 18K
    new InputDataDesc{ "AddMARIded28k", InputDataDesc.format.ID_FLOAT, OFFSET(addMARIded28k), "0.0"},		// 2002-03-22 ADDED BY S.G. MinMAR for this type of aicraft when target is ID'ed and below 28K
    new InputDataDesc{ "AddMARIdedSpike", InputDataDesc.format.ID_FLOAT, OFFSET(addMARIdedSpike), "0.0"}		// 2002-03-22 ADDED BY S.G. MinMAR for this type of aicraft when target is ID'ed and spiked
};
        //TODO #undef OFFSET

        private AuxAeroData AirframeAuxAeroRead(SimlibFileClass* inputFile)
        {
            AuxAeroData auxaeroData;

            auxaeroData = new AuxAeroData();

            if (ParseSimlibFile(auxaeroData, AuxAeroDataDesc, inputFile) == false)
            {
                //	    F4Assert(!"Bad parsing of aux aero data");
            }

            return (auxaeroData);
        }

        /***********************************************************************/
        /*                                                                     */
        /* Routine: EngineData *AirframeEngineRead (SimlibFileClass* inputFile) */
        /*                                                                     */
        /* Description:                                                        */
        /*    Read one set of engine data.                                     */
        /*                                                                     */
        /* Inputs:                                                             */
        /*    inputFile - file handle to read                                  */
        /*                                                                     */
        /* Outputs:                                                            */
        /*    engineData - Engine data set                                     */
        /*                                                                     */
        /*  Development History :                                              */
        /*  Date      Programer           Description                          */
        /*---------------------------------------------------------------------*/
        /*  23-Jan-95 LR                  Initial Write                        */
        /*                                                                     */
        /***********************************************************************/
        private EngineData AirframeEngineRead(SimlibFileClass* inputFile)
        {
            int numMach, numAlt;
            int i, mach, alt;
            EngineData engineData;

            /*-------------------------------------------------*/
            /* Allocate a block of memeory for the engine data */
            /*-------------------------------------------------*/
            engineData = new EngineData();

            /*-------------------------*/
            /* Read in thrust modifier */
            /*-------------------------*/
            engineData.thrustFactor = (float)atof(inputFile.GetNext());

            /*----------------------------*/
            /* Read in fuel flow modifier */
            /*----------------------------*/
            engineData.fuelFlowFactor = (float)atof(inputFile.GetNext());

            /*-------------------------*/
            /* Read in Num Mach Breaks */
            /*-------------------------*/
            engineData.numMach = atoi(inputFile.GetNext());
            numMach = engineData.numMach;

            /*-------------------------------------------*/
            /* Allocate storage for the mach breakpoints */
            /*-------------------------------------------*/
#if USE_SH_POOLS
	engineData.mach = (float *)MemAllocPtr(gReadInMemPool, sizeof(float)*numMach,0);
#else
            engineData.mach = new float[numMach];
#endif

            /*------------------------------*/
            /* Read in the Mach breakpoints */
            /*------------------------------*/
            for (mach = 0; mach < numMach; mach++)
            {
                engineData.mach[mach] = (float)atof(inputFile.GetNext());
            }

            /*------------------------*/
            /* Read in Num Alt Breaks */
            /*------------------------*/
            engineData.numAlt = atoi(inputFile.GetNext());
            numAlt = engineData.numAlt;

            /*-------------------------------------------*/
            /* Allocate storage for the alt breakpoints */
            /*-------------------------------------------*/
#if USE_SH_POOLS
	engineData.alt = (float *)MemAllocPtr(gReadInMemPool, sizeof(float)*numAlt,0);
#else
            engineData.alt = new float[numAlt];
#endif

            /*------------------------------*/
            /* Read in the Mach breakpoints */
            /*------------------------------*/
            for (alt = 0; alt < numAlt; alt++)
            {
                engineData.alt[alt] = (float)atof(inputFile.GetNext());
            }

            /*----------------------------------------------*/
            /* Allocate Storage for three thrust data sets  */
            /* Set 0 - Idle                                 */
            /* Set 1 - Mil                                  */
            /* Set 2 - AB                                   */
            /*----------------------------------------------*/
#if USE_SH_POOLS
	engineData.thrust[0] = (float *)MemAllocPtr(gReadInMemPool, sizeof(float)*numMach*numAlt,0);
	engineData.thrust[1] = (float *)MemAllocPtr(gReadInMemPool, sizeof(float)*numMach*numAlt,0);
	engineData.thrust[2] = (float *)MemAllocPtr(gReadInMemPool, sizeof(float)*numMach*numAlt,0);
#else
            engineData.thrust[0] = new float[numMach * numAlt];
            engineData.thrust[1] = new float[numMach * numAlt];
            engineData.thrust[2] = new float[numMach * numAlt];
#endif

            /*-------------------------*/
            /* Read in the thrust data */
            /*-------------------------*/
            for (i = 0; i < 3; i++)
            {
                for (alt = 0; alt < numAlt; alt++)
                {
                    for (mach = 0; mach < numMach; mach++)
                    {
                        engineData.thrust[i][alt * numMach + mach] =
                           (float)atof(inputFile.GetNext());
                    }
                }
            }

            // JB 010706
            engineData.hasAB = false;
            for (alt = 0; alt < numAlt && !engineData.hasAB; alt++)
                for (mach = 0; mach < numMach && !engineData.hasAB; mach++)
                    if (engineData.thrust[1][alt * numMach + mach] != engineData.thrust[2][alt * numMach + mach])
                        engineData.hasAB = true;

            return (engineData);
        }

        /********************************************************************/
        /*                                                                  */
        /* Routine: RollData *AirframeFcsRead (SimlibFileClass* inputFile)   */
        /*                                                                  */
        /* Description:                                                     */
        /*    Read one set of FCS data.                                     */
        /*                                                                  */
        /* Inputs:                                                          */
        /*    inputFile - file handle to read                               */
        /*                                                                  */
        /* Outputs:                                                         */
        /*    rollCmd - roll performance data set                           */
        /*                                                                  */
        /*  Development History :                                           */
        /*  Date      Programer           Description                       */
        /*------------------------------------------------------------------*/
        /*  23-Jan-95 LR                  Initial Write                     */
        /*                                                                  */
        /********************************************************************/
        private RollData AirframeFcsRead(SimlibFileClass* inputFile)
        {
            int numAlpha, numQbar;
            int alpha, qbar;
            RollData rollCmd;
            float scale;

            /*-------------------------------------*/
            /* Allocate storage for Roll rate Data */
            /*-------------------------------------*/
            rollCmd = new RollData();

            /*--------------------------------*/
            /* Read in number of alpha Breaks */
            /*--------------------------------*/
            rollCmd.numAlpha = atoi(inputFile.GetNext());
            numAlpha = rollCmd.numAlpha;

            /*---------------------------------------*/
            /* Reserve storage for alpha breakpoints */
            /*---------------------------------------*/
#if USE_SH_POOLS
	rollCmd.alpha = (float *)MemAllocPtr(gReadInMemPool, sizeof(float)*numAlpha,0);
#else
            rollCmd.alpha = new float[numAlpha];
#endif

            /*-------------------------------*/
            /* Read in the alpha breakpoints */
            /*-------------------------------*/
            for (alpha = 0; alpha < numAlpha; alpha++)
            {
                rollCmd.alpha[alpha] = (float)atof(inputFile.GetNext());
            }

            /*-------------------------------*/
            /* Read in number of qbar Breaks */
            /*-------------------------------*/
            rollCmd.numQbar = atoi(inputFile.GetNext());
            numQbar = rollCmd.numQbar;

            /*---------------------------------------*/
            /* Reserve storage for qbar breakpoints */
            /*---------------------------------------*/
#if USE_SH_POOLS
	rollCmd.qbar = (float *)MemAllocPtr(gReadInMemPool, sizeof(float)*numQbar,0);
#else
            rollCmd.qbar = new float[numQbar];
#endif

            /*-------------------------------*/
            /* Read in the qbar breakpoints */
            /*-------------------------------*/
            for (qbar = 0; qbar < numQbar; qbar++)
            {
                rollCmd.qbar[qbar] = (float)atof(inputFile.GetNext());
            }

            /*-------------------------------------*/
            /* Allocate memeory for roll rate data */
            /*-------------------------------------*/
#if USE_SH_POOLS
	rollCmd.roll = (float *)MemAllocPtr(gReadInMemPool, sizeof(float)*numQbar*numAlpha,0);
#else
            rollCmd.roll = new float[numAlpha * numQbar];
#endif

            /*-------------------*/
            /* read in roll rate */
            /*-------------------*/
            scale = (float)atof(inputFile.GetNext());
            for (alpha = 0; alpha < numAlpha; alpha++)
            {
                for (qbar = 0; qbar < numQbar; qbar++)
                {
                    rollCmd.roll[alpha * numQbar + qbar] = (float)atof(inputFile.GetNext()) * scale;
                }
            }

            return (rollCmd);
        }
        private static AeroDataSet[] aeroDataset = null;
        private static int num_sets = 0;
#endif
    }

}
