using System;
using System.Diagnostics;
using Unit=FalconNet.Campaign.UnitClass;
using Objective=FalconNet.Campaign.ObjectiveClass;
using FalconNet.VU;
using FalconNet.FalcLib;
using FalconNet.Common;

namespace FalconNet.Campaign
{
	// =========================
	// Types and Defines
	// =========================

	// Transmittable Flags
	[Flags]
	public enum TrasmittableFlags
	{
		U_DEAD			=0x1,
		U_B3			=0x2,	
		U_ASSIGNED		=0x4,
		U_ORDERED		=0x8,
		U_NO_PLANNING	=0x10,			// Don't run planning AI on this unit
		U_PARENT		=0x20,
		U_ENGAGED		=0x40,
		U_B1			=0x80,
		U_SCRIPTED		=0x100,			// Mission/Route scripted- Don't run planning AI
		U_COMMANDO		=0x200,			// Act like a commando (hit commando sites && kill ourselves after x time)
		U_MOVING		=0x400,
		U_REFUSED		=0x800,			// A request for transport was refused
		U_HASECM		=0x1000,			// This unit has defensive electronic countermeasures
		U_CARGO			=0x2000,			// We're being carried by someone else (airborne/marine/carrier air)
		U_COMBAT		=0x4000,
		U_BROKEN		=0x8000,
		U_LOSSES		=0x10000,
		U_INACTIVE		=0x20000,		// Ignore this unit for all purposes (generally reinforcements)
		U_FRAGMENTED	=0x40000,			// This is a unit fragment (separated from it's origional unit)
		
		// Ground Unit Specific
		U_TARGETED		=0x100000,		// Unit's targeting is being done public static ally
		U_RETREATING	=0x200000,
		U_DETACHED		=0x400000,
		U_SUPPORTED		=0x800000,		// Support is coming to this unit's aide
		U_TEMP_DEST		=0x1000000,		// This unit's current destination is not it's final destination
		
		// Air Unit Specific
		U_FINAL			=0x100000,		// Package elements finalized and sent, or flight contains actual a/c
		U_HAS_PILOTS	=0x200000,		// Campaign has assigned this flight pilots
		U_DIVERTED		=0x400000,		// This flight is currently being diverted
		U_FIRED			=0x800000,		// This flight has taken a shot
		U_LOCKED		=0x1000000,		// Someone is locked on us
		U_IA_KILL		=0x2000000,		// Instant Action "Expects" this flight to be killed for the next level to start
		U_NO_ABORT		=0x4000000		// Whatever happens - whatever the loadout - don't ABORT
	}
	
	// 2002-02-13 ADDED BY MN for S.G.'s Identify - S.G. Wrong place. Needs to be in Falcon4.UCD so defined in Vehicle.h which is used by UnitClassDataType and VehicleClassDataType
	//  U_HAS_NCTR		0x10000000
	//  U_HAS_EXACT_RWR 0x20000000
	
	// We use these for broad class types
	public enum RCLASS
	{
		RCLASS_AIR			=0,
		RCLASS_GROUND		=1,
		RCLASS_AIRDEFENSE	=2,
		RCLASS_NAVAL		=3
	}
	
	// Types of calculations for certain functions
	public enum CALC
	{
		CALC_TOTAL			=0,
		CALC_AVERAGE		=1,
		CALC_MEAN			=2,
		CALC_MAX			=3,
		CALC_MIN			=4,
		CALC_PERCENTAGE		=5
	}
	
	// Flags for what variables to take into account for certain function
	public enum MISCFLAGS
	{
		USE_EXP				=0x01,
		USE_VEH_COUNT		=0x02,
		IGNORE_BROKEN		=0x04
	}
	
	// =========================
	// Unit Class
	// =========================
	public class UnitClass :   CampBaseClass
	{
 	
		private CampaignTime		last_check;		// Last time we checked this unit
		private Int32			roster;			// 4 byte bitwise roster
		private Int32			unit_flags;		// Various user flags
		private GridIndex			dest_x;			// Destination
		private GridIndex			dest_y;
		private VU_ID				cargo_id;		// id of our cargo, or our carrier unit
		private VU_ID				target_id;		// Target we're engaged with (there can be only one! (c))
		private byte				moved;       	// Moves since check
		private byte				losses;			// How many vehicles we've lost
		private byte				tactic;			// Current Unit tactic
		private ushort				current_wp;		// Which WP we're heading to
		private short				name_id;		// Element number, used only for naming
		private short				reinforcement;	// What reinforcement level this unit becomes active at
		private short				odds;			// How much shit is coming our way
		private int					dirty_unit;
		public UnitClassDataType	class_data;
		public DrawablePoint		draw_pointer;	// inserted into draw list when unit aggregated
		public WayPoint			wp_list;

	
		// access functions
		public CampaignTime GetLastCheck ()
		{
			return last_check;
		}

		public Int32 GetRoster ()
		{
			return roster;
		}

		public Int32 GetUnitFlags ()
		{
			return unit_flags;
		}

		public GridIndex GetDestX ()
		{
			return dest_x;
		}

		public GridIndex GetDestY ()
		{
			return dest_y;
		}

		public VU_ID GetCargoId ()
		{
			return cargo_id;
		}

		public VU_ID GetTargetId ()
		{
			return target_id;
		}

		public byte GetMoved ()
		{
			return moved;
		}

		public byte GetLosses ()
		{
			return losses;
		}

		public byte GetTactic ()
		{
			return tactic;
		}

		public ushort GetCurrentWaypoint ()
		{
			return current_wp;
		}

		public short GetNameId ()
		{
			return name_id;
		}

		public short GetReinforcement ()
		{
			return reinforcement;
		}

		public short GetOdds ()
		{
			return odds;
		}

		public UnitClassDataType GetClassData ()
		{
			return class_data;
		}

		public void SetLastCheck (CampaignTime p)
		{throw new NotImplementedException();}

		public void SetRoster (Int32 p)
		{throw new NotImplementedException();}

		public void SetUnitFlags (Int32 p)
		{throw new NotImplementedException();}

		public void SetDestX (GridIndex p)
		{throw new NotImplementedException();}

		public void SetDestY (GridIndex p)
		{throw new NotImplementedException();}

		public void SetCargoId (VU_ID p)
		{throw new NotImplementedException();}

		public void SetTargetId (VU_ID p)
		{throw new NotImplementedException();}

		public void SetMoved (byte p)
		{throw new NotImplementedException();}

		public void SetLosses (byte p)
		{throw new NotImplementedException();}

		public void SetTactic (byte p)
		{throw new NotImplementedException();}

		public void SetCurrentWaypoint (ushort p)
		{throw new NotImplementedException();}

		public void SetNameId (short p)
		{throw new NotImplementedException();}

		public void SetReinforcement (short p)
		{throw new NotImplementedException();}

		public void SetOdds (short p)
		{throw new NotImplementedException();}

		public void MakeWaypointsDirty ()
		{throw new NotImplementedException();}

		// Dirty Functions
		public void MakeUnitDirty (Dirty_Unit bits, Dirtyness score)
		{throw new NotImplementedException();}

		public virtual void WriteDirty (byte[] stream)
		{throw new NotImplementedException();}

		public virtual void ReadDirty (byte[] stream)
		{throw new NotImplementedException();}

		// constructors and serial functions
		public UnitClass (int type)
		{throw new NotImplementedException();}
		public UnitClass (VU_BYTE[] stream)
		{throw new NotImplementedException();}
		//TODO public virtual ~UnitClass();
		public override int SaveSize ()
		{throw new NotImplementedException();}

		public virtual int Save (VU_BYTE[] stream)
		{throw new NotImplementedException();}

		// event Handlers
		public virtual VU_ERRCODE Handle (VuFullUpdateEvent evnt)
		{throw new NotImplementedException();}

		// Required pure virtuals handled by UnitClass
		public virtual void SendDeaggregateData (VuTargetEntity p)
		{throw new NotImplementedException();}

		public virtual int RecordCurrentState (FalconSessionEntity p, int i)
		{throw new NotImplementedException();}

		public virtual int Deaggregate (FalconSessionEntity session)
		{throw new NotImplementedException();}

		public virtual int Reaggregate (FalconSessionEntity session)
		{throw new NotImplementedException();}

		public virtual int TransferOwnership (FalconSessionEntity session)
		{throw new NotImplementedException();}

		public virtual int Wake ()
		{throw new NotImplementedException();}

		public virtual int Sleep ()
		{throw new NotImplementedException();}

		public virtual void InsertInSimLists (float cameraX, float cameraY)
		{throw new NotImplementedException();}

		public virtual void RemoveFromSimLists ()
		{throw new NotImplementedException();}

		public virtual void DeaggregateFromData (int size, byte[] data)
		{throw new NotImplementedException();}

		public virtual void ReaggregateFromData (int size, byte[] data)
		{throw new NotImplementedException();}

		public virtual void TransferOwnershipFromData (int size, byte[] data)
		{throw new NotImplementedException();}

		public virtual int ResetPlayerStatus () 
		{throw new NotImplementedException();}

		public virtual int ApplyDamage (FalconCampWeaponsFire cwfm, byte p)
		{throw new NotImplementedException();}

		public virtual int ApplyDamage (DamType d, ref int  str, int where, short flags)
		{throw new NotImplementedException();}

		public virtual int DecodeDamageData (byte[] data, Unit shooter, FalconDeathMessage dtm)
		{throw new NotImplementedException();}

		public virtual int CollectWeapons (byte[] dam, MoveType m, short[] w, byte[] wc, int dist)
		{throw new NotImplementedException();}

		public virtual byte[] GetDamageModifiers () 
		{throw new NotImplementedException();}

		public virtual string GetName (string buffer, int size, int obj)
		{throw new NotImplementedException();}

		public virtual string GetFullName (string buffer, int size, int obj)
		{throw new NotImplementedException();}

		public virtual string GetDivisionName (string buffer, int size, int obj)
		{throw new NotImplementedException();}

		public virtual int GetHitChance (int mt, int range)
		{throw new NotImplementedException();}

		public virtual int GetAproxHitChance (int mt, int range)
		{throw new NotImplementedException();}

		public virtual int GetCombatStrength (int mt, int range)
		{throw new NotImplementedException();}

		public virtual int GetAproxCombatStrength (int mt, int range)
		{throw new NotImplementedException();}

		public virtual int GetWeaponRange (int mt, FalconEntity target = null)
		{throw new NotImplementedException();} // 2002-03-08 MODIFIED BY S.G. Need to pass it a target sometime so default to null for most cases
		public virtual int GetAproxWeaponRange (int mt)
		{throw new NotImplementedException();}

		public override int GetDetectionRange (int mt)
		{throw new NotImplementedException();}						// Takes into account emitter status
		public override int GetElectronicDetectionRange (int mt)
		{throw new NotImplementedException();}			// Max Electronic detection range, even if turned off
		public override int CanDetect (FalconEntity ent)
		{throw new NotImplementedException();}						// Nonzero if this entity can see ent
		public virtual void GetComponentLocation (ref GridIndex x, ref GridIndex y, int component)
		{throw new NotImplementedException();}

		public virtual int GetComponentAltitude (int component)
		{throw new NotImplementedException();}

		public override float GetRCSFactor () 
		{throw new NotImplementedException();}

		public override float GetIRFactor () 
		{throw new NotImplementedException();}

		// These are only really relevant for sam/airdefense/radar entities
		public override int GetNumberOfArcs ()
		{throw new NotImplementedException();}

		public override float GetArcRatio (int anum)
		{throw new NotImplementedException();}

		public override float GetArcRange (int anum)
		{throw new NotImplementedException();}

		public virtual void GetArcAngle (int anum, ref float a1, ref float a2)
		{throw new NotImplementedException();}

		public override Radar_types GetRadarType ()
		{throw new NotImplementedException();}

		// Addition Virtual functions required by all derived classes
		public virtual int CanShootWeapon (int p)
		{
			return true ;
		}

		public virtual int GetDeaggregationPoint (int i, CampEntity p)
		{
			return 0;
		}

		public virtual UnitDeaggregationData GetUnitDeaggregationData ()
		{
			return null;
		}

		public virtual int	ShouldDeaggregate ()
		{
			return true ;
		}

		public virtual void ClearDeaggregationData ()
		{
		}

		public virtual int Reaction (CampEntity c, int i, float p)
		{
			return 0;
		}

		public virtual int MoveUnit (CampaignTime t)
		{
			return 0;
		}

		public virtual int DoCombat ()
		{
			return 0;
		}

		public virtual int ChooseTactic ()
		{
			return 0;
		}

		public virtual int CheckTactic (int t)
		{
			return 1;
		}

		public virtual int Father ()
		{
			return 0;
		}

		public virtual int Real ()
		{
			return 0;
		}

		public virtual float AdjustForSupply ()
		{
			return 1.0F;
		}

		public virtual int GetUnitSpeed ()
		{
			return GetMaxSpeed ();
		}

		public virtual int DetectOnMove ()
		{
			return -1;
		}

		public virtual int ChooseTarget ()
		{
			return -1;
		}

		public virtual CampaignTime UpdateTime ()
		{
			return CampaignDay;
		}

		public virtual CampaignTime CombatTime ()
		{
			return CampaignDay;
		}

		public virtual int GetUnitSupplyNeed (int p)
		{
			return 0;
		}

		public virtual int GetUnitFuelNeed (int p)
		{
			return 0;
		}

		public virtual void SupplyUnit (int p1, int p2)
		{
		}

		public virtual int GetVehicleDeagData (SimInitDataClass d, int p)
		{
			Debug.Assert ("Shouldn't be here");
			return 0;
		}

		// Core functions
		public void Setup (byte stype, byte sptype, Control who, Unit Parent)
		{ throw new NotImplementedException();}

		public void SendUnitMessage (VU_ID id, short msg, short d1, short d2, short d3)
		{throw new NotImplementedException();}

		public void BroadcastUnitMessage (VU_ID id, short msg, short d1, short d2, short d3)
		{throw new NotImplementedException();}

		public int	ChangeUnitLocation (CampaignHeading h)
		{throw new NotImplementedException();}

		public int MoraleCheck (int shot, int lost)
		{throw new NotImplementedException();}

		public virtual int IsUnit ()
		{
			return true ;
		}

		// Unit flags
		public void SetDead (int p)
		{throw new NotImplementedException();}

		public void SetAssigned (int p)
		{throw new NotImplementedException();}

		public void SetOrdered (int p)
		{throw new NotImplementedException();}

		public void SetDontPlan (int p)
		{throw new NotImplementedException();}

		public void SetParent (int p)
		{throw new NotImplementedException();}

		public void SetEngaged (int p)
		{throw new NotImplementedException();}

		public void SetScripted (int p)
		{throw new NotImplementedException();}

		public void SetCommando (int c)
		{throw new NotImplementedException();}

		public void SetMoving (int p)
		{throw new NotImplementedException();}

		public void SetRefused (int r)
		{throw new NotImplementedException();}

		public void SetHasECM (int e)
		{throw new NotImplementedException();}

		public void SetCargo (int c)
		{throw new NotImplementedException();}

		public void SetCombat (int p)
		{throw new NotImplementedException();}

		public void SetBroken (int p)
		{throw new NotImplementedException();}

		public void SetAborted (int p)
		{throw new NotImplementedException();}

		public void SetLosses (int p)
		{throw new NotImplementedException();}

		public void SetInactive (int i)
		{throw new NotImplementedException();}

		public void SetFragment (int f)
		{throw new NotImplementedException();}

		public void SetTargeted (int p)
		{throw new NotImplementedException();}

		public void SetRetreating (int p)
		{throw new NotImplementedException();}

		public void SetDetached (int p)
		{throw new NotImplementedException();}

		public void SetSupported (int s)
		{throw new NotImplementedException();}

		public void SetTempDest (int t)
		{throw new NotImplementedException();}

		public void SetFinal (int p)
		{throw new NotImplementedException();}

		public void SetPilots (int f)
		{throw new NotImplementedException();}

		public void SetDiverted (int d)
		{throw new NotImplementedException();}

		public void SetFired (int f)
		{throw new NotImplementedException();}

		public void SetLocked (int l)
		{throw new NotImplementedException();}

		public void SetIAKill (int f)
		{throw new NotImplementedException();}

		public void SetNoAbort (int f)
		{throw new NotImplementedException();}

		public virtual int IsDead ()
		{
			return (int)unit_flags & U_DEAD;
		}

		public int Dead ()
		{
			return IsDead ();
		}

		public int Assigned ()
		{
			return (int)unit_flags & U_ASSIGNED;
		}

		public int Ordered ()
		{
			return (int)unit_flags & U_ORDERED;
		}

		public int DontPlan ()
		{
			return (int)unit_flags & U_NO_PLANNING;
		}

		public int Parent ()
		{
			return (int)unit_flags & U_PARENT;
		}

		public int Engaged ()
		{
			return (int)unit_flags & U_ENGAGED;
		}

		public int Scripted ()
		{
			return (int)unit_flags & U_SCRIPTED;
		}

		public int Commando ()
		{
			return (int)unit_flags & U_COMMANDO;
		}

		public int Moving ()
		{
			return (int)unit_flags & U_MOVING;
		}

		public int Refused ()
		{
			return (int)unit_flags & U_REFUSED;
		}

		public int Cargo ()
		{
			return (int)unit_flags & U_CARGO;
		}

		public int Combat ()
		{
			return (int)unit_flags & U_COMBAT;
		}

		public int Broken ()
		{
			return (int)unit_flags & U_BROKEN;
		}

		public int Aborted ()
		{
			return (int)unit_flags & U_BROKEN;
		}

		public int Losses ()
		{
			return (int)unit_flags & U_LOSSES;
		}

		public int Inactive ()
		{
			return (int)unit_flags & U_INACTIVE;
		}

		public int Fragment ()
		{
			return (int)unit_flags & U_FRAGMENTED;
		}

		public int Targeted ()
		{
			return (int)unit_flags & U_TARGETED;
		}

		public int Retreating ()
		{
			return (int)unit_flags & U_RETREATING;
		}

		public int Detached ()
		{
			return (int)unit_flags & U_DETACHED;
		}

		public int Supported ()
		{
			return (int)unit_flags & U_SUPPORTED;
		}

		public int TempDest ()
		{
			return (int)unit_flags & U_TEMP_DEST;
		}

		public int Final ()
		{
			return (int)unit_flags & U_FINAL;
		}

		public int HasPilots ()
		{
			return (int)unit_flags & U_HAS_PILOTS;
		}

		public int Diverted ()
		{
			return (int)unit_flags & U_DIVERTED;
		}

		public int Fired ()
		{
			return (int)unit_flags & U_FIRED;
		}

		public int Locked ()
		{
			return (int)unit_flags & U_LOCKED;
		}

		public int IAKill ()
		{
			return (int)unit_flags & U_IA_KILL;
		}

		public int NoAbort ()
		{
			return (int)unit_flags & U_NO_ABORT;
		}

		// Entity information
		public UnitClassDataType GetUnitClassData ()
		{throw new NotImplementedException();}

		public string GetUnitClassName ()
		{throw new NotImplementedException();}

		public void SetUnitAltitude (int alt)
		{
			SetPosition (XPos (), YPos (), -1.0F * (float)alt);
			MakeCampBaseDirty (DIRTY_ALTITUDE, DDP [181].priority);
		}
		//void SetUnitAltitude (int alt)					{ SetPosition(XPos(),YPos(),-1.0F * (float)alt); MakeCampBaseDirty (DIRTY_ALTITUDE, SEND_SOON); }
		public int GetUnitAltitude ()
		{
			return FloatToInt32 (ZPos () * -1.0F);
		}

		public virtual void SimSetLocation (float x, float y, float z)
		{
			SetPosition (x, y, z);
			MakeCampBaseDirty (DIRTY_POSITION, DDP [182].priority);
			MakeCampBaseDirty (DIRTY_ALTITUDE, DDP [183].priority);
		}
		//virtual void SimSetLocation (float x, float y, float z)	{ SetPosition(x,y,z); MakeCampBaseDirty (DIRTY_POSITION, SEND_SOON); MakeCampBaseDirty (DIRTY_ALTITUDE, SEND_SOON); }
		public virtual void SimSetOrientation (float f1, float f2, float f3)
		{
		}

		public virtual void GetRealPosition (ref float f1, ref float f2, ref float f3)
		{
		}

		public virtual int GetBestVehicleWeapon (int i, byte[] buf, MoveType m, int a, int[] b)
		{throw new NotImplementedException();}

		public virtual int GetVehicleHitChance (int slot, MoveType mt, int range, int hitflags)
		{throw new NotImplementedException();}

		public virtual int GetVehicleCombatStrength (int slot, MoveType mt, int range)
		{throw new NotImplementedException();}

		public virtual int GetVehicleRange (int slot, int mt, FalconEntity target = null)
		{throw new NotImplementedException();} // 2002-03-08 MODIFIED BY S.G. Need to pass it a target sometime so default to null for most cases
		public virtual int GetUnitWeaponId (int hp, int slot)
		{throw new NotImplementedException();}

		public virtual int GetUnitWeaponCount (int hp, int slot)
		{throw new NotImplementedException();}

		// Unit_data information
		public void SetUnitDestination (GridIndex x, GridIndex y)
		{
			dest_x = (GridIndex)(x + 1);
			dest_y = (GridIndex)(y + 1);
		}
//		void SetUnitRoster (fourbyte r)					{ roster = r; }
		public void SetNumVehicles (int v, int n)
		{
			SetRoster ((roster & ~(3 << (v * 2))) | (n << (v * 2)));
		}

		public void SetTarget (FalconEntity e)
		{
			target_id = (e) ? e.Id () : FalconNullId;
		}

		public void SetUnitMoved (byte m)
		{
			moved = m;
		}

		public void SetUnitTactic (byte t)
		{
			tactic = t;
		}

		public void SetUnitReinforcementLevel (short r)
		{
			reinforcement = r;
		}

		public void GetUnitDestination (ref GridIndex x, ref GridIndex y)
		{throw new NotImplementedException();}
//		fourbyte GetUnitRoster () 					{ return roster; }
		public int GetNumVehicles (int v)
		{
			return (int)((roster >> (v * 2)) & 0x03);
		}

		public FalconEntity  GetTarget ()
		{
			return (FalconEntity )vuDatabase.Find (target_id);
		}

		public VU_ID GetTargetID ()
		{
			return target_id;
		}

		public SimBaseClass GetSimTarget ()
		{throw new NotImplementedException();}

		public CampBaseClass GetCampTarget ()
		{throw new NotImplementedException();}

		public CampEntity GetCargo ()
		{throw new NotImplementedException();}

		public CampEntity GetTransport ()
		{throw new NotImplementedException();}

		public VU_ID GetCargoID ()
		{throw new NotImplementedException();}

		public VU_ID GetTransportID ()
		{throw new NotImplementedException();}

		public int GetUnitMoved ()
		{
			return moved;
		}

		public int GetUnitTactic ()
		{
			return tactic;
		}

		public int GetUnitReinforcementLevel ()
		{
			return reinforcement;
		}

		public void AssignUnit (VU_ID mgr, VU_ID po, VU_ID so, VU_ID ao, int orders)
		{throw new NotImplementedException();}

		public void SetUnitNameID (short id)
		{
			name_id = id;
		}

		public int SetUnitSType (char t)
		{throw new NotImplementedException();}

		public int SetUnitSPType (char t)
		{throw new NotImplementedException();}

		public int GetUnitNameID ()
		{
			return name_id;
		}

		// Attribute data
		public VehicleID GetVehicleID (int v)
		{throw new NotImplementedException();}

		public int GetTotalVehicles ()
		{throw new NotImplementedException();}

		public int GetFullstrengthVehicles ()
		{throw new NotImplementedException();}

		public int GetFullstrengthVehicles (int slot)
		{throw new NotImplementedException();}

		public int GetMaxSpeed ()
		{throw new NotImplementedException();}

		public int GetCruiseSpeed ()
		{throw new NotImplementedException();}

		public int GetCombatSpeed ()
		{throw new NotImplementedException();}

		public int GetUnitEndurance ()
		{throw new NotImplementedException();}

		public int GetUnitRange ()
		{throw new NotImplementedException();}

		public int GetRClass ()
		{throw new NotImplementedException();}

		// Support routines
		public CampaignTime GetUnitReassesTime ()
		{throw new NotImplementedException();}

		public int CountUnitElements ()
		{throw new NotImplementedException();}

		public Unit GetRandomElement ()
		{throw new NotImplementedException();}

		public void ResetMoves ()
		{throw new NotImplementedException();}

		public void ResetLocations (GridIndex x, GridIndex y)
		{throw new NotImplementedException();}

		public void ResetDestinations (GridIndex x, GridIndex y)
		{throw new NotImplementedException();}

		public void ResetFlags ()
		{throw new NotImplementedException();}

		public void SortElementsByDistance (GridIndex x, GridIndex y)
		{throw new NotImplementedException();}

		public int FirstSP ()
		{throw new NotImplementedException();}

		public Unit FindPrevUnit (ref short type)
		{throw new NotImplementedException();}

		public void SaveUnits (int FHandle, int flags)
		{throw new NotImplementedException();}

		public void BuildElements ()
		{throw new NotImplementedException();}

		public int ChangeVehicles (int a)
		{throw new NotImplementedException();}

		public int GetFormationCruiseSpeed ()
		{throw new NotImplementedException();}

		public void KillUnit ()
		{throw new NotImplementedException();}

		public int NoMission ()
		{throw new NotImplementedException();}

		public int AtDestination ()
		{throw new NotImplementedException();}

		public int GetUnitFormation ()
		{throw new NotImplementedException();}

		public int GetUnitRoleScore (int role, int calcType, int use_to_calc)
		{throw new NotImplementedException();}

		public float GetUnitMovementCost (GridIndex x, GridIndex y, CampaignHeading h)
		{throw new NotImplementedException();}

		public int GetUnitObjectivePath (Path p, Objective o, Objective t)
		{throw new NotImplementedException();}

		public int GetUnitGridPath (Path p, GridIndex x, GridIndex y, GridIndex xx, GridIndex yy)
		{throw new NotImplementedException();}

		public void LoadUnit (Unit cargo)
		{throw new NotImplementedException();}

		public void UnloadUnit ()
		{throw new NotImplementedException();}

		public CampaignTime GetUnitSupplyTime ()
		{throw new NotImplementedException();}

		// Waypoint routines
		public WayPoint AddUnitWP (GridIndex x, GridIndex y, int alt, int speed, CampaignTime arr, int station, byte mission)
		{throw new NotImplementedException();}

		public WayPoint AddWPAfter (WayPoint pw, GridIndex x, GridIndex y, int alt, int speed, CampaignTime arr, int station, byte mission)
		{throw new NotImplementedException();}

		public void DeleteUnitWP (WayPoint w)
		{throw new NotImplementedException();}

		public int EncodeWaypoints (byte[] stream)
		{throw new NotImplementedException();}

		public void DecodeWaypoints (byte[]stream)
		{throw new NotImplementedException();}

		public WayPoint GetFirstUnitWP ()
		{
			return wp_list;
		}

		public WayPoint GetCurrentUnitWP ()
		{throw new NotImplementedException();}

		public WayPoint GetUnitMissionWP ()
		{throw new NotImplementedException();}

		public void FinishUnitWP ()
		{throw new NotImplementedException();}

		public void DisposeWayPoints ()
		{throw new NotImplementedException();}

		public void CheckBroken ()
		{throw new NotImplementedException();}

		public void SetCurrentUnitWP (WayPoint w)
		{throw new NotImplementedException();}

		public void AdjustWayPoints ()
		{throw new NotImplementedException();}

		// Virtual Functions (These are empty except for those derived classes they belong to)
		// AirUnit virtuals
		// None

		// Flight virtuals
		public virtual void SetUnitLastMove (CampaignTime t)
		{
		}

		public virtual void SetCombatTime (CampaignTime t)
		{
		}

		public virtual void SetBurntFuel (long p)
		{
		}

		public virtual void SetUnitMission (byte p)
		{
		}

		public virtual void SetUnitRole (byte p)
		{
		}

		public virtual void SetUnitPriority (int p)
		{
		}

		public virtual void SetUnitMissionID (int p)
		{
		}

		public virtual void SetUnitMissionTarget (int p)
		{
		}

		public virtual void SetUnitTOT (CampaignTime p)
		{
		}

		public virtual void SetUnitSquadron (VU_ID p)
		{
		}

		public virtual void SetUnitAirbase (VU_ID p)
		{
		}

		public virtual void SetLoadout (LoadoutStruct p, int i)
		{
			Debug.Assert ("Shouldn't be here");
		}

		public virtual int GetNumberOfLoadouts ()
		{
			return 0;
		}

		public virtual CampaignTime GetMoveTime ()
		{
			return TheCampaign.CurrentTime - last_check;
		}

		public virtual CampaignTime GetCombatTime ()
		{
			return 0;
		}

		public virtual VU_ID GetAirTargetID ()
		{
			return FalconNullId;
		}

		public virtual FalconEntity  GetAirTarget ()
		{
			return null;
		}

		public virtual int GetBurntFuel ()
		{
			return 0;
		}

		public virtual MissionTypeEnum GetUnitMission ()
		{
			return (MissionTypeEnum)0;
		}

		public virtual int GetUnitNormalRole ()
		{
			return 0;
		}

		public virtual int GetUnitCurrentRole ()
		{
			return 0;
		}

		public virtual int GetUnitPriority ()
		{
			return 0;
		}

		public virtual CampEntity GetUnitMissionTarget ()
		{
			return null;
		}

		public virtual VU_ID GetUnitMissionTargetID ()
		{
			return FalconNullId;
		}

		public virtual int GetUnitMissionID ()
		{
			return 0;
		}

		public virtual CampaignTime GetUnitTOT ()
		{
			return 0;
		}

		public virtual Unit GetUnitSquadron ()
		{
			return null;
		}

		public virtual VU_ID GetUnitSquadronID ()
		{
			return FalconNullId;
		}

		public virtual CampEntity GetUnitAirbase ()
		{
			return null;
		}

		public virtual VU_ID GetUnitAirbaseID ()
		{
			return FalconNullId;
		}

		public virtual int LoadWeapons (object p, byte[] buf, MoveType m, int a, int b, int c)
		{
			return 0;
		}

		public virtual int DumpWeapons ()
		{
			return 0;
		}

		public virtual CampaignTime ETA ()
		{
			return 0;
		}

		public virtual F4PFList GetKnownEmitters ()
		{
			return null;
		}

		public virtual int BuildMission (MissionRequestClass m)
		{
			return 0;
		}

		public virtual void IncrementTime (CampaignTime t)
		{
		}

		public virtual void UseFuel (long f)
		{
		}
			
		// Squadron virtuals
		public virtual void SetUnitSpecialty (int p)
		{
		}

		public virtual void SetUnitSupply (int p)
		{
		}

		public virtual void SetUnitMorale (int p)
		{
		}

		public virtual void SetSquadronFuel (long p)
		{
		}

		public virtual void SetUnitStores (int i, byte p)
		{
		}

		public virtual void SetLastResupply (int i)
		{
		}

		public virtual void SetLastResupplyTime (CampaignTime t)
		{
		}

		public virtual int GetUnitSpecialty ()
		{
			return 0;
		}

		public virtual int GetUnitSupply ()
		{
			return 0;
		}

		public virtual int GetUnitMorale ()
		{
			return 0;
		}

		public virtual long GetSquadronFuel ()
		{
			return 0;
		}

		public virtual byte GetUnitStores (int s)
		{
			return 0;
		}

		public virtual CampaignTime GetLastResupplyTime ()
		{
			return TheCampaign.CurrentTime;
		}

		public virtual int GetLastResupply ()
		{
			return 0;
		}

		// Package virtuals
		public virtual int BuildPackage (MissionRequest p, F4PFList d)
		{
			return 0;
		}

		public virtual void HandleRequestReceipt (int a, int b, VU_ID i)
		{
		}

		public virtual void SetUnitAssemblyPoint (int i, GridIndex x, GridIndex y)
		{
		}

		public virtual void GetUnitAssemblyPoint (int i, ref GridIndex x, ref GridIndex y)
		{
		}

		// Ground Unit virtuals
		public virtual void SetUnitPrimaryObj (VU_ID i)
		{
		}

		public virtual void SetUnitSecondaryObj (VU_ID i)
		{
		}

		public virtual void SetUnitObjective (VU_ID i)
		{
		}

		public virtual void SetUnitOrders (int i)
		{
		}

		public virtual void SetUnitOrders (int p, VU_ID i)
		{
		}

		public virtual void SetUnitFatigue (int i)
		{
		}
//		virtual void SetUnitElement (int e)							{}
		public virtual void SetUnitMode (int i)
		{
		}

		public virtual void SetUnitPosition (int i)
		{
		}

		public virtual void SetUnitDivision (int i)
		{
		}

		public virtual void SetUnitHeading (int i)
		{
		}

		public virtual Objective GetUnitPrimaryObj ()
		{
			return null;
		}

		public virtual Objective GetUnitSecondaryObj ()
		{
			return null;
		}

		public virtual Objective GetUnitObjective ()
		{
			return null;
		}

		public virtual VU_ID GetUnitPrimaryObjID ()
		{
			return FalconNullId;
		}

		public virtual VU_ID GetUnitSecondaryObjID ()
		{
			return FalconNullId;
		}

		public virtual VU_ID GetUnitObjectiveID ()
		{
			return FalconNullId;
		}

		public virtual int GetUnitOrders ()
		{
			return 0;
		}

		public virtual int GetUnitFatigue ()
		{
			return 0;
		}

		public virtual int GetUnitElement ()
		{
			return 0;
		}

		public virtual int GetUnitMode ()
		{
			return 0;
		}

		public virtual int GetUnitPosition ()
		{
			return 0;
		}

		public virtual int GetUnitDivision ()
		{
			return 0;
		}

		public virtual int GetUnitHeading ()
		{
			return Here;
		}

		public virtual void SetUnitNextMove ()
		{
		}

		public virtual void ClearUnitPath ()
		{
		}

		public virtual int GetNextMoveDirection ()
		{
			return Here;
		}

		public virtual void SetUnitCurrentDestination (GridIndex x, GridIndex y)
		{
		}

		public virtual void GetUnitCurrentDestination (ref GridIndex x, ref GridIndex y)
		{
		}

		public virtual MoveType GetObjMovementType (Objective o, int i)
		{
			return CampBaseClass.GetMovementType ();
		}

		public virtual int CheckForSurrender ()
		{
			return 1;
		}

		public virtual int BuildMission ()
		{
			return 0;
		}

		public virtual int RallyUnit (int i)
		{
			return 0;
		}

		// Battalion virtuals
		public virtual Unit GetUnitParent ()
		{
			return null;
		}

		public virtual VU_ID GetUnitParentID ()
		{
			return FalconNullId;
		}

		public virtual void SetUnitParent (Unit u)
		{
		}
#if USE_FLANKS
		virtual void GetLeftFlank (GridIndex *x, GridIndex *y)		{ GetLocation(x,y); }
		virtual void GetRightFlank (GridIndex *x, GridIndex *y)		{ GetLocation(x,y); }
#endif

		// Brigade virtuals
		public virtual Unit GetFirstUnitElement ()
		{
			return null;
		}

		public virtual Unit GetNextUnitElement ()
		{
			return null;
		}

		public virtual Unit GetUnitElement (int i)
		{
			return null;
		}

		public virtual Unit GetUnitElementByID (int i)
		{
			return null;
		}

		public virtual Unit GetPrevUnitElement (Unit u)
		{
			return null;
		}

		public virtual void AddUnitChild (Unit u)
		{
		}

		public virtual void DisposeChildren ()
		{
		}

		public virtual void RemoveChild (VU_ID i)
		{
		}

		public virtual void ReorganizeUnit ()
		{
		}

		public virtual int UpdateParentStatistics ()
		{
			return 0;
		}

		public void CalculateSOJ (VuGridIterator iter)
		{throw new NotImplementedException();}
		// Naval Unit virtuals
		// None


		// JB SOJ
		protected CampEntity sojSource;
		protected int sojOctant;
		protected float sojRangeSq;
	}

	public class UnitDriver : VuMaster
	{
		public UnitDriver (VuEntity entity)
		{throw new NotImplementedException();}
		// TODO public virtual ~UnitDriver();

		public virtual void AutoExec (VU_TIME timestamp)
		{throw new NotImplementedException();}

		public virtual bool ExecModel (VU_TIME timestamp)
		{throw new NotImplementedException();}
	}

	// ============================================================
	// Deaggregated data class
	// ============================================================

	public struct UnitPosition
	{
		public float			x, y, heading;
	}

// This class is used to store unit positions while it's in an aggregate state
	public class UnitDeaggregationData
	{

#if USE_SH_POOLS
   public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { ShiAssert( size == sizeof(UnitDeaggregationData) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(UnitDeaggregationData), 50, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif

	
		public short			num_vehicles;
		public UnitPosition[]	position_data = new UnitPosition[VEHICLES_PER_UNIT * 3];

		public UnitDeaggregationData ()
		{throw new NotImplementedException();}
		// TODO public ~UnitDeaggregationData ();

		public void StoreDeaggregationData (Unit theUnit)
		{throw new NotImplementedException();}
	}

// ============================================================
// Unit manipulation routines											
// ============================================================
	public static class UnitStatic
	{
		public static  void SaveUnits (string FileName)
		{throw new NotImplementedException();}
		
		public static  int LoadUnits (string FileName)
		{throw new NotImplementedException();}
		
		public static  Unit GetFirstUnit (F4LIt l)
		{throw new NotImplementedException();}
		
		public static  Unit GetNextUnit (F4LIt l)
		{throw new NotImplementedException();}
		
		public static  Unit LoadAUnit (int Num, int FHandle, Unit parent)
		{throw new NotImplementedException();}
		
		public static  DamageDataType GetDamageType (Unit u)
		{throw new NotImplementedException();}
		
		public static  Unit ConvertUnit (Unit u, int domain, int type, int stype, int sptype)
		{throw new NotImplementedException();}
		
		public static  int GetUnitRole (Unit u)
		{throw new NotImplementedException();}
		
		public static  string GetSizeName (int domain, int type, char[] buffer)
		{throw new NotImplementedException();}
		
		public static  string GetDivisionName (int div, char[] buffer, int size, int obj)
		{throw new NotImplementedException();}
		
		public static  int FindUnitNameID (Unit u)
		{throw new NotImplementedException();}
		
		public static  Unit NewUnit (int domain, int type, int stype, int sptype, Unit parent)
		{throw new NotImplementedException();}
		
		public static  Unit NewUnit (short tid, VU_BYTE[] stream)
		{throw new NotImplementedException();}
		
		public static  float GetOdds (Unit us, CampEntity them, int range)
		{throw new NotImplementedException();}
		
		public static  float GetRange (Unit us, CampEntity them)
		{throw new NotImplementedException();}
		
		public static  int EncodeUnitData (VU_BYTE[] stream, FalconSessionEntity owner)
		{throw new NotImplementedException();}
		
		public static  int DecodeUnitData (VU_BYTE[] stream, FalconSessionEntity owner)
		{throw new NotImplementedException();}
	}
}

