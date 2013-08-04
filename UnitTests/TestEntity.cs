using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.F4Common;
using FalconNet.FalcLib;

namespace UnitTestVU
{
    [TestClass]
    public class TestEntity
    {
        public static string FalconDirectory = @"C:\Falcon BMS 4.32\";

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            F4File.FalconDirectory = FalconDirectory;
        }

        [TestMethod]
        public void TestLoadClassTable()
        {
            EntityDB.LoadClassTable("Falcon4");
        }

        [TestMethod]
        public void TestLoadUnitData()
        {
            EntityDB.LoadUnitData("Falcon4");
        }

        [TestMethod]
        public void TestLoadFeatureEntryData()
        {
            EntityDB.LoadFeatureEntryData("Falcon4");
        }

        [TestMethod]
        public void TestLoadObjectiveData()
        {
            EntityDB.LoadObjectiveData("Falcon4"); 
        }

        [TestMethod]
        public void TestLoadWeaponData()
        {
            EntityDB.LoadWeaponData("Falcon4");
        }

        [TestMethod]
        public void TestLoadFeatureData()
        {
            EntityDB.LoadFeatureData("Falcon4"); 
        }

        [TestMethod]
        public void TestLoadVehicleData()
        {
            EntityDB.LoadVehicleData("Falcon4"); 
        }

        [TestMethod]
        public void TestLoadWeaponListData()
        {
            EntityDB.LoadWeaponListData("Falcon4");
        }

        [TestMethod]
        public void TestLoadPtHeaderData()
        {
            EntityDB.LoadPtHeaderData("Falcon4"); 
        }

        [TestMethod]
        public void TestLoadPtData()
        {
            EntityDB.LoadPtData("Falcon4");
        }

        [TestMethod]
        public void TestLoadRadarData()
        {
            EntityDB.LoadRadarData("Falcon4");
        }

        [TestMethod]
        public void TestLoadIRSTData()
        {
            EntityDB.LoadIRSTData("Falcon4");
        }

        [TestMethod]
        public void TestLoadRwrData()
        {
            EntityDB.LoadRwrData("Falcon4");
        }

        [TestMethod]
        public void TestLoadVisualData()
        {
            EntityDB.LoadVisualData("Falcon4");
        }

        [TestMethod]
        public void TestLoadSimWeaponData()
        {
            EntityDB.LoadSimWeaponData("Falcon4");
        }

        [TestMethod]
        public void TestLoadACDefData()
        {
            EntityDB.LoadACDefData("Falcon4");
        }

        [TestMethod]
        public void TestLoadSquadronStoresData()
        {
            EntityDB.LoadSquadronStoresData("Falcon4");
        }

        [TestMethod]
        public void TestLoadRocketData()
        {
            EntityDB.LoadRocketData("Falcon4");//aqui..
        }

        [TestMethod]
        public void TestLoadDirtyData()
        {
            EntityDB.LoadDirtyData("Falcon4");
        }

        [TestMethod]
        public void TestLoadRackTables()
        {
            EntityDB.LoadRackTables();
        }
    }
}
