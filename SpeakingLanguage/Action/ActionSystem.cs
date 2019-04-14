using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Action
{
    public class ActionSystem
    {
        private Compiler _compiler = new Compiler();

        private ObjectPool<StringBuilder> _builderPool
            = new ObjectPool<StringBuilder>(() => { return new StringBuilder(Config.MAX_KEY_LENGTH); }, Config.DEFAULT_WORKER_COUNT,
                builder => { builder.Clear(); });

        public void OnAwake()
        {
        }

        public void OnUpdate(IEnumerable<ActionSource> input, IInjectable<UnityComponentSource> output)
        {
            Parallel.ForEach(input, src =>
            {
                var builder = _builderPool.GetObject();

                string cmd;
                string arg;
                for (int i = src.startOffset; i != src.count; i++)
                {
                    if (src.context[i] == ' ')
                    {
                    }
                    else
                    {
                        builder.Append(src.context[i]);
                    }
                }

                output.Push();

                _builderPool.PutObject(builder);
            });
        }

        public void OnInject(IInjectable<UnityComponentSource> output)
        {
        }
    }
}
