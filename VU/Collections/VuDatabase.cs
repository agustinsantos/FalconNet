using System.Collections.Generic;
using BIG_SCALAR = System.Single;
using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{
    public class VuDatabase
    {
        /** creates the database of the given size and key. */
        public VuDatabase(int tableSize, uint key = VuHashTable.VU_DEFAULT_HASH_KEY)
        {
            dbHash_ = new VuHashTable(null, tableSize, key);
        }

        //public virtual ~VuDatabase();

        /** handles a message, passing it to collectionManager so that all other databases handle it */
        public VU_ERRCODE Handle(VuMessage msg)
        {
            // note: this should work on Create & Delete messages, but those are
            // currently handled elsewhere... for now... just pass on to collection mgr
            return VUSTATIC.vuCollectionManager.Handle(msg);
        }

        /** handles a move, passing it to collectionManager so that all grids get updated. */
        public void HandleMove(VuEntity ent, BIG_SCALAR coord1, BIG_SCALAR coord2)
        {
            if (ent.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                VUSTATIC.vuCollectionManager.HandleMove(ent, coord1, coord2);
            }
        }


        public VU_ERRCODE Insert(VuEntity entity)
        {
            if (entity == null)
            {
                return VU_ERRCODE.VU_NO_OP;
            }

#if NOTHING//BIRTH_LIST

    // already in
    if ((entity.VuState() == VU_MEM_ACTIVE) || (entity.VuState() == VU_MEM_TO_BE_INSERTED))
    {
        return VU_ERROR;
    }

    VuScopeLock l(GetMutex());
    entity.SetVuState(VU_MEM_TO_BE_INSERTED);
    vuCollectionManager.AddToBirthList(entity);
    VuEntity::VU_SEND_TYPE sendType = entity.SendCreate();

    if (entity.IsLocal() && (!entity.IsPrivate()) && (sendType != VuEntity::VU_SC_DONT_SEND))
    {
        VuCreateEvent *event = 0;
        VuTargetEntity *target = vuGlobalGroup;

        if (!entity.IsGlobal())
        {
            target = vuLocalSessionEntity.Game();
        }

        event = new VuCreateEvent(entity, target);
        event.RequestReliableTransmit();

        if (sendType == VuEntity::VU_SC_SEND_OOB)
        {
            event.RequestOutOfBandTransmit();
        }

        VuMessageQueue::PostVuMessage(event);
    }

    entity.InsertionCallback();
    return VU_SUCCESS;

#else

            // no duplicates allowed
            if ((entity.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE) || (dbHash_.Find(entity.Id()) != null))
            {
                return VU_ERRCODE.VU_ERROR;
            }

            entity.SetVuState(VU_MEM_STATE.VU_MEM_ACTIVE);
            dbHash_.Insert(entity);
            VUSTATIC.vuCollectionManager.Add(entity);

            VU_SEND_TYPE sendType = entity.SendCreate();

            if (entity.IsLocal() && (!entity.IsPrivate()) && (sendType != VU_SEND_TYPE.VU_SC_DONT_SEND))
            {
                VuCreateEvent event_ = null;
                VuTargetEntity target = VUSTATIC.vuGlobalGroup;

                if (!entity.IsGlobal())
                {
                    target = VUSTATIC.vuLocalSessionEntity.Game();
                }

                event_ = new VuCreateEvent(entity, target);
                event_.RequestReliableTransmit();

                if (sendType == VU_SEND_TYPE.VU_SC_SEND_OOB)
                {
                    event_.RequestOutOfBandTransmit();
                }

                VuMessageQueue.PostVuMessage(event_);
            }

            // sfr: its possible this is being called twice (because of the create event)
            entity.InsertionCallback();
            return VU_ERRCODE.VU_SUCCESS;
#endif
        }

        /** common part of all removes in DB (except the real removal). */
        internal VU_ERRCODE CommonRemove(VuEntity entity)
        {
            if (entity == null || entity.VuState() != VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                return VU_ERRCODE.VU_NO_OP;
            }

            VUSTATIC.vuCollectionManager.AddToGc(entity);
            entity.SetVuState(VU_MEM_STATE.VU_MEM_INACTIVE);

#if IMMEDIATE_REMOVAL_CALLBACK
    entity.RemovalCallback();
#endif

            return VU_ERRCODE.VU_SUCCESS;
        }


        /** calls RemovalCallback, sends remotely and marks entity as VU_MEM_INACTIVE. */
        public VU_ERRCODE Remove(VuEntity entity)
        {
            VU_ERRCODE ret = CommonRemove(entity);

            if (ret != VU_ERRCODE.VU_SUCCESS)
            {
                return ret;
            }

#if NO_RELEASE_EVENT

            if (entity.IsLocal() && !entity.IsPrivate())
            {
                VuEvent *event = new VuDeleteEvent(entity);
                event.RequestReliableTransmit();
                VuMessageQueue::PostVuMessage(event);
            }
#else
            VuEvent event_;

            if (entity.IsLocal() && !entity.IsPrivate())
            {
                event_ = new VuDeleteEvent(entity);
                event_.RequestReliableTransmit();
            }
            else
            {
                event_ = new VuReleaseEvent(entity);
            }

            VuMessageQueue.PostVuMessage(event_);
#endif
            return VU_ERRCODE.VU_SUCCESS;
        }

        /** like above, but dont send remove over network. */
        public virtual VU_ERRCODE SilentRemove(VuEntity entity)
        {
            return CommonRemove(entity);
        }

        /** like above, on active entitiy. */
        public VU_ERRCODE Remove(VU_ID entityId)
        {
            return Remove(dbHash_.Find(entityId));
        }

        /** returns an active entity with the given ID. */
        public VuEntity Find(VU_ID entityId)
        {
            return dbHash_.Find(entityId);
        }

        /** inserts entity into all registered collections which accept it (@see VuFilter::Test).
        * Used by collection manager birth list.
        */
        internal void ReallyInsert(VuEntity entity)
        {
            entity.SetVuState(VU_MEM_STATE.VU_MEM_ACTIVE);
            dbHash_.Insert(entity);
            VUSTATIC.vuCollectionManager.Add(entity);
        }

        internal void ReallyRemove(VuEntity entity)
        {
            // play it safe
            VuEntity safe = entity;
#if !IMMEDIATE_REMOVAL_CALLBACK
            entity.RemovalCallback();
#endif
            dbHash_.Remove(entity);
            VUSTATIC.vuCollectionManager.Remove(entity);
            entity.SetVuState(VU_MEM_STATE.VU_MEM_REMOVED);
        }

        public int Purge(VU_BOOL all = true)  // purges all from database
        {
            // this is a bit different from hash db since we need to save entities before purging
            // to avoid self destruction CTD (entity is destroyed and its callback removes other entities
            // from vudb
            // suspend calls removal callback
            List<VuEntity> toBePurged = new List<VuEntity>();
            int ret = 0;

            for (int i = 0; i < dbHash_.capacity_; ++i)
            {
                VuListIterator li = new VuListIterator(dbHash_.table_[i]);
                VuEntity e;

                for (
                    e = li.GetFirst();
                    e != null;
                    e = li.GetNext()
                )
                {
                    // run calling all callbacks and seting as removed... purge will do the actual removal
                    if (!(!all && ((e.IsPrivate() && e.IsPersistent()) || e.IsGlobal())))
                    {
                        toBePurged.Add(e);
                    }
                }
            }

            ret = dbHash_.Purge(all);
            // now delete all
            toBePurged.Clear();
            return ret;
        }

        /** just like purge, but calls removalCallback for entities removed. */
        internal int Suspend(VU_BOOL all = true)  // migrates all to antiDB
        {
            {
                // suspend calls removal callback
                int ret = 0;
                // similar to purge here...
                List<VuEntity> toBeSuspended = new List<VuEntity>();

                for (int i = 0; i < dbHash_.capacity_; ++i)
                {
                    VuListIterator li = new VuListIterator(dbHash_.table_[i]);
                    VuEntity e;

                    for (
                        e = li.GetFirst();
                        e != null;
                        e = li.GetNext()
                    )
                    {
                        // run calling all callbacks and seting as removed... purge will do the actual removal
                        if (!(!all && ((e.IsPrivate() && e.IsPersistent()) || e.IsGlobal())))
                        {
                            toBeSuspended.Add(e);
                            e.RemovalCallback();
                            e.SetVuState(VU_MEM_STATE.VU_MEM_REMOVED);
                        }
                    }
                }

                ret = dbHash_.Purge(all);
                // now remove all all
                toBeSuspended.Clear();
                return ret;
            }
        }

        /** the database container. */
        internal protected VuHashTable dbHash_;
    }


}
