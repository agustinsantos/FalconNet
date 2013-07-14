using System;
using FalconNet.VU;
using FalconNet.Common;

namespace FalconNet.FalcLib
{

	// ==============================
	// Vu Class heirarchy defines
	// ==============================
	public static class Vu_CLASS
	{
		public const int VU_DOMAIN		=0;
		public const int VU_CLASS		=1;
		public const int VU_TYPE		=2;
		public const int VU_STYPE		=3;
		public const int VU_SPTYPE		=4;
		public const int VU_OWNER		=5;
	}

	// ========================
	// Vis type defines
	// ========================
	public enum VIS_TYPES
	{
		VIS_NORMAL		=0,
		VIS_REPAIRED	=1,
		VIS_DAMAGED		=2,
		VIS_DESTROYED	=3,
		VIS_LEFT_DEST	=4,
		VIS_RIGHT_DEST	=5,
		VIS_BOTH_DEST	=6
	}



	// =========================
	// Function Prototypes
	// =========================

	public static class F4VUStatic
	{
		// ========================
		// These never change
		// ========================
		
		public const int  DOMAIN_ANY = 255;
		public const int  CLASS_ANY = 255;
		public const int  TYPE_ANY = 255;
		public const int  STYPE_ANY = 255;
		public const int  SPTYPE_ANY = 255;
		public const int  RFU1_ANY = 255;
		public const int  RFU2_ANY = 255;
		public const int  RFU3_ANY = 255;
		public const int  VU_ANY = 255;

		
		// ========================
		// Filter Defines
		// ========================

		public const int  VU_FILTERANY = 255;
		public const int  VU_FILTERSTOP = 0;

		// ========================
		// Default values
		// ========================

		public const int  F4_EVENT_QUEUE_SIZE = 2000;		// How many events we can have on the queue at one heading

		public static VuMainThread gMainThread;
		public static int NumEntities;
		public static VU_ID FalconNullId;
		public static F4CSECTIONHANDLE vuCritical;

		public static void InitVU ()
		{ throw new NotImplementedException();}

		public static void ExitVU ()
		{ throw new NotImplementedException();}
	}
}

