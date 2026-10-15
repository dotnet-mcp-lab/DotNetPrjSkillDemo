using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using DemoAzureAiSearchIndexForBlob.JsonModels;
using Microsoft.Extensions.Configuration;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddUserSecrets<Program>()
    .Build();


var searchEndpointUri = new Uri(RequiredConfig("SearchServiceEndpoint"));
string searchAdminKey = RequiredConfig("SearchServiceAdminKey");
string blobConnectionString = RequiredConfig("BlobConnectionString");
string blobContainerName = RequiredConfig("BlobContainerName");

string indexName = config["IndexName"] ?? throw new Exception("IndexName is not specified in appsettings.json");
string dataSourceName = config["DataSourceName"] ??
                        throw new Exception("DataSourceName is not specified in appsettings.json");
string indexerName = config["IndexerName"] ?? throw new Exception("IndexerName is not specified in appsettings.json");

AzureKeyCredential credential = new(searchAdminKey);

SearchIndexClient indexClient = new(searchEndpointUri, credential);

Console.WriteLine("Creating or updating search index...");

try
{
    await indexClient.GetIndexAsync(indexName);
    await indexClient.DeleteIndexAsync(indexName, null);
}
catch (RequestFailedException ex) when (ex.Status == 404)
{
    //if the specified index not exist, 404 will be thrown.
    Console.WriteLine($"Index {indexName} not found. Creating a new index.");
}

var fieldBuilder = new FieldBuilder();
await indexClient.CreateOrUpdateIndexAsync(new SearchIndex(indexName, fieldBuilder.Build(typeof(Order))));

Console.WriteLine("Creating or updating blob data source...");

SearchIndexerClient searchIndexerClient = new(searchEndpointUri, credential);

SearchIndexerDataSourceConnection blobDataSource = new(
    name: dataSourceName,
    type: SearchIndexerDataSourceType.AzureBlob,
    connectionString: blobConnectionString,
    container: new SearchIndexerDataContainer(blobContainerName));

await searchIndexerClient.CreateOrUpdateDataSourceConnectionAsync(blobDataSource);

Console.WriteLine("Creating or updating blob indexer...");

IndexingParameters indexingParameters = new();

indexingParameters.Configuration.Add("parsingMode", "json");

SearchIndexer blobIndexer = new(name: indexerName, dataSourceName: blobDataSource.Name, targetIndexName: indexName)
{
    Description = "Indexes content and metadata from Azure Blob Storage.",
    Parameters = indexingParameters,

    FieldMappings =
    {
        new FieldMapping("Id")
        {
            TargetFieldName = "CustomerOrderId"
        }
    }
};

// Reset indexer if it already exists
try
{
    await searchIndexerClient.GetIndexerAsync(indexerName);
    Console.WriteLine("Resetting indexer...");
    await searchIndexerClient.ResetIndexerAsync(indexerName);
}
catch (RequestFailedException ex) when (ex.Status == 404)
{
}

await searchIndexerClient.CreateOrUpdateIndexerAsync(blobIndexer);

Console.WriteLine("Running indexer...");

await searchIndexerClient.RunIndexerAsync(indexerName);

ShowIndexStatus(await searchIndexerClient.GetIndexerStatusAsync(indexerName));

Console.WriteLine("All indexing Done.");
Console.WriteLine("Program Exit");

return;

string RequiredConfig(string key)
{
    var value = config[key];

    return string.IsNullOrWhiteSpace(value)
        ? throw new InvalidOperationException($"Missing required configuration value: {key}")
        : value;
}

void ShowIndexStatus(SearchIndexerStatus status)
{
    if (status.LastResult is not null)
    {
        Console.WriteLine($"Last result status: {status.LastResult.Status}");
        Console.WriteLine($"Items processed: {status.LastResult.ItemCount}");
        Console.WriteLine($"Items failed: {status.LastResult.FailedItemCount}");

        foreach (SearchIndexerError error in status.LastResult.Errors)
        {
            Console.WriteLine($"Error: {error.ErrorMessage}");
        }

        foreach (SearchIndexerWarning warning in status.LastResult.Warnings)
        {
            Console.WriteLine($"Warning: {warning.Message}");
        }
    }

    Console.WriteLine($"Indexer status: {status.Status}");
}