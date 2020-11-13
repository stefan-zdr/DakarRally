using DakarRally.Common;
using System;
using System.Web.Http;
using System.Web.Mvc;

namespace DakarRally.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AppSettings.SetDebug(true, AppDomain.CurrentDomain.BaseDirectory);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}
