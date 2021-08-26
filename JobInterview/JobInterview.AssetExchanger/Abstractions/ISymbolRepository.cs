using System.Collections.Generic;

namespace JobInterview.AssetExchanger.Abstractions
{
    internal interface ISymbolRepository : IIsReadyProvider
    {
        IEnumerable<ISymbol> Items { get; }
    }
}