using System;
using System.Collections.Generic;
using WebSocketSharp;

namespace SpeakingLanguage.Server
{
    static class Function
    {
        public static void CastFailure(Protocol.Error code, WebSocket socket)
        {
            var writer = new Library.Writer(1 << 4);
            writer.WriteFailure((int)code);
            writer.Flush(socket);
            socket.Close();
        }

        public static int UriParse(string uri)
        {
            var i = uri.LastIndexOf('/');
            var indexStr = uri.Substring(i + 1);
            if (!int.TryParse(indexStr, out int index))
                throw new UriFormatException("Please enter the last / next index");
            return index;
        }
    }
}
