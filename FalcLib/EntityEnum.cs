using System;

namespace FalconNet.FalcLib
{
    [Flags]
    public enum EntityEnum : sbyte
    {
        FalconCampaignEntity = 0x1,
        FalconSimEntity = 0x2,
        FalconPersistantEntity = 0x8,
        FalconSimObjective = 0x20
    }
}
