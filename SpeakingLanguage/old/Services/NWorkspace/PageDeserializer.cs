using System;
using System.Collections.Generic;
using SpeakingLanguage.Library;
using SpeakingLanguage.Linkage;

namespace SpeakingLanguage.Command.NWorkspace
{
    // 주어진 바이트를 읽어 페이지를 읽는 과정. 
    // 읽고난 page결과파일을 pagereader에게 전달한다.
    class PageDeserializer : IDeserializer<Page>
    {
        private Book _book;

        public PageDeserializer(Book book)
        {
            _book = book;
        }

        public void Deserialize(byte[] streamBuffer, out Page ret)
        {
            ret = new Page { };

            int offset = 0;
            ToPage(streamBuffer, ref offset, ref ret);
        }

        private void ToPage(byte[] streamBuffer, ref int offset, ref Page page)
        {
            Library.BitConverter.PassInt(streamBuffer, ref offset);             // count
            Library.BitConverter.PassString(streamBuffer, ref offset);          // key
            Library.BitConverter.PassInt(streamBuffer, ref offset);             // type

            page.owner = Library.BitConverter.ToString(streamBuffer, ref offset);   // token -> string

            Library.BitConverter.PassString(streamBuffer, ref offset);          // key
            Library.BitConverter.PassInt(streamBuffer, ref offset);             // type

            var count = Library.BitConverter.ToInt(streamBuffer, ref offset);   // count
            page.begin = _book.Allocate(count);
            page.end = page.begin + count;
            for (int i = page.begin; i != page.end; i++)
            {
                _toLine(streamBuffer, ref offset, out _book.lines[i]);
            }
        }

        private void _toLine(byte[] streamBuffer, ref int offset, out Line line)
        {   // byte를 string이 아닌 곧바로 line객체로 만들어 

        }

    }
}
