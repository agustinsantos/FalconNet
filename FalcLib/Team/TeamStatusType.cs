using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.FalcLib
{
    public struct TeamStatusType
    {
        ushort airDefenseVehs;
        ushort aircraft;
        ushort groundVehs;
        ushort ships;
        ushort supply;
        ushort fuel;
        ushort airbases;
        byte supplyLevel;							// Supply in terms of pecentage
        byte fuelLevel;								// fuel in terms of pecentage
    }
}
