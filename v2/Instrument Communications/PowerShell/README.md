# PowerShell Samples
Here are the PowerShell examples for communicating with MarqMetrix instruments.

### PowerShell Module
MarqMetrix has created a PowerShell module for interacting with our instruments. To install, execute the following command within a PowerShell prompt: `Install-Module MarqMetrix-InstrumentManagement`

### Creating the client from a connection string

```PowerShell
Connect-InstrumentClient -ConnectionString "[connection string to connect with]"
```

### Using a short code to connect to an Instrument

```PowerShell
Connect-InstrumentClient -RemoteHost [IP address] -Port [port number]
```

![Connect with short code](Screenshots/ConnectShortCode.png)

If no port number is specified, the default port 80 is used. This is the port that a physical instrument uses.


### Retrieving instrument info

```PowerShell
Get-Instrument
```

### Acquiring a sample 

The `Invoke-AcquireSample` method has many parameters that change how it acquires samples. By specifying just the integration time and laser power, a sample acquisition request is sent to the device and the sample info is returned. It's important to note that the sample state may or may not be completed by using this method.

```PowerShell
Invoke-AcquireSample -IntegrationTime [integration time in milliseconds] -LaserPower [laser power in milliwatts]
```

To wait for a sample acquisition to complete, add the `-Wait` switch.

```PowerShell
Invoke-AcquireSample -IntegrationTime [integration time in milliseconds] -LaserPower [laser power in milliwatts] -Wait
``` 

To acquire a light sample to be acquired and include a new dark sample, add the `-AutoDark` switch.

```PowerShell
Invoke-AcquireSample -IntegrationTime [integration time in milliseconds] -LaserPower [laser power in milliwatts] -AutoDark -Wait
```

To acquire a light & dark sample and automatically save them as a SPC file, use the `-SPC [filename]` parameter. 

```PowerShell
Invoke-AcquireSample -IntegrationTime [integration time in milliseconds] -LaserPower [laser power in milliwatts] -AutoDark -Wait -SPC "[filename]"
```