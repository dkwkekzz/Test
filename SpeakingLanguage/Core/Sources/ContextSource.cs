using System;

namespace SpeakingLanguage.Core
{
    public struct ContextSource
    {
        public string key;
        public byte[] body;
        public int offset;
        public int count;
    }
}
