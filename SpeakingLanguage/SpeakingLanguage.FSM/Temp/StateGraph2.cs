using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpeakingLanguage.FSM
{
    //class StateGraph2
    //{
    //    private Dictionary<string, Dictionary<string, int>> _edgeDic = new Dictionary<string, Dictionary<string, int>>(new StringRefComparer());

    //    public bool HasLink(string src, string dest) => _edgeDic[src]?.ContainsKey(dest) ?? false;
    //    public void AddLink(string src, string dest, int weight = 0)
    //    {
    //        Dictionary<string, int> dic;
    //        if (!_edgeDic.TryGetValue(src, out dic))
    //        {
    //            dic = new Dictionary<string, int>(new StringRefComparer());
    //            _edgeDic.Add(src, dic);
    //        }

    //        dic[dest] = weight;
    //    }
    //}
}
