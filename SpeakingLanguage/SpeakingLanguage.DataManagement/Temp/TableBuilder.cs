using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.DataManagement
{
    /// <summary>
    /// 명명 규칙이 필요하다...
    /// 파일명: {타입명}.xml ---> 클래스명: {타입명}
    /// </summary>
    public sealed class TableBuilder
    {
        private Dictionary<string, Type> _typeDic = new Dictionary<string, Type>();

        public void Collect()
        {
            var types = AssemblyHelper.CollectType(t => t.IsClass && t.HasInterface("IDataTable"));
            foreach (var type in types)
            {
                var name = type.Name;
                _typeDic.Add(name, type);
            }
        }
        
        public async void BuildXml(string xml)
        {
            var name = xml.Replace(".xml", "");
            var type = _typeDic[name];
            var table = FileConverter.ConvertXmlToObject(type, xml);
            await BinaryConverter.ConvertObjectToBinaryAsync(table, $"{name}.bin");
        }
    }
}
