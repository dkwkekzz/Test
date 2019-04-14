using System;
using System.Collections.Generic;
using System.Linq;

namespace SpeakingLanguage.FSM
{
    sealed class EventDictionary
    {
        private Dictionary<int, Dictionary<int, Dictionary<int, int>>> _eventDic = 
            new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();

        public void Collect()
        {
        }

        public bool TrySearch(int actorHandle, int currentStateHandle, int fire, out int nextStateHandle)
        {
            Dictionary<int, Dictionary<int, int>> entityToStateDic;
            if (!_eventDic.TryGetValue(actorHandle, out entityToStateDic))
            {
                nextStateHandle = -1;
                return false;
            }

            Dictionary<int, int> stateToEventDic;
            if (!entityToStateDic.TryGetValue(currentStateHandle, out stateToEventDic))
            {
                nextStateHandle = -1;
                return false;
            }

            nextStateHandle = stateToEventDic[fire];
            return true;
        }
    }
}
