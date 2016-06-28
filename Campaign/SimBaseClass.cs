using System;
using FalconNet.Common;
using FalconNet.VU;
//using FalconNet.Sim;
using FalconNet.FalcLib;
using FalconNet.CampaignBase;
using FalconNet.F4Common;

namespace FalconNet.Campaign
{

	public class SimulationDriver
	{
		public const int MAX_IA_CAMP_UNIT = 0x10000;

		public SimulationDriver ()
		{
			throw new NotImplementedException ();
		}
		//TODO public ~SimulationDriver();

		public void Startup ()
		{
			throw new NotImplementedException ();
		}		// One time setup (at application start)
		public void Cleanup ()
		{
			throw new NotImplementedException ();
		}	// One time shutdown (at application exit)

		public void Exec ()
		{
			throw new NotImplementedException ();
		}		// The master thread loop -- runs from Startup to Cleanup
		public void Cycle ()
		{
			throw new NotImplementedException ();
		}		// One SIM cycle (could be multiple time steps)

		public void Enter ()
		{
			throw new NotImplementedException ();
		}		// Enter the SIM from the UI
		public void Exit ()
		{
			throw new NotImplementedException ();
		}		// Set up sim for exiting


		public bool InSim ()
		{
#if TODO
			return SimulationLoopControl.InSim ();
#endif
            throw new NotImplementedException();
        }

		public bool RunningInstantAction ()
		{
			return FalconSessionEntity.FalconLocalGame.GetGameType () == FalconGameType.game_InstantAction;
		}

		public bool RunningDogfight ()
		{
			return FalconSessionEntity.FalconLocalGame.GetGameType () == FalconGameType.game_Dogfight;
		}

		public bool RunningTactical ()
		{
			return FalconSessionEntity.FalconLocalGame.GetGameType () == FalconGameType.game_TacticalEngagement;
		}

		public bool RunningCampaign ()
		{
			return FalconSessionEntity.FalconLocalGame.GetGameType () == FalconGameType.game_Campaign;
		}

		public bool RunningCampaignOrTactical ()
		{
			return FalconSessionEntity.FalconLocalGame.GetGameType () == FalconGameType.game_Campaign ||
                   FalconSessionEntity.FalconLocalGame.GetGameType () == FalconGameType.game_TacticalEngagement;
		}

		public void NoPause ()
		{
			throw new NotImplementedException ();
		}	// Pause time in the sim
		public void TogglePause ()
		{
			throw new NotImplementedException ();
		}	// Pause time in the sim

		public void SetFrameDescription (int mSecPerFrame, int numMinorFrames)
		{
			throw new NotImplementedException ();
		}

		public void SetPlayerEntity (SimBaseClass newObject)
		{
			throw new NotImplementedException ();
		}

		public void UpdateIAStats (SimBaseClass oldEntity)
		{
			throw new NotImplementedException ();
		}

		public SimBaseClass FindNearestThreat (ref float bearing, ref float range, ref float altitude)
		{
			throw new NotImplementedException ();
		}

		public SimBaseClass FindNearestThreat (ref short x, ref short y, ref float alt)
		{
			throw new NotImplementedException ();
		}

#if TODO
		public SimBaseClass FindNearestThreat (AircraftClass aircraft, ref short x, ref short y, ref float alt)
		{
			throw new NotImplementedException ();
		}

		public SimBaseClass FindNearestEnemyPlane (AircraftClass aircraft, ref short x, ref short y, ref float alt)
		{
			throw new NotImplementedException ();
		}

		public CampBaseClass FindNearestCampThreat (AircraftClass aircraft, ref short x, ref short y, ref float alt)
		{
			throw new NotImplementedException ();
		}

		public CampBaseClass FindNearestCampEnemy (AircraftClass aircraft, ref short x, ref short y, ref float alt)
		{
			throw new NotImplementedException ();
		}
#endif

		public void UpdateRemoteData ()
		{
			throw new NotImplementedException ();
		}

		public SimBaseClass FindFac (SimBaseClass center)
		{
			throw new NotImplementedException ();
		}

		public FlightClass FindTanker (SimBaseClass center)
		{
			throw new NotImplementedException ();
		}

		public SimBaseClass FindATC (VU_ID desiredATC)
		{
			throw new NotImplementedException ();
		}

		public int MotionOn ()
		{
			return motionOn;
		}

		public void SetMotion (int newFlag)
		{
			motionOn = newFlag;
		}

		public int AVTROn ()
		{
			return avtrOn;
		}

		public void SetAVTR (int newFlag)
		{
			avtrOn = newFlag;
		}

		public void AddToFeatureList (VuEntity theObject)
		{
			throw new NotImplementedException ();
		}

		public void AddToObjectList (VuEntity theObject)
		{
			throw new NotImplementedException ();
		}

		public void AddToCampUnitList (VuEntity theObject)
		{
			throw new NotImplementedException ();
		}

		public void AddToCampFeatList (VuEntity theObject)
		{
			throw new NotImplementedException ();
		}

		public void AddToCombUnitList (VuEntity theObject)
		{
			throw new NotImplementedException ();
		}

		public void AddToCombFeatList (VuEntity theObject)
		{
			throw new NotImplementedException ();
		}

		public void RemoveFromFeatureList (VuEntity theObject)
		{
			throw new NotImplementedException ();
		}

		public void RemoveFromObjectList (VuEntity theObject)
		{
			throw new NotImplementedException ();
		}

		public void RemoveFromCampUnitList (VuEntity theObject)
		{
			throw new NotImplementedException ();
		}

		public void RemoveFromCampFeatList (VuEntity theObject)
		{
			throw new NotImplementedException ();
		}

		public void InitACMIRecord ()
		{
			throw new NotImplementedException ();
		}

		public void POVKludgeFunction (short povHatAngle)
		{
			throw new NotImplementedException ();
		}

		public void InitializeSimMemoryPools ()
		{
			throw new NotImplementedException ();
		}

		public void ReleaseSimMemoryPools ()
		{
			throw new NotImplementedException ();
		}

		public void ShrinkSimMemoryPools ()
		{
			throw new NotImplementedException ();
		}

		public void WakeCampaignFlight (int ctype, CampBaseClass baseEntity, TailInsertList flightList)
		{
			throw new NotImplementedException ();
		}

		public void WakeObject (SimBaseClass theObject)
		{
			throw new NotImplementedException ();
		}

		public void SleepCampaignFlight (TailInsertList flightList)
		{
			throw new NotImplementedException ();
		}

		public void SleepObject (SimBaseClass theObject)
		{
			throw new NotImplementedException ();
		}

		public void NotifyExit ()
		{
			doExit = true;
		}

		public void NotifyGraphicsExit ()
		{
			doGraphicsExit = true;
		}

#if TODO
		public AircraftClass playerEntity;
#endif
		public FalconPrivateOrderedList objectList;		// List of locally deaggregated sim vehicles
		public FalconPrivateOrderedList featureList;		// List of locally deaggregated sim features
		public FalconPrivateOrderedList campUnitList;		// List of nearby aggregated campaign units
		public FalconPrivateOrderedList campObjList;		// List of nearby aggregated campaign objectives
		public FalconPrivateOrderedList combinedList;		// List of everything nearby
		public FalconPrivateOrderedList combinedFeatureList;	// List of everything nearby
		public FalconPrivateList ObjsWithNoCampaignParentList;
		public FalconPrivateList facList;
		public VuFilteredList atcList;
		public VuFilteredList tankerList;
		public bool doFile;
		public int doEvent;
		public uint eventReadPointer;
		public ulong lastRealTime;

		// SCR:  These used to be local to the loop function, but moved
		// here when the loop got broken out into a cycle per function call.
		// Some or all of these may be unnecessary, but I'll keep them for now...

		private ulong last_elapsedTime;
		private int lastFlyState;
		private int curFlyState;
		private bool doExit;
		private bool doGraphicsExit;
		private VuThread vuThread;
		private int curIALevel;
		private string dataName;
		private int motionOn;
		private int avtrOn;
		private ulong nextATCTime;

		private void UpdateEntityLists ()
		{
			throw new NotImplementedException ();
		}

		private void ReaggregateAllFlights ()
		{
			throw new NotImplementedException ();
		}

		private SimBaseClass FindNearest (SimBaseClass center, VuLinkedList sourceList)
		{
			throw new NotImplementedException ();
		}

		private void UpdateATC ()
		{
			throw new NotImplementedException ();
		}

		public static SimulationDriver SimDriver;
	}
#if TODO 
	//DELETE ALL OF THIS...???
	public class SimBaseClass : FalconEntity
	{
		private CampBaseClass campaignObject;

		// Special Transmit Data
// 2000-11-17 MADE PUBLIC BY S.G. SO specialData.powerOutput IS VISIBLE OUTSIDE SimBaseClass.
		public	  SimBaseSpecialData specialData;
		private	int dirty_simbase;
		protected int callsignIdx;
		protected int slotNumber;

		protected virtual void InitData ()
		{
			throw new NotImplementedException ();
		}

		protected virtual void Cleanup ()
		{
			throw new NotImplementedException ();
		}
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
		public SimBaseClass (int type)
		{
			throw new NotImplementedException ();
		}

		public SimBaseClass (byte[] stream)
		{
			throw new NotImplementedException ();
		}

		public SimBaseClass (FileStream filePtr)
		{
			throw new NotImplementedException ();
		}
		// TODO  public virtual ~SimBaseClass();
		public int GetCallsignIdx ()
		{
			return callsignIdx;
		}

		public int GetSlot ()
		{
			return slotNumber;
		}

		public virtual byte GetTeam ()
		{
			throw new NotImplementedException ();
		}

		public virtual byte GetCountry ()
		{
			return (byte)specialData.country;
		}

		public virtual short GetCampID ()
		{
			throw new NotImplementedException ();
		}

		public byte GetDomain ()
		{
			return (EntityType ()).classInfo_ [(int)VU_CLASS.VU_DOMAIN];
		}

		public byte GetClass ()
		{
			return (EntityType ()).classInfo_ [(int)VU_CLASS.VU_CLASS];
		}

		public byte GetType ()
		{
			return (EntityType ()).classInfo_ [(int)VU_CLASS.VU_TYPE];
		}

		public byte GetSType ()
		{
			return (EntityType ()).classInfo_ [(int)VU_CLASS.VU_STYPE];
		}

		public byte GetSPType ()
		{
			return (EntityType ()).classInfo_ [VU_SPTYPE];
		}

		public void ChangeOwner (VU_ID new_owner)
		{
			throw new NotImplementedException ();
		}

		public virtual int Wake ()
		{
			throw new NotImplementedException ();
		}

		public virtual int Sleep ()
		{
			throw new NotImplementedException ();
		}

		public virtual MoveType GetMovementType ()
		{
			throw new NotImplementedException ();
		}

		public virtual void MakeLocal ()
		{
			throw new NotImplementedException ();
		}

		public virtual void MakeRemote ()
		{
			throw new NotImplementedException ();
		}

		public virtual int OnGround ()
		{
			return (specialData.flags & ON_GROUND ? true : false);
		}

		public virtual bool IsExploding ()
		{
			return (specialData.flags & OBJ_EXPLODING ? true : false);
		}

		public bool IsDead ()
		{
			return (specialData.flags & OBJ_DEAD ? true : false);
		}

		public bool IsDying ()
		{
			return (specialData.flags & OBJ_DYING ? true : false);
		}

		public bool IsFiring ()
		{
			return (specialData.flags & OBJ_FIRING_GUN ? true : false);
		}

		public bool IsAwake ()
		{
			return localFlags & OBJ_AWAKE;
		}

		public bool  IsSetFlag (int flag)
		{
			return ((specialData.flags & flag) ? true : false);
		}

		public bool  IsSetLocalFlag (int flag)
		{
			return ((localFlags & flag) ? true : false);
		}

		public void SetLocalFlag (int flag)
		{
			localFlags |= flag;
		}

		public void UnSetLocalFlag (int flag)
		{
			localFlags &= ~(flag);
		}

		public bool  IsSetCampaignFlag (int flag)
		{
			return ((campaignFlags & flag) ? true : false);
		}

		public void SetCampaignFlag (int flag)
		{
			campaignFlags |= flag;
		}

		public void UnSetCampaignFlag (int flag)
		{
			campaignFlags &= ~(flag);
		}

		public int IsSetRemoveFlag ()
		{
			return localFlags & REMOVE_NEXT_FRAME;
		}

		public void SetRemoveFlag ()
		{
			throw new NotImplementedException ();
		}

		public void SetRemoveSilentFlag ()
		{
			throw new NotImplementedException ();
		}

		public void SetExploding (int p)
		{
			throw new NotImplementedException ();
		}

		public void SetFiring (int p)
		{
			throw new NotImplementedException ();
		}

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
		{
			throw new NotImplementedException ();
		}

		public void SetNewestFlareID (VU_ID id)
		{
			throw new NotImplementedException ();
		}

		public VU_ID	NewestChaffID ()
		{
			return specialData.ChaffID;
		}

		public VU_ID	NewestFlareID ()
		{
			return specialData.FlareID;
		}

		public virtual int IsSPJamming ()
		{
			throw new NotImplementedException ();
		}

		public virtual int IsAreaJamming ()
		{
			throw new NotImplementedException ();
		}

		public virtual int HasSPJamming ()
		{
			throw new NotImplementedException ();
		}

		public virtual int HasAreaJamming ()
		{
			throw new NotImplementedException ();
		}
	
		public CampBaseClass GetCampaignObject ()
		{
			return campaignObject;
		}

		public void SetCampaignObject (CampBaseClass ent)
		{
			throw new NotImplementedException ();
		}

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
		{
			throw new NotImplementedException ();
		}

		public void SetFlag (int flag)
		{
			throw new NotImplementedException ();
		}

		public void UnSetFlag (int flag)
		{
			throw new NotImplementedException ();
		}

		public void SetFlagSilent (int flag)
		{
			throw new NotImplementedException ();
		}

		public void UnSetFlagSilent (int flag)
		{
			throw new NotImplementedException ();
		}

		public void SetCountry (int newSide)
		{
			throw new NotImplementedException ();
		}

		public void SetLastChaff (long a)
		{
			throw new NotImplementedException ();
		}

		public void SetLastFlare (long a)
		{
			throw new NotImplementedException ();
		}

		public void SetStatus (int status)
		{
			throw new NotImplementedException ();
		}

		public void SetStatusBit (int status)
		{
			throw new NotImplementedException ();
		}

		public void ClearStatusBit (int status)
		{
			throw new NotImplementedException ();
		}

		public virtual void SetVt (float vt)
		{
			throw new NotImplementedException ();
		}

		public virtual void SetKias (float kias)
		{
			throw new NotImplementedException ();
		}

		public void SetPowerOutput (float powerOutput)
		{
			throw new NotImplementedException ();
		}

		public void SetRdrAz (float az)
		{
			throw new NotImplementedException ();
		}

		public void SetRdrEl (float el)
		{
			throw new NotImplementedException ();
		}

		public void SetRdrAzCenter (float az)
		{
			throw new NotImplementedException ();
		}

		public void SetRdrElCenter (float el)
		{
			throw new NotImplementedException ();
		}

		public void SetRdrCycleTime (float cycle)
		{
			throw new NotImplementedException ();
		}

		public void SetRdrRng (float rng)
		{
			throw new NotImplementedException ();
		}

		public void SetAfterburnerStage (int s)
		{
			throw new NotImplementedException ();
		}

		public virtual void Init (SimInitDataClass initData)
		{
			throw new NotImplementedException ();
		}

		public virtual int Exec ()
		{
			return true;
		}

		public virtual void GetTransform (TransformMatrix p)
		{
		}

		public virtual void ApplyDamage (FalconDamageMessage damageMessage)
		{
			throw new NotImplementedException ();
		}

		public virtual void ApplyDeathMessage (FalconDeathMessage deathMessage)
		{
			throw new NotImplementedException ();
		}

		public virtual void SetDead (int p)
		{
			throw new NotImplementedException ();
		}

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

		public virtual void Regenerate (float p1, float p2, float p3, float p4)
		{
		}

		public VU_ID LastShooter ()
		{
			return lastShooter;
		}

		public void SetDying (int flag)
		{
			throw new NotImplementedException ();
		}// { if (flag) specialData.flags |= OBJ_DYING; else specialData.flags &= ~OBJ_DYING;};
		public SimBaseSpecialData SpecialData ()
		{
			return &specialData;
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

		public void SetIncomingMissile (SimBaseClass missile, bool clearAll = false);
// 2000-11-17 ADDED BY S.G. INSTEAD OF USING oldrpm[5], I'LL CREATE MY OWN VARIABLE TO KEEP THE MONITORED INCOMING MISSLE RANGE AND THE KEEP EVADING TIMER
		public float		incomingMissileRange;
		public VU_TIME	incomingMissileEvadeTimer;
// END OF ADDED SECTION
	
		// threat
		public FalconEntity threatObj;
		public ThreatEnum threatType;

		public void SetThreat (FalconEntity threat, ThreatEnum type)
		{
			throw new NotImplementedException ();
		}
	
	
		// virtual function interface
		// serialization functions
		public 	virtual int SaveSize ()
		{
			throw new NotImplementedException ();
		}

		public virtual int Save (byte[] stream)
		{
			throw new NotImplementedException ();
		}	// returns bytes written
		public virtual int Save (FileStream file)
		{
			throw new NotImplementedException ();
		}		// returns bytes written
	
		// event handlers
		public virtual int Handle (VuFullUpdateEvent evnt)
		{
			throw new NotImplementedException ();
		}

		public virtual int Handle (VuPositionUpdateEvent evnt)
		{
			throw new NotImplementedException ();
		}

		public virtual int Handle (VuTransferEvent evnt)
		{
			throw new NotImplementedException ();
		}

		public virtual VU_ERRCODE InsertionCallback ()
		{
			throw new NotImplementedException ();
		}

		public virtual VU_ERRCODE RemovalCallback ()
		{
			throw new NotImplementedException ();
		}
	
		// callsign and radio functions
		public int GetPilotVoiceId ()
		{
			throw new NotImplementedException ();
		}

		public int GetFlightCallsign ()
		{
			throw new NotImplementedException ();
		}
	
		// get focus point
		public virtual void GetFocusPoint (ref float x, ref float y, ref float z)
		{
			throw new NotImplementedException ();
		}  

		// Dirty Functions
		public void MakeSimBaseDirty (Dirty_Sim_Base bits, Dirtyness score)
		{
			throw new NotImplementedException ();
		}

		public void WriteDirty (byte[] stream)
		{
			throw new NotImplementedException ();
		}

		public void ReadDirty (byte[] stream)
		{
			throw new NotImplementedException ();
		}
	}	
#endif
}

