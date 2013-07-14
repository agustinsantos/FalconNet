﻿using System;
using System.Diagnostics;
using System.IO;

namespace FalconNet.Common.Encoding
{
    #region Encodign Little-Endian
    public class StringASCIIEncodingLE
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
        public static byte[] Encode(String val)
        {
            byte[] buf = System.Text.Encoding.ASCII.GetBytes(val);
            return buf;
        }

        public static void Encode(ByteWrapper buffer, String val)
        {

            byte[] buf = System.Text.Encoding.ASCII.GetBytes(val);
            Int32EncodingLE.Encode(buffer, buf.Length);
            buffer.Put(buf, 0, buf.Length);
        }

        public static void Encode(Stream stream, String val)
        {
            byte[] buf = System.Text.Encoding.ASCII.GetBytes(val);
            Int32EncodingLE.Encode(stream, buf.Length);
            stream.Write(buf, 0, buf.Length);
        }

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        public static String Decode(byte[] buffer)
        {
            throw new NotImplementedException();
        }
        public static String Decode(byte[] buffer, int startPos)
        {
            throw new NotImplementedException();
        }
        public static String Decode(ByteWrapper buffer)
        {
            int len = buffer.GetByte();
            len &= 0xff;
            byte[] buf = buffer.GetBytes(len);
            return System.Text.Encoding.ASCII.GetString(buf);
        }
        public static String Decode(Stream stream)
        {
            int len = stream.ReadByte();
            len &= 0xff;
            byte[] buf = stream.ReadBytes(0, len);
            return System.Text.Encoding.ASCII.GetString(buf);
        }

        public static int Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
    #endregion

    #region Encodign Big-Endian
    public class StringASCIIEncodingBE
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
        public static byte[] Encode(String val)
        {
            byte[] buf = System.Text.Encoding.Unicode.GetBytes(val);
            return buf;
        }

        public static void Encode(ByteWrapper buffer, String val)
        {
            byte[] buf = System.Text.Encoding.Unicode.GetBytes(val);
            Int32EncodingLE.Encode(buffer, buf.Length);
            buffer.Put(buf, 0, buf.Length);
        }

        public static void Encode(Stream stream, String val)
        {
            byte[] buf = System.Text.Encoding.Unicode.GetBytes(val);
            Int32EncodingBE.Encode(stream, buf.Length);
            stream.Write(buf, 0, buf.Length);
        }

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        public static String Decode(byte[] buffer)
        {
            throw new NotImplementedException();
        }
        public static String Decode(byte[] buffer, int startPos)
        {
            throw new NotImplementedException();
        }
        public static String Decode(ByteWrapper buffer)
        {
            int len = buffer.GetByte();
            len &= 0xff;
            byte[] buf = buffer.GetBytes(len);
            return System.Text.Encoding.ASCII.GetString(buf);
        }
        public static String Decode(Stream stream)
        {
            int len = stream.ReadByte();
            len &= 0xff;
            byte[] buf = stream.ReadBytes(0, len);
            return System.Text.Encoding.ASCII.GetString(buf);
        }

        public static int Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
    #endregion
}
