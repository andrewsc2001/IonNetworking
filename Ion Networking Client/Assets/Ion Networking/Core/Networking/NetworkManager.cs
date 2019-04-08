using IonClient.Core.Networking.Tools;
using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

namespace IonClient.Core.Networking
{
    public static class NetworkManager
    {
        //Server settings
        public static readonly string ServerIP = "127.0.0.1";
        public static readonly int Port = 35565;

        //Instanced variables
        public static bool isConnected;

        //Network Objects
        public static TcpClient Socket { get; private set; }

        private static NetworkStream _networkStream;
        private static StreamReader _streamReader;
        private static StreamWriter _streamWriter;

        private static byte[] asyncBuff;


        public static void Init()
        {
            Debug.Log("Initializing NetworkManager...");
            
            ConfigureTCP(); //Set up TcpSocket for connection to the server.
        }

        /////////////////Initialization Methods

        private static void ConfigureTCP()
        {
            //Configure Socket
            Socket = new TcpClient();
            Socket.ReceiveBufferSize = 4096;
            Socket.SendBufferSize = 4096;
            Socket.NoDelay = false;
            Array.Resize(ref asyncBuff, 8192);
        }

        public static void Connect(string IP, int PORT)
        {
            if (Socket.Connected || isConnected)
                throw new InvalidOperationException("Cannot Connect to Server: Already Connected!");

            //Connect to Server
            Debug.Log("Connecting...");
            Socket.BeginConnect(IP, PORT, new AsyncCallback(OnConnected), Socket);

        }

        /////////////////Networking/Async Methods

        private static void OnConnected(IAsyncResult result)
        {
            if (Socket != null)
            {
                Socket.EndConnect(result);
                if (Socket.Connected == false)
                {
                    isConnected = false;
                    Debug.Log("Connection Failed.");
                    return;
                }
                else
                {
                    Debug.Log("Connected to Game Server!");
                    Socket.NoDelay = true;
                    _networkStream = Socket.GetStream();
                    _networkStream.BeginRead(asyncBuff, 0, 8192, OnRecieve, null);
                    isConnected = true;
                    NetworkController.Singleton.OnConnected();
                }
            }
        }

        private static void OnRecieve(IAsyncResult result)
        {
            if (Socket != null)
            {
                int byteArray = _networkStream.EndRead(result);
                byte[] RawData = null;
                Array.Resize(ref RawData, byteArray);
                Buffer.BlockCopy(asyncBuff, 0, RawData, 0, byteArray);

                if (byteArray == 0)
                {
                    Debug.Log("You were disconnected from the server.");
                    Socket.Close();
                    return;
                }

                if (Socket == null)
                    return;

                _networkStream.BeginRead(asyncBuff, 0, 8192, OnRecieve, null);

                //Handle Bytes
                //Split data into packets.
                byte[][] SplitData = PacketSplitter.SplitBytes(RawData);


                //Add all packets to queue.
                for (int index = 0; index < SplitData.Length; index++)
                {
                    PacketHandler.AddToQueue(SplitData[index]);
                }
            }
        }

        //Sends data to the server over TCP.
        public static void SendToServer(byte[] data)
        {
            if (Socket == null)
                throw new InvalidOperationException("Cannot send data when Socket has not been initialized!");

            if (data.Length > Socket.SendBufferSize)
                throw new ArgumentException("Cannot send data block larger than Socket.SendBufferSize!");

            if (!Socket.Connected)
                throw new InvalidOperationException("Cannot send data when not connected!");

            if (_networkStream == null)
                throw new InvalidOperationException("Cannot send data when _networkStream is null!");

            //Add length of packet to beginning of it. This allows the client to separate two packets if they get stuck together.
            byte[] send = new byte[data.Length + 1];

            send[0] = (byte)(data.Length + 1);
            Buffer.BlockCopy(data, 0, send, 1, data.Length);

            _networkStream.Write(send, 0, send.Length);
        }
    }
}
