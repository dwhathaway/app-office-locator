# Azure Functions (Thumbnail) Hack

In this hack, we're going to upload images to Azure Blob Storage and trigger an Azure Function to create thumbnails using Microsoft Cognitive Services.  The URI for new thumbnails will be stored in the Easy Table you created in the Easy Table Hack. After completing this hack, you will be able to host your images in the cloud and have them automatically resized for use in the Office Locator App.

### Step 0: Prerequisite

Complete the [Easy Table Hack](easy-table-hack.md)


### Step 1: Create a new Azure Function App

- Browse to [https://portal.azure.com](https://portal.azure.com)
- New > Function App
- Give the app a name (this must be unique - don't worry, the portal will tell you if it's not)
- Select your Azure Subscription
- Choose "Use Existing" for the resource group, and use the same resource group you used for your Mobile App Service in the Easy Table Hack.  
- Choose "Consumption Plan" for the hosting plan.  This way you will only be charge for the resources your function actually uses.
- Select your location.
- Choose "Create New" for the Storage account.
- Click "Create"

![Create Function App](img/create-function.png)

### Step 2: Create your Function

- Once again, browse to [https://portal.azure.com](https://portal.azure.com)
- Click "App Services" from the menu on the left of the page
- Select the Function App you just created.
- Click on "Functions" from the "Functions App Menu" on the left-hand side
- Click `+ New Function` at the top
- Select Custom Function
- Scroll down and click BlobTrigger
- Set the "Language" to C#
- Name your Function "Thumbnail"
- Enter `locationimages/{name}` for the Path
- Click "New" next to "Storage account connection"
- Select the storage account you created in Step 1
- Click "Create"

![New Function](img/new-function.png)

### Step 3: Add a NuGet Package

- Click on "View Files" on the right side of the code editor
- Click "Add" to add a new File
- Name the file `project.json`

![Add project.json](img/add-project-file.png)

- Copy and paste this code into the project.json file 
- Click "Save"

```json
{
  "frameworks": {
    "net46":{
      "dependencies": {
        "Microsoft.Azure.Mobile.Client": "4.0.2"
      }
    }
  }
}
```

### Step 4: Set your Function Output

- Click "Integrate" on the left
- Click `+ New Output`
- Select "Azure Blob Storage"
- Click "Select"

![Blob Output](img/blob-output.png) 

- Enter `thumbnails/{name}` for the "Path"
- Under "Storage account connection", select the storage account we created in Step 1
- Click "Save"

![Configure Blob Output](img/blob-output-config.png)

> __Note:__ For the current version of Azure Functions we need to edit the function.json file the is automatically generated to work around a known issue.

- Click "Advanced editor" in the upper right
- Under the "OutputBlob" change the "direction" from `out` to `inout` as shown

![Edit function.json](img/edit-function-json.png)

### Step 5: Write your Function Code

- Click on "Tumbnail" (your function name) to open the code editor
- Copy / Paste the code below into the `run.csx` file (overwriting the default code that opens)
- Click Save

```csharp
#r "Microsoft.WindowsAzure.Storage"

using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MobileServices;

const string apiKey = "<insert your cognitive services api key>";
const string apiUrlBase = "https://api.projectoxford.ai/vision/v1.0/generateThumbnail";
static readonly HttpClient httpClient = GetHttpClient(apiUrlBase, apiKey);
static readonly MobileServiceClient mobileServiceClient = new MobileServiceClient("<insert your app service url>");

public static async Task Run(Stream myBlob, CloudBlockBlob outputBlob, string name, TraceWriter log)
{
    log.Info($"Image Name: {name}");

    const int width = 320;
    const int height = 320;
    const bool isSmartCroppingEnabled = true;

    using (HttpContent content = new StreamContent(myBlob))
    {
        log.Info($"Getting Vision API Response");
        content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

        var uri = $"{apiUrlBase}?width={width}&height={height}&smartCropping={isSmartCroppingEnabled.ToString()}";

        var response = await httpClient.PostAsync(uri, content);
        var responseBytes = await response.Content.ReadAsByteArrayAsync();

        log.Info($"Uploading Vision API Response to Blob Storage");
        await outputBlob.UploadFromByteArrayAsync(responseBytes, 0, responseBytes.Length);

        log.Info($"Updating ImageUrl in Mobile App Service Database to point to Azure Blob Storage");
        var nameNoExt = System.IO.Path.GetFileNameWithoutExtension(name);
        var results = await mobileServiceClient.GetTable<Location>().Where(x => x.Name.ToUpper() == nameNoExt.ToUpper()).ToListAsync();

        Location loc = results.FirstOrDefault();
        log.Info($" Image URI: {outputBlob.Uri.ToString()}");
        loc.Image = outputBlob.Uri.ToString();

        await mobileServiceClient.GetTable<Location>().UpdateAsync(loc);
    }
}

static HttpClient GetHttpClient(string baseUrl, string apiKey)
{
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(baseUrl);
    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

    return httpClient;
}

class Location
{
    public string Id { get; set; }
    public string Image { get; set; }
    public string Name { get; set; }
}
```

### Step 6: Create your Cognitive Service

- Click "Create a resource" in the upper left
- Search for `Computer Vision API`
- Give it a name and select a location
- Choose F0 for the "Pricing tier" - this is the free tier
- Select the same "Resource group" you have been using
- Click "Create"

![Create Vision API](img/create-vision-api.png)

### Step 7: Replace the values in your function script

- Navigate to the Cognitive Service you just created
- Go to "Keys"
- Copy Key 1

![CS Key](img/cs-key.png)

- Navigate back to your Function App
- Click on Thumbnail (the name of your function)
- Replace `<insert your cognitive services api key>` on line 16 with your copied API key
- Save the file
- Navigate to the Mobile App Service we created in the Easy Table Hack
- Copy the URL as shown

![App Service URL](img/mobile-service-url.png)

- Return to your Function App
- Replace `<insert your app service url>` on line 35 with your copied URL

### Step 8: Test your Function

- Click on "Resource Groups" on the left
- Select the Resource Group we created in the [Easy Table Hack](easy-table-hack.md)
- Select the Storage Account we created with our Function
- Click "Blobs"
- Click `+ Container`
- Enter `locationimages` for the name
- Change the "Public access level" to "Container"
- Click "Ok"

![Add Container](img/add-container.png)

- Click "locationimages" to open the container (folder) you just created
- Click "Upload"
- Upload an image with a filename that matches the name of one of your locations
  - **Note**: the filename must match the location name exactly.
  - If you used `data/Location.csv` for your table, you can upload `data/Xamarin HQ.png` to BLob Storage
- In just a few seconds a new folder named "thumbnails" will be created and contain your resized image.
- Try running the Office Locator app again and see if the new image appears.

## Congratulations!

You've complete this hack!  Why don't you try your hand at the [Visual Studio Mobile Center](mobile-center-hack.md) hack now?