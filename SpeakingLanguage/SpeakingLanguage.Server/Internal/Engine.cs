using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpeakingLanguage.Server
{
    class Engine
    {
        enum EState
        {
            None = 0,
            Running,
            Closing,
            Closed,
        }

        sealed class EngineState : IEquatable<EngineState>
        {
            public readonly int EndPoint;
            public readonly int Frequency;
            private EState _eState;

            public EngineState(int endPoint, int frequency)
            {
                EndPoint = endPoint;
                Frequency = frequency;
                _eState = EState.Closed;
            }

            public bool Is(EState s) => _eState == s;
            public void Set(EState s) => _eState = s;
            public bool Equals(EngineState other) => _eState == other._eState;
            public override bool Equals(object other) => _eState == ((EngineState)other)._eState;
            public override int GetHashCode() => (int)_eState;
            public override string ToString() => $"[EngineState] ep:{EndPoint.ToString()}/state:{_eState.ToString()}";
            public static bool operator ==(EngineState lhs, EngineState rhs) => lhs._eState == rhs._eState;
            public static bool operator !=(EngineState lhs, EngineState rhs) => lhs._eState != rhs._eState;
        }

        private readonly EngineState _state;

        public Engine(int endPoint, int frequency)
        {
            _state = new EngineState(endPoint, frequency);
        }
        
        public void Start(Component.SLComponent root, params Type[] templates)
        {
            if (!_state.Is(EState.Closed))
                throw new InvalidOperationException($"wrong state: {_state.ToString()}");
            
            Task.Factory.StartNew(() =>
            {
                var fq = _state.Frequency;
                var dispatcher = new Execution.Terminal.RawDispatcher(_state.EndPoint);
                var executorList = new List<Execution.ISLExecutor>(templates.Length);
                for (int i = 0; i != templates.Length; i++)
                {
                    var exe = (Execution.ISLExecutor)Activator.CreateInstance(templates[i]);
                    executorList.Add(exe);
                }
                
                _state.Set(EState.Running);
                while (_state.Is(EState.Running))
                {
                    var begin = Library.Ticker.MS;
                    try
                    {
                        for (int i = 0; i != executorList.Count; i++)
                        {
                            try
                            {
                                executorList[i].ExecuteFrame(ref dispatcher, root);
                            }
                            catch (ArgumentException e) { Library.Logger.Error($"{_state.ToString()}\n[Engine::{executorList[i].GetType().Name}]\n{e.Message}\n{e.StackTrace}"); }
                            catch (KeyNotFoundException e) { Library.Logger.Error($"{_state.ToString()}\n[Engine::{executorList[i].GetType().Name}]\n{e.Message}\n{e.StackTrace}"); }
                            catch (Exception) { Library.Logger.Error($"{_state.ToString()}\n[Engine::{executorList[i].GetType().Name}]\n"); throw; }
                        }
                    }
                    catch (Exception e) { Library.Logger.Exception(typeof(Engine), e); }
                    finally
                    {
                        var end = Library.Ticker.MS;
                        int leg = (int)(end - begin) - fq;
                        if (leg < 0)
                        {
                            Thread.Sleep(-leg);
                        }
                    }
                }

                _state.Set(EState.Closed);
            });
        }

        public void Stop()
        {
            _state.Set(EState.Closing);
        }
    }
}
