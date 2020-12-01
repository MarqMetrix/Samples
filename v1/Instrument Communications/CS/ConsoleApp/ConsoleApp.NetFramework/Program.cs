using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            MainApplication.MainAsync().GetAwaiter().GetResult();

#if DEBUG
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
#endif
        }
    }
}
