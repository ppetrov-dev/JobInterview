using System;

namespace JobInterview.AssetExchanger.Abstractions
{
    public interface IIsReadyProvider
    {
        bool IsReady { get; }
        event Action? IsReadyChanged;
    }
}