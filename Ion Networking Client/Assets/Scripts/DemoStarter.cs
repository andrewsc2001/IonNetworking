using IonClient.Core.Networking;
using UnityEngine;

public class DemoStarter : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        NetworkManager.Connect("127.0.0.1", 35565);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
