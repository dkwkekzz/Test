using System;
using System.Collections.Generic;
using System.Linq;
using SpeakingLanguage.Core;
using SpeakingLanguage.Library;

namespace SpeakingLanguage.Core
{
    public sealed class Graph : SpeakingService
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

        public bool TryGetNode(string src, string dest, out Node node)
        {
            Dictionary<string, Node> dic;
            if (!_nodeDic.TryGetValue(src, out dic))
            {
                node = default(Node);
                return false;
            }

            return dic.TryGetValue(dest, out node);
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
}
