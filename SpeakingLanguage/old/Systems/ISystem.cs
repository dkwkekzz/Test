using SpeakingLanguage.Library;

namespace SpeakingLanguage.Command
{
    public interface ISystem
    {
        void Initialize(IProvider linkage, IProvider command);
        void Update(IProvider linkage, IProvider command);
    }
}
