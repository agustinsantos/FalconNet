using System;

namespace FalconNet.Campaign
{
	
	public struct GroundDoctrineType {
		byte[]			stance[NUM_COUNS];					// Our air stance towards them: allied/friendly/neutral/alert/hostile/war
		byte			loss_ratio;							// Acceptable loss ratio
		byte			loss_score;							// Score loss for each friendly air loss
		};
	
	public class GroundTaskingManagerClass : CampManagerClass
		{
			public short			flags;
			// These don't need to be transmitted
			public GODNode			objList[GORD_LAST];				// Sorted lists of objectives we want to assign to
			public USNode			canidateList[GORD_LAST];		// List of all possible canidate units for each order
			public short			topPriority;					// Highest PO priority (for scaling)
			public short			priorityObj;					// CampID of highest priority objective
		 
			// constructors
			public GroundTaskingManagerClass(ushort type, Team t);
			public GroundTaskingManagerClass(VU_BYTE **stream);
			public GroundTaskingManagerClass(FILE *file);
			// public virtual ~GroundTaskingManagerClass();
			public virtual int SaveSize (void);
			public virtual int Save (VU_BYTE **stream);
			public virtual int Save (FILE *file);
			public virtual int Handle(VuFullUpdateEvent *event);
	
			// Required pure virtuals
			public virtual void DoCalculations();
			public virtual int Task();
	
			// support functions
			public void Setup ();
			public void Cleanup ();
			public int GetAddBits (Objective o, int to_collect);
			public int BuildObjectiveLists (int to_collect);
	   		public int CollectGroundAssets (int to_collect);
			public void AddToList (Unit u, int orders);
			public void AddToLists(Unit u, int to_collect);
			public int IsValidObjective (int orders, Objective o);
	
			public int	AssignUnit (Unit u, int orders, Objective o, int score);
			public int AssignUnits (int orders, int mode);
			public int AssignObjective (GODNode curo, int orders, int mode);
	
			public int ScoreUnit (USNode curu, GODNode curo, int orders, int mode);
			public int ScoreUnitFast (USNode curu, GODNode curo, int orders, int mode);
	
			public void FinalizeOrders();
	
			// core functions
			public void SendGTMMessage (VU_ID from, short message, short data1, short data2, VU_ID data3);
	
			// Private message handling functions (Called by Process())
			public void RequestSupport (VU_ID enemy, int division);
			public void RequestEngineer (Objective o, int division);
			public void RequestAirDefense (Objective o, int division);
	
		};
	
	typedef GroundTaskingManagerClass *GroundTaskingManager;
	typedef GroundTaskingManagerClass *GTM;
	
	// ==========================================
	// Global functions
	// ==========================================
	
	extern int MinAdjustLevel(Unit u);
	
	extern short EncodePrimaryObjectiveList (byte teammask, byte **buffer);
	
	extern void DecodePrimaryObjectiveList (byte *datahead, FalconEntity *fe);
	
	extern void SendPrimaryObjectiveList (byte teammask);
	
	extern void SavePrimaryObjectiveList (char* scenario);
	
	extern int LoadPrimaryObjectiveList (char* scenario);

}

