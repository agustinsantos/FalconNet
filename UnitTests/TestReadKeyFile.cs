using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.F4Common;
using System.IO;
using log4net;
using FalconNet.Campaign;
using FalconNet.FalcLib;
using FalconNet.VU;
using FalconNet.Common.Encoding;
using Acmi;
using FalconNet.SimBase.SimInput;
using System.Diagnostics;

namespace UnitTestVU
{
    [TestClass]
    public class TestReadKeyFile
    {
        public static string Falcon4Directory = @"C:\MicroProse\Falcon4";

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            log4net.Config.BasicConfigurator.Configure();
            F4File.FalconDirectory = Falcon4Directory;
        }

        [TestMethod]
        public void TestReadDefaultKeyFile()
        {
            try
            {
                InputFunctionHashTable.LoadFunctionTables("laptop");
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
        }
    }
}
