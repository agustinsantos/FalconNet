using VU_BOOL = System.Boolean;
using VU_KEY = System.UInt64;

namespace FalconNet.VU
{


    public class VuOpaqueFilter : VuFilter
    {
        public VuOpaqueFilter() : base() { }
        //TODO public virtual ~VuOpaqueFilter();

        public override VU_BOOL Test(VuEntity ent)
        {
            return false;
        }

        public override int Compare(VuEntity ent1, VuEntity ent2)
        {
            return (int)((VU_KEY)ent1.Id() - (VU_KEY)ent2.Id());
        }

        public override VuFilter Copy()
        {
            return new VuOpaqueFilter(this);
        }



        protected VuOpaqueFilter(VuOpaqueFilter other) : base(other) { }

        // DATA
    }
}
