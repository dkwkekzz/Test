using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Server
{
    class SharedContext
    {
        private static readonly Lazy<SharedContext> lazy = new Lazy<SharedContext>(() => new SharedContext());

        public static SharedContext Instance { get { return lazy.Value; } }
        public static bool IsCreated => lazy.IsValueCreated;


    }
}
