# C# Samples
Here are the C# examples for communicating with MarqMetrix instruments.

### Nuget Packages
MarqMetrix has created an SDK for interacting with our instruments. To add the SDK to your C# application, install the following nuget package: `MarqMetrix.InstrumentManagement.Client`

From Package Manager Console in Visual Studio: `Install-Package MarqMetrix.InstrumentManagement.Client`

You may need to include pre-releases depending on which versions are available, or that you are targeting: `Install-Package MarqMetrix.InstrumentManagement.Client -IncludePrerelease`

### Using Statements
Add the following using statement to your code:

```C#
using MarqMetrix.InstrumentManagement;
```

### Creating the client from a connection string

```C#
var connection = InstrumentClientConnection.Parse(connectionString);
var client = connection.CreateClient();
```

### Using a short code to connect to an Instrument

```C#
var shortCodeRequest = await InstrumentClientConnection.GenerateShortCodeAsync(ipAddress, port, false);
var connection = await shortCodeRequest.ConnectWithShortCodeAsync(shortCode);
var client = connection.CreateClient();
```

### Retrieving instrument info

```C#
var instrument = (await client.GetInstrumentsAsync()).Items.FirstOrDefault();
```

### Acquiring a sample (ComputedSampleAcquisitionOptions) 

The simplest way to acquire a sample is to use the ComputedSampleAcquisitionOptions. This is the highest level of overloads for the AcquireSampleAsync method. When executing, it acquires the sample, and a dark sample if requested, and waits for them to finish executing before returning execution.

```C#
var sampleInfo = await client.AcquireSampleAsync(instrument.Id, new ComputedSampleAcquisitionOptions
{
    DarkSampleOptions = DarkSampleOptions.NewDark,
    IntegrationTime = TimeSpan.FromMilliseconds(100),
    LaserPower = 100,
    SampleAverageCount = 1
});
```

#### ComputedSampleAcquisitionOptions

`AutoDark` If a new dark is requested, this method will also acquire it, and associate its "DarkSampleId" within the sample metadata.

`IntegrationTime` The integration time for the spectrometer to use.

`LaserPower` The laser power to use. Set this to 0 to collect a dark sample.

`SampleAverageCount` The number of samples to acquire and average spectra from. 

`Metadata` Additional metadata values.

`FailureRetryCount` Number of attempts to retry in the event of a component failure. This only attempts to retry specific component communications, and does not retry the entire sample acquisition process.

`NetworkExportSettings` Settings for exporting to a network location on sample acquisition completion. 


### Acquiring a sample (SampleAcquisitionOptions) 

The next level of overloads for the AcquireSampleAsync method has the standard sample acquisition options for IM 2.0. When executing, it acquires the sample, and waits for them to finish executing before returning execution. There are additional options for sample acquisition in this method that aren't available for IM 1.x.

```C#
var sampleInfo = await client.AcquireSampleAsync(instrument.Id, new SampleAcquisitionOptions
{
    IntegrationTime = TimeSpan.FromMilliseconds(100),
    LaserPower = 100,
    SampleAverageCount = 1
});
```

#### SampleAcquisitionOptions

`IntegrationTime` The integration time for the spectrometer to use.

`LaserPower` The laser power to use. Set this to 0 to collect a dark sample.

`SampleAverageCount` The number of samples to acquire and average spectra from. 

`Metadata` Additional metadata values.

`FailureRetryCount` Number of attempts to retry in the event of a component failure. This only attempts to retry specific component communications, and does not retry the entire sample acquisition process.

`NetworkExportSettings` Settings for exporting to a network location on sample acquisition completion. 

### Acquiring a sample for IM 1.x (BasicSampleAcquisitionOptions) 

The next level of overloads for the AcquireSampleAsync method has the standard sample acquisition options for IM 1.x. When executing, it acquires the sample, and waits for them to finish executing before returning execution.

```C#
var sampleInfo = await client.AcquireSampleAsync(instrument.Id, new BasicSampleAcquisitionOptions
{
    IntegrationTime = TimeSpan.FromMilliseconds(100),
    LaserPower = 100,
    SampleAverageCount = 1
});
```

#### BasicSampleAcquisitionOptions

`IntegrationTime` The integration time for the spectrometer to use.

`LaserPower` The laser power to use. Set this to 0 to collect a dark sample.

`SampleAverageCount` The number of samples to acquire and average spectra from. 

`Metadata` Additional metadata values.

`FailureRetryCount` Number of attempts to retry in the event of a component failure. This only attempts to retry specific component communications, and does not retry the entire sample acquisition process.

`NetworkExportSettings` Settings for exporting to a network location on sample acquisition completion. 


### Starting sample acquisition

The lowest level of abstraction is the StartAcquiringSampleAsync method. If your application wants to monitor the completion of sample acquisitions, use this method. This is typically used when attempting to maximize performance of communications between IM and the client application.

```C#
var sampleInfo = await client.StartAcquiringSampleAsync(instrument.Id, new SampleAcquisitionOptions
{
    IntegrationTime = TimeSpan.FromMilliseconds(100),
    LaserPower = 100,
    SampleAverageCount = 1
});
```

### Computing the sample

To retrieve the computed sample, call ComputeSampleAsync passing in the sample info. This method attempts to retrieve the calibration and the dark sample info, if applicable.

```C#
var computedSample = await client.ComputeSampleAsync(instrument.Id, sampleInfo);
```

#### ComputedSample

`Data` The data representing the intensity of the spectrum.

`DependencyInfo` The dependencies associated with the computed sample.

`RamanShiftData` The Raman shift data for the spectrum.

`WavelengthData` The wavelength data for the spectrum. 