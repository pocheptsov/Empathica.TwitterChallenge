namespace Empathica.TwitterChallenge.Wires.Resources
{
    public interface IResourceCache
    {
        string this[string key] { get; }
    }
}