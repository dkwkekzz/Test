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
    sealed class Create : IPacket
    {
        public Type Type => typeof(Lobby);
        public Protocol.Packet Code => Protocol.Packet.Lobby_Create;

        public Result Execute(ref Library.Reader reader, ref LocalContext context)
        {
            var userTableCom = context.target.Find("userTable").First();

            var read = true;
            read &= reader.ReadString(out string idStr);
            read &= reader.ReadString(out string pwdStr);
            if (!read)
                return new Result { error = Protocol.Error.Lobby_OnMessage_Create_FailToRead };

            if (userTableCom.TryFind(idStr, out Component.SLWrapper wrap))
                return new Result { error = Protocol.Error.Lobby_OnMessage_AlreadyExistId };

            var userInfo = new DataManagement.Table.Identifier();
            userInfo.handle = userTableCom.GroupCount;
            userInfo.id = idStr;
            userInfo.pwd = pwdStr;
            userInfo.lastConnectionTime = DateTime.Now;

            var userInfoCom = Component.SLComponent.Create(Component.ComponentType.DataRow, userInfo.handle);
            userInfoCom.Attach(userInfo);
            userTableCom.LinkTo(idStr, userInfoCom);

            var writer = new Library.Writer(1 << 6);
            writer.WriteSuccess();
            writer.Flush(context.socketContext.WebSocket);

            return new Result();
        }
    }
}
