using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Common.Encoding
{
    public enum EndianTypes
    {
        LittleEndian,
        BigEndian
    }

    /// <summary> 
    /// Utility methods for encoding and decoding basic types.
    /// </summary>
    /// <author> 
    /// Agustin Santos.
    /// </author>
    public static class EncodingHelpers
    {
        public static readonly EndianTypes Endian = (BitConverter.IsLittleEndian ? EndianTypes.LittleEndian : EndianTypes.BigEndian);

        #region Encode/Decode Int16
        /// <summary> 
        /// Encodes the specified value, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the value to Encode
        /// </param>
        /// <returns> 
        /// a byte array containing the encoded value
        /// An array of bytes with length 2.
        /// </returns>
        public static byte[] EncodeShortLE(short val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            return buf;
        }
        public static byte[] EncodeShortBE(short val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            return buf;
        }

        public static void EncodeShortLE(this ByteWrapper buffer, short val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }
        public static void EncodeShortBE(this ByteWrapper buffer, short val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        /// <exception cref="System.IO.IOException"> if the parameterValue could not be decoded
        /// </exception>
        public static short DecodeShortLE(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < sizeof(short))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer);
            return BitConverter.ToInt16(buffer, 0);
        }
        public static short DecodeShortBE(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < sizeof(short))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer);
            return BitConverter.ToInt16(buffer, 0);
        }
        public static short DecodeShortLE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < startPos + sizeof(short))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer, startPos, sizeof(short));
            return BitConverter.ToInt16(buffer, startPos);
        }
        public static short DecodeShortBE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < startPos + sizeof(short))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer, startPos, sizeof(short));
            return BitConverter.ToInt16(buffer, startPos);
        }
        public static short DecodeShortLE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            short val = DecodeShortLE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(short));
            return val;
        }
        public static short DecodeShortBE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            short val = DecodeShortBE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(short));
            return val;
        }

        public static int ShortSize
        {
            get { return sizeof(short); }
        }
        #endregion

        #region Encode/Decode Int32
        /// <summary> 
        /// Encodes the specified value, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the value to Encode
        /// </param>
        /// <returns> 
        /// a byte array containing the encoded value
        /// An array of bytes with length 4.
        /// </returns>
        public static byte[] EncodeIntLE(int val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            return buf;
        }
        public static byte[] EncodeIntBE(int val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            return buf;
        }

        public static void EncodeIntLE(this ByteWrapper buffer, int val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }
        public static void EncodeIntBE(this ByteWrapper buffer, int val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        public static int DecodeIntLE(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < sizeof(int))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }
        public static int DecodeIntBE(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < sizeof(int))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }
        public static int DecodeIntLE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < startPos + sizeof(int))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer, startPos, sizeof(int));
            return BitConverter.ToInt32(buffer, startPos);
        }
        public static int DecodeIntBE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < startPos + sizeof(int))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer, startPos, sizeof(int));
            return BitConverter.ToInt32(buffer, startPos);
        }
        public static int DecodeIntLE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            int val = DecodeIntLE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(int));
            return val;
        }
        public static int DecodeIntBE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            int val = DecodeIntBE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(int));
            return val;
        }

        public static int IntSize
        {
            get { return sizeof(int); }
        }
        #endregion

        #region Encode/Decode Int64
        /// <summary> 
        /// Encodes the specified value, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the value to Encode
        /// </param>
        /// <returns> 
        /// a byte array containing the encoded value
        /// An array of bytes with length 8.
        /// </returns>
        public static byte[] EncodeLongLE(long val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            return buf;
        }
        public static byte[] EncodeLongBE(long val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            return buf;
        }

        public static void EncodeLongLE(this ByteWrapper buffer, long val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }
        public static void EncodeLongBE(this ByteWrapper buffer, long val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        public static long DecodeLongLE(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < sizeof(long))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }
        public static long DecodeLongBE(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < sizeof(long))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }
        public static long DecodeLongLE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < startPos + sizeof(long))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer, startPos, sizeof(long));
            return BitConverter.ToInt64(buffer, startPos);
        }
        public static long DecodeLongBE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < startPos + sizeof(long))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer, startPos, sizeof(long));
            return BitConverter.ToInt64(buffer, startPos);
        }
        public static long DecodeLongLE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            long val = DecodeLongLE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(long));
            return val;
        }
        public static long DecodeLongBE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            long val = DecodeLongBE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(long));
            return val;
        }

        public static int LongSize
        {
            get { return sizeof(long); }
        }
        #endregion

        #region Encode/Decode UInt16
        /// <summary> 
        /// Encodes the specified value, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the value to Encode
        /// </param>
        /// <returns> 
        /// a byte array containing the encoded value
        /// An array of bytes with length 2.
        /// </returns>
        public static byte[] EncodeUShortLE(ushort val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            return buf;
        }
        public static byte[] EncodeUShortBE(ushort val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            return buf;
        }

        public static void EncodeUShortLE(this ByteWrapper buffer, ushort val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }
        public static void EncodeUShortBE(this ByteWrapper buffer, ushort val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        /// <exception cref="System.IO.IOException"> if the parameterValue could not be decoded
        /// </exception>
        public static ushort DecodeUShortLE(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < sizeof(ushort))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer);
            return BitConverter.ToUInt16(buffer, 0);
        }
        public static ushort DecodeUShortBE(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < sizeof(ushort))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer);
            return BitConverter.ToUInt16(buffer, 0);
        }
        public static ushort DecodeUShortLE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < startPos + sizeof(ushort))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer, startPos, sizeof(ushort));
            return BitConverter.ToUInt16(buffer, startPos);
        }
        public static ushort DecodeUShortBE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < startPos + sizeof(ushort))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer, startPos, sizeof(ushort));
            return BitConverter.ToUInt16(buffer, startPos);
        }
        public static ushort DecodeUShortLE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            ushort val = DecodeUShortLE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(ushort));
            return val;
        }
        public static ushort DecodeUShortBE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            ushort val = DecodeUShortBE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(ushort));
            return val;
        }

        public static int UShortSize
        {
            get { return sizeof(ushort); }
        }
        #endregion

        #region Encode/Decode UInt32
        /// <summary> 
        /// Encodes the specified value, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the value to Encode
        /// </param>
        /// <returns> 
        /// a byte array containing the encoded value
        /// An array of bytes with length 4.
        /// </returns>
        public static byte[] EncodeUIntLE(uint val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            return buf;
        }
        public static byte[] EncodeUIntBE(uint val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            return buf;
        }

        public static void EncodeUIntLE(this ByteWrapper buffer, uint val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }
        public static void EncodeUIntBE(this ByteWrapper buffer, uint val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        public static uint DecodeUIntLE(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < sizeof(uint))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }
        public static uint DecodeUIntBE(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < sizeof(uint))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }
        public static uint DecodeUIntLE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < startPos + sizeof(uint))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer, startPos, sizeof(uint));
            return BitConverter.ToUInt32(buffer, startPos);
        }
        public static uint DecodeUIntBE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < startPos + sizeof(uint))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer, startPos, sizeof(uint));
            return BitConverter.ToUInt32(buffer, startPos);
        }
        public static uint DecodeUIntLE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            uint val = DecodeUIntLE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(uint));
            return val;
        }
        public static uint DecodeUIntBE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            uint val = DecodeUIntBE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(uint));
            return val;
        }

        public static int UIntSize
        {
            get { return sizeof(uint); }
        }
        #endregion

        #region Encode/Decode UInt64
        /// <summary> 
        /// Encodes the specified value, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the value to Encode
        /// </param>
        /// <returns> 
        /// a byte array containing the encoded value
        /// An array of bytes with length 8.
        /// </returns>
        public static byte[] EncodeULongLE(ulong val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            return buf;
        }
        public static byte[] EncodeULongBE(ulong val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            return buf;
        }

        public static void EncodeULongLE(this ByteWrapper buffer, ulong val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }
        public static void EncodeULongBE(this ByteWrapper buffer, ulong val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        public static ulong DecodeULongLE(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < sizeof(ulong))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
        public static ulong DecodeULongBE(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < sizeof(ulong))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
        public static ulong DecodeULongLE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < startPos + sizeof(ulong))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer, startPos, sizeof(ulong));
            return BitConverter.ToUInt64(buffer, startPos);
        }
        public static ulong DecodeULongBE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            if (buffer.Length < startPos + sizeof(ulong))
                throw new IndexOutOfRangeException("length = " + buffer.Length);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer, startPos, sizeof(ulong));
            return BitConverter.ToUInt64(buffer, startPos);
        }
        public static ulong DecodeULongLE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            ulong val = DecodeULongLE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(ulong));
            return val;
        }
        public static ulong DecodeULongBE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            ulong val = DecodeULongBE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(ulong));
            return val;
        }

        public static int ULongSize
        {
            get { return sizeof(ulong); }
        }
        #endregion

        #region Encode/Decode Float
        /// <summary> 
        /// Encodes the specified parameterValue, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the parameterValue to Encode
        /// </param>
        /// <returns>
        /// a byte array containing the encoded parameterValue
        /// An array of bytes with length 4.
        /// </returns>
        public static byte[] EncodeFloatLE(float val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            return buf;
        }
        public static byte[] EncodeFloatBE(float val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            return buf;
        }

        public static void EncodeFloatLE(this ByteWrapper buffer, float val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }
        public static void EncodeFloatBE(this ByteWrapper buffer, float val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        /// <exception cref="System.IO.IOException"> if the parameterValue could not be decoded
        /// </exception>
        public static float DecodeFloatLE(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < sizeof(float))
                throw new Exception();
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }
        public static float DecodeFloatBE(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < sizeof(float))
                throw new Exception();
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }
        public static float DecodeFloatLE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(float))
                throw new Exception();
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer, startPos, sizeof(float));
            return BitConverter.ToSingle(buffer, startPos);
        }
        public static float DecodeFloatBE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(float))
                throw new Exception();
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer, startPos, sizeof(float));
            return BitConverter.ToSingle(buffer, startPos);
        }
        public static float DecodeFloatLE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            float val = DecodeFloatLE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(float));
            return val;
        }
        public static float DecodeFloatBE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            float val = DecodeFloatBE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(float));
            return val;
        }

        public static int FloatSize
        {
            get { return sizeof(float); }
        }
        #endregion

        #region Encode/Decode Double
        /// <summary> 
        /// Encodes the specified parameterValue, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the parameterValue to Encode
        /// </param>
        /// <returns> 
        /// a byte array containing the encoded parameterValue
        /// An array of bytes with length 8.
        /// </returns>
        public static byte[] EncodeDoubleLE(double val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            return buf;
        }
        public static byte[] EncodeDoubleBE(double val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            return buf;
        }
        public static void EncodeDoubleLE(this ByteWrapper buffer, double val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }
        public static void EncodeDoubleBE(this ByteWrapper buffer, double val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }
        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        /// <exception cref="System.IO.IOException"> if the parameterValue could not be decoded
        /// </exception>
        public static double DecodeDoubleLE(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < sizeof(double))
                throw new Exception();
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer);
            return BitConverter.ToDouble(buffer, 0);
        }
        public static double DecodeDoubleBE(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < sizeof(double))
                throw new Exception();
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer);
            return BitConverter.ToDouble(buffer, 0);
        }
        public static double DecodeDoubleLE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(double))
                throw new Exception();
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer, startPos, sizeof(double));
            return BitConverter.ToDouble(buffer, startPos);
        }
        public static double DecodeDoubleBE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(double))
                throw new Exception();
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer, startPos, sizeof(double));
            return BitConverter.ToDouble(buffer, startPos);
        }

        public static double DecodeDoubleLE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            double val = DecodeDoubleLE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(double));
            return val;
        }
        public static double DecodeDoubleBE(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            double val = DecodeDoubleBE(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(float));
            return val;
        }

        public static int DoubleSize
        {
            get { return sizeof(double); }
        }
        #endregion

        #region Encode/Decode Boolean
        /// <summary> 
        /// Encodes the specified parameterValue, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the parameterValue to Encode
        /// </param>
        /// <returns> a byte array containing the encoded parameterValue
        /// An array of bytes with length 1.
        /// </returns>
        public static byte[] EncodeBoolean(bool val)
        {
            return BitConverter.GetBytes(val);
        }
        public static void EncodeBoolean(this ByteWrapper buffer, bool val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            buffer.Put(buf);
        }

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        /// <exception cref="System.IO.System.IO.IOException"> if the parameterValue could not be decoded
        /// </exception>
        public static bool DecodeBoolean(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < sizeof(bool))
                throw new Exception();
            return BitConverter.ToBoolean(buffer, 0);
        }
        public static bool DecodeBoolean(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(bool))
                throw new Exception();
            return BitConverter.ToBoolean(buffer, startPos);
        }
        public static bool DecodeBoolean(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            bool val = DecodeBoolean(buffer.Buffer, buffer.Position);
            buffer.Advance(sizeof(bool));
            return val;
        }

        public static int BoolSize
        {
            get { return sizeof(bool); }
        }
        #endregion

        #region Encode/Decode Byte
        /// <summary> 
        /// Encodes the specified parameterValue, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the parameterValue to Encode
        /// </param>
        /// <returns> 
        /// a byte array containing the encoded parameterValue
        /// An array of bytes with length 1.
        /// </returns>
        public static byte[] EncodeByte(byte val)
        {
            return new byte[] { val };
        }
        public static void EncodeByte(this ByteWrapper buffer, byte val)
        {
            buffer.Put(val);
        }

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        /// <exception cref="System.IO.IOException"> if the parameterValue could not be decoded
        /// </exception>
        public static byte DecodeByte(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < sizeof(byte))
                throw new Exception();
            return buffer[0];
        }
        public static byte DecodeByte(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(byte))
                throw new Exception();
            return buffer[startPos];
        }
        public static byte DecodeByte(this ByteWrapper buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Buffer is null");
            return buffer.GetByte();
        }

        public static int ByteSize
        {
            get { return sizeof(byte); }
        }
        #endregion

        /// <summary> 
        /// Encodes the specified parameterValue, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the parameterValue to Encode
        /// </param>
        /// <returns> 
        /// a byte array containing the encoded parameterValue
        /// An array of bytes with length 2.
        /// </returns>
        public static byte[] EncodeCharLE(char val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buf);
            return buf;
        }
        public static byte[] EncodeCharBE(char val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buf);
            return buf;
        }

        

        


        /// <summary> 
        /// Encodes the specified parameterValue, returning the result as a byte array.
        /// As character encoding use Unicode UTF-16 format with the little-endian byte order 
        /// </summary>
        /// <param name="parameterValue">the parameterValue to Encode
        /// </param>
        /// <returns> a byte array containing the encoded parameterValue
        /// </returns>
        public static byte[] EncodeString(string val)
        {
            return System.Text.Encoding.Unicode.GetBytes(val);
        }

        




        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        /// <exception cref="System.IO.IOException"> if the parameterValue could not be decoded
        /// </exception>
        public static char DecodeChar(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < sizeof(char))
                throw new Exception();
            return BitConverter.ToChar(buffer, 0);
        }
        public static char DecodeChar(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(char))
                throw new Exception();
            return BitConverter.ToChar(buffer, startPos);
        }
        

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// As character encoding use Unicode UTF-16 format with the little-endian byte order 
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        /// <exception cref="System.IO.System.IO.IOException"> if the parameterValue could not be decoded
        /// </exception>
        public static System.String DecodeString(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            return System.Text.Encoding.Unicode.GetString(buffer);
        }
        public static System.String DecodeString(byte[] buffer, int startPos, int count)
        {
            if (buffer == null)
                throw new Exception();
            return System.Text.Encoding.Unicode.GetString(buffer, startPos, count);
        }

        /// <summary> 
        /// Reverses the specified byte array in-place.
        /// </summary>
        /// <param name="bufferStream">the byte array to Reverse
        /// </param>
        public static byte[] ReverseBytes(byte[] inArray)
        {
            byte temp;
            int highCtr = inArray.Length - 1;

            for (int ctr = 0; ctr < inArray.Length / 2; ctr++)
            {
                temp = inArray[ctr];
                inArray[ctr] = inArray[highCtr];
                inArray[highCtr] = temp;
                highCtr -= 1;
            }
            return inArray;
        }

        public static byte[] ReverseBytes(byte[] inArray, int startPos, int length)
        {
            byte temp;
            int highCtr = startPos + length - 1;

            for (int ctr = startPos; ctr < startPos + length / 2; ctr++)
            {
                temp = inArray[ctr];
                inArray[ctr] = inArray[highCtr];
                inArray[highCtr] = temp;
                highCtr -= 1;
            }
            return inArray;
        }
    }
}
