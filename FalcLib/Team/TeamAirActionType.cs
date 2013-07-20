using FalconNet.CampaignBase;
using FalconNet.VU;

namespace FalconNet.FalcLib
{
    public struct TeamAirActionType
    {
        CampaignTime actionStartTime;						// When we start.
        CampaignTime actionStopTime;							// When we are supposed to be done by.
        VU_ID actionObjective;						// Primary objective this is all about
        VU_ID lastActionObjective;
        byte actionType;
    }
}
