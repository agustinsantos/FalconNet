using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VU_BOOL = System.Boolean;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;
using SM_SCALAR = System.Single;
using BIG_SCALAR = System.Single;
using VU_DAMAGE = System.UInt64;
using System.IO;

namespace FalconNet.VU
{
    public abstract class VuMessageFilter
    {
        //VuMessageFilter() { }
        //virtual ~VuMessageFilter() { }
        public abstract VU_BOOL Test(VuMessage evnt);
        public abstract VuMessageFilter Copy();
    }

    /// <summary>
    /// The VuNullMessageFilter lets everything through
    /// </summary>
    public class VuNullMessageFilter : VuMessageFilter
    {
        public static readonly VuNullMessageFilter vuNullFilter = new VuNullMessageFilter();
        //VuNullMessageFilter() : VuMessageFilter() { }
        //virtual ~VuNullMessageFilter() { }
        public override VU_BOOL Test(VuMessage evnt)
        {
            return true;
        }

        public override VuMessageFilter Copy()
        {
            return new VuNullMessageFilter();
        }
    }

    /// <summary>
    /// provided default filters
    /// </summary>
    public class VuMessageTypeFilter : VuMessageFilter
    {

        public VuMessageTypeFilter(ulong bitfield)
        {
            msgTypeBitfield_ = bitfield;
        }

        // virtual ~VuMessageTypeFilter();
        public override VU_BOOL Test(VuMessage evnt)
        {
            return ((1 << (int)evnt.Type()) != 0 && msgTypeBitfield_ != 0) ? true : false;
        }
        public override VuMessageFilter Copy()
        {
            return new VuMessageTypeFilter(msgTypeBitfield_);
        }

        protected ulong msgTypeBitfield_;
    }

    // the VuStandardMsgFilter allows only these events:
    //   - VuEvent(s) from a remote session
    //   - All Delete and Release events
    //   - All Create, FullUpdate, and Manage events
    // it filters out these messages:
    //   - All update events on unknown entities
    //   - All update events on local entities
    //   - All non-event messages (though this can be overridden)
    public class VuStandardMsgFilter : VuMessageFilter
    {

        public VuStandardMsgFilter(VUBITS bitfield = VUBITS.VU_VU_EVENT_BITS)
        {
            msgTypeBitfield_ = bitfield;
        }

        //virtual ~VuStandardMsgFilter();
        public override VU_BOOL Test(VuMessage message)
        {
            VUBITS eventBit = (VUBITS)(1 << (int)message.Type());
            if ((eventBit & msgTypeBitfield_) == 0)
            {
                return false;
            }
            if ((eventBit & (VUBITS.VU_DELETE_EVENT_BITS | VUBITS.VU_CREATE_EVENT_BITS)) != 0)
            {
                return true;
            }
            if (message.Sender() == VUSTATIC.vuLocalSession)
            {
                return false;
            }
            // test to see if entity was found in database
            if (message.Entity() != null)
            {
                return true;
            }
            return false;
        }
        public override VuMessageFilter Copy()
        {
            return new VuStandardMsgFilter(msgTypeBitfield_);
        }

        protected VUBITS msgTypeBitfield_;
    }



    public delegate VU_BOOL EvalFunc(VuMessage msg, Object arg);

    public class VuMessageQueue
    {
        public VuMessageQueue(int queueSize, VuMessageFilter filter = null)
        {
            head_ = new VuMessage[queueSize];
            read_ = 0;
            write_ = 0;
            tail_ = queueSize;
#if _DEBUG
    _count = 0;
#endif

            // initialize queue
            for (int i = 0; i < queueSize; i++)
            {
                head_[i] = null;
            }
            if (filter == null)
            {
                filter = VuNullMessageFilter.vuNullFilter;
            }
            filter_ = filter.Copy();

            // add this queue to list of queues
            CriticalSection.VuEnterCriticalSection();
            nextqueue_ = queuecollhead_;
            queuecollhead_ = this;
            CriticalSection.VuExitCriticalSection();
        }

        //TODO public  ~VuMessageQueue();

        public VuMessage PeekVuMessage() { throw new NotImplementedException(); }
        public virtual VuMessage DispatchVuMessage(VU_BOOL autod = false)
        {
            VuMessage retval = null;
            CriticalSection.VuEnterCriticalSection();

            if (head_[read_] != null)
            {
#if DEBUG_ON
    VU_PRINT("VU: Dispatching message: ");
#endif
                retval = head_[read_];
                head_[read_++] = null;
#if _DEBUG
    _count --;
#endif
                if (read_ == tail_)
                {
                    read_ = 0;
                }
                retval.Ref();
#if FALCON4
	if (CritialSectionCount(vuCritical) != 1 && !autod) {
      VU_PRINT("VU: Dispatching message while holding a critical section.\n");
	}
#endif
                CriticalSection.VuExitCriticalSection();
                retval.Dispatch(autod);
                retval.UnRef();
                retval.UnRef();
#if DEBUG_ON
    VU_PRINT("\n");
#endif
                return retval;
            }
            CriticalSection.VuExitCriticalSection();
            return retval;
        }

        public int DispatchAllMessages(VU_BOOL autod = false)
        {
#if _DEBUG
    static int interestlevel = 20;
 //   if (_count > interestlevel)
//	MonoPrint ("Dispatching %d messages\n", _count);//me123
#endif
            int i = 0;
            while (DispatchVuMessage(autod) != null)
            {
                i++;
            }
#if _DEBUG
  vumessagecount = i;
  if (vumessagecount > vumessagepeakcount)
	  vumessagepeakcount = vumessagecount;
#endif
            return i;
        }

        public static int PostVuMessage(VuMessage msg) { throw new NotImplementedException(); }
        public static void FlushAllQueues() { throw new NotImplementedException(); }
        public static int InvalidateMessages(EvalFunc evalFunc, Object arg) { throw new NotImplementedException(); }
        public int InvalidateQueueMessages(EvalFunc evalFunc, Object arg) { throw new NotImplementedException(); }


        // called when queue is about to wrap -- default does nothing & returns false
        protected virtual VU_BOOL ReallocQueue()
        {
#if REALLOC_QUEUES
          int size = (tail_ - head_)*2;
          VuMessage	**newhead, **cp, **rp;
  
          newhead = new VuMessage*[size];
          cp      = newhead;

          for (rp = read_; rp != tail_; cp++,rp++)
            *cp = *rp;

          for (rp = head_; rp != write_; cp++,rp++)
            *cp = *rp;
  
          delete[] head_;
          head_  = newhead;
          read_  = head_;
          write_ = cp;
          tail_  = head_ + size;

          while (cp != tail_)
	          *cp++ = 0;

#if DEBUG
  FILE* fp = fopen ("vuerror.log","a");
  if (fp) {
    fprintf(fp,"Reallocating queue from size %d to size %d.\n",size/2,size);
    fclose(fp);
  }
#endif

  return true;

#else
            return false;
#endif
        }

        protected virtual int AddMessage(VuMessage evnt) // called only by PostVuMessage()
        {
            // JB 010121
            if (evnt == null ||
                filter_ == null) // JB 010715
                return 0;

            int retval = 0;
            if (filter_.Test(evnt) && evnt.Type() != VU_MSG_TYPE.VU_TIMER_EVENT)
            {
                retval = 1;
                evnt.Ref();
                head_[write_++] = evnt;
                if (write_ == tail_)
                {
                    write_ = 0;
                }
#if _DEBUG
		_count ++;
#endif

#if DEBUG_ON
		VU_PRINT("VU: Added message\n");
#endif

                if (write_ == read_ && head_[read_] != null)
                {
                    if (!ReallocQueue() && write_ == read_ && head_[read_] != null)
                    {
                        // do simple dispatch -- cannot be handled by user
                        // danm_note: should we issue a warning here?
                        DispatchVuMessage(true);

#if DEBUG_AUTO_DISPATCH
				VU_PRINT("VU: Auto message dispatch: sender %d; msg id %u\n",
					event->MsgId().num_, event->MsgId().id_);
#endif
                    }
                }
            }
            return retval;
        }

        protected static void RepostMessage(VuMessage msg, int delay)
        {
#if DEBUG_COMMS
	VU_PRINT("Send connection still pending... reposting at time %d\n", vuxRealTime+(delay * VU_TICS_PER_SECOND));
#endif
            msg.flags_ |= ~VU_MSG_FLAG.VU_LOOPBACK_MSG_FLAG;
            VuTimerEvent timer = new VuTimerEvent(null, VUSTATIC.vuxRealTime + (ulong)delay, TIMERTYPES.VU_DELAY_TIMER, msg);
            VuMessageQueue.PostVuMessage(timer);
        }
        // DATA

        protected VuMessage[] head_;	// also queue mem store
        protected int read_;
        protected int write_;
        protected int tail_;

        protected VuMessageFilter filter_;
#if _DEBUG
  int _count; // JPO - see what occupancy is like
#endif


        private static VuMessageQueue queuecollhead_;
        private VuMessageQueue nextqueue_;

    }

    public class VuMainMessageQueue : VuMessageQueue
    {
        public VuMainMessageQueue(int queueSize, VuMessageFilter filter = null)
            : base(queueSize, filter)
        {
            timerlisthead_ = null;
        }
        //~VuMainMessageQueue();

        public override VuMessage DispatchVuMessage(VU_BOOL autod = false) { throw new NotImplementedException(); }
        protected virtual int AddMessage(VuMessage evnt) // called only by PostVuMessage()
        { throw new NotImplementedException(); }

        // DATA
        protected VuTimerEvent timerlisthead_;
    }

    public class VuPendingSendQueue : VuMessageQueue
    {
        private static VuResendMsgFilter resendMsgFilter = new VuResendMsgFilter();

        public VuPendingSendQueue(int queueSize) : base(queueSize, resendMsgFilter) { throw new NotImplementedException(); }
        //TODO ~VuPendingSendQueue();

        public override VuMessage DispatchVuMessage(VU_BOOL autod = false) { throw new NotImplementedException(); }
        public void RemoveTarget(VuTargetEntity target) { throw new NotImplementedException(); }

        public int BytesPending() { return bytesPending_; }

        protected override int AddMessage(VuMessage evnt) // called only by PostVuMessage()
        { throw new NotImplementedException(); }

        // DATA
        protected int bytesPending_;
    }

}
