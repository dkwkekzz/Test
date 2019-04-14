using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Core
{
    public sealed class SpeakingDriver : IDisposable
    {
        public bool IsRun { get; private set; }
        
        public static void Run()
        {
            var driver = Singleton<SpeakingDriver>.Instance;
            if (driver.IsRun)
                throw new InvalidOperationException("Only one driver should be running.");

            try
            {
                Utils.FindAssemblyAndLoad("SpeakingLanguage", ".dll");

                var services = Utils.Collect<SpeakingService>();
                services.ForEach(service => { service.OnAwake(); });
                services.ForEach(service => { Locator.Provide(service); });

                Loop.Notify();
                for (int i = 0; i != Config.COUNT_DEFAULT_WORKER; i++)
                    Loop.Update(i);

                driver.IsRun = true;
            }
            catch (ArgumentException e) { driver.Dispose(); }
            catch (Exception e) { driver.Dispose(); }
        }
        
        public static void Stop()
        {
            var loop = Singleton<SpeakingDriver>.Instance;
            loop.Dispose();
        }

        /// <summary>
        /// 모든 기능을 중지하고 재개할 수 있어야 한다.
        /// 예외처리 복구에 대한 기능
        /// </summary>
        public static void Resume()
        {

        }

        public void Dispose()
        {
            IsRun = false;
        }
    }
}
