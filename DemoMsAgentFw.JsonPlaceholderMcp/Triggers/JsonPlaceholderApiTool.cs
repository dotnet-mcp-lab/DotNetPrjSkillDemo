using System.Diagnostics;
using System.Net.Http.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;
using Microsoft.Extensions.Logging;

namespace DemoMsAgentFw.JsonPlaceholderMcp.Triggers;

public class JsonPlaceholderApiTool(IHttpClientFactory httpClientFactory, ILogger<JsonPlaceholderApiTool> logger)
{
    private const string HttpClientName = "JsonPlaceholder-api";
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(HttpClientName);

    [Function("ListUsers")]
    public async Task<IEnumerable<User>> ListUsersAsync(
        [McpToolTrigger("ListUsers", "List all users")]
        ToolInvocationContext context,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting users [TraceId: {TraceId}]", Activity.Current?.TraceId);
        var response = await _httpClient.GetAsync("users", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<User>>(cancellationToken) ?? [];
    }

    [Function("GetUserById")]
    public async Task<User> GetUserByIdAsync(
        [McpToolTrigger("GetUserById", "Get a user by User ID")]
        ToolInvocationContext context,
        [McpToolProperty("id", "User ID", isRequired: true)]
        int id,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting user by ID {id} [TraceId: {TraceId}]", id, Activity.Current?.TraceId);
        var response = await _httpClient.GetAsync($"users/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<User>(cancellationToken) ?? new User();
    }

    [Function("ListTodos")]
    public async Task<IEnumerable<TodoItem>> ListTodosAsync(
        [McpToolTrigger("ListTodos", "Get all todos")]
        ToolInvocationContext context,
        [McpToolProperty("limit", "Limit", isRequired: false)]
        int limit = 200,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Listing todos with limit {limit} [TraceId: {TraceId}]", limit,
            Activity.Current?.TraceId);
        var response = await _httpClient.GetAsync("todos", cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<TodoItem>>(cancellationToken) ?? [];
        return result.Take(Math.Clamp(limit, 1, 200)).ToList();
    }

    [Function("GetTodosById")]
    public async Task<TodoItem> GetTodosByTodoIdAsync(
        [McpToolTrigger("GetTodosById", "Get todos by Todo ID")]
        ToolInvocationContext context,
        [McpToolProperty("todoId", "Todo ID", isRequired: true)]
        int todoId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting todos by ID {id} [TraceId: {TraceId}]", todoId, Activity.Current?.TraceId);
        var response = await _httpClient.GetAsync($"todos/{todoId}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoItem>(cancellationToken) ?? new TodoItem();
    }

    [Function("GetTodosByUserId")]
    public async Task<List<TodoItem>> GetTodosByUserIdAsync(
        [McpToolTrigger("GetTodoByUser", "Get todos by User ID")]
        ToolInvocationContext context,
        [McpToolProperty("userId", "User ID", isRequired: true)]
        int userId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting todos by User ID {id} [TraceId: {TraceId}]", userId, Activity.Current?.TraceId);
        var response = await _httpClient.GetAsync($"users/{userId}/todos", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<TodoItem>>(cancellationToken) ?? [];
    }
}