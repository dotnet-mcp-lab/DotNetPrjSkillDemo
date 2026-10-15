using System.Text.Json.Serialization;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace DemoAzureAiSearchIndexForBlob.JsonModels;

// ReSharper disable once ClassNeverInstantiated.Global
public class Item
{
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    [JsonPropertyName("Id")]
    public required string ItemId { get; set; }

    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.FrMicrosoft)]
    public required string ItemName { get; set; }

    [SimpleField(IsFilterable = true, IsFacetable = true)]
    public int Quantity { get; set; }

    [SimpleField(IsFilterable = true, IsFacetable = true)]
    public double Price { get; set; }
}