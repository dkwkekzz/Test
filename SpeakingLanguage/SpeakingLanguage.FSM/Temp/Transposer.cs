using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpeakingLanguage.FSM
{
    //public sealed class Transposer
    //{
    //    private Queue<TransitionData2> _waitQueue { get; } = new Queue<TransitionData2>();
    //    private EventDictionary _eventGraph = new EventDictionary();
    //    private StateGraph _stateGraph = new StateGraph();
    //    private StateDictionary _stateDictionary = new StateDictionary();
        
    //    public void Transition(string entityKey, string newStateKey)
    //    {
    //        var finded = _stateDictionary.Find(newStateKey);
    //        Trace.Assert(null != finded, $"state: {newStateKey} <- Please put the key in the stateDictionary.");

    //        _waitQueue.Enqueue(new TransitionData2 { entityKey = entityKey, state = finded });
    //    }

    //    internal int Count => _waitQueue.Count;
    //    internal void Pop(out TransitionData2 data) => data = _waitQueue.Dequeue();
    //}
}
