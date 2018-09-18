using Havir.Api.Speech;
using Havir.Sockets.Entities;
using Havir.Sockets.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Havir.Manager
{
    public class HavirController
    {
        private SpeechRegonizerManager _speechManager;
        private SocketServer<UnityActionMessage, ServerActionMessage> _server;

        public void Start()
        {
            _InitSocketServer();
            _InitSpeechManager();

        }

        private void _InitSocketServer()
        {
            _server = new SocketServer<UnityActionMessage, ServerActionMessage>();
            _server.Start(4224);
            _server.OnRecivedMessage += OnRecivedMessageHandler;
        }

        private void OnRecivedMessageHandler(ServerActionMessage message)
        {
            Console.WriteLine($"Message fom client. Pause: {message.Pause} Resume: {message.Resume}");
            if (message.Resume)
                _speechManager.Resume();
            if (message.Pause)
                _speechManager.Pause();
        }

        //Create an access to this controller to toggle the booleans
        private void _InitSpeechManager()
        {
            _speechManager = new SpeechRegonizerManager();
            _speechManager.InitRecognizer(true, true);
            _speechManager.OnEmitMessage += OnEmitMessageHandler;

        }

        private void OnEmitMessageHandler(UnityActionMessage message)
        {
            Debug.WriteLine("Mensaje emitido: " + message);
            _server.SendMessage(message);
        }
    }
}
