using System;
using System.Threading;

namespace CS422
{
    public class Program
    {
        static void Main()
        {
            /*var tpss = new ThreadPoolSleepSorter(Console.Out, 0);
            var testArray = new byte[] {7, 1, 4, 2, 7, 3, 9};
            tpss.Sort(testArray);
            tpss.Dispose();
            var testArray2 = new byte[]{5,3,1,7,3,8};
            tpss.Sort(testArray2);*/
            //Thread.Sleep(2000);
            //tpss.Dispose();

            Thread t1 = new Thread(en);
            Thread t2 = new Thread(de);
            var q = new PCQueue();
            t1.Start(q);
            t2.Start(q);
        }

        public static void en(object q)
        {
            Random r = new Random();
            for (int i = 0; i < 10000000; i++)
            {
                ((PCQueue)q).Enqueue(i);
                //Thread.Sleep(i * r.Next(1, 3) * 1000);
            }
        }

        public static void de(object q)
        {
            Random r = new Random();
            for (int i = 0; i < 10000000; i++)
            {
                int j = 0;
                ((PCQueue)q).Dequeue(ref j);
                Console.WriteLine(j + " ");
                //Thread.Sleep(i * r.Next(1, 3) * 1000);
            }
        }
    }
}

