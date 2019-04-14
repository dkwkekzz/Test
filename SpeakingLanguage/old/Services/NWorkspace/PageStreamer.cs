using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpeakingLanguage.Library;

namespace SpeakingLanguage.Command.NWorkspace
{
    // 메타파일만들어주기 -> offset, size얻기 위해서

    class PageStreamer
    {
        private ConstantStream<Page> _stream;

        public PageStreamer(Book book)
        {
            _stream = new ConstantStream<Page>("command.bin", new PageDeserializer(book));
            _stream.Start();
        }

        public Page Open(string owner)
        {
            return _stream.Capture(0L, 1024).Flush();
        }
    }
}
