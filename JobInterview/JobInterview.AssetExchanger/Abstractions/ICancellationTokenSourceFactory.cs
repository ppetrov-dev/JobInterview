using System.Threading;

namespace JobInterview.AssetExchanger.Abstractions
{
    public interface ICancellationTokenSourceFactory
    {
        CancellationTokenSource Create();
    }
}