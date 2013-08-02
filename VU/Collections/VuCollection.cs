using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BIG_SCALAR = System.Single;
using SM_SCALAR = System.Single;
using VU_BOOL = System.Boolean;
using VU_KEY = System.UInt64;
using VU_TIME = System.UInt64;
using VuMutex = System.Object;

namespace FalconNet.VU
{
    public abstract class VuCollection
    {

        /// <summary>
        /// register collection, so that it will be updated by vuCollection manager.
        /// For example: when an entity is removed from vuDatabase, it will also be removed
        /// from all registered collections. Registered collections also handle messages received.
        /// This is thread safe.
        /// </summary>
        public void Register()
        {
            if (!registered)
            {
                VUSTATIC.vuCollectionManager.Register(this);
                registered = true;
            }
        }

        /// <summary>
        /// unregister collection, so that it stops receiving updates and handling messages.
        /// Thread safe.
        /// </summary>
        public void Unregister()
        {
            if (registered)
            {
                VUSTATIC.vuCollectionManager.DeRegister(this);
                registered = false;
            }
        }

        //TODO public virtual ~VuCollection();

        /// <summary>
        ///  handles message for collection.
        ///  Default implementation does nothing if not filtered. Otherwise, if entity in message
        ///  should be in collection but is not, its inserted. If its in but should not, its removed.
        ///  This function is dangerous, not thread safe and its usage should be eliminated if possible.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual VU_ERRCODE Handle(VuMessage msg)
        {
            if (filter_ == null)
            {
                return VU_ERRCODE.VU_NO_OP;
            }
            else
            {
                if (filter_.Notice(msg))
                {
                    VuEntity ent = msg.Entity();

                    if (ent != null && filter_.RemoveTest(ent))
                    {
                        if (Find(ent))
                        {
                            if (!filter_.Test(ent))
                            {
                                // ent is in table, but shouldnt
                                PrivateRemove(ent);
                            }
                        }
                        else if (filter_.Test(ent))
                        {
                            // ent is not in table, but should be in.
                            PrivateInsert(ent);
                        }

                        return VU_ERRCODE.VU_SUCCESS;
                    }
                }

                return VU_ERRCODE.VU_NO_OP;
            }
        }


        /// <summary>
        /// inserts an entity into collection. If collection has filter, entity must pass
        /// filter.Test() to be inserted. This function is called on all registered collection
        /// by collection manager when a new entity is added to vuDatabase.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual VU_ERRCODE Insert(VuEntity entity)
        {
            if (entity == null)
            {
                return VU_ERRCODE.VU_NO_OP;
            }

            if (filter_ == null)
            {
                return PrivateInsert(entity);
            }

            lock (GetMutex())
            {
                // must pass test
                if (filter_.Test(entity))
                {
                    return PrivateInsert(entity);
                }

                return VU_ERRCODE.VU_NO_OP;
            }
        }

        /// <summary>
        /// inserts entity, regardless of filter.Test().
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual VU_ERRCODE ForcedInsert(VuEntity entity)
        {
            if (entity == null)
            {
                return VU_ERRCODE.VU_NO_OP;
            }

            return PrivateInsert(entity);
        }


        /// <summary>
        /// removes an entity from collection.
        /// If collection is filtered, entity must pass filter.RemoveTest() to run collection
        /// looking for entity. This function is called by collection manager on all registered collections
        /// when an entity is removed from vuDatabase.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual VU_ERRCODE Remove(VuEntity entity)
        {
            if (entity == null)
            {
                return VU_ERRCODE.VU_NO_OP;
            }

            if (filter_ == null)
            {
                return PrivateRemove(entity);
            }

            lock (GetMutex())
            {

                if (filter_.RemoveTest(entity))
                {
                    return PrivateRemove(entity);
                }

                return VU_ERRCODE.VU_NO_OP;
            }
        }

        //public abstract VU_ERRCODE Remove(VU_ID entityId);
        public abstract int Purge(VU_BOOL all = true);
        public abstract int Count();
        public abstract VU_COLL_TYPE Type();

        //public abstract VuEntity Find(VU_ID entityId);

        /// <summary>
        /// returns if entity is in collection or not.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Find(VuEntity entity)
        {
            if (entity == null)
            {
                return false;
            }

            if ((filter_ != null) && !filter_.Test(entity))
            {
                return false;
            }

            return PrivateFind(entity);
        }


        /// <summary>
        /// creates the collection. If threadSage, all iterators will lock it while running.
        /// This doesnt work yet. If filtered, most operations will rely on filter before actually
        /// doing it.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="threadSafe"></param>
        protected VuCollection(VuFilter filter, bool threadSafe = false)
        {
            //TODO mutex_ = threadSafe ? VuxMutex.VuxCreateMutex("collection mutex") : null;
            mutex_ = VuxMutex.VuxCreateMutex("collection mutex");
            filter_ = filter == null ? null : filter.Copy();
            registered = false;
        }

        /** private insertion, called by Insert and ForcedInsert. Do the actual insertion. */
        protected abstract VU_ERRCODE PrivateInsert(VuEntity entity);

        /** private remove, do the actual removal from collection. */
        protected abstract VU_ERRCODE PrivateRemove(VuEntity entity);

        /** does the actual look up for collection find. */
        protected abstract bool PrivateFind(VuEntity entity);

        /** gets the collection filter. */
        protected VuFilter GetFilter()
        {
            return filter_;
        }
        /** returns collection mutex. */
        public VuMutex GetMutex()
        {
            return mutex_;
        }

        // DATA
        /** collection mutex */
        private VuMutex mutex_;

        /** collection filter (if any). */
        public VuFilter filter_;

        /** tells if collection is registered. */
        private bool registered;
    }


}
