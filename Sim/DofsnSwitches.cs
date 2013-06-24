﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    public enum Switches
    {
        COMP_AB = 0,
        COMP_NOS_GEAR_SW = 1,
        COMP_LT_GEAR_SW = 2,
        COMP_RT_GEAR_SW = 3,
        COMP_NOS_GEAR_ROD = 4,
        COMP_CANOPY = 5,
        COMP_WING_VAPOR = 6,
        COMP_TAIL_STROBE = 7,
        COMP_NAV_LIGHTS = 8,
        COMP_LAND_LIGHTS = 9,
        COMP_EXH_NOZZLE = 10,
        COMP_TIRN_POD = 11,
        COMP_HTS_POD = 12,
        COMP_REFUEL_DR = 13,
        COMP_NOS_GEAR_DR_SW = 14,
        COMP_LT_GEAR_DR_SW = 15,
        COMP_RT_GEAR_DR_SW = 16,
        COMP_NOS_GEAR_HOLE = 17,
        COMP_LT_GEAR_HOLE = 18,
        COMP_RT_GEAR_HOLE = 19,
        COMP_BROKEN_NOS_GEAR_SW = 20,
        COMP_BROKEN_LT_GEAR_SW = 21,
        COMP_BROKEN_RT_GEAR_SW = 22,

        COMP_HOOK = 23,
        COMP_DRAGCHUTE = 24, // landing drag chute
        COMP_MAX_SWITCH = 25,	// update!

        SIMP_AB = 0,
        SIMP_TANKERLIGHTS = 1,
        SIMP_GEAR = 2,
        SIMP_WING_VAPOR = 3,
        SIMP_CANOPY = 5,
        SIMP_DRAGCHUTE = 6,
        SIMP_HOOK = 7,
        SIMP_MAX_SWITCH = 8,

        HELI_ROTORS = 0,
        HELI_MAX_SWITCH = 2,

        AIRDEF_MAX_SWITCH = 1,
    }

    public enum DOFS
    {
        COMP_LT_STAB = 0,
        COMP_RT_STAB = 1,
        COMP_LT_FLAP = 2, // Flapperons or ailerons
        COMP_RT_FLAP = 3,
        COMP_RUDDER = 4,
        COMP_NOS_GEAR_ROT = 5,
        COMP_NOS_GEAR_COMP = 6,
        COMP_LT_GEAR_COMP = 7,
        COMP_RT_GEAR_COMP = 8,
        COMP_LT_LEF = 9,
        COMP_RT_LEF = 10,
        COMP_BROKEN_NOS_GEAR = 11,
        COMP_BROKEN_LT_GEAR = 12,
        COMP_BROKEN_RT_GEAR = 13,
        COMP_NOTUSED_14 = 14,//available
        COMP_LT_AIR_BRAKE_TOP = 15,
        COMP_LT_AIR_BRAKE_BOT = 16,
        COMP_RT_AIR_BRAKE_TOP = 17,
        COMP_RT_AIR_BRAKE_BOT = 18,
        COMP_NOS_GEAR = 19,
        COMP_LT_GEAR = 20,
        COMP_RT_GEAR = 21,
        COMP_NOS_GEAR_DR = 22,
        COMP_LT_GEAR_DR = 23,
        COMP_RT_GEAR_DR = 24,
        // 25-27 are used in some models (like F16) for landing gear bits
        // hence this gap
        COMP_LT_TEF = 28, // JPO
        COMP_RT_TEF = 29,
        COMP_CANOPY_DOF = 30, // opening canopy
        // 31-37 earmarked for prop animation DOFs, so next named one should be 37 please.
        COMP_MAX_DOF = 31, // Make sure this is up to date!

        SIMP_LT_STAB = 0,
        SIMP_RT_STAB = 1,
        // this gap used by animations (including 6 & 7 actually) JPO
        SIMP_LT_AILERON = 6,
        SIMP_RT_AILERON = 7,
        SIMP_RUDDER_1 = 8,
        SIMP_RUDDER_2 = 9,
        SIMP_AIR_BRAKE = 10,
        SIMP_SWING_WING_1 = 11,
        SIMP_SWING_WING_2 = 12,
        SIMP_SWING_WING_3 = 13,
        SIMP_SWING_WING_4 = 14,
        SIMP_SWING_WING_5 = 15,
        SIMP_SWING_WING_6 = 16,
        SIMP_SWING_WING_7 = 17,
        SIMP_SWING_WING_8 = 18,
        SIMP_RT_TEF = 19, // JPO - new stuff
        SIMP_LT_TEF = 20,
        SIMP_RT_LEF = 21,
        SIMP_LT_LEF = 22,
        SIMP_CANOPY_DOF = 23, // opening canopy
        SIMP_MAX_DOF = 24, // MAKE SURE THIS IS UP TO DATE


        HELI_MAIN_ROTOR = 2,
        HELI_TAIL_ROTOR = 4,
        HELI_MAX_DOF = 6,

        AIRDEF_AZIMUTH = 0,
        AIRDEF_ELEV = 1,
        AIRDEF_ELEV2 = 2,
        AIRDEF_MAX_DOF = 3,
    }

    public enum Vertices
    {
        AIRCRAFT_MAX_DVERTEX = 6,
        HELI_MAX_DVERTEX = 0,
        VECH_MAX_DVERTEX = 0,

    }
}
