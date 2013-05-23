namespace Empathica.TwitterChallenge.Web.Extensions
{
    public interface ISessionAdapter
    {
        void SetValue(string key, object value);
        bool Contains(string key);
        T GetValueOrEmpty<T>(string key, T defaultValue = default(T));
        object GetValueOrEmpty(string key, object defaultValue = null);
        void Remove(string key);
        T PopValueOrEmpty<T>(string key, T defaultValue = default(T));
        void Clear();
    }
}