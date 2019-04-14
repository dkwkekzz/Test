using SpeakingLanguage.Library;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Core
{
    struct JobChunk
    {
        public int begin;
        public int end;
    }
    
    sealed class JobPartitioner : SpeakingService, IEnumerator<JobChunk> 
    {
        private readonly ConcurrentStack<JobChunk> _chunks = new ConcurrentStack<JobChunk>();
        private int _capacity;
        private int _workerCount;
        //private int _currentChunkIndex = -1;

        public JobChunk Current
        {
            get
            {
                JobChunk chunk;
                if (!_chunks.TryPop(out chunk))
                    return default(JobChunk);
                return chunk;
            }
        }
        object IEnumerator.Current => Current;
        
        public void Dispose()
        {
            Reset();
            _chunks.Clear();
        }
        
        public IEnumerator<JobChunk> Take(int workerCount, int capacity)
        {
            _capacity = capacity;
            _workerCount = workerCount;

            if (workerCount == 1 || capacity <= Config.LENGTH_MIN_CHUNK)
            {
                _chunks.Push(new JobChunk { begin = 0, end = capacity });
                return this;
            }

            // partitional by n
            //var offset = capacity >> 2;
            //for (int i = 0; i != workerCount; i++)
            //{
            //    _chunks.Push(new JobChunk { begin = 0, end = System.Math.Min(offset * i, capacity) });
            //}

            // partitional by small chunk
            //var head = 0;
            //var offset = Config.LENGTH_MIN_CHUNK;
            //while (head < capacity - 1)
            //{
            //    _chunks.Push(new JobChunk { begin = head, end = System.Math.Min((head += offset), capacity - 1) });
            //}

            var head = 0;
            var offset = Config.LENGTH_MIN_CHUNK;
            while (head < capacity - 1)
            {
                for (int i = 0; i != workerCount; i++)
                {
                    _chunks.Push(new JobChunk { begin = head, end = System.Math.Min(capacity - 1, head + offset) });
                    head += offset;
                    if (head >= capacity - 1) break;
                }

                offset <<= 1;
            }

            //var log2 = Library.Math.Log2ge(capacity);
            //var depth = Library.Math.Log2ge(log2) >> 1;
            //var bigOffset = (1 << Library.Math.Log2ge(capacity)) >> depth;
            //var offset = bigOffset >> Library.Math.Log2ge(workerCount);
            //var head = 0;
            //while (head < capacity - 1)
            //{
            //    var begin = head;
            //    while (head < begin + bigOffset && head < capacity - 1)
            //    {
            //        head += offset;
            //        if (head > capacity - 1)
            //            head = capacity - 1;

            //        _chunks.Add(new JobChunk { begin = head - offset, end = head });
            //    }

            //    offset >>= 1;
            //}

            return this;
        }

        public bool MoveNext()
        {
            //_currentChunkIndex++;
            //if (_currentChunkIndex < 0 || _currentChunkIndex >= _chunks.Count)
            //    return false;
            //return true;
            return _chunks.Count > 0;
        }

        public void Reset()
        {
            //_currentChunkIndex = -1;
        }
    }
}
