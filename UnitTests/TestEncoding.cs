using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.VU;
using FalconNet.Common.Encoding;

namespace UnitTestVU
{
    [TestClass]
    public class TestEncoding
    {
        [TestMethod]
        public void TestVU_SESSION_ID()
        {
            ByteWrapper buffer = new ByteWrapper(VU_SESSION_IDEncodingLE.Size);
            VU_SESSION_ID val = new VU_SESSION_ID(123456789);
            VU_SESSION_IDEncodingLE.Encode(buffer, val);
            buffer.Reset();
            VU_SESSION_ID val2 = VU_SESSION_IDEncodingLE.Decode(buffer);
            Assert.AreEqual(val, val2);
           
        }

        [TestMethod]
        public void TestVU_ID()
        {
            ByteWrapper buffer = new ByteWrapper(VU_IDEncodingLE.Size);
            VU_ID val = new VU_ID(123456789,9876543210);
            VU_IDEncodingLE.Encode(buffer, val);
            buffer.Reset();
            VU_ID val2 = VU_IDEncodingLE.Decode(buffer);
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
    }
}
