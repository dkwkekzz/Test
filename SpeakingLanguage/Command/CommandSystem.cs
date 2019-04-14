using SpeakingLanguage.Core;
using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeakingLanguage.Command
{
    public sealed class CommandSystem : SpeakingSystem
    {
        private EventCollector _collector = new EventCollector();
        private SourceBuffer<EventSource> _input;
        private SourceBuffer<TokenSource> _output;
        private TokenCache _cache;

        public override int Sequence => 1;

        public override void OnAwake()
        {
            _collector.Open();
            _input = state.Get<SourceBuffer<EventSource>>();
            _output = state.Get<SourceBuffer<TokenSource>>();
            _cache = state.Get<TokenCache>();
        }

        public override FrameResult OnUpdateAsParallel(int srcIndex)
        {
            var src = _input[srcIndex];

            Token token;
            if (!_cache.TryGetValue(src.key, out token))
            {
                if (!_collector.TryExecute(src.key, out token))
                    throw new ArgumentException($"fail to found Event by key: {src.key}");
            }

            _cache.AddOrUpdate(src.key, token);

            _output[srcIndex] = new TokenSource { root = token };

            return FrameResult.Success;
        }
    }
}
