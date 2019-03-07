using Assets.Scripts.Networking;
using IonClient.Core.Networking;
using UnityEngine;

public class DemoStarter : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        NetworkManager.Connect("127.0.0.1", 35565);
    }

    // Update is called once per frame
    bool hasSentEcho = false;
    void Update()
    {
        if (NetworkManager.isConnected && !hasSentEcho)
        {
            NetworkManager.SendToServer(new byte[] { PacketTable.ECHO });
            hasSentEcho = true;
        }
    }
}
