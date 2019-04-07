using System;
using System.Text;

namespace IonNetworking.Engine.Core.Networking.Tools
{
    public class PacketBuilder
    {
        byte[] packet = new byte[0];
        
        //No initial settings
        public PacketBuilder() { }

        //Writes header
        public PacketBuilder(string packetName)
        {
            Write(PacketManager.GetHeader(packetName));
        }

        //Clears current packet
        public void Clear()
        {
            packet = new byte[0];
        }

        //////////////////////////Type write functions

        //Writes an sbyte
        public void Write(sbyte data)
        {
            Write(BitConverter.GetBytes(data));
        }

        //Write a short
        public void Write(short data)
        {
            Write(BitConverter.GetBytes(data));
        }

        //Write a ushort
        public void Write(ushort data)
        {
            Write(BitConverter.GetBytes(data));
        }

        //Write an int
        public void Write(int data)
        {
            Write(BitConverter.GetBytes(data));
        }

        //Write a uint
        public void Write(uint data)
        {
            Write(BitConverter.GetBytes(data));
        }

        //Write a long
        public void Write(long data)
        {
            Write(BitConverter.GetBytes(data));
        }

        //Write a ulong
        public void Write(ulong data)
        {
            Write(BitConverter.GetBytes(data));
        }

        //Write a float
        public void Write(float data)
        {
            Write(BitConverter.GetBytes(data));
        }

        //Write a double
        public void Write(double data)
        {
            Write(BitConverter.GetBytes(data));
        }

        //Write a char
        public void Write(char data)
        {
            Write(BitConverter.GetBytes(data));
        }

        //Write a string
        public void Write(string data)
        {
            byte[] send = Encoding.UTF8.GetBytes(data);
            Write(send.Length);
            Write(send);
        }

        //Write a bool
        public void Write(bool data)
        {
            Write(BitConverter.GetBytes(data));
        }

        //////////////////////////Basic write functions

        //Adds bytes to the end of the packet
        public void Write(byte[] bytes)
        {
            Array.Resize(ref packet, bytes.Length + packet.Length);

            Buffer.BlockCopy(bytes, 0, packet, packet.Length - bytes.Length, bytes.Length);
        } 

        //Adds bytes to the front of the packet (mostly for wrapping already made packets).
        public void WriteToFront(byte[] bytes)
        {
            Array.Resize(ref bytes, bytes.Length + packet.Length);

            Buffer.BlockCopy(packet, 0, bytes, bytes.Length - packet.Length, packet.Length);
            packet = bytes;
        }

        //Writes a byte to the end of the packet
        public void Write(byte bytes)
        {
            Write(new byte[] { bytes });
        }

        //Writes a byte to the beginning of the packet
        public void WriteToFront(byte bytes)
        {
            WriteToFront(new byte[] { bytes });
        }

        //Returns the packet
        public byte[] GetPacket()
        {
            return packet;
        }
    }
}
