﻿using System;
using System.Diagnostics;
using System.IO;

namespace FalconNet.Common.Encoding
{
    #region Encodign Little-Endian
    public class UInt64EncodingLE
    {
        /// <summary> 
        /// Encodes the specified value, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the value to Encode
        /// </param>
        /// <returns> 
        /// a byte array containing the encoded value
        /// An array of bytes with length 8.
        /// </returns>
        public static byte[] Encode(UInt64 val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (EncodingHelpers.Endian != EndianTypes.LittleEndian)
                EncodingHelpers.ReverseBytes(buf);
            return buf;
        }

        public static void Encode(Stream stream, UInt64 val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (EncodingHelpers.Endian != EndianTypes.LittleEndian)
                EncodingHelpers.ReverseBytes(buf);
            stream.Write(buf, 0, buf.Length);
        }

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        public static UInt64 Decode(byte[] buffer)
        {
            Debug.Assert(buffer != null, "Buffer is null");
            Debug.Assert(buffer.Length >= Size, "length = " + buffer.Length);
            if (EncodingHelpers.Endian != EndianTypes.LittleEndian)
                EncodingHelpers.ReverseBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
        public static UInt64 Decode(byte[] buffer, int startPos)
        {
            Debug.Assert(buffer != null, "Buffer is null");
            Debug.Assert(buffer.Length >= Size, "length = " + buffer.Length);
            if (EncodingHelpers.Endian != EndianTypes.LittleEndian)
                EncodingHelpers.ReverseBytes(buffer, startPos, Size);
            return BitConverter.ToUInt64(buffer, startPos);
        }

        public static UInt64 Decode(Stream stream)
        {
            byte[] buffer = new byte[Size];
            stream.Read(buffer, 0, Size);

            return Decode(buffer, 0);
        }

        public static int Size
        {
            get { return sizeof(UInt64); }
        }
    }
    #endregion

    #region Encodign Big-Endian
    public class UInt64EncodingBE
    {

        /// <summary> 
        /// Encodes the specified value, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the value to Encode
        /// </param>
        /// <returns> 
        /// a byte array containing the encoded value
        /// An array of bytes with length 2.
        /// </returns>
        public static byte[] Encode(UInt64 val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (EncodingHelpers.Endian != EndianTypes.BigEndian)
                EncodingHelpers.ReverseBytes(buf);
            return buf;
        }


        public static void Encode(Stream stream, UInt64 val)
        {
            byte[] buf = BitConverter.GetBytes(val);
            if (EncodingHelpers.Endian != EndianTypes.BigEndian)
                EncodingHelpers.ReverseBytes(buf);
            stream.Write(buf, 0, buf.Length);
        }

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        public static UInt64 Decode(byte[] buffer)
        {
            Debug.Assert(buffer != null, "Buffer is null");
            Debug.Assert(buffer.Length >= Size, "length = " + buffer.Length);
            if (EncodingHelpers.Endian != EndianTypes.BigEndian)
                EncodingHelpers.ReverseBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
        public static UInt64 Decode(byte[] buffer, int startPos)
        {
            Debug.Assert(buffer != null, "Buffer is null");
            Debug.Assert(buffer.Length >= Size, "length = " + buffer.Length);
            if (EncodingHelpers.Endian != EndianTypes.BigEndian)
                EncodingHelpers.ReverseBytes(buffer, startPos, Size);
            return BitConverter.ToUInt64(buffer, startPos);
        }

        public static UInt64 Decode(Stream stream)
        {
            byte[] buffer = new byte[Size];
            stream.Read(buffer, 0, Size);

            return Decode(buffer, 0);
        }

        public static int Size
        {
            get { return sizeof(UInt64); }
        }
    }
    #endregion
}
