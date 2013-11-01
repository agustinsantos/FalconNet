using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.VU;
using FalconNet.FalcLib;

namespace UnitTestVU
{
    [TestClass]
    public class TestCollections
    {
        const int cnt = 1000;

        #region VuLinkedList Tests

        [TestMethod]
        public void TestVuLinkedListFilter()
        {
            VuEntity entity;
            VuLinkedList list = new VuLinkedList(new VuTypeFilter(VuEntity.VU_SESSION_ENTITY_TYPE));
            entity = new VuEntity((ushort)VuEntity.VU_UNKNOWN_ENTITY_TYPE, (uint)10);
            list.Insert(entity);
            Assert.AreEqual(0, list.l_.Count);
            entity = new VuEntity((ushort)VuEntity.VU_SESSION_ENTITY_TYPE, (uint)10);
            list.Insert(entity);
            Assert.AreEqual(1, list.l_.Count);
        }

        [TestMethod]
        public void TestVuLinkedListInsert()
        {
            VuLinkedList list = new VuLinkedList(null);

            for (int i = 0; i < cnt; i++)
            {
                VuEntity entity = new VuEntity((ushort)0, (uint)i);
                list.Insert(entity);
            }
            Assert.AreEqual(cnt, list.l_.Count);
        }

        [TestMethod]
        public void TestVuLinkedListFind()
        {
            VuLinkedList list = new VuLinkedList(null);

            for (int i = 0; i < cnt; i++)
            {
                VuEntity entity = new VuEntity((ushort)0, (uint)i);
                list.Insert(entity);
            }
            Assert.IsTrue(list.Find(new VuEntity((ushort)0, (uint)cnt / 71)));
        }

        [TestMethod]
        public void TestVuLinkedListRemove()
        {
            VuLinkedList list = new VuLinkedList(null);

            for (int i = 0; i < cnt; i++)
            {
                VuEntity entity = new VuEntity((ushort)0, (uint)i);
                list.Insert(entity);
            }
            for (int i = 0; i < cnt; i++)
            {
                VuEntity entity = new VuEntity((ushort)0, (uint)i);
                list.Remove(entity);
            }
            Assert.AreEqual(0, list.l_.Count);
        }

        [TestMethod]
        public void TestVuLinkedListType()
        {
            VuLinkedList list = new VuLinkedList(null);
            Assert.AreEqual(VU_COLL_TYPE.VU_LINKED_LIST_COLLECTION, list.Type());

        }

        [TestMethod]
        public void TestVuLinkedListCount()
        {
            VuLinkedList list = new VuLinkedList(null);

            for (int i = 0; i < cnt; i++)
            {
                VuEntity entity = new VuEntity((ushort)0, (uint)i);
                entity.vuState_ = (i % 2 == 0 ? VU_MEM_STATE.VU_MEM_ACTIVE : VU_MEM_STATE.VU_MEM_INACTIVE);
                list.Insert(entity);
            }
            Assert.AreEqual(cnt / 2, list.Count());
        }

        [TestMethod]
        public void TestVuLinkedListPurge()
        {
            VuLinkedList list = new VuLinkedList(null);

            for (int i = 0; i < cnt; i++)
            {
                VuEntity entity = new VuEntity((ushort)0, (uint)i);
                list.Insert(entity);
            }
            list.Purge();
            Assert.AreEqual(0, list.l_.Count);
        }
        #endregion

        #region VuListIterator Tests

        [TestMethod]
        public void TestVuListIterator()
        {
            VuLinkedList list = new VuLinkedList(null);

            for (int i = 0; i < cnt; i++)
            {
                VuEntity entity = new VuEntity((ushort)0, (uint)i);
                entity.vuState_ = VU_MEM_STATE.VU_MEM_ACTIVE;
                list.Insert(entity);
            }

            VuListIterator iterator = new VuListIterator(list);
            VuEntity first = iterator.GetFirst();
            Assert.AreEqual(0, (int)first.share_.id_.num_);

            VuEntity current = iterator.CurrEnt();
            Assert.AreEqual(0, (int)current.share_.id_.num_);

            for (int i = 1; i < cnt; i++)
            {
                VuEntity next = iterator.GetNext();
                Assert.AreEqual(i, (int)next.share_.id_.num_);
            }
            VuEntity noEnt = iterator.GetNext();
            Assert.AreEqual(null, noEnt);
        }


        [TestMethod]
        public void TestVuListIteratorCleanup()
        {
            VuLinkedList list = new VuLinkedList(null);

            for (int i = 0; i < cnt; i++)
            {
                VuEntity entity = new VuEntity((ushort)0, (uint)i);
                entity.vuState_ = VU_MEM_STATE.VU_MEM_ACTIVE;
                list.Insert(entity);
            }

            VuListIterator iterator = new VuListIterator(list);
            VuEntity first = iterator.GetFirst();
            iterator.Cleanup();
            VuEntity noEnt = iterator.GetNext();
            Assert.AreEqual(null, noEnt);
        }

        [TestMethod]
        public void TestVuListIteratorFilter()
        {
            VuLinkedList list = new VuLinkedList(null);
            VuEntity entity;
            for (int i = 0; i < cnt; i++)
            {
                if (i % 2 == 0)
                    entity = new VuEntity((ushort)VuEntity.VU_SESSION_ENTITY_TYPE, (uint)i);
                else
                    entity = new VuEntity((ushort)VuEntity.VU_UNKNOWN_ENTITY_TYPE, (uint)i);
                entity.vuState_ = VU_MEM_STATE.VU_MEM_ACTIVE;
                list.Insert(entity);
            }
            VuTypeFilter filter = new VuTypeFilter(VuEntity.VU_SESSION_ENTITY_TYPE);

            VuListIterator iterator = new VuListIterator(list);
            VuEntity first = iterator.GetFirst(filter);
            Assert.AreEqual(0, (int)first.share_.id_.num_);

            int found = 1;
            for (int i = 1; i < cnt / 2; i++)
            {
                VuEntity next = iterator.GetNext(filter);
                Assert.AreEqual(VuEntity.VU_SESSION_ENTITY_TYPE, (int)next.share_.entityType_);
                found++;
            }
            Assert.AreEqual(cnt / 2, found);
        }
        #endregion

        #region VuOrderedList Tests

        [TestMethod]
        public void TestVuOrderedList()
        {
            VuEntity entity;
            VuOrderedList list = new VuOrderedList(new VuTypeFilter(VuEntity.VU_SESSION_ENTITY_TYPE));
            entity = new VuEntity((ushort)VuEntity.VU_SESSION_ENTITY_TYPE, (uint)50);
            list.Insert(entity);
            Assert.AreEqual(1, list.l_.Count);
            entity = new VuEntity((ushort)VuEntity.VU_SESSION_ENTITY_TYPE, (uint)10);
            list.Insert(entity);
            Assert.AreEqual(2, list.l_.Count);
            entity = new VuEntity((ushort)VuEntity.VU_SESSION_ENTITY_TYPE, (uint)30);
            list.Insert(entity);
            Assert.AreEqual(3, list.l_.Count);
        }

        [TestMethod]
        public void TestVuOrderedList2()
        {
            VuEntity entity;
            VuOrderedList list = new VuOrderedList(new VuTypeFilter(VuEntity.VU_SESSION_ENTITY_TYPE));
            entity = new VuEntity((ushort)VuEntity.VU_SESSION_ENTITY_TYPE, (uint)50);
            entity.vuState_ = VU_MEM_STATE.VU_MEM_ACTIVE;
            list.Insert(entity);
            Assert.AreEqual(1, list.l_.Count);

            entity = new VuEntity((ushort)VuEntity.VU_SESSION_ENTITY_TYPE, (uint)10);
            entity.vuState_ = VU_MEM_STATE.VU_MEM_ACTIVE;
            list.Insert(entity);
            Assert.AreEqual(2, list.l_.Count);

            entity = new VuEntity((ushort)VuEntity.VU_SESSION_ENTITY_TYPE, (uint)30);
            entity.vuState_ = VU_MEM_STATE.VU_MEM_ACTIVE;
            list.Insert(entity);
            Assert.AreEqual(3, list.l_.Count);

            VuListIterator iterator = new VuListIterator(list);
            VuEntity first = iterator.GetFirst();
            Assert.AreEqual(10, (int)first.share_.id_.num_);

            VuEntity next = iterator.GetNext();
            Assert.AreEqual(30, (int)next.share_.id_.num_);

            next = iterator.GetNext();
            Assert.AreEqual(50, (int)next.share_.id_.num_);

        }

        #endregion

        #region VuHashTable Tests

        [TestMethod]
        public void TestVuHashTableInsert()
        {
            int num = 5;
            VuHashTable hash = new VuHashTable(null, num);
            VuEntity entity = new VuEntity((ushort)0, (uint)20);
            entity.vuState_ = VU_MEM_STATE.VU_MEM_ACTIVE;
            //entities[i]. = new VU_ID(null, i);
            hash.Insert(entity);
            Assert.AreEqual(num, hash.table_.Length);
            Assert.AreEqual(num, hash.capacity_);
            Assert.AreEqual(1, hash.table_[0].l_.Count);
        }

        [TestMethod]
        public void TestVuHashTableRemove()
        {
            int num = 5;
            VuHashTable hash = new VuHashTable(null, num);
            VuEntity entity = new VuEntity((ushort)0, (uint)20);
            entity.vuState_ = VU_MEM_STATE.VU_MEM_ACTIVE;
            //entities[i]. = new VU_ID(null, i);
            hash.Insert(entity);
            Assert.AreEqual(1, hash.table_[0].l_.Count);

            hash.Remove(new VuEntity((ushort)0, (uint)20));
            Assert.AreEqual(0, hash.table_[0].l_.Count);
        }

        [TestMethod]
        public void TestVuHashTableFind()
        {
            int num = 5;
            VuHashTable hash = new VuHashTable(null, num);
            VuEntity entity = new VuEntity((ushort)0, (uint)20);
            entity.vuState_ = VU_MEM_STATE.VU_MEM_ACTIVE;
            //entities[i]. = new VU_ID(null, i);
            hash.Insert(entity);
            Assert.AreEqual(1, hash.table_[0].l_.Count);

            bool hasEnt = hash.Find(new VuEntity((ushort)0, (uint)20));
            Assert.AreEqual(true, hasEnt);
            hasEnt = hash.Find(new VuEntity((ushort)0, (uint)10));
            Assert.AreEqual(false, hasEnt);
        }

        [TestMethod]
        public void TestVuHashTableFind2()
        {
            int num = 5;
            VuHashTable hash = new VuHashTable(null, num);
            VuEntity entity = new VuEntity((ushort)0, (uint)20);
            entity.vuState_ = VU_MEM_STATE.VU_MEM_ACTIVE;
            //entities[i]. = new VU_ID(null, i);
            hash.Insert(entity);
            Assert.AreEqual(1, hash.table_[0].l_.Count);

            VuEntity rstEntity = hash.Find(new VU_ID((ushort)0, (uint)20));
            Assert.AreEqual(entity, rstEntity);
            rstEntity = hash.Find(new VU_ID((ushort)0, (uint)10));
            Assert.AreEqual(null, rstEntity);
        }

        [TestMethod]
        public void TestVuHashTableCount()
        {
            int num = 5;
            VuHashTable hash = new VuHashTable(null, num);
            for (int i = 0; i < cnt; i++)
            {
                VuEntity entity = new VuEntity((ushort)0, (uint)i);
                entity.vuState_ = VU_MEM_STATE.VU_MEM_ACTIVE;
                hash.Insert(entity);
            }
            for (int i = 0; i < cnt; i++)
            {
                VuEntity entity = new VuEntity((ushort)0, (uint)i + cnt);
                entity.vuState_ = VU_MEM_STATE.VU_MEM_INACTIVE;
                hash.Insert(entity);
            }
            Assert.AreEqual(cnt, hash.Count());
        }

        #endregion

        [TestMethod]
        public void TestVuCollectionManager()
        {
            VuCollectionManager vuCollectionManager = new VuCollectionManager();
        }
        
        #region VuDatabase Tests

        [TestMethod]
        public void TestVuDatabase()
        {
            int num = 5;
            VuDatabase database = new VuDatabase(num);
            VuEntity entity = new VuEntity((ushort)0, (uint)20);
            entity.vuState_ = VU_MEM_STATE.VU_MEM_INACTIVE;
            database.Insert(entity);
        }

        #endregion

    }
}
