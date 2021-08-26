using System;

namespace JobInterview.AssetExchanger.Abstractions
{
    public interface ITaskPromise<out TResult>
    {
        TResult? Result { get; }
        event Action? Succeeded;
    }
}