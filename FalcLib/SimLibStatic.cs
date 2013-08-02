using SIM_FLOAT = System.Single;
using SIM_INT = System.Int32;
using SIM_UINT = System.UInt32;
using VU_TIME = System.UInt64;

namespace FalconNet.FalcLib
{
    public static class SimLibStatic
    {
        public static SIM_FLOAT SimLibMinorFrameTime = 0.02F;
        public static SIM_FLOAT SimLibMinorFrameRate = 50.0F;
        public static SIM_FLOAT SimLibMajorFrameTime = 0.06F;
        public static SIM_FLOAT SimLibMajorFrameRate = 16.667F;
        public static SIM_FLOAT SimLibTimeOfDay;
        public static VU_TIME SimLibElapsedTime;
        public static float SimLibElapsedSeconds; // COBRA - RED - Added Variable of Elasped Simulation Seconds
        public static float SimLibFrameElapsed, SimLibLastFrameTime; // COBRA - RED - Added Variable of Elasped Frame Time
        public static SIM_UINT SimLibFrameCount = 0;
        public static SIM_INT SimLibMinorPerMajor = 3;
    }
}
