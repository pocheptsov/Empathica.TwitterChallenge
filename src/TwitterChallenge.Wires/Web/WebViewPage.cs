using Empathica.TwitterChallenge.Wires.Config;
using Empathica.TwitterChallenge.Wires.Resources;

namespace Empathica.TwitterChallenge.Wires.Web
{
    public abstract class WebViewPage<T> : System.Web.Mvc.WebViewPage<T>
    {
        /// <summary>
        /// specialized property for accessing cached resources
        /// </summary>
        public IResourceCache R { get; private set; }

        public override void InitHelpers()
        {
            base.InitHelpers();

            R = Setup.CacheFactory(Html.ViewContext.HttpContext);
        }
    }

    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }
}