using System;
using System.Collections.Generic;

namespace SpeakingLanguage.FSM
{
    sealed class EventTree
    {
        private Dictionary<int, TransitionData> _entity2DataDic = new Dictionary<int, TransitionData>();

        public void Insert(ref TransitionData data)
        {
            _entity2DataDic.Add(data.actorHandle, data);
        }

        public bool TrySearch(int actorHandle, out TransitionData data)
        {
            if (!_entity2DataDic.TryGetValue(actorHandle, out data))
                return false;
            return true;
        }
    }
}
