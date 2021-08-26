using System;

namespace JobInterview.AssetExchanger.Abstractions
{
    public interface ITaskPromise<out TResult>
    {
        event Action<TResult>? Succeeded;
    }
}