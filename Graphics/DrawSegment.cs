using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalconNet.Graphics
{
    public enum TrailType
    {
        TRAIL_CONTRAIL, //0 White contrails
        TRAIL_VORTEX, //1 Not in use
        TRAIL_SAM, //2 Large SAMs trail
        TRAIL_AIM120, //3 ARH missiles weak trail
        TRAIL_MAVERICK, //4 Maverick type missiles trail
        TRAIL_SMOKE, //5 Old trail, not in use
        TRAIL_MEDIUM_SAM, //6 For smaller SAMs
        TRAIL_LARGE_AGM, //7 For large AGMs
        TRAIL_GUN2, //8 Second type gun trail
        TRAIL_SARH_MISSILE, //9 SARH missiles trail
        TRAIL_GUN3, //10 Third type gun trail
        TRAIL_GUN, //11 default gun trail
        TRAIL_LWING, //12 ACMI trail (still using old trail.txt trail)
        TRAIL_RWING, //13 ACMI trail (still using old trail.txt trail)
        TRAIL_ROCKET, //14 Rockets trail
        TRAIL_MISLSMOKE, //15 Old trail, not in use
        TRAIL_IR_MISSILE, //16 Small IR missiles trail (usually IR)
        TRAIL_DARKSMOKE, //17 Engien trail
        TRAIL_FIRE2, //18 Old trail, not in use
        TRAIL_WINGTIPVTX, //19 Wingtip vortex trail
        TRAIL_A10CANNON1, //20 A-10 Avenger cannon trail
        TRAIL_ENGINE_SMOKE_LIGHT, //21  Engien trail
        TRAIL_FEATURESMOKE, //22 Old trail, not in use
        TRAIL_BURNINGDEBRIS, // 23      Old trail, not in use
        TRAIL_FLARE, // 24 Flare trail (not used really, as it's called by PS script...)
        TRAIL_FLAREGLOW, // 25 Old trail, not in use
        TRAIL_CONTAIL_SHORT, // 26      Old trail, not in use
        TRAIL_WING_COLOR, // 27         Saved for wing color trail (if will be implemented later...)
        TRAIL_MISSILEHIT_FIRE, // 28    Old trail, not in use
        TRAIL_BURNING_SMOKE, // 29 AC damaged trail
        TRAIL_BURNING_SMOKE2, // 30     Old trail, not in use
        TRAIL_BURNING_FIRE,  // 31      Old trail, not in use
        TRAIL_ENGINE_SMOKE_SHARED, //32 Engien trail
        TRAIL_GROUND_EXP_SMOKE, //33    Old trail, not in use
        TRAIL_DUSTCLOUD, // 34 Stirred up dust when A/C close to ground
        TRAIL_MISTCLOUD, // 35 Stirred up mist when A/C close to water
        TRAIL_LINGERINGSMOKE, // 36 Old trail, not in use
        TRAIL_GROUNDVDUSTCLOUD, // 37 Old trail, not in use
        TRAIL_B52H_ENGINE_SMOKE, // 38 Engine trail to be used for all modern engine big birds
        TRAIL_F4_ENGINE_SMOKE, // 39    F-4 engine trail
        TRAIL_COLOR_0, // 40 Color trail (for CTRL-S trail) this one is default in PS as RED
        TRAIL_COLOR_1, //41 Color trail (for CTRL-S trail) this one is default in PS as GREEN
        TRAIL_COLOR_2, //42 Color trail (for CTRL-S trail) this one is default in PS as BLUE
        TRAIL_COLOR_3, //43 Color trail (for CTRL-S trail) this one is default in PS as YELLOW
        TRAIL_RWING_COLOR_0, // 44 Right wing Color trail this one is default in PS as WHITE
        TRAIL_RWING_COLOR_1, //45 Right wing Color trail this one is default in PS as RED
        TRAIL_RWING_COLOR_2, //46 Right wing Color trail this one is default in PS as GREEN
        TRAIL_RWING_COLOR_3, //47 Right wing Color trail this one is default in PS as BLUE
        TRAIL_RWING_COLOR_4, //48 Right wing Color trail this one is default in PS as YELLOW
        TRAIL_LWING_COLOR_0, // 49 Left wing Color trail this one is default in PS as WHITE
        TRAIL_LWING_COLOR_1, //50 Left wing Color trail this one is default in PS as RED
        TRAIL_LWING_COLOR_2, //51 Left wing Color trail this one is default in PS as GREEN
        TRAIL_LWING_COLOR_3, //52 Left wing Color trail this one is default in PS as BLUE
        TRAIL_LWING_COLOR_4, //53 Left wing Color trail this one is default in PS as YELLOW
        TRAIL_VORTEX_LARGE, //54     Large vortex trail for large fighters
        TRAIL_MAX = 150
    };

    public class DrawableTrail : DrawableObject
    {
        public DrawableTrail(TrailType s) : base(1)
        {
        }

        public override void Draw(RenderOTW renderer, int LOD)
        {
            throw new NotImplementedException();
        }

        public void KeepStaleSegs(bool v)
        {
            throw new NotImplementedException();
        }
    }
}
