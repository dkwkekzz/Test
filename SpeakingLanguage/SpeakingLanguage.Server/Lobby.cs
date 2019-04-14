using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SpeakingLanguage.Server
{
    public class Lobby : WebSocketBehavior
    {
        private static Component.SLComponent _lobbyCom;

        public Lobby() : base()
        {
            if (null == _lobbyCom)
            {
                var awaiter = Component.SLComponent.LoadAsync("userTable").GetAwaiter();
                var tableCom = awaiter.GetResult();
                if (null == tableCom)
                    tableCom = Component.SLComponent.Create(Component.ComponentType.DataTable);

                _lobbyCom = Component.SLComponent.Create(Component.ComponentType.Behaviour, "lobby");
                _lobbyCom.LinkTo("userTable", tableCom);
            }
        }

        protected override void OnOpen()
        {
            var sesCom = Component.SLComponent.Create(Component.ComponentType.Session);
            sesCom.Attach(Context.WebSocket);
            _lobbyCom.LinkTo(Context.SecWebSocketKey, sesCom);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (!e.IsBinary)
            {
                Function.CastFailure(SpeakingLanguage.Protocol.Error.Lobby_OnMessage_WrongMessageType, Context.WebSocket);
                return;
            }
            
            var reader = new Library.Reader(e.RawData);
            if (!reader.ReadInt(out int code))
            {
                Function.CastFailure(SpeakingLanguage.Protocol.Error.Lobby_OnMessage_FailToRead, Context.WebSocket);
                return;
            }
            
            var packet = PacketDictionary.Find(GetType(), code);
            if (null == packet)
            {
                Function.CastFailure(SpeakingLanguage.Protocol.Error.Lobby_OnMessage_WrongPacketCode, Context.WebSocket);
                Context.WebSocket.Close();
                return;
            }

            var local = new LocalContext { socketContext = Context, target = _lobbyCom };
            var res = packet.Execute(ref reader, ref local);
            if (!res.IsSuccess)
            {
                Function.CastFailure(res.error, Context.WebSocket);
                return;
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Component.SLComponent.Destroy(Context.SecWebSocketKey);
        }
        
        protected override void OnError(ErrorEventArgs e)
        {
        }
    }
}
