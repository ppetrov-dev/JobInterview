using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using JobInterview.AssetExchanger.Abstractions;
using JobInterview.AssetExchanger.Concretes;
using JobInterview.TestUtils;
using Moq;
using Xunit;

namespace JobInterview.AssetExchanger.Tests.Concretes
{
    public class AssetRepositoryTests
    {
        public AssetRepositoryTests()
        {
            _cancellationTokenSourceFactoryMock = new Mock<ICancellationTokenSourceFactory>();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSourceFactoryMock.Setup(f => f.Create()).Returns(_cancellationTokenSource);
            _assetsRequesterMock = new Mock<IAssetsRequester>();
            _getAssetsTaskPromiseMock = new Mock<ITaskPromise<IAsset[]>>();
            _getAssetsTaskPromiseMock.Setup(promise => promise.Result).Returns(new IAsset[0]);
            _assetsRequesterMock.Setup(r => r.GetAssetsAsync(_cancellationTokenSource.Token))
                .Returns(_getAssetsTaskPromiseMock.Object);
        }

        private readonly Mock<IAssetsRequester> _assetsRequesterMock;
        private readonly Mock<ICancellationTokenSourceFactory> _cancellationTokenSourceFactoryMock;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Mock<ITaskPromise<IAsset[]>> _getAssetsTaskPromiseMock;

        private AssetRepository CreateRepository()
        {
            return new(
                _assetsRequesterMock.Object,
                _cancellationTokenSourceFactoryMock.Object);
        }

        private static IAsset CreateAsset(long id)
        {
            return Mock.Of<IAsset>(asset => asset.Id == id);
        }

        [Fact]
        public void CreatesCancellationTokenSourceInCtor()
        {
            CreateRepository();

            _cancellationTokenSourceFactoryMock.Verify(f => f.Create(), Times.Once);
        }

        [Fact]
        public void CallsGetAssetsAsyncInCtor()
        {
            CreateRepository();

            _assetsRequesterMock.Verify(r => r.GetAssetsAsync(_cancellationTokenSource.Token), Times.Once);
        }

        [Fact]
        public void SetsIsReadyToTrueOnGetAssetsCompleted()
        {
            var repository = CreateRepository();

            _getAssetsTaskPromiseMock.Raise(promise => promise.Succeeded += null);

            repository.IsReady.Should().BeTrue();
        }

        [Fact]
        public void RaisesIsReadyChangedOnGetAssetsCompleted()
        {
            var repository = CreateRepository();
            var monitor = repository.Monitor();

            _getAssetsTaskPromiseMock.Raise(promise => promise.Succeeded += null);

            monitor.Should().Raise(nameof(repository.IsReadyChanged));
        }

        [Fact]
        public void GetsContainsAsFalseWhenRepositoryIsNotReadyYet()
        {
            var repository = CreateRepository();

            var contains = repository.Contains(100500);

            contains.Should().BeFalse("task promise was not completed");
        }

        [Fact]
        public void GetsContainsAsTrueWhenRepositoryIsReady()
        {
            const long id = 100500;
            var repository = CreateRepository();
            _getAssetsTaskPromiseMock.Setup(promise => promise.Result)
                .Returns(new[] { CreateAsset(id) });

            _getAssetsTaskPromiseMock.Raise(promise => promise.Succeeded += null);

            var contains = repository.Contains(id);

            contains.Should().BeTrue();
        }

        [Fact]
        public void GetsContainsAsFalseWhenRepositoryIsReadyForUnknownId()
        {
            var repository = CreateRepository();
            _getAssetsTaskPromiseMock.Setup(promise => promise.Result)
                .Returns(new[] { CreateAsset(100500) });

            _getAssetsTaskPromiseMock.Raise(promise => promise.Succeeded += null);

            var contains = repository.Contains(555777);

            contains.Should().BeFalse();
        }

        [Fact]
        public void ResolvesAssetWhenRepositoryIsReady()
        {
            var expectedAsset = CreateAsset(100500);
            var repository = CreateRepository();
            _getAssetsTaskPromiseMock.Setup(promise => promise.Result)
                .Returns(new[] { expectedAsset });

            _getAssetsTaskPromiseMock.Raise(promise => promise.Succeeded += null);

            var actualAsset = repository.Resolve(expectedAsset.Id);

            actualAsset.Should().BeEquivalentTo(expectedAsset);
        }

        [Fact]
        public void ThrowsExceptionOnResolveWhenRepositoryIsNotReadyYet()
        {
            var repository = CreateRepository();

            repository.Invoking(r => r.Resolve(100500))
                .Should()
                .Throw<KeyNotFoundException>();
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
            GCAssert.AssertCollectedAfterDispose(CreateRepository, () => _getAssetsTaskPromiseMock.Invocations.Clear());
        }

        [Fact]
        public void SetsItemsOnGetAssetsTaskPromiseSucceeded()
        {
            var expectedAssets = new[] { CreateAsset(1), CreateAsset(2), CreateAsset(3) };
            var repository = CreateRepository();

            _getAssetsTaskPromiseMock.Setup(promise => promise.Result)
                .Returns(expectedAssets);

            _getAssetsTaskPromiseMock.Raise(promise => promise.Succeeded += null);

            repository.Items.Should().Equal(expectedAssets);
        }
    }
}