using System;
using System.IO;
using FalconNet.Common;

namespace FalconNet.FalcLib
{
// =====================================
// Campaign defines and typedefs
// =====================================
	public struct CampaignTime
	{
		public ulong time;
		public const ulong CampaignSeconds = 1000;
		public const ulong CampaignMinutes = 60000;
		public const ulong CampaignHours = 3600000;
		public const ulong CampaignDay = 86400000;
		public const ulong INFINITE_TIME = 4294967295;		// Max value of CampaignTime

        public CampaignTime()
        {
            time = 0;
        }
        public CampaignTime(ulong t)
        {
            time = t;
        }
        public static CampaignTime operator +(CampaignTime c1, CampaignTime c2)
        {
            return new CampaignTime(c1.time + c2.time);
        }
        public static CampaignTime operator -(CampaignTime c1, CampaignTime c2)
        {
            return new CampaignTime(c1.time - c2.time);
        }
	}
	

/* TODO
#define MONOMODE_OFF		0
#define MONOMODE_TEXT		1
#define MONOMODE_MAP		2

class UnitClass;
typedef UnitClass* Unit;

// =====================================
// Vu shortcuts
// =====================================

typedef FalconPrivateList*			F4PFList;
typedef FalconPrivateOrderedList*	F4POList;
typedef VuListIterator*				F4LIt;
TODO */
	
// =====================================
// Campaign Library Functions
// =====================================

	public static class Camplib
	{
		public const int VEHICLES_PER_UNIT = 16;
		public const int FEATURES_PER_OBJ = 32;
		public const int MAXIMUM_ROLES = 16;
		public const int MAXIMUM_OBJTYPES = 32;
		public const int MAXIMUM_WEAPTYPES = 600;
		public const int MAX_UNIT_CHILDREN = 5;
		public const int MAX_FEAT_DEPEND = 5;

// JB 010709 Increase for larger theaters
/*
#define MAX_NUMBER_OF_OBJECTIVES		4000
#define MAX_NUMBER_OF_UNITS				2000	// Max # of NON volitile units only
#define MAX_NUMBER_OF_VOLITILE_UNITS	8000
#define MAX_CAMP_ENTITIES				(MAX_NUMBER_OF_OBJECTIVES+MAX_NUMBER_OF_UNITS+MAX_NUMBER_OF_VOLITILE_UNITS)
*/
		public const int MAX_NUMBER_OF_OBJECTIVES = 8000;
		public const int MAX_NUMBER_OF_UNITS = 4000;	// Max # of NON volitile units only
		public const int MAX_NUMBER_OF_VOLITILE_UNITS = 16000;
		public const int MAX_CAMP_ENTITIES = (MAX_NUMBER_OF_OBJECTIVES + MAX_NUMBER_OF_UNITS + MAX_NUMBER_OF_VOLITILE_UNITS);

		public static void Camp_Init (int processor)
		{
			throw new NotImplementedException ();
		}

		public static void Camp_Exit ()
		{
			throw new NotImplementedException ();
		}

		public static void Camp_SetPlayerSquadron (Unit squadron)
		{
			throw new NotImplementedException ();
		}

		public static Unit Camp_GetPlayerSquadron ()
		{
			throw new NotImplementedException ();
		}

		public static VuEntity Camp_GetPlayerEntity ()
		{
			throw new NotImplementedException ();
		}

		public static CampaignTime Camp_GetCurrentTime ()
		{
			throw new NotImplementedException ();
		}

		public static void Camp_SetCurrentTime (double newTime)
		{
			throw new NotImplementedException ();
		}

		public static void Camp_FreeMemory ()
		{
			throw new NotImplementedException ();
		}
 
		public static FileStream OpenCampFile (string filename, string ext, FileMode mode)
		{
			throw new NotImplementedException ();
		}
	}

}