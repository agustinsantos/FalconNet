using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Float32 = System.Single;
using BOOL = System.Boolean;
using VU_TIME = System.UInt64;
using FalconNet.FalcLib;

namespace FalconNet.Sim
{
    public class SimObjectLocalData
    {
#if TODO
        public SimObjectLocalData()
        {
            painted = false;
            rdrDetect = detFlags = nextLOSCheck = 0;
            range = ata = az = el = droll = 0.0F;
        }

        // Absolute true relative geometry for this target
        public Float32 ata;				// "antenna train angle" is total angle from nose to target
        public Float32 ataFrom;			// total angle from targets nose to our own position
        public Float32 atadot;				// ata change rate (radians/sec)
        public Float32 ataFromdot;			// ataFrom rate
        public Float32 az;					// body relative angle to target in "yaw" plane
        public Float32 azFrom;				// same for targets view of us.
        public Float32 azFromdot;			// rate of change of azFrom (radians/sec)
        public Float32 el;					// body relative angle to target in "pitch" plane
        public Float32 elFrom, elFromdot;	// from target to use values for elevation
        public Float32 droll;				// body relative roll to target (how far to roll to get lift vector on target)
        public Float32 range, rangedot;	// range to target (feet & feet/sec)

        // Radar specific target data (move into Radar classes???)
        public BOOL painted;					// Was this target painted this frame
        public uint lockmsgsend;					// 0=no, 1 = lock 2 = unlock
        public int lastRadarMode;					// 2002-02-10 ADDED BY S.G. Need to know the last mode the radar was in
        public Float32 aspect;						// Target aspect (= 180.0F*DTR - ataFrom)
        public Int32[] rdrSy = new int[NUM_RADAR_HISTORY];	// radar symbol (assigned by exec in RadarDoppler)
        public Float32[] rdrX = new Float32[NUM_RADAR_HISTORY];	// azmuth in radians?
        public Float32[] rdrY = new Float32[NUM_RADAR_HISTORY];	// range in feet (radial)
        public Float32[] rdrHd = new Float32[NUM_RADAR_HISTORY];	// our heading at target paint time (platform.Yaw())
        public VU_TIME rdrLastHit;					// Last time this target was detected (SimLibElapsedTime)
        public UInt32 rdrDetect;					// Bit field indicating when we did/didn't detect the target

        // Digi use only
        public Float32 threatTime;					// How long for him to kill us (digi use only) 
        public Float32 targetTime;					// How long to kill this target (digi use only)

        // Per sensor data
        public Int32[] sensorLoopCount = new int[SensorClass.NumSensorTypes];	// Number of frames since the target was last seen
        public SensorClass.TrackTypes[] sensorState = new SensorClass.TrackTypes[SensorClass.NumSensorTypes];	// What kind of sensor lock do we have

        public Float32 irSignature;									// Should go away - look it up when required

        public UInt32 detFlags;				//flag field indicating which los's are true
        public UInt32 nextLOSCheck;			//next time to check LOS again

        public int CloudLOS() { return (detFlags & 0x01) && true; }
        public void SetCloudLOS(int value) { if (value != 0)detFlags |= 0x01; else detFlags &= ~0x01; }
        public int TerrainLOS() { return (detFlags & 0x02) && true; }
        public void SetTerrainLOS(int value) { if (value != 0)detFlags |= 0x02; else detFlags &= ~0x02; }
#if USE_SH_POOLS
   public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { ShiAssert( size == sizeof(SimObjectLocalData) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(SimObjectLocalData), 1000, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif
#endif
    }


    public class SimObjectType
    {
#if TODO
#if DEBUG
        public SimObjectType(string from, FalconEntity owner, FalconEntity baseObj);
        public SimObjectType Copy(string from, FalconEntity owner);
#else
	public SimObjectType(FalconEntity* baseObj);
	public SimObjectType* Copy(void);
#endif

#if SIMOBJ_REF_COUNT_DEBUG
	//TODO #define SIM_OBJ_REF_ARGS	__LINE__, __FILE__
	void Reference( int line, char *file );
	void Release( int line, char *file );

	struct DebugRecord {
		int					line;
		char				file[256];
		int					refInc;
		struct DebugRecord	*prev;
	} *refOps;
#else
        //TODO	#define SIM_OBJ_REF_ARGS
        public void Reference();
        public void Release();
#endif

        public FalconEntity BaseData() { return baseData; }
        public BOOL IsReferenced();


        //TODO private ~SimObjectType();
        private FalconEntity baseData;

        private int refCount;

#if DEBUG
        private int dbgIndex;
#endif


        public SimObjectLocalData localData;

        public SimObjectType next;
        public SimObjectType prev;


#if USE_SH_POOLS
   public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { ShiAssert( size == sizeof(SimObjectType) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(SimObjectType), 1000, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif
#endif
    }

    //TODO typedef SimObjectType* SimObjectPtr;

}
