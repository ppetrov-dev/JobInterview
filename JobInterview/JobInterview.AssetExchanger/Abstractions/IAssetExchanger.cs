namespace JobInterview.AssetExchanger.Abstractions
{
    public interface IAssetExchanger : IIsReadyProvider
    {
        decimal? Convert(decimal amount, IAsset fromAsset, IAsset toAsset);
    }
}