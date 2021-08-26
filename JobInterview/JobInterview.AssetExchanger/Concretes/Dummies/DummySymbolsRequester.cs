using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JobInterview.AssetExchanger.Abstractions;

namespace JobInterview.AssetExchanger.Concretes.Dummies
{
    internal class DummySymbolsRequester : ISymbolsRequester
    {
        public ITaskPromise<SymbolDto[]> GetSymbolsDtoAsync(CancellationToken cancellationToken)
        {
            return new TaskPromise<SymbolDto[]>(
                Task.Factory.StartNew(
                    () =>
                    {
                        Task.Delay(new Random().Next(3000), cancellationToken);
                        return GetSymbolsDto().ToArray();
                    },
                    cancellationToken),
                cancellationToken);
        }

        private static IEnumerable<SymbolDto> GetSymbolsDto()
        {
            for (var i = 1; i <= DummyAssetsRequester.AssetsCount; i++)
            for (var j = 1; j <= DummyAssetsRequester.AssetsCount; j++)
            {
                if (i == j)
                    continue;

                yield return new SymbolDto(i, j, 1.1m + j + i);
            }
        }
    }
}