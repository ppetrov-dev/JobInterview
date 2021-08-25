using System;
using System.Linq;
using JobInterview.AssetExchanger.Abstractions;

namespace JobInterview.AssetExchanger.Concretes
{
    internal class ConversionRateProvider : IConversionRateProvider
    {
        private readonly ISymbolRepository _symbolRepository;

        public ConversionRateProvider(ISymbolRepository symbolRepository)
        {
            _symbolRepository = symbolRepository;
        }

        public bool IsReady => _symbolRepository.IsReady;

        public event Action? IsReadyChanged
        {
            add => _symbolRepository.IsReadyChanged += value;
            remove => _symbolRepository.IsReadyChanged -= value;
        }

        public decimal? GetRate(IAsset fromAsset, IAsset toAsset)
        {
            if (!_symbolRepository.Items.Any())
                return null;

            var directStrategySymbol = _symbolRepository.Items
                .FirstOrDefault(s => s.BaseAsset == fromAsset && s.QuoteAsset == toAsset);

            if (directStrategySymbol != null)
                return directStrategySymbol.Rate;

            var reverseStrategySymbol = _symbolRepository.Items
                .FirstOrDefault(s => s.BaseAsset == toAsset && s.QuoteAsset == fromAsset);

            if (reverseStrategySymbol == null || reverseStrategySymbol.Rate == 0)
                return null;

            return 1 / reverseStrategySymbol.Rate;
        }
    }
}