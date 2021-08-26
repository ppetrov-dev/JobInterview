using System.Threading;
using JobInterview.AssetExchanger.Concretes;

namespace JobInterview.AssetExchanger.Abstractions
{
    internal interface ISymbolsRequester
    {
        ITaskPromise<SymbolDto[]> GetSymbolsDtoAsync(CancellationToken cancellationToken);
    }
}