using SpeakingLanguage.Core;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Linkage
{
    sealed class Reader
    {
        public Token TryExecute(Graph graph, Token token)
        {
            Token head = null;
            Token tail = null;

            var lineItr = token;
            while (null != lineItr)
            {
                var pass = _readSingle(graph, lineItr);
                if (null != pass)
                {
                    if (null == head)
                    {
                        head = pass;
                        tail = pass;
                    }
                    else
                    {
                        tail += pass;
                    }
                }

                lineItr = lineItr.Enter;
            }
            
            return head;
        }

        private Token _readSingle(Graph graph, Token lineItr)
        {
            bool isContinue = true;

            var itr = lineItr;
            while (null != itr)
            {
                if ((itr._flag & TokenFlag.Read) != 0)
                {
                    if ((itr._flag & TokenFlag.And) != 0)
                        isContinue &= graph.HasLink(itr.Src, itr.Dest, itr.Value, itr.Weight);
                    else if ((itr._flag & TokenFlag.Or) != 0)
                        isContinue |= graph.HasLink(itr.Src, itr.Dest, itr.Value, itr.Weight);
                    else if ((itr._flag & TokenFlag.Enter) != 0)
                        isContinue = graph.HasLink(itr.Src, itr.Dest, itr.Value, itr.Weight);
                }
                else if ((itr._flag & TokenFlag.Write) != 0)
                {
                    if (!isContinue)
                        return null;
                    
                    if ((itr._flag & TokenFlag.Comma) != 0)
                        return itr;
                    else if ((itr._flag & TokenFlag.Add) != 0)
                        graph.AddLink(itr.Src, itr.Dest, itr.Value, itr.Weight);
                    else if ((itr._flag & TokenFlag.Remove) != 0)
                        graph.RemoveLink(itr.Src, itr.Dest, itr.Value, itr.Weight);
                }

                itr = itr.Next;
            }

            return null;
        }
    }
}
