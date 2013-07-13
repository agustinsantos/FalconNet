using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{
    /// <summary>
    /// provided default filters
    /// </summary>
    public class VuMessageTypeFilter : VuMessageFilter
    {

        public VuMessageTypeFilter(ulong bitfield)
        {
            msgTypeBitfield_ = bitfield;
        }

        // virtual ~VuMessageTypeFilter();
        public override VU_BOOL Test(VuMessage evnt)
        {
            return ((1 << (int)evnt.Type()) != 0 && msgTypeBitfield_ != 0) ? true : false;
        }
        public override VuMessageFilter Copy()
        {
            return new VuMessageTypeFilter(msgTypeBitfield_);
        }

        protected ulong msgTypeBitfield_;
    }


}
