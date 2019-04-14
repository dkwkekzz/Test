using SpeakingLanguage.Library;
using SpeakingLanguage.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeakingLanguage.Compile
{
    public sealed class CompileSystem : SpeakingSystem
    {
        private SpeakingCompiler _compiler = new SpeakingCompiler();

        public override int Sequence => 2;

        public override void OnAwake()
        {
        }

        public override void OnUpdate(LoopState state)
        {
            var inj = state.Get<Injection>();
            var cache = state.Get<TokenCache>();
            Parallel.ForEach(inj.Context, src => 
            {
                Token token;
                if (!cache.TryGetValue(src.key, out token))
                {
                    _compiler.Execute(src, out token);
                }

                cache.AddOrUpdate(src.key, token);

                inj.Link.Push(new TokenSource { root = token });
            });
        }
    }
}
