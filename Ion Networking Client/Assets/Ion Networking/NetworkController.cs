using IonClient.Core.Networking;
using UnityEngine;

namespace IonClient.Core
{
    public class NetworkController : MonoBehaviour
    {
        private void Awake()
        {
            NetworkManager.Init();
        }

        private void Update()
        {
            PacketHandler.ProcessQueue();
        }
    }
}