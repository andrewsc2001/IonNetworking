using IonServer.Engine.Core.Networking;
using System;

namespace IonServer.Content.Core
{
    public static class PacketInitialization
    {
        public static void Init()
        {
            Console.WriteLine("Initializing Packet Table");

            PacketManager.AddPacket(PacketTable.ECHO, new PacketManager.PacketAction(PacketTable.Echo));
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
