using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JobInterview.AssetExchanger.Abstractions;
using JobInterview.AssetExchanger.Extensions;

namespace JobInterview.AssetExchanger.Concretes
{
    internal class AssetRepository : IAssetRepository, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ITaskPromise<IAsset[]> _getAssetsTaskPromise;
        private Dictionary<long, IAsset> _assetsDictionary = new();

        public AssetRepository(IAssetsRequester assetsRequester, ICancellationTokenSourceFactory cancellationTokenSourceFactory)
        {
            _cancellationTokenSource = cancellationTokenSourceFactory.Create();
            _getAssetsTaskPromise = assetsRequester.GetAssetsAsync(_cancellationTokenSource.Token);

            _getAssetsTaskPromise.Succeeded += OnSucceeded;
        }

        public IEnumerable<IAsset> Items => _assetsDictionary.Values;

        public bool IsReady { get; private set; }

        public event Action? IsReadyChanged;

        public bool Contains(long id)
        {
            return _assetsDictionary.ContainsKey(id);
        }

        public IAsset Resolve(long id)
        {
            return _assetsDictionary[id];
        }

        private void OnSucceeded()
        {
            var assets = _getAssetsTaskPromise.Result!;
            _assetsDictionary = assets.ToDictionary(asset => asset.Id);
            IsReady = true;
            IsReadyChanged?.Invoke();
        }

        public void Dispose()
        {
            _getAssetsTaskPromise.Succeeded -= OnSucceeded;
            _cancellationTokenSource.CancelAndDispose();
        }
    }
}