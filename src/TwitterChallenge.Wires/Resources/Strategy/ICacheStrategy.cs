namespace Empathica.TwitterChallenge.Wires.Resources.Strategy
{
    public interface ICacheStrategy
    {
        void Init();
        string this[string key] { get; }
    }
}