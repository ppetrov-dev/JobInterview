using System.Collections.Generic;

namespace JobInterview.AssetExchanger.Abstractions
{
    public interface IAssetRepository : IIsReadyProvider
    {
        bool Contains(long id);
        IAsset Resolve(long id);

        IEnumerable<IAsset> Items { get; }
    }
}