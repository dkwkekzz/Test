using SpeakingLanguage.Core;
using SpeakingLanguage.Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeakingLanguage.Physics
{
    public sealed class Physics2DSystem : SpeakingSystem
    {
        private SourceBuffer<EventSource> _events;
        private SourceBuffer<PhysicsSource> _physics;
        private Ticker _timer;
        private ValueMap<int> _intMap;

        public override int Sequence => 4;

        public override void OnAwake(ILoopState state)
        {
            _events = state.Get<SourceBuffer<EventSource>>();
            _physics = state.Get<SourceBuffer<PhysicsSource>>();
            _timer = state.Get<Ticker>();
            _intMap = state.Get<ValueMap<int>>();
        }

        public override FrameResult OnUpdateAsParallel(int srcIndex, int updaterId, ILoopState state)
        {
            var e = _events[srcIndex].key;
            var dt = _timer.Recently * 0.001f;
            var iRoot = _intMap.Root[e];

            var messRef = iRoot[Constants.MESS].ValueRef;
            var posXRef = iRoot[Constants.POSITION][Constants.X].ValueRef;
            var posYRef = iRoot[Constants.POSITION][Constants.X].ValueRef;
            var speedXRef = iRoot[Constants.SPEED][Constants.X].ValueRef;
            var speedYRef = iRoot[Constants.SPEED][Constants.X].ValueRef;
            var accelXRef = iRoot[Constants.ACCEL][Constants.Y].ValueRef;
            var accelYRef = iRoot[Constants.ACCEL][Constants.Y].ValueRef;
            speedXRef.val += accelXRef.val;
            speedYRef.val += accelYRef.val;
            posXRef.val += speedXRef.val;
            posYRef.val += speedYRef.val;

            if (speedXRef.val > 0)
            {
                speedXRef.val -= messRef.val;
                if (speedXRef.val < 0)
                    speedXRef.val = 0;
            }
            else if (speedXRef.val < 0)
            {
                speedXRef.val += messRef.val;
                if (speedXRef.val > 0)
                    speedXRef.val = 0;
            }

            if (speedYRef.val > 0)
            {
                speedYRef.val -= messRef.val;
                if (speedYRef.val < 0)
                    speedYRef.val = 0;
            }
            else if (speedYRef.val < 0)
            {
                speedYRef.val += messRef.val;
                if (speedYRef.val > 0)
                    speedYRef.val = 0;
            }
            
            _physics[srcIndex] = new PhysicsSource { posx = (int)(posXRef.val * dt), posy = (int)(posYRef.val * dt) };

            return FrameResult.Success;
        }
    }
}
