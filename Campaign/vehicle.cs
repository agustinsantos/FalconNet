using System;
using Unit = FalconNet.Campaign.UnitClass;
using VehicleSP = System.Byte; //typedef  uchar VehicleSP;
using VehicleID = System.Int16;
using FalconNet.FalcLib; //typedef  short VehicleID;

namespace FalconNet.Campaign
{

    // ===================
    // Flags
    // ===================
    public enum VEH_FLAGS
    {
        // Aircraft special capibilities and service types
        VEH_AIRFORCE = 0x01,
        VEH_NAVY = 0x02,
        VEH_MARINES = 0x04,
        VEH_ARMY = 0x08,
        VEH_ALLWEATHER = 0x10,
        VEH_STEALTH = 0x20,
        VEH_NIGHT = 0x40,
        VEH_VTOL = 0x80,

        // General flags
        VEH_FLAT_CONTAINER = 0x100,			// Vehicles typically sit on this
        VEH_ELEV_CONTAINER = 0x200,

        VEH_CAN_EXPLODE = 0x400,
        VEH_CAN_BURN = 0x800,
        VEH_CAN_SMOKE = 0x1000,
        VEH_CAN_COLAPSE = 0x2000,

        VEH_HAS_CREW = 0x4000,
        VEH_IS_TOWED = 0x8000,
        VEH_HAS_JAMMER = 0x10000,		// Has built in self protection jamming
        VEH_IS_AIR_DEFENSE = 0x20000,			// This ground thing prefers to shoot air things

        // 2002-02-13 ADDED BY S.G. Used to flag the unit/vehicle has having NCTR or EXACT RWR capabilities
        VEH_HAS_NCTR = 0x40000,
        VEH_HAS_EXACT_RWR = 0x80000,

        VEH_USES_UNIT_RADAR = 0x100000,// 2002-02-28 ADDED BY S.G.

        // Service/capibility masks
        VEH_SERVICE_MASK = 0x0F,
        VEH_CAPIBILITY_MASK = 0xF0
    }



    // ===================
    // Global Functions
    // ===================
    public static class VehicleStatic
    {
        public static string GetVehicleName(VehicleID vid)
        { throw new NotImplementedException(); }

        public static VehicleClassDataType GetVehicleClassData(int index)
        { throw new NotImplementedException(); }

        // public static  int GetVehicleHitChance (Unit u, int id, int mt, int range, int hitflags);

        public static int GetAproxVehicleHitChance(int id, MoveType mt, int range)
        { throw new NotImplementedException(); }

        public static int CalculateVehicleHitChance(int id, MoveType mt)
        { throw new NotImplementedException(); }

        // public static  int GetVehicleCombatStrength (Unit u, int id, MoveType mt, int range);

        public static int GetAproxVehicleCombatStrength(int id, MoveType mt, int range)
        { throw new NotImplementedException(); }

        public static int CalculateVehicleCombatStrength(int id, MoveType mt)
        { throw new NotImplementedException(); }

        public static int GetAproxVehicleRange(int id, MoveType mt)
        { throw new NotImplementedException(); }

        // public static  int GetVehicleRange (Unit u, int id, int mt);

        public static int CalculateVehicleRange(int id, MoveType mt)
        { throw new NotImplementedException(); }

        public static int GetVehicleDetectionRange(int id, MoveType mt)
        { throw new NotImplementedException(); }

        public static int GetBestVehicleWeapon(int id, byte[] dam, MoveType m, int range, ref int hard_point)
        { throw new NotImplementedException(); }

        public static void CalculateVehicleStatistics(int id)
        { throw new NotImplementedException(); }

        public static int GetVehicleWeapon(int vid, int hp)
        { throw new NotImplementedException(); }

        public static int GetVehicleWeapons(int vid, int hp)
        { throw new NotImplementedException(); }

    }
}
