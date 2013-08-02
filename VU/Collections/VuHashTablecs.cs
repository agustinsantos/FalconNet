using VU_KEY = System.UInt64;

namespace FalconNet.VU
{
    public class VuHashTable : VuCollection
    {
        public const int VU_DEFAULT_HASH_KEY = 59;

        public VuHashTable(VuFilter filter, int tableSize, uint key = VU_DEFAULT_HASH_KEY)
            : base(filter)
        {
            capacity_ = tableSize;
            key_ = key;
            table_ = new VuLinkedList[tableSize];
        }
        //TODO public virtual ~VuHashTable();


        protected override VU_ERRCODE PrivateInsert(VuEntity entity)
        {
            int ind = getIndex(entity.Id());
            if (table_[ind] == null)
                table_[ind] = new VuLinkedList();
            return table_[ind].ForcedInsert(entity);
        }

        protected override VU_ERRCODE PrivateRemove(VuEntity entity)
        {
            int ind = getIndex(entity.Id());
            if (table_[ind] == null)
                return VU_ERRCODE.VU_NO_OP;
            return table_[ind].Remove(entity);
        }

        protected override bool PrivateFind(VuEntity entity)
        {
            int ind = getIndex(entity.Id());
            if (table_[ind] == null)
                return false;

            return table_[getIndex(entity.Id())].Find(entity);
        }


        public VU_ERRCODE Remove(VU_ID entityId)
        {
            VuLinkedList list = table_[getIndex(entityId)];
            for (int i = list.l_.Count - 1; i >= 0; i--)
            {
                VuEntity e = list.l_[i];
                if ((e.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE) && (e.Id() == entityId))
                {
                    list.l_.RemoveAt(i);
                    return VU_ERRCODE.VU_SUCCESS;
                }
            }

            return VU_ERRCODE.VU_NO_OP;

        }

        public VuEntity Find(VU_ID entityId)
        {
            VuLinkedList list = table_[getIndex(entityId)];
            if (list == null)
                return null;

            for (int i = list.l_.Count - 1; i >= 0; i--)
            {
                VuEntity e = list.l_[i];
                if ((e.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE) && (e.Id() == entityId))
                {
                    return e;
                }
            }

            return null;
        }



        protected int getIndex(VU_ID id)
        {
            return (int)((VU_KEY)id * (ulong)key_) % capacity_;
        }


        public override int Purge(bool all = true)
        {
            lock (GetMutex())
            {
                int ret = 0;

                for (int i = 0; i < capacity_; ++i)
                {
                    ret += table_[i].Purge(all);
                }

                return ret;
            }
        }

        public override int Count()
        {
            lock (GetMutex())
            {
                int ret = 0;

                for (int i = 0; i < capacity_; ++i)
                {
                    ret += table_[i].Count();
                }

                return ret;
            }
        }
        public override VU_COLL_TYPE Type()
        {
            return VU_COLL_TYPE.VU_HASH_TABLE_COLLECTION;
        }

        // DATA
        public int capacity_;
        public uint key_;
        public VuLinkedList[] table_;
    }


}
