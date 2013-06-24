using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Common.Encoding
{
    public static class ByteWrapperExtension
    {
        #region Encoders
        public static int Encode(this int val, ByteWrapper buffer)
        {
            byte[] buf = EncodingHelpers.EncodeIntBE(val);
            buffer.put(buf);
            return buf.Length;
        }

        public static int Encode(this short val, ByteWrapper buffer)
        {
            byte[] buf = EncodingHelpers.EncodeShortBE(val);
            buffer.put(buf);
            return buf.Length;
        }

        public static int Encode(this long val, ByteWrapper buffer)
        {
            byte[] buf = EncodingHelpers.EncodeLongBE(val);
            buffer.put(buf);
            return buf.Length;
        }

        public static int Encode(this ulong val, ByteWrapper buffer)
        {
            byte[] buf = EncodingHelpers.EncodeULongBE(val);
            buffer.put(buf);
            return buf.Length;
        }

        public static void EncodeFloat(this ByteWrapper buffer, float val)
        {
            byte[] buf = EncodingHelpers.EncodeFloatBE(val);
            buffer.put(buf);
        }

        public static void EncodeDouble(this ByteWrapper buffer, double val)
        {
            byte[] buf = EncodingHelpers.EncodeDoubleBE(val);
            buffer.put(buf);
        }

        #endregion

        #region Decoders
        public static int DecodeInt(this ByteWrapper buffer)
        {
            return EncodingHelpers.DecodeIntBE(buffer.array(), buffer.getPos());
        }

        public static short DecodeShort(this ByteWrapper buffer)
        {
            return EncodingHelpers.DecodeShortBE(buffer.array(), buffer.getPos());
        }

        public static long DecodeLong(this ByteWrapper buffer)
        {
            return EncodingHelpers.DecodeLongBE(buffer.array(), buffer.getPos());
        }
        public static ulong DecodeULong(this ByteWrapper buffer)
        {
            return EncodingHelpers.DecodeULongBE(buffer.array(), buffer.getPos());
        }

        public static float DecodeFloat(this ByteWrapper buffer)
        {
            return EncodingHelpers.DecodeFloatBE(buffer.array(), buffer.getPos());
        }

        public static double DecodeDouble(this ByteWrapper buffer)
        {
            return EncodingHelpers.DecodeDoubleBE(buffer.array(), buffer.getPos());
        }
        #endregion
    }
}
