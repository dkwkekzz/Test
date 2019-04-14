using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SpeakingLanguage.DataManagement
{
    public class Manager
    {
        private static readonly Lazy<Manager> lazy = new Lazy<Manager>(() => new Manager());

        public static Manager Instance { get { return lazy.Value; } }
        public static bool IsCreated => lazy.IsValueCreated;

        //public TableDictionary Table { get; } = new TableDictionary();
        //public GroupDictionary Property { get; } = new GroupDictionary();
    }
}
