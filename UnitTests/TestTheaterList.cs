using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.F4Common;
using FalconNet.FalcLib;

namespace UnitTestVU
{
    [TestClass]
    public class TestTheaterList
    {
        public static string FalconDirectory = @"C:\Falcon BMS 4.32\";

        [TestMethod]
        public void TestLoadTheaterList()
        {
            F4File.FalconDirectory = FalconDirectory;
            TheaterList tlist = new TheaterList();
            tlist.LoadTheaterList();

            Assert.AreEqual(5, tlist.Count());
            foreach (TheaterDef theater in tlist)
            {
                Console.WriteLine("Theater:" + theater.m_name + ", Description:" + theater.m_description);
            }
        }
    }
}
