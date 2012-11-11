using System;
using System.IO;
using FalconNet.Common;
using FalconNet.FalcLib;

namespace FalconNet.Campaign
{

    public struct GroundDoctrineType
    {
        byte[] stance = new byte[NUM_COUNS];					// Our air stance towards them: allied/friendly/neutral/alert/hostile/war
        byte loss_ratio;							// Acceptable loss ratio
        byte loss_score;							// Score loss for each friendly air loss
    };

    public class GroundTaskingManagerClass : CampManagerClass
    {
        public short flags;
        // These don't need to be transmitted
        public GndObjDataType objList = new GndObjDataType[GORD_LAST];				// Sorted lists of objectives we want to assign to
        public UnitScoreNode canidateList = new UnitScoreNode[GORD_LAST];		// List of all possible canidate units for each order
        public short topPriority;					// Highest PO priority (for scaling)
        public short priorityObj;					// CampID of highest priority objective

        // constructors
        public GroundTaskingManagerClass(ushort type, Team t);
        public GroundTaskingManagerClass(VU_BYTE** stream);
        public GroundTaskingManagerClass(FileStream* file);
        // public virtual ~GroundTaskingManagerClass();
        public virtual int SaveSize();
        public virtual int Save(VU_BYTE** stream);
        public virtual int Save(FILE* file);
        public virtual int Handle(VuFullUpdateEvent* evnt);

        // Required pure virtuals
        public virtual void DoCalculations();
        public virtual int Task();

        // support functions
        public void Setup();
        public void Cleanup();
        public int GetAddBits(ObjectiveClass o, int to_collect);
        public int BuildObjectiveLists(int to_collect);
        public int CollectGroundAssets(int to_collect);
        public void AddToList(Unit u, int orders);
        public void AddToLists(Unit u, int to_collect);
        public int IsValidObjective(int orders, ObjectiveClass o);

        public int AssignUnit(Unit u, int orders, ObjectiveClass o, int score);
        public int AssignUnits(int orders, int mode);
        public int AssignObjective(GndObjDataType curo, int orders, int mode);

        public int ScoreUnit(UnitScoreNode curu, GndObjDataType curo, int orders, int mode);
        public int ScoreUnitFast(UnitScoreNode curu, GndObjDataType curo, int orders, int mode);

        public void FinalizeOrders();

        // core functions
        public void SendGTMMessage(VU_ID from, short message, short data1, short data2, VU_ID data3);

        // Private message handling functions (Called by Process())
        public void RequestSupport(VU_ID enemy, int division);
        public void RequestEngineer(ObjectiveClass o, int division);
        public void RequestAirDefense(ObjectiveClass o, int division);

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

        public static int MinAdjustLevel(Unit u);

        public static short EncodePrimaryObjectiveList(byte teammask, byte** buffer);

        public static void DecodePrimaryObjectiveList(byte* datahead, FalconEntity* fe);

        public static void SendPrimaryObjectiveList(byte teammask);

        public static void SavePrimaryObjectiveList(string scenario);

        public static int LoadPrimaryObjectiveList(string scenario);
    }
}

