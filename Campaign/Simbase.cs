using System;
using System.Diagnostics;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using System.IO;
using VU_BYTE=System.Byte;
using VU_TIME = System.UInt64;
using Control = System.Byte;
using Team = System.Byte;
using FalconNet.CampaignBase;
namespace FalconNet.Campaign
{
// Flags used to convey special data
//NOTE top 16 bits are used for motion type
	[Flags]
    public enum SpecialFlags : int
	{
		RADAR_ON          = 0x1,
		ECM_ON             =0x2,
		AIR_BRAKES_OUT     =0x4,	// Should really be a position value, not a bit, but for now...
		//#define AVAILABLE        0x8
		OBJ_EXPLODING     = 0x10,
		OBJ_DEAD          = 0x20,
		OBJ_FIRING_GUN    = 0x40,
		ON_GROUND         = 0x80,
		SHOW_EXPLOSION    = 0x100,
		IN_PERSISTANT_LIST= 0x200,
		OBJ_DYING          =0x400,
		PILOT_EJECTED      =0x800,
		I_AM_A_TANKER     = 0x1000,
		IS_LASED          = 0x2000,
		HAS_MISSILES       =0x4000
		//#define AVAILABLE        0x8000
	}
	// Local flags
	[Flags]
    public enum LocalFlags:byte
	{
		OBJ_AWAKE         = 0x01,
		REMOVE_NEXT_FRAME  =0x02,
		NOT_LABELED        =0x04,
		IS_HIDDEN          =0x08
	}

// Status bits for Base Data
	[Flags]
    public enum STATUS_ENUM
	{
		VIS_TYPE_MASK = 0x07,
		
		STATUS_GEAR_DOWN = 0x1000,
		STATUS_EXT_LIGHTS = 0x2000,
		STATUS_EXT_NAVLIGHTS = 0x4000,
		STATUS_EXT_TAILSTROBE = 0x8000,
		STATUS_EXT_LANDINGLIGHT = 0x10000,
		STATUS_EXT_LIGHTMASK = (STATUS_EXT_LIGHTS |
	                            STATUS_EXT_NAVLIGHTS |
	                            STATUS_EXT_TAILSTROBE |
	                            STATUS_EXT_LANDINGLIGHT),
	}

	public class SimBaseSpecialData
	{
  
		public SimBaseSpecialData ()
		{throw new NotImplementedException();}
		//public ~SimBaseSpecialData();

		public float			rdrAz, rdrEl, rdrNominalRng;
		public float			rdrAzCenter, rdrElCenter;
		public float			rdrCycleTime;
		public SpecialFlags		flags;
		public int				status;
		public Control				country;
		public byte	afterburner_stage;

		// These should really move into SimMover or SimVeh, since they're only relevant there...
		public float			powerOutput;
		public byte	powerOutputNet;
// 2000-11-17 ADDED BY S.G. SO ENGINE HAS HEAT TEMP
		public float			engineHeatOutput;
		public byte	engineHeatOutputNet;
// END OF ADDED SECTION
		public VU_ID			ChaffID, FlareID;
	} 



// this class is used only for non-local entities
// for the most part, it will handle any special effects needed to
// run locally
	public class SimBaseNonLocalData
	{
	
		public int flags;
		//TODO	#define		NONLOCAL_GUNS_FIRING		0x00000001
		public float timer1;				// general purpose timer
		public float timer2;				// general purpose timer
		public float timer3;				// general purpose timer
		public float dx;					// use as a vector
		public float dy;
		public float dz;
		public DrawableTrail smokeTrail;	// smoke when guns are firing

		public SimBaseNonLocalData ()
		{throw new NotImplementedException();}
		//TODO public ~SimBaseNonLocalData();
	}

	public  class SimBaseClass :  FalconEntity
	{
  
		private CampBaseClass campaignObject;

		// Special Transmit Data
		// 2000-11-17 MADE PUBLIC BY S.G. SO specialData.powerOutput IS VISIBLE OUTSIDE SimBaseClass.
  
		public SimBaseSpecialData specialData;
		private int dirty_simbase;
		protected int callsignIdx;
		protected int slotNumber;

		protected virtual void InitData ()
		{throw new NotImplementedException();}

		protected virtual void Cleanup ()
		{throw new NotImplementedException();}
		// for damage, death and destruction....
		protected float strength;
		protected float maxStrength;
		protected float dyingTimer;
		protected float sfxTimer;
		protected long lastDamageTime;
		protected long explosionTimer;
		protected VU_ID lastShooter;				// KCK: replaces vu's lastShooter - Last person to hit this entity
		protected VU_TIME lastChaff, lastFlare;		// When will the most recently dropped counter-measures expire?
		protected long campaignFlags;
		protected LocalFlags localFlags;					// Don't transmit these, or else..
	
  
		public long timeOfDeath;
		public float pctStrength;
		public TransformMatrix dmx;
		public DrawableObject drawPointer;
		public char displayPriority;
		public ObjectGeometry platformAngles;
		public SimBaseNonLocalData nonLocalData;
	
		// For regenerating entities, we keep a pointer to our init data
		public SimInitDataClass reinitData;
	
		// this function can be called for entities which aren't necessarily
		// exec'd in a frame (ie ground units), but need to have their
		// gun tracers and (possibly other) weapons serviced
		public virtual void WeaponKeepAlive ()
		{
			return;
		}
	
		// Object grouping
		public virtual void JoinFlight ()
		{
		}
	
		//Functions
		public SimBaseClass (ushort type) : base(type,0)
		{throw new NotImplementedException();}
 
#if TODO
		public SimBaseClass (VU_BYTE[] stream) : base(FalconEntity.VU_LAST_ENTITY_TYPE,0)
		{throw new NotImplementedException();}
 
		public SimBaseClass (FileStream filePtr) : base(FalconEntity.VU_LAST_ENTITY_TYPE,0)
		{throw new NotImplementedException();}
#endif

		//TODO public virtual ~SimBaseClass();
		public int GetCallsignIdx ()
		{
			return callsignIdx;
		}

		public int GetSlot ()
		{
			return slotNumber;
		}

		public override Team GetTeam ()
		{throw new NotImplementedException();}

		public override Control GetCountry ()
		{
			return specialData.country;
		}

		public override short GetCampID ()
		{throw new NotImplementedException();}

		public override Domains GetDomain ()
		{
            return (Domains)(EntityType()).classInfo_[(int)Vu_CLASS.VU_DOMAIN];
		}

		public byte GetClass ()
		{
            return (EntityType()).classInfo_[(int)Vu_CLASS.VU_CLASS];
		}

        public byte GetFalconType()
		{
            return (EntityType()).classInfo_[(int)Vu_CLASS.VU_TYPE];
		}

		public byte GetSType ()
		{
            return (EntityType()).classInfo_[(int)Vu_CLASS.VU_STYPE];
		}

		public byte GetSPType ()
		{
            return (EntityType()).classInfo_[(int)Vu_CLASS.VU_SPTYPE];
		}

		public void ChangeOwner (VU_ID new_owner)
		{throw new NotImplementedException();}

		public override int Wake ()
		{throw new NotImplementedException();}

		public override int Sleep ()
		{throw new NotImplementedException();}

		public override MoveType GetMovementType ()
		{throw new NotImplementedException();}

		public virtual void MakeLocal ()
		{throw new NotImplementedException();}

		public virtual void MakeRemote ()
		{throw new NotImplementedException();}

		public override bool OnGround ()
		{
			return (specialData.flags.IsFlagSet(SpecialFlags.ON_GROUND) ? true : false);
		}

		public override bool IsExploding ()
		{
			return (specialData.flags.IsFlagSet(SpecialFlags.OBJ_EXPLODING) ? true : false);
		}

		public override bool IsDead ()
		{
			return (specialData.flags.IsFlagSet(SpecialFlags.OBJ_DEAD) ? true : false);
		}

		public bool IsDying ()
		{
			return (specialData.flags.IsFlagSet(SpecialFlags.OBJ_DYING) ? true : false);
		}

		public bool IsFiring ()
		{
			return (specialData.flags.IsFlagSet(SpecialFlags.OBJ_FIRING_GUN) ? true : false);
		}

		public bool IsAwake ()
		{
			return localFlags.IsFlagSet(LocalFlags.OBJ_AWAKE);
		}

		public bool  IsSetFlag (SpecialFlags flag)
		{
			return ((specialData.flags.IsFlagSet(flag)) ? true : false);
		}

		public bool  IsSetLocalFlag (LocalFlags flag)
		{
			return ((localFlags.IsFlagSet(flag)) ? true : false);
		}

		public void SetLocalFlag (LocalFlags flag)
		{
			localFlags |= flag;
		}

		public void UnSetLocalFlag (LocalFlags flag)
		{
			localFlags &= ~(flag);
		}

		public bool  IsSetCampaignFlag (int flag)
		{
			return ((campaignFlags.IsFlagSet(flag)) ? true : false);
		}

		public void SetCampaignFlag (int flag)
		{
			campaignFlags |= flag;
		}

		public void UnSetCampaignFlag (int flag)
		{
			campaignFlags &= ~(flag);
		}

		public bool IsSetRemoveFlag ()
		{
			return localFlags.IsFlagSet(LocalFlags.REMOVE_NEXT_FRAME);
		}

		public void SetRemoveFlag ()
		{throw new NotImplementedException();}

		public void SetRemoveSilentFlag ()
		{throw new NotImplementedException();}

		public void SetExploding (int p)
		{throw new NotImplementedException();}

		public void SetFiring (int p)
		{throw new NotImplementedException();}

		public void SetCallsign (int newCallsign)
		{
			callsignIdx = newCallsign;
		}

		public void SetSlot (int newSlot)
		{
			slotNumber = newSlot;
		}

		public int Status ()
		{
			return specialData.status;
		}

		// Seems like this stuff (and its support "sepecial data") could move into vehicle or aircraft???
		// LOCAL ONLY!
		public void SetChaffExpireTime (VU_TIME t)
		{
			Debug.Assert (IsLocal ());
			lastChaff = t;
		}

		public void SetFlareExpireTime (VU_TIME t)
		{
			Debug.Assert (IsLocal ());
			lastFlare = t;
		}

		public VU_TIME ChaffExpireTime ()
		{
			Debug.Assert (IsLocal ());
			return lastChaff;
		}

		public VU_TIME FlareExpireTime ()
		{
			Debug.Assert (IsLocal ());
			return lastFlare;
		}
		// REMOTE OR LOCAL
		public void SetNewestChaffID (VU_ID id)
		{throw new NotImplementedException();}

		public void SetNewestFlareID (VU_ID id)
		{throw new NotImplementedException();}

		public VU_ID	NewestChaffID ()
		{
			return specialData.ChaffID;
		}

		public VU_ID	NewestFlareID ()
		{
			return specialData.FlareID;
		}

		public override  bool IsSPJamming ()
		{throw new NotImplementedException();}

		public override  bool IsAreaJamming ()
		{throw new NotImplementedException();}

		public override  bool HasSPJamming ()
		{throw new NotImplementedException();}

		public override  bool HasAreaJamming ()
		{throw new NotImplementedException();}
	
		public CampBaseClass GetCampaignObject ()
		{
			return campaignObject;
		}

		public void SetCampaignObject (CampBaseClass ent)
		{throw new NotImplementedException();}

		public float PowerOutput ()
		{
			return specialData.powerOutput;
		}
		// 2000-11-17 ADDED BY S.G. IT RETURNS THE ENGINE TEMP INSTEAD. FOR NON AIRPLANE VEHICLE, RPM IS COPIED INTO ENGINE TEMP TO BE CONSISTENT
		//	float EngineTempOutput () {return specialData.engineHeatOutput;};
		// END OF ADDED SECTION
		public float RdrAz ()
		{
			return specialData.rdrAz;
		}

		public float RdrEl ()
		{
			return specialData.rdrEl;
		}

		public float RdrAzCenter ()
		{
			return specialData.rdrAzCenter;
		}

		public float RdrElCenter ()
		{
			return specialData.rdrElCenter;
		}

		public float RdrCycleTime ()
		{
			return specialData.rdrCycleTime;
		}

		public float RdrRng ()
		{
			return specialData.rdrNominalRng;
		}

		public float Strength ()
		{
			return strength;
		}

		public float MaxStrength ()
		{
			return maxStrength;
		}

		public int GetAfterburnerStage ()
		{throw new NotImplementedException();}

		public void SetFlag (int flag)
		{throw new NotImplementedException();}

		public void UnSetFlag (int flag)
		{throw new NotImplementedException();}

		public void SetFlagSilent (int flag)
		{throw new NotImplementedException();}

		public void UnSetFlagSilent (int flag)
		{throw new NotImplementedException();}

		public void SetCountry (int newSide)
		{throw new NotImplementedException();}

		public void SetLastChaff (long a)
		{throw new NotImplementedException();}

		public void SetLastFlare (long a)
		{throw new NotImplementedException();}

		public void SetStatus (int status)
		{throw new NotImplementedException();}

		public void SetStatusBit (int status)
		{throw new NotImplementedException();}

		public void ClearStatusBit (int status)
		{throw new NotImplementedException();}

		public virtual void SetVt (float vt)
		{throw new NotImplementedException();}

		public virtual void SetKias (float kias)
		{throw new NotImplementedException();}

		public void SetPowerOutput (float powerOutput)
		{throw new NotImplementedException();}

		public void SetRdrAz (float az)
		{throw new NotImplementedException();}

		public void SetRdrEl (float el)
		{throw new NotImplementedException();}

		public void SetRdrAzCenter (float az)
		{throw new NotImplementedException();}

		public void SetRdrElCenter (float el)
		{throw new NotImplementedException();}

		public void SetRdrCycleTime (float cycle)
		{throw new NotImplementedException();}

		public void SetRdrRng (float rng)
		{throw new NotImplementedException();}

		public void SetAfterburnerStage (int s)
		{throw new NotImplementedException();}

		public virtual void Init (SimInitDataClass initData)
		{throw new NotImplementedException();}

		public virtual bool Exec ()
		{
			return true;
		}

		public virtual void GetTransform (TransformMatrix m)
		{
		}

		public virtual void ApplyDamage (FalconDamageMessage damageMessage)
		{throw new NotImplementedException();}

		public virtual void ApplyDeathMessage (FalconDeathMessage deathMessage)
		{throw new NotImplementedException();}

		public virtual void SetDead (int i)
		{throw new NotImplementedException();}

		public virtual void MakePlayerVehicle ()
		{
		}

		public virtual void MakeNonPlayerVehicle ()
		{
		}

		public virtual void ConfigurePlayerAvionics ()
		{
		}

		public virtual void SetVuPosition ()
		{
		}

		public virtual void Regenerate (float f1, float f2, float f3, float f4)
		{
		}

		public VU_ID LastShooter ()
		{
			return lastShooter;
		}

		public void SetDying (int flag)
		{throw new NotImplementedException();}// { if (flag) specialData.flags |= OBJ_DYING; else specialData.flags &= ~OBJ_DYING;};
		public SimBaseSpecialData SpecialData ()
		{
			return specialData;
		}
	
		//a nice big number for ground vehicles and features
		public virtual float Mass ()
		{
			return 2500.0F;
		}
		// incoming missile notification
		// 2000-11-17 INSTEAD OF USING PREVIOUSLY UNUSED VARS IN AIRFRAME, I'LL CREATE MY OWN INSTEAD
		//	SimBaseClass *incomingMissile;
		//  PLUS SetIncomingMissile CAN CLEAR UP THE ARRAY IF true IS PASSED AS A SECOND PARAMETER (DEFAULTS TO false)
		//	void SetIncomingMissile ( SimBaseClass *missile );
		public SimBaseClass[] incomingMissile = new SimBaseClass[2];

		public void SetIncomingMissile (SimBaseClass missile, bool clearAll = false)
		{throw new NotImplementedException();}
		// 2000-11-17 ADDED BY S.G. INSTEAD OF USING oldrpm[5], I'LL CREATE MY OWN VARIABLE TO KEEP THE MONITORED INCOMING MISSLE RANGE AND THE KEEP EVADING TIMER
		public float		incomingMissileRange;
		public VU_TIME	incomingMissileEvadeTimer;
		// END OF ADDED SECTION
	
		// threat
		public FalconEntity threatObj;
		public int threatType;
		public enum THREAT
		{
			THREAT_NONE		=0,
			THREAT_SPIKE	=1,
			THREAT_MISSILE	=2,
			THREAT_GUN		=3
		}
		public void SetThreat (FalconEntity threat, int type)
		{throw new NotImplementedException();}
	
	
		// virtual function interface
		// serialization functions
#if TODO
		public override int SaveSize ()
		{throw new NotImplementedException();}

		public virtual int Save (VU_BYTE[] stream)
		{throw new NotImplementedException();}	// returns bytes written
		public override int Save (FileStream file)
		{throw new NotImplementedException();}		// returns bytes written
#endif

		// event handlers
		public virtual int Handle (VuFullUpdateEvent evnt)
		{throw new NotImplementedException();}

        public virtual int Handle(VuPositionUpdateEvent evnt)
		{throw new NotImplementedException();}

        public virtual int Handle(VuTransferEvent evnt)
		{throw new NotImplementedException();}

		public virtual VU_ERRCODE InsertionCallback ()
		{throw new NotImplementedException();}

		public virtual VU_ERRCODE RemovalCallback ()
		{throw new NotImplementedException();}
	
		// callsign and radio functions
		public int GetPilotVoiceId ()
		{throw new NotImplementedException();}

		public int GetFlightCallsign ()
		{throw new NotImplementedException();}
	
		// get focus point
		public virtual void GetFocusPoint (ref float x, ref float y, ref float z)
		{throw new NotImplementedException();}  

		// Dirty Functions
		public void MakeSimBaseDirty (Dirty_Sim_Base bits, Dirtyness score)
		{throw new NotImplementedException();}

        public virtual void WriteDirty(byte[] stream, ref int pos)
		{throw new NotImplementedException();}

        public virtual void ReadDirty(byte[] stream, ref int pos)
		{throw new NotImplementedException();}
	}

}
