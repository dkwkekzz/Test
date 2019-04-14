using SpeakingLanguage.Command.Event;
using SpeakingLanguage.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpeakingLanguage.Command
{
    public class EventCollector
    {
        private readonly Dictionary<string, EventEntity> _eventDic = new Dictionary<string, EventEntity>();

        public void Open()
        {
            var asm = Assembly.Load(Config.NAME_ASSEMBLY_EVENT);
            var eventTypes = asm.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(EventEntity)));
            foreach (var type in eventTypes)
            {
                var entity = (EventEntity)Activator.CreateInstance(type);
                _eventDic.Add(type.Name, entity);
            }
        }

        public bool TryExecute(string key, out Token token)
        {
            EventEntity ee;
            if (!_eventDic.TryGetValue(key, out ee))
            {
                token = null;
                return false;
            }

            token = ee.Execute(key);
            return true;
        }
    }
}
