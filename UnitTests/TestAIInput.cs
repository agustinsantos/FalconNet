using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.Campaign;
using FalconNet.F4Common;

namespace UnitTestVU
{
    [TestClass]
    public class TestAIInput
    {
        public static string FalconDirectory = @"C:\Falcon BMS 4.32\";

        [TestMethod]
        public void TestReadCampAIInputs()
        {
            F4File.FalconDirectory = FalconDirectory;
            AIInput.ReadCampAIInputs("Falcon4");
        }
    }
}
