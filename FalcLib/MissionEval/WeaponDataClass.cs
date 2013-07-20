using FalconNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.FalcLib
{
    // ===================================
    // Data Storage
    // ===================================

    public class WeaponDataClass
    {
        public string weapon_name;
        public short weapon_id;
        public short starting_load;
        public byte fired;
        public byte missed;
        public byte hit;
        public short events;				   // number of events
        public List<EventElement> root_event;  // List of relevant events

        public WeaponDataClass()
        {
            starting_load = 0;
            fired = 0;
            missed = 0;
            hit = 0;
            events = 0;
            root_event = null;
            weapon_id = 0;
        }
        //TODO public ~WeaponDataClass();
    }
}
