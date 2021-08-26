using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JobInterview.AssetExchanger.Abstractions;
using JobInterview.AssetExchanger.Extensions;

namespace JobInterview.AssetExchanger.Concretes
{
    internal class SymbolRepository : ISymbolRepository, IDisposable
    {
        private readonly IAssetRepository _assetRepository;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ISymbolsRequester _symbolsRequester;
        private ITaskPromise<SymbolDto[]>? _getSymbolsDtoPromise;
        private bool _isReady;

        public SymbolRepository(
            IAssetRepository assetRepository,
            ISymbolsRequester symbolsRequester,
            ICancellationTokenSourceFactory cancellationTokenSourceFactory)
        {
            _assetRepository = assetRepository;
            _symbolsRequester = symbolsRequester;

            _cancellationTokenSource = cancellationTokenSourceFactory.Create();

            CallGetSymbolsDtoAsyncIfReady();

            _assetRepository.IsReadyChanged += OnAssetRepositoryIsReadyChanged;
        }

        public bool IsReady
        {
            get => _isReady;
            private set
            {
                if (_isReady == value)
                    return;

                _isReady = value;
                IsReadyChanged?.Invoke();
            }
        }

        public event Action? IsReadyChanged;

        public IEnumerable<ISymbol> Items { get; private set; } = Enumerable.Empty<ISymbol>();

        private void OnAssetRepositoryIsReadyChanged()
        {
            if (_assetRepository.IsReady)
            {
                CallGetSymbolsDtoAsync();
                return;
            }

            Items = Enumerable.Empty<ISymbol>();
            IsReady = false;
        }

        private void CallGetSymbolsDtoAsyncIfReady()
        {
            if (!_assetRepository.IsReady)
                return;

            CallGetSymbolsDtoAsync();
        }

        private void CallGetSymbolsDtoAsync()
        {
            UnsubscribeFromSucceededIfNotNull();
            _getSymbolsDtoPromise = _symbolsRequester.GetSymbolsDtoAsync(_cancellationTokenSource.Token);
            _getSymbolsDtoPromise.Succeeded += OnGetSymbolsDtoPromiseSucceeded;
        }

        private void OnGetSymbolsDtoPromiseSucceeded()
        {
            var symbols = _getSymbolsDtoPromise!.Result!;
            Items = symbols.Select(CreateSymbol).ToArray();
            IsReady = true;
        }

        private ISymbol CreateSymbol(SymbolDto dto)
        {
            var baseAsset = _assetRepository.Resolve(dto.BaseAssetId);
            var quoteAsset = _assetRepository.Resolve(dto.QuoteAssetId);
            return new Symbol(baseAsset, quoteAsset, dto.Rate);
        }

        private void UnsubscribeFromSucceededIfNotNull()
        {
            if (_getSymbolsDtoPromise != null)
                _getSymbolsDtoPromise.Succeeded -= OnGetSymbolsDtoPromiseSucceeded;
        }

        public void Dispose()
        {
            _assetRepository.IsReadyChanged -= OnAssetRepositoryIsReadyChanged;
            UnsubscribeFromSucceededIfNotNull();

            _cancellationTokenSource.CancelAndDispose();
        }
    }
}