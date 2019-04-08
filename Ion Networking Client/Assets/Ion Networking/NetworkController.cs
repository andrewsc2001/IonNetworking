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

            PacketManager.AddEnginePackets();

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