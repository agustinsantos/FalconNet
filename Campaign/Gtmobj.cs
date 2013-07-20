using System;
using FalconNet.Common;
using FalconNet.VU;
using Objective=FalconNet.Campaign.ObjectiveClass;
using Unit=FalconNet.Campaign.UnitClass;
using FalconNet.FalcLib;

namespace FalconNet.Campaign
{

    // ====================================
    // Primary and secondary objective data
    // ====================================

    public class PrimaryObjectiveData
    {
        public VU_ID objective;						// Id of the objective
        public short[] ground_priority = new short[(int)TeamDataEnum.NUM_TEAMS];		// It's calculated priority (per team)
        public short[] ground_assigned = new short[(int)TeamDataEnum.NUM_TEAMS];		// Combat factors assigned (per team)
        public short[] air_priority = new short[(int)TeamDataEnum.NUM_TEAMS];		// Air tasking manager's assessment of priority
        public short[] player_priority = new short[(int)TeamDataEnum.NUM_TEAMS];		// Player adjusted priorities (or ATM's if no modifications)
        public byte flags;
    }
#if TODO
typedef PrimaryObjectiveData* POData;
#endif

    public class UnitScoreNode
    {

        public Unit unit;
        public int score;
        public int distance;
        public UnitScoreNode next;


        public UnitScoreNode()
		{throw new NotImplementedException();}

        public UnitScoreNode Insert(UnitScoreNode to_insert, int sort_by)
		{throw new NotImplementedException();}
        public UnitScoreNode Remove(UnitScoreNode to_remove)
		{throw new NotImplementedException();}
        public UnitScoreNode Remove(Unit u)
		{throw new NotImplementedException();}
        public UnitScoreNode Purge()
		{throw new NotImplementedException();}
        public UnitScoreNode Sort(int sort_by)
		{throw new NotImplementedException();}
    };

    // TODO typedef UnitScoreNode USNode;

    public class GndObjDataType
    {

        public Objective obj;
        public int priority_score;
        public int unit_options;
        public UnitScoreNode unit_list;
        public GndObjDataType next;


        public GndObjDataType()
		{throw new NotImplementedException();}
        //TODO public ~GndObjDataType ( );

        public GndObjDataType Insert(GndObjDataType to_insert, int sort_by)
		{throw new NotImplementedException();}
        public GndObjDataType Remove(GndObjDataType to_remove)
		{throw new NotImplementedException();}
        public GndObjDataType Remove(Objective o)
		{throw new NotImplementedException();}
        public GndObjDataType Purge()
		{throw new NotImplementedException();}
        public GndObjDataType Sort(int sort_by)
		{throw new NotImplementedException();}

        public void InsertUnit(Unit u, int s, int d)
		{throw new NotImplementedException();}
        public UnitScoreNode RemoveUnit(Unit u)
		{throw new NotImplementedException();}
        public void RemoveUnitFromAll(Unit u)
		{throw new NotImplementedException();}
        public void PurgeUnits()
		{throw new NotImplementedException();}

        //		GndObjDataType FindWorstOption (Unit u);
        //		int FindNewOptions (Unit u);
    }
#if TODO 
typedef GndObjDataType GODNode;
#endif

    public static class GtmobjStatic
    {
        // ====================================
        // Flags
        // ====================================

        public const int GTMOBJ_PLAYER_SET_PRIORITY = 0x01;					// Player has modified priorities for this objective
        public const int GTMOBJ_SCRIPTED_PRIORITY = 0x02;					// Priority set by script

        // ====================================
        // GndObjDataType:
        //
        // Simple sorted list of objective data
        // with scores and unit assignment data
        // ====================================

        public const int GODN_SORT_BY_PRIORITY = 1;
        public const int GODN_SORT_BY_OPTIONS = 2;

        public const int USN_SORT_BY_SCORE = 1;
        public const int USN_SORT_BY_DISTANCE = 2;

        // ==========================================
        // Global functions
        // ==========================================

        public static void CleanupObjList()
		{throw new NotImplementedException();}

        public static void DisposeObjList()
		{throw new NotImplementedException();}

        public static PrimaryObjectiveData GetPOData(Objective po)
		{throw new NotImplementedException();}

        public static void AddPODataEntry(Objective po)
		{throw new NotImplementedException();}

        public static void ResetObjectiveAssignmentScores()
		{throw new NotImplementedException();}

        public static int GetOptions(int score)
		{throw new NotImplementedException();}
    }
}
