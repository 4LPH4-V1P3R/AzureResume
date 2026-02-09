using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function;

public class GetResumeCounter
{
    private readonly ILogger<GetResumeCounter> _logger;
    private readonly CosmosClient _cosmosClient;

    private const string DatabaseName = "AzureResume";
    private const string ContainerName = "Counter";
    private const string CounterId = "1";

    public GetResumeCounter(ILogger<GetResumeCounter> logger)
    {
        _logger = logger;
        var connectionString = Environment.GetEnvironmentVariable("AzureConnectionString");
        _cosmosClient = new CosmosClient(connectionString, new CosmosClientOptions
        {
            UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions()
        });
    }

    [Function("GetResumeCounter")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("GetResumeCounter function triggered.");

        var container = _cosmosClient.GetContainer(DatabaseName, ContainerName);
        var partitionKey = new PartitionKey(CounterId);

        var response = await container.ReadItemAsync<Counter>(CounterId, partitionKey);
        var counter = response.Resource;

        counter.Count++;

        await container.ReplaceItemAsync(counter, CounterId, partitionKey);

        _logger.LogInformation($"Counter updated to {counter.Count}.");

        return new OkObjectResult(counter);
    }
}
