using System.Collections.Generic;
using UnityEngine;

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
                    PacketHandler.HandleData(packet);
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
            byte header = data[0]; //Header: first packet used for identifying the purpose of the packet.

            //Get the function associated with the header from the PacketTable.
            PacketManager.PacketAction action = PacketManager.GetAction(header);

            //If there is no packet type with that header, return.
            if (action == null)
            {
                Debug.Log("A packet was recieved but no PacketAction could be found! header:" + header);
                return;
            }

            //Send the data to the packet action.
            action(data);
        }
    }
}