using System.Collections.Generic;
using FluentAssertions;
using JobInterview.AssetExchanger.Abstractions;
using JobInterview.TestUtils;
using Moq;
using Xunit;

namespace JobInterview.AssetExchanger.Examples.Tests
{
    public class AssetExchangerExampleTests
    {
        public AssetExchangerExampleTests()
        {
            _assetRepositoryMock = new Mock<IAssetRepository>();
            _assetRepositoryMock.Setup(r => r.IsReady).Returns(true);
            _assets = new List<IAsset>();
            _assetRepositoryMock.Setup(r => r.Items).Returns(_assets);
            _assetExchangerMock = new Mock<IAssetExchanger>();
            _assetExchangerMock.Setup(r => r.IsReady).Returns(true);
        }

        private readonly Mock<IAssetExchanger> _assetExchangerMock;
        private readonly Mock<IAssetRepository> _assetRepositoryMock;
        private readonly List<IAsset> _assets;

        private AssetExchangerExample CreateExample()
        {
            return new(
                _assetRepositoryMock.Object,
                _assetExchangerMock.Object);
        }

        private static IAsset CreateAsset()
        {
            return Mock.Of<IAsset>();
        }

        [Fact]
        public void CallsConvertInCtor()
        {
            const decimal amount = AssetExchangerExample.AmountToConvert;
            var asset1 = CreateAsset();
            var asset2 = CreateAsset();
            var asset3 = CreateAsset();
            _assets.AddRange(new[] { asset1, asset2, asset3 });
            CreateExample();

            _assetExchangerMock.Verify(exr => exr.Convert(amount, asset1, asset2), Times.Once);
            _assetExchangerMock.Verify(exr => exr.Convert(amount, asset1, asset3), Times.Once);
            _assetExchangerMock.Verify(exr => exr.Convert(amount, asset2, asset3), Times.Once);
        }

        [Fact]
        public void SetsIsCompletedTrueWhenReady()
        {
            var example = CreateExample();

            example.IsCompleted.Should().BeTrue();
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void DoesNotCallConvertInCtorWhenNotReady(bool isRepositoryReady, bool isExchangerReady)
        {
            _assetRepositoryMock.Setup(r => r.IsReady).Returns(isRepositoryReady);
            _assetExchangerMock.Setup(r => r.IsReady).Returns(isExchangerReady);
            _assets.AddRange(new[] { CreateAsset(), CreateAsset() });

            CreateExample();

            _assetExchangerMock.Verify(exr => exr.Convert(It.IsAny<decimal>(), It.IsAny<IAsset>(), It.IsAny<IAsset>()), Times.Never);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void DoesNotSetIsCompletedTrueWhenNotReady(bool isRepositoryReady, bool isExchangerReady)
        {
            _assetRepositoryMock.Setup(r => r.IsReady).Returns(isRepositoryReady);
            _assetExchangerMock.Setup(r => r.IsReady).Returns(isExchangerReady);

            var example = CreateExample();

            example.IsCompleted.Should().BeFalse();
        }

        [Fact]
        public void CallsConvertOnAssetRepositoryIsReadyChanged()
        {
            _assets.Add(CreateAsset());
            _assetRepositoryMock.Setup(r => r.IsReady).Returns(false);
            CreateExample();

            _assetRepositoryMock.Setup(r => r.IsReady).Returns(true);
            _assetRepositoryMock.Raise(r => r.IsReadyChanged += null);

            _assetExchangerMock.Verify(exr => exr.Convert(It.IsAny<decimal>(), It.IsAny<IAsset>(), It.IsAny<IAsset>()), Times.Once);
        }

        [Fact]
        public void CallsConvertOnAssetExchangerIsReadyChanged()
        {
            _assets.Add(CreateAsset());
            _assetExchangerMock.Setup(r => r.IsReady).Returns(false);
            CreateExample();

            _assetExchangerMock.Setup(r => r.IsReady).Returns(true);
            _assetExchangerMock.Raise(r => r.IsReadyChanged += null);

            _assetExchangerMock.Verify(exr => exr.Convert(It.IsAny<decimal>(), It.IsAny<IAsset>(), It.IsAny<IAsset>()), Times.Once);
        }

        [Fact]
        public void CollectedByGCAfterDispose()
        {
            GCAssert.AssertCollectedAfterDispose(
                CreateExample,
                () =>
                {
                    _assetRepositoryMock.Invocations.Clear();
                    _assetExchangerMock.Invocations.Clear();
                });
        }
    }
}