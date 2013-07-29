using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.VU;
using FalconNet.FalcLib;

namespace UnitTestVU
{
    [TestClass]
    public class TestCollections
    {
        [TestMethod]
        public void TestVuHashTableInsertFind()
        {
            int cnt = 1000;
            VuHashTable hash = new VuHashTable(cnt);
            VuEntity[] entities = new VuEntity[cnt];
            for (int i = 0; i < cnt; i++)
            {
                entities[i] = new VuEntity((ushort)0, (ulong)i);
                entities[i].vuState_ = VU_MEM_STATE.VU_MEM_ACTIVE;
                //entities[i]. = new VU_ID(null, i);
                hash.Insert(entities[i]);
            }
            for (int i = 0; i < cnt; i++)
            {
                VuEntity entity = hash.Find(entities[i].Id());
                Assert.AreEqual(entity, entities[i]);
            }
        }

        [TestMethod]
        public void TestVuHashTableInsertRemove()
        {
            F4VUStatic.InitVU();
            int cnt = 1000;
            VuHashTable hash = new VuHashTable(cnt);
            VuEntity[] entities = new VuEntity[cnt];
            for (int i = 0; i < cnt; i++)
            {
                entities[i] = new VuEntity((ushort)0, (ulong)i);
                entities[i].vuState_ = VU_MEM_STATE.VU_MEM_ACTIVE;
                //entities[i]. = new VU_ID(null, i);
                hash.Insert(entities[i]);
            }
            for (int i = 0; i < cnt; i++)
            {
                VuEntity entity = hash.Find(entities[i].Id());
                hash.Remove(entity);
                entity = hash.Find(entities[i].Id());
                Assert.AreEqual(null, entity);
            }
        }
    }
}
