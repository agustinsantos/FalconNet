using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.F4Common;
using System.IO;
using log4net;
using FalconNet.Campaign;
using FalconNet.FalcLib;
using FalconNet.VU;
using FalconNet.Common.Encoding;

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
            VUSTATIC.vuLocalSessionEntity.game_ = new FalconGameEntity(0, "DummyTest");
        }

        [TestMethod]
        public void TestReadCampaignFiles()
        {
            // More information on CampaignClass::LoadCampaign(FalconGameType gametype, char *savefile), CmpClass line 423
            F4File.StartReadCampFile(FalconGameType.game_Campaign, "save0");

            using (Stream file = F4File.ReadCampFile("save0", "cmp"))
            {
                Assert.AreEqual(0, file.Position);
                Assert.AreEqual(4443, file.Length);
                CampaignClass camp = new CampaignClass();
                CampaignClassEncodingLE.Decode(file, camp);
                Assert.AreEqual("korea", camp.TheaterName);
            }

            using (Stream file = F4File.ReadCampFile("save0", "obj"))
            {
                Assert.AreEqual(0, file.Position);
                Assert.AreEqual(115657, file.Length);
            }
            ObjectivStatic.LoadBaseObjectives("save0");

            using (Stream file = F4File.ReadCampFile("save0", "obd"))
            {
                Assert.AreEqual(0, file.Position);
                Assert.AreEqual(13193, file.Length);
                int csize = Int32EncodingLE.Decode(file);
                // There is a dependency between CampBaseClass and ObjetiveClass
                //TODO ObjectivStatic.DecodeObjectiveDeltas(file, null);
            }
            using (Stream file = F4File.ReadCampFile("save0", "uni"))
            {
                Assert.AreEqual(0, file.Position);
                Assert.AreEqual(51328, file.Length);
            }
            using (Stream file = F4File.ReadCampFile("save0", "tea"))
            {
                Assert.AreEqual(0, file.Position);
                Assert.AreEqual(7106, file.Length);
            }
            using (Stream file = F4File.ReadCampFile("save0", "evt"))
            {
                Assert.AreEqual(0, file.Position);
                Assert.AreEqual(90, file.Length);
            }
            using (Stream file = F4File.ReadCampFile("save0", "plt"))
            {
                Assert.AreEqual(0, file.Position);
                Assert.AreEqual(2908, file.Length);
            }
            using (Stream file = F4File.ReadCampFile("save0", "pst"))
            {
                Assert.AreEqual(0, file.Position);
                Assert.AreEqual(4, file.Length);
            }
            using (Stream file = F4File.ReadCampFile("save0", "wth"))
            {
                Assert.AreEqual(0, file.Position);
                Assert.AreEqual(32805, file.Length);
            }
            using (Stream file = F4File.ReadCampFile("save0", "ver"))
            {
                Assert.AreEqual(0, file.Position);
                Assert.AreEqual(2, file.Length);
                using (StreamReader reader = new StreamReader(file))
                {
                    String line = reader.ReadToEnd();
                    int vers = int.Parse(line);
                    Assert.AreEqual(73, vers);
                }
            }
            using (Stream file = F4File.ReadCampFile("save0", "te"))
            {
                Assert.AreEqual(0, file.Position);
                Assert.AreEqual(86, file.Length);
            }
            F4File.EndReadCampFile();
        }
    }
}
