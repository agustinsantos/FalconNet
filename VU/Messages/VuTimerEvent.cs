using VU_BOOL = System.Boolean;
using VU_TIME = System.UInt64;

namespace FalconNet.VU
{
    //--------------------------------------------------
    public class VuTimerEvent : VuEvent
    {

        public VuTimerEvent(VuEntity entity, VU_TIME mark, TIMERTYPES timertype, VuMessage evnt = null)
            : base(VU_MSG_DEF.VU_TIMER_EVENT, (entity != null ? entity.Id() : VU_ID.vuNullId), VUSTATIC.vuLocalSessionEntity, true)
        {
            mark_ = mark;
            timertype_ = timertype;
            event_ = evnt;
            //next_ = null;
            SetEntity(entity);
            if (event_ != null)
            {
                event_.Ref();
            }
        }
        //TODO public virtual ~VuTimerEvent();

        public override VU_BOOL DoSend()
        {
            return false;
        }

        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;

            if (event_ != null)
            {
#if DEBUG_COMMS
    VU_PRINT("VU: Posting timer delayed event id %d -- time = %d\n", msgid_.id_, vuxRealTime);
#endif

                if (event_.Target() != null && event_.Target() != VUSTATIC.vuLocalSessionEntity)//me123 from Target() to event_.Target()
                {
                    retval = event_.Send();
                }
                else
                {
                    retval = event_.Dispatch(false);
                }

                //VuMessageQueue::PostVuMessage(event_);
                event_.UnRef();
                retval = VU_ERRCODE.VU_SUCCESS;
            }

            if (EntityId() != VU_ID.vuNullId)
            {
                if (Entity() != null)
                {
                    Entity().Handle(this);
                    retval++;
                }
            }
            return retval;
        }

        // data

        // time of firing
        public VU_TIME mark_;
        public TIMERTYPES timertype_;
        // event to launch on firing
        public VuMessage event_;


        //private VuTimerEvent next_;
    }

}
