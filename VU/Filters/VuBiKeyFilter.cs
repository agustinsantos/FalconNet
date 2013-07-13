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
    /// <summary>
    ///  a filter that converts coordinates to grid keys. Which coordinate to use (X/Y) depends on how
    ///  the map is organized (y major, or X major). So this class abstracts the order and correct usage
    ///  is responsability of higher classes.
    /// </summary>
    public abstract class VuBiKeyFilter : VuKeyFilter
    {
        //TDODO public virtual ~VuBiKeyFilter();


        public override VU_KEY Key(VuEntity ent)
        {
            return Key2(ent);
        }

        /// <summary>
        /// translates ent into a VU_KEY... used in Compare (above)
        /// default implemetation calls Key2(ent);
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public virtual VU_KEY Key1(VuEntity ent)
        {
#if VU_GRID_TREE_Y_MAJOR
	        return CoordToKey1(ent.YPos());
#else
            return CoordToKey(ent.XPos());
#endif
        }
        public virtual VU_KEY Key2(VuEntity ent)
        {
#if VU_GRID_TREE_Y_MAJOR
	        return CoordToKey2(ent.XPos());
#else
            return CoordToKey(ent.YPos());
#endif
        }

        public abstract VU_KEY Distance1(BIG_SCALAR dist);
        public abstract VU_KEY Distance2(BIG_SCALAR dist);

        /// <summary>
        /// converts a coordinate to key.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public VU_KEY CoordToKey(BIG_SCALAR coord)
        {
            int ret = (int)(factor_ * coord);

            if (ret < 0)
            {
                ret = 0;
            }
            else if ((uint)(ret) >= res_)
            {
                ret = (int)(res_ - 1);
            }

            return (VU_KEY)ret;
        }


        protected VuBiKeyFilter(uint res, BIG_SCALAR max)
        {
            res_ = res;
            factor_ = (BIG_SCALAR)res / max;
        }
        protected VuBiKeyFilter(VuBiKeyFilter rhs)
        {
            res_ = rhs.res_;
            factor_ = rhs.factor_;
        }

        // DATA
        /// <summary>
        /// grid resolution.
        /// </summary>
        private uint res_;

        /// <summary>
        /// multiply by a real coordinate to get key.
        /// </summary>
        private float factor_;
    }
}
