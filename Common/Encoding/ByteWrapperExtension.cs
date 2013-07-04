using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Common.Encoding
{
    public static class ByteWrapperExtension
    {
        #region Encoders
        public static int EncodeBE(this int val, ByteWrapper buffer)
        {
            byte[] buf = EncodingHelpers.EncodeIntBE(val);
            buffer.Put(buf);
            return buf.Length;
        }

        public static int EncodeBE(this short val, ByteWrapper buffer)
        {
            byte[] buf = EncodingHelpers.EncodeShortBE(val);
            buffer.Put(buf);
            return buf.Length;
        }

        public static int Encode(this long val, ByteWrapper buffer)
        {
            byte[] buf = EncodingHelpers.EncodeLongBE(val);
            buffer.Put(buf);
            return buf.Length;
        }

        public static int Encode(this ulong val, ByteWrapper buffer)
        {
            byte[] buf = EncodingHelpers.EncodeULongBE(val);
            buffer.Put(buf);
            return buf.Length;
        }

        public static void EncodeFloat(this ByteWrapper buffer, float val)
        {
            byte[] buf = EncodingHelpers.EncodeFloatBE(val);
            buffer.Put(buf);
        }

        public static void EncodeDouble(this ByteWrapper buffer, double val)
        {
            byte[] buf = EncodingHelpers.EncodeDoubleBE(val);
            buffer.Put(buf);
        }

        #endregion

        #region Decoders
        public static int DecodeInt(this ByteWrapper buffer)
        {
            return EncodingHelpers.DecodeIntBE(buffer.Buffer, buffer.Position);
        }

        public static short DecodeShort(this ByteWrapper buffer)
        {
            return EncodingHelpers.DecodeShortBE(buffer.Buffer, buffer.Position);
        }

        public static long DecodeLong(this ByteWrapper buffer)
        {
            return EncodingHelpers.DecodeLongBE(buffer.Buffer, buffer.Position);
        }
        public static ulong DecodeULong(this ByteWrapper buffer)
        {
            return EncodingHelpers.DecodeULongBE(buffer.Buffer, buffer.Position);
        }

        public static float DecodeFloat(this ByteWrapper buffer)
        {
            return EncodingHelpers.DecodeFloatBE(buffer.Buffer, buffer.Position);
        }

        public static double DecodeDouble(this ByteWrapper buffer)
        {
            return EncodingHelpers.DecodeDoubleBE(buffer.Buffer, buffer.Position);
        }
        #endregion
    }

}
