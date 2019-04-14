using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SpeakingLanguage.DataManagement
{
    enum EType
    {
        None,
        Integer,
        Float,
        Boolean,
        String,
    }

    struct TypeKey : IEquatable<TypeKey>
    {
        public string name;
        public string attribute;
        public bool Equals(TypeKey other) => other.name == name && attribute == other.attribute;
    }

    public sealed class TypeTable
    {
        private Dictionary<TypeKey, EType> _typeDic = new Dictionary<TypeKey, EType>();

        public void Parse(string file)
        {
            XmlReader reader = XmlReader.Create(file);
            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            XElement xe = XElement.Load(reader);
            parseElement(xe);
        }

        private void parseElement(XElement xe)
        {
            foreach (var child in xe.Elements())
            {
                parseElement(child);
            }

            var name = xe.Name.ToString();
            var attrs = xe.Attributes();
            foreach (var attr in attrs)
            {
                try
                {
                    var a3 = (string)attr;
                    var a1 = (int)attr;
                    var a2 = (float)attr;
                }
                catch { }

                var type = EType.None;
                switch (attr.Value[0])
                {
                    case 'i':
                        type = EType.Integer;
                        break;
                    case 'f':
                        type = EType.Float;
                        break;
                    case 'b':
                        type = EType.Boolean;
                        break;
                    case 's':
                        type = EType.String;
                        break;
                }
                Trace.Assert(type != EType.None);

                //_typeDic.Add(new TypeKey { name = name, attribute = attr.Name.ToString() }, type);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var node in _typeDic)
            {
                sb.AppendLine($"[name] {node.Key.name} [attribute] {node.Key.attribute} [type] {node.Value.ToString()}");
            }

            return sb.ToString();
        }
    }
}
