using System;
using FalconNet.FalcLib;
using FalconNet.VU;
using VU_BYTE=System.Byte;
using GTM=FalconNet.Campaign.GroundTaskingManagerClass;
using ATM=FalconNet.Campaign.AirTaskingManagerClass;
using Team=System.Byte;
using FalconNet.Common;
using System.IO;

namespace FalconNet.Campaign
{
	// Country defines
	public enum CountryListEnum {
	       COUN_NONE = 0,
	       COUN_US,
	       COUN_SOUTH_KOREA,
	       COUN_JAPAN,
	       COUN_RUSSIA,
	       COUN_CHINA,
	       COUN_NORTH_KOREA,
	       COUN_GORN,
	       NUM_COUNS,
	};
	
	// Team data defines
	public enum TeamDataEnum {
	       TEAM_NEUTRAL = 0,
	       TEAM_1,
	       TEAM_2,
	       TEAM_3,
	       TEAM_4,
	       TEAM_5,
	       TEAM_6,
	       TEAM_7,
	       NUM_TEAMS,
	};
	/* TODO	
	#define MAX_TEAM_NAME_SIZE		20
	#define MAX_MOTTO_SIZE			200
	#define MAX_AIR_ACTIONS			14
	#define ACTION_TIME_MIN			30		// Time per action, in minutes
	TODO */
	
	// Team flags
	[Flags]
	public enum TeamFlagEnum : short {
	       TEAM_ACTIVE         = 0x01,	// Set if team is being used
	       TEAM_HASSATS        = 0x02,	// Has satelites
	       TEAM_UPDATED        = 0x04,	// We've gotten remote data for this team
	};
	
	// Rules of engagement query types, add as needed.
	public enum ROEEngagementQueryTypeEnum {
	       ROE_GROUND_FIRE     = 1,		// Fire on their ground troops?
	       ROE_GROUND_MOVE     = 2,		// Move through their territory?
	       ROE_GROUND_CAPTURE  = 3,		// Capture their territory?
	       ROE_AIR_ENGAGE      = 4,		// Maneuver against their aircraft?
	       ROE_AIR_FIRE        = 5,		// Fire at their aircraft? (any range)
	       ROE_AIR_FIRE_BVR    = 6,		// Fire at their aircraft BVR
	       ROE_AIR_OVERFLY     = 7,		// Fly over their territory?
	       ROE_AIR_ATTACK      = 8,		// Bomb/attack their territory?
	       ROE_AIR_USE_BASES   = 9,		// Can we based aircraft at their airbases?
	       ROE_NAVAL_FIRE      = 10,		// Attack their shipping?
	       ROE_NAVAL_MOVE      = 11,		// Move into/through their harbors/straights?
	       ROE_NAVAL_BOMBARD   = 12,		// Bombard them messily?
	};
	
	public enum ROEAllowedEnum {
	       ROE_ALLOWED         = 1,
	       ROE_NOT_ALLOWED     = 0,
	};
	

	
	// Ground Action types
	public enum GroundActionTypeEnum {
	       GACTION_DEFENSIVE      = 1,
	       GACTION_CONSOLIDATE    = 2,
	       GACTION_MINOROFFENSIVE = 3,
	       GACTION_OFFENSIVE      = 4,
	};
	
	// Air Action types
	public enum AirActionTypeEnum {
	       AACTION_NOTHING        = 0,
	       AACTION_DCA            = 1,
	       AACTION_OCA            = 2,
	       AACTION_INTERDICT      = 3,
	       AACTION_ATTRITION      = 4,
	       AACTION_CAS            = 5,
	};
	
	// Air tactic types
	public enum AirTacticTypeEnum {
	       TAT_DEFENSIVE          = 1,
	       TAT_OFFENSIVE          = 2,
	       TAT_INTERDICT          = 3,
	       TAT_ATTRITION          = 4,
	       TAT_CAS                = 5,		// CAS must always be last tactic
	};
	
	public enum table_of_equipment_manufacturers
	{
		toe_unknown,
		toe_chinese,
		toe_dprk,
		toe_rok,
		toe_soviet,
		toe_us
	};
	
	// =======================================
	// Priority tables
	// =======================================
#if TODO	
	extern byte	DefaultObjtypePriority[TAT_CAS][MAX_TGTTYPE];		// AI's suggested settings
	extern byte	DefaultUnittypePriority[TAT_CAS][MAX_UNITTYPE];		// 
	extern byte	DefaultMissionPriority[TAT_CAS][(int)MissionTypeEnum.AMIS_OTHER];		// 
	
	// =======================================
	// Local classes
	// =======================================
	
	class AirTaskingManagerClass;
	class GroundTaskingManagerClass;
	class NavalTaskingManagerClass;
	class CampBaseClass;
	typedef AirTaskingManagerClass* ATM;
	typedef GroundTaskingManagerClass* GTM;
	typedef NavalTaskingManagerClass* NTM;
	typedef CampBaseClass* CampEntity;
	Team GetTeam (Control country);
#endif	
	//#pragma pack(1) // place on byte boundary
	public struct TeamStatusType {
		ushort			airDefenseVehs;
		ushort			aircraft;
		ushort			groundVehs;
		ushort			ships;
		ushort			supply;
		ushort			fuel;
		ushort			airbases;
		byte			supplyLevel;							// Supply in terms of pecentage
		byte			fuelLevel;								// fuel in terms of pecentage
		};
		// TODO #pragma pack()
	
	// TODO #pragma pack(1) // place on byte boundary
	public struct TeamGndActionType {
		public CampaignTime	actionTime;								// When we start.
		public CampaignTime	actionTimeout;							// Our action will fail if not completed by this time
		public VU_ID			actionObjective;						// Primary objective this is all about
		public byte			actionType;
		public byte			actionTempo;							// How "active" we want the action to be
		public byte			actionPoints;							// Countdown of how much longer it will go on
		};
	// TODO #pragma pack()
	
	public struct TeamAirActionType {
		CampaignTime	actionStartTime;						// When we start.
		CampaignTime	actionStopTime;							// When we are supposed to be done by.
		VU_ID			actionObjective;						// Primary objective this is all about
		VU_ID			lastActionObjective;
		byte			actionType;
		};
	
	public class TeamDoctrine
	{
	   public int simFlags;
	   public float radarShootShootPct;
	   public float heatShootShootPct;
	
	   public TeamDoctrine () {simFlags = 0;}
	   public enum ShootEnum {
	      SimRadarShootShoot = 0x1,
	      SimHeatShootShoot  = 0x2
	   };
	   public int IsSet (int val) { return simFlags & val;}
	   public void Set (int val) {simFlags |= val;}
	   public void Clear (int val) {simFlags &= ~val;}
	   public float RadarShootShootPct () {return radarShootShootPct;}
	   public float HeatShootShootPct () {return heatShootShootPct;}
	};
	
	// =============================================
	// Team class
	// =============================================
	
	public class TeamClass :  FalconEntity
	{
		public const int MAX_BONUSES		=20;		// Number of SOs which can receive bonuses at one time
		public const int MAX_TGTTYPE		=36;
		public const int MAX_UNITTYPE		=20;
		
		private short				initiative;
		private ushort				supplyAvail;
		private ushort				fuelAvail;
		private ushort				replacementsAvail;
		private TeamStatusType		currentStats;
		private short				reinforcement;
		private byte[]				objtype_priority = new byte[MAX_TGTTYPE];		// base priority, based on target type (obj)
		private byte[]				unittype_priority = new byte[MAX_UNITTYPE];	// base priority for unit types (cmbt/AD)
		private byte[]				mission_priority = new byte[(int)MissionTypeEnum.AMIS_OTHER];		// bonus by mission type
		private TeamGndActionType	groundAction;						// Team's current ground action
		private TeamAirActionType	defensiveAirAction;					// Current defensive air action
		private TeamAirActionType	offensiveAirAction;					// Current offensive air action
		private int					dirty_team;
	
		
		public Team 			who;
		public Team				cteam;								// The team this relative is on (for quick reference)
		public TeamFlagEnum			flags;
		public string			name;
		public string			teamMotto;
		public byte[]			member = new byte[(int)CountryListEnum.NUM_COUNS];
		public short[]			stance = new short[(int)TeamDataEnum.NUM_TEAMS];
		public short			firstColonel;						// Pilot ID indexies for this country
		public short			firstCommander;
		public short			firstWingman;
		public short			lastWingman;
		public float			playerRating;						// Average player rating over last 5 player missions
		public CampaignTime		lastPlayerMission;					// Last player mission flown
		public byte				airExperience;						// Experience for aircraft (affects pilot's skill)
		public byte				airDefenseExperience;				// Experience for air defenses
		public byte				groundExperience;					// Experience for ground troops
		public byte				navalExperience;					// Experience for ships
		public TeamStatusType	startStats;
		public VU_ID[]			bonusObjs = new VU_ID[MAX_BONUSES];
		public CampaignTime[]	bonusTime = new CampaignTime[MAX_BONUSES];
		public byte[]			max_vehicle = new byte[4];						// Max vehicle slot by air/ground/airdefense/naval
		public ATM				atm;
		public GTM				gtm;
		public NTM				ntm;
		public byte				teamFlag;							// This team's flag (as in cloth)
		public byte				teamColor;							// This team's color [Index into color table for TE]
		public byte				equipment;							// What equipment table to use
		public TeamDoctrine		doctrine;
	
		
			// Constructors
		public TeamClass (int typeindex, Control owner) : base(typeindex)
		{throw new NotImplementedException();}
		
		public TeamClass (VU_BYTE[] stream) : base(VuEntity.VU_LAST_ENTITY_TYPE)
		{throw new NotImplementedException();}
		public TeamClass (FileStream file): base(VuEntity.VU_LAST_ENTITY_TYPE)
		{throw new NotImplementedException();}
		public TeamClass(byte[] bytes, ref int offset, int version)
			: base(bytes, ref offset, version)
        {
#if TODO
            id = new VU_ID();
            id.num_ = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            id.creator_ = BitConverter.ToUInt32(bytes, offset);
            offset += 4;

            entityType = BitConverter.ToUInt16(bytes, offset);
            offset += 2;

            who = bytes[offset];
            offset++;

            cteam = bytes[offset];
            offset++;

            flags = BitConverter.ToInt16(bytes, offset);
            offset += 2;

            if (version > 2)
            {
                member = new byte[8];
                for (var j = 0; j < member.Length; j++)
                {
                    member[j] = bytes[offset];
                    offset++;
                }
                stance = new short[8];
                for (var j = 0; j < stance.Length; j++)
                {
                    stance[j] = BitConverter.ToInt16(bytes, offset);
                    offset += 2;
                }
            }
            else
            {
                member = new byte[8];
                for (var j = 0; j < 7; j++)
                {
                    member[j] = bytes[offset];
                    offset++;
                }
                stance = new short[8];
                for (var j = 0; j < 7; j++)
                {
                    stance[j] = BitConverter.ToInt16(bytes, offset);
                    offset += 2;
                }
            }
            firstColonel = BitConverter.ToInt16(bytes, offset);
            offset += 2;
            firstCommander = BitConverter.ToInt16(bytes, offset);
            offset += 2;
            firstWingman = BitConverter.ToInt16(bytes, offset);
            offset += 2;
            lastWingman = BitConverter.ToInt16(bytes, offset);
            offset += 2;

            playerRating = 0.0F;
            lastPlayerMission = 0;

            if (version > 11)
            {
                airExperience = bytes[offset];
                offset++;

                airDefenseExperience = bytes[offset];
                offset++;

                groundExperience = bytes[offset];
                offset++;

                navalExperience = bytes[offset];
                offset++;
            }
            else
            {
                offset += 4;
                airExperience = 80;
                airDefenseExperience = 80;
                groundExperience = 80;
                navalExperience = 80;
            }
            initiative = BitConverter.ToInt16(bytes, offset);
            offset += 2;

            supplyAvail = BitConverter.ToUInt16(bytes, offset);
            offset += 2;

            fuelAvail = BitConverter.ToUInt16(bytes, offset);
            offset += 2;

            if (version > 53)
            {
                replacementsAvail = BitConverter.ToUInt16(bytes, offset);
                offset += 2;

                playerRating = BitConverter.ToSingle(bytes, offset);
                offset += 4;

                lastPlayerMission = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
            }
            else
            {
                replacementsAvail = 0;
                playerRating = 0.0f;
                lastPlayerMission = 0;
            }
            if (version < 40)
            {
                offset += 4;
            }

            currentStats = new TeamStatus();
            currentStats.airDefenseVehs = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            currentStats.aircraft = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            currentStats.groundVehs = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            currentStats.ships = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            currentStats.supply = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            currentStats.fuel = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            currentStats.airbases = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            currentStats.supplyLevel = bytes[offset];
            offset++;
            currentStats.fuelLevel = bytes[offset];
            offset++;

            startStats = new TeamStatus();
            startStats.airDefenseVehs = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            startStats.aircraft = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            startStats.groundVehs = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            startStats.ships = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            startStats.supply = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            startStats.fuel = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            startStats.airbases = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            startStats.supplyLevel = bytes[offset];
            offset++;
            startStats.fuelLevel = bytes[offset];
            offset++;

            reinforcement = BitConverter.ToInt16(bytes, offset);
            offset += 2;

            bonusObjs = new VU_ID[20];
            for (var j = 0; j < bonusObjs.Length; j++)
            {
                var thisId = new VU_ID();
                thisId.num_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                thisId.creator_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                bonusObjs[j] = thisId;
            }
            bonusTime = new uint[20];
            for (var j = 0; j < bonusTime.Length; j++)
            {
                bonusTime[j] = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
            }
            objtype_priority = new byte[36];
            for (var j = 0; j < objtype_priority.Length; j++)
            {
                objtype_priority[j] = bytes[offset];
                offset++;
            }
            unittype_priority = new byte[20];
            for (var j = 0; j < unittype_priority.Length; j++)
            {
                unittype_priority[j] = bytes[offset];
                offset++;
            }
            if (version < 30)
            {
                mission_priority = new byte[40];
                for (var j = 0; j < mission_priority.Length; j++)
                {
                    mission_priority[j] = bytes[offset];
                    offset++;
                }
            }
            else
            {
                mission_priority = new byte[41];
                for (var j = 0; j < mission_priority.Length; j++)
                {
                    mission_priority[j] = bytes[offset];
                    offset++;
                }
            }
            if (version < 34)
            {
                attackTime = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                offensiveLoss = bytes[offset];
                offset++;
            }

            max_vehicle = new byte[4];
            for (var j = 0; j < max_vehicle.Length; j++)
            {
                max_vehicle[j] = bytes[offset];
                offset++;
            }
            var nullLoc = 0;
            if (version > 4)
            {
                teamFlag = bytes[offset];
                offset++;
                if (version > 32)
                {
                    teamColor = bytes[offset];
                    offset++;
                }
                else
                {
                    teamColor = 0;
                }
                equipment = bytes[offset];
                offset++;

                name = Encoding.ASCII.GetString(bytes, offset, 20);
                offset += 20;
                nullLoc = name.IndexOf('\0');
                if (nullLoc > 0)
                {
                    name = name.Substring(0, nullLoc);
                }
                else
                {
                    name = String.Empty;
                }
            }

            if (version > 32)
            {
                teamMotto = Encoding.ASCII.GetString(bytes, offset, 200);
                offset += 200;
                nullLoc = teamMotto.IndexOf('\0');
                if (nullLoc > 0)
                {
                    teamMotto = teamMotto.Substring(0, nullLoc);
                }
                else
                {
                    teamMotto = string.Empty;
                }
            }
            else
            {
                teamMotto = string.Empty;
            }

            if (version > 33)
            {
                if (version > 50)
                {
                    groundAction = new TeamGndActionType();
                    groundAction.actionTime = BitConverter.ToUInt32(bytes, offset);
                    offset += 4;
                    groundAction.actionTimeout = BitConverter.ToUInt32(bytes, offset);
                    offset += 4;
                    groundAction.actionObjective = new VU_ID();
                    groundAction.actionObjective.num_ = BitConverter.ToUInt32(bytes, offset);
                    offset += 4;
                    groundAction.actionObjective.creator_ = BitConverter.ToUInt32(bytes, offset);
                    offset += 4;
                    groundAction.actionType = bytes[offset];
                    offset++;
                    groundAction.actionTempo = bytes[offset];
                    offset++;
                    groundAction.actionPoints = bytes[offset];
                    offset++;
                }
                else if (version > 41)
                {
                    offset += 27;
                    groundAction = new TeamGndActionType();
                }
                else
                {
                    offset += 23;
                    groundAction = new TeamGndActionType();
                }
                defensiveAirAction = new TeamAirActionType();
                defensiveAirAction.actionStartTime = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                defensiveAirAction.actionStopTime = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                defensiveAirAction.actionObjective = new VU_ID();
                defensiveAirAction.actionObjective.num_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                defensiveAirAction.actionObjective.creator_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                defensiveAirAction.lastActionObjective = new VU_ID();
                defensiveAirAction.lastActionObjective.num_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                defensiveAirAction.lastActionObjective.creator_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                defensiveAirAction.actionType = bytes[offset];
                offset++;
                offset += 3; //align on int32 boundary

                offensiveAirAction = new TeamAirActionType();
                offensiveAirAction.actionStartTime = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                offensiveAirAction.actionStopTime = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                offensiveAirAction.actionObjective = new VU_ID();
                offensiveAirAction.actionObjective.num_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                offensiveAirAction.actionObjective.creator_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                offensiveAirAction.lastActionObjective = new VU_ID();
                offensiveAirAction.lastActionObjective.num_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                offensiveAirAction.lastActionObjective.creator_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                offensiveAirAction.actionType = bytes[offset];
                offset++;
                offset += 3; //align on int32 boundary
            }
            else
            {
                groundAction = new TeamGndActionType();
                defensiveAirAction = new TeamAirActionType();
                offensiveAirAction = new TeamAirActionType();
            }

            if (version < 43)
            {
                groundAction.actionType = 2;
                supplyAvail = fuelAvail = 1000;
            }

            if (version < 51)
            {
                if (who == (byte) CountryListEnum.COUN_RUSSIA)
                {
                    firstColonel = 500;
                    firstCommander = 505;
                    firstWingman = 538;
                    lastWingman = 583;
                }
                else if (who == (byte) CountryListEnum.COUN_CHINA)
                {
                    firstColonel = 600;
                    firstCommander = 605;
                    firstWingman = 639;
                    lastWingman = 686;
                }
                else if (who == (byte) CountryListEnum.COUN_US)
                {
                    firstColonel = 0;
                    firstCommander = 20;
                    firstWingman = 149;
                    lastWingman = 373;
                }
                else
                {
                    firstColonel = 400;
                    firstCommander = 408;
                    firstWingman = 460;
                    lastWingman = 499;
                }
            }
#endif
        }
		
		// TODO public ~TeamClass ();
	
			// event handlers
		public virtual int Handle(VuEvent evnt)
		{throw new NotImplementedException();}
		public virtual int Handle(VuFullUpdateEvent evnt)
		{throw new NotImplementedException();}
		public virtual int Handle(VuPositionUpdateEvent evnt)
		{throw new NotImplementedException();}
		public virtual int Handle(VuEntityCollisionEvent evnt)
		{throw new NotImplementedException();}
		public virtual int Handle(VuTransferEvent evnt)
		{throw new NotImplementedException();}
		public virtual int Handle(VuSessionEvent evnt)
		{throw new NotImplementedException();}
		public virtual VU_ERRCODE InsertionCallback()
		{throw new NotImplementedException();}
		public virtual VU_ERRCODE RemovalCallback()
		{throw new NotImplementedException();}
		public override int Wake () {return 0;}
		public override int Sleep () {return 0;}
	
			// Access Functions
		public short GetInitiative () { return initiative; }
		public ushort GetSupplyAvail () { return supplyAvail; }
		public ushort GetFuelAvail () { return fuelAvail; }
		public ushort GetReplacementsAvail () { return replacementsAvail; }
		public short GetReinforcement () { return reinforcement; }
		public byte GetObjTypePriority (int type) { return objtype_priority[type]; }
		public byte GetUnitTypePriority (int type) { return unittype_priority[type]; }
		public byte GetMissionPriority (int type) { return mission_priority[type]; }
		public TeamStatusType GetCurrentStats () { return currentStats; }
		public TeamGndActionType GetGroundAction () { return groundAction; }
		public TeamAirActionType GetDefensiveAirAction () { return defensiveAirAction; }
		public TeamAirActionType GetOffensiveAirAction () { return offensiveAirAction; }
	
		public void SetInitiative (short i)
		{throw new NotImplementedException();}
		public void SetReinforcement (short r)
		{throw new NotImplementedException();}
		public byte[] SetAllObjTypePriority ()
		{throw new NotImplementedException();}
		public byte[] SetAllUnitTypePriority ()
		{throw new NotImplementedException();}
		public byte[] SetAllMissionPriority ()
		{throw new NotImplementedException();}
		public void SetObjTypePriority (int i, byte b)
		{throw new NotImplementedException();}
		public void SetUnitTypePriority (int i, byte b)
		{throw new NotImplementedException();}
		public void SetMissionPriority (int i, byte b)
		{throw new NotImplementedException();}
		public void SetSupplyAvail (int i )
		{throw new NotImplementedException();}
		public void SetFuelAvail (int i)
		{throw new NotImplementedException();}
		public void SetReplacementsAvail (int i)
		{throw new NotImplementedException();}
		public TeamStatusType SetCurrentStats ()
		{throw new NotImplementedException();}
		public TeamGndActionType SetGroundAction ()
		{throw new NotImplementedException();}
		public TeamAirActionType SetDefensiveAirAction ()
		{throw new NotImplementedException();}
		public TeamAirActionType SetOffensiveAirAction ()
		{throw new NotImplementedException();}
	
		public void AddInitiative (short s)
		{throw new NotImplementedException();}
		public void AddReinforcement (short s)
		{throw new NotImplementedException();}
	
			// Core functions
		public override int SaveSize ()
		{throw new NotImplementedException();}
		public virtual int Save (ref VU_BYTE[] stream)
		{throw new NotImplementedException();}
		public override int Save (FileStream file)
		{throw new NotImplementedException();}
	
		public void ReadDoctrineFile ()
		{throw new NotImplementedException();}
		public void ReadPriorityFile (int tactic)
		{throw new NotImplementedException();}
		public int CheckControl(GridIndex X, GridIndex Y)
		{throw new NotImplementedException();}
		public void SetActive(int act)
		{throw new NotImplementedException();}
		public void DumpHeader()
		{throw new NotImplementedException();}
		public void Dump()
		{throw new NotImplementedException();}
		public void DoFullUpdate (VuTargetEntity target)
		{throw new NotImplementedException();}
	
		//TODO public int CStance(Control country)				{ return stance[GetTeam(country)]; }
		public int TStance(Team team)						{ return stance[team]; }
		public int Initiative()						{ return initiative; }
		public bool HasSatelites()						{ return flags.IsFlagSet(TeamFlagEnum.TEAM_HASSATS); }
		public ATM GetATM()							{ return atm; }
		public GTM GetGTM()							{ return gtm; }
		public NTM GetNTM()							{ return ntm; }
		public void SetName (string newname)
		{throw new NotImplementedException();}
		public string GetName()
		{throw new NotImplementedException();}
		public void SetFlag (byte flag)					{ teamFlag = flag; }
		public void SetEquipment (byte e)					{ equipment = e; }
		public int GetFlag ()							{ return (int) teamFlag; }
		public void SetColor (byte color)					{ teamColor = color; }
		public int GetColor ()							{ return (int) teamColor; }
		public int GetEquipment ()						{ return (int) equipment; }
		public void SetMotto (string motto)
		{throw new NotImplementedException();}
		public string GetMotto ()
		{throw new NotImplementedException();}
		public TeamDoctrine GetDoctrine ()			{ return doctrine; }
	//		int OnOffensive()						{ return offensiveLoss; }
		public byte GetGroundActionType ()			{ return groundAction.actionType; }
		public void SelectGroundAction ()
		{throw new NotImplementedException();}
		public void SelectAirActions ()
		{throw new NotImplementedException();}
		public void SetGroundAction (TeamGndActionType action)
		{throw new NotImplementedException();}
	
		public override bool IsTeam ()					{ return true; }
	
			// Dirty Data
		public void MakeTeamDirty (Dirty_Team bits, Dirtyness score)
		{throw new NotImplementedException();}
		public void WriteDirty (ref byte [] stream)
		{throw new NotImplementedException();}
		public void ReadDirty (ref byte [] stream)
		{throw new NotImplementedException();}
	
	};
	
	public static class TeamStatic
	{
		
		public static TeamClass[]	TeamInfo = new TeamClass[(int)TeamDataEnum.NUM_TEAMS];
	
	// =============================================
	// Global functions
	// =============================================		
	
		public static void AddTeam (int p)
		{throw new NotImplementedException();}

		public static void RemoveTeam (int p)
		{throw new NotImplementedException();}
		
		public static  void AddNewTeams (RelType defaultStance)
		{throw new NotImplementedException();}
		
		public static  void RemoveTeams ()
		{throw new NotImplementedException();}
		
		public static  int LoadTeams (string scenario)
		{throw new NotImplementedException();}
		
		public static  int SaveTeams (string scenario)
		{throw new NotImplementedException();}
		
		public static  void LoadPriorityTables ()
		{throw new NotImplementedException();}
		
		public static Team GetTeam (Control country)
		{throw new NotImplementedException();}
		
		public static int GetCCRelations (Control who, Control with)
		{throw new NotImplementedException();}
		
		public static int GetCTRelations (Control who, Team with)
		{throw new NotImplementedException();}
		
		public static int GetTTRelations (Team who, Team with)
		{throw new NotImplementedException();}
		
		public static int GetTCRelations (Team who, Control with)
		{throw new NotImplementedException();}
		
		public static void SetTeam (Control country, int team)
		{throw new NotImplementedException();}
		
		public static void SetCTRelations (Control who, Team with, int rel)
		{throw new NotImplementedException();}
		
		public static void SetTTRelations (Team who, Team with, int rel)
		{throw new NotImplementedException();}
		
		public static int GetRoE(Team a, Team b, int type)
		{throw new NotImplementedException();}
		
		public static void TransferInitiative (Team from, Team to, int i)
		{throw new NotImplementedException();}
		
		public static float AirExperienceAdjustment(Team t)
		{throw new NotImplementedException();}
		
		public static float AirDefenseExperienceAdjustment(Team t)
		{throw new NotImplementedException();}
		
		public static float GroundExperienceAdjustment(Team t)
		{throw new NotImplementedException();}
		
		public static float NavalExperienceAdjustment(Team t)
		{throw new NotImplementedException();}
		
		public static float CombatBonus(Team who, VU_ID poid)
		{throw new NotImplementedException();}
		
		public static void ApplyPlayerInput(Team who, VU_ID poid, int rating)
		{throw new NotImplementedException();}
		
		public static Team GetEnemyTeam (Team who)
		{throw new NotImplementedException();}
		
		public static int GetPriority (MissionRequest mis)
		{throw new NotImplementedException();}
		
		public static void AddReinforcements (Team who, int inc)
		{throw new NotImplementedException();}
		
		public static void UpdateTeamStatistics ()
		{throw new NotImplementedException();}
		
		public static int NavalSuperiority (Team who)
		{throw new NotImplementedException();}
		
		public static int AirSuperiority (Team who)
		{throw new NotImplementedException();}
		
	}
}