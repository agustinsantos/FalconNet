using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.FalcLib
{
    // flags for mission data
    [Flags]
    public enum MissionDataFlagEnum
    {
        AMIS_ADDAWACS = 0x01,				// Request AWACS
        AMIS_ADDJSTAR = 0x02,				// Request JSTAR
        AMIS_ADDECM = 0x04,				// Request ECM
        AMIS_ADDBDA = 0x08,				// Request BDA
        AMIS_ADDESCORT = 0x10,				// Request CA Escort (if enemy responds)
        AMIS_ADDSEAD = 0x20,				// Request SEAD Escort (if enemy AD present)
        AMIS_ADDBARCAP = 0x40,				// Trigger enemy BARCAP
        AMIS_ADDSWEEP = 0x80,				// Trigger enemy Sweep
        AMIS_ADDOCASTRIKE = 0x100,				// Trigger friendly OCA Strike vs enemy assets found
        AMIS_ADDTANKER = 0x200,				// Add a tanker, if possible
        AMIS_NEEDTANKER = 0x400,				// This mission can't go w/o a tanker
        AMIS_ADDFAC = 0x800,				// Request FAC aircraft
        AMIS_ADDINTERCEPT = 0x1000,			// Add an enemy intercept mission to intercept this mission

        AMIS_NOTHREAT = 0x2000,			// Abort if above the minimum threat threshold
        AMIS_AVOIDTHREAT = 0x4000,			// Dodge ADs vertically
        AMIS_HIGHTHREAT = 0x8000,			// Abort only if extreme threat
        AMIS_IMMEDIATE = 0x10000,			// Set if I'm looking for aircraft in flight
        AMIS_MATCHSPEED = 0x20000,			// Try to build package with equivalent speed a/c
        AMIS_TARGET_ONLY = 0x40000,			// go directly to target and back
        AMIS_NO_BREAKPT = 0x80000,			// Don't add a breakpoint to this mission
        AMIS_DONT_COORD = 0x100000,			// Don't match ingress/egress times with the main flight
        AMIS_AIR_LAUNCH_OK = 0x200000,			// This mission can launch in mid-air (ie: from the edge of the map)
        AMIS_DONT_USE_AC = 0x400000,			// Don't subtract aircraft from the squadron
        AMIS_NPC_ONLY = 0x800000,			// Don't assign to potential player squadrons
        AMIS_FUDGE_RANGE = 0x1000000,		// This mission is always in range - we'll just load more fuel
        AMIS_EXPECT_DIVERT = 0x2000000,		// We want to sit around until we're told to divert
        AMIS_ASSIGNED_TAR = 0x4000000,		// Used for diverts with a specific target in mind
        AMIS_NO_DIST_BONUS = 0x8000000,		// Don't adjust mission priority by distance to front (add a constant)
        AMIS_NO_TARGETABORT = 0x10000000,			// Don't abort after reaching our target - just fly home
        AMIS_FLYALWAYS = 0x20000000,			// Don't restrict percentage of missions flown
        AMIS_HELP_REQUEST = 0x40000000,		// 2001-10-27 S.G. It's from an help request
    }
}
