using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Component.Property
{
    [Serializable]
    [PropertyAttribute(typeof(Transmission))]
    public struct Transmission : IProperty
    {
        public int userHandle;
        public long lastEventTick;
    }
}
