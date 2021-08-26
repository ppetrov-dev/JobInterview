using JobInterview.AssetExchanger.Abstractions;

namespace JobInterview.AssetExchanger.Concretes
{
    internal class Asset : IAsset
    {
        public Asset(long id, string name)
        {
            Name = name;
            Id = id;
        }

        public long Id { get; }
        public string Name { get; }
    }
}