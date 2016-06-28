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
    public abstract class VuFilter
    {
        //TODO public virtual ~VuFilter();

        /// <summary>
        /// used for ordered containers mostly
        /// </summary>
        /// <param name="ent1"></param>
        /// <param name="ent2"></param>
        /// <returns>< 0 -. ent1  < ent2, == 0 -. ent1 == ent2, > 0 -. ent1  > ent2</returns>
        public virtual int Compare(VuEntity ent1, VuEntity ent2)
        {
            if (ent1 == null || ent2 == null)
                return 0;

            VU_ID key1 = ent1.Id();
            VU_ID key2 = ent2.Id();

            if (key1 > key2)
            {
                return 1;
            }
            else if (key1 < key2)
            {
                return -1;
            }

            return 0;
        }


        /// <summary>
        /// test if ent can be inserted into a filtered collection
        /// </summary>
        /// <param name="ent"></param>
        /// <returns>        
        /// < 0 -. ent1  < ent2
        /// == 0 -. ent1 == ent2
        ///  > 0 -. ent1  > ent2
        ///  </returns>
        public abstract VU_BOOL Test(VuEntity ent);

        /// <summary>
        /// called before removing an entity from collection
        /// </summary>
        /// <param name="ent"></param>
        /// <returns>true -. ent might be in sub-set and may be removed, false -. ent could never have been in sub-set</returns>
        public virtual VU_BOOL RemoveTest(VuEntity ent)
        {
            return true;
        }

        /// <summary>
        /// called to check if a message can affect the collection
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns>
        /// TRUE -. event may cause a change to result of Test(),
        /// FALSE -. event will never cause a change to result of Test()
        /// </returns>
        public virtual VU_BOOL Notice(VuMessage evnt)
        {
            return false;
        }

        /// <summary>
        /// allocates and returns a copy
        /// </summary>
        /// <returns></returns>
        public abstract VuFilter Copy();


        /// <summary>
        /// base is empty
        /// </summary>
        protected VuFilter() { }

        // copy constructor
        protected VuFilter(VuFilter filter) { }
    }
}
