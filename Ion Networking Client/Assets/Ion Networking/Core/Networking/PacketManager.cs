using IonClient.Core.Networking.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IonClient.Core.Networking
{
    public static class PacketManager
    {
        private static readonly List<Packet> _registerQueue = new List<Packet>();
        public static Hashtable _headersToActions = new Hashtable();
        public static Hashtable _headersToNames = new Hashtable();
        public static Hashtable _namesToHeaders = new Hashtable();

        public static bool Locked = false;

        public delegate void PacketAction(byte[] data);

        //Add packet. Will be added to list on Finalize();
        public static void AddPacket(string name, PacketAction action)
        {
            if (Locked)
                throw new InvalidOperationException("Cannot add packet after PacketTable has been locked!");

            foreach (Packet packet in _registerQueue)
            {
                if (packet.name == name)
                {
                    throw new ArgumentException("Cannot register " + packet.name + " packet: Name already taken.");
                }
            }

            _registerQueue.Add(new Packet(name, action));
        }

        //Assigns all packets with names and actions to byte headers.
        public static void Lock(Hashtable packetTable)
        {
            if (Locked)
                throw new InvalidOperationException("Cannot lock PacketTable after PacketTable has been locked!");

            Debug.Log("Finalizing Packet Table");

            foreach (DictionaryEntry pair in packetTable)
            {
                Packet packet;
                bool foundPacket = TryFindPacket(out packet, (string)pair.Value);
                if (!foundPacket)
                    throw new InvalidOperationException("Server sent PacketTable with unrecognized name: " + (string)pair.Value);

                AddPacket((string)pair.Value, (byte)pair.Key, packet.action);
                _registerQueue.Remove(packet);
            }

            Console.WriteLine("Finalizing Packet Types");
            
            if (_registerQueue.Count > 0)
            {
                //Throw error?
                Debug.Log("Not all client-registered packets were included in the server packet table!");
                foreach (Packet packet in _registerQueue)
                {
                    Debug.Log(packet.name);
                }
            }

            Locked = true;
        }
        private static void AddPacket(string name, byte header, PacketAction action)
        {
            if (Locked)
                throw new InvalidOperationException("Cannot add packet after PacketTable has been locked!");

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

        private static bool TryFindPacket(out Packet packet, string name)
        {
            foreach(Packet p in _registerQueue)
            {
                if(p.name == name)
                {
                    packet = p;
                    return true;
                }
            }

            packet = new Packet();
            return false;
        }

        //Takes a packet table from the server and registers it to the local packet table
        public static void SyncPacketTable(byte[] data)
        {
            Debug.Log("Received Packet Table from server");

            Hashtable headersToNames = new Hashtable();

            PacketReader pr = new PacketReader(data);
            byte lenghtOfPacketTable = pr.ReadByte();

            for (int i = 0; i < lenghtOfPacketTable; i++)
            {
                byte header = pr.ReadByte();
                string name = pr.ReadString();

                headersToNames.Add(header, name);
            }


            PacketManager.Lock(headersToNames);
        }

        //Adds core engine packets
        public static void AddEnginePackets()
        {
            Debug.Log("Adding Engine Packets");
            AddPacket("SyncPacketTable", 0, SyncPacketTable);
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