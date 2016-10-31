using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace CS422
{
    [TestFixture()]
    public class MemoryFileSystemUnitTests
    {
        MemoryFileSystem mfs;
        Dir422 root;

        [Test()]
        public void a_CreateMemoryFileSystem()
        {
            mfs = new MemoryFileSystem();
            root = mfs.GetRoot();
        }

        [Test()]
        public void b_CreateContainDir()
        {
            root.CreateDir("TestDir");
            Assert.AreEqual(true, root.ContainsDir("TestDir", false));
        }

        [Test()]
        public void c_CreateContainFile()
        {
            root.CreateFile("testFile");
            Assert.AreEqual(true, root.ContainsFile("testFile", false));
        }

        [Test()]
        public void d_CreateContainFileRecursive()
        {
            Dir422 TestDir = root.GetDir("TestDir");
            Dir422 BlahDir = TestDir.CreateDir("BlahDir");
            BlahDir.CreateFile("blahFile");
            Assert.AreEqual(true, root.ContainsFile("blahFile", true));
        }

        [Test()]
        public void e_CreateContainDirRecursive()
        {
            Assert.AreEqual(true, root.ContainsDir("BlahDir", true));
        }

        [Test()]
        public void f_ReadFileMany()
        {
            File422 testFile = root.GetFile("testFile");
            Stream fs = testFile.OpenReadOnly();
            Stream fs1 = testFile.OpenReadOnly();
            Stream fs2 = testFile.OpenReadOnly();
            Assert.AreNotEqual(null, fs);
            Assert.AreNotEqual(null, fs1);
            Assert.AreNotEqual(null, fs2);
            fs.Dispose();
            fs1.Dispose();
            fs2.Dispose();
        }

        [Test()]
        public void f_ReadFileManyWriteOpen()
        {
            File422 testFile = root.GetFile("testFile");
            Stream fs = testFile.OpenReadWrite();
            string text = "This is a test for MemFileSystem.";
            fs.Write(Encoding.ASCII.GetBytes(text), 0, text.Length);
            Stream fs1 = testFile.OpenReadOnly();
            Stream fs2 = testFile.OpenReadOnly();

            Assert.AreEqual(null, fs1);
            Assert.AreEqual(null, fs2);

            fs.Dispose();

            Stream fs3 = testFile.OpenReadOnly();
            byte[] buf = new byte[Encoding.ASCII.GetBytes(text).Length];
            fs3.Read(buf, 0, Encoding.ASCII.GetBytes(text).Length);
            Stream fs4 = testFile.OpenReadOnly();
            fs3.Close(); fs4.Close();
            Assert.AreEqual(Encoding.ASCII.GetBytes(text), buf);

            fs = testFile.OpenReadWrite();
            string blah = "This is added Text.";
            fs.Seek(Encoding.ASCII.GetBytes(text).Length, SeekOrigin.Begin);
            fs.Write(Encoding.ASCII.GetBytes(blah), 0, blah.Length);
            fs.Dispose();
            fs3 = testFile.OpenReadOnly();
            buf = new byte[Encoding.ASCII.GetBytes(text + blah).Length];
            fs3.Read(buf, 0, Encoding.ASCII.GetBytes(text + blah).Length);
            //string s = Encoding.ASCII.GetString(buf);

            Assert.AreEqual(Encoding.ASCII.GetBytes(text + blah), buf);
        }

        [Test()]
        public void g_RejectDir()
        {
            Dir422 BlahDir = root.CreateDir("/BlahDir/");
            Assert.AreEqual(null, BlahDir);

            Dir422 BlahDir2 = root.CreateDir("BlahDir2\\");
            Assert.AreEqual(null, BlahDir2);

            bool test = root.ContainsDir("Blah/Dir/", true);
            Assert.AreEqual(false, test);

            bool test2 = root.ContainsDir("BlahDir\\", true);
            Assert.AreEqual(false, test2);

            Dir422 testDir = root.GetDir("BlahDir/");
            Assert.AreEqual(null, testDir);

            Dir422 testDir2 = root.GetDir("Blah\\Dir");
            Assert.AreEqual(null, testDir2);
        }
    }
}

