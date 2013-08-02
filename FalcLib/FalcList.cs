using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/** sfr: I think priate here means they are not changed by VU insertions, since the Insert functions
* are all stubs which does nothing. So even if an entity pass the filter test, it wont be inserted
* at all. The result is that these collections only insert elements when you explicity call ForcedInsert.
* Removal works as it usual.
*/

// =================================
// Falcon's Private Filters
// =================================

namespace FalconNet.FalcLib
{
    /** accepts no entities. */
    public class FalconNothingFilterType : VuFilter
    {
        public static readonly FalconNothingFilterType Instance = new FalconNothingFilterType();
        public FalconNothingFilterType() { }
        // toDO virtual ~FalconNothingFilterType( ) {}

        public override bool Test(VuEntity ent)
        {
            return false;
        }
        public override bool RemoveTest(VuEntity ent)
        {
            return true;
        }

        public override VuFilter Copy()
        {
            return new FalconNothingFilterType();
        }
    }
    /** TailInsertList is a Falcon-Private list structure which will add entries to the end of the list and
* remove from the begining.
*/
    public class TailInsertList : VuLinkedList
    {

        public TailInsertList()
            : base(FalconNothingFilterType.Instance) { }
        public TailInsertList(VuFilter filter)
            : base(filter) { }

        //TODO public virtual ~TailInsertList(void);


        /** does nothing, so collection manager doesnt mess with us. */
        protected override VU_ERRCODE PrivateInsert(VuEntity entity)
        {
            return VU_ERRCODE.VU_NO_OP;
        }



        /** public insertion. */
        public override VU_ERRCODE ForcedInsert(VuEntity entity)
        {
            lock (GetMutex())
            {
                l_.Insert(l_.Count, entity);
                return VU_ERRCODE.VU_SUCCESS;
            }
        }


        /** removes first element. */
        public VuEntity PopHead()
        {
            lock (GetMutex())
            {

                while (l_.Count > 0)
                {
                    VuEntity eb = l_[0];
                    l_.RemoveAt(0);

                    if (eb.VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
                    {
                        // found a good one
                        return eb;
                    }
                }

                return null;
            }
        }
    }

    // HeadInsertList is a Falcon-Private list structure which will add entries to the beginning of the list
    public class HeadInsertList : VuLinkedList
    {

        public HeadInsertList()
            : base(FalconNothingFilterType.Instance) { }
        public HeadInsertList(VuFilter filter)
            : base(filter) { }
        //TODO virtual ~HeadInsertList(void);


        /** does nothing, so vu collection manager doesnt mess with us */
        protected override VU_ERRCODE PrivateInsert(VuEntity entity)
        {
            return VU_ERRCODE.VU_NO_OP;
        }


        /** public insertion. */
        public override VU_ERRCODE ForcedInsert(VuEntity entity)
        {
            lock (GetMutex())
            {
                l_.Insert(0, entity);
                return VU_ERRCODE.VU_SUCCESS;
            }
        }

    }

    // Falcon FalconPrivateList is simply a Falcon-Private vu entity storage list
    public class FalconPrivateList : VuLinkedList
    {

        public FalconPrivateList()
            : base(FalconNothingFilterType.Instance) { }
        public FalconPrivateList(VuFilter filter)
            : base(filter) { }
        //TODO public virtual ~FalconPrivateList();


        /** does nothing, so vu collection manager doesnt mess with us */
        protected override VU_ERRCODE PrivateInsert(VuEntity entity)
        {
            return VU_ERRCODE.VU_NO_OP;
        }


        /** public insertion. */
        public override VU_ERRCODE ForcedInsert(VuEntity entity)
        {
            return base.PrivateInsert(entity);
        }
    }

    // Falcon PrivateFilteredList is identical to above but sorts entries
    public class FalconPrivateOrderedList : VuLinkedList
    {

        public FalconPrivateOrderedList()
            : base(FalconNothingFilterType.Instance) { }
        public FalconPrivateOrderedList(VuFilter filter)
            : base(filter) { }
        //TODO public virtual ~FalconPrivateOrderedList();


        /** does nothing, so vu collection manager doesnt mess with us */
        protected override VU_ERRCODE PrivateInsert(VuEntity entity)
        {
            return VU_ERRCODE.VU_NO_OP;
        }

        /** public insertion. */
        public override VU_ERRCODE ForcedInsert(VuEntity entity)
        {
            for (int i = 0; i < l_.Count; i++)
            {
                VuEntity eb = l_[i];

                if (entity == eb)
                {
                    // already in
                    return VU_ERRCODE.VU_NO_OP;
                }
                else if (GetFilter().Compare(entity, eb) > 0)
                {
                    l_.Insert(i, entity);
                    return VU_ERRCODE.VU_SUCCESS;
                }
            }

            l_.Add(entity);
            return VU_ERRCODE.VU_SUCCESS;
        }

    }
}
