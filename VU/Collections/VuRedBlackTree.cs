using System.Collections.Generic;
using VU_BOOL = System.Boolean;
using VU_KEY = System.UInt64;
using VU_TIME = System.UInt64;

namespace FalconNet.VU
{
    public class VuRedBlackTree : VuCollection
    {
        public VuRedBlackTree(VuKeyFilter filter = null)
            : base(filter)
        {
        }

        //TODO virtual ~VuRedBlackTree();

        /** purges tree. */
        public override int Purge(VU_BOOL all = true)
        {
            int ret = 0;

            foreach (List<VuEntity> list in map_.Values)
            {
                if (list == null || list.Count == 0)
                    continue;

                for (int i = list.Count; i >= 0; i--)
                {
                    if (all || (!(list[i].IsPrivate() && list[i].IsPersistent()) && !list[i].IsGlobal()))
                    {
                        list.RemoveAt(i);
                        ++ret;
                    }
                }
            }

            return ret;
        }

        /** return number of active entities. */
        public override int Count()
        {
            int count = 0;

            foreach (List<VuEntity> list in map_.Values)
            {
                if (list == null || list.Count == 0)
                    continue;

                for (int i = list.Count; i >= 0; i--)
                {
                    if (list[i].VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
                    {
                        ++count;
                    }
                }
            }
            return count;
        }

        public override VU_COLL_TYPE Type()
        {
            return VU_COLL_TYPE.VU_RED_BLACK_TREE_COLLECTION;
        }


        protected override VU_ERRCODE PrivateInsert(VuEntity entity)
        {
            VuKeyFilter kf = GetKeyFilter();
            VU_KEY key = kf.Key(entity);
            List<VuEntity> list;
            map_.TryGetValue(key, out list);
            if (list == null)
            {
                list = new List<VuEntity>();
                map_.Add(key, list);
            }

            list.Add(entity);
            return VU_ERRCODE.VU_SUCCESS;
        }

        protected override VU_ERRCODE PrivateRemove(VuEntity entity)
        {
            VuKeyFilter kf = GetKeyFilter();
            VU_KEY k = kf.Key(entity);
            VU_KEY key = kf.Key(entity);
            List<VuEntity> list;
            map_.TryGetValue(key, out list);
            if (list == null || list.Count == 0)
            {
                return VU_ERRCODE.VU_NO_OP;
            }
            for (int i = list.Count; i >= 0; i--)
            {
                if (list[i] == entity)
                {
                    list.RemoveAt(i);
                    return VU_ERRCODE.VU_SUCCESS;
                }
            }

            return VU_ERRCODE.VU_NO_OP;
        }

        protected override VU_BOOL PrivateFind(VuEntity entity)
        {
            VuKeyFilter kf = GetKeyFilter();
            VU_KEY k = kf.Key(entity);
            VU_KEY key = kf.Key(entity);
            List<VuEntity> list;
            map_.TryGetValue(key, out list);
            if (list == null || list.Count == 0)
            {
                return false;
            }
            for (int i = list.Count; i >= 0; i--)
            {
                if (list[i].VuState() == VU_MEM_STATE.VU_MEM_ACTIVE && list[i] == entity)
                {
                    return true;
                }
            }

            // can happen if all entities with given key are inactive
            return false;
        }

        private VuKeyFilter GetKeyFilter()
        {
            return GetFilter() as VuBiKeyFilter;
        }


        // DATA
        internal Dictionary<VU_KEY, List<VuEntity>> map_ = new Dictionary<VU_TIME, List<VuEntity>>();
    }

}
