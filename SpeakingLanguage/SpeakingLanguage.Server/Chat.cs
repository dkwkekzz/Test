using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SpeakingLanguage.Server
{
    public class Chat : WebService
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            if (!e.IsText)
                return;

            var name = Context.QueryString["name"];
            Send(!name.IsNullOrEmpty() ? String.Format("\"{0}\" to {1}", e.Data, name) : e.Data);
            return;
        }

        protected override void OnError(ErrorEventArgs e)
        {
        }
    }
}
