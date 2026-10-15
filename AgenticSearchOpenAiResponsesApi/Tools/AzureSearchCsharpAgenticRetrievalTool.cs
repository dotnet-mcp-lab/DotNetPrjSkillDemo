using System.ComponentModel;
using Azure;
using Azure.Core;
using Azure.Search.Documents.KnowledgeBases;
using Azure.Search.Documents.KnowledgeBases.Models;
using Microsoft.Extensions.Options;
using SharedConfigLib;
using SharedConfigLib.Options;

namespace AgenticSearchOpenAiResponsesApi.Tools;

/// <summary>
/// LLM Tool for retrieving information from Azure Search using C# API
/// </summary>
public sealed class AzureSearchCsharpAgenticRetrievalTool(
    IOptions<AzSearchAgenticRetrievalOptions> options,
    TokenCredential credential)
{
    [Description(
        """
        Searches the knowledge base using Azure AI Search agentic retrieval. 
        Use this for questions that require grounded information.
        """)]
    public async Task<string> SearchAsync(
        [Description("The user's natural language question to retrieve grounded information for.")]
        string query,
        CancellationToken cancellationToken = default)
    {
        var optionsValue = options.Value;
        // Create KnowledgeBase Retrieval Client
        var kbRetrievalClient = new KnowledgeBaseRetrievalClient(
            endpoint: optionsValue.SearchEndpoint,
            knowledgeBaseName: optionsValue.KnowledgeBaseName, credential);

        // Create retrieval request
        var retrievalRequest = new KnowledgeBaseRetrievalRequest();
        retrievalRequest.Messages.Add(new KnowledgeBaseMessage(content:
        [
            new KnowledgeBaseMessageTextContent("""
                                                To get customer purchase item details, use {knowledgesource-customer-orders} Knowledge source.
                                                When need for User Data like "User Id", "Name", "Username", "Email", "Phone", prefer using {knowledgesource-userid} Knowledge source to retrieve real information
                                                """)
        ]) { Role = "assistant" });
        var knowledgeBaseMsg = new KnowledgeBaseMessage(content: [new KnowledgeBaseMessageTextContent(query)])
            { Role = "user" };
        retrievalRequest.Messages.Add(knowledgeBaseMsg);
        retrievalRequest.RetrievalReasoningEffort = optionsValue.ReasoningEffect switch
        {
            RetrievalReasoningEffect.Minimal => new KnowledgeRetrievalLowReasoningEffort(),
            RetrievalReasoningEffect.Low => new KnowledgeRetrievalLowReasoningEffort(),
            RetrievalReasoningEffect.Medium => new KnowledgeRetrievalMediumReasoningEffort(),
            _ => throw new NotImplementedException()
        };

        // Retrieve knowledge
        var retrievalResponse = await kbRetrievalClient.RetrieveAsync(retrievalRequest, cancellationToken);

        // Extract returned messages text
        // TODO: Add some logging to record retrieval response and related routing information.
        return ExtractTextRetrievalResult(retrievalResponse);
    }

    private static string ExtractTextRetrievalResult(Response<KnowledgeBaseRetrievalResponse> retrievalResponse)
    {
        if (retrievalResponse?.Value?.Response?[0]?.Content?[0] is KnowledgeBaseMessageTextContent result)
        {
            return result.Text;
        }

        throw new ArgumentException("Retrieval response is null or does not contain expected content.");
    }
}