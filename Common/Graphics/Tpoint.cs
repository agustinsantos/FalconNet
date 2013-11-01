
using FalconNet.Common.Encoding;
using System;
using System.IO;
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

   #region Encoding
    public static class TpointEncodingLE
    {
        public static void Encode(Stream stream, Tpoint val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ref Tpoint rst)
        {
            rst.x = SingleEncodingLE.Decode(stream);
            rst.y = SingleEncodingLE.Decode(stream);
            rst.z = SingleEncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }
    #endregion
}
