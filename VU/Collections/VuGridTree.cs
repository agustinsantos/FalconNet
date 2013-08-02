using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BIG_SCALAR = System.Single;
using SM_SCALAR = System.Single;
using VU_BOOL = System.Boolean;
using VU_KEY = System.UInt64;
using VU_TIME = System.UInt64;
using VuMutex = System.Object;

namespace FalconNet.VU
{

    public class VuGridTree : VuCollection
    {
        public VuGridTree(VuBiKeyFilter filter, uint res) :
            base(filter)
        {
            res_ = res;
            suspendUpdates_ = false;
            //TODO Delete tgextgrid_ = null;
            table_ = new VuRedBlackTree[res_];

            for (int i = 0; i < res_; ++i)
            {
                table_[i] = new VuRedBlackTree(filter);
            }

            VUSTATIC.vuCollectionManager.GridRegister(this);
        }


        /** called by collection manager to handle an entity move. */
        internal VU_ERRCODE Move(VuEntity ent, BIG_SCALAR coord1, BIG_SCALAR coord2)
        {
            lock (GetMutex())
            {
                VuBiKeyFilter bkf = GetBiKeyFilter();

                if ((ent != null) && (ent.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE) && bkf.RemoveTest(ent))
                {
                    VuEntity safe = ent;
                    VU_KEY ck1 = bkf.Key1(ent);
                    VU_KEY nk1 = bkf.CoordToKey(coord1);
                    VU_KEY ck2 = bkf.Key2(ent);
                    VU_KEY nk2 = bkf.CoordToKey(coord2);

                    if (ck1 != nk1 || ck2 != nk2)
                    {
                        // keys changed... have to remove and insert again
                        table_[ck1].Remove(ent);
                        table_[nk1].Insert(ent);
                    }

                    return VU_ERRCODE.VU_SUCCESS;
                }

                return VU_ERRCODE.VU_NO_OP;
            }
        }

        /** stops updating tree when entities moves. */
        private void SuspendUpdates()
        {
            suspendUpdates_ = true;
        }
        /** resumes updating tree when entities move. */
        private void ResumeUpdates()
        {
            suspendUpdates_ = false;
        }
        /** returns the filter casted correctly. */
        private VuBiKeyFilter GetBiKeyFilter()
        {
            return GetFilter() as VuBiKeyFilter;
 
        }


        // protected interface
        protected override VU_ERRCODE PrivateInsert(VuEntity entity)
        {
            VuBiKeyFilter bkf = GetBiKeyFilter();
            VuRedBlackTree row = table_[bkf.Key1(entity)];
            return row.ForcedInsert(entity);
        }

        protected override VU_ERRCODE PrivateRemove(VuEntity entity)
        {
            VuBiKeyFilter bkf = GetBiKeyFilter();
            VuRedBlackTree row = table_[bkf.Key1(entity)];
            VU_ERRCODE res = row.Remove(entity);
            return res;
        }

        protected override bool PrivateFind(VuEntity entity)
        {
            VuBiKeyFilter bkf = GetBiKeyFilter();
            VuRedBlackTree row = table_[bkf.Key1(entity)];
            return row.Find(entity);
        }

        // public interface
        public override int Purge(VU_BOOL all = true)
        {
            int retval = 0;
            lock (GetMutex())
            {
                for (int i = 0; i < res_; i++)
                {
                    retval += table_[i].Purge(all);
                }

                return retval;
            }
        }

        public override int Count()
        {
            lock (GetMutex())
            {
                int count = 0;

                for (int i = 0; i < res_; i++)
                {
                    count += table_[i].Count();
                }
                return count;
            }
        }

        public override VU_COLL_TYPE Type()
        {
            return VU_COLL_TYPE.VU_GRID_TREE_COLLECTION;
        }

        // DATA

        internal protected VuRedBlackTree[] table_;
        protected uint res_; ///< grid resolution (res x res)
        internal protected VU_BOOL suspendUpdates_;

        //TODO deleteme private VuGridTree nextgrid_;
    }


}
