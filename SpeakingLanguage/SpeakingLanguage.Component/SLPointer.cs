using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Component
{
    public enum PointerType
    {
        None = 0,
        Type,
        Index,
        Handle,
        Reference,
    }

    public struct SLPointer : IEquatable<SLPointer>, IComparable<SLPointer>
    {
        public PointerType type;
        public int value;

        public static SLPointer Type(ComponentType t) => new SLPointer { type = PointerType.Type, value = (int)t };
        public static SLPointer Index(int i) => new SLPointer { type = PointerType.Index, value = i };
        public static SLPointer Handle(int v) => new SLPointer { type = PointerType.Handle, value = v };
        public static SLPointer Reference(IntPtr ptr) => new SLPointer { type = PointerType.Reference, value = ptr.ToInt32() };
        public static SLPointer Reference(object key)
        {
            var gch = GCHandle.Alloc(key, GCHandleType.Weak);
            var gcp = GCHandle.ToIntPtr(gch);
            return SLPointer.Reference(gcp);
        }

        public static implicit operator long(SLPointer h) => h.value;
        public static implicit operator SLPointer(long v) => v;

        public bool Empty => type == PointerType.None;

        public void Release() => type = PointerType.None;
        public bool Equals(SLPointer other) => value == other.value && type == other.type;
        public int CompareTo(SLPointer other) => value.CompareTo(value);
        public override bool Equals(object obj) => value == ((SLPointer)obj).value && type == ((SLPointer)obj).type;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }
}
