using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Component.Property
{
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = true) ]
    class PropertyAttribute : Attribute
    {
        private readonly static Dictionary<int, Type> _indexDic = new Dictionary<int, Type>();

        public static Type GetProperty(int index)
        {
            return _indexDic[index];
        }

        public int Index { get; private set; }
        public int Size { get; private set; }

        public PropertyAttribute(Type t)
        {
            this.Index = _indexDic.Count;
            this.Size = Marshal.SizeOf(t);
            _indexDic.Add(this.Index, t);
        }
    }
}
