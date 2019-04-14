using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Server
{
    /// <summary>
    /// [입력]
    /// packetCode          : int
    /// id                  : string
    /// password            : string
    /// </summary>
    sealed class Login : IPacket
    {
        public Type Type => typeof(Lobby);
        public Protocol.Packet Code => Protocol.Packet.Lobby_Create;

        public Result Execute(ref Library.Reader reader, ref LocalContext context)
        {
            var userTableCom = context.target.Find("userTable").First();

            bool read = true;
            read &= reader.ReadString(out string idStr);
            read &= reader.ReadString(out string pwdStr);
            if (!read)
                return new Result { error = Protocol.Error.Lobby_OnMessage_Create_FailToRead };

            if (!userTableCom.TryFind(idStr, out Component.SLWrapper userWrap))
                return new Result { error = Protocol.Error.Lobby_OnMessage_NotExistId };

            var userInfo = userWrap.First().Context as DataManagement.Table.Identifier;
            if (userInfo.pwd != pwdStr)
                return new Result { error = Protocol.Error.Lobby_OnMessage_WrongPassward };
            
            if (!context.target.TryFind(context.socketContext.SecWebSocketKey, out Component.SLWrapper sesWrap))
                return new Result { error = Protocol.Error.Lobby_OnMessage_DuplicateLogin };

            var sesCom = sesWrap.First();
            var obCom = Component.SLComponent.Create(Component.ComponentType.Observer, userInfo.handle);
            Component.Function.BidirectLink(obCom, sesCom);

            var tick = Library.Ticker.UniversalMS;
            unsafe
            {
                var trans = obCom.Get<Component.Property.Transmission>();
                trans->userHandle = userInfo.handle;
                trans->lastEventTick = tick;
            }
           
            userInfo.lastConnectionTime = DateTime.Now;

            var writer = new Library.Writer(1 << 6);
            writer.WriteSuccess();
            writer.WriteInt(userInfo.handle);
            writer.WriteLong(tick);
            writer.Flush(context.socketContext.WebSocket);

            return new Result();
        }
    }
}
