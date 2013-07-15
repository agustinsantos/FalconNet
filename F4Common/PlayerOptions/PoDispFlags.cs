using System;
namespace FalconNet.F4Common
{
    // Display Flags
    [Flags]
    public enum PO_DISP_FLAGS
    {
        DISP_GOURAUD = 0x01,
        DISP_HAZING = 0x02,
        DISP_PERSPECTIVE = 0x04,
        DISP_SHADOWS = 0x08,
        DISP_BILINEAR = 0x10,
        DISP_ALPHA_BLENDING = 0x20,
    }
}