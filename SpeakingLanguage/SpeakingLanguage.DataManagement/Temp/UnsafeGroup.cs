using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.DataManagement
{
    public class UnsafeGroup
    {
        private Dictionary<int, int> _entityLookup;
        private Dictionary<Type, int> _typeOffsetDic;
        private IntPtr _headPtr;

        public UnsafeGroup(int size)
        {
            _entityLookup = new Dictionary<int, int>(size);
            _typeOffsetDic = new Dictionary<Type, int>();
            _headPtr = Marshal.AllocHGlobal(Config.SIZE_SEGMENT * size);
        }
        
        public void AddType(params Type[] types)
        {
            int offset = 0;
            for (int i = 0; i != types.Length; i++)
            {
                _typeOffsetDic.Add(types[i], offset);
                offset += Marshal.SizeOf(types[i]);
                if (offset >= Config.SIZE_SEGMENT)
                    throw new OverflowException("Offset of type can not exceed maximum size");
            }
        }

        public void AddStructure<T>(int actorHandle, T comp)
        {
            int index;
            if (!_entityLookup.TryGetValue(actorHandle, out index))
            {
                index = _entityLookup.Count;
                _entityLookup.Add(actorHandle, index);
            }
            
            Marshal.StructureToPtr(comp, _headPtr + Config.SIZE_SEGMENT * index + _typeOffsetDic[typeof(T)], false);
        }

        public void GetStructure<T>(int actorHandle, out T str)
        {
            int index;
            if (!_entityLookup.TryGetValue(actorHandle, out index))
            {
                index = _entityLookup.Count;
                _entityLookup.Add(actorHandle, index);
            }

            str = Marshal.PtrToStructure<T>(_headPtr + Config.SIZE_SEGMENT * index + _typeOffsetDic[typeof(T)]);
        }

        public unsafe void AddUnsafeStructure<T>(int actorHandle, T comp)
        {
            int index;
            if (!_entityLookup.TryGetValue(actorHandle, out index))
            {
                index = _entityLookup.Count;
                _entityLookup.Add(actorHandle, index);
            }

            Unsafe.Write<T>((void*)(_headPtr + Config.SIZE_SEGMENT * index + _typeOffsetDic[typeof(T)]), comp);
        }

        public unsafe void GetUnsafeStructure<T>(int actorHandle, out T str)
        {
            int index;
            if (!_entityLookup.TryGetValue(actorHandle, out index))
            {
                index = _entityLookup.Count;
                _entityLookup.Add(actorHandle, index);
            }

            str = Unsafe.Read<T>((void*)(_headPtr + Config.SIZE_SEGMENT * index + _typeOffsetDic[typeof(T)]));
        }
    }
}
