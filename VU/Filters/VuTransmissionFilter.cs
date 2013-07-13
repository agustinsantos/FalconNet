using VU_BOOL = System.Boolean;
using VU_KEY = System.UInt64;
using VU_TIME = System.UInt64;

namespace FalconNet.VU
{
    public class VuTransmissionFilter : VuKeyFilter
    {

        public VuTransmissionFilter() { }
        public VuTransmissionFilter(VuTransmissionFilter other) : base(other) { }
        //TODO public virtual ~VuTransmissionFilter();

        public override VU_BOOL Test(VuEntity ent)
        {
            return (VU_BOOL)(((ent.IsLocal() && (ent.UpdateRate() > (VU_TIME)0)) ? true : false));
        }

        // true -. ent in sub-set
        // FALSE -. ent not in sub-set
        public override int Compare(VuEntity ent1, VuEntity ent2)
        {
            ulong time1 = Key(ent1);
            ulong time2 = Key(ent2);

            return (time1 > time2 ? (int)(time1 - time2) : -(int)(time2 - time1));
        }

        //  < 0 -. ent1  < ent2
        // == 0 -. ent1 == ent2
        //  > 0 -. ent1  > ent2
        public override VuFilter Copy()
        {
            return new VuTransmissionFilter(this);
        }

        public override VU_BOOL Notice(VuMessage evnt)
        {
            if (evnt.Type() == VU_MSG_DEF.VU_TRANSFER_EVENT)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override VU_KEY Key(VuEntity ent)
        {
            return (ulong)(ent.LastTransmissionTime() + ent.UpdateRate());
        }
    }

}
