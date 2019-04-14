using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

namespace SpeakingLanguage.DataManagement
{
    public sealed class XmlReader2
    {
        public void Parse<TDataTable>(TDataTable table, string file)
        {
            XmlReader reader = XmlReader.Create(file);
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            XElement xe = XElement.Load(reader);
            parseElement(table, xe);
        }

        private void parseElement(object table, XElement xe)
        {
            var name = xe.Name.ToString();
            var field = table.GetType().GetField(name);
            var eType = field.GetType();
            var e = field.GetValue(table);

            foreach (var child in xe.Elements())
            {
                //parseElement(child);
            }

            var attrs = xe.Attributes();
            foreach (var attr in attrs)
            {
                var attrField = table.GetType().GetField(attr.Name.ToString());
                //_typeDic.Add(new TypeKey { name = name, attribute = attr.Name.ToString() }, type);
            }
        }
    }
}
