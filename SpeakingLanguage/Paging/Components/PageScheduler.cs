using SpeakingLanguage.Core;
using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeakingLanguage.Paging
{
    /// <summary>
    /// 페이지를 관리한다.
    /// 1. 한번 로드했던 페이지는 캐싱한 다음, 일정 시간동안 사용되지 않으면 자동으로 언로드한다.
    /// 2. 로드할때, 하나씩 로드하는 것이 아닌, 여러개를 취합한다음 가까운 것들은 모아서 한번에 로드하여 공통 컨텍스트를 제공한다.
    /// </summary>
    class PageScheduler
    {
        private Queue<string> _waitQueue = new Queue<string>();
        private Queue<int> _capturing = new Queue<int>();
        private BufferPool _pool = new BufferPool();
        private ConstantStream<Page> _stream = new ConstantStream<Page>(Config.NAME_FILE_PAGE_SOURCE, new PageDeserializer());

        public PageScheduler()
        {
            _stream = new ConstantStream<Page>(Config.NAME_FILE_PAGE_SOURCE, new PageDeserializer(), 
                capacity => { return _pool.GetBuffer(capacity); });
        }

        public void Awake()
        {
            _stream.Start();
        }
        
        public void Schedule(string key)
        {
            _waitQueue.Enqueue(key);
        }
        
        public void ExecuteFrame(PageCache cache)
        {
            if (0 == _waitQueue.Count)
                return;

            try
            {
                while (_waitQueue.Count != 0)
                {
                    var key = _waitQueue.Dequeue();

                    PagePointer ptr;
                    cache.AddContext(key, out ptr);

                    Page page;
                    if (!cache.TryGetPage(ptr.index, out page))
                    {
                        _capturing.Enqueue(ptr.index);
                    }
                }

                while (_capturing.Count != 0)
                {
                    var pageIndex = _capturing.Dequeue();
                    Task.Factory.StartNew(() =>
                    {
                        var page = _stream.Capture(pageIndex * Config.LENGTH_PAGE, (pageIndex + 1) * Config.LENGTH_PAGE).Flush();
                        page.index = pageIndex;
                        cache.AddPage(page);
                    });
                }
            }
            catch (KeyNotFoundException e) { }
            catch (Exception e){ }
            finally
            {
                _waitQueue.Clear();
                _capturing.Clear();
            }
        }
    }
}
