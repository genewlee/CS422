using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using CS422;

namespace CS422
{
    [TestFixture()]
    public class FileSystemDefsUnitTests
    {
        const string rootDir = "/home/gene/Desktop/Test";
        StandardFileSystem sfs;
        Dir422 root;

        [Test()]
        public void a_CreateNewStandardFileSystem()
        {
            sfs = StandardFileSystem.Create(rootDir);
            root = sfs.GetRoot();

            Assert.AreEqual("Test", root.Name);
        }

        [Test()]
        public void b_CreateContainsDirectory()
        {
            root.CreateDir("FooDir");
            Assert.AreEqual(true, root.ContainsDir("FooDir", false));
        }

        [Test()]
        public void c_CreateContainsDirectoryRecursive()
        {
            //Dir422 dir = new StdFSDir(rootDir + "/FooDir");
            //dir.CreateDir("BarDir");
            //Dir422 dirParent = dir.Parent;
            Dir422 FooDir = root.GetDir("FooDir");
            FooDir.CreateDir("BarDir");
            Assert.AreEqual(true, root.ContainsDir("BarDir", true));
        }

        [Test()]
        public void d_CreateContainsFile()
        {
            //Dir422 dir = new StdFSDir(rootDir, true);
            //dir.CreateFile("fooFile");
            //Assert.AreEqual(true, dir.ContainsFile("fooFile", false));
            root.CreateFile("fooFile");
            Assert.AreEqual(true, root.ContainsFile("fooFile", false));
        }

        [Test()]
        public void e_CreateContainsFileRecursive()
        {
            //Dir422 dir = new StdFSDir(rootDir + "/FooDir");
            //dir.CreateFile("barFile");
            //Dir422 dirParent = dir.Parent;
            //Assert.AreEqual(true, dirParent.ContainsFile("barFile", true));
            Dir422 FooDir = root.GetDir("FooDir");
            FooDir.CreateFile("barFile");
            Assert.AreEqual(true, root.ContainsFile("barFile", true));
        }

        [Test()]
        public void f_GetDirMakeFile()
        {
            //Dir422 dir = new StdFSDir(rootDir + "/FooDir");
            //Dir422 BarDir = dir.GetDir("BarDir");
            //BarDir.CreateFile("barFile2");
            //Assert.AreEqual(true, BarDir.ContainsFile("barFile2", true));
            Dir422 FooDir = root.GetDir("FooDir");
            Dir422 BarDir = FooDir.GetDir("BarDir");
            BarDir.CreateFile("barFile2");
            Assert.AreEqual(true, BarDir.ContainsFile("barFile2", true));
        }

        [Test()]
        public void g_GetFileWrite()
        {
            //Dir422 dir = new StdFSDir(rootDir, true);
            //File422 fooFile = dir.GetFile("fooFile");
            //Stream fs = fooFile.OpenReadWrite();
            //string s = "Hello World";
            //fs.Write(Encoding.ASCII.GetBytes(s), 0, s.Length);
            File422 fooFile = root.GetFile("fooFile");
            Stream fs = fooFile.OpenReadWrite();
            string s = "Hello World";
            fs.Write(Encoding.ASCII.GetBytes(s), 0, s.Length);
        }

        [Test()]
        public void h_GetFileReadOnly()
        {
            //Dir422 dir = new StdFSDir(rootDir, true);
            //File422 fooFile = dir.GetFile("fooFile");
            File422 fooFile = root.GetFile("fooFile");
            Stream fs = fooFile.OpenReadOnly();
            try{
                string s = "Hello World";
                fs.Write(Encoding.ASCII.GetBytes(s), 0, s.Length);
            }
            catch (Exception e){
                Assert.IsTrue(e is NullReferenceException);
            }
        }

        [Test()]
        public void i_RootParent()
        {
            //Dir422 dir = new StdFSDir(rootDir, true);
            //Assert.AreEqual(null, dir.Parent);
            Assert.AreSame(null, root.Parent);
        }

        [Test()]
        public void j_RejectDir()
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

