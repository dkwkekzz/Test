using System;

namespace SpeakingLanguage.Component.Property
{
    [Serializable]
    [PropertyAttribute(typeof(State))]
    public unsafe struct State : IProperty
    {
        [NonSerialized]
        public State* prev;
        public int refHandle;
        public int actorHandle;
        public long transposedTick;
        public long lastTick;
    }
}
