using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SpeakingLanguage.Server
{
    static class Extensions
    {
        public static Engine RunNewService<TService>(this WebSocketServer wssv, Component.SLComponent root, string path, params Type[] templates)
            where TService : WebSocketBehavior, new()
        {
            wssv.AddWebSocketService<TService>(path);

            var awaiter = Component.SLComponent.Factory.LoadAsync(root, path).GetAwaiter();
            var serviceCom = awaiter.GetResult();
            if (serviceCom == null)
            {
                serviceCom = Function._defaultConstructForTest(root);
            }

            var ep = Function.ParseEndPoint(path);
            root.LinkTo(ep, serviceCom);

            var engine = new Engine(ep, 60);
            engine.Start(serviceCom, templates);
            return engine;
        }

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
