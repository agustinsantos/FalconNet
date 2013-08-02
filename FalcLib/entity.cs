using System;
using FalconNet.Common;
using FalconNet.Campaign;
using FalconNet.VU;
using System.IO;
using System.Diagnostics;
using DWORD = System.UInt32;
using FalconNet.CampaignBase;
using FalconNet.Common.Maths;
using FalconNet.F4Common;
using FalconNet.Common.Encoding;
using log4net;

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

        public short Index;						// descriptionIndex pointing here
        public int[] NumElements = new int[Camplib.VEHICLES_PER_UNIT];
        public short[] VehicleType = new short[Camplib.VEHICLES_PER_UNIT];			// Class table description index
        public byte[,] VehicleClass = new byte[Camplib.VEHICLES_PER_UNIT, 8];		// 9 byte class description array
        public ushort Flags;						// Unit capibility flags (see VEH_ flags in vehicle.h)
        public string Name;					// Unit name 'Infantry', 'Armor'
        public MoveType MovementType;
        public short MovementSpeed;
        public short MaxRange;					// Movement/flight range with full supply
        public long Fuel;						// Fuel (internal)
        public short Rate;						// Fuel usage- in lbs per minute (cruise speed)
        public short PtDataIndex;				// Index into pt header data table
        public byte[] Scores = new byte[Camplib.MAXIMUM_ROLES];		// Score for each type of mission or role
        public byte Role;						// What sort of mission role is standard
        public byte[] HitChance = new byte[(int)MoveType.MOVEMENT_TYPES];	// Unit hit chances (best hitchance)
        public byte[] Strength = new byte[(int)MoveType.MOVEMENT_TYPES];	// Unit strengths (full strength only)
        public byte[] Range = new byte[(int)MoveType.MOVEMENT_TYPES];		// Firing ranges (maximum range)
        public byte[] Detection = new byte[(int)MoveType.MOVEMENT_TYPES];	// Electronic detection ranges at full strength
        public byte[] DamageMod = new byte[(int)DamageDataType.OtherDam + 1];		// How much each type will hurt me (% of strength applied)
        public byte RadarVehicle;				// ID of the radar vehicle for this unit
        public short SpecialIndex;				// Index into yet another table (max stores for squadrons)
        public short IconIndex;					// Index to this unit's icon type
    }

    public static class UnitClassDataTypeEncodingLE
    {
        public static void Encode(Stream stream, UnitClassDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, UnitClassDataType rst)
        {
            rst.Index = Int16EncodingLE.Decode(stream);
            stream.ReadBytes(2);
            for (int i = 0; i < Camplib.VEHICLE_GROUPS_PER_UNIT; i++)
                rst.NumElements[i] = Int32EncodingLE.Decode(stream);
            for (int i = 0; i < Camplib.VEHICLE_GROUPS_PER_UNIT; i++)
                rst.VehicleType[i] = Int16EncodingLE.Decode(stream);
            for (int i = 0; i < Camplib.VEHICLE_GROUPS_PER_UNIT; i++)
                for (int j = 0; j < 8; j++)
                    rst.VehicleClass[i, j] = (byte)stream.ReadByte();
            rst.Flags = UInt16EncodingLE.Decode(stream);
            rst.Name = StringFixedASCIIEncoding.Decode(stream, 20);
            stream.ReadBytes(2);
            rst.MovementType = (MoveType)Int32EncodingLE.Decode(stream);
            rst.MovementSpeed = Int16EncodingLE.Decode(stream);
            rst.MaxRange = Int16EncodingLE.Decode(stream);
            rst.Fuel = Int32EncodingLE.Decode(stream);
            rst.Rate = Int16EncodingLE.Decode(stream);
            rst.PtDataIndex = Int16EncodingLE.Decode(stream);
            for (int i = 0; i < Camplib.MAXIMUM_ROLES; i++)
                rst.Scores[i] = (byte)stream.ReadByte();
            rst.Role = (byte)stream.ReadByte();
            for (int i = 0; i < (int)MoveType.MOVEMENT_TYPES; i++)
                rst.HitChance[i] = (byte)stream.ReadByte();
            for (int i = 0; i < (int)MoveType.MOVEMENT_TYPES; i++)
                rst.Strength[i] = (byte)stream.ReadByte();
            for (int i = 0; i < (int)MoveType.MOVEMENT_TYPES; i++)
                rst.Range[i] = (byte)stream.ReadByte();
            for (int i = 0; i < (int)MoveType.MOVEMENT_TYPES; i++)
                rst.Detection[i] = (byte)stream.ReadByte();
            for (int i = 0; i < (int)DamageDataType.OtherDam + 1; i++)
                rst.DamageMod[i] = (byte)stream.ReadByte();
            rst.RadarVehicle = (byte)stream.ReadByte();
            stream.ReadByte();
            rst.SpecialIndex = Int16EncodingLE.Decode(stream);
            rst.IconIndex = Int16EncodingLE.Decode(stream);
            stream.ReadBytes(2);
        }

        public static int Size
        {
            get { return 336; }
        }
    }

    public class FeatureEntry
    {
        public short Index;						// Entity class index of feature
        public ushort Flags;
        public byte[] eClass = new byte[8];					// Entity class array of feature
        public byte Value;						// % loss in operational status for destruction
        public vector Offset;
        public Int16 Facing;
    }


    public static class FeatureEntryEncodingLE
    {
        public static void Encode(Stream stream, FeatureEntry val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, FeatureEntry rst)
        {
            rst.Index = Int16EncodingLE.Decode(stream);
            rst.Flags = UInt16EncodingLE.Decode(stream);
            for (int i = 0; i < 8; i++)
                rst.eClass[i] = (byte)stream.ReadByte();
            rst.Value = (byte)stream.ReadByte();
            stream.ReadBytes(3);
            vectorEncodingLE.Decode(stream, ref rst.Offset);
            rst.Facing = Int16EncodingLE.Decode(stream);
            stream.ReadBytes(2);
        }

        public static int Size
        {
            get { return 32; }
        }
    }


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
    }

    public static class ObjClassDataTypeEncodingLE
    {
        public static void Encode(Stream stream, ObjClassDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ObjClassDataType rst)
        {
            rst.Index = Int16EncodingLE.Decode(stream);
            rst.Name = StringFixedASCIIEncoding.Decode(stream, 20);
            rst.DataRate = Int16EncodingLE.Decode(stream);
            rst.DeagDistance = Int16EncodingLE.Decode(stream);
            rst.PtDataIndex = Int16EncodingLE.Decode(stream);
            for (int i = 0; i < (int)MoveType.MOVEMENT_TYPES; i++)
                rst.Detection[i] = (byte)stream.ReadByte();
            for (int i = 0; i < (int)DamageDataType.OtherDam + 1; i++)
                rst.DamageMod[i] = (byte)stream.ReadByte();
            stream.ReadByte();
            rst.IconIndex = Int16EncodingLE.Decode(stream);
            rst.Features = (byte)stream.ReadByte();
            rst.RadarFeature = (byte)stream.ReadByte();
            rst.FirstFeature = Int16EncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return 54; }
        }
    }

    public class WeaponClassDataType
    {
        public short Index;						// descriptionIndex pointing here
        // 2002-02-08 MN changed from short to ushort
        public ushort Strength;					// How much damage it'll do.
        public DamageDataType DamageType;					// What type of damage it does.
        public short Range;						// Range, in km.
        public ushort Flags;
        public string Name;
        public byte[] HitChance = new byte[(int)MoveType.MOVEMENT_TYPES];
        public byte FireRate;					// # of shots fired per barrage
        public byte Rariety;					// % of full supply which is actually provided
        public ushort GuidanceFlags;
        public byte Collective;
        public short SimweapIndex;				// Index into sim's weapon data tables
        public ushort Weight;						// Weight in lbs.
        public short DragIndex;
        public ushort BlastRadius;				// radius in ft.
        public short RadarType;					// Index into RadarDataTable
        public short SimDataIdx;					// Index int SimWeaponDataTable
        public byte MaxAlt;						// Maximum altitude it can hit in thousands of feet
    }

    public static class WeaponClassDataTypeEncodingLE
    {
        public static void Encode(Stream stream, WeaponClassDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, WeaponClassDataType rst)
        {
            rst.Index = Int16EncodingLE.Decode(stream);
            rst.Strength = UInt16EncodingLE.Decode(stream);
            rst.DamageType = (DamageDataType)Int32EncodingLE.Decode(stream);
            rst.Range = Int16EncodingLE.Decode(stream);
            rst.Flags = UInt16EncodingLE.Decode(stream);
            rst.Name = StringFixedASCIIEncoding.Decode(stream, 20);
            for (int i = 0; i < (int)MoveType.MOVEMENT_TYPES; i++)
                rst.HitChance[i] = (byte)stream.ReadByte();
            rst.FireRate = (byte)stream.ReadByte();
            rst.Rariety = (byte)stream.ReadByte();
            rst.GuidanceFlags = UInt16EncodingLE.Decode(stream);
            rst.Collective = (byte)stream.ReadByte();
            stream.ReadByte();
            rst.SimweapIndex = Int16EncodingLE.Decode(stream);
            rst.Weight = UInt16EncodingLE.Decode(stream);
            rst.DragIndex = Int16EncodingLE.Decode(stream);
            rst.BlastRadius = UInt16EncodingLE.Decode(stream);
            rst.RadarType = Int16EncodingLE.Decode(stream);
            rst.SimDataIdx = Int16EncodingLE.Decode(stream);
            rst.MaxAlt = (byte)stream.ReadByte();
            stream.ReadByte();
        }

        public static int Size
        {
            get { return 60; }
        }
    }

    // 2001-11-05 Added by M.N.

    public struct RocketClassDataType
    {
        public short weaponId;					// Weapon ID
        public short nweaponId;						// new Weapon ID (if type of munition changes)
        public short weaponCount;						// number of rockets to fire
    }
    public static class RocketClassDataTypeEncodingLE
    {
        public static void Encode(Stream stream, RocketClassDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ref RocketClassDataType rst)
        {
            rst.weaponId = Int16EncodingLE.Decode(stream);
            rst.nweaponId = Int16EncodingLE.Decode(stream);
            rst.weaponCount = Int16EncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return 3 * 4; }
        }
    }
    // 2002-04-20 Added by M.N. public static alised DD priorities

    public struct DirtyDataClassType
    {
        public Dirtyness priority;					// Priorities of DirtyData messages
    }
    public static class DirtyDataClassTypeEncodingLE
    {
        public static void Encode(Stream stream, DirtyDataClassType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ref DirtyDataClassType rst)
        {
            rst.priority = (Dirtyness)Int32EncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return 1 * 4; }
        }
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
        public byte[] DamageMod = new byte[(int)DamageDataType.OtherDam + 1]; // How much each type will hurt me (% of strength applied)
    }

    public static class FeatureClassDataTypeEncodingLE
    {
        public static void Encode(Stream stream, FeatureClassDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, FeatureClassDataType rst)
        {
            rst.Index = Int16EncodingLE.Decode(stream);
            rst.RepairTime = Int16EncodingLE.Decode(stream);
            rst.Priority = (byte)stream.ReadByte();
            stream.ReadByte();
            rst.Flags = UInt16EncodingLE.Decode(stream);
            rst.Name = StringFixedASCIIEncoding.Decode(stream, 20);
            rst.HitPoints = Int16EncodingLE.Decode(stream);
            rst.Height = Int16EncodingLE.Decode(stream);
            rst.Angle = SingleEncodingLE.Decode(stream);
            rst.RadarType = (Radar_types)Int32EncodingLE.Decode(stream);
            for (int i = 0; i < (int)MoveType.MOVEMENT_TYPES; i++)
                rst.Detection[i] = (byte)stream.ReadByte();
            for (int i = 0; i < (int)DamageDataType.OtherDam + 1; i++)
                rst.DamageMod[i] = (byte)stream.ReadByte();
            stream.ReadByte();
        }

        public static int Size
        {
            get { return 60; }
        }
    }

    public class VehicleClassDataType
    {
        public short Index;						// descriptionIndex pointing here
        public short HitPoints;					// Damage this thing can take
        public uint Flags;						// see VEH_ flags in vehicle.h
        public string Name;
        public string NCTR;
        public float RCSfactor;					// log2( 1 + RCS relative to an F16 )
        public long MaxWt;						// Max loaded weight in lbs.
        public long EmptyWt;					// Empty weight in lbs.	
        public long FuelWt;						// Weight of max fuel in lbs.
        public short FuelEcon;					// Fuel usage in lbs./min.
        public short EngineSound;				// SoundFX sample index of corresponding engine sound
        public short HighAlt;					// in hundreds of feet
        public short LowAlt;						// in hundreds of feet
        public short CruiseAlt;					// in hundreds of feet
        public short MaxSpeed;					// Maximum vehicle speed, in kph
        public short RadarType;					// Index into RadarDataTable
        public short NumberOfPilots;				// # of pilots (for eject)
        public ushort RackFlags;					//0x01 means hardpoint 0 needs a rack, 0x02 . hdpt 1, etc
        public ushort VisibleFlags;				//0x01 means hardpoint 0 is visible, 0x02 . hdpt 1, etc
        public byte CallsignIndex;
        public byte CallsignSlots;
        public byte[] HitChance = new byte[(int)MoveType.MOVEMENT_TYPES];	// Vehicle hit chances (best hitchance & bonus)
        public byte[] Strength = new byte[(int)MoveType.MOVEMENT_TYPES];	// Combat strengths (full strength only) (calculated)
        public byte[] Range = new byte[(int)MoveType.MOVEMENT_TYPES];		// Firing ranges (full strength only) (calculated)
        public byte[] Detection = new byte[(int)MoveType.MOVEMENT_TYPES];	// Electronic detection ranges
        public short[] Weapon = new short[CampWeapons.HARDPOINT_MAX];		// Weapon id of weapons (or weapon list)
        public byte[] Weapons = new byte[CampWeapons.HARDPOINT_MAX];		// Number of shots each (fully supplied)
        public byte[] DamageMod = new byte[(int)DamageDataType.OtherDam + 1];		// How much each type will hurt me (% of strength applied)
    }

    public static class VehicleClassDataTypeEncodingLE
    {
        public static void Encode(Stream stream, VehicleClassDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, VehicleClassDataType rst)
        {
            rst.Index = Int16EncodingLE.Decode(stream);
            rst.HitPoints = Int16EncodingLE.Decode(stream);
            rst.Flags = UInt32EncodingLE.Decode(stream);
            rst.Name = StringFixedASCIIEncoding.Decode(stream, 15);
            rst.NCTR = StringFixedASCIIEncoding.Decode(stream, 5);
            rst.RCSfactor = SingleEncodingLE.Decode(stream);
            rst.MaxWt = Int32EncodingLE.Decode(stream);
            rst.EmptyWt = Int32EncodingLE.Decode(stream);
            rst.FuelWt = Int32EncodingLE.Decode(stream);
            rst.FuelEcon = Int16EncodingLE.Decode(stream);
            rst.EngineSound = Int16EncodingLE.Decode(stream);
            rst.HighAlt = Int16EncodingLE.Decode(stream);
            rst.LowAlt = Int16EncodingLE.Decode(stream);
            rst.CruiseAlt = Int16EncodingLE.Decode(stream);
            rst.MaxSpeed = Int16EncodingLE.Decode(stream);
            rst.RadarType = Int16EncodingLE.Decode(stream);
            rst.NumberOfPilots = Int16EncodingLE.Decode(stream);
            rst.RackFlags = UInt16EncodingLE.Decode(stream);
            rst.VisibleFlags = UInt16EncodingLE.Decode(stream);
            rst.CallsignIndex = (byte)stream.ReadByte();
            rst.CallsignSlots = (byte)stream.ReadByte();
            for (int i = 0; i < (int)MoveType.MOVEMENT_TYPES; i++)
                rst.HitChance[i] = (byte)stream.ReadByte();
            for (int i = 0; i < (int)MoveType.MOVEMENT_TYPES; i++)
                rst.Strength[i] = (byte)stream.ReadByte();
            for (int i = 0; i < (int)MoveType.MOVEMENT_TYPES; i++)
                rst.Range[i] = (byte)stream.ReadByte();
            for (int i = 0; i < (int)MoveType.MOVEMENT_TYPES; i++)
                rst.Detection[i] = (byte)stream.ReadByte();
            for (int i = 0; i < CampWeapons.HARDPOINT_MAX; i++)
                rst.Weapon[i] = Int16EncodingLE.Decode(stream);
            for (int i = 0; i < CampWeapons.HARDPOINT_MAX; i++)
                rst.Weapons[i] = (byte)stream.ReadByte();
            for (int i = 0; i < (int)DamageDataType.OtherDam + 1; i++)
                rst.DamageMod[i] = (byte)stream.ReadByte();
            stream.ReadBytes(3);
        }

        public static int Size
        {
            get { return 160; }
        }
    }

    public class WeaponListDataType
    {
        public const int MAX_WEAPONS_IN_LIST = 64;
        public string Name;
        public short[] WeaponID = new short[MAX_WEAPONS_IN_LIST];
        public byte[] Quantity = new byte[MAX_WEAPONS_IN_LIST];
    }

    public static class WeaponListDataTypeEncodingLE
    {
        public static void Encode(Stream stream, WeaponListDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, WeaponListDataType rst)
        {
            rst.Name = StringFixedASCIIEncoding.Decode(stream, 16);
            for (int i = 0; i < WeaponListDataType.MAX_WEAPONS_IN_LIST; i++)
                rst.WeaponID[i] = Int16EncodingLE.Decode(stream);
            for (int i = 0; i < WeaponListDataType.MAX_WEAPONS_IN_LIST; i++)
                rst.Quantity[i] = (byte)stream.ReadByte();
        }

        public static int Size
        {
            get { return 208; }
        }
    }

    public class SquadronStoresDataType
    {
        public byte[] Stores = new byte[Camplib.MAXIMUM_WEAPTYPES];	// Weapon stores (only has meaning for squadrons)
        public byte infiniteAG;					// One AG weapon we've chosen to always have available
        public byte infiniteAA;					// One AA weapon we've chosen to always have available
        public byte infiniteGun;				// Our main gun weapon, which we will always have available
    }

    public static class SquadronStoresDataTypeEncodingLE
    {
        public static void Encode(Stream stream, SquadronStoresDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, SquadronStoresDataType rst)
        {
            for (int i = 0; i < Camplib.MAXIMUM_WEAPTYPES; i++)
                rst.Stores[i] = (byte)stream.ReadByte();
            rst.infiniteAG = (byte)stream.ReadByte();
            rst.infiniteAA = (byte)stream.ReadByte();
            rst.infiniteGun = (byte)stream.ReadByte();
        }

        public static int Size
        {
            get { return (Camplib.MAXIMUM_WEAPTYPES + 3); }
        }
    }
    public class PtHeaderDataType
    {
        public short objID;						// ID of the objective this belongs to
        public byte type;						// The type of pt data this contains
        public byte count;						// Number of points
        public byte[] features = new byte[Camplib.MAX_FEAT_DEPEND];	// Features this list depends on (# in objective's feature list)
        public short data;						// Other data (runway heading, for example)
        public float sinHeading;
        public float cosHeading;
        public short first;						// Index of first point
        public short texIdx;						// texture to apply to this runway
        public byte runwayNum;		// -1 if not a runway, indicates which runway this list applies to
        public byte ltrt;						// put base pt to rt or left
        public short nextHeader;					// Index of next header, if any
    }
    public static class PtHeaderDataTypeEncodingLE
    {
        public static void Encode(Stream stream, PtHeaderDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, PtHeaderDataType rst)
        {
            rst.objID = Int16EncodingLE.Decode(stream);
            rst.type = (byte)stream.ReadByte();
            rst.count = (byte)stream.ReadByte();
            for (int i = 0; i < Camplib.MAX_FEAT_DEPEND; i++)
                rst.features[i] = (byte)stream.ReadByte();
            stream.ReadByte();
            rst.data = Int16EncodingLE.Decode(stream);
            rst.sinHeading = SingleEncodingLE.Decode(stream);
            rst.cosHeading = SingleEncodingLE.Decode(stream);
            rst.first = Int16EncodingLE.Decode(stream);
            rst.texIdx = Int16EncodingLE.Decode(stream);
            rst.runwayNum = (byte)stream.ReadByte();
            rst.ltrt = (byte)stream.ReadByte();
            rst.nextHeader = Int16EncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return 28; }
        }
    }
    public struct PtDataType
    {
        public float xOffset, yOffset;			// X and Y offsets of this point (from center of objective tile)
        public byte type;						// The type of point this is
        public byte flags;
    }
    public static class PtDataTypeEncodingLE
    {
        public static void Encode(Stream stream, PtDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ref PtDataType rst)
        {
            rst.xOffset = SingleEncodingLE.Decode(stream);
            rst.yOffset = SingleEncodingLE.Decode(stream);
            rst.type = (byte)stream.ReadByte();
            rst.flags = (byte)stream.ReadByte();
            stream.ReadBytes(2);
        }

        public static int Size
        {
            get { return 12; }
        }
    }

    public class RadarDataType
    {
        public const int HIGH_ALT_LETHALITY = 0;
        public const int LOW_ALT_LETHALITY = 1;
        public const int NUM_ALT_LETHALITY = 2;
        public int RWRsound; // Which sound plays in the RWR
        public short RWRsymbol; // Which symbol shows up on the RWR
        public short RDRDataInd; // Index into radar data files
        public float[] Lethality = new float[NUM_ALT_LETHALITY]; // Lethality against low altitude targets
        public float NominalRange; // Detection range against F16 sized target
        public float BeamHalfAngle; // radians (degrees in file)
        public float ScanHalfAngle; // radians (degrees in file)
        public float SweepRate; // radians/sec (degrees in file)
        public uint CoastTime; // ms to hold lock on faded target (seconds in file)
        public float LookDownPenalty; // degrades SN ratio
        public float JammingPenalty; // degrades SN ratio
        public float NotchPenalty; // degrades SN ratio
        public float NotchSpeed; // ft/sec (kts in file)
        public float ChaffChance; // Base probability a bundle of chaff will decoy this radar
        public short flag; // 0x01 = NCTR capable
    }
    public static class RadarDataTypeEncodingLE
    {
        public static void Encode(Stream stream, RadarDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, RadarDataType rst)
        {
            rst.RWRsound = Int32EncodingLE.Decode(stream);
            rst.RWRsymbol = Int16EncodingLE.Decode(stream);
            rst.RDRDataInd = Int16EncodingLE.Decode(stream);
            for (int i = 0; i < RadarDataType.NUM_ALT_LETHALITY; i++)
                rst.Lethality[i] = SingleEncodingLE.Decode(stream);
            rst.NominalRange = SingleEncodingLE.Decode(stream);
            rst.BeamHalfAngle = SingleEncodingLE.Decode(stream);
            rst.ScanHalfAngle = SingleEncodingLE.Decode(stream);
            rst.SweepRate = SingleEncodingLE.Decode(stream);
            rst.CoastTime = UInt32EncodingLE.Decode(stream);
            rst.LookDownPenalty = SingleEncodingLE.Decode(stream);
            rst.JammingPenalty = SingleEncodingLE.Decode(stream);
            rst.NotchPenalty = SingleEncodingLE.Decode(stream);
            rst.NotchSpeed = SingleEncodingLE.Decode(stream);
            rst.ChaffChance = SingleEncodingLE.Decode(stream);
            rst.flag = Int16EncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return 12; }
        }
    }

    public class IRSTDataType
    {
        public float NominalRange; // Detection range against F16 sized target
        public float FOVHalfAngle; // radians (degrees in file)
        public float GimbalLimitHalfAngle; // radians (degrees in file)
        public float GroundFactor; // Range multiplier applied for ground targets
        public float FlareChance; // Base probability a flare will work
    }
    public static class IRSTDataTypeEncodingLE
    {
        public static void Encode(Stream stream, IRSTDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, IRSTDataType rst)
        {
            rst.NominalRange = SingleEncodingLE.Decode(stream);
            rst.FOVHalfAngle = SingleEncodingLE.Decode(stream);
            rst.GimbalLimitHalfAngle = SingleEncodingLE.Decode(stream);
            rst.GroundFactor = SingleEncodingLE.Decode(stream);
            rst.FlareChance = SingleEncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return 20; }
        }
    }


    public class RwrDataType
    {
        public float nominalRange;              // Nominal detection range
        public float top;                       // Scan volume top (Degrees in text file)
        public float bottom;                    // Scan volume bottom (Degrees in text file)
        public float left;                      // Scan volume left (Degrees in text file)
        public float right;                     // Scan volume right (Degrees in text file)
        public short flag;    /* 0x01 = can get exact heading
   0x02 = can only get vague direction
   0x04 = can detect exact radar type
   0x08 = can only detect group of radar types */
    }

    public static class RwrDataTypeEncodingLE
    {
        public static void Encode(Stream stream, RwrDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, RwrDataType rst)
        {
            rst.nominalRange = SingleEncodingLE.Decode(stream);
            rst.top = SingleEncodingLE.Decode(stream);
            rst.bottom = SingleEncodingLE.Decode(stream);
            rst.left = SingleEncodingLE.Decode(stream);
            rst.right = SingleEncodingLE.Decode(stream);
            rst.flag = Int16EncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return 20; }
        }
    }

    public class VisualDataType
    {
        public float nominalRange;              // Nominal detection range
        public float top;                       // Scan volume top (Degrees in text file)
        public float bottom;                    // Scan volume bottom (Degrees in text file)
        public float left;                      // Scan volume left (Degrees in text file)
        public float right;                     // Scan volume right (Degrees in text file)
    }

    public static class VisualDataTypeEncodingLE
    {
        public static void Encode(Stream stream, VisualDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, VisualDataType rst)
        {
            rst.nominalRange = SingleEncodingLE.Decode(stream);
            rst.top = SingleEncodingLE.Decode(stream);
            rst.bottom = SingleEncodingLE.Decode(stream);
            rst.left = SingleEncodingLE.Decode(stream);
            rst.right = SingleEncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return 20; }
        }
    }
    public class SimWeaponDataType
    {
        public int flags;                            // Flags for the SMS
        public float cd;                              // Drag coefficient
        public float weight;                          // Weight
        public float area;                            // sirface area for drag calc
        public float xEjection;                       // Body X axis ejection velocity
        public float yEjection;                       // Body Y axis ejection velocity
        public float zEjection;                       // Body Z axis ejection velocity
        public string mnemonic;       // SMS Mnemonic
        public int weaponClass;                     // SMS Weapon Class
        public int domain;                          // SMS Weapon Domain
        public int weaponType;                      // SMS Weapon Type
        public int dataIdx;                         // Aditional characteristics data file
    } // SimWEaponDataType;
    public static class SimWeaponDataTypeEncodingLE
    {
        public static void Encode(Stream stream, SimWeaponDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, SimWeaponDataType rst)
        {
            rst.flags = Int32EncodingLE.Decode(stream);
            rst.cd = SingleEncodingLE.Decode(stream);
            rst.weight = SingleEncodingLE.Decode(stream);
            rst.area = SingleEncodingLE.Decode(stream);
            rst.xEjection = SingleEncodingLE.Decode(stream);
            rst.yEjection = SingleEncodingLE.Decode(stream);
            rst.zEjection = SingleEncodingLE.Decode(stream);
            rst.mnemonic = StringFixedASCIIEncoding.Decode(stream, 8);
            rst.weaponClass = Int32EncodingLE.Decode(stream);
            rst.domain = Int32EncodingLE.Decode(stream);
            rst.weaponType = Int32EncodingLE.Decode(stream);
            rst.dataIdx = Int32EncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return 20; }
        }
    }

    public class SimACDefType
    {
        public int combatClass;                      // What type of combat does it do?
        public int airframeIdx;                      // Index into airframe tables
        public int signatureIdx;                     // Index into signature tables (IR only for now)
        public int[] sensorType = new int[5];                    // Sensor Types
        public int[] sensorIdx = new int[5];                     // Index into sensor data tables
    } //SimACDefType;
    public static class SimACDefTypeEncodingLE
    {
        public static void Encode(Stream stream, SimACDefType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, SimACDefType rst)
        {
            rst.combatClass = Int32EncodingLE.Decode(stream);
            rst.airframeIdx = Int32EncodingLE.Decode(stream);
            rst.signatureIdx = Int32EncodingLE.Decode(stream);
            for (int i = 0; i < 5; i++)
                rst.sensorType[i] = Int32EncodingLE.Decode(stream);
            for (int i = 0; i < 5; i++)
                rst.sensorIdx[i] = Int32EncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return 12 * 4; }
        }
    }

    public struct RackGroup
    {
        public int nentries;
        public int[] entries;
    } //RackGroup;

    public struct RackObject
    {
        public int ctind;
        public int maxoccupancy;
    } // RackObject;

    // ===============================================
    // public static als
    // ===============================================
    public static class EntityDB
    {
        public static UnitClassDataType[] UnitDataTable;
        public static ObjClassDataType[] ObjDataTable;
        public static FeatureEntry[] FeatureEntryDataTable;
        public static WeaponClassDataType[] WeaponDataTable;
        public static FeatureClassDataType[] FeatureDataTable;
        public static VehicleClassDataType[] VehicleDataTable;
        public static WeaponListDataType[] WeaponListDataTable;
        public static SquadronStoresDataType[] SquadronStoresDataTable;
        public static PtHeaderDataType[] PtHeaderDataTable;
        public static PtDataType[] PtDataTable;
        public static SimWeaponDataType[] SimWeaponDataTable;
        public static SimACDefType[] SimACDefTable;
        public static RocketClassDataType[] RocketDataTable;		// Added by M.N.
        public static DirtyDataClassType[] DDP;
        public static Falcon4EntityClassType[] Falcon4ClassTable;
        public static RadarDataType[] RadarDataTable;
        public static IRSTDataType[] IRSTDataTable;
        public static RwrDataType[] RwrDataTable;
        public static VisualDataType[] VisualDataTable;

        public static RackGroup[] RackGroupTable;
        public static int MaxRackGroups;
        public static RackObject[] RackObjectTable;
        public static int MaxRackObjects;


        public static short NumUnitEntries;
        public static short NumObjectiveEntries;
        public static short NumObjFeatEntries;
        public static short NumVehicleEntries;
        public static short NumFeatureEntries;
        public static short NumSquadTypes;
        public static short NumObjectiveTypes;
        public static short NumWeaponTypes;
        public static short NumPtHeaders;
        public static short NumPts;
        public static short NumSimWeaponEntries;
        public static short NumACDefEntries;
        public static short NumRocketTypes; // 2001-11-05 Added by M.N.
        public static short NumDirtyDataPriorities;   // 2002-04-20 Added by M.N.
        public static short NumRadarEntries;
        public static short NumIRSTEntries;
        public static short NumRwrEntries;
        public static short NumVisualEntries;

        public static bool fedtree;

        // A description index for our generic entity classes
        public static int SFXType;
        public static int F4SessionType;
        public static int F4GroupType;
        public static int F4GameType;
        public static int F4FlyingEyeType;
        public static int F4GenericTruckType;
        public static int F4GenericUSTruckType;

        // Rack id's for each rack type
        public static short gRackId_Single_Rack;
        public static short gRackId_Triple_Rack;
        public static short gRackId_Quad_Rack;
        public static short gRackId_Six_Rack;
        public static short gRackId_Two_Rack;
        public static short gRackId_Single_AA_Rack;
        public static short gRackId_Mav_Rack;

        // Id of our generic rocket.
        public static short gRocketId;

        // ===============================================
        // Functions
        // ===============================================

        public static int LoadClassTable(string filename)
        {
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
#if TODO
            objSet = newstr = strchr(FalconObjectDataDir, '\\');
            while (newstr != null)
            {
                objSet = newstr + 1;
                newstr = strchr(objSet, '\\');
            }
#endif
            string[] entries = F4File.FalconObjectDataDir.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            string objSet = entries[entries.Length - 1];
            // Check file integrity
            //	FileVerify();

            ClassTableStatic.InitClassTableAndData(filename, objSet);
            ReadClassTable();

#if !MISSILE_TEST_PROG
#if !ACMI
#if !IACONVERT

            if (!LoadUnitData(filename))
                throw new ApplicationException("Failed to load unit data");
            if (!LoadFeatureEntryData(filename))
                throw new ApplicationException("Failed to load feature entries");
            if (!LoadObjectiveData(filename))
                throw new ApplicationException("Failed to load objective data");
            if (!LoadWeaponData(filename))
                throw new ApplicationException("Failed to load weapon data");
            if (!LoadFeatureData(filename))
                throw new ApplicationException("Failed to load feature data");
            if (!LoadVehicleData(filename))
                throw new ApplicationException("Failed to load vehicle data");
            if (!LoadWeaponListData(filename))
                throw new ApplicationException("Failed to load weapon list");
            if (!LoadPtHeaderData(filename))
                throw new ApplicationException("Failed to load point headers");
            if (!LoadPtData(filename))
                throw new ApplicationException("Failed to load point data");
            if (!LoadRadarData(filename))
                throw new ApplicationException("Failed to load radar data");
            if (!LoadIRSTData(filename))
                throw new ApplicationException("Failed to load IRST data");
            if (!LoadRwrData(filename))
                throw new ApplicationException("Failed to load Rwr data");
            if (!LoadVisualData(filename))
                throw new ApplicationException("Failed to load Visual data");
            if (!LoadSimWeaponData(filename))
                throw new ApplicationException("Failed to load SimWeapon data");
            if (!LoadACDefData(filename))
                throw new ApplicationException("Failed to load AC Definition data");
            if (!LoadSquadronStoresData(filename))
                throw new ApplicationException("Failed to load Squadron stores data");
            if (!LoadRocketData(filename))
                throw new ApplicationException("Failed to load Rocket data");	// added by M.N.
            if (!LoadDirtyData(filename))
                throw new ApplicationException("Failed to load Dirty data priorities"); // added by M.N.
#if TODO       
            LoadMissionData();
            
            LoadVisIdMap();
            LoadRackTables();
            RDLoadRackData();

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
            Falcon4ClassTable[F4GroupType].vuClassData.persistent_ = false;
            Falcon4ClassTable[F4GameType].vuClassData.managementDomain_ = VU_GLOBAL_DOMAIN;
            Falcon4ClassTable[F4GameType].vuClassData.global_ = TRUE;
            Falcon4ClassTable[F4GameType].vuClassData.persistent_ = false;

            Falcon4ClassTable[F4FlyingEyeType].vuClassData.fineUpdateMultiplier_ = 0.2F;
#endif
#endif
#endif
#endif
            ReadClassTable();
            return 1;

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
            short entries;

            using (FileStream fp = F4File.OpenCampFile(filename, "UCD", FileAccess.Read))
            {
                entries = Int16EncodingLE.Decode(fp);
                UnitDataTable = new UnitClassDataType[entries];
                for (int i = 0; i < entries; i++)
                {
                    UnitDataTable[i] = new UnitClassDataType();
                    UnitClassDataTypeEncodingLE.Decode(fp, UnitDataTable[i]);
                }

                //fread(UnitDataTable, sizeof(UnitClassDataType), entries, fp);
                fp.Close();
            }
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
            return true;
        }

        public static bool LoadObjectiveData(string filename)
        {
            using (FileStream fp = F4File.OpenCampFile(filename, "OCD", FileAccess.Read))
            {
                NumObjectiveEntries = Int16EncodingLE.Decode(fp);
                ObjDataTable = new ObjClassDataType[NumObjectiveEntries];
                for (int i = 0; i < NumObjectiveEntries; i++)
                {
                    ObjDataTable[i] = new ObjClassDataType();
                    ObjClassDataTypeEncodingLE.Decode(fp, ObjDataTable[i]);
                }

                fp.Close();
            }
            return true;
        }

        public static bool LoadWeaponData(string filename)
        {
            using (FileStream fp = F4File.OpenCampFile(filename, "WCD", FileAccess.Read))
            {
                NumWeaponTypes = Int16EncodingLE.Decode(fp);
                WeaponDataTable = new WeaponClassDataType[NumWeaponTypes];
                for (int i = 0; i < NumWeaponTypes; i++)
                {
                    WeaponDataTable[i] = new WeaponClassDataType();
                    WeaponClassDataTypeEncodingLE.Decode(fp, WeaponDataTable[i]);
                }

                fp.Close();
            }
            return true;
        }

        public static bool LoadRocketData(string filename)
        {
            using (FileStream fp = F4File.OpenCampFile(filename, "RKT", FileAccess.Read))
            {
                NumRocketTypes = Int16EncodingLE.Decode(fp);
                RocketDataTable = new RocketClassDataType[NumRocketTypes];
                for (int i = 0; i < NumRocketTypes; i++)
                {
                    RocketDataTable[i] = new RocketClassDataType();
                    RocketClassDataTypeEncodingLE.Decode(fp, ref RocketDataTable[i]);
                }

                fp.Close();
            }
            return true;

        }

        public static bool LoadDirtyData(string filename)
        {
            using (FileStream fp = F4File.OpenCampFile(filename, "DDP", FileAccess.Read))
            {
                NumDirtyDataPriorities = Int16EncodingLE.Decode(fp);
                DDP = new DirtyDataClassType[NumDirtyDataPriorities];
                for (int i = 0; i < NumDirtyDataPriorities; i++)
                {
                    DDP[i] = new DirtyDataClassType();
                    DirtyDataClassTypeEncodingLE.Decode(fp, ref DDP[i]);
                }

                fp.Close();
            }
            return true;
        }

        public static bool LoadFeatureData(string filename)
        {
            short entries;
            string fname = filename + "tree";
            if (F4Config.g_bDisplayTrees)
            {
                string name = F4File.F4FindFile(fname);
                if (File.Exists(name))
                {
                    fedtree = true;
                }
                else
                {
                    fedtree = false;
                    fname = filename;
                }
            }
            using (FileStream fp = F4File.OpenCampFile(fname, "FCD", FileAccess.Read))
            {
                entries = Int16EncodingLE.Decode(fp);
                FeatureDataTable = new FeatureClassDataType[entries];
                for (int i = 0; i < entries; i++)
                {
                    FeatureDataTable[i] = new FeatureClassDataType();
                    FeatureClassDataTypeEncodingLE.Decode(fp, FeatureDataTable[i]);
                }
                fp.Close();
            }
            NumFeatureEntries = entries;
            return true;
        }


        public static bool LoadVehicleData(string filename)
        {

            short entries;

            using (FileStream fp = F4File.OpenCampFile(filename, "VCD", FileAccess.Read))
            {
                entries = Int16EncodingLE.Decode(fp);
                VehicleDataTable = new VehicleClassDataType[entries];
                for (int i = 0; i < entries; i++)
                {
                    VehicleDataTable[i] = new VehicleClassDataType();
                    VehicleClassDataTypeEncodingLE.Decode(fp, VehicleDataTable[i]);
                }

                fp.Close();
            }
            NumVehicleEntries = entries;
            return true;
        }

        public static bool LoadWeaponListData(string filename)
        {
            short entries;

            using (FileStream fp = F4File.OpenCampFile(filename, "WLD", FileAccess.Read))
            {
                entries = Int16EncodingLE.Decode(fp);
                WeaponListDataTable = new WeaponListDataType[entries];
                for (int i = 0; i < entries; i++)
                {
                    WeaponListDataTable[i] = new WeaponListDataType();
                    WeaponListDataTypeEncodingLE.Decode(fp, WeaponListDataTable[i]);
                }

                fp.Close();
            }
            return true;
        }

        public static bool LoadPtHeaderData(string filename)
        {
            using (FileStream fp = F4File.OpenCampFile(filename, "PHD", FileAccess.Read))
            {
                NumPtHeaders = Int16EncodingLE.Decode(fp);
                PtHeaderDataTable = new PtHeaderDataType[NumPtHeaders];
                for (int i = 0; i < NumPtHeaders; i++)
                {
                    PtHeaderDataTable[i] = new PtHeaderDataType();
                    PtHeaderDataTypeEncodingLE.Decode(fp, PtHeaderDataTable[i]);
                }

                fp.Close();
            }
            PtHeaderDataTable[0].cosHeading = 1.0F;

            if (F4Config.g_bCheckFeatureIndex)
            {
                for (int l = 0; l < NumPtHeaders; l++)
                {
                    if (PtHeaderDataTable[l].objID < NumObjectiveEntries)
                    {
                        int featureCount = ObjDataTable[PtHeaderDataTable[l].objID].Features;

                        for (int t = 0; t < Camplib.MAX_FEAT_DEPEND; t++)
                        {
                            if (PtHeaderDataTable[l].features[t] != 255 && PtHeaderDataTable[l].features[t] >= featureCount)
                            {
                                log.ErrorFormat("PtHeaderDataTable[{0}].features[{1}]={2} >= Objective[{3}]'s Features {4}",
                                            l, t, PtHeaderDataTable[l].features[t], PtHeaderDataTable[l].objID, featureCount);

                                PtHeaderDataTable[l].features[t] = 255;
                            }
                        }
                    }
                    else
                    {
                        log.ErrorFormat("PtHeaderDataTable[{0}].objId={1} >= NumObjectiveEntries={2}\n",
                                l, PtHeaderDataTable[l].objID, NumObjectiveEntries);
                    }
                }
            }

            for (int l = 0; l < NumObjectiveEntries; l++)
            {
                if ((ObjDataTable[l].PtDataIndex >= NumPtHeaders) || (ObjDataTable[l].PtDataIndex < 0))
                {
                    log.ErrorFormat("ObjDataTable[{0}].PtDataIndex >= NumPtHeaders = {1} or < 0",
                                l, ObjDataTable[l].PtDataIndex, NumPtHeaders);

                    ObjDataTable[l].PtDataIndex = 0;
                }
            }

            return true;
        }

        public static bool LoadPtData(string filename)
        {
            using (FileStream fp = F4File.OpenCampFile(filename, "PD", FileAccess.Read))
            {
                NumPts = Int16EncodingLE.Decode(fp);
                PtDataTable = new PtDataType[NumPts];
                for (int i = 0; i < NumPts; i++)
                {
                    PtDataTable[i] = new PtDataType();
                    PtDataTypeEncodingLE.Decode(fp, ref PtDataTable[i]);
                }

                fp.Close();
            }
            return true;
        }

        public static bool LoadFeatureEntryData(string filename)
        {
            string fname = filename + "tree";
            if (F4Config.g_bDisplayTrees)
            {
                string name = F4File.F4FindFile(fname + ".FED");
                if (File.Exists(name))
                {
                    fedtree = true;
                }
                else
                {
                    fedtree = false;
                    fname = filename;
                }
            }
            using (FileStream fp = F4File.OpenCampFile(fname, "FED", FileAccess.Read))
            {
                NumObjFeatEntries = Int16EncodingLE.Decode(fp);
                FeatureEntryDataTable = new FeatureEntry[NumObjFeatEntries];
                for (int i = 0; i < NumObjFeatEntries; i++)
                {
                    FeatureEntryDataTable[i] = new FeatureEntry();
                    FeatureEntryEncodingLE.Decode(fp, FeatureEntryDataTable[i]);
                }
                fp.Close();
            }
            return true;

        }

        public static bool LoadRadarData(string filename)
        {
            using (FileStream fp = F4File.OpenCampFile(filename, "RCD", FileAccess.Read))
            {
                NumRadarEntries = Int16EncodingLE.Decode(fp);
                RadarDataTable = new RadarDataType[NumRadarEntries];
                for (int i = 0; i < NumRadarEntries; i++)
                {
                    RadarDataTable[i] = new RadarDataType();
                    RadarDataTypeEncodingLE.Decode(fp, RadarDataTable[i]);
                }

                fp.Close();
            }
            return true;
        }

        public static bool LoadIRSTData(string filename)
        {
            using (FileStream fp = F4File.OpenCampFile(filename, "ICD", FileAccess.Read))
            {
                NumIRSTEntries = Int16EncodingLE.Decode(fp);
                IRSTDataTable = new IRSTDataType[NumIRSTEntries];
                for (int i = 0; i < NumIRSTEntries; i++)
                {
                    IRSTDataTable[i] = new IRSTDataType();
                    IRSTDataTypeEncodingLE.Decode(fp, IRSTDataTable[i]);
                }

                fp.Close();
            }
            return true;
        }

        public static bool LoadRwrData(string filename)
        {
            using (FileStream fp = F4File.OpenCampFile(filename, "rwd", FileAccess.Read))
            {
                NumRwrEntries = Int16EncodingLE.Decode(fp);
                RwrDataTable = new RwrDataType[NumRwrEntries];
                for (int i = 0; i < NumRwrEntries; i++)
                {
                    RwrDataTable[i] = new RwrDataType();
                    RwrDataTypeEncodingLE.Decode(fp, RwrDataTable[i]);
                }

                fp.Close();
            }
            return true;
        }

        public static bool LoadVisualData(string filename)
        {
            using (FileStream fp = F4File.OpenCampFile(filename, "vsd", FileAccess.Read))
            {
                NumVisualEntries = Int16EncodingLE.Decode(fp);
                VisualDataTable = new VisualDataType[NumVisualEntries];
                for (int i = 0; i < NumVisualEntries; i++)
                {
                    VisualDataTable[i] = new VisualDataType();
                    VisualDataTypeEncodingLE.Decode(fp, VisualDataTable[i]);
                }

                fp.Close();
            }
            return true;
        }

        public static bool LoadSimWeaponData(string filename)
        {
            using (FileStream fp = F4File.OpenCampFile(filename, "SWD", FileAccess.Read))
            {
                NumSimWeaponEntries = Int16EncodingLE.Decode(fp);
                SimWeaponDataTable = new SimWeaponDataType[NumSimWeaponEntries];
                for (int i = 0; i < NumSimWeaponEntries; i++)
                {
                    SimWeaponDataTable[i] = new SimWeaponDataType();
                    SimWeaponDataTypeEncodingLE.Decode(fp, SimWeaponDataTable[i]);
                }

                fp.Close();
            }
            return true;
        }

        public static bool LoadACDefData(string filename)
        {
            using (FileStream fp = F4File.OpenCampFile(filename, "ACD", FileAccess.Read))
            {
                NumACDefEntries = Int16EncodingLE.Decode(fp);
                SimACDefTable = new SimACDefType[NumACDefEntries];
                for (int i = 0; i < NumACDefEntries; i++)
                {
                    SimACDefTable[i] = new SimACDefType();
                    SimACDefTypeEncodingLE.Decode(fp, SimACDefTable[i]);
                }

                fp.Close();
            }
            return true;
        }

        public static bool LoadSquadronStoresData(string filename)
        {

            using (FileStream fp = F4File.OpenCampFile(filename, "SSD", FileAccess.Read))
            {
                NumSquadTypes = Int16EncodingLE.Decode(fp);
                SquadronStoresDataTable = new SquadronStoresDataType[NumSquadTypes];
                for (int i = 0; i < NumSquadTypes; i++)
                {
                    SquadronStoresDataTable[i] = new SquadronStoresDataType();
                    SquadronStoresDataTypeEncodingLE.Decode(fp, SquadronStoresDataTable[i]);
                }

                fp.Close();
            }
            return true;
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

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}

