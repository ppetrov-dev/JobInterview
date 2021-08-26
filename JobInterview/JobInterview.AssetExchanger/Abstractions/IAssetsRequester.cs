using System.Threading;

namespace JobInterview.AssetExchanger.Abstractions
{
    internal interface IAssetsRequester
    {
        ITaskPromise<IAsset[]> GetAssetsAsync(CancellationToken cancellationToken);
    }
}