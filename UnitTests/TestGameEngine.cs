using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.F4Common;

namespace UnitTestVU
{
    [TestClass]
    public class TestGameEngine
    {
        [TestMethod]
        public void TestGame()
        {
            GameEngine game = GameEngine.Instance;
            game.Initialize();

            Assert.AreEqual(21189L, game.ResourcesManager.IdFromName("DF_AC_CH46"));
            Assert.AreEqual(131034L, game.ResourcesManager.IdFromName("WIND_E"));
            Assert.AreEqual(1600010L, game.ResourcesManager.IdFromName("BIG_MAP_ID"));
        }
    }
}
