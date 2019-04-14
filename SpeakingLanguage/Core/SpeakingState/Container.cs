using SpeakingLanguage.Library;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Core
{
    class Container<TSource> : IContainer<TSource>
    {
        private List<TSource> _list;
        private IEnumerator<TSource> _iter;

        public int Count => _list.Count;
        public TSource this[int index] { get => _list[index]; set => _list[index] = value; }
        public TSource this[long index] { get => _list[(int)index]; set => _list[(int)index] = value; }

        public Container(int capacity)
        {
            _list = new List<TSource>(capacity);
            _iter = _list.GetEnumerator();
        }

        public void Clear()
        {
            _list.Clear();
        }
        
        public void Push(TSource src)
        {
            _list.Add(src);
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            _iter.Reset();
            return _iter;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            _iter.Reset();
            return _iter;
        }
    }

    public interface IContainer<TSource> : IEnumerable<TSource>, IInjectable<TSource>
    {
        int Count { get; }
        void Clear();
    }
}
