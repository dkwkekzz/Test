using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Component
{
    public struct Handle : IEquatable<Handle>, IComparable<Handle>
    {
        private const int TYPE_MASK = (1 << 16) - 1;
        public static Handle Generate(int type, int index) => new Handle { value = index << 16 | type };

        public int value;

        public bool Equals(Handle other) => value == other.value;
        public int CompareTo(Handle other) => value.CompareTo(value);
        public override bool Equals(object obj) => value == ((Handle)obj).value;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();

        public static implicit operator int(Handle h) => h.value;
        public static implicit operator Handle(int v) => v;

        public int GetComponentIndex => value >> 16;
        public ComponentType GetComponentType => (ComponentType)(TYPE_MASK & value);
    }
}
