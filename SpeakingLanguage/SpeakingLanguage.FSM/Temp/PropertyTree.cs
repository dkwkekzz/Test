using SpeakingLanguage.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.FSM
{
    public interface IGroup
    {
        string Subject { get; }
        bool TrySearch(int actorHandle, string valueKey, out int value);
        bool TrySearch(int actorHandle, string valueKey, out float value);
        bool TrySearch(int actorHandle, string valueKey, out string value);
    }

    public sealed class PropertyTree
    {
        class Group : IGroup
        {
            class ValueSet
            {
                public Dictionary<string, int> Integer { get; } = new Dictionary<string, int>(new StringRefComparer());
                public Dictionary<string, float> Float { get; } = new Dictionary<string, float>(new StringRefComparer());
                public Dictionary<string, string> String { get; } = new Dictionary<string, string>(new StringRefComparer());
            }

            private Dictionary<int, ValueSet> _setDic = new Dictionary<int, ValueSet>();
            private IEnumerator<KeyValuePair<int, ValueSet>> _setIter = null;

            public string Subject { get; private set; }

            public Group(string subject)
            {
                _setIter = _setDic.GetEnumerator();
                Subject = subject;
            }
            
            public bool TrySearch(int actorHandle, string valueKey, out int value)
            {
                if (!_setDic.TryGetValue(actorHandle, out ValueSet set))
                {
                    value = 0;
                    return false;
                }

                return set.Integer.TryGetValue(valueKey, out value);
            }

            public bool TrySearch(int actorHandle, string valueKey, out float value)
            {
                if (!_setDic.TryGetValue(actorHandle, out ValueSet set))
                {
                    value = default(float);
                    return false;
                }

                return set.Float.TryGetValue(valueKey, out value);
            }

            public bool TrySearch(int actorHandle, string valueKey, out string value)
            {
                if (!_setDic.TryGetValue(actorHandle, out ValueSet set))
                {
                    value = default(string);
                    return false;
                }

                return set.String.TryGetValue(valueKey, out value);
            }
        }

        private Dictionary<string, Group> _groupDic = new Dictionary<string, Group>(new StringRefComparer());

        public IGroup Search(string subject)
        {
            return _groupDic.ContainsKey(subject) ? _groupDic[subject] : null;
        }

        public void Insert(string subject)
        {
            _groupDic.Add(subject, new Group(subject));
        }
    }

    public sealed class PropertyTree3
    {
        struct Pointer : IEquatable<Pointer>
        {
            private static int noAllocator = 0;

            public static Pointer New => new Pointer { value = noAllocator++ };
            public static Pointer Null => new Pointer { value = 0 };

            private int value;
            
            public bool Equals(Pointer other) => value == other.value;
            public static implicit operator int(Pointer p) => p.value;
            public override int GetHashCode() => value;
        }

        class Node : IEnumerable<Pointer>
        {
            public static ObjectPool<Node> Factory { get; } = new ObjectPool<Node>(Config.COUNT_PROPERTYTREE_NODE, 
                () => { return new Node(); }, node => { node.Clear(); });

            private Dictionary<string, Pointer> _ptrDic = new Dictionary<string, Pointer>(new StringRefComparer());
            private IEnumerator<Pointer> _ptrIter;

            public Node()
            {
                var query = from ptrPair in _ptrDic
                            select ptrPair.Value;
                _ptrIter = query.GetEnumerator();
            }

            public IEnumerator<Pointer> GetEnumerator()
            {
                _ptrIter.Reset();
                return _ptrIter;
            }
            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            public Pointer this[string key]
            {
                get => _ptrDic[key];//_ptrDic.ContainsKey(key) ? _ptrDic[key] : Pointer.Null;
                set => _ptrDic[key] = value;
            }
            public bool TryTake(string key, out Pointer p) => _ptrDic.TryGetValue(key, out p);
            public void Clear() => _ptrDic.Clear();
        }

        class Cache
        {
            struct Chunk
            {
                public string key;
                public Node node;
            }

            private Chunk _gen0 = new Chunk();
            private Chunk _gen1 = new Chunk();
            private Chunk _gen2 = new Chunk();

            public Node this[int depth, string key]
            {
                get
                {
                    if (depth == 0 && Object.ReferenceEquals(_gen0.key, key))
                        return _gen0.node;
                    if (depth == 1 && Object.ReferenceEquals(_gen1.key, key))
                        return _gen1.node;
                    if (depth == 2 && Object.ReferenceEquals(_gen2.key, key))
                        return _gen2.node;
                    return null;
                }
                set
                {
                    if (depth == 0)
                        _gen0 = new Chunk { key = key, node = value };
                    if (depth == 1)
                        _gen1 = new Chunk { key = key, node = value };
                    if (depth == 2)
                        _gen2 = new Chunk { key = key, node = value };
                }
            }
        }

        private Node _root = new Node();

        private Dictionary<Pointer, Node> _nodeDic = new Dictionary<Pointer, Node>();
        private Dictionary<Pointer, int> _integerDic = new Dictionary<Pointer, int>();
        private Dictionary<Pointer, float> _floatDic = new Dictionary<Pointer, float>();
        private Dictionary<Pointer, string> _stringDic = new Dictionary<Pointer, string>();
        
        public void Exchange(int value, string key)
        {
            var head = _root;
            if (!head.TryTake(key, out Pointer p))
            {
                p = Pointer.New;
                head[key] = p;
            }

            _integerDic[p] = value;
        }

        public void Exchange(int value, string key1, string key2)
        {
            var head = _root;
            if (!head.TryTake(key1, out Pointer p1))
            {
                head = Node.Factory.GetObject();
                _nodeDic.Add(Pointer.New, head);
            }
            else
            {
                head = _nodeDic[p1];
            }

            if (!head.TryTake(key2, out Pointer p2))
            {
                p2 = Pointer.New;
                head[key2] = p2;
            }

            _integerDic[p2] = value;
        }

        public void Exchange(int value, string key1, string key2, string key3)
        {
            var head = _root;
            if (!head.TryTake(key1, out Pointer p1))
            {
                head = Node.Factory.GetObject();
                _nodeDic.Add(Pointer.New, head);
            }
            else
            {
                head = _nodeDic[p1];
            }

            if (!head.TryTake(key2, out Pointer p2))
            {
                head = Node.Factory.GetObject();
                _nodeDic.Add(Pointer.New, head);
            }
            else
            {
                head = _nodeDic[p2];
            }

            if (!head.TryTake(key3, out Pointer p3))
            {
                p3 = Pointer.New;
                head[key3] = p3;
            }

            _integerDic[p3] = value;
        }

        public void Exchange(int value, params string[] keys)
        {
            var head = _root;
            for (int i = 0; i != keys.Length - 1; i++)
            {
                if (!head.TryTake(keys[i], out Pointer p1))
                {
                    head = Node.Factory.GetObject();
                    _nodeDic.Add(Pointer.New, head);
                }
                else
                {
                    head = _nodeDic[p1];
                }
            }

            var finKey = keys[keys.Length - 1];
            if (!head.TryTake(finKey, out Pointer p2))
            {
                p2 = Pointer.New;
                head[finKey] = p2;
            }

            _integerDic[p2] = value;
        }

        public bool TryTake(out int value, string key)
        {
            var head = _root;
            if (!head.TryTake(key, out Pointer p))
            {
                value = 0;
                return false;
            }

            return _integerDic.TryGetValue(p, out value);
        }

        public bool TryTake(out int value, string key1, string key2)
        {
            var head = _root;
            if (!head.TryTake(key1, out Pointer p))
            {
                value = 0;
                return false;
            }

            head = _nodeDic[p];
            if (!head.TryTake(key2, out p))
            {
                value = 0;
                return false;
            }

            return _integerDic.TryGetValue(p, out value);
        }

        public bool TryTake(out int value, string key1, string key2, string key3)
        {
            var head = _root;
            if (!head.TryTake(key1, out Pointer p))
            {
                value = 0;
                return false;
            }

            head = _nodeDic[p];
            if (!head.TryTake(key2, out p))
            {
                value = 0;
                return false;
            }

            head = _nodeDic[p];
            if (!head.TryTake(key3, out p))
            {
                value = 0;
                return false;
            }

            return _integerDic.TryGetValue(p, out value);
        }

        public bool TryTake(out int value, params string[] keys)
        {
            var head = _root;
            if (!head.TryTake(keys[0], out Pointer p))
            {
                value = 0;
                return false;
            }

            for (int i = 1; i != keys.Length; i++)
            {
                head = _nodeDic[p];
                if (!head.TryTake(keys[i], out p))
                {
                    value = 0;
                    return false;
                }
            }
            
            return _integerDic.TryGetValue(p, out value);
        }
    }

    public class PropertyTree2
    {
        public ValueMap<int> Integer { get; } = new ValueMap<int>();
        public ValueMap<float> Float { get; } = new ValueMap<float>();
        public ValueMap<string> String { get; } = new ValueMap<string>();
    }
    
    public class ValueMap<T>
    {
        class Proxy : IProxy<T>
        {
            private Dictionary<string, T> _dic = new Dictionary<string, T>(new StringRefComparer());

            public T this[string key] { get => _dic[key]; set => _dic[key] = value; }
        }

        private Dictionary<string, Proxy> _valueDic = new Dictionary<string, Proxy>(new StringRefComparer());

        public IProxy<T> this[string entityKey]
        {
            get => _valueDic.ContainsKey(entityKey) ? _valueDic[entityKey] : null;
        }

        public void Insert(string entityKey)
        {
            _valueDic[entityKey] = new Proxy();
        }
    }

    public interface IProxy<T>
    {
        T this[string key] { get; set; }
    }
}
