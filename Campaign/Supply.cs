using System;
using FalconNet.FalcLib;
using Team = System.Byte;
using FalconNet.CampaignBase;

namespace FalconNet.Campaign
{
    public class Supply
    {
        public const int SUPPLY_PT_FUEL = 10000;			// How many lbs of fuel each point of supply fuel is worth

        // ==================
        // Supply functions
        // ==================

        public static int ProduceSupplies(CampaignTime delta)
		{throw new NotImplementedException();}

        public static int SupplyUnits(Team who, CampaignTime delta)
        { throw new NotImplementedException(); }

    }
}
