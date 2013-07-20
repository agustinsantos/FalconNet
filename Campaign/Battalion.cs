using System;
using FalconNet.Common;
using FalconNet.VU;
using VU_BYTE=System.Byte;
using VU_TIME = System.UInt64;
using Percentage=System.Int32;
using FalconNet.FalcLib;
using System.Diagnostics;
using Unit=FalconNet.Campaign.UnitClass;
using FalconNet.CampaignBase;

namespace FalconNet.Campaign
{
					
	// =========================
	// Battalion Class
	// =========================
	public class BattalionClass :  GroundUnitClass
	{
#if USE_SH_POOLS
   
      public // Overload new/delete to use a SmartHeap fixed size pool
      public void *operator new(size_t size) { Debug.Assert( size == sizeof(BattalionClass) ); return MemAllocFS(pool);	};
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
		private FEC_RADAR			radar_mode;				// Radar mode
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
		public BattalionClass (ushort type, Unit parent):base(type)
		{throw new NotImplementedException();}
#if TODO		
		public BattalionClass (byte[] stream, ref int offset):base(stream, ref offset)
		{throw new NotImplementedException();}
		
		public BattalionClass(byte[] bytes, ref int offset, int version)
            : base(bytes, ref offset, version)
        {
	
            last_move = BitConverter.ToUInt32(bytes, offset);
            offset += 4;

            last_combat = BitConverter.ToUInt32(bytes, offset);
            offset += 4;

            parent_id = new VU_ID();
            parent_id.num_ = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            parent_id.creator_ = BitConverter.ToUInt32(bytes, offset);
            offset += 4;

            last_obj = new VU_ID();
            last_obj.num_ = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            last_obj.creator_ = BitConverter.ToUInt32(bytes, offset);
            offset += 4;

            supply = bytes[offset];
            offset++;

            fatigue = bytes[offset];
            offset++;

            morale = bytes[offset];
            offset++;

            heading = bytes[offset];
            offset++;

            final_heading = bytes[offset];
            offset++;

            if (version < 15)
            {
                dummy = bytes[offset];
                offset++;
            }
            position = bytes[offset];
            offset++;

        }
		//TODO public virtual ~BattalionClass();
		public override int SaveSize ()
		{throw new NotImplementedException();}

		public override int Save (VU_BYTE[] stream)
		{throw new NotImplementedException();}
#endif
		// event Handlers
		public override VU_ERRCODE Handle (VuFullUpdateEvent evnt)
		{throw new NotImplementedException();}

		public override bool IsBattalion ()
		{
			return true;
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

		public override UnitDeaggregationData GetUnitDeaggregationData ()
		{throw new NotImplementedException();}

		public override void ClearDeaggregationData ()
		{throw new NotImplementedException();}

        public override int GetDeaggregationPoint(int slot, CampBaseClass ent)
		{throw new NotImplementedException();}

        public override int Reaction(CampBaseClass what, int zone, float range)
		{throw new NotImplementedException();}

		public override int ChooseTactic ()
		{throw new NotImplementedException();}

		public override int CheckTactic (int tid)
		{throw new NotImplementedException();}

		public override int Real ()
		{
			return 1;
		}

		public override void SetUnitOrders (int o, VU_ID oid)
		{throw new NotImplementedException();}

		public void PickFinalLocation ()
		{throw new NotImplementedException();}
//		virtual void SetUnitAction ();
		public override int GetCruiseSpeed ()
		{throw new NotImplementedException();}

		public override int GetCombatSpeed ()
		{throw new NotImplementedException();}

		public override int GetMaxSpeed ()
		{throw new NotImplementedException();}

		public override int GetUnitSpeed ()
		{throw new NotImplementedException();}

		public override CampaignTime UpdateTime ()
		{throw new NotImplementedException();}

		public override CampaignTime CombatTime ()
		{
			return new CampaignTime((ulong)AIInput.GROUND_COMBAT_CHECK_INTERVAL * CampaignTime.CampaignSeconds);
		}

		public override int GetUnitSupplyNeed (int total)
		{throw new NotImplementedException();}

		public override int GetUnitFuelNeed (int total)
		{throw new NotImplementedException();}

		public override void SupplyUnit (int supply, int fuel)
		{throw new NotImplementedException();}

		public override int GetDetectionRange (int mt)
		{throw new NotImplementedException();}				// Takes into account emitter status
        public override int GetElectronicDetectionRange(MoveType mt)
		{throw new NotImplementedException();}	// Max Electronic detection range, even if turned off
		public override FEC_RADAR GetRadarMode ()
		{
			return radar_mode;
		}

		public virtual int GetSearchMode ()
		{
			return search_mode;
		}
// 2001-06-27 MODIFIED BY S.G. DIFFERENT DECLARATION THEN FROM FalcEnt.h RESULTS IN IT NOT BEING CALLED!
//		virtual void SetRadarMode (byte mode)				{ radar_mode = mode; }
		public override void SetRadarMode (FEC_RADAR mode)
		{
			radar_mode = mode;
		}

		public override void ReturnToSearch ()
		{throw new NotImplementedException();}
// 2001-06-27 MODIFIED BY S.G. DIFFERENT DECLARATION THEN FROM FalcEnt.h RESULTS IN IT NOT BEING CALLED!
//		virtual void SetSearchMode (byte mode)				{ search_mode = mode; }
		public virtual void SetSearchMode (byte mode)
		{
			step_search_mode = search_mode = mode;
		} // 2002-03-22 MODIFIED BY S.G. Init our step_search_mode as well
		public override bool CanShootWeapon (int wid)
		{throw new NotImplementedException();}

		public override FEC_RADAR StepRadar (int t, int d, float range)
		{throw new NotImplementedException();} //me123 modifyed to take tracking/detection range parameter
		public override int GetVehicleDeagData (SimInitDataClass simdata, int remote)
		{throw new NotImplementedException();}

		public virtual int GetMissilesFlying ()
		{
			return missiles_flying;
		}//me123
		
		// core functions
		public override void SetUnitLastMove (CampaignTime t)
		{
			last_move = t;
		}

		public override void SetCombatTime (CampaignTime t)
		{
			last_combat = t;
		}

		public override void SetUnitParent (Unit p)
		{
			parent_id = p.Id ();
		}

		public override void SetUnitSupply (int s)
		{throw new NotImplementedException();}

		public override void SetUnitFatigue (int f)
		{throw new NotImplementedException();}

		public override void SetUnitMorale (int m)
		{throw new NotImplementedException();}

		public virtual void SetUnitHeading (byte h)
		{
			heading = h;
		}

		public override void SetUnitNextMove ()
		{
			path.StepPath ();
		}

		public override void ClearUnitPath ()
		{
			path.ClearPath ();
		}

		public override void SetLastResupplyTime (CampaignTime t)
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
			return new CampaignTime((CampaignClass.TheCampaign.CurrentTime.time > last_combat.time) ? 
				CampaignClass.TheCampaign.CurrentTime.time - last_combat.time : 0);
		}

		public override UnitClass GetUnitParent ()
		{
			return (UnitClass)VUSTATIC.vuDatabase.Find (parent_id);
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
			return (FalconEntity)VUSTATIC.vuDatabase.Find (air_target);
		}

		public virtual void SetAirTarget (FalconEntity t)
		{
			if (t != null)
				air_target = t.Id ();
			else
				air_target = VU_ID.FalconNullId;
		}

		public void IncrementMissileCount ()
		{
			missiles_flying++;
			SetRadarMode (FEC_RADAR.FEC_RADAR_GUIDE);
		}

		public void DecrementMissileCount ()
		{
			missiles_flying--;
			if (missiles_flying == 0)
				ReturnToSearch ();
			Debug.Assert (missiles_flying >= 0);
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

        public override void WriteDirty(byte[] stream, ref int pos)
		{throw new NotImplementedException();}

		public override void ReadDirty(byte[] stream, ref int pos)
		{throw new NotImplementedException();}
		// 2002-03-22 ADDED BY S.G. Needs them outside of battalion class
		public override void SetAQUIREtimer (VU_TIME newTime)
		{
			AQUIREtimer = newTime;
		}

		public override void SetSEARCHtimer (VU_TIME newTime)
		{
			SEARCHtimer = newTime;
		}

		public override void SetStepSearchMode (int mode)
		{
			// TODO	step_search_mode = mode;
			throw new NotImplementedException();
		}

		public override VU_TIME GetAQUIREtimer ()
		{
			return AQUIREtimer;
		}

		public override VU_TIME GetSEARCHtimer ()
		{
			return SEARCHtimer;
		}
		// END OF ADDED SECTION 2002-03-22
		
		public static BattalionClass NewBattalion (int type, Unit parent)
		{throw new NotImplementedException();}

	}
}

