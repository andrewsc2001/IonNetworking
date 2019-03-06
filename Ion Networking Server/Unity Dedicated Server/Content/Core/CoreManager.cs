namespace IonServer.Content.Core
{
    public static class CoreManager
    {
        public static void Init()
        {
            PacketTable.Init();
            CommandTable.Init();
        }
    }
}
