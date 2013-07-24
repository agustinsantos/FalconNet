using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.F4Common;

namespace UnitTestVU
{
    [TestClass]
    public class TestLogbook
    {
        public static string FalconDirectory = @"C:\Falcon BMS 4.32\";

        [TestMethod]
        public void TestLoadLogbook()
        {
            F4File.FalconDirectory = FalconDirectory;
            //pilot.LoadData("Viper");
            LogBookData.LogBook.LoadData("Auphim");
        }
    }
}
