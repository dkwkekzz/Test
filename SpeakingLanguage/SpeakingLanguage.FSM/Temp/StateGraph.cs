using SpeakingLanguage.Library;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.FSM
{
    sealed class StateGraph : IEnumerable<StateContext>
    {
        class ActorStock : IEnumerable<StateContext>
        {
            private Dictionary<int, StateContext> _sort2StateDic = new Dictionary<int, StateContext>();
            private IEnumerator<StateContext> _stateIter;

            public IEnumerator<StateContext> GetEnumerator()
            {
                _stateIter.Reset();
                return _stateIter;
            }
            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            public ActorStock()
            {
                _stateIter = _sort2StateDic.Values.GetEnumerator();
            }

            public void Exchange(StateContext state)
            {
                state.Prev = _sort2StateDic[state.Ref.SortHandle];
                state.Prev.TransposedTick = state.LastTick;
                _sort2StateDic[state.Ref.SortHandle] = state;
            }
        }

        class StateStock : IEnumerable<StateContext>
        {
            class KeyComparer : IEqualityComparer<Key>
            {
                public bool Equals(Key x, Key y) => x.actorHandle == y.actorHandle && x.sortHandle == y.sortHandle;
                public int GetHashCode(Key obj) => (obj.actorHandle << 16) | obj.sortHandle;
            }

            struct Key
            {
                public int actorHandle;
                public int sortHandle;
            }

            private Dictionary<Key, StateContext> _key2StateDic = new Dictionary<Key, StateContext>(new KeyComparer());
            private IEnumerator<StateContext> _stateIter;

            public IEnumerator<StateContext> GetEnumerator()
            {
                _stateIter.Reset();
                return _stateIter;
            }
            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            public StateStock()
            {
                _stateIter = _key2StateDic.Values.GetEnumerator();
            }

            public void Exchange(StateContext state)
            {
                _key2StateDic[new Key { actorHandle = state.ActorHandle, sortHandle = state.Ref.SortHandle }] = state;
            }
        }

        private Dictionary<int, ActorStock> _actorStockDic = new Dictionary<int, ActorStock>();
        private Dictionary<int, StateStock> _stateStockDic = new Dictionary<int, StateStock>();
        private IEnumerator<StateContext> _stateGroupIter;
        
        public StateGraph()
        {
            var query = from stock in _stateStockDic.Values
                        from state in stock
                        select state;
            _stateGroupIter = query.GetEnumerator();
        }

        public IEnumerator<StateContext> GetEnumerator()
        {
            _stateGroupIter.Reset();
            return _stateGroupIter;
        }
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public void Insert(StateContext newState)
        {
            ActorStock aStock;
            if (!_actorStockDic.TryGetValue(newState.ActorHandle, out aStock))
            {
                aStock = new ActorStock();
                _actorStockDic.Add(newState.ActorHandle, aStock);
            }

            StateStock sStock;
            if (!_stateStockDic.TryGetValue(newState.Ref.Handle, out sStock))
            {
                sStock = new StateStock();
                _stateStockDic.Add(newState.Ref.Handle, sStock);
            }

            aStock.Exchange(newState);
            sStock.Exchange(newState);
        }

        public IEnumerable<StateContext> Search(int actorHandle)
        {
            return _actorStockDic[actorHandle];
        }
    }
}
