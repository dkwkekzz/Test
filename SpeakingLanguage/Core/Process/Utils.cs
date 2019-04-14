using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SpeakingLanguage.Core
{
    static class Utils
    {
        public static List<T> Collect<T>()
        {
            var sysList = new List<T>();
            var sysTypes = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(t => t.IsClass && t.IsSubclassOf(typeof(T)));
            foreach (var type in sysTypes)
            {
                var sysInst = (T)Activator.CreateInstance(type);
                if (sysList.Contains(sysInst))
                    throw new ArgumentException($"dulicate system type: {type.FullName}");

                sysList.Add(sysInst);
            }

            return sysList;
        }

        public static void FindAssemblyAndLoad(string prefix, string suffix)
        {
            var directory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var files = System.IO.Directory.GetFiles(directory);
            foreach (var file in files)
            {
                if (file.Contains(prefix) && file.Contains(suffix))
                {
                    Assembly.LoadFrom(file);
                }
            }
        }

    }
}
