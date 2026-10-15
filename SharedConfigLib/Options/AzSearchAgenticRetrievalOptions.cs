namespace SharedConfigLib.Options;

// ReSharper disable once ClassNeverInstantiated.Global
public class AzSearchAgenticRetrievalOptions
{
    public required Uri SearchEndpoint { get; set; }
    public required string KnowledgeBaseName { get; set; }
    public RetrievalReasoningEffect ReasoningEffect { get; set; } = RetrievalReasoningEffect.Minimal;
}