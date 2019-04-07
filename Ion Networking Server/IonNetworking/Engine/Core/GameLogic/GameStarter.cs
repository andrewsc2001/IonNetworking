using IonNetworking.Engine.Core.CommandLine;
using IonNetworking.Engine.Core.Networking;
using System;
using System.Diagnostics;
using System.Threading;

namespace IonNetworking.Engine.Core.GameLogic
{
    public static class GameStarter
    {
        private static Thread _gameLogicThread = new Thread(StartInternal);
        private static bool isRunning = false;

        public delegate void GameFunction();

        private static GameFunction start;
        private static GameFunction stop;
        private static GameFunction update;

        public static int UpdatesPerSecond = 20;
        public static int AllottedPacketHandleTime = 10;
        public static Stopwatch Time;

        private static void StartInternal()
        {
            Console.WriteLine("Starting Game Logic");
            isRunning = true;

            //Run the start function
            start();

            StartUpdateLoop();
        }

        //Starts the GameStarter
        public static void Start(GameFunction start, GameFunction stop, GameFunction update)
        {
            GameStarter.start = start;
            GameStarter.stop = stop;
            GameStarter.update = update;

            Time = new Stopwatch();

            _gameLogicThread.Start();
        }

        public static void Stop()
        {
            Console.WriteLine("Stopping Game Thread");
            StopUpdateLoop();

            _gameLogicThread.Join(); //Wait for last update to finish running and for the _gameLogicThread to terminate.

            Console.WriteLine("Game Thread Stopped");
            Console.WriteLine("Finishing Game Logic");
            stop();
        }

        //Starts the update loop for the game. This is where the _gameLogicThread will be occupied.
        private static void StartUpdateLoop()
        {
            long timeBetweenUpdates = 1000L/UpdatesPerSecond;

            long nextUpdate = 0;
            long now;
            long delta;

            while (isRunning)
            {
                now = Time.ElapsedMilliseconds;
                delta = nextUpdate - now;

                if (delta <= 0)
                {
                    nextUpdate = now + timeBetweenUpdates;
                    
                    update();
                }
                else if (delta > AllottedPacketHandleTime)
                {
                    //Process commands and packets before the game update.
                    CommandManager.HandleQueue();

                    if (PacketHandler.HandleNextPacket())
                        continue;

                    Thread.Sleep(AllottedPacketHandleTime - 1);
                }
            }
        }

        //Stops the game loop.
        private static void StopUpdateLoop()
        {
            Console.WriteLine("Stopping Update Loop");
            isRunning = false;
        }
    }
}
