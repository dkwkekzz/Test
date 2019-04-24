using System;

namespace SpeakingLanguage.Component.Property
{
    [PropertyAttribute(typeof(State))]
    public unsafe struct State : IProperty
    {
        public long transposedTick;
        public long lastTick;
    }
}
