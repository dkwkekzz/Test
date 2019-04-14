using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Core
{
    public interface IStateComponent : IDisposable
    {
        void Awake();
    }
}
