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
    public class VuStandardFilter : VuFilter
    {

        public VuStandardFilter(VuFlagBits mask, VU_TRI_STATE localSession = VU_TRI_STATE.DONT_CARE)
            : base()
        {
            localSession_ = localSession;
            idmask_ = mask;
        }
        public VuStandardFilter(ushort mask, VU_TRI_STATE localSession = VU_TRI_STATE.DONT_CARE)
            : base()
        {
            localSession_ = localSession;
            idmask_ = (VuFlagBits)mask;
        }

        //TODOpublic virtual ~VuStandardFilter();

        public override VU_BOOL Test(VuEntity ent)
        {
            if(((ent.FlagValue()) & (ushort)idmask_) != 0 &&
                ((localSession_ == VU_TRI_STATE.DONT_CARE) ||
                ((localSession_ == VU_TRI_STATE.TRUE) && (ent.IsLocal())) ||
                ((localSession_ == VU_TRI_STATE.FALSE) && (!ent.IsLocal()))
                )
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int Compare(VuEntity ent1, VuEntity ent2)
        {
            return (int)((VU_KEY)ent1.Id() - (VU_KEY)ent2.Id());
        }

        public override VuFilter Copy()
        {
            return new VuStandardFilter(this);
        }


        public override VU_BOOL Notice(VuMessage evnt)
        {
            if ((localSession_ != VU_TRI_STATE.DONT_CARE) && ((evnt.Type() == VU_MSG_DEF.VU_TRANSFER_EVENT)))
            {
                return true;
            }
            return false;
        }



        protected VuStandardFilter(VuStandardFilter other)
            : base(other)
        {
            localSession_ = VU_TRI_STATE.DONT_CARE;
            idmask_ = new VuFlagBits();
            if (other != null)
            {
                idmask_ = other.idmask_;
                localSession_ = other.localSession_;
            }
        }

        // DATA
        protected VuFlagBits idmask_;
        protected VU_TRI_STATE localSession_;
    }
}
