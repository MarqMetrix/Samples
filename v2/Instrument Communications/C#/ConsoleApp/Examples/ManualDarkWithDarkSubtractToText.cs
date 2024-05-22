using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MarqMetrix.InstrumentManagement;

namespace ConsoleApp.Examples
{
    public static class ManualDarkWithDarkSubtractToText
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
                    {"ExperimentName", "Manual Dark"}
                }
            });
            
            var darkSampleDetails = await client.AcquireSampleAsync(new SampleAcquisitionOptions
            {
                IntegrationTime = TimeSpan.FromMilliseconds(100),
                SampleAverageCount = 1,
                LaserPower = 0,
                Metadata = new Dictionary<string, object>
                {
                    {"ExperimentName", "Manual Dark"}
                }
            });

            Console.WriteLine("Disabling laser");
            await client.SetLaserEnabledAsync(false);

            Console.WriteLine("Computing sample");
            Console.WriteLine("Getting X Axis");
            var calibration = await client.GetCurrentCalibrationAsync();
            
            Console.WriteLine("Dark subtracting sample");
            var sampleData = await client.GetSampleDataAsync(sampleDetails.Id);
            var darkSampleData = await client.GetSampleDataAsync(darkSampleDetails.Id);
            var darkSubtractedSampleData = sampleData.SubtractDark(darkSampleData);

            Console.WriteLine("Writing sample to SPC file");
            using var targetFile = File.Create("ManualDarkWithDarkSubtractToText.txt");
            await sampleData.WriteTextStreamDataAsync(darkSampleData, calibration, targetFile,
                applyIntensityCorrection: true, standardizedXAxis: true, cosmicRayReduction: true);
            
            Console.WriteLine("Complete");
        }
    }
}