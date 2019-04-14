using System;
using System.Collections.Generic;

namespace SpeakingLanguage.FSM
{
    struct TransitionData
    {
        public int actorHandle;
        public int fire;
        public long tick;
        public int endPoint;
    }
}
