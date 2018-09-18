using Havir.Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Havir.Main
{
    class Program
    {
        protected static TimeSpan receiveTimeout = TimeSpan.FromSeconds(10);

        static void Main(string[] args)
        {
            Debug.WriteLine("Iniciando componente v0.2");
            var manager = new HavirController();
            manager.Start();
            Console.ReadLine();
        }

        
    }
}
