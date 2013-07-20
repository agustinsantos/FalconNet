using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.F4Common;
using FalconNet.FalcLib;

namespace UnitTestVU
{
    [TestClass]
    public class TestEntity
    {
        public static string FalconDirectory = @"C:\Falcon BMS 4.32\data\";

        [TestMethod]
        public void TestLoadClassTable()
        {
            F4File.FalconDataDirectory = FalconDirectory;
            EntityDB.LoadClassTable("Falcon4");
        }
    }
}
