using System;
using System.Collections.Generic;
using System.IO;
using WebSocketSharp.Net.WebSockets;

namespace SpeakingLanguage.Server
{
    interface IPacket
    {
        Type Type { get; }
        Protocol.Packet Code { get; }
        Result Execute(ref Library.Reader reader, ref LocalContext context);
    }
}
