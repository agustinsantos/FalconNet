using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.FalcLib
{
    [Flags]
    public enum FEC_FLAGS : byte
    {
        FEC_HOLDSHORT = 0x01,		// Don't takeoff until a player attaches
        FEC_PLAYERONLY = 0x02,		// This entity is only valid if under player control
        FEC_HASPLAYERS = 0x04,		// One or more player is attached to this entity
        FEC_REGENERATING = 0x08,		// This entity is undead.
        FEC_PLAYER_ENTERING = 0x10,		// A player is soon to attach to this aircraft/flight
        FEC_INVULNERABLE = 0x20		// This thing can't be destroyed
    }
}
