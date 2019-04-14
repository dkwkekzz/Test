using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpeakingLanguage.Paging
{
    class PageCache
    {  
        private Header _header = new Header(Config.PAGE_SOURCE_FILE);
        private CleanManager<string> _cleaner = new CleanManager<string>();
        private SimpleSpinLock _lock = new SimpleSpinLock();

        private IDictionary<string, Context> _cachedCtx = new Dictionary<string, Context>(Config.MAX_SOURCE_COUNT);
        private IDictionary<int, Page> _pageDic = new Dictionary<int, Page>(Config.MAX_PAGE);
        private IDictionary<int, int> _pageReferenceDic = new Dictionary<int, int>(Config.MAX_PAGE);
        
        public void Awake()
        {
            _header.Start();
            _cleaner.Link(_removeCtx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetContext(string key, out Context ctx)
        {
            _cleaner.UpdatePriority(key);
            return _cachedCtx.TryGetValue(key, out ctx);
        }

        public void AddContext(string key, out PagePointer ptr)
        {
            if (!_header.TryGetPointer(key, out ptr))
                throw new KeyNotFoundException($"not found key in header: {key}");

            _cachedCtx.Add(key, new Context
            {
                pageIndex = ptr.index,
                key = key,
                offset = ptr.offset,
                count = ptr.size,
            });
            _cleaner.RegistPriority(key);

            _increaseRef(ptr.index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddPage(Page page)
        {
            _lock.Enter();
            _pageDic.Add(page.index, page);
            _lock.Leave();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetPage(int pageIndex, out Page page)
        {
            _lock.Enter();
            bool ret = _pageDic.TryGetValue(pageIndex, out page);
            _lock.Leave();
            return ret;
        }

        private void _removeCtx(string key)
        {
            Context ctx;
            if (!_cachedCtx.TryGetValue(key, out ctx))
                return;

            _cachedCtx.Remove(key);

            _decreaseRef(ctx.pageIndex);
        }

        private void _increaseRef(int pageIndex)
        {
            if (!_pageReferenceDic.ContainsKey(pageIndex))
            {
                _pageReferenceDic.Add(pageIndex, 0);
            }

            _pageReferenceDic[pageIndex]++;
        }

        private void _decreaseRef(int pageIndex)
        {
            if (_pageReferenceDic.ContainsKey(pageIndex))
                _pageReferenceDic[pageIndex]--;

            if (_pageReferenceDic[pageIndex] == 0)
            {
                var page = _pageDic[pageIndex];
                var pool = Locator.Get<BufferPool>();
                pool.PutBuffer(page.body);

                _pageDic.Remove(pageIndex);
                _pageReferenceDic.Remove(pageIndex);
            }
        }
    }
}
