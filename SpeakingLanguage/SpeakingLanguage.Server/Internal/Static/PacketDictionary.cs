using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    class PacketDictionary
    {
        class PacketCollection
        {
            private readonly Dictionary<Protocol.Packet, IPacket> _packets = new Dictionary<Protocol.Packet, IPacket>();
            public void Insert(IPacket packet) => _packets.Add(packet.Code, packet);
            public IPacket Find(Protocol.Packet code) => _packets.ContainsKey(code) ? _packets[code] : null;
        }

        private readonly static Dictionary<Type, PacketCollection> _collections = new Dictionary<Type, PacketCollection>();

        private static void collectPacket()
        {
            var packets = Library.AssemblyHelper.CollectType(t => t.IsClass && Library.TypeHelper.HasInterface(t, "IPacket"));
            foreach (var type in packets)
            {
                var inst = (IPacket)Activator.CreateInstance(type);
                if (!_collections.TryGetValue(inst.Type, out PacketCollection col))
                {
                    col = new PacketCollection();
                    _collections.Add(type, col);
                }

                col.Insert(inst);
            }
        }

        public static void Install()
        {
            collectPacket();
        }

        public static IPacket Find(Type behaviourType, Protocol.Packet code)
        {
            if (!_collections.TryGetValue(behaviourType, out PacketCollection col))
                return null;
            return col.Find(code);
        }

        public static IPacket Find(Type behaviourType, int code)
        {
            if (!_collections.TryGetValue(behaviourType, out PacketCollection col))
                return null;
            return col.Find((Protocol.Packet)code);
        }
    }
}
