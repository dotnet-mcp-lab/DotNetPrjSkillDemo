using AgenticSearchOpenAiResponsesApi.Tools;
using Azure.AI.OpenAI;
using Azure.Core;
using Azure.Identity;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using SharedConfigLib.Options;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

webApplicationBuilder.Services.AddOptions<AzSearchAgenticRetrievalOptions>()
    .Bind(webApplicationBuilder.Configuration.GetSection("AzureSearch"))
    .ValidateDataAnnotations();

// TODO: should use deployed service managed identity in production environment
var azureCredOptions = new DefaultAzureCredentialOptions { TenantId = webApplicationBuilder.Configuration["AZURE_TENANT_ID"] };
var credential = new DefaultAzureCredential(azureCredOptions);
webApplicationBuilder.Services.AddSingleton<TokenCredential>(credential);

// builder.Services.AddHttpClient<AzureSearchRestApiAgenticRetrievalTool>();
webApplicationBuilder.Services.AddSingleton<AzureSearchCsharpAgenticRetrievalTool>();

string azureOpenAiEndpoint = webApplicationBuilder.Configuration["AzureOpenAI:Endpoint"]
                             ?? throw new InvalidOperationException("Missing configuration: AzureOpenAI:Endpoint");

string azureOpenAiDeploymentName = webApplicationBuilder.Configuration["AzureOpenAI:DeploymentName"]
                                   ?? throw new InvalidOperationException(
                                       "Missing configuration: AzureOpenAI:DeploymentName");

// Agent Framework hosting for OpenAI Responses API.
webApplicationBuilder.AddOpenAIResponses();
webApplicationBuilder.AddOpenAIConversations();

// Register the model-backed chat client used by the agent.
webApplicationBuilder.Services.AddSingleton<IChatClient>(_ => new AzureOpenAIClient(
        new Uri(azureOpenAiEndpoint),
        credential)
    .GetChatClient(azureOpenAiDeploymentName)
    .AsIChatClient());

const string queryAgentName = "rag";
// Register the RAG agent.
var hostedAgentBuilder = webApplicationBuilder.Services.AddAIAgent(queryAgentName, (sp, _) =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    var retrievalTool = sp.GetRequiredService<AzureSearchCsharpAgenticRetrievalTool>();

    var searchFunction = AIFunctionFactory.Create(
        retrievalTool.SearchAsync,
        name: "retrieve_enterprise_knowledge",
        description:
        "Use this tool to find grounded information about customer info and he/she purchase order(s).");

    return chatClient.AsAIAgent(
        name: queryAgentName,
        instructions: """
                      You are a grounded enterprise assistant.

                      Rules:
                      - Use the retrieve_enterprise_knowledge tool for any question that might require facts from the configured knowledge base.
                      - Base your final answer on the retrieved content.
                      - If the retrieved content is insufficient, say what is missing instead of inventing details.
                      - Keep answers concise, but include useful citations or source labels when the tool returns them.
                      """,
        tools: [searchFunction]);
});

var app = webApplicationBuilder.Build();

app.MapGet("/", () => Results.Ok(new
{
    service = "Agentic Search Responses API",
    responsesEndpoint = $"/{queryAgentName}/v1/responses",
    conversationsEndpoint = "/v1/conversations"
}));

// Exposes OpenAI-compatible Responses API endpoint:
// POST /rag/v1/responses
app.MapOpenAIResponses(hostedAgentBuilder);
app.MapOpenAIConversations();

app.Run();