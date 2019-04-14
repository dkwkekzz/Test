//using System;
//using System.Collections.Generic;
//using WebSocketSharp;
//using WebSocketSharp.Net.WebSockets;

//namespace SpeakingLanguage.Server
//{
//    class Connection
//    {
//        private static int _idGenerator = 0;
//        private readonly int _userSessionId = _idGenerator++;
//        private WebSocket _socket;
//        private Component.SLComponent _observer;

//        public int USID => _userSessionId;
//        public WebSocket Socket => _socket;
//        public Component.SLComponent Observer => _observer;
        
//        public Connection(WebSocketContext context)
//        {
//            _socket = context.WebSocket;
//        }

//        public void Observe(Component.SLComponent ob)
//        {
//            _observer = ob;
//        }
//    }
//}
