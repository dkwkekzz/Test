using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SpeakingLanguage.DataManagement
{
    static class FileConverter
    {
        public static object ConvertXmlToObject(Type type, string xml)
        {
            object outObj;
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(type);
            using (var fs = new FileStream(xml, FileMode.Open, FileAccess.Read))
            {
                outObj = serializer.Deserialize(fs);
            }

            return outObj;
        }
    }
}
