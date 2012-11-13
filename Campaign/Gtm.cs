using System;
using System.IO;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;

namespace FalconNet.Campaign
{

    public class GroundDoctrineType
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
        public GroundTaskingManagerClass(ushort type, Team t)
		{throw new NotImplementedException();}
        public GroundTaskingManagerClass(VU_BYTE[] stream)
		{throw new NotImplementedException();}
        public GroundTaskingManagerClass(FileStream file)
		{throw new NotImplementedException();}
        // public virtual ~GroundTaskingManagerClass();
        public virtual int SaveSize()
		{throw new NotImplementedException();}
        public virtual int Save(VU_BYTE[] stream)
		{throw new NotImplementedException();}
        public virtual int Save(FileStream file)
		{throw new NotImplementedException();}
        public virtual int Handle(VuFullUpdateEvent evnt)
		{throw new NotImplementedException();}

        // Required pure virtuals
        public virtual void DoCalculations()
		{throw new NotImplementedException();}
        public virtual int Task()
		{throw new NotImplementedException();}

        // support functions
        public void Setup()
		{throw new NotImplementedException();}
        public void Cleanup()
		{throw new NotImplementedException();}
        public int GetAddBits(ObjectiveClass o, int to_collect)
		{throw new NotImplementedException();}
        public int BuildObjectiveLists(int to_collect)
		{throw new NotImplementedException();}
        public int CollectGroundAssets(int to_collect)
		{throw new NotImplementedException();}
        public void AddToList(Unit u, int orders)
		{throw new NotImplementedException();}
        public void AddToLists(Unit u, int to_collect)
		{throw new NotImplementedException();}
        public int IsValidObjective(int orders, ObjectiveClass o)
		{throw new NotImplementedException();}

        public int AssignUnit(Unit u, int orders, ObjectiveClass o, int score)
		{throw new NotImplementedException();}
        public int AssignUnits(int orders, int mode)
		{throw new NotImplementedException();}
        public int AssignObjective(GndObjDataType curo, int orders, int mode)
		{throw new NotImplementedException();}

        public int ScoreUnit(UnitScoreNode curu, GndObjDataType curo, int orders, int mode)
		{throw new NotImplementedException();}
        public int ScoreUnitFast(UnitScoreNode curu, GndObjDataType curo, int orders, int mode)
		{throw new NotImplementedException();}

        public void FinalizeOrders()
		{throw new NotImplementedException();}

        // core functions
        public void SendGTMMessage(VU_ID from, short message, short data1, short data2, VU_ID data3)
		{throw new NotImplementedException();}

        // Private message handling functions (Called by Process())
        public void RequestSupport(VU_ID enemy, int division)
		{throw new NotImplementedException();}
        public void RequestEngineer(ObjectiveClass o, int division)
		{throw new NotImplementedException();}
        public void RequestAirDefense(ObjectiveClass o, int division)
		{throw new NotImplementedException();}

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
		{throw new NotImplementedException();}

        public static short EncodePrimaryObjectiveList(byte teammask, byte[] buffer)
		{throw new NotImplementedException();}

        public static void DecodePrimaryObjectiveList(byte[] datahead, FalconEntity fe)
		{throw new NotImplementedException();}

        public static void SendPrimaryObjectiveList(byte teammask)
		{throw new NotImplementedException();}

        public static void SavePrimaryObjectiveList(string scenario)
		{throw new NotImplementedException();}

        public static int LoadPrimaryObjectiveList(string scenario)
		{throw new NotImplementedException();}
    }
}

