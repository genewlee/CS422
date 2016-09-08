using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace CS422
{
    public class ThreadPoolSleepSorter : IDisposable
    {
        private TextWriter _output;
        private ushort _numThreads;
        private BlockingCollection<Thread> _collThreads;

        public ThreadPoolSleepSorter(TextWriter output, ushort threadCount)
        {
            _output = output;
            _collThreads = new BlockingCollection<Thread>();
           
            if (threadCount == 0)
                _numThreads = 64;
            else
                _numThreads = threadCount;

            for (int i = 0; i < _numThreads; i++)
            {
                Thread t = new Thread(Sleep);
                _collThreads.Add(t);
            }
        }

        /// <summary>
        /// Sort doesn’t modify the array of values, it simply prints them out in sorted order
        /// </summary>
        public void Sort(byte[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                var t = _collThreads.Take();     // dequeue from collection
                t.Start(values[i]);              // run Sleep()
                _collThreads.Add(t);             // put back into collection
            }
        }

        /// <summary>
        /// Thread sleeps the specified num.
        /// </summary>
        private void Sleep(object num)
        {
            Thread.Sleep(1000 * (byte)num);     // multiply by 1000 because num is milliseconds
            _output.WriteLine((byte)num);
        }

        /// <summary>
        /// “free” all the threads associated with the thread pool
        /// </summary>
        public void Dispose()
        {
            foreach (var t in _collThreads)
                t.Abort();
        }
    }
}

