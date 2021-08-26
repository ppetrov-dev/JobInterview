using System.Threading;

namespace JobInterview.AssetExchanger.Extensions
{
    public static class CancellationTokenSourceExtensions
    {
        public static void CancelAndDispose(this CancellationTokenSource cancellationTokenSource)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
    }
}