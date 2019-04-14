using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Core
{
    public sealed class SourceBuffer<TSource> : IStateComponent, IEnumerable<TSource>
    {
        public class SourceIterator : IEnumerator<TSource>
        {
            private readonly TSource[] _srcBuffer;
            private readonly int _maxLength;
            private int _currentIndex;

            public TSource Current => _srcBuffer[_currentIndex];
            object IEnumerator.Current => _srcBuffer[_currentIndex];

            public SourceIterator(TSource[] srcBuffer, int length)
            {
                _srcBuffer = srcBuffer;
                _maxLength = length;
                _currentIndex = -1;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                _currentIndex++;
                if (_currentIndex < 0 || _currentIndex >= _maxLength)
                    return false;
                return true;
            }

            public void Reset()
            {
                _currentIndex = -1;
            }
        }

        private TSource[] _srcBuffer;
        private TSource[] _tempBuffer;
        private int _frontIndex;
        private int _backIndex;
        private int _validLength;

        public bool IsCommitted => _validLength > 0;
        public int Count => _validLength;

        public TSource this[int index] { get => _srcBuffer[index]; set => _tempBuffer[_capture(index)] = value; }
        public TSource this[long index] { get => _srcBuffer[(int)index]; set => _tempBuffer[_capture(index)] = value; }
        
        public void Awake()
        {
            _srcBuffer = new TSource[Config.COUNT_MAX_SOURCE];
            _tempBuffer = new TSource[Config.COUNT_MAX_SOURCE];
        }

        public void Dispose()
        {
        }

        public void Flush()
        {
            Volatile.Write(ref _validLength, 0);
            _frontIndex = 0;
            _backIndex = Config.COUNT_MAX_SOURCE - 1;
        }

        public void Commit()
        {
            //if (Volatile.Read(ref _validLength) != 0)
            //    throw new InvalidOperationException("You should first empty the buffer: _validLength != 0");

            //if (0 != _validLength)
            //{
            //    // srcBuffer ~ vaildLength + tempBuffer ~ maxIndex
            //    Array.Copy(_tempBuffer, 0, _srcBuffer, _validLength, _maxIndex + 1);
            //}
            //else
            {
                var backCount = Config.COUNT_MAX_SOURCE - 1 - _backIndex;
                Buffer.BlockCopy(_tempBuffer, _backIndex, _tempBuffer, _frontIndex, backCount);

                var temp = _srcBuffer;
                _srcBuffer = _tempBuffer;
                _tempBuffer = temp;
            }

            Volatile.Write(ref _validLength, _frontIndex + 1);
        }

        public void Insert(TSource src)
        {
            _tempBuffer[_frontIndex] = src;
            Interlocked.Increment(ref _frontIndex);
        }

        // Append는 뒤에서부터 추가한다. 
        public void Append(TSource src)
        {
            _tempBuffer[_backIndex] = src;
            Interlocked.Decrement(ref _backIndex);
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            return new SourceIterator(_srcBuffer, _validLength);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SourceIterator(_srcBuffer, _validLength);
        }

        private int _capture(int index)
        {
            Volatile.Write(ref _frontIndex, System.Math.Max(index, _frontIndex));
            return index;
        }

        private int _capture(long index)
        {
            Volatile.Write(ref _frontIndex, System.Math.Max((int)index, _frontIndex));
            return (int)index;
        }
    }
}
