using System;

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
		public BrigadeClass (int type);
		public BrigadeClass (VU_BYTE **stream);
		// TODO public virtual ~BrigadeClass();
		public virtual int SaveSize ();

		public virtual int Save (VU_BYTE **stream);

		// event Handlers
		public virtual VU_ERRCODE Handle (VuFullUpdateEvent *evnt);

		public virtual int IsBrigade ()
		{
			return true;
		}

		// Access Functions
		public void SetElements (byte p);

		public void SetCElement (byte p);

		public void SetElement (int i, VU_ID v);

		public void SetFullStrength (short p);

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
		public virtual int MoveUnit (CampaignTime time);

		public virtual int Reaction (CampEntity what, int zone, float range);

		public virtual int ChooseTactic ();

		public virtual int CheckTactic (int tid);

		public virtual int Father ()
		{
			return 1;
		}

		public virtual int Real ()
		{
			return 0;
		}

		public virtual void SetUnitOrders (int o, VU_ID oid);
//		virtual void SetUnitAction ();
		public virtual int GetUnitSpeed ();

		public virtual int GetUnitSupply ();

		public virtual int GetUnitMorale ();

		public virtual int GetUnitSupplyNeed (int total);

		public virtual int GetUnitFuelNeed (int total);

		public virtual void SupplyUnit (int supply, int fuel);

		// Core functions
		public virtual void SetUnitDivision (int d);

		public virtual int OrderElement (Unit e, F4PFList l);

		public virtual Unit GetFirstUnitElement ();

		public virtual Unit GetNextUnitElement ();

		public virtual Unit GetUnitElement (int e);

		public virtual Unit GetUnitElementByID (int eid);

		public virtual Unit GetPrevUnitElement (Unit e);

		public virtual void AddUnitChild (Unit e);

		public virtual void DisposeChildren ();

		public virtual void RemoveChild (VU_ID eid);

		public virtual void ReorganizeUnit ();

		public virtual int UpdateParentStatistics ();

		public virtual int RallyUnit (int minutes);
		
		public static BrigadeClass* NewBrigade (int type);
	}

}

