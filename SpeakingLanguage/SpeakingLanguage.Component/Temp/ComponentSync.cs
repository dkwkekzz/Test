using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Component
{
    public enum ComponentSync
    {
        None = 0,
        Managed = 1 << 1,
        Changed = 1 << 2,
    }
}
