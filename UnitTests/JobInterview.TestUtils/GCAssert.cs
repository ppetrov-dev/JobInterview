using System;
using FluentAssertions;

namespace JobInterview.TestUtils
{
    public static class GCAssert
    {
        public static void AssertCollectedAfterDispose<T>(Func<T> objectFactory, Action afterDispose = null)
            where T : class, IDisposable
        {
            WeakReference weakReference = null;
            new Action(
                () =>
                {
                    var target = objectFactory();
                    weakReference = new WeakReference(target);
                    target.Dispose();
                    afterDispose?.Invoke();
                })();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            weakReference.IsAlive.Should().BeFalse($"{typeof(T).FullName} was not collected by GC. Check Dispose() method");
        }
    }
}