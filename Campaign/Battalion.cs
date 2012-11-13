using System;
using FalconNet.Common;
using FalconNet.VU;
using FalconNet.FalcLib;

namespace FalconNet.Campaign
{
					
	// =========================
	// Battalion Class
	// =========================
	public class BattalionClass :  GroundUnitClass
	{
#if USE_SH_POOLS
   
      public // Overload new/delete to use a SmartHeap fixed size pool
      public void *operator new(size_t size) { ShiAssert( size == sizeof(BattalionClass) ); return MemAllocFS(pool);	};
      public void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      public static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(BattalionClass), 200, 0 ); };
      public static void ReleaseStorage()	{ MemPoolFree( pool ); };
      public static MEM_POOL	pool;
#endif

   

		private Percentage		supply;					// Unit statistics
		private Percentage      fatigue;
		private Percentage      morale;
		private byte			final_heading;			// Unit facing, at destination
		private byte			heading;				// Formation heading
		private byte			fullstrength;			// Number of vehicles at fullstrength
		private byte			radar_mode;				// Radar mode
		private byte			position;
		private byte			search_mode;			// Radar Search mode
		private byte			missiles_flying;		// Number of missiles being guided
		private VU_TIME SEARCHtimer;
		private VU_TIME AQUIREtimer;
		private byte			step_search_mode;		// 2002-03-04 ADDED BY S.G. The search mode used by radar stepping

		private int				dirty_battalion;
		private CampaignTime	last_resupply_time;		// Last time this unit received supplies

	
		public CampaignTime	last_move;				// Time we moved last
		public CampaignTime	last_combat;				// Last time this entity fired its weapons
		public VU_ID			parent_id;				// Brigade parent, if present
		public VU_ID			last_obj;				// The last objective this unit visited
		public VU_ID			air_target;				// The ID of any air target (in addition to regular target)
#if USE_FLANKS
		public GridIndex		lfx,lfy;				// Left flank
		public GridIndex		rfx,rfy;				// Right flank
#endif
		public SmallPathClass	path;					// The unit's path
//		byte           element;     			// Unit's position
		public UnitDeaggregationData	deag_data;		// Position data of previously deaggregated elements

	
		// constructors and serial functions
		public BattalionClass (int type, Unit parent)
		{throw new NotImplementedException();}

		public BattalionClass (VU_BYTE[] stream)
		{throw new NotImplementedException();}
		//TODO public virtual ~BattalionClass();
		public virtual int SaveSize ()
		{throw new NotImplementedException();}

		public virtual int Save (VU_BYTE[] stream)
		{throw new NotImplementedException();}

		// event Handlers
		public virtual VU_ERRCODE Handle (VuFullUpdateEvent evnt)
		{throw new NotImplementedException();}

		public virtual int IsBattalion ()
		{
			return TRUE;
		}

		// Access Functions
		public byte GetFinalHeading ()
		{
			return final_heading;
		}

		public byte GetFullStrength ()
		{
			return fullstrength;
		}

		public void SetFinalHeading (byte p)
		{throw new NotImplementedException();}

		public void SetFullStrength (byte p)
		{throw new NotImplementedException();}

		// Required pure virtuals handled by BattalionClass
		public override int MoveUnit (CampaignTime time)
		{throw new NotImplementedException();}

		public override int DoCombat ()
		{throw new NotImplementedException();}

		public virtual UnitDeaggregationData GetUnitDeaggregationData ()
		{throw new NotImplementedException();}

		public virtual void ClearDeaggregationData ()
		{throw new NotImplementedException();}

		public virtual int GetDeaggregationPoint (int slot, CampEntity ent)
		{throw new NotImplementedException();}

		public virtual int Reaction (CampEntity what, int zone, float range)
		{throw new NotImplementedException();}

		public virtual int ChooseTactic ()
		{throw new NotImplementedException();}

		public virtual int CheckTactic (int tid)
		{throw new NotImplementedException();}

		public virtual int Real ()
		{
			return 1;
		}

		public virtual void SetUnitOrders (int o, VU_ID oid)
		{throw new NotImplementedException();}

		public void PickFinalLocation ()
		{throw new NotImplementedException();}
//		virtual void SetUnitAction ();
		public virtual int GetCruiseSpeed ()
		{throw new NotImplementedException();}

		public virtual int GetCombatSpeed ()
		{throw new NotImplementedException();}

		public virtual int GetMaxSpeed ()
		{throw new NotImplementedException();}

		public virtual int GetUnitSpeed ()
		{throw new NotImplementedException();}

		public virtual CampaignTime UpdateTime ()
		{throw new NotImplementedException();}

		public virtual CampaignTime CombatTime ()
		{
			return GROUND_COMBAT_CHECK_INTERVAL * CampaignSeconds;
		}

		public virtual int GetUnitSupplyNeed (int total)
		{throw new NotImplementedException();}

		public virtual int GetUnitFuelNeed (int total)
		{throw new NotImplementedException();}

		public virtual void SupplyUnit (int supply, int fuel)
		{throw new NotImplementedException();}

		public virtual int GetDetectionRange (int mt)
		{throw new NotImplementedException();}				// Takes into account emitter status
		public virtual int GetElectronicDetectionRange (int mt)
		{throw new NotImplementedException();}	// Max Electronic detection range, even if turned off
		public virtual int GetRadarMode ()
		{
			return radar_mode;
		}

		public virtual int GetSearchMode ()
		{
			return search_mode;
		}
// 2001-06-27 MODIFIED BY S.G. DIFFERENT DECLARATION THEN FROM FalcEnt.h RESULTS IN IT NOT BEING CALLED!
//		virtual void SetRadarMode (byte mode)				{ radar_mode = mode; }
		public virtual void SetRadarMode (int mode)
		{
			radar_mode = mode;
		}

		public virtual void ReturnToSearch ()
		{throw new NotImplementedException();}
// 2001-06-27 MODIFIED BY S.G. DIFFERENT DECLARATION THEN FROM FalcEnt.h RESULTS IN IT NOT BEING CALLED!
//		virtual void SetSearchMode (byte mode)				{ search_mode = mode; }
		public virtual void SetSearchMode (int mode)
		{
			step_search_mode = search_mode = mode;
		} // 2002-03-22 MODIFIED BY S.G. Init our step_search_mode as well
		public virtual int CanShootWeapon (int wid)
		{throw new NotImplementedException();}

		public virtual int StepRadar (int t, int d, float range)
		{throw new NotImplementedException();} //me123 modifyed to take tracking/detection range parameter
		public virtual int GetVehicleDeagData (SimInitDataClass simdata, int remote)
		{throw new NotImplementedException();}

		public virtual int GetMissilesFlying ()
		{
			return missiles_flying;
		}//me123
		
		// core functions
		public virtual void SetUnitLastMove (CampaignTime t)
		{
			last_move = t;
		}

		public virtual void SetCombatTime (CampaignTime t)
		{
			last_combat = t;
		}

		public virtual void SetUnitParent (Unit p)
		{
			parent_id = p.Id ();
		}

		public virtual void SetUnitSupply (int s)
		{throw new NotImplementedException();}

		public virtual void SetUnitFatigue (int f)
		{throw new NotImplementedException();}

		public virtual void SetUnitMorale (int m)
		{throw new NotImplementedException();}

		public virtual void SetUnitHeading (byte h)
		{
			heading = h;
		}

		public virtual void SetUnitNextMove ()
		{
			path.StepPath ();
		}

		public virtual void ClearUnitPath ()
		{
			path.ClearPath ();
		}

		public virtual void SetLastResupplyTime (CampaignTime t)
		{
			last_resupply_time = t;
		}

		public virtual void SetUnitPosition (byte p)
		{
			position = p;
		}

		public override void SimSetLocation (float x, float y, float z)
		{throw new NotImplementedException();}

		public override void GetRealPosition (ref float x, ref float y, ref float z)
		{throw new NotImplementedException();}

		public override void HandleRequestReceipt (int type, int them, VU_ID flight)
		{throw new NotImplementedException();}

		public override CampaignTime GetMoveTime ()
		{throw new NotImplementedException();}

		public override CampaignTime GetCombatTime ()
		{
			return (TheCampaign.CurrentTime > last_combat) ? TheCampaign.CurrentTime - last_combat : 0;
		}

		public override UnitClass GetUnitParent ()
		{
			return (UnitClass)vuDatabase.Find (parent_id);
		}

		public override VU_ID GetUnitParentID ()
		{
			return parent_id;
		}

		public override VU_ID GetAirTargetID ()
		{
			return air_target;
		}

		public override FalconEntity GetAirTarget ()
		{
			return (FalconEntity)vuDatabase.Find (air_target);
		}

		public virtual void SetAirTarget (FalconEntity t)
		{
			if (t != null)
				air_target = t.Id ();
			else
				air_target = FalconNullId;
		}

		public void IncrementMissileCount ()
		{
			missiles_flying++;
			SetRadarMode (FEC_RADAR_GUIDE);
		}

		public void DecrementMissileCount ()
		{
			missiles_flying--;
			if (missiles_flying == 0)
				ReturnToSearch ();
			ShiAssert (missiles_flying >= 0);
		}
#if USE_FLANKS
		virtual void GetLeftFlank (GridIndex *x, GridIndex *y)	{ *x = lfx; *y = lfy; }
		virtual void GetRightFlank (GridIndex *x, GridIndex *y)	{ *x = rfx; *y = rfy; }
#endif
		public override int GetUnitSupply ()
		{
			return (int)supply;
		}

		public override int GetUnitFatigue ()
		{
			return (int)fatigue;
		}

		public override int GetUnitMorale ()
		{
			return (int)morale;
		}

		public override int GetUnitHeading ()
		{
			return (int)heading;
		}

		public override int GetNextMoveDirection ()
		{
			return path.GetNextDirection ();
		}

		public override int GetUnitElement ()
		{throw new NotImplementedException();}

		public override int GetUnitPosition ()
		{
			return position;
		}

		public override int RallyUnit (int minutes)
		{throw new NotImplementedException();}

		public override CampaignTime GetLastResupplyTime ()
		{
			return last_resupply_time;
		}

		// Support functions
		public float GetSpeedModifier ()
		{throw new NotImplementedException();}

		public override float AdjustForSupply ()
		{throw new NotImplementedException();}

		public override void IncrementTime (CampaignTime dt)
		{
			last_move += dt;
		}

		// Dirty Data
		public void MakeBattalionDirty (Dirty_Battalion bits, Dirtyness score)
		{throw new NotImplementedException();}

		public void WriteDirty (byte[] stream)
		{throw new NotImplementedException();}

		public void ReadDirty (byte[] stream)
		{throw new NotImplementedException();}
		// 2002-03-22 ADDED BY S.G. Needs them outside of battalion class
		public virtual void SetAQUIREtimer (VU_TIME newTime)
		{
			AQUIREtimer = newTime;
		}

		public virtual void SetSEARCHtimer (VU_TIME newTime)
		{
			SEARCHtimer = newTime;
		}

		public virtual void SetStepSearchMode (int mode)
		{
			step_search_mode = mode;
		}

		public virtual VU_TIME GetAQUIREtimer ()
		{
			return AQUIREtimer;
		}

		public virtual VU_TIME GetSEARCHtimer ()
		{
			return SEARCHtimer;
		}
		// END OF ADDED SECTION 2002-03-22
		
		public static BattalionClass NewBattalion (int type, Unit parent)
		{throw new NotImplementedException();}

	}
}

