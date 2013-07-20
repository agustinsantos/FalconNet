
namespace FalconNet.FalcLib
{
    // Local
    public enum CBC_ENUM
    {
        CBC_CHECKED = 0x001,			// Used by mission planning to prevent repeated targetting
        CBC_AWAKE = 0x002,			// Deaggregated on local machine
        CBC_IN_PACKAGE = 0x004,			// This item is in our local package (only applicable to flights)
        CBC_HAS_DELTA = 0x008,
        CBC_IN_SIM_LIST = 0x010,			// In the sim's nearby campaign entity lists
        CBC_INTEREST = 0x020,			// Some session still is interested in this entity
        CBC_RESERVED_ONLY = 0x040,			// This entity is here only in order to reserve namespace
        CBC_AGGREGATE = 0x080,
        CBC_HAS_TACAN = 0x100
    }
}
