using System;
using System.Diagnostics;
using FalconNet.FalcLib;
using FalconNet.CampaignBase;

namespace FalconNet.Campaign
{
    // Feature flags
    [Flags]
    public enum FEAT
    {
        FEAT_EREPAIR = 0x01,				// Set if enemy can repair this building
        FEAT_VIRTUAL = 0x02,				// Don't deaggregate as an entity

        FEAT_HAS_LIGHT_SWITCH = 0x04,				// Tells sim to turn on the lights at night
        FEAT_HAS_SMOKE_STACK = 0x08,				// Tells sim to add smoke

        FEAT_FLAT_CONTAINER = 0x100,				// Vehicles typically sit on this
        FEAT_ELEV_CONTAINER = 0x200,

        FEAT_CAN_EXPLODE = 0x400,
        FEAT_CAN_BURN = 0x800,
        FEAT_CAN_SMOKE = 0x1000,
        FEAT_CAN_COLAPSE = 0x2000,

        FEAT_CONTAINER_TOP = 0x4000,
        FEAT_NEXT_IS_TOP = 0x8000,

        // 2002-02-06 MN this feature is not to be added to mission evaluation (trees...)
        FEAT_NO_HITEVAL = 0x10,

        // Feature Entry flags
        FEAT_PREV_CRIT = 0x01,				// Feature associations
        FEAT_NEXT_CRIT = 0x02,
        FEAT_PREV_NORM = 0x04,
        FEAT_NEXT_NORM = 0x08,
    }
    // ---------------------------------------
    // public static al Function Declarations
    // ---------------------------------------
    public static class FeatureStatic
    {
        public static FeatureClassDataType GetFeatureClassData(int index)
        {
            Debug.Assert(EntityDB.Falcon4ClassTable[index].dataPtr != null);

            if (EntityDB.Falcon4ClassTable[index].dataType <= Data_Types.DTYPE_MIN || EntityDB.Falcon4ClassTable[index].dataType >= Data_Types.DTYPE_MAX)  // JB 010106 CTD sanity check
                return null; // JB 010106 CTD sanity check

            return (FeatureClassDataType)EntityDB.Falcon4ClassTable[index].dataPtr;
        }

        public static int GetFeatureRepairTime(int index)
        {
            FeatureClassDataType  fc;

            fc = FeatureStatic.GetFeatureClassData(index);
            if (fc == null)
                return 0;
            return fc.RepairTime;
        }

        public static int GetFeatureHitChance(int id, int mt, int range, int hitflags)
        { throw new NotSupportedException(); }

        public static int GetAproxFeatureHitChance(int id, int mt, int range)
        { throw new NotSupportedException(); }

        public static int CalculateFeatureHitChance(int id, int mt)
        { throw new NotSupportedException(); }

        public static int GetFeatureCombatStrength(int id, int mt, int range)
        { throw new NotSupportedException(); }

        public static int GetAproxFeatureCombatStrength(int id, int mt, int range)
        { throw new NotSupportedException(); }

        public static int CalculateFeatureCombatStrength(int id, int mt)
        { throw new NotSupportedException(); }

        public static int GetAproxFeatureRange(int id, int mt)
        { throw new NotSupportedException(); }

        public static int GetFeatureRange(int id, int mt)
        { throw new NotSupportedException(); }

        public static int CalculateFeatureRange(int id, int mt)
        { throw new NotSupportedException(); }

        public static int GetFeatureDetectionRange(int id, int mt)
        {
            FeatureClassDataType  fc;

            fc = FeatureStatic.GetFeatureClassData(id);
            if (fc == null)
                return 0;
            return fc.Detection[mt];
        }

        public static int GetBestFeatureWeapon(int id, byte[] dam, MoveType m, int range)
        { throw new NotSupportedException(); }
    }
}
