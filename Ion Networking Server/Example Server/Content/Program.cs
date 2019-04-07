using IonNetworking.Engine.Core.CommandLine;
using IonNetworking.Engine.Core.GameLogic;
using IonNetworking.Engine.Core.Networking;
using ExampleServer.Content;
using ExampleServer.Content.Core;
using System;

namespace ExampleServer
{
    //Example class for development purposes. This represents player code that starts the game.
    class Program
    {
        private static bool _isRunning;

        static void Main(string[] args)
        {
            //Initializes the server on port 35565. MaxPlayers=4
            NetworkManager.Init(35565, 4);


            //Initialize Content
            CommandTable.Init();
            PacketTable.Init();
            PacketManager.Lock(); //Locks the PacketManager.

            //Start Game Logic
            GameStarter.Start(Game.Start, Game.Stop, Game.Update);

            //Start listening for clients
            NetworkManager.StartListener();

            _isRunning = true;
            StartCommandLoop();
        }
        
        //Starts the command loop
        private static void StartCommandLoop()
        {
            Console.WriteLine("Starting Command Loop");
            while (_isRunning)
            {
                string raw = Console.ReadLine();
                CommandManager.QueueCommand(CommandManager.ParseCommand(raw));
            }

            Console.WriteLine("Exited Command Loop");
            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
        }

        //Graceful shutdown
        public static void Shutdown(string[] arguments)
        {
            Console.WriteLine("Shutting down");

            _isRunning = false;
            NetworkManager.Stop();
            GameStarter.Stop();
            
        }
    }
}
