using System;

namespace FalconNet.FalcLib
{
    // Flight specific status flags
    [Flags]
    public enum FlightStatus
    {
        MISEVAL_FLIGHT_LOSSES = 0x00000001,
        MISEVAL_FLIGHT_DESTROYED = 0x00000002,
        MISEVAL_FLIGHT_ABORTED = 0x00000004,
        MISEVAL_FLIGHT_GOT_AKILL = 0x00000010,		// Air kill
        MISEVAL_FLIGHT_GOT_GKILL = 0x00000020,		// Ground kill
        MISEVAL_FLIGHT_GOT_NKILL = 0x00000040,		// Naval kill
        MISEVAL_FLIGHT_GOT_SKILL = 0x00000080,		// Static kill
        MISEVAL_FLIGHT_HIT_HIGH_VAL = 0x00000100,		// Hit a high value target (Feature with value)
        MISEVAL_FLIGHT_HIT_BY_AIR = 0x00001000,		// Suffered loss to air (during ingress only!)
        MISEVAL_FLIGHT_HIT_BY_GROUND = 0x00002000,		// Suffered loss to ground (during ingress only!)
        MISEVAL_FLIGHT_HIT_BY_NAVAL = 0x00004000,		// Suffered loss to naval (during ingress only!)
        MISEVAL_FLIGHT_TARGET_HIT = 0x00010000,		// We hit our target
        MISEVAL_FLIGHT_TARGET_KILLED = 0x00020000,		// We killed out target
        MISEVAL_FLIGHT_TARGET_ABORTED = 0x00040000,		// We forced our target to abort
        MISEVAL_FLIGHT_AREA_HIT = 0x00080000,		// We hit an enemy in our target area
        MISEVAL_FLIGHT_F_TARGET_HIT = 0x00100000,		// The friendly we were assigned to was hit
        MISEVAL_FLIGHT_F_TARGET_KILLED = 0x00200000,		// The friendly we were assigned to was killed
        MISEVAL_FLIGHT_F_TARGET_ABORTED = 0x00400000,		// The friendly we were assigned to aborted
        MISEVAL_FLIGHT_F_AREA_HIT = 0x00800000,		// Our friendly target region was hit
        MISEVAL_FLIGHT_STARTED_LATE = 0x01000000,		// This mission wasn't started in time to count full
        MISEVAL_FLIGHT_GOT_TO_TARGET = 0x02000000,		// Flight got to target area
        MISEVAL_FLIGHT_STATION_OVER = 0x04000000,		// Station/mission time is over
        MISEVAL_FLIGHT_GOT_HOME = 0x08000000,		// We returned to friendly territory
        MISEVAL_FLIGHT_RELIEVED = 0x10000000,		// Flight was allowed to leave by AWACS/FAC
        MISEVAL_FLIGHT_OFF_STATION = 0x20000000,		// Flight left its station area

        // 2002-02-13 MN 
        MISEVAL_FLIGHT_ABORT_BY_AWACS = 0x40000000		// flight aborted by AWACS instruction - we occupied the target
    }

}
