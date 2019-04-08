using System;
using System.Text;

namespace IonNetworking.Engine.Core.Networking.Tools
{
    public class PacketReader
    {
        private int cursor = 1; //The index the reader is at. All read commands start at the cursor and add to it as they read.
        private byte[] loadedPacket = null;

        public PacketReader() { }

        public PacketReader(byte[] data)
        {
            try
            {
                LoadPacket(data);
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException(e.Message);
            }
        }

        //Loads a packet into the reader
        public void LoadPacket(byte[] packet)
        {
            loadedPacket = packet ?? throw new ArgumentNullException("Cannot load null packet!");
        }

        //Clears the current packet
        public void Clear()
        {
            loadedPacket = null;
        }

        //Sets the cursor
        public void SetCursor(int cursor)
        {
            if (cursor < 0)
                throw new ArgumentOutOfRangeException("Cursor cannot be less than 0!");

            this.cursor = cursor;
        }

        //Returns the remaining bytes
        public int GetRemainingLength()
        {
            if (loadedPacket == null)
                throw new InvalidOperationException("Cannot calculate remaining length with no packet loaded!");

            return loadedPacket.Length - cursor;
        }

        //////////////////////////Type reads

        //Read an sbyte
        public sbyte ReadSByte()
        {
            try
            {
                return (sbyte)ReadByte();
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
        }

        //Read a short
        public short ReadShort()
        {
            try
            {
                return BitConverter.ToInt16(ReadBytes(2), 0);
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
        }

        //Read a ushort
        public ushort ReadUShort()
        {
            try
            {
                return (ushort)ReadShort();
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
        }

        //Read an int
        public int ReadInt()
        {
            try
            {
                return BitConverter.ToInt32(ReadBytes(4), 0);
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
        }

        //Read a uint
        public uint ReadUInt()
        {
            try
            {
                return (uint)ReadInt();
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
        }

        //Read a long
        public long ReadLong()
        {
            try
            {
                return BitConverter.ToInt64(ReadBytes(8), 0);
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
        }

        //Read a ulong
        public ulong ReadULong()
        {
            try
            {
                return (ulong)ReadLong();
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
        }

        //Read a float
        public float ReadFloat()
        {
            try
            {
                return BitConverter.ToSingle(ReadBytes(4), 0);
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
        }

        //Read a double
        public double ReadDouble()
        {
            try
            {
                return BitConverter.ToDouble(ReadBytes(8), 0);
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
        }

        //Read a char
        public char ReadChar()
        {
            try
            {
                return BitConverter.ToChar(ReadBytes(2), 0);
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
        }

        //Read a string
        public string ReadString()
        {
            try
            {
                int length = ReadInt();

                return Encoding.UTF8.GetString(ReadBytes(length));
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
        }

        //Read a bool
        public bool ReadBool()
        {
            try
            {
                return BitConverter.ToBoolean(ReadBytes(1), 0);
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
        }

        //////////////////////////Basic read functions

        //Returns the next byte in the packet
        public byte ReadByte()
        {
            if (loadedPacket == null)
                throw new InvalidOperationException("Cannot read from null packet!");

            cursor++;
            return loadedPacket[cursor - 1];
        }

        //Returns a byte[] read from the packet.
        public byte[] ReadBytes(int length)
        {
            if (loadedPacket == null)
                throw new InvalidOperationException("Cannot read from null packet!");

            if (length < 1)
                throw new ArgumentOutOfRangeException("Length cannot be less than 1!");

            if (length > GetRemainingLength())
                throw new ArgumentOutOfRangeException("Cannot read past end of packet!");

            byte[] read = new byte[length];

            Buffer.BlockCopy(loadedPacket, cursor, read, 0, length);

            cursor += length;
            return read;
        }
    }
}
