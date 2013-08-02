using System.Collections.Generic;
using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{
    //-----------------------------------------------------------------------------
    public class VuLinkedList : VuCollection
    {
        public VuLinkedList(VuFilter filter = null)
            : base(filter)
        {
            l_ = new List<VuEntity>();
        }
        //TODO public virtual ~VuLinkedList();

        protected override VU_ERRCODE PrivateInsert(VuEntity entity)
        {
            lock (GetMutex())
            {
                l_.Add(entity);
                return VU_ERRCODE.VU_SUCCESS;
            }
        }

        protected override VU_ERRCODE PrivateRemove(VuEntity entity)
        {
            lock (GetMutex())
            {
                for (int i = l_.Count - 1; i >= 0; i--)
                {
                    if (l_[i] == entity)
                    {
                        l_.RemoveAt(i);
                        return VU_ERRCODE.VU_SUCCESS;
                    }
                }

                return VU_ERRCODE.VU_NO_OP;
            }
        }

        protected override bool PrivateFind(VuEntity entity)
        {
            lock (GetMutex())
            {
                for (int i = l_.Count - 1; i >= 0; i--)
                {
                    if (l_[i] == entity)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public override int Purge(VU_BOOL all = true)
        {
            lock (GetMutex())
            {
                int ret = 0;
                // this is to avoid self destructing CTDs when purging lists
                // like: simentity is removed. its the last from a featgure. feature is destroyed.
                // its component list is removed and its this list... guess what happens
                //TODO List<VuEntity> toBePurged = new List<VuEntity>();

                for (int i = l_.Count - 1; i >= 0; i--)
                {
                    VuEntity ent = l_[i];

                    if (!all && (ent.IsGlobal() || (ent.IsPrivate() && ent.IsPersistent())))
                    {
                        // dont remove global or private pesistant
                    }
                    else
                    {
                        //TODO toBePurged.Add(ent);
                        l_.RemoveAt(i);
                        ++ret;
                    }
                }

                //TODO toBePurged.Clear();
                return ret;
            }
        }

        public override int Count()
        {
            lock (GetMutex())
            {
                int count = 0;

                for (int i = l_.Count - 1; i >= 0; i--)
                {

                    if (l_[i].VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
                    {
                        ++count;
                    }
                }

                return count;
            }
        }

        //public override VuEntity Find(VU_ID entityId)
        //{
        //    VuListIterator iter = new VuListIterator(this);

        //    for (VuEntity ent = iter.GetFirst(); ent != null; ent = iter.GetNext())
        //    {
        //        if (ent.Id() == entityId)
        //        {
        //            return ent;
        //        }
        //    }
        //    return null;
        //}

        public override VU_COLL_TYPE Type()
        {
            return VU_COLL_TYPE.VU_LINKED_LIST_COLLECTION;
        }

        // DATA
        public List<VuEntity> l_;
    }


}
