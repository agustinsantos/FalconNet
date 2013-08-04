using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.F4Common;
using FalconNet.FalcLib;

namespace UnitTestVU
{
    [TestClass]
    public class TestTacan
    {
        public static string FalconDirectory = @"C:\Falcon BMS 4.32\";

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            log4net.Config.BasicConfigurator.Configure();
            F4File.FalconDirectory = FalconDirectory;
        }

        [TestMethod]
        public void TestReadTacanListFile()
        {
            TacanList tacanLst = new TacanList();
            
            Assert.AreEqual(101, tacanLst.mpCampList.Count);
            Assert.AreEqual(58, tacanLst.mpCampList[0].campaignID);
            Assert.AreEqual(3700, tacanLst.mpCampList[100].campaignID);
        }

        [TestMethod]
        public void TestGetChannelFromCampID()
        {
            TacanList tacanLst = new TacanList();
            Assert.AreEqual(101, tacanLst.mpCampList.Count);

            int channel;
            StationSet band;
            tacanLst.GetChannelFromCampID(out channel, out band, 58);
            Assert.AreEqual(72, channel);
            Assert.AreEqual(StationSet.X, band);
            tacanLst.GetChannelFromCampID(out channel, out band, 71);
            Assert.AreEqual(58, channel);
            Assert.AreEqual(StationSet.Y, band);

        }
    }
}
