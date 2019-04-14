using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Core
{
    public struct Result
    {
        public string key;
        public int createdTick;
    }

    public sealed class ResultBuffer : SpeakingService
    {
        private List<Source> _srcList = new List<Source>();
        private int selectedCount = 0;

        public Source this[int index] => _srcList[index];
        public int Count => selectedCount;

        public void Add(string key)
        {
            _srcList.Add(new Source { key = key, createdTick = Environment.TickCount });
        }

        public void Commit()
        {
            selectedCount = _srcList.Count;
        }

        public void Flush()
        {
            _srcList.Clear();
            selectedCount = 0;
        }
    }
}
