using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VU_TIME = System.UInt64;

namespace FalconNet.Sim
{
    class FaultClass
    {
#if TODO

        public const int MAX_MFL = 17; // 15 + T/O Land

        public enum type_FSubSystem
        {
            amux_fault, // Avionics Data Bus. If amux and bmux failed_fault, fail all avionics (very rare) 
            blkr_fault, // Interference Blanker. If failed_fault, radio drives radar crazy
            bmux_fault, // Avionics Data Bus. If amux and bmux failed_fault, fail all avionics (very rare) 
            cadc_fault, // Central Air Data Computer. If failed no baro altitude
            cmds_fault, // Countermeasures dispenser system, breaks chaff and flare
            dlnk_fault, // Improved Data Modem. If failed no data transfer in
            dmux_fault, // Weapon Data Bus. If failed_fault, no weapons fire on given station
            dte_fault,  // Just for show
            eng_fault,  // Engine. If failed usually means fire_fault, but could mean hydraulics
            epod_fault, // ECM pod can't shut up
            fcc_fault,  // Fire Control Computer. If failed_fault, no weapons solutions
            fcr_fault,  // Fire Control Radar. If failed_fault, no radar
            flcs_fault, // Digital Flight Control System. If failed one or more control surfaces stuck
            fms_fault,  // Fuel Measurement System. If failed_fault, fuel gauge stuck
            gear_fault, // Landing Gear. If failed one or more wheels stuck
            gps_fault,  // Global positioning system failure, no change if CADC or INS
            harm_fault, // Broken Harms
            hud_fault,  // Heads Up Display. If failed_fault, no HUD
            iff_fault,  // Identification_fault, Friend or Foe. If failed_fault, no IFF
            ins_fault,  // Inertial Navigation System. If failed_fault, no change in waypoint data
            isa_fault,  // Integrated servo actuator. If failed_fault, no rudder
            mfds_fault, // Multi Function Display Set. If an MFD fails_fault, it shows noise
            msl_fault,  // Missile Slave Loop. If failed_fault, missile will not slave to radar
            ralt_fault, // Radar Altimeter. If failed_fault, no ALOW warning
            rwr_fault,  // Radar Warning Reciever. If failed_fault, no rwr
            sms_fault,  // Stores Management System. If failed_fault, no weapon or inventory display_fault, and no launch
            tcn_fault,  // TACAN. If failed no TACAN data
            ufc_fault,  // Up Front Controller. If failed_fault, UFC/DED inoperative
            NumFaultListSubSystems,
            landing, takeoff, // pseudo entries for MFL
            TotalFaultStrings,
        };

        //-------------------------------------------------

        public enum type_FFunction
        {
            nofault = 0x0,    //0
            bus = 0x1,
            slnt = 0x2,
            chaf = 0x4,
            flar = 0x8,    // 5
            dmux = 0x10,
            dual = 0x20,
            sngl = 0x40,
            a_p = 0x80,   // 8
            rudr = 0x100,
            all = 0x200,
            xmtr = 0x400,
            a_i = 0x800,  // 12
            a_b = 0x1000,
            pfl = 0x2000,
            efire = 0x4000,
            hydr = 0x8000, //16
            m_3 = 0x10000,
            m_c = 0x20000,
            slv = 0x40000,
            lfwd = 0x80000,	//20
            rfwd = 0x100000,
            sta1 = 0x200000,
            sta2 = 0x400000,
            sta3 = 0x800000,	// 24
            sta4 = 0x1000000,
            sta5 = 0x2000000,
            sta6 = 0x4000000,
            sta7 = 0x8000000,	// 28
            sta8 = 0x10000000,
            sta9 = 0x20000000,
            ldgr = 0x40000000,
            fl_out = 0x80000000,	// 32
            NumFaultFunctions = 33
        };

        //-------------------------------------------------

        public enum type_FSeverity
        {
            cntl, degr,
            fail, low,
            rst, temp,
            warn, no_fail,
            NumFaultSeverity
        };

        //-------------------------------------------------
        // Structures for returning data when calling
        // GetFault() and GetFaultNames()
        //-------------------------------------------------

        public struct str_FEntry
        {
            public type_FFunction elFunction;
            public type_FSeverity elSeverity;
        }

        //-------------------------------------------------

        public struct str_FNames
        {
            public string elpFSubSystemNames;
            public string elpFFunctionNames;
            public string elpFSeverityNames;
        }

        public struct str_DmgProbs
        {
            public int numFuncs;
            public float systemProb;
            public float* funcProb;
        }

        public struct FaultListItem
        {
            public type_FSubSystem type; // which system
            public int subtype; // what fault number
            public int no; // count
            public int time; // when in seconds
        }


        private str_FEntry[] mpFaultList = new str_FEntry[(int)type_FSubSystem.TotalFaultStrings];
        private int mFaultCount;
        private FaultListItem[] mMflList = new FaultListItem[MAX_MFL];
        private VU_TIME mStartTime;
        private int mLastMfl;

        public enum FF
        { // fault flags
            FF_NONE = 0x00,
            FF_PSUEDO = 0x01, // not a real fault
            FF_PFL = 0x02, // significant for PFL
        }

        public struct InitFaultData
        { // JPO - initialisation structure
            public string mpFSSName;
            public type_FFunction mBreakable; // the functions
            public float mSProb; // system probability
            public float[] mFProb; // function probability
            public int mCount;	// count of these & breakables
            public uint flags;
        }


        public static string[] mpFFunctionNames = new string[(int)type_FFunction.NumFaultFunctions];
        public static string[] mpFSeverityNames = new string[(int)type_FSeverity.NumFaultSeverity];

        public type_FFunction PickFunction(type_FSubSystem system)
        {
            type_FFunction retval = type_FFunction.nofault;
            float pFail = (float)Rand() / (float)RAND_MAX;
            int i, counter;
            type_FFunction breakable = mpFaultData[(int)system].mBreakable;

            for (i = 0; i < mpFaultData[(int)system].mCount; i++)
            {
                if (mpFaultData[(int)system].mFProb[i] >= pFail)
                {
                    break;
                }
            }

            // Find the i'th bit in the failure
            i++;
            counter = -1;
            while (i != 0)
            {
                counter++;
                if (breakable & (1 << counter))
                    i--;
            }

            retval = (type_FFunction)(1 << counter);

            return retval;
        }

        public type_FSubSystem PickSubSystem(int systemBits)
        {
            type_FSubSystem retval = type_FSubSystem.amux_fault;
            int[] failedThings = new int[NumFaultListSubSystems];
            int failedThing;
            int i, j = 0;

            for (i = 0; i < FaultClass.NumFaultListSubSystems; i++)
            {
                if (subsystemBits & (1 << i))
                {
                    failedThings[j] = i;
                    j++;
                }
            }

            // Choose one of the available
            failedThing = rand() % j;
            failedThing = failedThings[failedThing];

            // Did it fail?
            if (mpFaultData[failedThing].mSProb <= (float)rand() / (float)RAND_MAX)
                retval = (type_FSubSystem)failedThing;

            return retval;
        }

        public bool IsFlagSet()
        {
            Debug.WriteLine("remove call\n");
            return false;
        }

        public void ClearFlag()
        {

        }

        public void SetFault(type_FSubSystem subsystem,
               type_FFunction function,
               type_FSeverity severity,
               bool doWarningMsg)
        {

            if (mpFaultList[(int)subsystem].elFunction == nofault)
            {

                mFaultCount++;

                if (doWarningMsg)
                {
                    // sound effect warning goes here?
                    F4SoundFXSetDist(SFX_BB_WARNING, FALSE, 0.0f, 1.0f);
                }
            }

            mpFaultList[(int)subsystem].elFunction |= function;
            mpFaultList[(int)subsystem].elSeverity = severity;
            AddMflList(SimLibElapsedTime, subsystem, (int)severity);
        }

        public void ClearFault(type_FSubSystem subsystem)
        {

            if (mpFaultList[(int)subsystem].elFunction != nofault)
            {
                mFaultCount--;
                //MI small fixup
                if (mFaultCount < 0)
                    mFaultCount = 0;
                //		mpFaultList[subsystem].elFunction	= nofault;
            }
        }

        public void ClearFault(type_FSubSystem subsystem, type_FFunction function)
        {
            mpFaultList[(int)subsystem].elFunction &= ~function;

            if (mpFaultList[(int)subsystem].elFunction == type_FFunction.nofault)
            {
                mFaultCount--;
                //MI small fixup
                if (mFaultCount < 0)
                    mFaultCount = 0;
            }
        }


        public void GetFault(type_FSubSystem subsystem, str_FEntry entry)
        {

            entry.elFunction = mpFaultList[(int)subsystem].elFunction;
            entry.elSeverity = mpFaultList[(int)subsystem].elSeverity;
        }

        public type_FFunction GetFault(type_FSubSystem subsystem)
        {

            return mpFaultList[(int)subsystem].elFunction;
        }

        public void GetFaultNames(type_FSubSystem subsystem,
                                int funcNum,
                 str_FNames names)
        {
#if TODO
	Debug.Assert(FALSE == F4IsBadReadPtr(names, sizeof *names));
	Debug.Assert(FALSE == F4IsBadReadPtr(mpFaultData, sizeof *mpFaultData));
	Debug.Assert(FALSE == F4IsBadReadPtr(mpFFunctionNames, sizeof *mpFFunctionNames));
	Debug.Assert(FALSE == F4IsBadReadPtr(mpFSeverityNames, sizeof *mpFSeverityNames));
#endif
            names.elpFSubSystemNames = mpFaultData[(int)subsystem].mpFSSName;
            names.elpFFunctionNames = mpFFunctionNames[funcNum];
            names.elpFSeverityNames = mpFSeverityNames[mpFaultList[(int)subsystem].elSeverity];
        }

        public int GetFaultCount() { return mFaultCount; }
        public void TotalPowerFailure()
        {
            int i, j;
            // set practically every fault known...
            for (i = 0; i < NumFaultListSubSystems; i++)
            {
                //if (i == ufc_fault) continue; // for debugging - so we can see whats happening
                for (j = 0; j < NumFaultFunctions; j++)
                {
                    switch (1 << j)
                    {
                        case efire: // skip engine fire
                        case ldgr: // skip landing gear
                            break;
                        default:
                            if (mpFaultData[i].mBreakable & (1 << j))
                            {
                                SetFault((type_FSubSystem)i, (type_FFunction)(1 << j), fail, TRUE);
                            }
                    }
                }
            }
        }

        public int Breakable(type_FSubSystem id) { return mpFaultData[(int)id].mBreakable; }

        // MFL support
        public void AddMflList(VU_TIME thetime, FaultClass.type_FSubSystem type, int subtype)
        {
            for (int i = 0; i < mLastMfl; i++)
            {
                if (mMflList[i].type == type &&
                    mMflList[i].subtype == subtype)
                {
                    mMflList[i].no++;
                    return;
                }
            }
            if (mLastMfl >= MAX_MFL) return;
            mMflList[mLastMfl].time = (int)((thetime - mStartTime) * MSEC_TO_SEC); // delta from start
            mMflList[mLastMfl].type = type;
            mMflList[mLastMfl].no = 1;
            mMflList[mLastMfl].subtype = subtype;
            mLastMfl++;
        }

        public bool GetMflEntry(int n, out string name, out int subsys, out int count, out string timestr)
        {
            Debug.Assert(n >= 0 && n < mLastMfl);
            if (n < 0 || n >= mLastMfl)
                return false;
            Debug.Assert(mMflList[n].type >= 0 && mMflList[n].type < TotalFaultStrings);
            name = mpFaultData[mMflList[n].type].mpFSSName;
            subsys = mMflList[n].subtype;
            count = mMflList[n].no;
            timestr = string.Format("%3d:%02d", mMflList[n].time / 60, mMflList[n].time % 60);
            return true;
        }

        public int GetMflListCount() { return mLastMfl; }
        public void ClearMfl() { mLastMfl = 0; }
        public void SetStartTime(VU_TIME time) { mStartTime = time; }

        public bool FindFirstFunction(type_FSubSystem sys, out int functionp)
        {
            for (int i = 0; i < NumFaultFunctions - 1; i++)
            {
                if (mpFaultList[sys].elFunction & (1U << i))
                {
                    functionp = i + 1;
                    return true;
                }
            }
            return false;
        }

        public bool FindNextFunction(type_FSubSystem sys, ref int functionp)
        {
            for (int i = functionp; i < NumFaultFunctions - 1; i++)
            {
                if (mpFaultList[sys].elFunction & (1U << i))
                {
                    functionp = i + 1;
                    return true;
                }
            }
            return false;
        }

        public bool GetFirstFault(out type_FSubSystem subsystemp, out int functionp)
        {
            for (int i = 0; i < FaultClass.NumFaultListSubSystems; i++)
            {
                if (mpFaultList[i].elFunction != nofault)
                {
                    subsystemp = (type_FSubSystem)i;
                    return FindFirstFunction((type_FSubSystem)i, functionp); // this should be true
                }
            }
            return false;
        }

        public bool GetNextFault(out type_FSubSystem subsystemp, out int functionp)
        {
            if (FindNextFunction(*subsystemp, functionp) == true)
                return true;

            for (int i = (subsystemp) + 1; i < FaultClass.NumFaultListSubSystems; i++)
            {
                if (mpFaultList[i].elFunction != nofault)
                {
                    subsystemp = (type_FSubSystem)i;
                    return FindFirstFunction((type_FSubSystem)i, functionp);
                }
            }
            return false;
        }

        public FaultClass()
        {

            int i;

            for (i = 0; i < NumFaultListSubSystems; i++)
            {

                mpFaultList[i].elFunction = type_FFunction.nofault;
                mpFaultList[i].elSeverity = type_FSeverity.no_fail;
            }
            mFaultCount = 0;
            //TODO ZeroMemory(mMflList, sizeof mMflList);
            mLastMfl = 0;
            mStartTime = 0;
        }
        //TODO public ~FaultClass();

        // Idea is to try and gather all fault data together. 
        // pity about the probabilities...
        static float[] parray1 = { 1.0f };
        static float[] cmds_array = { 0.2f, 0.6f, 1.0f };
        static float[] eng_array = { 0.2f, 0.4f, 0.6f, 0.7f, 0.8f, 1.0f };
        static float[] fcr_array = { 0.3f, 0.65f, 1.0f };
        static float[] flcs_array = { 0.2f, 0.4f, 0.8f, 1.0f };
        static float[] rudr_array = { 0.8f, 1.0f };
        static float[] mfds_array = { 0.5f, 1.0f };
        static float[] sms_array = { 0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.9f, 1.0f };

        //#define FDATA(s, type, sprob, aprob) { s, (type_FFunction)(type), (float)sprob, aprob, sizeof(aprob)/sizeof(aprob[0])}
        
        public static readonly FaultClass.InitFaultData[] mpFaultData = new InitFaultData[(int)type_FSubSystem.TotalFaultStrings] {
    FDATA("AMUX", bus,	0.1f, parray1),
	FDATA("BLKR", bus,	0.2f, parray1),
	FDATA("BMUX", bus,	0.1f, parray1),
	FDATA("CADC", bus,	0.1f, parray1),
	FDATA("CMDS", chaf|flar|bus, 0.15f, cmds_array),
	FDATA("DLNK", bus,	0.5f, parray1),
	FDATA("DMUX", bus,	0.05, parray1),
	FDATA("DTE",  bus,	0.2f, parray1),
	FDATA("ENG",  a_i|a_b|pfl|efire|hydr|fl_out, 0.4f, eng_array),
	FDATA("EPOD", slnt,	0.2f, parray1),
	FDATA("FCC",  bus,	0.2f, parray1),
	FDATA("FCR",  bus|sngl|xmtr, 0.2f, fcr_array),
	FDATA("FLCS", dmux|dual|sngl|a_p, 0.3f, flcs_array),
	FDATA("FMS",  bus,	0.1f, parray1),
	FDATA("GEAR", ldgr,	0.5f, parray1),
	FDATA("GPS",  bus,	0.5f, parray1),
	FDATA("HARM", bus,	0.5f, parray1),
	FDATA("HUD",  bus,	0.4f, parray1),
	FDATA("IFF",  bus,	0.2f, parray1),
	FDATA("INS",  bus,	0.2f, parray1),
	FDATA("ISA",  all|rudr,	0.2f, rudr_array),
	FDATA("MFDS", lfwd|rfwd,0.3f, mfds_array),
	FDATA("MSL",  bus,	0.0f, parray1),
	FDATA("RALT", xmtr,	0.3f, parray1),
	FDATA("RWR",  bus,	0.2f, parray1),
	FDATA("SMS",  bus | sta1 | sta2 | sta3 | sta4 | sta5 | sta6 | sta7 | sta8 | sta9,
				0.1f, sms_array),
	FDATA("TCN",  bus,	0.2f, parray1),
	FDATA("UFC",  bus,	0.2f, parray1),
	FDATA("???",  bus, 0, parray1), // bogus entries
	FDATA("LAND",  bus, 0, parray1), // bogus entries
	FDATA("TOF", bus, 0, parray1), // bogus entries
};

        const string[] mpFFunctionNames = 
        {
                           "",
                           "BUS",
                           "SLNT",
                           "CHAF",
                           "FLAR",
                           "DMUX",
                           "DUAL",
                           "SNGL",
                           "A/P",
                           "RUDR",
                           "ALL",
                           "XMTR",
                           "A/I",
                           "A/B",
                           "PFL",
                           "FIRE",
                           "HYDR",
                           "M 3",
                           "M C",
                           "SLV",
                           "LFWD",
                           "RFWD",
                           "STA1",
                           "STA2",
                           "STA3",
                           "STA4",
                           "STA5",
                           "STA6",
                           "STA7",
                           "STA8",
                           "STA9",
                           "LDGR",
	                   "FLOUT",
            };

        const string[] mpFSeverityNames = new string[NumFaultSeverity]  {
					"CNTL",	"DEGR",
					"FAIL",	"LOW",
					"RST",	"TEMP",
					"WARN",  ""
            };
#endif
    }
}
