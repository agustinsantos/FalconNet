using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.Campaign;
using FalconNet.F4Common;

namespace UnitTestVU
{
    [TestClass]
    public class TestAIInput
    {
        public static string FalconDirectory = @"C:\Falcon BMS 4.32\data\";

        [TestMethod]
        public void TestReadCampAIInputs()
        {
            F4File.FalconDataDirectory = FalconDirectory;
            AIInput.ReadCampAIInputs("Falcon4");
        }
    }
}
