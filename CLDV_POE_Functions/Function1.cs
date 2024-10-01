using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using CLDV_POE_Functions.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CLDV_POE_Functions.Models.CLDV_POE.Models;
using Azure.Storage.Files.Shares;
using System.IO.Pipes;
using System.Security.Cryptography.X509Certificates;

public class CombinedFunctions
{
    private readonly ILogger _logger;

    public CombinedFunctions(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CombinedFunctions>();
    }

    [Function("StoreInTable")]
    public async Task StoreInTableAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req) //(ggailey777, 2024)
    {
        string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage"); //(ggailey777, 2024)
        var serviceClient = new TableServiceClient(connectionString); //(ggailey777, 2024)
        var tableClient = serviceClient.GetTableClient("Customer"); //(ggailey777, 2024)

        var customer = new Customer //(ggailey777, 2024)
        {
            PartitionKey = "CustomerPartition",
            RowKey = Guid.NewGuid().ToString(),
            CustomerId = Guid.NewGuid().ToString(),
            Customer_Name = "Travis",
            Email = "travis@example.com",
            PhoneNumber = "0934567890"
        };

        await tableClient.AddEntityAsync(customer); //(ggailey777, 2024)
    }

    [Function("WriteToBlob")]
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log) //(Sumit Kharche, 2022)
    {
        string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var containerName = "products";
        Stream myBlob = new MemoryStream();

        var file = req.Form.Files["File"]; //(Sumit Kharche, 2022)
        myBlob = file.OpenReadStream();

        var blobClient = new BlobContainerClient(connectionString, containerName); //(Sumit Kharche, 2022)
        var blob = blobClient.GetBlobClient(file.FileName);

        await blob.UploadAsync(myBlob); //(Sumit Kharche, 2022)

        return new OkObjectResult("file successfully uploaded"); //(Sumit Kharche, 2022)
    }

    [Function("WriteToQueue")]
    public async Task WriteToQueueAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var queueClient = new QueueClient(connectionString, "orders");

        await queueClient.CreateIfNotExistsAsync(); //(ggailey777, 2023)

        string message = "OrderId: " + Guid.NewGuid().ToString() + ", Customer: Travis, Product: Super Nintendo"; //(ggailey777, 2023)

        await queueClient.SendMessageAsync(message); //(ggailey777, 2023)

        _logger.LogInformation("Message written to the queue: {Message}", message); //(ggailey777, 2023)
    }

    [Function("WriteToAzureFiles")]
    public static async Task<IActionResult> Run1([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
    {
        string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var fileShareName = "birdshare";  
        var directoryName = "uploads";  

        var form = await req.ReadFormAsync(); //(Sumit Kharche, 2022)
        var file = form.Files["File"]; 

        try
        {
            var serviceClient = new ShareServiceClient(connectionString);
            var directoryClient = serviceClient.GetShareClient(fileShareName).GetDirectoryClient(directoryName);
            await directoryClient.CreateIfNotExistsAsync();

            var fileClient = directoryClient.GetFileClient(file.FileName);
            await fileClient.CreateAsync(file.Length); 

            using var fileStream = file.OpenReadStream();
            await fileClient.UploadRangeAsync(new Azure.HttpRange(0, file.Length), fileStream); 

            return new OkObjectResult("file successfully uploaded");
        }
        catch (Exception ex)
        {
            throw new Exception("Error downloading file :" + ex.Message, ex);
        }
    }
}

