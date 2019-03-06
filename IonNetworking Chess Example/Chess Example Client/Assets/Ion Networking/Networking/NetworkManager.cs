using IonClient.Networking.Tools;
using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

namespace IonClient.Networking
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance; //Singleton

        //Server settings
        public static readonly string ServerIP = "127.0.0.1";
        public static readonly int Port = 35565;

        //Instanced variables
        public bool isConnected;

        //Network Objects
        private TcpClient _tcpSocket;
        private NetworkStream _networkStream;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;

        private byte[] asyncBuff;


        private void Start()
        {
            Debug.Log("Starting NetworkManager...");

            PacketInitialization.Init(); //Initialize PacketTable.

            ConfigureTCP(); //Set up TcpSocket for connection to the server.
            ConnectToGameServer(ServerIP, Port);
        }

        private void AfterConnected()
        {
            SendToServer(new byte[] { PacketActionList.ECHO });
        }

        /////////////////Initialization Methods

        private void Awake()
        {
            Instance = this; //Set Singleton
        }

        private void ConfigureTCP()
        {
            //Configure Socket
            _tcpSocket = new TcpClient();
            _tcpSocket.ReceiveBufferSize = 4096;
            _tcpSocket.SendBufferSize = 4096;
            _tcpSocket.NoDelay = false;
            Array.Resize(ref asyncBuff, 8192);
        }



        private void Update()
        {
            //Handle all queued packets from the server.
            PacketHandler.ProcessQueue();
        }

        private void ConnectToGameServer(string IP, int PORT)
        {

            if (_tcpSocket.Connected || isConnected)
            {
                Debug.LogError("Already connected!");
                return; //Already connected
            }

            //Connect to Server
            Debug.Log("Connecting...");
            _tcpSocket.BeginConnect(IP, PORT, new AsyncCallback(OnConnected), _tcpSocket);

        }

        /////////////////Networking/Async Methods

        private void OnConnected(IAsyncResult result)
        {
            if (_tcpSocket != null)
            {
                _tcpSocket.EndConnect(result);
                if (_tcpSocket.Connected == false)
                {
                    isConnected = false;
                    Debug.Log("Connection Failed.");
                    return;
                }
                else
                {
                    isConnected = true;
                    Debug.Log("Connected to Game Server!");
                    _tcpSocket.NoDelay = true;
                    _networkStream = _tcpSocket.GetStream();
                    _networkStream.BeginRead(asyncBuff, 0, 8192, OnRecieve, null);
                    AfterConnected();
                }
            }
        }

        void OnRecieve(IAsyncResult result)
        {
            if (_tcpSocket != null)
            {
                int byteArray = _networkStream.EndRead(result);
                byte[] RawData = null;
                Array.Resize(ref RawData, byteArray);
                Buffer.BlockCopy(asyncBuff, 0, RawData, 0, byteArray);

                if (byteArray == 0)
                {
                    Debug.Log("You were disconnected from the server.");
                    _tcpSocket.Close();
                    return;
                }

                if (_tcpSocket == null)
                {
                    return;
                }

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
        public void SendToServer(byte[] data)
        {
            if (_tcpSocket != null)
            {
                if (_tcpSocket.Connected)
                {
                    if (_networkStream != null)
                    {
                        byte length = (byte)(data.Length + 1);

                        byte[] send = new byte[data.Length + 1];
                        send[0] = length;
                        for (int index = 0; index < data.Length; index++)
                        {
                            send[index + 1] = data[index];
                        }

                        _networkStream.Write(send, 0, send.Length);
                        return;
                    }
                    Debug.LogError("Tried to send data to server, but the stream was null!");
                    return;
                }
                Debug.LogError("Tried to send data to server, but Socket is not connected!");
                return;
            }
            Debug.LogError("Tried to send data to server, but Socket is null!");
            return;
        }
    }
}
