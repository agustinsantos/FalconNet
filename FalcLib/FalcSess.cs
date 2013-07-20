using System;
using System.IO;
using FalconNet.VU;
using FalconNet.Common;
using VU_BYTE = System.Byte;
using FalconNet.CampaignBase;
using FalconNet.F4Common;
namespace FalconNet.FalcLib
{
    public class FlightClass : CampBaseClass
    {
        // It is already defined in Campaign :-(
        public FlightClass(): base(0,0)
        { throw new NotImplementedException(); }
    }

    public class SquadronClass
    {
        // It is already defined in Campaign :-(
        public SquadronClass()
        { throw new NotImplementedException(); }
    }

    // ==========================================
    // Fly states
    // ==========================================
    public enum FLYSTATE
    {
        FLYSTATE_IN_UI = 0,					// Sitting in the UI
        FLYSTATE_LOADING = 1,					// Loading the sim data
        FLYSTATE_WAITING = 2,					// Waiting for other players
        FLYSTATE_FLYING = 3,					// Flying
        FLYSTATE_DEAD = 4					// We're now dead, waiting for respawn
    }

    public static class FalcSessStatic
    {
        // ==========================================
        // Other defines
        // ==========================================

        public const int FS_MAXBLK = 320;	// Maximum number of data blocks

        // ==========================================
        // public static s
        // ==========================================

        public static byte GetTeam(byte country)
        {
            throw new NotImplementedException();
        }

    }

    public class FalconSessionEntity : VuSessionEntity
    {

        public enum SESSIONENUM
        {
            _AIR_KILLS_ = 0,
            _GROUND_KILLS_,
            _NAVAL_KILLS_,
            _STATIC_KILLS_,
            _KILL_CATS_,

            _VS_AI_ = 1,
            _VS_HUMAN_ = 2,
        };


        private string name;
        private string callSign;
        private VU_ID playerSquadron;
        private VU_ID playerFlight;
        private VU_ID playerEntity;
        private SquadronClass playerSquadronPtr;
        private FlightClass playerFlightPtr;
        private VuEntity playerEntityPtr;
        private float AceFactor;			// Player Ace Factor
        private float initAceFactor;		// AceFactor at beginning of match
        private float bubbleRatio;		// This session's multiplier for the player bubble size
        private ushort[] kills = new ushort[(int)SESSIONENUM._KILL_CATS_]; // Player kills - can't keep in log book
        private ushort missions;			// Player missions flown
        private byte country;			// Country or Team player is on
        private byte aircraftNum;		// Which aircraft in a flight we're using (0-3)
        private byte pilotSlot;			// Which pilot slot we've been assigned to
        private byte flyState;			// What we're doing (sitting in UI, waiting to load, flying)
        private short reqCompression;		// Requested compression rate
        private ulong latency;			// Current latency estimate for this session
        private byte samples;			// # of samples in current latency calculation
        private byte rating;				// Player rating (averaged)
        private byte voiceID;			// Player's voice of choice

        private byte assignedAircraftNum;
        private byte assignedPilotSlot;
        private FlightClass assignedPlayerFlightPtr;
        private byte[] unitDataSendBuffer;		// Unit data the local session is sending to this session
        private short unitDataSendSet;
        private long unitDataSendSize;
        private byte[] unitDataReceiveBuffer;	// Unit data the local session is receiving from this session
        private short unitDataReceiveSet;
        private byte[] unitDataReceived = new byte[FalcSessStatic.FS_MAXBLK / 8];
        private byte[] objDataSendBuffer;		// Objective data the local session is sending to this session
        private short objDataSendSet;
        private long objDataSendSize;
        private byte[] objDataReceiveBuffer;	// Objective data the local session is receiving from this session
        private short objDataReceiveSet;
        private byte[] objDataReceived = new byte[FalcSessStatic.FS_MAXBLK / 8];


        // constructors & destructor
        public FalconSessionEntity(ulong domainMask, string callsign)
            : base(domainMask, callsign)
        {
            throw new NotImplementedException();
        }

#if TODO
		public FalconSessionEntity (VU_BYTE[] stream)
		{
			throw new NotImplementedException ();
		}

		public FalconSessionEntity (FileStream file)
		{
			throw new NotImplementedException ();
		}
#endif
        public virtual VU_ERRCODE InsertionCallback()
        {
            throw new NotImplementedException();
        }
        //TODO public virtual ~FalconSessionEntity();

        // encoders
        public virtual int SaveSize()
        {
            throw new NotImplementedException();
        }

        public virtual int Save(VU_BYTE[] stream)
        {
            throw new NotImplementedException();
        }

        public virtual int Save(FileStream file)
        {
            throw new NotImplementedException();
        }

        public void DoFullUpdate()
        {
            throw new NotImplementedException();
        }

        // accessors
        public string GetPlayerName()
        {
            return name;
        }

        public string GetPlayerCallsign()
        {
            return callSign;
        }

        public VU_ID GetPlayerSquadronID()
        {
            return playerSquadron;
        }

        public VU_ID GetPlayerFlightID()
        {
            return playerFlight;
        }

        public VU_ID GetPlayerEntityID()
        {
            return playerEntity;
        }

        public VuEntity GetPlayerEntity()
        {
            return playerEntityPtr;
        }

        public FlightClass GetPlayerFlight()
        {
            return playerFlightPtr;
        }

        public SquadronClass GetPlayerSquadron()
        {
            return playerSquadronPtr;
        }

        public float GetAceFactor()
        {
            return (AceFactor);
        }

        public float GetInitAceFactor()
        {
            return (initAceFactor);
        }

        public byte GetTeam()
        {
            throw new NotImplementedException();
        }

        public byte GetCountry()
        {
            return country;
        }

        public byte GetAircraftNum()
        {
            return aircraftNum;
        }

        public byte GetPilotSlot()
        {
            return pilotSlot;
        }

        public byte GetFlyState()
        {
            return flyState;
        }

        public short GetReqCompression()
        {
            return reqCompression;
        }

        public float GetBubbleRatio()
        {
            return bubbleRatio;
        }

        public ushort GetKill(ushort CAT)
        {
            if (CAT < (int)SESSIONENUM._KILL_CATS_)
                return (kills[CAT]);
            return (0);
        }

        public ushort GetMissions()
        {
            return missions;
        }

        public byte GetRating()
        {
            return rating;
        }

        public byte GetVoiceID()
        {
            return voiceID;
        }

        public FalconGameEntity GetGame()
        {
            return (FalconGameEntity)game_;
        }

        public byte GetAssignedAircraftNum()
        {
            return assignedAircraftNum;
        }

        public byte GetAssignedPilotSlot()
        {
            return assignedPilotSlot;
        }

        public FlightClass GetAssignedPlayerFlight()
        {
            return assignedPlayerFlightPtr;
        }

        // setters
        public void SetPlayerName(string pname)
        {
            throw new NotImplementedException();
        }

        public void SetPlayerCallsign(string pcallsign)
        {
            throw new NotImplementedException();
        }

        public void SetPlayerSquadron(SquadronClass ent)
        {
            throw new NotImplementedException();
        }

        public void SetPlayerSquadronID(VU_ID id)
        {
            throw new NotImplementedException();
        }

        public void SetPlayerFlight(FlightClass ent)
        {
            throw new NotImplementedException();
        }

        public void SetPlayerFlightID(VU_ID id)
        {
            throw new NotImplementedException();
        }

        public void SetPlayerEntity(VuEntity ent)
        {
            throw new NotImplementedException();
        }

        public void SetPlayerEntityID(VU_ID id)
        {
            throw new NotImplementedException();
        }

        public void UpdatePlayer()
        {
            throw new NotImplementedException();
        }

        public void SetAceFactor(float af)
        {
            throw new NotImplementedException();
        }

        public void SetInitAceFactor(float af)
        {
            throw new NotImplementedException();
        }

        public void SetCountry(byte c)
        {
            throw new NotImplementedException();
        }

        public void SetAircraftNum(byte an)
        {
            throw new NotImplementedException();
        }

        public void SetPilotSlot(byte ps)
        {
            throw new NotImplementedException();
        }

        public void SetFlyState(byte fs)
        {
            throw new NotImplementedException();
        }

        public void SetReqCompression(short rc)
        {
            throw new NotImplementedException();
        }

        public void SetVuStateAccess(byte state)
        {
#if TODO
			SetVuState (state);
#endif
        }

        public void SetBubbleRatio(float ratio)
        {
            bubbleRatio = ratio;
        }

        public void SetKill(ushort CAT, ushort kill)
        {
            if (CAT < (int)SESSIONENUM._KILL_CATS_)
                kills[CAT] = kill;
        }

        public void SetMissions(ushort count)
        {
            missions = count;
        }

        public void SetRating(byte newrat)
        {
            rating = newrat;
        }

        public void SetVoiceID(byte v)
        {
            voiceID = v;
        }

        public void SetAssignedAircraftNum(byte an)
        {
            throw new NotImplementedException();
        }

        public void SetAssignedPilotSlot(byte ps)
        {
            throw new NotImplementedException();
        }

        public void SetAssignedPlayerFlight(FlightClass ent)
        {
            throw new NotImplementedException();
        }

        // Calculate new ace factor
        public void SetAceFactorKill(float opponent)
        {
            throw new NotImplementedException();
        }

        public void SetAceFactorDeath(float opponent)
        {
            throw new NotImplementedException();
        }
        // queries
        public int InSessionBubble(FalconEntity ent, float bubble_multiplier = 1.0F)
        {
            throw new NotImplementedException();
        }

        // event Handlers
        //		virtual VU_ERRCODE Handle(VuEvent *event);
        public virtual VU_ERRCODE Handle(VuFullUpdateEvent evnt)
        {
            throw new NotImplementedException();
        }

        //		virtual VU_ERRCODE Handle(VuSessionEvent *event);

        // Some conversion equivalencies between vuLocalSession and FalconLocalSession
        public static FalconSessionEntity virtualFalconLocalSession()
        {
#if TODO
			return ((FalconSessionEntity)vuLocalSessionEntity);
#endif
            throw new NotImplementedException();
        }

        public static object FalconLocalSessionId()
        {
#if TODO
			return vuLocalSession;
#endif
            throw new NotImplementedException();
        }

        public static FalconSessionEntity FalconLocalGame
        {
            // #define FalconLocalGame (((FalconSessionEntity*)vuLocalSessionEntity).GetGame())
            get
            {
#if TODO
			return (((FalconSessionEntity)vuLocalSessionEntity).GetGame());
#endif
                throw new NotImplementedException();
            }
        }

        public static object VuLocalGame()
        {
#if TODO
			return (vuLocalSessionEntity.Game());
#endif
            throw new NotImplementedException();
        }

        public static object FalconLocalGameId()
        {
#if TODO
			return (vuLocalSessionEntity.GameId());
#endif
            throw new NotImplementedException();
        }

        //TODO THis function is not declared in the .h file ??
        public FalconGameType GetGameType()
        {
            throw new NotImplementedException();
        }
    }

}

