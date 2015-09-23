using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace Sample.Documents.Api
{
    /// <summary>
    /// Configures required Http settings to setup web api
    /// </summary>
    public static class HttpConfigurator
    {
        private static void ConfigureRoutes(HttpConfiguration configuration)
        {
            configuration.MapHttpAttributeRoutes();
        }

        private static void ConfigureServices(HttpConfiguration configuration, IHttpControllerActivator activator)
        {
            configuration.Services.Replace(typeof(IHttpControllerActivator), activator);
        }

        private static void ConfigureFormatting(HttpConfiguration configuration)
        {
            configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = 
                new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        }

        public static void Configure(HttpConfiguration configuration, IHttpControllerActivator activator)
        {
            ConfigureRoutes(configuration);
            ConfigureServices(configuration, activator);
            ConfigureFormatting(configuration);
        }
    }
}
