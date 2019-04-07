using IonClient.Core.Networking;
using UnityEngine;

namespace IonClient.Core
{
    public class NetworkController : MonoBehaviour
    {
        public static NetworkController Singleton;

        private void Awake()
        {
            Singleton = this;

            //Adds SyncPacketTable in engine code because it is required for the engine to function.
            PacketManager.AddPacket("SyncPacketTable", 0, PacketManager.SyncPacketTable);

            NetworkManager.Init();
        }

        private void Update()
        {
            PacketHandler.ProcessQueue();
        }

        public void OnConnected()
        {
            Debug.Log("Connected!");
        }
    }
}