# Postman
Postman is an API platform for building and using APIs.  It can be used to interact with IM on an instrument or via the emulator.

- [Download Postman](https://www.postman.com/downloads/)
- [Learn Postman](https://learning.postman.com/docs/getting-started/introduction/)

---

## Setup
Once Postman is downloaded and installed, grab the Postman collection and environment from this repository and import them.  More information on importing can be found [here](https://learning.postman.com/docs/getting-started/introduction/), but the basic steps are below.

### Import API Collection
1. To import the collection into Postman, select Import in upper left.
![Postman Import](Images/postman-import.png)
1. Select upload files or drag and drop the `InstrumentManagerWebApi.postman_collection.json` file into the Import section.
1. Select the file(s) to import.
![Postman File Import](Images/postman-file-import.png)
1. Select **Import** to bring the collection into Postman.
1. Repeat the above steps with `MarqMetrixDevice.postman_environment.json` to import the Device Environment next.

---

## Connect to IM
1. With the Postman collection and environment imported, navigate to the `MarqMetrix Device` environment and update the HostName and PortNumber values to match your device. The default PortNumber for a physical device is `80` and `8080` for the emulator.
![Postman Environment](Images/postman-env-tab.png)
1. From the Collections tab, go to the `Connectiviy` folder and select `Generate Shortcode`.
1. Hit send to get a Shortcode, this will be displayed on the front display of the device or emulator.
![Postman Shortcode](Images/postman-shortcode.png)
1. In Postman, now go to `Connectivity` -> `Get Primary Api Key from Shortcode`.
1. Input the Shortcode into the body of the request and hit send.  This will save the API key into the Postman environment.
![Postman API Key](Images/postman-api-key.png)
1. From here all other endpoints can be used in the collection like `Get Instrument`.

### Postman Tips
- You must go through the shortcode process each time you connect to another device or emulator to acquire/reacquire the API key for that device.
- In Postman, items with `{{}}` are variables saved and retrieved from the environment tab.  These can be hand edited, but some endpoints like `Acquire Sample` will set the `{{SampleId}}` variable automatically on a successful call.

### Sample Data Serialization
Sample Data is returned in a `buffer` that is a Base64 encoded array. The `dataType` field is used to determine the array's type.
![Postman Sample Data](Images/postman-sample-data.png)
A C# example below shows how to convert the Base64 buffer to a double array.
```C#
var buffer = Convert.FromBase64String(sample.Data.Buffer);
var dataSize = buffer.Length / Marshal.SizeOf<double>();
var sampleData = new double[dataSize];
Buffer.BlockCopy(buffer, 0, sampleData, 0, buffer.Length);
```