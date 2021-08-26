using System;
using System.Threading;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace JobInterview.TestUtils
{
    public class CancellationTokenSourceAssertions :
        ReferenceTypeAssertions<CancellationTokenSource, CancellationTokenSourceAssertions>
    {
        public CancellationTokenSourceAssertions(CancellationTokenSource instance)
            : base(instance)
        {
        }

        protected override string Identifier => nameof(CancellationTokenSourceAssertions);

        private bool WasDisposed()
        {
            try
            {
                Subject.Cancel();
                return false;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }
        }

        public AndConstraint<CancellationTokenSourceAssertions> BeCancelledAndDisposed(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given(() => Subject)
                .ForCondition(source => source.IsCancellationRequested)
                .FailWith("The cancellation token source was not cancelled")
                .Then
                .ForCondition(_ => WasDisposed())
                .FailWith("The cancellation token source was not disposed");

            return new AndConstraint<CancellationTokenSourceAssertions>(this);
        }
    }
}