using System;
using System.Diagnostics;
using FalconNet.Common;
using FalconNet.FalcLib;


namespace FalconNet.Campaign
{
// Flags used to convey special data
//NOTE top 16 bits are used for motion type
    [Flags]
    public enum SpecialFlags
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
    public enum LocalFlags
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
        STATUS_EXT_LIGHTMASK = (STATUS_EXT_LIGHTS|
	                            STATUS_EXT_NAVLIGHTS|
	                            STATUS_EXT_TAILSTROBE|
	                            STATUS_EXT_LANDINGLIGHT),
    }

public class SimBaseSpecialData
{
  
	public SimBaseSpecialData();
	//public ~SimBaseSpecialData();

	public float			rdrAz, rdrEl, rdrNominalRng;
	public float			rdrAzCenter, rdrElCenter;
	public float			rdrCycleTime;
	public int				flags;
	public int				status;
	public int				country;
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
	public DrawableTrail *smokeTrail;	// smoke when guns are firing

	public SimBaseNonLocalData();
	//TODO public ~SimBaseNonLocalData();
}

    public  class SimBaseClass :  FalconEntity
    {
  
	    private CampBaseClass *campaignObject;

	    // Special Transmit Data
    // 2000-11-17 MADE PUBLIC BY S.G. SO specialData.powerOutput IS VISIBLE OUTSIDE SimBaseClass.
  
	      public SimBaseSpecialData specialData;
  

	    private int dirty_simbase;
	
  
	    protected int callsignIdx;
	    protected int slotNumber;
	    protected virtual void InitData();
	    protected virtual void Cleanup();
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
	    protected byte localFlags;					// Don't transmit these, or else..
	
  
	    public long timeOfDeath;
	    public float pctStrength;
	    public TransformMatrix dmx;
	    public DrawableObject* drawPointer;
	    public char displayPriority;
	    public ObjectGeometry* platformAngles;
	
	    public SimBaseNonLocalData *nonLocalData;
	
	    // For regenerating entities, we keep a pointer to our init data
	    public SimInitDataClass *reinitData;
	
	    // this function can be called for entities which aren't necessarily
	    // exec'd in a frame (ie ground units), but need to have their
	    // gun tracers and (possibly other) weapons serviced
	    public virtual void WeaponKeepAlive( ) { return; };
	
	    // Object grouping
	    public virtual void JoinFlight () {};
	
	    //Functions
	    public SimBaseClass(int type);
	    public SimBaseClass(VU_BYTE** stream);
	    public SimBaseClass(FILE* filePtr);
	    public virtual ~SimBaseClass();
	    public int GetCallsignIdx () { return callsignIdx; };
	    public int GetSlot () { return slotNumber;};
	    public virtual byte GetTeam ();
	    public virtual byte GetCountry() { return (byte)specialData.country; };
	    public virtual short GetCampID ();
	    public byte GetDomain ()						{	return (EntityType())->classInfo_[VU_DOMAIN]; }
	    public byte GetClass ()						{	return (EntityType())->classInfo_[VU_CLASS]; }
	    public byte GetType ()						{	return (EntityType())->classInfo_[VU_TYPE]; }
	    public byte GetSType ()						{	return (EntityType())->classInfo_[VU_STYPE]; }
	    public byte GetSPType ()						{	return (EntityType())->classInfo_[VU_SPTYPE]; }
	    public void ChangeOwner (VU_ID new_owner);
	    public virtual int Wake ();
	    public virtual int Sleep ();
	    public virtual MoveType GetMovementType ();
	    public virtual void MakeLocal ();
	    public virtual void MakeRemote ();
	    public virtual int OnGround () { return (specialData.flags & ON_GROUND ? true : false);}
	    public virtual int IsExploding () { return (specialData.flags & OBJ_EXPLODING ? true : false);}
	    public int IsDead () { return (specialData.flags & OBJ_DEAD ? true : false);}
	    public int IsDying () { return (specialData.flags & OBJ_DYING ? true : false);}
	    public int IsFiring () { return (specialData.flags & OBJ_FIRING_GUN ? true : false);}
	    public int IsAwake () { return localFlags & OBJ_AWAKE; }
	    public int  IsSetFlag (int flag) {return ((specialData.flags & flag) ? true : false);}
	    public int  IsSetLocalFlag (int flag) {return ((localFlags & flag) ? true : false);}
	    public void SetLocalFlag (int flag) { localFlags |= flag; }
	    public void UnSetLocalFlag (int flag) { localFlags &= ~(flag); }
	    public int  IsSetCampaignFlag (int flag) {return ((campaignFlags & flag) ? true : false);}
	    public void SetCampaignFlag (int flag) {campaignFlags |= flag;}
	    public void UnSetCampaignFlag (int flag) {campaignFlags &= ~(flag);}
	    public int IsSetRemoveFlag () { return localFlags & REMOVE_NEXT_FRAME;}
	    public void SetRemoveFlag ();
	    public void SetRemoveSilentFlag ();
	    public void SetExploding (int p);
	    public void SetFiring (int p);
	    public void SetCallsign(int newCallsign) {callsignIdx = newCallsign;};
	    public void SetSlot(int newSlot) {slotNumber = newSlot;};
	    public int Status() {return specialData.status;};

	    // Seems like this stuff (and its support "sepecial data") could move into vehicle or aircraft???
	    // LOCAL ONLY!
	    public void SetChaffExpireTime (VU_TIME t)	{Debug.Assert(IsLocal()); lastChaff = t;}
	    public void SetFlareExpireTime (VU_TIME t)	{Debug.Assert(IsLocal()); lastFlare = t;}
	    public VU_TIME ChaffExpireTime ()			{Debug.Assert(IsLocal()); return lastChaff;}
	    public VU_TIME FlareExpireTime ()			{Debug.Assert(IsLocal()); return lastFlare;}
	    // REMOTE OR LOCAL
	    public void SetNewestChaffID(VU_ID id);
	    public void SetNewestFlareID(VU_ID id);

	    public VU_ID	NewestChaffID()			{return specialData.ChaffID;};
	    public VU_ID	NewestFlareID()			{return specialData.FlareID;};

	    public virtual int IsSPJamming ();
	    public virtual int IsAreaJamming ();
	    public virtual int HasSPJamming ();
	    public virtual int HasAreaJamming ();
	
	    public CampBaseClass GetCampaignObject () { return campaignObject; }
	    public void SetCampaignObject (CampBaseClass *ent);

	    public float PowerOutput () {return specialData.powerOutput;};
    // 2000-11-17 ADDED BY S.G. IT RETURNS THE ENGINE TEMP INSTEAD. FOR NON AIRPLANE VEHICLE, RPM IS COPIED INTO ENGINE TEMP TO BE CONSISTENT
    //	float EngineTempOutput () {return specialData.engineHeatOutput;};
    // END OF ADDED SECTION
	    public float RdrAz () {return specialData.rdrAz;};
	    public float RdrEl () {return specialData.rdrEl;};
	    public float RdrAzCenter () {return specialData.rdrAzCenter;};
	    public float RdrElCenter () {return specialData.rdrElCenter;};
	    public float RdrCycleTime () {return specialData.rdrCycleTime;};
	    public float RdrRng () {return specialData.rdrNominalRng;};
	    public float Strength () {return strength;};
	    public float MaxStrength () {return maxStrength;};
	
	    public int GetAfterburnerStage ();
	    public void SetFlag (int flag);
	    public void UnSetFlag (int flag);
	    public void SetFlagSilent (int flag);
	    public void UnSetFlagSilent (int flag);
	    public void SetCountry(int newSide);
	    public void SetLastChaff (long a);
	    public void SetLastFlare (long a);
	    public void SetStatus (int status);
	    public void SetStatusBit (int status);
	    public void ClearStatusBit (int status);
	    public virtual void SetVt (float vt);
	    public virtual void SetKias (float kias);
	    public void SetPowerOutput (float powerOutput);
	    public void SetRdrAz (float az);
	    public void SetRdrEl (float el);
	    public void SetRdrAzCenter (float az);
	    public void SetRdrElCenter (float el);
	    public void SetRdrCycleTime (float cycle);
	    public void SetRdrRng (float rng);
	    public void SetAfterburnerStage (int s);
	    public virtual void Init (SimInitDataClass* initData);
	    public virtual int Exec () { return true; };
	    public virtual void GetTransform(TransformMatrix) {};
	    public virtual void ApplyDamage (FalconDamageMessage *damageMessage);
	    public virtual void ApplyDeathMessage (FalconDeathMessage *deathMessage);
	    public virtual void SetDead (int);
	    public virtual void MakePlayerVehicle () {}
	    public virtual void MakeNonPlayerVehicle () {}
	    public virtual void ConfigurePlayerAvionics() {}
	    public virtual void SetVuPosition () {};
	    public virtual void Regenerate (float, float, float, float) {};
	    public VU_ID LastShooter () { return lastShooter; };
	    public void SetDying (int flag);// { if (flag) specialData.flags |= OBJ_DYING; else specialData.flags &= ~OBJ_DYING;};
	    public SimBaseSpecialData* SpecialData() {return &specialData;}
	
	    //a nice big number for ground vehicles and features
	    public virtual float Mass()		{return 2500.0F;}
	    // incoming missile notification
    // 2000-11-17 INSTEAD OF USING PREVIOUSLY UNUSED VARS IN AIRFRAME, I'LL CREATE MY OWN INSTEAD
    //	SimBaseClass *incomingMissile;
    //  PLUS SetIncomingMissile CAN CLEAR UP THE ARRAY IF true IS PASSED AS A SECOND PARAMETER (DEFAULTS TO false)
    //	void SetIncomingMissile ( SimBaseClass *missile );
	    public SimBaseClass *incomingMissile[2];
	    public void SetIncomingMissile ( SimBaseClass *missile, bool clearAll = false);
    // 2000-11-17 ADDED BY S.G. INSTEAD OF USING oldrpm[5], I'LL CREATE MY OWN VARIABLE TO KEEP THE MONITORED INCOMING MISSLE RANGE AND THE KEEP EVADING TIMER
	    public float		incomingMissileRange;
	    public VU_TIME	incomingMissileEvadeTimer;
    // END OF ADDED SECTION
	
	    // threat
	    public FalconEntity *threatObj;
	    public int threatType;
	    #define	THREAT_NONE		0
	    #define	THREAT_SPIKE	1
	    #define	THREAT_MISSILE	2
	    #define	THREAT_GUN		3
	    public void SetThreat ( FalconEntity *threat, int type );
	
	
	    // virtual function interface
	    // serialization functions
	    public virtual int SaveSize();
	    public virtual int Save(VU_BYTE **stream);	// returns bytes written
	    public virtual int Save(FILE *file);		// returns bytes written
	
	    // event handlers
	    public virtual int Handle(VuFullUpdateevnt *evnt);
	    public virtual int Handle(VuPositionUpdateevnt *evnt);
	    public virtual int Handle(VuTransferevnt *evnt);
	    public virtual VU_ERRCODE InsertionCallback();
	    public virtual VU_ERRCODE RemovalCallback();
	
	    // callsign and radio functions
	    public int GetPilotVoiceId();
	    public int GetFlightCallsign();
	
	    // get focus point
	    public virtual void GetFocusPoint(BIG_SCALAR &x, BIG_SCALAR &y, BIG_SCALAR &z);  

	    // Dirty Functions
	    public void MakeSimBaseDirty (Dirty_Sim_Base bits, Dirtyness score);
	    public void WriteDirty (unsigned char **stream);
	    public void ReadDirty (unsigned char **stream);
    }

}
