using JobInterview.AssetExchanger.Abstractions;

namespace JobInterview.AssetExchanger.Concretes
{
    internal class Symbol : ISymbol
    {
        public Symbol(IAsset baseAsset, IAsset quoteAsset, decimal rate)
        {
            BaseAsset = baseAsset;
            QuoteAsset = quoteAsset;
            Rate = rate;
        }

        public IAsset BaseAsset { get; }
        public IAsset QuoteAsset { get; }
        public decimal Rate { get; }
    }
}