using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace SpeakingLanguage.DataManagement
{
    static class BinaryConverter
    {
        public static async Task ConvertObjectToBinaryAsync(object obj, string bin)
        {
            using (var fs = new FileStream(bin, FileMode.Create, FileAccess.Write))
            {
                var ms = new MemoryStream();
                SerializeToMemory(obj, ms);
                ms.Position = 0;
                await ms.CopyToAsync(fs);
            }
        }

        public static void ConvertObjectToBinary(object obj, string bin)
        {
            using (var fs = new FileStream(bin, FileMode.Create, FileAccess.Write))
            {
                var ms = new MemoryStream();
                SerializeToMemory(obj, ms);
                ms.Position = 0;
                ms.CopyTo(fs);
            }
        }

        public static async Task<T> ConvertBinaryToObjectAsync<T>(string bin) where T : class
        {
            T outObj;
            using (var fs = new FileStream(bin, FileMode.Open, FileAccess.Read))
            {
                var ms = new MemoryStream();
                fs.Position = 0;
                await fs.CopyToAsync(ms);
                ms.Position = 0;
                outObj = DeserializeFromMemory(ms) as T;
            }

            return outObj;
        }

        public static T ConvertBinaryToObject<T>(string bin) where T : class
        {
            T outObj;
            using (var fs = new FileStream(bin, FileMode.Open, FileAccess.Read))
            {
                var ms = new MemoryStream();
                fs.Position = 0;
                fs.CopyTo(ms);
                ms.Position = 0;
                outObj = DeserializeFromMemory(ms) as T;
            }

            return outObj;
        }

        public static void SerializeToMemory(object obj, Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
        }

        public static object DeserializeFromMemory(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }
    }
}
