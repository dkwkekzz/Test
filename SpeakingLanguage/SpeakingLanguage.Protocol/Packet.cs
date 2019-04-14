using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol
{
    public enum Packet
    {
        None = 1 << 16,
        Lobby_Create,
        Lobby_Login,

        Field_Sync,
        Field_Transition,
    }
}
