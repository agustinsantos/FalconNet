using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;
using VU_ID_NUMBER = System.UInt32;
using VU_KEY = System.UInt64;
using VU_BOOL = System.Boolean;
using BIG_SCALAR = System.Single;
using SM_SCALAR = System.Single;
using VuMutex = System.Object;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;

namespace FalconNet.VU
{
    public class VuCollectionManager
    {
        public VuCollectionManager()
        {
            collcoll_ = new List<VuCollection>();
            gridcoll_ = new List<VuGridTree>();

            gcMutex_ = VuxMutex.VuxCreateMutex("garbage collector mutex");
            birthMutex_ = VuxMutex.VuxCreateMutex("birthlist mutex");

            collsMutex_ = VuxMutex.VuxCreateMutex("colls mutex"); //TODO The original code has null 
            gridsMutex_ = VuxMutex.VuxCreateMutex("grids mutex"); //TODO The original code has null 
        }

        //TODO public ~VuCollectionManager();
        public void Register(VuCollection coll)
        {
            lock (collsMutex_)
            {
                collcoll_.Add(coll);
            }
        }

        public void DeRegister(VuCollection coll)
        {
            lock (collsMutex_)
            {
                collcoll_.Remove(coll);
            }
        }

        public void GridRegister(VuGridTree grid)
        {
            lock (gridsMutex_)
            {
                gridcoll_.Add(grid);
            }
        }

        public void GridDeRegister(VuGridTree grid)
        {
            lock (gridsMutex_)
            {
                gridcoll_.Remove(grid);
            }
        }

        /** adds an entity to all registered collections (except vuDatabase) */
        public void Add(VuEntity ent)
        {
            lock (collsMutex_)
            {
                foreach (VuCollection c in collcoll_)
                {
                    c.Insert(ent);

                }
            }
        }

        /** removes entity from all registered collections (except vuDatabase) */
        public void Remove(VuEntity ent)
        {
            lock (collsMutex_)
            {
                foreach (VuCollection c in collcoll_)
                {
                    c.Remove(ent);
                }
            }
        }

        /** handles a move in all registered grid collections (so they can be updated) */
        public int HandleMove(VuEntity ent, BIG_SCALAR coord1, BIG_SCALAR coord2)
        {
            lock (gridsMutex_)
            {
                int retval = 0;

                foreach (VuGridTree g in gridcoll_)
                {

                    if (!g.suspendUpdates_)
                    {
                        g.Move(ent, coord1, coord2);
                    }
                }

                return retval;
            }
        }

        /** handles message in all collections */
        public VU_ERRCODE Handle(VuMessage msg)
        {
            return VU_ERRCODE.VU_NO_OP;
        }

        public int FindEnt(VuEntity ent)
        {
            lock (collsMutex_)
            {
                int retval = 0;

                foreach (VuCollection c in collcoll_)
                {
                    if (c.Find(ent))
                    {
                        retval++;
                    }
                }

                return retval;
            }
        }

        public void AddToBirthList(VuEntity e)
        {
            lock (birthMutex_)
            {
                birthlist_.Add(e);
            }
        }

        public void AddToGc(VuEntity e)
        {
            lock (gcMutex_)
            {
                gclist_.Add(e);
            }
        }
        // create entities
        // at most max bper cycle
        const int bmax = 5;
        // remove list nodes, allow at most dmax per cycle
        const int dmax = 5;
        public void CreateEntitiesAndRunGc()
        {
            int bcount = 0;

            while (birthlist_.Count != 0 && (bcount < bmax))
            {
                ++bcount;
                VuEntity eb = birthlist_[0];
                VUSTATIC.vuDatabase.ReallyInsert(eb);
                birthlist_.RemoveAt(0);
            }
            int dcount = 0;

            while (gclist_.Count != 0 && (dcount < dmax))
            {
                ++dcount;
                VuEntity eb = gclist_[0];

                // some entities are removed and re-inserted in DB because of ID change.
                // when they are removed, they are collected, but they shouldnt be removed...
                if (eb.VuState() != VU_MEM_STATE.VU_MEM_ACTIVE)
                {
                    VUSTATIC.vuDatabase.ReallyRemove(eb);
                }

                gclist_.RemoveAt(0);
            }
            //REPORT_VALUE("collected", count);
        }

        public void Shutdown(VU_BOOL all)
        {
            // make last...
            //birthlist_.clear();
            //gclist_.clear();
            //vuDatabase->Suspend(all);
            lock (collsMutex_)
            {
                // copy the list since the purges can change the registered collection structure
                List<VuCollection> collcollCopy = new List<VuCollection>(collcoll_);

                foreach (VuCollection c in collcollCopy)
                {
                    c.Purge(all);

                }

                collcollCopy.Clear();

                // sfr: do at the end to avoid self destructions up there
                birthlist_.Clear();
                gclist_.Clear();
                VUSTATIC.vuDatabase.Suspend(all);

                // clear the collections? Dont, some exist during whole game time.
                //collcoll_.clear();
                //gridcoll_.clear();
            }
        }

        // registered collections and mutexes
        internal List<VuCollection> collcoll_;
        internal VuMutex collsMutex_;
        internal List<VuGridTree> gridcoll_;
        internal VuMutex gridsMutex_;

        // garbage collector
        internal VuMutex gcMutex_;          ///< garbage collector insertion mutex
        internal List<VuEntity> gclist_;    ///< list entities to be deleted
        internal VuMutex birthMutex_;       ///< birth list mutex
        internal List<VuEntity> birthlist_; ///< list entities to be created
    }

}
