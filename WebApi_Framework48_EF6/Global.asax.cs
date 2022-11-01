using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Newtonsoft.Json;

namespace WebApi_Framework48_EF6
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            GlobalConfiguration
                .Configuration
                .Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
            
            GlobalConfiguration
                .Configuration
                .Formatters
                .JsonFormatter
                .SerializerSettings
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }
    }
}
