using FalconNet.CampaignBase;
using FalconNet.Common;
using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Team = System.SByte;
using GridIndex = System.Int16;
using Control = System.Byte;
using VU_ID_NUMBER = System.UInt64;
using FalconNet.Common.Maths;
using FalconNet.Campaign;
using System.Diagnostics;
using System.IO;
using FalconNet.Common.Encoding;
using log4net;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;
using VU_KEY = System.UInt64;
using VU_BOOL = System.Boolean;
using BIG_SCALAR = System.Single;
using SM_SCALAR = System.Single;


namespace FalconNet.FalcLib
{
    public class CampBaseClass : FalconEntity
    {

        public CampaignTime spotTime;		// Last time this entity was spotted
        public short spotted;		// Bitwise array of spotting data, by team
        public Transmittable base_flags;		// Various user flags
        public short camp_id;		// Unique campaign id
        public Control owner;			// Controlling Country
        // Don't transmit below this line
        public CBC_ENUM local_flags;	// Non transmitted flags
        public TailInsertList components;	// List of deaggregated sim entities
        public VU_ID deag_owner;		// Owner of deaggregated components
        public VU_ID new_deag_owner;	// Who is most interrested in this guy
        public Dirty_Campaign_Base dirty_camp_base;


        // Access Functions
        public CampaignTime GetSpotTime()
        {
            return spotTime;
        }

        public short GetSpotted()
        {
            return spotted;
        }

        public Transmittable GetBaseFlags()
        {
            return base_flags;
        }

        public short GetCampId()
        {
            return camp_id;
        }

        public CBC_ENUM GetLocalFlags()
        {
            return local_flags;
        }

        public TailInsertList GetComponents()
        {
            return components;
        }

        public VU_ID GetDeagOwner()
        {
            return deag_owner;
        }

        public void SetBaseFlags(Transmittable flags)
        {
            if (base_flags != flags)
            {
                base_flags = flags;
                MakeCampBaseDirty(Dirty_Campaign_Base.DIRTY_BASE_FLAGS, EntityDB.DDP[8].priority);
                //		MakeCampBaseDirty (DIRTY_BASE_FLAGS, SEND_EVENTUALLY);
            }
        }
        public virtual void SetOwner(Control new_owner)
        {
            owner = new_owner;
            //MakeCampBaseDirty (DIRTY_OWNER, 1);
        }

        public void SetCampId(short new_camp_id)
        {
            camp_id = new_camp_id;
            //MakeCampBaseDirty (DIRTY_CAMP_ID, 1);
        }

        //public void SetLocalFlags ()
        //{throw new NotImplementedException();}

        public void SetComponents(TailInsertList new_list)
        {
            components = new_list;
            //MakeCampBaseDirty (DIRTY_COMPONENTS, 1);
        }

        public void SetDeagOwner(VU_ID new_id)
        {
            deag_owner = new_id;
            //MakeCampBaseDirty (DIRTY_DEAG_OWNER, 1);
        }

        // Dirty Functions
        public void MakeCampBaseDirty(Dirty_Campaign_Base bits, Dirtyness score)
        {
            if (!IsLocal())
                return;

            if (VuState() != VU_MEM_STATE.VU_MEM_ACTIVE)
                return;


            if (!IsAggregate())
            {
                score = (Dirtyness)((int)score * 10);
            }

            dirty_camp_base |= bits;

            MakeDirty(Dirty_Class.DIRTY_CAMPAIGN_BASE, score);
        }

        public virtual void WriteDirty(byte[] stream, ref int pos)
        {
            int ptr = pos;

            //	MonoPrint ("CB %08x\n", dirty_camp_base);

            // Encode it up

            ptr = (byte)dirty_camp_base;
            ptr += sizeof(byte);

            if (dirty_camp_base.HasFlag(Dirty_Campaign_Base.DIRTY_POSITION))
            {
                GridIndex x, y;

                GetLocation(out x, out y);
                Array.Copy(BitConverter.GetBytes(x), 0, stream, ptr, sizeof(GridIndex));
                ptr += sizeof(GridIndex);
                Array.Copy(BitConverter.GetBytes(y), 0, stream, ptr, sizeof(GridIndex));
                ptr += sizeof(GridIndex);
            }

            if (dirty_camp_base.HasFlag(Dirty_Campaign_Base.DIRTY_ALTITUDE))
            {
                Array.Copy(BitConverter.GetBytes(ZPos()), 0, stream, ptr, sizeof(float));
                ptr += sizeof(float);
            }

            if (dirty_camp_base.HasFlag(Dirty_Campaign_Base.DIRTY_SPOTTED))
            {
                Array.Copy(BitConverter.GetBytes(spotted), 0, stream, ptr, sizeof(GridIndex));
                ptr += sizeof(GridIndex);
                Array.Copy(BitConverter.GetBytes(spotTime), 0, stream, ptr, sizeof(ulong));
                ptr += sizeof(ulong);
            }

            if (dirty_camp_base.HasFlag(Dirty_Campaign_Base.DIRTY_BASE_FLAGS))
            {
                Array.Copy(BitConverter.GetBytes((short)base_flags), 0, stream, ptr, sizeof(short));
                ptr += sizeof(short);
            }

            dirty_camp_base = 0;
            pos = ptr;
        }

        public virtual void ReadDirty(byte[] stream, ref int pos)
        {
            int ptr = pos;
            Dirty_Campaign_Base bits = (Dirty_Campaign_Base)stream[pos];
            ptr += sizeof(byte);

            //MonoPrint ("  CB %08x", bits);

            if (bits.HasFlag(Dirty_Campaign_Base.DIRTY_POSITION))
            {
                GridIndex
                    x,
                    y;

                x = BitConverter.ToInt16(stream, pos);
                ptr += sizeof(short);

                y = BitConverter.ToInt16(stream, pos);
                ptr += sizeof(short);

                vector v;

                GridIndexMath.ConvertGridToSim(x, y, out v);
                SetPosition(v.x, v.y, ZPos());
                if (IsFlight())
                {
                    CampaignClass.TheCampaign.MissionEvaluator.RegisterMove((FlightClass)this);
                }
            }

            if (bits.HasFlag(Dirty_Campaign_Base.DIRTY_ALTITUDE))
            {
                float z;

                z = BitConverter.ToSingle(stream, pos);
                ptr += sizeof(float);
                SetPosition(XPos(), YPos(), z);
                if (IsFlight())
                {
                    CampaignClass.TheCampaign.MissionEvaluator.RegisterMove((FlightClass)this);
                }
            }

            if (bits.HasFlag(Dirty_Campaign_Base.DIRTY_SPOTTED))
            {
                spotted = BitConverter.ToInt16(stream, pos);
                ptr += sizeof(short);
                spotTime = new CampaignTime(BitConverter.ToUInt64(stream, pos));
                ptr += sizeof(ulong); //CampaignTime);
            }

            if (bits.HasFlag(Dirty_Campaign_Base.DIRTY_BASE_FLAGS))
            {
                base_flags = (Transmittable)BitConverter.ToInt16(stream, pos);
                ptr += sizeof(short);
            }

            pos = ptr;

            //MonoPrint ("(%d)", *stream - start);
        }
        private void InitLocalData()
        {
            SetTypeFlag(EntityEnum.FalconCampaignEntity);
            spotted = 0;
            base_flags = Transmittable.CBC_EMITTING;
            local_flags = CBC_ENUM.CBC_AGGREGATE;
            owner = 0;
            camp_id = CampbaseStatic.FindUniqueID();
            pos_.z_ = 0.0F;
            components = null;
            deag_owner = VU_ID.FalconNullId;
            SetAggregate(true);
            if (FalconSessionEntity.FalconLocalGame != null)
                SetAssociation(FalconSessionEntity.FalconLocalGame.Id());
            else
                log.Warn("FalconSessionEntity.FalconLocalGame is null, TODO");
            dirty_camp_base = 0;
            spotTime = new CampaignTime(0); // JB 010719
        }

        // Constructors and serial functions
        public CampBaseClass(ushort typeindex, VU_ID_NUMBER id)
            : base(typeindex, id)
        {
            InitLocalData();
            camp_id = CampbaseStatic.FindUniqueID();
        }
#if TODO
        public CampBaseClass(byte[] stream, ref int offset)
            : base(VuEntity.VU_LAST_ENTITY_TYPE)
        { throw new NotImplementedException(); }
        public CampBaseClass(byte[] bytes, ref int offset, int version)
            : base(VU_LAST_ENTITY_TYPE)
        {
            GridIndex x, y;
            short tmp;

            // Read vu stuff here
            share_.id_.num_ = BitConverter.ToUInt32(bytes, offset);
            offset += sizeof(uint);
            share_.id_.creator_ = BitConverter.ToUInt32(bytes, offset);
            offset += sizeof(uint);
            //memcpy(&share_.id_, *stream, sizeof(VU_ID)); *stream += sizeof(VU_ID);
#if DEBUG
            // VU_ID_NUMBERs moved to 32 bits
            //share_.id_.num_ &= 0xffff;
#endif
            share_.entityType_ = BitConverter.ToUInt16(bytes, offset);
            offset += sizeof(ushort);
            x = BitConverter.ToInt16(bytes, offset);
            offset += sizeof(short);
            y = BitConverter.ToInt16(bytes, offset);
            offset += sizeof(short);
            SetLocation(x, y);

            if (CampaignClass.gCampDataVersion < 70)
            {
                pos_.z_ = 0.0F;
            }
            else
            {
                pos_.z_ = BitConverter.ToSingle(bytes, offset);
                offset += sizeof(float);
            }

            SetEntityType(share_.entityType_);
            SetTypeFlag(EntityEnum.FalconCampaignEntity);

            spotTime = new CampaignTime(BitConverter.ToUInt64(bytes, offset));
            offset += sizeof(ulong); //CampaignTime
            spotted = BitConverter.ToInt16(bytes, offset);
            offset += sizeof(short);
            tmp = BitConverter.ToInt16(bytes, offset);
            offset += sizeof(short);
            base_flags = (Transmittable)tmp;
            owner = bytes[offset];
            offset += sizeof(Control);
            camp_id = BitConverter.ToInt16(bytes, offset);
            offset += sizeof(short);
            local_flags = CBC_ENUM.CBC_AGGREGATE;
            deag_owner = VU_ID.FalconNullId;
            components = null;
            dirty_camp_base = 0;

#if CAMPTOOL
   if (GetEntityByCampID(camp_id))
      {
      MonoPrint("Got duplicate camp ID #%d.\n",camp_id);
      for (int i=0; i<MAX_CAMP_ENTITIES; i++)
         {
         if (!CampIDRenameTable[i])
            {
            CampIDRenameTable[i] = camp_id;
            break;
            }
         }
      }
   if (VuDatabase.vuDatabase.Find(Id()))
      {
      MonoPrint("Got duplicate VU_ID #%d.\n",Id().num_);
      }
#endif

#if DEBUG
            // Clear out entities owned by non-existant teams
            // KCK NOTE: This doesn't work in multi-player remote, as we often don't have teams at this point
            if (FalconSessionEntity.FalconLocalGame != null && FalconSessionEntity.FalconLocalGame.IsLocal())
            {
                if (TeamStatic.TeamInfo[GetTeam()] == null || TeamStatic.TeamInfo[GetOwner()] == null)
                {
                    int i;
                    for (i = 0; i < (int)TeamDataEnum.NUM_TEAMS && TeamStatic.TeamInfo[i] == null; i++) ;
                    SetOwner((byte)i);
                }
            }
#endif

            // Set owner to game master and associate the entity
            Debug.Assert(FalconSessionEntity.FalconLocalGame != null);
            if (FalconSessionEntity.FalconLocalGame != null)
            {
                share_.ownerId_ = FalconSessionEntity.FalconLocalGame.OwnerId();
                SetAssociation(FalconSessionEntity.FalconLocalGame.Id());
            }
        }

        // public virtual ~CampBaseClass ();
        public override int SaveSize()
        {
            return sizeof(uint) + sizeof(uint) //VU_ID
                + sizeof(ushort)
                + sizeof(GridIndex)
                + sizeof(GridIndex)
                + sizeof(float)
                + sizeof(ulong) //CampaignTimesizeof(CampaignTime)
                + sizeof(short)
                + sizeof(short)
                + sizeof(Control)
                + sizeof(short);
        }

        public virtual int Save(ref VU_BYTE[] stream)
        { throw new NotImplementedException(); }
#endif
        // event handlers
        public override VU_ERRCODE Handle(VuEvent evnt)
        {
            return base.Handle(evnt);
        }


        public override VU_ERRCODE Handle(VuFullUpdateEvent evnt)
        {
#if TODO
	GridIndex		x,y;

	// copy data from temp entity to current entity
	CampBaseClass  tmp_ent = (CampBaseClass)(evnt.expandedData_);

	// Make sure the host owns this
	share_.ownerId_ = FalconSessionEntity.FalconLocalGame.OwnerId();

	// In the case of force on force TE, this is actually ok -
	// The host will receive the full update and MAKE this entity
	// local in the line above
//	Debug.Assert ( !IsLocal() );

	memcpy(&share_.entityType_, &tmp_ent.share_.entityType_, sizeof(ushort));
	tmp_ent.GetLocation(&x,&y);
	SetLocation(x,y);
	SetEntityType(share_.entityType_);

	memcpy(&spotTime, &tmp_ent.spotTime, sizeof(CampaignTime));
	memcpy(&spotted, &tmp_ent.spotted, sizeof(short));			
	memcpy(&owner, &tmp_ent.owner, sizeof(Control));
	base_flags = tmp_ent.base_flags;

	return (FalconEntity::Handle(event));
#endif
            throw new NotImplementedException();
        }

        public override VU_ERRCODE Handle(VuPositionUpdateEvent evnt)
        {
            return base.Handle(evnt);
        }

        public override VU_ERRCODE Handle(VuEntityCollisionEvent evnt)
        {
            return base.Handle(evnt);
        }

        public override VU_ERRCODE Handle(VuTransferEvent evnt)
        {
            return base.Handle(evnt);
        }

        public override VU_ERRCODE Handle(VuSessionEvent evnt)
        {
            return base.Handle(evnt);
        }

        // Required pure virtuals
        public virtual void SendDeaggregateData(VuTargetEntity t)
        {
        }

        public virtual int RecordCurrentState(FalconSessionEntity s, int i)
        {
            return 0;
        }

        public virtual int Deaggregate(FalconSessionEntity s)
        {
            return 0;
        }

        public virtual int Reaggregate(FalconSessionEntity s)
        {
            return 0;
        }

        public virtual int TransferOwnership(FalconSessionEntity s)
        {
            return 0;
        }

        public override int Wake()
        {
            return 0;
        }

        public override int Sleep()
        {
            return 0;
        }

        public virtual void InsertInSimLists(float f1, float f2)
        {
        }

        public virtual void RemoveFromSimLists()
        {
        }

        public virtual void DeaggregateFromData(int i, byte[] b)
        {
            return;
        }

        public virtual void ReaggregateFromData(int i, byte[] b)
        {
            return;
        }

        public virtual void TransferOwnershipFromData(int i, byte[] b)
        {
            return;
        }

        public virtual int ApplyDamage(FalconCampWeaponsFire f, byte b)
        {
            return 0;
        }

        public virtual int ApplyDamage(DamageDataType d, ref int i, int i2, short s)
        {
            return 0;
        }

        public virtual int DecodeDamageData(byte[] b, Unit u, FalconDeathMessage m)
        {
            return 0;
        }

        public virtual int CollectWeapons(byte[] buf, MoveType m, short[] s, byte[] b, int i)
        {
            return 0;
        }

        public virtual string GetName(string s, int i, int i2)
        {
            return "None";
        }

        public virtual string GetFullName(string s, int i, int i2)
        {
            return "None";
        }

        public virtual string GetDivisionName(string s, int i, int i2)
        {
            return "None";
        }

        public virtual int GetHitChance(int i1, int i2)
        {
            return 0;
        }

        public virtual int GetAproxHitChance(int i1, int i2)
        {
            return 0;
        }

        public virtual int GetCombatStrength(int i1, int i2)
        {
            return 0;
        }

        public virtual int GetAproxCombatStrength(int i1, int i2)
        {
            return 0;
        }

        public virtual int GetWeaponRange(int p, FalconEntity target = null)
        {
            return 0;
        } // 2008-03-08 ADDED SECOND DEFAULT PARM

        public virtual int GetAproxWeaponRange(int r)
        {
            return 0;
        }

        public virtual int GetDetectionRange(int r)
        {
            return 0;
        }			// Takes into account emitter status

        public virtual int GetElectronicDetectionRange(MoveType r)
        {
            return 0;
        }			// Full range, regardless of emitter

        public virtual int CanDetect(FalconEntity e)
        {
            return 0;
        }			// Nonzero if this entity can see ent

        public override bool OnGround()
        {
            return false;
        }


        public virtual float Vt()
        {
            return 0.0F;
        }

        public virtual float Kias()
        {
            return 0.0F;
        }

        public override short GetCampID()
        {
            return camp_id;
        }

        public override Team GetTeam()
        {
            return TeamStatic.GetTeam(owner);
        }

        public override Control GetCountry()
        {
            return owner;
        }		// New FalcEnt friendly form

        public virtual FEC_RADAR StepRadar(int t, int d, float range)
        {
            return FEC_RADAR.FEC_RADAR_OFF;
        }

        public Control GetOwner()
        {
            return owner;
        }			// Old form

        // These are only really relevant for sam/airdefense/radar entities
        public virtual int GetNumberOfArcs()
        {
            return 1;
        }

        public virtual float GetArcRatio(int i)
        {
            return 0.0F;
        }

        public virtual float GetArcRange(int i)
        {
            return 0.0F;
        }

        public virtual void GetArcAngle(int i, ref float a1, ref float a2)
        {
            a1 = 0.0F;
            a2 = (float)(2 * Math.PI);
        }

        // Core functions
        public void SendMessage(VU_ID id, short msg, short d1, short d2, short d3, short d4)
        {
#if TODO
            VuTargetEntity target = (VuTargetEntity)VuDatabase.vuDatabase,Find(OwnerId());
            FalconCampMessage cm = new FalconCampMessage(Id(), target);

            cm.dataBlock.from = id;
            cm.dataBlock.message = msg;
            cm.dataBlock.data1 = d1;
            cm.dataBlock.data2 = d2;
            cm.dataBlock.data3 = d3;
            cm.dataBlock.data4 = d4;
            FalcMesgStatic.FalconSendMessage(cm, true);
#endif
            throw new NotImplementedException();
        }


        public void BroadcastMessage(VU_ID id, short msg, short d1, short d2, short d3, short d4)
        { throw new NotImplementedException(); }

        public VU_ERRCODE Remove()
        {
            if (IsTacan())
            {
                SetTacan(0);
            }
            return VUSTATIC.vuDatabase.Remove(this);
        }

        public int ReSpot()
        { throw new NotImplementedException(); }

        public FalconSessionEntity GetDeaggregateOwner()
        { throw new NotImplementedException(); }

        // Component accessers (Sim Flight emulators)
        public int GetComponentIndex(VuEntity me)
        { throw new NotImplementedException(); }

        public FalconEntity /* TODO in the original version was SimBaseClass */ GetComponentEntity(int idx)
        { throw new NotImplementedException(); }

        public FalconEntity /* TODO in the original version was SimBaseClass */  GetComponentLead()
        { throw new NotImplementedException(); }

        public FalconEntity /* TODO in the original version was SimBaseClass */  GetComponentNumber(int component)
        { throw new NotImplementedException(); }

        public int NumberOfComponents()
        { throw new NotImplementedException(); }

        public override byte Domain()
        {
            return GetDomain();
        }

        // Queries
        public override bool IsEmitting()
        {
            return base_flags.HasFlag(Transmittable.CBC_EMITTING);
        }

        public bool IsJammed()
        {
            return base_flags.HasFlag(Transmittable.CBC_JAMMED);
        }
        // Local flag access
        public bool IsChecked()
        {
            return local_flags.HasFlag(CBC_ENUM.CBC_CHECKED);
        }

        public bool IsAwake()
        {
            return local_flags.HasFlag(CBC_ENUM.CBC_AWAKE);
        }

        public bool InPackage()
        {
            return local_flags.HasFlag(CBC_ENUM.CBC_IN_PACKAGE);
        }

        public bool InSimLists()
        {
            return local_flags.HasFlag(CBC_ENUM.CBC_IN_SIM_LIST);
        }

        public bool IsInterested()
        {
            return local_flags.HasFlag(CBC_ENUM.CBC_INTEREST);
        }

        public bool IsReserved()
        {
            return local_flags.HasFlag(CBC_ENUM.CBC_RESERVED_ONLY);
        }

        public bool IsAggregate()
        {
            return local_flags.HasFlag(CBC_ENUM.CBC_AGGREGATE);
        }

        public bool IsTacan()
        {
            return local_flags.HasFlag(CBC_ENUM.CBC_HAS_TACAN);
        }

        public bool HasDelta()
        {
            return local_flags.HasFlag(CBC_ENUM.CBC_HAS_DELTA);
        }

        // Getters
        public override byte GetDomain()
        {
            return (EntityType()).classInfo_[(int)Vu_CLASS.VU_DOMAIN];
        }

        public byte GetClass()
        {
            return (EntityType()).classInfo_[(int)Vu_CLASS.VU_CLASS];
        }

        public ClassTypes GetFalconType()
        {
            return (ClassTypes)(EntityType()).classInfo_[(int)Vu_CLASS.VU_TYPE];
        }

        public SubTypes GetSType()
        {
            return (SubTypes)(EntityType()).classInfo_[(int)Vu_CLASS.VU_STYPE];
        }

        public byte GetSPType()
        {
            return (EntityType()).classInfo_[(int)Vu_CLASS.VU_SPTYPE];
        }

        public CampaignTime GetSpottedTime()
        {
            return spotTime;
        }

        public int GetSpotted(Team t)
        {
            MoveType mt = GetMovementType();

            if (Camplib.Camp_GetCurrentTime() - spotTime > CampGlobal.ReconLossTime[(int)mt])
            {
                spotted = 0;
            }
            Debug.Assert(TeamStatic.TeamInfo[t] != null); // 2002-03-06 MN try to cath this
            if (((spotted >> t) & 0x01) != 0)
                return 1;
            switch (mt)
            {
                case MoveType.Air:
                case MoveType.LowAir:
                case MoveType.Foot:
                    return 0;
                case MoveType.Tracked:
                case MoveType.Wheeled:
                case MoveType.Rail:
                    return 0;
                case MoveType.Naval:
                    if (TeamStatic.TeamInfo[t] != null && TeamStatic.TeamInfo[t].HasSatelites()) // 2002-03-06 MN CTD fix
                    {
                        // Check for cloud cover
                        GridIndex x, y;
                        GetLocation(out x, out y);
#if TODO
                        if (((WeatherClass)TheWeather).GetCloudCover(x, y) < (WeatherMap.MAX_CLOUD_TYPE - (WeatherMap.MAX_CLOUD_TYPE / 4)))
                        {
                            SetSpotted(t, Camplib.Camp_GetCurrentTime());
                            return 1;
                        }
#endif
                        throw new NotImplementedException();
                    }
                    break;
                default:
                    // KCK: Experimental - Autospotted during first 12 hours of combat
                    if (Camplib.Camp_GetCurrentTime() < CampaignTime.CampaignDay / 2)
                        return 1;
                    else if (TeamStatic.TeamInfo[t] != null && TeamStatic.TeamInfo[t].HasSatelites()) // 2002-03-06 MN CTD fix
                    {
                        // Check for cloud cover
                        GridIndex x, y;
                        GetLocation(out x, out y);
#if TODO
                        if (((WeatherClass*)TheWeather).GetCloudCover(x, y) < (WeatherMap.MAX_CLOUD_TYPE - (WeatherMap.MAX_CLOUD_TYPE / 4)))
                        {
                            SetSpotted(t, Camplib.Camp_GetCurrentTime());
                            return 1;
                        }
#endif
                        throw new NotImplementedException();
                    }
                    break;
            }
            return 0;
        }

        public int GetIdentified(Team t)
        {
            return (spotted >> (t + 8)) & 0x01;
        } // 2002-02-11 ADDED BY S.G. Getter to know if the target is identified or not.

        // Setters
        public void SetLocation(GridIndex x, GridIndex y)
        {
            GridIndex cx, cy;

            // Check if flight has moved, and evaluate current situation if so
            GetLocation(out cx, out cy);
            if (cx != x || cy != y)
            {
                vector v;

                //		Debug.Assert (x >= 0 && y >= 0 && x < Map_Max_X && y < Map_Max_Y)
                if ((x < 1) || (x >= CampTerrStatic.Map_Max_X - 1) || (y < 1) || (y >= CampTerrStatic.Map_Max_Y - 1))
                {
                    // KCK Hack: Teleport units off map back onto map
                    if (x < 1)
                        x = 1;
                    if (y < 1)
                        y = 1;
                    if (x >= CampTerrStatic.Map_Max_X - 1)
                        x = (GridIndex)(CampTerrStatic.Map_Max_X - 2);
                    if (y >= CampTerrStatic.Map_Max_Y - 1)
                        y = (GridIndex)(CampTerrStatic.Map_Max_Y - 2);
                }

                GridIndexMath.ConvertGridToSim(x, y, out v);
                SetPosition(v.x, v.y, ZPos());

                //		MakeCampBaseDirty (DIRTY_POSITION, SEND_SOON);
                MakeCampBaseDirty(Dirty_Campaign_Base.DIRTY_POSITION, EntityDB.DDP[0].priority);
            }
        }

        public void SetAltitude(int alt)
        {
            SetPosition(XPos(), YPos(), (float)(alt * -1));

            MakeCampBaseDirty(Dirty_Campaign_Base.DIRTY_ALTITUDE, EntityDB.DDP[1].priority);
            //	MakeCampBaseDirty (DIRTY_ALTITUDE, SEND_SOON);
        }

        public void SetSpottedTime(CampaignTime t)
        {
            spotTime = t;
        }

        public void SetSpotted(Team t, CampaignTime time, int identified = 0) // 2002-02-11 ADDED S.G. Added identified which defaults to 0 (not identified or don't change)
        {
            // Make this dirty if we wern't previously spotted or our time has expired
            if (ReSpot() != 0 || ((spotted >> t) & 0x01) == 0 || (((spotted >> (t + 8)) & 0x01) == 0 && identified != 0)) // 2002-02-11 MODIFIED BY S.G. Or we were not identified and now we are
            {
                spotTime = time;
                // 2002-04-02 ADDED BY S.G. Need to send sooner if it gets identified.
                if (((spotted >> (t + 8)) & 0x01) == 0 && identified != 0)
                    MakeCampBaseDirty(Dirty_Campaign_Base.DIRTY_SPOTTED, EntityDB.DDP[2].priority);
                //			MakeCampBaseDirty (DIRTY_SPOTTED, SEND_RELIABLE);
                else
                    // END OF ADDED SECTION
                    MakeCampBaseDirty(Dirty_Campaign_Base.DIRTY_SPOTTED, EntityDB.DDP[3].priority);
                //			MakeCampBaseDirty (DIRTY_SPOTTED, SEND_SOMETIME);
            }

            spotted |= (short)(0x01 << t);

            // 2002-02-11 ADDED BY S.G. The upper 8 bits of spotted is now used to know if the target has been identified by that team.
            spotted |= (short)((identified & 0x01) << (t + 8)); // The only time you lose your identification is when you lose your spotting
        }

        public void SetEmitting(int e)
        {
#if TODO
            //	if (IsObjective())
            //	{
            //		if(((Objective)this).GetObjectiveStatus() < _FORCE_RADAR_OFF_)
            //		{
            //			e=0;
            //		}
            //	}
            if (e != 0)
            {
                if (!IsEmitting())
                {
                    base_flags |= Transmittable.CBC_EMITTING;
                    MakeCampBaseDirty(Dirty_Campaign_Base.DIRTY_BASE_FLAGS, EntityDB.DDP[4].priority);
                    //			MakeCampBaseDirty (DIRTY_BASE_FLAGS, SEND_SOMETIME);
                }

                if (!EmitterList.Find(Id()))
                {
                    if (IsBattalion() || IsObjective())
                        EmitterList.ForcedInsert(this);
                    if (GetRadarMode() == FEC_RADAR.FEC_RADAR_OFF)
                        SetRadarMode(FEC_RADAR.FEC_RADAR_SEARCH_1);//me123 + rand()%3);
                    //			ReturnToSearch();
                }
            }
            else if (IsEmitting())
            {
                base_flags &= ~Transmittable.CBC_EMITTING;
                if (IsBattalion() || IsObjective())
                    EmitterList.Remove(this);
                SetRadarMode(FEC_RADAR.FEC_RADAR_OFF);
                MakeCampBaseDirty(Dirty_Campaign_Base.DIRTY_BASE_FLAGS, EntityDB.DDP[5].priority);
                //		MakeCampBaseDirty (DIRTY_BASE_FLAGS, SEND_SOMETIME);
            }
#endif
            throw new NotImplementedException();
        }

        public void SetAggregate(bool agg)
        {
            if (agg)
            {
                local_flags |= CBC_ENUM.CBC_AGGREGATE;
            }
            else
            {
                local_flags &= (~CBC_ENUM.CBC_AGGREGATE);
            }
        }

        public virtual void SetJammed(int j)
        {
            if (j != 0)
            {
                if ((base_flags & Transmittable.CBC_JAMMED) == 0)
                {
                    base_flags |= Transmittable.CBC_JAMMED;
                    MakeCampBaseDirty(Dirty_Campaign_Base.DIRTY_BASE_FLAGS, EntityDB.DDP[6].priority);
                    //			MakeCampBaseDirty (DIRTY_BASE_FLAGS, SEND_SOMETIME);
                }
            }
            else
            {
                if ((base_flags & Transmittable.CBC_JAMMED) != 0)
                {
                    base_flags &= ~Transmittable.CBC_JAMMED;
                    MakeCampBaseDirty(Dirty_Campaign_Base.DIRTY_BASE_FLAGS, EntityDB.DDP[7].priority);
                    //			MakeCampBaseDirty (DIRTY_BASE_FLAGS, SEND_SOMETIME);
                }
            }
        }

        public void SetTacan(int t)
        {
            if (t == 0 && IsTacan() && Tacan.gTacanList != null)
            {
                if (IsObjective() && GetFalconType() == ClassTypes.TYPE_AIRBASE)
                {
                    Tacan.gTacanList.RemoveTacan(Id(), NavigationType.AIRBASE);
                }
                else if (EntityType().classInfo_[(int)Vu_CLASS.VU_CLASS] == (byte)Classes.CLASS_UNIT &&
                         EntityType().classInfo_[(int)Vu_CLASS.VU_TYPE] == (byte)ClassTypes.TYPE_FLIGHT)
                {
                    Tacan.gTacanList.RemoveTacan(Id(), NavigationType.TANKER);
                }
                else if (EntityType().classInfo_[(int)Vu_CLASS.VU_CLASS] == (byte)Classes.CLASS_UNIT &&
                         EntityType().classInfo_[(int)Vu_CLASS.VU_TYPE] == (byte)ClassTypes.TYPE_TASKFORCE &&
                         EntityType().classInfo_[(int)Vu_CLASS.VU_STYPE] != 0)
                {
                    Tacan.gTacanList.RemoveTacan(Id(), NavigationType.CARRIER);
                }
            }
            else if (t != 0 && (!IsTacan() || (IsObjective() && GetFalconType() ==  ClassTypes.TYPE_AIRBASE)) &&
                   Tacan.gTacanList != null)
            {
                Tacan.gTacanList.AddTacan(this);
            }

            local_flags |= CBC_ENUM.CBC_HAS_TACAN;
            if (t == 0)
                local_flags ^= CBC_ENUM.CBC_HAS_TACAN;
        }

        public void SetChecked()
        {
            local_flags |= CBC_ENUM.CBC_CHECKED;
        }

        public void UnsetChecked()
        {
            local_flags &= ~CBC_ENUM.CBC_CHECKED;
        }

        public void SetInterest()
        {
            local_flags |= CBC_ENUM.CBC_INTEREST;
        }

        public void UnsetInterest()
        {
            local_flags &= ~CBC_ENUM.CBC_INTEREST;
        }

        public void SetAwake(bool d)
        {
            local_flags |= CBC_ENUM.CBC_AWAKE;
            if (!d)
                local_flags ^= CBC_ENUM.CBC_AWAKE;
        }

        public void SetInPackage(int p)
        {
            local_flags |= CBC_ENUM.CBC_IN_PACKAGE;
            if (p == 0)
                local_flags ^= CBC_ENUM.CBC_IN_PACKAGE;
        }

        public void SetDelta(int d)
        {
            local_flags |= CBC_ENUM.CBC_HAS_DELTA;
            if (d == 0)
                local_flags ^= CBC_ENUM.CBC_HAS_DELTA;
        }

        public void SetInSimLists(int l)
        {
            local_flags |= CBC_ENUM.CBC_IN_SIM_LIST;
            if (l == 0)
                local_flags ^= CBC_ENUM.CBC_IN_SIM_LIST;
        }

        public void SetReserved(int r)
        {
            local_flags |= CBC_ENUM.CBC_RESERVED_ONLY;
            if (r == 0)
                local_flags ^= CBC_ENUM.CBC_RESERVED_ONLY;
        }

        public override void SetEntityType(ushort entityType)
        {
            if (entityType >= VU_LAST_ENTITY_TYPE)
                entityTypePtr_ = F4VUStatic.VuxType(entityType);
            base.SetEntityType(entityType);
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }

    public static class CampBaseClassEncodingLE
    {
        public static void Encode(Stream stream, CampBaseClass val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, CampBaseClass rst)
        {
            GridIndex x, y;

            // Read vu stuff here
            rst.share_.id_ = new VU_ID();
            VU_IDEncodingLE.Decode(stream, rst.share_.id_);

            //memcpy(&share_.id_, *stream, sizeof(VU_ID)); *stream += sizeof(VU_ID);
#if DEBUG
            // VU_ID_NUMBERs moved to 32 bits
            //share_.id_.num_ &= 0xffff;
#endif
            rst.share_.entityType_ = UInt16EncodingLE.Decode(stream);
            x = Int16EncodingLE.Decode(stream);
            y = Int16EncodingLE.Decode(stream);
            rst.SetLocation(x, y);

            if (CampaignClass.gCampDataVersion < 70)
            {
               rst.pos_.z_ = 0.0F;
            }
            else
            {
                rst.pos_.z_ = SingleEncodingLE.Decode(stream);
            }

            rst.SetEntityType(rst.share_.entityType_);
            rst.SetTypeFlag(EntityEnum.FalconCampaignEntity);

            rst.spotTime = new CampaignTime(UInt32EncodingLE.Decode(stream));
            rst.spotted = Int16EncodingLE.Decode(stream);
            rst.base_flags = (Transmittable)Int16EncodingLE.Decode(stream);
            rst.owner = (Control)stream.ReadByte();
            rst.camp_id = Int16EncodingLE.Decode(stream);
            rst.local_flags = CBC_ENUM.CBC_AGGREGATE;
            rst.deag_owner = VU_ID.FalconNullId;
            rst.components = null;
            rst.dirty_camp_base = 0;

#if CAMPTOOL
   if (GetEntityByCampID(camp_id))
      {
      MonoPrint("Got duplicate camp ID #%d.\n",camp_id);
      for (int i=0; i<MAX_CAMP_ENTITIES; i++)
         {
         if (!CampIDRenameTable[i])
            {
            CampIDRenameTable[i] = camp_id;
            break;
            }
         }
      }
   if (VuDatabase.vuDatabase.Find(Id()))
      {
      MonoPrint("Got duplicate VU_ID #%d.\n",Id().num_);
      }
#endif

#if TODO // DEBUG
            // Clear out entities owned by non-existant teams
            // KCK NOTE: This doesn't work in multi-player remote, as we often don't have teams at this point
            if (FalconSessionEntity.FalconLocalGame != null && FalconSessionEntity.FalconLocalGame.IsLocal())
            {
                if (TeamStatic.TeamInfo[GetTeam()] == null || TeamStatic.TeamInfo[GetOwner()] == null)
                {
                    int i;
                    for (i = 0; i < (int)TeamDataEnum.NUM_TEAMS && TeamStatic.TeamInfo[i] == null; i++) ;
                    SetOwner((byte)i);
                }
            }
#endif

            // Set owner to game master and associate the entity
            Debug.Assert(FalconSessionEntity.FalconLocalGame != null);
            if (FalconSessionEntity.FalconLocalGame != null)
            {
                rst.share_.ownerId_ = FalconSessionEntity.FalconLocalGame.OwnerId();
                rst.SetAssociation(FalconSessionEntity.FalconLocalGame.Id());
            }
        }

        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }
}
