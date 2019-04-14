using System;
using System.Collections.Generic;
using System.Linq;
using SpeakingLanguage.Core;
using SpeakingLanguage.Library;

namespace SpeakingLanguage.Core
{
    public interface INode<T>
        where T : struct
    {
        INode<T> this[string key] { get; }
        T Value { get; set; }
        ValueType<T> ValueRef { get; }
    }

    public sealed class ValueType<T>
    {
        public T val;

        public override string ToString()
        {
            return val.ToString();
        }
    }

    public sealed class ValueMap<T> : IStateComponent
        where T : struct
    {
        class Node : INode<T>
        {
            public static ObjectPool<Node> Factory { get; } =
                new ObjectPool<Node>(() => { return new Node { }; }, Config.COUNT_MAX_VALUE_NODE, node => node.Release());

            private Dictionary<string, Node> _childs = new Dictionary<string, Node>(new StringRefComparer());
            private ValueType<T> _valueRef;

            public T Value { get { return _valueRef.val; } set { _valueRef.val = value; } }
            public ValueType<T> ValueRef => _valueRef;

            public INode<T> this[string key] => _childs[key];
            public void Capture(T val) => _valueRef.val = val;
            public void Release() => _childs.Clear();
        }

        private Node _root = new Node();
        public INode<T> Root => _root;

        public void Awake()
        {
        }

        public void Dispose()
        {
            _root.Release();
        }
    }
}
