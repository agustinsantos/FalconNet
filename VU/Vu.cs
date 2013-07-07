using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;
using VU_ID_NUMBER = System.UInt64;
using VU_KEY = System.UInt64;
using VU_BOOL = System.Boolean;
using BIG_SCALAR = System.Single;
using SM_SCALAR = System.Single;
using System.Diagnostics;

namespace FalconNet.VU
{
    public struct VuReleaseInfo
    {
        public int version_;
        public int revision_;
        public int patch_;
        public string revisionDate_;
        public string patchDate_;
    }

    public static class VUSTATIC
    {
        public const int VU_VERSION = 3;
        public const int VU_REVISION = 1;
        public const int VU_PATCH = 0;
        public const string VU_REVISION_DATE = @"28/01/2007";
        public const string VU_PATCH_DATE = @"28/01/2007";
        public static VuReleaseInfo vuReleaseInfo = new VuReleaseInfo
        {
            version_ = VU_VERSION,
            revision_ = VU_REVISION,
            patch_ = VU_PATCH,
            revisionDate_ = VU_REVISION_DATE,
            patchDate_ = VU_PATCH_DATE,
        };

        public const string VU_DEFAULT_PLAYER_NAME = "anonymous";
        public const string VU_GAME_GROUP_NAME = "Vu2 Game";
        public const string VU_PLAYER_POOL_GROUP_NAME = "Player Pool";

        public static VuSessionEntity vuLocalSessionEntity = null;
        public static VuGlobalGroup vuGlobalGroup = null;
        public static VuPlayerPoolGame vuPlayerPoolGroup = null;
        public static VuFilteredList vuGameList = null;
        public static VuFilteredList vuTargetList = null;

        public static VuPendingSendQueue vuNormalSendQueue = null;
        public static VuPendingSendQueue vuLowSendQueue = null;

        public static VU_SESSION_ID vuKnownConnectionId;       // 0 -. not known (usual case)
        public static VU_SESSION_ID vuNullSession;
        public static VU_ID vuLocalSession = new VU_ID(0, 0);
        //TODO Already defined at VU_ID public static VU_ID vuNullId;
        public static VU_TIME vuTransmitTime = 0;

        //   app provided globals
        public static int SGpfSwitchVal;
        public static string vuxWorldName = null;
        public static ulong vuxLocalDomain = 0xffffffff;
        public static VU_TIME vuxGameTime = 0;
        public static VU_TIME vuxRealTime;
        public static VuDriverSettings vuxDriverSettings;
#if VU_AUTO_COLLISION
public static VuGridTree *vuxCollisionGrid;
#endif // VU_AUTO_COLLISION

        public static VuCollectionManager vuCollectionManager = null;
        public static VuDatabase vuDatabase = null;
        public static VuAntiDatabase vuAntiDB = null;

        // =========================
        // VU required globals
        // =========================

        public static ulong vuxVersion = 1;

        public static VuEntity VuxCreateEntity(ushort type, ushort size, VU_BYTE[] data)
        { throw new NotImplementedException(); }

        public static VuEntityType VuxType(ushort id)
        { throw new NotImplementedException(); }

        public static void VuxRetireEntity(VuEntity theEntity)
        {
            //F4Assert(theEntity);
            Console.WriteLine("You dropped an entity, better find it!!!\n");
        }

        public const int VU_TICS_PER_SECOND = 1000;
        public static SM_SCALAR vuxTicsPerSec = 1000.0F;
        public static VU_TIME vuxTargetGameTime = 0;
        public static VU_TIME vuxLastTargetGameTime = 0;
        public static VU_TIME vuxDeadReconTime = 0;
        public static VU_TIME vuxCurrentTime = 0;
        public static VU_TIME lastTimingMessage = 0;
        public static VU_TIME vuxTransmitTime = 0;
        public static VU_BYTE vuxLocalSession = 1;

        public static VuListIterator vuTargetListIter = null;

    }

    public abstract class VuBaseThread
    {
        public VuBaseThread() { }
        // public virtual ~VuBaseThread();

        public VuMessageQueue Queue() { return messageQueue_; }

        public abstract VuMainMessageQueue MainQueue();

        public virtual void Update()
        {
            messageQueue_.DispatchAllMessages(false);
        }

        protected VuMessageQueue messageQueue_;
    }

    public class VuThread : VuBaseThread
    {
        public const int VU_DEFAULT_QUEUE_SIZE = 100;

        public VuThread(VuMessageFilter filter, int queueSize = VU_DEFAULT_QUEUE_SIZE)
            : base()
        {
            if (filter != null)
            {
                messageQueue_ = new VuMessageQueue(queueSize, filter);
            }
            else
            {
                VuStandardMsgFilter smf = new VuStandardMsgFilter();
                messageQueue_ = new VuMessageQueue(queueSize, smf);
            }
        }

        public VuThread(int queueSize = VU_DEFAULT_QUEUE_SIZE)
            : base()
        {
            VuStandardMsgFilter smf = new VuStandardMsgFilter();
            messageQueue_ = new VuMessageQueue(queueSize, smf);
        }
        //public  virtual ~VuThread();

        public override VuMainMessageQueue MainQueue()
        {
            return null;
        }
    }


    public class VuMainThread : VuBaseThread
    {
        public static VuMainThread vuMainThread;

        public delegate VuSessionEntity SessionCtorFunc();

        public const int VU_DEFAULT_QUEUE_SIZE = 100;

        // initializes database, etc.
        public VuMainThread(int dbSize, VuMessageFilter filter,
            int queueSize = VU_DEFAULT_QUEUE_SIZE,
            SessionCtorFunc sessionCtorFunc = null)
            : base()
        {
            if (filter != null)
            {
                messageQueue_ = new VuMainMessageQueue(queueSize, filter);
            }
            else
            {
                VuStandardMsgFilter smf = new VuStandardMsgFilter();
                messageQueue_ = new VuMainMessageQueue(queueSize, smf);
            }

#if VU_AUTO_UPDATE
	autoUpdateList_ = 0;
#endif

            if (VUSTATIC.vuCollectionManager != null || VUSTATIC.vuDatabase != null)
            {
                Console.WriteLine("VU: Warning:  creating second VuMainThread!\n");
            }
            else
            {
                Init(dbSize, sessionCtorFunc);
            }
        }

        public VuMainThread(int dbSize, int queueSize = VU_DEFAULT_QUEUE_SIZE, SessionCtorFunc sessionCtorFunc = null)
            : base()
        {
            VuStandardMsgFilter smf = new VuStandardMsgFilter();
            messageQueue_ = new VuMainMessageQueue(queueSize, smf);

#if VU_AUTO_UPDATE
	autoUpdateList_ = 0;
#endif

            if (VUSTATIC.vuCollectionManager != null || VUSTATIC.vuDatabase != null)
            {
                Console.WriteLine("VU: Warning:  creating second VuMainThread!\n");
            }
            else
            {
                Init(dbSize, sessionCtorFunc);
            }
        }

        //public virtual ~VuMainThread();

        public override VuMainMessageQueue MainQueue()
        {
            return (VuMainMessageQueue)messageQueue_;
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public VU_ERRCODE JoinGame(VuGameEntity ent)
        {
            throw new NotImplementedException();
        }

        public VU_ERRCODE LeaveGame()
        {
            throw new NotImplementedException();
        }
#if VU_USE_COMMS
  VU_ERRCODE InitComms(ComAPIHandle handle, int bufSize=0, int packSize=0,
                       ComAPIHandle reliablehandle = NULL,
                       int relBufSize=0, int relPackSize=0,
                       int resendQueueSize = VU_DEFAULT_QUEUE_SIZE );
  VU_ERRCODE DeinitComms();

  static int FlushOutboundMessages();
  static void ReportXmit(int bytesSent);
  static void ResetXmit();
  static int BytesSent();
  static int BytesPending();
#endif

        protected void Init(int dbSize, SessionCtorFunc sessionCtorFunc)
        {
            // set global, for sneaky internal use...
            vuMainThread = this;

            VUSTATIC.vuCollectionManager = new VuCollectionManager();
            VUSTATIC.vuDatabase = new VuDatabase(dbSize);  // create global database
            VUSTATIC.vuAntiDB = new VuAntiDatabase(1 + dbSize / 8);
            VUSTATIC.vuAntiDB.Init();

            VuGameFilter gfilter = new VuGameFilter();
            VUSTATIC.vuGameList = new VuOrderedList(gfilter);
            VUSTATIC.vuGameList.Init();

            VuTargetFilter tfilter = new VuTargetFilter();
            VUSTATIC.vuTargetList = new VuFilteredList(tfilter);
            VUSTATIC.vuTargetList.Init();
            VUSTATIC.vuTargetListIter = new VuListIterator(VUSTATIC.vuTargetList);

            // create global group
            VUSTATIC.vuGlobalGroup = new VuGlobalGroup();
            VUSTATIC.vuDatabase.Insert(VUSTATIC.vuGlobalGroup);

            VUSTATIC.vuPlayerPoolGroup = null;

            // randomize assignment id
            VuEntity.vuAssignmentId = VuEntity.VuRandomID();
            if (VuEntity.vuAssignmentId < VU_ID.VU_FIRST_ENTITY_ID)
            {
                VuEntity.vuAssignmentId = VU_ID.VU_FIRST_ENTITY_ID;
            }

            // create local session
            if (sessionCtorFunc != null)
            {
                VUSTATIC.vuLocalSessionEntity = sessionCtorFunc();
            }
            else
            {
                VUSTATIC.vuLocalSessionEntity = new VuSessionEntity(VUSTATIC.vuxLocalDomain, "player");
            }

            VUSTATIC.vuLocalSession = VUSTATIC.vuLocalSessionEntity.OwnerId();
            VUSTATIC.vuDatabase.Insert(VUSTATIC.vuLocalSessionEntity);
        }

        protected void UpdateGroupData(VuGroupEntity group)
        {
            int count = 0;

            //	assert(VuHasCriticalSection()); // hmm apparently not required...
            VuSessionsIterator iter = new VuSessionsIterator(group);
            VuSessionEntity sess = iter.GetFirst();

            while (sess != null)
            {
#if TODO
                //assert(false == F4IsBadReadPtr(sess, sizeof *sess));
                if
                (
                    (sess != vuLocalSessionEntity) &&
                    (sess.GetReliableCommsStatus() == VU_CONN_ERROR)
                )
                {
#if DEBUG_KEEPALIVE
			VU_PRINT("VU: timing out session id 0x%x: time = %d, last sig = %d\n",
			(VU_KEY)sess.Id(), vuxRealTime, sess.LastTransmissionTime());
#endif

                    // time out this session
                    sess.CloseSession();
                }
                else
                {
                    count++;
                }
#endif
                sess = iter.GetNext();
            }
        }

        protected int GetMessages()
        {
            int
                count;

            Debug.Assert(CriticalSection.VuHasCriticalSection());
            count = 0;

#if VU_USE_COMMS
	VuListIterator  iter(vuTargetList);
	VuTargetEntity target;

	// Flush all outbound messages into various queues

	target = (VuTargetEntity) iter.GetFirst ();
	
	while (target)
	{
		// attempt to send one packet of each type
		target.FlushOutboundMessageBuffer();
		count += target.GetMessages ();

		// Get the next target
		target = (VuTargetEntity*) iter.GetNext ();
	}

	// Get all messages - and send any in a queue

#endif

            return count;
        }

        const int MAX_TARGETS = 256;

        static VU_KEY last_time = 0;
        static int[] lru_size = new int[MAX_TARGETS];

        protected int SendQueuedMessages()
        {
            VuTargetEntity[] targets = new VuTargetEntity[MAX_TARGETS];

            bool[] used = new bool[MAX_TARGETS];

            VU_KEY now;

            int index,
                size,
                best,
                total,
                count;

            VuTargetEntity target;

            VuListIterator iter = new VuListIterator(VUSTATIC.vuTargetList);

            Debug.Assert(CriticalSection.VuHasCriticalSection());
            total = 0;

            now = VUSTATIC.vuxRealTime;

            // Decay the LRU sizes

            if (now - last_time > 100)
            {
                last_time = now;

                for (index = 0; index < MAX_TARGETS; index++)
                {
                    lru_size[index] /= 2;
                }
            }

            // Build array of targets - for easy indexing later

            target = (VuTargetEntity)iter.GetFirst();
            index = 0;
            while (target != null)
            {
                if (target.IsSession())
                {
                    targets[index] = target;
                    used[index] = false;

                    index++;
                }

                target = (VuTargetEntity)iter.GetNext();
            }

            while (index < MAX_TARGETS)
            {
                targets[index] = null;
                index++;
            }

            // Now until we run out of stuff to send

            do
            {
                count = 0;

                // clear the used per iteration

                for (index = 0; index < MAX_TARGETS; index++)
                {
                    used[index] = false;
                }

                do
                {
                    // find the best - ie. the smallest lru_size
                    best = -1;
                    size = 0x7fffffff;

                    for (index = 0; (targets[index] != null) && (index < MAX_TARGETS); index++)
                    {
                        if ((!used[index]) && (lru_size[index] < size))
                        {
                            best = index;
                            size = lru_size[index];
                        }
                    }

                    // If we found a best target

                    if (best >= 0)
                    {
#if VU_USE_COMMS

                        // attempt to send one packet
                        size = targets[best].SendQueuedMessage();
#endif
                        //				if (size)
                        //				{
                        //					MonoPrint ("Send %08x %3d %d\n", targets[best], size, lru_size[best]);
                        //				}

                        // mark it as used
                        used[best] = true;

                        // add into the lru size
                        lru_size[best] += size;

                        // remember that we send something
                        count += size;
                        total += size;
                    }
                }
                while (best != -1);
            } while (count != 0);

            return total;
        }


        // data
#if VU_USE_COMMS
  static int bytesSent_;
#endif
#if VU_AUTO_UPDATE
  VuRedBlackTree * autoUpdateList_;
#endif
    }


    #region Private and Internal
    public class VuCollectionManager
    {
        public VuCollectionManager()
        {
            collcoll_ = null;
            gridcoll_ = null;
            itercoll_ = null;
            rbitercoll_ = null;
            llkillhead_ = VuTailNode.vuTailNode;
            rbkillhead_ = null;

            VuRBNode.bogusNode = new VuRBNode((VuEntity)null, UInt64.MaxValue);
        }
        //TODO public ~VuCollectionManager();

        public void Register(VuIterator iter)
        {
            CriticalSection.VuEnterCriticalSection();
            iter.nextiter_ = itercoll_;
            itercoll_ = iter;
            CriticalSection.VuExitCriticalSection();
        }

        public void DeRegister(VuIterator iter)
        {
            CriticalSection.VuEnterCriticalSection();
            VuIterator curr = itercoll_;
            VuIterator last = null;

            while (curr != null)
            {
                //assert(false == F4IsBadReadPtr(curr, sizeof *curr)); // JPO CTD
                if (curr == iter)
                {
                    if (last == null)
                    {
                        itercoll_ = curr.nextiter_;
                    }
                    else
                    {
                        last.nextiter_ = curr.nextiter_;
                    }
                    curr.nextiter_ = null;
                    break;
                }
                last = curr;
                curr = curr.nextiter_;
            }
            CriticalSection.VuExitCriticalSection();
        }

        public void RBRegister(VuRBIterator iter)
        {
            CriticalSection.VuEnterCriticalSection();
            iter.rbnextiter_ = rbitercoll_;
            rbitercoll_ = iter;
            CriticalSection.VuExitCriticalSection();
        }
        public void RBDeRegister(VuRBIterator iter)
        {
            CriticalSection.VuEnterCriticalSection();
            VuRBIterator curr = rbitercoll_;
            VuRBIterator last = null;

            while (curr != null)
            {
                //assert(false == F4IsBadReadPtr(curr, sizeof *curr)); // JPO
                if (curr == iter)
                {
                    if (last == null)
                    {
                        rbitercoll_ = curr.rbnextiter_;
                    }
                    else
                    {
                        last.rbnextiter_ = curr.rbnextiter_;
                    }
                    curr.rbnextiter_ = null;
                    break;
                }
                last = curr;
                curr = curr.rbnextiter_;
            }

            CriticalSection.VuExitCriticalSection();
        }

        public void Register(VuCollection coll)
        {
            CriticalSection.VuEnterCriticalSection(); // JPO protect the whole thing
            if (coll != VUSTATIC.vuDatabase)
            {
                coll.nextcoll_ = collcoll_;
                collcoll_ = coll;
            }
            CriticalSection.VuExitCriticalSection();
        }
        public void DeRegister(VuCollection coll)
        {
            CriticalSection.VuEnterCriticalSection();

            VuCollection curr = collcoll_;
            VuCollection last = null;

            while (curr != null)
            {
                if (curr == coll)
                {
                    if (last == null)
                    {
                        collcoll_ = curr.nextcoll_;
                    }
                    else
                    {
                        last.nextcoll_ = curr.nextcoll_;
                    }
                    curr.nextcoll_ = null;
                    break;
                }
                last = curr;
                curr = curr.nextcoll_;
            }

            CriticalSection.VuExitCriticalSection();
        }

        public void GridRegister(VuGridTree grid)
        {
            CriticalSection.VuEnterCriticalSection();
            grid.nextgrid_ = gridcoll_;
            gridcoll_ = grid;
            CriticalSection.VuExitCriticalSection();
        }
        public void GridDeRegister(VuGridTree grid)
        {
            CriticalSection.VuEnterCriticalSection();

            VuGridTree curr = gridcoll_;
            VuGridTree last = null;

            while (curr != null)
            {
                //assert(false == F4IsBadReadPtr(curr, sizeof *curr)); // JPO
                if (curr == grid)
                {
                    if (last == null)
                    {
                        gridcoll_ = curr.nextgrid_;
                    }
                    else
                    {
                        last.nextgrid_ = curr.nextgrid_;
                    }
                    curr.nextgrid_ = null;
                    break;
                }
                last = curr;
                curr = curr.nextgrid_;
            }

            CriticalSection.VuExitCriticalSection();
        }

        public void Add(VuEntity ent)
        {
            CriticalSection.VuEnterCriticalSection();
            for (VuCollection curr = collcoll_; curr != null; curr = curr.nextcoll_)
            {
                if (curr != VUSTATIC.vuDatabase)
                {
                    curr.Insert(ent);
                }
            }
            CriticalSection.VuExitCriticalSection();
        }

        public void Remove(VuEntity ent)
        {
            CriticalSection.VuEnterCriticalSection();

            for (VuCollection curr = collcoll_; curr != null; curr = curr.nextcoll_)
            {
                //assert(false == F4IsBadReadPtr(curr, sizeof *curr)); // JPO
                if (curr != VUSTATIC.vuDatabase)
                {
                    curr.Remove(ent);
                }
            }

            CriticalSection.VuExitCriticalSection();
        }
        public int HandleMove(VuEntity ent, BIG_SCALAR coord1, BIG_SCALAR coord2)
        {
            int retval = 0;
            CriticalSection.VuEnterCriticalSection();

            for (VuGridTree curr = gridcoll_; curr != null; curr = curr.nextgrid_)
            {
                //assert(false == F4IsBadReadPtr(curr, sizeof *curr)); // JPO
                if (!curr.suspendUpdates_)
                {
                    curr.Move(ent, coord1, coord2);
                }
            }

            CriticalSection.VuExitCriticalSection();
            return retval;
        }

        public VU_ERRCODE Handle(VuMessage msg)
        {
            CriticalSection.VuEnterCriticalSection();
            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;

            for (VuCollection curr = collcoll_; curr != null; curr = curr.nextcoll_)
            {
                //assert(false == F4IsBadReadPtr(curr, sizeof *curr)); // JPO
                if (curr != VUSTATIC.vuDatabase)
                {
                    if (curr.Handle(msg) == VU_ERRCODE.VU_SUCCESS)
                    {
                        retval = VU_ERRCODE.VU_SUCCESS;
                    }
                }
            }

            CriticalSection.VuExitCriticalSection();
            return retval;
        }

        public VU_BOOL IsReferenced(VuEntity ent)
        {
            VuIterator curr = itercoll_;

            while (curr != null)
            {
                //assert(false == F4IsBadReadPtr(curr, sizeof *curr)); // JPO
                if (curr.IsReferenced(ent))
                {
                    return true;
                }
                curr = curr.nextiter_;
            }

            return false;
        }

        internal VU_BOOL IsReferenced(VuRBNode node)
        {
            VuRBIterator curr = rbitercoll_;

            // 2002-02-04 ADDED BY S.G. If node is false, then it can't be a valid entity, right? That's what I think too :-)
            if (node == null)
                return false;

            while (curr != null)
            {
                //assert(false == F4IsBadReadPtr(curr, sizeof *curr)); // JPO
                if (curr.curnode_ == node)
                {
                    return true;
                }
                curr = curr.rbnextiter_;
            }

            return false;
        }
        public void Purge()
        {
            CriticalSection.VuEnterCriticalSection();

            // LinkNode cleanup
            VuLinkNode firstlast = new VuLinkNode(null, null);
            VuLinkNode curr;
            VuLinkNode last = firstlast;

            firstlast.freenext_ = llkillhead_;
            curr = llkillhead_;

            while (
                curr != null && //!F4IsBadReadPtr(curr, sizeof(VuLinkNode)) && // JB 010318 CTD (too much CPU)
                curr != VuTailNode.vuTailNode)
            {
                //assert(false == F4IsBadReadPtr(curr, sizeof *curr));
                if (IsReferenced(curr.entity_))
                {
                    last = curr;
                    curr = curr.freenext_;
                }
                else
                {
                    last.freenext_ = curr.freenext_;
                    //TODO delete curr;
                    curr = last.freenext_;
                }
            }
            llkillhead_ = firstlast.freenext_;

            // RBNode cleanup
            //	VuRBNode* rbnewhead = 0;
            VuRBNode rblast = null;
            VuRBNode tmp = null;
            VuRBNode rbcurr;

            rbcurr = rbkillhead_;
            rbkillhead_ = null;

            while (rbcurr != null)
            {
                if (IsReferenced(rbcurr))
                {
                    if (rbkillhead_ == null)
                    {
                        rbkillhead_ = rbcurr;
                    }
                    rblast = rbcurr;
                    rbcurr = rbcurr.parent_;
                }
                else
                {
                    if (rblast != null)
                    {
                        rblast.parent_ = rbcurr.parent_;
                    }
                    tmp = rbcurr.parent_;
                    //delete rbcurr;
                    rbcurr = tmp;
                }
            }

            CriticalSection.VuExitCriticalSection();
        }
        internal void PutOnKillQueue(VuLinkNode node, VU_BOOL killnow = false)
        {
            Debug.Assert(CriticalSection.VuHasCriticalSection()); // JB 010614

            if (killnow)
            {
                node.next_ = VuTailNode.vuTailNode;
            }
            else
            {
                // need to ensure that newly killed node has no dead references
                VuLinkNode curr = llkillhead_;
                if (curr == node)
                    return; // JB already in the queue

                while (curr != VuTailNode.vuTailNode)
                {
                    //assert(false == F4IsBadReadPtr(curr, sizeof *curr)); // JPO
                    if (curr.next_ == node)
                    {
                        curr.next_ = node.next_;
                    }

                    if (curr == curr.freenext_) // JB 010607 infinite loop
                    {
                        Debug.Assert(false);
                        break;
                    }

                    curr = curr.freenext_;
                }
            }
            node.freenext_ = llkillhead_;
            llkillhead_ = node;
        }
        internal void PutOnKillQueue(VuRBNode node, VU_BOOL killnow = false)
        {
            Debug.Assert(CriticalSection.VuHasCriticalSection()); // JB 010614

            if (killnow)
            {
                node.next_ = null;
            }
            else
            {
                // need to ensure that newly killed node has no dead references
                VuRBNode curr = rbkillhead_;

                if (curr == node)
                    return; // JB already in the queue

                while (curr != null)
                {
                    //assert(false == F4IsBadReadPtr(curr, sizeof *curr)); // JPO
                    if (curr.next_ == node)
                    {
                        curr.next_ = node.next_;
                    }

                    if (curr == curr.parent_) // JB 010607 infinite loop
                    {
                        Debug.Assert(false);
                        break;
                    }

                    curr = curr.parent_;
                }
            }
            node.parent_ = rbkillhead_;
            rbkillhead_ = node;
        }
        public void Shutdown(VU_BOOL all)
        {
            CriticalSection.VuEnterCriticalSection();
            SanitizeKillQueue();
            for (VuCollection curr = collcoll_; curr != null; curr = curr.nextcoll_)
            {
                //assert(false == F4IsBadReadPtr(curr, sizeof *curr)); // JPO
                if (curr != VUSTATIC.vuDatabase)
                {
                    curr.Purge(all);
                }
            }
            VUSTATIC.vuDatabase.Suspend(all);
            CriticalSection.VuExitCriticalSection();
        }

        public void SanitizeKillQueue()
        {
            CriticalSection.VuEnterCriticalSection();

            // LinkNode sanitize
            VuLinkNode curr;

            curr = llkillhead_;
            while (curr != VuTailNode.vuTailNode)
            {
                //assert(false == F4IsBadReadPtr(curr, sizeof *curr)); // JPO
                curr.next_ = VuTailNode.vuTailNode;
                curr = curr.freenext_;
            }

            // RBNode sanitize
            VuRBNode rbcurr;

            rbcurr = rbkillhead_;
            while (rbcurr != null)
            {
                rbcurr.next_ = null;
                rbcurr = rbcurr.parent_;
            }

            CriticalSection.VuExitCriticalSection();
        }


        public int FindEnt(VuEntity ent)
        {
            int retval = 0;
            CriticalSection.VuEnterCriticalSection();

            for (VuCollection curr = collcoll_; curr != null; curr = curr.nextcoll_)
            {
                //assert(false == F4IsBadReadPtr(curr, sizeof *curr)); // JPO
                VuEntity ent2 = curr.Find(ent);
                if (ent2 != null && ent2 == ent)
                {

#if DEBUG
                    Console.WriteLine("-. collection type 0x%x: Found entity 0x%x\n", curr.Type(), ent.Id());
#endif

                    retval++;
                }
            }
            CriticalSection.VuExitCriticalSection();
            return retval;
        }

        public VuCollection collcoll_;	// ;-)
        public VuGridTree gridcoll_;
        public VuIterator itercoll_;
        public VuRBIterator rbitercoll_;
        internal VuLinkNode llkillhead_;
        internal VuRBNode rbkillhead_;
    }

    internal class VuListMuckyIterator : VuIterator
    {

        public VuListMuckyIterator(VuLinkedList coll)
            : base(coll)
        {
            curr_ = VuTailNode.vuTailNode;
            last_ = null;
            VUSTATIC.vuCollectionManager.Register(this);
        }

        //TODO public virtual ~VuListMuckyIterator();

        public VuEntity GetFirst()
        {
            last_ = null; curr_ = ((VuLinkedList)collection_).head_;
            return curr_.entity_;
        }

        public VuEntity GetNext()
        {
            last_ = curr_;
            curr_ = curr_.next_;
            return curr_.entity_;
        }

        public virtual void InsertCurrent(VuEntity ent)
        {
            CriticalSection.VuEnterCriticalSection();

            if (curr_ != ((VuLinkedList)collection_).head_)
            {
                // ensure that last_ is not deleted
                if (last_ == null || last_.freenext_ != null)
                {
                    last_ = ((VuLinkedList)collection_).head_;

                    while (last_.entity_ != null && last_.next_ != curr_)
                    {
                        last_ = last_.next_;
                    }
                }

                if (curr_.freenext_ != null)
                {
                    curr_ = last_.next_;
                }

                last_.next_ = new VuLinkNode(ent, curr_);
            }
            else
            {
                ((VuLinkedList)collection_).head_ = new VuLinkNode(ent, ((VuLinkedList)collection_).head_);
            }

            CriticalSection.VuExitCriticalSection();
        }

        public virtual void RemoveCurrent()
        {
            if (curr_.freenext_ != null)
            {
                // already done
                return;
            }

            CriticalSection.VuEnterCriticalSection();

            if (curr_ != ((VuLinkedList)collection_).head_)
            {
                // ensure that last_ is not deleted
                if (last_ == null || last_.freenext_ != null)
                {
                    last_ = ((VuLinkedList)collection_).head_;

                    while (last_.entity_ != null && last_.next_ != curr_)
                    {
                        last_ = last_.next_;
                    }
                }

                last_.next_ = curr_.next_;
            }
            else
            {
                ((VuLinkedList)collection_).head_ = curr_.next_;
            }
            // put curr on VUs pending delete queue

            VUSTATIC.vuCollectionManager.PutOnKillQueue(curr_);
            curr_ = curr_.next_;

            CriticalSection.VuExitCriticalSection();
        }
        public override VU_BOOL IsReferenced(VuEntity ent)
        {
            // 2002-02-04 MODIFIED BY S.G. If ent is false, then it can't be a valid entity, right? That's what I think too :-)
            //	if ((last_ && last_.entity_ == ent) || curr_.entity_ == ent)
            if (ent != null && ((last_ != null && last_.entity_ == ent) || curr_.entity_ == ent))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override VuEntity CurrEnt()
        {
            return curr_.entity_;
        }


        public override VU_ERRCODE Cleanup()
        {
            curr_ = VuTailNode.vuTailNode;
            return VU_ERRCODE.VU_SUCCESS;
        }

        public VuLinkNode curr_;
        public VuLinkNode last_;
    }
    public class VuGameFilter : VuFilter
    {

        public VuGameFilter()
            : base()
        {
            // empty
        }
        //TODO public virtual ~VuGameFilter();

        public override VU_BOOL Test(VuEntity ent)
        {
            //  if (ent == vuPlayerPoolGroup) return false;
            return ent.IsGame();
        }
        public override VU_BOOL RemoveTest(VuEntity ent)
        {
            return ent.IsGame();
        }

        public override int Compare(VuEntity ent1, VuEntity ent2)
        {
            if ((VU_KEY)ent2.Id() > (VU_KEY)ent1.Id())
            {
                return -1;
            }
            else if ((VU_KEY)ent2.Id() < (VU_KEY)ent1.Id())
            {
                return 1;
            }

            return 0;
        }
        // returns ent2->Id() - ent1->Id()
        public override VuFilter Copy()
        {
            return new VuGameFilter(this);
        }

        protected VuGameFilter(VuGameFilter other)
            : base(other)
        {
            // empty
        }
    }
    public class VuTargetFilter : VuFilter
    {
        public VuTargetFilter()
            : base()
        {
            // empty
        }
        //TODO public virtual ~VuTargetFilter();

        public override VU_BOOL Test(VuEntity ent)
        {
            return ent.IsTarget();
        }
        public override VU_BOOL RemoveTest(VuEntity ent)
        {
            return ent.IsTarget();
        }
        public override int Compare(VuEntity ent1, VuEntity ent2)
        {
            if ((VU_KEY)ent2.Id() > (VU_KEY)ent1.Id())
            {
                return -1;
            }
            else if ((VU_KEY)ent2.Id() < (VU_KEY)ent1.Id())
            {
                return 1;
            }
            return 0;
        }
        // returns ent2->Id() - ent1->Id()
        public override VuFilter Copy()
        {
            return new VuTargetFilter(this);
        }

        protected VuTargetFilter(VuTargetFilter other)
            : base(other)
        {
            // empty
        }
    }
    #endregion
}
