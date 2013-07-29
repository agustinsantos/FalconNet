using System;
using FalconNet.VU;
using FalconNet.Common;
using log4net;
using FalconNet.F4Common;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;
using VU_ID_NUMBER = System.UInt64;
using VU_KEY = System.UInt64;
using VU_BOOL = System.Boolean;
using BIG_SCALAR = System.Single;
using SM_SCALAR = System.Single;

namespace FalconNet.FalcLib
{

    // ==============================
    // Vu Class heirarchy defines
    // ==============================
    public static class Vu_CLASS
    {
        public const int VU_DOMAIN = 0;
        public const int VU_CLASS = 1;
        public const int VU_TYPE = 2;
        public const int VU_STYPE = 3;
        public const int VU_SPTYPE = 4;
        public const int VU_OWNER = 5;
    }

    // ========================
    // Vis type defines
    // ========================
    public enum VIS_TYPES
    {
        VIS_NORMAL = 0,
        VIS_REPAIRED = 1,
        VIS_DAMAGED = 2,
        VIS_DESTROYED = 3,
        VIS_LEFT_DEST = 4,
        VIS_RIGHT_DEST = 5,
        VIS_BOTH_DEST = 6
    }



    // =========================
    // Function Prototypes
    // =========================

    public static class F4VUStatic
    {
        // ========================
        // These never change
        // ========================

        // Move definition to VuConsts TODO delete these lines
        //public const int DOMAIN_ANY = 255;
        //public const int CLASS_ANY = 255;
        //public const int TYPE_ANY = 255;
        //public const int STYPE_ANY = 255;
        //public const int SPTYPE_ANY = 255;
        //public const int RFU1_ANY = 255;
        //public const int RFU2_ANY = 255;
        //public const int RFU3_ANY = 255;
        //public const int VU_ANY = 255;


        // ========================
        // Filter Defines
        // ========================

        public const int VU_FILTERANY = 255;
        public const int VU_FILTERSTOP = 0;

        // ========================
        // Default values
        // ========================

        public const int F4_EVENT_QUEUE_SIZE = 2000;		// How many events we can have on the queue at one heading
        public const int F4_ENTITY_TABLE_SIZE = 10529; // Size of vu's hash table  - this is a prime number // JB 010718

        public static VuMainThread gMainThread;
        public static int NumEntities;
        public static VU_ID FalconNullId;
        public static F4CSECTIONHANDLE vuCritical;

        public static void InitVU()
        {

#if USE_SH_POOLS
    gVuMsgMemPool = MemPoolInit(0);
    gVuFilterMemPool = MemPoolInit(0);

    VuLinkNode::InitializeStorage();
    VuRBNode::InitializeStorage();
#endif

            // Make sure we're using the right VU
#if (VU_VERSION_USED  != VU_VERSION)
#error "Incorrect VU Version"
#endif

#if (VU_REVISION_USED != VU_REVISION)
#error "Incorrect VU Revision"
#endif

#if (VU_PATCH_USED    != VU_PATCH)
#error "Incorrect VU Patch"
#endif

#if NDEBUG // Differentiate Debug & Release versions so they can't be seen by each other (PJW)
    sprintf(tmpStr, "R%5d.%2d.%02d.%s.%d_\0", BuildNumber, gLangIDNum, MinorVersion, "EBS", MajorVersion);
#else
            //TODO string tmpStr = string.Format("K%5d.%2d.%02d.%s.%d_\0", BuildNumber, gLangIDNum, MinorVersion, "EBS", MajorVersion);
            string tmpStr = VUSTATIC.About;
#endif

            log.Debug(VUSTATIC.About);

            // Change this to stop different versions taking to each other

            // OW FIXME
            // strcpy (tmpStr, "F527");
            // strcpy(tmpStr, "F552"); //  according to REVISOR this will allow connections to 1.08 servers. we'll see
            //strcpy(tmpStr, "E109newmp"); //me123 we are not interested in 108 conections anymore since they'll ctd us
            tmpStr = F4Config.g_strWorldName;
            VUSTATIC.vuxWorldName = F4Config.g_strWorldName;

            // TODO #if VU_USE_ENUM_FOR_TYPES
            FalconMessageFilter falconFilter = new FalconMessageFilter(HandlingThread.SimThread, true);
            //#else
            //  FalconMessageFilter falconFilter = new FalconMessageFilter(HandlingThread.SimThread, VUBITS.VU_VU_MESSAGE_BITS);
            //#endif
            vuCritical = F4CriticalSection.F4CreateCriticalSection("Vu");
            //VU_ID_NUMBER low = FIRST_VOLATILE_VU_ID_NUMBER;
            //VU_ID_NUMBER hi = LAST_VOLATILE_VU_ID_NUMBER;
            gMainThread = new VuMainThread(
                /*low, hi, */F4_ENTITY_TABLE_SIZE, falconFilter, F4_EVENT_QUEUE_SIZE, vuxCreateSession
            );

            // Default VU namespace
            /*vuAssignmentId = FIRST_VOLATILE_VU_ID_NUMBER;
            vuLowWrapNumber = FIRST_VOLATILE_VU_ID_NUMBER;
            vuHighWrapNumber = LAST_VOLATILE_VU_ID_NUMBER;*/
        }

        public static void ExitVU()
        {
#if TODO
            //delete(gMainThread);
            //delete [] vuxWorldName;
            gMainThread = null;
            F4DestroyCriticalSection(vuCritical);
            vuCritical = null;

#if USE_SH_POOLS
    VuLinkNode::ReleaseStorage();
    VuRBNode::ReleaseStorage();
    MemPoolFree(gVuMsgMemPool);
    MemPoolFree(gVuFilterMemPool);
#endif
#endif
            throw new NotImplementedException();
        }

        public static VuSessionEntity vuxCreateSession()
        {
            return (VuSessionEntity)new FalconSessionEntity(vuxLocalDomain, "Falcon 4.0");
        }
        




 

        public static VuDriverSettings vuxDriverSettings;
#if VU_AUTO_COLLISION
public static VuGridTree *vuxCollisionGrid;
#endif // VU_AUTO_COLLISION

        public static ulong vuxVersion = 1;
        public static SM_SCALAR vuxTicsPerSec = 1000.0F;
        public static VU_TIME vuxTargetGameTime = 0;
        public static VU_TIME vuxLastTargetGameTime = 0;
        public static VU_TIME vuxDeadReconTime = 0;
        public static VU_TIME vuxCurrentTime = 0;
        public static VU_TIME lastTimingMessage = 0;
        public static VU_TIME vuxTransmitTime = 0;
        public static VU_BYTE vuxLocalSession = 1;
        //ulong vuxLocalDomain = 1; // range = 1-31
        public static ulong vuxLocalDomain = 0xffffffff; // range = 1-31 // JB 010718

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}

