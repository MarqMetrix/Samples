using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MarqMetrix.InstrumentManagement;

namespace ConsoleApp.Examples
{
    public static class AutoDarkWithDarkSubtractPushSettings
    {
        public static async Task Run(IInstrumentClient client)
        {
            Console.WriteLine("Enabling laser");
            await client.SetLaserEnabledAsync();

            Console.WriteLine("Acquiring sample");
            var pushSettings = new PushSampleSettings
            {
                OutputAxisAlignment = OutputAxisAlignment.RamanShift,
                OutputProcessingType = OutputProcessingType.DarkSubtraction,
                OutputFileFormat = FileFormat.Text,
                PushType = PushSampleFormat.File,
                SkipDarkSamples = true,
                FaultOnFailure = true,
                StandardizedXAxis = true,
                DirectoryName = "C:\\Samples", // This directory must exist on the instrument or emulator machine
                FilenameScheme = "{ExperimentName}{FileExtension}"
            };
            
            var sampleDetails = await client.AcquireSampleAsync(new SampleAcquisitionOptions
            {
                IntegrationTime = TimeSpan.FromMilliseconds(100),
                SampleAverageCount = 1,
                LaserPower = 100,
                Metadata = new Dictionary<string, object>
                {
                    {"ExperimentName", "Auto Dark"}
                },
            }, DarkSampleOptions.NewDark);

            Console.WriteLine("Disabling laser");
            await client.SetLaserEnabledAsync(false);
            
            Console.WriteLine("Complete");
        }
    }
}