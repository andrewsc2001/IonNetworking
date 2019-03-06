using System;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using IonServer.Content.Core;

namespace IonServer.Engine.Core.Networking
{
    class NetworkManager
    {
        //settings
        public static byte MaxPlayers = 4; //NOTE: Byte means that there can only be 0-255 players.
        public static int Port = 35565;

        private static Client[] _clientsList;

        private static TcpListener _serverSocket;

        public static void Start()
        {
            //Networking
            Console.WriteLine("Initializing NetworkManager");

            LoadSettings();
            PacketInitialization.Init();//Init packet types.
            InitClientSlots();//Set up client slots so that connections can be passed off to client objects

            StartListener();//Begin listening for incoming connections

        }

        public static void Stop()
        {
            Console.WriteLine("Stopping NetworkManager");

            //Stop listening
            StopListener();

            //Disconnect all clients
            for (byte index = 0; index < MaxPlayers; index++)
            {
                Client client = GetClientFromIndex(index);
                
                if(client._tcpSocket != null)
                {
                    client.CloseConnection();
                }
            }
        }

        //////////////////Internal Methods

        private static void LoadSettings()
        {
            Console.WriteLine("Loading config data for NetworkManager");

            byte.TryParse(ConfigurationManager.AppSettings["maxplayers"], out MaxPlayers);
            int.TryParse(ConfigurationManager.AppSettings["port"], out Port);
        }
           
        private static void InitClientSlots() //Initializes client slots so that they can be filled with connections.
        {
            Console.WriteLine("Initializing Client Objects");
            _clientsList = new Client[MaxPlayers];
            for (int i = 0; i < MaxPlayers; i++)
            {
                _clientsList[i] = new Client();
            }
        }

        private static void StartListener() //Starts _listener thread to wait for connections
        {
            Console.WriteLine("Starting ServerSocket");

            _serverSocket = new TcpListener(IPAddress.Any, Port);
            _serverSocket.Start();

            _serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
        }

        private static void StopListener() //Stops _listener so no NEW clients can connect
        {
            Console.WriteLine("Stopping ServerSocket");
            _serverSocket.Stop();
            _serverSocket = null;
        }

        //////////////////Public Methods

        public static Client GetClientFromEndPoint(IPEndPoint ep) //Used primarily by OnUDPRecieve to determine the sender.
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (ep.Address.ToString() == _clientsList[i].ip)
                {
                    return _clientsList[i];
                }
            }

            Console.WriteLine("Something requested a nonexistent client with IP: " + ep.ToString());

            return null;
        }

        public static Client GetClientFromIndex(byte Index)
        {
            if (Index < 0 || Index > MaxPlayers)
                return null;

            lock (_clientsList)
            {
                return _clientsList[Index];
            }
        }

        //////////////////Networking/Async Methods

        private static void OnClientConnect(IAsyncResult result) //Async Function. Called when ServerSocket recieves a new connection.
        {
            if (_serverSocket == null)
                return;

            TcpClient client = _serverSocket.EndAcceptTcpClient(result); //Stores connection in TcpClient
            client.NoDelay = false;
            _serverSocket.BeginAcceptTcpClient(OnClientConnect, null); //Reactivates ServerSocket so it can listen for new clients.
            //Assign connection to client slot for proper handling.
            lock (_clientsList) //Get a lock on the clients list
            {
                for (byte i = 0; i < MaxPlayers; i++)
                {
                    if (_clientsList[i]._tcpSocket == null) //If client has no connection
                    {

                        //Configure Client object
                        _clientsList[i]._tcpSocket = client;
                        _clientsList[i].index = i;
                        _clientsList[i].ip = client.Client.RemoteEndPoint.ToString().Split(':')[0];
                        Console.WriteLine("Incoming Connection from " + _clientsList[i].ip + " || Index: " + i);
                        _clientsList[i].Start(); //Client has been configured and is ready to communicate.


                        return; //Prevents a single connection from taking more than one Client.
                    }
                }

                Console.WriteLine("There was a connection from " + client.Client.RemoteEndPoint.ToString().Split(':')[0] + ", but the server is full!");
                client.Close();
            }
        }
    }
}
