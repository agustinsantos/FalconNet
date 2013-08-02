
namespace FalconNet.VU
{
    //-----------------------------------------------------------------------------
    public class VuOrderedList : VuLinkedList
    {
        public VuOrderedList(VuFilter filter) : base(filter) { }
        //TODO public virtual ~VuOrderedList();

        protected override VU_ERRCODE PrivateInsert(VuEntity entity)
        {
            for (int i = l_.Count - 1; i >= 0; i--)
            {
                VuEntity b = l_[i];

                if (GetFilter().Compare(b, entity) >= 0)
                {
                    l_.Insert(i, entity);
                    return VU_ERRCODE.VU_SUCCESS;
                }
            }

            l_.Add(entity);
            return VU_ERRCODE.VU_SUCCESS;
        }

        protected override VU_ERRCODE PrivateRemove(VuEntity entity)
        {
            for (int i = l_.Count - 1; i >= 0; i--)
            {
                VuEntity b = l_[i];
                int res = GetFilter().Compare(b, entity);

                if (res == 0)
                {
                    l_.RemoveAt(i);
                    return VU_ERRCODE.VU_SUCCESS;
                }
                else if (res > 0)
                {
                    // all other elements are smaller than entity, can stop
                    return VU_ERRCODE.VU_NO_OP;
                }
            }

            return VU_ERRCODE.VU_SUCCESS;
        }

        protected override bool PrivateFind(VuEntity entity)
        {
            for (int i = l_.Count - 1; i >= 0; i--)
            {
                VuEntity b = l_[i];
                int res = GetFilter().Compare(b, entity);

                if (res == 0)
                {
                    return true;
                }
                else if (res > 0)
                {
                    // all other elements are smaller than entity, can stop
                    return false;
                }
            }

            return false;
        }

        public override VU_COLL_TYPE Type()
        {
            return VU_COLL_TYPE.VU_ORDERED_LIST_COLLECTION;
        }
    }

}
