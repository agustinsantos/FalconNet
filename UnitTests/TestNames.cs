using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.Campaign;
using FalconNet.F4Common;

namespace UnitTestVU
{
    [TestClass]
    public class TestNames
    {
        public static string FalconDirectory = @"C:\Falcon BMS 4.32\data\";

        [TestMethod]
        public void TestLoadNames()
        {
            F4File.FalconDataDirectory = FalconDirectory;
            Name.LoadNames("Strings");
            for (int i = 0; i < Name.NameEntries - 1; i++)
            {
                string txt = Name.ReadNameString(i);
                Console.WriteLine(i + " " + txt);
            }
        }
    }
}
