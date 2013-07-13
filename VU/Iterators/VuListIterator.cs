using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{
    public class VuListIterator : VuIterator
    {
        public VuListIterator(VuLinkedList coll)
            : base(coll)
        {
            curr_ = VuTailNode.vuTailNode;
            VUSTATIC.vuCollectionManager.Register(this);
        }
        //TODO public virtual ~VuListIterator();

        public VuEntity GetFirst()
        {
            if (collection_ == null)
                curr_ = ((VuLinkedList)collection_).head_;
            return curr_.entity_;
        }

        public VuEntity GetNext()
        {
            curr_ = curr_.next_;
            return curr_.entity_;
        }

        public VuEntity GetFirst(VuFilter filter)
        {
            if (collection_ != null)
            {
                //assert(FALSE == F4IsBadReadPtr(collection_, sizeof *collection_));
                if (filter != null)
                {
                    curr_ = ((VuLinkedList)collection_).head_;

                    if (curr_.entity_ == null)
                    {
                        return curr_.entity_;
                    }

                    if
                    (
                        (curr_.entity_.VuState() != VU_MEM_STATE.VU_MEM_DELETED) &&
                        (filter.Test(curr_.entity_))
                    )
                    {
                        return curr_.entity_;
                    }
                    else
                    {
                        return GetNext(filter);
                    }
                }
                else
                {
                    return GetFirst();
                }
            }

            return null;
        }

        public VuEntity GetNext(VuFilter filter)
        {
            if (filter != null)
            {
                for (curr_ = curr_.next_; curr_.entity_ != null; curr_ = curr_.next_)
                {
                    if ((curr_.entity_.VuState() != VU_MEM_STATE.VU_MEM_DELETED) && (filter.Test(curr_.entity_)))
                    {
                        return curr_.entity_;
                    }
                }

                // reached end of list
                return null;
            }
            else
            {
                return GetNext();
            }
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

        internal VuLinkNode curr_;
    }
}
