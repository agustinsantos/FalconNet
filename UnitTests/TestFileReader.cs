using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FalconNet.F4Common;
using FalconNet.Common.Maths;

namespace UnitTestVU
{
    public class Sample
    {
        public int intVal;
        public float floatVal;
        public string strVal;
        public vector vecVal;
    }

    [TestClass]
    public class TestFileReader
    {
        private static InputDataDesc[]  fields = new InputDataDesc[] {
                new InputDataDesc { name = "StrVal", defvalue = "def", type = format.ID_STRING, action = (o, v) => (o as Sample).strVal = (string)v},
                new InputDataDesc { name ="IntVal", defvalue=10, type =format.ID_INT, action= (o, v) => (o as Sample).intVal = (int)v},
                new InputDataDesc { name = "FloatVal", defvalue = 20.123f, type = format.ID_FLOAT, action = (o, v) => (o as Sample).floatVal = (float)v },
                new InputDataDesc { name = "VectorVal", defvalue = new vector(11.0f, 12.0f, 13.0f), type = format.ID_VECTOR, action = (o, v) => (o as Sample).vecVal = (vector)v }
                };

        [TestMethod]
        public void TestAssignFieldInt()
        {
            InputDataDesc field = new InputDataDesc { name ="IntVal", defvalue=10, type =format.ID_INT, action= (o, v) => (o as Sample).intVal = (int)v};
            FileReader reader = new FileReader(null);
            Sample obj = new Sample();
            reader.AssignField(field, obj, "20");

            Assert.AreEqual(20, obj.intVal);
        }

        [TestMethod]
        public void TestAssignFieldFloat()
        {
            InputDataDesc field = new InputDataDesc { name = "FloatVal", defvalue = 10.0f, type = format.ID_FLOAT, action = (o, v) => (o as Sample).floatVal = (float)v };
            FileReader reader = new FileReader(null);
            Sample obj = new Sample();
            reader.AssignField(field, obj, "3.14");

            Assert.AreEqual(3.14f, obj.floatVal);
        }
        
        [TestMethod]
        public void TestAssignFieldStr()
        {
            InputDataDesc field = new InputDataDesc { name = "StrVal", defvalue = "", type = format.ID_STRING, action = (o, v) => (o as Sample).strVal = (string)v };
            FileReader reader = new FileReader(null);
            Sample obj = new Sample();
            reader.AssignField(field, obj, "hello");

            Assert.AreEqual("hello", obj.strVal);
        }

        [TestMethod]
        public void TestAssignFieldVec()
        {
            InputDataDesc field = new InputDataDesc { name = "VectorVal", defvalue = new vector(1.0f, 1.0f, 1.0f), type = format.ID_VECTOR, action = (o, v) => (o as Sample).vecVal = (vector)v };
            FileReader reader = new FileReader(null);
            Sample obj = new Sample();
            reader.AssignField(field, obj, "1.0, 2.0, 3.0");

            Assert.AreEqual(1.0f, obj.vecVal.x);
            Assert.AreEqual(2.0f, obj.vecVal.y);
            Assert.AreEqual(3.0f, obj.vecVal.z);
        }

        [TestMethod]
        public void TestInitialise()
        {
            FileReader reader = new FileReader(fields);
            Sample obj = new Sample();
            reader.Initialise(obj);

            Assert.AreEqual(10, obj.intVal);
            Assert.AreEqual(20.123f, obj.floatVal);
            Assert.AreEqual("def", obj.strVal);
            Assert.AreEqual(11.0f, obj.vecVal.x);
            Assert.AreEqual(12.0f, obj.vecVal.y);
            Assert.AreEqual(13.0f, obj.vecVal.z);
        }

        [TestMethod]
        public void TestParseFieldStr()
        {
            FileReader reader = new FileReader(fields);
            Sample obj = new Sample();
            reader.ParseField(obj, "StrVal A string value");

            Assert.AreEqual("A string value", obj.strVal);
        }
        
        [TestMethod]
        public void TestParseFieldInt()
        {
            FileReader reader = new FileReader(fields);
            Sample obj = new Sample();
            reader.ParseField(obj, "IntVal 10");

            Assert.AreEqual(10, obj.intVal);
        }
        
        [TestMethod]
        public void TestParseFieldFloat()
        {
            FileReader reader = new FileReader(fields);
            Sample obj = new Sample();
            reader.ParseField(obj, "FloatVal 3.14");

            Assert.AreEqual(3.14f, obj.floatVal);
        }
        
        [TestMethod]
        public void TestParseFieldVect()
        {
            FileReader reader = new FileReader(fields);
            Sample obj = new Sample();
            reader.ParseField(obj, "VectorVal 11,12,13");

            Assert.AreEqual(11.0f, obj.vecVal.x);
            Assert.AreEqual(12.0f, obj.vecVal.y);
            Assert.AreEqual(13.0f, obj.vecVal.z);
        }

        [TestMethod]
        public void TestParseFieldComment()
        {
            FileReader reader = new FileReader(fields);
            Sample obj = new Sample();
            reader.Initialise(obj);
            reader.ParseField(obj, "# just a comment");

            Assert.AreEqual(10, obj.intVal);
            Assert.AreEqual(20.123f, obj.floatVal);
            Assert.AreEqual("def", obj.strVal);
            Assert.AreEqual(11.0f, obj.vecVal.x);
            Assert.AreEqual(12.0f, obj.vecVal.y);
            Assert.AreEqual(13.0f, obj.vecVal.z);
        }
    }
}
