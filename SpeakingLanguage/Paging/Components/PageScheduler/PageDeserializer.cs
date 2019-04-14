using System;
using System.Collections.Generic;
using SpeakingLanguage.Library;

namespace SpeakingLanguage.Paging
{
    class PageDeserializer : IDeserializer<Page>
    {
        public void Deserialize(byte[] streamBuffer, int count, out Page ret)
        {
            ret = new Page
            {
                body = streamBuffer,
            };
        }
    }
}
