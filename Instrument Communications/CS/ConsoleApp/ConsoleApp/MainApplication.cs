using MarqMetrix.Communications;
using MarqMetrix.Communications.Client;
using MarqMetrix.Communications.Client.Direct;
using MarqMetrix.Communications.Instruments;
using MarqMetrix.Communications.Samples;
using MarqMetrix.Data.Samples;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public static class MainApplication
    {

        public static async Task MainAsync()
        {
            // Create the client context
            using (IClientContext clientContext = ClientContext.Factory.CreateDirectClientContext(
                new DirectOptions
                {
                    UseHttps = false,
                    HostName = "192.168.90.105",
                    Port = 80,
                    ApiKey = "zLQVNaCkNgrAm66j0+zVLoKYW602xdhcawgIdPm2HcY="
                }))
            {
                // Open the connection to the instrument
                await clientContext.OpenAsync();

                // List all of the instruments connected to the gateway instrument
                ICollectionResult<IInstrumentInfo> instrumentsResult;
                string instrumentId = null;
                do
                {
                    instrumentsResult = await clientContext.GetInstrumentsAsync(null);
                    foreach (IInstrumentInfo instrumentInfo in instrumentsResult.Items)
                    {
                        Console.WriteLine($"Instrument: {instrumentInfo.Id}");
                        if (instrumentId == null)
                            instrumentId = instrumentInfo.Id;
                    }
                } while (instrumentsResult.HasMoreItems);

                // Setup acquisition options for 5 samples, each at 1 second capture integration times, and average the 5 results into one sample.
                ISampleAcquisitionOptions options = new SampleAcquisitionOptions
                {
                    DarkSample = DarkSampleUsage.None,
                    IntegrationTime = TimeSpan.FromSeconds(1),
                    SampleAverageCount = 5,
                    LaserPower = 200
                };

                // Acquire the sample. This function will block asynchronously until the sample has been acquired.
                ISampleInfo sampleInfo = await clientContext.AcquireSampleAsync(instrumentId, options);

                // Retrieve the sample data.
                ISampleSet sampleSet = await clientContext.GetSampleAsync(instrumentId, sampleInfo.Id);

                Console.WriteLine($"Acquired Sample. [{sampleInfo.Id}]");

                // Close the connection to the instrument
                await clientContext.CloseAsync();
            }
        }

    }
}
