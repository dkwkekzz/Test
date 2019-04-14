using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Net.WebSockets;
using WebSocketSharp.Server;

namespace SpeakingLanguage.Server
{
    public class Field : WebSocketBehavior
    {
        private static Component.SLComponent _fieldCom;

        public Field() : base()
        {
            if (null == _fieldCom)
            {
                var fieldIndex = Function.UriParse(Context.RequestUri.AbsolutePath);
                _fieldCom = Component.SLComponent.Create(Component.ComponentType.Behaviour, $"field_{fieldIndex.ToString()}");
            }
        }

        protected override void OnOpen()
        {
            var sesCom = Component.SLComponent.Create(Component.ComponentType.Session);
            sesCom.Attach(Context.WebSocket);
            _fieldCom.LinkTo(Context.SecWebSocketKey, sesCom);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (!e.IsBinary)
            {
                Function.CastFailure(SpeakingLanguage.Protocol.Error.Field_OnMessage_WrongMessageType, Context.WebSocket);
                return;
            }
            
            var reader = new Library.Reader(e.RawData);
            var read = true;
            read &= reader.ReadInt(out int code);
            read &= reader.ReadInt(out int userHandle);
            if (!read)
            {
                Function.CastFailure(SpeakingLanguage.Protocol.Error.Field_OnMessage_FailToRead, Context.WebSocket);
                return;
            }
            
            var packet = PacketDictionary.Find(GetType(), code);
            if (null == packet)
            {
                Function.CastFailure(SpeakingLanguage.Protocol.Error.Field_OnMessage_WrongPacketCode, Context.WebSocket);
                return;
            }
            
            if (!Component.SLComponent.Root.TryFind(userHandle, out Component.SLWrapper wrapper))
            {
                Function.CastFailure(SpeakingLanguage.Protocol.Error.Field_OnMessage_NotExistObserverComponent, Context.WebSocket);
                return;
            }

            var obCom = wrapper.First();
            var key = Context.SecWebSocketKey;
            if (!obCom.TryFind(Component.ComponentType.Session, out Component.SLWrapper sesWrap))
            {
                var sesCom = _fieldCom.Find(key).First();
                Component.Function.BidirectLink(sesCom, obCom);

                // 이 시점에 옵저버에 대한 엑터가 생성되어야 한다.
                var awaiter = Component.SLComponent.LoadAsync($"user_{userHandle.ToString()}").GetAwaiter();
                var actorCom = awaiter.GetResult();
                if (null == actorCom)
                    actorCom = Component.SLComponent.Create(Component.ComponentType.Actor);

                Component.Function.BidirectLink(_fieldCom, actorCom);
                Component.Function.BidirectLink(obCom, actorCom);
            }

            var local = new LocalContext { socketContext = Context, target = _fieldCom, agent = obCom };
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
