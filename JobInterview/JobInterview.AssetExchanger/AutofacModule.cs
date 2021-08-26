using Autofac;
using JobInterview.AssetExchanger.Abstractions;
using JobInterview.AssetExchanger.Concretes;
using JobInterview.AssetExchanger.Concretes.Dummies;

namespace JobInterview.AssetExchanger
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<Concretes.AssetExchanger>().As<IAssetExchanger>().SingleInstance();
            builder.RegisterType<ConversionRateProvider>().As<IConversionRateProvider>().SingleInstance();
            builder.RegisterType<SymbolRepository>().As<ISymbolRepository>().SingleInstance();
            builder.RegisterType<AssetRepository>().As<IAssetRepository>().SingleInstance();
            builder.RegisterType<CancellationTokenSourceFactory>().As<ICancellationTokenSourceFactory>().SingleInstance();

            builder.RegisterType<DummyAssetsRequester>().As<IAssetsRequester>().SingleInstance();
            builder.RegisterType<DummySymbolsRequester>().As<ISymbolsRequester>().SingleInstance();
        }
    }
}