
namespace IonNetworking_Server.Engine.Core.Networking.Tools
{
    public class PacketSplitter
    {
        public static byte[][] SplitBytes(byte[] data) //[packetnumber][data index] to reference a packet, loop through [packetnumber][i]
        {
            //All packets have a length in front of them. The length is the number of bytes in the ENTIRE PACKET, including the length byte
            //LENGTH, HEADER, DATA[]

            byte length = data[0]; //Get length

            if (data.Length == length) //If the length of the first packet is the length of the whole array, there is only one packet. return packet.
            {

                byte[][] send = new byte[1][];
                send[0] = new byte[data.Length - 1];

                for (int index = 0; index < send[0].Length; index++)
                {
                    send[0][index] = data[index + 1];
                }

                return send;
            }

            //Multiple packets.

            int numOfPackets = 0;

            //Find number of packets.
            int place = 0; //Keep track of place.
            while (true)
            {
                if (place == data.Length)
                {
                    break;
                }

                numOfPackets++;

                place += data[place];
                //Example of two packets jammed together.
                // 2 D 3 D D
                // index: 0
                // packetLength: 2 (data[0])
                // numOfPackets: 1

                // index: 2
                // packetLength: 3 (data[2])
                // numOfPackets: 2

                // index: 5
                // index == length, no more packets.

            }

            //Create byte[][] for return
            byte[][] split = new byte[numOfPackets][];

            //Add packets into split
            int firstByte = 0; //Used for keeping place in data. Represents the location of the first byte of a packet.

            //Create byte[]s in split and copy raw data into them.
            for (int index = 0; index < numOfPackets; index++)
            {
                byte packetLength = (byte)(data[firstByte] - 1);

                split[index] = new byte[packetLength]; //Create array for packet.

                //Copy data from data to packet
                for (int pos = 1; pos < packetLength; pos++) //Pos starts at 1 to skip the length bytes
                {
                    split[index][pos - 1] = data[firstByte + pos];
                }

                firstByte += packetLength + 1;

            }
            return split;
        }
    }
}
