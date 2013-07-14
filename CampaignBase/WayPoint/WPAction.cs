
namespace FalconNet.CampaignBase
{
    public enum WPAction : byte
    {
        // Waypoint actions
        WP_NOTHING = 0,
        WP_TAKEOFF = 1,
        WP_ASSEMBLE = 2,
        WP_POSTASSEMBLE = 3,
        WP_REFUEL = 4,
        WP_REARM = 5,
        WP_PICKUP = 6,			// Pick up a unit
        WP_LAND = 7,
        WP_TIMING = 8,				// Just cruise around wasting time
        WP_CASCP = 9,				// CAS contact point

        WP_ESCORT = 10,				// Engage engaging fighters
        WP_CA = 11,			// Engage all enemy aircraft
        WP_CAP = 12,				// Patrol area for enemy aircraft
        WP_INTERCEPT = 13,				// Engage specific enemy aircraft
        WP_GNDSTRIKE = 14,				// Engage enemy units at target
        WP_NAVSTRIKE = 15,				// Engage enemy shits at target
        WP_SAD = 16,				// Engage any enemy at target
        WP_STRIKE = 17,				// Destroy enemy installation at target
        WP_BOMB = 18,				// Strategic bomb enemy installation at target				
        WP_SEAD = 19,				// Suppress enemy air defense at target
        WP_ELINT = 20,				// Electronic intellicence (AWACS, JSTAR, ECM)
        WP_RECON = 21,				// Photograph target location
        WP_RESCUE = 22,				// Rescue a pilot at location
        WP_ASW = 23,
        WP_TANKER = 24,				// Respond to tanker requests
        WP_AIRDROP = 25,
        WP_JAM = 26,

        // M.N. fix for Airlift missions
        //   WP_LAND2			27				// this is our 2nd landing waypoint for Airlift missions
        // supply will be given at WP_LAND MNLOOK . Change in string.txt 377

        WP_B5 = 27,
        WP_B6 = 28,
        WP_B7 = 29,
        WP_FAC = 30,

        WP_MOVEOPPOSED = 40,				// These are movement wps
        WP_MOVEUNOPPOSED = 41,
        WP_AIRBORNE = 42,
        WP_AMPHIBIOUS = 43,
        WP_DEFEND = 44,				// These are action wps
        WP_REPAIR = 45,
        WP_RESERVE = 46,
        WP_AIRDEFENSE = 47,
        WP_FIRESUPPORT = 48,
        WP_SECURE = 49
    }
}
