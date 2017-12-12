# C# Samples
Here are the C# samples for communicating with MarqMetrix instruments.

### Nuget Packages
MarqMetrix has created an SDK for interacting with our instruments. To add the SDK to your C# application, install the following nuget package: `MarqMetrix.Communications.Client.Direct`

From Package Manager Console in Visual Studio: `Install-Package MarqMetrix.Communications.Client.Direct`

You may need to include pre-releases depending on which versions are available, or that you are targeting: `Install-Package MarqMetrix.Communications.Client.Direct -IncludePrerelease`

### Using Statements
Add the following using statements to your code:
```C#
using MarqMetrix.Communications;
using MarqMetrix.Communications.Client;
using MarqMetrix.Communications.Client.Direct;
using MarqMetrix.Communications.Instruments;
using MarqMetrix.Communications.Samples;
using MarqMetrix.Data.Samples;
```

### Creating the IClientContext
```C#
IClientContext clientContext = ClientContext.Factory.CreateDirectClientContext(
	new DirectOptions
	{
	    UseHttps = false,
	    HostName = "192.168.90.105",
	    Port = 80,
	    ApiKey = "zLQVNaCkNgrAm66j0+zVLoKYW602xdhcawgIdPm2HcY="
	});
```

### Listing all Instruments
```C#
ICollectionResult<IInstrumentInfo> instrumentsResult;
string instrumentId = null;
do
{
    instrumentsResult = await clientContext.GetInstrumentsAsync(null);
    // instrumentsResult.Items contains the current result set.
} while (instrumentsResult.HasMoreItems);
```

### Acquiring a Sample
```C#
ISampleAcquisitionOptions options = new SampleAcquisitionOptions
{
    DarkSample = DarkSampleUsage.None,
    IntegrationTime = TimeSpan.FromSeconds(1),
    SampleAverageCount = 5,
    LaserPower = 200
};

ISampleInfo sampleInfo = await clientContext.AcquireSampleAsync(instrumentId, options);

ISampleSet sampleSet = await clientContext.GetSampleAsync(instrumentId, sampleInfo.Id);
```

