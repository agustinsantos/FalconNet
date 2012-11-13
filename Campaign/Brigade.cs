using System;
using FalconNet.VU;
using Unit=FalconNet.Campaign.UnitClass;
using FalconNet.Common;
using FalconNet.FalcLib;

namespace FalconNet.Campaign
{
	// =========================
	// Brigade Class
	// =========================
	public class BrigadeClass :  GroundUnitClass
	{
#if USE_SH_POOLS
    
      public // Overload new/delete to use a SmartHeap fixed size pool
      public void *operator new(size_t size) { ShiAssert( size == sizeof(BrigadeClass) ); return MemAllocFS(pool);	};
      public void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      public static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(BrigadeClass), 200, 0 ); };
      public static void ReleaseStorage()	{ MemPoolFree( pool ); };
      public static MEM_POOL	pool;
#endif

	
		private byte			elements;							// Number of child units
		private byte			c_element;							// Which one we're looking at
		private VU_ID[]			element = new VU_ID[MAX_UNIT_CHILDREN];			// VU_IDs of elements
		private short			fullstrength;						// How many vehicles we were before we took losses

		// constructors and serial functions
		public BrigadeClass (int type)
		{throw new NotImplementedException();}
		public BrigadeClass (VU_BYTE[] stream)
		{throw new NotImplementedException();}
		// TODO public virtual ~BrigadeClass();
		public virtual int SaveSize ()
		{throw new NotImplementedException();}

		public virtual int Save (VU_BYTE[] stream)
		{throw new NotImplementedException();}

		// event Handlers
		public virtual VU_ERRCODE Handle (VuFullUpdateEvent evnt)
		{throw new NotImplementedException();}

		public override bool IsBrigade ()
		{
			return true;
		}

		// Access Functions
		public void SetElements (byte p)
		{throw new NotImplementedException();}

		public void SetCElement (byte p)
		{throw new NotImplementedException();}

		public void SetElement (int i, VU_ID v)
		{throw new NotImplementedException();}

		public void SetFullStrength (short p)
		{throw new NotImplementedException();}

		public byte GetElements ()
		{
			return elements;
		}

		public byte GetCElement ()
		{
			return c_element;
		}

		public VU_ID GetElement (int i)
		{
			return element [i];
		}

		public short GetFullStrength ()
		{
			return fullstrength;
		}

		// Required pure virtuals handled by Brigade class
		public virtual int MoveUnit (CampaignTime time)
		{throw new NotImplementedException();}

		public virtual int Reaction (CampEntity what, int zone, float range)
		{throw new NotImplementedException();}

		public virtual int ChooseTactic ()
		{throw new NotImplementedException();}

		public virtual int CheckTactic (int tid)
		{throw new NotImplementedException();}

		public virtual int Father ()
		{
			return 1;
		}

		public virtual int Real ()
		{
			return 0;
		}

		public virtual void SetUnitOrders (int o, VU_ID oid)
		{throw new NotImplementedException();}
//		virtual void SetUnitAction ();
		public virtual int GetUnitSpeed ()
		{throw new NotImplementedException();}

		public virtual int GetUnitSupply ()
		{throw new NotImplementedException();}

		public virtual int GetUnitMorale ()
		{throw new NotImplementedException();}

		public virtual int GetUnitSupplyNeed (int total)
		{throw new NotImplementedException();}

		public virtual int GetUnitFuelNeed (int total)
		{throw new NotImplementedException();}

		public virtual void SupplyUnit (int supply, int fuel)
		{throw new NotImplementedException();}

		// Core functions
		public virtual void SetUnitDivision (int d)
		{throw new NotImplementedException();}

		public virtual int OrderElement (Unit e, F4PFList l)
		{throw new NotImplementedException();}

		public virtual Unit GetFirstUnitElement ()
		{throw new NotImplementedException();}

		public virtual Unit GetNextUnitElement ()
		{throw new NotImplementedException();}

		public virtual Unit GetUnitElement (int e)
		{throw new NotImplementedException();}

		public virtual Unit GetUnitElementByID (int eid)
		{throw new NotImplementedException();}

		public virtual Unit GetPrevUnitElement (Unit e)
		{throw new NotImplementedException();}

		public virtual void AddUnitChild (Unit e)
		{throw new NotImplementedException();}

		public virtual void DisposeChildren ()
		{throw new NotImplementedException();}

		public virtual void RemoveChild (VU_ID eid)
		{throw new NotImplementedException();}

		public virtual void ReorganizeUnit ()
		{throw new NotImplementedException();}

		public virtual int UpdateParentStatistics ()
		{throw new NotImplementedException();}

		public virtual int RallyUnit (int minutes)
		{throw new NotImplementedException();}
		
		public static BrigadeClass NewBrigade (int type)
		{throw new NotImplementedException();}
	}

}

