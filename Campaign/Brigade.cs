using System;
using FalconNet.VU;
using VU_BYTE=System.Byte;
using Unit=FalconNet.Campaign.UnitClass;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.CampaignBase;
using F4PFList = FalconNet.FalcLib.FalconPrivateList;
using F4POList = FalconNet.FalcLib.FalconPrivateOrderedList;

namespace FalconNet.Campaign
{
	// =========================
	// Brigade Class
	// =========================
	public class BrigadeClass :  GroundUnitClass
	{
#if USE_SH_POOLS
    
      public // Overload new/delete to use a SmartHeap fixed size pool
      public void *operator new(size_t size) { Debug.Assert( size == sizeof(BrigadeClass) ); return MemAllocFS(pool);	};
      public void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      public static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(BrigadeClass), 200, 0 ); };
      public static void ReleaseStorage()	{ MemPoolFree( pool ); };
      public static MEM_POOL	pool;
#endif

	
		private byte			elements;							// Number of child units
		private byte			c_element;							// Which one we're looking at
		private VU_ID[]			element = new VU_ID[Camplib.MAX_UNIT_CHILDREN];			// VU_IDs of elements
		private short			fullstrength;						// How many vehicles we were before we took losses

		// constructors and serial functions
		public BrigadeClass (ushort type) : base(type)
		{throw new NotImplementedException();}
#if TODO	
        public BrigadeClass (byte[] stream, ref int offset) : base(stream, ref offset)
		{throw new NotImplementedException();}
		public BrigadeClass(byte[] bytes, ref int offset, int version)
            : base(bytes, ref offset, version)
        {

            elements = bytes[offset];
            offset++;
            element = new VU_ID[elements];
            if (elements < 5) element = new VU_ID[5];
            for (var i = 0; i < elements; i++)
            {
                var thisElement = new VU_ID();
                thisElement.num_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                thisElement.creator_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                element[i] = thisElement;
            }
     
		}
		// TODO public virtual ~BrigadeClass();
		public override int SaveSize ()
		{throw new NotImplementedException();}

		public override int Save (VU_BYTE[] stream)
		{throw new NotImplementedException();}
#endif
		// event Handlers
		public override VU_ERRCODE Handle (VuFullUpdateEvent evnt)
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
		public override int MoveUnit (CampaignTime time)
		{throw new NotImplementedException();}

        public override int Reaction(CampBaseClass what, int zone, float range)
		{throw new NotImplementedException();}

		public override int ChooseTactic ()
		{throw new NotImplementedException();}

		public override int CheckTactic (int tid)
		{throw new NotImplementedException();}

		public override int Father ()
		{
			return 1;
		}

		public override int Real ()
		{
			return 0;
		}

		public override void SetUnitOrders (int o, VU_ID oid)
		{throw new NotImplementedException();}
//		virtual void SetUnitAction ();
		public override int GetUnitSpeed ()
		{throw new NotImplementedException();}

		public override int GetUnitSupply ()
		{throw new NotImplementedException();}

		public override int GetUnitMorale ()
		{throw new NotImplementedException();}

		public override int GetUnitSupplyNeed (int total)
		{throw new NotImplementedException();}

		public override int GetUnitFuelNeed (int total)
		{throw new NotImplementedException();}

		public override void SupplyUnit (int supply, int fuel)
		{throw new NotImplementedException();}

		// Core functions
		public override void SetUnitDivision (int d)
		{throw new NotImplementedException();}

		public virtual int OrderElement (Unit e, F4PFList l)
		{throw new NotImplementedException();}

		public override Unit GetFirstUnitElement ()
		{throw new NotImplementedException();}

		public override Unit GetNextUnitElement ()
		{throw new NotImplementedException();}

		public override Unit GetUnitElement (int e)
		{throw new NotImplementedException();}

		public override Unit GetUnitElementByID (int eid)
		{throw new NotImplementedException();}

		public override Unit GetPrevUnitElement (Unit e)
		{throw new NotImplementedException();}

		public override void AddUnitChild (Unit e)
		{throw new NotImplementedException();}

		public override void DisposeChildren ()
		{throw new NotImplementedException();}

		public override void RemoveChild (VU_ID eid)
		{throw new NotImplementedException();}

		public override void ReorganizeUnit ()
		{throw new NotImplementedException();}

		public override int UpdateParentStatistics ()
		{throw new NotImplementedException();}

		public override int RallyUnit (int minutes)
		{throw new NotImplementedException();}
		
		public static BrigadeClass NewBrigade (int type)
		{throw new NotImplementedException();}
	}

}

