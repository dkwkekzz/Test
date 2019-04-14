using System;
using System.Collections.Generic;

namespace SpeakingLanguage.FSM
{
    class State
    {
        public virtual string Name => GetType().Name;
        public virtual void Execute(int actorHandle, int deltaTick) { }

        public int Handle { get; }
        public int SortHandle { get; }
    }
}
