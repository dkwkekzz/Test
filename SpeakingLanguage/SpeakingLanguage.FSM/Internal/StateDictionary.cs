using System;
using System.Collections.Generic;

namespace SpeakingLanguage.FSM
{
    sealed class StateDictionary
    {
        private Dictionary<int, State> _stateDic = new Dictionary<int, State>();

        public State Find(int stateHandle)
        {
            return _stateDic[stateHandle];
        }
    }
}
