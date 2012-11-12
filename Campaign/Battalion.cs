using System;
using FalconNet.Common;

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
		private uchar			final_heading;			// Unit facing, at destination
		private uchar			heading;				// Formation heading
		private uchar			fullstrength;			// Number of vehicles at fullstrength
		private uchar			radar_mode;				// Radar mode
		private uchar			position;
		private uchar			search_mode;			// Radar Search mode
		private uchar			missiles_flying;		// Number of missiles being guided
		private VU_TIME SEARCHtimer;
		private VU_TIME AQUIREtimer;
		private uchar			step_search_mode;		// 2002-03-04 ADDED BY S.G. The search mode used by radar stepping

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
//		uchar           element;     			// Unit's position
		public UnitDeaggregationData	*deag_data;		// Position data of previously deaggregated elements

	
		// constructors and serial functions
		public BattalionClass (int type, Unit parent)
;
		public BattalionClass (VU_BYTE **stream);
		//TODO public virtual ~BattalionClass();
		public virtual int SaveSize ();

		public virtual int Save (VU_BYTE **stream);

		// event Handlers
		public virtual VU_ERRCODE Handle (VuFullUpdateEvent *evnt);

		public virtual int IsBattalion ()
		{
			return TRUE;
		}

		// Access Functions
		public uchar GetFinalHeading ()
		{
			return final_heading;
		}

		public uchar GetFullStrength ()
		{
			return fullstrength;
		}

		public void SetFinalHeading (uchar p);

		public void SetFullStrength (uchar p);

		// Required pure virtuals handled by BattalionClass
		public virtual int MoveUnit (CampaignTime time);

		public virtual int DoCombat ();

		public virtual UnitDeaggregationData* GetUnitDeaggregationData ();

		public virtual void ClearDeaggregationData ();

		public virtual int GetDeaggregationPoint (int slot, CampEntity *ent);

		public virtual int Reaction (CampEntity what, int zone, float range);

		public virtual int ChooseTactic ();

		public virtual int CheckTactic (int tid);

		public virtual int Real ()
		{
			return 1;
		}

		public virtual void SetUnitOrders (int o, VU_ID oid);

		public void PickFinalLocation ();
//		virtual void SetUnitAction ();
		public virtual int GetCruiseSpeed ();

		public virtual int GetCombatSpeed ();

		public virtual int GetMaxSpeed ();

		public virtual int GetUnitSpeed ();

		public virtual CampaignTime UpdateTime ();

		public virtual CampaignTime CombatTime ()
		{
			return GROUND_COMBAT_CHECK_INTERVAL * CampaignSeconds;
		}

		public virtual int GetUnitSupplyNeed (int total);

		public virtual int GetUnitFuelNeed (int total);

		public virtual void SupplyUnit (int supply, int fuel);

		public virtual int GetDetectionRange (int mt);				// Takes into account emitter status
		public virtual int GetElectronicDetectionRange (int mt);	// Max Electronic detection range, even if turned off
		public virtual int GetRadarMode ()
		{
			return radar_mode;
		}

		public virtual int GetSearchMode ()
		{
			return search_mode;
		}
// 2001-06-27 MODIFIED BY S.G. DIFFERENT DECLARATION THEN FROM FalcEnt.h RESULTS IN IT NOT BEING CALLED!
//		virtual void SetRadarMode (uchar mode)				{ radar_mode = mode; }
		public virtual void SetRadarMode (int mode)
		{
			radar_mode = mode;
		}

		public virtual void ReturnToSearch ();
// 2001-06-27 MODIFIED BY S.G. DIFFERENT DECLARATION THEN FROM FalcEnt.h RESULTS IN IT NOT BEING CALLED!
//		virtual void SetSearchMode (uchar mode)				{ search_mode = mode; }
		public virtual void SetSearchMode (int mode)
		{
			step_search_mode = search_mode = mode;
		} // 2002-03-22 MODIFIED BY S.G. Init our step_search_mode as well
		public virtual int CanShootWeapon (int wid);

		public virtual int StepRadar (int t, int d, float range); //me123 modifyed to take tracking/detection range parameter
		public virtual int GetVehicleDeagData (SimInitDataClass *simdata, int remote);

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
			parent_id = p->Id ();
		}

		public virtual void SetUnitSupply (int s);

		public virtual void SetUnitFatigue (int f);

		public virtual void SetUnitMorale (int m);

		public virtual void SetUnitHeading (uchar h)
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

		public virtual void SetUnitPosition (uchar p)
		{
			position = p;
		}

		public virtual void SimSetLocation (float x, float y, float z);

		public virtual void GetRealPosition (float *x, float *y, float *z);

		public virtual void HandleRequestReceipt (int type, int them, VU_ID flight);

		public virtual CampaignTime GetMoveTime ();

		public virtual CampaignTime GetCombatTime ()
		{
			return (TheCampaign.CurrentTime > last_combat) ? TheCampaign.CurrentTime - last_combat : 0;
		}

		public virtual Unit GetUnitParent ()
		{
			return (Unit)vuDatabase->Find (parent_id);
		}

		public virtual VU_ID GetUnitParentID ()
		{
			return parent_id;
		}

		public virtual VU_ID GetAirTargetID ()
		{
			return air_target;
		}

		public virtual FalconEntity* GetAirTarget ()
		{
			return (FalconEntity*)vuDatabase->Find (air_target);
		}

		public virtual void SetAirTarget (FalconEntity *t)
		{
			if (t)
				air_target = t->Id ();
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
		public virtual int GetUnitSupply ()
		{
			return (int)supply;
		}

		public virtual int GetUnitFatigue ()
		{
			return (int)fatigue;
		}

		public virtual int GetUnitMorale ()
		{
			return (int)morale;
		}

		public virtual int GetUnitHeading ()
		{
			return (int)heading;
		}

		public virtual int GetNextMoveDirection ()
		{
			return path.GetNextDirection ();
		}

		public virtual int GetUnitElement ();

		public virtual int GetUnitPosition ()
		{
			return position;
		}

		public virtual int RallyUnit (int minutes);

		public virtual CampaignTime GetLastResupplyTime ()
		{
			return last_resupply_time;
		}

		// Support functions
		public float GetSpeedModifier ();

		public virtual float AdjustForSupply ();

		public virtual void IncrementTime (CampaignTime dt)
		{
			last_move += dt;
		}

		// Dirty Data
		public void MakeBattalionDirty (Dirty_Battalion bits, Dirtyness score);

		public void WriteDirty (byte **stream);

		public void ReadDirty (byte **stream);
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
		
		public static BattalionClass NewBattalion (int type, Unit parent);

	}
}

