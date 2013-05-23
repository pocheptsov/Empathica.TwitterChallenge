using System.Web;

namespace Empathica.TwitterChallenge.Web.Extensions
{
    public class SessionAdapter : ISessionAdapter
    {
        private readonly HttpContextBase _httpContext;

        public SessionAdapter(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        public void SetValue(string key, object value)
        {
            if (_httpContext.Session != null)
            {
                _httpContext.Session[key] = value;
            }
        }

        public bool Contains(string key)
        {
            if (_httpContext.Session != null)
            {
                return _httpContext.Session[key] != null;
            }
            return false;
        }

        public T GetValueOrEmpty<T>(string key, T defaultValue = default(T))
        {
            var valueOrEmpty = GetValueOrEmpty(key, (object)defaultValue);

            return valueOrEmpty == null ? defaultValue : (T)valueOrEmpty;
        }

        public object GetValueOrEmpty(string key, object defaultValue = null)
        {
            if (_httpContext.Session != null)
            {
                var value = _httpContext.Session[key];
                return value;
            }
            return null;
        }

        public void Remove(string key)
        {
            if (_httpContext.Session != null)
            {
                _httpContext.Session.Remove(key);
            }
        }

        public T PopValueOrEmpty<T>(string key, T defaultValue = default(T))
        {
            if (_httpContext.Session != null)
            {
                var value = GetValueOrEmpty(key, defaultValue);
                Remove(key);

                return value;
            }
            return defaultValue;
        }

        public void Clear()
        {
            if (_httpContext.Session != null)
            {
                _httpContext.Session.Clear();
            }
        }
    }
}
