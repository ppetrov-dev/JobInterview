using System;
using Autofac;

namespace JobInterview.AssetExchanger.Examples
{
    internal class Bootstrapper : IDisposable
    {
        public IContainer? Container { get; private set; }

        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<AutofacModule>();
        }

        public void Run()
        {
            var builder = new ContainerBuilder();

            RegisterModules(builder);

            Container = builder.Build();
        }

        public void Dispose()
        {
            Container?.Dispose();
        }
    }
}