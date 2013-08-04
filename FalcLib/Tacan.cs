using FalconNet.F4Common;
using FalconNet.VU;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using BOOL = System.Boolean;

namespace FalconNet.FalcLib
{
    public enum StationSet { X, Y, NumSets }; // Tacan Stations are numbered 1 - 126 and are grouped in two sets X and Y band.

    public enum Domain { AA, AG, NumDomains };

    public struct TacanCampStr
    {
        public int channel;
        public StationSet set;
        public short campaignID;
        public int callsign;
        public int range;
        public int tactype;
        public float ilsfreq;
    }

    public class LinkedCampStationStr
    {
        public LinkedCampStationStr p_next;
        public TacanCampStr p_station;
    }

    public class LinkedTacanVUStr
    {
        //TODO public LinkedTacanVUStr p_next;
        //TODO public LinkedTacanVUStr p_previous;
        public int channel;
        public StationSet set;
        public Domain domain;
        public VU_ID vuID;
        public short camp_id;
    }

    public class Tacan
    {
        public const int NUM_CHANNELS = 126;
        public const int MIN_TACAN_CHANNEL = 70; // superceeded by global
        public const int NUM_TACAN_FIELDS = 4;

        public static TacanList gTacanList;
        public const string gpTacanFileName = "sim\\sigdata\\tacan\\stations.dat";
    }

    public class TacanList
    {
        // List Sorted By CampId

        //---------------------------------------------------------------
        // Airbase File Data
        //---------------------------------------------------------------

        public Dictionary<short, TacanCampStr> mpCampList = new Dictionary<short, TacanCampStr>();
        private int mCampListTally; // Total Num of airbases

        //---------------------------------------------------------------
        // THE Dynamically Assigned Tanker/Carrier List
        //---------------------------------------------------------------
        private Dictionary<VU_ID, LinkedTacanVUStr> mpAssigned = new Dictionary<VU_ID, LinkedTacanVUStr>(); // All Tacans will be in Y band
        private Stack<LinkedTacanVUStr> mpRetired = new Stack<LinkedTacanVUStr>(); // All Tacans will be in Y band
        private int mLastUnused; // Valid Numbers 126 ... 70

        //---------------------------------------------------------------
        // THE Airbase TACAN LIST
        //---------------------------------------------------------------

        private Dictionary<VU_ID, LinkedTacanVUStr> mpTList = new Dictionary<VU_ID, LinkedTacanVUStr>();

        // TODO tengo que entender bien que significan las estructuras. Por lo que he visto creo que:
        // mpCampList contiene informacion estatica de aeropuertos. 
        // mpAssigned contiene informacion de tacan procedentes de elementos generados dinamicamente (Tanker/Carrier)
        // mpRetired se utiliza de apoyo para guardar elementos dinamicos que han sido retirados
        // mpTList no se muy bien para que sirve (quizas para guardar elementos de aeropuertos??)

        //---------------------------------------------------------------


        /// <summary>
        /// This function is responsible for inserting tacan channel
        /// elements into a sorted linked list.  Essentially this function
        /// performs an insertion sort upon the list.  The list is created
        /// in ascending order, sorted by channel first then by band.
        /// Legal values for channel range from 1-126.  Legal values for
        /// band are TacanList::X and TacanList::Y.
        /// Used by the class only during initialization, operates upon mpCampList
        /// </summary>
        private BOOL StoreStation(short airbaseId, int channel,
                                  StationSet band, int callsign,
                                  int range, int tactype, float ilsfreq)
        {

            if (channel < 1 || channel > 126)   // Note: the value of channel should be
            {
                log.Warn("invalid channel" + channel + "constrained to 1 - 126");
                return false;
            }

            if (band != StationSet.X && band != StationSet.Y)   // Channels are constrained to
            {
                //F4Assert(false); // X or Y band.
                log.Warn("invalid band X or Y band.");
                return false;
            }

            TacanCampStr p_staStr;
            p_staStr = new TacanCampStr(); // Create a new element
            p_staStr.channel = channel; // Fill the element
            p_staStr.set = band;
            p_staStr.campaignID = airbaseId;
            p_staStr.callsign = callsign;
            p_staStr.range = range;
            p_staStr.tactype = tactype;
            p_staStr.ilsfreq = ilsfreq;

            mpCampList.Add(airbaseId, p_staStr);
            return true;
        }


        private void InsertIntoTacanList(VU_ID vuId,
                                        short camp_id,
                                        int channel,
                                        StationSet set,
                                        Domain domain)
        {

            LinkedTacanVUStr p_tacanVUStr = new LinkedTacanVUStr(); // Create a new link
            p_tacanVUStr.channel = channel; // Stuff data into link
            p_tacanVUStr.set = set;
            p_tacanVUStr.domain = domain;
            p_tacanVUStr.vuID = vuId;
            p_tacanVUStr.camp_id = camp_id;
            mpTList.Add(vuId, p_tacanVUStr);
        }

        public int AssignChannel(VU_ID vuID, Domain domain, short camp_id)
        {
            int channel = -1;

            if (mpRetired.Count == 0)
            {

                Debug.Assert(mLastUnused >= F4Config.g_nMinTacanChannel);

                if (mLastUnused >= F4Config.g_nMinTacanChannel)   // Note: we have a total of 56 channels to work with.
                {
                    // If we need more than 56 at a time, then the campaign has gone wild.
                    // Ignore anything over 56 requests.
                    // MN this doesn't seem to be an issue anymore...(why 56 at all ??)


                    LinkedTacanVUStr pTacanStr = new LinkedTacanVUStr();

                    pTacanStr.channel = mLastUnused--; // Be sure to decrement the last unused
                    pTacanStr.set = StationSet.Y;
                    pTacanStr.domain = domain;
                    pTacanStr.vuID = vuID;
                    pTacanStr.camp_id = camp_id;

                    mpAssigned.Add(vuID, pTacanStr); // Place it into the assigned list
                    channel = pTacanStr.channel;
                }
            }
            else
            {
                LinkedTacanVUStr pTacanStr = mpRetired.Pop();
                pTacanStr.set = StationSet.Y;
                pTacanStr.domain = domain;
                pTacanStr.vuID = vuID;
                pTacanStr.camp_id = camp_id;

                mpAssigned.Add(vuID, pTacanStr); // Place it into the assigned list
                channel = pTacanStr.channel;
            }

            return channel;
        }

        // Given a particular campaign ID, find the
        // corresponding channel.  This function performs a binary
        // search upon a sorted array of tacan stations.  If an airbase
        // if found, true is returned
        //---------------------------------------------------------------

        public BOOL GetChannelFromCampID(out int channel, out StationSet band, short airbaseId)
        {
            channel = -1;
            band = StationSet.NumSets;
            if (airbaseId < 0)
            {
                //F4Assert(false); // Bad Id
                log.Warn("bad airbase id");
                return false;
            }
            TacanCampStr rst;

            if (!mpCampList.TryGetValue(airbaseId, out rst))
                return false;
            else
            {
                channel = rst.channel;
                band = rst.set;
                return true;
            }
        }

        public void AddTacan(CampBaseClass p_campEntity)
        {
            if (p_campEntity.IsObjective() &&
                p_campEntity.GetFalconType() == ClassTypes.TYPE_AIRBASE) // If inserting an airbase
            {

                int channel = 0;
                StationSet set;
                Domain domain = Domain.AG;
                if (GetChannelFromCampID(out channel, out set, p_campEntity.GetCampId())) // Get channel and band from mpCampList
                {
                    InsertIntoTacanList(p_campEntity.Id(), p_campEntity.GetCampID(), channel, set, domain);
                }
            }
#if TODO
            else if (p_campEntity.EntityType().classInfo_[(int)Vu_CLASS.VU_CLASS] == (byte)Classes.CLASS_UNIT &&
                     p_campEntity.EntityType().classInfo_[(int)Vu_CLASS.VU_TYPE] == (byte)ClassTypes.TYPE_FLIGHT &&
                     ((Unit)p_campEntity).GetUnitMission() == MissionTypeEnum.AMIS_TANKER)// If inserting a tanker
            {
                ((FlightClass)p_campEntity).tacan_channel = (byte)AssignChannel(p_campEntity.Id(), AA, p_campEntity.GetCampID()); // assign a unique channel
                ((FlightClass)p_campEntity).tacan_band = 'Y';
            }
            else if (p_campEntity.EntityType().classInfo_[(int)Vu_CLASS.VU_CLASS] == (byte)Classes.CLASS_UNIT &&
                     p_campEntity.EntityType().classInfo_[(int)Vu_CLASS.VU_TYPE] == (byte)ClassTypes.TYPE_TASKFORCE &&
                     p_campEntity.GetSType() == SubTypes.STYPE_UNIT_CARRIER) // If inserting a carrier
            {
                ((TaskForceClass)p_campEntity).tacan_channel = (uchar)AssignChannel(p_campEntity.Id(), AG, p_campEntity.GetCampID()); // assign a unique channel
                ((TaskForceClass)p_campEntity).tacan_band = 'Y';
            }
#endif
        }


        // The campaign has just destroyed the tanker flight or carrier.
        // The channel is no longer in use so retire it by placing the
        // pointer into the retired list.  The retired list is bascally
        // a free pool.
        //---------------------------------------------------------------
        private void RetireChannel(VU_ID vuID)
        {

            LinkedTacanVUStr p_tacanVUStr = null;
            if (mpAssigned.TryGetValue(vuID, out p_tacanVUStr))
            {
                mpAssigned.Remove(vuID);
                mpRetired.Push(p_tacanVUStr);
            }

        }

        public void RemoveTacan(VU_ID id, NavigationType type) // Remove a tacan station from the list
        {
            if (type == NavigationType.AIRBASE) // If removing an airbase
            {
                LinkedTacanVUStr p_tacanVUStr = null;
                if (mpTList.TryGetValue(id, out p_tacanVUStr))
                {
                    mpTList.Remove(id);
                }
            }
            else if (type == NavigationType.TANKER) // If removing a tanker
            {
                RetireChannel(id);
            }
            else if (type == NavigationType.CARRIER) // If removing a carrier
            {
                RetireChannel(id);
            }
        }

#if TODO
    public BOOL GetVUIDFromChannel(int digits, StationSet xy, Domain dom,
                            VU_ID*vuid, int *rangep, int *ttype, float *ilsfreq); // Given the channel and band, we can get the VU_ID of a tacan station
    public BOOL GetVUIDFromLocation(float x, float y, Domain domain, VU_ID*vuid, int *rangep, int *ttype, float *ilsfreq); // Works for airbases only.  Find the closest tacan to this point
#endif

        // Works for airbases only.  Given a VU_ID, we can get the channel and band of a tacan station
        public BOOL GetChannelFromVUID(VU_ID id,
                                   out int p_channel, out StationSet p_set, out Domain p_domain,
                                   out int rangep, out int ttype, out float ilsfreq)
        {

            BOOL result = false;
            p_channel = rangep = ttype = - 1;
            ilsfreq = 0;
            p_set = StationSet.NumSets;
            p_domain = Domain.NumDomains;
            LinkedTacanVUStr tacanVUStr;

            if (mpTList.TryGetValue(id, out tacanVUStr))
            {
                p_channel = tacanVUStr.channel;
                p_set = tacanVUStr.set;
                p_domain = tacanVUStr.domain;
#if TODO
		       TacanCampStr  tinfo; /// XXXXX Bleah

                if (GetCampTacanFromVUID(&tinfo, tacanVUStr->camp_id))
                {
                    rangep = tinfo->range;
                    ttype = tinfo->tactype;
                    ilsfreq = tinfo->ilsfreq;
                }
                else
                {
                    *rangep = 150;
                    *ttype = 1;
                    *ilsfreq = 0;
                }  
#endif
                throw new NotImplementedException();

            }

            return result;
        }

        // Works for airbases only.  Given a campId, we can get the callsign of the respective objective, 0 if not found
        public BOOL GetCallsignFromCampID(short campId, out int callsign)
        {
            callsign = 0;
            if (campId < 0)
            {
                //F4Assert(FALSE); // Bad Id
                log.Warn("bad campid");
                return false;
            }

            TacanCampStr p_occurrence;
            BOOL returnStatus = false;

            if (mpCampList.TryGetValue(campId, out p_occurrence))
            {
                callsign = p_occurrence.callsign;
                returnStatus = true;
            }

            return returnStatus;
        }


        // utility function
        public static int ChannelToFrequency(StationSet set, int channel)
        {
            switch (set)
            {
                case StationSet.X:
                    if (channel < 64)
                        return 961 + channel;
                    else
                        return (channel = 126) + 1213;
                case StationSet.Y:
                    if (channel < 64)
                        return 1087 + channel;
                    else return (channel - 126) + 1087;
                default:
                    return 0;
            }
        }


        public void ReadTacanListFile(string filename = "stations")
        {
            using (StreamReader p_File = new StreamReader(F4File.OpenCampFile(filename, "dat", FileAccess.Read)))
            {
                string line;
                while ((line = p_File.ReadLine()) != null)
                {
                    if (String.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.StartsWith(";"))
                        continue;

                    string[] parts = line.Split(new char[] { ' ' }, 7, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < NUM_TACAN_FIELDS)
                        continue;

                    string bandChar = "x";
                    int channel = 106;
                    int stationId = 0;
                    int callsign = 0;
                    int range, tactype;
                    float ilsfreq;

                    stationId = int.Parse(parts[0]);
                    channel = int.Parse(parts[1]);
                    bandChar = parts[2];
                    callsign = int.Parse(parts[3]);
                    if (parts.Length > 4)
                        range = int.Parse(parts[4]);
                    else
                        range = 150;
                    if (parts.Length > 5)
                        tactype = int.Parse(parts[5]);
                    else
                        tactype = 1;
                    if (parts.Length > 6)
                        ilsfreq = float.Parse(parts[6], CultureInfo.InvariantCulture);
                    else
                        ilsfreq = 111.1f;

                    StationSet band = StationSet.X;
                    if (bandChar == "x" || bandChar == "X")
                    {
                        band = StationSet.X;
                    }
                    else if (bandChar == "y" || bandChar == "Y")
                    {
                        band = StationSet.Y;
                    }
                    else
                    {
                        log.Warn("Invalid Tacan band, Band can only be type X or Y");
                    }
                    Debug.Assert(channel > 0 && channel <= NUM_CHANNELS); // Channel must be between 1 - 126 inclusive
                    //LinkedCampStationStr p_stations = null;
                    if (StoreStation(/*p_stations,*/ (short)stationId, channel, band, callsign,
                                     range, tactype, ilsfreq))   // Insert into ordered linked list
                    {
                        mCampListTally++; // If there are no duplicates, increment Tally
                    }
                }
                p_File.Close();
            }
        }

        // Set up the dynamic lists and get ready to stuff data into the
        // list.  Note: This should only be called by either the
        // constructor or after CleanupDynamicChans() has been called
        //---------------------------------------------------------------

        private void InitDynamicChans()
        {
            mLastUnused = NUM_CHANNELS;
        }

        private void CleanupDynamicChans()
        {
            mpAssigned.Clear();
            mpRetired.Clear();
        }

        public TacanList()
        {
            ReadTacanListFile();
            InitDynamicChans();
        }

        public const int NUM_CHANNELS = 126;
        public const int MIN_TACAN_CHANNEL = 70; // superceeded by global
        public const int NUM_TACAN_FIELDS = 4;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }

}
