# C# Samples
Here are the C# examples for communicating with MarqMetrix instruments.

### Nuget Packages
MarqMetrix has created an SDK for interacting with our instruments. To add the SDK to your C# application, install the following nuget package: `MarqMetrix.InstrumentManagement.Client`

From Package Manager Console in Visual Studio: `Install-Package MarqMetrix.InstrumentManagement.Client`

### Using Statements
Add the following using statement to your code:

```C#
using MarqMetrix.InstrumentManagement;
```

### Creating the client with an API key

```C#
var client = InstrumentClientConnection.CreateClient(ipAddress, port, false, apiKey);
```

### Using a short code to connect to an Instrument

```C#
var shortCodeRequest = await InstrumentClientConnection.GenerateShortCodeAsync(ipAddress, port, false);
var client = await shortCodeRequest.ConnectWithShortCodeAsync(shortCode);
```

### Retrieving instrument info

```C#
var instrument = await client.GetInstrumentDetailsAsync();
```

### Acquiring a sample

The way to acquire a sample is to use the SampleAcquisitionOptions. The below example will acquire a single sample and wait until it completes.

```C#
var sampleDetails = await client.AcquireSampleAsync(new SampleAcquisitionOptions
{
    IntegrationTime = TimeSpan.FromMilliseconds(100),
    SampleAverageCount = 1,
    LaserPower = 100,
    Metadata = new Dictionary<string, object>
    {
        {"ExperimentName", "My Experiment"}
    }
});
```

In the example below it will acquire a light sample, a dark sample matching the integration time / averages, and wait until it 
completes.  The Dark sample taken will show in the light samples metadata with the key `DarkSampleId`.

```C#
var sampleDetails = await client.AcquireSampleAsync(new SampleAcquisitionOptions
{
    IntegrationTime = TimeSpan.FromMilliseconds(100),
    SampleAverageCount = 1,
    LaserPower = 100,
    Metadata = new Dictionary<string, object>
    {
        {"ExperimentName", "My Experiment"}
    }
}, DarkSampleOptions.NewDark);
```

In the example below it will acquire a light sample and try to use an existing dark on the instrument that matches the integration time and averages.  If no dark is found an exception is thrown.  The Dark sample used will show in the light samples metadata with the key `DarkSampleId`.

```C#
var sampleDetails = await client.AcquireSampleAsync(new SampleAcquisitionOptions
{
    IntegrationTime = TimeSpan.FromMilliseconds(100),
    SampleAverageCount = 1,
    LaserPower = 100,
    Metadata = new Dictionary<string, object>
    {
        {"ExperimentName", "My Experiment"}
    }
}, DarkSampleOptions.UseExistingDark);
```

### Starting sample acquisition

The lowest level of abstraction is the StartAcquiringSampleAsync method. If your application wants to monitor the completion of sample acquisitions itself, use this method. This is typically used when attempting to maximize performance of communications between IM and the client application.

```C#
var sampleDetails = await client.StartAcquiringSampleAsync(new SampleAcquisitionOptions
{
    IntegrationTime = TimeSpan.FromMilliseconds(100),
    SampleAverageCount = 1,
    LaserPower = 100,
    Metadata = new Dictionary<string, object>
    {
        {"ExperimentName", "My Experiment"}
    }
});
```

### Computing the sample (Dark Subtract)

To Dark Subtract the sample, two sample's data are needed.
```C#
var sampleData = await client.GetSampleDataAsync(sampleDetails.Id);
var darkSampleData = await client.GetSampleDataAsync(darkSampleDetails.Id);
var computedSample = sampleData.SubtractDark(darkSampleData);
```

### Getting X Axis info (RamanShift and Wavelength)

To get the RamanShift and Wavelength, the instrument's calibration is needed.
```C#
var calibration = await client.GetCurrentCalibrationAsync();
var xAxisValues = calibration.GetXAxisValues();
```
