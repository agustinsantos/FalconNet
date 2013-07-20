using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VU_ID_NUMBER = System.UInt64;

namespace FalconNet.FalcLib
{
    // ===================================
    // Name space shit
    // ===================================
    /** namespace class for getting ids */
    public class IdNamespace
    {
        public static readonly IdNamespace ObjectiveNS = new IdNamespace(FIRST_OBJECTIVE_VU_ID_NUMBER, LAST_OBJECTIVE_VU_ID_NUMBER);
        public static readonly IdNamespace NonVolatileNS = new IdNamespace(FIRST_NON_VOLATILE_VU_ID_NUMBER, LAST_NON_VOLATILE_VU_ID_NUMBER);
        public static readonly IdNamespace PackageNS = new IdNamespace(FIRST_PACKAGE_ID_NUMBER, LAST_PACKAGE_ID_NUMBER);
        public static readonly IdNamespace FlightNS = new IdNamespace(FIRST_FLIGHT_ID_NUMBER, LAST_FLIGHT_ID_NUMBER);
        public static readonly IdNamespace VolatileNS = new IdNamespace(FIRST_VOLATILE_VU_ID_NUMBER, LAST_VOLATILE_VU_ID_NUMBER);


        /** gets an id for the given namespace. The id will not exist in database. */
        public static VU_ID_NUMBER GetIdFromNamespace(IdNamespace ns)
        {
            VU_ID tryId = new VU_ID(VUSTATIC.vuLocalSession.creator_, 0);

            do
            {
                tryId.num_ = ns.GetId();
            }
            while (VUSTATIC.vuDatabase.Find(tryId) != null);

            return tryId.num_;
        }

        /** resets all namespaces */
        public static void ResetNamespaces()
        {
            /* VuEnterCriticalSection();
             lastObjectiveId = FIRST_OBJECTIVE_VU_ID_NUMBER;
             lastNonVolatileId = FIRST_NON_VOLATILE_VU_ID_NUMBER;
             lastPackageId = FIRST_LOW_VOLATILE_VU_ID_NUMBER_1;
             lastFlightId = FIRST_LOW_VOLATILE_VU_ID_NUMBER_2;
             lastVolatileId = FIRST_VOLATILE_VU_ID_NUMBER;
             VuExitCriticalSection();*/
            ObjectiveNS.Reset();
            NonVolatileNS.Reset();
            PackageNS.Reset();
            FlightNS.Reset();
            VolatileNS.Reset();
        }

        /** minumum id for entities of this type */
        public readonly VU_ID_NUMBER lowWrap;
        /** maximum id for entities of this type */
        public readonly VU_ID_NUMBER hiWrap;
        /** constructor creates mutex and sets id to low */
        public IdNamespace(VU_ID_NUMBER low, VU_ID_NUMBER hi)
        {
            lowWrap = low;
            hiWrap = hi;
            curId = low;
            //TODO m = VuxCreateMutex("namespace mutex");
        }

        /** destructor destroy mutex */
        //public  ~IdNamespace()
        // {
        //     VuxDestroyMutex(m);
        // }

        /** resets namespace */
        public void Reset()
        {
            lock (m)
            {
                curId = lowWrap;
            }
        }
        /** sets the id if higher than current */
        public void UseId(VU_ID_NUMBER id)
        {
            if (id >= curId)
            {
                ++curId;

                if (curId > hiWrap)
                {
                    curId = lowWrap;
                }
            }
        }
        /** get an id */
        public VU_ID_NUMBER GetId()
        {
            lock (m)
            {
                VU_ID_NUMBER ret = curId++;

                if (curId > hiWrap)
                {
                    curId = lowWrap;
                }

                return ret;
            }
        }


        /** current id for entities of this type */
        private VU_ID_NUMBER curId;
        /** mutex for this trait */
        private object m;

        private const ulong FIRST_OBJECTIVE_VU_ID_NUMBER = VU_ID.VU_FIRST_ENTITY_ID;
        private const ulong LAST_OBJECTIVE_VU_ID_NUMBER = VU_ID.VU_FIRST_ENTITY_ID + Camplib.MAX_NUMBER_OF_OBJECTIVES;
        private const ulong FIRST_NON_VOLATILE_VU_ID_NUMBER = LAST_OBJECTIVE_VU_ID_NUMBER + 1;
        private const ulong LAST_NON_VOLATILE_VU_ID_NUMBER = FIRST_NON_VOLATILE_VU_ID_NUMBER + Camplib.MAX_NUMBER_OF_UNITS;
        // divided low volatilies in 2 halfs
        //#define FIRST_LOW_VOLATILE_VU_ID_NUMBER (LAST_NON_VOLATILE_VU_ID_NUMBER+1)
        //#define LAST_LOW_VOLATILE_VU_ID_NUMBER (FIRST_LOW_VOLATILE_VU_ID_NUMBER+(MAX_NUMBER_OF_VOLITILE_UNITS))
        private const ulong FIRST_PACKAGE_ID_NUMBER = LAST_NON_VOLATILE_VU_ID_NUMBER + 1;
        private const ulong LAST_PACKAGE_ID_NUMBER = FIRST_PACKAGE_ID_NUMBER + (Camplib.MAX_NUMBER_OF_VOLATILE_UNITS / 2);
        private const ulong FIRST_FLIGHT_ID_NUMBER = LAST_PACKAGE_ID_NUMBER + 1;
        private const ulong LAST_FLIGHT_ID_NUMBER = FIRST_PACKAGE_ID_NUMBER + Camplib.MAX_NUMBER_OF_VOLATILE_UNITS;
        private const ulong FIRST_VOLATILE_VU_ID_NUMBER = LAST_FLIGHT_ID_NUMBER + 1;
        private const ulong LAST_VOLATILE_VU_ID_NUMBER = ~((VU_ID_NUMBER)0);

    }
}
