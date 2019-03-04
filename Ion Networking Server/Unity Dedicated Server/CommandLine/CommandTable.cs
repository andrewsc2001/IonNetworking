using IonServer.Networking.Engine;
using System;

namespace IonServer.CommandLine
{
    public static class CommandInitialization
    {
        private static bool hasInitialized = false;

        public static void Init()
        {
            if (hasInitialized)
                return;

            Console.WriteLine("Initializing Console Commands");

            CommandManager.AddCommand("echo", CommandTable.Echo);
            CommandManager.AddCommand("quit", CommandTable.Quit);
            CommandManager.AddCommand("kick", CommandTable.Kick);
            CommandManager.AddCommand("listclients", CommandTable.ListClients);
        }
    }

    public static class CommandTable
    {
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
            
            Console.WriteLine("Kicking client " + index);
            client.CloseConnection();
        }

        //List clients command
        public static void ListClients(string[] arguments)
        {
            byte connectedClients = 0;
            for(byte index = 0; index < NetworkManager.MaxPlayers; index++)
            {
                Console.Write("Client " + index + ": ");

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
