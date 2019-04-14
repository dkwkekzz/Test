using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpeakingLanguage.FSM
{
    public class Transition
    {
        class DataQueue
        {
            private readonly ConcurrentQueue<TransitionData> _queue = new ConcurrentQueue<TransitionData>();

            internal int Count => _queue.Count;
            internal void Enqueue(TransitionData data) => _queue.Enqueue(data);
            internal bool TryDequeue(out TransitionData data) => _queue.TryDequeue(out data);
        }

        private static readonly ConcurrentDictionary<int, DataQueue> _ep2QueueDic = new ConcurrentDictionary<int, DataQueue>();

        public struct Writer
        {
            private readonly int _endPoint;
            private readonly DataQueue _captured;

            public Writer(int endPoint = 0)
            {
                _endPoint = endPoint;

                if (!_ep2QueueDic.TryGetValue(_endPoint, out _captured))
                {
                    _captured = new DataQueue();
                    var res = _ep2QueueDic.TryAdd(_endPoint, _captured);
                    Library.Logger.Assert(res == true);
                }
            }

            public void PushBack(int actorHandle, int fire, long tick)
            {
                Library.Logger.Assert(null != _captured);
                _captured.Enqueue(new TransitionData { actorHandle = actorHandle, fire = fire, tick = tick, endPoint = _endPoint });
            }
        }

        internal struct Reader
        {
            private readonly int _endPoint;
            private readonly DataQueue _captured;

            public Reader(int endPoint = 0)
            {
                _endPoint = endPoint;

                if (!_ep2QueueDic.TryGetValue(_endPoint, out _captured))
                {
                    _captured = new DataQueue();
                    var res = _ep2QueueDic.TryAdd(_endPoint, _captured);
                    Library.Logger.Assert(res == true);
                }
            }

            public bool TryPopFront(out TransitionData data)
            {
                Library.Logger.Assert(null != _captured);
                return _captured.TryDequeue(out data);
            }
        }
    }
}
