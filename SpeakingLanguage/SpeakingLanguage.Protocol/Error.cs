using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol
{
    public enum Error
    {
        None = 0,
        Lobby_OnMessage_FailToRead,
        Lobby_OnMessage_DuplicateLogin,
        Lobby_OnMessage_WrongMessageType,
        Lobby_OnMessage_AlreadyExistId,
        Lobby_OnMessage_NotExistId,
        Lobby_OnMessage_WrongPassward,
        Lobby_OnMessage_WrongPacketCode,
        Lobby_OnMessage_Create_FailToRead,
        Lobby_OnMessage_Login_FailToRead,

        Field_OnMessage_FailToRead,
        Field_OnMessage_WrongMessageType,
        Field_OnMessage_NotExistObserverComponent,
        Field_OnMessage_FailToTickValidate,
        Field_OnMessage_WrongPacketCode,

        Field_OnMessage_Transition_FailToRead,
        Field_OnMessage_Transition_NotExistActor,
        Field_OnMessage_Sync_FailToRead,
        Field_OnMessage_Sync_NotExistCell,
    }
}
