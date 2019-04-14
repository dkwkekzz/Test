using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SpeakingLanguage.DataManager
{
    public class BigDictionary : GroupDictionary
    {
        private static readonly Lazy<BigDictionary> lazy = new Lazy<BigDictionary>(() => new BigDictionary());

        public static BigDictionary Instance { get { return lazy.Value; } }
        public static bool IsCreated => lazy.IsValueCreated;
    }
}
