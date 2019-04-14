using System;

namespace SpeakingLanguage.Core
{
    public enum TokenFlag
    {
        None = 0,
        Read = 1 << 1,
        Write = 1 << 2,
        And = 1 << 3,       // current - next relationship
        Or = 1 << 4,
        Enter = 1 << 5,
        Comma = 1 << 6,
        Add = 1 << 7,
        Remove = 1 << 8,
    }
}
