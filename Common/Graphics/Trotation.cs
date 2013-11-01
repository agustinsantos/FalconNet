
using FalconNet.Common.Encoding;
using System;
using System.IO;
namespace FalconNet.Common.Graphics
{
    /// <summary>
    /// Three by three rotation matrix
    /// </summary>
    public struct Trotation
    {
        public float M11, M12, M13;
        public float M21, M22, M23;
        public float M31, M32, M33;
    }

    #region Encoding
    public static class TrotationEncodingLE
    {
        public static void Encode(Stream stream, Trotation val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ref Trotation rst)
        {
            rst.M11 = SingleEncodingLE.Decode(stream);
            rst.M12 = SingleEncodingLE.Decode(stream);
            rst.M13 = SingleEncodingLE.Decode(stream);
            rst.M21 = SingleEncodingLE.Decode(stream);
            rst.M22 = SingleEncodingLE.Decode(stream);
            rst.M23 = SingleEncodingLE.Decode(stream);
            rst.M31 = SingleEncodingLE.Decode(stream);
            rst.M32 = SingleEncodingLE.Decode(stream);
            rst.M33 = SingleEncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return 9 * SingleEncodingLE.Size; }
        }
    }
    #endregion
}
