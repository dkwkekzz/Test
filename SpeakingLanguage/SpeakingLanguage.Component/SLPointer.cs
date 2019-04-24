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
        Text,
        Reference,
    }

    public struct SLPointer : IEquatable<SLPointer>, IComparable<SLPointer>, ISerializable
    {
        public PointerType type;
        public int value;
        public object refValue;

        public static SLPointer Type(ComponentType t) => new SLPointer { type = PointerType.Type, value = (int)t };
        public static SLPointer Index(int i) => new SLPointer { type = PointerType.Index, value = i };
        public static SLPointer Handle(int v) => new SLPointer { type = PointerType.Handle, value = v };
        public static SLPointer Text(string key) => new SLPointer { type = PointerType.Text, value = key.GetHashCode(), refValue = string.Intern(key) };
        public static SLPointer Reference(object key)
        {
            var gch = GCHandle.Alloc(key, GCHandleType.Weak);
            var gcp = GCHandle.ToIntPtr(gch);
            return SLPointer.Reference(gcp);
        }
        public static SLPointer Reference(IntPtr ptr) => new SLPointer { type = PointerType.Reference, value = ptr.ToInt32() };

        public static bool operator ==(SLPointer lhs, SLPointer rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                    return true;

                return false;
            }

            return lhs.Equals(rhs);
        }
        public static bool operator !=(SLPointer lhs, SLPointer rhs) => !(lhs == rhs);

        public static implicit operator long(SLPointer h) => h.value;
        public static implicit operator SLPointer(long v) => v;
        
        public bool Equals(SLPointer other)
        {
            if (refValue != null || other.refValue != null)
                return type == other.type && object.ReferenceEquals(refValue, other.refValue);

            return value == other.value && type == other.type;
        }
        public int CompareTo(SLPointer other) => value.CompareTo(value);
        public override bool Equals(object obj) => Equals((SLPointer)obj);
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();

        public bool IsEmpty() => type == PointerType.None;
        public void Release() => type = PointerType.None;

        public void OnSerialized(ref Library.Writer writer)
        {
            writer.WriteInt((int)type);
            if (type == PointerType.Text)
            {
                var ptr = new IntPtr(value);
                var gcp = GCHandle.FromIntPtr(ptr);
                var text = gcp.Target as string;
                writer.WriteString(text);
            }
            else if (type == PointerType.Reference)
            {
                throw new NotSupportedException("reference type pointer can't serialized.");
            }
        }

        public void OnDeserialized(ref Library.Reader reader, SLComponent streamingComponent)
        {
            var read = reader.ReadInt(out int t);
            type = (PointerType)t;
            if (type == PointerType.Text)
            {
                read &= reader.ReadString(out string text);
                this = SLPointer.Text(text);
            }
        }
    }
}
