using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{

    // the VuStandardMsgFilter allows only these events:
    //   - VuEvent(s) from a remote session
    //   - All Delete and Release events
    //   - All Create, FullUpdate, and Manage events
    // it filters out these messages:
    //   - All update events on unknown entities
    //   - All update events on local entities
    //   - All non-event messages (though this can be overridden)
    public class VuStandardMsgFilter : VuMessageFilter
    {

        public VuStandardMsgFilter(VUBITS bitfield = VUBITS.VU_VU_EVENT_BITS)
        {
            msgTypeBitfield_ = bitfield;
        }

        //virtual ~VuStandardMsgFilter();
        public override VU_BOOL Test(VuMessage message)
        {
            VUBITS eventBit = (VUBITS)(1 << (int)message.Type());
            if ((eventBit & msgTypeBitfield_) == 0)
            {
                return false;
            }
            if ((eventBit & (VUBITS.VU_DELETE_EVENT_BITS | VUBITS.VU_CREATE_EVENT_BITS)) != 0)
            {
                return true;
            }
            if (message.Sender() == VUSTATIC.vuLocalSession)
            {
                return false;
            }
            // test to see if entity was found in database
            if (message.Entity() != null)
            {
                return true;
            }
            return false;
        }

        public override VuMessageFilter Copy()
        {
            return new VuStandardMsgFilter(msgTypeBitfield_);
        }

        protected VUBITS msgTypeBitfield_;
    }
}
