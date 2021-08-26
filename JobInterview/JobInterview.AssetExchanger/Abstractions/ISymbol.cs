namespace JobInterview.AssetExchanger.Abstractions
{
    public interface ISymbol
    {
        IAsset BaseAsset { get; }
        IAsset QuoteAsset { get; }
        decimal Rate { get; }
    }
}