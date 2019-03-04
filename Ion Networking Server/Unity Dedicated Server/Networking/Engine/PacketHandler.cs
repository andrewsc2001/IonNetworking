using System;

namespace IonServer.Networking.Engine
{
    public static class PacketHandler
    {
        //Take data and route it to the correct packet type to be processed.
        public static void HandleData(Client client, byte[] data)
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
