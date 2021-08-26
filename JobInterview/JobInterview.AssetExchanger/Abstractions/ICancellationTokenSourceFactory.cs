using System.Threading;

namespace JobInterview.AssetExchanger.Abstractions
{
    internal interface ICancellationTokenSourceFactory
    {
        CancellationTokenSource Create();
    }
}