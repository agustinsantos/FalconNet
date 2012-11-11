using System;

namespace FalconNet.Campaign
{
// ==================================
// Air Tasking Manager class
// ==================================
/* TODO 
#define ATM_STEALTH_AVAIL	0x01							// We've got stealth aircraft to use
#define ATM_NEW_PLANES		0x04							// We've got more planes to play with
#define ATM_NEW_REQUESTS	0x08							// We've got one or more new mission requests

#define ATM_MAX_CYCLES		32								// Bit size of a long

// Our scheduling requires a bitwise array of 1 minute time blocks. We've got 8 bits, so full would = 0xFF
// However, if our cycle is < 8 minutes long, we need modify the meaning of 'full'
#define ATM_CYCLE_FULL		0x1F							//	What a full schedule looks like (5 bits)
TODO */
	public class ATMAirbaseClass
	{
	
		public VU_ID			id;
		public byte[]			schedule = new byte[ATM_MAX_CYCLES];
		public byte			usage;
		public ATMAirbaseClass	*next;
		
		public ATMAirbaseClass();
		public ATMAirbaseClass(CampEntity ent);
		public ATMAirbaseClass(VU_BYTE **stream);
		public ATMAirbaseClass(FILE *file);
		//TODO public ~ATMAirbaseClass();
		public int Save(VU_BYTE **stream);
		public int Save(FILE *file);
		public int Size()					{ return sizeof(VU_ID) + ATM_MAX_CYCLES; }
	};

	public class AirTaskingManagerClass : CampManagerClass 
	{
		// Transmittable data
		public short			flags;
		public short			squadrons;							// Number of available friendly squadrons
		public short			averageCAStrength;					// Rolling average CA strength of CA missions
		public short			averageCAMissions;					// Average # of CA missions being flow per hour
		public byte			currentCAMissions;					// # of CA missions planned so far during current cycle
		public byte			sampleCycles;						// # of cycles we've averaged missions over
		// Anything below here doesn't get transmitted
		public int				missionsToFill;						// Amount of missions above to actually task.
		public int				missionsFilled;						// # actually filled to date
		public List			awacsList;							// List of awacs/jstar locations
		public List			tankerList;							// List of tanker track locations
		public List			ecmList;							// List of standoff jammer locations
		public List			requestList;						// List of mission requests yet to be processed.
		public List			delayedList;						// List of mission requests already handled, but not filled
		public F4PFList		squadronList;						// List of this team's squadrons
		public F4PFList		packageList;						// List of all active packages
		public ATMAirbaseClass	*airbaseList;						// List of active airbases
		public byte			supplyBase;
		public byte			cycle;								// which planning block we're in.
		public CampaignTime	scheduleTime;						// Last time we updated our blocks

		// constructors & serial functions
		public AirTaskingManagerClass(ushort type, Team t);
		public AirTaskingManagerClass(VU_BYTE **stream);
		public AirTaskingManagerClass(FILE *file);
		//TODO public virtual ~AirTaskingManagerClass();
		public virtual int SaveSize ();
		public virtual int Save (VU_BYTE **stream);
		public virtual int Save (FILE *file);

		// Required pure virtuals
		public virtual int Task();
		public virtual void DoCalculations();
		public virtual int Handle(VuFullUpdateEvent *evnt);

		// core functions
		public int BuildPackage (Package *pc, MissionRequest mis);
		public int BuildDivert(MissionRequest mis);
		public int BuildSpecificDivert(Flight flight);
		public void ProcessRequest(MissionRequest request);
		public Squadron FindBestAir(MissionRequest mis, GridIndex bx, GridIndex by);
		public Flight FindBestAirFlight(MissionRequest mis);
		public void SendATMMessage(VU_ID from, Team to, short msg, short d1, short d2, void* d3, int flags);
		public int FindTakeoffSlot(VU_ID abid, WayPoint w);
		public void ScheduleAircraft(VU_ID abid, WayPoint wp, int aircraft);
		public void ZapAirbase(VU_ID abid);
		public void ZapSchedule(int rw, ATMAirbaseClass *airbase, int tilblock);
		public ATMAirbaseClass* FindATMAirbase(VU_ID abid);
		public ATMAirbaseClass* AddToAirbaseList (CampEntity airbase);
		public int FindNearestActiveTanker(GridIndex *x, GridIndex *y, CampaignTime *time);
		public int FindNearestActiveJammer(GridIndex *x, GridIndex *y, CampaignTime *time);
	};
#if TODO
typedef AirTaskingManagerClass *AirTaskingManager;
typedef AirTaskingManagerClass *ATM;
#endif
// ==========================================
// Global functions
// ==========================================

public enum RequIntHint { // 2001-10-27 ADDED BY S.G. Tells the RequestIntercept function not to ignore anything
	RI_NORMAL, RI_HELP
};
	
	public static class AtmStatic
	{
		public static void InitATM ( );
		
		public static  void EndATM ( );
		
		public static  int LoadMissionLists (string scenario);
		
		public static  int SaveMissionLists (string  scenario);
		
		public static  int RequestSARMission (FlightClass flight);
		
		public static  void RequestIntercept (FlightClass enemy, int who, RequIntHint hint = RI_NORMAL);
		
		public static  int TargetAllSites (Objective po, int action, int team, CampaignTime startTime);
		
		//extern void TargetAdditionalSites (void);
	}
}

