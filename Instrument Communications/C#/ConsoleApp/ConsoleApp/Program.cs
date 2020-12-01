using MarqMetrix.InstrumentManagement;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            var clientConnection = await shortCodeRequest.ConnectWithShortCodeAsync(shortCode);

            Console.WriteLine("Connected");

            var client = clientConnection.CreateClient();

            Console.WriteLine("Retrieving instrument info");
            var instrument = (await client.GetInstrumentsAsync()).Items.FirstOrDefault();

            Console.WriteLine("Acquiring sample");
            var sampleInfo = await client.AcquireSampleAsync(instrument.Id, new ComputedSampleAcquisitionOptions
            {
                DarkSampleOptions = DarkSampleOptions.NewDark,
                IntegrationTime = TimeSpan.FromMilliseconds(100),
                LaserPower = 100,
                SampleAverageCount = 1
            });

            Console.WriteLine("Computing sample");
            var computedSample = await client.ComputeSampleAsync(instrument.Id, sampleInfo);

            Console.WriteLine("Writing sample to SPC file");
            using var targetFile = File.Create("sample.spc");
            SpcWriter.WriteSpcStreamData(targetFile, computedSample.Data, computedSample.RamanShiftData, DateTime.UtcNow, string.Empty);
            
            Console.WriteLine("Complete");
        }
    }
}
