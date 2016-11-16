using NUnit.Framework;
using System;
using System.IO;

namespace CS422
{
    [TestFixture()]
    public class UnitTest
    {
        /*
         * ConcatStream unit test that combines two memory streams, 
         * reads back all the data in random chunk sizes, and verifies 
         * it against the original data
         */
        [Test()]
        public void TestMethod1()
        {
            byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] buf2 = { 11, 12, 13, 14, 15, 16, 17, 18 };

            MemoryStream mem1 = new MemoryStream(buf1);
            MemoryStream mem2 = new MemoryStream(buf2);
            ConcatStream cs = new ConcatStream(mem1, mem2);

            byte[] buf = new byte[512];
            int nbytes = 11;
            cs.Read(buf, 0, nbytes);

            for (int i =1; i <= nbytes; i++ )
            {
                Assert.AreEqual(buf[i - 1], i);
            }
        }

        /*
         * ConcatStream unit test that combines a memory stream as the
         * first and a NoSeekMemoryStream as the second, and verifies that all data can be read
         */
        [Test()]
        public void TestMethod2()
        {
            byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] buf2 = { 11, 12, 13, 14, 15, 16, 17, 18 };

            MemoryStream mem1 = new MemoryStream(buf1);
            NoSeekMemoryStream mem2 = new NoSeekMemoryStream(buf2);
            ConcatStream cs = new ConcatStream(mem1, mem2);

            byte[] buf = new byte[512];
            int nbytes = 11;
            cs.Read(buf, 0, nbytes);

            for (int i = 1; i <= nbytes; i++)
            {
                //Console.Write("{0}", i);
                //Console.WriteLine(buf[i-1]);
                Assert.AreEqual(buf[i - 1], i);
            }
        }

        /* 
         * ConcatStream test that query Length property where it can be computed without an exception.
         */
        [Test()]
        public void TestMethod3()
        {
            byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] buf2 = { 11, 12, 13, 14, 15, 16, 17, 18 };

            MemoryStream mem1 = new MemoryStream(buf1);
            NoSeekMemoryStream mem2 = new NoSeekMemoryStream(buf2);
            ConcatStream cs = new ConcatStream(mem1, mem2, 18);

            Assert.AreEqual(18, cs.Length);
        }

        /* 
         * ConcatStream test that query Length property cannot be computed without an exception.
         */
        [Test()]
        public void TestMethod4()
        {
            byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] buf2 = { 11, 12, 13, 14, 15, 16, 17, 18 };

            MemoryStream mem1 = new MemoryStream(buf1);
            NoSeekMemoryStream mem2 = new NoSeekMemoryStream(buf2);
            ConcatStream cs = new ConcatStream(mem1, mem2, 18);
            try
            {
                Assert.AreEqual(18, cs.Length);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(NotSupportedException));
            }
        }

        /* 
         * NoSeekMemoryStream unit test that attempts to seek using all relevant 
         * properties/methods that provide seeking capabilities in a stream, 
         * and makes sure that each throws the NotSupportedException.
        /*
         * if a class derived from Stream does not support seeking, 
         * calls to Length, SetLength, Position, and Seek throw a NotSupportedException.
         * TestMethod 5 - 8
         */
        [Test()]
        public void TestMethod5()
        {
            try
            {
                byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                NoSeekMemoryStream nsms = new NoSeekMemoryStream(buf1);
                long testLen = nsms.Length;
                Assert.Fail("An exception should have been thrown");
            }
            catch (Exception e)
            {
                //Assert.Fail(
                //     string.Format("Unexpected exception of type {0} caught: {1}",
                //                    e.GetType(), e.Message)
                //);
                Assert.AreEqual(e.GetType(), typeof(NotSupportedException));
            }
        }

        [Test()]
        public void TestMethod6()
        {
            try
            {
                byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                NoSeekMemoryStream nsms = new NoSeekMemoryStream(buf1);
                nsms.SetLength(8);
                Assert.Fail("An exception should have been thrown");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(NotSupportedException));
            }
        }

        [Test()]
        public void TestMethod7()
        {
            try
            {
                byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                NoSeekMemoryStream nsms = new NoSeekMemoryStream(buf1);
                nsms.Position = 5;
                Assert.Fail("An exception should have been thrown");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(NotSupportedException));
            }
        }

        [Test()]
        public void TestMethod8()
        {
            try
            {
                byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                NoSeekMemoryStream nsms = new NoSeekMemoryStream(buf1);
                nsms.Seek(2, SeekOrigin.Current);
                Assert.Fail("An exception should have been thrown");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(NotSupportedException));
            }
        }

        /*
         * Test writing to ConcatStream that is less than length of first stream
         */ 
        [Test()]
        public void TestMethod9()
        {
            byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] buf2 = { 11, 12, 13, 14, 15, 16, 17, 18 };

            MemoryStream mem1 = new MemoryStream(buf1);
            MemoryStream mem2 = new MemoryStream(buf2);
            ConcatStream cs = new ConcatStream(mem1, mem2);

            byte[] buf3 = { 19, 20, 21 };
            cs.Write(buf3, 0, 3);

            byte[] expected = { 19, 20, 21, 4, 5, 6, 7, 8, 9, 10 };
            byte[] result = new byte[10];
            cs.Position = 0;
            cs.Read(result, 0, 10);
            Assert.AreEqual(expected, result);
        }

        /*
         * Test writing to ConcatStream that is greater than length of first stream
         */
        [Test()]
        public void TestMethod10()
        {
            byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] buf2 = { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            MemoryStream mem1 = new MemoryStream(buf1);
            MemoryStream mem2 = new MemoryStream(buf2);
            ConcatStream cs = new ConcatStream(mem1, mem2);

            byte[] buf3 = { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
            cs.Write(buf3, 0, buf3.Length);

            byte[] expected = { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20};
            byte[] result = new byte[expected.Length];
            cs.Position = 0;
            cs.Read(result, 0, expected.Length);
            Assert.AreEqual(expected, result);
        }

        /*
         * Test writing to ConcatStream that where first stream position is not 0
         */
        [Test()]
        public void TestMethod11()
        {
            byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] buf2 = { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            MemoryStream mem1 = new MemoryStream(buf1);
            MemoryStream mem2 = new MemoryStream(buf2);
            ConcatStream cs = new ConcatStream(mem1, mem2);

            byte[] buf3 = { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
            cs.Position = 5;
            cs.Write(buf3, 0, buf3.Length);

            byte[] expected = { 1, 2, 3, 4, 5, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 16, 17, 18, 19, 20 };
            byte[] result = new byte[expected.Length];
            cs.Position = 0;
            cs.Read(result, 0, expected.Length);
            Assert.AreEqual(expected, result);
        }

        /*
         * Test writing to ConcatStream that where first stream position is at end
         * and write everything to second stream
         */
        [Test()]
        public void TestMethod12()
        {
            byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] buf2 = { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            MemoryStream mem1 = new MemoryStream(buf1);
            MemoryStream mem2 = new MemoryStream(buf2);
            ConcatStream cs = new ConcatStream(mem1, mem2);

            byte[] buf3 = { 21, 22, 23, 24, 25 };
            cs.Position = buf1.Length;
            cs.Write(buf3, 0, buf3.Length);

            byte[] expected = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 21, 22, 23, 24, 25, 16, 17, 18, 19, 20 };
            byte[] result = new byte[expected.Length];
            cs.Position = 0;
            cs.Read(result, 0, expected.Length);
            Assert.AreEqual(expected, result);
        }


        /*
         * Test writing to NoSeemMemoryStream
         */
        [Test()]
        public void TestMethod13()
        {
            byte[] buf = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            NoSeekMemoryStream nsms = new NoSeekMemoryStream(buf);
            byte[] buf2 = { 11, 12, 13, 14, 15 };
            nsms.Write(buf2, 0, 5); // not at Position = 5

            byte[] expected = { 6, 7, 8, 9, 10, 0, 0, 0, 0, 0 };
            byte[] result = new byte[expected.Length];
            nsms.Read(result, 0, expected.Length);
            Assert.AreEqual(expected, result); // checking without offset
        }

        /* Test to read more bytes than the size of both buf/streams combined */
        [Test()]
        public void TestMethod14()
        {
            byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] buf2 = { 11, 12, 13, 14, 15, 16, 17, 18 };

            MemoryStream mem1 = new MemoryStream(buf1);
            MemoryStream mem2 = new MemoryStream(buf2);
            ConcatStream cs = new ConcatStream(mem1, mem2);

            byte[] buf = new byte[512];
            int nbytes = 11;
            cs.Read(buf, 0, nbytes + 100);

            for (int i = 1; i <= nbytes; i++)
            {
                Console.Write(i);
                Console.WriteLine(buf[i - 1]);
                Assert.AreEqual(buf[i - 1], i);
            }
        }
    }
}
