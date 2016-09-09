using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace CS422
{
    public class ThreadPoolSleepSorter : IDisposable
    {
        private static TextWriter _mwriter;
        private ushort _numThreads;
        private BlockingCollection<SleepSortTask> _coll;

        public ThreadPoolSleepSorter(TextWriter output, ushort threadCount)
        {
            _mwriter = output;
            _coll = new BlockingCollection<SleepSortTask>();

            if (threadCount == 0)
                _numThreads = 64;
            else
                _numThreads = threadCount;

            for (int i = 0; i < _numThreads; i++)
            {
                Thread t = new Thread(ThreadWorkFunc);
                t.Start(_coll);
            }
        }

        private void ThreadWorkFunc(object coll)
        {
            while (true)
            {
                var t = ((BlockingCollection<SleepSortTask>)coll).Take();     // dequeue from collection
                if (t == null) break;
                t.Execute();              // run Sleep()
            }
        }

        public void Sort(byte[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                _coll.Add(new SleepSortTask(values[i]));
            }
        }

        /// <summary>
        /// Releases all resource used by the threads object.
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < _numThreads; i++)
            {
                _coll.Add(null);
            }  
        }

        /// <summary>
        /// Sleep sort task.
        /// </summary>
        private class SleepSortTask
        {
            private int _data;

            public int Data
            {
                get
                {
                    return _data;
                }
                set
                {
                    _data = value;
                }
            }

            public SleepSortTask (int data)
            {
                _data = data;
            }

            public void Execute ()
            {
                Thread.Sleep(1000 * (byte)_data);     // multiply by 1000 because num is milliseconds
                _mwriter.WriteLine((byte)_data);
            }

        }

    }
}

