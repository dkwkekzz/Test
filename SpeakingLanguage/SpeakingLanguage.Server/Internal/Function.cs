using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Net.WebSockets;

namespace SpeakingLanguage.Server
{
    static class Function
    {
        public static Component.SLComponent _defaultConstructForTest(Component.SLComponent root)
        {
            var com = Component.SLComponent.Factory.Create(root, Component.ComponentType.Service);
            unsafe
            {
                var serviceProp = com.Get<Component.Property.Service>();
                serviceProp->maxDestroyCount = 9999;
                serviceProp->destroyCount = 0;
            }

            var pkCom1 = Component.SLComponent.Factory.Create(com, Component.ComponentType.Package, Component.PackageKey.Observer);
            var pkCom2 = Component.SLComponent.Factory.Create(com, Component.ComponentType.Package, Component.PackageKey.Cell);
            return com;
        }

        public static void CastFailure(Protocol.Error code, WebSocket socket)
        {
            var writer = new Library.Writer(1 << 4);
            writer.WriteFailure((int)code);
            writer.Flush(socket);
            socket.Close();
        }

        public static int ParseEndPoint(string uri)
        {
            var i = uri.LastIndexOf('/');
            var indexStr = uri.Substring(i + 1);
            if (!int.TryParse(indexStr, out int index))
                throw new UriFormatException("Please enter the last / next index");
            return index;
        }
    }
}
