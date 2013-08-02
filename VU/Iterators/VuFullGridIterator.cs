
using System;
namespace FalconNet.VU
{
    public class VuFullGridIterator : VuRBIterator
    {

        public VuFullGridIterator(VuGridTree coll)
            : base(coll)
        {
            curRB_ = 0;
            table_ = null;
            currow_ = 0;
            // empty
        }
        //TODO public virtual ~VuFullGridIterator();

        // note: these implementations HIDE the RBIterator methods, which
        //       is intended, but some compilers will flag this as a warning
        public new VuEntity GetFirst()
        {
#if TODO
		            if (collection_ != null)
            {
                currow_ = 0;
                curlink_ = VuTailNode.vuTailNode;
                curRB_ = 0;
                table_ = ((VuGridTree)collection_).table_;
                curnode_ = table_[curRB_].root_;

                if (curnode_ != null)
                {
                    curnode_ = curnode_.TreeMinimum();
                    curlink_ = curnode_.head_;

                    return curlink_.entity_;
                }

                return GetNext();
            }

            return null;  
#endif
            throw new NotImplementedException();
        }

        public new VuEntity GetNext()
        {
#if TODO
		            while ((curnode_ == null) && (++currow_ < ((VuGridTree)collection_).rowcount_))
            {
                curRB_++;
                curnode_ = table_[curRB_].root_;

                if (curnode_ != null)
                {
                    curnode_ = curnode_.TreeMinimum();
                    curlink_ = curnode_.head_;

                    return curlink_.entity_;
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

                if (curnode_ == null)
                {
                    // skip to next row
                    curlink_ = VuTailNode.vuTailNode;
                    curnode_ = null;

                    return GetNext();
                }

                curlink_ = curnode_.head_;
            }

            return curlink_.entity_;  
#endif
            throw new NotImplementedException();
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
        protected int currow_;
    }


}
