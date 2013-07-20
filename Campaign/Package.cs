using System;
using FalconNet.VU;
using GridIndex = System.Int16;
using WayPoint=FalconNet.CampaignBase.WayPointClass;
using Flight=FalconNet.Campaign.FlightClass;
using Unit=FalconNet.Campaign.UnitClass;
using FalconNet.FalcLib;
using FalconNet.Common;
using FalconNet.CampaignBase;

namespace FalconNet.Campaign
{
		// ===================================================
		// Mission Group Class
		// ===================================================


// Reponse receipt flags
	[Flags]
	public enum PRESPONSE
	{
		 PRESPONSE_CA		=0x01,
		 PRESPONSE_ESCORT	=0x02,
		 PRESPONSE_AWACS		=0x04,
		 PRESPONSE_JSTAR		=0x08,
		 PRESPONSE_TANKER	=0x10,
		 PRESPONSE_ECM		=0x20
	}
	
	// =========================
	// Package Class
	// =========================

	public class PackageClass :   AirUnitClass 
	{
#if USE_SH_POOLS
   
      public // Overload new/delete to use a SmartHeap fixed size pool
      public void *operator new(size_t size) { ShiAssert( size == sizeof(PackageClass) ); return MemAllocFS(pool);	};
      public void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      public static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(PackageClass), 200, 0 ); };
      spublic tatic void ReleaseStorage()	{ MemPoolFree( pool ); };
      public static MEM_POOL	pool;
#endif

	
		private byte 				elements;					// Number of child units
		private byte 				c_element;					// Which one we're looking at
		private VU_ID[]				element = new VU_ID[Camplib.MAX_UNIT_CHILDREN];	// VU_IDs of elements
		private VU_ID				interceptor;				// ID of enemy BARCAP/SWEEP/Etc flight, if any
		private VU_ID				awacs;						// ID of any awacs support
		private VU_ID				jstar;						// ID of any jstar support
		private VU_ID				ecm;						// ID of any ecm support
		private VU_ID				tanker;						// ID of any tanker support
		private byte 				wait_cycles;				// How many cycles until timeout
		private byte 				flights;					// flights in this package
		private ushort				wait_for;					// Mission Requests to wait for (until timeout)
		// This stuff shouldn't change after init
		private GridIndex			iax,iay;					// Ingress assembly point
		private GridIndex			eax,eay;					// Egress assembly point (if any)
		private GridIndex			bpx,bpy;					// Break point (if any)
		private GridIndex			tpx,tpy;					// Turn point (if any)
		private CampaignTime		takeoff;					// Earliest flight's takeoff time
		private CampaignTime		tp_time;
		private ulong				package_flags;
		private byte 				escort_type;				// 2001-11-10 M.N.
		private short				caps;						// capabilities required for this package
		private short				requests;					// What other mission types we want.
		private short				responses;					// What sort of reaction we've caused
		private WayPoint			ingress;					// Ingress Route
		private WayPoint			egress;						// Egress Route
		private MissionRequestClass	mis_request;				// The origional request (lot'so repeated data here, we could trim)
		// Not added to i/o functions
		private short				aa_strength;					// The combined Air to Air strength of this package
		private int					dirty_package;

	
		// Access Functions

		public VU_ID GetInterceptor ()						{ return interceptor; }
		public VU_ID GetAwacs ()							{ return awacs; }
		public VU_ID GetJStar ()							{ return jstar; }
		public VU_ID GetECM ()								{ return ecm; }
/* 2001-04-07 ADDED BY S.G. */ void SetECM (VU_ID newEcm) { ecm = newEcm; }
		public VU_ID GetTanker ()							{ return tanker; }
		public Flight GetFACFlight ()	
		{ throw new NotImplementedException ();	}
		public CampaignTime GetTakeoff ()					{ return takeoff; }
		public CampaignTime GetTPTime ()					{ return tp_time; }
		public byte  GetFlights ()							{ return flights; }
		public short GetResponses ()						{ return responses; }
		public WayPoint GetIngress ()						{ return ingress; }
		public WayPoint GetEgress ()						{ return egress; }
		public short GetAAStrength ()						{ return aa_strength; }
		public MissionRequestClass GetMissionRequest ()	{ return mis_request; }

		public void SetTanker (VU_ID t )	
		{ throw new NotImplementedException ();	}
		public void SetTakeoff (CampaignTime t)	
		{ throw new NotImplementedException ();	}
		public void SetPackageFlags (ulong f)	
		{ throw new NotImplementedException ();	}
		public void SetTPTime (CampaignTime ct)	
		{ throw new NotImplementedException ();	}
 
		// constructors and serial functions
        public PackageClass(ushort type)
            : base(type, IdNamespace.GetIdFromNamespace(IdNamespace.PackageNS))	
		{ throw new NotImplementedException ();	}
#if TODO
		public PackageClass(byte[] stream, ref int pos)	: base(stream, ref pos)
		{ throw new NotImplementedException ();	}
		//TODO public virtual ~PackageClass();
		public override int SaveSize ()	
		{ throw new NotImplementedException ();	}
		public override int Save (byte[] stream, ref int pos)	
		{ throw new NotImplementedException ();	}

		// event Handlers
		public override VU_ERRCODE Handle(VuFullUpdateEvent evnt)	
		{ throw new NotImplementedException ();	}
#endif

		// required virtuals
        public override int Reaction(CampBaseClass e, int p, float f) { return 0; }
		public override int MoveUnit (CampaignTime c)	
		{ throw new NotImplementedException ();	}
		public override int ChooseTactic ()									{	return 0; }
		public override int CheckTactic (int t)								{	return 0; }
		public override int Father ()										{	return 1; }
		public override int Real ()											{	return 0; }
		public override bool IsPackage ()									{	return true; }

		// Dirty Data Stuff
		public void MakePackageDirty (Dirty_Package bits, Dirtyness score)	
		{ throw new NotImplementedException ();	}
		public override void WriteDirty (byte[] stream, ref int pos)	
		{ throw new NotImplementedException ();	}
		public override void ReadDirty (byte[] stream, ref int pos)	
		{ throw new NotImplementedException ();	}

		// core functions
		public override int BuildPackage(MissionRequest mis, F4PFList assemblyList)	
		{ throw new NotImplementedException ();	}
		public int RecordFlightAddition (Flight flight, MissionRequest mis, int targetd)	
		{ throw new NotImplementedException ();	}
		public void FindSupportFlights (MissionRequest mis, int targetd)	
		{ throw new NotImplementedException ();	}
		public override void HandleRequestReceipt(int type, int them, VU_ID flight)	
		{ throw new NotImplementedException ();	}
		public override Unit GetFirstUnitElement ()	
		{ throw new NotImplementedException ();	}
		public override Unit GetNextUnitElement ()	
		{ throw new NotImplementedException ();	}
		public override Unit GetUnitElement (int e)	
		{ throw new NotImplementedException ();	}
		public override Unit GetUnitElementByID (int eid)	
		{ throw new NotImplementedException ();	}
		public virtual void AddUnitChild (Unit e)	
		{ throw new NotImplementedException ();	}
		public override void DisposeChildren ()	
		{ throw new NotImplementedException ();	}
		public override void RemoveChild (VU_ID eid)	
		{ throw new NotImplementedException ();	}
		public int CheckNeedRequests ()	
		{ throw new NotImplementedException ();	}
		public void CancelFlight (Flight flight)	
		{ throw new NotImplementedException ();	}
		public void SetPackageType (byte  p) 									{	mis_request.mission = p; }
		public int GetPackageType ()										{	return mis_request.mission; }
		public VU_ID GetMainFlightID ()	
		{ throw new NotImplementedException ();	}
		public Flight GetMainFlight ()	
		{ throw new NotImplementedException ();	}

		public virtual void SetUnitAssemblyPoint (int type, GridIndex x, GridIndex y)	
		{ throw new NotImplementedException ();	}
		public virtual void GetUnitAssemblyPoint (int type, ref GridIndex x, ref GridIndex y)	
		{ throw new NotImplementedException ();	}
	}

	//TODO typedef PackageClass* Package;

// ===================================================
// Global functions
// ===================================================
	public static class PackageStatic
	{
		// Return values for BuildMissionGroup
		public const int PRET_NO_ASSETS		=1;
		public const int  PRET_DELAYED		=2;
		public const int  PRET_SUCCESS		=3;
		public const int  PRET_ABORTED		=4;
		public const int  PRET_CANCELED		=5;
		public const int  PRET_REPEAT		=6;
		public const int  PRET_HANDLED		=7;
		public const int  PRET_NOTARGET		=8;
		public const int  PRET_TIMEOUT		=9;
		
		public static PackageClass NewPackage (int type)	
		{ throw new NotImplementedException ();	}
		
		public static Flight AttachFlight (MissionRequest mis, PackageClass pack)	
		{ throw new NotImplementedException ();	}
	}
}

