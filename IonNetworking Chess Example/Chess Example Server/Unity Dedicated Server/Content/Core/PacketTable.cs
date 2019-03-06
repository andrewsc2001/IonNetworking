using IonServer.Engine.Core.Networking;
using System;

namespace IonServer.Content.Core
{
    public static class PacketTable
    {
        public static void Init()
        {
            Console.WriteLine("Initializing Core Packet Table");

            PacketManager.AddPacket(ECHO, Echo);
        }

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
