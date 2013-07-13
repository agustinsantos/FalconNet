using System;
using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{
    //--------------------------------------------------
    public class VuBroadcastGlobalEvent : VuEvent
    {

        public VuBroadcastGlobalEvent(VuEntity entity, VuTargetEntity target, VU_BOOL loopback = false)
            : base(VU_MSG_DEF.VU_BROADCAST_GLOBAL_EVENT, entity.Id(), target, loopback)
        {

#if VU_USE_CLASS_INFO
	memcpy(classInfo_, entity.EntityType().classInfo_, CLASS_NUM_BYTES);
#endif


            vutype_ = entity.Type();

            SetEntity(entity);
            updateTime_ = entity.LastUpdateTime();
        }

        public VuBroadcastGlobalEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_DEF.VU_BROADCAST_GLOBAL_EVENT, senderid, target)
        {
#if VU_USE_CLASS_INFO
	memset(classInfo_, '\0', CLASS_NUM_BYTES);
#endif
            vutype_ = 0;
        }
        //TODO public virtual ~VuBroadcastGlobalEvent();

        public void MarkAsKeepalive() { flags_ |= VU_MSG_FLAG.VU_KEEPALIVE_MSG_FLAG; }

        protected override VU_ERRCODE Activate(VuEntity ent) { throw new NotImplementedException(); }
        protected override VU_ERRCODE Process(VU_BOOL autod) { throw new NotImplementedException(); }

        // data

#if  VU_USE_CLASS_INFO
  VU_BYTE classInfo_[CLASS_NUM_BYTES];	// entity class type
#endif
        protected ushort vutype_;			// entity type

    }

}
