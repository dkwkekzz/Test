using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpeakingLanguage.Library
{
    public static class AssemblyHelper
    {
        public static List<T> Collect<T>()
        {
            var collected = new List<T>();
            var types = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(asm => asm.GetTypes())
                       .Where(t => t.IsClass && t.IsSubclassOf(typeof(T)));
            foreach (var type in types)
            {
                var inst = (T)Activator.CreateInstance(type);
                if (collected.Contains(inst))
                    throw new ArgumentException($"dulicate type: {type.FullName}");

                collected.Add(inst);
            }

            return collected;
        }

        public static IEnumerable<Type> CollectType(Predicate<Type> predicator)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(asm => GetTypesSafely(asm))
                       .Where(t => predicator(t));
            return types;
        }

        public static IEnumerable<Type> GetTypesSafely(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null);
            }
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
