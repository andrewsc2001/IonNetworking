using System;
using System.Text;

namespace IonNetworking_Server.Engine.Core.Networking.Tools
{
    public class PacketReader
    {
        private int cursor = 1; //The index the reader is at. All read commands start at the cursor and add to it as they read.
        private byte[] loadedPacket = null;

        public PacketReader() { }

        public PacketReader(byte[] data)
        {
            LoadPacket(data);
        }

        //Loads a packet into the reader
        public void LoadPacket(byte[] packet)
        {
            loadedPacket = packet;
        }

        //Clears the current packet
        public void Clear()
        {
            loadedPacket = null;
        }

        //Sets the cursor
        public void SetCursor(int cursor)
        {
            this.cursor = cursor;
        }

        //Returns the remaining bytes
        public int GetRemainingLength()
        {
            if (loadedPacket == null)
                return 0;
            return loadedPacket.Length - cursor;
        }

        //////////////////////////Type reads

        //Read an sbyte
        public sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }

        //Read a short
        public short ReadShort()
        {
            return BitConverter.ToInt16(ReadBytes(2), 0);
        }

        //Read a ushort
        public ushort ReadUShort()
        {
            return (ushort)ReadShort();
        }

        //Read an int
        public int ReadInt()
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }

        //Read a uint
        public uint ReadUInt()
        {
            return (uint)ReadInt();
        }

        //Read a long
        public long ReadLong()
        {
            return BitConverter.ToInt64(ReadBytes(8), 0);
        }

        //Read a ulong
        public ulong ReadULong()
        {
            return (ulong)ReadLong();
        }

        //Read a float
        public float ReadFloat()
        {
            return BitConverter.ToSingle(ReadBytes(4), 0);
        }

        //Read a double
        public double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBytes(8), 0);
        }

        //Read a char
        public char ReadChar()
        {
            return BitConverter.ToChar(ReadBytes(2), 0);
        }

        //Read a string
        public string ReadString()
        {
            int length = ReadInt();

            return Encoding.UTF8.GetString(ReadBytes(length));
        }

        //Read a bool
        public bool ReadBool()
        {
            byte read = ReadByte();

            if (read == 0)
                return false;
            else if (read == 1)
                return true;
            else
            {
                Console.WriteLine("Tried to read bool from packet but found " + read);
                return false;
            }
        }

        //////////////////////////Basic read functions

        //Returns the next byte in the packet
        public byte ReadByte()
        {
            if (loadedPacket == null)
            {
                Console.WriteLine("Cannot read from null packet!");
                return 0;
            }

            cursor++;
            return loadedPacket[cursor - 1];
        }

        //Returns a byte[] read from the packet.
        public byte[] ReadBytes(int length)
        {
            if (length <= 0 || length > GetRemainingLength())
            {
                Console.WriteLine("Length out of bounds!");
                return null;
            }

            byte[] read = new byte[length];

            Buffer.BlockCopy(loadedPacket, cursor, read, 0, length);

            cursor += length;
            return read;
        }
    }
}
