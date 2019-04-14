using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Component
{
    class Allocator : IDisposable
    {
        #region STATIC
        private static readonly Dictionary<Type, Allocator> _heads = new Dictionary<Type, Allocator>();

        public static IntPtr Calloc(Type type)
        {
            if (!_heads.TryGetValue(type, out Allocator allocator))
            {
                allocator = new Allocator(type, Config.DEFAULT_POOL_SIZE);
                _heads.Add(type, allocator);
            }

            return allocator.allocate();
        }

        public static void Cfree(Type type, IntPtr ptr, bool indirect)
        {
            if (!_heads.TryGetValue(type, out Allocator allocator))
                return;

            allocator.free(ptr, indirect);
        }
        #endregion

        private readonly int _chunkSize;
        private readonly IntPtr _root;
        private int _capacity;
        private long _tailOffset;
        private IntPtr _head;
        private Stack<IntPtr> _garbage = new Stack<IntPtr>();

        private Allocator(Type type, int capacity)
        {
            _chunkSize = Marshal.SizeOf(type);
            _capacity = capacity;
            try
            {
                _root = _head = Marshal.AllocHGlobal(capacity);
                _tailOffset = IntPtr.Add(_head, capacity).ToInt64();
            }
            catch (OutOfMemoryException oe)
            {
                _root = _head = Marshal.AllocHGlobal(capacity >> 1);
            }
        }

        private IntPtr allocate()
        {
            // 최대크기를 넘었을 경우, 더 큰 힙을 할당받아서 복사하는 구현을 해야함
            if (_tailOffset - _chunkSize <= _head.ToInt64())
                throw new OverflowException("allocate overflow");

            var p = _head;
            _head += _chunkSize;
            return p;
        }

        private void free(IntPtr ptr, bool indirect)
        {
            if (!indirect)
            {
                _garbage.Push(ptr);
            }
            else
            {
                throw new NotImplementedException("지금 존나 귀찮으니까 좀 나중에 구현");
            }
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_root);
        }
    }
}
