using IonServer.Content.Core;
using IonServer.Engine.Core.Networking;

namespace IonServer.Content
{
    public static class Initializer
    {
        public static void Init()
        {
            CoreManager.Init();

            PacketManager.Lock();
        }
    }
}
