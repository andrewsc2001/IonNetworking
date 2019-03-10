using IonClient.Core.Networking;
using IonClient.Core.Networking.Tools;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class PacketTable : MonoBehaviour
    {
        public void Awake()
        {
            Debug.Log("Initializing Packets");

            PacketManager.AddPacket("SyncPacketTable", 0, SyncPacketTable);
            PacketManager.AddPacket("echo", Echo);
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

        //Echo packet
        public static void Echo(byte[] data)
        {
            

            byte lifespan = data[1];
            Debug.Log("Received echo packet from server with a lifespan of " + lifespan);

            if (lifespan > 0)
            {
                data[1]--;
                Debug.Log("Sending it back!");
                NetworkManager.SendToServer(data);
            }
        }
        
    }
}
