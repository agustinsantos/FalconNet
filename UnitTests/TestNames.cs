using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.Campaign;
using FalconNet.F4Common;

namespace UnitTestVU
{
    [TestClass]
    public class TestNames
    {
        public static string FalconDirectory = @"C:\Falcon BMS 4.32\";

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            log4net.Config.BasicConfigurator.Configure();
            F4File.FalconDirectory = FalconDirectory;
        }

        [TestMethod]
        public void TestLoadNames()
        {
            Name.LoadNames("Strings");
            for (int i = 0; i < Name.NameEntries - 1; i++)
            {
                string txt = Name.ReadNameString(i);
                Console.WriteLine(i + " " + txt);
            }
        }
    }
}
