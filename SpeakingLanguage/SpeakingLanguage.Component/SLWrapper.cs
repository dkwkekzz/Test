using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Component
{
    public struct SLWrapper : IEnumerable<SLComponent>
    {
        private SLComponent _root;
        private List<SLComponent> _coms;

        public int Count => _coms?.Count ?? 1;
        public bool IsNull => _root == null;

        public SLWrapper(SLComponent com)
        {
            _root = com;
            _coms = null;
        }

        IEnumerator<SLComponent> IEnumerable<SLComponent>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        
        public void Add(SLComponent com)
        {
            if (null == _root)
            {
                _root = com;
                return;
            }

            if (null == _coms)
            {
                _coms = new List<SLComponent>();
                _coms.Add(_root);
            }

            _coms.Add(com);
        }

        public int Sweep()
        {
            for (int i = _coms.Count - 1; i >= 0; i--)
            {
                var com = _coms[i];
                //if (null != filter && filter(com))
                //    _coms.RemoveAt(i);
                if (com.Type == ComponentType.None)
                    _coms.RemoveAt(i);
            }

            return _coms.Count;
        }

        public SLComponent First()
        {
            return _root;
        }

        public SLEnumerator GetEnumerator()
        {
            if (null == _coms)
                return new SLEnumerator(_root);
            return new SLEnumerator(_coms);
        }
    }
}
