using FalconNet.VU;
using System;
using GridIndex = System.Int16;

namespace FalconNet.Campaign
{


    // =======================
    // Division data structure
    // =======================

    public class DivisionClass
    {
#if USE_SH_POOLS
   public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { ShiAssert( size == sizeof(DivisionClass) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(DivisionClass), 50, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif

        public GridIndex x;									// Averaged Location
        public GridIndex y;
        public short nid;								// Division id
        public short owner;
        public byte type;								// type type (armor/infanty/etc)
        public byte elements;							// Number of child units
        public byte c_element;							// Which one we're looking at
        public VU_ID element;							// VU_IDs of elements
        public DivisionClass next;


        public DivisionClass()
        { throw new NotImplementedException(); }
        //TODO public ~DivisionClass ();

        public int BuildDivision(int control, int div)
        { throw new NotImplementedException(); }
        public void GetLocation(out GridIndex rx, out GridIndex ry) { rx = x; ry = y; }
        public byte GetDivisionType() { return type; }
        public string GetName(string buffer, int size, int object_)
        { throw new NotImplementedException(); }

        public UnitClass GetFirstUnitElement()
        { throw new NotImplementedException(); }
        public UnitClass GetNextUnitElement()
        { throw new NotImplementedException(); }
        public UnitClass GetUnitElement(int e)
        { throw new NotImplementedException(); }
        public UnitClass GetUnitElementByID(int eid)
        { throw new NotImplementedException(); }
        public UnitClass GetPrevUnitElement(UnitClass e)
        { throw new NotImplementedException(); }

        public void UpdateDivisionStats()
        { throw new NotImplementedException(); }

        public void RemoveChildren()
        { throw new NotImplementedException(); }
        public void RemoveChild(VU_ID eid)
        { throw new NotImplementedException(); }

        // ========================
        // Other functions
        // ========================

        public static void DumpDivisionData()
        { throw new NotImplementedException(); }

        public static void BuildDivisionData()
        { throw new NotImplementedException(); }

        public static DivisionClass GetFirstDivision(int team)
        { throw new NotImplementedException(); }

        public static DivisionClass GetNextDivision(DivisionClass d)
        { throw new NotImplementedException(); }

        public static DivisionClass GetFirstDivisionByCountry(int country)
        { throw new NotImplementedException(); }

        public static DivisionClass GetNextDivisionByCountry(DivisionClass d, int country)
        { throw new NotImplementedException(); }

        public static DivisionClass GetDivisionByUnit(UnitClass u)
        { throw new NotImplementedException(); }

        public static DivisionClass FindDivisionByXY(GridIndex x, GridIndex y)
        { throw new NotImplementedException(); }
    }

    //TODO typedef DivisionClass	*Division;


}
