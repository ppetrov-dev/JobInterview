using FluentAssertions;
using JobInterview.AssetExchanger.Abstractions;
using Moq;
using Xunit;

namespace JobInterview.AssetExchanger.Tests.Concretes
{
    public class AssetExchangerTests
    {
        public AssetExchangerTests()
        {
            _conversionRateProviderMock = new Mock<IConversionRateProvider>();
            _conversionRateProviderMock.Setup(repository => repository.IsReady).Returns(true);
            _conversionRateProviderMock.Setup(repository => repository.GetRate(It.IsAny<IAsset>(), It.IsAny<IAsset>())).Returns(1.5m);
            _exchanger = CreateExchanger();
        }

        private readonly AssetExchanger.Concretes.AssetExchanger _exchanger;
        private readonly Mock<IConversionRateProvider> _conversionRateProviderMock;

        private AssetExchanger.Concretes.AssetExchanger CreateExchanger()
        {
            return new(_conversionRateProviderMock.Object);
        }

        private static IAsset CreateAsset(string name = "")
        {
            return Mock.Of<IAsset>(asset => asset.Name == name);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ExposesIsReady(bool conversionRateProviderIsReady)
        {
            _conversionRateProviderMock.Setup(repository => repository.IsReady).Returns(conversionRateProviderIsReady);

            var exchangerIsReady = _exchanger.IsReady;

            exchangerIsReady.Should().Be(conversionRateProviderIsReady);
        }

        [Fact]
        public void RaisesIsReadyChangedOnSymbolRepositoryIsReadyChanged()
        {
            using var monitor = _exchanger.Monitor();

            _conversionRateProviderMock.Raise(repository => repository.IsReadyChanged += null);

            monitor.Should().Raise(nameof(_exchanger.IsReadyChanged));
        }

        [Fact]
        public void ConvertsAsNullConversionRateProviderReturnsNull()
        {
            _conversionRateProviderMock.Setup(repository => repository.GetRate(It.IsAny<IAsset>(), It.IsAny<IAsset>()))
                .Returns<decimal?>(null);

            var result = _exchanger.Convert(100m, CreateAsset(), CreateAsset());

            result.Should().BeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(15.15)]
        [InlineData(100)]
        public void ConvertsUsingRateFromProvider(decimal rate)
        {
            var asset1 = CreateAsset();
            var asset2 = CreateAsset();
            _conversionRateProviderMock.Setup(repository => repository.GetRate(asset1, asset2))
                .Returns(rate);

            const decimal amount = 100m;
            var result = _exchanger.Convert(amount, asset1, asset2);

            var expected = amount * rate;
            result.Should().Be(expected);
        }
    }
}