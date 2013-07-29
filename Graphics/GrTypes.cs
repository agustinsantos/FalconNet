using System;

namespace FalconNet.Graphics
{
#if TODO // TODO delete all this
    // Three by three rotation matrix
    public struct Trotation
    {
        public float M11, M12, M13;
        public float M21, M22, M23;
        public float M31, M32, M33;
    }

    // Three space point
    public class Tpoint
    {
        public float x, y, z;
        public static explicit operator float[](Tpoint p)
        {
            return new float[] { p.x, p.y, p.z };
        }
    }

    // RGB color
    public struct Tcolor
    {
        public float r;
        public float g;
        public float b;
    }
#endif
}

