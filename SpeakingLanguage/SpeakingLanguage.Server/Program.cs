using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SpeakingLanguage.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            PacketDictionary.Install();

            var wssv = new WebSocketServer(4649);
#if DEBUG
            wssv.Log.Level = LogLevel.Trace;
#endif
            wssv.AddWebSocketService<Lobby>("/Lobby");
            wssv.AddWebSocketService<Chat>("/Chat");
            wssv.AddWebSocketService<Field>("/Field/1");
            wssv.AddWebSocketService<Field>("/Field/2");

            wssv.Start();
            if (wssv.IsListening)
            {
                Console.WriteLine("Listening on port {0}, and providing WebSocket services:", wssv.Port);
                foreach (var path in wssv.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);
            }

            var engine = new Engine();
            engine.Start(0, 60);

            Console.WriteLine("\nPress Enter key to stop the server...");
            Console.ReadLine();
            
            wssv.Stop();
            engine.Stop();
        }
    }
}
