using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.VU;
using FalconNet.Common.Encoding;
using System.IO;
using System.Diagnostics;

namespace UnitTestVU
{
    [TestClass]
    public class TestEncoding
    {
        [TestMethod]
        public void TestInt16LE()
        {
            MemoryStream buffer = new MemoryStream(Int16EncodingLE.Size);
            Assert.AreEqual(true, buffer.CanRead);
            Assert.AreEqual(true, buffer.CanWrite);
            Int16 val = 12345;
            Int16EncodingLE.Encode(buffer, val);
            buffer.Seek(0, SeekOrigin.Begin);
            Int16 val2 = 0;
            Int16EncodingLE.Decode(buffer, ref val2);
            Assert.AreEqual(val, val2);
            Assert.AreEqual(Int16EncodingLE.Size, buffer.Position);
        }

        [TestMethod]
        public void TestInt16BE()
        {
            MemoryStream buffer = new MemoryStream(Int16EncodingBE.Size);
            Assert.AreEqual(true, buffer.CanRead);
            Assert.AreEqual(true, buffer.CanWrite);
            Int16 val = 12345;
            Int16EncodingBE.Encode(buffer, val);
            buffer.Seek(0, SeekOrigin.Begin);
            Int16 val2 = 0;
            Int16EncodingBE.Decode(buffer, ref val2);
            Assert.AreEqual(val, val2);
            Assert.AreEqual(Int16EncodingBE.Size, buffer.Position);
        }

        [TestMethod]
        public void TestPerformanceInt16LE()
        {
            TestPerformanceBuffer();
            TestPerformanceMemoryStream();
        }

        [TestMethod]
        public void TestVU_SESSION_ID()
        {
            MemoryStream buffer = new MemoryStream(VU_SESSION_IDEncodingLE.Size);
            VU_SESSION_ID val = new VU_SESSION_ID(123456789);
            VU_SESSION_IDEncodingLE.Encode(buffer, val);
            buffer.Seek(0, SeekOrigin.Begin);
            VU_SESSION_ID val2 = new VU_SESSION_ID();
            VU_SESSION_IDEncodingLE.Decode(buffer, val2);
            Assert.AreEqual(val, val2);
           
        }

        [TestMethod]
        public void TestVU_ID()
        {
            MemoryStream buffer = new MemoryStream(VU_IDEncodingLE.Size);
            VU_ID val = new VU_ID(123456789,9876543210);
            VU_IDEncodingLE.Encode(buffer, val);
            buffer.Seek(0, SeekOrigin.Begin);
            VU_ID val2 = new VU_ID();
            VU_IDEncodingLE.Decode(buffer, val2);
            Assert.AreEqual(val, val2);

        }

        [TestMethod]
        public void TestVuFlagBits()
        {
            VuFlagBits flags = new VuFlagBits();
            flags.collidable_ = true;
            flags.global_ = false;
            flags.pad_ = 0;
            flags.persistent_ = false;
            flags.private_ = true;
            flags.tangible_ = false;
            flags.transfer_ = true;

            ushort val = (ushort)flags;
            VuFlagBits flags2 = (VuFlagBits)val;
            Assert.AreEqual(flags, flags2);
           
            flags.collidable_ = false;
            flags.global_ = true;
            flags.pad_ = 0;
            flags.persistent_ = true;
            flags.private_ = false;
            flags.tangible_ = false;
            flags.transfer_ = true;

             val = (ushort)flags;
             flags2 = (VuFlagBits)val;
            Assert.AreEqual(flags, flags2);
        }

        #region Performance tests

        private const int MSGNUMBER = 2000;
        private const int OBJNUMBER = 1000;
        private static void TestPerformanceBuffer()
        {
            Int16 val = 12345;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < MSGNUMBER; i++)
            {
                ByteStream buff = new ByteStream(Int16EncodingLE.Size * OBJNUMBER);
                for (int j = 0; j < OBJNUMBER; j++)
                    Int16EncodingLE.Encode(buff, val);

                buff.Reset();
                for (int j = 0; j < OBJNUMBER; j++)
                {
                    Int16 val2 = 0;
                    Int16EncodingLE.Decode(buff, ref val2);
                    //Debug.Assert (cs.Equals (cs2));
                }
            }

            sw.Stop();
            Console.WriteLine("Time Elapsed with Buffer ={0}", sw.Elapsed);
        }
        
        private static void TestPerformanceMemoryStream()
        {
            Int16 val = 12345;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < MSGNUMBER; i++)
            {
                MemoryStream memStream = new MemoryStream(Int16EncodingLE.Size * OBJNUMBER);
                for (int j = 0; j < OBJNUMBER; j++)
                    Int16EncodingLE.Encode(memStream, val);

                memStream.Seek(0, SeekOrigin.Begin);
                for (int j = 0; j < OBJNUMBER; j++)
                {
                    Int16 val2 = 0;
                    Int16EncodingLE.Decode(memStream, ref val2);
                    //Debug.Assert (cs.Equals (cs2));
                }
            }

            sw.Stop();
            Console.WriteLine("Time Elapsed with MemoryStream ={0}", sw.Elapsed);
        }

        
        #endregion
    }
}
