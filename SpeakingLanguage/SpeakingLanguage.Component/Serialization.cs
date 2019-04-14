using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Component
{
    public static class Serialization
    {
        public static void WriteComponent(ref Library.Writer writer, SLComponent com, Predicate<SLComponent> filter = null)
        {
            var comList = new Dictionary<int, SLComponent>();
            com.Traversal(comList);
            
            if (null != filter)
            {
                unsafe
                {
                    var tempList = stackalloc int[comList.Count];
                    var tempIdx = 0;
                    var iter = comList.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        if (filter(iter.Current.Value))
                            tempList[tempIdx++] = iter.Current.Key;
                    }

                    while (tempIdx > 0)
                    {
                        comList.Remove(tempList[--tempIdx]);
                    }
                }
            }

            writer.WriteInt(comList.Count);
            var pairIter = comList.GetEnumerator();
            while (pairIter.MoveNext())
            {
                writer.WriteInt((int)pairIter.Current.Value.Type);
                writer.WriteInt(pairIter.Current.Key);
            }

            var ctxList = new Dictionary<int, object>();
            pairIter = comList.GetEnumerator();
            while (pairIter.MoveNext())
            {
                var com2 = pairIter.Current.Value;
                com2.OnSerialized(ref writer);
                if (null != com2.Context && Attribute.IsDefined(com2.Context.GetType(), typeof(SerializableAttribute)))
                    ctxList.Add(pairIter.Current.Key, com2.Context);
            }

            writer.WriteInt(ctxList.Count);
            if (ctxList.Count > 0)
            {
                var stream = new MemoryStream();
                Library.SerializeHelper.SerializeToMemory(ctxList, stream);
                writer.WriteStream(stream);
            }
        }
        
        //private static void collectContext(SLComponent com, Dictionary<int, object> ctxDic)
        //{
        //    if (null != com.Context && Attribute.IsDefined(com.Context.GetType(), typeof(SerializableAttribute)))
        //        ctxDic.Add(com._index, com.Context);

        //    var iter = com.GetGroupPairEnumerator();
        //    while (iter.MoveNext())
        //    {
        //        var list = iter.Current.Value;
        //        var listIter = list.GetEnumerator();
        //        {
        //            var inner = listIter.Current;
        //            collectContext(inner, ctxDic);
        //        }
        //    }
        //}

        public static SLComponent ReadComponent(ref Library.Reader reader)
        {
            var read = reader.ReadInt(out int comCount);
            if (!read)
                return null;

            var comList = new List<SLComponent>(comCount);
            for (int i = 0; i != comCount; i++)
            {
                read &= reader.ReadInt(out int comType);
                read &= reader.ReadInt(out int fakeIndex);
                if (!read)
                    return null;

                var com = SLComponent.Create((ComponentType)comType);
                SLComponent.FakeRoot.LinkTo(fakeIndex, com);

                comList.Add(com);
            }
            
            for (int i = 0; i != comList.Count; i++)
            {
                comList[i].OnDeserialized(ref reader);
            }

            read &= reader.ReadInt(out int ctxCount);
            if (!read)
                return null;

            if (ctxCount > 0)
            {
                var stream = reader.GetStream();
                var ctxList = Library.SerializeHelper.DeserializeFromMemory(stream) as Dictionary<int, object>;
                
                var ctxIter = ctxList.GetEnumerator();
                while (ctxIter.MoveNext())
                {
                    var com3 = SLComponent.FakeRoot.Find(ctxIter.Current.Key).First();
                    com3.Attach(ctxIter.Current.Value);
                }
            }

            return comList[0];
        }
    }
}
