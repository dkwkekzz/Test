using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Component
{
    public sealed class SimpleLookup
    {
        private readonly Dictionary<object, SLComponent> _refs = new Dictionary<object, SLComponent>();
        private readonly Dictionary<int, SLComponent> _ints = new Dictionary<int, SLComponent>();
        //private readonly Dictionary<string, SLComponent> _strs = new Dictionary<string, SLComponent>();
        
        public SLComponent Find(object key)
        {
            if (!_refs.TryGetValue(key, out SLComponent com))
                return null;
            return com;
        }

        public void Regist(object key, SLComponent com)
        {
            _refs.Add(key, com);
        }

        public void Unregist(object key)
        {
            _refs.Remove(key);
        }

        public SLComponent Find(int key)
        {
            if (!_ints.TryGetValue(key, out SLComponent com))
                return null;
            return com;
        }

        public void Regist(int key, SLComponent com)
        {
            _ints.Add(key, com);
        }

        public void Unregist(int key)
        {
            _ints.Remove(key);
        }

        //public SLComponent Find(string key)
        //{
        //    if (!_strs.TryGetValue(key, out SLComponent com))
        //        return null;
        //    return com;
        //}

        //public void Regist(string key, SLComponent com)
        //{
        //    _strs.Add(key, com);
        //}

        //public void Unregist(string key)
        //{
        //    _strs.Remove(key);
        //}
    }
}
