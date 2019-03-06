using IonServer.Engine.Core.Networking;
using System;

namespace Unity_Dedicated_Server.Content
{
    public class PacketTable
    {
        public static void Init()
        {
            Console.WriteLine("Initializing Packet Table");

            PacketManager.AddPacket(MOVE, Move);
        }

        //Move packet
        public static byte MOVE = 1;
        public static void Move(byte[] data)
        {

        }
    }
}
