using System;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Empathica.TwitterChallenge.Web.App_Start;
using Empathica.TwitterChallenge.Web.Controllers;
using Empathica.TwitterChallenge.Wires;
using Empathica.TwitterChallenge.Wires.Config;

namespace Empathica.TwitterChallenge.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public readonly ThreadLocal<DependencyStoreAdapter> AdapterInstance =
            new ThreadLocal<DependencyStoreAdapter>(
                () => new DependencyStoreAdapter(
                          new HttpContextWrapper(HttpContext.Current)));

        public MvcApplication()
        {
            BeginRequest += OnBeginRequest;
            EndRequest += OnEndRequest;
        }

        private void OnBeginRequest(object sender, EventArgs eventArgs)
        {
            var setup = new Setup();
            AdapterInstance.Value.Container = setup.Build();
        }

        private void OnEndRequest(object sender, EventArgs eventArgs)
        {
            try
            {
                AdapterInstance.Value.Container.Dispose();
            }
            catch (Exception)
            {
                //throw;
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public CustomErrorsSection GetCustomErrorSection()
        {
            var customErrorsSection =
                WebConfigurationManager.GetWebApplicationSection("system.web/customErrors") as CustomErrorsSection;
            return customErrorsSection ?? new CustomErrorsSection();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //skip error handling if it disabled in config
            if (GetCustomErrorSection().Mode == CustomErrorsMode.Off)
            {
                return;
            }

            HttpContext ctx = HttpContext.Current;
            //Exception ex = ctx.Server.GetLastError();
            ctx.Response.Clear();

            ControllerContext context;

            var controller = new TwitterController(); // here we can use any controller
            var handler = ctx.CurrentHandler as MvcHandler;
            if (handler == null)
            {
                //in case of FileNotFound or other handlers
                var routeData = new RouteData();
                routeData.Values.Add("controller", "twitter");
                context = new ControllerContext(new RequestContext(new HttpContextWrapper(Context), routeData), new TwitterController());
            }
            else
            {
                context  = new ControllerContext(handler.RequestContext, controller);
            }

            var viewResult = new ViewResult {ViewName = "Error"};

            viewResult.ExecuteResult(context);
            ctx.Server.ClearError();
        }
    }
}