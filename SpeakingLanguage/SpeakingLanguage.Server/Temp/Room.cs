//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using WebSocketSharp.Net.WebSockets;

//namespace SpeakingLanguage.Server
//{
//    class Room
//    {
//        private Dictionary<string, Connection> _connections = new Dictionary<string, Connection>();

//        public Connection GetConnection(WebSocketContext context)
//        {
//            var key = context.SecWebSocketKey;
//            return _connections.ContainsKey(key) ? _connections[key] : null;
//        }

//        public void Enter(WebSocketContext context)
//        {
//            try
//            {
//                _connections.Add(context.SecWebSocketKey, new Connection(context));
//            }
//            catch (ArgumentException e)
//            {
//                Library.Logger.Exception(GetType(), e);
//            }
//        }

//        public void Leave(WebSocketContext context)
//        {
//            _connections.Remove(context.SecWebSocketKey);
//        }
//    }
//}
