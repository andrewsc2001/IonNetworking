using IonServer.Engine.Core.Networking.Tools;
using System;
using System.Collections;
using System.Net.Sockets;

namespace IonServer.Engine.Core.Networking
{
    public class Client
    {
        //Identifiers
        
        public byte Index { get; private set; }
        public string IP { get; private set; }
        public bool Connected { get; private set; }
        public bool UseNoDelay { get; private set; }

        private TcpClient _tcpSocket;
        private NetworkStream _networkStream;
        private byte[] _readBuffer;

        public void Start(byte Index, string IP, bool UseNoDelay, TcpClient socket)
        {
            Connected = true;
            Console.WriteLine(IP + " has been assigned to Index: " + Index);

            this.Index = Index;
            this.IP = IP;
            this._tcpSocket = socket;

            UpdateConfiguration();
            BeginRead();
            SendPacketTable();
        }

        //////////////////////////Public Methods

        public void Send(byte[] data) //Send data over TCP
        {
            if(data.Length > NetworkManager.ClientSocketSendBufferSize)
            {
                //Trying to send more than buffer size.
            }

            if (_tcpSocket == null)
            {
                Console.WriteLine("Tried to send data to client:" + Index + ", but Socket is null!");
                return;
            }

            if (!_tcpSocket.Connected)
            {
                Console.WriteLine("Tried to send data to client:" + Index + ", but Socket is not connected!");
                return;
            }

            if (_networkStream == null)
            {
                Console.WriteLine("Tried to send data to client:" + Index + ", but the stream was null!");
                return;
            }

            //Add length of packet to beginning of it. This allows the client to separate two packets if they get stuck together.
            byte[] send = new byte[data.Length + 1];

            send[0] = (byte)(data.Length + 1);
            Buffer.BlockCopy(data, 0, send, 1, data.Length);

            _networkStream.Write(send, 0, send.Length);
        }

        //Set whether the TCP socket will use a delay
        public void SetUseNoDelay(bool NoDelay)
        {
            if (Connected)
            {
                _tcpSocket.NoDelay = NoDelay;
            }
        }

        //Configures the TCP socket and other internal values based on the NetworkManager's settings.
        public void UpdateConfiguration()
        {
            _tcpSocket.SendBufferSize = NetworkManager.ClientSocketSendBufferSize;
            _tcpSocket.ReceiveBufferSize = NetworkManager.ClientSocketRecieveBufferSize;
            _networkStream = _tcpSocket.GetStream();
            Array.Resize(ref _readBuffer, _tcpSocket.ReceiveBufferSize);
        }

        //////////////////////////Networking/Async Methods
        
        private void OnRecieveData(IAsyncResult result)
        {
            try
            {
                //Where data will be stored in when pulled from the stream.
                byte[] rawData;

                if (_tcpSocket == null)
                    return;
                lock (_tcpSocket)
                {
                    if (_networkStream == null)
                        return;
                    lock (_networkStream)
                    {
                        //Length of the data recieved in the stream
                        int messageLength = _networkStream.EndRead(result);

                        //Disconnection, run CloseConnection
                        if(messageLength == 0)
                        {
                            CloseConnection();
                            return;
                        }

                        //Dump read into rawData
                        rawData = new byte[messageLength];
                        Buffer.BlockCopy(_readBuffer, 0, rawData, 0, messageLength);

                        //Start listening for data again
                        BeginRead();
                    }
                }

                //Split data into packets.
                byte[][] SplitData = PacketSplitter.SplitBytes(rawData);
                //Add all packets to queue.
                for (int index = 0; index < SplitData.Length; index++)
                {
                    PacketHandler.QueuePacket(this, SplitData[index]);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                CloseConnection();
                return;
            }
        }

        //////////////////////////Internal Methods

        //Sends the packet table to the client
        private void SendPacketTable()
        {
            if (!PacketManager.Locked)
            {
                Console.WriteLine("Cannot send packet table to client " + Index + " until PacketManager has been locked!");
                return;
            }

            PacketBuilder pb = new PacketBuilder("SyncPacketTable");

            pb.Write((byte)(PacketManager._headersToNames.Count - 1)); //count - 1 because it will not send the data for SyncPacketTable.

            foreach(DictionaryEntry pair in PacketManager._headersToNames)
            {
                if ((string)pair.Value == "SyncPacketTable") //SyncPacketTable is the only hard-coded packet type on the client. It doesnt need to be queued.
                    continue;
                pb.Write((byte)pair.Key);
                pb.Write((string)pair.Value);
            }

            Console.WriteLine("Sending packet table to client " + Index);
            Send(pb.GetPacket());
        }

        //Shortens the BeginRead line.
        private void BeginRead()
        {
            _networkStream.BeginRead(_readBuffer, 0, NetworkManager.ClientSocketRecieveBufferSize, OnRecieveData, null);
        }

        public void CloseConnection()
        {
            if (_tcpSocket == null)
            {
                Console.WriteLine("Cannot disconnect client " + Index + ", because it is not connected!");
                return;
            }
            
            lock (_tcpSocket)
            {
                if (_networkStream == null)
                    return;
                lock (_networkStream)
                {
                    try
                    {
                        _tcpSocket.Close();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.Error.WriteLine("Did not properly disconnect Client " + Index + "!");
                    }


                    _tcpSocket = null;


                    Console.WriteLine("Client " + Index + " disconnected.");

                    IP = null;
                    _networkStream = null;
                    _readBuffer = null;
                    Connected = false;
                }
            }
        }

        //////////////////////////Static Methods

        public static void ParseIPString(string source, ref string IP, ref int port)
        {
            string[] split = source.Split(':');
            IP = split[0];
            port = int.Parse(split[1]);
        }
    }
}
