using NUnit.Framework;
using System;
using System.IO;

namespace CS422
{
    [TestFixture()]
    public class NUnitTestClass
    {
        [Test()]
        public void TestNumberedTextWriter()
        {
            var sw = new StreamWriter ("TestFile1.txt");
            var nsw = new NumberedTextWriter (sw);
            nsw.WriteLine ("Hello");
            nsw.WriteLine ("This");
            nsw.WriteLine ("is");
            nsw.WriteLine ("a");
            nsw.WriteLine ("test");
            sw.Close();
            nsw.Close ();
        }

        [Test()]
        public void TestIndexedNumsStream_Position_negative ()
        {
            var ins = new IndexedNumsStream(5);
            ins.Position = -1;
            Assert.AreEqual(0, ins.Position);

            ins.Position = -100;
            Assert.AreEqual(0, ins.Position);
        }

        [Test()]
        public void TestIndexedNumsStream_Position_greater ()
        {
            var streamlength = 5;
            var ins = new IndexedNumsStream(streamlength);
            ins.Position = 6;
            Assert.AreEqual(streamlength, ins.Position);

            ins.Position = 600;
            Assert.AreEqual(streamlength, ins.Position);
        }

        [Test()]
        public void TestIndexedNumsStream_Position ()
        {
            var streamlength = 50;
            var ins = new IndexedNumsStream(streamlength);
            ins.Position = 3;
            Assert.AreEqual(3, ins.Position);

            ins.Position = 49;
            Assert.AreEqual(49, ins.Position);
        }

        [Test()]
        public void TestIndexedNumsStream_negative2 ()
        {
            var streamlength = 50;
            var ins = new IndexedNumsStream(streamlength);
            // test for negative -> should be set to 0 
            ins.SetLength(-1);
            Assert.AreEqual(0, ins.Length);

            // other case is set to that length
            ins.SetLength(100);
            Assert.AreEqual(100, ins.Length);
        }
    }
}

