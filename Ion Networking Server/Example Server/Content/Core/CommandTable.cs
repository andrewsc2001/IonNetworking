
using IonNetworking.Engine.Core.CommandLine;
using IonNetworking.Engine.Core.Networking;
using IonNetworking.Engine.Core.Networking.Tools;
using System;

namespace ExampleServer.Content.Core
{
    public static class CommandTable
    {
        public static void Init()
        {
            Console.WriteLine("Initializing Console Commands");

            CommandManager.AddCommand("shutdown", Program.Shutdown);
            CommandManager.AddCommand("kick", Kick);
            CommandManager.AddCommand("listclients", ListClients);
            CommandManager.AddCommand("echo", Echo);
        }
        
        //Echo command
        public static void Echo(string[] arguments)
        {
            if(arguments.Length != 3)
            {
                Console.WriteLine("Invalid arguments.");
                return;
            }

            byte clientID;
            byte lifespan;

            byte.TryParse(arguments[1], out clientID);
            byte.TryParse(arguments[2], out lifespan);

            Client client = NetworkManager.GetClientFromIndex(clientID);

            PacketBuilder pb = new PacketBuilder("echo");
            pb.Write(lifespan);

            Console.WriteLine("Sending echo packet to client " + clientID + " with lifespan of " + lifespan);

            client.Send(pb.GetPacket());
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

                if (NetworkManager.GetClientFromIndex(index).Connected)
                {
                    Console.WriteLine("CONNECTED");
                    connectedClients++;
                }
                else
                {
                    Console.WriteLine("OPEN");
                }
            }
            Console.WriteLine();
            Console.WriteLine(connectedClients + " connected out of " + NetworkManager.MaxPlayers + " max.");
        }
    }
}
