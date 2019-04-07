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
            
            PacketManager.AddPacket("echo", Echo);
        }

        //Echo packet
        public static void Echo(byte[] data)
        {
            byte lifespan = data[1];

            if(lifespan == 0)
            {
                Debug.Log("Echo finished");
            }

            if (lifespan > 0)
            {
                data[1]--;
                NetworkManager.SendToServer(data);
            }
        }
        
    }
}
