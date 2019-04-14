using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Component
{
    
    
    struct Edge
    {
        public Type type;
        public Handle dest;
        public int value;
    }

    public struct Actor
    {
    }
    
    public sealed class Graph
    {
        private readonly Dictionary<Handle, List<Edge>> _hd2edgeDic = new Dictionary<Handle, List<Edge>>();

        public IEnumerable<Handle> GetLink(Handle h, Type type = null)
        {
            if (!_hd2edgeDic.TryGetValue(h, out List<Edge> list))
                yield break;
            
            for (int i = 0; i != list.Count; i++)
            {
                var edge = list[i];
                if (null == type || edge.type == type)
                    yield return edge.dest;
            }
        }

        public void AddLink(Handle src, Type type, Handle dest)
        {
            if (!_hd2edgeDic.TryGetValue(src, out List<Edge> list))
            {
                list = new List<Edge>();
                _hd2edgeDic.Add(src, list);
            }

            list.Add(new Edge { type = type, dest = dest, value = 1 });
        }

        public void RemoveLink(Handle src, Handle dest, bool bidirect = true)
        {
            if (!_hd2edgeDic.TryGetValue(src, out List<Edge> list))
                return;

            int i;
            for (i = 0; i != list.Count; i++)
            {
                if (list[i].dest == dest)
                    break;
            }

            list.RemoveAt(i);
            if (bidirect)
                RemoveLink(dest, src, false);
        }

        public void RemoveLink(Handle src, Type type, bool bidirect = true)
        {
            if (!_hd2edgeDic.TryGetValue(src, out List<Edge> list))
                return;

            int i;
            for (i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].type == type)
                {
                    list.RemoveAt(i);
                    if (bidirect)
                        RemoveLink(list[i].dest, src, false);
                }
            }
        }
    }
}
