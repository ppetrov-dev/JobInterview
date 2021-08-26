namespace JobInterview.AssetExchanger.Abstractions
{
    internal interface IAssetRepository : IIsReadyProvider
    {
        bool Contains(long id);
        IAsset Resolve(long id);
    }
}