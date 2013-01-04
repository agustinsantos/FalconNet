using System;
using FalconNet.FalcLib;
using FalconNet.VU;
using VU_KEY = System.UInt64;
using BIG_SCALAR = System.Single;
using FalconNet.Common;

namespace FalconNet.Campaign
{
    /******************************************************************************
    *
    * VuList managing routines
    *
    *****************************************************************************/

    // ==================================
    // Unit specific filters
    // ==================================

    public class UnitFilter : VuFilter
    {
        public byte parent;					// Set if parents only
        public byte real;					// Set if real only
        public ushort host;					// Set if this host only
        public byte inactive;				// active or inactive units only


        public UnitFilter(byte p, byte r, ushort h, byte a)
        {
            parent = p;
            real = r;
            host = h;
            inactive = a;
        }
        //TODO public virtual ~UnitFilter( )			{}

        public virtual bool Test(VuEntity e)
        {
#if TODO
            if (!(e.EntityType()).classInfo_[(int)VU_CLASS.VU_DOMAIN] || (e.EntityType()).classInfo_[(int)VU_CLASS.VU_CLASS] != CLASS_UNIT)
                return false;
            if (parent && !((Unit)e).Parent())
                return false;
            if (real && !Real((e.EntityType()).classInfo_[VU_TYPE]))
                return false;
            if (host != 0 && !e.IsLocal())
                return false;
            if (!inactive && ((Unit)e).Inactive())
                return false;
            else if (inactive && !((Unit)e).Inactive())
                return false;
            return true;
#endif
            throw new NotImplementedException();
        }
        public virtual bool RemoveTest(VuEntity e)
        {
#if TODO
            if (!(e.EntityType()).classInfo_[(int)VU_CLASS.VU_DOMAIN] || (e.EntityType()).classInfo_[(int)VU_CLASS.VU_CLASS] != CLASS_UNIT)
                return false;
            if (parent && !((Unit)e).Parent())
                return false;
            if (real && !Real((e.EntityType()).classInfo_[VU_TYPE]))
                return false;
            if (host && !e.IsLocal())
                return false;
            if (!inactive && ((Unit)e).Inactive())
                return false;
            else if (inactive && !((Unit)e).Inactive())
                return false;
            return true;
#endif
            throw new NotImplementedException();
        }
        public virtual int Compare(VuEntity ent1, VuEntity ent2) { return (VuEntity.SimCompare(ent1, ent2)); }
        public virtual VuFilter Copy() { return new UnitFilter(parent, real, host, inactive); }
    }

    public class AirUnitFilter : VuFilter
    {

        public byte parent;					// Set if parents only
        public byte real;					// Set if real only
        public ushort host;					// Set if this host only


        public AirUnitFilter(byte p, byte r, ushort h)
        {
            parent = p;
            real = r;
            host = h;
        }
        //TODO public virtual ~AirUnitFilter()		{}

        public virtual bool Test(VuEntity e)
        {
#if TODO
            if (!(e.EntityType()).classInfo_[(int)VU_CLASS.VU_DOMAIN] || (e.EntityType()).classInfo_[(int)VU_CLASS.VU_CLASS] != CLASS_UNIT)
                return false;
            if (((Unit)e).GetDomain() != DOMAIN_AIR)
                return false;
            if (parent && !((Unit)e).Parent())
                return false;
            if (real && !Real((e.EntityType()).classInfo_[VU_TYPE]))
                return false;
            if (host && !e.IsLocal())
                return false;
            if (((Unit)e).Inactive())
                return false;
            return true;
#endif
            throw new NotImplementedException();
        }
        public virtual bool RemoveTest(VuEntity e)
        {
#if TODO
            if (!(e.EntityType()).classInfo_[(int)VU_CLASS.VU_DOMAIN] || (e.EntityType()).classInfo_[(int)VU_CLASS.VU_CLASS] != CLASS_UNIT)
                return false;
            if (((Unit)e).GetDomain() != DOMAIN_AIR)
                return false;
            if (parent && !((Unit)e).Parent())
                return false;
            if (real && !Real((e.EntityType()).classInfo_[VU_TYPE]))
                return false;
            if (host && !e.IsLocal())
                return false;
            if (((Unit)e).Inactive())
                return false;
            return true;
#endif
            throw new NotImplementedException();
        }
        public virtual int Compare(VuEntity ent1, VuEntity ent2) { return (VuEntity.SimCompare(ent1, ent2)); }
        public virtual VuFilter Copy() { return new AirUnitFilter(parent, real, host); }
    }

    public class GroundUnitFilter : VuFilter
    {

        public byte parent;					// Set if parents only
        public byte real;					// Set if real only
        public ushort host;					// Set if this host only


        public GroundUnitFilter(byte p, byte r, ushort h)
        {
            parent = p;
            real = r;
            host = h;
        }
        // TODO public virtual ~GroundUnitFilter( )		{}

        public virtual bool Test(VuEntity e)
        {
#if TODO
            if (!(e.EntityType()).classInfo_[(int)VU_CLASS.VU_DOMAIN] || (e.EntityType()).classInfo_[(int)VU_CLASS.VU_CLASS] != CLASS_UNIT)
                return false;
            if (((Unit)e).GetDomain() != DOMAIN_LAND)
                return false;
            if (parent && !((Unit)e).Parent())
                return false;
            if (real && !Real((e.EntityType()).classInfo_[VU_TYPE]))
                return false;
            if (host && !e.IsLocal())
                return false;
            if (((Unit)e).Inactive())
                return false;
            return true;
#endif
            throw new NotImplementedException();
        }
        public virtual bool RemoveTest(VuEntity e)
        {
#if TODO
            if (!(e.EntityType()).classInfo_[(int)VU_CLASS.VU_DOMAIN] || (e.EntityType()).classInfo_[(int)VU_CLASS.VU_CLASS] != Classtable_Classes.CLASS_UNIT)
                return false;
            if (((Unit)e).GetDomain() != DOMAIN_LAND)
                return false;
            if (parent && !((Unit)e).Parent())
                return false;
            if (real && !Real((e.EntityType()).classInfo_[VU_TYPE]))
                return false;
            if (host && !e.IsLocal())
                return false;
            if (((Unit)e).Inactive())
                return false;
            return true;
#endif
            throw new NotImplementedException();
        }
        public virtual int Compare(VuEntity ent1, VuEntity ent2) { return (VuEntity.SimCompare(ent1, ent2)); }
        public virtual VuFilter Copy() { return new GroundUnitFilter(parent, real, host); }
    }

    public class NavalUnitFilter : VuFilter
    {

        public byte parent;					// Set if parents only
        public byte real;					// Set if real only
        public ushort host;					// Set if this host only


        public NavalUnitFilter(byte p, byte r, ushort h)
        {
            parent = p;
            real = r;
            host = h;
        }
        //TODO public virtual ~NavalUnitFilter()		{}

        public virtual bool Test(VuEntity e)
        {
#if TODO
            if (!(e.EntityType()).classInfo_[(int)VU_CLASS.VU_DOMAIN] || (e.EntityType()).classInfo_[(int)VU_CLASS.VU_CLASS] != Classtable_Classes.CLASS_UNIT)
                return false;
            if (((Unit)e).GetDomain() != DOMAIN_SEA)
                return false;
            if (parent && !((Unit)e).Parent())
                return false;
            if (real && !Real((e.EntityType()).classInfo_[VU_TYPE]))
                return false;
            if (host && !e.IsLocal())
                return false;
            if (((Unit)e).Inactive())
                return false;
            return true;
#endif
            throw new NotImplementedException();
        }
        public virtual bool RemoveTest(VuEntity e)
        {
#if TODO
            if (!(e.EntityType()).classInfo_[(int)VU_CLASS.VU_DOMAIN] || (e.EntityType()).classInfo_[(int)VU_CLASS.VU_CLASS] != Classtable_Classes.CLASS_UNIT)
                return false;
            if (((Unit)e).GetDomain() != DOMAIN_SEA)
                return false;
            if (parent && !((Unit)e).Parent())
                return false;
            if (real && !Real((e.EntityType()).classInfo_[VU_TYPE]))
                return false;
            if (host && !e.IsLocal())
                return false;
            if (((Unit)e).Inactive())
                return false;
            return true;
#endif
            throw new NotImplementedException();
        }
        public virtual int Compare(VuEntity ent1, VuEntity ent2) { return (VuEntity.SimCompare(ent1, ent2)); }
        public virtual VuFilter Copy() { return new NavalUnitFilter(parent, real, host); }
    }



    // Unit proximity filters
    public class UnitProxFilter : VuBiKeyFilter
    {

        public float xStep;
        public float yStep;
        public byte real;					// Set if real only


        public UnitProxFilter(int r)
        { throw new NotImplementedException(); }
        public UnitProxFilter(UnitProxFilter other, int r)
        { throw new NotImplementedException(); }
        // TODO public virtual ~UnitProxFilter()					{}

        public virtual bool Test(VuEntity ent)
        { throw new NotImplementedException(); }
        public virtual bool RemoveTest(VuEntity ent)
        { throw new NotImplementedException(); }
        public virtual VuFilter Copy() { return new UnitProxFilter(real); }

#if VU_GRID_TREE_Y_MAJOR
		public virtual VU_KEY CoordToKey1(BIG_SCALAR coord)	{ return (VU_KEY)(yStep * coord); }
		public virtual VU_KEY CoordToKey2(BIG_SCALAR coord)	{ return (VU_KEY)(xStep * coord); }
		public virtual VU_KEY Distance1(BIG_SCALAR dist)		{ return (VU_KEY)(yStep * dist); }
		public virtual VU_KEY Distance2(BIG_SCALAR dist)		{ return (VU_KEY)(xStep * dist); }
#else
        public virtual VU_KEY CoordToKey1(BIG_SCALAR coord) { return (VU_KEY)(xStep * coord); }
        public virtual VU_KEY CoordToKey2(BIG_SCALAR coord) { return (VU_KEY)(yStep * coord); }
        public virtual VU_KEY Distance1(BIG_SCALAR dist) { return (VU_KEY)(xStep * dist); }
        public virtual VU_KEY Distance2(BIG_SCALAR dist) { return (VU_KEY)(yStep * dist); }
#endif
    }


    // ==============================
    // Manager Filters
    // ==============================

    //public static  UnitFilter  AllTMFilter;
    //public static  UnitFilter  MyTMFilter;

    // ==============================
    // Objective specific filters
    // ==============================

    // Standard Objective filter
    public class ObjFilter : VuFilter
    {

        public ushort host;					// Set if this host only


        public ObjFilter(ushort h)
        { throw new NotImplementedException(); }
        // TODO  public virtual ~ObjFilter( )			{}

        public virtual bool Test(VuEntity ent)
        { throw new NotImplementedException(); }
        public virtual bool RemoveTest(VuEntity ent)
        { throw new NotImplementedException(); }
        public virtual int Compare(VuEntity ent1, VuEntity ent2) { return (VuEntity.SimCompare(ent1, ent2)); }
        public virtual VuFilter Copy() { return new ObjFilter(host); }
    }


    // Objective Proximity filter
    public class ObjProxFilter : VuBiKeyFilter
    {

        public float xStep;
        public float yStep;


        public ObjProxFilter()
        { throw new NotImplementedException(); }
        public ObjProxFilter(ObjProxFilter other)
        { throw new NotImplementedException(); }
        //TODO public virtual ~ObjProxFilter( )					{}

        public virtual bool Test(VuEntity ent)
        { throw new NotImplementedException(); }
        public virtual bool RemoveTest(VuEntity ent)
        { throw new NotImplementedException(); }
        public virtual VuFilter Copy() { return new ObjProxFilter(); }

#if  VU_GRID_TREE_Y_MAJOR
		public virtual VU_KEY CoordToKey1(BIG_SCALAR coord)	{ return (VU_KEY)(yStep * coord); }
		public virtual VU_KEY CoordToKey2(BIG_SCALAR coord)	{ return (VU_KEY)(xStep * coord); }
		public virtual VU_KEY Distance1(BIG_SCALAR dist)		{ return (VU_KEY)(yStep * dist); }
		public virtual VU_KEY Distance2(BIG_SCALAR dist)		{ return (VU_KEY)(xStep * dist); }
#else
        public virtual VU_KEY CoordToKey1(BIG_SCALAR coord) { return (VU_KEY)(xStep * coord); }
        public virtual VU_KEY CoordToKey2(BIG_SCALAR coord) { return (VU_KEY)(yStep * coord); }
        public virtual VU_KEY Distance1(BIG_SCALAR dist) { return (VU_KEY)(xStep * dist); }
        public virtual VU_KEY Distance2(BIG_SCALAR dist) { return (VU_KEY)(yStep * dist); }
#endif
    }



    // ==============================
    // General Filters
    // ==============================

    public class CampBaseFilter : VuFilter
    {
        public CampBaseFilter()
        {
        }

        // TODO public virtual ~CampBaseFilter( )		{}

        public virtual bool Test(VuEntity e)
        {
            if ((e.EntityType()).classInfo_[(int)VU_CLASS.VU_DOMAIN] != 0 &&
                ((e.EntityType()).classInfo_[(int)VU_CLASS.VU_CLASS] == (byte)Classes.CLASS_UNIT ||
                 (e.EntityType()).classInfo_[(int)VU_CLASS.VU_CLASS] == (byte)Classes.CLASS_OBJECTIVE))
                return true;
            return false;
        }

        public virtual bool RemoveTest(VuEntity e)
        {
            if ((e.EntityType()).classInfo_[(int)VU_CLASS.VU_DOMAIN] != 0 &&
                ((e.EntityType()).classInfo_[(int)VU_CLASS.VU_CLASS] == (byte)Classes.CLASS_UNIT ||
                 (e.EntityType()).classInfo_[(int)VU_CLASS.VU_CLASS] == (byte)Classes.CLASS_OBJECTIVE))
                return true;
            return false;
        }
        public virtual int Compare(VuEntity ent1, VuEntity ent2) { return (VuEntity.SimCompare(ent1, ent2)); }
        public virtual VuFilter Copy() { return new CampBaseFilter(); }
    }

    public static class CampListStatic
    {
        public static CampBaseFilter CampFilter;
        public static UnitFilter AllUnitFilter = new UnitFilter(0, 0, 0, 0);
        public static AirUnitFilter AllAirFilter = new AirUnitFilter(0, 0, 0);
        public static GroundUnitFilter AllGroundFilter = new GroundUnitFilter(0, 0, 0);
        public static NavalUnitFilter AllNavalFilter = new NavalUnitFilter(0, 0, 0);
        public static UnitFilter AllParentFilter = new UnitFilter(1, 0, 0, 0);
        public static UnitFilter AllRealFilter = new UnitFilter(0, 1, 0, 0);
        public static UnitFilter InactiveFilter = new UnitFilter(0, 0, 0, 1);
        public static UnitProxFilter AllUnitProxFilter;
        public static UnitProxFilter RealUnitProxFilter;
        public static ObjProxFilter AllObjProxFilter;
        public static ObjFilter AllObjFilter;

        // ==============================
        // Registered Lists
        // ==============================

        public static VuFilteredList AllUnitList;		// All units
        public static VuFilteredList AllAirList;		// All air units
        public static VuFilteredList AllRealList;		// All ground units
        public static VuFilteredList AllParentList;	// All parent units
        public static VuFilteredList AllObjList;		// All objectives
        public static VuFilteredList AllCampList;		// All campaign entities
        public static VuFilteredList InactiveList;	// Inactive units (reinforcements)

        // ==============================
        // Maintained Lists
        // ==============================

        public const int MAX_DIRTY_BUCKETS = 9; //me123 from 8

        public static F4PFList FrontList;				// Frontline objectives
        public static F4POList POList;					// Primary objective list
        public static F4PFList SOList;					// Secondary objective list
        public static F4PFList AirDefenseList;			// All air defenses
        public static F4PFList EmitterList;			// All emitters
        public static F4PFList DeaggregateList;		// All deaggregated entities
        public static TailInsertList[] DirtyBucket = new TailInsertList[MAX_DIRTY_BUCKETS];	// Dirty entities

        // ==============================
        // Objective data lists
        // ==============================

        public static List PODataList;

        // Front Line List
        public static List FLOTList;

        // ==============================
        // Proximity Lists
        // ==============================

        public static VuGridTree ObjProxList;			// Proximity list of all objectives
        public static VuGridTree RealUnitProxList;	// Proximity list of all real units

        // ==============================
        // Global Iterators
        // ==============================

        // ==============================
        // List maintenance routines
        // ==============================

        public static void InitLists()
        { throw new NotImplementedException(); }

        public static void InitProximityLists()
        { throw new NotImplementedException(); }

        public static void InitIALists()
        { throw new NotImplementedException(); }

        public static void DisposeLists()
        { throw new NotImplementedException(); }

        public static void DisposeProxLists()
        { throw new NotImplementedException(); }

        public static void DisposeIALists()
        { throw new NotImplementedException(); }

        public static int RebuildFrontList(int do_barcaps, int incremental)
        { throw new NotImplementedException(); }

        public static int RebuildObjectiveLists()
        { throw new NotImplementedException(); }

        public static int RebuildParentsList()
        { throw new NotImplementedException(); }

        public static int RebuildEmitterList()
        { throw new NotImplementedException(); }

        public static void StandardRebuild()
        { throw new NotImplementedException(); }


    }
}
