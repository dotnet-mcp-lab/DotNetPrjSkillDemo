using Azure.Search.Documents.Indexes;

namespace DemoAzureAiSearchIndexForBlob.JsonModels;

public class Order
{
    [SimpleField(IsFilterable = true, IsKey = true)]
    public required string CustomerOrderId { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true)]
    public required string CustomerName { get; set; }
    [SearchableField(IsFilterable = true, IsSortable = true)]
    public required string OrderId { get; set; }
    public required Item[] Items { get; set; }
}