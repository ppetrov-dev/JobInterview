using System.Collections.Generic;
using FluentAssertions;
using JobInterview.AssetExchanger.Abstractions;
using JobInterview.AssetExchanger.Concretes;
using Moq;
using Xunit;

namespace JobInterview.AssetExchanger.Tests.Concretes
{
    public class ConversionRateProviderTests
    {
        public ConversionRateProviderTests()
        {
            _symbolRepositoryMock = new Mock<ISymbolRepository>();
            _symbols = new List<ISymbol>();
            _symbolRepositoryMock.Setup(repository => repository.Items).Returns(_symbols);
            _symbolRepositoryMock.Setup(repository => repository.IsReady).Returns(true);
            _provider = CreateProvider();
        }

        private readonly ConversionRateProvider _provider;
        private readonly Mock<ISymbolRepository> _symbolRepositoryMock;
        private readonly List<ISymbol> _symbols;

        private ConversionRateProvider CreateProvider()
        {
            return new(_symbolRepositoryMock.Object);
        }

        private static IAsset CreateAsset(string name = "")
        {
            return Mock.Of<IAsset>(asset => asset.Name == name);
        }

        private static ISymbol CreateSymbol(decimal rate = 0m, IAsset baseAsset = null, IAsset quoteAsset = null)
        {
            return Mock.Of<ISymbol>(symbol => symbol.Rate == rate && symbol.BaseAsset == baseAsset && symbol.QuoteAsset == quoteAsset);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ExposesIsReady(bool symbolRepositoryIsReady)
        {
            _symbolRepositoryMock.Setup(repository => repository.IsReady).Returns(symbolRepositoryIsReady);

            var providerIsReady = _provider.IsReady;

            providerIsReady.Should().Be(symbolRepositoryIsReady);
        }

        [Fact]
        public void RaisesIsReadyChangedOnSymbolRepositoryIsReadyChanged()
        {
            using var monitor = _provider.Monitor();

            _symbolRepositoryMock.Raise(repository => repository.IsReadyChanged += null);

            monitor.Should().Raise(nameof(_provider.IsReadyChanged));
        }

        [Fact]
        public void GetsRateAsNullWhenNoSymbols()
        {
            _symbols.Clear();

            var actualRate = _provider.GetRate(CreateAsset(), CreateAsset());

            actualRate.Should().BeNull($"No Symbols in {nameof(ISymbolRepository)}");
        }

        [Fact]
        public void GetsRateUsingRateWithDirectStrategy()
        {
            var asset1 = CreateAsset();
            var asset2 = CreateAsset();
            const decimal expectedRate = 0.5m;
            _symbols.Add(CreateSymbol(expectedRate, asset1, asset2));

            var actualRate = _provider.GetRate(asset1, asset2);

            actualRate.Should().Be(expectedRate);
        }

        [Fact]
        public void GetsRateUsingRateWithReverseStrategy()
        {
            var asset1 = CreateAsset();
            var asset2 = CreateAsset();
            const decimal rate = 0.5m;
            _symbols.Add(CreateSymbol(rate, asset1, asset2));

            var actualRate = _provider.GetRate(asset2, asset1);

            const decimal expectedRate = 1 / rate;
            actualRate.Should().Be(expectedRate);
        }

        [Fact]
        public void DoesNotThrowExceptionWhenSymbolNotFound()
        {
            _symbols.Add(CreateSymbol(10m, CreateAsset(), CreateAsset()));
            _symbols.Add(CreateSymbol(20m, CreateAsset(), CreateAsset()));

            _provider.Invoking(provider => provider.GetRate(CreateAsset(), CreateAsset()))
                .Should()
                .NotThrow("not an exceptional reason to break the app");
        }

        [Fact]
        public void ReturnsNullWhenSymbolRateIsZeroForReverseStrategy()
        {
            var asset1 = CreateAsset();
            var asset2 = CreateAsset();
            _symbols.Add(CreateSymbol(0, asset1, asset2));

            var actualRate = _provider.GetRate(asset2, asset1);

            actualRate.Should().BeNull("mathematics disallows division by zero because the resulting answer is indeterminate");
        }
    }
}