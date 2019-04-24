using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SpeakingLanguage.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            TextWriterTraceListener tr1 = new TextWriterTraceListener(System.Console.Out);
            Trace.Listeners.Add(tr1);

            TextWriterTraceListener tr2 = new TextWriterTraceListener(System.IO.File.CreateText("Output.txt"));
            Trace.Listeners.Add(tr2);

            var wssv = new WebSocketServer(4649);
#if DEBUG
            wssv.Log.Level = LogLevel.Trace;
#endif
            Engine engine0 = null;
            Engine engine1 = null;
            Engine engine2 = null;
            try
            {
                var root = Component.SLComponent.Factory.Create(null, Component.ComponentType.Root);

                engine0 = wssv.RunNewService<Chat>(root, "/Chat/0",
                    typeof(Connection));
                engine1 = wssv.RunNewService<World>(root, "/Lobby/1",
                    typeof(Connection), typeof(Authentication));
                engine2 = wssv.RunNewService<World>(root, "/Field/2",
                    typeof(Connection), typeof(Authentication), typeof(SceneSelector), typeof(SpreadTransition),
                    typeof(Execution.Transposer), typeof(Execution.StateMachine), typeof(Execution.Interaction),
                    typeof(Streamer), typeof(Execution.Sweeper));

                wssv.Start();

                if (wssv.IsListening)
                {
                    Console.WriteLine("Listening on port {0}, and providing WebSocket services:", wssv.Port);
                    foreach (var path in wssv.WebSocketServices.Paths)
                        Console.WriteLine("- {0}", path);
                }

                Console.WriteLine("\nPress Enter key to stop the server...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("\n======critical error======\n");
                Library.Logger.Exception(typeof(Program), e);
                Console.ReadLine();
            }
            finally
            {
                engine0?.Stop();
                engine1?.Stop();
                engine2?.Stop();
                wssv.Stop();
            }
        }
    }
}
