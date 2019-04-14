using SpeakingLanguage.Library;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Command
{
    struct Worker
    {
        public int state;
    }

    class WorkerPool : IService, IEnumerable<KeyValuePair<string, Worker>>
    {
        private Dictionary<string, Worker> _workerDic = new Dictionary<string, Worker>(Config.MAX_WORK, new StringRefComparer());

        public void Initialize(IProvider provider)
        {
        }
        
        public void AddWorker(string owner)
        {
            if (_workerDic.ContainsKey(owner))
                return;

            _workerDic.Add(owner, new Worker { state = 0 });
        }

        public IEnumerator<KeyValuePair<string, Worker>> GetEnumerator()
        {
            return _workerDic.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _workerDic.GetEnumerator();
        }

        public void OnCompletedWork(string owner)
        {
            var old = _workerDic[owner];
            _workerDic[owner] = new Worker { state = old.state + 1 };
        }
    }
}
