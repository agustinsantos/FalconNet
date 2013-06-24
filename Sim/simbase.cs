using System;
using FalconNet.VU;
using FalconNet.Common;
using System.Diagnostics;
using System.IO;

namespace FalconNet.Sim
{

// Flags used to convey special data
//NOTE top 16 bits are used for motion type
	public enum SIMFLAGS
	{
		RADAR_ON           =0x1,
		ECM_ON             =0x2,
		AIR_BRAKES_OUT     =0x4,	// Should really be a position value, not a bit, but for now...
//#define AVAILABLE        0x8,
		OBJ_EXPLODING      =0x10,
		OBJ_DEAD           =0x20,
		OBJ_FIRING_GUN     =0x40,
		ON_GROUND          =0x80,
		HOW_EXPLOSION     =0x100,
		IN_PERSISTANT_LIST =0x200,
		OBJ_DYING          =0x400,
		PILOT_EJECTED      =0x800,
		I_AM_A_TANKER      =0x1000,
		IS_LASED           =0x2000,
		HAS_MISSILES       =0x4000,
//#define AVAILABLE        0x8000
	}
// Local flags
	public enum LOCALFLAGS
	{
		OBJ_AWAKE          =0x01,
		REMOVE_NEXT_FRAME  =0x02,
		NOT_LABELED        =0x04,
		IS_HIDDEN          =0x08
	}
// Status bits for Base Data
	public enum SIMSTATUS
	{
		VIS_TYPE_MASK = 0x07,
		
		STATUS_GEAR_DOWN = 0x1000,
		STATUS_EXT_LIGHTS = 0x2000,
		STATUS_EXT_NAVLIGHTS = 0x4000,
		STATUS_EXT_TAILSTROBE = 0x8000,
		STATUS_EXT_LANDINGLIGHT = 0x10000,
		STATUS_EXT_LIGHTMASK = (STATUS_EXT_LIGHTS |
	    STATUS_EXT_NAVLIGHTS |
	    STATUS_EXT_TAILSTROBE |
	    STATUS_EXT_LANDINGLIGHT),
	}

	public class SimBaseSpecialData
	{

		public SimBaseSpecialData ()
		{throw new NotImplementedException();}
		//TODO public ~SimBaseSpecialData();

		public float			rdrAz, rdrEl, rdrNominalRng;
		public float			rdrAzCenter, rdrElCenter;
		public float			rdrCycleTime;
		public int				flags;
		public int				status;
		public int				country;
		public byte	afterburner_stage;

		// These should really move into SimMover or SimVeh, since they're only relevant there...
		public float			powerOutput;
		public byte	powerOutputNet;
// 2000-11-17 ADDED BY S.G. SO ENGINE HAS HEAT TEMP
		public float			engineHeatOutput;
		public byte	engineHeatOutputNet;
// END OF ADDED SECTION
		public VU_ID			ChaffID, FlareID;
	}

	public enum ThreatEnum
	{
		THREAT_NONE		=0,
		THREAT_SPIKE	=1,
		THREAT_MISSILE	=2,
		THREAT_GUN		=3,
	}

// this class is used only for non-local entities
// for the most part, it will handle any special effects needed to
// run locally
	public class SimBaseNonLocalData
	{
	
		public int flags;
		public const int NONLOCAL_GUNS_FIRING = 0x00000001;
		public float timer1;				// general purpose timer
		public float timer2;				// general purpose timer
		public float timer3;				// general purpose timer
		public float dx;					// use as a vector
		public float dy;
		public float dz;
		public DrawableTrail smokeTrail;	// smoke when guns are firing

		public SimBaseNonLocalData ()
		{throw new NotImplementedException();}
		//TODO public ~SimBaseNonLocalData();
	}

	
	// TODO Clases SimBaseClass and SimulationDriver are in Campaign project

}
