using MarqMetrix.Communications;
using MarqMetrix.Communications.Client;
using MarqMetrix.Communications.Client.Direct;
using MarqMetrix.Communications.Instruments;
using MarqMetrix.Communications.Samples;
using MarqMetrix.Communications.Security;
using MarqMetrix.Data.Samples;
using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public static class MainApplication
    {

        public static async Task MainAsync()
        {
            string host = "192.168.90.105";
            int port = 80;
            bool useHttps = false;
            string apiKey;
            // Request a shortcode to be generated, then get the primary API key using the shortcode.
            using (DirectClientConnection connection = await DirectClientConnection.RequestShortCodeAsync(host, port, useHttps))
            {
                Console.Write("Enter the shortcode provided by the instrument: ");
                apiKey = await connection.GetApiKeyAsync(Console.ReadLine(), ApiKeyType.Primary);
            }

            // Create the client context
            using (IClientContext clientContext = ClientContext.Factory.CreateDirectClientContext(
                new DirectOptions
                {
                    UseHttps = false,
                    HostName = host,
                    Port = 80,
                    ApiKey = apiKey
                }))
            {
                // Open the connection to the instrument
                await clientContext.OpenAsync();

                // List all of the instruments connected to the gateway instrument, individual instruments should only return one item in the results.
                ICollectionResult<IInstrumentInfo> instrumentsResult;
                string instrumentId = null;
                do
                {
                    instrumentsResult = await clientContext.GetInstrumentsAsync();
                    foreach (IInstrumentInfo instrumentInfo in instrumentsResult.Items)
                    {
                        Console.WriteLine($"Instrument: {instrumentInfo.Id}");
                        if (instrumentId == null)
                            instrumentId = instrumentInfo.Id;
                    }
                } while (instrumentsResult.HasMoreItems);

                // Setup dark acquisition options for 5 samples, each at 1 second capture integration times, and average the 5 results into one sample.
                ISampleAcquisitionOptions darkSampleOptions = new SampleAcquisitionOptions
                {
                    IntegrationTime = TimeSpan.FromSeconds(1),
                    SampleAverageCount = 5,
                    LaserPower = 0 // 0 - No laser - Dark sample
                };

                // Acquire the sample. This function will block asynchronously until the sample has been acquired.
                ISampleInfo darkSampleInfo = await clientContext.AcquireSampleAsync(instrumentId, darkSampleOptions);

                // Retrieve the sample data.
                ISampleSet darkSampleSet = await clientContext.GetSampleAsync(instrumentId, darkSampleInfo.Id);



                // Setup acquisition options for 5 samples, each at 1 second capture integration times, and average the 5 results into one sample.
                ISampleAcquisitionOptions options = new SampleAcquisitionOptions
                {
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
