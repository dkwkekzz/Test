using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpeakingLanguage.Library;
using SpeakingLanguage.Linkage;
using System.Linq;

namespace SpeakingLanguage.Command
{
    class Reader : ISystem
    {
        public void Initialize(IProvider linkage, IProvider command)
        {
        }

        public void Update(IProvider linkage, IProvider command)
        {
            var edgeMap = linkage.Get<EdgeMap>();
            var pool = command.Get<WorkerPool>();
            foreach (var owner in edgeMap.GetLinkedDest("root"))
            {
                pool.AddWorker(owner);
            }
        }
    }
}
