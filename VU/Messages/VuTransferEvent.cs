using VU_BOOL = System.Boolean;
using VU_MSG_TYPE = System.Byte;

namespace FalconNet.VU
{
    //--------------------------------------------------
    public class VuTransferEvent : VuEvent
    {

        public VuTransferEvent(VuEntity entity, VuTargetEntity target, VU_BOOL loopback = false)
            : base(VU_MSG_DEF.VU_TRANSFER_EVENT, entity.Id(), target, loopback)
        {
            newOwnerId_ = entity.OwnerId();
            SetEntity(entity);
        }
        public VuTransferEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_DEF.VU_TRANSFER_EVENT, senderid, target)
        {
            newOwnerId_ = VU_ID.vuNullId;
            // empty
        }
        //TODO public virtual ~VuTransferEvent();

        protected override VU_ERRCODE Activate(VuEntity ent)
        {
            return base.Activate(ent);
        }
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
        public VU_ID newOwnerId_;
    }


}
