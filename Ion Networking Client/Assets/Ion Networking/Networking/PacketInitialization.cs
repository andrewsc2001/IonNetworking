using UnityEngine;

namespace IonClient.Networking
{
    public static class PacketInitialization
    {
        private static bool hasInitialized = false;

        public static void Init()
        {
            if (hasInitialized)
                return;

            Debug.Log("Initializing Packets");

            PacketTable.AddPacket(PacketActionList.ECHO, new PacketTable.PacketAction(PacketActionList.Echo));

            hasInitialized = true;
        }
    }

    public static class PacketActionList
    {
        //Echo packet
        public static readonly byte ECHO = 0;
        public static void Echo(byte[] data)
        {
            Debug.Log("Received echo packet from server!");
            
        }
    }
}
