using System;

namespace FalconNet.Campaign
{	
	// Deals with weapon information

// ===================================
// Guidance Types
// ==================================
	[Flags]
	public enum WEAP_GUIDANCE
	{
		WEAP_VISUALONLY	= 0x0,					// Default state unless one of these flags are set
		WEAP_ANTIRADATION = 0x1,
		WEAP_HEATSEEKER	 = 0x2,
		WEAP_RADAR = 0x4,
		WEAP_LASER = 0x8,
		WEAP_TV = 0x10,
		WEAP_REAR_ASPECT = 0x20,				// Really only applies to heatseakers
		WEAP_FRONT_ASPECT = 0x40,				// Really only applies to SAMs	
		WEAP_DUMB_ONLY = 0x1000,				// ONLY load non-guided weapons
		WEAP_GUIDED_MASK = 0x1F
	}

// ===================================
// Flags
// ===================================
	[Flags]
	public enum WEAP_FLAGS 
	{
		WEAP_RECON			=	0x01,				// Used for recon
		WEAP_FUEL			=	0x02,				// Fuel tank
		WEAP_ECM			=	0x04,				// Used for ECM
		WEAP_AREA			=	0x08,				// Can damage multiple vehicles
		WEAP_CLUSTER		=	0x10,				// CBU -- cluster bomb
		WEAP_TRACER			=	0x20,				// Use tracers when drawing weapon fire
		WEAP_ALWAYSRACK		=	0x40,				// this weapon has no rack
		WEAP_LGB_3RD_GEN	=	0x80,				// 3rd generation LGB's
		WEAP_BOMBWARHEAD	=	0x100,				// this is for example a missile with a bomb war head. MissileEnd when ground impact, not when lethalradius reached
		WEAP_NO_TRAIL		=	0x200,				// do not display any weapon trails or engine glow
		WEAP_BOMBDROPSOUND	=	0x400,				// play the bomb drop sound instead of missile launch
		WEAP_BOMBGPS		=	0x800,				// we use this for JDAM "missile" bombs to have them always CanDetectObject and CanSeeObject true
		WEAP_FORCE_ON_ONE	=	0x2000,				// Put all requested weapons on one/two hardpoints
		WEAP_GUN			=	0x4000,				// Used by LoadWeapons only - to specify guns only
		WEAP_ONETENTH		=	0x8000,				// # listed is actually 1/10th the # of shots	
		WEAP_BAI_LOADOUT	=	0x10000				// special loadout for BAI type missions:
													// only Mavericks and dumb bombs, GBU only
													// GBU-12 (wid 68) and GBU-22 (wid 310)
													// this flag is NOT used in the WeaponClassDataType,
													// only for loading specific weapons only
	}

		// ===================================
		// Damage Types
		// ===================================

		public enum  DamageDataType {
			NoDamage,
			PenetrationDam,						// Hardened structures, tanks, ships
			HighExplosiveDam,					// Soft targets, area targets
			HeaveDam,							// Runways
			IncendairyDam,						// Burn baby, burn!
			ProximityDam,						// AA missiles, etc.
			KineticDam,							// Guns, small arms fire
			HydrostaticDam,						// Submarines
			ChemicalDam,
			NuclearDam,					
			OtherDam 
		}

// ===================================
// Functions
// ===================================
	
	public static class CampWeapons
	{
		
		public const int WEAP_INFINITE_MASK	= 0x07;				// Things which meet this mask never run out.

		public const int HARDPOINT_MAX = 16;						// Number of hardpoints in the weapon table

		public static int LoadWeaponTable (string filename)
		{ throw new NotImplementedException(); }
		
		public static int GetWeaponStrength (int w)
		{ throw new NotImplementedException(); }
		
		public static int GetWeaponRange (int w, int mt)
		{ throw new NotImplementedException(); }
			
		public static int GetWeaponFireRate (int w)
		{ throw new NotImplementedException(); }
		
		public static int GetWeaponHitChance (int w, int mt)
		{ throw new NotImplementedException(); }
		
		public static int GetWeaponHitChance (int w, int mt, int range)
		{ throw new NotImplementedException(); }
		
		public static int GetWeaponHitChance (int w, int mt, int range, int wrange)
		{ throw new NotImplementedException(); }
		
		public static int GetWeaponScore (int w, int mt, int range)
		{ throw new NotImplementedException(); }
		
		public static int GetWeaponScore (int w, int mt, int range, int wrange)
		{ throw new NotImplementedException(); }
		
		public static int GetWeaponScore (int w, byte[] dam, int m, int range)
		{ throw new NotImplementedException(); }
		
		public static int GetWeaponScore (int w, byte[] dam, int mt, int range, int wrange)
		{ throw new NotImplementedException(); }
		
		public static int GetWeaponDamageType (int w)
		{ throw new NotImplementedException(); }
		
		public static int GetWeaponDescriptionIndex (int w)
		{ throw new NotImplementedException(); }
		
		public static int GetWeaponIdFromDescriptionIndex (int index)
		{ throw new NotImplementedException(); }
		
		public static int GetWeaponFlags (int w)
		{ throw new NotImplementedException(); }
	}
}

