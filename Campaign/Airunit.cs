using System;

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
		public AirUnitClass(int type);
		public AirUnitClass(VU_BYTE **stream);
		//TODO public virtual ~AirUnitClass();
		public virtual int SaveSize ();
		public virtual int Save (VU_BYTE **stream);

		// event Handlers
		public virtual VU_ERRCODE Handle(VuFullUpdateEvent *evnt);

		// Required pure virtuals handled by AirUnitClass
		public virtual MoveType GetMovementType ( );
		public virtual int GetUnitSpeed ( );
		public virtual CampaignTime UpdateTime ( ) 	{ return AIR_UPDATE_CHECK_INTERVAL*CampaignSeconds; }
      	public virtual float Vt ( )				   	{ return GetUnitSpeed() * KPH_TO_FPS; }
      	public virtual float Kias ( )				{ return Vt() * FTPSEC_TO_KNOTS; }

		// core functions
		public virtual int IsHelicopter ( );
		public virtual int OnGround ( );
	
		// =========================================
		// Air Unit functions
		// =========================================
		
		public static int GetUnitScore (Unit u, MoveType mt);
	}
}