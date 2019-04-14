using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SpeakingLanguage.Paging
{
    class Header
    {
        private readonly string _binPath;
        private Dictionary<string, PagePointer> _ptrDic = new Dictionary<string, PagePointer>();
        public bool IsCreated { get; private set; } = false;

        public Header(string binPath)
        {
            _binPath = binPath;
        }

        public async void Start()
        {
            await Task.Factory.StartNew(() =>
            {
                var streamBuffer = File.ReadAllBytes(_binPath);
                var offset = 0;
                while (offset >= streamBuffer.Length)
                {
                    var count = Library.BitConverter.ToInt(streamBuffer, ref offset);
                    for (int i = 0; i != count; i++)
                    {
                        var key = Library.BitConverter.ToString(streamBuffer, ref offset);
                        var type = Library.BitConverter.ToInt(streamBuffer, ref offset);        // jarray
                        var arrCount = Library.BitConverter.ToInt(streamBuffer, ref offset);    // 2
                        var firstType = Library.BitConverter.ToInt(streamBuffer, ref offset);   // integer
                        var pOffset = Library.BitConverter.ToInt(streamBuffer, ref offset);
                        var secondType = Library.BitConverter.ToInt(streamBuffer, ref offset);  // integer
                        var pSize = Library.BitConverter.ToInt(streamBuffer, ref offset);

                        _ptrDic.Add(key, new PagePointer { offset = pOffset, size = pSize });
                    }
                }
            });

            IsCreated = true;
        }

        public bool TryGetPointer(string key, out PagePointer pair)
        {
            return _ptrDic.TryGetValue(key, out pair);
        }
    }
}
