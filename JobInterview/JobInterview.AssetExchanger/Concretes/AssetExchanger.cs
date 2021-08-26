using System;
using JobInterview.AssetExchanger.Abstractions;

namespace JobInterview.AssetExchanger.Concretes
{
    internal class AssetExchanger : IAssetExchanger
    {
        private readonly IConversionRateProvider _conversionRateProvider;

        public AssetExchanger(IConversionRateProvider conversionRateProvider)
        {
            _conversionRateProvider = conversionRateProvider;
        }

        public decimal? Convert(decimal amount, IAsset fromAsset, IAsset toAsset)
        {
            var rate = _conversionRateProvider.GetRate(fromAsset, toAsset);

            return amount * rate;
        }

        public bool IsReady => _conversionRateProvider.IsReady;

        public event Action? IsReadyChanged
        {
            add => _conversionRateProvider.IsReadyChanged += value;
            remove => _conversionRateProvider.IsReadyChanged -= value;
        }
    }
}