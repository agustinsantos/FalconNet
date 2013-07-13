using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VU_BOOL = System.Boolean;
using VU_KEY = System.UInt64;
using BIG_SCALAR = System.Single;
using SM_SCALAR = System.Single;
using VU_TIME = System.UInt64;
using System.IO;

namespace FalconNet.VU
{
    public abstract class VuKeyFilter : VuFilter
    {
        //TODO public virtual ~VuKeyFilter();
        public override int Compare(VuEntity ent1, VuEntity ent2)
        {
            VU_KEY key1 = Key(ent1);
            VU_KEY key2 = Key(ent2);

            if (key1 > key2)
            {
                return 1;
            }
            else if (key1 < key2)
            {
                return -1;
            }

            return 0;
        }

        //  uses Key()...
        //  < 0 -. ent1  < ent2
        // == 0 -. ent1 == ent2
        //  > 0 -. ent1  > ent2

        public virtual VU_KEY Key(VuEntity ent)
        {
            return (ulong)ent.Id();
        }

        // translates ent into a VU_KEY... used in Compare (above)
        // default implemetation returns id coerced to VU_KEY


        protected VuKeyFilter() { }
        protected VuKeyFilter(VuKeyFilter filter) { }
    }
}
