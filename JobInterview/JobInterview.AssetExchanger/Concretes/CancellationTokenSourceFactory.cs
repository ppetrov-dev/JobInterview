using System.Threading;
using JobInterview.AssetExchanger.Abstractions;

namespace JobInterview.AssetExchanger.Concretes
{
    internal class CancellationTokenSourceFactory : ICancellationTokenSourceFactory
    {
        public CancellationTokenSource Create()
        {
            return new ();
        }
    }
}