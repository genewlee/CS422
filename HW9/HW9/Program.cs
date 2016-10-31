using System;
using System.Text;
namespace CS422
{
    public class Program
    {
        static void Main()
        {
            //Mac
            //WebServer.AddService(new FilesWebService(StandardFileSystem.Create("/Users/Gene/Desktop")));

            //Linux
            WebServer.AddService(new FilesWebService(StandardFileSystem.Create("/home/gene/Desktop")));

            /*var mfs = new MemoryFileSystem();
            var root = mfs.GetRoot();
            root.CreateDir("FooDir");
            root.CreateFile("barFile");
            var barFile = root.GetFile("barFile");
            var barFileStream = barFile.OpenReadWrite();
            barFileStream.Write(Encoding.ASCII.GetBytes("Hello World"), 0, Encoding.ASCII.GetBytes("Hello World").Length);
            barFileStream.Close();
            WebServer.AddService(new FilesWebService(mfs));*/

            WebServer.Start(8080, 64);

        }
    }
}

