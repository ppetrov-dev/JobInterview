namespace JobInterview.AssetExchanger.Concretes
{
    internal class SymbolDto
    {
        public SymbolDto(long baseAssetId, long quoteAssetId, decimal rate)
        {
            BaseAssetId = baseAssetId;
            QuoteAssetId = quoteAssetId;
            Rate = rate;
        }

        public long BaseAssetId { get; }
        public long QuoteAssetId { get; }
        public decimal Rate { get; }
    }
}