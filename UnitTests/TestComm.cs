using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.VU.Comms;

namespace UnitTestVU
{
    [TestClass]
    public class TestComm
    {

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            log4net.Config.BasicConfigurator.Configure();
            CommApi.ComAPIInitComms();
        }

        [ClassCleanup]
        public static void Cleanup(TestContext context)
        {
            log4net.Config.BasicConfigurator.Configure();
            CommApi.ComAPICloseOpenHandles();
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
