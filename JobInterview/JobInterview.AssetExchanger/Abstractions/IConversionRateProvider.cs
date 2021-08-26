namespace JobInterview.AssetExchanger.Abstractions
{
    internal interface IConversionRateProvider : IIsReadyProvider
    {
        public decimal? GetRate(IAsset fromAsset, IAsset toAsset);
    }
}