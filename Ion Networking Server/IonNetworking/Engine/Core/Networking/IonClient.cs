using IonNetworking.Engine.Core.Networking.Tools;
using System;
using System.Collections;
using System.Net.Sockets;

namespace IonNetworking.Engine.Core.Networking
{
    public class IonClient
    {
        //Identifiers
        
        public byte Index { get; private set; }
        public string IP { get; private set; }
        public bool Connected { get; private set; }
        public TcpClient Client { get; private set; }

        private NetworkStream _networkStream;
        private byte[] _readBuffer;

        public void Start(byte Index, string IP, bool UseNoDelay, TcpClient Client)
        {
            Connected = true;
            Console.WriteLine(IP + " has been assigned to Index: " + Index);

            this.Index = Index;
            this.IP = IP;
            this.Client = Client;

            SetClientToDefaultConfiguration();
            ConfigureNetworkStream();
            BeginRead();
            SendPacketTable();
        }

        //////////////////////////Public Methods

        public void Send(byte[] data) //Send data over TCP
        {
            if (Client == null)
                throw new InvalidOperationException("Cannot send data when IonClient object is not attached to a client!");

            if (data.Length > Client.SendBufferSize)
                throw new ArgumentException("Cannot send data block larger than Client.SendBufferSize!");

            if (!Client.Connected)
                throw new InvalidOperationException("Cannot send data when Client is not connected!");

            if (_networkStream == null)
                throw new InvalidOperationException("Cannot send data when _networkStream is null!");

            //Add length of packet to beginning of it. This allows the client to separate two packets if they get stuck together.
            byte[] send = new byte[data.Length + 1];

            send[0] = (byte)(data.Length + 1);
            Buffer.BlockCopy(data, 0, send, 1, data.Length);

            _networkStream.Write(send, 0, send.Length);
        }
        
        //Configures the TCP socket and other internal values based on the NetworkManager's settings.
        public void SetClientToDefaultConfiguration()
        {
            Client.SendBufferSize = NetworkManager.DefaultSendBufferSize;
            Client.ReceiveBufferSize = NetworkManager.DefaultReceiveBufferSize;
        }

        //Pulls _networkStream from Client.
        private void ConfigureNetworkStream()
        {
            _networkStream = Client.GetStream();
            Array.Resize(ref _readBuffer, Client.ReceiveBufferSize);
        }

        //////////////////////////Networking/Async Methods
        
        private void OnRecieveData(IAsyncResult result)
        {
            try
            {
                //Where data will be stored in when pulled from the stream.
                byte[] rawData;

                if (Client == null)
                    return;
                lock (Client)
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
                throw new InvalidOperationException("Cannot send PacketTable before it has been locked!");
            
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
            _networkStream.BeginRead(_readBuffer, 0, NetworkManager.DefaultReceiveBufferSize, OnRecieveData, null);
        }

        //Closes connection with the client
        public void CloseConnection()
        {
            if (Client == null)
                throw new InvalidOperationException("Cannot disconnect from client when client isn't connected!");
            
            lock (Client)
            {
                if (_networkStream == null)
                    return;
                lock (_networkStream)
                {
                    try
                    {
                        Client.Close();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.Error.WriteLine("Did not properly disconnect Client " + Index + "!");
                    }


                    Client = null;


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

            if (split.Length != 2)
                throw new ArgumentException("Invalid IP String");

            IP = split[0];
            port = int.Parse(split[1]);
        }
    }
}
