using System;
using System.Diagnostics;
using System.IO;
using BIG_SCALAR = System.Single;
using SM_SCALAR = System.Single;
using VU_BOOL = System.Boolean;
using VU_KEY = System.UInt64;
using VU_TIME = System.UInt64;

namespace FalconNet.VU
{

    public enum VU_TRI_STATE
    {
        TRUE,
        FALSE,
        DONT_CARE
    }

    public abstract class VuCollection
    {
        //TODO public virtual ~VuCollection();
        public virtual void Init()
        {
            initialized_ = true;
            VUSTATIC.vuCollectionManager.Register(this);
        }

        public virtual void DeInit()
        {
            VUSTATIC.vuCollectionManager.DeRegister(this);
            initialized_ = false;
        }

        public virtual VU_ERRCODE Handle(VuMessage msg)
        {
            return VU_ERRCODE.VU_NO_OP;
        }

        public abstract VU_ERRCODE Insert(VuEntity entity);
        public abstract VU_ERRCODE Remove(VuEntity entity);
        public abstract VU_ERRCODE Remove(VU_ID entityId);
        public abstract int Purge(VU_BOOL all = true);
        public abstract int Count();
        public abstract VuEntity Find(VU_ID entityId);
        public abstract VuEntity Find(VuEntity entity);
        public abstract int Type();


        protected VuCollection()
        {
            initialized_ = false;
            // empty
        }

        // DATA
        // for use by VuCollectionManager only!
        internal VuCollection nextcoll_;
        protected VU_BOOL initialized_;
    }

    public class VuHashTable : VuCollection
    {
        public const int VU_DEFAULT_HASH_KEY = 59;
        public const int VU_HASH_TABLE_COLLECTION = 0x101;

        public VuHashTable(int tableSize, uint key = VU_DEFAULT_HASH_KEY)
            : base()
        {
            count_ = 0;
            capacity_ = tableSize;
            key_ = key;
            table_ = new VuLinkNode[tableSize + 1];
        }
        //TODO public virtual ~VuHashTable();

        public override VU_ERRCODE Insert(VuEntity entity)
        {
            if (entity == null)
                return VU_ERRCODE.VU_SUCCESS;

            if (entity.VuState() != VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                return VU_ERRCODE.VU_ERROR;
            }
            int index = (int)((VU_KEY)entity.Id() * key_) % capacity_;
            VuLinkNode entry = table_[index];

            CriticalSection.VuEnterCriticalSection();
            Debug.Assert(Find(entity) == null); // should not be in already
            entry = new VuLinkNode(entity, entry);
            count_++;
            CriticalSection.VuExitCriticalSection();

            return VU_ERRCODE.VU_SUCCESS;
        }

        public override VU_ERRCODE Remove(VuEntity entity)
        {
            if (entity == null)
                return VU_ERRCODE.VU_SUCCESS;

            int index = (int)((VU_KEY)entity.Id() * key_) % capacity_;
            VuLinkNode entry = table_[index];
            VuLinkNode last = null;
            VuLinkNode ptr;
            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;

            CriticalSection.VuEnterCriticalSection();

            ptr = entry;
            if (ptr.entity_ == null)
            {
                // not found
                CriticalSection.VuExitCriticalSection();
                return VU_ERRCODE.VU_NO_OP;
            }
            if (ptr.freenext_ != null)
            {
                // already done
                CriticalSection.VuExitCriticalSection();
                return VU_ERRCODE.VU_NO_OP;
            }

            while (ptr.entity_ != null && ptr.entity_ != entity)
            {
                last = ptr;
                ptr = ptr.next_;
            }
            if (ptr.entity_ != null)
            {
                if (last != null)
                {
                    last.next_ = ptr.next_;
                }
                else
                {
                    entry = ptr.next_;
                }
                // put curr on VUs pending delete queue
                VUSTATIC.vuCollectionManager.PutOnKillQueue(ptr);
                count_--;
                retval = VU_ERRCODE.VU_SUCCESS;
            }

            CriticalSection.VuExitCriticalSection();
            return retval;
        }

        public override VU_ERRCODE Remove(VU_ID entityId)
        {
            int index = (int)((VU_KEY)entityId * key_) % capacity_;
            VuLinkNode entry = table_[index];
            VuLinkNode ptr = entry;
            VuLinkNode last = null;
            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;

            if (ptr == null)
            {
                // not found
                return VU_ERRCODE.VU_NO_OP;
            }
            if (ptr.freenext_ != null)
            {
                // already done
                return VU_ERRCODE.VU_NO_OP;
            }

            CriticalSection.VuEnterCriticalSection();
            while (ptr.entity_ != null && ptr.entity_.Id() != entityId)
            {
                last = ptr;
                ptr = ptr.next_;
            }
            if (ptr.entity_ != null)
            {
                if (last != null)
                {
                    last.next_ = ptr.next_;
                }
                else
                {
                    entry = ptr.next_;
                }
                // put curr on VUs pending delete queue
                VUSTATIC.vuCollectionManager.PutOnKillQueue(ptr);
                count_--;
                retval = VU_ERRCODE.VU_SUCCESS;
            }
            CriticalSection.VuExitCriticalSection();
            return retval;
        }

        public override int Purge(VU_BOOL all = true)
        {
            int retval = 0;
            int index = 0;
            VuLinkNode entry = table_[0];
            VuLinkNode ptr = entry;
            VuLinkNode next, last;

            CriticalSection.VuEnterCriticalSection();
            while (index < capacity_)
            {
                last = null;
                while (ptr.entity_ != null)
                {
                    next = ptr.next_;
                    VuEntity ent = ptr.entity_;
                    if (!all && ((ent.IsPrivate() && ent.IsPersistent()) || ent.IsGlobal()))
                    {
                        ptr.next_ = VuTailNode.vuTailNode;
                        if (last != null)
                        {
                            last.next_ = ptr;
                        }
                        else
                        {
                            entry = ptr;
                        }
                        last = ptr;
                    }
                    else
                    {
                        VUSTATIC.vuCollectionManager.PutOnKillQueue(ptr, true);
                        retval++;
                        count_--;
                    }
                    ptr = next;
                }
                if (last == null)
                {
                    entry = VuTailNode.vuTailNode;
                }
                index++;
                entry = table_[index];
                ptr = entry;
            }
            CriticalSection.VuExitCriticalSection();
            return retval;
        }

        public override int Count()
        {
            return count_;
        }

        public override VuEntity Find(VU_ID entityId)
        {
            int index = (int)((VU_KEY)entityId * key_) % capacity_;
            VuLinkNode ptr = table_[index];

            while (ptr.entity_ != null && ptr.entity_.Id() != entityId)
            {
                ptr = ptr.next_;
            }
            return ptr.entity_;
        }

        public override VuEntity Find(VuEntity ent)
        {
            int index = (int)((VU_KEY)ent.Id() * key_) % capacity_;
            VuLinkNode ptr = table_[index];

            while (ptr.entity_ != null && ptr.entity_.Id() != ent.Id())
            {
                ptr = ptr.next_;
            }
            return ptr.entity_;
        }

        public override int Type()
        {
            return VU_HASH_TABLE_COLLECTION;
        }

        // DATA
        protected int count_;
        protected int capacity_;
        protected uint key_;
        internal VuLinkNode[] table_;
    }

    public class VuFilteredHashTable : VuHashTable
    {
        public const int VU_FILTERED_HASH_TABLE_COLLECTION = 0x102;

        public VuFilteredHashTable(VuFilter filter, int tableSize,
                          uint key = VU_DEFAULT_HASH_KEY)
            : base(tableSize, key)
        {
            filter_ = filter.Copy();
        }
        //TODO public virtual ~VuFilteredHashTable();

        public override VU_ERRCODE Handle(VuMessage msg)
        {
            if (filter_.Notice(msg))
            {
                VuEntity ent = msg.Entity();

                if (ent != null && filter_.RemoveTest(ent))
                {
                    if (Find(ent.Id()) != null)
                    {
                        if (!filter_.Test(ent))
                        {
                            // ent is in table, but doesn't belong there...
                            Remove(ent);
                        }
                    }
                    else if (filter_.Test(ent))
                    {
                        // ent is not in table, but does belong there...
                        Insert(ent);
                    }

                    return VU_ERRCODE.VU_SUCCESS;
                }
            }

            return VU_ERRCODE.VU_NO_OP;
        }

        public virtual VU_ERRCODE ForcedInsert(VuEntity entity)
        {
            if (!filter_.RemoveTest(entity))
            {
                return VU_ERRCODE.VU_NO_OP;
            }

            return base.Insert(entity);
        }
        public override VU_ERRCODE Insert(VuEntity entity)
        {
            if (!filter_.Test(entity))
            {
                return VU_ERRCODE.VU_NO_OP;
            }

            return base.Insert(entity);
        }

        public override int Type()
        {
            return VU_FILTERED_HASH_TABLE_COLLECTION;
        }

        protected VuFilter filter_;
    }

    public class VuDatabase : VuHashTable
    {
        public const int VU_DATABASE_COLLECTION = 0x103;

        public VuDatabase(int tableSize, uint key = VU_DEFAULT_HASH_KEY)
            : base(tableSize, key)
        {
            // do init
        }

        //public virtual ~VuDatabase();

        public override VU_ERRCODE Handle(VuMessage msg)
        {
            // note: this should work on Create & Delete messages, but those are
            // currently handled elsewhere... for now... just pass on to collection mgr
            VUSTATIC.vuCollectionManager.Handle(msg);
            return VU_ERRCODE.VU_SUCCESS;
        }

        public virtual VU_ERRCODE QuickInsert(VuEntity entity)
        {
            if (Find(entity.Id()) != null)
            {
                return VU_ERRCODE.VU_ERROR;
            }
            entity.SetVuState(VU_MEM_STATE.VU_MEM_ACTIVE);
            base.Insert(entity);
            VuEntity.VuReferenceEntity(entity);
            VUSTATIC.vuCollectionManager.Add(entity);
            if (entity.IsLocal() && (!entity.IsPrivate()))
            {
                VuCreateEvent evnt = null;
                VuTargetEntity target = VUSTATIC.vuGlobalGroup;
                if (!entity.IsGlobal())
                {
                    target = VUSTATIC.vuLocalSessionEntity.Game();
                }
                evnt = new VuCreateEvent(entity, target);
                evnt.RequestReliableTransmit();
                evnt.RequestOutOfBandTransmit();
                VuMessageQueue.PostVuMessage(evnt);
            }
            entity.InsertionCallback();
            return VU_ERRCODE.VU_SUCCESS;
        }
        public override VU_ERRCODE Insert(VuEntity entity)
        {
            if (Find(entity.Id()) != null)
            {
                return VU_ERRCODE.VU_ERROR;
            }
            entity.SetVuState(VU_MEM_STATE.VU_MEM_ACTIVE);
            base.Insert(entity);
            VuEntity.VuReferenceEntity(entity);
            VUSTATIC.vuCollectionManager.Add(entity);
            if (entity.IsLocal() && (!entity.IsPrivate()))
            {
                VuCreateEvent evnt = null;
                VuTargetEntity target = VUSTATIC.vuGlobalGroup;
                if (!entity.IsGlobal())
                {
                    target = VUSTATIC.vuLocalSessionEntity.Game();
                }
                evnt = new VuCreateEvent(entity, target);
                evnt.RequestReliableTransmit();
                VuMessageQueue.PostVuMessage(evnt);
            }
            entity.InsertionCallback();
            return VU_ERRCODE.VU_SUCCESS;
        }

        public virtual VU_ERRCODE SilentInsert(VuEntity entity)    // don't send create
        {
            if (Find(entity.Id()) != null)
            {
                return VU_ERRCODE.VU_ERROR;
            }
            entity.SetVuState(VU_MEM_STATE.VU_MEM_ACTIVE);
            base.Insert(entity);
            VuEntity.VuReferenceEntity(entity);
            VUSTATIC.vuCollectionManager.Add(entity);
            entity.InsertionCallback();
            return VU_ERRCODE.VU_SUCCESS;
        }
        public override VU_ERRCODE Remove(VuEntity entity)
        {
            if (base.Remove(entity) != VU_ERRCODE.VU_NO_OP && entity.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                entity.SetVuState(VU_MEM_STATE.VU_MEM_PENDING_DELETE);
                VUSTATIC.vuCollectionManager.Remove(entity);
                entity.RemovalCallback();
                VuEvent evnt;
                if (entity.IsLocal() && !entity.IsPrivate())
                {
                    evnt = new VuDeleteEvent(entity);
                    evnt.RequestReliableTransmit();
                    //me123			event.RequestOutOfBandTransmit ();
                }
                else
                {
                    evnt = new VuReleaseEvent(entity);
                }
                VuMessageQueue.PostVuMessage(evnt);
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        public virtual VU_ERRCODE SilentRemove(VuEntity entity)
        {
            if (base.Remove(entity) != VU_ERRCODE.VU_NO_OP && entity.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                entity.SetVuState(VU_MEM_STATE.VU_MEM_PENDING_DELETE);
                VUSTATIC.vuCollectionManager.Remove(entity);
                entity.RemovalCallback();
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        public override VU_ERRCODE Remove(VU_ID entityId)
        {
            VuEntity ent = Find(entityId);

            if (ent != null)
            {
                return Remove(ent);
            }

            return VU_ERRCODE.VU_NO_OP;
        }

        public delegate VU_ERRCODE HeaderCB(Stream file);
        public delegate VU_ERRCODE SaveCB(Stream file, VuEntity entity);
        public delegate VuEntity RestoreCB(Stream file);

        public VU_ERRCODE Save(string filename,
                                HeaderCB headercb, 	  // >= 0 -. continue
                                SaveCB savecb) // > 0 -. save it
        {
            //if (String.IsNullOrWhiteSpace(filename)) return VU_ERRCODE.VU_ERROR;
            //if (savecb == null) return VU_ERRCODE.VU_ERROR;

            //FileStream file = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            //if (file == null) return VU_ERRCODE.VU_ERROR;

            //if (headercb != null)
            //{
            //    if (headercb(file) < 0)
            //    {
            //        file.Close();
            //        return VU_ERRCODE.VU_ERROR;
            //    }
            //}

            //// reverse order...
            //VuLinkedList tmp = new VuLinkedList();
            //VuEntity ent;
            //VuDatabaseIterator dbiter = new VuDatabaseIterator();
            //for (ent = dbiter.GetFirst(); ent != null; ent = dbiter.GetNext())
            //{
            //    tmp.Insert(ent);
            //}
            //VuListIterator lliter = new VuListIterator(tmp);
            //for (ent = lliter.GetFirst(); ent != null; ent = lliter.GetNext())
            //{
            //    if (savecb(file, ent) >= 0)
            //    {
            //        ent.Save(file);
            //    }
            //}
            //int tail = 0;
            //file.Write(EncodingHelpers.EncodeIntLE(tail), sizeof(int), 1);
            //file.Close();
            return VU_ERRCODE.VU_SUCCESS;
        }

        public VU_ERRCODE Restore(string filename,
                                    HeaderCB headercb,	  // >= 0 -. continue
                                    RestoreCB restorecb) // returns pointer to alloc'ed ent
        {
            if (String.IsNullOrWhiteSpace(filename)) return VU_ERRCODE.VU_ERROR;
            if (restorecb == null) return VU_ERRCODE.VU_ERROR;

            FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read);

            if (file == null) return VU_ERRCODE.VU_ERROR;

            if (headercb != null)
            {
                if (headercb(file) < 0)
                {
                    file.Close();
                    return VU_ERRCODE.VU_ERROR;
                }
            }

            VuEntity ent = restorecb(file);
            while (ent != null)
            {
                Insert(ent);
                ent = restorecb(file);
            }

            file.Close();
            return VU_ERRCODE.VU_SUCCESS;
        }

        public override int Type()
        {
            return VU_DATABASE_COLLECTION;
        }


        // to be called by delete event only!
        public VU_ERRCODE DeleteRemove(VuEntity entity)
        {
            if (base.Remove(entity) != VU_ERRCODE.VU_ERROR && entity.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                entity.SetVuState(VU_MEM_STATE.VU_MEM_PENDING_DELETE);
                VUSTATIC.vuCollectionManager.Remove(entity);
                entity.RemovalCallback();
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        public override int Purge(VU_BOOL all = true)  // purges all from database
        {
            int retval = 0;
            int index = 0;
            VuLinkNode entry = table_[0];
            VuLinkNode ptr = entry;
            VuLinkNode next, last = null;

            CriticalSection.VuEnterCriticalSection();

            while (index < capacity_)
            {
                last = null;
                while (ptr.entity_ != null)
                {
                    next = ptr.next_;
                    VuEntity ent = ptr.entity_;

                    if (!all && ((ent.IsPrivate() && ent.IsPersistent()) || ent.IsGlobal()))
                    {
                        ptr.next_ = VuTailNode.vuTailNode;
                        if (last != null)
                        {
                            last.next_ = ptr;
                        }
                        else
                        {
                            entry = ptr;
                        }
                        last = ptr;
                    }
                    else
                    {
                        VUSTATIC.vuCollectionManager.PutOnKillQueue(ptr, true);
                        ptr.entity_ = null; // mark dead
                        VuEntity.VuDeReferenceEntity(ent); // JPO - swap around so not detected in database
                        retval++;
                        count_--;
                    }
                    ptr = next;
                }
                if (last == null)
                {
                    entry = VuTailNode.vuTailNode;
                }
                index++;
                entry = table_[index];
                ptr = entry;
            }
            CriticalSection.VuExitCriticalSection();
            return retval;
        }

        internal int Suspend(VU_BOOL all = true)  // migrates all to antiDB
        {
            int retval = 0;
            int index = 0;
            VuLinkNode entry = table_[0];
            VuLinkNode next, last = null;
            VuLinkNode ptr;

            CriticalSection.VuEnterCriticalSection();

            while (index < capacity_)
            {
                last = null;
                ptr = entry;
                while (ptr.entity_ != null)
                {
                    next = ptr.next_;
                    VuEntity ent = ptr.entity_;
                    if (!all && ((ent.IsPrivate() && ent.IsPersistent()) || ent.IsGlobal()))
                    {
                        ptr.next_ = VuTailNode.vuTailNode;
                        if (last != null)
                        {
                            last.next_ = ptr;
                        }
                        else
                        {
                            entry = ptr;
                        }
                        last = ptr;
                    }
                    else
                    {
                        VUSTATIC.vuCollectionManager.PutOnKillQueue(ptr, true);
                        if (last != null)
                        {
                            last.next_ = next;
                        }
                        else
                        {
                            entry = next;
                        }
                        ptr.entity_.RemovalCallback();
                        VUSTATIC.vuAntiDB.Insert(ptr.entity_);
                        VuEntity.VuDeReferenceEntity(ptr.entity_);
                        ptr.entity_ = null; // JPO mark dead.
                        retval++;
                        count_--;
                    }
                    ptr = next;
                }
                index++;
                entry = table_[index];
            }
            CriticalSection.VuExitCriticalSection();
            return retval;
        }
    }

    public class VuAntiDatabase : VuHashTable
    {
        public const int VU_ANTI_DATABASE_COLLECTION = 0x104;

        public VuAntiDatabase(int tableSize, uint key = VU_DEFAULT_HASH_KEY) : base(tableSize, key) { }
        //TODO public virtual ~VuAntiDatabase();

        public override VU_ERRCODE Insert(VuEntity entity)
        {
            if (VUSTATIC.vuDatabase.Find(entity.Id()) == null)
            {
                CriticalSection.VuEnterCriticalSection();

                // to ensure hash table insert succeeds
                entity.SetVuState(VU_MEM_STATE.VU_MEM_ACTIVE);
                base.Insert(entity);
                entity.SetVuState(VU_MEM_STATE.VU_MEM_SUSPENDED);

                VuEntity.VuReferenceEntity(entity);
                CriticalSection.VuExitCriticalSection();

                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }
        public override VU_ERRCODE Remove(VuEntity entity)
        {
            if (base.Remove(entity) != VU_ERRCODE.VU_ERROR)
            {
                if (VUSTATIC.vuDatabase.Find(entity.Id()) == null)
                {
                    entity.SetVuState(VU_MEM_STATE.VU_MEM_PENDING_DELETE);
                }

                VuEntity.VuDeReferenceEntity(entity);

                return VU_ERRCODE.VU_SUCCESS;
            }

            return VU_ERRCODE.VU_NO_OP;
        }
        public override VU_ERRCODE Remove(VU_ID entityId)
        {
            VuEntity ent = Find(entityId);

            if (ent != null)
            {
                return Remove(ent);
            }

            return VU_ERRCODE.VU_NO_OP;
        }

        public override int Type()
        {
            return VU_ANTI_DATABASE_COLLECTION;
        }


        public override int Purge(VU_BOOL all = true)  // purges all from database
        {
            int retval = 0;
            int index = 0;
            VuLinkNode entry = table_[0];
            VuLinkNode ptr = entry;
            VuLinkNode next, last;

            CriticalSection.VuEnterCriticalSection();

            while (index < capacity_)
            {
                last = null;
                while (ptr.entity_ != null)
                {
                    next = ptr.next_;
                    VuEntity ent = ptr.entity_;

                    if (!all && ((ent.IsPrivate() && ent.IsPersistent()) || ent.IsGlobal()))
                    {
                        ptr.next_ = VuTailNode.vuTailNode;
                        if (last != null)
                        {
                            last.next_ = ptr;
                        }
                        else
                        {
                            entry = ptr;
                        }

                        last = ptr;
                    }
                    else
                    {
                        VUSTATIC.vuCollectionManager.PutOnKillQueue(ptr, true);
                        ptr.entity_ = null;
                        VuEntity.VuDeReferenceEntity(ent);
                        retval++;
                        count_--;
                    }

                    ptr = next;
                }

                if (last == null)
                {
                    entry = VuTailNode.vuTailNode;
                }

                index++;
                entry = table_[index];
                ptr = entry;
            }

            CriticalSection.VuExitCriticalSection();

            return retval;
        }
    }

    public class VuRedBlackTree : VuCollection
    {

        public const int VU_RED_BLACK_TREE_COLLECTION = 0x401;

        public VuRedBlackTree(VuKeyFilter filter)
            : base()
        {
            root_ = null;
            filter_ = (VuKeyFilter)filter.Copy();
        }

        //TODO public virtual ~VuRedBlackTree();

        public override VU_ERRCODE Handle(VuMessage msg)
        {
            if (filter_ != null && filter_.Notice(msg))
            {
                VuEntity ent = msg.Entity();
                if (ent != null && filter_.RemoveTest(ent))
                {
                    if (Find(ent.Id()) != null)
                    {
                        if (!filter_.Test(ent))
                        {
                            // ent is in table, but doesn't belong there...
                            Remove(ent);
                        }
                    }
                    else if (filter_.Test(ent))
                    {
                        // ent is not in table, but does belong there...
                        Insert(ent);
                    }
                    return VU_ERRCODE.VU_SUCCESS;
                }
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        public virtual VU_ERRCODE ForcedInsert(VuEntity entity)
        {
            if (entity.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                if (!filter_.RemoveTest(entity))
                {
                    return VU_ERRCODE.VU_NO_OP;
                }

                VuRBNode curNode = root_;
                VU_KEY key = Key(entity);


                //if (key == 0x90da || key == 0x90db)
                //{
                //    int i = 0;
                //    i = 1;
                //}



                if (root_ == null)
                {
                    root_ = new VuRBNode(entity, key);
                    curNode = root_;
                    return VU_ERRCODE.VU_SUCCESS;
                }

                CriticalSection.VuEnterCriticalSection();

                while (curNode != null)
                {
                    if (key < curNode.key_)
                    {
                        if (curNode.left_ != null)
                        {
                            curNode = curNode.left_;
                        }
                        else
                        {
                            curNode = new VuRBNode(entity, key, curNode, RBSIDE.LEFT);
                            break;
                        }
                    }
                    else if (key > curNode.key_)
                    {
                        if (curNode.right_ != null)
                        {
                            curNode = curNode.right_;
                        }
                        else
                        {
                            curNode = new VuRBNode(entity, key, curNode, RBSIDE.RIGHT);
                            break;
                        }
                    }
                    else
                    {
                        VuLinkNode node = curNode.head_;
                        while (node.next_.entity_ != null)
                        {
                            node = node.next_;
                        }
                        node.next_ = new VuLinkNode(entity, VuTailNode.vuTailNode);
                        CriticalSection.VuExitCriticalSection();
                        return VU_ERRCODE.VU_SUCCESS;
                    }
                }

                VU_ERRCODE retval = InsertFixUp(curNode);

                CriticalSection.VuExitCriticalSection();

                return retval;
            }
            return VU_ERRCODE.VU_ERROR;
        }
        public override VU_ERRCODE Insert(VuEntity entity)
        {
            if (!filter_.Test(entity))
            {
                return VU_ERRCODE.VU_NO_OP;
            }
            return ForcedInsert(entity);
        }
        public override VU_ERRCODE Remove(VuEntity entity) { return Remove(entity, false); }
        public override VU_ERRCODE Remove(VU_ID entityId)
        {
            if (root_ == null) return 0;
            if (filter_ != null)
            {
                VuEntity ent = VUSTATIC.vuDatabase.Find(entityId);
                if (ent != null)
                {
                    return Remove(ent);
                }
                else
                {
                    return VU_ERRCODE.VU_NO_OP;
                }
            }
            VU_KEY key = (VU_KEY)entityId;

            //if (key == 0x90da || key == 0x90db)
            //{
            //    int i = 0;
            //    i = 1;
            //}

            VuRBNode rbnode = root_.Find(key);
            VuLinkNode ptr = null;

            if (rbnode != null)
            {
                ptr = rbnode.head_;
                if (ptr == null || ptr.entity_ == null)
                {
                    // fatal error...
                    return VU_ERRCODE.VU_ERROR;
                }
                if (ptr.freenext_ != null)
                {
                    // already done
                    return VU_ERRCODE.VU_NO_OP;
                }

                CriticalSection.VuEnterCriticalSection();
                VuLinkNode last = null;
                while (ptr.entity_ != null && ptr.entity_.Id() != entityId)
                {
                    last = ptr;
                    ptr = ptr.next_;
                }
                if (ptr.entity_ != null)
                {
                    if (last != null)
                    {
                        last.next_ = ptr.next_;
                    }
                    else
                    {
                        rbnode.head_ = ptr.next_;
                        if (rbnode.head_ == VuTailNode.vuTailNode)
                        {
                            RemoveNode(rbnode);
                            VUSTATIC.vuCollectionManager.PutOnKillQueue(rbnode);
                        }
                    }
                    // put curr on VUs pending delete queue
                    VUSTATIC.vuCollectionManager.PutOnKillQueue(ptr);
                }
                CriticalSection.VuExitCriticalSection();
            }
            return (ptr != null ? VU_ERRCODE.VU_SUCCESS : VU_ERRCODE.VU_NO_OP);
        }
        public override int Purge(VU_BOOL all = true)
        {
            int retval = 0;

            CriticalSection.VuEnterCriticalSection();

            if (all)
            {

                while (root_ != null)
                {
                    retval++;
                    if (Remove(root_.head_.entity_, true) == VU_ERRCODE.VU_NO_OP)
                    {
                        // JPO if this goes off the VU database has got corrupted
                        Console.WriteLine("VuRedBlackTree::Purge, failed to remove root id");
#if _DEBUG
			    VU_PRINT ("VuRedBlackTree::Purge, failed to remove root id %d\n",
				root_.head_.entity_.Id());
#endif
                        break; // emergency fix - JPO
                    }
                }
            }
            else
            {
                VuRBIterator iter = new VuRBIterator(this);
                VuEntity ent = iter.GetFirst();
                while (ent != null)
                {
                    VuEntity next = iter.GetNext();
                    if (!(ent.IsPrivate() && ent.IsPersistent()) && !ent.IsGlobal())
                    {
                        retval++;
                        if (Remove(ent, true) == VU_ERRCODE.VU_NO_OP)
                        {
                            Console.WriteLine("Purge !all failed to remove id");
#if _DEBUG
				    VU_PRINT ("Purge !all failed to remove id %d\n",
					ent.Id());
#endif
                        }
                    }
                    ent = next;
                }
            }
            CriticalSection.VuExitCriticalSection();
            return retval;
        }

        public override int Count()
        {
            VuRBIterator iter = new VuRBIterator(this);
            int cnt = 0;

            for (VuEntity ent = iter.GetFirst(); ent != null; ent = iter.GetNext())
            {
                cnt++;
            }
            return cnt;
        }
        public override VuEntity Find(VU_ID entityId)
        {
            VuEntity ent = VUSTATIC.vuDatabase.Find(entityId);
            if (ent != null)
            {
                return Find(ent);
            }
            return null;
        }

        public virtual VuEntity FindByKey(VU_KEY key)
        {
            VuRBNode rbnode = root_.Find(key);
            return (rbnode != null ? rbnode.head_.entity_ : null);
        }
        public override VuEntity Find(VuEntity ent)
        {
            VuRBNode rbnode = root_.Find(Key(ent));
            VuEntity retval = null;

            if (rbnode != null)
            {
                VuLinkNode lnode = rbnode.head_;
                while (lnode != null && lnode.entity_ != null && retval == null)
                {
                    if (lnode.entity_.Id() == ent.Id())
                    {
                        retval = lnode.entity_;
                    }
                    lnode = lnode.next_;
                }
            }
            return retval;
        }

        public override int Type()
        {
            return VU_RED_BLACK_TREE_COLLECTION;
        }


        // for debugging
        public void Dump()
        {
            Dump(root_, 0);
        }

        internal void Dump(VuRBNode node, int level)
        {
#if DEBUG
            if (node == null) return;

            Dump(node.left_, level + 1);
            char c = (node.color_ == RBCOLOR.BLACK ? '-' : '+');
            for (int i = 0; i < level - 1; i++)
            {
                Console.WriteLine("  ");
            }
            if (level != 0) Console.WriteLine("%c%c", c, c);
            Console.WriteLine("> id 0x%x, key %d:\t", (VU_KEY)node.head_.entity_.Id(), node.key_);
            if (level < 3) Console.WriteLine("\t");
            Console.WriteLine("parent %d; left %d; right %d; next %d\n",
                (node.parent_ != null ? node.parent_.key_ : 0),
                (node.left_ != null ? node.left_.key_ : 0),
                (node.right_ != null ? node.right_.key_ : 0),
                (node.next_ != null ? node.next_.key_ : 0));
            Dump(node.right_, level + 1);
#endif
        }

        internal int Purge(VuLinkNode repository)
        {
            int retval = 0;

            CriticalSection.VuEnterCriticalSection();
            VuRBNode cur = root_, next;

            while (cur != null)
            {
                if (cur.left_ != null)
                {
                    next = cur.left_;
                    cur.left_ = null;
                }
                else if (cur.right_ != null)
                {
                    next = cur.right_;
                    cur.right_ = null;
                }
                else
                {
                    next = cur.parent_;
                    while (cur.head_ != VuTailNode.vuTailNode)
                    {
                        if (cur.head_.freenext_ == null)
                        { // ensure node isn't on kill queue
                            cur.head_.freenext_ = repository;
                            repository = cur.head_;
                            retval++;
                        }
                        cur.head_ = cur.head_.next_;
                    }
                    VUSTATIC.vuCollectionManager.PutOnKillQueue(cur, true);
                }
                cur = next;
            }
            root_ = null;

            CriticalSection.VuExitCriticalSection();
            return retval;
        }
        internal VuRBNode RemoveNode(VuRBNode z)
        {
            VuRBNode x, y, prev;
            RBCOLOR ycolor;

            VuRBNode.bogusNode.parent_ = null;

            /* Fix next_ pointer */

            prev = z.Predecessor();

            if (prev != null) prev.next_ = z.next_;

            if (z.left_ == null || z.right_ == null)
            {
                y = z;
            }
            else
            {
                y = z.Successor();
            }

            ycolor = y.color_;

            if (y.left_ != null)
            {
                x = y.left_;
            }
            else if (y.right_ != null)
            {
                x = y.right_;
            }
            else
            {
                x = VuRBNode.bogusNode;
            }

            x.parent_ = y.parent_;

            if (y.parent_ == null)
            {
                root_ = x;
            }
            else if (y == y.parent_.left_)
            {
                y.parent_.left_ = x;
            }
            else
            {
                y.parent_.right_ = x;
            }

            if (y != z)
            {
                if (z.parent_ != null)
                {
                    if (z == z.parent_.left_)
                    {
                        z.parent_.left_ = y;
                    }
                    else if (z == z.parent_.right_)
                    {
                        z.parent_.right_ = y;
                    }
                }
                else
                {
                    root_ = y;
                }
                y.parent_ = z.parent_;
                y.color_ = z.color_;
                y.left_ = z.left_;
                y.right_ = z.right_;

                if (y.left_ != null)
                {
                    y.left_.parent_ = y;
                }

                if (y.right_ != null)
                {
                    y.right_.parent_ = y;
                }
            }

            if (ycolor == RBCOLOR.BLACK)
            {
                DeleteFixUp(x);
            }

            if (root_ == VuRBNode.bogusNode)
            {
                root_ = null;  /*  tree is gone  */
            }
            else if (x == VuRBNode.bogusNode)
            {
                if (x == x.parent_.left_)
                {
                    x.parent_.left_ = null;
                }
                else if (x == x.parent_.right_)
                {
                    x.parent_.right_ = null;
                }

                if (x.left_ != null || x.right_ != null)
                {
                    return null;  /* tree is trashed  */
                }
            }
            return z;
        }

        internal VU_ERRCODE InsertLink(VuLinkNode link, VU_KEY key)
        {
            //if (key == 0x90da || key == 0x90db)
            //{
            //    int i = 0;
            //    i = 1;
            //}

            if (link != null)
            {
                if (link.entity_ != null)
                {

                    if (!filter_.Test(link.entity_))
                    {
                        return VU_ERRCODE.VU_NO_OP;
                    }

                    VuRBNode curNode = root_;

                    if (root_ == null)
                    {
                        root_ = new VuRBNode(link, key);
                        return VU_ERRCODE.VU_SUCCESS;
                    }

                    CriticalSection.VuEnterCriticalSection();
                    while (curNode != null)
                    {
                        if (key < curNode.key_)
                        {
                            if (curNode.left_ != null)
                            {
                                curNode = curNode.left_;
                            }
                            else
                            {
                                curNode = new VuRBNode(link, key, curNode, RBSIDE.LEFT);
                                break;
                            }
                        }
                        else if (key > curNode.key_)
                        {
                            if (curNode.right_ != null)
                            {
                                curNode = curNode.right_;
                            }
                            else
                            {
                                curNode = new VuRBNode(link, key, curNode, RBSIDE.RIGHT);
                                break;
                            }
                        }
                        else
                        {
                            // insert at head...
                            link.next_ = curNode.head_;
                            curNode.head_ = link;
                            CriticalSection.VuExitCriticalSection();
                            return VU_ERRCODE.VU_SUCCESS;
                        }
                    }

                    VU_ERRCODE retval = InsertFixUp(curNode);

                    CriticalSection.VuExitCriticalSection();

                    return retval;
                }
            }

            return VU_ERRCODE.VU_ERROR;
        }
        internal VU_ERRCODE InsertNode(VuRBNode newNode)
        {
            //if (newNode.key_ == 0x90da || newNode.key_ == 0x90db)
            //{
            //    int i = 0;
            //    i = 1;
            //}

            if (newNode == null || newNode.head_.entity_ == null)
            {
                return VU_ERRCODE.VU_ERROR;
            }
            VuEntity ent = newNode.head_.entity_;
            if (!filter_.Test(ent))
            {
                return VU_ERRCODE.VU_NO_OP;
            }
            VuRBNode curNode = root_;

            if (root_ == null)
            {
                root_ = newNode;
                newNode.parent_ = null;
                newNode.left_ = null;
                newNode.right_ = null;
                newNode.next_ = null;
                newNode.color_ = RBCOLOR.BLACK;
                return VU_ERRCODE.VU_SUCCESS;
            }

            CriticalSection.VuEnterCriticalSection();
            while (curNode != null)
            {
                if (newNode.key_ < curNode.key_)
                {
                    if (curNode.left_ != null)
                    {
                        curNode = curNode.left_;
                    }
                    else
                    {
                        newNode.parent_ = curNode;
                        newNode.left_ = null;
                        newNode.right_ = null;
                        newNode.color_ = RBCOLOR.RED;
                        curNode.left_ = newNode;
                        newNode.next_ = newNode.SuccessorViaWalk();
                        VuRBNode prevNode = newNode.Predecessor();
                        if (prevNode != null)
                        {
                            prevNode.next_ = newNode;
                        }
                        break;
                    }
                }
                else if (newNode.key_ > curNode.key_)
                {
                    if (curNode.right_ != null)
                    {
                        curNode = curNode.right_;
                    }
                    else
                    {
                        newNode.parent_ = curNode;
                        newNode.left_ = null;
                        newNode.right_ = null;
                        newNode.color_ = RBCOLOR.RED;
                        curNode.right_ = newNode;
                        newNode.next_ = newNode.SuccessorViaWalk();
                        VuRBNode prevNode = newNode.Predecessor();
                        if (prevNode != null)
                        {
                            prevNode.next_ = newNode;
                        }
                        break;
                    }
                }
                else
                {
                    VuLinkNode node = curNode.head_;
                    while (node.next_.entity_ != null)
                    {
                        node = node.next_;
                    }
                    node.next_ = newNode.head_;
                    newNode.head_ = VuTailNode.vuTailNode;
                    VUSTATIC.vuCollectionManager.PutOnKillQueue(newNode);
                    CriticalSection.VuExitCriticalSection(); // JPO swapped this & previous lines.

                    return VU_ERRCODE.VU_SUCCESS;
                }
            }

            VU_ERRCODE retval = InsertFixUp(newNode);

            CriticalSection.VuExitCriticalSection();

            return retval;
        }
        internal VU_ERRCODE InsertFixUp(VuRBNode newNode)
        {
            VuRBNode x, y;

            x = newNode;
            while (x != root_ && x.parent_.color_ == RBCOLOR.RED)
            {
                if (x.parent_ == x.parent_.parent_.left_)
                {
                    y = x.parent_.parent_.right_;
                    if (y != null && y.color_ == RBCOLOR.RED)
                    {
                        x.parent_.color_ = RBCOLOR.BLACK;
                        y.color_ = RBCOLOR.BLACK;
                        x.parent_.parent_.color_ = RBCOLOR.RED;
                        x = x.parent_.parent_;
                    }
                    else
                    {
                        if (x == x.parent_.right_)
                        {
                            x = x.parent_;
                            RotateLeft(x);
                        }
                        x.parent_.color_ = RBCOLOR.BLACK;
                        x.parent_.parent_.color_ = RBCOLOR.RED;
                        RotateRight(x.parent_.parent_);
                    }
                }
                else
                {
                    y = x.parent_.parent_.left_;
                    if (y != null && y.color_ == RBCOLOR.RED)
                    {
                        x.parent_.color_ = RBCOLOR.BLACK;
                        y.color_ = RBCOLOR.BLACK;
                        x.parent_.parent_.color_ = RBCOLOR.RED;
                        x = x.parent_.parent_;
                    }
                    else
                    {
                        if (x == x.parent_.left_)
                        {
                            x = x.parent_;
                            RotateRight(x);
                        }
                        x.parent_.color_ = RBCOLOR.BLACK;
                        x.parent_.parent_.color_ = RBCOLOR.RED;
                        RotateLeft(x.parent_.parent_);
                    }
                }
            }
            root_.color_ = RBCOLOR.BLACK;

            return VU_ERRCODE.VU_SUCCESS;
        }
        internal void DeleteFixUp(VuRBNode x)
        {
            VuRBNode w;

            while (x != root_ && x.color_ == RBCOLOR.BLACK)
            {
                if (x == x.parent_.left_)
                {
                    w = x.parent_.right_;

                    if (w != null)
                    {
                        if (w != null && w.color_ == RBCOLOR.RED)
                        {
                            w.color_ = RBCOLOR.BLACK;
                            x.parent_.color_ = RBCOLOR.RED;
                            RotateLeft(x.parent_);
                            w = x.parent_.right_;
                        }

                        if ((w.left_ == null || w.left_.color_ == RBCOLOR.BLACK) &&
                            (w.right_ == null || w.right_.color_ == RBCOLOR.BLACK))
                        {
                            if (w != null) w.color_ = RBCOLOR.RED;
                            x = x.parent_;
                        }
                        else
                        {
                            if (w.right_ == null || w.right_.color_ == RBCOLOR.BLACK)
                            {
                                if (w.left_ != null) w.left_.color_ = RBCOLOR.BLACK;
                                if (w != null) w.color_ = RBCOLOR.RED;
                                RotateRight(w);
                                w = x.parent_.right_;  /* ok x != root */
                            }
                            if (w != null) w.color_ = x.parent_.color_;
                            x.parent_.color_ = RBCOLOR.BLACK;
                            if (w.right_ != null) w.right_.color_ = RBCOLOR.BLACK;
                            RotateLeft(x.parent_);
                            x = root_;
                        }
                    }
                }
                else
                {
                    w = x.parent_.left_;
                    if (w != null)
                    {
                        if (w != null && w.color_ == RBCOLOR.RED)
                        {
                            w.color_ = RBCOLOR.BLACK;
                            x.parent_.color_ = RBCOLOR.RED;
                            RotateRight(x.parent_);
                            w = x.parent_.left_;
                        }

                        if ((w.left_ == null || w.left_.color_ == RBCOLOR.BLACK) &&
                            (w.right_ == null || w.right_.color_ == RBCOLOR.BLACK))
                        {
                            if (w != null) w.color_ = RBCOLOR.RED;
                            x = x.parent_;
                        }
                        else
                        {
                            if (w.left_ == null || w.left_.color_ == RBCOLOR.BLACK)
                            {
                                if (w.right_ != null) w.right_.color_ = RBCOLOR.BLACK;
                                if (w != null) w.color_ = RBCOLOR.RED;
                                RotateLeft(w);
                                w = x.parent_.left_;
                            }
                            if (w != null) w.color_ = x.parent_.color_;
                            x.parent_.color_ = RBCOLOR.BLACK;
                            if (w.left_ != null) w.left_.color_ = RBCOLOR.BLACK;
                            RotateRight(x.parent_);
                            x = root_;
                        }
                    }
                }
            }
            x.color_ = RBCOLOR.BLACK;
        }

        internal void RotateLeft(VuRBNode x)
        {
            VuRBNode y = x.right_;

            x.right_ = y.left_;

            if (y.left_ != null)
            {
                y.left_.parent_ = x;
            }

            y.parent_ = x.parent_;

            if (x.parent_ == null)
            {
                root_ = y;
            }
            else
            {
                if (x == x.parent_.left_)
                {
                    x.parent_.left_ = y;
                }
                else
                {
                    x.parent_.right_ = y;
                }
            }
            y.left_ = x;
            x.parent_ = y;
        }

        internal void RotateRight(VuRBNode y)
        {
            VuRBNode x;

            x = y.left_;
            y.left_ = x.right_;

            if (x.right_ != null)
            {
                x.right_.parent_ = y;
            }

            x.parent_ = y.parent_;

            if (y.parent_ == null)
            {
                root_ = x;
            }
            else
            {
                if (y == y.parent_.right_)
                {
                    y.parent_.right_ = x;
                }
                else
                {
                    y.parent_.left_ = x;
                }
            }

            x.right_ = y;
            y.parent_ = x;
        }

        protected VU_KEY Key(VuEntity ent)
        { return filter_ != null ? filter_.Key(ent) : (VU_KEY)ent.Id(); }


        // VuGridTree interface

        private VuRedBlackTree()
            : base()
        {
            root_ = null;
            filter_ = null;
        }
        private VU_ERRCODE Remove(VuEntity entity, VU_BOOL purge)
        {
            VU_KEY key = Key(entity);

            
            //if (key == 0x90da || key == 0x90db)
            //{
            //    int i = 0;
            //    i = 1;
            //}

            if (root_ == null) return VU_ERRCODE.VU_NO_OP;

            if (filter_ != null && !filter_.RemoveTest(entity)) return VU_ERRCODE.VU_NO_OP;
            VuRBNode rbnode = root_.Find(key);
            VuLinkNode ptr = null;

            if (rbnode != null)
            {
                ptr = rbnode.head_;
                if (ptr == null || ptr.entity_ == null)
                {
                    // fatal error...
                    return VU_ERRCODE.VU_ERROR;
                }
                if (ptr.freenext_ != null)
                {
                    // already done
                    return VU_ERRCODE.VU_NO_OP;
                }

                CriticalSection.VuEnterCriticalSection();
                VuLinkNode last = null;
                while (ptr.entity_ != null && ptr.entity_ != entity)
                {
                    last = ptr;
                    ptr = ptr.next_;
                }
                if (ptr.entity_ != null)
                {
                    if (last != null)
                    {
                        last.next_ = ptr.next_;
                    }
                    else
                    {
                        rbnode.head_ = ptr.next_;
                        if (rbnode.head_ == VuTailNode.vuTailNode)
                        {
                            RemoveNode(rbnode);
                            VUSTATIC.vuCollectionManager.PutOnKillQueue(rbnode, purge);
                        }
                    }
                    // put curr on VUs pending delete queue
                    VUSTATIC.vuCollectionManager.PutOnKillQueue(ptr, purge);
                }
                CriticalSection.VuExitCriticalSection();
            }
            return (ptr != null ? VU_ERRCODE.VU_SUCCESS : VU_ERRCODE.VU_NO_OP);
        }

        internal void InitFilter(VuKeyFilter filter)
        {
            filter_ = (VuKeyFilter)filter.Copy();
        }

        // DATA

        internal VuRBNode root_;
        protected VuKeyFilter filter_;
    }

    //-----------------------------------------------------------------------------
    public class VuGridTree : VuCollection
    {
        public const int VU_GRID_TREE_COLLECTION = 0x801;

        public VuGridTree(VuBiKeyFilter filter, int numrows,
            BIG_SCALAR center, BIG_SCALAR radius, VU_BOOL wrap = false)
            : base()
        {
            rowcount_ = numrows;
            wrap_ = wrap;
            suspendUpdates_ = false;
            nextgrid_ = null;
            filter_ = (VuBiKeyFilter)filter.Copy();
            ulong icenter = filter.CoordToKey(center);
            ulong iradius = filter.Distance1(radius);
            bottom_ = icenter - iradius;
            top_ = icenter + iradius;
            rowheight_ = (top_ - bottom_) / (VU_TIME)rowcount_;
            invrowheight_ = 1.0f / (float)((top_ - bottom_) / (VU_TIME)rowcount_);
            table_ = new VuRedBlackTree[numrows];

            int currow = 0;

            for (int i = 0; i < numrows; i++)
            {
                table_[currow].InitFilter(filter_);
                currow++;
            }

            VUSTATIC.vuCollectionManager.GridRegister(this);
        }
        //TODO public virtual ~VuGridTree();

        public virtual VU_ERRCODE ForcedInsert(VuEntity entity)
        {
            if (entity.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                Debug.Assert(Find(entity) == null); // JPO check its not there already
                if (!filter_.RemoveTest(entity)) return VU_ERRCODE.VU_NO_OP;
                VuRedBlackTree row = Row(filter_.Key1(entity));

                return row.ForcedInsert(entity);
            }
            return VU_ERRCODE.VU_ERROR;
        }

        public override VU_ERRCODE Insert(VuEntity entity)
        {
            if (entity.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                Debug.Assert(Find(entity) == null); // JPO check its not there already
                if (!filter_.Test(entity)) return VU_ERRCODE.VU_NO_OP;
                VuRedBlackTree row = Row(filter_.Key1(entity));
                return row.Insert(entity);
            }
            return VU_ERRCODE.VU_ERROR;
        }

        public override VU_ERRCODE Remove(VuEntity entity)
        {
            if (filter_.RemoveTest(entity))
            {
                VuRedBlackTree row = Row(filter_.Key1(entity));
                VU_ERRCODE res = row.Remove(entity);
                Debug.Assert(Find(entity) == null); // JPO should be gone now.
                return res;
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        public override VU_ERRCODE Remove(VU_ID entityId)
        {
            VuEntity ent = VUSTATIC.vuDatabase.Find(entityId);
            if (ent != null)
            {
                return Remove(ent);
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        public virtual VU_ERRCODE Move(VuEntity ent, BIG_SCALAR coord1, BIG_SCALAR coord2)
        {
            if (filter_.RemoveTest(ent))
            {

                VU_KEY key1 = filter_.Key1(ent);
                VU_KEY key2 = filter_.Key2(ent);

                VuRedBlackTree row = Row(key1);

                if (row.root_ != null)
                {
                    VuRBNode node = row.root_.Find(key2);

                    if (node != null)
                    {
                        VuLinkNode linknode = node.head_;
                        VuLinkNode lastlinknode = null;

                        while (linknode.entity_ != null && linknode.entity_ != ent)
                        {
                            lastlinknode = linknode;
                            linknode = linknode.next_;
                        }

                        if (linknode.entity_ != null)
                        {
                            VU_KEY newKey1 = filter_.CoordToKey(coord1);
                            VU_KEY newKey2 = filter_.CoordToKey(coord2);

                            if (key1 == newKey1 && key2 == newKey2)
                            {
                                // we didn't move
                                return VU_ERRCODE.VU_SUCCESS;
                            }

                            VuRedBlackTree newrow = Row(newKey1);
                            if (lastlinknode != null || linknode.next_ != VuTailNode.vuTailNode)
                            {
                                // we have multiple ents on this RBNode
                                if (key2 == newKey2 && row == newrow)
                                {
                                    // we didn't move
                                    return VU_ERRCODE.VU_SUCCESS;
                                }
                                CriticalSection.VuEnterCriticalSection();
                                if (lastlinknode != null)
                                {
                                    lastlinknode.next_ = linknode.next_;
                                }
                                else
                                {
                                    node.head_ = linknode.next_;
                                }
                                if (newrow.InsertLink(linknode, newKey2) != VU_ERRCODE.VU_SUCCESS)
                                {
                                    VUSTATIC.vuCollectionManager.PutOnKillQueue(linknode);
                                }
                                CriticalSection.VuExitCriticalSection();
                                return VU_ERRCODE.VU_SUCCESS;
                            }

                            CriticalSection.VuEnterCriticalSection();
                            row.RemoveNode(node);
                            node.key_ = newKey2;
                            newrow.InsertNode(node);
                            CriticalSection.VuExitCriticalSection();

                            return VU_ERRCODE.VU_SUCCESS;
                        }
                    }
                    else
                    {	// JPO if this goes off the VU database has got corrupted

#if _DEBUG
			    //ShiWarning("Entity out of position on key2");
//			    VU_PRINT ("Entity out of position on key2 %d id %d\n", key2, ent.Id());
#endif
                    }
                }
                else
                {	// JPO if this goes off the VU database has got corrupted
#if _DEBUG
		    //ShiWarning("Entity out of position on key1");
//		    VU_PRINT ("Entity out of position on key1 %d id %d\n", key1, ent.Id());
#endif
                }
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        public override int Purge(VU_BOOL all = true)
        {
            int retval = 0;
            CriticalSection.VuEnterCriticalSection();
            int currow = 0;
            for (int i = 0; i < rowcount_; i++)
            {
                retval += table_[currow].Purge(all);
                currow++;
            }
            CriticalSection.VuExitCriticalSection();

            return retval;
        }

        public override int Count()
        {
            int count = 0;
            int currow = 0;
            for (int i = 0; i < rowcount_; i++)
            {
                count += table_[currow].Count();
                currow++;
            }
            return count;
        }

        public override VuEntity Find(VU_ID entityId)
        {
            VuEntity ent = VUSTATIC.vuDatabase.Find(entityId);
            if (ent != null)
            {
                return Find(ent);
            }
            return null;
        }

        public virtual VuEntity FindByKey(VU_KEY key1, VU_KEY key2)
        {
            VuRedBlackTree row = Row(key1);
            return row.FindByKey(key2);
        }
        public override VuEntity Find(VuEntity ent)
        {
            VuRedBlackTree row = Row(filter_.Key1(ent));
            return row.Find(ent);
        }

        public void SuspendUpdates() { suspendUpdates_ = true; }
        public void ResumeUpdates() { suspendUpdates_ = false; }
        public VU_ERRCODE Rebuild()
        {
            CriticalSection.VuEnterCriticalSection();
            VuLinkNode holding = VuTailNode.vuTailNode, next;
            int entcount = 0;

            // step 1: dismantle...
            int currow = 0;

            for (int i = 0; i < rowcount_; i++)
            {
                entcount += table_[currow].Purge(holding);
                currow++;
            }
            // step 2: rebuild...
            while (holding != VuTailNode.vuTailNode)
            {
                next = holding.freenext_;
                Insert(holding.entity_);
                VUSTATIC.vuCollectionManager.PutOnKillQueue(holding, true);
                holding = next;
            }

            CriticalSection.VuExitCriticalSection();
            return VU_ERRCODE.VU_SUCCESS;
        }


        public void DebugString(out string buf)
        {
#if _DEBUG
	int count = 0;
	
	VuRedBlackTree* currow = table_;
	for (int i = 0; i < rowcount_; i++) {
		if (CheckIntegrity(currow.root_, i) > 0) {
			assert(0);
		}
		count = currow.Count();
		if (count < 0) {
			*buf++ = '-';
		} else if (count == 0) {
			*buf++ = '.';
		} else if (count < 10) {
			*buf++ = '1' + count - 1;
		} else {
			*buf++ = '*';
		}
		currow++;
	}
	*buf++ = '\0';
#else
            buf = "";
#endif
        }
        public override int Type()
        {
            return VU_GRID_TREE_COLLECTION;
        }


        internal int RowIndex(VU_KEY key1)
        {
            int index = 0;

            //Debug.Assert(FTOL(58.9f) == 58); // jpo - detects a slip in fpu state
            // if this goes off - the FPU is rounding to nearest rather than down.
            //_controlfp(_RC_CHOP, MCW_RC);

            if (wrap_)
            {
                if (key1 < bottom_)
                {
                    key1 = top_ - ((bottom_ - key1) % (top_ - bottom_));
                }
                else
                {
                    key1 = ((key1 - bottom_) % (top_ - bottom_)) + bottom_;
                }
            }

            if (key1 > bottom_)
            {
                index = (int)((key1 - bottom_) * invrowheight_);
            }

            if (index > rowcount_ - 1)
            {
                index = rowcount_ - 1;
            }
            return index;
        }

        internal VuRedBlackTree Row(VU_KEY key1)
        {
            return table_[RowIndex(key1)];
        }

        internal int CheckIntegrity(VuRBNode node, int column)
        {
            int retval = 0;

#if _DEBUG
        if (node) {
		retval += CheckIntegrity(node.left_, col);
		VuLinkNode *link = node.head_;
		int i = 0;
		while (link.entity_) {
			if (filter_.Key(link.entity_) != node.key_) {
				VU_PRINT("CheckIntegrity(0x%x, col %d): nodekey = %d, entkey = %d, depth = %d\n", 
					(VU_KEY)link.entity_.Id(), col, node.key_, filter_.Key(link.entity_), i);
				retval++;
			}
			link = link.next_;
			i++;
		}
		retval += CheckIntegrity(node.right_, col);
	}
#endif
            return retval;
        }

        // DATA

        internal VuRedBlackTree[] table_;
        internal VuBiKeyFilter filter_;
        internal int rowcount_;
        internal VU_KEY bottom_;
        internal VU_KEY top_;
        internal VU_KEY rowheight_;
        internal SM_SCALAR invrowheight_;
        internal VU_BOOL wrap_;
        internal VU_BOOL suspendUpdates_;

        internal VuGridTree nextgrid_;
    }

    //-----------------------------------------------------------------------------
    public class VuLinkedList : VuCollection
    {
        public const int VU_LINKED_LIST_COLLECTION = 0x201;
        public VuLinkedList()
            : base()
        {
            tail_ = VuTailNode.vuTailNode;
            head_ = tail_;
        }
        //TODO public virtual ~VuLinkedList();

        public override VU_ERRCODE Insert(VuEntity entity)
        {
            if (entity.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                CriticalSection.VuEnterCriticalSection();
                //		assert(Find(entity) == null); // this fires all the heading - from FindNearestSupplySource
                head_ = new VuLinkNode(entity, head_);
                CriticalSection.VuExitCriticalSection();
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_ERROR;
        }

        public override VU_ERRCODE Remove(VuEntity entity)
        {
            VuListMuckyIterator iter = new VuListMuckyIterator(this);

            for (VuEntity ptr = iter.GetFirst(); ptr != null; ptr = iter.GetNext())
            {
                if (ptr == entity)
                {
                    iter.RemoveCurrent();
                    return VU_ERRCODE.VU_NO_OP;
                }
            }
            // did not find entity
            return VU_ERRCODE.VU_SUCCESS;
        }

        public override VU_ERRCODE Remove(VU_ID entityId)
        {
            VuListMuckyIterator iter = new VuListMuckyIterator(this);

            for (VuEntity ptr = iter.GetFirst(); ptr != null; ptr = iter.GetNext())
            {
                if (ptr.Id() == entityId)
                {
                    iter.RemoveCurrent();
                    return VU_ERRCODE.VU_SUCCESS;
                }
            }
            // did not find entity
            return VU_ERRCODE.VU_NO_OP;
        }
        public override int Purge(VU_BOOL all = true)
        {
            int retval = 0;
            VuLinkNode next, cur, last = null;
            CriticalSection.VuEnterCriticalSection();
            cur = head_;

            while (cur != null && cur.entity_ != null)
            {
                next = cur.next_;
                VuEntity ent = cur.entity_;

                if (!all && ((ent.IsPrivate() && ent.IsPersistent()) || ent.IsGlobal()))
                {
                    cur.next_ = VuTailNode.vuTailNode;
                    if (last != null)
                    {
                        last.next_ = cur;
                    }
                    else
                    {
                        head_ = cur;
                    }
                    last = cur;
                }
                else
                {
                    VUSTATIC.vuCollectionManager.PutOnKillQueue(cur, true);
                    retval++;
                }
                cur = next;
            }
            if (last == null)
            {
                head_ = VuTailNode.vuTailNode;
            }

            CriticalSection.VuExitCriticalSection();
            return retval;
        }
        public override int Count()
        {
            VuListIterator iter = new VuListIterator(this);
            int cnt = 0;

            for (VuEntity ent = iter.GetFirst(); ent != null; ent = iter.GetNext())
            {
                cnt++;
            }
            return cnt;
        }
        public override VuEntity Find(VU_ID entityId)
        {
            VuListIterator iter = new VuListIterator(this);

            for (VuEntity ent = iter.GetFirst(); ent != null; ent = iter.GetNext())
            {
                if (ent.Id() == entityId)
                {
                    return ent;
                }
            }
            return null;
        }
        public override VuEntity Find(VuEntity ent)
        {
            return Find(ent.Id());
        }
        public override int Type()
        {
            return VU_LINKED_LIST_COLLECTION;
        }


        // DATA

        internal VuLinkNode head_;
        internal VuLinkNode tail_;
    }

    //-----------------------------------------------------------------------------
    public class VuFilteredList : VuLinkedList
    {
        public const int VU_FILTERED_LIST_COLLECTION = 0x202;
        public VuFilteredList(VuFilter filter)
            : base()
        {
            filter_ = filter.Copy();
        }

        //TODO public virtual ~VuFilteredList();

        public override VU_ERRCODE Handle(VuMessage msg)
        {
            if (filter_.Notice(msg))
            {
                VuEntity ent = msg.Entity();
                if (ent != null && filter_.RemoveTest(ent))
                {
                    if (Find(ent.Id()) != null)
                    {
                        if (!filter_.Test(ent))
                        {
                            // ent is in table, but doesn't belong there...
                            Remove(ent);
                        }
                    }
                    else if (filter_.Test(ent))
                    {
                        // ent is not in table, but does belong there...
                        Insert(ent);
                    }
                    return VU_ERRCODE.VU_SUCCESS;
                }
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        public virtual VU_ERRCODE ForcedInsert(VuEntity entity)
        {
            if (filter_.RemoveTest(entity))
                return base.Insert(entity);
            else
                return VU_ERRCODE.VU_NO_OP;
        }
        public override VU_ERRCODE Insert(VuEntity entity)
        {
            if (filter_.Test(entity))
                return base.Insert(entity);
            else
                return VU_ERRCODE.VU_NO_OP;
        }
        public override VU_ERRCODE Remove(VuEntity entity)
        {
            if (filter_.RemoveTest(entity))
                return base.Remove(entity);
            else
                return VU_ERRCODE.VU_NO_OP;
        }
        public override VU_ERRCODE Remove(VU_ID entityId)
        {
            VuEntity ent = VUSTATIC.vuDatabase.Find(entityId);

            if (ent != null)
            {
                return Remove(ent);
            }

            return base.Remove(entityId);
        }

        public override int Type()
        {
            return VU_FILTERED_LIST_COLLECTION;
        }


        protected VuFilter filter_;
    }

    //-----------------------------------------------------------------------------
    public class VuOrderedList : VuFilteredList
    {
        public const int VU_ORDERED_LIST_COLLECTION = 0x203;
        public VuOrderedList(VuFilter filter) : base(filter) { }
        //TODO public virtual ~VuOrderedList();

        public override VU_ERRCODE ForcedInsert(VuEntity entity)
        {
            if (entity.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                if (filter_.RemoveTest(entity))
                {

                    VuListMuckyIterator iter = new VuListMuckyIterator(this);

                    for (VuEntity ptr = iter.GetFirst(); ptr != null; ptr = iter.GetNext())
                    {
                        if (filter_.Compare(ptr, entity) >= 0)
                        {
                            iter.InsertCurrent(entity);
                            return VU_ERRCODE.VU_SUCCESS;
                        }
                    }

                    iter.InsertCurrent(entity);
                    return VU_ERRCODE.VU_SUCCESS;
                }
                return VU_ERRCODE.VU_NO_OP;
            }
            return VU_ERRCODE.VU_ERROR;
        }
        public override VU_ERRCODE Insert(VuEntity entity)
        {
            if (filter_.Test(entity))
                return ForcedInsert(entity);
            else
                return VU_ERRCODE.VU_NO_OP;
        }
        public override int Type()
        {
            return VU_ORDERED_LIST_COLLECTION;
        }
    }

    //-----------------------------------------------------------------------------
    public class VuLifoQueue : VuFilteredList
    {
        public const int VU_LIFO_QUEUE_COLLECTION = 0x204;

        public VuLifoQueue(VuFilter filter) : base(filter) { }
        //TODO public virtual ~VuLifoQueue();

        public override int Type()
        {
            return VU_LIFO_QUEUE_COLLECTION;
        }

        public VU_ERRCODE Push(VuEntity entity) { return ForcedInsert(entity); }
        public VuEntity Peek()
        {
            return head_.entity_;
        }
        public VuEntity Pop()
        {
            VuEntity retval = Peek();
            Remove(retval);
            return retval;
        }
    }

    //-----------------------------------------------------------------------------
    public class VuFifoQueue : VuFilteredList
    {
        public const int VU_FIFO_QUEUE_COLLECTION = 0x205;

        public VuFifoQueue(VuFilter filter)
            : base(filter)
        {
            last_ = head_;
        }

        //TODO public virtual ~VuFifoQueue();

        public override VU_ERRCODE ForcedInsert(VuEntity entity)
        {
            if (entity.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                if (filter_.RemoveTest(entity))
                {

                    CriticalSection.VuEnterCriticalSection();
                    if (head_ != tail_)
                    {
                        last_.next_ = new VuLinkNode(entity, tail_);
                        last_ = last_.next_;
                    }
                    else
                    {
                        head_ = last_ = new VuLinkNode(entity, tail_); // first entity in queue...
                    }
                    CriticalSection.VuExitCriticalSection();

                    return VU_ERRCODE.VU_SUCCESS;
                }
                return VU_ERRCODE.VU_NO_OP;
            }
            return VU_ERRCODE.VU_ERROR;
        }
        public override VU_ERRCODE Insert(VuEntity entity)
        {
            if (filter_.Test(entity))
                return ForcedInsert(entity);
            else
                return VU_ERRCODE.VU_NO_OP;
        }
        public override VU_ERRCODE Remove(VuEntity entity)
        {
            CriticalSection.VuEnterCriticalSection();

            VuLinkNode cur = head_;
            VuLinkNode last = null;

            while (cur.entity_ != null)
            {
                if (cur.entity_ == entity)
                {
                    if (last != null)
                    {
                        last.next_ = cur.next_;
                    }
                    else
                    {
                        head_ = cur.next_;
                    }

                    if (cur == last_)
                    {
                        if (last != null)
                        {
                            last_ = last;
                        }
                        else
                        {
                            last_ = head_;
                        }
                    }

                    VUSTATIC.vuCollectionManager.PutOnKillQueue(cur);
                    CriticalSection.VuExitCriticalSection();

                    return VU_ERRCODE.VU_SUCCESS;
                }

                last = cur;
                cur = cur.next_;
            }

            CriticalSection.VuExitCriticalSection();

            return VU_ERRCODE.VU_NO_OP;
        }
        public override VU_ERRCODE Remove(VU_ID entityId)
        {
            VuEntity ent = VUSTATIC.vuDatabase.Find(entityId);

            if (ent != null)
            {
                return Remove(ent);
            }

            return VU_ERRCODE.VU_NO_OP;
        }
        public override int Purge(VU_BOOL all = true)
        {
            CriticalSection.VuEnterCriticalSection();

            int retval = base.Purge(all);

            last_ = head_;

            while (last_.entity_ != null)
            {
                last_ = last_.next_;
            }

            CriticalSection.VuExitCriticalSection();

            return retval;
        }
        public override int Type()
        {
            return VU_FIFO_QUEUE_COLLECTION;
        }

        public VU_ERRCODE Push(VuEntity entity) { return ForcedInsert(entity); }
        public VuEntity Peek()
        {
            return head_.entity_;
        }
        public VuEntity Pop()
        {
            VuEntity retval = Peek();
            Remove(retval);

            return retval;
        }


        internal VuLinkNode last_;
    }

     //-----------------------------------------------------------------------------
    public class VuDatabaseIterator : VuHashIterator
    {

        public VuDatabaseIterator() : base(VUSTATIC.vuDatabase) { }
        //TODO public virtual ~VuDatabaseIterator();
    }

    internal class VuLinkNode
    {
        public VuLinkNode(VuEntity entity, VuLinkNode next)
        {
            freenext_ = null;
            entity_ = entity;
            next_ = next;
        }
        //TODO ~VuLinkNode();

        // data
        internal VuLinkNode freenext_;	// used only for mem management
        internal VuEntity entity_;
        internal VuLinkNode next_;
    }

    internal enum RBCOLOR
    {
        RED = 1,
        BLACK = 2
    }

    internal enum RBSIDE
    {
        LEFT = 1,
        RIGHT = 2
    }

    internal class VuRBNode
    {
#if USE_SH_POOLS
   public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { assert( size == sizeof(VuRBNode) ); return MemAllocFS(vuRBNodepool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ vuRBNodepool = MemPoolInitFS( sizeof(VuRBNode), 400, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( vuRBNodepool ); };
#endif
        public static VuRBNode bogusNode; // = new VuRBNode((VuEntity )null, UInt64.MaxVal));

        public VuRBNode(VuLinkNode link, VU_KEY key)
        {
            parent_ = null;
            left_ = null;
            right_ = null;
            next_ = null;
            key_ = key;
            color_ = RBCOLOR.BLACK;
            link.next_ = VuTailNode.vuTailNode;
            head_ = link;
        }
        // root
        public VuRBNode(VuEntity ent, VU_KEY key) // root
        {
            parent_ = null;
            left_ = null;
            right_ = null;
            next_ = null;
            key_ = key;
            color_ = RBCOLOR.BLACK;
            head_ = new VuLinkNode(ent, VuTailNode.vuTailNode);
        }
        public VuRBNode(VuEntity ent, VU_KEY key, VuRBNode parent, RBSIDE side)
        {
            parent_ = parent;
            left_ = null;
            right_ = null;
            next_ = null;
            key_ = key;
            color_ = RBCOLOR.BLACK;
            head_ = new VuLinkNode(ent, VuTailNode.vuTailNode);

            if (side == RBSIDE.LEFT)
                parent.left_ = this;
            else
                parent.right_ = this;

            next_ = SuccessorViaWalk();
            VuRBNode prevNode = Predecessor();
            if (prevNode != null) prevNode.next_ = this;
        }

        public VuRBNode(VuLinkNode link, VU_KEY key, VuRBNode parent, RBSIDE side)
        {
            parent_ = parent;
            left_ = null;
            right_ = null;
            next_ = null;
            key_ = key;
            color_ = RBCOLOR.BLACK;
            link.next_ = VuTailNode.vuTailNode;
            head_ = link;

            if (side == RBSIDE.LEFT)
                parent.left_ = this;
            else
                parent.right_ = this;

            next_ = SuccessorViaWalk();
            VuRBNode prevNode = Predecessor();
            if (prevNode != null) prevNode.next_ = this;
        }

        //TODO public ~VuRBNode();

        public VuRBNode Successor() { return next_; }
        public VuRBNode SuccessorViaWalk()
        {
            if (right_ != null)
            {
                return right_.TreeMinimum();
            }
            else
            {
                VuRBNode x = this;
                VuRBNode y = parent_;

                while (y != null && x == y.right_)
                {
                    x = y;
                    y = y.parent_;
                }
                return y;
            }
        }

        public VuRBNode Predecessor()
        {
            if (left_ != null)
            {
                return left_.TreeMaximum();
            }
            else
            {
                VuRBNode x = this;

                while (x.parent_ != null)
                {
                    if (x == x.parent_.right_)
                    {
                        return x.parent_;
                    }
                    x = x.parent_;
                }
                return null;
            }
        }

        public VuRBNode TreeMinimum()
        {
            VuRBNode x = this;
            while (x.left_ != null)
                x = x.left_;

            return x;
        }

        public VuRBNode TreeMaximum()
        {
            VuRBNode x = this;
            while (x.right_ != null)
                x = x.right_;

            return x;
        }

        public VuRBNode UpperBound(VU_KEY key)
        {
            VuRBNode x = this;
            VuRBNode retval = null;

            if (x.key_ == key) return x;

            while (x != null && key != x.key_)
            {
                if (key < x.key_)
                {
                    if (x.left_ == null)
                        return retval;
                    x = x.left_;
                }
                else
                {
                    retval = x;
                    if (x.right_ == null)
                        return x;
                    x = x.right_;
                }
            }
            return retval;
        }

        public VuRBNode LowerBound(VU_KEY key)
        {
            VuRBNode x = this;
            VuRBNode retval = null;

            if (x.key_ == key) return x;

            while (x != null && key != x.key_)
            {
                if (key < x.key_)
                {
                    retval = x;
                    if (x.left_ == null)
                        return x;
                    x = x.left_;
                }
                else
                {
                    if (x.right_ == null)
                        return retval;
                    x = x.right_;
                }
            }
            return retval;
        }

        public VuRBNode Find(VU_KEY key)
        {
            VuRBNode x = this;

            while (x != null && key != x.key_)
            {
                if (key < x.key_)
                    x = x.left_;
                else
                    x = x.right_;
            }
            return x;
        }

        // data
        public VuRBNode parent_;
        public VuRBNode left_;
        public VuRBNode right_;
        public VuRBNode next_;
        public VuLinkNode head_;
        public VU_KEY key_;
        public RBCOLOR color_;
    }

    internal class VuTailNode : VuLinkNode
    {
        private VuTailNode()
            : base(null, null)
        {
            next_ = this;
        }

        // tail (special empty end of list)
        public static VuTailNode vuTailNode = new VuTailNode();
    }
}
