using System;

namespace JobInterview.AssetExchanger
{
    internal interface IIsReadyProvider
    {
        bool IsReady { get; }
        event Action? IsReadyChanged;
    }
}