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

        CAMP_NAME_SIZE = 40,						// Size of name string arrays for scenario name and such

        // wParam values for FM_JOIN_CAMPAIGN messages:
        JOIN_NOT_JOINING = 0,
        JOIN_PRELOAD_ONLY = 1,
        JOIN_REQUEST_ALL_DATA = 2,
        JOIN_CAMP_DATA_ONLY = 3,
    };

    public struct TeamBasicInfo
    {
        public byte teamColor;  // Colour
        public byte teamFlag;   // Flags
        public string teamMotto;// Motto
        public string teamName; // Name
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
        public long CampMapSize;						// Size of currently allocated map data
        public long SamMapSize; 							// Size of currently allocated map data
        public long RadarMapSize;						// Size of currently allocated map data
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
        public long TE_VictoryPoints;					// TE Points required to win
        public long TE_type;							// Type of tacitcal engagement
        public long TE_number_teams;					// Number of teams
        public long[] TE_number_aircraft = new long[8];				// Number of Aircraft per team
        public long[] TE_number_f16s = new long[8];					// Number of F16s per team
        public long TE_team;							// Number of teams
        public long[] TE_team_pts = new long[8];						// Points per team
        public long TE_flags;							// Various TE Flags

        //public byte[] team_flags = new byte[8];						// Flags
        //public byte[] team_colour = new byte[8];						// Colour
        //public string[] team_name = new string[8];					// Name
        //public string[] team_motto = new string[8];					// Motto
        public TeamBasicInfo[] TeamBasicInfo = new TeamBasicInfo[8];

        public bool campRunning;

        public DWORD LoopStarter()
        { throw new NotImplementedException(); }

        public CampaignClass()
        { throw new NotImplementedException(); }
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

        public void SetTEVictoryPoints(long points)
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

        public int Decode(ref VU_BYTE[] stream)
        { throw new NotImplementedException(); }

        public int Encode(ref VU_BYTE[] stream)
        { throw new NotImplementedException(); }

        public long SaveSize()
        { throw new NotImplementedException(); }

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
        public static int gCampDataVersion;
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
        private const int version = 71; //TODO fix that
        private const int FLAGS_WIDENED_AT_VERSION = 73;

        public static void Encode(Stream stream, CampaignClass val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream,ref CampaignClass rst)
        {
#if TODO
            CampaignClass rst = new CampaignClass();
            var expanded = buffer.ExpandLE();
            if (expanded != null) Decode(expanded);
            {
                var curByte = 0;
                var nullLoc = 0;
                rst.CurrentTime = UInt32EncodingLE.Decode(expanded);
                if (rst.CurrentTime == 0) 
                    rst.CurrentTime = 1;
                if (version >= 48)
                {
                    rst.TE_StartTime = UInt32EncodingLE.Decode(expanded);
                    rst.TE_TimeLimit = UInt32EncodingLE.Decode(expanded);
                    if (version >= 49)
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
                if (version >= 52)
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

                    nullLoc = 0;
                    for (var i = 0; i < 8; i++)
                    {
                        var info = new TeamBasicInfo();
                        info.teamFlag = bytes[curByte];
                        curByte++;
                        info.teamColor = bytes[curByte];
                        curByte++;
                        info.teamName = Encoding.ASCII.GetString(bytes, curByte, 20);
                        curByte += 20;
                        nullLoc = info.teamName.IndexOf('\0');
                        if (nullLoc > -1) info.teamName = info.teamName.Substring(0, nullLoc);
                        info.teamMotto = Encoding.ASCII.GetString(bytes, curByte, 200);
                        curByte += 200;
                        nullLoc = info.teamMotto.IndexOf('\0');
                        if (nullLoc > -1) info.teamMotto = info.teamMotto.Substring(0, nullLoc);
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
                if (version >= 19)
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
                rst.CurrentDay = bytes[curByte];
                curByte++;

                rst.ActiveTeams = bytes[curByte];
                curByte++;

                rst.DayZero = bytes[curByte];
                curByte++;

                rst.EndgameResult = bytes[curByte];
                curByte++;

                rst.Situation = bytes[curByte];
                curByte++;

                rst.EnemyAirExp = bytes[curByte];
                curByte++;

                rst.EnemyADExp = bytes[curByte];
                curByte++;

                rst.BullseyeName = bytes[curByte];
                curByte++;

                rst.BullseyeX = Int16EncodingLE.Decode(expanded);
                rst.BullseyeY = Int16EncodingLE.Decode(expanded);
                rst.TheaterName = Encoding.ASCII.GetString(bytes, curByte, 40);
                curByte += 40;
                nullLoc = rst.TheaterName.IndexOf('\0');
                if (nullLoc > -1) rst.TheaterName = rst.TheaterName.Substring(0, nullLoc);

                rst.Scenario = Encoding.ASCII.GetString(bytes, curByte, 40);
                curByte += 40;
                nullLoc = rst.Scenario.IndexOf('\0');
                if (nullLoc > -1) rst.Scenario = rst.Scenario.Substring(0, nullLoc);

                rst.SaveFile = Encoding.ASCII.GetString(bytes, curByte, 40);
                curByte += 40;
                nullLoc = rst.SaveFile.IndexOf('\0');
                if (nullLoc > -1) rst.SaveFile = rst.SaveFile.Substring(0, nullLoc);

                rst.UIName = Encoding.ASCII.GetString(bytes, curByte, 40);
                curByte += 40;
                nullLoc = rst.UIName.IndexOf('\0');
                if (nullLoc > -1) rst.UIName = rst.UIName.Substring(0, nullLoc);

                var squadronId = new VU_ID();
                squadronId.num_ = UInt32EncodingLE.Decode(expanded);
                curByte += 4;
                squadronId.creator_ = UInt32EncodingLE.Decode(expanded);
                curByte += 4;
                rst.PlayerSquadronID = squadronId;

                rst.NumRecentEventEntries = Int16EncodingLE.Decode(expanded);
                curByte += 2;
                if (rst.NumRecentEventEntries > 0)
                {
                    rst.RecentEventEntries = new EventNode[NumRecentEventEntries];
                    for (var i = 0; i < rst.NumRecentEventEntries; i++)
                    {
                        var thisNode = new EventNode();
                        thisNode.x = Int16EncodingLE.Decode(expanded);
                        thisNode.y = Int16EncodingLE.Decode(expanded);
                        thisNode.time = UInt32EncodingLE.Decode(expanded);

                        thisNode.flags = bytes[curByte];
                        curByte++;

                        thisNode.Team = bytes[curByte];
                        curByte++;

                        curByte += 2; //align on int32 boundary
                        //skip EventText pointer
                        curByte += 4;
                        //skip UiEventNode pointer
                        curByte += 4;
                        var eventTextSize = UInt16EncodingLE.Decode(expanded);
                        var eventText = Encoding.ASCII.GetString(bytes, curByte, eventTextSize);
                        curByte += eventTextSize;
                        nullLoc = eventText.IndexOf('\0');
                        if (nullLoc > -1) eventText = eventText.Substring(0, nullLoc);
                        thisNode.eventText = eventText;
                        RecentEventEntries[i] = thisNode;
                    }
                }


                NumPriorityEventEntries = Int16EncodingLE.Decode(expanded);
                curByte += 2;
                if (NumPriorityEventEntries > 0)
                {
                    PriorityEventEntries = new EventNode[NumPriorityEventEntries];
                    for (var i = 0; i < NumPriorityEventEntries; i++)
                    {
                        var thisNode = new EventNode();
                        thisNode.x = Int16EncodingLE.Decode(expanded);
                        thisNode.y = Int16EncodingLE.Decode(expanded);
                        thisNode.time = UInt32EncodingLE.Decode(expanded);
                        thisNode.flags = bytes[curByte];
                        curByte++;

                        thisNode.Team = bytes[curByte];
                        curByte++;

                        curByte += 2; //align on int32 boundary
                        //skip EventText pointer
                        curByte += 4;
                        //skip UiEventNode pointer
                        curByte += 4;

                        var eventTextSize = UInt16EncodingLE.Decode(expanded);
                        curByte += 2;
                        var eventText = Encoding.ASCII.GetString(bytes, curByte, eventTextSize);
                        curByte += eventTextSize;
                        nullLoc = eventText.IndexOf('\0');
                        if (nullLoc > -1) eventText = eventText.Substring(0, nullLoc);
                        thisNode.eventText = eventText;
                        PriorityEventEntries[i] = thisNode;
                    }
                }
                CampMapSize = Int16EncodingLE.Decode(expanded);
                curByte += 2;
                if (CampMapSize > 0)
                {
                    CampMap = new byte[CampMapSize];
                    Array.Copy(bytes, CampMap, CampMapSize);
                }
                curByte += CampMapSize;

                LastIndexNum = Int16EncodingLE.Decode(expanded);
                NumAvailableSquadrons = Int16EncodingLE.Decode(expanded);
                if (NumAvailableSquadrons > 0)
                {
                    SquadInfo = new SquadInfo[NumAvailableSquadrons];
                    for (var i = 0; i < NumAvailableSquadrons; i++)
                    {
                        var thisSquadInfo = new SquadInfo();
                        thisSquadInfo.x = SingleEncodingLE.Decode(expanded);
                        thisSquadInfo.y = SingleEncodingLE.Decode(expanded);

                        thisSquadInfo.id = VU_IDEncodingLE.Decode(expanded);
                        thisSquadInfo.descriptionIndex = Int16EncodingLE.Decode(expanded);
                        thisSquadInfo.nameId = Int16EncodingLE.Decode(expanded);
                        thisSquadInfo.airbaseIcon = Int16EncodingLE.Decode(expanded);
                        thisSquadInfo.squadronPath = Int16EncodingLE.Decode(expanded);
                        thisSquadInfo.specialty = bytes[curByte];
                        curByte++;

                        thisSquadInfo.currentStrength = bytes[curByte];
                        curByte++;

                        thisSquadInfo.country = bytes[curByte];
                        curByte++;

                        thisSquadInfo.airbaseName = Encoding.ASCII.GetString(bytes, curByte, 40);
                        nullLoc = thisSquadInfo.airbaseName.IndexOf('\0');
                        if (nullLoc > -1) thisSquadInfo.airbaseName = thisSquadInfo.airbaseName.Substring(0, nullLoc);
                        curByte += 40;

                        if (version < 42)
                        {
                            curByte += 40;
                            //skip additional string length for squad name in older versions that had 80 bytes
                        }

                        curByte++; //align on int32 boundary
                        rst.SquadInfo[i] = thisSquadInfo;
                    }
                }
                if (version >= 31)
                {
                    rst.Tempo = bytes[curByte];
                    curByte++;
                }
                if (version >= 43)
                {
                    rst.CreatorIP = Int32EncodingLE.Decode(expanded);
                    rst.CreationTime = Int32EncodingLE.Decode(expanded);
                    rst.CreationRand = Int32EncodingLE.Decode(expanded);
                }

            }
            return rst;
#endif
            throw new NotImplementedException();
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

