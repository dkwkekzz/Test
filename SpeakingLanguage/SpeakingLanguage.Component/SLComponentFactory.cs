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
        public class ComponentPool
        {
            private readonly Library.ObjectPool<SLComponent> _pool;
            
            public ComponentPool(int capacity)
            {
                _pool = new Library.ObjectPool<SLComponent>(capacity, () => new SLComponent());
            }
            
            public SLComponent Create(SLComponent root, ComponentType type)
            {
                var com = _pool.GetObject().onTake(type, _pool.GenerateIndex);
                if (null != root)
                {
                    defaultLink(root, com);
                    unsafe
                    {
                        var serviceProp = root.Get<Property.Service>();
                        serviceProp->createCount++;
                    }
                }
                
                return com;
            }

            private void defaultLink(SLComponent root, SLComponent com)
            {
                root.LinkTo(com);
                var ptr = SLPointer.Index(com._index);
                root.insert(ref ptr, com);
            }

            public SLComponent Create(SLComponent root, ComponentType type, int handle)
            {
                var com = Create(root, type);
                root.LinkTo(handle, com);
                return com;
            }

            public SLComponent Create(SLComponent root, ComponentType type, string key)
            {
                var com = Create(root, type);
                root.LinkTo(key, com);
                return com;
            }
            
            public void Destroy(SLComponent root, SLComponent com)
            {
                if (null != root)
                {
                    unsafe
                    {
                        var serviceProp = root.Get<Property.Service>();
                        serviceProp->destroyCount++;
                    }
                }

                _pool.PutObject(com.onRelease());
            }

            public bool Destroy(SLComponent root, int handle)
            {
                if (!root.TryFind(handle, out SLWrapper wrapper))
                    return false;

                Destroy(root, wrapper);
                return true;
            }

            public bool Destroy(SLComponent root, string key)
            {
                if (!root.TryFind(key, out SLWrapper wrapper))
                    return false;

                Destroy(root, wrapper);
                return true;
            }
            
            public async Task<SLComponent> LoadAsync(SLComponent root, string name)
            {
                var path = $"{name}.slc";
                if (!File.Exists(path))
                    return null;

                using (var fs = File.OpenRead(path))
                {
                    var buffer = Library.Locator.BufferPool.GetBuffer(1 << 16);
                    Library.Logger.Assert(buffer.Length >= fs.Length, "overflow buffer");

                    var n = await fs.ReadAsync(buffer, 0, (int)fs.Length);
                    if (n == 0)
                        return null;
                    
                    var reader = new Library.Reader(buffer, n);
                    var com = Serialization.ReadComponent(ref reader);
                    defaultLink(root, com);
                    return com;
                }
            }

            public async Task SaveAsync(SLComponent com, string name)
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
}
