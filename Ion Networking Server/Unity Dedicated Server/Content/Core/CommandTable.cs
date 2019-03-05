using IonServer.Engine.Core.CommandLine;
using IonServer.Engine.Core.Networking;
using System;

namespace IonServer.Content.Core
{
    public static class CommandInitialization
    {
        public static void Init()
        {
            Console.WriteLine("Initializing Console Commands");
            
            CommandManager.AddCommand("quit", CommandTable.Quit);
            CommandManager.AddCommand("kick", CommandTable.Kick);
            CommandManager.AddCommand("listclients", CommandTable.ListClients);
        }
    }

    public static class CommandTable
    {
        //Quit command
        public static void Quit(string[] arguments)
        {
            if(arguments.Length > 1)
            {
                Console.WriteLine("Invalid arguments.");
                return;
            }

            Program.Shutdown();
        }

        //Kick command
        public static void Kick(string[] arguments)
        {
            if (arguments.Length != 2)
            {
                Console.WriteLine("Invalid arguments.");
                return;
            }

            int index = -1;
            int.TryParse(arguments[1], out index);

            if (index < 0 || index >= NetworkManager.MaxPlayers)
            {
                Console.WriteLine("Client " + index + " is out of bounds.");
                return;
            }

            Client client = NetworkManager.GetClientFromIndex((byte)index);

            if(client == null)
            {
                Console.WriteLine("Couldn't kick client " + index);
                return;
            }
            
            client.CloseConnection();
        }

        //List clients command
        public static void ListClients(string[] arguments)
        {
            if (arguments.Length > 1)
            {
                Console.WriteLine("Invalid arguments.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("List of connected clients:");

            byte connectedClients = 0;
            
            for(byte index = 0; index < NetworkManager.MaxPlayers; index++)
            {
                Console.Write("    Client " + index + ": ");

                if (NetworkManager.GetClientFromIndex(index)._tcpSocket == null)
                {
                    Console.WriteLine("OPEN");
                }
                else
                {
                    Console.WriteLine("CONNECTED");
                    connectedClients++;
                }
            }
            Console.WriteLine();
            Console.WriteLine(connectedClients + " connected out of " + NetworkManager.MaxPlayers + " max.");
        }
    }
}
