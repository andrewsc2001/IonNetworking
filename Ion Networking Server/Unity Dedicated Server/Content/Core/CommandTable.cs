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

            CommandManager.AddCommand("echo", CommandTable.Echo);
            CommandManager.AddCommand("quit", CommandTable.Quit);
            CommandManager.AddCommand("kick", CommandTable.Kick);
            CommandManager.AddCommand("listclients", CommandTable.ListClients);
            CommandManager.AddCommand("test", CommandTable.Test);
        }
    }

    public static class CommandTable
    {
        //Test command
        public static void Test(string[] arguments)
        {
            Console.WriteLine("test");
        }

        //Echo command
        public static void Echo(string[] arguments)
        {
            if(arguments.Length > 1)
            {
                for(int index = 1; index < arguments.Length; index++)
                {
                    Console.Write(arguments[index] + " ");
                }
                Console.WriteLine();
            }
        }
        
        //Quit command
        public static void Quit(string[] arguments)
        {
            Program.Shutdown();
        }

        //Kick command
        public static void Kick(string[] arguments)
        {
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
