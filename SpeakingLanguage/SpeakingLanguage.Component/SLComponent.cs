using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Component
{
    public unsafe partial class SLComponent : IEquatable<SLComponent>, IComparable<SLComponent>, IEnumerable<SLComponent>, ISerializable
    {
        public static SLComponent Root { get; } = _factory.GetObject().onTake(ComponentType.Root, _factory.GenerateIndex);
        public static SLComponent FakeRoot { get; } = _factory.GetObject().onTake(ComponentType.Root, _factory.GenerateIndex);

        private int _index;
        private Dictionary<SLPointer, SLWrapper> _groups = new Dictionary<SLPointer, SLWrapper>();
        private Dictionary<Type, IntPtr> _props = new Dictionary<Type, IntPtr>();
        private Type _cachedType;
        private IntPtr _cachedPtr;

        public ComponentType Type { get; private set; }
        public object Context { get; private set; }

        #region INTERFACE
        public int CompareTo(SLComponent other) => _index.CompareTo(other._index);
        public bool Equals(SLComponent other) => _index == other._index;
        public override bool Equals(object obj) => Equals((SLComponent)obj);
        public override int GetHashCode() => _index;
        public override string ToString() => $"[SLComponent] Type:{this.Type.ToString()}, Index:{_index.ToString()}";
        
        public IEnumerator<SLComponent> GetEnumerator()
        {
            var iter = _groups.Values.GetEnumerator();
            while (iter.MoveNext())
            {
                var group = iter.Current;
                var slIter = group.GetEnumerator();
                while (slIter.MoveNext())
                    yield return slIter.Current;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void OnSerialized(ref Library.Writer writer)
        {
            writer.WriteInt(_groups.Count);
            var gIter = _groups.GetEnumerator();
            while (gIter.MoveNext())
            {
                writer.WriteInt((int)gIter.Current.Key.type);
                writer.WriteInt(gIter.Current.Key.value);

                var wrapper = gIter.Current.Value;
                writer.WriteInt(wrapper.Count);
                var wIter = wrapper.GetEnumerator();
                while (wIter.MoveNext())
                {
                    writer.WriteInt(wIter.Current._index);
                }
            }

            writer.WriteInt(_props.Count);
            var pIter = _props.GetEnumerator();
            while (pIter.MoveNext())
            {
                var type = pIter.Current.Key;
                var attr = type.GetCustomAttributes(false)[0] as Property.PropertyAttribute;
                var index = attr.Index;
                var size = attr.Size;
                writer.WriteInt(index);
                writer.WriteInt(size);

                var ptr = pIter.Current.Value;
                writer.WriteMemory(ptr.ToPointer(), size);
            }
        }

        public void OnDeserialized(ref Library.Reader reader)
        {
            var read = true;
            read &= reader.ReadInt(out int groupCount);
            Library.Logger.Assert(read, "[OnDeserialized] fail to read component: linkedCount");

            for (int i = 0; i != groupCount; i++)
            {
                read &= reader.ReadInt(out int type);
                read &= reader.ReadInt(out int value);
                read &= reader.ReadInt(out int childCount);
                Library.Logger.Assert(read, "[OnDeserialized] fail to read component: type|value|rootFakeIndex|childCount");

                var ptr = new SLPointer { type = (PointerType)type, value = value };
                var wrapper = new SLWrapper();

                for (int k = 0; k != childCount; k++)
                {
                    read &= reader.ReadInt(out int fakeIndex);
                    Library.Logger.Assert(read, "[OnDeserialized] fail to read component: fakeIndex");

                    var child = SLComponent.FakeRoot.Find(fakeIndex);
                    wrapper.Add(child.First());
                }

                _groups.Add(ptr, wrapper);
            }
            
            read &= reader.ReadInt(out int propCount);
            Library.Logger.Assert(read, "[OnDeserialized] fail to read component: propCount");

            for (int i = 0; i != propCount; i++)
            {
                read &= reader.ReadInt(out int propIndex);
                read &= reader.ReadInt(out int propSize);
                Library.Logger.Assert(read, "[OnDeserialized] fail to read component: propIndex|propSize");

                var propType = Property.PropertyAttribute.GetProperty(propIndex);
                unsafe
                {
                    var propPtr = this.Get(propType);
                    reader.ReadMemory((void*)propPtr, propSize);
                }
            }
        }
        #endregion

        public int GroupCount => _groups.Count;
        public int PropertyCount => _props.Count;
        public SLWrapper Find(ComponentType type) => _groups[SLPointer.Type(type)];
        public SLWrapper Find(int handle) => _groups[SLPointer.Handle(handle)];
        public SLWrapper Find(object key) => _groups[SLPointer.Reference(key)];
        public bool TryFind(ComponentType type, out SLWrapper com) => _groups.TryGetValue(SLPointer.Type(type), out com);
        public bool TryFind(int handle, out SLWrapper com) => _groups.TryGetValue(SLPointer.Handle(handle), out com);
        public bool TryFind(object key, out SLWrapper com) => _groups.TryGetValue(SLPointer.Reference(key), out com);

        public void Traversal(Dictionary<int, SLComponent> list)
        {
            if (list.ContainsKey(_index))
                return;

            list.Add(_index, this);

            var gIter = _groups.GetEnumerator();
            while (gIter.MoveNext())
            {
                var slIter = gIter.Current.Value.GetEnumerator();
                while (slIter.MoveNext())
                {
                    slIter.Current.Traversal(list);
                }
            }
        }

        public SLComponent Attach(object context)
        {
            Context = context;
            return this;
        }

        public void LinkTo(SLComponent com)
        {
            var ptr = SLPointer.Type(com.Type);
            insert(ref ptr, com);
        }
        
        public void LinkTo(int handle, SLComponent com)
        {
            var ptr = SLPointer.Handle(handle);
            insert(ref ptr, com);
        }

        public void LinkTo(object key, SLComponent com)
        {
            var gch = GCHandle.Alloc(key, GCHandleType.Weak);
            var gcp = GCHandle.ToIntPtr(gch);
            var ptr = SLPointer.Reference(gcp);
            insert(ref ptr, com);
        }

        private void insert(ref SLPointer ptr, SLComponent com)
        {
            if (!_groups.TryGetValue(ptr, out SLWrapper wrapper))
            {
                wrapper = new SLWrapper(com);
                _groups.Add(ptr, wrapper);
            }
            else
            {
                wrapper.Add(com);
            }
        }

        public void UnlinkTo(ComponentType type)
        {
            var ptr = SLPointer.Type(type);
            _groups.Remove(ptr);
        }

        public void UnlinkTo(int handle)
        {
            var ptr = SLPointer.Handle(handle);
            _groups.Remove(ptr);
        }

        public void UnlinkTo(object key)
        {
            var ptr = SLPointer.Reference(key);
            _groups.Remove(ptr);
        }

        public unsafe T* Get<T>() where T : unmanaged
        {
            return (T*)Get(typeof(T));
        }

        public IntPtr Get(Type propType)
        {
            if (_cachedType == propType)
                return _cachedPtr;

            if (!_props.TryGetValue(propType, out IntPtr ptr))
            {
                ptr = Allocator.Calloc(propType);
                _props.Add(propType, ptr);
            }

            _cachedType = propType;
            return _cachedPtr = ptr;
        }

        public void Sweep()
        {
            var temp = stackalloc SLPointer[_groups.Values.Count];
            var ti = 0;
            var iter = _groups.GetEnumerator();
            while (iter.MoveNext())
            {
                var group = iter.Current.Value;
                if (0 == group.Sweep())
                    temp[ti++] = iter.Current.Key;
            }

            while (0 <= --ti)
            {
                _groups.Remove(temp[ti]);
            }
        }
        
        public void Sweep(ComponentType type)
        {
            var ptr = SLPointer.Type(type);
            if (_groups.TryGetValue(ptr, out SLWrapper group))
                return;

            if (0 == group.Sweep())
            {
                _groups.Remove(ptr);
            }
        }

        private SLComponent onTake(ComponentType type, int index)
        {
            this.Type = type;
            this._index = index;
            return this;
        }
        
        private SLComponent onRelease()
        {
            this.Type = ComponentType.None;
            this.Context = null;
            
            var pIter = _props.GetEnumerator();
            while (pIter.MoveNext())
            {
                Allocator.Cfree(pIter.Current.Key, pIter.Current.Value, false);
            }

            _groups.Clear();
            _props.Clear();

            return this;
        }
    }
}
