using System;
using System.Collections.Generic;

namespace SpeakingLanguage.FSM
{
    public sealed class StateMachine
    {
        private readonly int _endPoint;
        private readonly EventDictionary _eventDictionary = new EventDictionary();
        private readonly StateDictionary _stateDictionary = new StateDictionary();
        
        public StateMachine(int endPoint)
        {
            _endPoint = endPoint;
        }

        public unsafe void ExecuteFrame()
        {
            var reader = new Transition.Reader(_endPoint);
            var stateType = typeof(Component.Property.State);

            TransitionData data;
            while (reader.TryPopFront(out data))
            {
                var actor = Component.SLComponent.Root.Find(data.actorHandle).First();
                var stateList = actor.Find(Component.ComponentType.State);
                var stateIter = stateList.GetEnumerator();
                while (stateIter.MoveNext())
                {
                    var stateCom = stateIter.Current;
                    var stateProp = (Component.Property.State*)(stateCom.Get(stateType));

                    int nextStateHandle;
                    if (!_eventDictionary.TrySearch(data.actorHandle, stateProp->refHandle, data.fire, out nextStateHandle))
                        continue;

                    var nextStateCom = Component.SLComponent.Create(Component.ComponentType.State);
                    actor.LinkTo(nextStateCom);
                    Component.SLComponent.Destroy(stateCom);

                    var nextStateProp = (Component.Property.State*)(nextStateCom.Get(stateType));
                    nextStateProp->prev = stateProp;
                    nextStateProp->actorHandle = data.actorHandle;
                    nextStateProp->refHandle = nextStateHandle;
                    nextStateProp->transposedTick = -1;    // 아직 전이되지 않음
                    nextStateProp->lastTick = data.tick;
                }
            }

            var tick = Library.Ticker.UniversalMS;
            var transposedStack = stackalloc Component.Property.State*[16];
            var stackPtr = 0;
            var iter = Component.SLComponent.Root.Find(Component.ComponentType.State).GetEnumerator();
            while (iter.MoveNext())
            {
                var stateProp = (Component.Property.State*)(iter.Current.Get(stateType));
                var prev = stateProp->prev;
                while (null != prev)
                {
                    transposedStack[stackPtr++] = prev;
                    prev = prev->prev;
                }

                while (stackPtr > 0)
                {
                    var transposed = transposedStack[--stackPtr];
                    var transposedState = _stateDictionary.Find(transposed->refHandle);
                    transposedState.Execute(transposed->actorHandle, (int)(transposed->transposedTick - transposed->lastTick));
                }

                Library.Logger.Assert(stackPtr == 0);
                var state = _stateDictionary.Find(stateProp->refHandle);
                state.Execute(stateProp->actorHandle, (int)(stateProp->transposedTick - stateProp->lastTick));
                stateProp->lastTick = tick;
            }
        }
    }
}
