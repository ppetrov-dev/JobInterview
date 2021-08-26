using System;
using System.Threading;
using System.Threading.Tasks;
using JobInterview.AssetExchanger.Abstractions;

namespace JobInterview.AssetExchanger.Concretes.Dummies
{
    internal class DummyAssetsRequester : IAssetsRequester
    {
        private static readonly IAsset[] Assets =
        {
            new Asset(1, "EUR"),
            new Asset(2, "USD"),
            new Asset(3, "RUB"),
            new Asset(4, "CHF"),
            new Asset(5, "JPY"),
            new Asset(6, "AUD"),
            new Asset(7, "CAD"),
            new Asset(8, "NZD"),
            new Asset(9, "SGD"),
            new Asset(10, "UAH"),
        };

        public static int AssetsCount => Assets.Length;

        public ITaskPromise<IAsset[]> GetAssetsAsync(CancellationToken cancellationToken)
        {
            return new TaskPromise<IAsset[]>(
                Task.Factory.StartNew(
                    () =>
                    {
                        Task.Delay(new Random().Next(1500), cancellationToken);
                        return Assets;
                    },
                    cancellationToken),
                cancellationToken);
        }
    }
}