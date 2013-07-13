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
    //-----------------------------------------------------------------------------
    public abstract class VuIterator
    {

        public VuIterator(VuCollection coll)
        {
            collection_ = coll;
        }

        //TODO public virtual ~VuIterator();

        public abstract VuEntity CurrEnt();
        public abstract VU_BOOL IsReferenced(VuEntity ent);
        public virtual VU_ERRCODE Cleanup()
        {
            // by default, do nothing
            return VU_ERRCODE.VU_SUCCESS;
        }


        protected VuCollection collection_;

        // for use by VuCollectionManager only!
        internal VuIterator nextiter_;
    }


}
