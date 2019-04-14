using System;
using System.Collections.Generic;
using System.Linq;
using SpeakingLanguage.Core;
using SpeakingLanguage.Library;

namespace SpeakingLanguage.Linkage
{
    sealed class Graph
    {
        struct SyncData
        {
            public int referencedTick;
        }

        public static readonly string ROOT = "root";

        /// <summary>
        /// Node Dictionary
        /// List :: uniqueKey -> childKey : value
        /// 
        /// root -> observer : value
        /// observer -> mess : value 식으로 연결되어 있다.
        /// 하지만 mess의 속성을 접근하려고 하면 다음과 같은 규칙을 정한다.
        /// {상위객체}.mess -> speed : value
        /// </summary>
        private Dictionary<string, Dictionary<string, Node>> _nodeDic
            = new Dictionary<string, Dictionary<string, Node>>(Config.COUNT_MAX_SOURCE, new StringRefComparer());
        private Dictionary<string, SyncData> _root = new Dictionary<string, SyncData>(new StringRefComparer());

        private ObjectPool<Dictionary<string, Node>> _nodePool 
            = new ObjectPool<Dictionary<string, Node>>(() => 
            { return new Dictionary<string, Node>(Config.COUNT_DEFAULT_PROP); }, Config.COUNT_MAX_SOURCE);

        public IReadOnlyDictionary<string, Node> this[string name] => !_nodeDic.ContainsKey(name) ? null : _nodeDic[name];
        public IEnumerable<string> Root => _root.Select(pair => pair.Key);

        public bool HasLink(string src, string dest, int value, int weight)
        {
            Dictionary<string, Node> dic;
            if (!_nodeDic.TryGetValue(src, out dic))
                return false;

            Node node;
            if (!dic.TryGetValue(dest, out node))
                return false;

            return node.value == value && node.weight == weight;
        }

        public void AddLink(string src, string dest, int value, int weight)
        {
            _root[src] = new SyncData { referencedTick = Environment.TickCount };
            _root[dest] = new SyncData { referencedTick = Environment.TickCount };

            Dictionary<string, Node> dic;
            if (!_nodeDic.TryGetValue(src, out dic))
            {
                dic = _nodePool.GetObject();
                _nodeDic.Add(src, dic);
            }

            dic.Add(dest, new Node { key = dest, value = value, weight = weight });
        }

        public void RemoveLink(string src, string dest, int value, int weight)
        {
            Dictionary<string, Node> dic;
            if (!_nodeDic.TryGetValue(src, out dic))
                return;

            dic.Remove(dest);
            if (dic.Count == 0)
            {
                _nodePool.PutObject(dic);
            }
        }
    }

    public interface INode
    {
        string Key { get; set; }
        int Value { get; set; }
    }

    struct Edge
    {
        public int weight;
    }

    sealed class _Graph
    {
        sealed class Node : INode
        {
            public string Key { get; set; }
            public int Value { get; set; }
        }

        private static readonly string ROOT = "root";
        private Dictionary<string, Dictionary<string, INode>> _nodeDic 
            = new Dictionary<string, Dictionary<string, INode>>(new StringRefComparer());
        private Dictionary<string, Dictionary<string, Edge>> _edgeDic
            = new Dictionary<string, Dictionary<string, Edge>>(new StringRefComparer());
        private ObjectPool<Node> _nodePool;
        
        public _Graph()
        {
            //_nodePool = new ObjectPool<Node>(() => { return new Node(); }, Config.COUNT_MAX_NODE);
        }
        
        public INode CreateNode(string key, int value, INode parent)
        {
            var ret = _nodePool.GetObject();
            ret.Key = key;
            ret.Value = value;

            AddLink(parent, ret);

            return ret;
        }

        public INode CreateNode(string key, int value)
        {
            var ret = _nodePool.GetObject();
            ret.Key = key;
            ret.Value = value;
            return ret;
        }

        public bool HasLink(string src, string dest)
        {
            Dictionary<string, Edge> dic;
            if (!_edgeDic.TryGetValue(src, out dic))
                return false;

            return dic.ContainsKey(dest);
        }

        public bool HasLink(string src, string dest, int value)
        {
            INode node;
            if (!TryGetNode(src, dest, out node))
                return false;

            return node.Value == value;
        }

        public bool HasLink(string src, string dest, int value, int weight)
        {
            INode node;
            if (!TryGetNode(src, dest, out node))
                return false;

            Edge edge;
            if (!TryGetEdge(src, dest, out edge))
                return false;

            return node.Value == value && edge.weight == weight;
        }

        public bool TryGetNode(string src, string dest, out INode node)
        {
            Dictionary<string, INode> dic;
            if (!_nodeDic.TryGetValue(src, out dic))
            {
                node = null;
                return false;
            }

            return dic.TryGetValue(dest, out node);
        }

        public bool TryGetSource(string src, out INode node)
        {
            return TryGetNode(ROOT, src, out node);
        }

        public bool TryGetEdge(string src, string dest, out Edge edge)
        {
            Dictionary<string, Edge> dic;
            if (!_edgeDic.TryGetValue(src, out dic))
            {
                edge = default(Edge);
                return false;
            }

            return dic.TryGetValue(dest, out edge);
        }

        public void AddLink(string src, string dest, int value, int weight)
        {
            INode srcNode;
            if (!TryGetSource(src, out srcNode))
            {
                srcNode = CreateNode(src, value);
            }
            _nodeDic[dest].Add(src, srcNode);

            INode destNode;
            if (!TryGetSource(dest, out destNode))
            {
                destNode = CreateNode(dest, value);
            }
            _nodeDic[src].Add(dest, destNode);

            var edge = new Edge { weight = weight };
            _edgeDic[src][dest] = edge;
            _edgeDic[dest][src] = edge;
        }

        public void AddLink(INode src, INode dest, int weight = 0)
        {
            _nodeDic[src.Key].Add(dest.Key, dest);
            _nodeDic[dest.Key].Add(src.Key, src);

            var edge = new Edge { weight = weight };
            _edgeDic[src.Key][dest.Key] = edge;
            _edgeDic[dest.Key][src.Key] = edge;
        }

        public void RemoveLink(INode src, INode dest)
        {
            _nodeDic[src.Key].Remove(dest.Key);
            _nodeDic[dest.Key].Remove(src.Key);
            _edgeDic[src.Key].Remove(dest.Key);
            _edgeDic[dest.Key].Remove(src.Key);
        }

        //public int GetWeight(string src, string dest)
        //{
        //    Dictionary<string, Edge> dic;
        //    if (_edgeDic.TryGetValue(src, out dic))
        //    {
        //        Edge edge;
        //        if (dic.TryGetValue(dest, out edge))
        //        {
        //            return edge.weight;
        //        }
        //    }

        //    return 0;
        //}

        //public bool HasEdge(string src, string dest)
        //{
        //    Dictionary<string, Edge> dic;
        //    if (_edgeDic.TryGetValue(src, out dic))
        //    {
        //        Edge edge;
        //        if (dic.TryGetValue(dest, out edge))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //public void AddEdge(string src, string dest, int weight = 0)
        //{
        //    Dictionary<string, Edge> dic;
        //    if (!_edgeDic.TryGetValue(src, out dic))
        //    {
        //        dic = new Dictionary<string, Edge>(new StringRefComparer());
        //        _edgeDic.Add(src, dic);
        //    }

        //    dic[dest] = new Edge { src = src, dest = dest, weight = weight };
        //}

        //public void RemoveEdge(string src, string dest)
        //{
        //    Dictionary<string, Edge> dic;
        //    if (!_edgeDic.TryGetValue(src, out dic))
        //        return;

        //    dic.Remove(dest);
        //}
    }
}
