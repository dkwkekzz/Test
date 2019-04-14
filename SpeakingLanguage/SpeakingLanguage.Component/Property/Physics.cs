using System;
using System.Numerics;

namespace SpeakingLanguage.Component.Property
{
    [Serializable]
    [PropertyAttribute(typeof(Physics))]
    public struct Physics : IProperty
    {
        public float x;
        public float y;
        public float z;
    }
}
