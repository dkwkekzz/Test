using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Core
{
    public class Injection : IStateComponent
    {
        public IContainer<EventSource> Entity { get; } = new Container<EventSource>(Config.COUNT_MAX_SOURCE);
        public IContainer<ContextSource> Context { get; } = new Container<ContextSource>(Config.COUNT_MAX_SOURCE);
        public IContainer<TokenSource> Link { get; } = new Container<TokenSource>(Config.COUNT_MAX_SOURCE);
        public IContainer<PhysicsSource> Physics { get; } = new Container<PhysicsSource>(Config.COUNT_MAX_SOURCE);

        public void Awake()
        {
        }

        public void Clear()
        {
            Entity.Clear();
            Context.Clear();
            Link.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
