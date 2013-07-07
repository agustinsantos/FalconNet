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
