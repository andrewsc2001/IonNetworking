using System;
using System.Collections.Generic;

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

        private static readonly List<Packet> queue = new List<Packet>();

        //Handles all packets in the queue
        public static void HandleQueue()
        {
            lock (queue)
            {
                foreach(Packet packet in queue)
                {
                    HandleData(packet.sender, packet.data);
                }
                queue.Clear();
            }
        }

        //Queues packets to be handled by another thread
        public static void QueuePacket(Client sender, byte[] data)
        {
            lock (queue)
            {
                queue.Add(new Packet(sender, data));
            }
        }

        //Take data and route it to the correct packet type to be processed.
        private static void HandleData(Client client, byte[] data)
        {
            byte header = data[0]; //Header: first packet used for identifying the purpose of the packet.
            data[0] = client.index;

            //Get the function associated with the header from the PacketTable.
            PacketManager.PacketAction action = PacketManager.GetAction(header);

            //If there is no packet type with that header, return.
            if (action == null)
            {
                Console.WriteLine("A packet was recieved but no PacketAction could be found! header:" + header);
                return;
            }

            //Send the data to the packet action.
            action(data);
        }
    }
}
