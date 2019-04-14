using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Core
{
    sealed class LegAnalyzer : IStateComponent
    {
        private readonly static int max_leg = 1 << 6;
        private int[] _legHistory = new int[max_leg];
        private int _head = -1;
        private int _tail = 0;
        private int _lateTick = Environment.TickCount;
        
        public int Current => Environment.TickCount - _lateTick - Config.MS_PER_UPDATE;

        public int Accelation
        {
            get
            {
                return _legHistory[_head] - _legHistory[(_head - 1 + max_leg) % max_leg];
            }
        }

        public void Awake()
        {
        }

        public void Dispose()
        {
        }

        public void Enter()
        {
            _lateTick = Environment.TickCount;
        }

        public int Leave()
        {
            _head++;
            _head %= max_leg;
            return _legHistory[_head] = Current;
        }
    }

}
