using FalconNet.CampaignBase;
using FalconNet.VU;
using System;
using FalconNet.Common;
using FalconNet.FalcLib;
using Team = System.Byte;
using GridIndex = System.Int16;

namespace FalconNet.FalcLib
{
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
        public ROEEngagementQueryTypeEnum roe_check;
        public byte delayed;					// number of times it's been pushed back
        public byte start_block;				// time block we're taking off during
        public byte final_block;				// time block we're landing during
        public byte[] slots = new byte[4] { 255, 255, 255, 255 };		// squadron slots we're using.
        public sbyte min_to;						// minimum block we found planes for
        public sbyte max_to;						// maximum block we found planes for

        public MissionRequestClass()
        {
            targetID = VU_ID.FalconNullId;
            requesterID = VU_ID.FalconNullId;
            who = vs = 0;
            tot = 0;
            tx = ty = 0;
            flags = 0;
            caps = 0;
            target_num = 255;
            tot_type = 0;
            mission = 0;
            priority = 0;
            speed = 0;
            match_strength = 0;
            aircraft = 0;
            context = 0;
            roe_check = 0;
            delayed = 0;
            start_block = 0;
            final_block = 0;
            action_type = 0;
            min_to = -127;
            max_to = 127;
        }

        //TODO public ~MissionRequestClass();
        public int RequestMission()
        {
            if (mission == 0 || priority < 0 || (vs != 0 && TeamStatic.GetRoE(who, vs, roe_check) == ROEAllowedEnum.ROE_NOT_ALLOWED))
                return -1;

            if ((TeamStatic.TeamInfo[who] != null) &&
                (TeamStatic.TeamInfo[who].atm != null) && 
                (TeamStatic.TeamInfo[who].flags.HasFlag(TeamFlagEnum.TEAM_ACTIVE)))
            {
                VuTargetEntity target = (VuTargetEntity)VUSTATIC.vuDatabase.Find(TeamStatic.TeamInfo[who].atm.OwnerId());
                FalconMissionRequestMessage message =
                    new FalconMissionRequestMessage(TeamStatic.TeamInfo[who].atm.Id(), VUSTATIC.vuLocalSessionEntity);
                TeamStatic.GetPriority(this);
                message.request = this;
                message.team = who;

                if (priority > 0)
                {
                    FalcMesgStatic.FalconSendMessage(message, false); // KCK NOTE: Go ahead and let a few messages miss their target
                    return 0;
                }
                else
                    ;//  delete message; // JPO mem leak
                }

            return -1;
        }


        public int RequestEnemyMission()
        {
            short bonus = priority;

            if (vs == 0)
                return -1;

            for (who = 1; who < (int)TeamDataEnum.NUM_TEAMS; who++)
            {
                if (TeamStatic.TeamInfo[who] != null)
                {
                    priority = bonus; // Reset to bonus priority;
                    RequestMission();
                }
            }

            return 0;
        }
    }
}
