using System.Web;
using Empathica.TwitterChallenge.Wires.Config;

namespace Empathica.TwitterChallenge.Wires
{
    /// <summary>
    /// HttpContext items abstraction for get/set Container
    /// </summary>
    public class DependencyStoreAdapter
    {
        private const string SetupKey = "_CONTAINER_";
        private readonly HttpContextBase _httpContext;

        public DependencyStoreAdapter(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        public Container Container
        {
            get { return (Container)_httpContext.Items[SetupKey]; }
            set { _httpContext.Items[SetupKey] = value; }
        }
    }
}