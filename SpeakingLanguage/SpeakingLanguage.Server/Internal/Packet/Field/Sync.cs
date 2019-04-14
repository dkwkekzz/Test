using System;
using System.Collections.Generic;
using WebSocketSharp.Net.WebSockets;

namespace SpeakingLanguage.Server.Packet
{
    /// <summary>
    /// [입력]
    /// packetCode          : int
    /// observerHandle      : int
    /// loop
    ///     cellHandle      : int
    /// </summary>
    sealed class Sync : IPacket
    {
        public Type Type => typeof(Field);
        public Protocol.Packet Code => Protocol.Packet.Field_Sync;

        public Result Execute(ref Library.Reader reader, ref LocalContext context)
        {
            var fieldCom = context.target;
            var obCom = context.agent;
            obCom.UnlinkTo(Component.ComponentType.Cell);

            var writer = new Library.Writer(1 << 16);
            for (int i = 0; i != 9; i++)
            {
                if (!reader.ReadInt(out int cellHandle))
                    return new Result { error = Protocol.Error.Field_OnMessage_Sync_FailToRead };

                if (!fieldCom.TryFind(cellHandle, out Component.SLWrapper cellWrap))
                    return new Result { error = Protocol.Error.Field_OnMessage_Sync_NotExistCell };

                var cellCom = cellWrap.First();
                Component.Function.BidirectLink(obCom, cellCom);
                Component.Serialization.WriteComponent(ref writer, cellCom);
            }

            writer.Flush(context.socketContext.WebSocket);

            return new Result();
        }
    }
}
