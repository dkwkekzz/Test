using SpeakingLanguage.Core;
using SpeakingLanguage.Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SpeakingLanguage.Core
{
    public sealed class TokenCache : IStateComponent
    {
        private ConcurrentDictionary<string, Token> _cached;
        private CleanManager<string> _cleaner;
        
        public void Awake()
        {
            _cached = new ConcurrentDictionary<string, Token>();
            _cleaner = new CleanManager<string>(Config.MS_CACHE_DURATION, Config.COUNT_MAX_SOURCE, _remove);
        }

        public void Dispose()
        {
        }

        public bool TryGetValue(string key, out Token token)
        {
            return _cached.TryGetValue(key, out token);
        }
        
        public void AddOrUpdate(string key, Token token)
        {
            if (!_cached.ContainsKey(key))
            {
                _cached.TryAdd(key, token);
                _cleaner.RegistPriority(key);
            }
            else
            {
                //_cleaner.UpdatePriority(key);
            }
        }

        public void Clear(int leg)
        {
            _cleaner.Clear(leg);
        }
        
        private void _remove(string key)
        {
            Token token;
            _cached.TryRemove(key, out token);
        }
    }
}
