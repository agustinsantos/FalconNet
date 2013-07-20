using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.VU;

namespace UnitTestVU
{
    [TestClass]
    public class TestVuStatic
    {
        [TestMethod]
        public void TestVuVersion()
        {
            Assert.AreEqual(3, VUSTATIC.vuReleaseInfo.version_);
            Assert.AreEqual(1, VUSTATIC.vuReleaseInfo.revision_);
        }
    }
}
