using System;

namespace SpeakingLanguage.Core
{
    public abstract class SpeakingSystem
    {
        public abstract int Sequence { get; }

        public virtual void OnAwake() { }
        public virtual void OnUpdate() { }
        public virtual FrameResult OnUpdateAsParallel(int srcIndex) { return FrameResult.None; }
        public virtual void OnDestroy() { }
    }
}
