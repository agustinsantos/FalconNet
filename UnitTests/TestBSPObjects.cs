using FalconNet.F4Common;
using FalconNet.Graphics;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestVU
{

    [TestClass]
    public class TestBSPObjects
    {
        public static string FalconDirectory = @"C:\Falcon BMS 4.32\";

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            //log4net.Config.BasicConfigurator.Configure();
             XmlConfigurator.Configure(new System.IO.FileInfo("log4netconfig.xml"));
            F4File.FalconDirectory = FalconDirectory;
        }

        [TestMethod]
        public void TestObjectParentSetupTable()
        {
            ObjectParent.SetupTable("KoreaObj");
        }
    }
}
