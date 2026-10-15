using Azure;
using Azure.Search.Documents.KnowledgeBases;
using Azure.Search.Documents.KnowledgeBases.Models;
using Microsoft.Extensions.Configuration;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddUserSecrets<Program>()
    .Build();

var searchEndpointUri = new Uri(RequiredConfig("SearchServiceEndpoint"));
var searchServiceApiKey = RequiredConfig("SearchServiceApiKey");
var knowledgeBaseName = RequiredConfig("KnowledgeBase");

// Create knowledge base retrieval client
var kbClient =
    new KnowledgeBaseRetrievalClient(searchEndpointUri, knowledgeBaseName, new AzureKeyCredential(searchServiceApiKey));
var retrievalRequest = new KnowledgeBaseRetrievalRequest();
retrievalRequest.Messages.Add(new KnowledgeBaseMessage(content:
[
    new KnowledgeBaseMessageTextContent("You can only answer customer info and purchase order(s) related question(s)")
]) { Role = "assistant" });

const string demoQuery = "What are the products that Ervin Howell purchased?";
retrievalRequest.Messages.Add(new KnowledgeBaseMessage(content:
[
    new KnowledgeBaseMessageTextContent(demoQuery)
]) { Role = "user" });

Console.WriteLine($"Start query:\r\n{demoQuery}");
var result = await kbClient.RetrieveAsync(retrievalRequest);

Console.WriteLine("\r\nResponse TextContent:");
Console.WriteLine(
    (result.Value.Response[0].Content[0] as KnowledgeBaseMessageTextContent)!.Text
);

return;

string RequiredConfig(string key)
{
    var value = config[key];

    return string.IsNullOrWhiteSpace(value)
        ? throw new InvalidOperationException($"Missing required configuration value: {key}")
        : value;
}