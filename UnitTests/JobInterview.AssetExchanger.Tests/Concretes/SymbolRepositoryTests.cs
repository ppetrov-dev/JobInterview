using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using JobInterview.AssetExchanger.Abstractions;
using JobInterview.AssetExchanger.Concretes;
using JobInterview.TestUtils;
using Moq;
using Xunit;

namespace JobInterview.AssetExchanger.Tests.Concretes
{
    public class SymbolRepositoryTests
    {
        public SymbolRepositoryTests()
        {
            _cancellationTokenSourceFactoryMock = new Mock<ICancellationTokenSourceFactory>();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSourceFactoryMock.Setup(factory => factory.Create())
                .Returns(_cancellationTokenSource);

            _assetRepositoryMock = new Mock<IAssetRepository>();
            _assetRepositoryMock.Setup(r => r.IsReady).Returns(true);
            _assetRepositoryMock.Setup(r => r.Contains(It.IsAny<long>()))
                .Returns(true);

            _assetRepositoryMock.Setup(r => r.Resolve(It.IsAny<long>()))
                .Returns<long>(id => Mock.Of<IAsset>(a => a.Id == id));

            _symbolsRequesterMock = new Mock<ISymbolsRequester>();
            _getSymbolsDtoPromiseMock = new Mock<ITaskPromise<SymbolDto[]>>();
            _getSymbolsDtoPromiseMock.Setup(promise => promise.Result)
                .Returns(new SymbolDto[0]);

            _symbolsRequesterMock.Setup(requester => requester.GetSymbolsDtoAsync(_cancellationTokenSource.Token))
                .Returns(_getSymbolsDtoPromiseMock.Object);
        }

        private readonly Mock<IAssetRepository> _assetRepositoryMock;
        private readonly Mock<ISymbolsRequester> _symbolsRequesterMock;
        private readonly Mock<ICancellationTokenSourceFactory> _cancellationTokenSourceFactoryMock;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Mock<ITaskPromise<SymbolDto[]>> _getSymbolsDtoPromiseMock;

        private SymbolRepository CreateRepository()
        {
            return new(
                _assetRepositoryMock.Object,
                _symbolsRequesterMock.Object,
                _cancellationTokenSourceFactoryMock.Object);
        }

        [Fact]
        public void SetsIsReadyToTrueOnGetSymbolsDtoSucceeded()
        {
            var repository = CreateRepository();

            _getSymbolsDtoPromiseMock.Raise(promise => promise.Succeeded += null);

            repository.IsReady.Should().BeTrue();
        }

        [Fact]
        public void RaisesIsReadyChangedOnGetSymbolsDtoSucceeded()
        {
            var repository = CreateRepository();
            using var monitor = repository.Monitor();

            _getSymbolsDtoPromiseMock.Raise(promise => promise.Succeeded += null);

            monitor.Should().Raise(nameof(repository.IsReadyChanged));
        }

        [Fact]
        public void CreatesCancellationTokenSourceInCtor()
        {
            CreateRepository();

            _cancellationTokenSourceFactoryMock.Verify(factory => factory.Create(), Times.Once);
        }

        [Fact]
        public void CallsGetSymbolsDtoAsyncIfAssetRepositoryIsReady()
        {
            _assetRepositoryMock.Setup(r => r.IsReady).Returns(true);

            CreateRepository();

            _symbolsRequesterMock.Verify(requester => requester.GetSymbolsDtoAsync(_cancellationTokenSource.Token), Times.Once);
        }

        [Fact]
        public void DoesNotCallGetSymbolsDtoAsyncIfAssetRepositoryIsNotReady()
        {
            _assetRepositoryMock.Setup(r => r.IsReady).Returns(false);

            CreateRepository();

            _symbolsRequesterMock.Verify(requester => requester.GetSymbolsDtoAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void CallsGetSymbolsDtoAsyncOnIsReadyChanged()
        {
            _assetRepositoryMock.Setup(r => r.IsReady).Returns(false);
            CreateRepository();

            _assetRepositoryMock.Setup(r => r.IsReady).Returns(true);
            _assetRepositoryMock.Raise(r => r.IsReadyChanged += null);

            _symbolsRequesterMock.Verify(requester => requester.GetSymbolsDtoAsync(_cancellationTokenSource.Token), Times.Once);
        }

        [Fact]
        public void CancelsAndDisposesTokenSourceOnDispose()
        {
            CreateRepository().Dispose();

            _cancellationTokenSource.Should().BeCancelledAndDisposed();
        }

        [Fact]
        public void CollectedByGCAfterDispose()
        {
            GCAssert.AssertCollectedAfterDispose(
                () =>
                {
                    var repository = CreateRepository();
                    _assetRepositoryMock.Raise(r => r.IsReadyChanged += null);
                    _assetRepositoryMock.Raise(r => r.IsReadyChanged += null);
                    return repository;
                },
                () =>
                {
                    _assetRepositoryMock.Invocations.Clear();
                    _getSymbolsDtoPromiseMock.Invocations.Clear();
                });
        }

        [Fact]
        public void SetsIsReadyToTrueOnGetSymbolsDtoPromiseSucceeded()
        {
            var repository = CreateRepository();

            _getSymbolsDtoPromiseMock.Raise(p => p.Succeeded += null);

            repository.IsReady.Should().BeTrue();
        }

        [Fact]
        public void RaisesIsReadyChangedOnGetSymbolsDtoPromiseSucceeded()
        {
            var repository = CreateRepository();
            using var monitor = repository.Monitor();

            _getSymbolsDtoPromiseMock.Raise(p => p.Succeeded += null);

            monitor.Should().Raise(nameof(repository.IsReadyChanged));
        }

        [Fact]
        public void InitializesItemsOnGetSymbolsDtoPromiseSucceeded()
        {
            var repository = CreateRepository();
            var symbolsDto = new[] { new SymbolDto(1, 2, 3.4m), new SymbolDto(3, 4, 5.6m) };
            IEnumerable<ISymbol> expectedSymbols = symbolsDto.Select(
                    dto =>
                    {
                        var baseAsset = _assetRepositoryMock.Object.Resolve(dto.BaseAssetId);
                        var quoteAsset = _assetRepositoryMock.Object.Resolve(dto.QuoteAssetId);
                        return new Symbol(baseAsset, quoteAsset, dto.Rate);
                    })
                .ToArray();

            _getSymbolsDtoPromiseMock.Setup(promise => promise.Result).Returns(symbolsDto);
            _getSymbolsDtoPromiseMock.Raise(p => p.Succeeded += null);

            repository.Items.Should().BeEquivalentTo(expectedSymbols);
        }

        [Fact]
        public void SetsIsReadyToFalseOnAssetRepositoryBecameNotReady()
        {
            var repository = CreateRepository();
            _getSymbolsDtoPromiseMock.Raise(p => p.Succeeded += null);

            _assetRepositoryMock.Setup(r => r.IsReady).Returns(false);
            _assetRepositoryMock.Raise(r => r.IsReadyChanged += null);

            repository.IsReady.Should().BeFalse();
        }

        [Fact]
        public void RaisesIsReadyChangedOnAssetRepositoryBecameNotReady()
        {
            var repository = CreateRepository();
            _getSymbolsDtoPromiseMock.Raise(p => p.Succeeded += null);
            using var monitor = repository.Monitor();

            _assetRepositoryMock.Setup(r => r.IsReady).Returns(false);
            _assetRepositoryMock.Raise(r => r.IsReadyChanged += null);

            monitor.Should().Raise(nameof(repository.IsReadyChanged));
        }

        [Fact]
        public void ClearsItemsOnAssetRepositoryBecameNotReady()
        {
            var repository = CreateRepository();
            _getSymbolsDtoPromiseMock.Setup(promise => promise.Result).Returns(new[] { new SymbolDto(1, 2, 3.4m) });
            _getSymbolsDtoPromiseMock.Raise(p => p.Succeeded += null);

            _assetRepositoryMock.Setup(r => r.IsReady).Returns(false);
            _assetRepositoryMock.Raise(r => r.IsReadyChanged += null);

            repository.Items.Should().BeEmpty();
        }
    }
}