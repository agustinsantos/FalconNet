using System;
using System.IO;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using Objective = FalconNet.Campaign.ObjectiveClass;
using FalconNet.CampaignBase;

namespace FalconNet.Campaign
{


    public enum SPLF
    {
        SPLF_IS_LINKED = 0x01,				// Linked persistant object
        SPLF_IS_TIMED = 0x02,				// Timed persistant object
        SPLF_IN_USE = 0x04				// This entry is being used
    }
    // =============================
    // Packed VU_ID & index data
    // =============================

    public class PackedVUIDClass
    {

        public ulong creator_;
        public ulong num_;
        public ulong index_;
    };

    // =============================
    // Timed/Linked Persistant Class
    // =============================

    // Base persistant object class
    public class SimPersistantClass
    {


#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap fixed size pool
		void *operator new(size_t size) { ShiAssert( size == sizeof(SimPersistantClass) ); return MemAllocFS(pool);	};
		void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
		static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(SimPersistantClass), MAX_PERSISTANT_OBJECTS, 0 ); };
		static void ReleaseStorage()	{ MemPoolFree( pool ); };
		static MEM_POOL	pool;
#endif


        public float x, y;
        //TODO public DrawableBSP  	drawPointer;
        public struct union
        {
            CampaignTime removeTime;
            PackedVUIDClass campObject;
        }				;
        public union unionData;
        public short visType;
        public SPLF flags;


        // constructors/destructors
        public SimPersistantClass()
        { throw new NotImplementedException(); }
        //TODO public ~SimPersistantClass();

        // serialization functions
        public void Load(byte[] stream, ref int pos)
        { throw new NotImplementedException(); }
        public void Load(FileStream filePtr)
        { throw new NotImplementedException(); }
        public int SaveSize()
        { throw new NotImplementedException(); }
        public int Save(byte[] stream, ref int pos)
        { throw new NotImplementedException(); }									// returns bytes written
        public int Save(FileStream file)
        { throw new NotImplementedException(); }										// returns bytes written

        // function interface
        public void Init(int visualType, float X, float Y)// Sets up initial data
        {
            x = X;
            y = Y;
            visType = (short)visualType;
        }
        public void Deaggregate()
        { throw new NotImplementedException(); }									// Makes this drawable
        public void Reaggregate()
        { throw new NotImplementedException(); }									// Cleans up the drawable object
        public bool IsTimed() { return flags.IsFlagSet(SPLF.SPLF_IS_TIMED); }
        public bool IsLinked() { return flags.IsFlagSet(SPLF.SPLF_IS_LINKED); }
        public bool InUse() { return flags.IsFlagSet(SPLF.SPLF_IN_USE); }
        public void Cleanup()
        { throw new NotImplementedException(); }
        public CampaignTime RemovalTime()
        { throw new NotImplementedException(); }
        public FalconEntity GetCampObject()
        { throw new NotImplementedException(); }
        public int GetCampIndex()
        { throw new NotImplementedException(); }
    }


    public static class PersistStatic
    {
        // =============================
        // Defines and Flags
        // =============================

        public const int MAX_PERSISTANT_OBJECTS = 5000;				// Max number of persistant objects
        // =============================
        // Default timeout values
        // =============================

        public const int CRATER_REMOVAL_TIME = 3600000;		// Removal time, in campaign time
        public const int HULK_REMOVAL_TIME = 7200000;		// Removal time, in campaign time

        // =============================
        // Global access functions
        // =============================

        public static void InitPersistantDatabase()
        { throw new NotImplementedException(); }

        public static void CleanupPersistantDatabase()
        { throw new NotImplementedException(); }

        // These two functions will create a persistant object and broadcast to all remote machines
        public static void AddToTimedPersistantList(int vistype, CampaignTime removalTime, float x, float y)
        { throw new NotImplementedException(); }
        public static void AddToLinkedPersistantList(int vistype, FalconEntity campObj, int campIdx, float x, float y)
        { throw new NotImplementedException(); }

        // These two functions will create a persistant object LOCALLY ONLY
        public static void NewTimedPersistantObject(int vistype, CampaignTime removalTime, float x, float y)
        { throw new NotImplementedException(); }
        public static void NewLinkedPersistantObject(int vistype, VU_ID campObjID, int campIdx, float x, float y)
        { throw new NotImplementedException(); }

        public static void SavePersistantList(string scenario)
        { throw new NotImplementedException(); }

        public static void LoadPersistantList(string scenario)
        { throw new NotImplementedException(); }

        public static int EncodePersistantList(byte[] stream, ref int pos, int maxSize)
        { throw new NotImplementedException(); }

        public static void DecodePersistantList(byte[] stream, ref int pos)
        { throw new NotImplementedException(); }

        public static int SizePersistantList(int maxSize)
        { throw new NotImplementedException(); }

        public static void CleanupPersistantList()
        { throw new NotImplementedException(); }

        public static void UpdatePersistantObjectsWakeState(float px, float py, float range, CampaignTime now)
        { throw new NotImplementedException(); }

        public static void CleanupLinkedPersistantObjects(FalconEntity campObject, int index, int newVis, int ratio)
        { throw new NotImplementedException(); }

        // These functions were designed to allow the campaign to create local copies of persistant
        // entities upon receiving a damage message
        public static void AddRunwayCraters(Objective o, int f, int craters)
        { throw new NotImplementedException(); }
        public static void AddMissCraters(FalconEntity e, int craters)
        { throw new NotImplementedException(); }
        public static void AddHulk(FalconEntity e, int hulkVisId)
        { throw new NotImplementedException(); }

        // This stuff should go to another file

        public static void UpdateNoCampaignParentObjectsWakeState(float px, float py, float range)
        { throw new NotImplementedException(); }

    }

}
