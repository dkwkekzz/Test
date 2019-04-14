using System;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Core
{
    sealed class SyncHandle : SpeakingService
    {
        private CountdownEvent _completeHandle;
        private ManualResetEvent _waitHandle;
        
        public int WorkerCount => _completeHandle.InitialCount;
        public bool IsCompleted => _completeHandle.IsSet;
        
        public override void OnAwake()
        {
            _completeHandle = new CountdownEvent(Config.COUNT_DEFAULT_WORKER);
            _waitHandle = new ManualResetEvent(false);
        }

        public override void OnDestroy()
        {
            _completeHandle.Dispose();
            _waitHandle.Dispose();
        }

        public void SignalCompleted(int id)
        {
            _waitHandle.Reset();
            _completeHandle.Signal();
        }

        public void SignalWorking()
        {
            _completeHandle.Reset();
            _waitHandle.Set();
        }

        public void WaitForComplete()
        {
            _completeHandle.Wait();
        }

        public void WaitForWork(int id)
        {
            _waitHandle.WaitOne();
        }
    }
}
