using System;
using IonServer.Networking.Engine;

namespace IonServer.Networking.Content
{
    public static class PacketInitialization
    {
        private static bool hasInitialized = false;

        public static void Init()
        {
            if (hasInitialized)
                return;

            Console.WriteLine("Initializing Packet Table");

            PacketManager.AddPacket(PacketTable.ECHO, new PacketManager.PacketAction(PacketTable.Echo));

            hasInitialized = true;
        }
    }

    public static class PacketTable
    {
        //Echo packet
        public static readonly byte ECHO = 0;
        public static void Echo(byte[] data)
        {
            Client client = NetworkManager.GetClientFromIndex(data[0]);
            data[0] = ECHO; //Set first byte back to the ECHO header.

            Console.WriteLine("Received echo packet from client " + client.index);
            
            //Send data back to client
            client.Send(data);
        }
    }
}
