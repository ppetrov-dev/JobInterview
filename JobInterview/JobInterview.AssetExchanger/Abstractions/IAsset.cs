namespace JobInterview.AssetExchanger.Abstractions
{
    public interface IAsset
    {
        long Id { get; }
        string Name { get; }
    }
}