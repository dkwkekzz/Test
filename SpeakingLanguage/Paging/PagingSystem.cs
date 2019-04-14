using SpeakingLanguage.Core;
using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Paging
{
    public sealed class PagingSystem : SpeakingSystem
    {
        private PageScheduler _scheduler = new PageScheduler();
        private PageCache _cache = new PageCache();

        public override int Sequence => 1;

        public override void OnAwake()
        {
            _scheduler.Awake();
            _cache.Awake();
        }
        
        public override void OnUpdate(LoopState state)
        {
            var inj = state.Get<Injection>();
            var cache = state.Get<TokenCache>();
            var iter = inj.Entity.GetEnumerator();
            while (iter.MoveNext())
            {  
                var src = iter.Current;
                
                Token token;
                if (cache.TryGetValue(src.key, out token))
                {
                    inj.Link.Push(new TokenSource { root = token });
                    continue;
                }

                Page page;
                if (!_cache.TryGetPage(cxt.pageIndex, out page))
                {
                    continue;
                }

                inj.Context.Push(new ContextSource { key = cxt.key, body = page.body, count = cxt.count, offset = cxt.offset });
            }

            _scheduler.ExecuteFrame(_cache);
        }
    }
}
