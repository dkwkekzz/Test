using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Action
{
    class Compiler
    {
        internal struct Result
        {
            public string type;
            public int value;
        }

        private ObjectPool<CompileStack> _stackPool
            = new ObjectPool<CompileStack>(() => { return new CompileStack(); }, Config.DEFAULT_WORKER_COUNT,
                stack => { stack.Clear(); });

        public void CompileParallel(IEnumerable<ActionSource> input, IInjectable<UnityComponentSource> output)
        {
            int count = -1;
            Parallel.ForEach(input, src =>
            {
                var builder = new StringBuilder(Config.MAX_KEY_LENGTH);
                
                for (int i = src.startOffset; i != src.count; i++)
                {
                    if (src.context[i] == ' ')
                    {
                        output.Push()
                    }
                    else
                    {
                        builder.Append(src.context[i]);
                    }
                }
            });
        }
    }
}
