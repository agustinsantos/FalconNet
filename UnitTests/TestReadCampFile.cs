using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.F4Common;
using System.IO;
using log4net;
using FalconNet.Campaign;
using FalconNet.FalcLib;

namespace UnitTestVU
{
    [TestClass]
    public class TestReadCampFile
    {
        public static string FalconDirectory = @"C:\Falcon BMS 4.32\";

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            log4net.Config.BasicConfigurator.Configure();
            F4File.FalconDirectory = FalconDirectory;
            F4VUStatic.InitVU();
            EntityDB.LoadClassTable("Falcon4");
        }

        [TestMethod]
        public void TestReadCampaignFiles()
        {
            FileStream file;

            F4File.StartReadCampFile(FalconGameType.game_Campaign, "save0");

            file = F4File.OpenCampFile("save0", "cmp", FileAccess.Read);
            Assert.AreEqual(4, file.Position);
            CampaignClass camp = new CampaignClass();
            CampaignClassEncodingLE.Decode(file, camp);
            Assert.AreEqual("korea", camp.TheaterName);

            file = F4File.OpenCampFile("save0", "obj", FileAccess.Read);
            Assert.AreEqual(4447, file.Position);
            ObjectiveClassListEncodingLE.Decode(file);

            file = F4File.OpenCampFile("save0", "obd", FileAccess.Read);
            Assert.AreEqual(120104, file.Position);
            file = F4File.OpenCampFile("save0", "uni", FileAccess.Read);
            Assert.AreEqual(133297, file.Position);
            file = F4File.OpenCampFile("save0", "tea", FileAccess.Read);
            Assert.AreEqual(184625, file.Position);
            file = F4File.OpenCampFile("save0", "evt", FileAccess.Read);
            Assert.AreEqual(191731, file.Position);
            file = F4File.OpenCampFile("save0", "plt", FileAccess.Read);
            Assert.AreEqual(191821, file.Position);
            file = F4File.OpenCampFile("save0", "pst", FileAccess.Read);
            Assert.AreEqual(194729, file.Position);
            file = F4File.OpenCampFile("save0", "wth", FileAccess.Read);
            Assert.AreEqual(194733, file.Position);
            file = F4File.OpenCampFile("save0", "ver", FileAccess.Read);
            Assert.AreEqual(227538, file.Position);
            file = F4File.OpenCampFile("save0", "te", FileAccess.Read);
            Assert.AreEqual(227540, file.Position);
            F4File.EndReadCampFile();
        }
    }
}
