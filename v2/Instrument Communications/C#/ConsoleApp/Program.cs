using MarqMetrix.InstrumentManagement;
using System;
using System.Threading.Tasks;
using ConsoleApp.Examples;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("IP Address: ");
            var ipAddress = Console.ReadLine();
            Console.Write("Port: ");
            var port = int.Parse(Console.ReadLine());

            var shortCodeRequest = await InstrumentClientConnection.GenerateShortCodeAsync(ipAddress, port, false);

            Console.Write("Short Code: ");
            var shortCode = Console.ReadLine();
            var client = await shortCodeRequest.ConnectWithShortCodeAsync(shortCode);
            Console.WriteLine("Connected");

            Console.WriteLine("Retrieving instrument info");
            var instrument = await client.GetInstrumentDetailsAsync();
            Console.WriteLine($"Instrument info retrieved for: {instrument.SerialNumber}");

            //await AutoDarkWithDarkSubtractToSpc.Run(client);
            //await ManualDarkWithDarkSubtractToSpc.Run(client);
            await WebsocketEvents.Run(client);
        }
    }
}
