using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.FalcLib
{
    public enum UHF_Mode_Type
    {
        UHF_NORM,
        UHF_BACKUP
    }

    public enum Point_Type
    {
        NODATA,
        GMPOINT,
        POS
    }



    public enum NavigationType
    {
        AIRBASE,
        CARRIER,
        TANKER,
        TOTAL_TYPES
    }

    // Storage for Tacan Data
    public enum Tacan_Channel_Src
    {
        ICP,
        AUXCOMM,
        TOTAL_SRC
    }

    public enum Instrument_Mode
    {
        NAV,
        ILS_NAV,
        ILS_TACAN,
        TACAN,
        TOTAL_MODES
    }

    public enum Attribute
    {
        X_POS,
        Y_POS,
        Z_POS,
        RWY_NUM,
        GP_DEV,
        GS_DEV,
        AIRBASE_ID,
        RANGE,
        ILSFREQ,
        TOTAL_ATTRIBUTES
    }
}
