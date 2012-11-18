using System;
using FalconNet.Common;
using FalconNet.Campaign;
using FalconNet.VU;
using System.IO;
using System.Diagnostics;

namespace FalconNet.FalcLib
{
    // ==================================
    // Class table entry structure
    // ==================================
    public class Falcon4EntityClassType
    {
        public VuEntityType vuClassData;
        public short[] visType = new short[7];
        public short vehicleDataIndex;
        public Data_Types dataType;
        public object dataPtr;
    }

    // ==================================
    // Private data data structures
    // ==================================

    public class UnitClassDataType
    {

        short Index;						// descriptionIndex pointing here
        int[] NumElements = new int[Camplib.VEHICLES_PER_UNIT];
        short[] VehicleType = new short[Camplib.VEHICLES_PER_UNIT];			// Class table description index
        byte[,] VehicleClass = new byte[Camplib.VEHICLES_PER_UNIT, 8];		// 9 byte class description array
        int Flags;						// Unit capibility flags (see VEH_ flags in vehicle.h)
        string Name;					// Unit name 'Infantry', 'Armor'
        MoveType MovementType;
        short MovementSpeed;
        short MaxRange;					// Movement/flight range with full supply
        long Fuel;						// Fuel (internal)
        short Rate;						// Fuel usage- in lbs per minute (cruise speed)
        short PtDataIndex;				// Index into pt header data table
        byte[] Scores = new byte[Camplib.MAXIMUM_ROLES];		// Score for each type of mission or role
        byte Role;						// What sort of mission role is standard
        byte[] HitChance = new byte[(int)MoveType.MOVEMENT_TYPES];	// Unit hit chances (best hitchance)
        byte[] Strength = new byte[(int)MoveType.MOVEMENT_TYPES];	// Unit strengths (full strength only)
        byte[] Range = new byte[(int)MoveType.MOVEMENT_TYPES];		// Firing ranges (maximum range)
        byte[] Detection = new byte[(int)MoveType.MOVEMENT_TYPES];	// Electronic detection ranges at full strength
        byte[] DamageMod = new byte[(int)DamageDataType.OtherDam + 1];		// How much each type will hurt me (% of strength applied)
        byte RadarVehicle;				// ID of the radar vehicle for this unit
        short SpecialIndex;				// Index into yet another table (max stores for squadrons)
        short IconIndex;					// Index to this unit's icon type
    };

    public class FeatureEntry
    {
        short Index;						// Entity class index of feature
        ushort Flags;
        byte[] eClass = new byte[8];					// Entity class array of feature
        byte Value;						// % loss in operational status for destruction
        vector Offset;
        Int16 Facing;
    };

    public class ObjClassDataType
    {
        public short Index;						// descriptionIndex pointing here
        public string Name;
        public short DataRate;					// Sorte Rate and other cool data
        public short DeagDistance;				// Distance to deaggregate at.
        public short PtDataIndex;				// Index into pt header data table
        public byte[] Detection = new byte[(int)MoveType.MOVEMENT_TYPES];	// Detection ranges
        public byte[] DamageMod = new byte[(int)DamageDataType.OtherDam + 1];		// How much each type will hurt me (% of strength applied)
        public short IconIndex;					// Index to this objective's icon type
        public byte Features;					// Number of features in this objective
        public byte RadarFeature;				// ID of the radar feature for this objective
        public short FirstFeature;				// Index of first feature entry
    };

    public class WeaponClassDataType
    {
        short Index;						// descriptionIndex pointing here
        // 2002-02-08 MN changed from short to ushort
        ushort Strength;					// How much damage it'll do.
        DamageDataType DamageType;					// What type of damage it does.
        short Range;						// Range, in km.
        ushort Flags;
        string Name;
        byte[] HitChance = new byte[(int)MoveType.MOVEMENT_TYPES];
        byte FireRate;					// # of shots fired per barrage
        byte Rariety;					// % of full supply which is actually provided
        ushort GuidanceFlags;
        byte Collective;
        short SimweapIndex;				// Index into sim's weapon data tables
        ushort Weight;						// Weight in lbs.
        short DragIndex;
        ushort BlastRadius;				// radius in ft.
        short RadarType;					// Index into RadarDataTable
        short SimDataIdx;					// Index int SimWeaponDataTable
        char MaxAlt;						// Maximum altitude it can hit in thousands of feet
    };

    // 2001-11-05 Added by M.N.

    public struct RocketClassDataType
    {
        short weaponId;					// Weapon ID
        short nweaponId;						// new Weapon ID (if type of munition changes)
        short weaponCount;						// number of rockets to fire
    }

    // 2002-04-20 Added by M.N. public static alised DD priorities

    public struct DirtyDataClassType
    {
        public Dirtyness priority;					// Priorities of DirtyData messages
    }

    // END of added section

    public class FeatureClassDataType
    {
        public short Index;						// descriptionIndex pointing here
        public short RepairTime;					// How long it takes to repair
        public byte Priority;					// Display priority
        public ushort Flags;						// See FEAT_ flags in feature.h
        public string Name;					// 'Control Tower'
        public short HitPoints;					// Damage this thing can take
        public short Height;						// Height of vehicle ramp, if any
        public float Angle;						// Angle of vehicle ramp, if any
        public Radar_types RadarType;					// Index into RadarDataTable
        public byte[] Detection = new byte[(int)MoveType.MOVEMENT_TYPES];	// Electronic detection ranges
        public byte[] DamageMod = new byte[(int)DamageDataType.OtherDam + 1];		// How much each type will hurt me (% of strength applied)
    };

    public class VehicleClassDataType
    {
        short Index;						// descriptionIndex pointing here
        short HitPoints;					// Damage this thing can take
        uint Flags;						// see VEH_ flags in vehicle.h
        string Name;
        string NCTR;
        float RCSfactor;					// log2( 1 + RCS relative to an F16 )
        long MaxWt;						// Max loaded weight in lbs.
        long EmptyWt;					// Empty weight in lbs.	
        long FuelWt;						// Weight of max fuel in lbs.
        short FuelEcon;					// Fuel usage in lbs./min.
        short EngineSound;				// SoundFX sample index of corresponding engine sound
        short HighAlt;					// in hundreds of feet
        short LowAlt;						// in hundreds of feet
        short CruiseAlt;					// in hundreds of feet
        short MaxSpeed;					// Maximum vehicle speed, in kph
        short RadarType;					// Index into RadarDataTable
        short NumberOfPilots;				// # of pilots (for eject)
        ushort RackFlags;					//0x01 means hardpoint 0 needs a rack, 0x02 . hdpt 1, etc
        ushort VisibleFlags;				//0x01 means hardpoint 0 is visible, 0x02 . hdpt 1, etc
        byte CallsignIndex;
        byte CallsignSlots;
        byte[] HitChance = new byte[(int)MoveType.MOVEMENT_TYPES];	// Vehicle hit chances (best hitchance & bonus)
        byte[] Strength = new byte[(int)MoveType.MOVEMENT_TYPES];	// Combat strengths (full strength only) (calculated)
        byte[] Range = new byte[(int)MoveType.MOVEMENT_TYPES];		// Firing ranges (full strength only) (calculated)
        byte[] Detection = new byte[(int)MoveType.MOVEMENT_TYPES];	// Electronic detection ranges
        short[] Weapon = new short[CampWeapons.HARDPOINT_MAX];		// Weapon id of weapons (or weapon list)
        byte[] Weapons = new byte[CampWeapons.HARDPOINT_MAX];		// Number of shots each (fully supplied)
        byte[] DamageMod = new byte[(int)DamageDataType.OtherDam + 1];		// How much each type will hurt me (% of strength applied)
    };

    public class SquadronStoresDataType
    {
        byte[] Stores = new byte[Camplib.MAXIMUM_WEAPTYPES];	// Weapon stores (only has meaning for squadrons)
        byte infiniteAG;					// One AG weapon we've chosen to always have available
        byte infiniteAA;					// One AA weapon we've chosen to always have available
        byte infiniteGun;				// Our main gun weapon, which we will always have available
    };

    public class PtHeaderDataType
    {
        short objID;						// ID of the objective this belongs to
        byte type;						// The type of pt data this contains
        byte count;						// Number of points
        byte[] features = new byte[Camplib.MAX_FEAT_DEPEND];	// Features this list depends on (# in objective's feature list)
        short data;						// Other data (runway heading, for example)
        float sinHeading;
        float cosHeading;
        short first;						// Index of first point
        short texIdx;						// texture to apply to this runway
        char runwayNum;					// -1 if not a runway, indicates which runway this list applies to
        char ltrt;						// put base pt to rt or left
        short nextHeader;					// Index of next header, if any
    };

    public struct PtDataType
    {
        float xOffset, yOffset;			// X and Y offsets of this point (from center of objective tile)
        byte type;						// The type of point this is
        byte flags;
    };

    public class SimWeaponDataType
    {
        int flags;                            // Flags for the SMS
        float cd;                              // Drag coefficient
        float weight;                          // Weight
        float area;                            // sirface area for drag calc
        float xEjection;                       // Body X axis ejection velocity
        float yEjection;                       // Body Y axis ejection velocity
        float zEjection;                       // Body Z axis ejection velocity
        char[] mnemonic = new char[8];                     // SMS Mnemonic
        int weaponClass;                     // SMS Weapon Class
        int domain;                          // SMS Weapon Domain
        int weaponType;                      // SMS Weapon Type
        int dataIdx;                         // Aditional characteristics data file
    } // SimWEaponDataType;

    public class SimACDefType
    {
        int combatClass;                      // What type of combat does it do?
        int airframeIdx;                      // Index into airframe tables
        int signatureIdx;                     // Index into signature tables (IR only for now)
        int[] sensorType = new int[5];                    // Sensor Types
        int[] sensorIdx = new int[5];                     // Index into sensor data tables
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
        public static UnitClassDataType UnitDataTable;
        public static ObjClassDataType ObjDataTable;
        public static FeatureEntry FeatureEntryDataTable;
        public static WeaponClassDataType WeaponDataTable;
        public static FeatureClassDataType FeatureDataTable;
        public static VehicleClassDataType VehicleDataTable;
        public static SquadronStoresDataType SquadronStoresDataTable;
        public static PtHeaderDataType PtHeaderDataTable;
        public static PtDataType PtDataTable;
        public static SimWeaponDataType SimWeaponDataTable;
        public static SimACDefType SimACDefTable;
        public static RocketClassDataType RocketDataTable;		// Added by M.N.
        public static DirtyDataClassType[] DDP;
        public static Falcon4EntityClassType[] Falcon4ClassTable;
        public static short NumObjectiveTypes;
        public static int F4GenericTruckType;
        public static int F4GenericUSTruckType;
        public static RackGroup[] RackGroupTable;
        public static int MaxRackGroups;
        public static RackObject[] RackObjectTable;
        public static int MaxRackObjects;
        // ===============================================
        // Functions
        // ===============================================

        public static int LoadClassTable(string filename)
        {
#if TODO 
            int i;
            string objSet;
            string newstr;

            // dbgMemSetSafetyLevel( MEM_SAFETY_DEBUG );

#if NOTHING // not required JPO?
	HKEY	theKey;
	DWORD	size,type;
	// Get the path data from the registry
	RegOpenKeyEx(HKEY_LOCAL_MACHINE, FALCON_REGISTRY_KEY, 0, KEY_ALL_ACCESS, &theKey);
	
	size = sizeof (FalconObjectDataDir);
	RegQueryValueEx(theKey, "objectdir", 0, &type, (LPBYTE)FalconObjectDataDir, &size);
	RegCloseKey(theKey);
#endif
            objSet = newstr = strchr(FalconObjectDataDir, '\\');
            while (newstr != null)
            {
                objSet = newstr + 1;
                newstr = strchr(objSet, '\\');
            }

            // Check file integrity
            //	FileVerify();

            ClassTableStatic.InitClassTableAndData(filename, objSet);
            ReadClassTable();
#if !MISSILE_TEST_PROG
#if !ACMI
#if !IACONVERT

            if (!LoadUnitData(filename)) throw new ApplicationException("Failed to load unit data");
            if (!LoadFeatureEntryData(filename)) throw new ApplicationException("Failed to load feature entries");
            if (!LoadObjectiveData(filename)) throw new ApplicationException("Failed to load objective data");
            if (!LoadWeaponData(filename)) throw new ApplicationException("Failed to load weapon data");
            if (!LoadFeatureData(filename)) throw new ApplicationException("Failed to load feature data");
            if (!LoadVehicleData(filename)) throw new ApplicationException("Failed to load vehicle data");
            if (!LoadWeaponListData(filename)) throw new ApplicationException("Failed to load weapon list");
            if (!LoadPtHeaderData(filename)) throw new ApplicationException("Failed to load point headers");
            if (!LoadPtData(filename)) throw new ApplicationException("Failed to load point data");
            if (!LoadRadarData(filename)) throw new ApplicationException("Failed to load radar data");
            if (!LoadIRSTData(filename)) throw new ApplicationException("Failed to load IRST data");
            if (!LoadRwrData(filename)) throw new ApplicationException("Failed to load Rwr data");
            if (!LoadVisualData(filename)) throw new ApplicationException("Failed to load Visual data");
            if (!LoadSimWeaponData(filename)) throw new ApplicationException("Failed to load SimWeapon data");
            if (!LoadACDefData(filename)) throw new ApplicationException("Failed to load AC Definition data");
            if (!LoadSquadronStoresData(filename)) throw new ApplicationException("Failed to load Squadron stores data");
            if (!LoadRocketData(filename)) throw new ApplicationException("Failed to load Rocket data");	// added by M.N.
            if (!LoadDirtyData(filename)) throw new ApplicationException("Failed to load Dirty data priorities"); // added by M.N.
            LoadMissionData();
            LoadVisIdMap();
            LoadRackTables();

            F4Assert(Falcon4ClassTable);
            WriteClassTable();
            // Build ptr data
            for (i = 0; i < NumEntities; i++)
            {
                if (Falcon4ClassTable[i].dataPtr != null)
                {
                    if (Falcon4ClassTable[i].dataType == DTYPE_UNIT)
                    {
                        //				if (Falcon4ClassTable[i].vuClassData.classInfo_[VU_DOMAIN] == DOMAIN_AIR && Falcon4ClassTable[i].vuClassData.classInfo_[VU_TYPE] == TYPE_SQUADRON)
                        //					NumSquadTypes++;
                        Debug.Assert((int)Falcon4ClassTable[i].dataPtr < NumUnitEntries);
                        UnitDataTable[(int)Falcon4ClassTable[i].dataPtr].Index = i;
                        Falcon4ClassTable[i].dataPtr = (void*)&UnitDataTable[(int)Falcon4ClassTable[i].dataPtr];
                    }
                    else if (Falcon4ClassTable[i].dataType == DTYPE_OBJECTIVE)
                    {
                        if (Falcon4ClassTable[i].vuClassData.classInfo_[VU_TYPE] >= NumObjectiveTypes)
                            NumObjectiveTypes = Falcon4ClassTable[i].vuClassData.classInfo_[VU_TYPE];
                        Debug.Assert((int)Falcon4ClassTable[i].dataPtr < NumObjectiveEntries);
                        ObjDataTable[(int)Falcon4ClassTable[i].dataPtr].Index = i;
                        Falcon4ClassTable[i].dataPtr = (void*)&ObjDataTable[(int)Falcon4ClassTable[i].dataPtr];
                    }
                    else if (Falcon4ClassTable[i].dataType == DTYPE_WEAPON)
                    {
                        Debug.Assert((int)Falcon4ClassTable[i].dataPtr < NumWeaponTypes);
                        WeaponDataTable[(int)Falcon4ClassTable[i].dataPtr].Index = i;
                        Falcon4ClassTable[i].dataPtr = (void*)&WeaponDataTable[(int)Falcon4ClassTable[i].dataPtr];
                    }
                    else if (Falcon4ClassTable[i].dataType == DTYPE_FEATURE)
                    {
                        Debug.Assert((int)Falcon4ClassTable[i].dataPtr < NumFeatureEntries);
                        FeatureDataTable[(int)Falcon4ClassTable[i].dataPtr].Index = i;
                        Falcon4ClassTable[i].dataPtr = (void*)&FeatureDataTable[(int)Falcon4ClassTable[i].dataPtr];
                    }
                    else if (Falcon4ClassTable[i].dataType == DTYPE_VEHICLE)
                    {
                        Debug.Assert((int)Falcon4ClassTable[i].dataPtr < NumVehicleEntries);
                        VehicleDataTable[(int)Falcon4ClassTable[i].dataPtr].Index = i;
                        Falcon4ClassTable[i].dataPtr = (void*)&VehicleDataTable[(int)Falcon4ClassTable[i].dataPtr];
                    }
                    else
                        Falcon4ClassTable[i].dataPtr = null;
                }
            }
            ReadClassTable();
            // Update some precalculated statistics
            // KCK: I do these precalculations in the classtable builder now.. 
            //	UpdateVehicleCombatStatistics();
            //	UpdateFeatureCombatStatistics();
            //	UpdateUnitCombatStatistics();
            //	UpdateObjectiveCombatStatistics();
            // Set our special indices;
            SFXType = GetClassID(DOMAIN_ABSTRACT, CLASS_SFX, TYPE_ANY, STYPE_ANY, SPTYPE_ANY, VU_ANY, VU_ANY, VU_ANY);
            F4SessionType = GetClassID(DOMAIN_ABSTRACT, CLASS_SESSION, TYPE_ANY, STYPE_ANY, SPTYPE_ANY, VU_ANY, VU_ANY, VU_ANY);
            F4GroupType = GetClassID(DOMAIN_ABSTRACT, CLASS_GROUP, TYPE_ANY, STYPE_ANY, SPTYPE_ANY, VU_ANY, VU_ANY, VU_ANY);
            F4GameType = GetClassID(DOMAIN_ABSTRACT, CLASS_GAME, TYPE_ANY, STYPE_ANY, SPTYPE_ANY, VU_ANY, VU_ANY, VU_ANY);
            F4FlyingEyeType = GetClassID(DOMAIN_ABSTRACT, CLASS_ABSTRACT, TYPE_FLYING_EYE, STYPE_ANY, SPTYPE_ANY, VU_ANY, VU_ANY, VU_ANY);
            F4GenericTruckType = GetClassID(DOMAIN_LAND, CLASS_VEHICLE, TYPE_WHEELED, STYPE_WHEELED_TRANSPORT, SPTYPE_KrAz255B, VU_ANY, VU_ANY, VU_ANY);
            //	KCK: Temporary until the classtable gets rebuilt - then replace with the commented out line
            // F4GenericUSTruckType = GetClassID(DOMAIN_LAND,CLASS_VEHICLE,TYPE_WHEELED,STYPE_WHEELED_TRANSPORT,SPTYPE_HUMMVCARGO,VU_ANY,VU_ANY,VU_ANY);
            F4GenericUSTruckType = 534;
            //	F4GenericCrewType = GetClassID(DOMAIN_LAND,CLASS_VEHICLE,TYPE_FOOT,STYPE_FOOT_SQUAD,SPTYPE_DPRKARTSQD,VU_ANY,VU_ANY,VU_ANY);
            // Set our special rack Ids
            i = GetClassID(DOMAIN_ABSTRACT, CLASS_WEAPON, TYPE_RACK, STYPE_RACK, SPTYPE_SINGLE, VU_ANY, VU_ANY, VU_ANY);
            gRackId_Single_Rack = (short)(((int)Falcon4ClassTable[i].dataPtr - (int)WeaponDataTable) / sizeof(WeaponClassDataType));
            i = GetClassID(DOMAIN_ABSTRACT, CLASS_WEAPON, TYPE_RACK, STYPE_RACK, SPTYPE_TRIPLE, VU_ANY, VU_ANY, VU_ANY);
            gRackId_Triple_Rack = (short)(((int)Falcon4ClassTable[i].dataPtr - (int)WeaponDataTable) / sizeof(WeaponClassDataType));
            i = GetClassID(DOMAIN_ABSTRACT, CLASS_WEAPON, TYPE_RACK, STYPE_RACK, SPTYPE_QUAD, VU_ANY, VU_ANY, VU_ANY);
            gRackId_Quad_Rack = (short)(((int)Falcon4ClassTable[i].dataPtr - (int)WeaponDataTable) / sizeof(WeaponClassDataType));
            i = GetClassID(DOMAIN_ABSTRACT, CLASS_WEAPON, TYPE_RACK, STYPE_RACK, SPTYPE_SIX, VU_ANY, VU_ANY, VU_ANY);
            gRackId_Six_Rack = (short)(((int)Falcon4ClassTable[i].dataPtr - (int)WeaponDataTable) / sizeof(WeaponClassDataType));
            i = GetClassID(DOMAIN_ABSTRACT, CLASS_WEAPON, TYPE_RACK, STYPE_RACK, SPTYPE_2RAIL, VU_ANY, VU_ANY, VU_ANY);
            gRackId_Two_Rack = (short)(((int)Falcon4ClassTable[i].dataPtr - (int)WeaponDataTable) / sizeof(WeaponClassDataType));
            i = GetClassID(DOMAIN_ABSTRACT, CLASS_WEAPON, TYPE_RACK, STYPE_RACK, SPTYPE_SINGLE_AA, VU_ANY, VU_ANY, VU_ANY);
            gRackId_Single_AA_Rack = (short)(((int)Falcon4ClassTable[i].dataPtr - (int)WeaponDataTable) / sizeof(WeaponClassDataType));
            i = GetClassID(DOMAIN_ABSTRACT, CLASS_WEAPON, TYPE_RACK, STYPE_RACK, SPTYPE_MAVRACK, VU_ANY, VU_ANY, VU_ANY);
            gRackId_Mav_Rack = (short)(((int)Falcon4ClassTable[i].dataPtr - (int)WeaponDataTable) / sizeof(WeaponClassDataType));
            // Find our "Rocket Type". That is, the rocket all aircraft rocket pods will fire.
            i = GetClassID(DOMAIN_AIR, CLASS_VEHICLE, TYPE_ROCKET, STYPE_ROCKET, SPTYPE_2_75mm, VU_ANY, VU_ANY, VU_ANY);
            gRocketId = (short)(((int)Falcon4ClassTable[i].dataPtr - (int)WeaponDataTable) / sizeof(WeaponClassDataType));
            // Special hardcoded class data for sessions/groups/games
            WriteClassTable();
            Falcon4ClassTable[F4SessionType].vuClassData.managementDomain_ = VU_GLOBAL_DOMAIN;
            Falcon4ClassTable[F4SessionType].vuClassData.global_ = TRUE;
            Falcon4ClassTable[F4SessionType].vuClassData.persistent_ = TRUE;
            // KCK: High disconnect time for comms debugging
            if (F4SessionAliveTimeout)
            {
                Falcon4ClassTable[F4SessionType].vuClassData.updateTolerance_ = F4SessionAliveTimeout * 1000;
            }
            else
            {
#if DEBUG
                Falcon4ClassTable[F4SessionType].vuClassData.updateTolerance_ = 30000;			// MS before a session times out
#else
		Falcon4ClassTable[F4SessionType].vuClassData.updateTolerance_ = g_nSessionTimeout*1000;			// MS before a session times out
#endif
            }

            if (F4SessionUpdateTime)
            {
                Falcon4ClassTable[F4SessionType].vuClassData.updateRate_ = F4SessionUpdateTime * 1000;
            }
            else
            {
                Falcon4ClassTable[F4SessionType].vuClassData.updateRate_ = g_nSessionUpdateRate * 1000;				// MS before a session update
            }
            Falcon4ClassTable[F4GroupType].vuClassData.updateRate_ = Falcon4ClassTable[F4SessionType].vuClassData.updateRate_;

            Falcon4ClassTable[F4GroupType].vuClassData.managementDomain_ = VU_GLOBAL_DOMAIN;
            Falcon4ClassTable[F4GroupType].vuClassData.global_ = TRUE;
            Falcon4ClassTable[F4GroupType].vuClassData.persistent_ = FALSE;
            Falcon4ClassTable[F4GameType].vuClassData.managementDomain_ = VU_GLOBAL_DOMAIN;
            Falcon4ClassTable[F4GameType].vuClassData.global_ = TRUE;
            Falcon4ClassTable[F4GameType].vuClassData.persistent_ = FALSE;

            Falcon4ClassTable[F4FlyingEyeType].vuClassData.fineUpdateMultiplier_ = 0.2F;
#endif
#endif
#endif
            ReadClassTable();
            return 1;
#endif
            throw new NotImplementedException();
        }


        public static bool UnloadClassTable()
        {
#if TODO 
            UnitDataTable = null;
            ObjDataTable = null;
            WeaponDataTable = null;
            FeatureDataTable = null;
            VehicleDataTable = null;
            WeaponListDataTable = null;
            SquadronStoresDataTable = null;
            Falcon4ClassTable = null;
            PtHeaderDataTable = null;
            PtDataTable = null;
            FeatureEntryDataTable = null;
            RadarDataTable = null;
            IRSTDataTable = null;
            RwrDataTable = null;
            VisualDataTable = null;
            SimWeaponDataTable = null;
            SimACDefTable = null;
            RocketDataTable = null;			// Added by M.N.
            DDP = null;						// Added by M.N.
            RackGroupTable = null;
            RackObjectTable = null;
            MaxRackObjects = 0;
            MaxRackGroups = 0;
            return (true);
#endif
            throw new NotImplementedException();
        }


        public static bool LoadUnitData(string filename)
        {
#if TODO 
            FileStream fp;
            //	int			i,j;
            short entries;

            if ((fp = OpenCampFile(filename, "UCD", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&entries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(UnitClassDataType) * entries + 2)
                return 0;
            UnitDataTable = new UnitClassDataType[entries];
            fread(UnitDataTable, sizeof(UnitClassDataType), entries, fp);
            fclose(fp);
            /*
                // Build vehicle ids
                for (i=0; i<entries; i++)
                    {
                    for (j=0; j<VEHICLES_PER_UNIT; j++)
                        UnitDataTable[i].VehicleType[j] = GetClassID(UnitDataTable[i].VehicleClass[j][0],
                          UnitDataTable[i].VehicleClass[j][1],UnitDataTable[i].VehicleClass[j][2],
                          UnitDataTable[i].VehicleClass[j][3],UnitDataTable[i].VehicleClass[j][4],
                          UnitDataTable[i].VehicleClass[j][5],UnitDataTable[i].VehicleClass[j][6],
                          UnitDataTable[i].VehicleClass[j][7]);
                    }
            */
            NumUnitEntries = entries;
            return 1;
#endif
            throw new NotImplementedException();
        }

        public static bool LoadObjectiveData(string filename)
        {
#if TODO 
            FileStream fp;
            //	int			i,j,fid;
            short entries;
            string fname;

            strcpy(fname, filename);	// M.N. switch between objectives with and without trees
            if (g_bDisplayTrees)
                name += "tree";

            if ((fp = OpenCampFile(fname, "OCD", "rb")) == null)
            {
                if (g_bDisplayTrees && fedtree)	// we've loaded a fedtree previously, so we also need a ocdtree version
                {
                    throw new ApplicationException("Failed to load objective data");
                    return 0;
                }
                if ((fp = OpenCampFile(filename, "OCD", "rb")) == null)	// if we have no "tree" version, just load the standard one
                    return 0;
            }
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&entries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(ObjClassDataType) * entries + 2)
                return 0;
            ObjDataTable = new ObjClassDataType[entries];
            fread(ObjDataTable, sizeof(ObjClassDataType), entries, fp);
            fclose(fp);
            /*
                // Build feature ids
                for (i=0; i<entries; i++)
                    {
                    fid = ObjDataTable[i].FirstFeature;
                    for (j=0; j<ObjDataTable[i].Features; j++)
                        {
                        FeatureEntryDataTable[fid].Index = GetClassID(FeatureEntryDataTable[fid].eClass[0],
                          FeatureEntryDataTable[fid].eClass[1],FeatureEntryDataTable[fid].eClass[2],
                          FeatureEntryDataTable[fid].eClass[3],FeatureEntryDataTable[fid].eClass[4],
                          FeatureEntryDataTable[fid].eClass[5],FeatureEntryDataTable[fid].eClass[6],
                          FeatureEntryDataTable[fid].eClass[7]);
                        fid++;
                        }
                    }
            */
            NumObjectiveEntries = entries;
            return 1;
#endif
            throw new NotImplementedException();
        }

        public static bool LoadWeaponData(string filename)
        {
#if TODO 
            FileStream fp;
            short entries;

            if ((fp = OpenCampFile(filename, "WCD", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&entries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(WeaponClassDataType) * entries + 2)
                return 0;
            WeaponDataTable = new WeaponClassDataType[entries];
            fread(WeaponDataTable, sizeof(WeaponClassDataType), entries, fp);
            fclose(fp);
            NumWeaponTypes = entries;
            return 1;
#endif
            throw new NotImplementedException();
        }

        public static int LoadRocketData(string filename)
        {
#if TODO 
            FileStream fp;
            short entries;

            if ((fp = OpenCampFile(filename, "RKT", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&entries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(RocketClassDataType) * entries + 2)
                return 0;
            RocketDataTable = new RocketClassDataType[entries];
            fread(RocketDataTable, sizeof(RocketClassDataType), entries, fp);
            fclose(fp);
            NumRocketTypes = entries;
            return 1;
#endif
            throw new NotImplementedException();
        }

        public static bool LoadDirtyData(string filename)
        {
#if TODO 
            FileStream fp;
            short entries;

            if ((fp = OpenCampFile(filename, "DDP", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&entries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(DirtyDataClassType) * entries + 2)
                return false;
            DDP = new DirtyDataClassType[entries];
            fread(DDP, sizeof(DirtyDataClassType), entries, fp);
            fclose(fp);
            NumDirtyDataPriorities = entries;
            return true;
#endif
            throw new NotImplementedException();
        }

        public static bool LoadFeatureData(string filename)
        {
#if TODO 
            FileStream fp;
            short entries;

            if ((fp = OpenCampFile(filename, "FCD", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&entries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(FeatureClassDataType) * entries + 2)
                return 0;
            FeatureDataTable = new FeatureClassDataType[entries];
            fread(FeatureDataTable, sizeof(FeatureClassDataType), entries, fp);
            fclose(fp);
            NumFeatureEntries = entries;
            return 1;
#endif
            throw new NotImplementedException();
        }


        public static bool LoadVehicleData(string filename)
        {
#if TODO
            FileStream fp;
            short entries;

            if ((fp = OpenCampFile(filename, "VCD", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&entries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(VehicleClassDataType) * entries + 2)
                return 0;
            VehicleDataTable = new VehicleClassDataType[entries];
            fread(VehicleDataTable, sizeof(VehicleClassDataType), entries, fp);
            fclose(fp);
            NumVehicleEntries = entries;
            return 1;
#endif
            throw new NotImplementedException();
        }

        public static bool LoadWeaponListData(string filename)
        {
#if TODO
            FileStream fp;
            short entries;

            if ((fp = OpenCampFile(filename, "WLD", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&entries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(WeaponListDataType) * entries + 2)
                return 0;
            WeaponListDataTable = new WeaponListDataType[entries];
            fread(WeaponListDataTable, sizeof(WeaponListDataType), entries, fp);
            fclose(fp);
            return 1;
#endif
            throw new NotImplementedException();
        }

        public static bool LoadPtHeaderData(string filename)
        {
#if TODO 
            FileStream fp;

            if ((fp = OpenCampFile(filename, "PHD", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&NumPtHeaders, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(PtHeaderDataType) * NumPtHeaders + 2)
                return 0;
            PtHeaderDataTable = new PtHeaderDataType[NumPtHeaders];
            fread(PtHeaderDataTable, sizeof(PtHeaderDataType), NumPtHeaders, fp);
            fclose(fp);
            PtHeaderDataTable[0].cosHeading = 1.0F;
            return 1;
#endif
            throw new NotImplementedException();
        }

        public static bool LoadPtData(string filename)
        {
#if TODO 
            FileStream fp;

            if ((fp = OpenCampFile(filename, "PD", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&NumPts, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(PtDataType) * NumPts + 2)
                return 0;
            PtDataTable = new PtDataType[NumPts];
            fread(PtDataTable, sizeof(PtDataType), NumPts, fp);
            fclose(fp);
            return 1;
#endif
            throw new NotImplementedException();
        }

        public static bool LoadFeatureEntryData(string filename)
        {
#if TODO 
            FileStream fp;
            string fname;

            fedtree = false;
            strcpy(fname, filename);	// M.N. switch between objectives with and without trees
            if (g_bDisplayTrees)
            {
                strcat(fname, "tree");
                fedtree = true;
            }

            if ((fp = OpenCampFile(fname, "FED", "rb")) == null)
            {
                fedtree = false;
                if ((fp = OpenCampFile(filename, "FED", "rb")) == null)	// if we have no "tree" version, just load the standard one
                    return 0;
            }
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&NumObjFeatEntries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(FeatureEntry) * NumObjFeatEntries + 2)
                return 0;
            FeatureEntryDataTable = new FeatureEntry[NumObjFeatEntries];
            fread(FeatureEntryDataTable, sizeof(FeatureEntry), NumObjFeatEntries, fp);
            fclose(fp);
            return 1;
#endif
            throw new NotImplementedException();
        }

        public static bool LoadRadarData(string filename)
        {
#if TODO 
            FileStream fp;

            if ((fp = OpenCampFile(filename, "RCD", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&NumRadarEntries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(RadarDataType) * NumRadarEntries + 2)
                return 0;
            RadarDataTable = new RadarDataType[NumRadarEntries];
            Debug.Assert(RadarDataTable);
            fread(RadarDataTable, sizeof(RadarDataType), NumRadarEntries, fp);
            fclose(fp);
            return 1;
#endif
            throw new NotImplementedException();
        }

        public static bool LoadIRSTData(string filename)
        {
#if TODO 
            FileStream fp;

            if ((fp = OpenCampFile(filename, "ICD", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&NumIRSTEntries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(IRSTDataType) * NumIRSTEntries + 2)
                return 0;
            IRSTDataTable = new IRSTDataType[NumIRSTEntries];
            Debug.Assert(IRSTDataTable);
            fread(IRSTDataTable, sizeof(IRSTDataType), NumIRSTEntries, fp);
            fclose(fp);
            return 1;
#endif
            throw new NotImplementedException();
        }

        public static bool LoadRwrData(string filename)
        {
#if TODO 
            FileStream fp;

            if ((fp = OpenCampFile(filename, "rwd", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&NumRwrEntries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(RwrDataType) * NumRwrEntries + 2)
                return 0;
            RwrDataTable = new RwrDataType[NumRwrEntries];
            Debug.Assert(RwrDataTable);
            fread(RwrDataTable, sizeof(RwrDataType), NumRwrEntries, fp);
            fclose(fp);
            return 1;
#endif
            throw new NotImplementedException();
        }

        public static bool LoadVisualData(string filename)
        {
#if TODO
            FileStream fp;

            if ((fp = OpenCampFile(filename, "vsd", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&NumVisualEntries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(VisualDataType) * NumVisualEntries + 2)
                return 0;
            VisualDataTable = new VisualDataType[NumVisualEntries];
            Debug.Assert(VisualDataTable);
            fread(VisualDataTable, sizeof(VisualDataType), NumVisualEntries, fp);
            fclose(fp);
            return 1;
#endif
            throw new NotImplementedException();
        }

        public static bool LoadSimWeaponData(string filename)
        {
#if TODO
            FileStream fp;

            if ((fp = OpenCampFile(filename, "SWD", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&NumSimWeaponEntries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(SimWeaponDataType) * NumSimWeaponEntries + 2)
                return 0;
            SimWeaponDataTable = new SimWeaponDataType[NumSimWeaponEntries];
            Debug.Assert(SimWeaponDataTable);
            fread(SimWeaponDataTable, sizeof(SimWeaponDataType), NumSimWeaponEntries, fp);
            fclose(fp);
            return 1;
#endif
            throw new NotImplementedException();
        }

        public static void InitEntityClasses()
        {
            throw new NotImplementedException();
        }

        public static int GetClassID(byte domain, byte eclass, byte type, byte stype, byte sp, byte owner, byte c6, byte c7)
        {
            throw new NotImplementedException();
        }

        public static string GetClassName(int ID)
        {
            throw new NotImplementedException();
        }

        public static DWORD MapVisId(DWORD ID)
        {
            throw new NotImplementedException();
        }

        public static void LoadRackTables()
        {
            throw new NotImplementedException();
        } // JPO

        public static int FindBestRackID(int rackgroup, int count)
        {
            throw new NotImplementedException();
        }

        public static int FindBestRackIDByPlaneAndWeapon(int planerg, int weaponrg, int count)
        {
            throw new NotImplementedException();
        }
        private static bool LoadACDefData(string filename)
        {
#if TODO
            FileStream fp;

            if ((fp = OpenCampFile(filename, "ACD", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&NumACDefEntries, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(SimACDefType) * NumACDefEntries + 2)
                return 0;
            SimACDefTable = new SimACDefType[NumACDefEntries];
            Debug.Assert(SimACDefTable);
            fread(SimACDefTable, sizeof(SimACDefType), NumACDefEntries, fp);
            fclose(fp);
            return 1;
#endif
            throw new NotImplementedException();
        }

        private static bool LoadSquadronStoresData(string filename)
        {
#if TODO
            FileStream fp;

            if ((fp = OpenCampFile(filename, "SSD", "rb")) == null)
                return 0;
            fseek(fp, 0, SEEK_END); // JPO - work out if the file looks the right size.
            uint size = ftell(fp);
            fseek(fp, 0, SEEK_SET);
            if (fread(&NumSquadTypes, sizeof(short), 1, fp) < 1)
                return 0;
            if (size != sizeof(SquadronStoresDataType) * NumSquadTypes + 2)
                return 0;
            SquadronStoresDataTable = new SquadronStoresDataType[NumSquadTypes];
            Debug.Assert(SquadronStoresDataTable);
            fread(SquadronStoresDataTable, sizeof(SquadronStoresDataType), NumSquadTypes, fp);
            fclose(fp);
            return 1;
#endif
            throw new NotImplementedException();
        }

        private static void WriteClassTable()
        {
            /*
            if (!VirtualProtect (Falcon4ClassTable, NumEntities * sizeof (Falcon4EntityClassType), PAGE_READWRITE, null))
            {
                Debug.Assert (!"Cannot Read/Write Protect ClassTable\n");
            }
            */
        }

        private static void ReadClassTable()
        {
            /*
            if (!VirtualProtect (Falcon4ClassTable, NumEntities * sizeof (Falcon4EntityClassType), PAGE_READONLY, null))
            {
                Debug.Assert (!"Cannot ReadOnly Protect ClassTable\n");
            }
            */
        }
    }
}

