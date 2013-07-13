using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{
    //--------------------------------------------------
    // the VuResendMsgFilter is used to hold messages for which send failed:
    //   - IsLocal() is true
    //   - VU_SEND_FAILED_MSG_FLAG is set
    //   - either VU_RELIABLE_MSG_FLAG or VU_KEEPALIVE_MSG_FLAG are set
    internal class VuResendMsgFilter : VuMessageFilter
    {

        public VuResendMsgFilter()
        {
            // emtpy
        }

        //TODO public virtual ~VuResendMsgFilter();
        public override VU_BOOL Test(VuMessage message)
        {
#if NOTHING
  if ((message.Flags() & VU_SEND_FAILED_MSG_FLAG) &&
      ((message.Flags() & VU_RELIABLE_MSG_FLAG) ||
       (message.Flags() & VU_KEEPALIVE_MSG_FLAG)) ) {
    return TRUE;
  }
  return FALSE;
#else
            if ((message.Flags() & VU_MSG_FLAG.VU_SEND_FAILED_MSG_FLAG) != 0 &&
                (message.Flags() & (VU_MSG_FLAG.VU_RELIABLE_MSG_FLAG | VU_MSG_FLAG.VU_KEEPALIVE_MSG_FLAG)) != 0)
                return true;
            else
                return false;
#endif
        }

        public override VuMessageFilter Copy()
        {
            return new VuResendMsgFilter();
        }

    }

}
