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
using MarqMetrix.Communications.Security;
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

### Using a Shortcode to Connect to an Instrument
Either of the API keys can be retrieved

```C#
using (DirectClientConnection connection = await DirectClientConnection.RequestShortCodeAsync(host, port, useHttps))
{    
    Console.Write("Enter the shortcode provided by the instrument: ");
    apiKey = await connection.GetApiKeyAsync(Console.ReadLine(), ApiKeyType.Primary);
}
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
    IntegrationTime = TimeSpan.FromSeconds(1),
    SampleAverageCount = 5,
    LaserPower = 200
};

ISampleInfo sampleInfo = await clientContext.AcquireSampleAsync(instrumentId, options);

ISampleSet sampleSet = await clientContext.GetSampleAsync(instrumentId, sampleInfo.Id);
```

### Acquiring a Dark Sample
Acquiring a dark sample is the same as acquiring a light sample. The only difference is the laser power is set to 0.

```C#
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
```