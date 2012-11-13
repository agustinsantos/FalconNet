using System;
using Flight=FalconNet.Campaign.FlightClass;
namespace FalconNet.Campaign
{
	public class ACSelect
	{
		public static bool RequestACSlot (Flight flight, byte team, byte plane_slot, byte skill, int ac_type, int player)
		{throw new NotImplementedException();}
		public static void LeaveACSlot (Flight flight, byte plane_slot)
		{throw new NotImplementedException();}
		public static void RequestFlightDelete (Flight flight)
		{throw new NotImplementedException();}
		public static void RequestTeamChange (Flight flight, int newteam)
		{throw new NotImplementedException();}
		public static void RequestTypeChange (Flight flight, int newtype)
		{throw new NotImplementedException();}
		public static void RequestCallsignChange (Flight flight, int newcallsign)
		{throw new NotImplementedException();}
		public static void RequestSkillChange (Flight flight, int plane_slot, int newskill)
		{throw new NotImplementedException();}
	}
}

