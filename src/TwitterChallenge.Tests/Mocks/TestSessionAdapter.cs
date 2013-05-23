using System.Collections.Generic;
using Empathica.TwitterChallenge.Web.Extensions;

namespace Empathica.TwitterChallenge.Tests.Mocks
{
    public class TestSessionAdapter : ISessionAdapter
    {
        private readonly Dictionary<string, object> _sessionStore = new Dictionary<string, object>();


        public void SetValue(string key, object value)
        {
            _sessionStore[key] = value;
        }

        public bool Contains(string key)
        {
            return _sessionStore[key] != null;
        }

        public T GetValueOrEmpty<T>(string key, T defaultValue = default(T))
        {
            object valueOrEmpty = GetValueOrEmpty(key, (object) defaultValue);

            return valueOrEmpty == null ? defaultValue : (T) valueOrEmpty;
        }

        public object GetValueOrEmpty(string key, object defaultValue = null)
        {
            object value;
            _sessionStore.TryGetValue(key, out value);
            return value;
        }

        public void Remove(string key)
        {
            _sessionStore.Remove(key);
        }

        public T PopValueOrEmpty<T>(string key, T defaultValue = default(T))
        {
            T value = GetValueOrEmpty(key, defaultValue);
            Remove(key);

            return value;
        }

        public void Clear()
        {
            _sessionStore.Clear();
        }
    }
}