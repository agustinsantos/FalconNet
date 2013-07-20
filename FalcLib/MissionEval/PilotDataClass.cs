using FalconNet.Campaign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.FalcLib 
{
    public class PilotDataClass
    {		
        // Kill tracking vs..
        public const int VS_AI = 0;
        public const int VS_HUMAN = 1;
        public const int VS_EITHER = 2;


        public string pilot_name;
        public string pilot_callsign;
        public byte aircraft_slot;						// Which aircraft we're occupying
        public byte pilot_slot;							// Pilot's position in the pilot_list
        public short pilot_id;							// ID of the pre-assigned non-player pilot
        public byte pilot_flags;
        public byte pilot_status;
        public byte aircraft_status;
        public byte aa_kills;
        public byte ag_kills;
        public byte as_kills;
        public byte an_kills;
        public byte player_kills;
        public byte shot_at;							// Times this player was shot at (only tracks for local player)
        public short[] deaths = new short[VS_EITHER];					// Dogfight statistics [AI/PLAYER]
        public short score;
        public byte rating;
        public byte weapon_types;						// number of actually different weapons
        public bool donefiledebrief;					//me123 has a file debrief already been made
        public WeaponDataClass[] weapon_data = new WeaponDataClass[(CampWeapons.HARDPOINT_MAX / 2) + 2];	// Weapon data for this pilot/aircraft
        public PilotDataClass next_pilot;

        public PilotDataClass()
        {
            aircraft_slot = 0;
            pilot_slot = 0;
            pilot_id = 0;
            pilot_flags = 0;
            pilot_status = 0;
            aircraft_status = 0;
            aa_kills = ag_kills = as_kills = an_kills = 0;
            donefiledebrief = false;
            player_kills = 0;
            //	memset(kills,0,DogfightStatic.MAX_DOGFIGHT_TEAMS*VS_EITHER*sizeof(short));
            // memset(deaths,0,VS_EITHER*sizeof(short));
            shot_at = 0;
            score = 0;
            rating = 0;
            weapon_types = 0;
            next_pilot = null;
            pilot_callsign = "";
            pilot_name = "";
        }

        // TODO public ~PilotDataClass();
    }
}
