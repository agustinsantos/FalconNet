using VU_BOOL = System.Boolean;
using VU_KEY = System.UInt64;

namespace FalconNet.VU
{
    public class VuAssociationFilter : VuFilter
    {
        public VuAssociationFilter(VU_ID association)
            : base()
        {
            association_ = association;
        }
        //TODO public virtual ~VuAssociationFilter();

        public override VU_BOOL Test(VuEntity ent)
        {
            return (ent.Association() == association_) ? true : false;
        }

        public override int Compare(VuEntity ent1, VuEntity ent2)
        {
            return (int)((VU_KEY)ent1.Id() - (VU_KEY)ent2.Id());
        }

        public override VuFilter Copy()
        {
            return new VuAssociationFilter(this);
        }



        protected VuAssociationFilter(VuAssociationFilter other)
            : base(other)
        {
            if (other != null)
            {
                association_ = other.association_;
            }
        }

        // DATA
        protected VU_ID association_;
    }

}
