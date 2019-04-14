using SpeakingLanguage.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SpeakingLanguage.DataManagement
{
    public interface IGroup
    {
        Type Type { get; }
        object Get(int actorHandle);
        void Set(int actorHandle, object prop);
    }

    public sealed class Group<TProperty> : IGroup, IEnumerable<TProperty>
        where TProperty : struct
    {
        private Type _originType;
        private Dictionary<int, TProperty> _actor2ItemDic;

        public Type Type => _originType;
        public IEnumerator<TProperty> GetEnumerator() => _actor2ItemDic.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public Group(int capacity)
        {
            _originType = typeof(TProperty);
            _actor2ItemDic = new Dictionary<int, TProperty>(capacity);
        }

        public void Add(int actorHandle)
        {
            _actor2ItemDic.Add(actorHandle, default(TProperty));
        }

        public void Set(int actorHandle, ref TProperty prop)
        {
            _actor2ItemDic[actorHandle] = prop;
        }

        public void Set(int actorHandle, object prop)
        {
            _actor2ItemDic[actorHandle] = (TProperty)prop;
        }

        public object Get(int actorHandle)
        {
            return _actor2ItemDic.TryGetValue(actorHandle, out TProperty prop) ? (object)prop : null;
        }

        public bool TryGet(int actorHandle, out TProperty prop)
        {
            return _actor2ItemDic.TryGetValue(actorHandle, out prop);
        }
    }
}