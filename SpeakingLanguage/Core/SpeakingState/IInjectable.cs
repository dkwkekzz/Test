using System;

namespace SpeakingLanguage.Core
{
    public interface IInjectable<TSource>
    {
        TSource this[int index] { get; set; }
        TSource this[long index] { get; set; }
        void Push(TSource src);
    }
}
