using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sample.Documents.Api
{
    /// <summary>
    /// Handles startup of owin-hosted api
    /// </summary>
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            var compositon = new CompositionRoot();
            HttpConfigurator.Configure(config, compositon);
            app.UseWebApi(config);
        }
    }
}
