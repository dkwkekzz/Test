//using System;
//using System.Collections.Generic;
//using WebSocketSharp;
//using WebSocketSharp.Net.WebSockets;
//using WebSocketSharp.Server;

//namespace SpeakingLanguage.Server
//{
//    public class VirtualWorld : WebSocketBehavior
//    {
//        private readonly Dictionary<string, int> _key2sessionLookup = new Dictionary<string, int>();
//        private readonly Component.SLComponent _worldCom;

//        public VirtualWorld()
//        {
//        }
        
//        protected override void OnOpen()
//        {
//            var key = Context.SecWebSocketKey;
//            if (_key2sessionLookup.ContainsKey(key))
//                return;

//            var sesCom = Component.SLComponent.Create(Component.ComponentType.LobbySession);
//            var sesHandle = sesCom.Handle;
//            _key2sessionLookup.Add(key, sesHandle);
//        }
        
//        protected override void OnClose(CloseEventArgs e)
//        {
//            var key = Context.SecWebSocketKey;
//            if (!_key2sessionLookup.TryGetValue(key, out int secHandle))
//                return;

//            var sesCom = Component.SLComponent.Root.Search(secHandle);
//            Component.SLComponent.Destroy(sesCom);

//            _key2sessionLookup.Remove(key);
//        }

//        protected override void OnError(ErrorEventArgs e)
//        {
//        }

//        protected Component.SLComponent SearchSession()
//        {
//            var sesCom = Component.SLComponent.Root.Search(_key2sessionLookup[Context.SecWebSocketKey]);
//            Library.Logger.Assert(sesCom != null);
//            return sesCom;
//        }
//    }
//}
