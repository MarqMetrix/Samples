using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MarqMetrix.InstrumentManagement;

namespace ConsoleApp.Examples
{
    public static class AutoDarkWithDarkSubtractToSpc
    {
        public static async Task Run(IInstrumentClient client)
        {
            Console.WriteLine("Enabling laser");
            await client.SetLaserEnabledAsync();

            Console.WriteLine("Acquiring sample");
            var sampleDetails = await client.AcquireSampleAsync(new SampleAcquisitionOptions
            {
                IntegrationTime = TimeSpan.FromMilliseconds(100),
                SampleAverageCount = 1,
                LaserPower = 100,
                Metadata = new Dictionary<string, object>
                {
                    {"ExperimentName", "Auto Dark"}
                }
            }, DarkSampleOptions.NewDark);

            Console.WriteLine("Disabling laser");
            await client.SetLaserEnabledAsync(false);

            Console.WriteLine("Computing sample");
            Console.WriteLine("Getting X Axis");
            var calibration = await client.GetCurrentCalibrationAsync();
            
            Console.WriteLine("Dark subtracting sample");
            var sampleData = await client.GetSampleDataAsync(sampleDetails.Id);
            var darkSampleData = await client.GetSampleDataAsync((string)sampleData.Metadata["DarkSampleId"]);

            Console.WriteLine("Writing sample to SPC file");
            using var targetFile = File.Create("AutoDarkWithDarkSubtractToSpc.spc");
            sampleData.WriteSpcStreamData(darkSampleData, calibration, OutputAxisAlignment.RamanShift, targetFile);
            
            Console.WriteLine("Complete");
        }
    }
}