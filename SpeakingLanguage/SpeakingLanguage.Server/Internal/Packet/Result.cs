using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Server
{
    struct Result
    {
        public Protocol.Error error;
        public bool IsSuccess => error == SpeakingLanguage.Protocol.Error.None;
    }
}
