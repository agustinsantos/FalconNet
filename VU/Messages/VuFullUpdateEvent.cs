using System;
using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{

    //--------------------------------------------------
    public class VuFullUpdateEvent : VuCreateEvent
    {

        public VuFullUpdateEvent(VuEntity entity, VuTargetEntity target, VU_BOOL loopback = false)
            : base(VU_MSG_DEF.VU_FULL_UPDATE_EVENT, entity, target, loopback)
        {
            SetEntity(entity);
            updateTime_ = entity.LastUpdateTime();
        }

        public VuFullUpdateEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_DEF.VU_FULL_UPDATE_EVENT, senderid, target)
        {
            // empty
        }
        //TODO public virtual ~VuFullUpdateEvent();

        public VU_BOOL WasCreated() { return (VU_BOOL)(Entity() == expandedData_ ? true : false); }
        public void MarkAsKeepalive() { flags_ |= VU_MSG_FLAG.VU_KEEPALIVE_MSG_FLAG; }


        protected override VU_ERRCODE Activate(VuEntity ent) { throw new NotImplementedException(); }

    }
}
