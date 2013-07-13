using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{
    public abstract class VuMessageFilter
    {
        //VuMessageFilter() { }
        //virtual ~VuMessageFilter() { }
        public abstract VU_BOOL Test(VuMessage evnt);
        public abstract VuMessageFilter Copy();
    }


}
