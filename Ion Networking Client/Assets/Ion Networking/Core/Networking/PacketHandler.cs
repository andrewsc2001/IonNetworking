
using System;
using System.Collections.Generic;

namespace IonClient.Core.Networking
{
    static class PacketHandler
    {
        private static List<byte[]> queue = new List<byte[]>();

        //Process all packets in the queue
        public static void ProcessQueue()
        {
            lock (queue)
            {
                foreach (byte[] packet in queue)
                {
                    HandleData(packet);
                }
                queue.Clear();
            }
        }

        //Add packet to queue
        public static void AddToQueue(byte[] data)
        {
            lock (queue)
            {
                queue.Add(data);
            }
        }

        //Take data and route it to the correct packet type to be processed.
        public static void HandleData(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("Cannot handle null packet!");

            byte header = data[0]; //Header: first packet used for identifying the purpose of the packet.

            //Get the function associated with the header from the PacketTable.
            PacketManager.PacketAction action = PacketManager.GetAction(header);

            //This allows developers to register packets that don't have actions. For example, the server will never receive the SyncPacketTable packet.
            if (action == null)
                return;

            //Send the data to the packet action.
            action(data);
        }
    }
}