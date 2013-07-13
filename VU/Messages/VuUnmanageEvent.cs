using VU_BOOL = System.Boolean;
using VU_TIME = System.UInt64;

namespace FalconNet.VU
{
    public class VuUnmanageEvent : VuEvent
    {

        public VuUnmanageEvent(VuEntity entity, VuTargetEntity target,
                              VU_TIME mark, VU_BOOL loopback = false)
            : base(VU_MSG_DEF.VU_UNMANAGE_EVENT, entity.Id(), target, loopback)
        {
            mark_ = mark;

            SetEntity(entity);
            // set kill fuse
            VuReleaseEvent release = new VuReleaseEvent(entity);
            VuTimerEvent timer = new VuTimerEvent(null, mark, TIMERTYPES.VU_DELETE_TIMER, release);
            VuMessageQueue.PostVuMessage(timer);
        }

        public VuUnmanageEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_DEF.VU_UNMANAGE_EVENT, senderid, target)
        {
            mark_ = 0;
            // empty
        }

        //TODO public virtual ~VuUnmanageEvent();


        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            if (Entity() != null)
            {
                Entity().Handle(this);
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        // data

        // time of removal
        public VU_TIME mark_;
    }

}
