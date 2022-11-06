using System.Web;
using System.Web.Http;
using Newtonsoft.Json;

namespace WebApi_Framework48_EF6;

public class WebApiApplication : HttpApplication
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
