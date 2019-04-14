using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpeakingLanguage.FSM
{
    public sealed class StateRunner
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
                var sm = new StateMachine(endPoint);

                _state.Value = EngineState.RUNNING;
                while (_state.Value == EngineState.RUNNING)
                {
                    var begin = Library.Ticker.MS;
                    try
                    {
                        sm.ExecuteFrame();
                    }
                    catch (NotImplementedException e) { Library.Logger.Error(nameof(StateMachine), $"{e.Message}/{e.StackTrace}"); }
                    catch (ArgumentException e) { Library.Logger.Error(nameof(StateMachine), $"{e.Message}/{e.StackTrace}"); }
                    catch (Exception e) { Library.Logger.Error(nameof(StateMachine), $"{e.Message}/{e.StackTrace}"); }
                    finally
                    {
                        var end = Library.Ticker.MS;
                        int leg = frequency - (int)(end - begin);
                        if (leg > 0)
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
