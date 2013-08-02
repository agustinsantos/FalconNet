using System;
using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{
    public class VuHashIterator : VuIterator
    {
        public VuHashIterator(VuHashTable coll)
            : base(coll)
        {
            idx_ = coll.capacity_;
            curr_ = null;
#if NOTHING
    curr_ = vuTailNode;
    entry_ = 0;
#endif

            // vuCollectionManager->Register(this);
        }


        //TODO public virtual ~VuHashIterator();

        public VuEntity GetFirst()
        {
            {
                VuHashTable h = (VuHashTable)collection_;

                if (h == null || h.capacity_ <= 0)
                {
                    return null;
                }

                idx_ = 0;
                VuEntity ret;

                do
                {
                    curr_ = new VuListIterator(h.table_[idx_]);
                    ret = curr_.GetFirst();
                }
                while (ret == null && ++idx_ < h.capacity_);

                return ret;

#if NOTHING

    if (collection_)
    {
        entry_ = ((VuHashTable*)collection_)->table_;

        while (*entry_ == vuTailNode)
        {
            entry_++;
        }

        if (*entry_ == 0)
        {
            curr_ = vuTailNode;
        }
        else
        {
            curr_ = *entry_;
        }

        // sfr: smartpointer
        return curr_->entity_.get();
    }

    return 0;
#endif
            }
        }

        public VuEntity GetNext()
        {
            VuHashTable h = (VuHashTable)collection_;
            VuEntity ret;

            do
            {
                // try next
                ret = curr_.GetNext();

                while (ret == null && ++idx_ < h.capacity_)
                {
                    // here we couldnt find a valid next, so try next entry until we find a valid one
                    curr_ = new VuListIterator(h.table_[idx_]);
                    ret = curr_.GetFirst();
                }
            }
            while (ret == null && idx_ < h.capacity_);

            return ret;

#if NOTHING
    curr_ = curr_->next_;

    if (curr_ != vuTailNode)
    {
        // sfr: smartpointer
        return curr_->entity_.get();
    }

    entry_++;

    while (*entry_ == vuTailNode)
    {
        entry_++;
    }

    if (*entry_ == 0)
    {
        curr_ = vuTailNode;
    }
    else
    {
        curr_ = *entry_;
    }

    // sfr: smartpointer
    return curr_->entity_.get();
#endif
        }

        public VuEntity GetFirst(VuFilter filter)
        {
            if (filter == null)
            {
                return GetFirst();
            }

            VuEntity ret = GetFirst();

            if (ret == null)
            {
                return null;
            }

            if (filter.Test(ret))
            {
                return ret;
            }
            else
            {
                return GetNext(filter);
            }

#if NOTHING
        // sfr: smartpointer
        GetFirst();

        if (filter)
        {
            if (curr_->entity_.get() == 0)
            {
                return curr_->entity_.get();
            }

            if (filter->Test(curr_->entity_.get()))
            {
                return curr_->entity_.get();
            }

            return GetNext(filter);
        }

        return curr_->entity_.get();
#endif
        }

        public VuEntity GetNext(VuFilter filter)
        {
            if (filter == null)
            {
                return GetNext();
            }

            VuEntity ret;

            do
            {
                ret = GetNext();

                if (ret == null || filter.Test(ret))
                {
                    return ret;
                }
            }
            while (true);

#if NOTHING
            // sfr: smartpointer
            GetNext();

            if (filter)
            {
                if (curr_->entity_.get() == 0)
                {
                    return curr_->entity_.get();
                }

                if (filter->Test(curr_->entity_.get()))
                {
                    return curr_->entity_.get();
                }

                return GetNext(filter);
            }

            return curr_->entity_.get();
#endif
        }

        public override VuEntity CurrEnt()
        {
            return curr_.CurrEnt();
#if NOTHING
            // sfr: smartpointer
            return curr_->entity_.get();
#endif
        }


        //public override VU_BOOL IsReferenced(VuEntity ent)
        //{
        //    // 2002-02-04 MODIFIED BY S.G. If ent is false, then it can't be a valid entity, right? That's what I think too :-)
        //    //	if (curr_.entity_ == ent)
        //    if (ent != null && curr_.entity_ == ent)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public override VU_ERRCODE Cleanup()
        {
#if NOTHING
    curr_ = vuTailNode;
#endif
            return VU_ERRCODE.VU_SUCCESS;
        }

        protected int idx_;
        protected VuListIterator curr_;
    }


}
