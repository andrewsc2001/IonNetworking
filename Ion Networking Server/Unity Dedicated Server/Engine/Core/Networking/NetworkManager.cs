using System;
using System.Net;
using System.Net.Sockets;

namespace IonServer.Engine.Core.Networking
{
    public static class NetworkManager
    {
        //settings
        public static int Port { get; private set; }
        public static byte MaxPlayers { get; private set; }
        public static int ClientSocketReceiveBufferSize { get; private set; }
        public static int ClientSocketSendBufferSize { get; private set; }
        public static bool UseNoDelay { get; private set; }

        private static Client[] _clientsList;

        private static TcpListener _serverSocket;

        public static void Init(int Port, byte MaxPlayers, int ClientSocketReceiveBufferSize=4096, int ClientSocketSendBufferSize=4096, bool UseNoDelay=false)
        {
            //Networking
            Console.WriteLine("Initializing NetworkManager");

            NetworkManager.Port = Port;
            NetworkManager.MaxPlayers = MaxPlayers;
            NetworkManager.ClientSocketReceiveBufferSize = ClientSocketReceiveBufferSize;
            NetworkManager.ClientSocketSendBufferSize = ClientSocketSendBufferSize;
            NetworkManager.UseNoDelay = UseNoDelay;

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
                Client client = GetClientFromIndex(index);
                
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
            _clientsList = new Client[MaxPlayers];
            for (int i = 0; i < MaxPlayers; i++)
            {
                _clientsList[i] = new Client();
            }
        }

        //////////////////Public Methods

        //Returns a client by its endpoint
        public static Client GetClientFromEndPoint(IPEndPoint ep) //Used primarily by OnUDPRecieve to determine the sender.
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
        public static Client GetClientFromIndex(byte Index)
        {
            if (Index < 0 || Index > MaxPlayers)
                return null;

            lock (_clientsList)
            {
                return _clientsList[Index];
            }
        }

        //Starts listening for new connections
        public static void StartListener() //Starts _listener thread to wait for connections
        {
            if (!PacketManager.Locked)
            {
                Console.WriteLine("Cannot start listener until PacketManager has been locked!");
                return;
            }

            Console.WriteLine("Starting ServerSocket");

            _serverSocket = new TcpListener(IPAddress.Any, Port);
            _serverSocket.Start();

            _serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
        }

        //Stops listening for new connections
        public static void StopListener() //Stops _listener so no NEW clients can connect
        {
            Console.WriteLine("Stopping ServerSocket");
            _serverSocket.Stop();
            _serverSocket = null;
        }

        //////////////////Change settings

        //Changes the port that the server listens on
        public static void SetPort(int port)
        {
            if(_serverSocket != null)//Server is already listening, can't change ports.
            {
                Console.Error.WriteLine("Cannot change port when server is listening for clients!");
                return;
            }

            NetworkManager.Port = port;
        }

        //Changes the maximum number of players
        public static void SetMaxPlayers(byte MaxPlayers)
        {
            if(_serverSocket != null)
            {
                Console.WriteLine("Cannot change the maximum number of players while the server is running!");
                return;
            }

            NetworkManager.MaxPlayers = MaxPlayers;
            InitClientSlots();
        }

        //Set the read buffer for clients
        public static void SetClientSocketReceiveBufferSize(int size)
        {
            if (_serverSocket != null)
            {
                Console.WriteLine("Cannot change client read buffer size while the server is running!");
                return;
            }

            for (int index = 0; index < MaxPlayers; index++)
            {
                _clientsList[index].UpdateConfiguration();
            }
        }

        //Set the write buffer for clients
        public static void SetClientSocketWriteBufferSize(int size)
        {
            if (_serverSocket != null)
            {
                Console.WriteLine("Cannot change client write buffer size while the server is running!");
                return;
            }

            for (int index = 0; index < MaxPlayers; index++)
            {
                _clientsList[index].UpdateConfiguration();
            }
        }

        //Set NoDelay for all clients
        public static void SetUseNoDelay(bool NoDelay)
        {
            for(int index = 0; index < MaxPlayers; index++)
            {
                _clientsList[index].SetUseNoDelay(NoDelay);
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
                    if (!_clientsList[i].Connected) //If client has no connection
                    {
                        Console.WriteLine("Incoming Connection from " + _clientsList[i].IP + " || Index: " + i);
                        _clientsList[i].Start(i, client.Client.RemoteEndPoint.ToString().Split(':')[0], UseNoDelay, client); //Client has been configured and is ready to communicate.
                        
                        return; //Prevents a single connection from taking more than one Client.
                    }
                }

                Console.WriteLine("There was a connection from " + client.Client.RemoteEndPoint.ToString().Split(':')[0] + ", but the server is full!");
                client.Close();
            }
        }
    }
}
