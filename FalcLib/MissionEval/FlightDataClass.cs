using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Team = System.Byte;
using GridIndex = System.Int16;
using FalconNet.Common;
using FalconNet.F4Common;

namespace FalconNet.FalcLib
{
    public class FlightDataClass
    {
        public string name;
        public ulong status_flags;
        public short camp_id;
        public VU_ID flight_id;
        public string aircraft_name;
        public byte start_aircraft;
        public byte finish_aircraft;
        public Team flight_team;
        public MissionTypeEnum mission;
        public byte old_mission;						// Old mission, if we were diverted
        public VU_ID requester_id;						// ID of entity which caused this mission
        public VU_ID target_id;
        public short target_camp_id;
        public byte target_building;
        public GridIndex target_x;
        public GridIndex target_y;
        public byte[] target_features = new byte[MissionEvaluationClass.MAX_TARGET_FEATURES];
        public byte target_status;
        public byte mission_context;
        public byte mission_success;
        public byte failure_code;
        public short failure_data;
        public VU_ID failure_id;
        public byte num_pilots;
        public string context_entity_name;			// Name of the entity this mission was about
        public PilotDataClass pilot_list;
        public short events;								// number of events
        public List<EventElement> root_event;						// List of relevant events
        public FlightDataClass next_flight;

        public FlightDataClass()
        {
            camp_id = 0;
            flight_id = target_id = VU_ID.FalconNullId;
            requester_id = VU_ID.FalconNullId;
            start_aircraft = finish_aircraft = 0;
            flight_team = 0;
            mission = 0;
            target_camp_id = 0;
            target_building = 0;
            //TODO target_x = target_y = null;
            status_flags = 0;
            mission_success = 0;
            mission_context = 0;
            failure_code = 0;
            failure_data = 0;
            num_pilots = 0;
            pilot_list = null;
            events = 0;
            root_event = null;
            next_flight = null;
            //memset(target_features,0,MAX_TARGET_FEATURES);
            target_status = 0;
            context_entity_name = "";
        }

        // public ~FlightDataClass();
    }

}
