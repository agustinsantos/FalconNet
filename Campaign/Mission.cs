using System;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using Flight = FalconNet.Campaign.FlightClass;
using Team = System.SByte;
using GridIndex = System.Int16;
using FalconNet.CampaignBase;
namespace FalconNet.Campaign
{
    // ===================================================
    // Mission defines
    // ===================================================


    // Returned by TargetThreats
    // Flags for what we really need
    [Flags]
    public enum TargetThreatReturnFlagEnum
    {
        NEED_SEAD = 0x01,
        NEED_ECM = 0x02,

        // Specifics as to the threat types
        THREAT_LALT_SAM = 0x10,
        THREAT_HALT_SAM = 0x20,
        THREAT_AAA = 0x40,

        // Specifics as to the threat location
        THREAT_ENROUTE = 0x100,
        THREAT_TARGET = 0x200,
    }

    // Mission profiles
    public enum MissionProfileEnum
    {
        MPROF_LOW = 0x01,				// Low engress profile
        MPROF_HIGH = 0x02,				// High engress profile
    }

    // Target area profiles
    public enum TargetAreaProfileEnum
    {
        TPROF_NONE = 0,					// No target WP
        TPROF_ATTACK = 1,					// IP, Target, Turn point  (Assembly, Break point)
        TPROF_HPATTACK = 2,					// IP, Target (Assembly, Break point)
        TPROF_LOITER = 3,					// 2 Turn points (Assembly)
        TPROF_TARGET = 4,					// Target only (Assembly, Break point)
        TPROF_AVOID = 5,					// Break point, Turn point (Assembly)
        TPROF_SWEEP = 6,					// 3 circular Turn points
        TPROF_FLYBY = 7,					// Pass over target, at lowest possible threat
        TPROF_SEARCH = 8,					// Like loiter, but at full speed
        TPROF_LAND = 9,					// Land at target for station time, then takeoff again
    }

    // Target desctiption types (when to use target actions)
    public enum TargetDescriptionType
    {
        TDESC_NONE = 0,
        TDESC_TTL = 1,					// Takeoff To Landing (mission actions during whole route)
        TDESC_ATA = 2,					// Assembly To Assembly
        TDESC_TAO = 3,					// Target Area Only
    }

    // Special time on target types
    public enum SpecialTOTTypeEnum
    {
        TOT_TAKEOFF = 10,
        TOT_ENROUTE = 11,
        TOT_INGRESS = 12,
    }



    // ===================================================
    // Mission Request Class		
    // ===================================================

    // This class is filled in order to request a mission
    public class MissionRequestClass
    {
#if USE_SH_POOLS
   public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { Debug.Assert( size == sizeof(MissionRequestClass) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(MissionRequestClass), 50, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif
        public VU_ID requesterID;
        public VU_ID targetID;
        public VU_ID secondaryID;				// Is this being used?
        public VU_ID pakID;
        public Team who;
        public Team vs;
        public CampaignTime tot;
        public GridIndex tx, ty;
        public ulong flags;
        public short caps;
        public short target_num;
        public short speed;
        public short match_strength;				// How much Air to Air strength we should try to match
        public short priority;
        public byte tot_type;
        public byte action_type;				// Type of action we're associated with
        public byte mission;
        public byte aircraft;
        public byte context;					// Context code (why this was requested)
        public byte roe_check;
        public byte delayed;					// number of times it's been pushed back
        public byte start_block;				// time block we're taking off during
        public byte final_block;				// time block we're landing during
        public byte[] slots = new byte[4];					// squadron slots we're using.
        public char min_to;						// minimum block we found planes for
        public char max_to;						// maximum block we found planes for

        public MissionRequestClass()
        {
            throw new NotImplementedException();
        }
        //TODO public ~MissionRequestClass();
        public int RequestMission()
        {
            throw new NotImplementedException();
        }

        public int RequestEnemyMission()
        {
            throw new NotImplementedException();
        }
    };

    // TODO typedef MissionRequestClass MissionRequest;

    // ======================================================
    // Mission Data Type - Stores data used to build missions
    // ======================================================

    public struct MissionDataType
    {
        public byte type;
        public byte target;						// Target type
        public byte skill;						// Primary skill to look for
        public byte mission_profile;			// Allowable mission profiles
        public byte target_profile;				// Type of target profile
        public byte target_desc;				// When to use our target action (target area/whole mission/etc)
        public byte routewp;					// Waypoint type along route
        public byte targetwp;					// What to do at target location
        public short minalt;						// Minimum alt at target (in hundreds of feet)
        public short maxalt;						// Maximum alt at target (in hundreds of feet)
        public short missionalt;					// Suggested mission altitude (in hundreds of feet)
        public short separation;					// Seperation from main flight, in seconds
        public short loitertime;					// Loiter time in minutes
        public byte str;						// Aircraft strength typically assigned
        public byte min_time;					// Minimum # of minutes in advance to consider a planned mission
        public byte max_time;					// Maximum # of minutes in advance to consider a planned mission
        public byte escorttype;					// What sort of mission to build for an escort
        public byte mindistance;				// Minimum distance between two similar missions
        public byte mintime;					// Minimum time between missions of similar types
        public byte caps;						// Special capibilities required (stealth, navy, etc)
        public ulong flags;						// flags
    };

    public static class MissionStatic
    {
        public static MissionDataType[] MissionData = new MissionDataType[(int)MissionTypeEnum.AMIS_OTHER];

        // ===================================================
        // Global functions
        // ===================================================

        public static int BuildPathToTarget(Flight u, MissionRequestClass mis, VU_ID airbaseID)
        {
            throw new NotImplementedException();
        }

        public static void BuildDivertPath(Flight u, MissionRequestClass mis)
        {
            throw new NotImplementedException();
        }

        public static void AddInformationWPs(Flight u, MissionRequestClass mis)
        {
            throw new NotImplementedException();
        }

        public static void ClearDivertWayPoints(Flight flight)
        {
            throw new NotImplementedException();
        }

        public static int AddTankerWayPoint(Flight u, int refuel)
        {
            throw new NotImplementedException();
        } // M.N.

        public static long SetWPTimes(Flight u, MissionRequestClass mis)
        {
            throw new NotImplementedException();
        }

        public static long SetWPTimesTanker(Flight u, MissionRequestClass mis, bool type, CampaignTime time)
        {
            throw new NotImplementedException();
        }

        public static int SetWPAltitudes(Flight u)
        {
            throw new NotImplementedException();
        }

        public static int CheckPathThreats(Unit u)
        {
            throw new NotImplementedException();
        }

        public static int TargetThreats(Team team, int priority, F4PFList list, MoveType mt, CampaignTime time, long target_flags, ref short targeted)
        {
            throw new NotImplementedException();
        }

        public static bool LoadMissionData()
        {
            throw new NotImplementedException();
        }
    }
}

