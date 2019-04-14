using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Net.WebSockets;

namespace SpeakingLanguage.Server.Packet
{
    /// <summary>
    /// [입력]
    /// packetCode          : int
    /// userHandle          : int
    /// eventTick           : long
    /// loop
    ///    fire             : int
    ///    executedTick     : long
    /// end
    /// </summary>
    sealed class Transition : IPacket
    {
        public Type Type => typeof(Field);
        Protocol.Packet IPacket.Code => Protocol.Packet.Field_Transition;

        public Result Execute(ref Library.Reader reader, ref LocalContext context)
        {
            var obCom = context.agent;

            var read = reader.ReadInt(out int actorHandle);
            if (!read)
                return new Result { error = Protocol.Error.Field_OnMessage_Sync_FailToRead };
            
            if (!Component.SLComponent.Root.TryFind(actorHandle, out Component.SLWrapper actorWrap))
                return new Result { error = Protocol.Error.Field_OnMessage_Transition_NotExistActor };

            var writer = new Library.Writer(1 << 6);
            writer.WriteSuccess();
            unsafe
            {
                var transProp = obCom.Get<Component.Property.Transmission>();
                var lastEventTick = transProp->lastEventTick;
                var currentTick = Library.Ticker.MS;

                var transition = new FSM.Transition.Writer(0);
                while (!reader.EOB)
                {
                    read &= reader.ReadInt(out int fire);
                    read &= reader.ReadLong(out long executedTick);
                    if (!read)
                        return new Result { error = Protocol.Error.Field_OnMessage_Sync_FailToRead };

                    if (!Validator.IsValidateTick(executedTick, currentTick, lastEventTick))
                        return new Result { error = Protocol.Error.Field_OnMessage_FailToTickValidate };

                    lastEventTick = Math.Max(lastEventTick, executedTick);

                    writer.WriteInt(fire);
                    writer.WriteLong(executedTick);

                    transition.PushBack(actorHandle, fire, executedTick);
                }
                
                transProp->lastEventTick = lastEventTick;
            }

            var actorCom = actorWrap.First();
            var cellCom = actorCom.Find(Component.ComponentType.Cell).First();

            var obList = cellCom.Find(Component.ComponentType.Observer);
            var iter = obList.GetEnumerator();
            while (iter.MoveNext())
            {
                var sesCom = iter.Current.Find(Component.ComponentType.Session).First();
                Library.Logger.Assert(null != sesCom);

                var sesContext = sesCom.Context as WebSocketContext;
                writer.Flush(sesContext.WebSocket);
            }

            return new Result();
        }
    }
}
