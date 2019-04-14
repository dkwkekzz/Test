using SpeakingLanguage.Core;
using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpeakingLanguage.Compile
{
    enum CompileState
    {
        None,
        Body,
        Src,
        Dest,
        Value,
        Weight,
        Finish,
    }

    class SpeakingCompiler
    {
        private ObjectPool<Interpreter> _interpreterPool = new ObjectPool<Interpreter>(
            () => { return new Interpreter(); }, Config.COUNT_DEFAULT_WORKER, ipt => { ipt.Clear(); });
        
        public void Execute(ContextSource src, out Token ret)
        {
            var ipt = _interpreterPool.GetObject();
            try
            {
                var current = CompileState.Body;
                for (int i = 0; i != src.count; i++)
                {
                    ipt.Execute(src.body[i + src.offset], ref current);
                    if (current == CompileState.Finish)
                    {
                        ipt.Commit();
                        current = CompileState.Body;
                    }
                }
            }
            catch (NotImplementedException e) { }
            catch (InvalidOperationException e) { }
            catch (Exception e) { }
            finally
            {
                ret = ipt.Result;
                _interpreterPool.PutObject(ipt);
            }
        }
    }
}
