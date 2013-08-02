using VU_BOOL = System.Boolean;
using VU_KEY = System.UInt64;

namespace FalconNet.VU
{

    public class VuTypeFilter : VuFilter
    {
        public VuTypeFilter(ushort type)
            : base()
        {
            type_ = type;
        }
        //TODO public virtual ~VuTypeFilter();

        public override VU_BOOL Test(VuEntity ent)
        {
            return ent.Type().Equals(type_) ? true : false;
        }

        public override VU_BOOL RemoveTest(VuEntity ent)
        {
            return ent.Type().Equals(type_) ? true : false;
        }

        public override int Compare(VuEntity ent1, VuEntity ent2)
        {
            return (int)((VU_KEY)ent1.Id() - (VU_KEY)ent2.Id());
        }

        public override VuFilter Copy()
        {
            return new VuTypeFilter(this);
        }


        protected VuTypeFilter(VuTypeFilter other)
            : base(other)
        {
            type_ = other.type_;
        }

        // DATA
        protected ushort type_;
    }
}
