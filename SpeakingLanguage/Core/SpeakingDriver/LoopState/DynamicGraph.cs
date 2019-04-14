using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpeakingLanguage.Core
{
    public sealed class DynamicGraph : IStateComponent
    {
        struct Weight
        {
            public int value;
        }

        private Dictionary<string, Dictionary<string, Weight>> _nodeDic
            = new Dictionary<string, Dictionary<string, Weight>>(Config.COUNT_MAX_SOURCE, new StringRefComparer());

        public void Awake()
        {
        }

        public void Dispose()
        {
        }

        public void AddLink(string src, string dest, int weight)
        {
            Dictionary<string, Weight> dic;
            if (!_nodeDic.TryGetValue(src, out dic))
            {
                dic = new Dictionary<string, Weight>(new StringRefComparer());
                _nodeDic.Add(src, dic);
            }

            dic[dest] = new Weight { value = weight };
        }

        public void Refresh()
        {
            foreach (var dicPair in _nodeDic)
            {
                dicPair.Value.Clear();
            }
        }

    }
}
