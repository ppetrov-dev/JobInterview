using System;
using Autofac;
using JobInterview.AssetExchanger.Abstractions;

namespace JobInterview.AssetExchanger.Examples
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using var bootstrapper = new Bootstrapper();

            bootstrapper.Run();
            if (bootstrapper.Container == null)
            {
                Console.WriteLine("＼_(ツ)_/");
                return;
            }

            var assetRepository = bootstrapper.Container.Resolve<IAssetRepository>();
            var assetExchanger = bootstrapper.Container.Resolve<IAssetExchanger>();
            using var example = new AssetExchangerExample(assetRepository, assetExchanger);

            while (!example.IsCompleted)
            {
            }

            Console.WriteLine("Done!");
        }
    }
}