using System;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using Objective = FalconNet.Campaign.ObjectiveClass;
using ObjectiveType = System.Byte;
using PriorityLevel = System.Byte;
using Unit = FalconNet.Campaign.UnitClass;
using Flight = FalconNet.Campaign.FlightClass;
using WayPoint = FalconNet.CampaignBase.WayPointClass;
using Team = System.SByte;
using GridIndex = System.Int16;
using VU_BYTE = System.Byte;
using Control = System.Byte;
using System.Diagnostics;
//using FalconNet.Sim;
using FalconNet.CampaignBase;
using FalconNet.Campaign;
using System.IO;
using FalconNet.Common.Encoding;
//using VU_ERRCODE=System.Int32;
using F4PFList = FalconNet.FalcLib.FalconPrivateList;
using F4POList = FalconNet.FalcLib.FalconPrivateOrderedList;

namespace FalconNet.Campaign
{

    // =======================
    // Campaign Objectives ADT
    // =======================
    public struct CampObjectiveTransmitDataType
    {
        public CampaignTime last_repair;	// Last time this objective got something repaired
        public short aiscore;		// Used for scoring junque
        public O_FLAGS obj_flags;		// Transmitable flags
        public byte supply;			// Amount of supply going through here
        public byte fuel;			// Amount of fuel going through here 
        public byte losses;			// Amount of supply/fuel losses (in percentage)
        public byte status;			// % operational
        public byte priority;		// Target's general priority
        public byte[] fstatus;		// Array of feature statuses (was [((FEATURES_PER_OBJ*2)+7)/8])
    };

    public struct CampObjectiveStaticDataType
    {
        public short nameid;			// Index into name table
        public short local_data;		// Local AI data dump
        public VU_ID parent;			// ID of parent SO or PO
        public Control first_owner;	// Origional objective owner
        public byte links;			// Number of links
        public RadarRangeClass radar_data;		// Data on what a radar stationed here can see
        public ObjClassDataType class_data;		// Pointer to class data
    }

    public class CampObjectiveLinkDataType
    {
        public byte[] costs = new byte[(int)MoveType.MOVEMENT_TYPES];	// Cost to go here, depending on movement type
        public VU_ID id;
    }

    public class ObjectiveClass : CampBaseClass
    {
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap fixed size pool
		void *operator new(size_t size) { Debug.Assert( size == sizeof(ObjectiveClass) ); return MemAllocFS(pool);	};
		void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
		static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(ObjectiveClass), 2500, 0 ); };
		static void ReleaseStorage()	{ MemPoolFree( pool ); };
		static MEM_POOL	pool;
#endif

        public CampObjectiveTransmitDataType obj_data;
        private int dirty_objective;
        public CampObjectiveStaticDataType static_data;
        public CampObjectiveLinkDataType[] link_data;		// The actual link data (was [OBJ_MAX_NEIGHBORS])
        public ATCBrain brain;

        // access functions
        public ulong GetObjFlags()
        {
            throw new NotSupportedException();
        }

        public void ClearObjFlags(O_FLAGS flags)
        {
            obj_data.obj_flags &= ~(flags);
        }

        public void SetObjFlags(O_FLAGS flags)
        {
            obj_data.obj_flags |= (flags);
        }

        // constructors
        public ObjectiveClass(ushort typeindex)
            : base(typeindex, 0 ) // Review this implementation for FreeFalcon. it has a second parameter
        {
            int size;

            dirty_objective = 0;
            static_data.first_owner = 0;
            static_data.nameid = 0;
            static_data.class_data = (ObjClassDataType)EntityDB.Falcon4ClassTable[typeindex - VU_LAST_ENTITY_TYPE].dataPtr;
            static_data.links = 0;
            //TODO static_data.radar_data = 0;
            obj_data.priority = 10;
            obj_data.status = 100;
            obj_data.obj_flags = 0;
            obj_data.fuel = 0;
            obj_data.supply = 0;
            obj_data.losses = 0;
            obj_data.last_repair = new CampaignTime(0);
            size = ((static_data.class_data.Features * 2) + 7) / 8;
#if USE_SH_POOLS
	obj_data.fstatus = (byte *)MemAllocPtr(gObjMemPool, sizeof(byte)*size, 0 );
#else
            obj_data.fstatus = new byte[size];
#endif
            obj_data.fstatus = new Control[size];
            //	for (i=0; i<size; i++)
            //		obj_data.fstatus[i] = 0xFF;

            link_data = null;

            if (GetFalconType() == ClassTypes.TYPE_AIRBASE ||
                GetFalconType() == ClassTypes.TYPE_AIRSTRIP)
            {

                if (GetFalconType() == ClassTypes.TYPE_AIRBASE)
                {
                    SetTacan(1);
                }
                brain = new ATCBrain(this);
            }
            else
            {
                brain = null;
            }
#if DEBUG_COUNT
	myolist.AddObj(Id().num_);
	gObjectiveCount++;
#endif
        }
 #if TODO 
        public ObjectiveClass(byte[] stream, ref int offset)
            : base(stream, ref offset)
        {
            throw new NotImplementedException();
        }
        //public virtual ~ObjectiveClass();

        public override int SaveSize()
        {

			int size = base.SaveSize ()
                + sizeof(ulong) //(CampaignTime)
                + sizeof(ulong)
                + sizeof(byte)
                + sizeof(byte)
                + sizeof(byte)
                + sizeof(byte)
                + ((static_data.class_data.Features * 2) + 7) / 8
                + sizeof(byte)
                + sizeof(short)
                + sizeof(VU_ID)
                + sizeof(Control)
                + sizeof(byte)
                + sizeof(byte)
                + static_data.links * sizeof(CampObjectiveLinkDataType);
			if (static_data.radar_data != null)
				size += sizeof(RadarRangeClass);
			return size;

            throw new NotImplementedException();
        }

        public virtual int SaveSize(int toDisk)
        {
            return sizeof(ulong) //(CampaignTime)
                + sizeof(byte)
                + sizeof(byte)
                + sizeof(byte)
                + sizeof(byte)
                + sizeof(byte)
                + ((static_data.class_data.Features * 2) + 7) / 8;
        }

        public virtual int Save(VU_BYTE[] stream)
        {
            throw new NotImplementedException();
        }

        public virtual int Save(VU_BYTE[] stream, int toDisk)
        {
            throw new NotImplementedException();
        }

        public void UpdateFromData(VU_BYTE[] stream)
        {
            throw new NotImplementedException();
        }
#endif
        // event Handlers
        public override VU_ERRCODE Handle(VuFullUpdateEvent evnt)
        {
            throw new NotImplementedException();
        }

        // Required pure virtuals handled by objective.h
        public override void SendDeaggregateData(VuTargetEntity p)
        {
            throw new NotImplementedException();
        }

        public override int RecordCurrentState(FalconSessionEntity f, int i)
        {
            throw new NotImplementedException();
        }

        public override int Deaggregate(FalconSessionEntity session)
        {
            throw new NotImplementedException();
        }

        public override int Reaggregate(FalconSessionEntity session)
        {
            throw new NotImplementedException();
        }

        public override int TransferOwnership(FalconSessionEntity session)
        {
            throw new NotImplementedException();
        }

        public override int Wake()
        {
#if TODO
#if ! NDEBUG
			//	MonoPrint ("Waking Objective #%d!\n",GetCampID());
#endif

			if (!OTWDriver.IsActive ())
				return 0;

			if (GetComponents () != null) {
				SetAwake (1);
				SimulationDriver.SimDriver.WakeCampaignFlight (false, this, GetComponents ());
			} else {
				return 0;
			}

			AwakeCampaignEntities++;
			//	RemoveFromSimLists();

			return 1;
#endif
            throw new NotImplementedException();
        }

        public override int Sleep()
        {
#if !NDEBUG
            //	MonoPrint ("Sleeping objective #%d!\n",GetCampID());
#endif

            //	OTWDriver.LockObject ();
            // 2002-04-14 put back in by MN - we need to sleep our features, and this does it, 
            //while a more general function name could have been chosen ;)
            SimulationDriver.SimDriver.SleepCampaignFlight(GetComponents()); //2002-02-11 REMOVED BY S.G. MPS original Cut and paste bug from UnitClass. Objectives have no flights!

            SetAwake(0);
            AwakeCampaignEntities--;

            //	InsertInSimLists(XPos(),YPos());
            //	OTWDriver.UnLockObject ();

            return 1;
        }

        public override void InsertInSimLists(float cameraX, float cameraY)
        {
            throw new NotImplementedException();
        }

        public override void RemoveFromSimLists()
        {
            throw new NotImplementedException();
        }

        public override void DeaggregateFromData(int size, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void ReaggregateFromData(int size, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void TransferOwnershipFromData(int size, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override MoveType GetMovementType()
        {
            return MoveType.NoMove;
        }

        private Random rand = new Random();

        public override int ApplyDamage(FalconCampWeaponsFire cwfm, byte bonusToHit)
        {
#if TODO
			Int32 i, hc, range, shot, losses = 0, totalShots = 0;
			GridIndex sx, sy, tx, ty;
			int str = 0, strength, flags;
			DamageDataType dt;
			Unit shooter = (Unit)VuDatabase.vuDatabase.Find (cwfm.dataBlock.shooterID);
			byte size, addcrater = 0;

			if (!IsLocal ())
				return 0;

			if (shooter == null)
				return 0;

			Debug.Assert (IsAggregate ());

			CampWeaponsFireStatic.gDamageStatusPtr = CampWeaponsFireStatic.gDamageStatusBuffer + 1;
			CampWeaponsFireStatic.gDamageStatusBuffer [0] = 0;
			shooter.GetLocation (out sx, out sy);
			GetLocation (out tx, out ty);
			range = (int)(Distance (sx, sy, tx, ty));

			// Unit type specific stuff
			if (shooter.IsFlight () && cwfm.dataBlock.dPilotId == 255) {
				WayPoint w = shooter.GetCurrentUnitWP ();
				// If we're attacking our pre-assigned WP target, check for a specific feature ID
				if (w != null && w.GetWPTargetID () == shooter.GetTargetID ())
					cwfm.dataBlock.dPilotId = w.GetWPTargetBuilding ();
			}

			for (i = 0; i < CampWeaponsFireStatic.MAX_TYPES_PER_CAMP_FIRE_MESSAGE && cwfm.dataBlock.weapon[i] != 0 && cwfm.dataBlock.shots[i] != 0; i++) {
				hc = CampWeapons.GetWeaponHitChance (cwfm.dataBlock.weapon [i], MoveType.NoMove, range) + bonusToHit;

				// Flight's get bonuses to hit based on vehicle type (ground vehicles should too -
				// but at this point, we don't really know which vehicle shot which weapon)

				// A.S. removed, as objectives have no defensive bonus
				if (CampBugFixes == null) {
					if (shooter.IsFlight ())
						hc += GetVehicleClassData (shooter.GetVehicleID (0)).HitChance [(int)MoveType.NoMove];
				}
				// end removed

				// HARMs will snap to current radar feature, if we're emitting
				if ((WeaponDataTable [cwfm.dataBlock.weapon [i]].GuidanceFlags & WEAP_GUIDANCE.WEAP_ANTIRADATION) && IsEmitting ())
					cwfm.dataBlock.dPilotId = static_data.class_data.RadarFeature;

				// Tally the losses
				strength = CampWeapons.GetWeaponStrength (cwfm.dataBlock.weapon [i]);
				str = 0;
				dt = (DamageDataType)CampWeapons.GetWeaponDamageType (cwfm.dataBlock.weapon [i]);
				if (dt == DamageDataType.NuclearDam)
					hc = Math.Max (95, hc); // minimum of 95% hitchance for nukes.
				flags = CampWeapons.GetWeaponFlags ().GetWeaponFlags (cwfm.dataBlock.weapon [i]);
				shot = 0;
				totalShots += cwfm.dataBlock.shots [i];
				while (cwfm.dataBlock.shots[i] - shot > 0) {
					if (rand.Next () % 100 < hc) {
						str += strength;
						losses += ApplyDamage (dt, ref str, cwfm.dataBlock.dPilotId, (short)flags);
					} else if (shooter.IsFlight () && addcrater < 3) {
						// Add a few craters if it's an air attack
						addcrater++;
					}
					shot += 1 + (rand.Next () % (losses + 1));		// Random stray shots - let's be nice
				}
			}

#if KEEP_STATISTICS
	if (shooter.IsFlight())
		{
		AS_Kills += losses;
		AS_Shots += totalShots;
		}
#endif

#if KEV_DEBUG
	//MonoPrint("%d (%d,%d) took %d losses from %d (%d,%d). range = %d\n",GetCampID(),tx,ty,losses,shooter.GetCampID(),sx,sy,range);
#endif
			// Record # of craters to add
			CampWeaponsFireStatic.gDamageStatusPtr = addcrater;
			CampWeaponsFireStatic.gDamageStatusPtr++;

			// Record the final state, to keep remote entities consitant
			size = (byte)((static_data.class_data.Features + 3) / 4);
			CampWeaponsFireStatic.gDamageStatusPtr = size;
			CampWeaponsFireStatic.gDamageStatusPtr++;
			memcpy (CampWeaponsFireStatic.gDamageStatusPtr, obj_data.fstatus, size);
			CampWeaponsFireStatic.gDamageStatusPtr += size;

			// Copy the data into the message
			cwfm.dataBlock.size = (ushort)(CampWeaponsFireStatic.gDamageStatusPtr - CampWeaponsFireStatic.gDamageStatusBuffer);
			cwfm.dataBlock.data = new byte[cwfm.dataBlock.size];
			memcpy (cwfm.dataBlock.data, CampWeaponsFireStatic.gDamageStatusBuffer, cwfm.dataBlock.size);

			// Send the weapon fire message (with target's post-damage status)
			FalcMesgStatic.FalconSendMessage (cwfm, false);

			return losses;
#endif
            throw new NotImplementedException();
        }

        // This Apply damage is called only for local entities, and resolves the number and type
        // of weapon shots vs this objective.
        // HOWEVER, it them broadcasts a FalconWeaponFireMessage which will generate visual effects,
        // update remote copies of this entity, call the mission evaluation/event storage routines,
        // and add any craters we require.
        public virtual int ApplyDamage(DamageDataType d, ref int str, int f, WEAP_FLAGS flags)
        {
#if TODO			
			int fid, hp, lost = 0, count = 0, this_pass;
			VIS_TYPES s;
			FeatureClassDataType fc;

			while (*str > 0 && count < AIInput.MAX_DAMAGE_TRIES) {
				count++;
				if (f >= static_data.class_data.Features || f < 0 || GetFeatureStatus (f) == VIS_TYPES.VIS_DESTROYED || GetFeatureID (f) == 0) {
					// Find something to bomb
					for (fid = 0, f = 255; fid < static_data.class_data.Features && f >= static_data.class_data.Features; fid++) {
						if (GetFeatureStatus (fid) != VIS_TYPES.VIS_DESTROYED && FeatureStatic.GetFeatureClassData (GetFeatureID (fid)).DamageMod [(int)d] > 0)
							f = fid;
					}
					continue;			// Force another pass;
				}
				fid = GetFeatureID (f);
				fc = FeatureStatic.GetFeatureClassData (fid);
				if (fc != null && fc.DamageMod [(int)d] > 0) {
					hp = fc.HitPoints * 100 / fc.DamageMod [(int)d];
					// Check if high explosive damage will do more
					if ((flags.IsFlagSet (WEAP_FLAGS.WEAP_AREA)) && fc.DamageMod [(int)DamageDataType.HighExplosiveDam] > fc.DamageMod [(int)d])
						hp = fc.HitPoints * 100 / fc.DamageMod [(int)DamageDataType.HighExplosiveDam];
					s = GetFeatureStatus (f);
					if (s == VIS_TYPES.VIS_DAMAGED)
						hp /= 2;							// Feature is already damaged
					this_pass = 1;

					//if (s != VIS_TYPES.VIS_DESTROYED && *str > rand()%hp)
					if (s != VIS_TYPES.VIS_DESTROYED && (hp == 0 || *str > rand.Next () % hp)) // JB 010401 CTD
						s = VIS_TYPES.VIS_DESTROYED;
                    //else if (s != VIS_TYPES.VIS_DESTROYED && s != VIS_TYPES.VIS_DAMAGED && *str > rand()%(hp/2))
					else if (s != VIS_TYPES.VIS_DESTROYED && s != VIS_TYPES.VIS_DAMAGED && (hp < 2 || *str > rand.Next () % (hp / 2))) // JB 010401 CTD
						s = VIS_TYPES.VIS_DAMAGED;
					else {
						count = AIInput.MAX_DAMAGE_TRIES;			// Stop applying damage, since we can't hurt our target
						this_pass = 0;						// Didn't actually kill this
						return lost;
					}
					if (this_pass != null) {
						SetFeatureStatus (f, s);
						CampWeaponsFireStatic.gDamageStatusPtr = (byte)f;
						CampWeaponsFireStatic.gDamageStatusPtr++;
						CampWeaponsFireStatic.gDamageStatusPtr = (byte)s;
						CampWeaponsFireStatic.gDamageStatusPtr++;
						CampWeaponsFireStatic.gDamageStatusBuffer [0]++;
						lost++;
					}
					if (!(flags.IsFlagSet (WEAP_FLAGS.WEAP_AREA)))				// Not area effect weapon, only get one kill per shot
                        *
						str = 0;
					else if (*str > AIInput.MINIMUM_STRENGTH * 2)		// Otherwise halve our strength and keep applying damage
                        *
						str /= 2;							// NOTE: This doesn't guarentee nearest adjacent feature
					else
						count = AIInput.MAX_DAMAGE_TRIES;
				} else
					f = rand.Next () % static_data.class_data.Features;
			}

			ResetObjectiveStatus ();

			return lost;
#endif
            throw new NotImplementedException();
        }

        public virtual int DecodeDamageData(byte[] data, Unit shooter, FalconDeathMessage dtm)
        {
#if TODO
			int lost, f, i;
			VIS_TYPES s;
			bool islocal = IsLocal ();
			byte size, addcrater;

			lost = *data;
			data++;
			for (i = 0; i < lost; i++) {
				// Score each hit, for mission evaluator
				f = *data;
				data++;
				s = *data;
				data++;
				// Record status only for remote entities
				if (islocal)
					SetFeatureStatus (f, s);
				// Add runway craters
				if (EntityDB.Falcon4ClassTable [GetFeatureID (f)].vuClassData.classInfo_ [(int)VU_CLASS.VU_TYPE] == Classtable_Types.TYPE_RUNWAY) { // (IS_RUNWAY)
					if (s == VIS_TYPES.VIS_DESTROYED)
						AddRunwayCraters (this, f, 8);
					else if (s == VIS_TYPES.VIS_DAMAGED)
						AddRunwayCraters (this, f, 4);
				}
				// Generate a death message if we or the shooter is a member of the package 
				if (dtm != null) {
					FeatureClassDataType fc;
					fc = FeatureStatic.GetFeatureClassData (GetFeatureID (f));
					dtm.dataBlock.dPilotID = 255;
					dtm.dataBlock.dIndex = (ushort)(fc.Index + VU_LAST_ENTITY_TYPE);
					// Update squadron and pilot statistics
					EvaluateKill (dtm, null, shooter, null, this);
				} else if (shooter.IsFlight ())
					EvaluateKill (dtm, null, shooter, null, this);
			}

			// Add any necessary craters
			addcrater = *data;
			data++;
			AddMissCraters (this, addcrater);

			// Record the current state of all features, for consistancy
			// (this is admittidly redundant, but could help avoid problems with missed messages)
			if (!islocal) {
				size = *data;
				data++;
				Debug.Assert (size == (static_data.class_data.Features + 3) / 4);
				memcpy (obj_data.fstatus, data, size);
				data += size;
			}

			// Reset objective status upon losses
			if (lost != 0) {
				if (GetObjectiveStatus () == 100)
					SetObjectiveRepairTime (Camp_GetCurrentTime ());
				// Reset local status
				ResetObjectiveStatus ();
				// The local entity sends atm a message if there's a chance we lost a runway
				// 2001-08-01 MODIFIED BY S.G. ARMYBASE SHOULD BE DEALT WITH TOO SINCE THEY CARRY CHOPPERS
				//		if (islocal && GetFalconType() == Classtable_Types.Classtable_Types.TYPE_AIRBASE && GetObjectiveStatus() < 51)
				if (islocal && (GetFalconType () == Classtable_Types.TYPE_AIRBASE || GetFalconType () == Classtable_Types.TYPE_ARMYBASE) && GetObjectiveStatus () < 51)
					TeamInfo [GetTeam ()].atm.SendATMMessage (Id (), GetTeam (), FalconAirTaskingMessage.atmZapAirbase, 0, 0, null, 0);
			}

			ResetObjectiveStatus ();

			return lost;
#endif
            throw new NotImplementedException();
        }

        public override byte[] GetDamageModifiers()
        {
            ObjClassDataType oc;

            oc = GetObjectiveClassData();
            if (oc == null)
                return null;
            return oc.DamageMod;
        }

        public override string GetName(string buffer, int size, int obj)
        {
#if TODO
            int nid, pnid = 0;
            Objective p;
            string buffer; //??

            nid = GetObjectiveNameID();
            if (nid == 0)
            {
                p = GetObjectiveParent();
                if (p != null)
                {
                    pnid = p.GetObjectiveNameID();
                    if (gLangIDNum == F4LANG_FRENCH)
                    {
                        string namestr;
                        ReadNameString(pnid, namestr, 79);
                        if (namestr[0] == 'A' || namestr[0] == 'a' || namestr[0] == 'E' || namestr[0] == 'e' ||
                            namestr[0] == 'I' || namestr[0] == 'i' || namestr[0] == 'O' || namestr[0] == 'o' ||
                            namestr[0] == 'U' || namestr[0] == 'u')
                            _sntprintf(name, size, "%s d'%s", ObjectiveStr[GetFalconType()], namestr);
                        else
                            _sntprintf(name, size, "%s de %s", ObjectiveStr[GetFalconType()], namestr);
                    }
                    else if (gLangIDNum == F4LANG_ITALIAN || gLangIDNum == F4LANG_SPANISH || gLangIDNum == F4LANG_PORTUGESE)
                        _sntprintf(name, size, "%s %s", ObjectiveStr[GetFalconType()], ReadNameString(pnid, buffer, 79));
                    else
                        _sntprintf(name, size, "%s %s", ReadNameString(pnid, buffer, 79), ObjectiveStr[GetFalconType()]);
                }
                else
                    _sntprintf(name, size, "%s", ReadNameString(nid, buffer, 79));
            }
            else
                _sntprintf(name, size, "%s", ReadNameString(nid, buffer, 79));

            if (mode && _istlower(name[0]))
                name[0] = (char)_toupper(name[0]);

            // _sntprintf should do this for us, but for some reason it sometimes doesn't
            name[size - 1] = 0;

            return name;
#endif
            throw new NotImplementedException();
        }

        public override string GetFullName(string name, int size, int obj)
        {
            return GetName(name, size, obj);
        }

        // Returns best hitchance of the objective at a given range
        public override int GetHitChance(int mt, int range)
        {
            return 0;
            // Commented out body removed ny leonr
        }

        // Quicker, aproximate version of the above
        public override int GetAproxHitChance(int mt, int range)
        {
            return 0;
            // Commented out body removed ny leonr
        }

        // Returns strength of the objective at a given range
        public override int GetCombatStrength(int mt, int range)
        {
            return 0;
            // Commented out body removed ny leonr
        }

        // Quicker, aproximate version of the above
        public override int GetAproxCombatStrength(int mt, int range)
        {
            // Commented out body removed ny leonr
            return 0;
        }

        public override int GetWeaponRange(int mt, FalconEntity target = null)
        {
            // Commented out body removed ny leonr
            return 0;
        } // 2008-03-08 ADDED SECOND DEFAULT PARM

        // Returns the maximum range of the objective (this is precalculated)
        public override int GetAproxWeaponRange(int mt)
        {
            // Commented out body removed ny leonr
            return 0;
        }

        public override int GetDetectionRange(int mt)
        {
#if TODO
			ObjClassDataType oc = GetObjectiveClassData ();
			int dr = 0;

			Debug.Assert (oc);
			if (IsEmitting () && oc.RadarFeature < 255 && GetFeatureStatus (oc.RadarFeature) != VIS_TYPES.VIS_DESTROYED) {
            // 2001-04-21 MODIFIED BY S.G. ABOVE 250 HAS A NEW MEANING SO USE THE UNIT ELECTRONIC DETECTION RANGE INSTEAD...
            //		dr = oc.Detection[mt];
				if ((dr = oc.Detection [mt]) > 250)
					dr = 250 + (oc.Detection [mt] - 250) * 50;
			}
			// END OF MODIFIED SECTION
			if (!dr)
				dr = CampbaseStatic.GetVisualDetectionRange (mt);
			return dr;
#endif
            throw new NotImplementedException();
        }						// Takes into account emitter status

        public override int GetElectronicDetectionRange(MoveType mt)
        {
            if (static_data.class_data.RadarFeature < 255 && GetFeatureStatus(static_data.class_data.RadarFeature) != VIS_TYPES.VIS_DESTROYED)
            {
                // 2001-04-21 MODIFIED BY S.G. ABOVE 250 HAS A NEW MEANING SO USE THE UNIT ELECTRONIC DETECTION RANGE INSTEAD...
                //		return static_data.class_data.Detection[mt];
                if (static_data.class_data.Detection[(int)mt] > 250)
                    return 250 + (static_data.class_data.Detection[(int)mt] - 250) * 50;
                return static_data.class_data.Detection[(int)mt];
            }
            return 0;
        }		// Max Electronic detection range, even if turned off

        public override int CanDetect(FalconEntity ent)
        {
#if TODO			
			float ds, mrs, vdr, dx, dy;
			MoveType mt;

			mt = ent.GetMovementType ();
			ds = DistSqu (XPos (), YPos (), ent.XPos (), ent.YPos ());
			mrs = (float)(GetDetectionRange (mt) * Phyconst.KM_TO_FT);
			mrs *= mrs;
			if (ds > mrs)
				return 0;

			// Additional detection requirements against aircraft
			if (mt == LowAir || mt == Air) {
				// 2001-04-22 ADDED BY S.G. OBJECTIVES NEEDS TO BE AFFECTED BY SOJ AS MUCH AS UNITS
				if (ent.IsFlight ()) {
					Flight ecmFlight = ((FlightClass*)ent).GetECMFlight ();
					if (((FlightClass*)ent).HasAreaJamming ())
						ecmFlight = (FlightClass*)ent;
					else if (ecmFlight) {
						if (!ecmFlight.IsAreaJamming ())
							ecmFlight = null;
					}

					if (ecmFlight) {
						// Now, here's what we need to do:
						// 1. For now jamming power has 360 degrees coverage
						// 2. The radar range will be reduced by the ratio of its normal range and the jammer's range to the radar to the power of two
						// 3. The jammer is dropping the radar gain, effectively dropping its detection distance
						// 4. If the flight is outside this new range, it's not detected.

						// Get the range of the SOJ to the radar
						float jammerRange = DistSqu (ecmFlight.XPos (), ecmFlight.YPos (), XPos (), YPos ());

						// If the SOJ is within the radar normal range, 'adjust' it. If this is now less that ds (our range to the radar), return 0.
						// SOJ can jamm even if outside the detection range of the radar
						if (jammerRange < mrs * 2.25f) {
							jammerRange = jammerRange / (mrs * 2.25f); // No need to check for zero because jammerRange has to be LESS than mrs to go in
							mrs *= jammerRange * jammerRange;
							if (ds > mrs) {
								// The radar is being jammed, check if visual detection will do
								vdr = (float)CampbaseStatic.GetVisualDetectionRange (mt) * Phyconst.KM_TO_FT;
								vdr *= vdr;
								if (ds < vdr)
									return 1;
								// Nope, then it's not detected.
								return 0;
							}
						}
					}
				}
				// END OF ADDED SECTION
				if (!HasRadarRanges ()) {
					// Only check vs visual detection range
					// 2001-03-16 MODIFIED BY S.G. LOOKS LIKE THEY FORGOT GetVisualDetectionRange IS IN KILOMETERS AND NOT FEET
					//			vdr = (float)CampbaseStatic.GetVisualDetectionRange(mt);
					vdr = (float)CampbaseStatic.GetVisualDetectionRange (mt) * Phyconst.KM_TO_FT;
					vdr *= vdr;
					if (ds < vdr)
						return 1;
					return 0;
				}
				dx = ent.XPos () - XPos ();
				dy = ent.YPos () - YPos ();
				// Stealth aircraft act as if they're flying at double their range
				if (ent.IsFlight ()) {
					UnitClassDataType uc = ((Flight)ent).GetUnitClassData ();
					if (uc.Flags & VEH_FLAGS.VEH_STEALTH) {
						// 2001-04-29 MODIFIED BY S.G. IF IT'S A STEALTH AND IT GOT HERE, IT WASN'T DETECTED VISUALLY SO ABORT RIGHT NOW
						//				dx *= 2.0F;
						//				dy *= 2.0F;
						vdr = (float)CampbaseStatic.GetVisualDetectionRange (mt) * Phyconst.KM_TO_FT;
						vdr *= vdr;
						if (ds < vdr)
							return 1;
						return 0;
					}
				}
				// 2001-04-06 MODIFIED BY S.G. NEED TO ACCOMODATE FOR THE OBJECTIVE'S MSL ALTITUDE SINCE ZPos IS 0 FOR OBJECTIVE...
				//		return static_data.radar_data.CanDetect(dx,dy,ent.ZPos()-ZPos());
				Debug.Assert (ZPos () == 0.0f);	// Warn if the objective ZPos is something else than 0
				float AGLz = ent.ZPos () + TheMap.GetMEA (XPos (), YPos ());
				if (AGLz > 0.0f)
					return 0; // Can't see very well if our target is lower than us...

				return static_data.radar_data.CanDetect (dx, dy, AGLz); // GetMEA returns a positive number that we must substract from our target altitude
			}
			return 1;
#endif
            throw new NotImplementedException();
        }					// Nonzero if this entity can see ent

        public override bool OnGround()
        {
            return true;
        }

        public override FEC_RADAR GetRadarMode()
        {
            if (IsEmitting())
                return FEC_RADAR.FEC_RADAR_SEARCH_100;
            else
                return FEC_RADAR.FEC_RADAR_OFF;
        }

        public override Radar_types GetRadarType()
        {
            ObjClassDataType data = GetObjectiveClassData();
            Debug.Assert(data != null);
            if (data.RadarFeature < 255)
            {
                return FeatureStatic.GetFeatureClassData(GetFeatureID(data.RadarFeature)).RadarType;
            }
            else
            {
                return Radar_types.RDR_NO_RADAR;
            }
        }

        // These are only really relevant for sam/airdefense/radar entities
        public override int GetNumberOfArcs()
        {
            if (!HasRadarRanges())
                return 1;
            return static_data.radar_data.GetNumberOfArcs();
        }

        public override float GetArcRatio(int anum)
        {
            if (!HasRadarRanges())
                return 0.0F;
            return static_data.radar_data.GetArcRatio(anum);
        }

        public override float GetArcRange(int anum)
        {
            if (!HasRadarRanges())
                return 0.0F;
            return static_data.radar_data.GetArcRange(anum);
        }

        public override void GetArcAngle(int anum, ref float a1, ref float a2)
        {
            if (!HasRadarRanges())
            {
                a1 = 0.0F;
                a2 = (float)(2.0F * Math.PI);
                return;
            }
            static_data.radar_data.GetArcAngle(anum, ref a1, ref a2);
        }

        public int SiteCanDetect(FalconEntity ent)
        {
            float dx, dy;

            if (!HasRadarRanges())
                return 0;
            dx = ent.XPos() - XPos();
            dy = ent.YPos() - YPos();
            // Stealth aircraft act as if they're flying at double their range
            if (ent.IsFlight())
            {
                UnitClassDataType uc = ((Flight)ent).GetUnitClassData();
                if (((VEH_FLAGS)uc.Flags).IsFlagSet(VEH_FLAGS.VEH_STEALTH))
                {
                    dx *= 2.0F;
                    dy *= 2.0F;
                }
            }
            return static_data.radar_data.CanDetect(dx, dy, ent.ZPos() - ZPos());
        }

        public float GetSiteRange(FalconEntity ent)
        {
            float dx, dy;

            if (!HasRadarRanges())
                return 0.0F;
            dx = ent.XPos() - XPos();
            dy = ent.YPos() - YPos();
            return static_data.radar_data.GetRadarRange(dx, dy, ent.ZPos() - ZPos());
        }

        // core functions
        public void SendObjMessage(VU_ID from, short mes, short d1, short d2, short d3)
        {
#if TODO
			FalconObjectiveMessage message = new FalconObjectiveMessage (Id (), FalconSessionEntity.FalconLocalGame);

			message.dataBlock.from = from;
			message.dataBlock.message = mes;
			message.dataBlock.data1 = d1;
			message.dataBlock.data2 = d2;
			message.dataBlock.data3 = d3;
			FalcMesgStatic.FalconSendMessage (message, true);
#endif
            throw new NotImplementedException();
        }

        public void DisposeObjective()
        {
            throw new NotImplementedException();
        }

        public void DamageObjective(int loss)
        {
            throw new NotImplementedException();
        }

        public void AddObjectiveNeighbor(Objective o, byte[] c)
        {
#if TODO
			int i;
			Objective n;
			CampObjectiveLinkDataType[] tmp_link;

			// Find if it exists first
			for (i = 0; i < static_data.links; i++) {
				n = (Objective)(VuDatabase.vuDatabase.Find (link_data [i].id));
				if (n != null && n == o) {
					SetNeighborCosts (i, c);
					return;
				}
			}

			// Otherwise, add a new one
			static_data.links++;
			tmp_link = link_data;
#if USE_SH_POOLS
	link_data = (CampObjectiveLinkDataType *)MemAllocPtr(gObjMemPool, sizeof(CampObjectiveLinkDataType)*static_data.links, 0 );
#else
			link_data = new CampObjectiveLinkDataType[static_data.links];
#endif
			memcpy (link_data, tmp_link, (static_data.links - 1) * sizeof(CampObjectiveLinkDataType));
			tmp_link = null;
			link_data [static_data.links - 1].id = o.Id ();
			SetNeighborCosts (i, c);
#endif
            throw new NotImplementedException();
        } //TODO[MOVEMENT_TYPES]);

        public void RemoveObjectiveNeighbor(int n)
        {
#if TODO
			CampObjectiveLinkDataType[] tmp_link;
			int nn;

			if (n >= static_data.links)
				return;

			static_data.links--;
			tmp_link = link_data;
#if USE_SH_POOLS
	link_data = (CampObjectiveLinkDataType *)MemAllocPtr(gObjMemPool, sizeof(CampObjectiveLinkDataType)*static_data.links, 0 );
#else
			link_data = new CampObjectiveLinkDataType[static_data.links];
#endif
			for (nn = 0; nn < static_data.links + 1; nn++) {
				if (nn < n)
					memcpy (&link_data [nn], &tmp_link [nn], sizeof(CampObjectiveLinkDataType));
				if (nn > n)
					memcpy (&link_data [nn - 1], &tmp_link [nn], sizeof(CampObjectiveLinkDataType));
			}
			tmp_link = null;
#endif
            throw new NotImplementedException();
        }

        public void SetNeighborCosts(int num, byte[] c)
        {
            int j;

            if (num >= static_data.links)
                return;

            for (j = 0; j < (int)MoveType.MOVEMENT_TYPES; j++)
            {
                if (c[j] < 255)
                    link_data[num].costs[j] = (byte)c[j];
                else
                    link_data[num].costs[j] = 255;
            }
        }// TODO [MOVEMENT_TYPES]);

        public override bool IsObjective()
        {
            return true;
        }

        public bool IsFrontline()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_FRONTLINE);
        }

        public bool IsSecondline()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_SECONDLINE);
        }

        public bool IsThirdline()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_THIRDLINE);
        }

        public bool IsNearfront()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_THIRDLINE | O_FLAGS.O_SECONDLINE | O_FLAGS.O_FRONTLINE);
        }

        public bool IsBeach()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_BEACH);
        }

        public bool IsPrimary()
        {
            if (GetFalconType() == ClassTypes.TYPE_CITY && obj_data.priority > AIInput.PRIMARY_OBJ_PRIORITY)
                return true;
            return false;
        }

        public bool IsSecondary()
        {
            // Only cities and towns can be secondary objectives, and ALL automatically are
            if ((GetFalconType() == ClassTypes.TYPE_CITY || GetFalconType() == ClassTypes.TYPE_TOWN) && obj_data.priority > AIInput.SECONDARY_OBJ_PRIORITY)
                return true;
            return false;
        }

        public bool IsSupplySource()
        {
            if (GetFalconType() == ClassTypes.TYPE_CITY || GetFalconType() == ClassTypes.TYPE_PORT || GetFalconType() == ClassTypes.TYPE_DEPOT || GetFalconType() == ClassTypes.TYPE_ARMYBASE)
            {
                if (!IsFrontline() && !IsSecondline())
                    return true;
            }
            return false;
        }

        public bool IsGCI()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_IS_GCI);
        }	// 2002-02-13 ADDED BY S.G.
        public bool HasNCTR()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_HAS_NCTR);
        }	// 2002-02-13 ADDED BY S.G.

        public bool HasRadarRanges()
        {
            if (static_data.radar_data != null)
                return true;

            return false;
        }

        public void UpdateObjectiveLists()
        {
            throw new NotImplementedException();
        }

        public void ResetLinks()
        {
            throw new NotImplementedException();
        }

        public void Dump()
        {
            throw new NotImplementedException();
        }

        public void Repair()
        {
#if TODO
			int repair, bf;
			CampaignTime time;

			if (GetObjectiveStatus () != 100) {
				time = Camp_GetCurrentTime () - GetObjectiveRepairTime ();
				repair = time / CampaignHours;
				bf = BestRepairFeature (this, &repair);

				// JB 000811
				// Repair one feature at a time and reset the repair time
				// only after something has been fixed.
				/*//-		while (bf > -1)
                            {
                            RepairFeature(bf);
                            bf = BestRepairFeature(this, &repair);
                            }
                //-*/
				if (bf > -1) {
					RepairFeature (bf);
					SetObjectiveRepairTime (Camp_GetCurrentTime ());
				}
			}
			// SetObjectiveRepairTime(Camp_GetCurrentTime()); //-
			// JB 000811

			ResetObjectiveStatus ();
#endif
            throw new NotImplementedException();
        }

        // Flag setting stuff
        public void SetManual(int s)
        {
            obj_data.obj_flags |= O_FLAGS.O_MANUAL_SET;
            if (s == 0)
                obj_data.obj_flags ^= O_FLAGS.O_MANUAL_SET;
        }

        public bool ManualSet()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_MANUAL_SET);
        }

        public override void SetJammed(int j)
        {
            obj_data.obj_flags |= O_FLAGS.O_JAMMED;
            if (j == 0)
                obj_data.obj_flags ^= O_FLAGS.O_JAMMED;
        }

        public bool Jammed()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_JAMMED);
        }

        public void SetSamSite(int s)
        {
            obj_data.obj_flags |= O_FLAGS.O_SAM_SITE;
            if (s == 0)
                obj_data.obj_flags ^= O_FLAGS.O_SAM_SITE;
        }

        public bool SamSite()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_SAM_SITE);
        }

        public void SetArtillerySite(int a)
        {
            obj_data.obj_flags |= O_FLAGS.O_ARTILLERY_SITE;
            if (a == 0)
                obj_data.obj_flags ^= O_FLAGS.O_ARTILLERY_SITE;
        }

        public bool ArtillerySite()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_ARTILLERY_SITE);
        }

        public void SetAmbushCAPSite(int a)
        {
            obj_data.obj_flags |= O_FLAGS.O_AMBUSHCAP_SITE;
            if (a == 0)
                obj_data.obj_flags ^= O_FLAGS.O_AMBUSHCAP_SITE;
        }

        public bool AmbushCAPSite()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_AMBUSHCAP_SITE);
        }

        public void SetBorderSite(int a)
        {
            obj_data.obj_flags |= O_FLAGS.O_BORDER_SITE;
            if (a == 0)
                obj_data.obj_flags ^= O_FLAGS.O_BORDER_SITE;
        }

        public bool BorderSite()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_BORDER_SITE);
        }

        public void SetMountainSite(int a)
        {
            obj_data.obj_flags |= O_FLAGS.O_MOUNTAIN_SITE;
            if (a == 0)
                obj_data.obj_flags ^= O_FLAGS.O_MOUNTAIN_SITE;
        }

        public bool MountainSite()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_MOUNTAIN_SITE);
        }

        public void SetCommandoSite(int c)
        {
            obj_data.obj_flags |= O_FLAGS.O_COMMANDO_SITE;
            if (c == 0)
                obj_data.obj_flags ^= O_FLAGS.O_COMMANDO_SITE;
        }

        public bool CommandoSite()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_COMMANDO_SITE);
        }

        public void SetFlatSite(int a)
        {
            obj_data.obj_flags |= O_FLAGS.O_FLAT_SITE;
            if (a == 0)
                obj_data.obj_flags ^= O_FLAGS.O_FLAT_SITE;
        }

        public bool FlatSite()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_FLAT_SITE);
        }

        public void SetRadarSite(int r)
        {
            obj_data.obj_flags |= O_FLAGS.O_RADAR_SITE;
            if (r == 0)
                obj_data.obj_flags ^= O_FLAGS.O_RADAR_SITE;
        }

        public bool RadarSite()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_RADAR_SITE);
        }

        public void SetAbandoned(int a)
        {
            obj_data.obj_flags |= O_FLAGS.O_ABANDONED;
            if (a == 0)
                obj_data.obj_flags ^= O_FLAGS.O_ABANDONED;
        }

        public bool Abandoned()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_ABANDONED);
        }

        public void SetNeedRepair(int r)
        {
            obj_data.obj_flags |= O_FLAGS.O_NEED_REPAIR;
            if (r == 0)
                obj_data.obj_flags ^= O_FLAGS.O_NEED_REPAIR;
        }

        public bool NeedRepair()
        {
            return obj_data.obj_flags.IsFlagSet(O_FLAGS.O_NEED_REPAIR);
        }

        // Dirty Functions
        public void MakeObjectiveDirty(Dirty_Objective bits, Dirtyness score)
        {
            throw new NotImplementedException();
        }

        public override void WriteDirty(byte[] stream, ref int pos)
        {
            throw new NotImplementedException();
        }

        public override void ReadDirty(byte[] stream, ref int pos)
        {
            throw new NotImplementedException();
        }

        // Objective data stuff
        public override void SetOwner(Control c)
        {
            base.SetOwner(c);
            SetDelta(1);
        }

        public void SetObjectiveOldown(Control c)
        {
            static_data.first_owner = c;
        }

        public void SetObjectiveParent(VU_ID p)
        {
            static_data.parent = p;
        }

        public void SetObjectiveNameID(short n)
        {
            static_data.nameid = n;
        }

        public void SetObjectiveName(string name)
        {
#if TODO
			int nid;

			nid = GetObjectiveNameID ();
			if (name == null || name [0] == 0 || name [0] == '0') {
				if (nid == 0)
					return;
				SetObjectiveNameID (0);
				return;
			}
			if (nid == 0) {
				nid = AddName (name);
				SetObjectiveNameID ((short)nid);
			} else
				SetName (nid, name);
#endif
            throw new NotImplementedException();
        }

        public void SetObjectivePriority(PriorityLevel p)
        {
#if TODO		
			obj_data.priority = p;
#endif
            throw new NotImplementedException();
        }

        public void SetObjectiveScore(short score)
        {
            obj_data.aiscore = score;
        }

        public void SetObjectiveRepairTime(CampaignTime t)
        {
            obj_data.last_repair = t;
        }
        // JB 000811
        // Set the last repair time to now if some damage has been taken
        //void SetObjectiveStatus (byte s)						{	obj_data.status = s; MakeObjectiveDirty (DIRTY_STATUS, SEND_NOW); }
        public void SetObjectiveStatus(byte s)
        {
            if (obj_data.status > s)
                obj_data.last_repair = Camplib.Camp_GetCurrentTime();
            obj_data.status = s;
            MakeObjectiveDirty(Dirty_Objective.DIRTY_STATUS, EntityDB.DDP[180].priority);
        }
        //void SetObjectiveStatus (byte s)						{	if (obj_data.status > s) obj_data.last_repair = Camp_GetCurrentTime(); obj_data.status = s; MakeObjectiveDirty (DIRTY_STATUS, SEND_NOW); }
        // JB 000811
        public void SetObjectiveSupply(byte s)
        {
            obj_data.supply = s;
        }

        public void SetObjectiveFuel(byte f)
        {
            obj_data.fuel = f;
        }

        public void SetObjectiveSupplyLosses(byte l)
        {
            obj_data.losses = l;
        }

        public void SetObjectiveType(ObjectiveType t)
        {
#if TODO
			byte type, stype;
			int dindex;

			type = t;
			stype = 1;						// Try for a real objective
			dindex = GetClassID (DOMAIN_LAND, CLASS_OBJECTIVE, type, stype, 0, 0, 0, 0);
			if (!dindex)
				return;
			SetObjectiveClass (dindex + VU_LAST_ENTITY_TYPE);
			UpdateObjectiveLists ();
#endif
            throw new NotImplementedException();
        }

        void SetObjectiveSType(byte s)
        {
#if TODO
			byte type, stype;
			int dindex;

			type = GetFalconType ();
			stype = s;						// Try for a real objective
			dindex = GetClassID (DOMAIN_LAND, CLASS_OBJECTIVE, type, stype, 0, 0, 0, 0);
			if (!dindex)
				return;
			SetObjectiveClass (dindex + VU_LAST_ENTITY_TYPE);
			UpdateObjectiveLists ();
#endif
            throw new NotImplementedException();
        }

        public void SetObjectiveClass(int dindex)
        {
#if TODO
			int nsize;

			SetEntityType ((ushort)dindex);
			static_data.class_data = (ObjClassDataType*)EntityDB.Falcon4ClassTable [dindex - VU_LAST_ENTITY_TYPE].dataPtr;
			if (obj_data.fstatus != null)
				obj_data.fstatus = null;
			nsize = ((static_data.class_data.Features * 2) + 7) / 8;
#if USE_SH_POOLS
	obj_data.fstatus = (byte *)MemAllocPtr(gObjMemPool, sizeof(byte)*nsize, 0 );
#else
			obj_data.fstatus = new byte[nsize];
#endif
			memset (obj_data.fstatus, 0, nsize);
#endif
            throw new NotImplementedException();
        }

        public void SetFeatureStatus(int f, VIS_TYPES n)
        {
#if TODO
			int i = f / 4;

			if (GetFeatureStatus (f) == n)
				return;

			// Check for critical links and set those features accordingly. NOTE: repair accross critical links too..
			if (n == VIS_TYPES.VIS_DESTROYED || n == VIS_TYPES.VIS_REPAIRED) {
				if (EntityDB.FeatureEntryDataTable [static_data.class_data.FirstFeature + f].Flags & FEAT_PREV_CRIT)
					SetFeatureStatus (f - 1, n, f);
				if (EntityDB.FeatureEntryDataTable [static_data.class_data.FirstFeature + f].Flags & FEAT_NEXT_CRIT)
					SetFeatureStatus (f + 1, n, f);
			}

			f -= i * 4;
			obj_data.fstatus [i] = (byte)((obj_data.fstatus [i] & ~(3 << (f * 2))) | (n << (f * 2)));
			MakeObjectiveDirty (DIRTY_STATUS, DDP [9].priority);
			MakeObjectiveDirty (DIRTY_STATUS, SEND_NOW);
			SetDelta (1);
#endif
            throw new NotImplementedException();
        }

        public void SetFeatureStatus(int f, VIS_TYPES n, int from)
        {
#if TODO
			int i = f / 4;

			if (GetFeatureStatus (f) == n)
				return;

			// Check for critical links and set those features accordingly.
			if (n == VIS_TYPES.VIS_DESTROYED || n == VIS_TYPES.VIS_REPAIRED) {
				if (from != f - 1 && (EntityDB.FeatureEntryDataTable [static_data.class_data.FirstFeature + f].Flags & FEAT_PREV_CRIT))
					SetFeatureStatus (f - 1, n, f);
				if (from != f + 1 && (EntityDB.FeatureEntryDataTable [static_data.class_data.FirstFeature + f].Flags & FEAT_NEXT_CRIT))
					SetFeatureStatus (f + 1, n, f);
			}

			f -= i * 4;
			obj_data.fstatus [i] = (byte)((obj_data.fstatus [i] & ~(3 << (f * 2))) | (n << (f * 2)));
			ResetObjectiveStatus ();
			SetDelta (1);
#endif
            throw new NotImplementedException();
        }

        public VU_ID GetNeighborId(int n)
        {
            return link_data[n].id;
        }

        public Objective GetNeighbor(int num)
        {
            Objective n;

            if (num >= static_data.links)
                return null;
            n = (Objective)VUSTATIC.vuDatabase.Find(link_data[num].id);
            if (n == null)
            {
                // Better axe this, since we couldn't find it.
                RemoveObjectiveNeighbor(num);
            }
            return n;
        }

        public float GetNeighborCost(int n, MoveType t)
        {
            return link_data[n].costs[(int)t];
        }

        public Control GetObjectiveOldown()
        {
            return static_data.first_owner;
        }

        public ObjectiveClass GetObjectiveParent()
        {
            return FindStatic.FindObjective(static_data.parent);
        }

        public ObjectiveClass GetObjectiveSecondary()
        {
            if (IsSecondary())
                return this;
            else
                return (Objective)VUSTATIC.vuDatabase.Find(static_data.parent);
        }

        public ObjectiveClass GetObjectivePrimary()
        {
            if (IsPrimary())
                return this;
            else if (IsSecondary())
                return (Objective)VUSTATIC.vuDatabase.Find(static_data.parent);
            else
            {
                Objective so = (Objective)VUSTATIC.vuDatabase.Find(static_data.parent);
                if (so != null)
                    return so.GetObjectivePrimary();
            }
            return null;
        }

        public VU_ID GetObjectiveParentID()
        {
            return static_data.parent;
        }

        public int GetObjectiveNameID()
        {
            return static_data.nameid;
        }

        public int NumLinks()
        {
            return static_data.links;
        }

        public short GetObjectivePriority()
        {
            return obj_data.priority;
        }

        public byte GetObjectiveStatus()
        {
            return obj_data.status;
        }

        public int GetObjectiveScore()
        {
            return obj_data.aiscore;
        }

        public CampaignTime GetObjectiveRepairTime()
        {
            return obj_data.last_repair;
        }

        public short GetObjectiveSupply()
        {
            return obj_data.supply;
        }

        public short GetObjectiveFuel()
        {
            return obj_data.fuel;
        }

        public short GetObjectiveSupplyLosses()
        {
            return obj_data.losses;
        }

        public short GetObjectiveDataRate()
        {
            if (static_data.class_data == null)
                return 0;
            return (short)(static_data.class_data.DataRate * GetObjectiveStatus() / 100);
        }

        public short GetAdjustedDataRate()
        {
            int almost;

            if (static_data.class_data == null)
                return 0;

            if (static_data.class_data.DataRate == 0)
                static_data.class_data.DataRate = 1;
            almost = (100 / static_data.class_data.DataRate) - 1;
            return (short)((GetObjectiveStatus() + almost) * static_data.class_data.DataRate / 100);
        }

        public short GetTotalFeatures()
        {
            return static_data.class_data.Features;
        }

        public VIS_TYPES GetFeatureStatus(int f)
        {
            int i = f / 4;
            f -= i * 4;
            return (VIS_TYPES)((obj_data.fstatus[i] >> (f * 2)) & 0x03);
        }

        public int GetFeatureValue(int f)
        {
            if (static_data.class_data == null)
                return 0;
            return EntityDB.FeatureEntryDataTable[static_data.class_data.FirstFeature + f].Value;
        }

        public int GetFeatureRepairTime(int f)
        {
            if (static_data.class_data == null)
                return 0;
            return GetFeatureRepairTime(EntityDB.FeatureEntryDataTable[static_data.class_data.FirstFeature + f].Index);
        }

        public int GetFeatureID(int f)
        {
            Debug.Assert(static_data.class_data != null);
            return EntityDB.FeatureEntryDataTable[static_data.class_data.FirstFeature + f].Index;
        }

        public int GetFeatureOffset(int f, ref float x, ref float y, ref float z)
        {
            if (static_data.class_data == null)
                return 0;

            x = EntityDB.FeatureEntryDataTable[static_data.class_data.FirstFeature + f].Offset.x;
            y = EntityDB.FeatureEntryDataTable[static_data.class_data.FirstFeature + f].Offset.y;
            z = EntityDB.FeatureEntryDataTable[static_data.class_data.FirstFeature + f].Offset.z;
            return 1;
        }

        public ObjClassDataType GetObjectiveClassData()
        {
            return static_data.class_data;
        }

        public string GetObjectiveClassName()
        {
            throw new NotImplementedException();
        }
        //		int RoE (VuEntity  e, int type);

        public byte GetExpectedStatus(int hours)
        {
#if TODO
			int bf, s;

			s = GetObjectiveStatus ();
			hours += ((Camp_GetCurrentTime () - GetObjectiveRepairTime ()) / CampaignHours);
			bf = BestRepairFeature (this, &hours);
			while (bf > -1) {
				s += GetFeatureValue (bf) / 2;
				bf = BestRepairFeature (this, &hours);
			}
			if (s > 100)
				s = 100;
			return (byte)s;
#endif
            throw new NotImplementedException();
        }

        public int GetRepairTime(int status)
        {
#if TODO		
			int s, hours = 2400, f;

			ResetObjectiveStatus ();
			s = GetObjectiveStatus ();
			while (s < status) {
				f = BestRepairFeature (this, &hours);
				if (f < 0)
					break;
				s += GetFeatureValue (f) / 2;
			}
			return 2400 - hours;
#endif
            throw new NotImplementedException();
        }

        public byte GetBestTarget()
        {
            int i, v, bv = 0, f = 0;

            for (i = 0; i < static_data.class_data.Features; i++)
            {
                v = GetFeatureValue(i);
                if (v > bv && GetFeatureStatus(i) != VIS_TYPES.VIS_DESTROYED)
                {
                    bv = v;
                    f = i;
                }
            }
            return (byte)f;
        }

        public void ResetObjectiveStatus()
        {
#if TODO
			int f, s;

			s = 100;
			// Airbases use their own (slightly different) version of determining status:
			if (GetFalconType () == Classtable_Types.TYPE_AIRBASE) {
				// AIRBASE version
				for (f = 0; s && f < static_data.class_data.Features; f++) {
					// Only adjust status for non-runways
					if (EntityDB.Falcon4ClassTable [GetFeatureID (f)].vuClassData.classInfo_ [(int)VU_CLASS.VU_TYPE] != Classtable_Types.TYPE_RUNWAY) { // (IS_RUNWAY)
						if (GetFeatureStatus (f) == VIS_TYPES.VIS_DAMAGED)
							s -= GetFeatureValue (f) / 2;
						if (GetFeatureStatus (f) == VIS_TYPES.VIS_DESTROYED)
							s -= GetFeatureValue (f);
					}
				}
				if (s <= 0)
					s = 0;
				else {
					// Make sure we're below our maximum for # of active runways
					int index, runways = 0, inactive = 0, max;
					ObjClassDataType* oc = GetObjectiveClassData ();
					index = oc.PtDataIndex;
					while (index) {
						if (PtHeaderDataTable [index].type == RunwayPt) {
							runways++;
							if (CheckHeaderStatus (this, index) == VIS_TYPES.VIS_DESTROYED)
								inactive++;
						}
						index = PtHeaderDataTable [index].nextHeader;
					}
					if (!runways)
						max = 0;
					else
						max = ((runways - inactive) * 100) / runways;
					if (s > max)
						s = max;
				}
			} else {
				for (f = 0; s > 0 && f < static_data.class_data.Features; f++) {
					if (GetFeatureStatus (f) == VIS_TYPES.VIS_DAMAGED)
						s -= GetFeatureValue (f) / 2;
					if (GetFeatureStatus (f) == VIS_TYPES.VIS_DESTROYED)
						s -= GetFeatureValue (f);
				}
				if (s < 0)
					s = 0;
			}

			if (s != obj_data.status) {
				SetObjectiveStatus ((byte)s);
			}
#endif
            throw new NotImplementedException();
        }

        public void RepairFeature(int f)
        {
#if TODO
			VIS_TYPES cur;

			cur = GetFeatureStatus (f);
			if (cur == VIS_TYPES.VIS_DAMAGED || cur == VIS_TYPES.VIS_DESTROYED) {
				SetFeatureStatus (f, VIS_TYPES.VIS_REPAIRED);
				if (EntityDB.Falcon4ClassTable [GetFeatureID (f)].vuClassData.classInfo_ [(int)VU_CLASS.VU_TYPE] == Classtable_Types.TYPE_RUNWAY) // (IS_RUNWAY)
					CleanupLinkedPersistantObjects (this, f, MapVisId (VIS_RWYPATCH), 1);
			}
			// KCK: Used to go from destroyed to damaged. This seems a little weird now that I think about it,
			// except for the case of runways... 
			//	if (cur == VIS_TYPES.VIS_DESTROYED)
			//		{
			//		SetFeatureStatus(f,VIS_TYPES.VIS_DAMAGED);
			//		if (EntityDB.Falcon4ClassTable[GetFeatureID(f)].vuClassData.classInfo_[(int)VU_CLASS.VU_TYPE] == Classtable_Types.TYPE_RUNWAY) // (IS_RUNWAY)
			//			CleanupLinkedPersistantObjects (this, f, VIS_RWYPATCH, 2);
			//		}
#endif
            throw new NotImplementedException();
        }

        public void RecalculateParent()
        {
#if TODO	
			Objective n, s = null, bp = null;
			Int32 i, j, d, bd = 9999;
			Team who, own;
			GridIndex x, y, X, Y;
			//	POData		pod=null;
			//	SOData		sod=null;

			if (!this)
				return;
			if (IsPrimary ())
                // Primary Objective, no parent
				SetObjectiveParent (VU_ID.FalconNullId);
			else if (IsSecondary ()) {
				VuListIterator myit = new VuListIterator (POList);
				// Secondary Objective. Find closest Primary (Modify distances by relations && scores)
				GetLocation (&x, &y);
				own = GetTeam ();
				n = GetFirstObjective (myit);
				while (n != null) {
					n.GetLocation (&X, &Y);
					who = n.GetTeam ();
					d = FloatToInt32 (Distance (X, Y, x, y));
					if (TeamStatic.GetTTRelations (who, own) == Allied)
						d /= 2;
					else if (TeamStatic.GetTTRelations (who, own) == War)
						d *= 2;
					else
						d = 9999;
					d = (int)((float)d / (n.GetObjectivePriority () / 100.0F));
					if (d < bd) {
						s = n;
						bd = d;
					}
					n = ObjectivStatic.GetNextObjective (myit);
				}
				if (s != null) {
					SetObjectiveParent (s.Id ());
					//			pod = GetPOData(s);
					//			if (pod)
					//				pod.children++;
				} else
					SetObjectiveParent (VU_ID.FalconNullId);
				// Set frontline flags for us and our PO, if any.
				//		if (IsNearfront())
				//			{
				//			sod = GetSOData(this);
				//			if (sod)
				//				sod.flags |= GTMOBJ_FRONTLINE;
				//			if (pod)
				//				pod.flags |= GTMOBJ_FRONTLINE;
				//			}
			} else {
				// Everything else. Find best Secondary for parent
				i = 0;
				while (i < static_data.links) {
					n = GetNeighbor (i);
					if (n != null && n.IsSecondary ()) {
						SetObjectiveParent (n.Id ());
						// KCK WARNING: Am I using this?
						//				sod = GetSOData(n);
						//				if (sod)
						//					sod.children++;
						return;
					}
					i++;
				}
				i = 0;
				bd = 0;
				while (i < static_data.links) {
					n = GetNeighbor (i);
					j = 0;
					while (n != null && j < n.static_data.links) {
						s = n.GetNeighbor (j);
						if (s && s.IsSecondary () && s.GetObjectivePriority () > bd) {
							bp = s;
							bd = s.GetObjectivePriority ();
						}
						j++;
					}
					i++;
				}
				if (bp == null) {
					SetObjectiveParent (bp.Id ());
					// KCK WARNING: Am I using this?
					//			sod = GetSOData(bp);
					//			if (sod)
					//				sod.children++;
				} else
					SetObjectiveParent (VU_ID.FalconNullId);
			}
#endif
            throw new NotImplementedException();
        }

        public static int AwakeCampaignEntities = 0; // TODO this variable is defined as extern in Campaign.cpp (??)
    }

    // =======================
    // Transmitable flags
    // =======================
    [Flags]
    public enum O_FLAGS : uint
    {
        O_FRONTLINE = 0x1,
        O_SECONDLINE = 0x2,
        O_THIRDLINE = 0x4,
        O_B3 = 0x8,
        O_JAMMED = 0x10,
        O_BEACH = 0x20,
        O_B1 = 0x40,
        O_B2 = 0x80,
        O_MANUAL_SET = 0x100,
        O_MOUNTAIN_SITE = 0x200,
        O_SAM_SITE = 0x400,
        O_ARTILLERY_SITE = 0x800,
        O_AMBUSHCAP_SITE = 0x1000,
        O_BORDER_SITE = 0x2000,
        O_COMMANDO_SITE = 0x4000,
        O_FLAT_SITE = 0x8000,
        O_RADAR_SITE = 0x10000,
        O_NEED_REPAIR = 0x20000,
        O_EMPTY1 = 0x40000,
        O_EMPTY2 = 0x80000,
        O_ABANDONED = 0x100000,
        // 2002-02-13 ADDED BY MN for Sylvain's new Identify
        O_HAS_NCTR = 0x200000,
        O_IS_GCI = 0x400000
    }

    public static class ObjectivStatic
    {

        // =======================
        // Random public static als
        // =======================

        public static ObjectiveClass FindObjective(VU_ID id)
        {
            throw new NotImplementedException();
        }

        // ================================
        // Objective public static als
        // ================================

        // ================================
        // Inline functions
        // ================================

        // ---------------------------------------
        // public static al Function Declarations
        // ---------------------------------------

        public static ObjectiveClass NewObjective()
        {
            throw new NotImplementedException();
        }

        public static ObjectiveClass NewObjective(short tid, VU_BYTE[] stream)
        {
            throw new NotImplementedException();
        }

        public static ObjectiveClass NewObjective(short tid, VU_BYTE[] stream, int fromDisk)
        {
            throw new NotImplementedException();
        }

        public static int LoadBaseObjectives(string scenario)
        {
            throw new NotImplementedException();
        }

        public static int LoadObjectiveDeltas(string savefile)
        {
            throw new NotImplementedException();
        }

        public static void SaveBaseObjectives(string scenario)
        {
            throw new NotImplementedException();
        }

        public static void SaveObjectiveDeltas(string savefile)
        {
            throw new NotImplementedException();
        }

        public static ObjectiveClass GetObjectiveByID(int ID)
        {
            throw new NotImplementedException();
        }

        public static int BestRepairFeature(ObjectiveClass o, int[] hours)
        {
            throw new NotImplementedException();
        }

        public static int BestTargetFeature(ObjectiveClass o, byte[] targeted)
        {
            throw new NotImplementedException();
        }

        public static void RepairObjectives()
        {
            throw new NotImplementedException();
        }

        public static DamageDataType GetDamageType(ObjectiveClass o, int f)
        {
            throw new NotImplementedException();
        }

        public static F4PFList GetChildObjectives(ObjectiveClass o, int maxdist, int flags)
        {
            throw new NotImplementedException();
        }

        public static ObjectiveClass GetFirstObjective(VuListIterator l)
        {
            throw new NotImplementedException();
        }

        public static ObjectiveClass GetNextObjective(VuListIterator l)
        {
            throw new NotImplementedException();
        }

        public static ObjectiveClass GetFirstObjective(VuGridIterator l)
        {
            throw new NotImplementedException();
        }

        public static ObjectiveClass GetNextObjective(VuGridIterator l)
        {
            throw new NotImplementedException();
        }

        public static void CaptureObjective(ObjectiveClass co, Control who, Unit u = null)
        {
            throw new NotImplementedException();
        }

        public static int EncodeObjectiveDeltas(VU_BYTE[] stream, FalconSessionEntity owner)
        {
            throw new NotImplementedException();
        }

        public static int DecodeObjectiveDeltas(VU_BYTE[] stream, FalconSessionEntity owner)
        {
            throw new NotImplementedException();
        }

        public static int EvaluateKill(FalconDeathMessage dtm, SimBaseClass simShooter, CampBaseClass campShooter, SimBaseClass simTarget, CampBaseClass campTarget) //TODO This function is defined as extern in DeathMessage.cpp (??)
        {
            throw new NotImplementedException();
        }

    }

    public static class CampObjectiveTransmitDataTypeEncodingLE
    {
        public static void Encode(Stream stream, CampObjectiveTransmitDataType val)
        {
            throw new NotImplementedException();
        }
        public static void Decode(Stream stream, ref CampObjectiveTransmitDataType rst)
        {
            rst.last_repair = UInt32EncodingLE.Decode(stream);


            if (CampaignClass.gCampDataVersion > 1)
            {
                rst.obj_flags = (O_FLAGS)UInt32EncodingLE.Decode(stream);
            }
            else
            {
                rst.obj_flags = (O_FLAGS)UInt16EncodingLE.Decode(stream);
            }

            rst.supply = (byte)stream.ReadByte();
            rst.fuel = (byte)stream.ReadByte();
            rst.losses = (byte)stream.ReadByte();

            var numStatuses = (byte)stream.ReadByte();

            rst.fstatus = new byte[numStatuses];
            for (var i = 0; i < numStatuses; i++)
            {
                rst.fstatus[i] = (byte)stream.ReadByte();
            }
            rst.priority = (byte)stream.ReadByte();
        }
    }

    public static class CampObjectiveStaticDataTypeEncodingLE
    {
        public static void Encode(Stream stream, CampObjectiveStaticDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ref CampObjectiveStaticDataType rst)
        {
            rst.nameid = Int16EncodingLE.Decode(stream);
            rst.parent = new VU_ID();
            VU_IDEncodingLE.Decode(stream, rst.parent);
            rst.first_owner = (byte)stream.ReadByte();
            rst.links = (byte)stream.ReadByte();
        }
    }
    public static class CampObjectiveLinkDataTypeEncodingLE
    {
        public static void Encode(Stream stream, CampObjectiveLinkDataType val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, CampObjectiveLinkDataType rst)
        {
                rst.costs = new byte[(int) MoveType.MOVEMENT_TYPES];
                for (var j = 0; j < (int) MoveType.MOVEMENT_TYPES; j++)
                {
                    rst.costs[j] = (byte)stream.ReadByte();
                }

                var newId = new VU_ID();
                VU_IDEncodingLE.Decode(stream, rst.id);
        }
    }
    public static class ObjectiveClassEncodingLE
    {
        public static void Encode(Stream stream, ObjectiveClass val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ObjectiveClass rst)
        {
            CampObjectiveTransmitDataTypeEncodingLE.Decode(stream, ref rst.obj_data);
            CampObjectiveStaticDataTypeEncodingLE.Decode(stream, ref rst.static_data);


            if (rst.static_data.links > 0)
            {
                rst.link_data = new CampObjectiveLinkDataType[rst.static_data.links];
            }
            else
            {
                rst.link_data = null;
            }
            for (var i = 0; i < rst.static_data.links; i++)
            {
                rst.link_data[i] = new CampObjectiveLinkDataType();
                CampObjectiveLinkDataTypeEncodingLE.Decode(stream, rst.link_data[i]);
            }

            if (CampaignClass.gCampDataVersion >= 20)
            {
                var hasRadarData = (byte)stream.ReadByte();

                if (hasRadarData > 0)
                {
                    rst.static_data.radar_data = new RadarRangeClass();
                    RadarRangeClassEncodingLE.Decode(stream, ref rst.static_data.radar_data);
                }
            //    else
            //    {
            //        detect_ratio = null;
            //    }
            }
            //else
            //{
            //    detect_ratio = null;
            //}
        }
        

        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class ObjectiveClassListEncodingLE
    {
        public static void Encode(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream)
        {
            short numObjectives;

            var expanded = Expand(stream, out numObjectives);
            if (expanded != null)
            {
                for (var i = 0; i < numObjectives; i++)
                {
                    var thisObjectiveType = UInt16EncodingLE.Decode(expanded);
                    ObjectiveClass thisObjective = new ObjectiveClass(thisObjectiveType);
                    ObjectiveClassEncodingLE.Decode(expanded, thisObjective);
                    //objectives[i] = thisObjective;
                }
            }
        }

        public static int Size
        {
            get { throw new NotImplementedException(); }
        }

        private static MemoryStream Expand(Stream compressed, out short numObjectives)
        {
            numObjectives = Int16EncodingLE.Decode(compressed);
            var uncompressedSize = Int32EncodingLE.Decode(compressed);
            var newSize = Int32EncodingLE.Decode(compressed);
            if (uncompressedSize == 0) return null;

            int remaining = (int)Math.Min(newSize, compressed.Length - compressed.Position);
            var actualCompressed = compressed.ReadBytes(remaining);
            byte[] uncompressed = LZSS.Decompress(actualCompressed, uncompressedSize);
            return new MemoryStream(uncompressed);
        }
    }
}
