using Azure.Monitor.OpenTelemetry.Exporter;
using DemoMsAgentFw.JsonPlaceholderMcp.Triggers;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = FunctionsApplication.CreateBuilder(args);

var jsonPlaceholderUrl = builder.Configuration["JsonPlaceholderUrl"] ??
                         throw new InvalidOperationException("JsonPlaceholderUrl is not set");
builder.Services.AddHttpClient("JsonPlaceholder-api",
    client => { client.BaseAddress = new Uri(jsonPlaceholderUrl); });

builder.ConfigureFunctionsWebApplication();

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();

    builder.Logging.AddOpenTelemetry(b => b.IncludeScopes = true);

    builder.Logging.Services.Configure<LoggerFilterOptions>(options =>
    {
        options.Rules.Add(new LoggerFilterRule(null, "default", LogLevel.Warning, null));
        options.Rules.Add(new LoggerFilterRule(null, typeof(HttpClient).FullName, LogLevel.Information, null));
        options.Rules.Add(new LoggerFilterRule(null, typeof(JsonPlaceholderApiTool).FullName, LogLevel.Information,
            null));
    });

    // Stupid Workaround for ILogger<T> data not recorded in Application Insights, see: 
    // https://learn.microsoft.com/azure/azure-functions/opentelemetry-howto?tabs=app-insights%2Cihostapplicationbuilder%2Cmaven&pivots=programming-language-csharp#console-logging
    builder.Logging.AddConsole();
}

builder.Build().Run();