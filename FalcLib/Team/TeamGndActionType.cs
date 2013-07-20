using FalconNet.CampaignBase;
using FalconNet.VU;

namespace FalconNet.FalcLib
{
    public struct TeamGndActionType
    {
        public CampaignTime actionTime;								// When we start.
        public CampaignTime actionTimeout;							// Our action will fail if not completed by this time
        public VU_ID actionObjective;						// Primary objective this is all about
        public byte actionType;
        public byte actionTempo;							// How "active" we want the action to be
        public byte actionPoints;							// Countdown of how much longer it will go on
    }
}
