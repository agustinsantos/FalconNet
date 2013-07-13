using VU_BOOL = System.Boolean;
using VU_MSG_TYPE = System.Byte;

namespace FalconNet.VU
{
    //--------------------------------------------------
    public class VuUnknownMessage : VuMessage
    {

        public VuUnknownMessage()
            : base(VU_MSG_DEF.VU_UNKNOWN_MESSAGE, VU_ID.vuNullId, VUSTATIC.vuLocalSessionEntity, true)
        {
            // empty
        }
        //TODO public virtual ~VuUnknownMessage();

        public override VU_BOOL DoSend()     // returns false
        {
            return false;
        }

        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            return VU_ERRCODE.VU_NO_OP;
        }
    }

}
