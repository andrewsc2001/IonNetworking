using System;

namespace Techn_000.Networking.Engine.Tools
{
    public class PacketBuilder
    {
        byte[] data = new byte[0];

        public void Flush()
        {
            data = new byte[0];
        }

        public void AddBytes(byte[] bytes)
        {
            byte[] newBytes = new byte[data.Length + bytes.Length];

            //add 'data' to front of newBytes
            Buffer.BlockCopy(data, 0, newBytes, 0, data.Length);
            //add 'bytes' after data
            Buffer.BlockCopy(bytes, 0, newBytes, data.Length, bytes.Length);

            data = newBytes;
        } 

        public void AddBytesToFront(byte[] bytes)
        {
            byte[] newBytes = new byte[data.Length + bytes.Length];

            //add 'bytes' to front of newBytes
            Buffer.BlockCopy(bytes, 0, newBytes, data.Length, bytes.Length);
            //add 'data' after bytes
            Buffer.BlockCopy(data, 0, newBytes, 0, data.Length);

            data = newBytes;
        }

        public void AddBytes(byte bytes)
        {
            AddBytes(new byte[] { bytes });
        }

        public void AddBytesToFront(byte bytes)
        {
            AddBytesToFront(new byte[] { bytes });
        }

        public byte[] GetPacket()
        {
            return data;
        }
    }
}
