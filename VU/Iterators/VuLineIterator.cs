using System;
using BIG_SCALAR = System.Single;
using VU_KEY = System.UInt64;

namespace FalconNet.VU
{
    //-----------------------------------------------------------------------------
    class VuLineIterator : VuRBIterator
    {
        public VuLineIterator(VuGridTree coll, VuEntity origin, VuEntity destination, BIG_SCALAR radius)
            : base(coll)
        {
            curRB_ = null;
            key1min_ = 0;
            key1max_ = UInt64.MaxValue;
            key2min_ = 0;
            key2max_ = UInt64.MaxValue;
            key1cur_ = 0;

            VU_KEY key1origin = coll.filter_.Key1(origin);
            VU_KEY key2origin = coll.filter_.Key2(origin);
            //	VU_KEY key1destination = coll.filter_.Key1(destination);
            //	VU_KEY key2destination = coll.filter_.Key2(destination);
            VU_KEY key1radius = coll.filter_.Distance1(radius);
            VU_KEY key2radius = coll.filter_.Distance2(radius);
            lineA_ = 1;
            lineB_ = 1;
            lineC_ = 1;

            if (key1origin > key1radius)
            {
                key1min_ = key1origin - key1radius;
            }

            if (key2origin > key2radius)
            {
                key2min_ = key2origin - key2radius;
            }

            key1max_ = key1origin + key1radius;
            key2max_ = key2origin + key2radius;

            VuGridTree gt = (VuGridTree)collection_;

            if (!gt.wrap_)
            {
                if (key1min_ < gt.bottom_)
                {
                    key1min_ = gt.bottom_ + ((gt.bottom_ - key1min_) % gt.rowheight_);
                }

                if (key1max_ > gt.top_)
                {
                    key1max_ = gt.top_;
                }
            }
        }

        public VuLineIterator(VuGridTree coll,
              BIG_SCALAR xPos0, BIG_SCALAR yPos0,
              BIG_SCALAR xPos1, BIG_SCALAR yPos1, BIG_SCALAR radius)
            : base(coll)
        {
            curRB_ = null;
            key1min_ = 0;
            key1max_ = UInt64.MaxValue;
            key2min_ = 0;
            key2max_ = UInt64.MaxValue;
            key1cur_ = 0;
            VU_KEY key1origin = coll.filter_.CoordToKey(xPos0);
            VU_KEY key2origin = coll.filter_.CoordToKey(yPos0);
            //	VU_KEY key1destination = coll.filter_.CoordToKey1(xPos1);
            //	VU_KEY key2destination = coll.filter_.CoordToKey2(yPos1);
            VU_KEY key1radius = coll.filter_.Distance1(radius);
            VU_KEY key2radius = coll.filter_.Distance2(radius);
            lineA_ = 1;
            lineB_ = 1;
            lineC_ = 1;

            if (key1origin > key1radius)
            {
                key1min_ = key1origin - key1radius;
            }

            if (key2origin > key2radius)
            {
                key2min_ = key2origin - key2radius;
            }

            key1max_ = key1origin + key1radius;
            key2max_ = key2origin + key2radius;

            VuGridTree gt = (VuGridTree)collection_;

            if (!gt.wrap_)
            {
                if (key1min_ < gt.bottom_)
                {
                    key1min_ = gt.bottom_ + ((gt.bottom_ - key1min_) % gt.rowheight_);
                }

                if (key1max_ > gt.top_)
                {
                    key1max_ = gt.top_;
                }
            }
        }
        //TODO public virtual ~VuLineIterator();

        // note: these implementations HIDE the RBIterator methods, which
        //       is intended, but some compilers will flag this as a warning
        public new VuEntity GetFirst()
        {
            if (collection_ != null)
            {
                curlink_ = VuTailNode.vuTailNode;
                key1cur_ = key1min_;
                curRB_ = ((VuGridTree)collection_).Row(key1cur_);
                curnode_ = curRB_.root_;

                if (curnode_ != null)
                {
                    curnode_ = curnode_.LowerBound(key2min_);
                    if (curnode_ != null && curnode_.head_ != null && curnode_.key_ < key2max_)
                    {
                        curlink_ = curnode_.head_;

                        return curlink_.entity_;
                    }
                }

                return GetNext();
            }

            return null;
        }

        public new VuEntity GetNext()
        {
            while (curnode_ == null && key1cur_ < key1max_)
            {
                // danm_TBD: what about non-wrapping edges?
                key1cur_ += ((VuGridTree)collection_).rowheight_;
                curRB_ = ((VuGridTree)collection_).Row(key1cur_);
                curnode_ = curRB_.root_;

                if (curnode_ != null)
                {
                    curnode_ = curnode_.LowerBound(key2min_);

                    if (curnode_ != null && curnode_.head_ != null && curnode_.key_ < key2max_)
                    {
                        curlink_ = curnode_.head_;

                        return curlink_.entity_;
                    }
                }
            }

            if (curnode_ == null)
            {
                return null;
            }

            curlink_ = curlink_.next_;
            if (curlink_ == VuTailNode.vuTailNode)
            {
                curnode_ = curnode_.next_;
                if (curnode_ == null || curnode_.key_ > key2max_)
                {
                    // skip to next row
                    curlink_ = VuTailNode.vuTailNode;
                    curnode_ = null;

                    return GetNext();
                }

                curlink_ = curnode_.head_;
            }

            return curlink_.entity_;
        }

        public new VuEntity GetFirst(VuFilter filter)
        {
            VuEntity retval = GetFirst();

            if (retval == null || filter.Test(retval))
            {
                return retval;
            }

            return GetNext(filter);
        }

        public new VuEntity GetNext(VuFilter filter)
        {
            VuEntity retval = null;

            while ((retval = GetNext()) != null)
            {
                if (filter.Test(retval))
                {
                    return retval;
                }
            }

            return retval;
        }

        protected VuRedBlackTree curRB_;
        protected VU_KEY key1min_;
        protected VU_KEY key1max_;
        protected VU_KEY key2min_;
        protected VU_KEY key2max_;
        protected VU_KEY key1cur_;
        protected VU_KEY lineA_;
        protected VU_KEY lineB_;
        protected VU_KEY lineC_;
    }

}
