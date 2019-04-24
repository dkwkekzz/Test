using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Component
{
    public struct SLEnumerator : IEnumerator<SLComponent>
    {
        private SLComponent _root;
        private List<SLComponent> _coms;
        private int _index;
        public SLComponent Current { get; private set; }
        object IEnumerator.Current => this.Current;

        public SLEnumerator(List<SLComponent> coms)
        {
            _root = coms[0];
            _coms = coms;
            _index = -1;
            Current = null;
        }

        public SLEnumerator(SLComponent com)
        {
            _root = com;
            _coms = null;
            _index = -1;
            Current = null;
        }

        public bool MoveNext()
        {
            if (null == _coms)
            {
                Current = _root;
                return _index++ < 1;
            }

            if (++_index >= _coms.Count)
                return false;

            Current = _coms[_index];
            if (Current.Type == ComponentType.None)
                return MoveNext();

            return true;
        }

        public bool Advance(int ofs)
        {
            _index += ofs;
            if (_index >= _coms.Count)
                return false;
            if (_index < 0)
                return false;

            Current = _coms[_index];
            return true;
        }

        public void Dispose()
        {
        }

        public void Reset()
        {
            _index = -1;
            Current = null;
        }
    }
}
