using System;
using System.Collections.Generic;
using System.Diagnostics;
using FalconNet.Common;
using FalconNet.FalcLib;
//using FalconNet.UI;
using FalconNet.VU;
using Flight = FalconNet.Campaign.FlightClass;
using Objective = FalconNet.Campaign.ObjectiveClass;
using Team = System.Byte;
using GridIndex = System.Int16;
using FalconNet.CampaignBase;
using FalconNet.F4Common;
namespace FalconNet.Campaign
{

    public class MissEvakStatic
    {
        // ====================================
        // Some defines (maybe move to AIInput)
        // ====================================




        /* TODO
    // Mission Evaluator status flags
    #define MISEVAL_MISSION_IN_PROGRESS		0x01			// We want to start evaluating stuff
    #define MISEVAL_EVALUATE_HITS_IN_AREA	0x02			// Check to see if someone has bombed our area
    #define MISEVAL_GAME_COMPLETED			0x04			// We finished a dogfight game
    #define MISEVAL_ONLINE_GAME				0x08			// This was a multi-player game
	
    // Pilot flags
    #define PFLAG_PLAYER_CONTROLLED			0x01			// Pilot is a player
    #define PFLAG_WON_GAME					0x02			// Pilot is on the winning side
*/
    }


    // =============================
    // Global setter/query functions
    // =============================
    public static class MissEval
    {
        public static int OverFriendlyTerritory(Flight flight)
        {
            throw new NotImplementedException();
        }
    }
}

