using System.Collections.Generic;
using VU_BOOL = System.Boolean;
using System;

namespace FalconNet.VU
{
    public class VuListIterator : VuIterator
    {
        public VuListIterator(VuLinkedList coll)
            : base(coll)
        {
            if (coll != null)
            {
                curr_ = coll.l_.Count - 1;
            }
        }
        //TODO public virtual ~VuListIterator();
        public void RemoveCurrent()
        {
            List<VuEntity> bl = ((VuLinkedList)collection_).l_;
            if (curr_ >= bl.Count)
            {
                return;
            }

            bl.RemoveAt(curr_);
        }

        public VuEntity GetFirst()
        {
            if (collection_ == null)
            {
                return null;
            }

            List<VuEntity> bl = ((VuLinkedList)collection_).l_;

            if (bl.Count == 0)
            {
                return null;
            }

            curr_ = 0;
            VuEntity eb = bl[curr_];

            if (eb.VuState() != VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                return GetNext();
            }
            else
            {
                return eb;
            }
        }

        public VuEntity GetNext()
        {
            List<VuEntity> bl = ((VuLinkedList)collection_).l_;

            do
            {
                if (++curr_ == bl.Count)
                {
                    return null;
                }
                else if (bl[curr_].VuState() != VU_MEM_STATE.VU_MEM_ACTIVE)
                {
                    continue;
                }
                else
                {
                    return bl[curr_];
                }
            }
            while (true);
        }

        public VuEntity GetFirst(VuFilter filter)
        {
            if (collection_ == null)
            {
                return null;
            }

            VuEntity e = GetFirst();

            if (e == null)
            {
                return null;
            }
            else if ((filter == null) || filter.Test(e))
            {
                return e;
            }
            else
            {
                return GetNext(filter);
            }
        }

        public VuEntity GetNext(VuFilter filter)
        {
            VuEntity e;

            do
            {
                e = GetNext();

                if (filter == null || filter.Test(e))
                {
                    return e;
                }
            }
            while (e != null);

            return null;
        }

        public override VuEntity CurrEnt()
        {
            return ((VuLinkedList)collection_).l_[curr_];
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
            if (collection_ != null)
            {
                curr_ = ((VuLinkedList)collection_).l_.Count - 1;
            }

            return VU_ERRCODE.VU_SUCCESS;
        }

        protected int curr_;
    }
}
