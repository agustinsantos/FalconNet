using System.Text;
using VU_BOOL = System.Boolean;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;
using SM_SCALAR = System.Single;
using BIG_SCALAR = System.Single;
using VU_DAMAGE = System.UInt64;
using VU_KEY = System.UInt64;
using System.IO;

namespace FalconNet.VU
{
    /// <summary>
    /// The VuNullMessageFilter lets everything through
    /// </summary>
    public class VuNullMessageFilter : VuMessageFilter
    {
        public static readonly VuNullMessageFilter vuNullFilter = new VuNullMessageFilter();

        //VuNullMessageFilter() : VuMessageFilter() { }
        //virtual ~VuNullMessageFilter() { }
        public override VU_BOOL Test(VuMessage evnt)
        {
            return true;
        }

        public override VuMessageFilter Copy()
        {
            return new VuNullMessageFilter();
        }
    }
}
