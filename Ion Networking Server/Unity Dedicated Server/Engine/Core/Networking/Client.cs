using IonServer.Engine.Core.Networking.Tools;
using System;
using System.Net.Sockets;

namespace IonServer.Engine.Core.Networking
{
    public class Client
    {
        //Identifiers
        
        public byte index;
        public string ip;

        public TcpClient _tcpSocket;
        private NetworkStream _networkStream;
        private byte[] _readBuffer;

        public void Start()
        {
            Console.WriteLine(ip + " has been assigned to Index: " + index);

            ConfigureTCP();
        }

        //////////////////////////Public Methods

        public void Send(byte[] data) //Send data over TCP
        {
            if (_tcpSocket != null)
            {
                if (_tcpSocket.Connected)
                {
                    if (_networkStream != null)
                    {

                        byte length = (byte) (data.Length + 1);

                        byte[] send = new byte[data.Length + 1];
                        send[0] = length;
                        for(int index = 0; index < data.Length; index ++)
                        {
                            send[index + 1] = data[index];
                        }

                        _networkStream.Write(send, 0, send.Length);
                        return;
                    }
                    Console.WriteLine("Tried to send data to client:" + index + ", but the stream was null!");
                    return;
                }
                Console.WriteLine("Tried to send data to client:" + index + ", but Socket is not connected!");
                return;
            }
            Console.WriteLine("Tried to send data to client:" + index + ", but Socket is null!");
            return;
        }

        //////////////////////////Networking/Async Methods

            /*
        private void OnRecieveData(IAsyncResult result)
        {
            try
            {

            }
            catch(Exception e)
            {
                
            }
        }*/
        
        private void OnRecieveData(IAsyncResult result)
        {
            try
            {
                if (_tcpSocket == null)
                    return;

                

                byte[] RawData = null;
                lock (_tcpSocket)
                {

                    if (_networkStream == null)
                        return;

                    lock (_networkStream)
                    {
                        if (!_tcpSocket.Connected)
                            return;
                        int readBytes = _networkStream.EndRead(result);



                        //if recieved no data, end connection.
                        if (readBytes <= 0)
                        {
                            CloseConnection();
                            return;
                        }

                        //Create new byte[] newBytes and copy contents of readBuff into it

                        Array.Resize(ref RawData, readBytes);
                        Buffer.BlockCopy(_readBuffer, 0, RawData, 0, readBytes);

                        //Reset stream
                        _networkStream.BeginRead(_readBuffer, 0, _tcpSocket.ReceiveBufferSize, OnRecieveData, null);
                    }
                }
                //Handle Bytes
                //Split data into packets.
                byte[][] SplitData = PacketSplitter.SplitBytes(RawData);
                //Add all packets to queue.
                for (int index = 0; index < SplitData.Length; index++)
                {
                    PacketHandler.QueuePacket(this, SplitData[index]);
                }         
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                CloseConnection();
                return;
            }
        }

        //////////////////////////Internal Methods

        private void ConfigureTCP()
        {
            _tcpSocket.SendBufferSize = 4096;
            _tcpSocket.ReceiveBufferSize = 4096;
            _networkStream = _tcpSocket.GetStream();
            Array.Resize(ref _readBuffer, _tcpSocket.ReceiveBufferSize);
            _networkStream.BeginRead(_readBuffer, 0, _tcpSocket.ReceiveBufferSize, OnRecieveData, null);
        }

        public void CloseConnection()
        {
            if (_tcpSocket == null)
            {
                Console.WriteLine("Cannot disconnect client " + index + ", because it is not connected!");
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
                        Console.Error.WriteLine("Did not properly disconnect Client " + index + "!");
                    }


                    _tcpSocket = null;


                    Console.WriteLine("Client " + index + " disconnected.");

                    ip = null;
                    _networkStream = null;
                    _readBuffer = null;
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
