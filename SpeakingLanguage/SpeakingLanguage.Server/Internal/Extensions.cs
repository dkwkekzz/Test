using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace SpeakingLanguage.Server
{
    static class Extensions
    {
        public static void Flush(this Library.Writer writer, WebSocket socket)
        {
            var stream = writer.GetStream();
            socket.Send(stream, (int)stream.Length);
        }

        public static void FlushAsync(this Library.Writer writer, WebSocket socket)
        {
            var stream = writer.GetStream();
            socket.SendAsync(stream, (int)stream.Length, null);
        }
    }
}
