using System;
using FalconNet.Common;
using FalconNet.Campaign;
using FalconNet.VU;

namespace FalconNet.FalcLib
{
	// ==================================
	// Class table entry structure
	// ==================================
	public class Falcon4EntityClassType
	{
		VuEntityType	vuClassData;
		short[]          visType = new short[7];
		short          vehicleDataIndex;
		byte          dataType;
		object          dataPtr;
	} 
	
// ==================================
// Private data data structures
// ==================================

	public class UnitClassDataType
	{
		
		short		Index;						// descriptionIndex pointing here
		int[]			NumElements = new int[Camplib.VEHICLES_PER_UNIT];
		short[]		VehicleType = new short[Camplib.VEHICLES_PER_UNIT];			// Class table description index
		byte[,]		VehicleClass = new byte[Camplib.VEHICLES_PER_UNIT,8];		// 9 byte class description array
		ushort		Flags;						// Unit capibility flags (see VEH_ flags in vehicle.h)
		string		Name;					// Unit name 'Infantry', 'Armor'
		MoveType	MovementType;
		short		MovementSpeed;
		short		MaxRange;					// Movement/flight range with full supply
		long		Fuel;						// Fuel (internal)
		short		Rate;						// Fuel usage- in lbs per minute (cruise speed)
		short		PtDataIndex;				// Index into pt header data table
		byte[]		Scores = new byte[Camplib.MAXIMUM_ROLES];		// Score for each type of mission or role
		byte		Role;						// What sort of mission role is standard
		byte[]		HitChance = new byte[(int) MoveType.MOVEMENT_TYPES];	// Unit hit chances (best hitchance)
		byte[]		Strength = new byte[(int) MoveType.MOVEMENT_TYPES];	// Unit strengths (full strength only)
		byte[]		Range = new byte[(int) MoveType.MOVEMENT_TYPES];		// Firing ranges (maximum range)
		byte[]		Detection = new byte[(int) MoveType.MOVEMENT_TYPES];	// Electronic detection ranges at full strength
		byte[]		DamageMod = new byte[(int)DamageDataType.OtherDam + 1];		// How much each type will hurt me (% of strength applied)
		byte		RadarVehicle;				// ID of the radar vehicle for this unit
		short		SpecialIndex;				// Index into yet another table (max stores for squadrons)
		short		IconIndex;					// Index to this unit's icon type
	};

	public class FeatureEntry
	{
		short		Index;						// Entity class index of feature
		ushort		Flags;
		byte[]		eClass = new byte[8];					// Entity class array of feature
		byte		Value;						// % loss in operational status for destruction
		vector		Offset;
		Int16		Facing;
	};

	public class ObjClassDataType
	{
		public short		Index;						// descriptionIndex pointing here
		public string		Name;
		public short		DataRate;					// Sorte Rate and other cool data
		public short		DeagDistance;				// Distance to deaggregate at.
		public short		PtDataIndex;				// Index into pt header data table
		public byte[]		Detection = new byte[(int) MoveType.MOVEMENT_TYPES];	// Detection ranges
		public byte[]		DamageMod = new byte[(int)DamageDataType.OtherDam + 1];		// How much each type will hurt me (% of strength applied)
		public short		IconIndex;					// Index to this objective's icon type
		public byte		Features;					// Number of features in this objective
		public byte		RadarFeature;				// ID of the radar feature for this objective
		public short		FirstFeature;				// Index of first feature entry
	};

	public class WeaponClassDataType
	{
		short		Index;						// descriptionIndex pointing here
// 2002-02-08 MN changed from short to ushort
		ushort		Strength;					// How much damage it'll do.
		DamType		DamageType;					// What type of damage it does.
		short		Range;						// Range, in km.
		ushort		Flags;
		string		Name;
		byte[]		HitChance = new	byte[(int) MoveType.MOVEMENT_TYPES];
		byte		FireRate;					// # of shots fired per barrage
		byte		Rariety;					// % of full supply which is actually provided
		ushort		GuidanceFlags;
		byte		Collective;
		short		SimweapIndex;				// Index into sim's weapon data tables
		ushort		Weight;						// Weight in lbs.
		short		DragIndex;
		ushort		BlastRadius;				// radius in ft.
		short		RadarType;					// Index into RadarDataTable
		short		SimDataIdx;					// Index int SimWeaponDataTable
		char		MaxAlt;						// Maximum altitude it can hit in thousands of feet
	};

// 2001-11-05 Added by M.N.

	public struct RocketClassDataType
	{
		short		weaponId;					// Weapon ID
		short		nweaponId;						// new Weapon ID (if type of munition changes)
		short		weaponCount;						// number of rockets to fire
	};

// 2002-04-20 Added by M.N. public static alised DD priorities

	public struct DirtyDataClassType
	{
		Dirtyness	priority;					// Priorities of DirtyData messages
	};

// END of added section

	public class FeatureClassDataType
	{
		short		Index;						// descriptionIndex pointing here
		short		RepairTime;					// How long it takes to repair
		byte		Priority;					// Display priority
		ushort		Flags;						// See FEAT_ flags in feature.h
		string		Name;					// 'Control Tower'
		short		HitPoints;					// Damage this thing can take
		short		Height;						// Height of vehicle ramp, if any
		float		Angle;						// Angle of vehicle ramp, if any
		short		RadarType;					// Index into RadarDataTable
		byte[]		Detection = new byte[(int) MoveType.MOVEMENT_TYPES];	// Electronic detection ranges
		byte[]		DamageMod = new byte[(int)DamageDataType.OtherDam + 1];		// How much each type will hurt me (% of strength applied)
	};

	public class VehicleClassDataType
	{
		short		Index;						// descriptionIndex pointing here
		short		HitPoints;					// Damage this thing can take
		uint	Flags;						// see VEH_ flags in vehicle.h
		string		Name;
		string		NCTR;
		float		RCSfactor;					// log2( 1 + RCS relative to an F16 )
		long		MaxWt;						// Max loaded weight in lbs.
		long		EmptyWt;					// Empty weight in lbs.	
		long		FuelWt;						// Weight of max fuel in lbs.
		short		FuelEcon;					// Fuel usage in lbs./min.
		short		EngineSound;				// SoundFX sample index of corresponding engine sound
		short		HighAlt;					// in hundreds of feet
		short		LowAlt;						// in hundreds of feet
		short		CruiseAlt;					// in hundreds of feet
		short		MaxSpeed;					// Maximum vehicle speed, in kph
		short		RadarType;					// Index into RadarDataTable
		short		NumberOfPilots;				// # of pilots (for eject)
		ushort		RackFlags;					//0x01 means hardpoint 0 needs a rack, 0x02 . hdpt 1, etc
		ushort		VisibleFlags;				//0x01 means hardpoint 0 is visible, 0x02 . hdpt 1, etc
		byte		CallsignIndex;
		byte		CallsignSlots;
		byte[]		HitChance = new byte[(int) MoveType.MOVEMENT_TYPES];	// Vehicle hit chances (best hitchance & bonus)
		byte[]		Strength = new byte[(int) MoveType.MOVEMENT_TYPES];	// Combat strengths (full strength only) (calculated)
		byte[]		Range = new byte[(int) MoveType.MOVEMENT_TYPES];		// Firing ranges (full strength only) (calculated)
		byte[]		Detection = new byte[(int) MoveType.MOVEMENT_TYPES];	// Electronic detection ranges
		short[]		Weapon = new short[CampWeapons.HARDPOINT_MAX];		// Weapon id of weapons (or weapon list)
		byte[]		Weapons = new byte[CampWeapons.HARDPOINT_MAX];		// Number of shots each (fully supplied)
		byte[]		DamageMod = new byte[(int)DamageDataType.OtherDam + 1];		// How much each type will hurt me (% of strength applied)
	};

	public class SquadronStoresDataType
	{
		byte[]		Stores = new byte[Camplib.MAXIMUM_WEAPTYPES];	// Weapon stores (only has meaning for squadrons)
		byte		infiniteAG;					// One AG weapon we've chosen to always have available
		byte		infiniteAA;					// One AA weapon we've chosen to always have available
		byte		infiniteGun;				// Our main gun weapon, which we will always have available
	};

	public class PtHeaderDataType
	{
		short		objID;						// ID of the objective this belongs to
		byte		type;						// The type of pt data this contains
		byte		count;						// Number of points
		byte[]		features = new byte[Camplib.MAX_FEAT_DEPEND];	// Features this list depends on (# in objective's feature list)
		short		data;						// Other data (runway heading, for example)
		float		sinHeading;
		float		cosHeading;
		short		first;						// Index of first point
		short		texIdx;						// texture to apply to this runway
		char		runwayNum;					// -1 if not a runway, indicates which runway this list applies to
		char		ltrt;						// put base pt to rt or left
		short		nextHeader;					// Index of next header, if any
	};

	public struct PtDataType
	{
		float		xOffset, yOffset;			// X and Y offsets of this point (from center of objective tile)
		byte		type;						// The type of point this is
		byte		flags;
	};

	public class SimWeaponDataType
	{
		int  flags;                            // Flags for the SMS
		float cd;                              // Drag coefficient
		float weight;                          // Weight
		float area;                            // sirface area for drag calc
		float xEjection;                       // Body X axis ejection velocity
		float yEjection;                       // Body Y axis ejection velocity
		float zEjection;                       // Body Z axis ejection velocity
		char[]  mnemonic = new char[8];                     // SMS Mnemonic
		int   weaponClass;                     // SMS Weapon Class
		int   domain;                          // SMS Weapon Domain
		int   weaponType;                      // SMS Weapon Type
		int   dataIdx;                         // Aditional characteristics data file
	} // SimWEaponDataType;

	public class SimACDefType
	{
		int  combatClass;                      // What type of combat does it do?
		int  airframeIdx;                      // Index into airframe tables
		int  signatureIdx;                     // Index into signature tables (IR only for now)
		int[]  sensorType = new int[5];                    // Sensor Types
		int[]  sensorIdx = new int[5];                     // Index into sensor data tables
	} //SimACDefType;

	public struct RackGroup
	{
		int nentries;
		int[] entries;
	} //RackGroup;

	public struct RackObject
	{
		int ctind;
		int maxoccupancy;
	} // RackObject;
	
// ===============================================
// public static als
// ===============================================
	public static class EntityDB
	{
		public static  UnitClassDataType		UnitDataTable;
		public static  ObjClassDataType		ObjDataTable;
		public static  FeatureEntry			FeatureEntryDataTable;
		public static  WeaponClassDataType		WeaponDataTable;
		public static  FeatureClassDataType	FeatureDataTable;
		public static  VehicleClassDataType	VehicleDataTable;
		public static  SquadronStoresDataType	SquadronStoresDataTable;
		public static  PtHeaderDataType		PtHeaderDataTable;
		public static  PtDataType				PtDataTable;
		public static  SimWeaponDataType		SimWeaponDataTable;
		public static  SimACDefType           SimACDefTable;
		public static  RocketClassDataType		RocketDataTable;		// Added by M.N.
		public static  DirtyDataClassType		DDP;
		public static  Falcon4EntityClassType	Falcon4ClassTable;
		public static  short NumObjectiveTypes;
		public static  int F4GenericTruckType;
		public static  int F4GenericUSTruckType;
		public static  RackGroup[] RackGroupTable;
		public static  int MaxRackGroups;
		public static  RackObject[] RackObjectTable;
		public static  int MaxRackObjects;
// ===============================================
// Functions
// ===============================================

		public static  int LoadClassTable (string filename)
		{
			throw new NotImplementedException();
		}

		public static  int UnloadClassTable ()
		{
			throw new NotImplementedException();
		}

		public static  int LoadUnitData (string filename)
		{
			throw new NotImplementedException();
		}

		public static  int LoadObjectiveData (string filename)
		{
			throw new NotImplementedException();
		}

		public static  int LoadWeaponData (string filename)
		{
			throw new NotImplementedException();
		}

		public static  int LoadRocketData (string filename)
		{
			throw new NotImplementedException();
		}		// 2001-11-05 Added by M.N.

		public static  int LoadDirtyData (string filename)
		{
			throw new NotImplementedException();
		} // 2002-04-20 Added by M.N.

		public static  int LoadFeatureData (string filename)
		{
			throw new NotImplementedException();
		}

		public static  int LoadVehicleData (string filename)
		{
			throw new NotImplementedException();
		}

		public static  int LoadWeaponListData (string filename)
		{
			throw new NotImplementedException();
		}

		public static  int LoadPtHeaderData (string filename)
		{
			throw new NotImplementedException();
		}

		public static  int LoadPtData (string filename)
		{
			throw new NotImplementedException();
		}

		public static  int LoadFeatureEntryData (string filename)
		{
			throw new NotImplementedException();
		}

		public static  int LoadRadarData (string filename)
		{
			throw new NotImplementedException();
		}

		public static  int LoadIRSTData (string filename)
		{
			throw new NotImplementedException();
		}

		public static  int LoadRwrData (string filename)
		{
			throw new NotImplementedException();
		}

		public static  int LoadVisualData (string filename)
		{
			throw new NotImplementedException();
		}

		public static  int LoadSimWeaponData (string filename)
		{
			throw new NotImplementedException();
		}

		public static  void InitEntityClasses ()
		{
			throw new NotImplementedException();
		}

		public static  int GetClassID (byte domain, byte eclass, byte type, byte stype, byte sp, byte owner, byte c6, byte c7)
		{
			throw new NotImplementedException();
		}

		public static  string GetClassName (int ID)
		{
			throw new NotImplementedException();
		}

		public static  DWORD MapVisId (DWORD ID)
		{
			throw new NotImplementedException();
		}

		public static  void LoadRackTables ()
		{
			throw new NotImplementedException();
		} // JPO
		
		public static  int FindBestRackID (int rackgroup, int count)
		{
			throw new NotImplementedException();
		}

		public static  int FindBestRackIDByPlaneAndWeapon (int planerg, int weaponrg, int count)
		{
			throw new NotImplementedException();
		}
	}
}

