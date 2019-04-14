using SpeakingLanguage.Core;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Command.Event
{
    public sealed class Observer : EventEntity
    {
        public override Token Execute(string me)
        {
            return  IF(me, "left").AND(me, "fire_a").CREATE(me, "skill_a").PERIOD()
                +   IF(me, "health", 100, 100).AND(me, "fire_b").ACT(me, "skill_b").PERIOD()
                +   IF(me, "right").AND(me, "fire_c").ACT(me, "skill_c").PERIOD();
        }
    }
}
