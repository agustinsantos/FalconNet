using FalconNet.Common.Encoding;
using System;
using System.Diagnostics;
using System.IO;
using VU_BOOL = System.Boolean;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;
using VU_MSG_TYPE = System.Byte;

namespace FalconNet.VU
{
    public abstract class VuMessage
    {

#if USE_SH_POOLS
public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(gVuMsgMemPool,size,false);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        //TODO public virtual ~VuMessage();

        public VU_MSG_TYPE Type() { return type_; }
        public VU_ID Sender() { return sender_; }
        public VU_ID Destination() { return tgtid_; }
        public VU_BOOL IsLocal()
        {
            return sender_.creator_ == VUSTATIC.vuLocalSession.creator_ ?
                                true : false;
        }
        public VU_ID EntityId() { return entityId_; }
        public VU_MSG_FLAG Flags() { return flags_; }
        public VuEntity Entity() { return ent_; }
        public VU_TIME PostTime() { return postTime_; }
        public VuTargetEntity Target() { return target_; }

        public void SetPostTime(VU_TIME posttime) { postTime_ = posttime; }

        public VU_ERRCODE Dispatch(VU_BOOL autod)
        {
            //if (F4IsBadReadPtr(this, sizeof(VuMessage))) // JB 010318 CTD
            //return VU_ERROR; // JB 010318 CTD

            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;

            if (!IsLocal() || (flags_ & VU_MSG_FLAG.VU_LOOPBACK_MSG_FLAG) != 0)
            {
                //assert(FALSE == F4IsBadReadPtr(vuDatabase, sizeof *vuDatabase));
                if (Entity() == null)
                {
                    // try to find ent again -- may have been in queue
                    VuEntity ent = VUSTATIC.vuDatabase.Find(entityId_);
                    if (ent != null)
                    {
                        Activate(ent);
                    }
                }
                retval = Process(autod);

                //if (F4IsBadCodePtr((FARPROC)VUSTATIC.vuDatabase)) // JB 010404 CTD
                //        return VU_ERRCODE.VU_ERROR;

                VUSTATIC.vuDatabase.Handle(this);
                // mark as sent
                flags_ |= VU_MSG_FLAG.VU_PROCESSED_MSG_FLAG;
            }

            return retval;
        }

        public VU_ERRCODE Send()
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_ERROR;
            if (Target() != null && Target() != VUSTATIC.vuLocalSessionEntity)
            {

#if VU_USE_COMMS
    retval = Target().SendMessage(this);
#endif

                if (retval <= 0)
                    flags_ |= VU_MSG_FLAG.VU_SEND_FAILED_MSG_FLAG;
            }
            return retval;
        }


        public void RequestLoopback() { flags_ |= VU_MSG_FLAG.VU_LOOPBACK_MSG_FLAG; }
        public void RequestReliableTransmit() { flags_ |= VU_MSG_FLAG.VU_RELIABLE_MSG_FLAG; }
        public void RequestOutOfBandTransmit() { flags_ |= VU_MSG_FLAG.VU_OUT_OF_BAND_MSG_FLAG; }
        public void RequestLowPriorityTransmit() { flags_ &= (VU_MSG_FLAG)0xf0; }

        // app needs to Ref & UnRef messages they keep around
        // 	most often this need not be done
        public int Ref()
        {
            return ++refcnt_;
        }

        public int UnRef()
        {
            // NOTE: must assign temp here as memory may be freed prior to return
            int retval = --refcnt_;
            if (refcnt_ <= 0)
                throw new NotImplementedException();
            //delete this;

            return retval;
        }

        // the following determines just prior to sending message whether or not
        // it goes out on the wire (default is true, of course)
        public virtual VU_BOOL DoSend()
        {
            return true;
        }

        protected VuMessage(VU_MSG_TYPE type, VU_ID entityId, VuTargetEntity target,
                      VU_BOOL loopback)
        {
            refcnt_ = 0;
            type_ = type;
            flags_ = VU_MSG_FLAG.VU_NORMAL_PRIORITY_MSG_FLAG;
            entityId_ = entityId;
            target_ = target;
            postTime_ = 0;
            ent_ = null;
            if (target == VUSTATIC.vuLocalSessionEntity)
            {
                loopback = true;
            }
            if (target != null)
            {
                target_ = target;
            }
            else if (VUSTATIC.vuGlobalGroup != null)
            {
                target_ = VUSTATIC.vuGlobalGroup;
            }
            else
            {
                target_ = VUSTATIC.vuLocalSessionEntity;
            }
            if (target_ != null)
            {
                tgtid_ = target_.Id();
            }
            if (loopback)
            {
                flags_ |= VU_MSG_FLAG.VU_LOOPBACK_MSG_FLAG;
            }
            // note: msg id is set only for external messages which are sent out
            sender_.num_ = VUSTATIC.vuLocalSession.num_;
            sender_.creator_ = VUSTATIC.vuLocalSession.creator_;
        }

        protected VuMessage(VU_MSG_TYPE type, VU_ID sender, VU_ID target)
        {
            refcnt_ = 0;
            type_ = type;
            flags_ = VU_MSG_FLAG.VU_REMOTE_MSG_FLAG;
            sender_ = sender;
            tgtid_ = target;
            entityId_ = new VU_ID(0, 0);
            target_ = null;
            postTime_ = 0;
            ent_ = null;

        }

        protected virtual VU_ERRCODE Activate(VuEntity ent)
        {
            SetEntity(ent);
            return VU_ERRCODE.VU_SUCCESS;
        }
        protected abstract VU_ERRCODE Process(VU_BOOL autod);

        protected VuEntity SetEntity(VuEntity ent)
        {
            // basically try and catch the bad case (ref/deref now swapped)
            // its ok if 0, cos nothing bad will happen
            // its ok if they are not the same entity as we don't care then
            // its ok if the refcount is more than 1, cos the deref wouldn't destroy it anyway
            Debug.Assert(ent == null || ent != ent_ || ent.RefCount() > 1); // JPO test

            if (ent != null)
                VuEntity.VuReferenceEntity(ent);

            if (ent_ != null)
                VuEntity.VuDeReferenceEntity(ent_);

            ent_ = ent;
            return ent_;
        }


        private VU_BYTE refcnt_;		// vu references


        protected VU_MSG_TYPE type_;
        internal VU_MSG_FLAG flags_;		// misc flags
        protected VU_ID sender_;
        protected VU_ID tgtid_;
        internal VU_ID entityId_;
        // scratch variables (not networked)
        protected VuTargetEntity target_;
        protected VU_TIME postTime_;

        private VuEntity ent_;
    }


    public static class VuMessageEncodingLE
    {
        public static void Encode(Stream stream, VuMessage val)
        {
            VU_IDEncodingLE.Encode(stream, val.entityId_);
        }

        public static void Decode(Stream stream, VuMessage rst)
        {
             VU_IDEncodingLE.Decode(stream, rst.entityId_);
        }

        public static int Size
        {
            get { return VU_IDEncodingLE.Size; }
        }
    }

}
