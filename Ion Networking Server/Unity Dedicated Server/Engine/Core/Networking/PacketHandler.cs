using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace IonServer.Engine.Core.Networking
{
    public static class PacketHandler
    {
        private struct Packet
        {
            public Client sender;
            public byte[] data;

            public Packet(Client sender, byte[] data)
            {
                this.sender = sender;
                this.data = data;
            }
        }

        //private static readonly List<Packet> queue = new List<Packet>();
        private static ConcurrentQueue<Packet> queue = new ConcurrentQueue<Packet>();

        //Handles next packet
        public static bool HandleNextPacket()
        {
            Packet packet;
            if (queue.TryDequeue(out packet))
            {
                HandleData(packet.sender, packet.data);
                return true;
            } else  {
                return false;
            }
        }

        //Queues packets to be handled by another thread
        public static void QueuePacket(Client sender, byte[] data)
        {
            lock (queue)
            {
                queue.Enqueue(new Packet(sender, data));
            }
        }

        //Take data and route it to the correct packet type to be processed.
        private static void HandleData(Client client, byte[] data)
        {
            byte header = data[0]; //Header: first packet used for identifying the purpose of the packet.
            data[0] = client.Index;

            //Get the function associated with the header from the PacketTable.
            PacketManager.PacketAction action = PacketManager.GetAction(header);

            //If there is no packet type with that header, return.
            if (action == null)
            {
                return;
            }

            //Send the data to the packet action.
            action(data);
        }
    }
}
