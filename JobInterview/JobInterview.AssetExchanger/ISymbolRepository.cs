using System.Collections.Generic;

namespace JobInterview.AssetExchanger
{
    internal interface ISymbolRepository : IIsReadyProvider
    {
        IEnumerable<ISymbol> Items { get; }
    }
}