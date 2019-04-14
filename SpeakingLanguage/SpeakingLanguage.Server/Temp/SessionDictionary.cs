//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;

//namespace SpeakingLanguage.Server
//{
//    class SessionDictionary
//    {
//        private readonly static ConcurrentDictionary<string, Component.SLComponent> _key2sessionLookup 
//            = new ConcurrentDictionary<string, Component.SLComponent>();
        
//        public static void Install()
//        {
//        }

//        public static bool ContainsKey(string secWebSocketKey) => _key2sessionLookup.ContainsKey(secWebSocketKey);
//        public static bool TryGetValue(string secWebSocketKey, out Component.SLComponent sesCom)
//            => _key2sessionLookup.TryGetValue(secWebSocketKey, out sesCom);
//        public static bool TryAdd(string secWebSocketKey, Component.SLComponent sesCom)
//            => _key2sessionLookup.TryAdd(secWebSocketKey, sesCom);
//        public static bool TryRemove(string secWebSocketKey, out Component.SLComponent sesCom)
//            => _key2sessionLookup.TryRemove(secWebSocketKey, out sesCom);
//    }
//}
