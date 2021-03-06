﻿using FalconNet.Common;
using System;

namespace FalconNet.CampaignBase
{
    public static class CampGlobal
    {
        // ----------------
        // Type Definitions
        // ----------------

        public const float GRID_SIZE_FT = Phyconst.FEET_PER_KM;		// Grid size, in feet (standard sim unit)
        public const float GRID_SIZE_KM = 1.0F;			// Grid size, in km (standard campaign unit)

        public const int HALF_CHANCE = 16000;			// Half of RAND_MAX

        public const int TOD_SUNUP = 300;				// Sun up, in minutes since midnight
        public const int TOD_SUNDOWN = 1260;			// Sun down, in minutes since midnight
        public const int TOD_NIGHT = 1;
        public const int TOD_DAWNDUSK = 2;
        public const int TOD_DAY = 3;

        public const int TYPE_NE = 0;				// Equivalency defines.. See FORTRAN.. ;-)
        public const int TYPE_LT = 1;
        public const int TYPE_LE = 2;
        public const int TYPE_EQ = 3;
        public const int TYPE_GE = 4;
        public const int TYPE_GT = 5;

        public const int NOT_DETECTED = 0;
        public const int DETECTED_VISUAL = 1;
        public const int DETECTED_RADAR = 2;

        public static CampaignTime[] ReconLossTime = new CampaignTime[(int)MoveType.MOVEMENT_TYPES]
        {
            CampaignTime.CampaignHours, 
            20 * CampaignTime.CampaignMinutes, 
            10 * CampaignTime.CampaignMinutes,
            10 * CampaignTime.CampaignMinutes, 
            1 * CampaignTime.CampaignMinutes,
            1 * CampaignTime.CampaignMinutes,
            10 * CampaignTime.CampaignMinutes,
            10 * CampaignTime.CampaignMinutes
        };



        public const int ALT_LEVELS = 5;

#if TODO
        typedef char CampaignSaveKey;
        typedef byte Percentage;
        typedef byte Control;
        typedef byte Team;
        typedef byte Value;
        typedef byte UnitType;
        typedef byte UnitSize;
        typedef byte ObjectiveType;
        typedef byte CampaignOrders;
        typedef byte CampaignHeading;
              
        typedef char PriorityLevel;
#endif



        public static bool MOVE_GROUND(MoveType X) { return X == MoveType.Foot || X == MoveType.Wheeled || X == MoveType.Tracked; }
        public static bool MOVE_AIR(MoveType X) { return X == MoveType.Air || X == MoveType.LowAir; }
        public static bool MOVE_NAVAL(MoveType X) { return X == MoveType.Naval; }
        public static bool MOVE_NONE(MoveType X) { return X == MoveType.NoMove; }

        public const int North = 0;
        public const int NorthEast = 1;
        public const int East = 2;
        public const int SouthEast = 3;
        public const int South = 4;
        public const int SouthWest = 5;
        public const int West = 6;
        public const int NorthWest = 7;
        public const int Here = 8;

        public const int RELIEF_TYPES = 4;
        public const int COVER_TYPES = 8;

    }
}
