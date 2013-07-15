using System;
namespace FalconNet.F4Common
{
    // Object Flags
    [Flags]
    public enum PO_OBJ_FLAGS
    {
        DISP_OBJ_TEXTURES = 0x01,
        DISP_OBJ_SHADING = 0x02,
        DISP_OBJ_DYN_SCALING = 0x04,
    }
}