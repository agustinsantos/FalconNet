using FalconNet.Common.Encoding;
using FalconNet.F4Common;
using System;
using System.IO;

namespace FalconNet.Campaign
{
    public class PilotStatic
    {
        // =======================
        // Pilot defines
        // =======================

        public const int PILOT_AVAILABLE = 0;
        public const int PILOT_KIA = 1;
        public const int PILOT_MIA = 2;
        public const int PILOT_RESCUED = 3;
        public const int PILOT_IN_USE = 4;
        public const int PILOTS_PER_FLIGHT = 4;						// Maximum pilots/ac per flight
        public const int PILOTS_PER_SQUADRON = 48;						// Maximum pilots per squadron
        public const int NUM_PILOT_VOICES = 12;
        public const int PILOT_SKILL_RANGE = 5;						// # of different AI models

        public const int NO_PILOT = 255;						// No pilot is assigned

        public const int NUM_CALLSIGNS = 160;
        public const int NUM_PILOTS = 686;
        public const int FIRST_PILOT_ID = 2300;
        public const int FIRST_CALLSIGN_ID = 2000;


        // ================
        // Data
        // ================

        #region Public Fields
        public static byte[] CallsignData;
        public static PilotInfoClass[] PilotInfo;
        public static int NumPilots;
        public static int NumCallsigns;
        #endregion

        // ================
        // functions
        // ================
        public static bool LoadPilotInfo(Stream stream, int version)
        {
            if (CampaignClass.gCampDataVersion < 60)
            {
                return false;
            }
            PilotStaticEncodingLE.Decode(stream);
            return true;
        }

        public static void NewPilotInfo()
        {
            NumPilots = NUM_PILOTS;
            PilotInfo = new PilotInfoClass[NumPilots];

            for (int i = 0; i < NumPilots; i++)
                PilotInfo[i].ResetStats();

            // Some hard coded values (Because I'm vain)
            // Col. Klemmick
            PilotInfo[1].voice_id = 1;
            PilotInfo[1].photo_id = 55;
            // Col. Bonanni
            PilotInfo[2].voice_id = 2;
            PilotInfo[2].photo_id = 102;
            // Col. Reiner
            PilotInfo[3].voice_id = 3;
            PilotInfo[3].photo_id = 77;
            // Unassigned pilot (don't use)
            PilotInfo[255].usage = 32000;
            NumCallsigns = NUM_CALLSIGNS;
            CallsignData = new byte[NumCallsigns];
        }

        public static bool LoadPilotInfo(string filename)
        {
#if TODO
		 char /* *data,*/ * data_ptr;
            short max;

            if (CampaignClass.gCampDataVersion < 60)
            {
                NewPilotInfo();
                return true;
            }

            CampaignData cd = ReadCampFile(scenario, "plt");

            if (cd.dataSize == -1)
            {
                return false;
            }

            data_ptr = cd.data;

            // Pilot Data
            max = *((short*)data_ptr);
            data_ptr += sizeof(short);

            //if (PilotInfo)
            //delete [] PilotInfo;

            NumPilots = max;
            PilotInfo = new PilotInfoClass[NumPilots];

            memcpy(PilotInfo, data_ptr, sizeof(PilotInfoClass) * max);
            data_ptr += sizeof(PilotInfoClass) * max;

            // Callsign Data
            max = *((short*)data_ptr);
            data_ptr += sizeof(short);

            //if (CallsignData)
            //   delete [] CallsignData;

            NumCallsigns = max;

            CallsignData = new byte[NumCallsigns];
            memcpy(CallsignData, data_ptr, sizeof(uchar) * NumCallsigns);
            data_ptr += sizeof(uchar) * NumCallsigns;

            //delete cd.data;
            return true;  
#endif
            throw new NotImplementedException();
        }

        public static void SavePilotInfo(string filename)
        {
            throw new NotImplementedException();
        }

        public static void DisposePilotInfo()
        {
            throw new NotImplementedException();
        }

        public static int GetAvailablePilot(int first, int last, int owner)
        {
            int best_pilot = -1;
            short best = short.MaxValue;

            if (last > NumPilots)
                last = NumPilots;

            for (int i = first; i < last; i++)
            {
                if (PilotInfo[i].usage < best)
                {
                    best = PilotInfo[i].usage;
                    best_pilot = i;
                }
            }

            if (best_pilot > -1)
                PilotInfo[best_pilot].usage++;

            if (PilotInfo[best_pilot].voice_id == 255)
            {
                PilotInfo[best_pilot].AssignVoice(owner);
            }

            return best_pilot;
        }

        public static void GetPilotName(int id, string name, int size)
        {
#if TODO
		            ReadIndexedString(FIRST_PILOT_ID + id, name, size);
 
#endif
            throw new NotImplementedException();
        }

        public static void GetCallsignID(ref byte id, ref byte num, int range)
        {
            throw new NotImplementedException();
        }

        public static void SetCallsignID(int id, int num)
        {
            throw new NotImplementedException();
        }

        public static void UnsetCallsignID(int id, int num)
        {
            throw new NotImplementedException();
        }

        public static void GetCallsign(int id, int num, string callsign)
        {
            throw new NotImplementedException();
        }

        public static void GetCallsign(FlightClass fl, string callsign)
        {
            throw new NotImplementedException();
        }

        public static void GetDogfightCallsign(FlightClass flight)
        {
            throw new NotImplementedException();
        }
    }

    public static class PilotStaticEncodingLE
    {
        public static void Encode(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream)
        {
            PilotStatic.NumPilots = Int16EncodingLE.Decode(stream);
            PilotStatic.PilotInfo = new PilotInfoClass[PilotStatic.NumPilots];
            for (var j = 0; j < PilotStatic.PilotInfo.Length; j++)
            {
                var thisPilot = new PilotInfoClass();
                thisPilot.usage = Int16EncodingLE.Decode(stream);
                thisPilot.voice_id = (byte)stream.ReadByte();
                thisPilot.photo_id = (byte)stream.ReadByte();
            }

            PilotStatic.NumCallsigns = Int16EncodingLE.Decode(stream);
            PilotStatic.CallsignData = new byte[PilotStatic.NumCallsigns];
            for (var j = 0; j < PilotStatic.NumCallsigns; j++)
            {
                PilotStatic.CallsignData[j] = (byte)stream.ReadByte();
            }
        }

        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }
    // ===========================
    // Pilot name index structure
    // ===========================

    public class PilotClass
    {

        public short pilot_id;							// Index into the PilotInfoClass table
        public byte pilot_skill_and_rating;				// LowByte: Skill, HiByte: Rating
        public byte pilot_status;
        public byte aa_kills;
        public byte ag_kills;
        public byte as_kills;
        public byte an_kills;
        public short missions_flown;

        public PilotClass()
        {
            throw new NotImplementedException();
        }
        // 2000-11-17 MODIFIED BY S.G. I NEED TO PASS THE 'airExperience'.
        //		void ResetStats(void);
        public void ResetStats(byte airExperience)
        {
            throw new NotImplementedException();
        }
        // 2001-11-19 ADDED by M.N. for TE squad change pilots rating
        public void SetTEPilotRating(byte rating)
        {
            throw new NotImplementedException();
        }

        public int GetPilotSkill()
        {
            return (pilot_skill_and_rating & 0xF);
        }

        public int GetPilotRating()
        {
            return ((byte)((pilot_skill_and_rating & 0xF0) >> 4));
        }

        public void SetPilotSR(byte skill, byte rating)
        {
            pilot_skill_and_rating = (byte)((rating << 4) | skill);
        }
    }

    public class PilotInfoClass
    {
        public short usage;								// How many times this pilot is being used
        public byte voice_id;							// Which voice data to use
        public byte photo_id;							// Assigned through the UI

        public PilotInfoClass()
        {
            throw new NotImplementedException();
        }

        public void ResetStats()
        {
            throw new NotImplementedException();
        }

        public void AssignVoice(int owner)
        {
            throw new NotImplementedException();
        } // JPO
    };
}

