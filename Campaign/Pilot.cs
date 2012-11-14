using System;

namespace FalconNet.Campaign
{
	public class PilotStatic
	{
// =======================
// Pilot defines
// =======================

		public const int PILOT_AVAILABLE = 0;
		public const int  PILOT_KIA = 1;
		public const int  PILOT_MIA = 2;
		public const int  PILOT_RESCUED = 3;
		public const int  PILOT_IN_USE = 4;
		public const int  PILOTS_PER_FLIGHT = 4;						// Maximum pilots/ac per flight
		public const int  PILOTS_PER_SQUADRON = 48;						// Maximum pilots per squadron
		public const int NUM_PILOT_VOICES = 12;
		public const int  PILOT_SKILL_RANGE = 5;						// # of different AI models

		public const int  NO_PILOT = 255;						// No pilot is assigned
		
// ================
// Data
// ================

		public static byte[] CallsignData;
		public static PilotInfoClass PilotInfo;
		public static int NumPilots;
		public static int NumCallsigns;

// ================
// functions
// ================


		public static void NewPilotInfo ()
		{
			throw new NotImplementedException ();
		}

		public static int LoadPilotInfo (string filename)
		{
			throw new NotImplementedException ();
		}

		public static void SavePilotInfo (string filename)
		{
			throw new NotImplementedException ();
		}

		public static void DisposePilotInfo ()
		{
			throw new NotImplementedException ();
		}

		public static int GetAvailablePilot (int first, int last, int owner)
		{
			throw new NotImplementedException ();
		}

		public static void GetPilotName (int id, string name, int size)
		{
			throw new NotImplementedException ();
		}

		public static void GetCallsignID (ref byte id, ref byte num, int range)
		{
			throw new NotImplementedException ();
		}

		public static void SetCallsignID (int id, int num)
		{
			throw new NotImplementedException ();
		}

		public static void UnsetCallsignID (int id, int num)
		{
			throw new NotImplementedException ();
		}

		public static void GetCallsign (int id, int num, string callsign)
		{
			throw new NotImplementedException ();
		}

		public static void GetCallsign (FlightClass fl, string callsign)
		{
			throw new NotImplementedException ();
		}

		public static void GetDogfightCallsign (FlightClass flight)
		{
			throw new NotImplementedException ();
		}
	}
// ===========================
// Pilot name index structure
// ===========================

	public class PilotClass
	{
	
		public short		pilot_id;							// Index into the PilotInfoClass table
		public byte		pilot_skill_and_rating;				// LowByte: Skill, HiByte: Rating
		public byte		pilot_status;
		public byte		aa_kills;
		public byte		ag_kills;
		public byte		as_kills;
		public byte		an_kills;
		public short		missions_flown;

		public PilotClass ()
		{
			throw new NotImplementedException ();
		}
// 2000-11-17 MODIFIED BY S.G. I NEED TO PASS THE 'airExperience'.
//		void ResetStats(void);
		public void ResetStats (byte airExperience)
		{
			throw new NotImplementedException ();
		}
// 2001-11-19 ADDED by M.N. for TE squad change pilots rating
		public void SetTEPilotRating (byte rating)
		{
			throw new NotImplementedException ();
		}

		public int GetPilotSkill ()
		{
			return (pilot_skill_and_rating & 0xF);
		}

		public int GetPilotRating ()
		{
			return ((byte)((pilot_skill_and_rating & 0xF0) >> 4));
		}

		public void SetPilotSR (byte skill, byte rating)
		{
			pilot_skill_and_rating = (byte)((rating << 4) | skill);
		}
	}

	public class PilotInfoClass
	{
		public short		usage;								// How many times this pilot is being used
		public byte		voice_id;							// Which voice data to use
		public byte		photo_id;							// Assigned through the UI
		
		public PilotInfoClass ()
		{
			throw new NotImplementedException ();
		}
		
		public void ResetStats ()
		{
			throw new NotImplementedException ();
		}

		public void AssignVoice (int owner)
		{
			throw new NotImplementedException ();
		} // JPO
	};
}

