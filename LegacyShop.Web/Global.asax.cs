using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;
using log4net.Config;

namespace LegacyShop.Web
{
    public class Global : HttpApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Global));

        protected void Application_Start(object sender, EventArgs e)
        {
            XmlConfigurator.Configure();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Log.Info("LegacyShop started");
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["VisitStart"] = DateTime.Now;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Log.Error("Unhandled exception", Server.GetLastError());
        }
    }
}
