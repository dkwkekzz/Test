using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Server
{
    static class Validator
    {
        public static bool IsValidateTick(long eventTick, long curTick, long lastEventTick)
        {
            if (eventTick > lastEventTick)
                return false;
            if (eventTick < curTick)
                return false;

            return true;
        }
    }
}
