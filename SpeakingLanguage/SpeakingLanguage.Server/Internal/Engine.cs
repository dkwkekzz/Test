using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SpeakingLanguage.Server
{
    sealed class Engine
    {
        sealed class EngineState : IEquatable<EngineState>
        {
            public const int CLOSING = 0;
            public const int RUNNING = 1;

            public int Value { get; set; }
            
            public bool Equals(EngineState other) => Value == other.Value;
            public override bool Equals(object other) => Value == ((EngineState)other).Value;
            public override int GetHashCode() => Value;
            public static bool operator ==(EngineState lhs, EngineState rhs) => lhs.Value == rhs.Value;
            public static bool operator !=(EngineState lhs, EngineState rhs) => lhs.Value != rhs.Value;
        }

        private readonly EngineState _state = new EngineState();

        public void Start(int endPoint, int frequency)
        {
            Task.Factory.StartNew(() =>
            {
                var sm = new FSM.StateMachine(endPoint);

                _state.Value = EngineState.RUNNING;
                while (_state.Value == EngineState.RUNNING)
                {
                    var begin = Library.Ticker.MS;
                    try
                    {
                        sm.ExecuteFrame();
                    }
                    catch (NotImplementedException e) { Library.Logger.Error($"[Server.Engine] {e.Message}/{e.StackTrace}"); }
                    catch (ArgumentException e) { Library.Logger.Error($"[Server.Engine] {e.Message}/{e.StackTrace}"); }
                    catch (Exception e) { Library.Logger.Error($"[Server.Engine] {e.Message}/{e.StackTrace}"); }
                    finally
                    {
                        var end = Library.Ticker.MS;
                        int leg = (int)(end - begin) - frequency;
                        if (leg < 0)
                        {
                            Thread.Sleep(-leg);
                        }
                    }
                }
            });
        }

        public void Stop()
        {
            _state.Value = EngineState.CLOSING;
        }
    }
}
