
namespace FalconNet.Common.Graphics
{
    /// <summary>
    /// Three space point
    /// </summary>
    public struct Tpoint
    {
        public float x, y, z;

        public static explicit operator float[](Tpoint p)
        {
            return new float[] { p.x, p.y, p.z };
        }
    }
}
