using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.VU;

namespace UnitTestVU
{
    [TestClass]
    public class TestCollections
    {
        [TestMethod]
        public void TestVuHashTable()
        {
            int cnt = 10;
            VuHashTable hash = new VuHashTable(cnt);
            VuEntity[] entities = new VuEntity[cnt];
            for (int i = 0; i < cnt; i++)
            {
                entities[i] = new VuEntity();
                //entities[i]. = new VU_ID(null, i);
                hash.Insert(entities[i]);
            }
            VuEntity entity = hash.Find(entities[0].Id());
            Assert.AreEqual(entity, entities[0]);
        }
    }
}
