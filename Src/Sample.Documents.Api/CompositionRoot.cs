using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Sample.Documents.Api
{
    public class CompositionRoot : IHttpControllerActivator
    {
        public CompositionRoot()
        {
        }

        public IHttpController Create(
            HttpRequestMessage request, 
            HttpControllerDescriptor controllerDescriptor, 
            Type controllerType)
        {
            if(controllerType == typeof(HomeController))
            {
                return new HomeController();
            }

            throw new NotImplementedException(
                string.Format("controller of type {0} is not supported", controllerType.FullName));
        }
    }
}
