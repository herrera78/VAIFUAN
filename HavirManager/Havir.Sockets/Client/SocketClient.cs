using Havir.Sockets.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havir.Sockets.Client
{
    public class SocketClient<SendObject, RecibeObject> : IDisposable
        where SendObject : BaseMessage
        where RecibeObject : BaseMessage
    {
        public delegate void RecivedMessage(RecibeObject message);
        public RecivedMessage OnRecivedMessage;

        private Socket senderSock;
        byte[] bytes = new byte[10240];

        public void Dispose()
        {
            try
            {
                // Disables sends and receives on a Socket. 
                senderSock.Shutdown(SocketShutdown.Both);

                //Closes the Socket connection and releases all resources 
                senderSock.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                throw;
            }
        }

        public void Connect(int port = 4421, string ip = null)
        {
            try
            {
                // Create one SocketPermission for socket access restrictions  
                SocketPermission permission = new SocketPermission(
                    NetworkAccess.Connect,    // Connection permission  
                    TransportType.Tcp,        // Defines transport types  
                    "",                       // Gets the IP addresses  
                    SocketPermission.AllPorts // All ports  
                    );

                // Ensures the code to have permission to access a Socket  
                permission.Demand();

                if (string.IsNullOrWhiteSpace(ip))
                {
                    ip = "127.0.0.1";
                }
                IPAddress ipAddr = System.Net.IPAddress.Parse(ip);
                // Gets first IP address associated with a localhost  

                // Creates a network endpoint  
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

                // Create one Socket object to setup Tcp connection  
                senderSock = new Socket(
                    ipAddr.AddressFamily,// Specifies the addressing scheme  
                    SocketType.Stream,   // The type of socket   
                    ProtocolType.Tcp     // Specifies the protocols   
                    );

                senderSock.NoDelay = false;   // Using the Nagle algorithm  

                // Establishes a connection to a remote host  
                senderSock.Connect(ipEndPoint);
                Console.WriteLine("Socket connected to " + senderSock.RemoteEndPoint.ToString());
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                throw;
            }

        }
        public void SendMessage(SendObject message)
        {
            try
            {
                var jsonObject = JsonConvert.SerializeObject(message);
                byte[] msg = Encoding.Unicode.GetBytes(jsonObject + "<EndMessage>");
                /// 
                // Sends data to a connected Socket.  
                int bytesSend = senderSock.Send(msg);
                //ReceiveDataFromServer();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                throw;
            }
        }

        public void ReceiveDataFromServer()
        {
            try
            {
                // Receives data from a bound Socket. 
                int bytesRec = senderSock.Receive(bytes);

                // Converts byte array to string 
                String theMessageToReceive = Encoding.Unicode.GetString(bytes, 0, bytesRec);

                // Continues to read the data till data isn't available 
                while (senderSock.Available > 0)
                {
                    bytesRec = senderSock.Receive(bytes);
                    theMessageToReceive += Encoding.Unicode.GetString(bytes, 0, bytesRec);
                }
                Console.WriteLine("The server reply: " + theMessageToReceive);
                foreach (var str in theMessageToReceive.Split(new string[] { "<EndMessage>" }, StringSplitOptions.None))
                {
                    if (string.IsNullOrWhiteSpace(str))
                        continue;
                    try
                    {
                        var message = JsonConvert.DeserializeObject<RecibeObject>(str);
                        message.MessageType = MessageTypeEnum.Success;
                        if (OnRecivedMessage != null)
                            OnRecivedMessage(message);
                    }
                    catch (Exception ex)
                    {
                        var message = (RecibeObject)Activator.CreateInstance(typeof(RecibeObject), new object[] { });
                        message.MessageType = MessageTypeEnum.Error;
                        message.Message = ex.ToString();
                        Console.WriteLine(ex.ToString());
                        OnRecivedMessage(message);
                    }
                }
                ReceiveDataFromServer();
            }
            catch (Exception exc)
            {
                var message = (RecibeObject)Activator.CreateInstance(typeof(RecibeObject), new object[] { });
                message.MessageType = MessageTypeEnum.Error;
                message.Message = exc.ToString();
                Console.WriteLine(exc.ToString());
                OnRecivedMessage(message);
                throw;
            }
        }
    }
}