using SpeakingLanguage.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace SpeakingLanguage.Core
{
    public interface ILoopState
    {
        TComponent Get<TComponent>() where TComponent : class, IStateComponent, new();
    }

    sealed class LoopState : ILoopState, IDisposable
    {
        private Dictionary<Type, IStateComponent> _components = new Dictionary<Type, IStateComponent>();
        
        public void Awake()
        {
            foreach (var comp in _components)
                comp.Value.Awake();
        }

        public TComponent Add<TComponent>() where TComponent : class, IStateComponent, new()
        {
            var comp = new TComponent();
            _components.Add(typeof(TComponent), comp);
            return comp;
        }

        public TComponent Get<TComponent>() where TComponent : class, IStateComponent, new()
        {
            IStateComponent comp;
            if (!_components.TryGetValue(typeof(TComponent), out comp)) return null;
            return comp as TComponent;
        }
        
        public void Dispose()
        {
            foreach (var comp in _components)
                comp.Value.Dispose();
            _components.Clear();
        }
    }
}
