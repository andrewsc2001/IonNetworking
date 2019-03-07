using IonClient.Core.Networking;
using IonClient.Core.Networking.Tools;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class PacketTable : MonoBehaviour
    {
        public void Awake()
        {
            Debug.Log("Initializing Packets");

            PacketManager.AddPacket(ECHO, Echo);
            PacketManager.AddPacket(MOVE, Move);
        }
        
        //Echo packet
        public static readonly byte ECHO = 0;
        public static void Echo(byte[] data)
        {
            Debug.Log("Received echo packet from server!");
        }
        
        //Move packet
        public static readonly byte MOVE = 1;
        public static void Move(byte[] data)
        {
            PacketReader pr = new PacketReader();

            pr.SetCursor(1); //Move the cursor past the header.

            byte netID = pr.ReadByte();
            int x = pr.ReadInt();
            int y = pr.ReadInt();

            pr.Clear();


        }
    }
}
