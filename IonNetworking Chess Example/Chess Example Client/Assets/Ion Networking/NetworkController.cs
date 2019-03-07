
using IonClient.Core.Networking;
using UnityEngine;

namespace Assets.IonClient
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
