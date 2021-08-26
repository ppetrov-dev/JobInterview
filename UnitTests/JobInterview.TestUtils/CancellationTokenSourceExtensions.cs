using System.Threading;

namespace JobInterview.TestUtils
{
    public static class CancellationTokenSourceExtensions
    {
        public static CancellationTokenSourceAssertions Should(this CancellationTokenSource cancellationTokenSource)
        {
            return new(cancellationTokenSource);
        }
    }
}