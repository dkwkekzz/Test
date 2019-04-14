using SpeakingLanguage.Library;
using System;

namespace SpeakingLanguage.FSM
{
    sealed class StateContext : PoolingObject<StateContext>
    {
        public State Ref { get; set; }
        public StateContext Prev { get; set; }
        public int ActorHandle { get; set; }
        public int TransposedTick { get; set; }
        public int LastTick { get; set; }
    }
}
