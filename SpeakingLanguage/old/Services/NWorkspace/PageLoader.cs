using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Command.NWorkspace
{
    class PageLoader
    {
        private WorkerPool _pool;
        private PageStreamer _pageStreamer;

        public PageLoader(WorkerPool pool, Book book)
        {
            _pool = pool;
            _pageStreamer = new PageStreamer(book);
        }

        public async void Load(string owner)
        {
            var page = await _work(owner);
            _pool.OnCompletedWork(owner);
        }

        private Task<Page> _work(string owner)
        {
            return Task<Page>.Factory.StartNew(() =>
            {
                return _pageStreamer.Open(owner);
            });
        }
    }
}
