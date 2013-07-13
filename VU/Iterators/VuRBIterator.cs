using VU_BOOL = System.Boolean;
using VU_KEY = System.UInt64;

namespace FalconNet.VU
{
    //-----------------------------------------------------------------------------
    public class VuRBIterator : VuIterator
    {

        public VuRBIterator(VuRedBlackTree coll)
            : base(coll)
        {
            curnode_ = null;
            curlink_ = VuTailNode.vuTailNode;
            rbnextiter_ = null;

            VUSTATIC.vuCollectionManager.Register(this);
            VUSTATIC.vuCollectionManager.RBRegister(this);
        }
        //TODO public virtual ~VuRBIterator();

        public VuEntity GetFirst()
        {
            if (collection_ != null)
            {
                curnode_ = ((VuRedBlackTree)collection_).root_;

                if (curnode_ != null)
                {
                    curnode_ = curnode_.TreeMinimum();
                    curlink_ = curnode_.head_;
                }
                else
                {
                    curlink_ = VuTailNode.vuTailNode;
                }

                return curlink_.entity_;
            }

            return null;
        }
        public VuEntity GetFirst(VU_KEY min)
        {
            if (collection_ != null)
            {
                curnode_ = ((VuRedBlackTree)collection_).root_;

                if (curnode_ != null)
                {
                    curnode_ = curnode_.LowerBound(min);
                    curlink_ = curnode_.head_;
                }
                else
                {
                    curlink_ = VuTailNode.vuTailNode;
                }

                return curlink_.entity_;
            }

            return null;
        }

        public VuEntity GetNext()
        {
            if (curnode_ != null)
            {
                curlink_ = curlink_.next_;

                if (curlink_ == VuTailNode.vuTailNode)
                {
                    curnode_ = curnode_.next_;

                    if (curnode_ != null)
                    {
                        curlink_ = curnode_.head_;
                    }
                }

                return curlink_.entity_;
            }

            return null;
        }
        public VuEntity GetFirst(VuFilter filter)
        {
            if (collection_ != null)
            {
                VuEntity retval = GetFirst();

                if ((retval == null) || (filter.Test(retval)))
                {
                    return retval;
                }

                return GetNext(filter);
            }

            return null;
        }

        public VuEntity GetNext(VuFilter filter)
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

        public override VuEntity CurrEnt()
        {
            return curlink_.entity_;
        }


        public override VU_BOOL IsReferenced(VuEntity ent)
        {
            // 2002-02-04 MODIFIED BY S.G. If ent is false, then it can't be a valid entity, right? That's what I think too :-)
            //	if (curlink_.entity_ == ent)
            if (ent != null && curlink_.entity_ == ent)
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
            curnode_ = null;
            curlink_ = VuTailNode.vuTailNode;

            return VU_ERRCODE.VU_SUCCESS;
        }

        public void RemoveCurrent()
        {
            if (curnode_ != null && curlink_.entity_ != null && curlink_.freenext_ == null)
            {
                CriticalSection.VuEnterCriticalSection();

                if (curnode_.head_ == curlink_)
                {
                    curnode_.head_ = curlink_.next_;

                    if (curnode_.head_ == VuTailNode.vuTailNode)
                    {
                        ((VuRedBlackTree)collection_).RemoveNode(curnode_);
                    }
                }
                else
                {
                    VuLinkNode last = curnode_.head_;

                    while (last.next_ != curlink_)
                    {
                        last = last.next_;
                    }

                    last.next_ = curlink_.next_;
                }

                // put curr on VUs pending delete queue
                VUSTATIC.vuCollectionManager.PutOnKillQueue(curlink_);
                CriticalSection.VuExitCriticalSection();
            }
        }

        protected VuRBIterator(VuCollection coll)
            : base(coll)
        {
            curnode_ = null;
            curlink_ = VuTailNode.vuTailNode;
            rbnextiter_ = null;

            VUSTATIC.vuCollectionManager.Register(this);
            VUSTATIC.vuCollectionManager.RBRegister(this);
        }


        internal VuRBNode curnode_;
        internal VuLinkNode curlink_;
        internal VuRBIterator rbnextiter_;
    }
}
