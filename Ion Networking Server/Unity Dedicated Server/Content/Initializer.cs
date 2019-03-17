using IonServer.Content.Core;
using IonServer.Engine.Core.Networking;

namespace IonServer.Content
{
    public static class Initializer
    {
        public static void Init()
        {
            NetworkManager.Init(35565, 4); //Initializes the server on port 35565. MaxPlayers=4

            CoreManager.Init(); //Initializes core content.

            PacketManager.Lock(); //Locks the PacketManager. 
        }
    }
}
