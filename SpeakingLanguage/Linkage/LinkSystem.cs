using SpeakingLanguage.Core;
using SpeakingLanguage.Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpeakingLanguage.Linkage
{
    public sealed class LinkSystem : SpeakingSystem
    {
        private Graph _graph = new Graph();
        private Reader _reader = new Reader();

        private ObjectPool<Reader> _readerPool = new ObjectPool<Reader>(() => { return new Reader(); }, Config.COUNT_DEFAULT_WORKER);
        
        public override int Sequence => 2;

        private SourceBuffer<TokenSource> _input;
        private SourceBuffer<RenderingSource> _output;

        public override void OnAwake(ILoopState state)
        {
            _input = state.Get<SourceBuffer<TokenSource>>();
            _output = state.Get<SourceBuffer<RenderingSource>>();
        }

        public override void OnUpdate(ILoopState state)
        {
            //var slock = new SimpleSpinLock();

            //var options = new ParallelOptions();
            //options.TaskScheduler = new LimitedConcurrencyLevelTaskScheduler(4);

            //var inj = state.Injection;
            //Parallel.ForEach(inj.Link, options, src =>
            //{
            //    var reader = _readerPool.GetObject();
            //    var react = reader.TryExecute(_graph, src.root);
            //    if (null == react)
            //        return;

            //    _readerPool.PutObject(reader);

            //    slock.Enter();
            //    _tempDic.Add(src.key, react);
            //    slock.Leave();
            //});

            //foreach (var pair in _tempDic)
            //{
            //    var token = pair.Value;
            //    var react = _writer.TryExecute(_graph, token);
            //    if (null == react)
            //        continue;
            //}

            //foreach (var key in _graph.Root)
            //{
            //    inj.Physics.Push(new PhysicsSource
            //    {
            //        root = !_tempDic.ContainsKey(key) ? null : _tempDic[key],
            //        mess = _graph[$"{key}.mess"]
            //    });
            //}

            //var options = new ParallelOptions();
            //options.TaskScheduler = new LimitedConcurrencyLevelTaskScheduler(4);

            //var rangePartitioner = Partitioner.Create(0, _inputBuffer.Count);

            //Parallel.ForEach(rangePartitioner, (range, loopState) =>
            //{
            //    for (int i = range.Item1; i < range.Item2; i++)
            //    {
            //        var src = _inputBuffer[i];
            //        var reader = _readerPool.GetObject();
            //        var react = reader.TryExecute(_graph, src.root);
            //        if (null == react)
            //            continue;

            //        _readerPool.PutObject(reader);

            //        _outputBuffer.Insert(new PhysicsSource
            //        {
            //            root = react,
            //            mess = _graph[$"{src.key}.mess"]
            //        });
            //    }
            //});

            //for (int i = 0; i != _inputBuffer.Count; i++)
            //{
            //    var src = _inputBuffer[i];
            //    var react = _reader.TryExecute(_graph, src.root);
            //    if (null == react)
            //        continue;

            //    _outputBuffer.Insert(new PhysicsSource
            //    {
            //        root = react,
            //        mess = _graph[$"{src.key}.mess"]
            //    });
            //}
        }

        public override FrameResult OnUpdateAsParallel(int srcIndex, int updaterId, ILoopState state)
        {
            var src = _input[srcIndex];
            
            var react = _reader.TryExecute(_graph, src.root);
            if (null == react)
                return FrameResult.Fail;

            _output[srcIndex] = new RenderingSource { value = 0 };

            return FrameResult.Success;
        }
    }
}
