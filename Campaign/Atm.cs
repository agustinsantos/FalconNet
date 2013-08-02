using System;
using System.IO;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using VU_BYTE = System.Byte;
using Flight = FalconNet.Campaign.FlightClass;
using Squadron = FalconNet.Campaign.SquadronClass;
using WayPoint = FalconNet.CampaignBase.WayPointClass;
using Package = FalconNet.Campaign.PackageClass;
using Team = System.SByte;
using GridIndex = System.Int16;
using FalconNet.CampaignBase;
using F4PFList = FalconNet.FalcLib.FalconPrivateList;
using F4POList = FalconNet.FalcLib.FalconPrivateOrderedList;

namespace FalconNet.Campaign
{

    public class ATMAirbaseClass
    {

        public VU_ID id;
        public byte[] schedule = new byte[AtmStatic.ATM_MAX_CYCLES];
        public byte usage;
        public ATMAirbaseClass next;

        public ATMAirbaseClass()
        {
            id = VU_ID.FalconNullId;
            //TODO memset(schedule, 0, sizeof(byte) * AtmStatic.ATM_MAX_CYCLES);
            usage = 0;
            next = null;
        }
        public ATMAirbaseClass(CampBaseClass ent)
        {
            id = ent.Id();
            //TODO memset(schedule, 0, sizeof(byte) * AtmStatic.ATM_MAX_CYCLES);
            usage = 0;
            next = null;
        }
        public ATMAirbaseClass(ref VU_BYTE[] stream)
        { throw new NotImplementedException(); }
        public ATMAirbaseClass(FileStream file)
        { throw new NotImplementedException(); }
        //TODO public ~ATMAirbaseClass();
        public int Save(ref VU_BYTE[] stream)
        { throw new NotImplementedException(); }
        public int Save(FileStream file)
        { throw new NotImplementedException(); }
        public int Size()
        {
            // TODO return sizeof(VU_ID) + AtmStatic.ATM_MAX_CYCLES;
            throw new NotImplementedException();
        }
    }

    public class AirTaskingManagerClass : CampManagerClass
    {
        // Transmittable data
        public short flags;
        public short squadrons;							// Number of available friendly squadrons
        public short averageCAStrength;					// Rolling average CA strength of CA missions
        public short averageCAMissions;					// Average # of CA missions being flow per hour
        public byte currentCAMissions;					// # of CA missions planned so far during current cycle
        public byte sampleCycles;						// # of cycles we've averaged missions over
        // Anything below here doesn't get transmitted
        public int missionsToFill;						// Amount of missions above to actually task.
        public int missionsFilled;						// # actually filled to date
        public List awacsList;							// List of awacs/jstar locations
        public List tankerList;							// List of tanker track locations
        public List ecmList;							// List of standoff jammer locations
        public List requestList;						// List of mission requests yet to be processed.
        public List delayedList;						// List of mission requests already handled, but not filled
        public F4PFList squadronList;						// List of this team's squadrons
        public F4PFList packageList;						// List of all active packages
        public ATMAirbaseClass airbaseList;						// List of active airbases
        public byte supplyBase;
        public byte cycle;								// which planning block we're in.
        public CampaignTime scheduleTime;						// Last time we updated our blocks

        // constructors & serial functions
        public AirTaskingManagerClass(ushort type, Team t)
            : base(type, t)
        { throw new NotImplementedException(); }
#if TODO
        public AirTaskingManagerClass(ref VU_BYTE[] stream) :base(stream)
		{throw new NotImplementedException();}
		 public AirTaskingManagerClass(byte[] bytes, ref int offset, int version)
            : base(bytes, ref offset, version)
        {

            flags = BitConverter.ToInt16(bytes, offset);
            offset += 2;

            if (version >= 28)
            {
                if (version >= 63)
                {
                    averageCAStrength = BitConverter.ToInt16(bytes, offset);
                    offset += 2;
                }
                averageCAMissions = BitConverter.ToInt16(bytes, offset);
                offset += 2;
                sampleCycles = bytes[offset];
                offset++;
            }
            if (version < 63)
            {
                averageCAMissions = 500;
                averageCAStrength = 500;
                sampleCycles = 10;
            }
            numAirbases = bytes[offset];
            offset++;

            airbases = new ATMAirbase[numAirbases];
            for (var j = 0; j < numAirbases; j++)
            {
                airbases[j] = new ATMAirbase(bytes, ref offset, version);
            }

            cycle = bytes[offset];
            offset++;

            numMissionRequests = BitConverter.ToInt16(bytes, offset);
            offset += 2;

            missionRequests = new MissionRequest[numMissionRequests];
            if (version < 35)
            {
                for (var j = 0; j < numMissionRequests; j++)
                {
                    offset += 64;
                }
            }
            else
            {
                for (var j = 0; j < numMissionRequests; j++)
                {
                    var mis_request = new MissionRequest();

                    mis_request.requesterID = new VU_ID();
                    mis_request.requesterID.num_ = BitConverter.ToUInt32(bytes, offset);
                    offset += 4;
                    mis_request.requesterID.creator_ = BitConverter.ToUInt32(bytes, offset);
                    offset += 4;

                    mis_request.targetID = new VU_ID();
                    mis_request.targetID.num_ = BitConverter.ToUInt32(bytes, offset);
                    offset += 4;
                    mis_request.targetID.creator_ = BitConverter.ToUInt32(bytes, offset);
                    offset += 4;

                    mis_request.secondaryID = new VU_ID();
                    mis_request.secondaryID.num_ = BitConverter.ToUInt32(bytes, offset);
                    offset += 4;
                    mis_request.secondaryID.creator_ = BitConverter.ToUInt32(bytes, offset);
                    offset += 4;

                    mis_request.pakID = new VU_ID();
                    mis_request.pakID.num_ = BitConverter.ToUInt32(bytes, offset);
                    offset += 4;
                    mis_request.pakID.creator_ = BitConverter.ToUInt32(bytes, offset);
                    offset += 4;

                    mis_request.who = bytes[offset];
                    offset++;

                    mis_request.vs = bytes[offset];
                    offset++;

                    offset += 2; //align on int32 boundary

                    mis_request.tot = BitConverter.ToUInt32(bytes, offset);
                    offset += 4;

                    mis_request.tx = BitConverter.ToInt16(bytes, offset);
                    offset += 2;

                    mis_request.ty = BitConverter.ToInt16(bytes, offset);
                    offset += 2;

                    mis_request.flags = BitConverter.ToUInt32(bytes, offset);
                    offset += 4;

                    mis_request.caps = BitConverter.ToInt16(bytes, offset);
                    offset += 2;

                    mis_request.target_num = BitConverter.ToInt16(bytes, offset);
                    offset += 2;

                    mis_request.speed = BitConverter.ToInt16(bytes, offset);
                    offset += 2;

                    mis_request.match_strength = BitConverter.ToInt16(bytes, offset);
                    offset += 2;

                    mis_request.priority = BitConverter.ToInt16(bytes, offset);
                    offset += 2;

                    mis_request.tot_type = bytes[offset];
                    offset++;

                    mis_request.action_type = bytes[offset];
                    offset++;


                    mis_request.mission = bytes[offset];
                    offset++;

                    mis_request.aircraft = bytes[offset];
                    offset++;

                    mis_request.context = bytes[offset];
                    offset++;

                    mis_request.roe_check = bytes[offset];
                    offset++;

                    mis_request.delayed = bytes[offset];
                    offset++;

                    mis_request.start_block = bytes[offset];
                    offset++;

                    mis_request.final_block = bytes[offset];
                    offset++;

                    mis_request.slots = new byte[4];
                    for (var k = 0; k < 4; k++)
                    {
                        mis_request.slots[k] = bytes[offset];
                        offset++;
                    }

                    mis_request.min_to = bytes[offset];
                    offset++;

                    mis_request.max_to = bytes[offset];
                    offset++;

                    offset += 3; // align on int32 boundary
                    missionRequests[j] = mis_request;
                }
            }

        }
        public AirTaskingManagerClass(FileStream file):base(file)
		{throw new NotImplementedException();}
        //TODO public virtual ~AirTaskingManagerClass();
        public override int SaveSize()
		{throw new NotImplementedException();}
        public virtual int Save(ref VU_BYTE[] stream)
		{throw new NotImplementedException();}
        public override int Save(FileStream file)
		{throw new NotImplementedException();}
#endif
        // Required pure virtuals
        public override int Task()
        { throw new NotImplementedException(); }
        public override void DoCalculations()
        { throw new NotImplementedException(); }
        public override VU_ERRCODE Handle(VuFullUpdateEvent evnt)
        { throw new NotImplementedException(); }

        // core functions
        public int BuildPackage(Package pc, MissionRequestClass mis)
        { throw new NotImplementedException(); }
        public int BuildDivert(MissionRequestClass mis)
        { throw new NotImplementedException(); }
        public int BuildSpecificDivert(Flight flight)
        { throw new NotImplementedException(); }
        public void ProcessRequest(MissionRequestClass request)
        { throw new NotImplementedException(); }
        public Squadron FindBestAir(MissionRequestClass mis, GridIndex bx, GridIndex by)
        { throw new NotImplementedException(); }
        public Flight FindBestAirFlight(MissionRequestClass mis)
        { throw new NotImplementedException(); }
        public void SendATMMessage(VU_ID from, Team to, short msg, short d1, short d2, object d3, int flags)
        { throw new NotImplementedException(); }
        public int FindTakeoffSlot(VU_ID abid, WayPoint w)
        { throw new NotImplementedException(); }
        public void ScheduleAircraft(VU_ID abid, WayPoint wp, int aircraft)
        { throw new NotImplementedException(); }
        public void ZapAirbase(VU_ID abid)
        { throw new NotImplementedException(); }
        public void ZapSchedule(int rw, ATMAirbaseClass airbase, int tilblock)
        { throw new NotImplementedException(); }
        public ATMAirbaseClass FindATMAirbase(VU_ID abid)
        { throw new NotImplementedException(); }
        public ATMAirbaseClass AddToAirbaseList(CampBaseClass airbase)
        { throw new NotImplementedException(); }
        public int FindNearestActiveTanker(ref GridIndex x, ref GridIndex y, ref CampaignTime time)
        { throw new NotImplementedException(); }
        public int FindNearestActiveJammer(ref GridIndex x, ref GridIndex y, ref CampaignTime time)
        { throw new NotImplementedException(); }
    };
#if TODO
typedef AirTaskingManagerClass *AirTaskingManager;
typedef AirTaskingManagerClass *ATM;
#endif
    // ==========================================
    // Global functions
    // ==========================================

    public enum RequIntHint
    { // 2001-10-27 ADDED BY S.G. Tells the RequestIntercept function not to ignore anything
        RI_NORMAL, RI_HELP
    };

    public static class AtmStatic
    {
        // ==================================
        // Air Tasking Manager class
        // ==================================

        public const int ATM_STEALTH_AVAIL = 0x01;							// We've got stealth aircraft to use
        public const int ATM_NEW_PLANES = 0x04;							// We've got more planes to play with
        public const int ATM_NEW_REQUESTS = 0x08;							// We've got one or more new mission requests

        public const int ATM_MAX_CYCLES = 32;								// Bit size of a long

        // Our scheduling requires a bitwise array of 1 minute time blocks. We've got 8 bits, so full would = 0xFF
        // However, if our cycle is < 8 minutes long, we need modify the meaning of 'full'
        public const int ATM_CYCLE_FULL = 0x1F;							//	What a full schedule looks like (5 bits)

        public static void InitATM()
        { throw new NotImplementedException(); }

        public static void EndATM()
        { throw new NotImplementedException(); }

        public static int LoadMissionLists(string scenario)
        { throw new NotImplementedException(); }

        public static int SaveMissionLists(string scenario)
        { throw new NotImplementedException(); }

        public static int RequestSARMission(FlightClass flight)
        { throw new NotImplementedException(); }

        public static void RequestIntercept(FlightClass enemy, int who, RequIntHint hint = RequIntHint.RI_NORMAL)
        { throw new NotImplementedException(); }

        public static int TargetAllSites(ObjectiveClass po, int action, int team, CampaignTime startTime)
        { throw new NotImplementedException(); }

        //public static  void TargetAdditionalSites ();
    }
}

