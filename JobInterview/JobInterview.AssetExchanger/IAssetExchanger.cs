namespace JobInterview.AssetExchanger
{
    internal interface IAssetExchanger : IIsReadyProvider
    {
        decimal? Convert(decimal amount, IAsset fromAsset, IAsset toAsset);
    }
}