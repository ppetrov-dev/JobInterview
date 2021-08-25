namespace JobInterview.AssetExchanger
{
    internal interface IConversionRateProvider : IIsReadyProvider
    {
        public decimal? GetRate(IAsset fromAsset, IAsset toAsset);
    }
}