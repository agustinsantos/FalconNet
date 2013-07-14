using System;

namespace FalconNet.CampaignBase
{
    // WP flags
    [Flags]
    public enum WPFlags :ulong
    {
        WPF_TARGET = 0x0001,			// This is a target wp
        WPF_ASSEMBLE = 0x0002,			// Wait for other elements here
        WPF_BREAKPOINT = 0x0004,			// Break point
        WPF_IP = 0x0008,			// IP waypoint
        WPF_TURNPOINT = 0x0010,			// Turn point
        WPF_CP = 0x0020,			// Contact point
        WPF_REPEAT = 0x0040,			// Return to previous WP until time is exceeded
        WPF_TAKEOFF = 0x0080,
        WPF_LAND = 0x0100,			// Suck aircraft back into squadron
        WPF_DIVERT = 0x0200,			// This is a divert WP (deleted upon completion of divert)
        WPF_ALTERNATE = 0x0400,			// Alternate landing site
        // Climb profile flags			
        WPF_HOLDCURRENT = 0x0800,			// Stay at current altitude until last minute
        // Other stuff
        WPF_REPEAT_CONTINUOUS = 0x1000,			// Do this until the end of time
        WPF_IN_PACKAGE = 0x2000,			// This is a package-coordinated wp
        // Even better "Other Stuff"
        WPF_TIME_LOCKED = 0x4000,			// This waypoint will have an arrive time as given, and will not be changed
        WPF_SPEED_LOCKED = 0x8000,			// This waypoint will have a speed as given, and will not be changed.
        WPF_REFUEL_INFORMATION = 0x10000,			// This waypoint is only an informational waypoint, no mission waypoint
        WPF_REQHELP = 0x20000,			// This divert waypoint is one from a request help call

        WPF_CRITICAL_MASK = 0x07FF 			// If it's one of these, we can't skip this waypoint
    }
}
