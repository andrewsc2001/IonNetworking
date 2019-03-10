using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IonClient.Core.Networking
{
    public static class PacketManager
    {
        private static readonly List<Packet> _registerQueue = new List<Packet>();
        private static readonly Hashtable _headersToActions = new Hashtable();
        private static readonly Hashtable _headersToNames = new Hashtable();
        private static readonly Hashtable _namesToHeaders = new Hashtable();

        public static bool Locked { get; private set; }

        public delegate void PacketAction(byte[] data);

        //Add packet. Will be added to list on Finalize();
        public static void AddPacket(string name, PacketAction action)
        {
            if (Locked)
                return;

            foreach (Packet packet in _registerQueue)
            {
                if (packet.name == name)
                {
                    Debug.Log("Cannot create duplicate packet type: " + name + "!");
                    return;
                }
            }

            _registerQueue.Add(new Packet(name, action));
        }

        //Assigns all packets with names and actions to byte headers.
        public static void Lock(Hashtable packetTable)
        {
            if (Locked)
                return;

            Debug.Log("Finalizing Packet Types");

            foreach(DictionaryEntry pair in packetTable)
            {
                Packet packet = GetQueuedPacket((string)pair.Value);
                if(packet == null)
                {
                    Debug.Log("Server sent a packet with an unrecognized packet name: " + pair.Value + "!");
                    return;
                }
                AddPacket((string)pair.Value, (byte)pair.Key, packet.action);
                _registerQueue.Remove(packet);
            }

            if (_registerQueue.Count > 0)
            {
                Debug.Log("Not all client-registered packets were included in the server packet table!");
                foreach(Packet packet in _registerQueue)
                {
                    Debug.Log(packet.name);
                }
            }
            
            Locked = true;
        }

        private static Packet GetQueuedPacket(string name)
        {
            foreach (Packet packet in _registerQueue)
            {
                if (name == packet.name)
                {
                    return packet;
                }
            }

            return null;
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

            if (action != null)
                return (PacketAction)action;

            return null;
        }

        //Returns a header from a name
        public static byte GetHeader(string name)
        {
            object header = _namesToHeaders[name];

            if (header != null)
                return (byte)header;

            return 255;
        }

        //Returns a name from a header
        public static string GetName(byte header)
        {
            object name = _headersToNames[header];

            if (name != null)
                return (string)name;

            return null;
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
        private class Packet
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