using System;
using System.Collections.Generic;
using WebSocketSharp.Net.WebSockets;

namespace SpeakingLanguage.Server
{
    struct LocalContext
    {
        public WebSocketContext socketContext;
        public Component.SLComponent target;
        public Component.SLComponent agent;
    }
}
