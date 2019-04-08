using System;
using System.Net;
using System.Net.Sockets;

namespace IonNetworking.Engine.Core.Networking
{
    public static class NetworkManager
    {
        //settings
        public static int Port { get; private set; }
        public static byte MaxPlayers { get; private set; }
        public static int DefaultReceiveBufferSize { get; private set; }
        public static int DefaultSendBufferSize { get; private set; }
        public static bool DefaultUseNoDelay { get; private set; }

        private static IonClient[] _clientsList;

        private static TcpListener _serverSocket;

        public static void Init(int Port, byte MaxPlayers, int DefaultReceiveBufferSize=4096, int DefaultSendBufferSize=4096, bool DefaultUseNoDelay=false)
        {
            if (Port < 0)
                throw new ArgumentOutOfRangeException("Invalid Port Number");
            if (Port < 1024)
                throw new ArgumentOutOfRangeException("Ports 0-1023 are reserved for system use");

            //Networking
            Console.WriteLine("Initializing NetworkManager");

            NetworkManager.Port = Port;
            NetworkManager.MaxPlayers = MaxPlayers;
            NetworkManager.DefaultReceiveBufferSize = DefaultReceiveBufferSize;
            NetworkManager.DefaultSendBufferSize = DefaultSendBufferSize;
            NetworkManager.DefaultUseNoDelay = DefaultUseNoDelay;

            InitClientSlots();//Set up client slots so that connections can be passed off to client objects
        }

        public static void Stop()
        {
            Console.WriteLine("Stopping NetworkManager");

            //Stop listening
            StopListener();

            //Disconnect all clients
            for (byte index = 0; index < MaxPlayers; index++)
            {
                IonClient client = GetClientFromIndex(index);
                
                if(client.Connected)
                {
                    client.CloseConnection();
                }
            }
        }

        //////////////////Internal Methods
        private static void InitClientSlots() //Initializes client slots so that they can be filled with connections.
        {
            Console.WriteLine("Initializing Client Objects");
            _clientsList = new IonClient[MaxPlayers];
            for (int i = 0; i < MaxPlayers; i++)
            {
                _clientsList[i] = new IonClient();
            }
        }

        //////////////////Public Methods

        //Returns a client by its endpoint
        public static IonClient GetClientFromEndPoint(IPEndPoint ep) //Used primarily by OnUDPRecieve to determine the sender.
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (ep.Address.ToString() == _clientsList[i].IP)
                {
                    return _clientsList[i];
                }
            }

            Console.WriteLine("Something requested a nonexistent client with IP: " + ep.ToString());

            return null;
        }

        //Returns a client object by its Index
        public static IonClient GetClientFromIndex(byte Index)
        {
            if (Index < 0 || Index > MaxPlayers)
                throw new ArgumentOutOfRangeException("Index is out of range!");

            lock (_clientsList)
            {
                return _clientsList[Index];
            }
        }

        //Starts listening for new connections
        public static void StartListener() //Starts _listener thread to wait for connections
        {
            if (!PacketManager.Locked)
                throw new InvalidOperationException("Cannot start listening for clients until PacketManager is locked!");

            Console.WriteLine("Starting ServerSocket");

            _serverSocket = new TcpListener(IPAddress.Any, Port);
            _serverSocket.Start();

            _serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
        }

        //Stops listening for new connections
        public static void StopListener() //Stops _listener so no NEW clients can connect
        {
            if (_serverSocket == null)
                throw new InvalidOperationException("Can't stop listening when server is not listening for clients!");

            Console.WriteLine("Stopping ServerSocket");
            _serverSocket.Stop();
            _serverSocket = null;
        }

        //////////////////Change settings

        //Changes the port that the server listens on
        public static void SetPort(int Port)
        {
            if(_serverSocket != null)//Server is already listening, can't change ports.
            {
                Console.Error.WriteLine("Cannot change port when server is listening for clients!");
                return;
            }

            NetworkManager.Port = Port;
        }

        //Changes the maximum number of players
        public static void SetMaxPlayers(byte MaxPlayers)
        {
            if (_serverSocket != null)
                throw new InvalidOperationException("Cannot change MaxPlayers while server is open!");

            NetworkManager.MaxPlayers = MaxPlayers;
            InitClientSlots();
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
                    if (!_clientsList[i].Connected) //If client has no connection
                    {
                        Console.WriteLine("Incoming Connection from " + _clientsList[i].IP + " || Index: " + i);
                        _clientsList[i].Start(i, client.Client.RemoteEndPoint.ToString().Split(':')[0], DefaultUseNoDelay, client); //Client has been configured and is ready to communicate.
                        
                        return; //Prevents a single connection from taking more than one Client.
                    }
                }

                Console.WriteLine("There was a connection from " + client.Client.RemoteEndPoint.ToString().Split(':')[0] + ", but the server is full!");
                client.Close();
            }
        }
    }
}
