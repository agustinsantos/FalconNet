using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.FalcLib
{
    public class TeamDoctrine
    {
        public ShootEnum simFlags;
        public float radarShootShootPct;
        public float heatShootShootPct;

        public TeamDoctrine() 
        { 
            simFlags = 0; 
        }

        public enum ShootEnum
        {
            SimRadarShootShoot = 0x1,
            SimHeatShootShoot = 0x2
        }

        public bool IsSet(ShootEnum val)
        { 
            return simFlags.HasFlag(val);
        }

        public void Set(ShootEnum val)
        { 
            simFlags |= val;
        }

        public void Clear(ShootEnum val) 
        { 
            simFlags &= ~val; 
        }

        public float RadarShootShootPct() 
        { 
            return radarShootShootPct; 
        }

        public float HeatShootShootPct() 
        { 
            return heatShootShootPct;
        }
    }
}
