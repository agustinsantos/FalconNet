using VU_MSG_TYPE = System.Byte;
namespace FalconNet.VU
{
    public abstract class VuRequestMessage : VuMessage
    {

        //TODO public virtual ~VuRequestMessage();

        protected VuRequestMessage(VU_MSG_TYPE type, VU_ID entityid, VuTargetEntity target)
            : base(type, entityid, target, false)
        {
            // empty
        }
        protected VuRequestMessage(VU_MSG_TYPE type, VU_ID senderid, VU_ID dest)
            : base(type, senderid, dest)
        {
            // empty
        }
        //protected abstract VU_ERRCODE Process(VU_BOOL autod);
    }


}
