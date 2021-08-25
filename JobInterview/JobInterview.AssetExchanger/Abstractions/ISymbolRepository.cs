using System.Collections.Generic;

namespace JobInterview.AssetExchanger.Abstractions
{
    public interface ISymbolRepository : IIsReadyProvider
    {
        IEnumerable<ISymbol> Items { get; }
    }
}