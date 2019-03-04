using System.Collections.Generic;
using UnityEngine;

namespace Techn_000.Networking
{
    class PacketHandler : MonoBehaviour
    {
        public static PacketHandler Instance;

        private void Awake()
        {
            Instance = this;
        }


        List<byte[]> queue = new List<byte[]>();
        public void AddToQueue(byte[] data)
        {
            lock (queue)
            {
                queue.Add(data);
            }
        }

        private void Update()
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

        //Take data and route it to the correct packet type to be processed.
        public void HandleData(byte[] data)
        {
            byte header = data[0]; //Header: first packet used for identifying the purpose of the packet.

            //Get the function associated with the header from the PacketTable.
            PacketTable.PacketAction action = PacketTable.GetAction(header);

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