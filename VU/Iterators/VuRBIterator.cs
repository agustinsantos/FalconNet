using System;
using System.Collections.Generic;
using System.Linq;
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
            if (coll == null)
            {
                return;
            }

            curr_ = coll.map_.Count;
        }

        //TODO public virtual ~VuRBIterator();

        public VuEntity GetFirst()
        {
            if (collection_ == null)
            {
                return null;
            }

            VuRedBlackTree rbt = collection_ as VuRedBlackTree;
            curr_ = 0;
            currList = null;
            while ((currList == null || currList.Count == 0) && curr_ < rbt.map_.Count)
            {
                currList = rbt.map_.ElementAt(curr_).Value;
                curr_++;
                currPos = 0;
            }
            VuEntity ret = CurrEnt();

            if (ret != null && ret.VuState() != VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                return GetNext();
            }
            else
            {
                return ret;
            }
        }


        public VuEntity GetFirst(VU_KEY low)
        {
            if (collection_ == null)
            {
                return null;
            }

            VuRedBlackTree rbt =  collection_ as VuRedBlackTree;
            List<VuEntity> list;
            rbt.map_.TryGetValue(low, out list);
            if (list == null || list.Count == 0) return null;

            curr_ = rbt.map_.Count;
            rbt.map_.TryGetValue(low, out currList);
            currPos = 0;

            VuEntity ret = CurrEnt();

            if (ret != null && ret.VuState() != VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                return GetNext();
            }
            else
            {
                return ret;
            }
        }

        public VuEntity GetNext()
        {
            VuEntity ret = null;
            if (collection_ == null)
            {
                return null;
            }

            VuRedBlackTree rbt =  collection_ as VuRedBlackTree;
            if (currPos < currList.Count)
            {
                currPos++;
            }
            else
            {
                currPos = 0;
                currList = null;
                while ((currList == null || currList.Count == 0) && curr_ < rbt.map_.Count)
                {
                    currList = rbt.map_.ElementAt(curr_).Value;
                    curr_++;
                }
            }
            ret = CurrEnt();

            if (ret != null && ret.VuState() != VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                return GetNext();
            }

            return ret;
        }


        //public VuEntity GetFirst(VuFilter filter)
        //{
        //    throw new NotImplementedException();
        //}

        //public VuEntity GetNext(VuFilter filter)
        //{
        //    throw new NotImplementedException();
        //}

        public override VuEntity CurrEnt()
        {
            VuRedBlackTree rbt = collection_ as VuRedBlackTree;

            if (currList == null)
            {
                return null;
            }

            return currList[currPos];
        }

        //public override VU_BOOL IsReferenced(VuEntity ent)
        //{
        //    throw new NotImplementedException();
        //}

        public override VU_ERRCODE Cleanup()
        {
            VuRedBlackTree rbt = collection_ as VuRedBlackTree;
            curr_ = rbt.map_.Count;
            return VU_ERRCODE.VU_SUCCESS;
        }



        //public void RemoveCurrent()
        //{
        //    throw new NotImplementedException();
        //}


        protected VuRBIterator(VuCollection coll)
            : base(coll)
        {
            if (coll == null)
            {
                return;
            }
        }


        protected int curr_;
        protected List<VuEntity> currList;
        protected int currPos;

    }
}
