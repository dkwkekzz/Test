using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.FSM
{
    public sealed class ValueSet
    {
        public Dictionary<string, int> Integer { get; } = new Dictionary<string, int>(new StringRefComparer());
        public Dictionary<string, float> Float { get; } = new Dictionary<string, float>(new StringRefComparer());
        public Dictionary<string, string> String { get; } = new Dictionary<string, string>(new StringRefComparer());
    }
}
