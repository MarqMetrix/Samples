using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarqMetrix.InstrumentManagement;

namespace ConsoleApp.Examples
{
    public static class WebsocketEvents
    {
        public static async Task Run(IInstrumentClient client)
        {
            // When wanting sample data notifications, it must be manually turned on and off
            // because samples can happen very quickly and can hinder performance if the data
            // is not needed in real time.
            client.RegisterSampleDataNotifications();

            try
            {
                client.SampleAcquisitionStatusChanged.Subscribe(changed =>
                {
                    Console.WriteLine($"SAMPLE STATUS CHANGED: ID - {changed.SampleDetails.Id} | NEW STATUS - {changed.SampleDetails.Status.ToString()}");
                });
                
                client.SampleDataAcquired.Subscribe(sample =>
                {
                    Console.WriteLine($"SAMPLE DATA RECEIVED: ID - {sample.SampleId} | ARRAY LENGTH - {sample.SampleData.Data.Length}");
                });
                
                Console.WriteLine("Enabling laser");
                await client.SetLaserEnabledAsync();

                Console.WriteLine("Acquiring sample");
                var sampleDetails = await client.StartAcquiringSampleAsync(new SampleAcquisitionOptions
                {
                    IntegrationTime = TimeSpan.FromMilliseconds(100),
                    SampleAverageCount = 1,
                    LaserPower = 100,
                    Metadata = new Dictionary<string, object>
                    {
                        {"ExperimentName", "Notification Sample"}
                    }
                });

                Console.WriteLine("Waiting 2.5 seconds...");
                await Task.Delay(2500);
                
                Console.WriteLine("Disabling laser");
                await client.SetLaserEnabledAsync(false);

                Console.WriteLine("Complete");
            }
            finally
            {
                client.UnregisterSampleDataNotifications();
            }
        }
    }
}