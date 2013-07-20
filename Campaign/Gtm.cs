using System;
using System.IO;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using VU_BYTE = System.Byte;
using Team = System.SByte;

namespace FalconNet.Campaign
{

    public class GroundDoctrineType
    {
        byte[] stance = new byte[(int)CountryListEnum.NUM_COUNS];					// Our air stance towards them: allied/friendly/neutral/alert/hostile/war
        byte loss_ratio;							// Acceptable loss ratio
        byte loss_score;							// Score loss for each friendly air loss
    };

    public class GroundTaskingManagerClass : CampManagerClass
    {
        public short flags;
        // These don't need to be transmitted
        public GndObjDataType[] objList = new GndObjDataType[(int)GORD.GORD_LAST];				// Sorted lists of objectives we want to assign to
        public UnitScoreNode[] canidateList = new UnitScoreNode[(int)GORD.GORD_LAST];		// List of all possible canidate units for each order
        public short topPriority;					// Highest PO priority (for scaling)
        public short priorityObj;					// CampID of highest priority objective

        // constructors
        public GroundTaskingManagerClass(ushort type, Team t)
            : base(type, t)
        {
            flags = 0;
            topPriority = 0;
            //TODO memset(canidateList,0,sizeof(void*)*GORD_LAST);
            //TODO memset(objList,0,sizeof(void*)*GORD_LAST);
        }

#if TODO  
        public GroundTaskingManagerClass(VU_BYTE[] stream)
            : base(stream)
        { throw new NotImplementedException(); }
        public GroundTaskingManagerClass(FileStream file)
            : base(file)
        { throw new NotImplementedException(); }
        // public virtual ~GroundTaskingManagerClass();
        public virtual int SaveSize()
        {
            return base.SaveSize() + sizeof(short);
        }

        public virtual int Save(byte[] stream)
        {

	        base.Save(stream);
	        memcpy(*stream, &flags, sizeof(short));
            *stream += sizeof(short);
	        return this.SaveSize();

            throw new NotImplementedException();
        }

        public virtual int Save(FileStream file)
        {
#if TODO
            int retval = 0;

            if (file == null)
                return 0;
            retval += base.Save(file);
            retval += fwrite(&flags, sizeof(short), 1, file);
            return retval;
#endif
            throw new NotImplementedException();
        }

#endif
        // Required pure virtuals
        public override int Task()
        {
#if TODO
            int done = 0;
            int count = 0, collect;
            int action;

            // Don't do this if we're not active, or not owned by this machine
            if (!(TeamStatic.TeamInfo[owner].flags & TEAM_ACTIVE) || !IsLocal())
                return 0;

            action = TeamStatic.TeamInfo[owner].GetGroundActionType();

            // Check for offensive grinding to a halt
            if (action == GACTION_OFFENSIVE && TeamStatic.TeamInfo[owner].GetGroundAction().actionPoints == 0)
            {
                TeamStatic.TeamInfo[owner].SelectGroundAction();
                action = TeamStatic.TeamInfo[owner].GetGroundActionType();
            }

#if DEBUG
            ulong time;//,newtime;
            time = GetTickCount();
#endif

            Cleanup();

            // Choose types of orders we can give
            collect = COLLECT_AIRDEFENSE | COLLECT_SUPPORT | COLLECT_REPAIR | COLLECT_RESERVE | COLLECT_DEFEND | COLLECT_RADAR;
            if (action == GACTION_OFFENSIVE)
                collect |= COLLECT_CAPTURE | COLLECT_ASSAULT | COLLECT_AIRBORNE | COLLECT_COMMANDO | COLLECT_SECURE;
            else if (action == GACTION_MINOROFFENSIVE)
                collect |= COLLECT_SECURE;
            else if (action == GACTION_CONSOLIDATE)
                collect |= COLLECT_SECURE;

#if KEV_GDEBUG
	ulong	ltime;
	ltime = GetTickCount();
#endif

            if (CollectGroundAssets(collect))
            {
#if KEV_GDEBUG
		ListBuildTime = GetTickCount() - ltime;
#endif
                // Give orders based on action type
                switch (action)
                {
                    case GACTION_OFFENSIVE:
                        // Full offensive - priorities are offensive, securing, then defense
                        AssignUnits(GORD_CAPTURE, GTM_MODE_FASTEST);
                        //				if (NavalSuperiority(owner) >= STATE_CONTESTED)
                        AssignUnits(GORD_ASSAULT, GTM_MODE_BEST);
                        //				if (AirSuperiority(owner) >= STATE_CONTESTED)
                        {
                            AssignUnits(GORD_AIRBORNE, GTM_MODE_BEST);
                            AssignUnits(GORD_COMMANDO, GTM_MODE_BEST);
                        }
                        AssignUnits(GORD_SECURE, GTM_MODE_FASTEST);
                        AssignUnits(GORD_DEFEND, GTM_MODE_BEST);
                        break;
                    case GACTION_MINOROFFENSIVE:
                        // Consolidation/Counterattack phase - priorities are securing objectives then defense
                        AssignUnits(GORD_SECURE, GTM_MODE_BEST);
                        AssignUnits(GORD_DEFEND, GTM_MODE_BEST);
                    case GACTION_CONSOLIDATE:
                        // Cautious consolidation phase - priorities are defense, then securing objectives
                        AssignUnits(GORD_DEFEND, GTM_MODE_FASTEST);
                        AssignUnits(GORD_SECURE, GTM_MODE_BEST);
                        break;
                    case GACTION_DEFENSIVE:
                    default:
                        // Defensive posture - priorities are defense only
                        AssignUnits(GORD_DEFEND, GTM_MODE_FASTEST);
                        break;
                }

                // Now do the things we do all the time:
                AssignUnits(GORD_AIRDEFENSE, GTM_MODE_FASTEST);
                AssignUnits(GORD_SUPPORT, GTM_MODE_FASTEST);
                AssignUnits(GORD_REPAIR, GTM_MODE_FASTEST);
                AssignUnits(GORD_RADAR, GTM_MODE_FASTEST);
                AssignUnits(GORD_RESERVE, GTM_MODE_BEST);
            }

            // Check if our tasking failed to meet at least 50 of our offensive requests
            if (sOffensiveDesired && sOffensiveAssigned < sOffensiveDesired / 2)
                TeamStatic.TeamInfo[owner].GetGroundAction().actionPoints = 0;

#if KEV_GDEBUG
	int i;
	MonoPrint("Assigned:     ");
	for (i=0; i<GORD_LAST; i++)
		MonoPrint("%3d  ",AssignedCount[i]);
	MonoPrint("%3d\n",Assigned);
	MonoPrint("Time (s):     ");
	for (i=0; i<GORD_LAST; i++)
		MonoPrint("%3.1f  ",(float)(Time[i]/1000.0F));
	MonoPrint("\n");
#endif

            Cleanup();

#if KEV_GDEBUG
	newtime = GetTickCount();
	MonoPrint("Ground tasking for team %d: %d ms\n",owner,newtime-time);
#endif

            return Assigned;
#endif
            throw new NotImplementedException();
        }

        public virtual void DoCalculations()
        {
#if TODO
            Objective o;
            int score, fs, es, i;
            float d;
            Team t;
            GridIndex x, y;
            POData pd;
            VuListIterator poit = new VuListIterator(POList);

            // Don't do this if we're not active, or not owned by this machine
            if (!(TeamStatic.TeamInfo[owner].flags & TEAM_ACTIVE) || !IsLocal())
                return;

            topPriority = 0;
            o = GetFirstObjective(&poit);
            while (o != null)
            {
                // Get score for proximity to front
                o.GetLocation(&x, &y);
                d = DistanceToFront(x, y);
                fs = FloatToInt32((200.0F - d) * 0.2F);

                t = o.GetTeam();
                pd = GetPOData(o);
                es = 0;
                // Get score for enemy strength
                if (d < 100.0F)
                {
                    for (i = 1; i < TeamDataEnum.NUM_TEAMS; i++)
                    {
                        if (GetRoE(owner, i, ROE_GROUND_FIRE))
                            es += pd.ground_assigned[i] / 50;	// 1 assignment pt = 1 vehicle, so 1 enemy strength pt per 50 vehs..
                    }
                    if (es > 30)
                        es = 30;								// Cap enemy strength after 1500 vehicles
                    if (owner != t)
                        es = -es + (rand() % 5) - 2;
                }

                score = fs + es + (rand() % 5);
                if (o.GetObjectivePriority() > 95)
                    score += 50;
                if (o.GetObjectivePriority() > 90)
                    score += 20;
                else
                    score += o.GetObjectivePriority() - 80;

                //		os = (o.GetObjectivePriority()-80)*3;
                //		score = os + fs + es + (rand()%5);

                if (score < 0)
                    score = 0;
                if (score > 100)
                    score = 100;
                // Minimum of 1 priority if it's owned by us.
                if (!score && t == owner)
                    score = 1;

                // KCK: AI's air and ground priorities are identical for now
                if (!(pd.flags & GTMOBJ_SCRIPTED_PRIORITY))
                {
                    pd.ground_priority[owner] = score;
                    pd.air_priority[owner] = score;
                    // KCK: player_priority only used now if it's >= 0
                    //			if (!(pd.flags & GTMOBJ_PLAYER_SET_PRIORITY))
                    //				pd.player_priority[owner] = pd.air_priority[owner];
                }

                if (!GetRoE(owner, t, ROE_GROUND_CAPTURE) && owner != t)
                    pd.ground_priority[owner] = 0;
                if (!GetRoE(owner, t, ROE_AIR_ATTACK) && owner != t)
                    pd.air_priority[owner] = 0;

                if (score > topPriority)
                {
                    topPriority = score;
                    priorityObj = o.GetCampID();
                }
                o = ObjectivStatic.GetNextObjective(&poit);
            }
#endif
            throw new NotImplementedException();
        }

        public virtual int Handle(VuFullUpdateEvent evnt)
        { throw new NotImplementedException(); }

        // core functions
        public void Setup()
        {
#if TODO
            for (int i = 0; i < GORD_LAST; i++)
            {
                canidateList[i] = null;
                objList[i] = null;
            }
#if KEV_GDEBUG
ScoreTime = PickTime = ListBuildTime;
#endif
            Assigned = 0;
#endif
            throw new NotImplementedException();
        }


        // support functions

        public void Cleanup()
        { throw new NotImplementedException(); }
        public int GetAddBits(ObjectiveClass o, int to_collect)
        { throw new NotImplementedException(); }
        public int BuildObjectiveLists(int to_collect)
        { throw new NotImplementedException(); }
        public int CollectGroundAssets(int to_collect)
        { throw new NotImplementedException(); }
        public void AddToList(Unit u, int orders)
        { throw new NotImplementedException(); }
        public void AddToLists(Unit u, int to_collect)
        { throw new NotImplementedException(); }
        public int IsValidObjective(int orders, ObjectiveClass o)
        { throw new NotImplementedException(); }

        public int AssignUnit(Unit u, int orders, ObjectiveClass o, int score)
        { throw new NotImplementedException(); }
        public int AssignUnits(int orders, int mode)
        { throw new NotImplementedException(); }
        public int AssignObjective(GndObjDataType curo, int orders, int mode)
        { throw new NotImplementedException(); }

        public int ScoreUnit(UnitScoreNode curu, GndObjDataType curo, int orders, int mode)
        { throw new NotImplementedException(); }
        public int ScoreUnitFast(UnitScoreNode curu, GndObjDataType curo, int orders, int mode)
        { throw new NotImplementedException(); }

        public void FinalizeOrders()
        { throw new NotImplementedException(); }

        // core functions
        public void SendGTMMessage(VU_ID from, short message, short data1, short data2, VU_ID data3)
        { throw new NotImplementedException(); }

        // Private message handling functions (Called by Process())
        public void RequestSupport(VU_ID enemy, int division)
        { throw new NotImplementedException(); }
        public void RequestEngineer(ObjectiveClass o, int division)
        { throw new NotImplementedException(); }
        public void RequestAirDefense(ObjectiveClass o, int division)
        { throw new NotImplementedException(); }

    };
#if TODO
    typedef GroundTaskingManagerClass *GroundTaskingManager;
	typedef GroundTaskingManagerClass *GTM;
#endif

    public static class GtmStatic
    {
        // ==========================================
        // Global functions
        // ==========================================

        public static int MinAdjustLevel(Unit u)
        { throw new NotImplementedException(); }

        public static short EncodePrimaryObjectiveList(byte teammask, byte[] buffer)
        { throw new NotImplementedException(); }

        public static void DecodePrimaryObjectiveList(byte[] datahead, FalconEntity fe)
        { throw new NotImplementedException(); }

        public static void SendPrimaryObjectiveList(byte teammask)
        { throw new NotImplementedException(); }

        public static void SavePrimaryObjectiveList(string scenario)
        { throw new NotImplementedException(); }

        public static int LoadPrimaryObjectiveList(string scenario)
        { throw new NotImplementedException(); }
    }
}

