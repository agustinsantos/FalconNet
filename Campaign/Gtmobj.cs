using System;
using FalconNet.Common;


namespace FalconNet.Campaign
{

    // ====================================
    // Primary and secondary objective data
    // ====================================

    public struct PrimaryObjectiveData
    {
        public VU_ID objective;						// Id of the objective
        public short[] ground_priority = new short[NUM_TEAMS];		// It's calculated priority (per team)
        public short[] ground_assigned = new short[NUM_TEAMS];		// Combat factors assigned (per team)
        public short[] air_priority = new short[NUM_TEAMS];		// Air tasking manager's assessment of priority
        public short[] player_priority = new short[NUM_TEAMS];		// Player adjusted priorities (or ATM's if no modifications)
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
        public UnitScoreNode* next;


        public UnitScoreNode();

        public UnitScoreNode* Insert(UnitScoreNode* to_insert, int sort_by);
        public UnitScoreNode* Remove(UnitScoreNode* to_remove);
        public UnitScoreNode* Remove(Unit u);
        public UnitScoreNode* Purge();
        public UnitScoreNode* Sort(int sort_by);
    };

    // TODO typedef UnitScoreNode* USNode;

    public class GndObjDataType
    {

        public Objective obj;
        public int priority_score;
        public int unit_options;
        public UnitScoreNode* unit_list;
        public GndObjDataType* next;


        public GndObjDataType();
        //TODO public ~GndObjDataType ( );

        public GndObjDataType* Insert(GndObjDataType* to_insert, int sort_by);
        public GndObjDataType* Remove(GndObjDataType* to_remove);
        public GndObjDataType* Remove(Objective o);
        public GndObjDataType* Purge();
        public GndObjDataType* Sort(int sort_by);

        public void InsertUnit(Unit u, int s, int d);
        public UnitScoreNode* RemoveUnit(Unit u);
        public void RemoveUnitFromAll(Unit u);
        public void PurgeUnits();

        //		GndObjDataType* FindWorstOption (Unit u);
        //		int FindNewOptions (Unit u);
    }
#if TODO 
typedef GndObjDataType* GODNode;
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

        public static void CleanupObjList();

        public static void DisposeObjList();

        public static PrimaryObjectiveData GetPOData(Objective po);

        public static void AddPODataEntry(Objective po);

        public static void ResetObjectiveAssignmentScores();

        public static int GetOptions(int score);
    }
}
