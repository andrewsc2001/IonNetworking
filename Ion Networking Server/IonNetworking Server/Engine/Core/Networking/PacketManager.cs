using System;
using System.Collections;
using System.Collections.Generic;

namespace IonNetworking_Server.Engine.Core.Networking
{
    public static class PacketManager
    {
        private static readonly List<Packet> _registerQueue = new List<Packet>();
        public static Hashtable _headersToActions { get; private set; } = new Hashtable();
        public static Hashtable _headersToNames { get; private set; } = new Hashtable();
        public static Hashtable _namesToHeaders { get; private set; } = new Hashtable();

        public static bool Locked { get; private set; } = false;

        public delegate void PacketAction(byte[] data);

        //Add packet. Will be added to list on Finalize();
        public static void AddPacket(string name, PacketAction action)
        {
            if (Locked)
                return;

            foreach(Packet packet in _registerQueue)
            {
                if(packet.name == name)
                {
                    Console.WriteLine("Cannot create duplicate packet type: " + name + "!");
                    return;
                }
            }

            _registerQueue.Add(new Packet(name, action));
        }

        //Assigns all packets with names and actions to byte headers.
        public static void Lock()
        {
            if (Locked)
                return;



            Console.WriteLine("Finalizing Packet Types");

            foreach(Packet packet in _registerQueue)
            {
                byte header = GetUnusedHeader();

                _headersToActions.Add(header, packet.action);
                _headersToNames.Add(header, packet.name);
                _namesToHeaders.Add(packet.name, header);
            }

            _registerQueue.Clear();

            Locked = true;
        }
        
        //Allows dev to specify the header. Useful for initialization that may take place before all PacketTables are loaded on the client.
        public static void AddPacket(string name, byte header, PacketAction action)
        {
            if (Locked)
                return;

            if (_headersToActions.ContainsKey(header))
            {
                Console.WriteLine("Cannot add core packet " + name + "!");
                return;
            }

            _headersToActions.Add(header, action);
            _headersToNames.Add(header, name);
            _namesToHeaders.Add(name, header);
        }

        //Returns an action from a byte header
        public static PacketAction GetAction(byte header)
        {
            object action = _headersToActions[header];

            if (action == null)
                throw new Exception("No packet with header: " + header + "!");

            return (PacketAction)action;
        }

        //Returns a header from a name
        public static byte GetHeader(string name)
        {
            object header = _namesToHeaders[name];

            if (header == null)
                throw new Exception("No packet with named " + name + "!");

            return (byte)header;
        }

        //Returns a name from a header
        public static string GetName(byte header)
        {
            object name = _headersToNames[header];

            if (name == null)
                throw new Exception("No packet with header: " + header + "!");

            return (string)name;
        }

        //Returns an unused header.
        private static byte GetUnusedHeader()
        {
            byte header = 1; //Header 0 is reserved for sending PacketTables from the server.
            while (true)
            {
                if (_headersToActions.ContainsKey(header))
                {
                    if (header == 255)
                        throw new Exception("Could not find unused header!");

                    header++;
                    continue;
                }

                return header;
            }
        }

        //Used to store packets before they're locked into the register.
        private struct Packet
        {
            public string name;
            public PacketAction action;

            public Packet(string name, PacketAction action)
            {
                this.name = name;
                this.action = action;
            }
        }
    }
}
