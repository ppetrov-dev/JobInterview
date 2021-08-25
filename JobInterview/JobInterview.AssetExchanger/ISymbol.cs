namespace JobInterview.AssetExchanger
{
    internal interface ISymbol
    {
        IAsset BaseAsset { get; }
        IAsset QuoteAsset { get; }
        decimal Rate { get; }
    }
}