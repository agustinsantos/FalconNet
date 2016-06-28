using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.F4Common;
using System.IO;
using log4net;
using FalconNet.Campaign;
using FalconNet.FalcLib;
using FalconNet.VU;
using FalconNet.Common.Encoding;
using Acmi;

namespace UnitTestVU
{
    [TestClass]
    public class TestReadAcmiFile
    {
        public static string FalconBMSDirectory = @"C:\Falcon BMS 4.33 U1\User\Acmi";
        public static string Falcon4Directory = @"C:\MicroProse\Falcon4\acmibin";

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            log4net.Config.BasicConfigurator.Configure();
            //log4net.Config.BasicConfigurator.Configure(
            //  new log4net.Appender.ConsoleAppender
            //  {
            //      Layout = new log4net.Layout.SimpleLayout()
            //  });
        }

        [TestMethod]
        public void TestReadVHSFile01()
        {
            ACMITape acmi = new ACMITape("TAPE0000.vhs", null, null, FalconBMSDirectory);
            acmi.SetHeadPosition(0);
        }

        [TestMethod]
        public void TestReadVHSFile02()
        {
            ACMITape acmi = new ACMITape( "demo.vhs", null, null, Falcon4Directory);
            acmi.SetHeadPosition(0);
        }
    }
}
