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

namespace Havir.Sockets.Server
{
    public class SocketServer<SendObject, RecibeObject> : IDisposable
        where SendObject : BaseMessage
        where RecibeObject : BaseMessage
    {
        public delegate void RecivedMessage(RecibeObject message);
        public RecivedMessage OnRecivedMessage;

        SocketPermission permission;
        Socket sListener;
        IPEndPoint ipEndPoint;
        List<Socket> handlers = new List<Socket>();

        private bool _isListening;

        public bool IsListening
        {
            get { return _isListening; }
        }

        public SocketServer()
        {
            _isListening = false;
        }

        public void Start(int port = 4421, string ip = null)
        {
            try
            {
                // Creates one SocketPermission object for access restrictions
                permission = new SocketPermission(
                NetworkAccess.Accept,     // Allowed to accept connections 
                TransportType.Tcp,        // Defines transport types 
                "",                       // The IP addresses of local host 
                SocketPermission.AllPorts // Specifies all ports 
                );

                // Listening Socket object 
                sListener = null;

                // Ensures the code to have permission to access a Socket 
                permission.Demand();

                if (string.IsNullOrWhiteSpace(ip))
                {
                    ip = "127.0.0.1";
                }
                IPAddress ipAddr = System.Net.IPAddress.Parse(ip);

                // Creates a network endpoint 
                ipEndPoint = new IPEndPoint(ipAddr, port);

                // Create one Socket object to listen the incoming connection 
                sListener = new Socket(
                    ipAddr.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp
                    );

                // Associates a Socket with a local endpoint 
                sListener.Bind(ipEndPoint);

                Console.WriteLine("Server started.");
                Listen();
            }
            catch (Exception exc) { Console.WriteLine(exc.ToString()); throw; }
        }


        public void Listen()
        {
            try
            {
                // Places a Socket in a listening state and specifies the maximum 
                // Length of the pending connections queue 
                sListener.Listen(10);
                // Begins an asynchronous operation to accept an attempt 
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                sListener.BeginAccept(aCallback, sListener);

                Console.WriteLine("Server is now listening on " + ipEndPoint.Address + " port: " + ipEndPoint.Port);
                _isListening = true;
            }
            catch (Exception exc) { Console.WriteLine(exc.ToString()); throw; }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = null;

            // A new Socket to handle remote host communication 
            Socket handler = null;
            try
            {
                // Receiving byte array 
                byte[] buffer = new byte[1024];
                // Get Listening Socket object 
                listener = (Socket)ar.AsyncState;
                // Create a new socket 
                handler = listener.EndAccept(ar);

                // Using the Nagle algorithm 
                handler.NoDelay = false;

                // Creates one object array for passing data 
                object[] obj = new object[2];
                obj[0] = buffer;
                obj[1] = handler;

                //Add handler to list
                handlers.Add(handler);

                // Begins to asynchronously receive data 
                handler.BeginReceive(
                    buffer,        // An array of type Byt for received data 
                    0,             // The zero-based position in the buffer  
                    buffer.Length, // The number of bytes to receive 
                    SocketFlags.None,// Specifies send and receive behaviors 
                    new AsyncCallback(ReceiveCallback),//An AsyncCallback delegate 
                    obj            // Specifies infomation for receive operation 
                    );

                // Begins an asynchronous operation to accept an attempt 
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                listener.BeginAccept(aCallback, listener);
            }
            catch (Exception exc) { Console.WriteLine(exc.ToString()); throw; }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            // Fetch a user-defined object that contains information 
            object[] obj = new object[2];
            obj = (object[])ar.AsyncState;
            // Received byte array 
            byte[] buffer = (byte[])obj[0];

            // A Socket to handle remote host communication. 
            var handler = (Socket)obj[1];

            try
            {

                // Received message 
                string content = string.Empty;


                // The number of bytes received. 
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    content += Encoding.Unicode.GetString(buffer, 0,
                        bytesRead);

                    // If message contains "<EndMessage>", finish receiving
                    if (content.IndexOf("<EndMessage>") > -1)
                    {
                        foreach (var str in content.Split(new string[] { "<EndMessage>" }, StringSplitOptions.None))
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
                    }
                    //else
                    //{
                    // Continues to asynchronously receive data
                    byte[] buffernew = new byte[1024];
                    obj[0] = buffernew;
                    obj[1] = handler;
                    handler.BeginReceive(buffernew, 0, buffernew.Length,
                        SocketFlags.None,
                        new AsyncCallback(ReceiveCallback), obj);
                    //}
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
                handler.Dispose();
                handlers.Remove(handler);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public void SendMessage(SendObject message)
        {
            try
            {
                var jsonObject = JsonConvert.SerializeObject(message);
                // Prepare the reply message 
                byte[] byteData =
                    Encoding.Unicode.GetBytes(jsonObject + "<EndMessage>");
                foreach (var handler in handlers)
                {
                    // Sends data asynchronously to a connected Socket 
                    handler.BeginSend(byteData, 0, byteData.Length, 0,
                        new AsyncCallback(SendCallback), handler);
                }
            }
            catch (Exception exc) { Console.WriteLine(exc.ToString()); throw; }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // A Socket which has sent the data to remote host 
                Socket handler = (Socket)ar.AsyncState;

                // The number of bytes sent to the Socket 
                int bytesSend = handler.EndSend(ar);
                Console.WriteLine(
                    "Sent {0} bytes to Client", bytesSend);
            }
            catch (Exception exc) { Console.WriteLine(exc.ToString()); throw; }
        }

        public void Dispose()
        {
            try
            {
                if (sListener.Connected)
                {
                    sListener.Shutdown(SocketShutdown.Receive);
                    sListener.Close();
                }
            }
            catch (Exception exc) { Console.WriteLine(exc.ToString()); throw; }
        }
    }
}
