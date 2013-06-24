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
    /// Utility methods for encoding and decoding basic types, modeled after those
    /// supplied with the DMSO RTI.
    /// </summary>
    /// <author> 
    /// Agustin Santos. Based on code originally written by Andrzej Kapolka
    /// </author>
    public class EncodingHelpers
    {
        public static readonly EndianTypes Endian = (BitConverter.IsLittleEndian ? EndianTypes.LittleEndian : EndianTypes.BigEndian);

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

        /// <summary> 
        /// Encodes the specified parameterValue, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the parameterValue to Encode
        /// </param>
        /// <returns>
        /// a byte array containing the encoded parameterValue
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

        /// <summary> 
        /// Encodes the specified parameterValue, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the parameterValue to Encode
        /// </param>
        /// <returns> 
        /// a byte array containing the encoded parameterValue
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

        /// <summary> 
        /// Encodes the specified parameterValue, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the parameterValue to Encode
        /// </param>
        /// <returns> 
        /// a byte array containing the encoded parameterValue
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

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        /// <exception cref="System.IO.IOException"> if the parameterValue could not be decoded
        /// </exception>
        public static int DecodeIntLE(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < sizeof(int))
                throw new Exception();
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }
        public static int DecodeIntBE(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < sizeof(int))
                throw new Exception();
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }
        public static int DecodeIntLE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(int))
                throw new Exception();
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer, startPos, sizeof(int));
            return BitConverter.ToInt32(buffer, startPos);
        }
        public static int DecodeIntBE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(int))
                throw new Exception();
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer, startPos, sizeof(int));
            return BitConverter.ToInt32(buffer, startPos);
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
        public static long DecodeLongLE(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < sizeof(long))
                throw new Exception();
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }
        public static long DecodeLongBE(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < sizeof(long))
                throw new Exception();
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }
        public static long DecodeLongLE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(long))
                throw new Exception();
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer, startPos, sizeof(long));
            return BitConverter.ToInt64(buffer, startPos);
        }
        public static long DecodeLongBE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(long))
                throw new Exception();
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer, startPos, sizeof(long));
            return BitConverter.ToInt64(buffer, startPos);
        }

        public static ulong DecodeULongLE(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < sizeof(ulong))
                throw new Exception();
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
        public static ulong DecodeULongBE(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < sizeof(ulong))
                throw new Exception();
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
        public static ulong DecodeULongLE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(ulong))
                throw new Exception();
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer, startPos, sizeof(ulong));
            return BitConverter.ToUInt64(buffer, startPos);
        }
        public static ulong DecodeULongBE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(ulong))
                throw new Exception();
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer, startPos, sizeof(ulong));
            return BitConverter.ToUInt64(buffer, startPos);
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
                throw new Exception();
            if (buffer.Length < sizeof(short))
                throw new Exception();
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer);
            return BitConverter.ToInt16(buffer, 0);
        }
        public static short DecodeShortBE(byte[] buffer)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < sizeof(short))
                throw new Exception();
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer);
            return BitConverter.ToInt16(buffer, 0);
        }
        public static short DecodeShortLE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(short))
                throw new Exception();
            if (Endian != EndianTypes.LittleEndian)
                ReverseBytes(buffer, startPos, sizeof(short));
            return BitConverter.ToInt16(buffer, startPos);
        }
        public static short DecodeShortBE(byte[] buffer, int startPos)
        {
            if (buffer == null)
                throw new Exception();
            if (buffer.Length < startPos + sizeof(short))
                throw new Exception();
            if (Endian != EndianTypes.BigEndian)
                ReverseBytes(buffer, startPos, sizeof(short));
            return BitConverter.ToInt16(buffer, startPos);
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
