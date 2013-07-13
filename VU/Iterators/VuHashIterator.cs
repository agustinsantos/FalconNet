using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{
    public class VuHashIterator : VuIterator
    {
        public VuHashIterator(VuHashTable coll)
            : base(coll)
        {
            curr_ = VuTailNode.vuTailNode;
            entry_ = 0;

            VUSTATIC.vuCollectionManager.Register(this);
        }
        //TODO public virtual ~VuHashIterator();

        public VuEntity GetFirst()
        {
            if (collection_ != null)
            {
                table_ = ((VuHashTable)collection_).table_;
                entry_ = 0;

                while (table_[entry_] == VuTailNode.vuTailNode)
                {
                    entry_++;
                }

                if (table_[entry_] == null)
                {
                    curr_ = VuTailNode.vuTailNode;
                }
                else
                {
                    curr_ = table_[entry_];
                }

                return curr_.entity_;
            }

            return null;
        }

        public VuEntity GetNext()
        {
            curr_ = curr_.next_;

            if (curr_ != VuTailNode.vuTailNode)
            {
                return curr_.entity_;
            }

            entry_++;

            while (table_[entry_] == VuTailNode.vuTailNode)
            {
                entry_++;
            }

            if (table_[entry_] == null)
            {
                curr_ = VuTailNode.vuTailNode;
            }
            else
            {
                curr_ = table_[entry_];
            }

            return curr_.entity_;
        }

        public VuEntity GetFirst(VuFilter filter)
        {
            GetFirst();

            if (filter != null)
            {
                if (curr_.entity_ == null)
                {
                    return curr_.entity_;
                }

                if (filter.Test(curr_.entity_))
                {
                    return curr_.entity_;
                }

                return GetNext(filter);
            }

            return curr_.entity_;
        }
        public VuEntity GetNext(VuFilter filter)
        {
            GetNext();

            if (filter != null)
            {
                if (curr_.entity_ == null)
                {
                    return curr_.entity_;
                }

                if (filter.Test(curr_.entity_))
                {
                    return curr_.entity_;
                }

                return GetNext(filter);
            }

            return curr_.entity_;
        }

        public override VuEntity CurrEnt()
        {
            return curr_.entity_;
        }


        public override VU_BOOL IsReferenced(VuEntity ent)
        {
            // 2002-02-04 MODIFIED BY S.G. If ent is false, then it can't be a valid entity, right? That's what I think too :-)
            //	if (curr_.entity_ == ent)
            if (ent != null && curr_.entity_ == ent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override VU_ERRCODE Cleanup()
        {
            curr_ = VuTailNode.vuTailNode;

            return VU_ERRCODE.VU_SUCCESS;
        }



        internal VuLinkNode[] table_;
        internal int entry_;
        internal VuLinkNode curr_;
    }


}
