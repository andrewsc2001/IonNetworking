using System.Collections;

namespace IonClient.Networking
{
    public static class PacketTable
    {
        private static readonly Hashtable _packetList = new Hashtable();
        public delegate void PacketAction(byte[] data);

        public static void AddPacket(byte header, PacketAction action)
        {
            if (_packetList.ContainsKey(header))
                return;
            
            _packetList.Add(header, action);
        }

        public static void RemovePacket(byte header, PacketAction action)
        {
            _packetList.Remove(header);
        }

        public static PacketAction GetAction(byte header)
        {
            return (PacketAction)_packetList[header];
        }
    }
}