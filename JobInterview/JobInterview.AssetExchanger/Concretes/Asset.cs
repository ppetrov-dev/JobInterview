using JobInterview.AssetExchanger.Abstractions;

namespace JobInterview.AssetExchanger.Concretes
{
    internal class Asset : IAsset
    {
        public Asset(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}