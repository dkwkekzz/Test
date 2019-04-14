using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SpeakingLanguage.Library
{
    public struct Writer
    {
        private readonly static Library.BufferPool _pool = new Library.BufferPool();

        private byte[] _buffer;
        private int _offset;
        
        public Writer(int capacity = 1<<6)
        {
            _buffer = _pool.GetBuffer(capacity);
            _offset = 0;
        }

        public void WriteSuccess()
        {
            WriteBoolean(true);
        }

        public void WriteFailure(int error)
        {
            WriteBoolean(true);
            WriteInt(error);
        }

        public void WriteBoolean(bool v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _offset, v);
        }

        public void WriteInt(int v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _offset, v);
        }

        public void WriteLong(long v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _offset, v);
        }

        public void WriteFloat(float v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _offset, v);
        }

        public void WriteString(string v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _offset, v);
        }
        
        public unsafe void WriteMemory(void* p, int sz)
        {
            fixed (void* bp = &_buffer[_offset])
            {
                Buffer.MemoryCopy(p, bp, sz, sz);
                _offset += sz;
            }
        }

        public void WriteStream(Stream stream)
        {
            stream.Position = 0;
            stream.Read(_buffer, _offset, (int)stream.Length);
            _offset += (int)stream.Length;
        }

        public void Flush(Stream stream)
        {
            stream.Write(_buffer, 0, _offset);
        }

        public async Task FlushAsync(Stream stream)
        {
            await stream.WriteAsync(_buffer, 0, _offset);
        }

        public byte[] GetBuffer()
        {
            var nBuf = new byte[_offset];
            Buffer.BlockCopy(_buffer, 0, nBuf, 0, _offset);
            return nBuf;
        }

        public MemoryStream GetStream()
        {
            return new MemoryStream(_buffer, 0, _offset);
        }
    }
}
