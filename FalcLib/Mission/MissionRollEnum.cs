using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.FalcLib
{
    // Mission rolls
    public enum MissionRollEnum :byte
    {
        ARO_NOTHING = 0,
        ARO_CA = 1,
        ARO_TACTRANS = 2,     // AMIS_SAR | AMIS_AIRCAV
        ARO_S = 3,     // AMIS_OCASTRIKE | AMIS_INTSTRIKE | 																// Strike target
        ARO_GA = 4,     // AMIS_SAD | AMIS_BAI | AMIS_ONCALLCAS | AMIS_PRPLANCAS	 									// Ground attack
        ARO_SB = 5,     // AMIS_STRATBOMB           																		// Strategic bomb
        ARO_ECM = 6,     // AMIS_ELJAM           																				// Jam radar
        ARO_SEAD = 7,     // AMIS_SEADSTRIKE | AMIS_SEADESCORT																// SEAD
        ARO_ASW = 8,	   // AMIS_ASW
        ARO_ASHIP = 9,     // AMIS_ASHIP
        ARO_REC = 10,    // AMIS_BDA | AMIS_RECON | AMIS_PATROL																// Recon
        ARO_TRANS = 11,    // AMIS_AIRBORNE | AMIS_AIRLIFT          														// Drop off cargo
        ARO_ELINT = 12,    // AMIS_AWACS
        ARO_AWACS = 12,
        ARO_JSTAR = 13,    // AMIS_JSTAR
        ARO_TANK = 14,    // AMIS_TANKER
        ARO_FAC = 15,
        ARO_OTHER = 16,
    }
}
