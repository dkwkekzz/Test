using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Component
{
    public static class Function
    {
        public static void BidLink(SLComponent lhs, SLComponent rhs)
        {
            lhs.LinkTo(rhs);
            rhs.LinkTo(lhs);
        }
        
        public static void DFS()
        {

        }

        public static SLComponent BFS(SLComponent root, ComponentType t)
        {
            var visited = new HashSet<SLComponent>();
            var q = new Queue<SLComponent>();
            q.Enqueue(root);
            visited.Add(root);
            while (q.Count > 0)
            {
                var here = q.Dequeue();
                if (here.Type == t)
                    return here;

                var gIter = here.GetGroupEnumerator();
                while (gIter.MoveNext())
                {
                    var wrap = gIter.Current;
                    var wIter = wrap.GetEnumerator();
                    while (wIter.MoveNext())
                    {
                        var there = wIter.Current;
                        if (!visited.Contains(there))
                        {
                            q.Enqueue(there);
                            visited.Add(there);
                        }
                    }
                }
            }

            return null;
        }
    }
}
