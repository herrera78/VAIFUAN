using Havir.Sockets.Entities;
using Havir.Sockets.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havir.Sockets.ServerTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new SocketServer<UnityActionMessage, ServerActionMessage>();
            server.Start(3322, "localhost");
            server.OnRecivedMessage += OnRecivedMessageHandler;
            while (true)
            {
                Console.Write("Message to clients: ");
                var message = new UnityActionMessage();
                message.MessageType = MessageTypeEnum.Success;
                message.Message = Console.ReadLine();
                server.SendMessage(message);
            }
        }

        private static void OnRecivedMessageHandler(ServerActionMessage message)
        {
            Console.WriteLine("Manejo del mensaje por parte de la aplicación servidor: " + message);
        }
    }
}
