using System;

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
