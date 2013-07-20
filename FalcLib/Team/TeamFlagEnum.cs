using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.FalcLib
{
    // Team flags
    [Flags]
    public enum TeamFlagEnum : short
    {
        TEAM_ACTIVE = 0x01,	// Set if team is being used
        TEAM_HASSATS = 0x02,	// Has satelites
        TEAM_UPDATED = 0x04,	// We've gotten remote data for this team
    }
}
