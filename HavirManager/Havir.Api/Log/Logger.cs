using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havir.Api.Log
{
    public class Logger
    {
        private static bool _debug = ConfigurationManager.AppSettings["Debug"].Equals("true");
        public static void Write(string message)
        {
            Trace.WriteLine(message);
        }
        public static void Error(string message, Exception ex)
        {
            Trace.WriteLine(message);
        }
        public static void Debug(string message)
        {
            Trace.WriteLineIf(_debug, message);
        }
    }
}
