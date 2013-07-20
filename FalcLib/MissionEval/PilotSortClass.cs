using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.FalcLib
{
    // ===========================================================
    // class used for temporary pilot list building/sorting
    // ===========================================================

    public class PilotSortClass
    {
        public short team;
        public PilotDataClass pilot_data;
        public PilotSortClass next;

        public PilotSortClass(PilotDataClass pilot_ptr)
        {
            pilot_data = pilot_ptr;
            next = null;
        }
    }
}
