using System;

namespace FalconNet.UI
{
	[Flags]
	public  enum RESULT_FLAGS
	{
		AWARD_MEDAL		= 0x001,
		MDL_AFCROSS 	= 0x002,
		MDL_SILVERSTAR	= 0x004,
		MDL_DIST_FLY	= 0x008,
		MDL_AIR_MDL		= 0x010,
		MDL_KOR_CAMP	= 0x020,
		MDL_LONGEVITY	= 0x040,
		COURT_MARTIAL	= 0x080,
		CM_FR_FIRE1		= 0x100,
		CM_FR_FIRE2		= 0x200,
		CM_FR_FIRE3		= 0x400,
		CM_CRASH		= 0x800,
		CM_EJECT		= 0x1000,
		PROMOTION		= 0x2000,			
	};

	[Flags]
	public enum CAMP_MISS_FLAGS
	{
		DESTROYED_PRIMARY	= 0x01,
		LANDED_AIRCRAFT		= 0x02,
		CRASH_UNDAMAGED		= 0x04,
		EJECT_UNDAMAGED		= 0x08,
		FR_HUMAN_KILLED		= 0x10,
		DONT_SCORE_MISSION	= 0x20,		// Incomplete mission, only record the basics
	};

	public struct CAMP_MISS_STRUCT
	{
		internal CAMP_MISS_FLAGS	Flags;
		internal float	FlightHours;

		//for mission complexity
		internal int		WeaponsExpended;
		internal int		ShotsAtPlayer;
		internal int		AircraftInPackage;
		
		//mission score from Kevin
		internal int		Score;
		
		//Air-to-Air
		internal int		Kills;
		internal int		HumanKills;
		internal int		Killed;
		internal int		KilledByHuman;
		internal int		KilledBySelf;

		//Air-to-Ground
		internal int		GroundUnitsKilled;
		internal int		FeaturesDestroyed;
		internal int		NavalUnitsKilled;

		//other
		internal int		FriendlyFireKills;
		internal int		WingmenLost;
	};

	public static class CampaignMission
	{
		public static RESULT_FLAGS MissionResult;
	}
}
