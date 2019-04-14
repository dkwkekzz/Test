using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SpeakingLanguage.Core
{
    public sealed class Ticker : SpeakingService
    {
        private Stopwatch _timer = new Stopwatch();
        private int _executeCount;
        private long _elapsed;
        private long _elapsedAverage;

        private int[] _simpleQ = new int[100];
        private int _currentIndex = -1;

        public int Recently => _simpleQ[_currentIndex];
        public string Log => $"[frame complete]\nelapsed={_elapsed.ToString()}\nelapsedAverage={_elapsedAverage.ToString()}";

        public void Enter()
        {
            _timer.Restart();
        }

        public int Leave()
        {
            _timer.Stop();
            _elapsed = _timer.ElapsedMilliseconds;

            _executeCount++;
            _elapsedAverage = ((_elapsedAverage * (_executeCount - 1)) + _elapsed) / _executeCount;
            
            ++_currentIndex;
            if (_currentIndex == _simpleQ.Length)
                _currentIndex = 0;
            return _simpleQ[_currentIndex] = (int)_elapsed - Config.MS_PER_UPDATE;
        }
    }
}
