using System;
using System.Linq;
using JobInterview.AssetExchanger.Abstractions;

namespace JobInterview.AssetExchanger.Examples
{
    internal class AssetExchangerExample : IDisposable
    {
        public const decimal AmountToConvert = 123.456m;

        private readonly IAssetExchanger _assetExchanger;
        private readonly IAssetRepository _assetRepository;

        public AssetExchangerExample(
            IAssetRepository assetRepository,
            IAssetExchanger assetExchanger)
        {
            _assetRepository = assetRepository;
            _assetExchanger = assetExchanger;

            WriteIsReadyState();
            RecalculateIfReady();

            _assetExchanger.IsReadyChanged += OnIsReadyChanged;
            _assetRepository.IsReadyChanged += OnIsReadyChanged;
        }

        public bool IsCompleted { get; private set; }

        private void RecalculateIfReady()
        {
            if (!_assetRepository.IsReady
                || !_assetExchanger.IsReady)
                return;

            var assets = _assetRepository.Items.ToArray();

            foreach (var fromAsset in assets.Reverse())
            foreach (var toAsset in assets.Reverse())
            {
                var result = _assetExchanger.Convert(AmountToConvert, fromAsset, toAsset);
                Console.WriteLine(
                    "Convert {0} {1:n3} to {2} (rate is {3:n2})= {2} {4:n3}",
                    fromAsset.Name,
                    AmountToConvert,
                    toAsset.Name,
                    result / AmountToConvert,
                    result);
            }

            IsCompleted = true;
        }

        private void OnIsReadyChanged()
        {
            WriteIsReadyState();
            RecalculateIfReady();
        }

        private void WriteIsReadyState()
        {
            Console.WriteLine($"{_assetRepository.GetType().Name}.IsReady: {_assetRepository.IsReady}");
            Console.WriteLine($"{_assetExchanger.GetType().Name}.IsReady: {_assetExchanger.IsReady}");
        }

        public void Dispose()
        {
            _assetExchanger.IsReadyChanged -= OnIsReadyChanged;
            _assetRepository.IsReadyChanged -= OnIsReadyChanged;
        }
    }
}