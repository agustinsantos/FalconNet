using System;
using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{
    //--------------------------------------------------
    public class VuShutdownEvent : VuEvent
    {

        public VuShutdownEvent(VU_BOOL all = false)
            : base(VU_MSG_DEF.VU_SHUTDOWN_EVENT, VU_ID.vuNullId, VUSTATIC.vuLocalSessionEntity, true)
        {
            shutdownAll_ = all;
            done_ = false;
            // empty
        }
        //TODO public virtual ~VuShutdownEvent();

        public override VU_BOOL DoSend()     // returns false
        { throw new NotImplementedException(); }

        protected override VU_ERRCODE Process(VU_BOOL autod) { throw new NotImplementedException(); }

        // data

        public VU_BOOL shutdownAll_;
        public VU_BOOL done_;
    }

}
