using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havir.GraphProcessor
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var processor = new GraphProcessor();
                processor.Run().Wait();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Preprocesamiento terminado");
            Console.ReadLine();
        }

    }
}
