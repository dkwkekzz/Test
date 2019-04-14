using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Component
{
    interface ISerializable
    {
        void OnSerialized(ref Library.Writer writer);
        void OnDeserialized(ref Library.Reader reader);
    }
}
