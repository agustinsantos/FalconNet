using BIG_SCALAR = System.Single;
using VU_KEY = System.UInt64;

namespace FalconNet.VU
{
    public class VuGridIterator : VuIterator
    {

        public VuGridIterator(VuGridTree coll, BIG_SCALAR xPos, BIG_SCALAR yPos, BIG_SCALAR radius)
            : base(coll)
        {
            p1_ = xPos;
            p2_ = yPos;
            radius_ = radius;
            it_ = null;

            // sfr: temp test
            //static int temp = 1;
            //radius *= temp;

            // low and up row (inclusive)
#if VU_ALL_FILTERED
    VuBiKeyFilter *bkf = coll->GetBiKeyFilter();
#else
            VuBiKeyFilter bkf = coll.filter_ as VuBiKeyFilter;
#endif
            rowlow_ = bkf.CoordToKey(xPos - radius);
            rowhi_ = bkf.CoordToKey(xPos + radius);
            // low and up coll
            collow_ = bkf.CoordToKey(yPos - radius);
            colhi_ = bkf.CoordToKey(yPos + radius);

        }

        //TODO public virtual ~VuGridIterator();

        public VuEntity GetFirst()
        {

            if (collection_ == null)
            {
                return null;
            }

            VuGridTree g = collection_ as VuGridTree;
#if VU_ALL_FILTERED
    VuBiKeyFilter *bkf = g->GetBiKeyFilter();
#else
            VuBiKeyFilter bkf = g.filter_ as VuBiKeyFilter;
#endif
            rowcur_ = rowlow_;
            it_ = new VuRBIterator(g.table_[rowcur_]);
            VuEntity ret = it_.GetFirst(collow_);

            if ((ret == null) || (bkf.Key2(ret) > colhi_))
            {
                ret = GetNext();
            }

            return ret;
        }

        public VuEntity GetNext()
        {
            VuGridTree g = collection_ as VuGridTree;
#if VU_ALL_FILTERED
    VuBiKeyFilter *bkf = g->GetBiKeyFilter();
#else
            VuBiKeyFilter bkf = g.filter_ as VuBiKeyFilter;
#endif
            VuEntity ret = it_.GetNext(); ;

            while ((ret == null) || (bkf.Key2(ret) > colhi_))
            {
                // end of column
                if (rowcur_ + 1 > rowhi_)
                {
                    // last row... end
                    return null;
                }
                else
                {
                    // next row
                    it_ = new VuRBIterator(g.table_[++rowcur_]);
                    ret = it_.GetFirst(collow_);
                }
            }

            return ret;
        }

        public VuEntity GetFirst(VuFilter filter)
        {
            VuEntity retval = GetFirst();

            if (retval == null || filter.Test(retval))
            {
                return retval;
            }

            return GetNext(filter);
        }

        public VuEntity GetNext(VuFilter filter)
        {
            VuEntity retval = null;

            while ((retval = GetNext()) != null)
            {
                if (filter.Test(retval))
                {
                    break;
                }
            }

            return retval;
        }

        public override VuEntity CurrEnt()
        {
            return it_.CurrEnt();
        }


        /** tree row interval, open at end. */
        protected VU_KEY rowlow_, rowhi_, rowcur_;
        /** tree column intervals, open at end. */
        protected VU_KEY collow_, colhi_, colcur_;
        /** origin, radius and radius squared of search in real units. */
        protected BIG_SCALAR p1_, p2_, radius_, radius_2_;
        /** iterator on trees. */
        protected VuRBIterator it_;
    }
}
