using System;
using System.IO;

namespace CS422
{
	public class Program
	{
		static void Main(string[] args)
		{
			var sw = new StreamWriter ("TestFile.txt");
			var nsw = new NumberedTextWriter (sw);
			nsw.WriteLine ("Hello");
			nsw.WriteLine ("This");
			nsw.WriteLine ("is");
			nsw.WriteLine ("a");
			nsw.WriteLine ("test");
			sw.Close();
			nsw.Close ();

            var streamlength = 30;
            var buf = new Byte[streamlength];
            var ins = new IndexedNumsStream(streamlength);
            ins.Read(buf, 2, 15);
            ins.Read(buf, 2, 16);

//            System.Console.WriteLine("hello");
//            foreach (var b in buf)
//            {
//                Console.WriteLine(b);
//            }

		}
	}
}

