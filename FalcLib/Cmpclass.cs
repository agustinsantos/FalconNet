using System;
using FalconNet.FalcLib;
using FalconNet.Common;
using System.IO;
using FalconNet.VU;
using VU_BYTE = System.Byte;
using GridIndex = System.Int16;
using DWORD = System.UInt32;
using FalconNet.CampaignBase;
using FalconNet.F4Common;
using FalconNet.Common.Encoding;
using System.Text;

namespace FalconNet.Campaign
{
    // Campaign flags
    [Flags]
    public enum CAMP_ENUM
    {
        CAMP_RUNNING = 0x0000001,					// Set if Campaign is running
        CAMP_LOADED = 0x0000002,					// Set if a valid campaign is in memory
        CAMP_SHUTDOWN_REQUEST = 0x0000004,					// Flagged when we want to shutdown
        CAMP_SUSPEND_REQUEST = 0x0000008,					// Flagged when we want to pause
        CAMP_SUSPENDED = 0x0000010,
        CAMP_THEATER_LOADED = 0x0000020,
        CAMP_SLAVE = 0x0000040,					// Set if another machine is doing timing
        CAMP_LIGHT = 0x0000080,					// Only build player bubble and handle VU messages
        CAMP_PRELOADED = 0x0000100,					// Preload has already been done
        CAMP_ONLINE = 0x0000200,					// This is a multi-player game
        CAMP_TACTICAL = 0x0000400,					// This is a tactical Engagement "Campaign"

        CAMP_GAME_FULL = 0x0010000,					// This game is full
        DF_MATCH_IN_PROGRESS = 0x0020000,					// This is a dogfight match game and it is in progress

        CAMP_TACTICAL_PAUSE = 0x0040000,		// This means a tacitcal engagement is being run - don't do any movement.
        CAMP_TACTICAL_EDIT = 0x0080000,		// This means a tacitcal engagement is being edited - don't do any movement.

        CAMP_NEED_ENTITIES = 0x00100000,
        CAMP_NEED_WEATHER = 0x00200000,
        CAMP_NEED_PERSIST = 0x00400000,
        CAMP_NEED_OBJ_DELTAS = 0x00800000,
        CAMP_NEED_PRELOAD = 0x01000000,
        CAMP_NEED_TEAM_DATA = 0x02000000,
        CAMP_NEED_UNIT_DATA = 0x04000000,
        CAMP_NEED_VC = 0x08000000,
        CAMP_NEED_PRIORITIES = 0x10000000,

        CAMP_NEED_MASK = 0x1FF00000,


        // wParam values for FM_JOIN_CAMPAIGN messages:
        JOIN_NOT_JOINING = 0,
        JOIN_PRELOAD_ONLY = 1,
        JOIN_REQUEST_ALL_DATA = 2,
        JOIN_CAMP_DATA_ONLY = 3,
    }

    public struct EventNode
    {
        public byte Team;
        public string eventText;
        public byte flags;
        public uint time;
        public short x;
        public short y;
    }
    public static class EventNodeEncodingLE
    {
        public static void Encode(Stream stream, EventNode val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ref EventNode rst)
        {
            rst.x = Int16EncodingLE.Decode(stream);
            rst.y = Int16EncodingLE.Decode(stream);
            rst.time = UInt32EncodingLE.Decode(stream);

            rst.flags = (byte)stream.ReadByte();
            rst.Team = (byte)stream.ReadByte();
            stream.ReadBytes(2); //align on int32 boundary
            //skip EventText pointer
            stream.ReadBytes(4);
            //skip UiEventNode pointer
            stream.ReadBytes(4);
            var eventTextSize = UInt16EncodingLE.Decode(stream);
            var eventText = StringFixedASCIIEncoding.Decode(stream, eventTextSize);
            rst.eventText = eventText;
        }

        public static int Size
        {
            get { return -1; }
        }
    }

    public struct SquadInfo
    {
        public short airbaseIcon;
        public string airbaseName;
        public byte country;
        public byte currentStrength;
        public short descriptionIndex;
        public VU_ID id;
        public short nameId;
        public byte specialty;
        public short squadronPath;
        public float x;
        public float y;
    }
    public static class SquadInfoEncodingLE
    {
        public static void Encode(Stream stream, SquadInfo val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ref SquadInfo rst)
        {
            rst.x = SingleEncodingLE.Decode(stream);
            rst.y = SingleEncodingLE.Decode(stream);

            rst.id = new VU_ID();
            VU_IDEncodingLE.Decode(stream, rst.id);
            rst.descriptionIndex = Int16EncodingLE.Decode(stream);
            rst.nameId = Int16EncodingLE.Decode(stream);
            rst.airbaseIcon = Int16EncodingLE.Decode(stream);
            rst.squadronPath = Int16EncodingLE.Decode(stream);
            rst.specialty = (byte)stream.ReadByte();
            rst.currentStrength = (byte)stream.ReadByte();
            rst.country = (byte)stream.ReadByte();
            rst.airbaseName = StringFixedASCIIEncoding.Decode(stream, 40);

            if (CampaignClass.gCampDataVersion < 42)
            {
                stream.ReadBytes(40);
                //skip additional string length for squad name in older versions that had 80 bytes
            }

            stream.ReadByte(); ; //align on int32 boundary
        }

        public static int Size
        {
            get { return -1; }
        }
    }
    public struct TeamBasicInfo
    {
        public byte teamColor;  // Colour
        public byte teamFlag;   // Flags
        public string teamMotto;// Motto
        public string teamName; // Name
    }

    public static class TeamBasicInfoEncodingLE
    {
        public static void Encode(Stream stream, TeamBasicInfo val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ref TeamBasicInfo rst)
        {
            rst.teamFlag = (byte)stream.ReadByte();
            rst.teamColor = (byte)stream.ReadByte();
            rst.teamName = StringFixedASCIIEncoding.Decode(stream, 20);
            rst.teamMotto = StringFixedASCIIEncoding.Decode(stream, 200);
        }

        public static int Size
        {
            get { return 2 + 20 + 200; }
        }
    }

    // =====================
    // Campaign Class
    // =====================

    // This stores all data we need to know how to start a particular scenario
    public class CampaignClass
    {
        public CampaignTime CurrentTime;
        public CampaignTime TE_StartTime;
        public CampaignTime TE_TimeLimit;
        public CampaignTime TimeOfDay;							// Time since last midnight
        public CampaignTime lastGroundTask;
        public CampaignTime lastAirTask;
        public CampaignTime lastNavalTask;
        public CampaignTime lastGroundPlan;
        public CampaignTime lastAirPlan;
        public CampaignTime lastNavalPlan;
        public CampaignTime lastResupply;
        public CampaignTime lastRepair;
        public CampaignTime lastReinforcement;
        public CampaignTime lastStatistic;
        public CampaignTime lastMajorEvent;
        public CampaignTime last_victory_time;
        public CAMP_ENUM Flags;
        public bool InMainUI;			// MN for weather UI
        public short TimeStamp;
        public short Group;								// Multiplayer Session ID
        public short GroundRatio;						// Our strength vs their strength (at start conditions)
        public short AirRatio;
        public short AirDefenseRatio;
        public short NavalRatio;
        public short Brief;								// Index to theater briefing
        public short Processor;
        public short TheaterSizeX;
        public short TheaterSizeY;
        public byte CurrentDay;
        public byte ActiveTeams;						// Number of participating teams
        public byte DayZero;         					// Marks start of war
        public byte EndgameResult;						// Is campaign over and who won?
        public byte Situation;							// How're things going?
        public byte EnemyAirExp;						// KCK: These two can probably be removed
        public byte EnemyADExp;
        public byte BullseyeName;						// Bullseye data for the theater
        public GridIndex BullseyeX;
        public GridIndex BullseyeY;
        public string TheaterName;		// Theater by name
        public string Scenario;			// Name of scenario (one of x origional scenarios)
        public string SaveFile;			// Name of save file (saved scenario)
        public string UIName;				// UI's description of the game
        public VuThread vuThread;							// Pointer to vu's thread structure
        public VU_ID PlayerSquadronID;					// VU_ID of player squadron (from last load)
        public CampUIEventElement StandardEventQueue;					// Queue of last few standard events
        public CampUIEventElement PriorityEventQueue;	// Queue of last few priority events

        // JPO - upgraded the next 3 to long, to allow bigger campaigns.
        public int CampMapSize;						// Size of currently allocated map data
        public int SamMapSize; 							// Size of currently allocated map data
        public int RadarMapSize;						// Size of currently allocated map data
        public byte[] CampMapData;						// Data for tiny occupation map.
        public byte[] SamMapData;							// Data for other maps
        public byte[] RadarMapData;
        public short LastIndexNum;						// Last index assigned to point into the name/patch data
        public short NumAvailSquadrons;					// Number of active selectable squadrons
        public SquadUIInfoClass CampaignSquadronData;				// The data array
        public short NumberOfValidTypes;					// Number of different types of aircraft we can fly
        public short[] ValidAircraftTypes;				// An array of dIndexs for squadron's we're allowed to join
        public MissionEvaluationClass MissionEvaluator;				// Mission evaluation class (for player)
        public FalconGameEntity CurrentGame;
        public VU_ID HotSpot;							// The most important primary currently
        public byte Tempo;								// How fast/dense we want things to happen
        public long CreatorIP;							// IP Address when started
        public long CreationTime;						// Time when started
        public long CreationRand;						// Random Number to pretty much guarantee we are unique to the universe
        public int TE_VictoryPoints;					// TE Points required to win
        public int TE_type;							// Type of tacitcal engagement
        public int TE_number_teams;					// Number of teams
        public int[] TE_number_aircraft = new int[8];				// Number of Aircraft per team
        public int[] TE_number_f16s = new int[8];					// Number of F16s per team
        public int TE_team;							// Number of teams
        public int[] TE_team_pts = new int[8];						// Points per team
        public int TE_flags;							// Various TE Flags

        //public byte[] team_flags = new byte[8];						// Flags
        //public byte[] team_colour = new byte[8];						// Colour
        //public string[] team_name = new string[8];					// Name
        //public string[] team_motto = new string[8];					// Motto
        public TeamBasicInfo[] TeamBasicInfo = new TeamBasicInfo[8];

        public bool campRunning;

        public DWORD LoopStarter()
        { throw new NotImplementedException(); }

        public CampaignClass()
        {
            Flags = 0;
            Processor = 1;
            MissionEvaluator = null;
            vuThread = null;
            EndgameResult = 0;
            //TODO StandardEventQueue = null;
            //TODO PriorityEventQueue = null;
            TheaterSizeX = 0;
            TheaterSizeY = 0;
            CampMapSize = SamMapSize = RadarMapSize = 0;
            CampMapData = null;
            SamMapData = null;
            RadarMapData = null;
            LastIndexNum = 0;
            NumAvailSquadrons = 0;
            //TODO CampaignSquadronData = null;
            NumberOfValidTypes = 0;
            ValidAircraftTypes = null;
            BullseyeName = 0;
            BullseyeX = 0;
            BullseyeY = 0;
            //TODO  CurrentGame.reset();
            last_victory_time = 0;

            CreatorIP = 0;
            CreationTime = 0;
            CreationRand = 0;

#if USE_SH_POOLS
    // Initialize our Smart Heap pools
    ObjectiveClass::InitializeStorage();
    BattalionClass::InitializeStorage();
    BrigadeClass::InitializeStorage();
    FlightClass::InitializeStorage();
    SquadronClass::InitializeStorage();
    PackageClass::InitializeStorage();
    TaskForceClass::InitializeStorage();
    runwayQueueStruct::InitializeStorage();
    //LoadoutStruct::InitializeStorage();
#endif
            //TODO SimPersistantClass.InitPersistantDatabase();
        }

        // public ~CampaignClass ();
        public void Reset()
        { throw new NotImplementedException(); }

        public F4THREADHANDLE InitCampaign(FalconGameType gametype, FalconGameEntity joingame)
        { throw new NotImplementedException(); }	// Don't call directly.

        public void SetCurrentTime(CampaignTime newTime)
        {
            CurrentTime = newTime;
        }

        public void SetTEStartTime(CampaignTime newTime)
        {
            TE_StartTime = newTime;
        }

        public void SetTETimeLimitTime(CampaignTime newTime)
        {
            TE_TimeLimit = newTime;
        }

        public CampaignTime GetCampaignTime()
        {
            return CurrentTime;
        }

        public CampaignTime GetTEStartTime()
        {
            return TE_StartTime;
        }

        public CampaignTime GetTETimeLimitTime()
        {
            return TE_TimeLimit;
        }

        public int GetActiveTeams()
        {
            return ActiveTeams;
        }

        public int GetCampaignDay()
        { throw new NotImplementedException(); }

        public int GetCurrentDay()
        { throw new NotImplementedException(); }

        public int GetMinutesSinceMidnight()
        { throw new NotImplementedException(); }

        public string GetTheaterName()
        {
            return TheaterName;
        }

        public int SetTheater(string name)
        { throw new NotImplementedException(); }

        public int SetScenario(string scenario)
        { throw new NotImplementedException(); }

        public string GetScenario()
        {
            return Scenario;
        }

        public string GetSavedName()
        {
            return SaveFile;
        }

        public void SetPlayerSquadronID(VU_ID id)
        {
            PlayerSquadronID = id;
        }

        public VU_ID GetPlayerSquadronID()
        {
            return PlayerSquadronID;
        }

        public void GetPlayerLocation(ref short x, ref short y)
        { throw new NotImplementedException(); }

        public void GotJoinData()
        { throw new NotImplementedException(); }

        public bool IsRunning()
        {
            return Flags.IsFlagSet(CAMP_ENUM.CAMP_RUNNING);
        }

        public bool IsLoaded()
        {
            return Flags.IsFlagSet(CAMP_ENUM.CAMP_LOADED);
        }

        public bool IsPreLoaded()
        {
            return Flags.IsFlagSet(CAMP_ENUM.CAMP_PRELOADED);
        }

        public bool IsSuspended()
        {
            return Flags.IsFlagSet(CAMP_ENUM.CAMP_SUSPENDED);
        }

        public bool IsMaster()
        { throw new NotImplementedException(); }

        public bool IsOnline()
        {
            return Flags.IsFlagSet(CAMP_ENUM.CAMP_ONLINE);
        }

        public void ProcessEvents()
        { throw new NotImplementedException(); }

        // Here's the UI Interface routines:
        public int LoadScenarioStats(FalconGameType type, string savefile)
        { throw new NotImplementedException(); }

        public int RequestScenarioStats(FalconGameEntity game)
        { throw new NotImplementedException(); }

        public void ClearCurrentPreload()
        { throw new NotImplementedException(); }

        public int NewCampaign(FalconGameType gametype, string scenario)
        { throw new NotImplementedException(); }			// Calls InitCampaign Internally
        public int LoadCampaign(FalconGameType gametype, string savefile)
        { throw new NotImplementedException(); }			// Calls InitCampaign Internally
        public int JoinCampaign(FalconGameType gametype, FalconGameEntity game)
        { throw new NotImplementedException(); }	// Calls InitCampaign Internally
        public int StartRemoteCampaign(FalconGameEntity game)
        { throw new NotImplementedException(); }

        public int SaveCampaign(FalconGameType type, string savefile, int save_scenario_data)
        { throw new NotImplementedException(); }

        public void EndCampaign()
        { throw new NotImplementedException(); }											// Ends Campaign Instance, halts thread.
        public void Suspend()
        { throw new NotImplementedException(); }

        public void Resume()
        { throw new NotImplementedException(); }

        public void SetOnlineStatus(int online)
        { throw new NotImplementedException(); }

        // PJW These functions are for my player matching stuff
        // should ONLY get set when someone does a "NEW" campaign
        public void SetCreatorIP(long ip)
        {
            CreatorIP = ip;
        }

        public void SetCreationTime(long time)
        {
            CreationTime = time;
        }

        public void SetCreationIter(long rand)
        {
            CreationRand = rand;
        }

        public long GetCreatorIP()
        {
            return (CreatorIP);
        }

        public long GetCreationTime()
        {
            return (CreationTime);
        }

        public long GetCreationIter()
        {
            return (CreationRand);
        }

        public void SetTEVictoryPoints(int points)
        {
            TE_VictoryPoints = points;
        }

        public long GetTEVictoryPoints()
        {
            return (TE_VictoryPoints);
        }

        // Bullseye data
        public void GetBullseyeLocation(ref GridIndex x, ref GridIndex y)
        { throw new NotImplementedException(); }

        public void GetBullseyeSimLocation(ref float x, ref float y)
        { throw new NotImplementedException(); }

        public byte GetBullseyeName()
        { throw new NotImplementedException(); }

        public void SetBullseye(byte nameid, GridIndex x, GridIndex y)
        { throw new NotImplementedException(); }

        public int BearingToBullseyeDeg(float x, float y)
        { throw new NotImplementedException(); }

        public int RangeToBullseyeFt(float x, float y)
        { throw new NotImplementedException(); }

        // Some serialization routines;
        public int LoadData(FileStream fp)
        { throw new NotImplementedException(); }

        public int SaveData(FileStream fp)
        { throw new NotImplementedException(); }

#if TODO
        public int Decode(ref VU_BYTE[] stream)
        { throw new NotImplementedException(); }

        public int Encode(ref VU_BYTE[] stream)
        { throw new NotImplementedException(); }

        public long SaveSize()
        { throw new NotImplementedException(); }
#endif

        // The Campaign Event manipulation functions
        public CampUIEventElement GetRecentEventlist()
        { throw new NotImplementedException(); }

        public CampUIEventElement GetRecentPriorityEventList()
        { throw new NotImplementedException(); }

        public void AddCampaignEvent(CampUIEventElement newEvent)
        { throw new NotImplementedException(); }

        public void DisposeEventLists()
        { throw new NotImplementedException(); }

        public void TrimCampUILists()
        { throw new NotImplementedException(); }

        // Map Stuff (small map)
        public byte[] MakeCampMap(int type)
        { throw new NotImplementedException(); }

        public void FreeCampMaps()
        { throw new NotImplementedException(); }

        // Squadron UI data stuff
        public void VerifySquadrons(int team)
        { throw new NotImplementedException(); }						// Rebuilds any changable squadron data
        public void FreeSquadronData()
        { throw new NotImplementedException(); }

        public void ReadValidAircraftTypes(string typefile)
        { throw new NotImplementedException(); }			// Reads text file with valid squadron types
        public int IsValidAircraftType(Unit u)
        { throw new NotImplementedException(); }						// Checks if passed Squadron is valid
        public int IsValidSquadron(int id)
        { throw new NotImplementedException(); }

        public void ChillTypes()
        { throw new NotImplementedException(); }


        // The one and only Campaign instance:
        public static CampaignClass TheCampaign;

        // Current data version
        public static int gCampDataVersion = 71;
    }

    // ======================
    // Time adjustment class
    // ======================

    public class TimeAdjustClass
    {
        public CampaignTime currentTime;
        public byte currentDay;
        public short compression;
        public static TimeAdjustClass TimeAdjust;
    }

    public static class CampaignClassEncodingLE
    {
        public const byte WP_HAVE_DEPTIME = 0x01;
        public const byte WP_HAVE_TARGET = 0x02;
        //private const int version = 73; //TODO fix that CampaignClass.gCampDataVersion
        private const int FLAGS_WIDENED_AT_VERSION = 73;
        private const int CAMP_NAME_SIZE = 40; // Size of name string arrays for scenario name and such

        public static void Encode(Stream stream, CampaignClass val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, CampaignClass rst)
        {
            var expanded = stream.ExpandLE();
            if (expanded != null)
            {
                rst.CurrentTime = UInt32EncodingLE.Decode(expanded);
                if (rst.CurrentTime == 0)
                    rst.CurrentTime = 1;
                if (CampaignClass.gCampDataVersion >= 48)
                {
                    rst.TE_StartTime = UInt32EncodingLE.Decode(expanded);
                    rst.TE_TimeLimit = UInt32EncodingLE.Decode(expanded);
                    if (CampaignClass.gCampDataVersion >= 49)
                    {
                        rst.TE_VictoryPoints = Int32EncodingLE.Decode(expanded);
                    }
                    else
                    {
                        rst.TE_VictoryPoints = 0;
                    }
                }
                else
                {
                    rst.TE_StartTime = rst.CurrentTime;
                    rst.TE_TimeLimit = rst.CurrentTime + (60 * 60 * 5 * 1000);
                    rst.TE_VictoryPoints = 0;
                }
                if (CampaignClass.gCampDataVersion >= 52)
                {
                    rst.TE_type = Int32EncodingLE.Decode(expanded);
                    rst.TE_number_teams = Int32EncodingLE.Decode(expanded);

                    for (var i = 0; i < 8; i++)
                    {
                        rst.TE_number_aircraft[i] = Int32EncodingLE.Decode(expanded);
                    }

                    for (var i = 0; i < 8; i++)
                    {
                        rst.TE_number_f16s[i] = Int32EncodingLE.Decode(expanded);
                    }

                    rst.TE_team = Int32EncodingLE.Decode(expanded);

                    for (var i = 0; i < 8; i++)
                    {
                        rst.TE_team_pts[i] = Int32EncodingLE.Decode(expanded);
                    }

                    rst.TE_flags = Int32EncodingLE.Decode(expanded);

                    for (var i = 0; i < 8; i++)
                    {
                        var info = new TeamBasicInfo();
                        TeamBasicInfoEncodingLE.Decode(expanded, ref info);
                        rst.TeamBasicInfo[i] = info;
                    }
                }
                else
                {
                    rst.TE_type = 0;
                    rst.TE_number_teams = 0;
                    rst.TE_number_aircraft = new int[8];
                    rst.TE_number_f16s = new int[8];
                    rst.TE_team = 0;
                    rst.TE_team_pts = new int[8];
                    rst.TE_flags = 0;
                }
                if (CampaignClass.gCampDataVersion >= 19)
                {
                    rst.lastMajorEvent = UInt32EncodingLE.Decode(expanded);
                }
                rst.lastResupply = UInt32EncodingLE.Decode(expanded);
                rst.lastRepair = UInt32EncodingLE.Decode(expanded);
                rst.lastReinforcement = UInt32EncodingLE.Decode(expanded);
                rst.TimeStamp = Int16EncodingLE.Decode(expanded);
                rst.Group = Int16EncodingLE.Decode(expanded);
                rst.GroundRatio = Int16EncodingLE.Decode(expanded);
                rst.AirRatio = Int16EncodingLE.Decode(expanded);
                rst.AirDefenseRatio = Int16EncodingLE.Decode(expanded);
                rst.NavalRatio = Int16EncodingLE.Decode(expanded);
                rst.Brief = Int16EncodingLE.Decode(expanded);
                rst.TheaterSizeX = Int16EncodingLE.Decode(expanded);
                rst.TheaterSizeY = Int16EncodingLE.Decode(expanded);
                rst.CurrentDay = (byte)expanded.ReadByte();
                rst.ActiveTeams = (byte)expanded.ReadByte();
                rst.DayZero = (byte)expanded.ReadByte();
                rst.EndgameResult = (byte)expanded.ReadByte();
                rst.Situation = (byte)expanded.ReadByte();
                rst.EnemyAirExp = (byte)expanded.ReadByte();
                rst.EnemyADExp = (byte)expanded.ReadByte();
                rst.BullseyeName = (byte)expanded.ReadByte();

                rst.BullseyeX = Int16EncodingLE.Decode(expanded);
                rst.BullseyeY = Int16EncodingLE.Decode(expanded);
                rst.TheaterName = StringFixedASCIIEncoding.Decode(expanded, CAMP_NAME_SIZE);
                rst.Scenario = StringFixedASCIIEncoding.Decode(expanded, CAMP_NAME_SIZE);
                rst.SaveFile = StringFixedASCIIEncoding.Decode(expanded, CAMP_NAME_SIZE);
                rst.UIName = StringFixedASCIIEncoding.Decode(expanded, CAMP_NAME_SIZE);
                rst.PlayerSquadronID = new VU_ID();
                VU_IDEncodingLE.Decode(expanded, rst.PlayerSquadronID);
#if TODO
                FalconLocalSession.SetPlayerSquadronID(rst.PlayerSquadronID);
                // Load the recent event queues
                DisposeEventLists();
#endif
                int NumRecentEventEntries = Int16EncodingLE.Decode(expanded);
                EventNode[] RecentEventEntries;
                if (NumRecentEventEntries > 0)
                {
                    RecentEventEntries = new EventNode[NumRecentEventEntries];
                    for (var i = 0; i < NumRecentEventEntries; i++)
                    {
                        var thisEvent = new EventNode(); //TODO CampUIEventElement();
                        EventNodeEncodingLE.Decode(expanded, ref thisEvent);
                        RecentEventEntries[i] = thisEvent;
#if TODO
                        if (!StandardEventQueue)
                        {
                            StandardEventQueue = thisEvent;
                            last = thisEvent;
                        }
                        else
                        {
                            last.next = thisEvent;
                            last = thisEvent;
                        }
#endif
                    }
                }


                int NumPriorityEventEntries = Int16EncodingLE.Decode(expanded);
                EventNode[] PriorityEventEntries;
                if (NumPriorityEventEntries > 0)
                {
                    PriorityEventEntries = new EventNode[NumPriorityEventEntries];
                    for (var i = 0; i < NumPriorityEventEntries; i++)
                    {
                        var thisEvent = new EventNode();
                        EventNodeEncodingLE.Decode(expanded, ref thisEvent);
                        PriorityEventEntries[i] = thisEvent;
#if TODO
                        if (!PriorityEventQueue)
                        {
                            PriorityEventQueue = thisEvent;
                            last = thisEvent;
                        }
                        else
                        {
                            if (last != null)
                                last.next = thisEvent;

                            last = thisEvent;
                        }
#endif
                    }
                }

                rst.CampMapSize = Int16EncodingLE.Decode(expanded);
                if (rst.CampMapSize > 0)
                {
                    rst.CampMapData = expanded.ReadBytes(rst.CampMapSize);
                }
                rst.LastIndexNum = Int16EncodingLE.Decode(expanded);

                int NumAvailableSquadrons = Int16EncodingLE.Decode(expanded);
                if (NumAvailableSquadrons > 0)
                {
                    SquadInfo[] squads = new SquadInfo[NumAvailableSquadrons];
                    for (var i = 0; i < NumAvailableSquadrons; i++)
                    {
                        var thisSquadInfo = new SquadInfo();
                        SquadInfoEncodingLE.Decode(expanded, ref thisSquadInfo);
                        squads[i] = thisSquadInfo;
                    }
                }

                if (CampaignClass.gCampDataVersion >= 31)
                {
                    rst.Tempo = (byte)expanded.ReadByte();
                }
                if (CampaignClass.gCampDataVersion >= 43)
                {
                    rst.CreatorIP = Int32EncodingLE.Decode(expanded);
                    rst.CreationTime = Int32EncodingLE.Decode(expanded);
                    rst.CreationRand = Int32EncodingLE.Decode(expanded);
                }
            }
        }

        public static int Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

}

