using IonServer.Content;
using IonServer.Engine.Core.CommandLine;
using IonServer.Engine.Core.GameLogic;
using IonServer.Engine.Core.Networking;
using System;
using System.Threading;

namespace IonServer
{
    class Program
    {
        public static Thread _gameLogicThread = new Thread(GameStarter.Start);

        private static bool _isRunning;

        static void Main(string[] args)
        {
            //Initialize Content
            Initializer.Init();

            //Start the NetworkManager. Will accept client connections past this point.
            NetworkManager.Start();
            
            //Start Game Logic
            _gameLogicThread.Start();

            _isRunning = true;
            StartCommandLoop();
        }
        
        //Starts the command loop
        private static void StartCommandLoop()
        {
            Console.WriteLine("Starting Command Loop");
            while (_isRunning)
            {
                string rawInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(rawInput))
                    continue;

                string[] arguments = rawInput.Split(' '); //Split the raw text into arguments.                

                CommandManager.QueueCommand(arguments);
            }

            Console.WriteLine("Exited Command Loop");
            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
        }

        //Graceful shutdown
        public static void Shutdown()
        {
            Console.WriteLine("Shutting down");

            _isRunning = false;
            NetworkManager.Stop();
            GameStarter.Stop();
            
        }
    }
}
