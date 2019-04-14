using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Core
{
    public sealed class SystemIterator : IEnumerator<SpeakingSystem>
    {
        private List<SpeakingSystem> _systemList;
        private int _currentIndex;

        public SpeakingSystem Current => (_currentIndex < 0 || _currentIndex >= _systemList.Count) ? null : _systemList[_currentIndex];
        object IEnumerator.Current => (_currentIndex < 0 || _currentIndex >= _systemList.Count) ? null : _systemList[_currentIndex];

        public SystemIterator(List<SpeakingSystem> systemList)
        {
            _systemList = systemList;
            _currentIndex = -1;
        }

        public void Dispose()
        {
            foreach (var system in _systemList)
                system.OnDestroy();
            _systemList.Clear();
        }

        public bool MoveNext()
        {
            _currentIndex++;
            if (_currentIndex < 0 || _currentIndex > _systemList.Count)
                return false;

            return true;
        }

        public void Reset()
        {
            _currentIndex = -1;
        }
    }

}
