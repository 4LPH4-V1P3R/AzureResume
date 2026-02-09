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

    public GetResumeCounter(ILogger<GetResumeCounter> logger, CosmosClient cosmosClient)
    {
        _logger = logger;
        _cosmosClient = cosmosClient;
    }

    [Function("GetResumeCounter")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("GetResumeCounter function triggered.");

        try
        {
            var container = _cosmosClient.GetContainer(DatabaseName, ContainerName);
            var partitionKey = new PartitionKey(CounterId);

            // Read with ETag for optimistic concurrency
            var response = await container.ReadItemAsync<Counter>(CounterId, partitionKey);
            var counter = response.Resource;
            var etag = response.ETag;

            counter.Count++;

            // Replace only if the document hasn't changed since we read it
            await container.ReplaceItemAsync(counter, CounterId, partitionKey, new ItemRequestOptions
            {
                IfMatchEtag = etag
            });

            _logger.LogInformation("Counter updated to {Count}.", counter.Count);

            return new OkObjectResult(counter);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
        {
            // Concurrency conflict — another request updated the counter first.
            // Re-read the latest value and return it without retrying the increment.
            _logger.LogWarning("Concurrency conflict on counter increment. Returning latest value.");

            var container = _cosmosClient.GetContainer(DatabaseName, ContainerName);
            var response = await container.ReadItemAsync<Counter>(CounterId, new PartitionKey(CounterId));

            return new OkObjectResult(response.Resource);
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "CosmosDB error: {StatusCode} - {Message}", ex.StatusCode, ex.Message);
            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetResumeCounter.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
