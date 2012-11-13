using System;
using FalconNet.VU;
using FalconNet.FalcLib;
using Unit=FalconNet.Campaign.UnitClass;

namespace FalconNet.Campaign
{

	// =========================
	// Air unit Class 
	// =========================

	public class AirUnitClass :  UnitClass
	{
		// =========================
		// Types and Defines
		// =========================
		
		public const int RESERVE_MINUTES	= 15;					// How much extra fuel to load. Setable?

	
		// constructors and serial functions
		public AirUnitClass(int type)
		{throw new NotImplementedException();}
		public AirUnitClass(VU_BYTE[] stream)
		{throw new NotImplementedException();}
		//TODO public virtual ~AirUnitClass();
		public virtual int SaveSize ()
		{throw new NotImplementedException();}
		public virtual int Save (VU_BYTE[] stream)
		{throw new NotImplementedException();}

		// event Handlers
		public virtual VU_ERRCODE Handle(VuFullUpdateEvent evnt)
		{throw new NotImplementedException();}

		// Required pure virtuals handled by AirUnitClass
		public virtual MoveType GetMovementType ( )
		{throw new NotImplementedException();}
		public virtual int GetUnitSpeed ( )
		{throw new NotImplementedException();}
		public virtual CampaignTime UpdateTime ( ) 	{ return AIR_UPDATE_CHECK_INTERVAL*CampaignSeconds; }
      	public virtual float Vt ( )				   	{ return GetUnitSpeed() * KPH_TO_FPS; }
      	public virtual float Kias ( )				{ return Vt() * FTPSEC_TO_KNOTS; }

		// core functions
		public virtual int IsHelicopter ( )
		{throw new NotImplementedException();}
		public virtual int OnGround ( )
		{throw new NotImplementedException();}
	
		// =========================================
		// Air Unit functions
		// =========================================
		
		public static int GetUnitScore (Unit u, MoveType mt)
		{throw new NotImplementedException();}
	}
}