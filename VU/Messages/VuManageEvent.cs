using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{
    //--------------------------------------------------
    public class VuManageEvent : VuCreateEvent
    {

        public VuManageEvent(VuEntity entity, VuTargetEntity target, VU_BOOL loopback = false)
            : base(VU_MSG_DEF.VU_MANAGE_EVENT, entity, target, loopback)
        {
            // empty
        }

        public VuManageEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_DEF.VU_MANAGE_EVENT, senderid, target)
        {
            // empty
        }

        //TODO public virtual ~VuManageEvent();

    }

}
