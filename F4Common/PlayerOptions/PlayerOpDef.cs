using System;

namespace FalconNet.F4Common
{
    // =====================
    // Defines & enums
    // =====================


    //TODO enum{	PL_FNAME_LEN		=	32};

    [Flags]
    public enum SoundFlagType
    {
        SNDFNEWENG = 0x01, // MLR 12/13/2003 -
        SNDFDOP = 0x02,
        SNDFDISTE = 0x04,
        SNDFVMSEXT = 0x08
    }
}

