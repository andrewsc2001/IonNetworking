using IonNetworking.Engine.Core.Networking;
using System;

namespace ExampleServer.Content.Core
{
    public static class PacketTable
    {
        public static void Init()
        {
            Console.WriteLine("Initializing Packet Table");
            
            PacketManager.AddPacket("echo", new PacketManager.PacketAction(Echo));
        }

        //Echo packet
        public static void Echo(byte[] data)
        {
            Client client = NetworkManager.GetClientFromIndex(data[0]);
            
            data[0] = PacketManager.GetHeader("echo");

            byte lifespan = data[1];

            if(lifespan == 0) 
            {
                Console.WriteLine("Finished echo");
            }

            if (lifespan > 0)
            {
                data[1]--;
                client.Send(data);
            }
        }
    }
}
