using System;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Core
{
    class SpinSyncHandle : IDisposable
    {
        public bool IsCompleted => Volatile.Read(ref _completeCount) == _workerCount;

        private readonly int _workerCount;
        private bool _isBreak;
        private int _completeCount;
        private int _jobCount;
        private int[] _jobOrders;
        
        public SpinSyncHandle(int workerCount)
        {
            _workerCount = workerCount;
            _completeCount = 0;
            _jobCount = 1;
            _jobOrders = new int[workerCount];
            _isBreak = false;
        }

        public void SignalCompleted(int id)
        {
            Interlocked.Increment(ref _jobOrders[id]);
            Interlocked.Increment(ref _completeCount);
        }

        public void SignalWorking()
        {
            Volatile.Write(ref _completeCount, 0);
            Interlocked.Increment(ref _jobCount);
        }

        public void WaitForComplete()
        {
            while (true)
            {
                if (Volatile.Read(ref _completeCount) == _workerCount)
                    return;

                if (_isBreak)
                    return;

                Thread.Sleep(1);
            }
        }

        public void WaitForWork(int id)
        {
            while (true)
            {
                if (Volatile.Read(ref _jobOrders[id]) < Volatile.Read(ref _jobCount))
                    return;

                if (_isBreak)
                    return;

                Thread.Sleep(1);
            }
        }

        public void Dispose()
        {
            _isBreak = true;
        }
    }
}
