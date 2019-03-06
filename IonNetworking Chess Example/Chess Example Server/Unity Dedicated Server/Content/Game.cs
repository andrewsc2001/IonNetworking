using IonServer.Engine.Core.Networking;
using System;
using System.Diagnostics;
using static Unity_Dedicated_Server.Content.Board;

namespace IonServer.Content
{
    public static class Game
    {
        //Settings
        public static int UpdatesPerSecond = 10;
        public static byte MaxPlayers = 2;

        public static bool isRunning = false;
        public static Stopwatch Time;

        public static Client whiteClient;
        public static Client blackClient;
        public static Team currentTurn; 

        public static bool allClientsConnected = false;

        //Run once at startup
        public static void Start()
        {
            Console.WriteLine("Starting Game Logic");
            whiteClient = NetworkManager.GetClientFromIndex(0);
            blackClient = NetworkManager.GetClientFromIndex(1);

            currentTurn = Team.White;
        }

        //Run once at shutdown
        public static void Stop()
        {

        }

        //Game Loop
        public static void Update()
        {
            //Waiting for connections
            if (!allClientsConnected)
            {
                if(whiteClient.IsConnected() && blackClient.IsConnected())
                {
                    allClientsConnected = true;
                }
                return;
            }
        }
    }
}
