using System;

namespace FalconNet.Campaign
{
	public class ACSelect
	{
		public static BOOL RequestACSlot (Flight flight, uchar team, uchar plane_slot, uchar skill, int ac_type, int player);
		public static void LeaveACSlot (Flight flight, uchar plane_slot);
		public static void RequestFlightDelete (Flight flight);
		public static void RequestTeamChange (Flight flight, int newteam);
		public static void RequestTypeChange (Flight flight, int newtype);
		public static void RequestCallsignChange (Flight flight, int newcallsign);
		public static void RequestSkillChange (Flight flight, int plane_slot, int newskill);
	}
}

