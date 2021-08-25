using Autofac;
using JobInterview.AssetExchanger.Abstractions;
using JobInterview.AssetExchanger.Concretes;

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
        }
    }
}