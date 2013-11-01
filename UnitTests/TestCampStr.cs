using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.F4Common;
using FalconNet.Campaign;
using FalconNet.FalcLib;

namespace UnitTestVU
{
    [TestClass]
    public class TestCampStr
    {
        public static string FalconDirectory = @"C:\Falcon BMS 4.32\";

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            log4net.Config.BasicConfigurator.Configure();
            F4File.FalconDirectory = FalconDirectory;
            CampStr.ReadIndex("Strings");
        }

        [TestMethod]
        public void TestReadIndex()
        {
            Assert.AreEqual("None", CampStr.MissStr[0]);
            Assert.AreEqual("Training", CampStr.MissStr[40]);

            Assert.AreEqual("--", CampStr.WPActStr[0]);
            Assert.AreEqual("Secure", CampStr.WPActStr[49]);

            Assert.AreEqual("Nothing", CampStr.AirSTypesStr[0]);
            Assert.AreEqual("Airlift", CampStr.AirSTypesStr[14]);

            Assert.AreEqual("Nothing", CampStr.GroundSTypesStr[0]);
            Assert.AreEqual("Reserve", CampStr.GroundSTypesStr[15]);

            Assert.AreEqual("Nothing", CampStr.NavalSTypesStr[0]);
            Assert.AreEqual("Tanker", CampStr.NavalSTypesStr[9]);

            Assert.AreEqual("XX", CampStr.CountryNameStr[0]);
            Assert.AreEqual("Gorn", CampStr.CountryNameStr[7]);

            Assert.AreEqual("PAUSED", CampStr.CompressionStr[0]);
            Assert.AreEqual("FREEZE", CampStr.CompressionStr[4]);

            Assert.AreEqual("FLY-BY CAMERA", CampStr.CameraLabel[0]);
            Assert.AreEqual("PAUSED", CampStr.CameraLabel[15]);

            Assert.AreEqual("{0}, {1}", CampStr.gUnitNameFormat);
        }

        [TestMethod]
        public void TestGetSTypeName()
        {
            Assert.AreEqual("Airlift", CampStr.GetSTypeName(Domains.DOMAIN_AIR, ClassTypes.TYPE_NOTHING, 1));
            Assert.AreEqual("Air Defense", CampStr.GetSTypeName(Domains.DOMAIN_LAND, ClassTypes.TYPE_NOTHING, 1));
            Assert.AreEqual("Amphibious", CampStr.GetSTypeName(Domains.DOMAIN_SEA, ClassTypes.TYPE_NOTHING, 1));
        }

        [TestMethod]
        public void TestGetNumberName()
        {
            F4Config.gLangIDNum = F4LANGUAGE.F4LANG_ENGLISH;
            Assert.AreEqual("1st", CampStr.GetNumberName(1));
            Assert.AreEqual("2nd", CampStr.GetNumberName(2));
            Assert.AreEqual("3rd", CampStr.GetNumberName(3));
            Assert.AreEqual("10th", CampStr.GetNumberName(10));
            Assert.AreEqual("11th", CampStr.GetNumberName(11));
            Assert.AreEqual("12th", CampStr.GetNumberName(12));
            Assert.AreEqual("13th", CampStr.GetNumberName(13));
        }

        [TestMethod]
        public void TestGetTimeString()
        {
            Assert.AreEqual("00:01:00", CampStr.GetTimeString(1 * 60 * 1000));
            Assert.AreEqual("10:00:00", CampStr.GetTimeString(10 * 60 * 60 * 1000));
            Assert.AreEqual("10:30:00", CampStr.GetTimeString((10 * 60 + 30) * 60 * 1000));
            Assert.AreEqual("10:30:30", CampStr.GetTimeString(((10 * 60 + 30) * 60 + 30) * 1000));
            Assert.AreEqual("18:51:30", CampStr.GetTimeString(67890 * 1000));

            Assert.AreEqual("00:01", CampStr.GetTimeString(1 * 60 * 1000, false));
            Assert.AreEqual("10:00", CampStr.GetTimeString(10 * 60 * 60 * 1000, false));
            Assert.AreEqual("10:30", CampStr.GetTimeString((10 * 60 + 30) * 60 * 1000, false));
            Assert.AreEqual("10:30", CampStr.GetTimeString(((10 * 60 + 30) * 60 + 30) * 1000, false));
            Assert.AreEqual("18:51", CampStr.GetTimeString(67890 * 1000, false));

        }
    }
}
