using System;
using System.Collections.Generic;
using JobInterview.AssetExchanger.Abstractions;

namespace JobInterview.AssetExchanger.Concretes
{
    internal class SymbolRepository : ISymbolRepository
    {
        public SymbolRepository()
        {
            Items = new ISymbol[]
            {
                new Symbol(new Asset("EUR"), new Asset("USD"), 1.2m)
            };
        }

        public bool IsReady { get; } = true;
        public event Action? IsReadyChanged;
        public IEnumerable<ISymbol> Items { get; }
    }
}