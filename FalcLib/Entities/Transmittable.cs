using System;

namespace FalconNet.FalcLib
{
    // Transmittable
    // Various user flags
    [Flags]
    public enum Transmittable : short
    {
        CBC_EMITTING = 0x01,
        CBC_JAMMED = 0x04
    }
}
