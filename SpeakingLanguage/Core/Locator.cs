using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Core
{
    public static class Locator
    {
        private static Dictionary<Type, SpeakingService> _services = new Dictionary<Type, SpeakingService>();

        private static Logger _logger = null;
        public static Logger Logger => _logger ?? (_logger = _services[typeof(Logger)] as Logger);

        private static Ticker _ticker = null;
        public static Ticker Ticker => _ticker ?? (_ticker = _services[typeof(Ticker)] as Ticker);
        
        public static TService Add<TService>() where TService : SpeakingService, new()
        {
            var serv = new TService();
            _services.Add(typeof(TService), serv);
            return serv;
        }

        public static void Provide(SpeakingService serv)
        {
            _services.Add(serv.GetType(), serv);
        }

        public static TService Get<TService>() where TService : SpeakingService, new()
        {
            SpeakingService serv;
            if (!_services.TryGetValue(typeof(TService), out serv)) return null;
            return serv as TService;
        }

        public static void Clear()
        {
            foreach (var pair in _services)
                pair.Value.OnDestroy();
            _services.Clear();
        }
    }
}
