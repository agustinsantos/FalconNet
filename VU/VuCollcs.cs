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

    public enum VU_TRI_STATE
    {
        TRUE,
        FALSE,
        DONT_CARE
    }

    public enum VU_COLL_TYPE
    {
        VU_UNKNOWN_COLLECTION = 0x000,
        VU_HASH_TABLE_COLLECTION = 0x101,
        VU_FILTERED_HASH_TABLE_COLLECTION = 0x102,
        VU_DATABASE_COLLECTION = 0x103,
        VU_ANTI_DATABASE_COLLECTION = 0x104,
        VU_LINKED_LIST_COLLECTION = 0x201,
        VU_FILTERED_LIST_COLLECTION = 0x202,
        VU_ORDERED_LIST_COLLECTION = 0x203,
        VU_LIFO_QUEUE_COLLECTION = 0x204,
        VU_FIFO_QUEUE_COLLECTION = 0x205,
        VU_RED_BLACK_TREE_COLLECTION = 0x401,
        VU_GRID_TREE_COLLECTION = 0x801
    }

    //typedef VuLinkedList VuFilteredList ;
    public class VuFilteredList : VuLinkedList
    {
        public VuFilteredList(VuFilter filter) : base(filter) { }
    }

 
 #if TODO // Not needed??? 
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

        public override VU_ERRCODE ForcedInsert(VuEntity entity)
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

#endif

#if TODO // Not needed??? 
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
#endif


    //-----------------------------------------------------------------------------
    public class VuDatabaseIterator : VuHashIterator
    {

        public VuDatabaseIterator() : base(VUSTATIC.vuDatabase.dbHash_) { }
        //TODO public virtual ~VuDatabaseIterator();
    }
#if TODO
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
#endif
}
