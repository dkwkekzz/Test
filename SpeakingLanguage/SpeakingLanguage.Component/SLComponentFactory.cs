using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Component
{
    public partial class SLComponent
    {
        private readonly static Library.ObjectPool<SLComponent> _factory
            = new Library.ObjectPool<SLComponent>(Config.MAX_COUNT_SLC, () => new SLComponent());
        private readonly static Library.BufferPool _bfFactory = new Library.BufferPool();

        public static SLComponent Create(ComponentType type)
        {
            var com = _factory.GetObject().onTake(type, _factory.GenerateIndex);
            prelink(com);
            return com;
        }

        public static SLComponent Create(ComponentType type, int handle)
        {
            var com = _factory.GetObject().onTake(type, _factory.GenerateIndex);
            prelink(com);
            Root.LinkTo(handle, com);
            return com;
        }

        public static SLComponent Create(ComponentType type, object key)
        {
            var com = _factory.GetObject().onTake(type, _factory.GenerateIndex);
            prelink(com);
            Root.LinkTo(key, com);
            return com;
        }

        private static void prelink(SLComponent com)
        {
            Root.LinkTo(com);
            var ptr = SLPointer.Index(com._index);
            com.insert(ref ptr, com);
        }

        public static bool Destroy(int handle)
        {
            if (!Root.TryFind(handle, out SLWrapper wrapper))
                return false;

            _factory.PutObject(wrapper.First().onRelease());
            return true;
        }

        public static bool Destroy(object key)
        {
            if (!Root.TryFind(key, out SLWrapper wrapper))
                return false;

            _factory.PutObject(wrapper.First().onRelease());
            return true;
        }

        public static void Destroy(SLComponent com)
        {
            _factory.PutObject(com.onRelease());
        }

        public static async Task<SLComponent> LoadAsync(string name)
        {
            var path = $"{name}.slc";
            if (!File.Exists(path))
                return null;

            using (var fs = File.OpenRead(path))
            {
                var buffer = _bfFactory.GetBuffer(1 << 16);
                Trace.Assert(buffer.Length >= fs.Length, "overflow buffer");

                var n = await fs.ReadAsync(buffer, 0, (int)fs.Length);
                if (n == 0)
                    return null;

                var reader = new Library.Reader(buffer, n);
                return Serialization.ReadComponent(ref reader);
            }
        }

        public static async Task SaveAsync(SLComponent com, string name)
        {
            using (var fs = File.OpenWrite($"{name}.slc"))
            {
                var writer = new Library.Writer(1 << 16);
                Serialization.WriteComponent(ref writer, com);
                await writer.FlushAsync(fs);
            }
        }
    }
}
