using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeakingLanguage.Library;

namespace SpeakingLanguage.Command
{
    class Workspace : ISystem
    {
        private PageLoader _loader;
        private PageReader _reader;

        public void Initialize(IProvider linkage, IProvider command)
        {
            _loader = new PageLoader(command.Get<WorkerPool>(), command.Get<Book>());
            _reader = new PageReader();
        }

        public void Update(IProvider linkage, IProvider command)
        {
            var pool = command.Get<WorkerPool>();
            Parallel.ForEach(pool, (pair) =>
            {
                _work(ref pair);
            });
        }

        private void _work(ref KeyValuePair<string, Worker> pair)
        {
            var owner = pair.Key;
            var worker = pair.Value;
            switch (worker.state)
            {
                case 0: // init
                    _loader.Load(owner);
                    break;
                case 1: // load complete
                    _reader.Read(owner);
                    break;
            }
        }
    }
}
