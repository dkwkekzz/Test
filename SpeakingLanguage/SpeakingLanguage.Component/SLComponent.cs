using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpeakingLanguage.Component
{
    public unsafe partial class SLComponent : IEquatable<SLComponent>, IComparable<SLComponent>, IEnumerable<SLComponent>, ISerializable
    {
        public static ComponentPool Factory { get; } = new ComponentPool(Config.MAX_COUNT_SLC);
        //public static SLComponent Root { get; } = _factory.GetObject().onTake(ComponentType.Root, _factory.GenerateIndex);
        //public static SLComponent FakeRoot { get; } = _factory.GetObject().onTake(ComponentType.Root, _factory.GenerateIndex);

        private int _index;
        private Dictionary<SLPointer, SLWrapper> _groups = new Dictionary<SLPointer, SLWrapper>();
        private Dictionary<Type, IntPtr> _props;
        private Type _cachedType;
        private IntPtr _cachedPtr;

        public ComponentType Type { get; private set; }
        public object Context { get; set; }

        #region INTERFACE
        public int CompareTo(SLComponent other) => other == null ? 1 : _index.CompareTo(other._index);
        public bool Equals(SLComponent other) => other == null ? false : _index == other._index;
        public override bool Equals(object obj) => obj == null ? false : Equals((SLComponent)obj);
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
                gIter.Current.Key.OnSerialized(ref writer);

                var wrapper = gIter.Current.Value;
                writer.WriteInt(wrapper.Count);
                var wIter = wrapper.GetEnumerator();
                while (wIter.MoveNext())
                {
                    writer.WriteInt(wIter.Current._index);
                }
            }

            if (null != _props)
            {
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
            else
            {
                writer.WriteInt(0);
            }
        }

        public void OnDeserialized(ref Library.Reader reader, SLComponent streamingComponent)
        {
            var read = true;
            read &= reader.ReadInt(out int groupCount);
            Library.Logger.Assert(read, "[OnDeserialized] fail to read component: linkedCount");

            for (int i = 0; i != groupCount; i++)
            {
                var ptr = new SLPointer();
                ptr.OnDeserialized(ref reader, null);

                read &= reader.ReadInt(out int childCount);
                Library.Logger.Assert(read, "[OnDeserialized] fail to read component: type|value|rootFakeIndex|childCount");

                var wrapper = new SLWrapper();
                for (int k = 0; k != childCount; k++)
                {
                    read &= reader.ReadInt(out int fakeIndex);
                    Library.Logger.Assert(read, "[OnDeserialized] fail to read component: fakeIndex");

                    var child = streamingComponent.Find(fakeIndex);
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
        
        public static bool operator ==(SLComponent lhs, SLComponent rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                    return true;

                return false;
            }

            return lhs.Equals(rhs);
        }
        public static bool operator !=(SLComponent lhs, SLComponent rhs) => !(lhs == rhs);

        public int GroupCount => _groups.Count;
        public int PropertyCount => _props?.Count ?? 0;
        public SLWrapper Find(ComponentType type) => _groups[SLPointer.Type(type)];
        public SLWrapper Find(int handle) => _groups[SLPointer.Handle(handle)];
        public SLWrapper Find(string key) => _groups[SLPointer.Text(key)];
        public SLWrapper Find(object key) => _groups[SLPointer.Reference(key)];
        public bool TryFind(ComponentType type, out SLWrapper wrap) => _groups.TryGetValue(SLPointer.Type(type), out wrap);
        public bool TryFind(int handle, out SLWrapper wrap) => _groups.TryGetValue(SLPointer.Handle(handle), out wrap);
        public bool TryFind(string key, out SLWrapper wrap) => _groups.TryGetValue(SLPointer.Text(key), out wrap);
        public bool TryFind(object key, out SLWrapper wrap) => _groups.TryGetValue(SLPointer.Reference(key), out wrap);
        public Dictionary<SLPointer, SLWrapper>.ValueCollection.Enumerator GetGroupEnumerator() => _groups.Values.GetEnumerator();

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

        public void LinkTo(string key, SLComponent com)
        {
            var ptr = SLPointer.Text(key);
            insert(ref ptr, com);
        }

        public void LinkTo(object key, SLComponent com)
        {
            var ptr = SLPointer.Reference(key);
            insert(ref ptr, com);
        }

        private void insert(ref SLPointer ptr, SLComponent com)
        {
            if (null == com)
                throw new ArgumentNullException($"[{this.ToString()}] this com can't link to null component.");

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
            remove(ref ptr);
        }

        public void UnlinkTo(int handle)
        {
            var ptr = SLPointer.Handle(handle);
            remove(ref ptr);
        }

        public void UnlinkTo(string key)
        {
            var ptr = SLPointer.Text(key);
            remove(ref ptr);
        }

        public void UnlinkTo(object key)
        {
            var ptr = SLPointer.Reference(key);
            remove(ref ptr);
        }

        private void remove(ref SLPointer ptr)
        {
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
            
            if (null == _cachedType)
            {
                _cachedPtr = Allocator.Calloc(propType);
            }
            else
            {
                if (null == _props)
                {
                    _props = new Dictionary<Type, IntPtr>();
                    _props.Add(_cachedType, _cachedPtr);
                }

                if (!_props.TryGetValue(propType, out _cachedPtr))
                {
                    _cachedPtr = Allocator.Calloc(propType);
                    _props.Add(propType, _cachedPtr);
                }
            }
            
            _cachedType = propType;
            return _cachedPtr;
        }

        public void Sweep()
        {
            var stack = new Stack<SLPointer>(_groups.Count);
            var iter = _groups.GetEnumerator();
            while (iter.MoveNext())
            {
                var group = iter.Current.Value;
                if (0 == group.Sweep())
                    stack.Push(iter.Current.Key);
            }

            while (stack.Count > 0)
            {
                _groups.Remove(stack.Pop());
            }
            //var temp = stackalloc SLPointer[_groups.Values.Count];
            //var ti = 0;
            //var iter = _groups.GetEnumerator();
            //while (iter.MoveNext())
            //{
            //    var group = iter.Current.Value;
            //    if (0 == group.Sweep())
            //        temp[ti++] = iter.Current.Key;
            //}

            //while (0 <= --ti)
            //{
            //    _groups.Remove(temp[ti]);
            //}
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

        public void Clear()
        {
            _groups.Clear();

            var pIter = _props.GetEnumerator();
            while (pIter.MoveNext())
            {
                Allocator.Cfree(pIter.Current.Key, pIter.Current.Value, false);
            }
            _props.Clear();
        }

        private SLComponent onTake(ComponentType type, int index)
        {
            Library.Logger.Assert(this.Type == ComponentType.None);

            this.Type = type;
            this._index = index;
            return this;
        }
        
        private SLComponent onRelease()
        {
            this.Type = ComponentType.None;
            this.Context = null;
            this.Clear();
            return this;
        }
    }
}
