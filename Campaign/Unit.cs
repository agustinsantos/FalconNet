using System;
using System.Diagnostics;
using Unit = FalconNet.Campaign.UnitClass;
using Objective = FalconNet.Campaign.ObjectiveClass;
using Path = FalconNet.Campaign.BasePathClass;
using WayPoint = FalconNet.CampaignBase.WayPointClass;
using FalconNet.VU;
using VU_BYTE = System.Byte;
using Team = System.SByte;
using VU_TIME = System.UInt64;
using GridIndex = System.Int16;
using Control = System.Byte;
using VU_ID_NUMBER = System.UInt64;
using VehicleID = System.Int16;
using FalconNet.FalcLib;
using FalconNet.Common;
using FalconNet.CampaignBase;

namespace FalconNet.Campaign
{
    // =========================
    // Types and Defines
    // =========================

    // Transmittable Flags
    [Flags]
    public enum TrasmittableFlags : int
    {
        U_DEAD = 0x1,
        U_B3 = 0x2,
        U_ASSIGNED = 0x4,
        U_ORDERED = 0x8,
        U_NO_PLANNING = 0x10,			// Don't run planning AI on this unit
        U_PARENT = 0x20,
        U_ENGAGED = 0x40,
        U_B1 = 0x80,
        U_SCRIPTED = 0x100,			// Mission/Route scripted- Don't run planning AI
        U_COMMANDO = 0x200,			// Act like a commando (hit commando sites && kill ourselves after x time)
        U_MOVING = 0x400,
        U_REFUSED = 0x800,			// A request for transport was refused
        U_HASECM = 0x1000,			// This unit has defensive electronic countermeasures
        U_CARGO = 0x2000,			// We're being carried by someone else (airborne/marine/carrier air)
        U_COMBAT = 0x4000,
        U_BROKEN = 0x8000,
        U_LOSSES = 0x10000,
        U_INACTIVE = 0x20000,		// Ignore this unit for all purposes (generally reinforcements)
        U_FRAGMENTED = 0x40000,			// This is a unit fragment (separated from it's origional unit)

        // Ground Unit Specific
        U_TARGETED = 0x100000,		// Unit's targeting is being done public static ally
        U_RETREATING = 0x200000,
        U_DETACHED = 0x400000,
        U_SUPPORTED = 0x800000,		// Support is coming to this unit's aide
        U_TEMP_DEST = 0x1000000,		// This unit's current destination is not it's final destination

        // Air Unit Specific
        U_FINAL = 0x100000,		// Package elements finalized and sent, or flight contains actual a/c
        U_HAS_PILOTS = 0x200000,		// Campaign has assigned this flight pilots
        U_DIVERTED = 0x400000,		// This flight is currently being diverted
        U_FIRED = 0x800000,		// This flight has taken a shot
        U_LOCKED = 0x1000000,		// Someone is locked on us
        U_IA_KILL = 0x2000000,		// Instant Action "Expects" this flight to be killed for the next level to start
        U_NO_ABORT = 0x4000000		// Whatever happens - whatever the loadout - don't ABORT
    }

    // 2002-02-13 ADDED BY MN for S.G.'s Identify - S.G. Wrong place. Needs to be in Falcon4.UCD so defined in Vehicle.h which is used by UnitClassDataType and VehicleClassDataType
    //  U_HAS_NCTR		0x10000000
    //  U_HAS_EXACT_RWR 0x20000000

    // We use these for broad class types
    public enum RCLASS
    {
        RCLASS_AIR = 0,
        RCLASS_GROUND = 1,
        RCLASS_AIRDEFENSE = 2,
        RCLASS_NAVAL = 3
    }

    // Types of calculations for certain functions
    public enum CALC
    {
        CALC_TOTAL = 0,
        CALC_AVERAGE = 1,
        CALC_MEAN = 2,
        CALC_MAX = 3,
        CALC_MIN = 4,
        CALC_PERCENTAGE = 5
    }

    // Flags for what variables to take into account for certain function
    public enum MISCFLAGS
    {
        USE_EXP = 0x01,
        USE_VEH_COUNT = 0x02,
        IGNORE_BROKEN = 0x04
    }

    // =========================
    // Unit Class
    // =========================
    public class UnitClass : CampBaseClass
    {

        private CampaignTime last_check;		// Last time we checked this unit
        private Int32 roster;			// 4 byte bitwise roster
        private TrasmittableFlags unit_flags;		// Various user flags
        private GridIndex dest_x;			// Destination
        private GridIndex dest_y;
        private VU_ID cargo_id;		// id of our cargo, or our carrier unit
        private VU_ID target_id;		// Target we're engaged with (there can be only one! (c))
        private byte moved;       	// Moves since check
        private byte losses;			// How many vehicles we've lost
        private byte tactic;			// Current Unit tactic
        private ushort current_wp;		// Which WP we're heading to
        private short name_id;		// Element number, used only for naming
        private short reinforcement;	// What reinforcement level this unit becomes active at
        private short odds;			// How much shit is coming our way
        private Dirty_Unit dirty_unit;
        public UnitClassDataType class_data;
        public DrawablePoint draw_pointer;	// inserted into draw list when unit aggregated
        public WayPoint wp_list;


        // access functions
        public CampaignTime GetLastCheck()
        {
            return last_check;
        }

        public Int32 GetRoster()
        {
            return roster;
        }

        public TrasmittableFlags GetUnitFlags()
        {
            return unit_flags;
        }

        public GridIndex GetDestX()
        {
            return dest_x;
        }

        public GridIndex GetDestY()
        {
            return dest_y;
        }

        public VU_ID GetCargoId()
        {
            return cargo_id;
        }

        public VU_ID GetTargetId()
        {
            return target_id;
        }

        public byte GetMoved()
        {
            return moved;
        }

        public byte GetLosses()
        {
            return losses;
        }

        public byte GetTactic()
        {
            return tactic;
        }

        public ushort GetCurrentWaypoint()
        {
            return current_wp;
        }

        public short GetNameId()
        {
            return name_id;
        }

        public short GetReinforcement()
        {
            return reinforcement;
        }

        public short GetOdds()
        {
            return odds;
        }

        public UnitClassDataType GetClassData()
        {
            return class_data;
        }

        public void SetLastCheck(CampaignTime t)
        {
            last_check = t;
#if HOTSPOT_FIX
    CampaignTime max = MaxUpdateTime();
    int randFactor;
    CampaignTime tenSecs = 10 * CampaignSeconds;

    if (max <= tenSecs)
    {
        // somwhere between -max/2 and max/2
        randFactor = (rand() % max) - (max / 2);
    }
    else
    {
        // somewhere between max - 5 and max + 5
        randFactor = (rand() % tenSecs) - (tenSecs / 2);
    }

    update_interval = max + randFactor;
#endif
            // MakeUnitDirty (DIRTY_LAST_CHECK, SEND_WHENEVER);
        }

        public void SetRoster(Int32 r)
        {
            roster = r;
            MakeUnitDirty(Dirty_Unit.DIRTY_ROSTER, Dirtyness.SEND_EVENTUALLY);
        }

        public void SetUnitFlags(TrasmittableFlags f)
        {
            if (unit_flags != f)
            {
                unit_flags = f;
                MakeUnitDirty(Dirty_Unit.DIRTY_UNIT_FLAGS, Dirtyness.SEND_EVENTUALLY);
            }
        }


        public void SetDestX(GridIndex x)
        {
            dest_x = x;
            MakeUnitDirty(Dirty_Unit.DIRTY_DESTINATION, Dirtyness.SEND_EVENTUALLY);
        }

        public void SetDestY(GridIndex y)
        {
            dest_y = y;
            MakeUnitDirty(Dirty_Unit.DIRTY_DESTINATION, Dirtyness.SEND_EVENTUALLY);
        }

        public void SetCargoId(VU_ID cid)
        {
            cargo_id = cid;
            MakeUnitDirty(Dirty_Unit.DIRTY_CARGO_ID, Dirtyness.SEND_EVENTUALLY);
        }


        public void SetTargetId(VU_ID tid)
        {
            target_id = tid;
            MakeUnitDirty(Dirty_Unit.DIRTY_TARGET_ID, Dirtyness.SEND_EVENTUALLY);
        }


        public void SetMoved(byte m)
        {
            moved = m;
            MakeUnitDirty(Dirty_Unit.DIRTY_MOVED, Dirtyness.SEND_EVENTUALLY);
        }

        public void SetLosses(byte l)
        {
            losses = l;
        }

        public void SetTactic(byte t)
        {
            tactic = t;
            MakeUnitDirty(Dirty_Unit.DIRTY_TACTIC, Dirtyness.SEND_EVENTUALLY);
        }

        public void SetCurrentWaypoint(ushort p)
        { throw new NotImplementedException(); }

        public void SetNameId(short nid)
        {
            name_id = nid;
        }

        public void SetReinforcement(short r)
        {
            reinforcement = r;
            MakeUnitDirty(Dirty_Unit.DIRTY_REINFORCEMENT, Dirtyness.SEND_EVENTUALLY);
        }

        public void SetOdds(short o)
        {
            odds = o;
        }

        public void MakeWaypointsDirty()
        {
            // KCK: This is not a great way of doing this.
            // Basically, it works fine for flights (which don't change waypoints often-
            // in fact, only on player tweaking). However, it would be fatal to do this
            // for Battalions (bandwidth wise). The bitch is, in Force-on-Force TE, we
            // need to. We can either do a TE check here (kinda scary) or set up a new
            // message for ordering battalions, which is what I'm going to do.
            if (IsFlight())
            {
                if (IsLocal())
                {
                    //MakeUnitDirty (DIRTY_WP_LIST, DDP[89].priority);
                    MakeUnitDirty(Dirty_Unit.DIRTY_WP_LIST, Dirtyness.SEND_NOW);
                }
                else
                {
                    // Need to send data to the host
                    VuSessionEntity target = (VuSessionEntity)VUSTATIC.vuDatabase.Find(OwnerId());
                    FalconFlightPlanMessage msg = new FalconFlightPlanMessage(Id(), target);
                    byte[] tmp = new byte[1024];
                    int ptr;

                    ptr = 0;
                    msg.dataBlock.size = EncodeWaypoints(tmp);
                    msg.dataBlock.type = FalconFlightPlanMessage.DataType.waypointData;
                    msg.dataBlock.data = new byte[msg.dataBlock.size];

                    Debug.Assert(msg.dataBlock.size < 1024);
                    msg.dataBlock.data = tmp; // TODO memcpy(msg.dataBlock.data, tmp, msg.dataBlock.size);
                    FalcMesgStatic.FalconSendMessage(msg, true);
                }
            }
        }

        // Dirty Functions
        public void MakeUnitDirty(Dirty_Unit bits, Dirtyness score)
        {
            if ((!IsLocal()) || (VuState() != VU_MEM_STATE.VU_MEM_ACTIVE))
            {
                return;
            }

            if (!IsAggregate() && (score != Dirtyness.SEND_RELIABLEANDOOB))
            {
                // increase score for deagged units
                score = (Dirtyness)((int)score << 4);
            }

            dirty_unit |= bits;
            MakeDirty(Dirty_Class.DIRTY_UNIT, score);
        }

        public override void WriteDirty(byte[] stream, ref int pos)
        {
#if TODO
    uchar
    *start,
    *ptr;

    start = *stream;
    ptr = *stream;

    // MonoPrint ("Send UC %08x\n", dirty_unit);

    // Encode it up
    *(ushort*) ptr = dirty_unit;
    ptr += sizeof(ushort);

    if (dirty_unit & Dirty_Unit.DIRTY_WAYPOINT)
    {
        *(ushort*)ptr = current_wp;
        ptr += sizeof(ushort);
#if DEBUG

        if (!current_wp)
            ShiAssert(!IsFlight());

#endif
    }

    /* if (dirty_unit & Dirty_Unit.DIRTY_LAST_CHECK)
     {
     *(CampaignTime*)ptr = last_check;
     ptr += sizeof (CampaignTime);
     }
     */
    if (dirty_unit &Dirty_Unit.DIRTY_ROSTER)
    {
        *(fourbyte*)ptr = roster;
        ptr += sizeof(fourbyte);
    }

    if (dirty_unit & Dirty_Unit.DIRTY_UNIT_FLAGS)
    {
        *(fourbyte*)ptr = unit_flags;
        ptr += sizeof(fourbyte);
    }

    if (dirty_unit & Dirty_Unit.DIRTY_DESTINATION)
    {
        *(GridIndex*)ptr = dest_x;
        ptr += sizeof(GridIndex);

        *(GridIndex*)ptr = dest_y;
        ptr += sizeof(GridIndex);
    }

    if (dirty_unit & Dirty_Unit.DIRTY_CARGO_ID)
    {
        *(VU_ID*)ptr = cargo_id;
        ptr += sizeof(VU_ID);
    }

    if (dirty_unit & Dirty_Unit.DIRTY_TARGET_ID)
    {
        *(VU_ID*)ptr = target_id;
        ptr += sizeof(VU_ID);
    }

    if (dirty_unit & Dirty_Unit.DIRTY_MOVED)
    {
        *(uchar*)ptr = moved;
        ptr += sizeof(uchar);
    }

    if (dirty_unit & Dirty_Unit.DIRTY_TACTIC)
    {
        *(uchar*)ptr = tactic;
        ptr += sizeof(uchar);
    }

    if (dirty_unit & Dirty_Unit.DIRTY_REINFORCEMENT)
    {
        *(short*)ptr = reinforcement;
        ptr += sizeof(short);
    }

    if (dirty_unit & Dirty_Unit.DIRTY_WP_LIST)
    {
        EncodeWaypoints(&ptr);
    }

    dirty_unit = 0;

    *stream = ptr;

    // MonoPrint ("(%d)\n", *stream - start);
#endif
            throw new NotImplementedException();
        }

        public override void ReadDirty(byte[] stream, ref int pos)
        {
#if TODO
            bool refresh_required;
            ushort bits;

            refresh_required = false;

            memcpychk(&bits, stream, sizeof(ushort), rem);

            // MonoPrint ("Recv UC %08x", bits);

            if (bits & Dirty_Unit.DIRTY_WAYPOINT)
            {
                memcpychk(&current_wp, stream, sizeof(ushort), rem);
#if DEBUG

                if (!current_wp)
                    ShiAssert(!IsFlight());

#endif
            }

            if (bits & Dirty_Unit.DIRTY_ROSTER)
            {
                memcpychk(&roster, stream, sizeof(fourbyte), rem);
                refresh_required = true;
            }

            if (bits & Dirty_Unit.DIRTY_UNIT_FLAGS)
            {
                memcpychk(&unit_flags, stream, sizeof(fourbyte), rem);
            }

            if (bits & Dirty_Unit.DIRTY_DESTINATION)
            {

                memcpychk(&dest_x, stream, sizeof(GridIndex), rem);
                memcpychk(&dest_y, stream, sizeof(GridIndex), rem);
            }

            if (bits & Dirty_Unit.DIRTY_CARGO_ID)
            {
                memcpychk(&cargo_id, stream, sizeof(VU_ID), rem);
            }

            if (bits & Dirty_Unit.DIRTY_TARGET_ID)
            {
                memcpychk(&target_id, stream, sizeof(VU_ID), rem);
            }

            if (bits & Dirty_Unit.DIRTY_MOVED)
            {
                memcpychk(&moved, stream, sizeof(uchar), rem);
            }

            if (bits & Dirty_Unit.DIRTY_TACTIC)
            {
                memcpychk(&tactic, stream, sizeof(uchar), rem);
            }

            if (bits & Dirty_Unit.DIRTY_REINFORCEMENT)
            {
                memcpychk(&reinforcement, stream, sizeof(short), rem);
            }

            if (bits & Dirty_Unit.DIRTY_WP_LIST)
            {
                DecodeWaypoints(stream, rem);
                //sfr: refresh UI
                refresh_required = true;
            }

            if (refresh_required)
            {
                UI_Refresh();
            }
#endif
            throw new NotImplementedException();
        }

        // constructors and serial functions
        public UnitClass(ushort type, VU_ID_NUMBER id)
            : base(type, id)
        {
            last_check = 0;
#if HOTSPOT_FIX
    update_interval = 0;
#endif
            roster = 0;
            unit_flags = 0;
            dest_x = dest_y = 0;
            target_id = VU_ID.FalconNullId;
            cargo_id = VU_ID.FalconNullId;
            moved = 0;
            losses = 0;
            tactic = 0;
            current_wp = 0;
            name_id = GetCampId();
            reinforcement = 0;
            odds = 50;
            wp_list = null;
            draw_pointer = null;
            class_data = (UnitClassDataType)EntityDB.Falcon4ClassTable[type - VU_LAST_ENTITY_TYPE].dataPtr;
            dirty_unit = 0;

            SetUnitAltitude(0);
#if DEBUG_COUNT
    gUnitCount++;
    myunitlist.AddObj(Id().num_, this);
#endif

            // JB SOJ
            sojSource = null;
            sojOctant = 0;
            sojRangeSq = 0.0f;
        }
#if TODO		
        public UnitClass (byte[] stream, ref int offset):base(stream, ref offset)
		{throw new NotImplementedException();}
		public UnitClass(byte[] bytes, ref int offset, int version)
            : base(bytes, ref offset, version)
        {

            last_check = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            roster = BitConverter.ToInt32(bytes, offset);
            offset += 4;
            unit_flags = BitConverter.ToInt32(bytes, offset);
            offset += 4;
            dest_x = BitConverter.ToInt16(bytes, offset);
            offset += 2;
            dest_y = BitConverter.ToInt16(bytes, offset);
            offset += 2;
            target_id = new VU_ID();
            target_id.num_ = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            target_id.creator_ = BitConverter.ToUInt32(bytes, offset);
            offset += 4;

            if (version > 1)
            {
                cargo_id = new VU_ID();
                cargo_id.num_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                cargo_id.creator_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
            }
            else
            {
                cargo_id = new VU_ID();
            }
            moved = bytes[offset];
            offset++;
            losses = bytes[offset];
            offset++;
            tactic = bytes[offset];
            offset++;

            if (version >= 71)
            {
                current_wp = BitConverter.ToUInt16(bytes, offset);
                offset += 2;
            }
            else
            {
                current_wp = bytes[offset];
                offset++;
            }
            name_id = BitConverter.ToInt16(bytes, offset);
            offset += 2;
            reinforcement = BitConverter.ToInt16(bytes, offset);
            offset += 2;
            DecodeWaypoints(bytes, ref offset, version);
 
		}
		//TODO public virtual ~UnitClass();
		public override int SaveSize ()
		{throw new NotImplementedException();}

		public virtual int Save (VU_BYTE[] stream)
		{throw new NotImplementedException();}
#endif
        // event Handlers
        public override VU_ERRCODE Handle(VuFullUpdateEvent evnt)
        {
            // copy data from temp entity to current entity
            UnitClass tmp_ent = (UnitClass)(evnt.expandedData_);

            DisposeWayPoints();

            last_check = tmp_ent.last_check;
            roster = tmp_ent.roster;
            unit_flags = tmp_ent.unit_flags;
            dest_x = tmp_ent.dest_x;
            dest_y = tmp_ent.dest_y;
            target_id = tmp_ent.target_id;
            cargo_id = tmp_ent.cargo_id;
            moved = tmp_ent.moved;
            losses = tmp_ent.losses;
            tactic = tmp_ent.tactic;
            current_wp = tmp_ent.current_wp;
            name_id = tmp_ent.name_id;
            reinforcement = tmp_ent.reinforcement;

            wp_list = tmp_ent.wp_list;
            tmp_ent.wp_list = null;

            SetPosition(tmp_ent.XPos(), tmp_ent.YPos(), tmp_ent.ZPos());

            return base.Handle(evnt);
        }


        // Required pure virtuals handled by UnitClass
        public override void SendDeaggregateData(VuTargetEntity p)
        { throw new NotImplementedException(); }

        public override int RecordCurrentState(FalconSessionEntity p, int i)
        { throw new NotImplementedException(); }

        public override int Deaggregate(FalconSessionEntity session)
        { throw new NotImplementedException(); }

        public override int Reaggregate(FalconSessionEntity session)
        { throw new NotImplementedException(); }

        public override int TransferOwnership(FalconSessionEntity session)
        { throw new NotImplementedException(); }

        public override int Wake()
        { throw new NotImplementedException(); }

        public override int Sleep()
        { throw new NotImplementedException(); }

        public override void InsertInSimLists(float cameraX, float cameraY)
        { throw new NotImplementedException(); }

        public override void RemoveFromSimLists()
        { throw new NotImplementedException(); }

        public override void DeaggregateFromData(int size, byte[] data)
        { throw new NotImplementedException(); }

        public override void ReaggregateFromData(int size, byte[] data)
        { throw new NotImplementedException(); }

        public override void TransferOwnershipFromData(int size, byte[] data)
        { throw new NotImplementedException(); }

        public virtual int ResetPlayerStatus()
        { throw new NotImplementedException(); }

        public override int ApplyDamage(FalconCampWeaponsFire cwfm, byte p)
        { throw new NotImplementedException(); }

        public override int ApplyDamage(DamageDataType d, ref int str, int where, short flags)
        { throw new NotImplementedException(); }

        public virtual int DecodeDamageData(byte[] data, Unit shooter, FalconDeathMessage dtm)
        { throw new NotImplementedException(); }

        public override int CollectWeapons(byte[] dam, MoveType m, short[] w, byte[] wc, int dist)
        { throw new NotImplementedException(); }

        public override byte[] GetDamageModifiers()
        { throw new NotImplementedException(); }

        public override string GetName(string buffer, int size, int obj)
        { throw new NotImplementedException(); }

        public override string GetFullName(string buffer, int size, int obj)
        { throw new NotImplementedException(); }

        public override string GetDivisionName(string buffer, int size, int obj)
        { throw new NotImplementedException(); }

        public override int GetHitChance(int mt, int range)
        { throw new NotImplementedException(); }

        public override int GetAproxHitChance(int mt, int range)
        { throw new NotImplementedException(); }

        public override int GetCombatStrength(int mt, int range)
        { throw new NotImplementedException(); }

        public override int GetAproxCombatStrength(int mt, int range)
        { throw new NotImplementedException(); }

        public override int GetWeaponRange(int mt, FalconEntity target = null)
        { throw new NotImplementedException(); } // 2002-03-08 MODIFIED BY S.G. Need to pass it a target sometime so default to null for most cases

        public override int GetAproxWeaponRange(int mt)
        { throw new NotImplementedException(); }

        public override int GetDetectionRange(int mt)
        { throw new NotImplementedException(); }						// Takes into account emitter status
        public override int GetElectronicDetectionRange(MoveType mt)
        { throw new NotImplementedException(); }			// Max Electronic detection range, even if turned off
        public override int CanDetect(FalconEntity ent)
        { throw new NotImplementedException(); }						// Nonzero if this entity can see ent
        public virtual void GetComponentLocation(ref GridIndex x, ref GridIndex y, int component)
        { throw new NotImplementedException(); }

        public virtual int GetComponentAltitude(int component)
        { throw new NotImplementedException(); }

        public override float GetRCSFactor()
        { throw new NotImplementedException(); }

        public override float GetIRFactor()
        { throw new NotImplementedException(); }

        // These are only really relevant for sam/airdefense/radar entities
        public override int GetNumberOfArcs()
        { throw new NotImplementedException(); }

        public override float GetArcRatio(int anum)
        { throw new NotImplementedException(); }

        public override float GetArcRange(int anum)
        { throw new NotImplementedException(); }

        public override void GetArcAngle(int anum, ref float a1, ref float a2)
        { throw new NotImplementedException(); }

        public override Radar_types GetRadarType()
        { throw new NotImplementedException(); }

        // Addition Virtual functions required by all derived classes
        public virtual bool CanShootWeapon(int p)
        {
            return true;
        }

        public virtual int GetDeaggregationPoint(int i, CampBaseClass p)
        {
            return 0;
        }

        public virtual UnitDeaggregationData GetUnitDeaggregationData()
        {
            return null;
        }

        public virtual bool ShouldDeaggregate()
        {
            return true;
        }

        public virtual void ClearDeaggregationData()
        {
        }

        public virtual int Reaction(CampBaseClass c, int i, float p)
        {
            return 0;
        }

        public virtual int MoveUnit(CampaignTime t)
        {
            return 0;
        }

        public virtual int DoCombat()
        {
            return 0;
        }

        public virtual int ChooseTactic()
        {
            return 0;
        }

        public virtual int CheckTactic(int t)
        {
            return 1;
        }

        public virtual int Father()
        {
            return 0;
        }

        public virtual int Real()
        {
            return 0;
        }

        public virtual float AdjustForSupply()
        {
            return 1.0F;
        }

        public virtual int GetUnitSpeed()
        {
            return GetMaxSpeed();
        }

        public virtual int DetectOnMove()
        {
            return -1;
        }

        public virtual int ChooseTarget()
        {
            return -1;
        }

        public virtual CampaignTime UpdateTime()
        {
            return new CampaignTime(CampaignTime.CampaignDay);
        }

        public virtual CampaignTime CombatTime()
        {
            return new CampaignTime(CampaignTime.CampaignDay);
        }

        public virtual int GetUnitSupplyNeed(int p)
        {
            return 0;
        }

        public virtual int GetUnitFuelNeed(int p)
        {
            return 0;
        }

        public virtual void SupplyUnit(int p1, int p2)
        {
        }

        public virtual int GetVehicleDeagData(SimInitDataClass d, int p)
        {
            throw new NotSupportedException("Shouldn't be here");
        }

        // Core functions
        public void Setup(byte stype, byte sptype, Control who, Unit Parent)
        { throw new NotImplementedException(); }

        public void SendUnitMessage(VU_ID id, short msg, short d1, short d2, short d3)
        {
            VuTargetEntity target = (VuTargetEntity)VUSTATIC.vuDatabase.Find(OwnerId());
            FalconUnitMessage um = new FalconUnitMessage( Id(), target);
            um.dataBlock.from = id;
            um.dataBlock.message = msg;
            um.dataBlock.data1 = d1;
            um.dataBlock.data2 = d2;
            um.dataBlock.data3 = d3;
            FalcMesgStatic.FalconSendMessage(um, true);
        }


        public void BroadcastUnitMessage(VU_ID id, short msg, short d1, short d2, short d3)
        {
            FalconUnitMessage um = new FalconUnitMessage(Id(), FalconSessionEntity.FalconLocalGame);
            um.dataBlock.from = id;
            um.dataBlock.message = msg;
            um.dataBlock.data1 = d1;
            um.dataBlock.data2 = d2;
            um.dataBlock.data3 = d3;
            FalcMesgStatic.FalconSendMessage(um, true);
        }

        public int ChangeUnitLocation(CampaignHeading h)
        { throw new NotImplementedException(); }

        public int MoraleCheck(int shot, int lost)
        { throw new NotImplementedException(); }

        public override bool IsUnit()
        {
            return true;
        }

        // Unit flags
        public void SetDead(int p)
        { throw new NotImplementedException(); }

        public void SetAssigned(int p)
        { throw new NotImplementedException(); }

        public void SetOrdered(int p)
        { throw new NotImplementedException(); }

        public void SetDontPlan(int p)
        { throw new NotImplementedException(); }

        public void SetParent(int p)
        { throw new NotImplementedException(); }

        public void SetEngaged(int p)
        { throw new NotImplementedException(); }

        public void SetScripted(int p)
        { throw new NotImplementedException(); }

        public void SetCommando(int c)
        { throw new NotImplementedException(); }

        public void SetMoving(int p)
        { throw new NotImplementedException(); }

        public void SetRefused(int r)
        { throw new NotImplementedException(); }

        public void SetHasECM(int e)
        { throw new NotImplementedException(); }

        public void SetCargo(int c)
        { throw new NotImplementedException(); }

        public void SetCombat(int p)
        { throw new NotImplementedException(); }

        public void SetBroken(int p)
        { throw new NotImplementedException(); }

        public void SetAborted(int p)
        { throw new NotImplementedException(); }

        public void SetLosses(int p)
        { throw new NotImplementedException(); }

        public void SetInactive(int i)
        { throw new NotImplementedException(); }

        public void SetFragment(int f)
        { throw new NotImplementedException(); }

        public void SetTargeted(int p)
        { throw new NotImplementedException(); }

        public void SetRetreating(int p)
        { throw new NotImplementedException(); }

        public void SetDetached(int p)
        { throw new NotImplementedException(); }

        public void SetSupported(int s)
        { throw new NotImplementedException(); }

        public void SetTempDest(int t)
        { throw new NotImplementedException(); }

        public void SetFinal(int p)
        { throw new NotImplementedException(); }

        public void SetPilots(int f)
        { throw new NotImplementedException(); }

        public void SetDiverted(int d)
        { throw new NotImplementedException(); }

        public void SetFired(int f)
        { throw new NotImplementedException(); }

        public void SetLocked(int l)
        { throw new NotImplementedException(); }

        public void SetIAKill(int f)
        { throw new NotImplementedException(); }

        public void SetNoAbort(int f)
        { throw new NotImplementedException(); }

        public override bool IsDead()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_DEAD);
        }

        public bool Dead()
        {
            return IsDead();
        }

        public bool Assigned()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_ASSIGNED);
        }

        public bool Ordered()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_ORDERED);
        }

        public bool DontPlan()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_NO_PLANNING);
        }

        public bool Parent()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_PARENT);
        }

        public bool Engaged()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_ENGAGED);
        }

        public bool Scripted()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_SCRIPTED);
        }

        public bool Commando()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_COMMANDO);
        }

        public bool Moving()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_MOVING);
        }

        public bool Refused()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_REFUSED);
        }

        public bool Cargo()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_CARGO);
        }

        public bool Combat()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_COMBAT);
        }

        public bool Broken()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_BROKEN);
        }

        public bool Aborted()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_BROKEN);
        }

        public bool Losses()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_LOSSES);
        }

        public bool Inactive()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_INACTIVE);
        }

        public bool Fragment()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_FRAGMENTED);
        }

        public bool Targeted()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_TARGETED);
        }

        public bool Retreating()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_RETREATING);
        }

        public bool Detached()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_DETACHED);
        }

        public bool Supported()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_SUPPORTED);
        }

        public bool TempDest()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_TEMP_DEST);
        }

        public bool Final()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_FINAL);
        }

        public bool HasPilots()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_HAS_PILOTS);
        }

        public bool Diverted()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_DIVERTED);
        }

        public bool Fired()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_FIRED);
        }

        public bool Locked()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_LOCKED);
        }

        public bool IAKill()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_IA_KILL);
        }

        public bool NoAbort()
        {
            return unit_flags.IsFlagSet(TrasmittableFlags.U_NO_ABORT);
        }

        // Entity information
        public UnitClassDataType GetUnitClassData()
        { throw new NotImplementedException(); }

        public string GetUnitClassName()
        { throw new NotImplementedException(); }

        public void SetUnitAltitude(int alt)
        {
            SetPosition(XPos(), YPos(), -1.0F * (float)alt);
            MakeCampBaseDirty(Dirty_Campaign_Base.DIRTY_ALTITUDE, EntityDB.DDP[181].priority);
        }
        //void SetUnitAltitude (int alt)					{ SetPosition(XPos(),YPos(),-1.0F * (float)alt); MakeCampBaseDirty (DIRTY_ALTITUDE, SEND_SOON); }
        public int GetUnitAltitude()
        {
            return (int)(ZPos() * -1.0F);
        }

        public virtual void SimSetLocation(float x, float y, float z)
        {
            SetPosition(x, y, z);
            MakeCampBaseDirty(Dirty_Campaign_Base.DIRTY_POSITION, EntityDB.DDP[182].priority);
            MakeCampBaseDirty(Dirty_Campaign_Base.DIRTY_ALTITUDE, EntityDB.DDP[183].priority);
        }
        //virtual void SimSetLocation (float x, float y, float z)	{ SetPosition(x,y,z); MakeCampBaseDirty (DIRTY_POSITION, SEND_SOON); MakeCampBaseDirty (DIRTY_ALTITUDE, SEND_SOON); }
        public virtual void SimSetOrientation(float f1, float f2, float f3)
        {
        }

        public virtual void GetRealPosition(ref float f1, ref float f2, ref float f3)
        {
        }

        public virtual int GetBestVehicleWeapon(int i, byte[] buf, MoveType m, int a, int[] b)
        { throw new NotImplementedException(); }

        public virtual int GetVehicleHitChance(int slot, MoveType mt, int range, int hitflags)
        { throw new NotImplementedException(); }

        public virtual int GetVehicleCombatStrength(int slot, MoveType mt, int range)
        { throw new NotImplementedException(); }

        public virtual int GetVehicleRange(int slot, int mt, FalconEntity target = null)
        { throw new NotImplementedException(); } // 2002-03-08 MODIFIED BY S.G. Need to pass it a target sometime so default to null for most cases
        public virtual int GetUnitWeaponId(int hp, int slot)
        { throw new NotImplementedException(); }

        public virtual int GetUnitWeaponCount(int hp, int slot)
        { throw new NotImplementedException(); }

        // Unit_data information
        public void SetUnitDestination(GridIndex x, GridIndex y)
        {
#if TODO		
			dest_x = (GridIndex)(x + 1);
			dest_y = (GridIndex)(y + 1);
#endif
            throw new NotImplementedException();
        }
        //		void SetUnitRoster (fourbyte r)					{ roster = r; }
        public void SetNumVehicles(int v, int n)
        {
            SetRoster((roster & ~(3 << (v * 2))) | (n << (v * 2)));
        }

        public void SetTarget(FalconEntity e)
        {
            target_id = (e != null) ? e.Id() : VU_ID.FalconNullId;
        }

        public void SetUnitMoved(byte m)
        {
            moved = m;
        }

        public void SetUnitTactic(byte t)
        {
            tactic = t;
        }

        public void SetUnitReinforcementLevel(short r)
        {
            reinforcement = r;
        }

        public void GetUnitDestination(ref GridIndex x, ref GridIndex y)
        { throw new NotImplementedException(); }
        //		fourbyte GetUnitRoster () 					{ return roster; }
        public int GetNumVehicles(int v)
        {
            return (int)((roster >> (v * 2)) & 0x03);
        }

        public FalconEntity GetTarget()
        {
            return (FalconEntity)VUSTATIC.vuDatabase.Find(target_id);
        }

        public VU_ID GetTargetID()
        {
            return target_id;
        }

        public SimBaseClass GetSimTarget()
        { throw new NotImplementedException(); }

        public CampBaseClass GetCampTarget()
        { throw new NotImplementedException(); }

        public CampBaseClass GetCargo()
        { throw new NotImplementedException(); }

        public CampBaseClass GetTransport()
        { throw new NotImplementedException(); }

        public VU_ID GetCargoID()
        { throw new NotImplementedException(); }

        public VU_ID GetTransportID()
        { throw new NotImplementedException(); }

        public int GetUnitMoved()
        {
            return moved;
        }

        public int GetUnitTactic()
        {
            return tactic;
        }

        public int GetUnitReinforcementLevel()
        {
            return reinforcement;
        }

        public void AssignUnit(VU_ID mgr, VU_ID po, VU_ID so, VU_ID ao, int orders)
        { throw new NotImplementedException(); }

        public void SetUnitNameID(short id)
        {
            name_id = id;
        }

        public int SetUnitSType(char t)
        { throw new NotImplementedException(); }

        public int SetUnitSPType(char t)
        { throw new NotImplementedException(); }

        public int GetUnitNameID()
        {
            return name_id;
        }

        // Attribute data
        public VehicleID GetVehicleID(int v)
        { throw new NotImplementedException(); }

        public int GetTotalVehicles()
        { throw new NotImplementedException(); }

        public int GetFullstrengthVehicles()
        { throw new NotImplementedException(); }

        public int GetFullstrengthVehicles(int slot)
        { throw new NotImplementedException(); }

        public virtual int GetMaxSpeed()
        { throw new NotImplementedException(); }

        public virtual int GetCruiseSpeed()
        { throw new NotImplementedException(); }

        public virtual int GetCombatSpeed()
        { throw new NotImplementedException(); }

        public int GetUnitEndurance()
        { throw new NotImplementedException(); }

        public int GetUnitRange()
        { throw new NotImplementedException(); }

        public int GetRClass()
        { throw new NotImplementedException(); }

        // Support routines
        public CampaignTime GetUnitReassesTime()
        { throw new NotImplementedException(); }

        public int CountUnitElements()
        { throw new NotImplementedException(); }

        public Unit GetRandomElement()
        { throw new NotImplementedException(); }

        public void ResetMoves()
        { throw new NotImplementedException(); }

        public void ResetLocations(GridIndex x, GridIndex y)
        { throw new NotImplementedException(); }

        public void ResetDestinations(GridIndex x, GridIndex y)
        { throw new NotImplementedException(); }

        public void ResetFlags()
        { throw new NotImplementedException(); }

        public void SortElementsByDistance(GridIndex x, GridIndex y)
        { throw new NotImplementedException(); }

        public int FirstSP()
        { throw new NotImplementedException(); }

        public Unit FindPrevUnit(ref short type)
        { throw new NotImplementedException(); }

        public void SaveUnits(int FHandle, int flags)
        { throw new NotImplementedException(); }

        public void BuildElements()
        { throw new NotImplementedException(); }

        public int ChangeVehicles(int a)
        { throw new NotImplementedException(); }

        public int GetFormationCruiseSpeed()
        { throw new NotImplementedException(); }

        public void KillUnit()
        { throw new NotImplementedException(); }

        public int NoMission()
        { throw new NotImplementedException(); }

        public int AtDestination()
        { throw new NotImplementedException(); }

        public int GetUnitFormation()
        { throw new NotImplementedException(); }

        public int GetUnitRoleScore(int role, int calcType, int use_to_calc)
        { throw new NotImplementedException(); }

        public float GetUnitMovementCost(GridIndex x, GridIndex y, CampaignHeading h)
        { throw new NotImplementedException(); }

        public int GetUnitObjectivePath(Path p, Objective o, Objective t)
        { throw new NotImplementedException(); }

        public int GetUnitGridPath(Path p, GridIndex x, GridIndex y, GridIndex xx, GridIndex yy)
        { throw new NotImplementedException(); }

        public void LoadUnit(Unit cargo)
        { throw new NotImplementedException(); }

        public void UnloadUnit()
        { throw new NotImplementedException(); }

        public CampaignTime GetUnitSupplyTime()
        { throw new NotImplementedException(); }

        // Waypoint routines
        public WayPoint AddUnitWP(GridIndex x, GridIndex y, int alt, int speed,
                                  CampaignTime arr, CampaignTimeInterval station, WPAction mission)
        {
            WayPoint w, t;

            w = new WayPointClass(x, y, alt, speed, arr, station, mission, 0);

            if (wp_list ==null)
            {
                wp_list = w;
                current_wp = 1;
            }
            else
            {
                t = wp_list;

                while (t.GetNextWP() != null)
                    t = t.GetNextWP();

                t.InsertWP(w);
            }

            MakeWaypointsDirty();
            return w;
        }


        public WayPoint AddWPAfter(WayPoint pw, GridIndex x, GridIndex y, int alt, int speed,
                                   CampaignTime arr, CampaignTimeInterval station, WPAction mission)
        {
            WayPoint w;

            w = new WayPointClass(x, y, alt, speed, arr, station, mission, 0);

            if (pw != null && wp_list != null)
                pw.InsertWP(w);
            else
            {
                if (wp_list != null)
                    w.InsertWP(wp_list);

                wp_list = w;
            }

            if (wp_list == null)
                current_wp = 1;

            MakeWaypointsDirty();
            return w;
        }


        public void DeleteUnitWP(WayPoint w)
        {
            WayPoint t;

            CampaignStatic.CampEnterCriticalSection();
            t = wp_list;

            if ( t == null)
                return;

            if (t == w)
                wp_list = w.GetNextWP();

            w.DeleteWP();
            CampaignStatic.CampLeaveCriticalSection();
            MakeWaypointsDirty();
        }


        public int EncodeWaypoints(byte[] stream)
        {
#if TODO
            byte[] start;
            ushort count = 0;
            WayPointClass  w;

            start = stream;

            // Count waypoints
            w = wp_list;

            while (w != null)
            {
                count++;
                w = w.GetNextWP();
            }

            if (save_log)
            {
                fprintf(save_log, "%d ", count);
                fflush(save_log);
            }

            memcpy(*stream, &count, sizeof(ushort));
            *stream += sizeof(ushort);

            // Save waypoints
            w = wp_list;

            while (w)
            {
                w.Save(stream);
                w = w.GetNextWP();
            }

            return *stream - start;
#endif
            throw new NotImplementedException();
        }


        public void DecodeWaypoints(byte[] stream)
        {
#if TODO
            ushort count;
            WayPointClass new_list, lw, nw, w;


            if (gCampDataVersion >= 71)
            {
                memcpychk(&count, stream, sizeof(ushort), rem);
            }
            else
            {
                count = 0;
                memcpychk(&count, stream, sizeof(uchar), rem);
            }

            if (load_log)
            {
                fprintf(load_log, "%d ", count);
                fflush(load_log);
            }

            // KCK: Rather than replace our waypoint list,
            // I'm going to copy the new list into our old one.
            // Although this is more time consuming considering
            // the copy, it lowers the possibily of running into
            // deleted waypoints in the UI (I doubt we'll be able
            // to critical section all references to waypoints in
            // time).
            new_list = lw = null;

            while (count != 0)
            {
                w = new WayPointClass(stream, rem);

                if (lw == null)
                    new_list = lw = w;
                else
                    lw.InsertWP(w);

                lw = w;
                count--;
            }

            CampEnterCriticalSection();
            nw = new_list;
            w = wp_list;
            lw = null;

            while (w && nw)
            {
                w.CloneWP(nw);
                lw = w;
                w = w.GetNextWP();
                nw = nw.GetNextWP();
            }

            // Delete any extra old ones
            if (w == wp_list)
                wp_list = null;

            while (w != null)
            {
                lw = w;
                w = w.GetNextWP();
                lw.DeleteWP();
            }

            // Add any extra new ones
            if (nw != null)
            {
                if (nw.GetPrevWP() != null)
                    nw.GetPrevWP().UnlinkNextWP();
                else
                    new_list = null;

                if (lw != null)
                    lw.SetNextWP(nw);
                else
                    wp_list = nw;
            }

            if (new_list != null)
                DeleteWPList(new_list);

            CampLeaveCriticalSection();

            // Fix up Speeds
            w = wp_list;

            while (w != null)
            {
                SetWPSpeed(w);
                w = w.GetNextWP();
            }

            update_active_flight(this);
#endif
            throw new NotImplementedException();
        }

        public WayPoint GetFirstUnitWP()
        {
            return wp_list;
        }

        public WayPoint GetCurrentUnitWP()
        { throw new NotImplementedException(); }

        public WayPoint GetUnitMissionWP()
        { throw new NotImplementedException(); }

        public void FinishUnitWP()
        { throw new NotImplementedException(); }

        public void DisposeWayPoints()
        { throw new NotImplementedException(); }

        public void CheckBroken()
        { throw new NotImplementedException(); }

        public void SetCurrentUnitWP(WayPoint w)
        { throw new NotImplementedException(); }

        public void AdjustWayPoints()
        { throw new NotImplementedException(); }

        // Virtual Functions (These are empty except for those derived classes they belong to)
        // AirUnit virtuals
        // None

        // Flight virtuals
        public virtual void SetUnitLastMove(CampaignTime t)
        {
        }

        public virtual void SetCombatTime(CampaignTime t)
        {
        }

        public virtual void SetBurntFuel(long p)
        {
        }

        public virtual void SetUnitMission(byte p)
        {
        }

        public virtual void SetUnitRole(byte p)
        {
        }

        public virtual void SetUnitPriority(int p)
        {
        }

        public virtual void SetUnitMissionID(int p)
        {
        }

        public virtual void SetUnitMissionTarget(int p)
        {
        }

        public virtual void SetUnitTOT(CampaignTime p)
        {
        }

        public virtual void SetUnitSquadron(VU_ID p)
        {
        }

        public virtual void SetUnitAirbase(VU_ID p)
        {
        }

        public virtual void SetLoadout(LoadoutStruct p, int i)
        {
            throw new NotSupportedException("Shouldn't be here");
        }

        public virtual int GetNumberOfLoadouts()
        {
            return 0;
        }

        public virtual CampaignTime GetMoveTime()
        {
            return CampaignClass.TheCampaign.CurrentTime - last_check;
        }

        public virtual CampaignTime GetCombatTime()
        {
            return new CampaignTime(0);
        }

        public virtual VU_ID GetAirTargetID()
        {
            return VU_ID.FalconNullId;
        }

        public virtual FalconEntity GetAirTarget()
        {
            return null;
        }

        public virtual int GetBurntFuel()
        {
            return 0;
        }

        public virtual MissionTypeEnum GetUnitMission()
        {
            return (MissionTypeEnum)0;
        }

        public virtual int GetUnitNormalRole()
        {
            return 0;
        }

        public virtual int GetUnitCurrentRole()
        {
            return 0;
        }

        public virtual int GetUnitPriority()
        {
            return 0;
        }

        public virtual CampBaseClass GetUnitMissionTarget()
        {
            return null;
        }

        public virtual VU_ID GetUnitMissionTargetID()
        {
            return VU_ID.FalconNullId;
        }

        public virtual int GetUnitMissionID()
        {
            return 0;
        }

        public virtual CampaignTime GetUnitTOT()
        {
            return new CampaignTime(0);
        }

        public virtual Unit GetUnitSquadron()
        {
            return null;
        }

        public virtual VU_ID GetUnitSquadronID()
        {
            return VU_ID.FalconNullId;
        }

        public virtual CampBaseClass GetUnitAirbase()
        {
            return null;
        }

        public virtual VU_ID GetUnitAirbaseID()
        {
            return VU_ID.FalconNullId;
        }

        public virtual int LoadWeapons(object p, byte[] buf, MoveType m, int a, int b, int c)
        {
            return 0;
        }

        public virtual int DumpWeapons()
        {
            return 0;
        }

        public virtual CampaignTime ETA()
        {
            return new CampaignTime(0);
        }

        public virtual F4PFList GetKnownEmitters()
        {
            return null;
        }

        public virtual int BuildMission(MissionRequestClass m)
        {
            return 0;
        }

        public virtual void IncrementTime(CampaignTime t)
        {
        }

        public virtual void UseFuel(long f)
        {
        }

        // Squadron virtuals
        public virtual void SetUnitSpecialty(int p)
        {
        }

        public virtual void SetUnitSupply(int p)
        {
        }

        public virtual void SetUnitMorale(int p)
        {
        }

        public virtual void SetSquadronFuel(long p)
        {
        }

        public virtual void SetUnitStores(int i, byte p)
        {
        }

        public virtual void SetLastResupply(int i)
        {
        }

        public virtual void SetLastResupplyTime(CampaignTime t)
        {
        }

        public virtual int GetUnitSpecialty()
        {
            return 0;
        }

        public virtual int GetUnitSupply()
        {
            return 0;
        }

        public virtual int GetUnitMorale()
        {
            return 0;
        }

        public virtual long GetSquadronFuel()
        {
            return 0;
        }

        public virtual byte GetUnitStores(int s)
        {
            return 0;
        }

        public virtual CampaignTime GetLastResupplyTime()
        {
            return CampaignClass.TheCampaign.CurrentTime;
        }

        public virtual int GetLastResupply()
        {
            return 0;
        }

        // Package virtuals
        public virtual int BuildPackage(MissionRequest p, F4PFList d)
        {
            return 0;
        }

        public virtual void HandleRequestReceipt(int a, int b, VU_ID i)
        {
        }

        public virtual void SetUnitAssemblyPoint(int i, GridIndex x, GridIndex y)
        {
        }

        public virtual void GetUnitAssemblyPoint(int i, ref GridIndex x, ref GridIndex y)
        {
        }

        // Ground Unit virtuals
        public virtual void SetUnitPrimaryObj(VU_ID i)
        {
        }

        public virtual void SetUnitSecondaryObj(VU_ID i)
        {
        }

        public virtual void SetUnitObjective(VU_ID i)
        {
        }

        public virtual void SetUnitOrders(int i)
        {
        }

        public virtual void SetUnitOrders(int p, VU_ID i)
        {
        }

        public virtual void SetUnitFatigue(int i)
        {
        }
        //		virtual void SetUnitElement (int e)							{}
        public virtual void SetUnitMode(int i)
        {
        }

        public virtual void SetUnitPosition(int i)
        {
        }

        public virtual void SetUnitDivision(int i)
        {
        }

        public virtual void SetUnitHeading(int i)
        {
        }

        public virtual Objective GetUnitPrimaryObj()
        {
            return null;
        }

        public virtual Objective GetUnitSecondaryObj()
        {
            return null;
        }

        public virtual Objective GetUnitObjective()
        {
            return null;
        }

        public virtual VU_ID GetUnitPrimaryObjID()
        {
            return VU_ID.FalconNullId;
        }

        public virtual VU_ID GetUnitSecondaryObjID()
        {
            return VU_ID.FalconNullId;
        }

        public virtual VU_ID GetUnitObjectiveID()
        {
            return VU_ID.FalconNullId;
        }

        public virtual int GetUnitOrders()
        {
            return 0;
        }

        public virtual int GetUnitFatigue()
        {
            return 0;
        }

        public virtual int GetUnitElement()
        {
            return 0;
        }

        public virtual int GetUnitMode()
        {
            return 0;
        }

        public virtual int GetUnitPosition()
        {
            return 0;
        }

        public virtual int GetUnitDivision()
        {
            return 0;
        }

        public virtual int GetUnitHeading()
        {
            return CampGlobal.Here;
        }

        public virtual void SetUnitNextMove()
        {
        }

        public virtual void ClearUnitPath()
        {
        }

        public virtual int GetNextMoveDirection()
        {
            return CampGlobal.Here;
        }

        public virtual void SetUnitCurrentDestination(GridIndex x, GridIndex y)
        {
        }

        public virtual void GetUnitCurrentDestination(ref GridIndex x, ref GridIndex y)
        {
        }

        public virtual MoveType GetObjMovementType(Objective o, int i)
        {
            return base.GetMovementType();
        }

        public virtual int CheckForSurrender()
        {
            return 1;
        }

        public virtual int BuildMission()
        {
            return 0;
        }

        public virtual int RallyUnit(int i)
        {
            return 0;
        }

        // Battalion virtuals
        public virtual Unit GetUnitParent()
        {
            return null;
        }

        public virtual VU_ID GetUnitParentID()
        {
            return VU_ID.FalconNullId;
        }

        public virtual void SetUnitParent(Unit u)
        {
        }
#if USE_FLANKS
		virtual void GetLeftFlank (GridIndex *x, GridIndex *y)		{ GetLocation(x,y); }
		virtual void GetRightFlank (GridIndex *x, GridIndex *y)		{ GetLocation(x,y); }
#endif

        // Brigade virtuals
        public virtual Unit GetFirstUnitElement()
        {
            return null;
        }

        public virtual Unit GetNextUnitElement()
        {
            return null;
        }

        public virtual Unit GetUnitElement(int i)
        {
            return null;
        }

        public virtual Unit GetUnitElementByID(int i)
        {
            return null;
        }

        public virtual Unit GetPrevUnitElement(Unit u)
        {
            return null;
        }

        public virtual void AddUnitChild(Unit u)
        {
        }

        public virtual void DisposeChildren()
        {
        }

        public virtual void RemoveChild(VU_ID i)
        {
        }

        public virtual void ReorganizeUnit()
        {
        }

        public virtual int UpdateParentStatistics()
        {
            return 0;
        }

        public void CalculateSOJ(VuGridIterator iter)
        {
            Team who = GetTeam();
            CampBaseClass e;

            // JB SOJ
            sojSource = null;
            sojOctant = 0;
            // start my assuming the biggest distance possible.
            sojRangeSq = Phyconst.KM_TO_FT * GetElectronicDetectionRange(MoveType.Air);

            if (sojRangeSq == 0) return; // no point looking further

            sojRangeSq *= sojRangeSq;
            e = (CampBaseClass)iter.GetFirst();

            while (e != null)
            {
                // JPO - use IsAreaJamming - the virtual functions will sort it out
                if (TeamStatic.GetRoE(who, e.GetTeam(), ROEEngagementQueryTypeEnum.ROE_GROUND_FIRE) == ROEAllowedEnum.ROE_ALLOWED &&
                    e.IsAreaJamming())
                {
                    float rangesq = GridIndexMath.DistSqu(XPos(), YPos(), e.XPos(), e.YPos());

                    if (rangesq < sojRangeSq)
                    {
                        sojSource = e;
                        sojOctant = GridIndexMath.OctantTo(0.0F, 0.0F, e.XPos() - XPos(), e.YPos() - YPos());
                        sojRangeSq = rangesq;
                    }
                }

                e = (CampBaseClass)iter.GetNext();
            }

            // JB SOJ
        }
        // Naval Unit virtuals
        // None


        // JB SOJ
        protected CampBaseClass sojSource;
        protected int sojOctant;
        protected float sojRangeSq;
    }
#if TODO //Not used
	public class UnitDriver : VuMaster
	{
		public UnitDriver (VuEntity entity)
		{throw new NotImplementedException();}
		// TODO public virtual ~UnitDriver();

        public override void AutoExec(VU_TIME timestamp)
		{throw new NotImplementedException();}

        public override bool ExecModel(VU_TIME timestamp)
		{throw new NotImplementedException();}
	}
#endif

    // ============================================================
    // Deaggregated data class
    // ============================================================

    public struct UnitPosition
    {
        public float x, y, heading;
    }

    // This class is used to store unit positions while it's in an aggregate state
    public class UnitDeaggregationData
    {

#if USE_SH_POOLS
   public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { Debug.Assert( size == sizeof(UnitDeaggregationData) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(UnitDeaggregationData), 50, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif


        public short num_vehicles;
        public UnitPosition[] position_data = new UnitPosition[Camplib.VEHICLES_PER_UNIT * 3];

        public UnitDeaggregationData()
        { throw new NotImplementedException(); }
        // TODO public ~UnitDeaggregationData ();

        public void StoreDeaggregationData(Unit theUnit)
        { throw new NotImplementedException(); }
    }

    // ============================================================
    // Unit manipulation routines											
    // ============================================================
    public static class UnitStatic
    {
        public static void SaveUnits(string FileName)
        { throw new NotImplementedException(); }

        public static int LoadUnits(string FileName)
        { throw new NotImplementedException(); }

        public static Unit GetFirstUnit(F4LIt l)
        { throw new NotImplementedException(); }

        public static Unit GetNextUnit(F4LIt l)
        { throw new NotImplementedException(); }

        public static Unit LoadAUnit(int Num, int FHandle, Unit parent)
        { throw new NotImplementedException(); }

        public static DamageDataType GetDamageType(Unit u)
        { throw new NotImplementedException(); }

        public static Unit ConvertUnit(Unit u, int domain, int type, int stype, int sptype)
        { throw new NotImplementedException(); }

        public static int GetUnitRole(Unit u)
        { throw new NotImplementedException(); }

        public static string GetSizeName(int domain, int type, char[] buffer)
        { throw new NotImplementedException(); }

        public static string GetDivisionName(int div, char[] buffer, int size, int obj)
        { throw new NotImplementedException(); }

        public static int FindUnitNameID(Unit u)
        { throw new NotImplementedException(); }

        public static Unit NewUnit(int domain, int type, int stype, int sptype, Unit parent)
        { throw new NotImplementedException(); }

        public static Unit NewUnit(short tid, VU_BYTE[] stream)
        { throw new NotImplementedException(); }

        public static float GetOdds(Unit us, CampBaseClass them, int range)
        { throw new NotImplementedException(); }

        public static float GetRange(Unit us, CampBaseClass them)
        { throw new NotImplementedException(); }

        public static int EncodeUnitData(VU_BYTE[] stream, FalconSessionEntity owner)
        { throw new NotImplementedException(); }

        public static int DecodeUnitData(VU_BYTE[] stream, FalconSessionEntity owner)
        { throw new NotImplementedException(); }
    }
}

