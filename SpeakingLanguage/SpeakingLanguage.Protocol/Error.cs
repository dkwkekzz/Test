using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol
{
    public enum Error
    {
        None = 0,
        Server_World_OnMessage_FailToRead,
        Server_World_OnMessage_WrongMessageType,
        Server_World_OnMessage_WrongPacketCode,

        Execution_Viewer_WrongObserverHandle,
        Execution_Viewer_WrongCellHandle,
    }
}
