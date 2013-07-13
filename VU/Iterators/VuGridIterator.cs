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
    public class VuGridIterator : VuRBIterator
    {

        public VuGridIterator(VuGridTree coll, VuEntity origin, BIG_SCALAR radius)
            : base(coll)
        {
            curRB_ = 0;
            table_ = null;
            key1min_ = 0;
            key1max_ = UInt64.MaxValue;
            key2min_ = 0;
            key2max_ = UInt64.MaxValue;

            CriticalSection.VuEnterCriticalSection(); // JB 010719

            VU_KEY key1origin = coll.filter_.Key1(origin);
            VU_KEY key2origin = coll.filter_.Key2(origin);
            VU_KEY key1radius = coll.filter_.Distance1(radius);
            VU_KEY key2radius = coll.filter_.Distance2(radius);
            key1minRB_ = coll.table_[0];
            key1maxRB_ = coll.table_[coll.rowcount_];

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

            CriticalSection.VuExitCriticalSection(); // JB 010719
        }

        public VuGridIterator(VuGridTree coll,
              BIG_SCALAR xPos, BIG_SCALAR yPos, BIG_SCALAR radius)
            : base(coll)
        {

            curRB_ = 0;
            table_ = null;
            key1min_ = 0;
            key1max_ = UInt64.MaxValue;
            key2min_ = 0;
            key2max_ = UInt64.MaxValue;

            CriticalSection.VuEnterCriticalSection(); // JB 010719

            VU_KEY key1origin = coll.filter_.CoordToKey(xPos);
            VU_KEY key2origin = coll.filter_.CoordToKey(yPos);
            VU_KEY key1radius = coll.filter_.Distance1(radius);
            VU_KEY key2radius = coll.filter_.Distance2(radius);
            key1minRB_ = coll.table_[0];
            key1maxRB_ = coll.table_[coll.rowcount_];

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

            CriticalSection.VuExitCriticalSection(); // JB 010719
        }
        //TODO public virtual ~VuGridIterator();

        // note: these implementations HIDE the RBIterator methods, which
        //       is intended, but some compilers will flag this as a warning
        public new VuEntity GetFirst()
        {
            if (collection_ != null)
            {
                curlink_ = VuTailNode.vuTailNode;
                key1cur_ = key1min_;

                CriticalSection.VuEnterCriticalSection(); // JPO lock access
                curRB_ = ((VuGridTree)collection_).RowIndex(key1cur_);
                table_ = ((VuGridTree)collection_).table_;
                curnode_ = table_[curRB_].root_;

                if (curnode_ != null)
                {
                    curnode_ = curnode_.LowerBound(key2min_);

                    if (curnode_ != null && curnode_.head_ != null && curnode_.key_ < key2max_)
                    {
                        curlink_ = curnode_.head_;
                        CriticalSection.VuExitCriticalSection();
                        return curlink_.entity_;
                    }
                }

                VuEntity res = GetNext();
                CriticalSection.VuExitCriticalSection();
                return res;

            }

            return null;
        }
        public new VuEntity GetNext()
        {
            CriticalSection.VuEnterCriticalSection(); // JPO lock access
            while (curnode_ == null && key1cur_ < key1max_)
            {
                // danm_TBD: what about non-wrapping edges?
                key1cur_ += ((VuGridTree)collection_).rowheight_;
                curRB_++;
                if (table_[curRB_] == key1maxRB_) curRB_ = 0;

                //if (!F4IsBadReadPtr(curRB_, sizeof(VuRedBlackTree))) // JB 011205 (too much CPU) // JB 010318 CTD 
                if (curRB_ != 0) // JB 011205
                    curnode_ = table_[curRB_].root_;
                else
                    curnode_ = null; // JB 010318 CTD

                if (curnode_ != null)
                {
                    curnode_ = curnode_.LowerBound(key2min_);

                    if (curnode_ != null && curnode_.head_ != null && curnode_.key_ < key2max_)
                    {
                        curlink_ = curnode_.head_;

                        CriticalSection.VuExitCriticalSection();
                        //if (curlink_) // JB 010222 CTD
                        if (curlink_ != null)// && !F4IsBadReadPtr(curlink_, sizeof(VuLinkNode))) // JB 010404 CTD
                            return curlink_.entity_;
                        else
                            return null; // JB 010222 CTD
                    }
                }
            }

            if (curnode_ == null)
            {
                CriticalSection.VuExitCriticalSection();
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
                    CriticalSection.VuExitCriticalSection();

                    return GetNext();
                }

                curlink_ = curnode_.head_;
            }
            CriticalSection.VuExitCriticalSection();

            //if (curlink_) // JB 010222 CTD
            if (curlink_ != null)// && !F4IsBadReadPtr(curlink_, sizeof(VuLinkNode))) // JB 010404 CTD
                return curlink_.entity_;
            else
                return null; // JB 010222 CTD
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


        protected VuRedBlackTree[] table_;
        protected int curRB_;
        protected VuRedBlackTree key1minRB_;
        protected VuRedBlackTree key1maxRB_;
        protected VU_KEY key1min_;
        protected VU_KEY key1max_;
        protected VU_KEY key2min_;
        protected VU_KEY key2max_;
        protected VU_KEY key1cur_;
    }
}
