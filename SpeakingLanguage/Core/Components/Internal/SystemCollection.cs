using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Core
{
    sealed class SystemCollection : SpeakingService
    {
        private List<SpeakingSystem> _systemList;

        public SpeakingSystem this[int index] => _systemList[index];
        public int Count => _systemList.Count;

        public override void OnAwake()
        {
            _systemList = Utils.Collect<SpeakingSystem>();
            _systemList.ForEach(sys => { sys.OnAwake(); });
            _systemList.Sort((lhs, rhs) => { return lhs.Sequence.CompareTo(rhs.Sequence); });
            _systemList.ForEach(sys => { Console.WriteLine($"load system: {sys.GetType().Name}"); });
        }

        public override void OnDestroy()
        {
            for (int i = 0; i != _systemList.Count; i++)
                _systemList[i].OnDestroy();
            _systemList.Clear();
        }
    }
}
